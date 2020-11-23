using System.Threading;
using System.Threading.Tasks;

namespace Api.Interfaces
{
    /// <summary>
    /// Worker to update the Best Stories cache in the background. 
    /// </summary>
    public interface IHackerNewsApiServiceWorker
    {
        /// <summary>
        /// Task to be executed in the background.
        /// </summary>
        /// <param name="cancellationToken">Task cancellationToken.</param>
        /// <returns>Returns a Task to be executed.</returns>
        public Task Execute(CancellationToken cancellationToken);
    }
}
