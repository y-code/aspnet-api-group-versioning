using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sample1;

namespace Ycode.AspNetCore.Mvc.GroupVersioning.Test
{
    public enum RunEnvironment
    {
        Development,
        Release,
    }

    public class TestBed<T> : IDisposable where T : new()
    {
        public const int ApiServiceStartUpTimeoutSeconds = 7;

        ILogger _logger;
        IReadOnlyDictionary<string, string> _envVars;
        string[] _args;

        RunEnvironment RunEnvironment { get; }
        public string ApiServiceBaseUrl { get; }
        public Task ApiService;
        public IHost Host { get; private set; }

        public TestBed(RunEnvironment runEnvironment, string apiServiceBaseUrl, IDictionary<string, string> envVars, string[] args, ILoggerFactory loggerFactory)
        {
            ApiServiceBaseUrl = apiServiceBaseUrl;
            RunEnvironment = runEnvironment;
            _envVars = new ReadOnlyDictionary<string, string>(envVars);
            _args = args;
            _logger = loggerFactory.CreateLogger(GetType());
        }

        public async Task Start()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", RunEnvironment switch
            {
                RunEnvironment.Development => "Development",
                RunEnvironment.Release => "Release",
                _ => "",
            });
            Environment.SetEnvironmentVariable("ASPNETCORE_URLS", ApiServiceBaseUrl);
            foreach (var envVar in _envVars)
            {
                Environment.SetEnvironmentVariable(envVar.Key, envVar.Value);
            }

            ApiService = Task.Run(async () =>
            {
                var program = new T();
                var createHostBuilder = program.GetType().GetMethod("CreateHostBuilder");
                if (createHostBuilder == null)
                {
                    throw new Exception("Test target does not contain a method named \"CreateHostBuilder\".");
                }
                if (createHostBuilder.ReturnType != typeof(IHostBuilder))
                {
                    throw new Exception("Test target contains CreateHostBuilder method, but this return type is not \"IHostBuilder\".");
                }
                var hostBuilder = (IHostBuilder)createHostBuilder.Invoke(program, new object[] { _args });
                Host = hostBuilder.Build();
                await Host.RunAsync();
            });

            Stream stream = null;
            DateTime start = DateTime.UtcNow;
            DateTime timeout = DateTime.UtcNow + TimeSpan.FromSeconds(ApiServiceStartUpTimeoutSeconds);
            Exception exception = null;
            while (true)
            {
                try
                {
                    await Task.Delay(500);

                    _logger.LogDebug("Checking service startup...");

                    if (ApiService.Exception != null)
                    {
                        throw new Exception("An exception was thrown during starting the test target service.", ApiService.Exception);
                    }

                    var client = new WebClient();
                    using (stream = await client.OpenReadTaskAsync(new Uri(ApiServiceBaseUrl))) { }
                    var wakeup = DateTime.UtcNow - start;
                    _logger.LogInformation("API service started successfully.");

                    break;
                }
                catch (Exception e)
                {
                    exception = e;
                    if (e is WebException)
                    {
                        if (((WebException)e).Status != WebExceptionStatus.UnknownError)
                        {
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (DateTime.UtcNow > timeout)
                    {
                        var error = $"Test target API service did not start up within {ApiServiceStartUpTimeoutSeconds} seconds";
                        _logger.LogError(error);
                        throw new Exception(error, exception);
                    }

                    throw;
                }
            }
        }

        public void Dispose()
        {
            Host?.Dispose();
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "");
            Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "");
        }
    }
}
