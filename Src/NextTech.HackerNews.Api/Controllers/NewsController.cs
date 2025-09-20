namespace NexTech.HackerNews.Api.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Nextech.HackerNews.Application.Interfaces;
    using NexTech.HackerNews.Api.Models;

    /// <summary>
    /// Defines the <see cref="NewsController" />
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController(INewsStoryService newsStoryService, IMapper mapper, ILogger<NewsController> logger) : ControllerBase
    {
        /// <summary>
        /// The Get
        /// </summary>
        /// <param name="page">The page<see cref="int"/></param>
        /// <param name="pageSize">The pageSize<see cref="int"/></param>
        /// <returns>The <see cref="Task{ActionResult{List{StoryResponse}}}"/></returns>
        [HttpGet]
        public async Task<ActionResult<PagedStories>> Get(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var storyList = await newsStoryService.GetNewestNewStoriesAsync(page * pageSize, CancellationToken.None);
                // Check if storyList is null or empty
                if (storyList == null || storyList.TotalCount == 0)
                {
                    return NotFound("No stories were found");
                }
                var pagedStories = storyList.Stories.Skip((page - 1) * pageSize).Take(pageSize);
                var storyResponses = mapper.Map<List<StoryResponse>>(pagedStories);

                return Ok(new PagedStories
                {
                    Stories = storyResponses,
                    Count = storyList.TotalCount
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while fetching stories from api");
                return StatusCode(500, "An unexpected error occurred while processing your request.");
            }
        }

        /// <summary>
        /// The Search
        /// </summary>
        /// <param name="query">The query<see cref="string"/></param>
        /// <param name="page">The page<see cref="int"/></param>
        /// <param name="pageSize">The pageSize<see cref="int"/></param>
        /// <returns>The <see cref="Task{ActionResult{PagedStories}}"/></returns>
        [HttpGet("search")]
        public async Task<ActionResult<PagedStories>> Search(
             [FromQuery] string query,
             [FromQuery] int page = 1,
             [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest("Query parameter cannot be empty.");
                }

                var storyList = await newsStoryService.SearchStoriesAsync(query, page, pageSize);

                // Check if storyList is null or empty
                if (storyList == null || storyList.TotalCount == 0)
                {
                    return NotFound("No stories found matching the search criteria.");
                }

                var storyResponses = mapper.Map<List<StoryResponse>>(storyList.Stories);

                return Ok(new PagedStories
                {
                    Stories = storyResponses,
                    Count = storyList.TotalCount
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while searching for stories.");
                return StatusCode(500, "An unexpected error occurred while processing your request.");
            }
        }
    }
}