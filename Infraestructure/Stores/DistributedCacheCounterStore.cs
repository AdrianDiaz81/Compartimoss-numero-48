using Compartimoss.Example.AspNetCore.Throttling.Stores;
using Compartimoss.Example.Throotling.Model;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Compartimoss.Example.Throttling.Stores
{
	internal class DistributedCacheCounterStore : ICounterStore
	{
		private readonly IDistributedCache _cache;

		public DistributedCacheCounterStore(IDistributedCache cache)
		{
			_cache = cache;
		}

		public Task SetAsync(string id, Counter entry, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default)
		{
			var options = new DistributedCacheEntryOptions();

			if (expirationTime.HasValue)
			{
				options.SetAbsoluteExpiration(expirationTime.Value);
			}

			return _cache.SetStringAsync(id, JsonSerializer.Serialize(entry), options, cancellationToken);
		}

		public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
		{
			var stored = await _cache.GetStringAsync(id, cancellationToken);

			return !string.IsNullOrEmpty(stored);
		}

		public async Task<Counter> GetAsync(string id, CancellationToken cancellationToken = default)
		{
			var stored = await _cache.GetStringAsync(id, cancellationToken);

			if (!string.IsNullOrEmpty(stored))
			{
				return JsonSerializer.Deserialize<Counter>(stored);
			}

			return default;
		}

		public Task RemoveAsync(string id, CancellationToken cancellationToken = default)
		{
			return _cache.RemoveAsync(id, cancellationToken);
		}
	}
}