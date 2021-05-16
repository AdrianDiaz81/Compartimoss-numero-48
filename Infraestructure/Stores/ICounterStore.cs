using System;
using System.Threading;
using System.Threading.Tasks;
using Compartimoss.Example.Throotling.Model;

namespace Compartimoss.Example.AspNetCore.Throttling.Stores
{
	public interface ICounterStore
	{
		Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
		Task<Counter> GetAsync(string id, CancellationToken cancellationToken = default);
		Task RemoveAsync(string id, CancellationToken cancellationToken = default);
		Task SetAsync(string id, Counter entry, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default);
	}
}