using System.ComponentModel;

namespace ThinkITAM.ViewModels.Dashboard
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        public int NetworkCount
        {
            get; set;
        }

        public int IpAddressCount
        {
            get; set;
        }

        /// <summary>
        /// 可用地址总数
        /// </summary>
        public int AvailableAddressCount
        {
            get; set;
        }


        public int DeviceCount
        {
            get; set;
        }

        public int ComputerCount
        {
            get; set;
        }

        /// <summary>
        /// 可用设备端口数
        /// </summary>
        public int AvailableDevicePortCount
        {
            get; set;
        }


        public int LinkCount
        {
            get; set;
        }

        public int LinkNodeCount
        {
            get; set;
        }

        /// <summary>
        /// 机架未用端口总数
        /// </summary>
        public int RackUnusedPortCount
        {
            get; set;
        }

        /// <summary>
        /// 资产类型总数
        /// </summary>
        public int AssetTypeCount     
        {
            get; set;
        }




        /// <summary>
        /// 资产总数
        /// </summary>
        public int AssetCount
        {
            get; set;
        }


        /// <summary>
        /// 未部署资产总数
        /// </summary>
        public int AssetUnDeploy
        {
            get; set;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
