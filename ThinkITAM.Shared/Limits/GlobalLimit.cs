namespace ThinkITAM.Shared.Limits
{
    public class GlobalLimit
    {
        /// <summary>
        /// 端口扫描器单次扫描的主机数量限制，512台
        /// </summary>
        public static readonly int ScanHostNumber = 512;

        /// <summary>
        /// IP地址数量限制
        /// </summary>
        public static readonly int IpAddressCount = 1048576;

        /// <summary>
        ///  设备数量限制
        /// </summary>
        public static readonly int DevicesCount = 65536;

        /// <summary>
        /// 机架数量限制
        /// </summary>
        public static readonly int RackCount = 65536;

        /// <summary>
        /// 终端数量限制
        /// </summary>
        public static readonly int ComputerCount = 65536;

        /// <summary>
        ///  节点数量限制
        /// </summary>
        public static readonly int NodeCount = 65536;
    }
}
