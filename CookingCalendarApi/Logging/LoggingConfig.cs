using System;
using System.Collections.ObjectModel;
using System.Data;
using CookingCalendarApi.StartupClasses;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Serilog.Sinks.SystemConsole.Themes;

namespace CookingCalendarApi.Logging
{
    /// <summary>
    /// LoggingConfig Class
    /// </summary>
    public static class LoggingConfig
    {
        private const string DefaultConsoleOutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} [{SourceContext}]{NewLine}{Exception}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appConfig"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Serilog.ILogger BuildLogger(AppConfig appConfig)
        {
            if (appConfig == null)
                throw new InvalidOperationException("You must load the application configuration before building the logger");

            LoggerConfiguration loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("AppName", "SCRA API")
                .Enrich.WithProperty("Context", appConfig.Logging.Database.Context);

            if (appConfig.Logging.Console.IsEnabled)
                ConfigureConsoleSink(appConfig, loggerConfig);

            if (appConfig.Logging.Database.IsEnabled)
                ConfigureDatabaseSink(appConfig, loggerConfig);

            return loggerConfig.CreateLogger();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appConfig"></param>
        /// <param name="loggerConfig"></param>
        private static void ConfigureConsoleSink(AppConfig appConfig, LoggerConfiguration loggerConfig)
        {
            Log.ForContext<Program>().Information("Adding Console sink...");

            loggerConfig.WriteTo
                .Console(
                    outputTemplate:
                    string.IsNullOrWhiteSpace(appConfig.Logging.Console.OutTemplate)
                        ? DefaultConsoleOutputTemplate
                        : appConfig.Logging.Console.OutTemplate,
                    theme: SystemConsoleTheme.Literate,
                    restrictedToMinimumLevel: appConfig.Logging.Console.Level);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appConfig"></param>
        /// <param name="loggerConfig"></param>
        private static void ConfigureDatabaseSink(AppConfig appConfig, LoggerConfiguration loggerConfig)
        {
            Log.Logger.ForContext<Program>().Information("Adding MSSqlServer sink...");

            var columnOptions = new ColumnOptions
            {
                AdditionalColumns = new Collection<SqlColumn>
                {
                    new SqlColumn {ColumnName = "AppName", PropertyName = "AppName", DataType = SqlDbType.NVarChar, DataLength = 128 },
                    new SqlColumn {ColumnName = "Context", PropertyName = "Context", DataType = SqlDbType.NVarChar, DataLength = 128 }
                },
                TimeStamp =
                {
                    ConvertToUtc = true
                }
            };

            //loggerConfig.WriteTo.MSSqlServer(
            //    connectionString: appConfig.HubDb.ConnectionString,
            //    restrictedToMinimumLevel: appConfig.Logging.Database.Level,
            //    columnOptions: columnOptions,
            //    sinkOptions: new MSSqlServerSinkOptions { TableName = "Logs" }
            //);
        }
    }
}

