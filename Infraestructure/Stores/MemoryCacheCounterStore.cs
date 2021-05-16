using Compartimoss.Example.AspNetCore.Throttling.Stores;
using Compartimoss.Example.Throotling.Model;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Compartimoss.Example.Throttling.Stores
{
	internal class MemoryCacheCounterStore : ICounterStore
	{
		private readonly IMemoryCache _cache;

		public MemoryCacheCounterStore(IMemoryCache cache)
		{
			_cache = cache;
		}

		public Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
		{
			return Task.FromResult(_cache.TryGetValue(id, out _));
		}

		public Task<Counter> GetAsync(string id, CancellationToken cancellationToken = default)
		{
			if (_cache.TryGetValue(id, out Counter stored))
			{
				return Task.FromResult(stored);
			}

			return Task.FromResult(default(Counter));
		}

		public Task RemoveAsync(string id, CancellationToken cancellationToken = default)
		{
			_cache.Remove(id);

			return Task.CompletedTask;
		}

		public Task SetAsync(string id, Counter entry, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default)
		{
			var options = new MemoryCacheEntryOptions
			{
				Priority = CacheItemPriority.NeverRemove
			};

			if (expirationTime.HasValue)
			{
				options.SetAbsoluteExpiration(expirationTime.Value);
			}

			_cache.Set(id, entry, options);

			return Task.CompletedTask;
		}
	}
}