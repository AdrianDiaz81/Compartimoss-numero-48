using Compartimoss.Example.Throttling.Model;

namespace Compartimoss.Example.Throttling.Services
{
	public interface IRequestIdentityService
	{
		RequestIdentity Resolve();
	}
}