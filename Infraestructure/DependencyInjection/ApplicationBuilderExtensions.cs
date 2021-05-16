using Compartimoss.Example.Throttling;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseThrottling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ThrottlingMiddleware>();
        }
    }
}