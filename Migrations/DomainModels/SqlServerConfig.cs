using System.Text.RegularExpressions;

namespace Migrations.DomainModels
{
    /// <summary>
    /// SqlServerConfig Class
    /// </summary>
    public class SqlServerConfig
    {
        #region Member Variables
        private readonly Dictionary<string, string> _properties;
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public string Server
        {
            get => GetProperty("Data Source");
            set => SetProperty("Data Source", value);
        }

        /// <summary>
        /// 
        /// </summary>
        public string Database
        {
            get => GetProperty("Initial Catalog");
            set => SetProperty("Initial Catalog", value);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IntegratedSecurity
        {
            get => GetProperty("Integrated Security") == "true";
            set => SetProperty("Integrated Security", value ? "true" : "false");
        }

        /// <summary>
        /// 
        /// </summary>
        public string Username
        {
            get => GetProperty("User Id");
            set => SetProperty("User Id", value);
        }

        /// <summary>
        /// 
        /// </summary>
        public string Password
        {
            get => GetProperty("Password");
            set => SetProperty("Password", value);
        }

        /// <summary>
        /// 
        /// </summary>
        public int Timeout
        {
            get => GetProperty("Timeout") != null
                ? int.Parse(GetProperty("Timeout"))
                : 0;
            set => SetProperty("Timeout", value.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString => string.Join(";", _properties.Select(x => $"{x.Key}={x.Value}"));
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public SqlServerConfig()
        {
            _properties = new Dictionary<string, string>();
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetProperty(string key)
        {
            return (_properties.ContainsKey(key)
                ? _properties[key]
                : null)!;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void SetProperty(string key, string value)
        {
            if (_properties.ContainsKey(key))
                _properties[key] = value;
            else
                _properties.Add(key, value);
        }
        #endregion

        #region Factory Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
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

            string GetValue(string label) =>
                Regex
                    .Match(connectionString, @$"({label}[ ]*=[ ]*(?<value>[^;]+);)", RegexOptions.IgnoreCase)
                    ?.Groups["value"]
                    ?.Value!;
        }
        #endregion
    }
}
