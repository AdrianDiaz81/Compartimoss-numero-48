using System;
using Compartimoss.Example.Throttling.Utilities;

namespace Compartimoss.Example.Throotling.Model
{
    	public class Rule
	{
		/// <summary>
		/// The IP or IP range to apply the rule
		/// </summary>
		/// <example>
		/// *
		/// 80.35.125.4
		/// 192.168.0.0/24
		/// fe80::/10
		/// 192.168.0.0-192.168.0.255
		/// </example>
		public string Ip { get; set; } = "*";

		/// <summary>
		/// HTTP verb and path 
		/// </summary>
		/// <example>
		/// get:/api/values
		/// *:/api/values
		/// *
		/// </example>
		public string Endpoint { get; set; }

		/// <summary>
		/// Rate limit period as in 1s, 1m, 1h
		/// </summary>
		public string Period { get; set; }

		public TimeSpan? PeriodTimespan { get => Period.ToTimeSpan(); }

		/// <summary>
		/// Maximum number of requests that a client can make in a defined period
		/// </summary>
		public double Limit { get; set; }
	}
}