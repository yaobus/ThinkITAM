namespace ThinkITAM.ViewModels.NetworkManage
{

    public class NetworkInfoViewMode
    {

        public bool IsSelected
        {
            get; set;
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


    }


}