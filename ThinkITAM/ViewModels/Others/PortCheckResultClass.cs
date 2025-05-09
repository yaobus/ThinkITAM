using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkITAM.ViewModels.Others
{
    public class PortCheckResult
    {
        /// <summary>
        /// 被检测的IP地址
        /// </summary>
        public string Host
        {
            get;
            set;
        } // 新增 Host 属性

        /// <summary>
        /// 被检测的端口
        /// </summary>
        public int Port
        {
            get;
            set;
        }

        /// <summary>
        /// 端口状态
        /// </summary>
        public bool IsOpen
        {
            get;
            set;
        }
    }


    /// <summary>
    /// 主机检测结果
    /// </summary>
    public class HostCheckResult : INotifyPropertyChanged
    {
        private int _index;
        private string _host;
        private string _hostName;
        private string _mac;
        private string _vendor;
        private string _openedPorts;

        public int Index
        {
            get => _index;
            set
            {
                if (_index != value)
                {
                    _index = value;
                    OnPropertyChanged(nameof(Index));
                }
            }
        }

        public string Host
        {
            get => _host;
            set
            {
                if (_host != value)
                {
                    _host = value;
                    OnPropertyChanged(nameof(Host));
                }
            }
        }

        public string HostName
        {
            get => _hostName;
            set
            {
                if (_hostName != value)
                {
                    _hostName = value;
                    OnPropertyChanged(nameof(HostName));
                }
            }
        }

        public string Mac
        {
            get => _mac;
            set
            {
                if (_mac != value)
                {
                    _mac = value;
                    OnPropertyChanged(nameof(Mac));
                }
            }
        }

        public string Vendor
        {
            get => _vendor;
            set
            {
                if (_vendor != value)
                {
                    _vendor = value;
                    OnPropertyChanged(nameof(Vendor));
                }
            }
        }

        public string OpenedPorts
        {
            get => _openedPorts;
            set
            {
                if (_openedPorts != value)
                {
                    _openedPorts = value;
                    OnPropertyChanged(nameof(OpenedPorts));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
