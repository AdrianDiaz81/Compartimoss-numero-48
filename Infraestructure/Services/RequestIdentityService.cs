using Compartimoss.Example.Throttling.Model;
using Microsoft.AspNetCore.Http;

namespace Compartimoss.Example.Throttling.Services
{
	internal class RequestIdentityService : IRequestIdentityService
	{
		private const string IpBehindGatewayHeaderKey = "x-forwarded-for";

		private readonly IHttpContextAccessor _httpContextAccessor;

		public RequestIdentityService(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public RequestIdentity Resolve()
		{
			return new RequestIdentity
			{
				Ip = GetIp(),
				Path = _httpContextAccessor.HttpContext.Request.Path.ToString().ToLowerInvariant(),
				HttpMethod = _httpContextAccessor.HttpContext.Request.Method.ToLowerInvariant()
			};
		}

		private string GetIp()
		{
			if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue(IpBehindGatewayHeaderKey, out var values))
			{
				return values.ToString();
			}

			return _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
		}
	}
}
