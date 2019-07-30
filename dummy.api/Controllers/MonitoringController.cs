using dummy.api.Infrastructure.ActionFilters;
using Microsoft.AspNetCore.Mvc;

namespace dummy.api.Controllers
{
    [ApiController]
    [ApiVersionNeutral]
    [IgnoreCollectPrometheusMetrics]
    [Route("monitoring")]
    public class MonitoringController : ControllerBase
    {
        [HttpGet]
        [Route("readiness")]
        public IActionResult Readiness()
        {
            return this.Ok();
        }

        [HttpGet]
        [Route("health")]
        public IActionResult Health()
        {
            return this.Ok();
        }
    }
}