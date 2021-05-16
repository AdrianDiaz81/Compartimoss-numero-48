using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Compartimoss.Example.Throotling.Model;
using Compartimoss.Example.Throttling.Model;

namespace Compartimoss.Example.Throttling.Services
{
	public interface IThrottlingService
	{
		Task<IEnumerable<Rule>> GetMatchingRulesAsync(RequestIdentity identity, CancellationToken cancellationToken = default);

		RateLimitHeaders GetRateLimitHeaders(Counter counter, Rule rule, CancellationToken cancellationToken = default);

		Task<Counter> ProcessRequestAsync(RequestIdentity requestIdentity, Rule rule, CancellationToken cancellationToken = default);

		Task<bool> IsWhitelistedAsync(RequestIdentity requestIdentity, CancellationToken cancellationToken = default);
	}
}