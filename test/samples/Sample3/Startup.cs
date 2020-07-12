using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample3.Controllers;

namespace Sample3
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
            });

            var useGroupVersioning = Configuration.GetValue<bool>("useGroupVersioning", true);
            if (useGroupVersioning)
            {
                services.AddApiGroupVersioning(options =>
                {
                });
            }

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'ver. 'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            foreach (var version in new[]
                {
                    WeatherForecastV1Controller.V1_0,
                    WeatherForecastV1Controller.V1_1,
                    WeatherForecastV2Controller.V2_0,
                })
            {
                services.AddOpenApiDocument(settings =>
                {
                    settings.DocumentName = $"v{version}";
                    settings.Version = version;
                    settings.GenerateExamples = false;
                    settings.UseRouteNameAsOperationId = true;
                    settings.Title = "My API";
                    settings.ApiGroupNames = new[] { $"ver. {version}" };
                });
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/api/error/dev");
            }
            else
            {
                app.UseExceptionHandler("/api/error");
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}
