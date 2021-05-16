using System.Collections.Generic;

namespace  Compartimoss.Example.Throotling.Model
{
	public class RuleCollection
	{
        public List<string> EndpointWhitelist { get; set; } = new List<string>();

		public List<string> IpWhitelist { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a value indicating whether all requests, including the rejected ones, should be stacked in this order: day, hour, min, sec
        /// </summary>
        public bool StackBlockedRequests { get; set; } = true;

        /// <summary>
        /// Enabled the comparison logic to use Regex instead of wildcards.
        /// </summary>
        public bool EnableRegexRuleMatching { get; set; } = false;

        public List<Rule> Rules { get; set; } = new List<Rule>();
	}
}
