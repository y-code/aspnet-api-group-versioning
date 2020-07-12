using Microsoft.AspNetCore.Mvc;

namespace CodeSample.Controllers
{
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
}
