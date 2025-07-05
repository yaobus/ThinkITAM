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

        public string? User
        {
            get;
            set;
        }


        public string? Model
        {
            get;
            set;
        }

        public string? TagA
        {
            get;
            set;
        }

        public string? TagB
        {
            get;
            set;
        }
        public string? TagC
        {
            get;
            set;
        }
        public string? TagD
        {
            get;
            set;
        }

        public string? TagE
        {
            get;
            set;
        }

        public string? TagF
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



    /// <summary>
    /// 仅限于在编辑设备信息时使用
    /// </summary>
    public class DeviceEditViewModel
    {
        public string AssetNumber
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }

        public string EnableDate
        {
            get; set;
        }

        public string DeviceRoom
        {
            get;
            set;
        }

        public string DeviceCabinet
        {
            get; set;

        }

        public string TagA
        {
            get; set;
        }
        public string TagB
        {
            get; set;
        }
        public string TagC
        {
            get; set;
        }
        public string TagD
        {
            get; set;
        }
        public string TagE
        {
            get; set;
        }
        public string TagF
        {
            get; set;
        }

    }
}
