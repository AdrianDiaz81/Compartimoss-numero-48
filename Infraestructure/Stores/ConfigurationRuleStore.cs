using Compartimoss.Example.Throotling.Model;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace Compartimoss.Example.AspNetCore.Throttling.Stores
{
	internal class ConfigurationRuleStore : IRuleStore
	{
		private readonly RuleCollection _rules;

		public ConfigurationRuleStore(IOptions<RuleCollection> options)
		{
			_rules = options.Value;
		}

		public Task<RuleCollection> GetAsync(CancellationToken cancellationToken = default)
		{
			return Task.FromResult(_rules);
		}

        
    }
}
