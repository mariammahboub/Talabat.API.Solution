using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Services.Contract;

namespace Talabat.Services
{
    public class ResponseCacheServices : IResponseCacheServices
    {
        private readonly IDatabase _database;

        public ResponseCacheServices(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }
        public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
        {
            if (response is null) return;
            var serializeOptions = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var serializeResponse = JsonSerializer.Serialize(response);//basxel case lazam aholh
            await _database.StringSetAsync(cacheKey, serializeResponse, timeToLive);
        }

        public async Task<string?> GetCacheResponseAsync(string cacheKey)
        {
            var cacheResponse = await _database.StringGetAsync(cacheKey);
            if (cacheResponse.IsNullOrEmpty) return null;//astruct
            return cacheResponse;
        }
    }
}
