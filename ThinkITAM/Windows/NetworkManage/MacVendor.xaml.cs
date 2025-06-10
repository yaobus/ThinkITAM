using ThinkITAM.FunctionClass;
using ThinkITAM.UserControls.General;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.ViewModels.Others;
using ZXing;

namespace ThinkITAM.Windows.NetworkManage
{
    /// <summary>
    /// MacVendor.xaml 的交互逻辑
    /// </summary>
    public partial class MacVendor : Window
    {
        public MacVendor()
        {
            InitializeComponent();
        }
        private readonly VendorInfoFetcher _vendorInfoFetcher = new VendorInfoFetcher();
        private async void MacHelp_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new ConfirmationDialog
            {
                Title = "MAC地址输入帮助",
                Prompt = $"在对话框输入要检测的MAC地址，多个地址请用逗号或空格隔开",
                ConfirmButtonText = "确认",
                

            };

            // 显示对话框
            await DialogHost.Show(dialog, "MessageDialogHost");


        }

        private async void ScanButton_OnClick(object sender, RoutedEventArgs e)
        {
            vendors.Clear();

            string input = MacTextBox.Text;

            var macAddresses = MacAddressParser.ParseMacAddresses(input);

            vendors = await ProcessMacAddressesAsync(macAddresses);

            MacVendorDataGrid.ItemsSource = vendors;
        }

        ObservableCollection<MacVendorViewModel> vendors = new ObservableCollection<MacVendorViewModel>();

        public async Task<ObservableCollection<MacVendorViewModel>> ProcessMacAddressesAsync(List<string> macAddresses)
        {
            var result = new ObservableCollection<MacVendorViewModel>();
            int index = 0;

            foreach (var mac in macAddresses)
            {
                string vendorInfo = await _vendorInfoFetcher.GetVendorInfo(mac);

                
                var viewModel = new MacVendorViewModel
                {
                    Index = ++index,
                    Mac = mac,
                    Vendor = vendorInfo
                };
                result.Add(viewModel);
            }

            return result;
        }

        private void MacVendor_OnLoaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void MacVendorDataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var info = MacVendorDataGrid.SelectedItem as MacVendorViewModel;
            

            var duration = 0.5;
            MessageSnackbar.MessageQueue?.Enqueue(
                $"已将{info.Vendor}发送到剪贴板",
                null,
                null,
                null,
                false,
                true,
                TimeSpan.FromSeconds(duration));

          //  Dispatcher.Invoke(() => ClipboardOperation.TrySetClipboardData(info.Vendor, 3, 10));

        }

    }
}


