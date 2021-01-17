using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Serilog.Sinks.MSSqlServer.Sinks.MSSqlServer.Options;
using Serilog.Sinks.SystemConsole.Themes;

namespace ZimaHrm.Web.Infrastructure
{
    public static class SerilogLogging
    {
        public static LoggerConfiguration InitLogger(WebHostBuilderContext context, LoggerConfiguration loggerConfiguration)
        {
            var connectionString = context.Configuration.GetConnectionString("Default");

            var sinkOpts = new MSSqlServerSinkOptions
            {
                TableName = "Logs",
                AutoCreateSqlTable = true,
            };

            var columnOpts = new ColumnOptions
            {
                ClusteredColumnstoreIndex = false
            };
            string logTemplate = "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}";


            var logger = loggerConfiguration
                .Enrich
                //https://github.com/serilog/serilog/wiki/Enrichment used to add custom enrichers like int the CorrelationIdMiddleware
                .FromLogContext()
                // https://github.com/saleem-mirza/serilog-enrichers-context/wiki
                // .Enrich.WithMachineName()
                .WriteTo.MSSqlServer(
                    connectionString,
                    sinkOptions: sinkOpts,
                    columnOptions: columnOpts)
                .WriteTo.Console(
                    outputTemplate:
                    logTemplate,
                    theme: AnsiConsoleTheme.Literate)
                .MinimumLevel.Warning()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .WriteTo.File($"logs{Path.DirectorySeparatorChar}log.txt", shared: true, rollingInterval: RollingInterval.Day, outputTemplate: logTemplate)
                .ReadFrom.Configuration(context.Configuration);
#if DEBUG
            // This will break automatically if Serilog throws an exception.
            SelfLog.Enable(
                msg =>
                {
                    Debug.Print(msg);
                    Debugger.Break();
                });
#endif
            return logger;
        }
    }
}
