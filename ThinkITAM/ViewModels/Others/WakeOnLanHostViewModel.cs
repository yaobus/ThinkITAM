


using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ThinkITAM.ViewModels.Others
{
    public class WakeOnLanHostViewModel : INotifyPropertyChanged
    {

        private int index;
        public int Index
        {
            get => index;
            set
            {
                if (index != value)
                {
                    index = value;
                    OnPropertyChanged(nameof(Index));
                }
            }
        }


        private int uid;
        public int UID
        {
            get => uid;
            set
            {
                if (uid != value)
                {
                    uid = value;
                    OnPropertyChanged(nameof(UID));
                }
            }
        }

        private string hostGroup;
        public string HostGroup
        {
            get => hostGroup;
            set
            {
                if (hostGroup != value)
                {
                    hostGroup = value;
                    OnPropertyChanged(nameof(HostGroup));
                }
            }
        }


        private string name;
        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }


        private bool status;
        public bool Status
        {
            get => status;
            set
            {
                if (status != value)
                {
                    status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }



        private string ipAddress;
        public string IpAddress
        {
            get => ipAddress;
            set
            {
                if (ipAddress != value)
                {
                    ipAddress = value;
                    OnPropertyChanged(nameof(IpAddress));
                }
            }
        }


        private string netmask;
        public string Netmask
        {
            get => netmask;
            set
            {
                if (netmask != value)
                {
                    netmask = value;
                    OnPropertyChanged(nameof(Netmask));
                }
            }
        }


        private int port;
        public int Port
        {
            get => port;
            set
            {
                if (port != value)
                {
                    port = value;
                    OnPropertyChanged(nameof(Port));
                }
            }
        }


        private string mac;
        public string Mac
        {
            get => mac;
            set
            {
                if (mac != value)
                {
                    mac = value;
                    OnPropertyChanged(nameof(Mac));
                }
            }
        }



        private string note;
        public string Note
        {
            get => note;
            set
            {
                if (note != value)
                {
                    note = value;
                    OnPropertyChanged(nameof(Note));
                }
            }
        }



        private bool? pinToStart;
        public bool? PinToStart
        {
            get => pinToStart;
            set
            {
                if (pinToStart != value)
                {
                    pinToStart = value;
                    OnPropertyChanged(nameof(PinToStart));
                }
            }
        }













        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    
    /// <summary>
    /// Wol窗口HostPanel数据
    /// </summary>
    public class HostPanelViewModel : INotifyPropertyChanged
    {
        
        private string _groupName;
        private ObservableCollection<WakeOnLanHostViewModel> _hosts;
        
        public string GroupName
        {
            get => _groupName;
            set
            {
                if (_groupName != value)
                {
                    _groupName = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public ObservableCollection<WakeOnLanHostViewModel> Hosts
        {
            get => _hosts;
            set
            {
                if (_hosts != value)
                {
                    _hosts = value;
                    OnPropertyChanged();
                }
            }
        }
        
        
        
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
    
}
