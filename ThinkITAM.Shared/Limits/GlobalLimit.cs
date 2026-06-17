namespace ThinkITAM.Shared.Limits
{
    public class GlobalLimit
    {
        /// <summary>
        /// 端口扫描器单次扫描的主机数量限制，512台
        /// </summary>
        public static readonly int ScanHostNumber = 512;

        /// <summary>
        /// IP地址数量限制，8192个
        /// </summary>
        public static readonly int IpAddressCount = 16384;

        /// <summary>
        ///  设备数量限制，1024个
        /// </summary>
        public static readonly int DevicesCount = 1024;

        /// <summary>
        /// 机架数量限制，256个
        /// </summary>
        public static readonly int RackCount = 256;

        /// <summary>
        /// 终端数量限制，2048个
        /// </summary>
        public static readonly int ComputerCount = 2048;

        /// <summary>
        ///  节点数量限制，12288个
        /// </summary>
        public static readonly int NodeCount = 12288;
    }
}
