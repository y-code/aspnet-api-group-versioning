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
    [ApiVersion(V2020_11_09_2_0)]
    //[ApiVersion(V2020_12_02_2_0)]
    [Route("api/v{version:apiVersion}/WeatherForecast")]
    [OpenApiTag("API Example: Weather Forecast")]
    public partial class WeatherForecastV2Controller : ControllerBase
    {
        internal const string V2020_11_09_2_0 = Ver.V2020_11_09 + ".2.0";
        //internal const string V2020_12_02_2_0 = Ver.V2020_12_02 + ".2.0";

        private static readonly string[] Areas = new[]
        {
            "New York", "Tokyo", "Christchurch",
        };

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
        };

        private readonly ILogger _logger;

        public WeatherForecastV2Controller(ILogger<WeatherForecastV2Controller> logger)
        {
            _logger = logger;
        }

        [HttpGet("Area/{area}")]
        public IEnumerable<WeatherForecastV2> GetWeatherForecast(
            [FromRoute] string area,
            [FromQuery] string from,
            [FromQuery] int? days = null)
        {
            if (area != null && !Areas.Contains(area))
            {
                throw new ArgumentException($"'{area}' is not supported area.", "area");
            }

            DateTime baseDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
            if (from != null
                && !DateTime.TryParseExact(
                    from,
                    "yyyyMMdd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out baseDate))
            {
                throw new ArgumentException($"'{from}' should be in yyyyMMdd format.", "from");
            }

            if (days == null)
            {
                days = 5;
            }

            var areas = Areas.Where(a => area == null || a == area);

            var random = new Random();
            return Enumerable.Range(0, days.Value)
                .Select(index => areas.Select(a => new WeatherForecastV2
                {
                    Area = a,
                    Date = baseDate.AddDays(index),
                    TemperatureC = random.Next(-20, 55),
                    Summary = Summaries[random.Next(Summaries.Length)],
                }))
                .SelectMany(w => w);
        }

        // The endpoint below can be covered by the one above, having the parameter of HttpGet attribute "{date?}".
        // However, OpenAPI Specification does not support it, and neither does NSwag.
        [HttpGet("Area")]
        [HttpGet]
        public IEnumerable<WeatherForecastV2> GetWeatherForecast(
            [FromQuery] string from,
            [FromQuery] int? days)
            => GetWeatherForecast(null, from, days);
    }
}
