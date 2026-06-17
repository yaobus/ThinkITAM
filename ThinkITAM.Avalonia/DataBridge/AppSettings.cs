namespace ThinkITAM.DataBridge
{
    /// <summary>
    /// 应用配置管理 — 替代 WPF Properties.Settings.Default
    /// 使用 JSON 文件持久化：主题、语言、加密字符串、版本号
    /// </summary>
    public static class AppSettings
    {
        private static readonly string ConfigDir;
        private static readonly string ConfigFilePath;
        private static AppSettingsData _data = new();

        static AppSettings()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            ConfigDir = Path.Combine(documentsPath, "ThinkITAM", "DatabaseConfig");
            ConfigFilePath = Path.Combine(ConfigDir, "AppSettings.json");
            Load();
        }

        /// <summary>
        /// 主题索引：0=亮色, 1=暗色
        /// </summary>
        public static int ThemeIndex
        {
            get => _data.ThemeIndex;
            set { _data.ThemeIndex = value; Save(); }
        }

        /// <summary>
        /// 语言索引：0=简体中文, 1=English
        /// </summary>
        public static int LanguageIndex
        {
            get => _data.LanguageIndex;
            set { _data.LanguageIndex = value; Save(); }
        }

        /// <summary>
        /// 加密字符串（密码哈希）
        /// </summary>
        public static string? EncryptString
        {
            get => _data.EncryptString;
            set { _data.EncryptString = value; Save(); }
        }

        /// <summary>
        /// 当前版本号
        /// </summary>
        public static string? Version
        {
            get => _data.Version;
            set { _data.Version = value; Save(); }
        }

        /// <summary>
        /// 数据库版本号（用于字段升级检查）
        /// </summary>
        public static int VersionNumber
        {
            get => _data.VersionNumber;
            set { _data.VersionNumber = value; Save(); }
        }

        /// <summary>
        /// 是否显示欢迎窗口
        /// </summary>
        public static bool ShowWelcome
        {
            get => _data.ShowWelcome;
            set { _data.ShowWelcome = value; Save(); }
        }

        private static void Load()
        {
            try
            {
                if (!Directory.Exists(ConfigDir))
                    Directory.CreateDirectory(ConfigDir);

                if (File.Exists(ConfigFilePath))
                {
                    var json = File.ReadAllText(ConfigFilePath);
                    _data = System.Text.Json.JsonSerializer.Deserialize<AppSettingsData>(json) ?? new AppSettingsData();
                }
            }
            catch
            {
                _data = new AppSettingsData();
            }
        }

        public static void Save()
        {
            try
            {
                if (!Directory.Exists(ConfigDir))
                    Directory.CreateDirectory(ConfigDir);

                var json = System.Text.Json.JsonSerializer.Serialize(_data, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ConfigFilePath, json);
            }
            catch { /* 忽略保存错误 */ }
        }
    }

    internal class AppSettingsData
    {
        public int ThemeIndex { get; set; } = 0;
        public int LanguageIndex { get; set; } = 0;
        public string? EncryptString { get; set; }
        public string? Version { get; set; }
        public int VersionNumber { get; set; } = 50;
        public bool ShowWelcome { get; set; } = true;
    }
}
