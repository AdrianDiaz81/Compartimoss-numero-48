using Compartimoss.Example.Throotling.Model;
using Compartimoss.Example.Throttling.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Compartimoss.Example.Throttling
{
	public class ThrottlingOptions
    {

        /// <summary>
        /// Enabled Options at throttling
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the HTTP Status code returned when rate limiting occurs, by default value is set to 429 (Too Many Requests)
        /// </summary>
        public int HttpStatusCode { get; set; } = 429;

        /// <summary>
        /// Gets or sets a value that will be used as a formatter for the QuotaExceeded response message.
        /// If none specified the default will be: 
        /// API calls quota exceeded! maximum admitted {0} per {1}
        /// </summary>
        public string QuotaExceededMessage { get; set; }

        /// <summary>
        /// Gets or sets a model that represents the QuotaExceeded response (content-type, content, status code).
        /// </summary>
        public QuotaExceededResponse QuotaExceededResponse { get; set; }

        /// <summary>
        /// Disables X-Rate-Limit and Rety-After headers
        /// </summary>
        public bool DisableRateLimitHeaders { get; set; }

        /// <summary>
        /// Action we want to take when the quota is reached
        /// </summary>
        public ThrottlingMode Mode { get; set; } = ThrottlingMode.Observed;

        /// <summary>
        /// Gets or sets the store where the counters are stored
        /// </summary>
        public CounterStoreType CounterStoreType { get; set; } = CounterStoreType.InMemory;

        /// <summary>
        /// Gets or sets behavior after the request is blocked
        /// </summary>
        public Func<HttpContext, RequestIdentity, Counter, Rule, Task> RequestBlockedBehaviorAsync { get; set; }
    }

    public enum ThrottlingMode
    {   
        Observed,
        Restricted
    }

    public enum CounterStoreType
    {
        InMemory,
        DistributedCache
    }
}
