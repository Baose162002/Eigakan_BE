using Eigakan.Application.Helper.Configuration;
using Eigakan.Application.Interface;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Service
{
	public class CacheService : ICacheService
	{
		private readonly StackExchange.Redis.IDatabase _cacheDb;
		private readonly RedisConfiguration _redisConfig;

		public CacheService(IOptions<RedisConfiguration> redisConfig, ConnectionMultiplexer redis)
		{
			_redisConfig = redisConfig.Value; 

			if (_redisConfig.Enable)
			{
				_cacheDb = redis.GetDatabase();
			}
		}


		public async Task<T?> GetCacheResponseAsync<T>(string cacheKey)
		{
			var cacheResponse = await _cacheDb.StringGetAsync(cacheKey);
			return cacheResponse.HasValue ? JsonConvert.DeserializeObject<T>(cacheResponse) : default;
		}

		public async Task SetCacheResponseAsync<T>(string cacheKey, T response, TimeSpan timeToLive)
		{
			var serializedData = JsonConvert.SerializeObject(response);
			await _cacheDb.StringSetAsync(cacheKey, serializedData, timeToLive);
		}

		public async Task RemoveCacheAsync(string cacheKey)
		{
			await _cacheDb.KeyDeleteAsync(cacheKey);
		}
	}
}