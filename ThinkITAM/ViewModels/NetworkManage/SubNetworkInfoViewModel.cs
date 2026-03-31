namespace ThinkITAM.ViewModels.NetworkManage
{
    public class SubNetworkInfoViewModel
    {

        //索引
        public float Index
        {
            get; set;
        }

        public string? TableName
        {
            get; set;
        }

        public string? Network
        {
            get; set;
        }

        public string? Netmask
        {
            get; set;
        }


        public string? Range
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

        //备注
        public string Note
        {
            get; set;
        }
    }
}
