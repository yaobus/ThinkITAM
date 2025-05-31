using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkITAM.ViewModels.Dashboard
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        public int NetworkCount    { get; set; }

        public int IpAddressCount   { get; set; }

        public int DeviceCount   { get; set; }

        public int ComputerCount   { get; set; }


        public int DevicePortCount   { get; set; }


        public int LinkCount { get; set; }

        public int LinkNodeCount { get; set; }



        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
