using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace dummy.api.Infrastructure.Extensions
{
    /// <summary>
    /// Extension method to easily add swagger to the IServiceCollection.
    /// </summary>
    public static class SwaggerServiceExtensions
    {
        private static readonly ILogger Logger = Log.ForContext("SourceContext", "SwaggerServiceExtensions");

        /// <summary>
        /// Adds swagger documentation functionality to the IServiceCollection.
        /// </summary>
        /// <param name="services">A instance of <see cref="IServiceCollection"/> to be modified.</param>
        /// <param name="configuration">Configuration.</param>
        /// <returns>Modified instance of <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services, IConfiguration configuration)
        {
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<ExamplesOperationFilter>();

                // Resolve the IApiVersionDescriptionProvider service
                // Note: that we have to build a temporary service provider here because one has not been created yet
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                // Add a swagger document for each discovered API version
                // note: you might choose to skip or document deprecated API versions differently
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description, configuration));
                }

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                c.DescribeAllEnumsAsStrings();

                // Swagger 2.+ support - Add possibility to execute api methods with a provided bearer token
                var security = new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", new string[] { } },
                };

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "Header",
                    Type = "apiKey",
                });

                c.AddSecurityRequirement(security);
            });

            return services;
        }

        /// <summary>
        /// Configure middleware to handle swagger.
        /// </summary>
        /// <param name="app">Application builder.</param>
        /// <param name="configuration">Configuration.</param>
        /// <param name="provider">Api description provider.</param>
        /// <returns>A instance of <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app, IConfiguration configuration, IApiVersionDescriptionProvider provider)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            // app.UseSwagger();
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.Host = httpReq.Host.Value);
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(
                options =>
                {
                    // Build a swagger endpoint for each discovered API version
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                        options.DocumentTitle = configuration["SwaggerSettings:DocumentTitle"];
                        options.DocExpansion(DocExpansion.None);
                    }
                });

            return app;
        }

        private static Info CreateInfoForApiVersion(ApiVersionDescription description, IConfiguration configuration)
        {
            var info = new Info
            {
                Version = description.ApiVersion.ToString(),
                Title = $"dummy {description.ApiVersion}",
                Description = configuration["SwaggerSettings:Description"],
                TermsOfService = configuration["SwaggerSettings:TermsOfService"],
                Contact = new Contact
                {
                    Name = configuration["SwaggerSettings:Contact:Name"],
                    Email = configuration["SwaggerSettings:Contact:Email"],
                    Url = configuration["SwaggerSettings:Contact:Url"],
                },
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }
    }
}
