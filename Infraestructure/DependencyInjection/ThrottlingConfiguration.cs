using Compartimoss.Example.AspNetCore.Throttling.Stores;
using Compartimoss.Example.Throttling.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
	public class ThrottlingConfiguration
	{
		internal ThrottlingConfiguration(IServiceCollection services)
		{
			Services = services;
		}

		public IServiceCollection Services { get; }

		public ThrottlingConfiguration AddCounterStore<T>() where T : class, ICounterStore
		{
			return Replace<ICounterStore, T>();
		}

		public ThrottlingConfiguration AddRuleStore<T>() where T : class, IRuleStore
		{
			return Replace<IRuleStore, T>();
		}

		public ThrottlingConfiguration AddCounterKeyService<T>() where T : class, ICounterKeyService
		{
			return Replace<ICounterKeyService, T>();
		}

		public ThrottlingConfiguration AddRequestIdentityService<T>() where T : class, IRequestIdentityService
		{
			return Replace<IRequestIdentityService, T>();
		}

		private ThrottlingConfiguration Replace<TInterface, TService>(ServiceLifetime lifeTime = ServiceLifetime.Scoped) where TService : class, TInterface
		{
			var descriptor = new ServiceDescriptor(typeof(TInterface), typeof(TService), lifeTime);
			Services.Replace(descriptor);
			return this;
		}
	}
}
