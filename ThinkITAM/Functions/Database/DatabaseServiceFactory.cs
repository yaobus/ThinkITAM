using System;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.ViewModels.DataBaseConfig;

namespace ThinkITAM.Functions.Database
{
    /// <summary>
    /// 根据统一配置模型创建对应的数据库服务实例
    /// </summary>
    public static class DatabaseServiceFactory
    {
        public static IDatabaseService CreateService(DataBaseConfigViewModel config)
        {
            ValidateConfig(config);

            switch (config.Type?.ToLower())
            {
                case "sqlite":
                    return new SqliteDatabaseService(BuildSqliteConnectionString(config));

                case "mysql":
                case "mariadb":
                    return new MySqlDatabaseService(BuildMySqlConnectionString(config));

                case "sqlserver":
                    return new SqlServerDatabaseService(BuildSqlServerConnectionString(config));

                default:
                    throw new NotSupportedException($"不支持的数据库类型: {config.Type}");
            }
        }

        private static void ValidateConfig(DataBaseConfigViewModel config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (string.IsNullOrWhiteSpace(config.Type))
                throw new ArgumentException("数据库类型（type）不能为空。");

            switch (config.Type.ToLower())
            {
                case "sqlite" when string.IsNullOrWhiteSpace(config.Path):
                    throw new ArgumentException("Sqlite 配置中路径（path）不能为空。");
                case "mysql" or "mariadb" when string.IsNullOrWhiteSpace(config.Host) ||
                                               string.IsNullOrWhiteSpace(config.DatabaseName) ||
                                               string.IsNullOrWhiteSpace(config.UserName):
                    throw new ArgumentException("MySQL/MariaDB 配置中 host/database/username 不能为空。");
                case "sqlserver" when string.IsNullOrWhiteSpace(config.Host) ||
                                                  string.IsNullOrWhiteSpace(config.DatabaseName) ||
                                                  string.IsNullOrWhiteSpace(config.UserName):
                    throw new ArgumentException("SqlServer 配置中 server/database/username 不能为空。");
            }
        }

        private static string BuildSqliteConnectionString(DataBaseConfigViewModel config)
        {
            var builder = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder
            {
                DataSource = config.Path
            };
            if (!string.IsNullOrEmpty(config.Password))
            {
                builder.Password = config.Password;
            }
            return builder.ToString();
        }

        private static string BuildMySqlConnectionString(DataBaseConfigViewModel config)
        {
            return $"server={config.Host};port={config.Port};user={config.UserName};password={config.Password};database={config.DatabaseName};Allow User Variables=true;";
        }

        private static string BuildSqlServerConnectionString(DataBaseConfigViewModel config)
        {
            return $"Server={config.Host},{config.Port};Database={config.DatabaseName};User Id={config.UserName};Password={config.Password};TrustServerCertificate=True;";
        }
    }
}