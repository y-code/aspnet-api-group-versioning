using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using SampleCommon.Models;
using static Sample3.Controllers.WeatherForecastV2Controller;

namespace Sample3.Controllers
{
    public class WeatherForecastV2Controller
    {
        internal const string V2_0 = "2.0";
        internal const string V2_1 = "2.1";

        private static readonly string[] Areas = new[]
        {
            "New York", "Tokyo", "Christchurch",
        };

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
        };

        private string _controllerName;
        private readonly ILogger _logger;

        public WeatherForecastV2Controller(string controllerName, ILogger<WeatherForecastV2Controller> logger)
        {
            _controllerName = controllerName;
            _logger = logger;
        }

        protected IEnumerable<WeatherForecastV2> PutActionName(IEnumerable<WeatherForecastV2> data, string actionName)
            => data.Select(d =>
            {
                d.Summary = $"{_controllerName}:{actionName}";
                return d;
            });

        protected IEnumerable<WeatherForecastV2> _GetV2( string area, string from, int? days = null)
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

        public IEnumerable<WeatherForecastV2> GetV2(string area, string from, int? days = null)
            => PutActionName(_GetV2(area, from, days), "GetV2(string, string, int?)");

        public IEnumerable<WeatherForecastV2> GetV2(string from, int? days = null)
            => PutActionName(_GetV2(null, from, days), "GetV2(string, int?)");

        public IEnumerable<WeatherForecastV2> GetV2_1(string area, string from, int? days = null)
            => PutActionName(_GetV2(area, from, days), "GetV2_1(string, string, int?)");

        public IEnumerable<WeatherForecastV2> GetV2_1(string from, int? days = null)
            => PutActionName(_GetV2(null, from, days), "GetV2_1(string, int?)");
    }

    /// <summary>
    /// Controller Attr: none
    /// Base Class: none
    /// </summary>
    [ApiVersion(V2_0)]
    [Route("api/v{version:apiVersion}/Pattern1")]
    [OpenApiTag("API Example: Weather Forecast")]
    public partial class WeatherForecastV2P11Controller
    {
        WeatherForecastV2Controller _base;
        public WeatherForecastV2P11Controller(ILogger<WeatherForecastV2Controller> logger)
            => _base = new WeatherForecastV2Controller("Pattern1", logger);

        [HttpGet("Area/{area}")]
        public IEnumerable<WeatherForecastV2> GetV2([FromRoute] string area, [FromQuery] string from, [FromQuery] int? days = null)
            => _base.GetV2(area, from, days);

        // The endpoint below can be covered by the one above, having the parameter of HttpGet attribute "{date?}".
        // However, OpenAPI Specification does not support it, and neither does NSwag.
        [HttpGet("Area")]
        [HttpGet]
        public IEnumerable<WeatherForecastV2> GetV2([FromQuery] string from, [FromQuery] int? days)
            => _base.GetV2(from, days);
    }

    /// <summary>
    /// Controller Attr: ApiControllerAttribute
    /// Base Class: none
    /// </summary>
    [ApiController]
    [ApiVersion(V2_0)]
    [Route("api/v{version:apiVersion}/Pattern2")]
    [OpenApiTag("API Example: Weather Forecast")]
    public partial class WeatherForecastV2P2Controller
    {
        WeatherForecastV2Controller _base;
        public WeatherForecastV2P2Controller(ILogger<WeatherForecastV2Controller> logger)
            => _base = new WeatherForecastV2Controller("Pattern2", logger);

        [HttpGet("Area/{area}")]
        public IEnumerable<WeatherForecastV2> GetWeatherForecast([FromRoute] string area, [FromQuery] string from, [FromQuery] int? days = null)
            => _base.GetV2(area, from, days);

        [HttpGet("Area")]
        [HttpGet]
        public IEnumerable<WeatherForecastV2> GetWeatherForecast([FromQuery] string from, [FromQuery] int? days)
            => _base.GetV2(from, days);
    }

    /// <summary>
    /// Controller Attr: ControllerAttribute
    /// Base Class: none
    /// </summary>
    [Controller]
    [ApiVersion(V2_0)]
    [Route("api/v{version:apiVersion}/Pattern3")]
    [OpenApiTag("API Example: Weather Forecast")]
    public partial class WeatherForecastV2P3Controller
    {
        WeatherForecastV2Controller _base;
        public WeatherForecastV2P3Controller(ILogger<WeatherForecastV2Controller> logger)
            => _base = new WeatherForecastV2Controller("Pattern3", logger);

        [HttpGet("Area/{area}")]
        public IEnumerable<WeatherForecastV2> GetWeatherForecast([FromRoute] string area, [FromQuery] string from, [FromQuery] int? days = null)
            => _base.GetV2(area, from, days);

        [HttpGet("Area")]
        [HttpGet]
        public IEnumerable<WeatherForecastV2> GetWeatherForecast([FromQuery] string from, [FromQuery] int? days)
            => _base.GetV2(from, days);
    }

    /// <summary>
    /// Controller Attr: none
    /// Base Class: ControllerBase
    /// </summary>
    [ApiVersion(V2_0)]
    [Route("api/v{version:apiVersion}/Pattern4")]
    [OpenApiTag("API Example: Weather Forecast")]
    public partial class WeatherForecastV2P4Controller : ControllerBase
    {
        WeatherForecastV2Controller _base;
        public WeatherForecastV2P4Controller(ILogger<WeatherForecastV2Controller> logger)
            => _base = new WeatherForecastV2Controller("Pattern4", logger);

        [HttpGet("Area/{area}")]
        public IEnumerable<WeatherForecastV2> GetWeatherForecast([FromRoute] string area, [FromQuery] string from, [FromQuery] int? days = null)
            => _base.GetV2(area, from, days);

        [HttpGet("Area")]
        [HttpGet]
        public IEnumerable<WeatherForecastV2> GetWeatherForecast([FromQuery] string from, [FromQuery] int? days)
            => _base.GetV2(from, days);
    }

    /// <summary>
    /// Controller Attr: ApiControllerAttribute
    /// Base Class: ControllerBase
    /// </summary>
    [ApiController]
    [ApiVersion(V2_0)]
    [Route("api/v{version:apiVersion}/Pattern5")]
    [OpenApiTag("API Example: Weather Forecast")]
    public partial class WeatherForecastV2P5Controller : ControllerBase
    {
        WeatherForecastV2Controller _base;
        public WeatherForecastV2P5Controller(ILogger<WeatherForecastV2Controller> logger)
            => _base = new WeatherForecastV2Controller("Pattern5", logger);

        [HttpGet("Area/{area}")]
        public IEnumerable<WeatherForecastV2> GetWeatherForecast([FromRoute] string area, [FromQuery] string from, [FromQuery] int? days = null)
            => _base.GetV2(area, from, days);

        [HttpGet("Area")]
        [HttpGet]
        public IEnumerable<WeatherForecastV2> GetWeatherForecast([FromQuery] string from, [FromQuery] int? days)
            => _base.GetV2(from, days);
    }

    /// <summary>
    /// Controller Attr: ControllerAttribute
    /// Base Class: ControllerBase
    /// </summary>
    [Controller]
    [ApiVersion(V2_0)]
    [Route("api/v{version:apiVersion}/Pattern6")]
    [OpenApiTag("API Example: Weather Forecast")]
    public partial class WeatherForecastV2P6Controller : ControllerBase
    {
        WeatherForecastV2Controller _base;
        public WeatherForecastV2P6Controller(ILogger<WeatherForecastV2Controller> logger)
            => _base = new WeatherForecastV2Controller("Pattern6", logger);

        [HttpGet("Area/{area}")]
        public IEnumerable<WeatherForecastV2> GetWeatherForecast([FromRoute] string area, [FromQuery] string from, [FromQuery] int? days = null)
            => _base.GetV2(area, from, days);

        [HttpGet("Area")]
        [HttpGet]
        public IEnumerable<WeatherForecastV2> GetWeatherForecast([FromQuery] string from, [FromQuery] int? days)
            => _base.GetV2(from, days);
    }
}
