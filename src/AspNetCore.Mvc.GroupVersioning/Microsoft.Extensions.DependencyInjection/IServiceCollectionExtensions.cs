using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ycode.AspNetCore.Mvc.GroupVersioning.Routing;
using Ycode.AspNetCore.Mvc.GroupVersioning.Versioning;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static void AddApiGroupVersioning(
            this IServiceCollection services,
            Action<ApiGroupVersioningOptions> setup)
        {
            if (!services.Any(s => s.ImplementationType == typeof(ApiVersionMatcherPolicy)))
            {
                throw new InvalidOperationException(
                    "ApiVersioningMiddleware is requied by API Group Versioning " +
                    "so that ApiVersioningFeature is added to HttpContext during the request execution pipeline. " +
                    "Please add ApiVersioningMiddleware by calling 'IServiceCollection.AddApiVersioning' " +
                    "inside the call to 'ConfigureServices(...)' in the application startup code.");
            }

            services.Configure<ApiGroupVersioningOptions>(setup);
            services.TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, ApiGroupVersioningApiVersionMatcherPolicy>() );
            services.TryAddEnumerable(ServiceDescriptor.Transient<IActionDescriptorProvider, ApiGroupVersioningApiVersionCollator>() );
        }
    }
}
