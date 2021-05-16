using System;
using Compartimoss.Example.Throttling;
using Compartimoss.Example.Throttling.Services;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{
		public static ThrottlingConfiguration AddThrottling(this IServiceCollection services, Action<ThrottlingOptions> configure = null)
		{
			services.Configure<ThrottlingOptions>(op => configure?.Invoke(op));
			services.AddScoped<IThrottlingService, ThrottlingService>();
			services.AddScoped<ThrottlingMiddleware>();

			var configuration = new ThrottlingConfiguration(services);

            var options = new ThrottlingOptions();
            configure.Invoke(options);
               if (options.CounterStoreType == CounterStoreType.DistributedCache)
            {
                configuration.AddDistributedCacheCounterStore();
            } else
            {
                configuration.AddMemoryCacheCounterStore();
            }				

			configuration.AddCounterKeyService<CounterKeyService>();
			configuration.AddRequestIdentityService<RequestIdentityService>();

			return configuration;
		}
    }
}
