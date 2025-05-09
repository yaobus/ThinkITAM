using ThinkITAM.DatabaseOperation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ThinkITAM.Windows.DevicePortManage
{
    /// <summary>
    /// AddDeviceWindowSet.xaml 的交互逻辑
    /// </summary>
    public partial class AddDeviceWindowSet : Window
    {


        public AddDeviceWindowSet()
        {
            InitializeComponent();
        }

        private void AddDeviceWindowSet_OnLoaded(object sender, RoutedEventArgs e)
        {


            var tags = DbClass.LoadWindowTag("AddDevice");

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

        private void AddDeviceWindowSet_OnClosing(object? sender, CancelEventArgs e)
        {
          
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

            DbClass.SaveWindowTag("AddDevice", json);

            this.DialogResult = true;
            this.Close();
        }

        private void ResetButton_OnClick(object sender, RoutedEventArgs e)
        {

            TagA.Text = "自定义标签A";    // 自定义标签1
            TagB.Text = "自定义标签B";    // 自定义标签1
            TagC.Text = "自定义标签C";    // 自定义标签1
            TagD.Text = "自定义标签D";    // 自定义标签1
            TagE.Text = "自定义标签E";    // 自定义标签1
            TagF.Text = "自定义标签F";    // 自定义标签1



        }
    }
}
