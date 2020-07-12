using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ycode.AspNetCore.Mvc.GroupVersioning.Versioning
{
    public class ApiGroupVersioningApiVersionCollator : IActionDescriptorProvider
    {
        ILogger logger;

        IOptions<ApiGroupVersioningOptions> _options;
        protected ApiGroupVersioningOptions Options => _options.Value;

        public ApiGroupVersioningApiVersionCollator(IOptions<ApiGroupVersioningOptions> options, ILoggerFactory loggerFactory)
        {
            _options = options;
            logger = loggerFactory.CreateLogger(GetType());
        }

        public int Order { get; protected set; }

        public void OnProvidersExecuted(ActionDescriptorProviderContext context)
        {
            // API Group Versioning Validation

            // The validation here is based on the definition in Microsoft REST API Guidelines - 12 Versioning.
            // https://github.com/Microsoft/api-guidelines/blob/master/Guidelines.md#12-versioning
            //
            // However, note that API Group Versioning never supports the pattern that the same group version is associated
            // with multiple major.minor versions over the same endpoint, which the guideline document shows in an example.
            //
            // This was already discussed in GitHub microsoft/aspnet-api-versioning, which is in the issue linked below,
            // and API Group Versioning takes the same stance as @commonsensesoftware explained there.
            // https://github.com/microsoft/aspnet-api-versioning/issues/334

            List<DateTime> allGroupVersions = null;

            if (Options.ImplicitGroupVersioning)
            {
                allGroupVersions = new List<DateTime>();

                foreach (var actionDescriptor in context.Results)
                {
                    var model = actionDescriptor.GetProperty<ApiVersionModel>();

                    if (model == null)
                        continue;

                    foreach (var apiVersion in model.ImplementedApiVersions)
                    {
                        if (apiVersion.GroupVersion != null
                            && !allGroupVersions.Contains(apiVersion.GroupVersion.Value))
                        {
                            allGroupVersions.Add(apiVersion.GroupVersion.Value);
                        }
                    }
                }
                allGroupVersions.Sort();
            }

            foreach (var actionDescriptor in context.Results)
            {
                var implicitGroupVersioning = Options.ImplicitGroupVersioning;

                var constraints = actionDescriptor.ActionConstraints;
                var model = actionDescriptor.GetProperty<ApiVersionModel>();
                var apiVersionsByGroup = new Dictionary<DateTime, List<ApiVersion>>();

                if (model == null)
                    continue;

                foreach (var apiVersion in model.ImplementedApiVersions)
                {
                    if (apiVersion.GroupVersion == null)
                    {
                        if (apiVersion.MajorVersion != null)
                        {
                            implicitGroupVersioning = false;

                            logger.LogWarning("{Action} has an ApiVersion without group version, which is {Version}. " +
                                "This makes the endpoints undiscoverable by requrests with only group, " +
                                "and it also disables implicit group versioning.", actionDescriptor.DisplayName, apiVersion.MajorVersion);
                        }
                    }
                    else
                    {
                        if (apiVersion.MajorVersion == null)
                        {
                            logger.LogError("ApiVersion specified for {Action} contains only group and does not have version numbers.", actionDescriptor.DisplayName);
                            throw new Exception($"ApiVersion specified for {actionDescriptor.DisplayName} contains only group and does not have version numbers.");
                        }

                        if (!apiVersionsByGroup.TryGetValue(apiVersion.GroupVersion.Value, out var apiVersions))
                        {
                            apiVersions = new List<ApiVersion>();
                            apiVersionsByGroup.Add(apiVersion.GroupVersion.Value, apiVersions);
                        }
                        apiVersions.Add(apiVersion);
                    }
                }

                var invalidCollations = apiVersionsByGroup.Where(g => g.Value.Count > 1);
                if (invalidCollations.Any())
                {
                    logger.LogError("The same group version are defined multiple times for {Action}.", actionDescriptor.DisplayName);
                    throw new Exception($"The same group version are defined multiple times for {actionDescriptor.DisplayName}.");
                }

                if (apiVersionsByGroup.Count > 0)
                {
                    IEnumerable<ApiVersion> supportedVersions = null;
                    if (implicitGroupVersioning)
                    {
                        supportedVersions = allGroupVersions
                            .Select(g =>
                            {
                                for (var i = allGroupVersions.IndexOf(g); i >= 0; i--)
                                {
                                    var candidate = allGroupVersions.ElementAt(i);
                                    var version = model.SupportedApiVersions
                                        .FirstOrDefault(a => a.GroupVersion == candidate);
                                    if (version != null)
                                    {
                                        return new ApiVersion(g, version.MajorVersion.Value, version.MinorVersion.Value, version.Status);
                                    }
                                }
                                return null;
                            })
                            .Where(v => v != null);
                    }

                    supportedVersions = (supportedVersions ?? model.SupportedApiVersions)
                        .SelectMany(v => DeriveGroupVersion(v));
                    var deprecatedVersions = model.DeprecatedApiVersions
                        .SelectMany(v => DeriveGroupVersion(v));

                    actionDescriptor.SetProperty<ApiVersionModel>(
                        new ApiVersionModel(
                            model.DeclaredApiVersions,
                            supportedVersions,
                            deprecatedVersions,
                            Array.Empty<ApiVersion>(),
                            Array.Empty<ApiVersion>()));
                }
            }
        }

        public void OnProvidersExecuting(ActionDescriptorProviderContext context) { }

        IEnumerable<ApiVersion> DeriveGroupVersion(ApiVersion apiVersion)
        {
            yield return apiVersion;
            if (apiVersion.GroupVersion.HasValue)
            {
                yield return new ApiVersion(apiVersion.GroupVersion.Value);
                yield return apiVersion.MinorVersion == null
                    ? ApiVersion.Parse($"{apiVersion.MajorVersion}-{apiVersion.Status}")
                    : new ApiVersion(apiVersion.MajorVersion.Value, apiVersion.MinorVersion.Value, apiVersion.Status);
            }
        }
    }
}
