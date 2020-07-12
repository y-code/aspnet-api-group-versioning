using Microsoft.AspNetCore.Mvc;
using CodeSample.Models;

namespace CodeSample.Controllers
{
    [ApiController]
    [ApiVersion("2019-05-03.1.0")]
    [ApiVersion("2019-11-02.1.1")]
    [ApiVersion("2020-02-14.1.2")]
    [Route("api/v{version:apiVersion}/endpoint-a")]
    public class EndpointAV1Controller : ControllerBase
    {
        [HttpGet]
        [MapToApiVersion("2019-05-03.1.0")]
        public string GetV1()
        {
            return $"Received a requrest to Endpoint A v1.0";
        }

        [HttpGet]
        [MapToApiVersion("2019-11-02.1.1")]
        public string GetV1_1()
        {
            return $"Received a requrest to Endpoint A v1.1";
        }

        [HttpGet]
        [MapToApiVersion("2020-02-14.1.2")]
        public string GetV1_2()
        {
            return $"Received a requrest to Endpoint A v1.2";
        }
    }
}
