# ASP.NET API Group Versioning

ASP.NET API Group Versioning adds group versioning functionality to ASP.NET API Versioning. It allows API users to request only with group version. It also allows you to increment endpoint versions only when the feature changes.

## The problem to solve

When we have a web API and are versioning the entire API in each version, we need to bump up version number even when releasing changes only for a few endpoints while all the others are the same. Let's say you provide an API with Endpoint A and B and the current API version is 1.0. When releasing a change in the response data of Endpoint B, you need to increment the API version to 1.1. In this case, the Endpoint A in version 1.0 and the one in version 1.1 are the same, but we still need to add a certain amount of code for it for the sake of versioning. Although the amount depends on how you implement and organize source code in terms of the versioning.

If that kind of code became such amount that is painful from the maintenance point of view, it would turn you off to have small releases. If big bang releases do not bother you, it won't problem you, although possibly problems your API users. If you are in agile development, I'm sure it is a problem.

## This solution

API Group Versioning allows you to have group versions as API version, and to use major-minor versions for each endpoint. In this way, Endpoint A, in the previous example, does not need to have version 1.1.

The following diagram describes how we can version API and endpoints. The lanes represent endpoint version each, and group version 2019-11-02 represents Endpoint A v1.1, Endpoint B v1.1, and Endpoint C v1.0. When you release Endpoint A v1.2, you release API v2020-02-14, which also represents Endpoint B v1.1 and Endpoint C v1.0.

API users can send a request to an endpoint with a group version. API Group Versioning will route the request to the action versioned with the corresponding major-minor version. For example, the request to Endpoint A with API version 2020-02-14 is routed to Endpoint A v1.2, while the request to Endpoint B with the same API version goes to Endpoint B v1.1.

![Group Version](https://github.com/y-code/aspnet-api-group-versioning/raw/master/doc/images/group_versioning.png)

## Code Sample

The code below is a code sample of the controllers with the API Group Versioning, which versioning is equivalent to Endpoint B in the previous example.

```
[ApiController]
[ApiVersion("2019-05-03.1.0")]
[ApiVersion("2019-09-22.1.1")]
[Route("api/v{version:apiVersion}/endpoint-b")]
public class EndpointBV1Controller : ControllerBase
{
    [HttpGet]
    [MapToApiVersion("2019-05-03.1.0")]
    public string GetV1()
    {
        return $"Received a requrest to Endpoint B v1.0";
    }

    [HttpGet]
    [MapToApiVersion("2019-09-22.1.1")]
    public string GetV1_1()
    {
        return $"Received a requrest to Endpoint B v1.1";
    }
}

[ApiController]
[ApiVersion("2020-06-07.2.0")]
[Route("api/v{version:apiVersion}/endpoint-b")]
public class EndpointBV2Controller : ControllerBase
{
    [HttpGet]
    public string GetV2()
    {
        return $"Received a requrest to Endpoint B v2.0";
    }
}
```

## Set-up

To use ApiGroupVersioning in your ASP.NET Core application, add `Ycode.AspNetCore.Mvc.GroupVersioning` NuGet package to your project, and call `AddApiGroupVersioning` extension method inside the call to 'ConfigureServices(...)' in the application start-up code, as well as depending functionalities.

The following code shows the minimum code required in the application start-up code.

```
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddApiVersioning(options => { });
        services.AddApiGroupVersioning(options => { });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action=Index}/{id?}");
        });
    }
}
```

## Design notes

### Versioning formats

ASP.NET API Group Versioning validates versions based on the definition in [Microsoft REST API Guidelines - 12 Versioning](https://github.com/Microsoft/api-guidelines/blob/master/Guidelines.md#12-versioning)

### Invalid versioning

The guideline document contains a versioning pattern that the same *group version* is associated with multiple *major.minor versions* over the same endpoint. However, note that ASP.NET API Group Versioning will never support it. Such pattern was previously discussed in [GitHub microsoft/aspnet-api-versioning](https://github.com/microsoft/aspnet-api-versioning/issues/334), and ASP.NET API Group Versioning takes the same stance as @commonsensesoftware explained there.
