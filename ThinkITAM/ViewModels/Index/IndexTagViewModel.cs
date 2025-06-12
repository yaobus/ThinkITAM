using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ThinkITAM.ViewModels.Index
{
    public class IndexTagViewModel : INotifyPropertyChanged
    {
        private string _indexId;
        private string _group;
        private string _name;
        private string _protocol;
        private string _host;
        private string _port;
        private int _color = 0;
        private string _url;
        private string _browser;
        private int _index;
        private int _pinToStart;

        public event PropertyChangedEventHandler PropertyChanged;

        public string IndexId
        {
            get => _indexId;
            set
            {
                if (_indexId != value)
                {
                    _indexId = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Group
        {
            get => _group;
            set
            {
                if (_group != value)
                {
                    _group = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Protocol
        {
            get => _protocol;
            set
            {
                if (_protocol != value)
                {
                    _protocol = value;
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
            }
        }

        public string Port
        {
            get => _port;
            set
            {
                if (_port != value)
                {
                    _port = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Color
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Url
        {
            get => _url;
            set
            {
                if (_url != value)
                {
                    _url = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Browser
        {
            get => _browser;
            set
            {
                if (_browser != value)
                {
                    _browser = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// BrowserCombobox的索引
        /// </summary>
        public int Index
        {
            get => _index;
            set
            {
                if (_index != value)
                {
                    _index = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否Pin到开始页面
        /// </summary>
        public int PinToStart
        {
            get => _pinToStart;
            set
            {
                if (_pinToStart != value)
                {
                    _pinToStart = value;
                    OnPropertyChanged();
                }
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }


    public class DashboardIndexTagViewModel : INotifyPropertyChanged
    {
        private string _groupName;
        private ObservableCollection<IndexTagViewModel> _indexTags;

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

        public ObservableCollection<IndexTagViewModel> IndexTags
        {
            get => _indexTags;
            set
            {
                if (_indexTags != value)
                {
                    _indexTags = value;
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
