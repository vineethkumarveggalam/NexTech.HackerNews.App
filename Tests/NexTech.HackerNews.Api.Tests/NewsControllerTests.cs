using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Nextech.HackerNews.Application.Entities;
using Nextech.HackerNews.Application.Interfaces;
using NexTech.HackerNews.Api.Controllers;
using NexTech.HackerNews.Api.Models;

public class NewsControllerTests
{
    private readonly Mock<INewsStoryService> _newsStoryServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<NewsController>> _loggerMock;
    private readonly NewsController _controller;

    public NewsControllerTests()
    {
        _newsStoryServiceMock = new Mock<INewsStoryService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<NewsController>>();

        _controller = new NewsController(
            _newsStoryServiceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Get_ReturnsPagedStories_WhenStoriesExist()
    {
        // Arrange
        var stories = Enumerable.Range(1, 20).Select(i =>
            new HackerNewsStory { Id = i, Title = $"Title {i}", Url = $"http://url/{i}" }).ToList();
        var storyList = new StoryList { Stories = stories, TotalCount = stories.Count };

        _newsStoryServiceMock.Setup(s => s.GetNewestNewStoriesAsync(It.IsAny<int>(), default))
     .ReturnsAsync(storyList);

        var mapped = stories.Take(10).Select(s => new StoryResponse { Title = s.Title, Url = s.Url }).ToList();

        _mapperMock.Setup(m => m.Map<List<StoryResponse>>(It.IsAny<IEnumerable<HackerNewsStory>>()))
            .Returns(mapped);

        // Act
        var result = await _controller.Get(page: 1, pageSize: 10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var paged = Assert.IsType<PagedStories>(okResult.Value);
        Assert.Equal(20, paged.Count);
        Assert.Equal(10, paged.Stories.Count);
        Assert.All(paged.Stories, s => Assert.Contains("Title", s.Title));
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenNoStories()
    {
        // Arrange
        _newsStoryServiceMock.Setup(s => s.GetNewestNewStoriesAsync(It.IsAny<int>(), default))
            .ReturnsAsync(new StoryList { Stories = new List<HackerNewsStory>(), TotalCount = 0 });

        // Act
        var result = await _controller.Get();

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("No stories were found", notFound.Value);
    }

    [Fact]
    public async Task Get_ReturnsServerError_OnException()
    {
        // Arrange
        _newsStoryServiceMock.Setup(s => s.GetNewestNewStoriesAsync(It.IsAny<int>(), default))
            .ThrowsAsync(new Exception("Simulated failure"));

        // Act
        var result = await _controller.Get();

        // Assert
        var statusCode = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCode.StatusCode);
        Assert.Equal("An unexpected error occurred while processing your request.", statusCode.Value);       
    }

    [Fact]
    public async Task Search_ReturnsPagedStories_WhenMatchingStoriesExist()
    {
        // Arrange
        var stories = Enumerable.Range(1, 5).Select(i =>
            new HackerNewsStory { Id = i, Title = $"SearchTitle {i}", Url = $"http://url/{i}" }).ToList();
        var storyList = new StoryList { Stories = stories, TotalCount = stories.Count };

        _newsStoryServiceMock.Setup(s => s.SearchStoriesAsync("query", 1, 10, default))
            .ReturnsAsync(storyList);

        var mapped = stories.Select(s => new StoryResponse { Title = s.Title, Url = s.Url }).ToList();

        _mapperMock.Setup(m => m.Map<List<StoryResponse>>(It.IsAny<IEnumerable<HackerNewsStory>>()))
            .Returns(mapped);

        // Act
        var result = await _controller.Search("query", 1, 10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var paged = Assert.IsType<PagedStories>(okResult.Value);
        Assert.Equal(5, paged.Count);
        Assert.Equal(5, paged.Stories.Count);
        Assert.All(paged.Stories, s => Assert.Contains("SearchTitle", s.Title));
    }

    [Fact]
    public async Task Search_ReturnsBadRequest_WhenQueryIsEmpty()
    {
        // Act
        var result = await _controller.Search("", 1, 10);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Query parameter cannot be empty.", badRequest.Value);
    }

    [Fact]
    public async Task Search_ReturnsNotFound_WhenNoMatchingStories()
    {
        // Arrange
        _newsStoryServiceMock.Setup(s => s.SearchStoriesAsync("missing", 1, 10, default))
            .ReturnsAsync(new StoryList { Stories = new List<HackerNewsStory>(), TotalCount = 0 });

        // Act
        var result = await _controller.Search("missing", 1, 10);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("No stories found matching the search criteria.", notFound.Value);
    }

    [Fact]
    public async Task Search_ReturnsServerError_OnException()
    {
        // Arrange
        _newsStoryServiceMock.Setup(s => s.SearchStoriesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), default))
            .ThrowsAsync(new Exception("Simulated failure"));

        // Act
        var result = await _controller.Search("query", 1, 10);

        // Assert
        var statusCode = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCode.StatusCode);
        Assert.Equal("An unexpected error occurred while processing your request.", statusCode.Value);       
    }

    [Fact]
    public async Task Get_PagesResultsCorrectly()
    {
        // Arrange
        var stories = Enumerable.Range(1, 30).Select(i =>
            new HackerNewsStory { Id = i, Title = $"Title {i}", Url = $"http://url/{i}" }).ToList();
        var storyList = new StoryList { Stories = stories, TotalCount = stories.Count };

        _newsStoryServiceMock.Setup(s => s.GetNewestNewStoriesAsync(It.IsAny<int>(), default))
    .ReturnsAsync(storyList);

        var mapped = stories.Skip(10).Take(10).Select(s => new StoryResponse { Title = s.Title, Url = s.Url }).ToList();

        _mapperMock.Setup(m => m.Map<List<StoryResponse>>(It.IsAny<IEnumerable<HackerNewsStory>>()))
            .Returns(mapped);

        // Act
        var result = await _controller.Get(page: 2, pageSize: 10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var paged = Assert.IsType<PagedStories>(okResult.Value);
        Assert.Equal(30, paged.Count);
        Assert.Equal(10, paged.Stories.Count);
        Assert.All(paged.Stories, s => Assert.Contains("Title", s.Title));
        Assert.Equal("Title 11", paged.Stories.First().Title);
    }

    [Fact]
    public async Task Search_PagesResultsCorrectly()
    {
        // Arrange
        var stories = Enumerable.Range(1, 25).Select(i =>
            new HackerNewsStory { Id = i, Title = $"SearchTitle {i}", Url = $"http://url/{i}" }).ToList();
        var storyList = new StoryList { Stories = stories.Skip(10).Take(10).ToList(), TotalCount = stories.Count };

        _newsStoryServiceMock.Setup(s => s.SearchStoriesAsync("query", 2, 10, default))
            .ReturnsAsync(storyList);

        var mapped = storyList.Stories.Select(s => new StoryResponse { Title = s.Title, Url = s.Url }).ToList();

        _mapperMock.Setup(m => m.Map<List<StoryResponse>>(It.IsAny<IEnumerable<HackerNewsStory>>()))
            .Returns(mapped);

        // Act
        var result = await _controller.Search("query", 2, 10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var paged = Assert.IsType<PagedStories>(okResult.Value);
        Assert.Equal(25, paged.Count);
        Assert.Equal(10, paged.Stories.Count);
        Assert.All(paged.Stories, s => Assert.Contains("SearchTitle", s.Title));
        Assert.Equal("SearchTitle 11", paged.Stories.First().Title);
    }
}
