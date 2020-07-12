using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sample1
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
                options.GroupNameFormat = "'ver. 'V";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddOpenApiDocument(settings =>
            {
                settings.DocumentName = "v1";
                settings.Version = "1.X";
                settings.GenerateExamples = false;
                settings.UseRouteNameAsOperationId = true;
                settings.Title = "My API";
                settings.ApiGroupNames = new[] { "ver. 1" };
            });
            services.AddOpenApiDocument(settings =>
            {
                settings.DocumentName = "v2";
                settings.Version = "2.X";
                settings.Title = "Example API";
                settings.ApiGroupNames = new[] { "ver. 2" };
            });
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
