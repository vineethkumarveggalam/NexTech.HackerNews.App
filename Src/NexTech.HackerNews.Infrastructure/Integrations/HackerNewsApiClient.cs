namespace Nextech.HackerNews.Infrastructure.Integrations
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Nextech.HackerNews.Application.Entities;
    using Nextech.HackerNews.Application.Interfaces;
    using NexTech.HackerNews.Infrastructure.Integrations;
    using NexTech.HackerNews.Infrastructure.Models;

    /// <summary>
    /// Defines the <see cref="HackerNewsApiClient" />
    /// </summary>
    public class HackerNewsApiClient : IHackerNewsApiClient
    {
        /// <summary>
        /// Defines the _httpClient
        /// </summary>
        private readonly HttpClientWrapper _httpClient;

        /// <summary>
        /// Defines the _logger
        /// </summary>
        private readonly ILogger<HackerNewsApiClient> _logger;

        /// <summary>
        /// Defines the _options
        /// </summary>
        private readonly HackerNewsOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="HackerNewsApiClient"/> class.
        /// </summary>
        /// <param name="httpClient">The httpClient<see cref="HttpClientWrapper"/></param>
        /// <param name="logger">The logger<see cref="ILogger{HackerNewsApiClient}"/></param>
        /// <param name="options">The options<see cref="IOptions{HackerNewsOptions}"/></param>
        public HackerNewsApiClient(HttpClientWrapper httpClient, ILogger<HackerNewsApiClient> logger, IOptions<HackerNewsOptions> options)
        {
            _httpClient = httpClient;
            _logger = logger;
            _options = options.Value;
        }

        /// <summary>
        /// The GetTopStoriesAsync
        /// </summary>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task{List{int}}"/></returns>
        public async Task<List<int>> GetTopStoriesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var ids = await _httpClient.GetAsync<List<int>>(_options.Endpoints.TopStories, cancellationToken);
                _logger.LogInformation("Successfully retrieved {Count} story IDs", ids?.Count ?? 0);
                return ids ?? new List<int>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch top stories from HackerNews API");
                throw;
            }
        }

        /// <summary>
        /// The GetStoryByIdAsync
        /// </summary>
        /// <param name="id">The id<see cref="int"/></param>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task{HackerNewsStory?}"/></returns>
        public async Task<HackerNewsStory?> GetStoryByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var endpoint = string.Format(_options.Endpoints.StoryById, id);
            try
            {
                var story = await _httpClient.GetAsync<HackerNewsStory>(endpoint, cancellationToken);
                if (story != null)
                {
                    _logger.LogInformation("Successfully retrieved story: {Title}", story.Title);
                }
                else
                {
                    _logger.LogWarning("Story with ID {StoryId} returned null", id);
                }
                return story;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch story detail for ID: {StoryId}", id);
                throw;
            }
        }
    }
}
