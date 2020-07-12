using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using SampleCommon.Models;
using Ver = SampleCommon.Version;

namespace Sample4.Controllers
{
    [ApiController]
    [ApiVersion(V2020_03_21_1_0)]
    //[ApiVersion(V2020_05_12_1_0)]
    [ApiVersion(V2020_07_05_1_1)]
    [Route("api/v{version:apiVersion}/WeatherForecast")]
    [OpenApiTag("API Example: Weather Forecast")]
    public partial class WeatherForecastV1Controller : ControllerBase
    {
        internal const string V2020_03_21_1_0 = Ver.V2020_03_21 + ".1.0";
        //internal const string V2020_05_12_1_0 = Version.V2020_05_12 + ".1.0";
        internal const string V2020_07_05_1_1 = Ver.V2020_07_05 + ".1.1";

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
        };

        private readonly ILogger _logger;

        public WeatherForecastV1Controller(ILogger<WeatherForecastV1Controller> logger)
        {
            _logger = logger;
        }

        private bool TryParseDateParam(string param, out DateTime dateTime)
        {
            dateTime = DateTime.UtcNow;
            return param == null || DateTime.TryParseExact(param, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
        }

        [HttpGet("{date}")]
        [MapToApiVersion(V2020_03_21_1_0)]
        //[MapToApiVersion(V2020_05_12_1_0)]
        public IEnumerable<WeatherForecastV1> GetV1([FromRoute] string date)
        {
            if (!TryParseDateParam(date, out var baseDate))
            {
                throw new ArgumentException($"'{date}' in URL should be in yyyyMMdd format.", "date");
            }

            var random = new Random();
            return Enumerable.Range(0, 5)
                .Select(index => new WeatherForecastV1
                {
                    Date = baseDate.AddDays(index).ToString("d MMM, yyyy"),
                    Summary = Summaries[random.Next(Summaries.Length)],
                });
        }

        // The endpoint below can be covered by the one above, having the parameter of HttpGet attribute "{date?}".
        // However, OpenAPI Specification does not support it, and neither does NSwag.
        [HttpGet]
        [MapToApiVersion(V2020_03_21_1_0)]
        //[MapToApiVersion(V2020_05_12_1_0)]
        public IEnumerable<WeatherForecastV1> GetV1()
            => GetV1(null);

        [HttpGet("{date}")]
        [MapToApiVersion(V2020_07_05_1_1)]
        public IEnumerable<WeatherForecastV1_1> GetV1_1([FromRoute] string date)
        {
            if (!TryParseDateParam(date, out var baseDate))
            {
                throw new ArgumentException($"'{date}' in URL should be in yyyyMMdd format.", "date");
            }

            var random = new Random();
            return Enumerable.Range(0, 5)
                .Select(index => new WeatherForecastV1_1
                {
                    Date = baseDate.AddDays(index).ToString("d MMM, yyyy"),
                    Summary = Summaries[random.Next(Summaries.Length)],
                    TemperatureC = random.Next(-20, 55),
                });
        }

        // The endpoint below can be covered by the one above, having the parameter of HttpGet attribute "{date?}".
        // However, OpenAPI Specification does not support it, and neither does NSwag.
        [HttpGet]
        [MapToApiVersion(V2020_07_05_1_1)]
        public IEnumerable<WeatherForecastV1_1> GetV1_1()
            => GetV1_1(null);
    }
}
