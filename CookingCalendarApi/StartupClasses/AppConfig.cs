

using Serilog.Events;

namespace CookingCalendarApi.StartupClasses
{
    public class ConsoleLogConfig
    {
        public bool IsEnabled { get; set; }
        public LogEventLevel Level { get; set; }
        public string OutTemplate { get; set; }
    }

    public class DatabaseLogConfig
    {
        public bool IsEnabled { get; set; }
        public LogEventLevel Level { get; set; }
        public string Context { get; set; }
    }

    public class AppLoggingConfig
    {
        public ConsoleLogConfig Console { get; set; }
        public DatabaseLogConfig Database { get; set; }

    }
    public class AppConfig
    {
        public string JwtToken { get; set; }
        public AppLoggingConfig Logging { get; set; }
        public SqlServerConfig Db { get; set; }
    }
}
