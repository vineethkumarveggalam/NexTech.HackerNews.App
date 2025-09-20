namespace Nextech.HackerNews.Application.Entities
{
    /// <summary>
    /// Defines the <see cref="StoryList" />
    /// </summary>
    public class StoryList
    {
        /// <summary>
        /// Gets or sets the Stories
        /// </summary>
        public List<HackerNewsStory> Stories { get; set; } = [];

        /// <summary>
        /// Gets or sets the TotalCount
        /// </summary>
        public int TotalCount { get; set; }
    }
}
