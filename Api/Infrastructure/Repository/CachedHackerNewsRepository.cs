using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Interfaces;
using Api.Domain.Model;
using Api.Infrastructure.Repository.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Api.Infrastructure.Repository
{
    /// <summary>
    /// Cached Hacker News Repository implementation.
    /// Uses a Decorator Pattern to acquire the data from <see cref="HackerNewsApiRepository"/> and save it in the
    /// Memory Cache instance.
    /// </summary>
    public class CachedHackerNewsRepository : ICachedHackerNewsRepository
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IHackerNewsRepository _hackerNewsRepository;

        /// <summary>
        /// Cached Hacker News Repository constructor used to Dependency Injection.
        /// </summary>
        /// <param name="memoryCache">Memory Cache instance to save the Best Stories.</param>
        /// <param name="hackerNewsRepository">Hacker News Repository instance to requests the data.</param>
        public CachedHackerNewsRepository(IMemoryCache memoryCache, IHackerNewsRepository hackerNewsRepository)
        {
            _memoryCache = memoryCache;
            _hackerNewsRepository = hackerNewsRepository;
        }

        /// <summary>
        /// Get the Hacker News Best Stories cached in memory.
        /// </summary>
        /// <returns>Returns a Task with the Best Stories. Null if there is no data in memory</returns>
        public async Task<List<Story>> GetBestStories()
        {
            return await Task.FromResult(_memoryCache.TryGetValue(CacheKeys.StoriesList,
                out List<Story> cacheEntry) ? cacheEntry : null);
        }

        /// <summary>
        /// Request the Best Stories and save them in the Memory Cache instance.
        /// </summary>
        /// <returns>Return a Task with the result status of the process.</returns>
        public async Task<bool> RequestBestStoriesAndCache()
        {
            var cacheEntry = await _hackerNewsRepository.GetBestStories();
            if (cacheEntry == null || cacheEntry.Count == 0)
            {
                return false;
            }

            _memoryCache.Set(CacheKeys.StoriesList, cacheEntry);

            return true;
        }

        /// <summary>
        /// Class with the Memory Cache Keys.
        /// </summary>
        private static class CacheKeys
        {
            public static string StoriesList => "_StoriesList";
        }
    }
}
