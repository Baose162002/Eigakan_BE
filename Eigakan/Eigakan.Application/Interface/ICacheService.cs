using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface
{
	public interface ICacheService
	{
		Task<T?> GetCacheResponseAsync<T>(string cacheKey);
		Task SetCacheResponseAsync<T>(string cacheKey, T response, TimeSpan timeToLive);
		Task RemoveCacheAsync(string cacheKey);
	}
}
