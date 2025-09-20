namespace Nextech.HackerNews.Application.Services
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Nextech.HackerNews.Application.Entities;
    using Nextech.HackerNews.Application.Interfaces;

    /// <summary>
    /// Defines the <see cref="NewsStoryService" />
    /// </summary>
    public class NewsStoryService(IHackerNewsApiClient apiClient, ICache cache, IConfiguration configuration, ILogger<NewsStoryService> logger) : INewsStoryService
    {
        /// <summary>
        /// Defines the _apiClient
        /// </summary>
        private readonly IHackerNewsApiClient _apiClient = apiClient;

        /// <summary>
        /// Defines the _cache
        /// </summary>
        private readonly ICache _cache = cache;

        /// <summary>
        /// Defines the _logger
        /// </summary>
        private readonly ILogger<NewsStoryService> _logger = logger;

        /// <summary>
        /// Defines the _cacheKey
        /// </summary>
        private readonly string _cacheKey = configuration.GetValue<string>("CacheSettings:TopStoriesCacheKey", "TopStories");

        /// <summary>
        /// Defines the _maxNewsItems
        /// </summary>
        private readonly int _maxNewsItems = configuration.GetValue<int>("HackerNews:MaxNewsItems", 200);

        /// <summary>
        /// Defines the _cacheDuration
        /// </summary>
        private readonly int _cacheDuration = configuration.GetValue<int>("CacheSettings:TopStoriesCacheDurationMinutes", 5);

        /// <summary>
        /// The GetNewestNewStoriesAsync
        /// </summary>
        /// <param name="count">The count<see cref="int"/></param>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task{StoryList}"/></returns>
        public async Task<StoryList> GetNewestNewStoriesAsync(int count, CancellationToken cancellationToken = default)
        {
            try
            {
                //Try cache first
                var cached = await _cache.GetAsync<List<HackerNewsStory>>(_cacheKey, cancellationToken);
                if (cached != null && cached.Count != 0)
                    return new StoryList { TotalCount = cached.Count, Stories = cached.Take(count).ToList() };

                var ids = await _apiClient.GetTopStoriesAsync(cancellationToken);
                if (ids == null || ids.Count == 0)
                    return new StoryList();

                var topIds = ids.Take(_maxNewsItems).ToList();

                // 4. Fetch stories in parallel
                var storyTasks = topIds.Select(id => _apiClient.GetStoryByIdAsync(id, cancellationToken));

                var stories = (await Task.WhenAll(storyTasks))
                       .OfType<HackerNewsStory>()
                       .Where(s => s != null && !string.IsNullOrWhiteSpace(s!.Url))                       
                       .ToList();

                // 5. Cache result 
                await _cache.SetAsync(_cacheKey, stories, TimeSpan.FromMinutes(_cacheDuration), cancellationToken);

                // 6. Return only what was asked for
                return new StoryList { TotalCount = stories.Count, Stories = stories.Take(count).ToList() };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch top stories from HackerNews Application");
            }
            return new StoryList();
        }

        /// <summary>
        /// The SearchStoriesAsync
        /// </summary>
        /// <param name="query">The query<see cref="string"/></param>
        /// <param name="pageNumber">The pageNumber<see cref="int"/></param>
        /// <param name="pageSize">The pageSize<see cref="int"/></param>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task{StoryList}"/></returns>
        public async Task<StoryList> SearchStoriesAsync(string query, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                // 1. Fetch top stories (cached)
                var storyList = await GetNewestNewStoriesAsync(_maxNewsItems, cancellationToken);

                if (storyList != null && storyList.TotalCount > 0)
                {
                    // 2. Filter stories by query (case-insensitive)
                    var filtered = storyList.Stories
                        .Where(s => !string.IsNullOrWhiteSpace(s.Url))
                        .Where(s => s.Title.Contains(query, StringComparison.OrdinalIgnoreCase));

                    return new StoryList()
                    {
                        TotalCount = filtered.Count(),
                        Stories = filtered
                                  .Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToList()
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch results while searching");
            }
            return new StoryList();
        }
    }
}
