using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using ThinkITAM.FunctionClass;
using ThinkITAM.IPAddressCalculations;
using ThinkITAM.UserControls.General;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json.Linq;

namespace ThinkITAM.Windows.NetworkManage
{
    /// <summary>
    /// NetworkHelper.xaml 的交互逻辑
    /// </summary>
    public partial class NetworkHelper : Window
    {
        public NetworkHelper()
        {
            InitializeComponent();
        }




        private async void MaskSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {



            if (this.IsLoaded == true)
            {
                //同步掩码条位置
                MaskSlider2.Value = MaskSlider.Value;

                //判断是不是瞎写的IP地址
                if (IsValidIp(IpTextBox.Text) == true)
                {
                    UpdateIPCalculations();
                }
                else
                {
                    IpTextBox.Text = "";

                    var dialog = new ConfirmationDialog
                    {
                        Title = "IP地址不合法",
                        Prompt = "你输入的IP地址不合法，请重新输入",
                        ConfirmButtonText = "确定",
                        TitleColor = ColorConverterClass.ColorToBrush("#CF3539"),
                        PromptColor = ColorConverterClass.ColorToBrush("#CF3539"),
                        ConfirmButtonColor = ColorConverterClass.ColorToBrush("#CF3539")
                    };



                    // 假设你的 DialogHost 在 XAML 中定义，并且 x:Name 设置为 "RootDialogHost"
                    var dialogHost = DialogHost.GetDialogSession("MessageDialogHost"); // 或者直接引用你的 DialogHost, 如: this.RootDialogHost

                    if (dialogHost != null)
                    {
                        // 如果 DialogHost 已经打开，则先关闭它
                        dialogHost.Close();
                        // 等待一小段时间以确保对话框已关闭，如果必要的话
                        // await Task.Delay(100);  // 这可能不需要，视具体情况而定
                    }

                    // 显示对话框并获取结果
                    var result = await DialogHost.Show(dialog, "MessageDialogHost");



                }
            }



        }


        /// <summary>
        /// IP地址计算
        /// </summary>
        private void UpdateIPCalculations()
        {
            try
            {
                IPAddress ip;
                if (IPAddress.TryParse(IpTextBox.Text, out ip))
                {
                    int maskLength = (int)MaskSlider.Value;
                    IPAddress mask = IPAddressCalculations.IPAddressCalculations.SubnetMaskFromPrefixLength(maskLength);
                    Netmask.Text = mask.ToString();

                    IPAddress networkAddress = ip.GetNetworkAddress(mask);
                    Network.Text = networkAddress.ToString();

                    IPAddress firstAddress = networkAddress.GetFirstUsable(ip.AddressFamily);
                    First.Text = firstAddress.ToString();

                    IPAddress lastAddress = networkAddress.GetLastUsable(ip.AddressFamily, maskLength);

                    Last.Text = lastAddress.ToString();

                    IPAddress broadcastAddress = networkAddress.GetBroadcastAddress(maskLength);
                    Broadcast.Text = broadcastAddress.ToString();


                    var addressCount = IPAddressCalculations.IPAddressCalculations.AddressCount(maskLength)-2;
                    NumBox.Text = addressCount.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }





        /// <summary>
        /// 判断是否是合法IP
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        static bool IsValidIp(string ipAddress)
        {
            IPAddress address;
            return IPAddress.TryParse(ipAddress, out address);
        }


        private void MaskSlider2_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.IsLoaded == true)
            {
                int value = Convert.ToInt32(MaskSlider2.Value);
                MaskLengthBox2.Text = value.ToString();
                MaskDecBox2.Text = IPAddressCalculations.IPAddressCalculations.GetSubnetMask(10, value);
                MaskHexBox2.Text = IPAddressCalculations.IPAddressCalculations.GetSubnetMask(16, value);
                MaskBinBox2.Text = IPAddressCalculations.IPAddressCalculations.GetSubnetMask(2, value);
                AvailableAddressBox.Text = IPAddressCalculations.IPAddressCalculations.GetAvailableAddresses(value).ToString();
            }
        }


        private void NeedNumBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            int number;
            try
            {
                number = int.Parse(NeedNumBox.Text);
            }
            catch (Exception exception)
            {
                NeedNumBox.Text = "0";
               number = 0;
            }

            int num = IPAddressCalculations.IPAddressCalculations.GetMinimumSubnetMaskBits(number);

            MaskLengthBox.Text = num.ToString();
            AvailableNumBox.Text= IPAddressCalculations.IPAddressCalculations.GetAvailableAddresses(num).ToString();
            MaskBox.Text= IPAddressCalculations.IPAddressCalculations.GetSubnetMask(10, int.Parse(MaskLengthBox.Text));

        }
    }
}
