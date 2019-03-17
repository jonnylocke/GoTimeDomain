using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GoTimeDomain.Host
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var currentEnv = Environment.GetEnvironmentVariable("ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{currentEnv}.json", true)
                .Build();

            if (!configuration.GetChildren().Any())
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateLogger();
                Log.Fatal("No configuration present, exiting...");
                return 1;
            }

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.WithProperty("HostEnv", currentEnv)
                .CreateLogger();

            Log.Information($"Injected Environment: '{currentEnv}'");

            try
            {
                Log.Information("Starting...");
                await BuildHost(args, configuration, currentEnv).RunAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }

        private static IHost BuildHost(string[] args, IConfiguration configuration, string currentEnv)
        {
            var hostBuilder = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddEnvironmentVariables();
                    configHost.AddConfiguration(configuration);
                })
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddConfiguration(configuration);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(configuration);
                    services.AddScoped<Settings>();
                    services.AddHostedService<EsPersistentSubscription>();
                })
                .UseSerilog()
                .UseConsoleLifetime();

            return hostBuilder.Build();
        }
    }
}
