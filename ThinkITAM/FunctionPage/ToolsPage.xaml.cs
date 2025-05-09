using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThinkITAM.ChildrenWindows.NetworkManage;
using ThinkITAM.ChildrenWindows.ToolWindows;

namespace ThinkITAM.FunctionPage
{
    /// <summary>
    /// ToolsPage.xaml 的交互逻辑
    /// </summary>
    public partial class ToolsPage : UserControl
    {
        public ToolsPage()
        {
            InitializeComponent();
        }

        private void PortScanToolButton_OnClick(object sender, RoutedEventArgs e)
        {
           PortScanWindow portScanWindow = new PortScanWindow(null);

           var window = Window.GetWindow(this);
           if (window != null)
           {
               portScanWindow.Owner = window;
           }

           portScanWindow.ShowDialog();

        }

        private void MacVendorButton_OnClick(object sender, RoutedEventArgs e)
        {
            MacVendor macVendor = new MacVendor();

            var window = Window.GetWindow(this);
            if (window != null)
            {
                macVendor.Owner = window;
            }

            macVendor.ShowDialog();

        }

        private void NetworkCalculate_OnClick(object sender, RoutedEventArgs e)
        {
            NetworkHelper networkHelper = new NetworkHelper();

            var window = Window.GetWindow(this);
            if (window != null)
            {
                networkHelper.Owner = window;
            }

            networkHelper.ShowDialog();
        }

        private void DiskCalculationButton_OnClick(object sender, RoutedEventArgs e)
        {
            StorageCalculationWindow storageCalculationWindow = new StorageCalculationWindow();
            var window = Window.GetWindow(this);
            if (window != null)
            {
                storageCalculationWindow.Owner = window;
            }

            storageCalculationWindow.ShowDialog();
        }

        private void WakeOnLanButton_OnClick(object sender, RoutedEventArgs e)
        {
            WakeOnLanWindow wakeOnLanWindow = new WakeOnLanWindow();
            var window = Window.GetWindow(this);
            if (window != null)
            {
                wakeOnLanWindow.Owner = window;
            }

            wakeOnLanWindow.ShowDialog();
        }
    }
}
