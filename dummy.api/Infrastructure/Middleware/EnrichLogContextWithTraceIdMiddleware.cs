using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OpenTracing;
using Serilog.Context;
using Serilog.Core.Enrichers;

namespace dummy.api.Infrastructure.Middleware
{
    public class EnrichLogContextWithTraceIdMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ITracer tracer;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnrichLogContextWithTraceIdMiddleware"/> class.
        /// </summary>
        /// <param name="next">Next <see cref="RequestDelegate"/> to execute. </param>
        /// <param name="tracer">Tracer.</param>
        public EnrichLogContextWithTraceIdMiddleware(RequestDelegate next, ITracer tracer)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
        }

        /// <summary>
        /// Executes a middleware. Enriches log context with a tracing id from tracer.
        /// </summary>
        /// <param name="httpContext">Current <see cref="HttpContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Invoke(HttpContext httpContext)
        {
            _ = httpContext ?? throw new ArgumentNullException(nameof(httpContext));

            using (LogContext.Push(new PropertyEnricher("TraceId", this.GetTraceId())))
            {
                await this.next(httpContext);
            }
        }

        private string GetTraceId()
        {
            return this.tracer.ActiveSpan?.Context.TraceId;
        }
    }
}