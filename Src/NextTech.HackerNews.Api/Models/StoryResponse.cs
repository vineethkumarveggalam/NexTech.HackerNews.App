namespace NexTech.HackerNews.Api.Models
{
    /// <summary>
    /// Defines the <see cref="StoryResponse" />
    /// </summary>
    public class StoryResponse
    {
        /// <summary>
        /// Gets or sets the Title
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Url
        /// </summary>
        public string Url { get; set; } = string.Empty;
    }

    /// <summary>
    /// Defines the <see cref="PagedStories" />
    /// </summary>
    public class PagedStories
    {
        /// <summary>
        /// Gets or sets the Count
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the Stories
        /// </summary>
        public List<StoryResponse> Stories { get; set; } = [];
    }
}
