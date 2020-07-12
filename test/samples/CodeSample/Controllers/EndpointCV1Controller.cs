using Microsoft.AspNetCore.Mvc;

namespace CodeSample.Controllers
{
    [ApiController]
    [ApiVersion("2019-05-03.1.0")]
    [Route("api/v{version:apiVersion}/endpoint-c")]
    public class EndpointCV1Controller : ControllerBase
    {
        [HttpGet]
        public string GetV1()
        {
            return $"Received a requrest to EndpointC v1.0";
        }
    }
}
