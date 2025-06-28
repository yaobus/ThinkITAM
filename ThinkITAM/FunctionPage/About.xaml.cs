using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ThinkITAM.Functions.FunctionClass;

namespace ThinkITAM.FunctionPage
{
    /// <summary>
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class About : UserControl
    {
        public About()
        {
            InitializeComponent();
        }


        private void Sipam_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            //如果按下的是鼠标左键，则在浏览器中打开SIPAM官网
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                OpenUrlClass.OpenUrlInSpecificBrowser("https://github.com/yaobus/SIPAM", null);
            }
        }

        private void IpamNote_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            //如果按下的是鼠标左键，则在浏览器中打开SIPAM官网
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                OpenUrlClass.OpenUrlInSpecificBrowser("https://github.com/yaobus/IPAM-NOTE", null);
            }
        }

        private void About_OnLoaded(object sender, RoutedEventArgs e)
        {
            VersionTextBlock.Text = DataBridge.DataBridge.Version;
        }
    }
}
