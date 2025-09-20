namespace NexTech.HackerNews.Infrastructure.Models
{
    /// <summary>
    /// Defines the <see cref="HackerNewsOptions" />
    /// </summary>
    public class HackerNewsOptions
    {
        /// <summary>
        /// Gets or sets the BaseUrl
        /// </summary>
        public string BaseUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Endpoints
        /// </summary>
        public EndpointsConfig Endpoints { get; set; } = new();

        /// <summary>
        /// Gets or sets the TimeoutSeconds
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// Defines the <see cref="EndpointsConfig" />
        /// </summary>
        public class EndpointsConfig
        {
            /// <summary>
            /// Gets or sets the TopStories
            /// </summary>
            public string TopStories { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the StoryById
            /// </summary>
            public string StoryById { get; set; } = string.Empty;
        }
    }

}
