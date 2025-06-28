namespace ThinkITAM.ViewModels.DevicePortManage
{
    class DeviceInfoClass
    {
        //索引
        public int Index
        {
            get; set;
        }

        //设备名称
        public string? DeviceName
        {
            get; set;
        }

        //设备编号
        public string Number
        {
            get; set;
        }

        //设备型号
        public string? Model
        {
            get; set;
        }

        //注释
        public string? Description
        {
            get; set;
        }
    }






    public class SlotInfo
    {
        /// <summary>
        /// 槽位标签，例如E,G,XG,F
        /// </summary>
        public string SlotTag
        {
            get; set;
        }

        /// <summary>
        /// 槽位编号比如0/0/X,或者1/0/X
        /// </summary>
        public string SlotNumber
        {
            get; set;
        }

        /// <summary>
        /// 端口号的第一个号是0，还是1
        /// </summary>
        public int FirstNumber
        {
            get; set;
        }




    }
}
