using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace spnode.communication.bolt
{
    public class BoltServer
    {
        public void Start(int port, CancellationToken cancellationToken = default)
        {
            Task.Run(async () =>
            {
                try
                {
                    // Ensure server content path exists
                    var contentRoot = "BoltRoot";
                    var contentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, contentRoot);
                    if (!Directory.Exists(contentPath))
                        Directory.CreateDirectory(contentPath);

                    var boltLogger = Log.Logger.ForContext<BoltServer>();

                    var config = new ConfigurationBuilder()
                        .Build();

                    var host = new WebHostBuilder()
                        .UseKestrel()
                        .CaptureStartupErrors(true)
                        .UseContentRoot(contentPath)
                        .UseSetting(WebHostDefaults.DetailedErrorsKey, true.ToString())
                        .UseConfiguration(config)
                        .UseUrls($"https://+:{port}")
                        .ConfigureServices(s => s.AddSingleton(config))
                        .UseSerilog(boltLogger, true)
                        .UseContentRoot(contentRoot)
                        .UseStartup<BoltStartup>()
                        .Build();

                    try
                    {
                        boltLogger.Information("Starting on port: {BoltPort} ...", port);
                        await host.StartAsync(cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Bolt terminated unexpectedly.");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to start Bolt server.");
                }
            }, cancellationToken);
        }
    }
}
