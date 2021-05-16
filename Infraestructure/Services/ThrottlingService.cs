using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Compartimoss.Example.AspNetCore.Throttling.Stores;
using Compartimoss.Example.Throotling.Model;
using Compartimoss.Example.Throttling.Model;
using Compartimoss.Example.Throttling.Utilities;

namespace Compartimoss.Example.Throttling.Services
{
	public class ThrottlingService : IThrottlingService
	{
		/// The key-lock used for limiting requests.
		private static readonly AsyncKeyLock AsyncLock = new AsyncKeyLock();

		private readonly IRuleStore _ruleStore;
		private readonly ICounterStore _counterStore;
		private readonly ICounterKeyService _counterKeyBuilder;

		public ThrottlingService(IRuleStore ruleStore, ICounterStore counterStore, ICounterKeyService counterKeyBuilder)
		{
			_ruleStore = ruleStore;
			_counterStore = counterStore;
			_counterKeyBuilder = counterKeyBuilder;
		}

		public async Task<IEnumerable<Rule>> GetMatchingRulesAsync(RequestIdentity identity, CancellationToken cancellationToken = default)
		{
			var rules = await _ruleStore.GetAsync(cancellationToken);
			if (rules?.Rules?.Any() == true)
			{
				return rules.Rules
					        .Where(item => (item.Ip == "*" || IpParser.ContainsIp(item.Ip, identity.Ip)) && RuleMatches(identity, rules, item))
					        .GroupBy(l => l.Period)
							.Select(l => l.OrderBy(x => x.Limit))
							.Select(l => l.First())
							.OrderBy(l => l.PeriodTimespan)
							.ToList();
			}

			return new Rule[0];
		}

		public async Task<bool> IsWhitelistedAsync(RequestIdentity requestIdentity, CancellationToken cancellationToken = default)
		{
			var rules = await _ruleStore.GetAsync(cancellationToken);
			if (rules==null)
            {
				return false;
            }
			if (rules.IpWhitelist != null && IpParser.ContainsIp(rules.IpWhitelist, requestIdentity.Ip))
			{
				return true;
			}

			if (rules.EndpointWhitelist != null && rules.EndpointWhitelist.Any())
			{
				var path = rules.EnableRegexRuleMatching ? $".+:{requestIdentity.Path}" : $"*:{requestIdentity.Path}";
				return rules.EndpointWhitelist.Any(x => $"{requestIdentity.HttpMethod}:{requestIdentity.Path}".IsUrlMatch(x, rules.EnableRegexRuleMatching))
					   || rules.EndpointWhitelist.Any(x => path.IsUrlMatch(x, rules.EnableRegexRuleMatching));
			}

			return false;
		}

		public async Task<Counter> ProcessRequestAsync(RequestIdentity requestIdentity, Rule rule, CancellationToken cancellationToken = default)
		{
			var counter = new Counter
			{
				Timestamp = DateTime.UtcNow,
				Count = 1
			};

			var counterId = BuildCounterKey(requestIdentity, rule);
			using (await AsyncLock.WriterLockAsync(counterId).ConfigureAwait(false))
			{
				var entry = await _counterStore.GetAsync(counterId, cancellationToken);
				if (entry != null)
				{
					// entry has not expired
					if (entry.Timestamp + rule.PeriodTimespan.Value >= DateTime.UtcNow)
					{
						// increment request count
						var totalCount = entry.Count + 1;

						// deep copy
						counter = new Counter
						{
							Timestamp = entry.Timestamp,
							Count = totalCount
						};
					}
				}

				// stores: id (string) - timestamp (datetime) - total_requests (long)
				await _counterStore.SetAsync(counterId, counter, rule.PeriodTimespan.Value, cancellationToken);
			}

			return counter;
		}

		public RateLimitHeaders GetRateLimitHeaders(Counter counter, Rule rule, CancellationToken cancellationToken = default)
		{
			var reset = (counter?.Timestamp ?? DateTime.UtcNow) + (rule.PeriodTimespan ?? rule.Period.ToTimeSpan());
			var remaining = rule.Limit - (counter?.Count ?? 0);
			return new RateLimitHeaders
			{
				Reset = reset.ToUniversalTime().ToString("o", DateTimeFormatInfo.InvariantInfo),
				Limit = rule.Period,
				Remaining = remaining.ToString()
			};
		}

		private string BuildCounterKey(RequestIdentity requestIdentity, Rule rule)
		{
			var key = _counterKeyBuilder.Create(requestIdentity, rule);
			var bytes = Encoding.UTF8.GetBytes(key);
			using (var algorithm = new SHA1Managed())
			{
				var hash = algorithm.ComputeHash(bytes);
				return Convert.ToBase64String(hash);
			}
		}

		private bool RuleMatches(RequestIdentity identity, RuleCollection rules, Rule item)
		{
			var path1 = rules.EnableRegexRuleMatching ? $".+:{identity.Path}" : $"*:{identity.Path}";
			var path2 = $"{identity.HttpMethod}:{identity.Path}";
			if (path1.IsUrlMatch(item.Endpoint, rules.EnableRegexRuleMatching))
			{
				// search for rules with endpoints like "*" and "*:/matching_path"
				return true;
			}

			if (path2.IsUrlMatch(item.Endpoint, rules.EnableRegexRuleMatching))
			{
				// search for rules with endpoints like "matching_verb:/matching_path"
				return true;
			}

			if (item.Endpoint == "*")
			{
				return true;
			}

			return false;
		}
	}
}