using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
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
using Newtonsoft.Json;

namespace ThinkITAM.Windows.NetworkManage
{
    /// <summary>
    /// AddNetworkWindowSet.xaml 的交互逻辑
    /// </summary>
    public partial class AddNetworkWindowSet : Window
    {
        private DbClass dbClass;

        public AddNetworkWindowSet()
        {
            InitializeComponent();
        }
       

        private void AddNetworkWindowSet_OnLoaded(object sender, RoutedEventArgs e)
        {
            string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\Address_database.db";

            dbClass = new DbClass(dbFilePath);
            dbClass.OpenConnection();

            var tags = dbClass.LoadWindowTag("AddNetwork");

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

            dbClass.SaveWindowTag("AddNetwork",json);
            
            this.DialogResult = true;
            this.Close();
        }

        private void ResetButton_OnClick(object sender, RoutedEventArgs e)
        {
            TagA.Text = "单位";    // 自定义标签1
            TagB.Text = "部门";    // 自定义标签1
            TagC.Text = "默认网关";    // 自定义标签1
            TagD.Text = "DNS";    // 自定义标签1
            TagE.Text = "NTP";    // 自定义标签1
            TagF.Text = "KMS";    // 自定义标签1
        }

        private void AddNetworkWindowSet_OnClosing(object? sender, CancelEventArgs e)
        {
           //dbClass.connection.Close();
        }
    }
}
