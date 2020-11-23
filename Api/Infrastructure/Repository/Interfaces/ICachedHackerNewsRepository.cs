using System.Threading.Tasks;
using Api.Domain.Interfaces;

namespace Api.Infrastructure.Repository.Interfaces
{
    /// <summary>
    /// Cached Hacker News Repository specification.
    /// </summary>
    public interface ICachedHackerNewsRepository : IHackerNewsRepository
    {
        /// <summary>
        /// Request the Best Stories and save them in the Memory Cache instance.
        /// </summary>
        /// <returns>Return a Task with the result status of the process.</returns>
        public Task<bool> RequestBestStoriesAndCache();
    }
}
