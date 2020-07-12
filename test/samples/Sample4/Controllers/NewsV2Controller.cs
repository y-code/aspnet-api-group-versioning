using System;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SampleCommon.Models;
using Ver = SampleCommon.Version;

namespace Sample4.Controllers
{
    [ApiController]
    [ApiVersion(V2020_12_02_2_0)]
    [Route("api/v{version:apiVersion}/News")]
    [OpenApiTag("API Example: News")]
    public class NewsV2Controller : NewsV1Controller
    {
        internal const string V2020_12_02_2_0 = Ver.V2020_12_02 + ".2.0";

        [HttpGet("Headlines/{utcTime}")]
        public override NewsHeadlineV1[] GetHeadlines(DateTime? utcTime)
            => base.GetHeadlines(utcTime);
    }
}
