using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkITAM.ViewModels.NetworkManage
{

    /// <summary>
    /// 网段导出向导数据类型
    /// </summary>
    public class ExportNetworkInfoClass
    {

        public string  Network { get; set; }

        public string Netmask { get; set; }

        public string TableName { get; set; }

        /// <summary>
        /// 自定义字段
        /// </summary>
        public dynamic WindowTags { get; set; }
    }
}
