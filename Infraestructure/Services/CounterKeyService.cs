using Compartimoss.Example.Throotling.Model;
using Compartimoss.Example.Throttling.Model;

namespace Compartimoss.Example.Throttling.Services
{
	public class CounterKeyService : ICounterKeyService
	{
		public string Create(RequestIdentity requestIdentity, Rule rule)
		{
			return $"tht_{requestIdentity.Ip}_{rule.Period}_{requestIdentity.HttpMethod}_{requestIdentity.Path}";
		}
	}
}