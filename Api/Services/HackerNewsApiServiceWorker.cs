using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Api.Infrastructure.Repository.Interfaces;
using Api.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace API.Services
{
    /// <summary>
    /// Worker to update the Best Stories cache in the background. 
    /// </summary>
    public class HackerNewsApiServiceWorker : IHackerNewsApiServiceWorker
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HackerNewsApiServiceWorker> _logger;
        private readonly ICachedHackerNewsRepository _cachedHackerNewsRepository;

        /// <summary>
        /// Hacker News API Service Worker constructor used to Dependency Injection.
        /// </summary>
        /// <param name="configuration">Configuration instance.</param>
        /// <param name="logger">Logger instance.</param>
        /// <param name="cachedHackerNewsRepository">Hacker News Repository update the data.</param>
        public HackerNewsApiServiceWorker(IConfiguration configuration, ILogger<HackerNewsApiServiceWorker> logger,
            ICachedHackerNewsRepository cachedHackerNewsRepository)
        {
            _configuration = configuration;
            _logger = logger;
            _cachedHackerNewsRepository = cachedHackerNewsRepository;
        }

        /// <summary>
        /// Task to be executed in the background.
        /// </summary>
        /// <param name="cancellationToken">Task cancellationToken.</param>
        /// <returns>Returns a Task to be executed.</returns>
        public async Task Execute(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var start = DateTime.Now;
                var result = await _cachedHackerNewsRepository.RequestBestStoriesAndCache();
                var timeTook = (DateTime.Now - start).TotalMilliseconds.ToString(CultureInfo.InvariantCulture);

                if (result)
                {
                    _logger.LogInformation($"Worker - Successfully retrieved Best Stories. Time took: {timeTook}");
                }
                else
                {
                    _logger.LogCritical($"Worker - Failed to retrieve Best Stories. Time took: {timeTook}");
                }

                await Task.Delay(TimeSpan.FromSeconds(int.Parse(_configuration["CacheDurationSeconds"])),
                    cancellationToken);
            }
        }
    }
}
