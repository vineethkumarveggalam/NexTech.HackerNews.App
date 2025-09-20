namespace Nextech.HackerNews.Infrastructure.Caching
{
    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.Extensions.Caching.Memory;
    using Nextech.HackerNews.Application.Interfaces;

    /// <summary>
    /// Defines the <see cref="MemoryCacheService" />
    /// </summary>
    public class MemoryCacheService : ICache
    {
        /// <summary>
        /// Defines the _cache
        /// </summary>
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCacheService"/> class.
        /// </summary>
        /// <param name="cache">The cache<see cref="IMemoryCache"/></param>
        public MemoryCacheService(IMemoryCache cache)
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
        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            if (_cache.TryGetValue(key, out T? value))
            {
                return Task.FromResult(value);
            }

            return Task.FromResult(default(T));
        }

        /// <summary>
        /// The SetAsync
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key<see cref="string"/></param>
        /// <param name="value">The value<see cref="T"/></param>
        /// <param name="expiry">The expiry<see cref="TimeSpan"/></param>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public Task SetAsync<T>(string key, T value, TimeSpan expiry, CancellationToken cancellationToken = default)
        {
            var options = new MemoryCacheEntryOptions();
            options.AbsoluteExpirationRelativeToNow = expiry;
            _cache.Set(key, value, options);
            return Task.CompletedTask;
        }
    }
}
