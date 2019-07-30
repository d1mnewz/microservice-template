using System.Runtime.Loader;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;

namespace dummy.api
{
    public static class Program
    {
        private static IConfigurationBuilder ConfigurationBuilder { get; set; }

        private static IHostingEnvironment HostingEnvironment { get; set; }

        public static void Main(string[] args)
        {
            LoggerCallbackHandler.UseDefaultLogging = false;
            AssemblyLoadContext.Default.Unloading += UnloadOnSigTerm;
            var host = CreateWebHostBuilder(args).Build();
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost
                .CreateDefaultBuilder(args)
                .UseSerilog((ctx, config) =>
                {
                    config
                        .MinimumLevel.Information()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .MinimumLevel.Override("System", LogEventLevel.Warning)
                        .Enrich.FromLogContext();

                    if (ctx.HostingEnvironment.IsDevelopment())
                    {
                        config.WriteTo.Console();
                    }
                    else
                    {
                        config.WriteTo.Console(new ElasticsearchJsonFormatter());
                    }
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    ConfigurationBuilder = config;

                    config.AddEnvironmentVariables();

                    // we will need env variables to setup kv access
                    var builtConfig = config.Build();

                    // Load environment variables that hold KV related secrets
                    // These should ideally be coming from k8s secret in the cluster
                    var clientId = builtConfig.GetValue<string>("clientid");
                    var clientSecret = builtConfig.GetValue<string>("clientsecret");
                    var keyVaultIdentifier = builtConfig.GetValue<string>("keyvaultidentifier");
                    var keyVaultUri = $"https://{keyVaultIdentifier}.vault.azure.net/";

                    config.AddAzureKeyVault(keyVaultUri, clientId, clientSecret);

                    HostingEnvironment = hostingContext.HostingEnvironment;

                    if (HostingEnvironment.IsDevelopment())
                    {
                        config.AddJsonFile(
                            $"appsettings.{HostingEnvironment.EnvironmentName}.json",
                            optional: true,
                            reloadOnChange: true);
                    }

                    if (HostingEnvironment.IsStaging())
                    {
                        config.AddJsonFile(
                            $"appsettings.{HostingEnvironment.EnvironmentName}.json",
                            optional: true,
                            reloadOnChange: true);
                    }

                    if (HostingEnvironment.IsProduction())
                    {
                        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    }
                })
                .UseKestrel()
                .UseStartup<Startup>();

        /// <summary>
        /// Handle SIGINT or SIGTERM.
        /// </summary>
        /// <param name="context">Assembly Load Context.</param>
        private static void UnloadOnSigTerm(AssemblyLoadContext context)
        {
            Log.Warning("The program is being terminated");
        }
    }
}