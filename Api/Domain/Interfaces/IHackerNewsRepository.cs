using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Model;

namespace Api.Domain.Interfaces
{
    /// <summary>
    /// Main Hacker News Repository Specification.
    /// </summary>
    public interface IHackerNewsRepository
    {
        /// <summary>
        /// Get the Hacker News Best Stories.
        /// </summary>
        /// <returns>Returns a Task with the Best Stories.</returns>
        public Task<List<Story>> GetBestStories();
    }
}
