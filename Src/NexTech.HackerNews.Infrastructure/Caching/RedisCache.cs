namespace Nextech.HackerNews.Infrastructure.Caching
{
    using Microsoft.Extensions.Caching.Distributed;
    using Nextech.HackerNews.Application.Interfaces;
    using System.Text.Json;

    /// <summary>
    /// Defines the <see cref="RedisCache" />
    /// </summary>
    public class RedisCache : ICache
    {
        /// <summary>
        /// Defines the _cache
        /// </summary>
        private readonly IDistributedCache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCache"/> class.
        /// </summary>
        /// <param name="cache">The cache<see cref="IDistributedCache"/></param>
        public RedisCache(IDistributedCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// The GetAsync
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key<see cref="string"/></param>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task{T?}"/></returns>
        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var cached = await _cache.GetStringAsync(key, cancellationToken);
            return cached == null ? default : JsonSerializer.Deserialize<T>(cached);
        }

        /// <summary>
        /// The SetAsync
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key<see cref="string"/></param>
        /// <param name="value">The value<see cref="T"/></param>
        /// <param name="ttl">The ttl<see cref="TimeSpan"/></param>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            };

            var serialized = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, serialized, options, cancellationToken);
        }
    }
}
