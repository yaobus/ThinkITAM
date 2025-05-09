using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkITAM.ViewModels.AssetManage
{
   public class AssetTypeViewModel
    {//索引
        public int Index
        {
            get;
            set;
        }
        //资产类型
        public string? AssetType
        {
            get;
            set;
        }
        //设备类型
        public string? DeviceTypeCount
        {
            get; set;
        }
        //设备总数
        public string? DeviceCount
        {
            get;
            set;
        }
    }


    public class DeviceTypeViewModel
    {
        public int Index
        {
            get;
            set;
        }

        /// <summary>
        /// 资产ID，对应详表表名
        /// </summary>
        public string? AssetId
        {
            get;
            set;
        }


        public string? AssetType
        {
            get;
            set;
        }


        //资产类型
        public string? DeviceType
        {
            get;
            set;
        }


        public string? AssetNumber
        {
            get;
            set;
        }


        public string? Description
        {
            get;
            set;
        }


        public string? Model
        {
            get;
            set;
        }


        public string? ToolTip
        {
            get;
            set;
        }

        /// <summary>
        /// 资产数量
        /// </summary>
        public int? AssetCount
        {
            get;
            set;
        }
    }
}
