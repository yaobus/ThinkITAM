using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkITAM.ViewModels.DevicePortManage
{
    /// <summary>
    /// 通用导出类
    /// </summary>
    public class CommonExportClass
    {
        /// <summary>
        /// 要导出的数据类型 0:IP地址信息 1:设备端口信息 2:资产信息 3:资产LOG
        /// </summary>
        public int Type
        {
            get;
            set;
        }

        public dynamic WindowTags
        {
            get;
            set;
        }
    }
}
