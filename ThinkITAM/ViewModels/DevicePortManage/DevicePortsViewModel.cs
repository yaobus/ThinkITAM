using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ThinkITAM.ViewModels.Index;


namespace ThinkITAM.ViewModels.DevicePortManage;


public class DevicePortsViewModel : INotifyPropertyChanged
{
    
    private string _soltNumber;
    private ObservableCollection<PortTypeClass.PortDetailedInfo> _ports;

    public string SoltNumber
    {
        get => _soltNumber;
        set
        {
            if (_soltNumber != value)
            {
                _soltNumber = value;
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<PortTypeClass.PortDetailedInfo> Ports
    {
        get => _ports;
        set
        {
            if (_ports != value)
            {
                _ports = value;
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