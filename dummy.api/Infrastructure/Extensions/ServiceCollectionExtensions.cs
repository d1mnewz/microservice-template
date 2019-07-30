using dummy.api.Configuration;
using dummy.api.Controllers.ApiModels.Request;
using dummy.api.Controllers.ApiModels.Result;
using dummy.api.Handlers;
using Microsoft.Extensions.DependencyInjection;
using OpenTracing.Contrib.NetCore.Configuration;

namespace dummy.api.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection WithIgnoredPathPattern(this IServiceCollection services, string pattern)
        {
            services.Configure<AspNetCoreDiagnosticOptions>(options =>
            {
                options.Hosting.IgnorePatterns.Add(x => x.Request.Path.StartsWithSegments(pattern));
            });
            return services;
        }

        public static IServiceCollection AddRequestHandlers(this IServiceCollection services)
        {
            return services
                .AddTransient<IHandleRequest<GetValuesRequest, GenericRequestResultModel<GetValuesResult>>,
                    GetValuesRequestHandler>()
                .AddTransient<IHandleRequest<FillValuesRequest, SimpleRequestResultModel>,
                    FillValuesRequestHandler>();
        }

        public static IServiceCollection AddServiceConfiguration(this IServiceCollection services)
        {
            return services
                .AddSingleton<IDummyConfiguration, DummyConfiguration>();
        }
    }
}