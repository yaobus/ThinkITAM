using System.ComponentModel;

namespace ThinkITAM.ViewModels.Others
{
    public class MacVendorViewModel : INotifyPropertyChanged
    {
        private int _index;
        private string _mac;
        private string _vendor;

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




        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }



}
