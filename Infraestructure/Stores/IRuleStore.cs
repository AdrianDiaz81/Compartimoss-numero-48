using Compartimoss.Example.Throotling.Model;
using System.Threading;
using System.Threading.Tasks;

namespace  Compartimoss.Example.AspNetCore.Throttling.Stores
{
	public interface IRuleStore
	{
		Task<RuleCollection> GetAsync(CancellationToken cancellationToken = default);
	}
}