using dummy.api.Infrastructure.ActionFilters;
using dummy.api.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace dummy.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = false;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddVersionedApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";

                    // Note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;
                });
            services.AddSwaggerDocumentation(this.Configuration);
            services.AddServiceConfiguration();
            services.AddRequestHandlers();

            services.AddMvc(options =>
            {
                options.Filters.Add(new GlobalExceptionFilter());
                options.Filters.Add(new CollectPrometheusMetrics());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddOpenTracing()
                .WithIgnoredPathPattern("/monitoring")
                .AddJaeger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IApiVersionDescriptionProvider provider)
        {
            if (env.IsProduction() || env.IsStaging())
            {
                // app.UseExceptionHandler("/Error");
            }

            app.UseSwaggerDocumentation(
                this.Configuration,
                provider);

            app.UseMvc();
        }
    }
}