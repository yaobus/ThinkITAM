using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkITAM.ViewModels.AssetManage
{
    public class AssetViewModel : INotifyPropertyChanged
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

        public int Index { get; set; }//索引

        // 全局唯一资产ID
        public string? AssetId
        {
            get; set;
        } 

        //可以单独绑定的资产二维码ID
        public string? AssetQrCode
        {
            get;
            set;
        }


        public string? AssetType { get; set; } // 资产类型


        public string? DeviceType {get; set; } // 设备类型


        public string? AssetTag
        {
            get; set;
        } // 资产编号前缀

        public string? TagNumber
        {
            get; set;
        } // 资产编号序号


        public string? AssetNumber {get; set;} // 资产编号

        public string? PurchaseDate { get; set; } // 购买日期

        public string? PurchasePrice { get; set; } // 购置价格

        public string? Manufacturer { get; set; } // 制造商

        public string? Model { get; set; } // 型号

        public string? SerialNumber { get; set; } // 序列号

        public string? Configuration { get; set; } // 配置

        public string? Location { get; set; } // 存放地点



        public string? UserOrganization { get; set; } //责任人单位

        public string? UserDepartment { get; set; } //责任人部门

        public string? UserGroup
        {
            get; set;
        } //责任人群组

        public string? User { get; set; } // 责任人

        public string? UserPhone { get; set; } //责任人电话


        public string? Consumer {get; set; } //使用人

        public string? Status { get; set; } // 状态（使用中、闲置、维修、待报废、已报废等）

        public string? UsedYear { get; set; } // 已用年限

        public string? ScrapDate { get; set; } //报废时间

        public string? Notes { get; set; } // 备注

        public string? TagA { get; set; } // 自定义备注A
        public string? TagB { get; set; } // 自定义备注B
        public string? TagC { get; set; } // 自定义备注C
        public string? TagD { get; set; } // 自定义备注D
        public string? TagE { get; set; } // 自定义备注E
        public string? TagF { get; set; } // 自定义备注F


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class AssetType
    {

    }
}
