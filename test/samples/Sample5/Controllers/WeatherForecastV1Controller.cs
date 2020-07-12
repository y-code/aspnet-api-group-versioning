using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using SampleCommon.Models;
using Ver = SampleCommon.Version;
using static Sample5.Controllers.WeatherForecastV1Controller;

namespace Sample5.Controllers
{
    public class WeatherForecastV1Controller
    {
        internal const string V2020_03_21_1_0 = Ver.V2020_03_21 + ".1.0";
        internal const string V2020_05_12_1_0 = Ver.V2020_05_12 + ".1.0";
        internal const string V2020_07_05_1_1 = Ver.V2020_07_05 + ".1.1";

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
        };

        private string _controllerName;
        private readonly ILogger _logger;

        public WeatherForecastV1Controller(string controllerName, ILogger<WeatherForecastV1Controller> logger)
        {
            _controllerName = controllerName;
            _logger = logger;
        }

        protected bool TryParseDateParam(string param, out DateTime dateTime)
        {
            dateTime = DateTime.UtcNow;
            return param == null || DateTime.TryParseExact(param, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
        }

        protected IEnumerable<WeatherForecastV1> PutActionName(IEnumerable<WeatherForecastV1> data, string actionName)
            => data.Select(d =>
            {
                d.Summary = $"{_controllerName}:{actionName}";
                return d;
            });

        protected IEnumerable<WeatherForecastV1_1> PutActionName(IEnumerable<WeatherForecastV1_1> data, string actionName)
            => data.Select(d =>
            {
                d.Summary = $"{_controllerName}:{actionName}";
                return d;
            });

        protected IEnumerable<WeatherForecastV1> _GetV1(string date)
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

        public IEnumerable<WeatherForecastV1> GetV1(string date)
            => PutActionName(_GetV1(date), "GetV1(string)");

        public IEnumerable<WeatherForecastV1> GetV1()
            => PutActionName(_GetV1(null), "GetV1()");

        protected IEnumerable<WeatherForecastV1_1> _GetV1_1(string date)
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

        public IEnumerable<WeatherForecastV1_1> GetV1_1(string date)
            => PutActionName(_GetV1_1(date), "GetV1_1(string)");

        public IEnumerable<WeatherForecastV1_1> GetV1_1()
            => PutActionName(_GetV1_1(null), "GetV1_1()");
    }

    /// <summary>
    /// Controller Attr: none
    /// Base Class: none
    /// MapToApiVersion Attr: √
    /// </summary>
    [ApiVersion(V2020_03_21_1_0)]
    [ApiVersion(V2020_05_12_1_0)]
    [ApiVersion(V2020_07_05_1_1)]
    [Route("api/v{version:apiVersion}/Pattern1")]
    [OpenApiTag("API Example: Weather Forecast")]
    public class WeatherForecastV1P1Controller
    {
        WeatherForecastV1Controller _base;

        public WeatherForecastV1P1Controller(ILogger<WeatherForecastV1Controller> logger)
            => _base = new WeatherForecastV1Controller("Pattern1", logger);

        [HttpGet("{date}")]
        [MapToApiVersion(V2020_03_21_1_0)]
        [MapToApiVersion(V2020_05_12_1_0)]
        public IEnumerable<WeatherForecastV1> GetV1([FromRoute] string date)
            => _base.GetV1(date);

        // The endpoint below can be covered by the one above, having the parameter of HttpGet attribute "{date?}".
        // However, OpenAPI Specification does not support it, and neither does NSwag.
        [HttpGet]
        [MapToApiVersion(V2020_03_21_1_0)]
        [MapToApiVersion(V2020_05_12_1_0)]
        public IEnumerable<WeatherForecastV1> GetV1()
            => GetV1(null);

        [HttpGet("{date}")]
        [MapToApiVersion(V2020_07_05_1_1)]
        public IEnumerable<WeatherForecastV1_1> GetV1_1([FromRoute] string date)
            => _base.GetV1_1(date);

        // The endpoint below can be covered by the one above, having the parameter of HttpGet attribute "{date?}".
        // However, OpenAPI Specification does not support it, and neither does NSwag.
        [HttpGet]
        [MapToApiVersion(V2020_07_05_1_1)]
        public IEnumerable<WeatherForecastV1_1> GetV1_1()
            => GetV1_1(null);
    }

    /// <summary>
    /// Controller Attr: ApiControllerAttribute
    /// Base Class: none
    /// MapToApiVersion Attr: √
    /// </summary>
    [ApiController]
    [ApiVersion(V2020_03_21_1_0)]
    [ApiVersion(V2020_05_12_1_0)]
    [ApiVersion(V2020_07_05_1_1)]
    [Route("api/v{version:apiVersion}/Pattern2")]
    [OpenApiTag("API Example: Weather Forecast")]
    public class WeatherForecastV1P2Controller
    {
        WeatherForecastV1Controller _base;

        public WeatherForecastV1P2Controller(ILogger<WeatherForecastV1Controller> logger)
            => _base = new WeatherForecastV1Controller("Pattern2", logger);

        [HttpGet("{date}")]
        [MapToApiVersion(V2020_03_21_1_0)]
        [MapToApiVersion(V2020_05_12_1_0)]
        public IEnumerable<WeatherForecastV1> GetV1([FromRoute] string date)
            => _base.GetV1(date);

        [HttpGet]
        [MapToApiVersion(V2020_03_21_1_0)]
        [MapToApiVersion(V2020_05_12_1_0)]
        public IEnumerable<WeatherForecastV1> GetV1()
            => GetV1(null);

        [HttpGet("{date}")]
        [MapToApiVersion(V2020_07_05_1_1)]
        public IEnumerable<WeatherForecastV1_1> GetV1_1([FromRoute] string date)
            => _base.GetV1_1(date);

        [HttpGet]
        [MapToApiVersion(V2020_07_05_1_1)]
        public IEnumerable<WeatherForecastV1_1> GetV1_1()
            => GetV1_1(null);
    }

    /// <summary>
    /// Controller Attr: ControllerAttribute
    /// Base Class: none
    /// MapToApiVersion Attr: √
    /// </summary>
    [Controller]
    [ApiVersion(V2020_03_21_1_0)]
    [ApiVersion(V2020_05_12_1_0)]
    [ApiVersion(V2020_07_05_1_1)]
    [Route("api/v{version:apiVersion}/Pattern3")]
    [OpenApiTag("API Example: Weather Forecast")]
    public class WeatherForecastV1P3Controller
    {
        WeatherForecastV1Controller _base;

        public WeatherForecastV1P3Controller(ILogger<WeatherForecastV1Controller> logger)
            => _base = new WeatherForecastV1Controller("Pattern3", logger);

        [HttpGet("{date}")]
        [MapToApiVersion(V2020_03_21_1_0)]
        [MapToApiVersion(V2020_05_12_1_0)]
        public IEnumerable<WeatherForecastV1> GetV1([FromRoute] string date)
            => _base.GetV1(date);

        [HttpGet]
        [MapToApiVersion(V2020_03_21_1_0)]
        [MapToApiVersion(V2020_05_12_1_0)]
        public IEnumerable<WeatherForecastV1> GetV1()
            => GetV1(null);

        [HttpGet("{date}")]
        [MapToApiVersion(V2020_07_05_1_1)]
        public IEnumerable<WeatherForecastV1_1> GetV1_1([FromRoute] string date)
            => _base.GetV1_1(date);

        [HttpGet]
        [MapToApiVersion(V2020_07_05_1_1)]
        public IEnumerable<WeatherForecastV1_1> GetV1_1()
            => GetV1_1(null);
    }

    /// <summary>
    /// Controller Attr: none
    /// Base Class: ControllerBase
    /// MapToApiVersion Attr: √
    /// </summary>
    [ApiVersion(V2020_03_21_1_0)]
    [ApiVersion(V2020_05_12_1_0)]
    [ApiVersion(V2020_07_05_1_1)]
    [Route("api/v{version:apiVersion}/Pattern4")]
    [OpenApiTag("API Example: Weather Forecast")]
    public class WeatherForecastV1P4Controller : ControllerBase
    {
        WeatherForecastV1Controller _base;

        public WeatherForecastV1P4Controller(ILogger<WeatherForecastV1Controller> logger)
            => _base = new WeatherForecastV1Controller("Pattern4", logger);

        [HttpGet("{date}")]
        [MapToApiVersion(V2020_03_21_1_0)]
        [MapToApiVersion(V2020_05_12_1_0)]
        public IEnumerable<WeatherForecastV1> GetV1([FromRoute] string date)
            => _base.GetV1(date);

        [HttpGet]
        [MapToApiVersion(V2020_03_21_1_0)]
        [MapToApiVersion(V2020_05_12_1_0)]
        public IEnumerable<WeatherForecastV1> GetV1()
            => GetV1(null);

        [HttpGet("{date}")]
        [MapToApiVersion(V2020_07_05_1_1)]
        public IEnumerable<WeatherForecastV1_1> GetV1_1([FromRoute] string date)
            => _base.GetV1_1(date);

        [HttpGet]
        [MapToApiVersion(V2020_07_05_1_1)]
        public IEnumerable<WeatherForecastV1_1> GetV1_1()
            => GetV1_1(null);
    }

    /// <summary>
    /// Controller Attr: ApiControllerAttribute
    /// Base Class: ControllerBase
    /// MapToApiVersion Attr: √
    /// </summary>
    [ApiController]
    [ApiVersion(V2020_03_21_1_0)]
    [ApiVersion(V2020_05_12_1_0)]
    [ApiVersion(V2020_07_05_1_1)]
    [Route("api/v{version:apiVersion}/Pattern5")]
    [OpenApiTag("API Example: Weather Forecast")]
    public class WeatherForecastV1P5Controller : ControllerBase
    {
        WeatherForecastV1Controller _base;

        public WeatherForecastV1P5Controller(ILogger<WeatherForecastV1Controller> logger)
            => _base = new WeatherForecastV1Controller("Pattern5", logger);

        [HttpGet("{date}")]
        [MapToApiVersion(V2020_03_21_1_0)]
        [MapToApiVersion(V2020_05_12_1_0)]
        public IEnumerable<WeatherForecastV1> GetV1([FromRoute] string date)
            => _base.GetV1(date);

        [HttpGet]
        [MapToApiVersion(V2020_03_21_1_0)]
        [MapToApiVersion(V2020_05_12_1_0)]
        public IEnumerable<WeatherForecastV1> GetV1()
            => GetV1(null);

        [HttpGet("{date}")]
        [MapToApiVersion(V2020_07_05_1_1)]
        public IEnumerable<WeatherForecastV1_1> GetV1_1([FromRoute] string date)
            => _base.GetV1_1(date);

        [HttpGet]
        [MapToApiVersion(V2020_07_05_1_1)]
        public IEnumerable<WeatherForecastV1_1> GetV1_1()
            => GetV1_1(null);
    }

    /// <summary>
    /// Controller Attr: ControllerAttribute
    /// Base Class: ControllerBase
    /// MapToApiVersion Attr: √
    /// </summary>
    [Controller]
    [ApiVersion(V2020_03_21_1_0)]
    [ApiVersion(V2020_05_12_1_0)]
    [ApiVersion(V2020_07_05_1_1)]
    [Route("api/v{version:apiVersion}/Pattern6")]
    [OpenApiTag("API Example: Weather Forecast")]
    public class WeatherForecastV1P6Controller : ControllerBase
    {
        WeatherForecastV1Controller _base;

        public WeatherForecastV1P6Controller(ILogger<WeatherForecastV1Controller> logger)
            => _base = new WeatherForecastV1Controller("Pattern6", logger);

        [HttpGet("{date}")]
        [MapToApiVersion(V2020_03_21_1_0)]
        [MapToApiVersion(V2020_05_12_1_0)]
        public IEnumerable<WeatherForecastV1> GetV1([FromRoute] string date)
            => _base.GetV1(date);

        [HttpGet]
        [MapToApiVersion(V2020_03_21_1_0)]
        [MapToApiVersion(V2020_05_12_1_0)]
        public IEnumerable<WeatherForecastV1> GetV1()
            => GetV1(null);

        [HttpGet("{date}")]
        [MapToApiVersion(V2020_07_05_1_1)]
        public IEnumerable<WeatherForecastV1_1> GetV1_1([FromRoute] string date)
            => _base.GetV1_1(date);

        [HttpGet]
        [MapToApiVersion(V2020_07_05_1_1)]
        public IEnumerable<WeatherForecastV1_1> GetV1_1()
            => GetV1_1(null);
    }

    /// <summary>
    /// Controller Attr: none
    /// Base Class: none
    /// MapToApiVersion Attr: x
    /// </summary>
    [ApiVersion(V2020_03_21_1_0)]
    [Route("api/v{version:apiVersion}/Pattern11")]
    [OpenApiTag("API Example: Weather Forecast")]
    public class WeatherForecastV1P11Controller
    {
        WeatherForecastV1Controller _base;

        public WeatherForecastV1P11Controller(ILogger<WeatherForecastV1Controller> logger)
            => _base = new WeatherForecastV1Controller("Pattern11", logger);

        [HttpGet("{date}")]
        public IEnumerable<WeatherForecastV1> GetV1([FromRoute] string date)
            => _base.GetV1(date);

        // The endpoint below can be covered by the one above, having the parameter of HttpGet attribute "{date?}".
        // However, OpenAPI Specification does not support it, and neither does NSwag.
        [HttpGet]
        public IEnumerable<WeatherForecastV1> GetV1()
            => GetV1(null);
    }

    [ApiVersion(V2020_07_05_1_1)]
    [Route("api/v{version:apiVersion}/Pattern11")]
    [OpenApiTag("API Example: Weather Forecast")]
    public class WeatherForecastV1_1P11Controller
    {
        WeatherForecastV1Controller _base;

        public WeatherForecastV1_1P11Controller(ILogger<WeatherForecastV1Controller> logger)
            => _base = new WeatherForecastV1Controller("Pattern11", logger);

        [HttpGet("{date}")]
        public IEnumerable<WeatherForecastV1_1> GetV1_1([FromRoute] string date)
            => _base.GetV1_1(date);

        [HttpGet]
        public IEnumerable<WeatherForecastV1_1> GetV1_1()
            => GetV1_1(null);
    }

    /// <summary>
    /// Controller Attr: ApiControllerAttribute
    /// Base Class: none
    /// MapToApiVersion Attr: x
    /// </summary>
    [ApiController]
    [ApiVersion(V2020_03_21_1_0)]
    [Route("api/v{version:apiVersion}/Pattern12")]
    [OpenApiTag("API Example: Weather Forecast")]
    public class WeatherForecastV1P12Controller
    {
        WeatherForecastV1Controller _base;

        public WeatherForecastV1P12Controller(ILogger<WeatherForecastV1Controller> logger)
            => _base = new WeatherForecastV1Controller("Pattern12", logger);

        [HttpGet("{date}")]
        public IEnumerable<WeatherForecastV1> GetV1([FromRoute] string date)
            => _base.GetV1(date);

        // The endpoint below can be covered by the one above, having the parameter of HttpGet attribute "{date?}".
        // However, OpenAPI Specification does not support it, and neither does NSwag.
        [HttpGet]
        public IEnumerable<WeatherForecastV1> GetV1()
            => GetV1(null);
    }

    [ApiController]
    [ApiVersion(V2020_07_05_1_1)]
    [Route("api/v{version:apiVersion}/Pattern12")]
    [OpenApiTag("API Example: Weather Forecast")]
    public class WeatherForecastV1_1P12Controller
    {
        WeatherForecastV1Controller _base;

        public WeatherForecastV1_1P12Controller(ILogger<WeatherForecastV1Controller> logger)
            => _base = new WeatherForecastV1Controller("Pattern12", logger);

        [HttpGet("{date}")]
        public IEnumerable<WeatherForecastV1_1> GetV1_1([FromRoute] string date)
            => _base.GetV1_1(date);

        [HttpGet]
        public IEnumerable<WeatherForecastV1_1> GetV1_1()
            => GetV1_1(null);
    }

    /// <summary>
    /// Controller Attr: ControllerAttribute
    /// Base Class: none
    /// MapToApiVersion Attr: x
    /// </summary>
    [Controller]
    [ApiVersion(V2020_03_21_1_0)]
    [Route("api/v{version:apiVersion}/Pattern13")]
    [OpenApiTag("API Example: Weather Forecast")]
    public class WeatherForecastV1P13Controller
    {
        WeatherForecastV1Controller _base;

        public WeatherForecastV1P13Controller(ILogger<WeatherForecastV1Controller> logger)
            => _base = new WeatherForecastV1Controller("Pattern13", logger);

        [HttpGet("{date}")]
        public IEnumerable<WeatherForecastV1> GetV1([FromRoute] string date)
            => _base.GetV1(date);

        // The endpoint below can be covered by the one above, having the parameter of HttpGet attribute "{date?}".
        // However, OpenAPI Specification does not support it, and neither does NSwag.
        [HttpGet]
        public IEnumerable<WeatherForecastV1> GetV1()
            => GetV1(null);
    }

    [Controller]
    [ApiVersion(V2020_07_05_1_1)]
    [Route("api/v{version:apiVersion}/Pattern13")]
    [OpenApiTag("API Example: Weather Forecast")]
    public class WeatherForecastV1_1P13Controller
    {
        WeatherForecastV1Controller _base;

        public WeatherForecastV1_1P13Controller(ILogger<WeatherForecastV1Controller> logger)
            => _base = new WeatherForecastV1Controller("Pattern13", logger);

        [HttpGet("{date}")]
        public IEnumerable<WeatherForecastV1_1> GetV1_1([FromRoute] string date)
            => _base.GetV1_1(date);

        [HttpGet]
        public IEnumerable<WeatherForecastV1_1> GetV1_1()
            => GetV1_1(null);
    }

    /// <summary>
    /// Controller Attr: none
    /// Base Class: ControllerBase
    /// MapToApiVersion Attr: x
    /// </summary>
    [ApiVersion(V2020_03_21_1_0)]
    [Route("api/v{version:apiVersion}/Pattern14")]
    [OpenApiTag("API Example: Weather Forecast")]
    public class WeatherForecastV1P14Controller : ControllerBase
    {
        WeatherForecastV1Controller _base;

        public WeatherForecastV1P14Controller(ILogger<WeatherForecastV1Controller> logger)
            => _base = new WeatherForecastV1Controller("Pattern14", logger);

        [HttpGet("{date}")]
        public IEnumerable<WeatherForecastV1> GetV1([FromRoute] string date)
            => _base.GetV1(date);

        // The endpoint below can be covered by the one above, having the parameter of HttpGet attribute "{date?}".
        // However, OpenAPI Specification does not support it, and neither does NSwag.
        [HttpGet]
        public IEnumerable<WeatherForecastV1> GetV1()
            => GetV1(null);
    }

    [ApiVersion(V2020_07_05_1_1)]
    [Route("api/v{version:apiVersion}/Pattern14")]
    [OpenApiTag("API Example: Weather Forecast")]
    public class WeatherForecastV1_1P14Controller : ControllerBase
    {
        WeatherForecastV1Controller _base;

        public WeatherForecastV1_1P14Controller(ILogger<WeatherForecastV1Controller> logger)
            => _base = new WeatherForecastV1Controller("Pattern14", logger);

        [HttpGet("{date}")]
        public IEnumerable<WeatherForecastV1_1> GetV1_1([FromRoute] string date)
            => _base.GetV1_1(date);

        [HttpGet]
        public IEnumerable<WeatherForecastV1_1> GetV1_1()
            => GetV1_1(null);
    }

    /// <summary>
    /// Controller Attr: ApiControllerAttribute
    /// Base Class: ControllerBase
    /// MapToApiVersion Attr: x
    /// </summary>
    [ApiController]
    [ApiVersion(V2020_03_21_1_0)]
    [Route("api/v{version:apiVersion}/Pattern15")]
    [OpenApiTag("API Example: Weather Forecast")]
    public class WeatherForecastV1P15Controller : ControllerBase
    {
        WeatherForecastV1Controller _base;

        public WeatherForecastV1P15Controller(ILogger<WeatherForecastV1Controller> logger)
            => _base = new WeatherForecastV1Controller("Pattern15", logger);

        [HttpGet("{date}")]
        public IEnumerable<WeatherForecastV1> GetV1([FromRoute] string date)
            => _base.GetV1(date);

        // The endpoint below can be covered by the one above, having the parameter of HttpGet attribute "{date?}".
        // However, OpenAPI Specification does not support it, and neither does NSwag.
        [HttpGet]
        public IEnumerable<WeatherForecastV1> GetV1()
            => GetV1(null);
    }

    [ApiController]
    [ApiVersion(V2020_07_05_1_1)]
    [Route("api/v{version:apiVersion}/Pattern15")]
    [OpenApiTag("API Example: Weather Forecast")]
    public class WeatherForecastV1_1P15Controller : ControllerBase
    {
        WeatherForecastV1Controller _base;

        public WeatherForecastV1_1P15Controller(ILogger<WeatherForecastV1Controller> logger)
            => _base = new WeatherForecastV1Controller("Pattern15", logger);

        [HttpGet("{date}")]
        public IEnumerable<WeatherForecastV1_1> GetV1_1([FromRoute] string date)
            => _base.GetV1_1(date);

        [HttpGet]
        public IEnumerable<WeatherForecastV1_1> GetV1_1()
            => GetV1_1(null);
    }

    /// <summary>
    /// Controller Attr: ControllerAttribute
    /// Base Class: ControllerBase
    /// MapToApiVersion Attr: x
    /// </summary>
    [Controller]
    [ApiVersion(V2020_03_21_1_0)]
    [Route("api/v{version:apiVersion}/Pattern16")]
    [OpenApiTag("API Example: Weather Forecast")]
    public class WeatherForecastV1P16Controller : ControllerBase
    {
        WeatherForecastV1Controller _base;

        public WeatherForecastV1P16Controller(ILogger<WeatherForecastV1Controller> logger)
            => _base = new WeatherForecastV1Controller("Pattern16", logger);

        [HttpGet("{date}")]
        public IEnumerable<WeatherForecastV1> GetV1([FromRoute] string date)
            => _base.GetV1(date);

        // The endpoint below can be covered by the one above, having the parameter of HttpGet attribute "{date?}".
        // However, OpenAPI Specification does not support it, and neither does NSwag.
        [HttpGet]
        public IEnumerable<WeatherForecastV1> GetV1()
            => GetV1(null);
    }

    [Controller]
    [ApiVersion(V2020_07_05_1_1)]
    [Route("api/v{version:apiVersion}/Pattern16")]
    [OpenApiTag("API Example: Weather Forecast")]
    public class WeatherForecastV1_1P16Controller : ControllerBase
    {
        WeatherForecastV1Controller _base;

        public WeatherForecastV1_1P16Controller(ILogger<WeatherForecastV1Controller> logger)
            => _base = new WeatherForecastV1Controller("Pattern16", logger);

        [HttpGet("{date}")]
        public IEnumerable<WeatherForecastV1_1> GetV1_1([FromRoute] string date)
            => _base.GetV1_1(date);

        [HttpGet]
        public IEnumerable<WeatherForecastV1_1> GetV1_1()
            => GetV1_1(null);
    }
}
