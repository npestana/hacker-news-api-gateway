using System.Threading;
using System.Threading.Tasks;
using Api.Interfaces;
using Microsoft.Extensions.Hosting;

namespace API.Services
{
    /// <summary>
    /// Background service to update the Best Stories cache data.
    /// </summary>
    public class HackerNewsApiService : BackgroundService
    {
        private readonly IHackerNewsApiServiceWorker _worker;

        /// <summary>
        /// Hacker News API Service constructor used to Dependency Injection.
        /// </summary>
        /// <param name="worker"></param>
        public HackerNewsApiService(IHackerNewsApiServiceWorker worker)
        {
            _worker = worker;
        }

        /// <summary>
        /// Run the Service worker that will get and save the data in the memory cache.
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns>Return a Task to be executed.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _worker.Execute(stoppingToken);
        }
    }
}
