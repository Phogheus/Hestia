using System.Threading.Tasks;
using Hestia.Web.Examples.gRPC.Models;
using Hestia.Web.Examples.gRPC.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc.Server;

namespace Hestia.Web.Examples.gRPC
{
    public class Program
    {
        private const string USE_AZURE_KEY = "UseAzure";
        private const string AZURE_APP_CONFIG_CONNECTION_STRING_KEY = "AzureAppConfigConnectionString";
        private const string MY_APP_CONFIG_KEY = nameof(MyApplicationConfiguration);

        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            InitializeConfiguration(builder.Configuration, builder.Services);

            var app = builder.Build();
            InitializeEnvironment(app);
            MapServices(app);

            await app.RunAsync();
        }

        private static void InitializeConfiguration(ConfigurationManager configuration, IServiceCollection services)
        {
            // Example code for accessing Azure features like Application Configuration and Application Insights
            if (configuration.GetValue(USE_AZURE_KEY, false))
            {
                // Load the connection string for the target Azure Application Configuration instance
                var azureAppConfigConnectionString = configuration[AZURE_APP_CONFIG_CONNECTION_STRING_KEY];

                _ = configuration.AddAzureAppConfiguration(x =>
                    x.Connect(azureAppConfigConnectionString)
                     .Select(MY_APP_CONFIG_KEY) // Loads "MyApplicationConfiguration" from Azure App Config with key name "MyApplicationConfiguration"
                     .UseFeatureFlags()); // Loads feature flags

                // App config and App Insights
                _ = services.AddAzureAppConfiguration();
                _ = services.AddApplicationInsightsTelemetry();
            }

            _ = services.Configure<MyApplicationConfiguration>(configuration.GetSection(MY_APP_CONFIG_KEY));

            // Register code-first grpc provider
            services.AddCodeFirstGrpc();

            // Add console logging
            _ = services.AddLogging(x => x.AddConsole());

            //_ = services.AddHttpContextAccessor();
        }

        private static void InitializeEnvironment(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                _ = app.UseExceptionHandler("/Error");
                _ = app.UseHsts();
            }
        }

        private static void MapServices(WebApplication app)
        {
            _ = app.MapGrpcService<GetServiceStatusService>();
        }
    }
}