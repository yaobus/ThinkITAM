namespace ThinkITAM.ViewModels.AssetManage
{
    public class ImportAssetDataViewModel
    {

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


        public string? AssetType
        {
            get; set;
        } // 资产类型


        public string? DeviceType
        {
            get; set;
        } // 设备类型


        public string? AssetTag
        {
            get; set;
        } // 资产编号前缀

        public string? AssetNumber
        {
            get; set;
        } // 资产编号

        public string? PurchaseDate
        {
            get; set;
        } // 购买日期

        public string? PurchasePrice
        {
            get; set;
        } // 购置价格

        public string? Manufacturer
        {
            get; set;
        } // 制造商

        public string? Model
        {
            get; set;
        } // 型号

        public string? SerialNumber
        {
            get; set;
        } // 序列号

        public string? Configuration
        {
            get; set;
        } // 配置

        public string? Location
        {
            get; set;
        } // 存放地点


        public string? Notes
        {
            get; set;
        } // 备注

        public string? TagA
        {
            get; set;
        } // 自定义备注A
        public string? TagB
        {
            get; set;
        } // 自定义备注B
        public string? TagC
        {
            get; set;
        } // 自定义备注C
        public string? TagD
        {
            get; set;
        } // 自定义备注D
        public string? TagE
        {
            get; set;
        } // 自定义备注E
        public string? TagF
        {
            get; set;
        } // 自定义备注F

    }
}
