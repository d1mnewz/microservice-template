using Microsoft.AspNetCore.Mvc.Filters;

namespace dummy.api.Infrastructure.ActionFilters
{
    /// <summary>
    /// Attribute to ignore collecting Prometheus metrics.
    /// </summary>
    public class IgnoreCollectPrometheusMetrics : ActionFilterAttribute
    {
    }
}