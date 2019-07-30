using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Prometheus;

namespace dummy.api.Infrastructure.ActionFilters
{
    /// <summary>
    /// Enables Prometheus metrics to be collected.
    /// Metrics collected: {serviceName}_service_requests, {serviceName}_service_requests_duration_ms.
    /// </summary>
    public class CollectPrometheusMetrics : ActionFilterAttribute
    {
        private static string serviceName => "dummy";

        private static readonly string ServiceRequestsMetricName = $"{serviceName}_service_requests";

        private static readonly string ServiceRequestsDurationMsMetricName =
            $"{serviceName}_service_requests_duration_ms";

        private AsyncLocal<Stopwatch> sw;

        private static Counter ServiceRequestsMetric { get; set; }

        private static Histogram ServiceRequestsDurationMsMetric { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectPrometheusMetrics"/> class with default set of metrics to track:
        /// {serviceName}_service_requests and {serviceName}_service_requests_duration_ms
        /// with default set of labels:
        /// http_verb, status_code, status_code_group, url_template.
        /// </summary>
        public CollectPrometheusMetrics()
        {
            this.sw = new AsyncLocal<Stopwatch>();
            ServiceRequestsMetric =
                Metrics.CreateCounter(
                    ServiceRequestsMetricName,
                    $"Query counter for {serviceName} service",
                    new CounterConfiguration
                        { LabelNames = new[] { "http_verb", "status_code", "status_code_group", "url_template", } });
            ServiceRequestsDurationMsMetric = Metrics.CreateHistogram(
                ServiceRequestsDurationMsMetricName,
                $"Time it takes to process a request on {serviceName} service.",
                new HistogramConfiguration
                    { LabelNames = new[] { "http_verb", "status_code", "status_code_group", "url_template", } });
        }

        /// <summary>
        /// Happens before action is executed.
        /// Starts a stopwatch to track latency.
        /// </summary>
        /// <param name="context">Action Executing context.</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionDescriptor.FilterDescriptors.Any(x => x.Filter is IgnoreCollectPrometheusMetrics))
            {
                return;
            }

            this.sw.Value = Stopwatch.StartNew();
            base.OnActionExecuting(context);
        }

        /// <summary>
        /// Happens when action is executed.
        /// Increments {serviceName}_service_requests and observes {serviceName}_service_requests_duration_ms with corresponding labels:
        /// http_verb, status_code, status_code_group, url_template.
        /// </summary>
        /// <param name="context">Action Executed context.</param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.ActionDescriptor.FilterDescriptors.Any(x => x.Filter is IgnoreCollectPrometheusMetrics))
            {
                return;
            }

            this.sw.Value.Stop();
            var responseStatusCode = GetResponseStatusCode(context);
            var responseStatusCodeGroup = GetResponseStatusCodeGroup(responseStatusCode);
            var urlTemplatePath = GetUrlTemplatePath(context);
            var requestMethod = GetRequestMethod(context);

            this.ObserveLatency(requestMethod, responseStatusCode.ToString(), responseStatusCodeGroup, urlTemplatePath);

            IncrementRequestCounter(
                requestMethod,
                responseStatusCode.ToString(),
                responseStatusCodeGroup,
                urlTemplatePath);

            base.OnActionExecuted(context);
        }

        /// <summary>
        /// Increments request counter with labels.
        /// </summary>
        /// <param name="requestMethod">HTTP method. Corresponds to http_verb label.</param>
        /// <param name="responseStatusCode">Response status code in format 200, 404, 500. Corresponds to status_code label.</param>
        /// <param name="responseStatusCodeGroup">Response status code group in format 2xx, 3xx, 4xx,5xx. Corresponds to status_code_group label.</param>
        /// <param name="urlTemplatePath">URL template path in format api/v{version:apiVersion}/Values/... Corresponds to url_template label.</param>
        private static void IncrementRequestCounter(
            string requestMethod,
            string responseStatusCode,
            string responseStatusCodeGroup,
            string urlTemplatePath)
        {
            ServiceRequestsMetric
                .WithLabels(requestMethod, responseStatusCode, responseStatusCodeGroup, urlTemplatePath).Inc();
        }

        private static string GetRequestMethod(ActionExecutedContext context)
        {
            return context.HttpContext.Request.Method;
        }

        private static string GetResponseStatusCodeGroup(int responseStatusCode)
        {
            if (responseStatusCode >= 100 && responseStatusCode < 600)
            {
                return $"{responseStatusCode / 100}xx";
            }
            else
            {
                throw new Exception("Incorrect HTTP response status code");
            }
        }

        private static int GetResponseStatusCode(ActionExecutedContext context)
        {
            const int defaultFailureStatusCode = (int)HttpStatusCode.InternalServerError;
            if (context.Exception is null && context.Result is IStatusCodeActionResult sc)
            {
                return sc.StatusCode ?? defaultFailureStatusCode;
            }
            else
            {
                return defaultFailureStatusCode;
            }
        }

        private static string GetUrlTemplatePath(ActionExecutedContext context)
        {
            return context.ActionDescriptor.AttributeRouteInfo.Template;
        }

        /// <summary>
        /// Observes latency with labels.
        /// </summary>
        /// <param name="requestMethod">HTTP method. Corresponds to http_verb label.</param>
        /// <param name="responseStatusCode">Response status code in format 200, 404, 500. Corresponds to status_code label.</param>
        /// <param name="responseStatusCodeGroup">Response status code group in format 2xx, 3xx, 4xx,5xx. Corresponds to status_code_group label.</param>
        /// <param name="urlTemplatePath">URL template path in format api/v{version:apiVersion}/Values/... Corresponds to url_template label.</param>
        private void ObserveLatency(
            string requestMethod,
            string responseStatusCode,
            string responseStatusCodeGroup,
            string urlTemplatePath)
        {
            ServiceRequestsDurationMsMetric
                .WithLabels(requestMethod, responseStatusCode, responseStatusCodeGroup, urlTemplatePath)
                .Observe(this.sw.Value.ElapsedMilliseconds);
        }
    }
}