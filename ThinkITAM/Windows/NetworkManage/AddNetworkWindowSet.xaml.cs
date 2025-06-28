using System.ComponentModel;
using System.Windows;
using Newtonsoft.Json;
using ThinkITAM.DatabaseOperation;

namespace ThinkITAM.Windows.NetworkManage
{
    /// <summary>
    /// AddNetworkWindowSet.xaml 的交互逻辑
    /// </summary>
    public partial class AddNetworkWindowSet : Window
    {


        public AddNetworkWindowSet()
        {
            InitializeComponent();
        }


        private void AddNetworkWindowSet_OnLoaded(object sender, RoutedEventArgs e)
        {


            var tags = DbClass.LoadWindowTag("AddNetwork");

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


        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {


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


            if (GlobalTag.IsChecked == true)
            {
                DbClass.SaveWindowTag("AddNetwork", json);

            }
            else
            {
                DbClass.SaveWindowTag($"AddNetwork{DataBridge.DataBridge.NetworkTableName}", json);
            }



            this.DialogResult = true;
            this.Close();
        }




        private void ResetButton_OnClick(object sender, RoutedEventArgs e)
        {
            TagA.Text = "TagA";    // 自定义标签1
            TagB.Text = "TagB";    // 自定义标签1
            TagC.Text = "TagC";// 自定义标签1
            TagD.Text = "TagD";    // 自定义标签1
            TagE.Text = "TagE";    // 自定义标签1
            TagF.Text = "TagF";    // 自定义标签1
        }

        private void AddNetworkWindowSet_OnClosing(object? sender, CancelEventArgs e)
        {
            //dbClass.connection.Close();
        }
    }
}
