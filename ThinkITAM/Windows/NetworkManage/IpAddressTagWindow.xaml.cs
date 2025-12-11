using System.Windows;
using Newtonsoft.Json;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;

namespace ThinkITAM.Windows.NetworkManage
{
    /// <summary>
    /// IpAddressTagWindow.xaml 的交互逻辑
    /// </summary>
    public partial class IpAddressTagWindow : Window
    {
        DbClass dbClass;
        public IpAddressTagWindow()
        {
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            string tagWindow;


            if (!string.IsNullOrWhiteSpace(DataBridge.DataBridge.NetworkTableName))
            {
                tagWindow = "IpAddressInfoTag" + DataBridge.DataBridge.NetworkTableName;

                //LocalRadioButton.IsEnabled = true;
            }
            else
            {
                tagWindow = "IpAddressInfoTag";
                //LocalRadioButton.IsEnabled = false;
            }



            if (tagWindow != "IpAddressInfoTag") //存在自定义标签
            {
                string sqlTemp = $"SELECT COUNT(*) FROM WindowTag WHERE WindowName ='{tagWindow}'";

                var num = DbClass.ExecuteScalarTableNum(sqlTemp);

                if (num > 0)//存在本地自定义标签
                {
                    LocalRadioButton.IsChecked = true;

                    var tags = DbClass.LoadWindowTag(tagWindow);

                    if (tags != null)
                    {
                       
                        dynamic settings = JsonConvert.DeserializeObject(tags);

                        TagA.Text = settings.TagA;
                        TagB.Text = settings.TagB;
                        TagC.Text = settings.TagC;
                        TagD.Text = settings.TagD;
                        TagE.Text = settings.TagE;
                        TagF.Text = settings.TagF;

                    }

                }
                else
                {
                    sqlTemp = $"SELECT COUNT(*) FROM WindowTag WHERE WindowName ='IpAddressInfoTag'";

                    num = DbClass.ExecuteScalarTableNum(sqlTemp);

                    if (num > 0)
                    {
                        GlobalRadioButton.IsChecked = true;

                        var tags = DbClass.LoadWindowTag("IpAddressInfoTag");

                        if (tags != null)
                        {
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
            else//全局标签或者默认标签
            {

                string sqlTemp = $"SELECT COUNT(*) FROM WindowTag WHERE WindowName ='IpAddressInfoTag'";

                var num = DbClass.ExecuteScalarTableNum(sqlTemp);

                if (num > 0)
                {
                    GlobalRadioButton.IsChecked = true;

                    var tags = DbClass.LoadWindowTag("IpAddressInfoTag");

                    if (tags != null)
                    {
                      
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string tagWindow;

            if (GlobalRadioButton.IsChecked == true)//全局
            {

                tagWindow = "IpAddressInfoTag";

            }
            else//本地
            {
                tagWindow = "IpAddressInfoTag" + DataBridge.DataBridge.NetworkTableName;
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
        /// <summary>
        /// 重置标签
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            string tagWindow;
            string message;
            if (LocalRadioButton.IsChecked == true)
            {

                tagWindow = "IpAddressInfoTag" + DataBridge.DataBridge.NetworkTableName;
                message = "你正在重置该网段独的有自定义标签，是否继续？";

            }
            else
            {
                tagWindow = "IpAddressInfoTag";
                message = "你正在重置全局网段信息自定义标签，是否继续？";
            }

            MessageBoxResult result = MessageBox.Show(message, "重置标签", MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                string sql = $"DELETE FROM  WindowTag  WHERE WindowName = '{tagWindow}'";

                GlobalVariables.DbService.ExecuteNonQuery(sql);
            }
        }



        /// <summary>
        /// 加载网段独有标签
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LocalRadioButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (LocalRadioButton.IsChecked == true)
            {
                var tagWindow = "IpAddressInfoTag" + DataBridge.DataBridge.NetworkTableName;

                var tags = DbClass.LoadWindowTag(tagWindow);

                if (tags != null)
                {
                    dynamic settings = JsonConvert.DeserializeObject(tags);

                    TagA.Text = settings.TagA;
                    TagB.Text = settings.TagB;
                    TagC.Text = settings.TagC;
                    TagD.Text = settings.TagD;
                    TagE.Text = settings.TagE;
                    TagF.Text = settings.TagF;

                }
                else
                {
                    TagA.Text=String.Empty;
                    TagB.Text = String.Empty;
                    TagC.Text = String.Empty;
                    TagD.Text = String.Empty;
                    TagE.Text = String.Empty;
                    TagF.Text = String.Empty;
                }
            }


        }

        /// <summary>
        /// 加载全局标签
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GlobalRadioButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (GlobalRadioButton.IsChecked == true)
            {
                var tags = DbClass.LoadWindowTag("IpAddressInfoTag");

                if (tags != null)
                {
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
}
