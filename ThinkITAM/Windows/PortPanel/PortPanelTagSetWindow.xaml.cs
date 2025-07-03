using System.ComponentModel;
using System.Windows;
using Newtonsoft.Json;
using ThinkITAM.DatabaseOperation;

namespace ThinkITAM.Windows.PortPanel
{
    /// <summary>
    /// PortTagSetWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PortPanelTagSetWindow : Window
    {

        public PortPanelTagSetWindow()
        {
            InitializeComponent();
        }

        private void PortTagSetWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            string tagWindow = "PortPanelTag";


            string sqlTemp = $"SELECT COUNT(*) FROM WindowTag WHERE Window ='{tagWindow}'";

            var num = DbClass.ExecuteScalarTableNum(sqlTemp);

            if (num > 0)
            {
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

        }

        private void PortTagSetWindow_OnClosing(object? sender, CancelEventArgs e)
        {

        }



        private void ResetButton_OnClick(object sender, RoutedEventArgs e)
        {

            string message = "你正在重置终端自定义标签，是否继续？";

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
            string tagWindow = "PortPanelTag";

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


        }
    }
}
