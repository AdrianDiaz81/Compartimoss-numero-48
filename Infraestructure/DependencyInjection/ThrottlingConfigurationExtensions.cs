using System;
using Compartimoss.Example.AspNetCore.Throttling.Stores;
using Compartimoss.Example.Throotling.Model;
using Compartimoss.Example.Throttling.Stores;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class ThrottlingConfigurationExtensions
	{
		public static ThrottlingConfiguration AddMemoryCacheCounterStore(this ThrottlingConfiguration configuration)
		{
			return configuration.AddCounterStore<MemoryCacheCounterStore>();
		}
		public static ThrottlingConfiguration AddDistributedCacheCounterStore(this ThrottlingConfiguration configuration)
		{
			return configuration.AddCounterStore<DistributedCacheCounterStore>();
		}
		public static ThrottlingConfiguration AddRules(this ThrottlingConfiguration configuration, Action<RuleCollection> rules)
		{
			configuration.Services.Configure<RuleCollection>(p => rules?.Invoke(p));
			return configuration.AddRuleStore<ConfigurationRuleStore>();
		}
	}
}
