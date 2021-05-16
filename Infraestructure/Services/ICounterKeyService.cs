using Compartimoss.Example.Throotling.Model;
using Compartimoss.Example.Throttling.Model;

namespace Compartimoss.Example.Throttling.Services
{
	public interface ICounterKeyService
	{
		string Create(RequestIdentity requestIdentity, Rule rule);
	}
}