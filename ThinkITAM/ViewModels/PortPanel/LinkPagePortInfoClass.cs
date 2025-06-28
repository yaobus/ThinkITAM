namespace ThinkITAM.ViewModels.PortPanel
{


    /// <summary>
    /// 连输上的端口信息类
    /// </summary>
    public class LinkPagePortInfoClass
    {
        /// <summary>
        /// 机架或者建筑ID
        /// </summary>
        public string RackId
        {
            get; set;
        }

        /// <summary>
        /// 机房或者建筑 名称
        /// </summary>
        public string RoomOrBuilding
        {
            get; set;
        }


        /// <summary>
        /// 机柜  名称
        /// </summary>
        public string Cabinet
        {
            get; set;
        }

        /// <summary>
        /// 机架名称
        /// </summary>
        public string RackName
        {
            get; set;
        }

        /// <summary>
        /// 槽位或者楼层 号
        /// </summary>
        public string SlotOrFloor
        {
            get; set;
        }

        /// <summary>
        /// 房间号
        /// </summary>
        public string RoomNumber
        {
            get; set;
        }

        /// <summary>
        /// 端口号
        /// </summary>
        public string PortId
        {
            get; set;
        }
    }
}
