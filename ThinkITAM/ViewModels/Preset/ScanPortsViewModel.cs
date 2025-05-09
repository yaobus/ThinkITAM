using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkITAM.ViewModels.Preset
{
    public class ScanPortsViewModel
    {
        /// <summary>
        /// 端口组名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 端口组
        /// </summary>
        public string Ports { get; set; }
    }
}
