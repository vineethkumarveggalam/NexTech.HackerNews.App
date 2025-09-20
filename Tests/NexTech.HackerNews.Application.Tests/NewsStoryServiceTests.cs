using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Nextech.HackerNews.Application.Entities;
using Nextech.HackerNews.Application.Interfaces;
using Nextech.HackerNews.Application.Services;

namespace Nextech.HackerNews.Application.Tests.Services
{
    public class NewsStoryServiceTests
    {
        private readonly Mock<IHackerNewsApiClient> _apiClientMock;
        private readonly Mock<ICache> _cacheMock;
        private readonly Mock<ILogger<NewsStoryService>> _loggerMock;
        private readonly IConfiguration _configuration;
        private readonly NewsStoryService _service;

        public NewsStoryServiceTests()
        {
            _apiClientMock = new Mock<IHackerNewsApiClient>();
            _cacheMock = new Mock<ICache>();
            _loggerMock = new Mock<ILogger<NewsStoryService>>();

            var inMemorySettings = new Dictionary<string, string>
            {
                { "CacheSettings:TopStoriesCacheKey", "TopStories" },
                { "CacheSettings:TopStoriesCacheDurationMinutes", "5" },
                { "HackerNews:MaxNewsItems", "200" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _service = new NewsStoryService(
                _apiClientMock.Object,
                _cacheMock.Object,
                _configuration,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetNewestNewStoriesAsync_ReturnsCachedStories_IfAvailable()
        {
            // Arrange
            var cachedStories = new List<HackerNewsStory>
            {
                new() { Id = 1, Title = "Story 1", Url = "http://example.com/1" },
                new() { Id = 2, Title = "Story 2", Url = "http://example.com/2" }
            };

            _cacheMock.Setup(c => c.GetAsync<List<HackerNewsStory>>("TopStories", It.IsAny<CancellationToken>()))
                      .ReturnsAsync(cachedStories);

            // Act
            var result = await _service.GetNewestNewStoriesAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Stories);
            Assert.Equal("Story 1", result.Stories[0].Title);
            _apiClientMock.Verify(a => a.GetTopStoriesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetNewestNewStoriesAsync_CallsApiAndCaches_WhenCacheIsEmpty()
        {
            // Arrange
            _cacheMock.Setup(c => c.GetAsync<List<HackerNewsStory>>("TopStories", It.IsAny<CancellationToken>()))
                      .ReturnsAsync((List<HackerNewsStory>?)null);

            var topIds = new List<int> { 1, 2 };
            _apiClientMock.Setup(a => a.GetTopStoriesAsync(It.IsAny<CancellationToken>()))
                          .ReturnsAsync(topIds);

            _apiClientMock.Setup(a => a.GetStoryByIdAsync(1, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new HackerNewsStory { Id = 1, Title = "Story 1", Url = "http://example.com/1" });

            _apiClientMock.Setup(a => a.GetStoryByIdAsync(2, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new HackerNewsStory { Id = 2, Title = "Story 2", Url = "http://example.com/2" });

            // Act
            var result = await _service.GetNewestNewStoriesAsync(2);

            // Assert
            Assert.Equal(2, result.Stories.Count);
            _cacheMock.Verify(c => c.SetAsync(
                "TopStories",
                It.IsAny<List<HackerNewsStory>>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetNewestNewStoriesAsync_ReturnsEmptyList_IfApiReturnsNull()
        {
            // Arrange
            _cacheMock.Setup(c => c.GetAsync<List<HackerNewsStory>>("TopStories", It.IsAny<CancellationToken>()))
                      .ReturnsAsync((List<HackerNewsStory>?)null);

            _apiClientMock.Setup(a => a.GetTopStoriesAsync(It.IsAny<CancellationToken>()))
                          .ReturnsAsync((List<int>?)null);

            // Act
            var result = await _service.GetNewestNewStoriesAsync(5);

            // Assert
            Assert.Empty(result.Stories);
        }

        [Fact]
        public async Task SearchStoriesAsync_FiltersAndPaginates_Correctly()
        {
            // Arrange
            var allStories = new List<HackerNewsStory>
    {
        new() { Id = 1, Title = "C# News", Url = "http://example.com/1" },
        new() { Id = 2, Title = "Python News", Url = "http://example.com/2" },
        new() { Id = 3, Title = "C# Advanced", Url = "http://example.com/3" },
        new() { Id = 4, Title = "GoLang Tips", Url = "http://example.com/4" },
    };

            var topStoryIds = allStories.Select(s => s.Id).ToList();

            // No cache
            _cacheMock.Setup(c => c.GetAsync<List<HackerNewsStory>>("TopStories", It.IsAny<CancellationToken>()))
                      .ReturnsAsync((List<HackerNewsStory>?)null);

            _apiClientMock.Setup(a => a.GetTopStoriesAsync(It.IsAny<CancellationToken>()))
                          .ReturnsAsync(topStoryIds);

            foreach (var story in allStories)
            {
                _apiClientMock.Setup(a => a.GetStoryByIdAsync(story.Id, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(story);
            }

            // Act
            var result = await _service.SearchStoriesAsync("C#", pageNumber: 1, pageSize: 2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Stories.Count);
            Assert.All(result.Stories, s => Assert.Contains("C#", s.Title, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task SearchStoriesAsync_ReturnsEmptyList_OnException()
        {
            // Arrange
            _cacheMock.Setup(c => c.GetAsync<List<HackerNewsStory>>("TopStories", It.IsAny<CancellationToken>()))
                      .ReturnsAsync((List<HackerNewsStory>?)null);

            _apiClientMock.Setup(a => a.GetTopStoriesAsync(It.IsAny<CancellationToken>()))
                          .ThrowsAsync(new Exception("Simulated API failure"));

            // Act
            var result = await _service.SearchStoriesAsync("C#");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Stories);          
        }

    }
}