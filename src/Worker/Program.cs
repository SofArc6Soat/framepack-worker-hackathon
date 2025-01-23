using Microsoft.AspNetCore;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using System.Diagnostics.CodeAnalysis;

namespace Worker
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        public static void Main(string[] args)
        {
            LoggingConfig.AddSerilogConfig();

            try
            {
                Log.Information("Starting application");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application stopped by exception");
                throw;
            }
            finally
            {
                Log.Information("Server Shutting down");
                Log.CloseAndFlush();
            }
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSerilog(Log.Logger, dispose: true);
            });
    }

    [ExcludeFromCodeCoverage]
    public static class LoggingConfig
    {
        public static void AddSerilogConfig()
        {
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder().WithDefaultDestructurers().WithDestructurers([new DbUpdateExceptionDestructurer()]))
                .Enrich.WithCorrelationId()
                .Enrich.WithCorrelationIdHeader()
                .MinimumLevel.Debug()
                .WriteTo.Async(wt => wt.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"));

            Log.Logger = loggerConfiguration.CreateLogger();
            LogContext.PushProperty("trace_id", Guid.NewGuid().ToString());
        }
    }
}
