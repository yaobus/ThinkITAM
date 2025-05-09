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
using System.Windows.Shapes;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.ViewModels.AssetManage;
using ThinkITAM.ViewModels.Others;
using Nodify;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.IPAddressHelper;

namespace ThinkITAM.Windows.ToolWindows
{
    /// <summary>
    /// AddWakeOnLan.xaml 的交互逻辑
    /// </summary>
    public partial class AddWakeOnLan : Window
    {
        public AddWakeOnLan(WakeOnLanHostViewModel info = null)
        {
            InitializeComponent();
            if (info != null)
            {
                wakeOnLanHostViewModel = info;
                this.DataContext = wakeOnLanHostViewModel;
            }

        }

        private WakeOnLanHostViewModel wakeOnLanHostViewModel;
        private void AddWakeOnLan_OnLoaded(object sender, RoutedEventArgs e)
        {

        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {

            var info = CheckInput();

            if (info.Item1 == 0)
            {
                string mac =Functions.FunctionClass.MacAddressValidator.ValidateAndFormatMacAddress(MacTextBox.Text);

                string sqlTemp = $"SELECT COUNT(*) FROM WakeOnLan WHERE Mac='{mac}'";

                var countNum = DbClass.ExecuteScalarTableNum(sqlTemp);

                if (countNum == 0)
                {
                    int uid = GetNextAvailableNumber();

                    string sql =
                        $"INSERT INTO WakeOnLan (UID,HostGroup,Name,IpAddress,Netmask,Port,Mac,PinToStart) VALUES ('{uid}','{GroupTextBox.Text}','{NameTextBox.Text}','{IpAddressTextBox.Text}','{NetmaskTextBox.Text}','{PortTextBox.Text}','{mac}','{PinToStart.IsChecked}')";

                    GlobalVariables.DbService.ExecuteNonQuery(sql);
                }
                else
                {

                    string sql =
                        $"UPDATE WakeOnLan SET HostGroup='{GroupTextBox.Text}',Name='{NameTextBox.Text}',IpAddress='{IpAddressTextBox.Text}',Netmask='{NetmaskTextBox.Text}',Port='{PortTextBox.Text}',PinToStart='{PinToStart.IsChecked}' WHERE Mac='{mac}'";


                    GlobalVariables.DbService.ExecuteNonQuery(sql);


                }


                DialogResult = true;

            }
            else
            {
                MessageBox.Show(info.Item2);
            }

        }

        /// <summary>
        /// 获取下一个可用的编号
        /// </summary>
        /// <returns></returns>
        public int GetNextAvailableNumber()
        {
            var usedNumbers = new HashSet<int>();

            string sql = "SELECT UID FROM WakeOnLan "; // 假设Del为0表示未删除的记录   WHERE Del != 1 OR Del IS NULL

            var rows = GlobalVariables.DbService.ExecuteQuery(sql);

            foreach (var row in rows)
            {
                usedNumbers.Add(Convert.ToInt32(row["UID"]));
            }




            int nextNumber = 1; // Start with the smallest possible number
            while (usedNumbers.Contains(nextNumber))
            {
                nextNumber++;
            }

            return nextNumber;
        }


        private (int, string) CheckInput()
        {
            int index = 0;

            string message = "当前存在以下问题需要解决:\r";

            if (!string.IsNullOrWhiteSpace(IpAddressTextBox.Text))
            {

                string ip = IpAddressTextBox.Text;


                if (IPAddressCalculations.IsValidIp(ip) == false)
                {

                    index++;

                    message += index.ToString() + ":IP地址不合法\r";


                }
            }
            else
            {
                string ip = IpAddressTextBox.Text;


                if (IPAddressCalculations.IsValidIp(ip) == false)
                {

                    index++;

                    message += index.ToString() + ":IP地址不得为空,如果不清楚设备具体IP，可填写设备所在网段广播地址(或设备所在网段任意IP地址，唤醒时选择“广播唤醒”)\r";


                }
            }

            if (!string.IsNullOrWhiteSpace(NetmaskTextBox.Text))
            {

                string ip = NetmaskTextBox.Text;


                if (IPAddressCalculations.IsValidIp(ip) == false)
                {

                    index++;

                    message += index.ToString() + ":子网掩码地址不合法\r";


                }
            }

            if (!string.IsNullOrWhiteSpace(MacTextBox.Text))
            {

                string mac = MacTextBox.Text;


                if (Functions.FunctionClass.MacAddressValidator.ValidateAndFormatMacAddress(mac) == null)
                {
                    index++;

                    message += index.ToString() + ":MAC地址不合法\r";
                }

            }
            else
            {
                index++;

                message += index.ToString() + ":Wake On Lan 功能必须填写MAC地址\r";

            }


            if (!string.IsNullOrWhiteSpace(PortTextBox.Text))
            {

                string port = PortTextBox.Text;

                if (Functions.FunctionClass.PortValidator.ValidatePort(port) == null)
                {
                    index++;

                    message += index.ToString() + ":端口输入有误，如无必要请勿填写自定义端口\r";
                }


            }
          

            return (index, message);
        }

    }
}
