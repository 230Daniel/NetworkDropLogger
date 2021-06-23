using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetworkDropLogger.Configuration;
using NetworkDropLogger.Services;

namespace NetworkDropLogger
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(builder => 
                    builder.AddCommandLine(args))
                .ConfigureServices(ConfigureServices)
                .Build();

            try
            {
                await host.RunAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Critical failure");
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddHostedService<NetworkDropLoggerService>();
            
            services.AddTransient<DetectorService>();
            services.AddTransient<LoggerService>();
            
            services.Configure<DetectorConfiguration>(context.Configuration.GetSection("Detector"));
            services.Configure<LoggerConfiguration>(context.Configuration.GetSection("Logger"));
        }
    }
}
