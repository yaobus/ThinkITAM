using ThinkITAM.Database;
using ThinkITAM.ViewModels.DataBaseConfig;

namespace ThinkITAM.DataBridge
{
    /// <summary>
    /// 全局变量容器
    /// 存储数据库服务和配置的静态引用，供所有页面使用
    /// </summary>
    public class GlobalVariables
    {
        /// <summary>
        /// 全局数据库服务实例
        /// 由 MainWindow 在选择项目时创建
        /// </summary>
        public static IDatabaseService? DbService;

        /// <summary>
        /// 当前数据库配置
        /// </summary>
        public static DataBaseConfigViewModel? dbConfig;
    }
}
