using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ycode.AspNetCore.Mvc.GroupVersioning.Versioning;

namespace Ycode.AspNetCore.Mvc.GroupVersioning.Routing
{
    public class ApiGroupVersioningApiVersionMatcherPolicy : MatcherPolicy, IEndpointSelectorPolicy
    {
        ILogger Logger;

        IOptions<ApiGroupVersioningOptions> _options;
        protected ApiGroupVersioningOptions Options => _options.Value;

        public ApiGroupVersioningApiVersionMatcherPolicy(
            IOptions<ApiGroupVersioningOptions> options,
            ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger(GetType());

            _options = options;
        }

        public override int Order => -1;

        public bool AppliesToEndpoints(IReadOnlyList<Endpoint> endpoints)
        {
            return endpoints.Any(e =>
                e.Metadata?.GetMetadata<ActionDescriptor>()?.GetProperty<ApiVersionModel>() != null);
        }

        public Task ApplyAsync(HttpContext httpContext, CandidateSet candidates)
        {
            var feature = httpContext.Features.Get<IApiVersioningFeature>();
            var requestedVersion = httpContext.GetRequestedApiVersion();

            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (candidates == null)
            {
                throw new ArgumentNullException(nameof(candidates));
            }

            if (requestedVersion == null)
            {
                return Task.CompletedTask;
            }

            var implicitGroupVersioning = Options.ImplicitGroupVersioning;
            var declaredVersionList = new List<ApiVersion>();
            var declaredGroupVersionMap = new Dictionary<DateTime, List<ApiVersion>>();

            for (var i = 0; i < candidates.Count; i++)
            {
                var action = candidates[i].Endpoint.Metadata?.GetMetadata<ActionDescriptor>();
                if (action == null)
                {
                    continue;
                }

                var actionVersions = action.GetApiVersionModel(ApiVersionMapping.Implicit);
                if (actionVersions.IsApiVersionNeutral)
                {
                    continue;
                }

                declaredVersionList.AddRange(actionVersions.DeclaredApiVersions);

                foreach (var declaredApiVersion in actionVersions.DeclaredApiVersions)
                {
                    if (!declaredApiVersion.GroupVersion.HasValue)
                    {
                        implicitGroupVersioning = false;
                        continue;
                    }

                    if (!declaredGroupVersionMap.TryGetValue(declaredApiVersion.GroupVersion.Value, out var actions))
                    {
                        actions = new List<ApiVersion>();
                        declaredGroupVersionMap.Add(declaredApiVersion.GroupVersion.Value, actions);
                    }
                    actions.Add(declaredApiVersion);
                }
            }

            if (requestedVersion.GroupVersion.HasValue)
            {
                var groupApiVersions = declaredGroupVersionMap.Keys.ToList().OrderByDescending(g => g);
                foreach (var groupApiVersion in groupApiVersions)
                {
                    if (implicitGroupVersioning
                        ? requestedVersion.GroupVersion >= groupApiVersion
                        : requestedVersion.GroupVersion == groupApiVersion)
                    {
                        var declaredVersion = declaredGroupVersionMap[groupApiVersion]
                            .FirstOrDefault(v =>
                                requestedVersion.MajorVersion == null
                                || requestedVersion.EqualsIgnoringGroupVersion(v));
                        feature.RequestedApiVersion = declaredVersion;
                        break;
                    }
                }
            }
            else
            {
                var declaredVersion = declaredVersionList.FirstOrDefault(v =>
                    requestedVersion.EqualsIgnoringGroupVersion(v));
                if (declaredVersion != null)
                {
                    feature.RequestedApiVersion = declaredVersion;
                }
            }

            return Task.CompletedTask;
        }
    }
}
