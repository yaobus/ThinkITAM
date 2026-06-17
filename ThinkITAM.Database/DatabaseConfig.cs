namespace ThinkITAM.Database
{
    /// <summary>
    /// 数据库配置信息
    /// </summary>
    public class DatabaseConfig
    {
        /// <summary>
        /// 数据库类型: sqlite, mysql, mariadb, sqlserver
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 昵称/项目名称
        /// </summary>
        public string NickName { get; set; } = string.Empty;

        /// <summary>
        /// SQLite数据库文件路径（仅SQLite使用）
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// 主机地址（MySQL/SQL Server使用）
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// 端口号（MySQL/SQL Server使用）
        /// </summary>
        public int Port { get; set; } = 0;

        /// <summary>
        /// 用户名（MySQL/SQL Server使用）
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 密码（MySQL/SQL Server使用）
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 数据库名称（MySQL/SQL Server使用）
        /// </summary>
        public string DatabaseName { get; set; } = string.Empty;
    }
}
