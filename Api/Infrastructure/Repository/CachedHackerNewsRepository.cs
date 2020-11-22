using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Interfaces;
using Api.Domain.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Api.Infrastructure.Repository
{
    public class CachedHackerNewsRepository : ICachedHackerNewsRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private readonly IHackerNewsRepository _hackerNewsRepository;

        public CachedHackerNewsRepository(IConfiguration configuration, IMemoryCache memoryCache,
            IHackerNewsRepository hackerNewsRepository)
        {
            _configuration = configuration;
            _memoryCache = memoryCache;
            _hackerNewsRepository = hackerNewsRepository;
        }

        public async Task<List<Story>> GetBestStories()
        {
            if (_memoryCache.TryGetValue(CacheKeys.StoriesList, out List<Story> cacheEntry))
            {
                return cacheEntry;
            }

            cacheEntry = await _hackerNewsRepository.GetBestStories();
            if (cacheEntry == null || cacheEntry.Count == 0)
            {
                return null;
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(int.Parse(_configuration["CacheDurationSeconds"])));

            _memoryCache.Set(CacheKeys.StoriesList, cacheEntry, cacheEntryOptions);

            return cacheEntry;
        }

        private static class CacheKeys
        {
            public static string StoriesList => "_StoriesList";
        }
    }
}
