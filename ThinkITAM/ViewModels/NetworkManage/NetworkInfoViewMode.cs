using System.ComponentModel;

namespace ThinkITAM.ViewModels.NetworkManage
{

    public class NetworkInfoViewMode : INotifyPropertyChanged
    {
        //是否选中
        private bool isSelected;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        //索引
        public int Index
        {
            get; set;
        }
        public string? NetworkId
        {
            get; set;
        }

        public int? SortIndex{ get; set; }

        public string? TableName
        {
            get; set;
        }

        public string? Name
        {
            get; set;
        }


        public string? Description
        {
            get; set;
        }



        //网段信息
        public string? Network
        {
            get; set;
        }


        //子网掩码
        public string? Netmask
        {
            get; set;
        }


        public string? Parent
        {
            get; set;
        }

        public string? Child
        {
            get; set;
        }



        /// <summary>
        /// 地址数量  已用/总数
        /// </summary>
        public string? AddressCount
        {
            get;
            set;
        }


        //使用率
        public double Percentage
        {
            get; set;
        }
        //注释
        public string? TagA
        {
            get; set;
        }

        //注释
        public string? TagB
        {
            get; set;
        }

        //注释
        public string? TagC
        {
            get; set;
        }

        //注释
        public string? TagD
        {
            get; set;
        }

        //注释
        public string? TagE
        {
            get; set;
        }

        //注释
        public string? TagF
        {
            get; set;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}