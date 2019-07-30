using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing.Util;

namespace dummy.api.Infrastructure.Extensions
{
    /// <summary>
    /// Service collection extensions to support Jaeger.
    /// </summary>
    public static class JaegerServiceCollectionExtensions
    {
        /// <summary>
        /// Add Jaeger as ITracer.
        /// </summary>
        /// <param name="services">Services collection.</param>
        /// <returns>Services collection to pass further.</returns>
        /// <exception cref="ArgumentNullException">Thrown if services are null.</exception>
        public static IServiceCollection AddJaeger(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton(serviceProvider =>
            {
                var loggerFactory = new LoggerFactory();
                var tracer = Jaeger.Configuration.FromEnv(loggerFactory).GetTracer();

                GlobalTracer.Register(tracer);

                return tracer;
            });

            return services;
        }
    }
}