using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Api.Domain.Model;
using Api.Infrastructure.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    /// <summary>
    /// Main API controller.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        private readonly ILogger<ApiController> _logger;
        private readonly ICachedHackerNewsRepository _hackerNewsRepository;

        /// <summary>
        /// API controller constructor used to Dependency Injection.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        /// <param name="hackerNewsRepository">Hacker News Repository to get the data from.</param>
        public ApiController(ILogger<ApiController> logger, ICachedHackerNewsRepository hackerNewsRepository)
        {
            _logger = logger;
            _hackerNewsRepository = hackerNewsRepository;
        }

        /// <summary>
        /// Returns the Best Hacker News Stories in JSON format.
        /// </summary>
        /// <returns>JSON Array with the Best Stories.</returns>
        [HttpGet("BestStories")]
        public async Task<List<Story>> BestStories()
        {
            var start = DateTime.Now;

            var result = await _hackerNewsRepository.GetBestStories();

            var timeTook = (DateTime.Now - start).TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
            _logger.LogInformation($"Controller - Time took to get the stories: {timeTook}");

            Response.Headers.Add("X-Time-Took", timeTook);

            if (result == null || result.Count == 0)
            {
                Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            }

            return result;
        }
    }
}
