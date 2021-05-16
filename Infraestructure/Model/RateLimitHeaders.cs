using Microsoft.AspNetCore.Http;

namespace Compartimoss.Example.Throttling.Model
{	public class RateLimitHeaders
	{
		public HttpContext Context { get; set; }

		public string Limit { get; set; }

		public string Remaining { get; set; }

		public string Reset { get; set; }
	}
}