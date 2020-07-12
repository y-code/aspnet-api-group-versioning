using Microsoft.AspNetCore.Mvc;

namespace CodeSample.Controllers
{
    [ApiController]
    [ApiVersion("2020-06-07.2.0")]
    [Route("api/v{version:apiVersion}/endpoint-b")]
    public class EndpointBV2Controller : ControllerBase
    {
        [HttpGet]
        public string GetV2()
        {
            return $"Received a requrest to Endpoint B v2.0";
        }
    }
}
