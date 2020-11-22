using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Api.Domain.Model;
using Api.Infrastructure.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        private readonly ILogger<ApiController> _logger;
        private readonly ICachedHackerNewsRepository _hackerNewsRepository;

        public ApiController(ILogger<ApiController> logger, ICachedHackerNewsRepository hackerNewsRepository)
        {
            _logger = logger;
            _hackerNewsRepository = hackerNewsRepository;
        }

        [HttpGet("BestStories")]
        public async Task<List<Story>> BestStories()
        {
            var start = DateTime.Now;

            var result = await _hackerNewsRepository.GetBestStories();

            var timeTook = (DateTime.Now - start).TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
            _logger.LogInformation($"Time took to get the stories: {timeTook}");

            Response.Headers.Add("X-Time-Took", timeTook);

            if (result == null || result.Count == 0)
            {
                Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            }

            return result;
        }
    }
}
