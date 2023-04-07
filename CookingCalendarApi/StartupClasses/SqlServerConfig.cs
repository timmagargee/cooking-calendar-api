using System.Text.RegularExpressions;

namespace CookingCalendarApi.StartupClasses
{
    public class SqlServerConfig
    {
        #region Member Variables
        private readonly Dictionary<string, string> _properties;
        #endregion

        #region Properties

        public string Server
        {
            get => GetProperty("Data Source");
            set => SetProperty("Data Source", value);
        }

        public string Database
        {
            get => GetProperty("Initial Catalog");
            set => SetProperty("Initial Catalog", value);
        }

        public bool IntegratedSecurity
        {
            get => GetProperty("Integrated Security") == "true";
            set => SetProperty("Integrated Security", value ? "true" : "false");
        }

        public string Username
        {
            get => GetProperty("User Id");
            set => SetProperty("User Id", value);
        }

        public string Password
        {
            get => GetProperty("Password");
            set => SetProperty("Password", value);
        }

        public int Timeout
        {
            get => GetProperty("Timeout") != null
                ? int.Parse(GetProperty("Timeout"))
                : 0;
            set => SetProperty("Timeout", value.ToString());
        }

        public string ConnectionString => string.Join(";", _properties.Select(x => $"{x.Key}={x.Value}"));
        #endregion

        #region Constructor

        public SqlServerConfig()
        {
            _properties = new Dictionary<string, string>();
        }
        #endregion

        #region Utility Methods

        private string GetProperty(string key)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return _properties.ContainsKey(key)
                ? _properties[key]
                : null;
#pragma warning restore CS8603 // Possible null reference return.
        }


        private void SetProperty(string key, string value)
        {
            if (_properties.ContainsKey(key))
                _properties[key] = value;
            else
                _properties.Add(key, value);
        }
        #endregion

        #region Factory Methods

        public static SqlServerConfig Parse(string connectionString)
        {
            return
                new SqlServerConfig
                {
                    Server = GetValue("Data Source"),
                    Username = GetValue("User Id"),
                    Password = GetValue("Password"),
                    Database = GetValue("Initial Catalog")
                };

#pragma warning disable CS8603 // Possible null reference return.
            string GetValue(string label) =>
                Regex
                    .Match(connectionString, @$"({label}[ ]*=[ ]*(?<value>[^;]+);)", RegexOptions.IgnoreCase)
                    ?.Groups["value"]
                    ?.Value;
#pragma warning restore CS8603 // Possible null reference return.
        }
        #endregion
    }
}
