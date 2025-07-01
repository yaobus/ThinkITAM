using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.ViewModels.NetworkManage;

namespace ThinkITAM.Windows.Computer
{
    /// <summary>
    /// NetworkEditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ComputerEditWindow : Window
    {
        public ComputerEditWindow()
        {
            InitializeComponent();
        }



        private void ComputerEditWindow_OnLoaded(object sender, RoutedEventArgs e)
        {




        }




        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {


        }




        /// <summary>
        /// 加载自定义标签
        /// </summary>
        private void LoadTags()
        {

            var tags = DbClass.LoadWindowTag("AddNetwork");

            if (tags != null)
            {
                dynamic settings = JsonConvert.DeserializeObject(tags);

                LabelA.Content = settings.TagA + ":";
                LabelB.Content = settings.TagB + ":";
                LabelC.Content = settings.TagC + ":";
                LabelD.Content = settings.TagD + ":";
                LabelE.Content = settings.TagE + ":";
                LabelF.Content = settings.TagF + ":";
            }

        }


    }
}
