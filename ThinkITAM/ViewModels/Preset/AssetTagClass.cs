using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkITAM.ViewModels.Preset
{
   public class AssetTagClass
    {

        //索引
        public int Index
        {
            get; set;
        }

        //资产类型
        public string AssetType
        {
            get; set;
        }

        //设备类型
        public string DeviceType
        {
            get; set;
        }

        //组织
        public string NumberPrefix
        {

            get; set;
        }

        //备注
        public string Note
        {
            get; set;
        }
    }
}
