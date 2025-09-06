using System.Windows;
using System.Windows.Controls;
using ThinkITAM.DataBridge;
using ThinkITAM.Windows.NetworkManage;
using ThinkITAM.Windows.ToolWindows;

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

            //var window = Window.GetWindow(this);
            //if (window != null)
            //{
            //    portScanWindow.Owner = window;
            //}

            portScanWindow.Show();

        }

        private void MacVendorButton_OnClick(object sender, RoutedEventArgs e)
        {
            MacVendor macVendor = new MacVendor();

            //var window = Window.GetWindow(this);
            //if (window != null)
            //{
            //    macVendor.Owner = window;
            //}

            macVendor.Show();

        }

        private void NetworkCalculate_OnClick(object sender, RoutedEventArgs e)
        {
            NetworkHelper networkHelper = new NetworkHelper();

            //var window = Window.GetWindow(this);
            //if (window != null)
            //{
            //    networkHelper.Owner = window;
            //}

            networkHelper.Show();
        }

        private void DiskCalculationButton_OnClick(object sender, RoutedEventArgs e)
        {
            StorageCalculationWindow storageCalculationWindow = new StorageCalculationWindow();
            //var window = Window.GetWindow(this);
            //if (window != null)
            //{
            //    storageCalculationWindow.Owner = window;
            //}

            storageCalculationWindow.Show();
        }

        private void WakeOnLanButton_OnClick(object sender, RoutedEventArgs e)
        {
            WakeOnLanWindow wakeOnLanWindow = new WakeOnLanWindow();
            //var window = Window.GetWindow(this);
            //if (window != null)
            //{
            //    wakeOnLanWindow.Owner = window;
            //}

            wakeOnLanWindow.Show();
        }


        private void NotePadButton_OnClick(object sender, RoutedEventArgs e)
        {
           NotePadWindow notePadWindow = new NotePadWindow();
            //var window = Window.GetWindow(this);
            //if (window != null)
            //{
            //    notePadWindow.Owner = window;
            //}
            notePadWindow.Show();
        }
    }


}
