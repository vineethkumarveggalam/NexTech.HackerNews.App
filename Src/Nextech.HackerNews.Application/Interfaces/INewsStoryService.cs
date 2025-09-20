namespace Nextech.HackerNews.Application.Interfaces
{
    using Nextech.HackerNews.Application.Entities;

    /// <summary>
    /// Defines the <see cref="INewsStoryService" />
    /// </summary>
    public interface INewsStoryService
    {
        /// <summary>
        /// The GetNewestNewStoriesAsync
        /// </summary>
        /// <param name="count">The count<see cref="int"/></param>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task{StoryList}"/></returns>
        Task<StoryList> GetNewestNewStoriesAsync(int count, CancellationToken cancellationToken);

        /// <summary>
        /// The SearchStoriesAsync
        /// </summary>
        /// <param name="query">The query<see cref="string"/></param>
        /// <param name="pageNumber">The pageNumber<see cref="int"/></param>
        /// <param name="pageSize">The pageSize<see cref="int"/></param>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task{StoryList}"/></returns>
        Task<StoryList> SearchStoriesAsync(
        string query,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
    }
}
