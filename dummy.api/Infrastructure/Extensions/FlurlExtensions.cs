using System.Collections.Generic;
using Flurl;
using Flurl.Http;
using OpenTracing;
using OpenTracing.Propagation;

namespace dummy.api.Infrastructure.Extensions
{
    public static class FlurlExtensions
    {
        public static IFlurlRequest WithTracingPropagation(this Url url, ITracer tracer)
        {
            return new FlurlRequest(url).WithTracingPropagation(tracer, tracer.ActiveSpan);
        }

        public static IFlurlRequest WithTracingPropagation(this Url url, ITracer tracer, ISpan span)
        {
            return new FlurlRequest(url).WithTracingPropagation(tracer, span);
        }

        public static IFlurlRequest WithTracingPropagation(this IFlurlRequest request, ITracer tracer)
        {
            return request.WithTracingPropagation(tracer, tracer.ActiveSpan);
        }

        public static IFlurlRequest WithTracingPropagation(this IFlurlRequest request, ITracer tracer, ISpan span)
        {
            var context = span?.Context;

            if (context == null)
            {
                return request;
            }

            var headers = new Dictionary<string, string>();
            tracer.Inject(context, BuiltinFormats.HttpHeaders, new TextMapInjectAdapter(headers));
            return request.WithHeaders(headers, false);
        }

        public static IFlurlRequest WithUserContext(this IFlurlRequest request, string userContext)
        {
            return request.WithHeader("UserContext", userContext);
        }
    }
}