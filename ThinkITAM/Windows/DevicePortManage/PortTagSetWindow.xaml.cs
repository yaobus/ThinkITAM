using System.ComponentModel;
using System.Windows;
using Newtonsoft.Json;
using ThinkITAM.DatabaseOperation;

namespace ThinkITAM.Windows.DevicePortManage
{
    /// <summary>
    /// PortTagSetWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PortTagSetWindow : Window
    {

        public PortTagSetWindow()
        {
            InitializeComponent();
        }

        private void PortTagSetWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            string tagWindow;

            if (DataBridge.DataBridge.SelectDeviceTableInfo != null)
            {
                tagWindow = "DevicePortTag" + DataBridge.DataBridge.SelectDeviceTableInfo.AssetId;
                LocalRadioButton.IsEnabled = true;
            }
            else
            {
                tagWindow = "DevicePortTag";
                LocalRadioButton.IsEnabled = false;
            }



            if (tagWindow != "DevicePortTag")//存在自定义标签
            {

                string sqlTemp = $"SELECT COUNT(*) FROM WindowTag WHERE WindowName ='{tagWindow}'";

                var num = DbClass.ExecuteScalarTableNum(sqlTemp);

                if (num > 0 )//存在本地自定义标签
                {
                    var tags = DbClass.LoadWindowTag(tagWindow);

                    if (tags != null)
                    {
                        LocalRadioButton.IsChecked = true;

                        dynamic settings = JsonConvert.DeserializeObject(tags);

                        TagA.Text = settings.TagA;
                        TagB.Text = settings.TagB;
                        TagC.Text = settings.TagC;
                        TagD.Text = settings.TagD;
                        TagE.Text = settings.TagE;
                        TagF.Text = settings.TagF;

                    }

                }

            }
            else//默认标签
            {
                string sqlTemp = $"SELECT COUNT(*) FROM WindowTag WHERE WindowName ='DevicePortTag'";

                var num = DbClass.ExecuteScalarTableNum(sqlTemp);

                if (num > 0)
                {
                    var tags = DbClass.LoadWindowTag(tagWindow);

                    if (tags != null)
                    {
                        LocalRadioButton.IsChecked = true;

                        dynamic settings = JsonConvert.DeserializeObject(tags);

                        TagA.Text = settings.TagA;
                        TagB.Text = settings.TagB;
                        TagC.Text = settings.TagC;
                        TagD.Text = settings.TagD;
                        TagE.Text = settings.TagE;
                        TagF.Text = settings.TagF;

                    }

                }
            }

        }

        private void PortTagSetWindow_OnClosing(object? sender, CancelEventArgs e)
        {

        }

        private void GlobalRadioButton_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void LocalRadioButton_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void ResetButton_OnClick(object sender, RoutedEventArgs e)
        {
            string tagWindow;
            string message;

            if (LocalRadioButton.IsChecked == true)
            {


                message = "你正在重置该设备独有自定义标签，是否继续？";

            }
            else
            {

                message = "你正在重置全局设备默认自定义标签，是否继续？";
            }

            MessageBoxResult result = MessageBox.Show(message, "重置标签", MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                TagA.Text = "自定义标签A";
                TagB.Text = "自定义标签B";
                TagC.Text = "自定义标签C";
                TagD.Text = "自定义标签D";
                TagE.Text = "自定义标签E";
                TagF.Text = "自定义标签F";
            }

        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            string tagWindow;

            if (GlobalRadioButton.IsChecked == true || DataBridge.DataBridge.SelectDeviceTableInfo == null)//全局
            {

                tagWindow = "DevicePortTag";

            }
            else//本地
            {

                tagWindow = "DevicePortTag" + DataBridge.DataBridge.SelectDeviceTableInfo.AssetId;

            }



            var settings = new
            {
                TagA = TagA.Text,    // 自定义标签1
                TagB = TagB.Text,    // 自定义标签2
                TagC = TagC.Text,    // 自定义标签3
                TagD = TagD.Text,    // 自定义标签4
                TagE = TagE.Text,    // 自定义标签5
                TagF = TagF.Text,    // 自定义标签6
            };

            // 将匿名对象序列化为JSON字符串
            string json = JsonConvert.SerializeObject(settings);

            DbClass.SaveWindowTag(tagWindow, json);

            this.DialogResult = true;
            this.Close();

        }
    }
}
