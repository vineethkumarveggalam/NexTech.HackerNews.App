namespace NexTech.HackerNews.Infrastructure.Integrations
{
    using System.Net.Http;
    using System.Net.Http.Json;

    /// <summary>
    /// Defines the <see cref="HttpClientWrapper" />
    /// </summary>
    public class HttpClientWrapper
    {
        /// <summary>
        /// Defines the _httpClient
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientWrapper"/> class.
        /// </summary>
        /// <param name="httpClient">The httpClient<see cref="HttpClient"/></param>
        public HttpClientWrapper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// The GetAsync
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint">The endpoint<see cref="string"/></param>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task{T?}"/></returns>
        public async Task<T?> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        }
    }
}
