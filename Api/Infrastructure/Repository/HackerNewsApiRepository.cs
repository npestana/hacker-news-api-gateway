using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Api.Domain.Interfaces;
using Api.Domain.Model;
using Api.Infrastructure.Repository.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Api.Infrastructure.Repository
{
    public class HackerNewsApiRepository : IHackerNewsRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<HackerNewsApiRepository> _logger;

        public HackerNewsApiRepository(IConfiguration configuration, IHttpClientFactory clientFactory,
            ILogger<HackerNewsApiRepository> logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task<List<Story>> GetBestStories()
        {
            var bestStoriesIds = await GetBestStoriesIds();
            var storiesIds = bestStoriesIds as int[] ?? bestStoriesIds.ToArray();

            if (storiesIds.Length == 0)
            {
                return null;
            }

            var validStories = 0;
            var list = new List<Story>();

            foreach (var storyId in storiesIds)
            {
                var item = await GetBestStory(storyId);

                if (item == null)
                {
                    continue;
                }

                list.Add(new Story
                {
                    Title = item.Title,
                    Uri = item.Url,
                    PostedBy = item.Author,
                    Time = item.Time.ToString(),
                    Score = item.Score,
                    CommentCount = item.Comments?.Count ?? 0
                });

                if (++validStories == int.Parse(_configuration["TopStoriesNumber"]))
                {
                    break;
                }
            }

            return list.Count > 0 ? list : null;
        }

        private async Task<IEnumerable<int>> GetBestStoriesIds()
        {
            HttpResponseMessage response;

            try
            {
                var client = _clientFactory.CreateClient();
                response = await client.SendAsync(CreateRequest(_configuration["HackerNewsAPI:BestStoriesUrl"]));
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, "Can't get the Best Stories list.");
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            await using var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<IEnumerable<int>>(responseStream);
        }

        private async Task<HackerNewsStoryDto> GetBestStory(int storyId)
        {
            HttpResponseMessage response;

            try
            {
                var client = _clientFactory.CreateClient();
                response = await client.SendAsync(CreateRequest(string.Format(
                    _configuration["HackerNewsAPI:ItemDetailUrl"], storyId.ToString())));
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, $"Can't get the Story {storyId} details.");
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            
            await using var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<HackerNewsStoryDto>(responseStream);
        }

        private HttpRequestMessage CreateRequest(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("User-Agent", _configuration["HttpUserAgent"]);

            return request;
        }
    }
}