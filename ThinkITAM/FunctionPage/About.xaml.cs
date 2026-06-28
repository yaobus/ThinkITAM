using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.Windows.AboutWindow;

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




        private void About_OnLoaded(object sender, RoutedEventArgs e)
        {
            VersionTextBlock.Text = DataBridge.DataBridge.Version;
        }

        private void Chip_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string uri = "ms-windows-store://review/?ProductId=9P4L15BSJWR0"; 
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(uri)
                {
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                // 可选：处理异常（例如系统不支持该协议）
                Console.WriteLine(ex);
            }
        }

        private void Sipam_OnClick(object sender, RoutedEventArgs e)
        {

            try
            {
                OpenUrlClass.OpenUrlInSpecificBrowser("https://github.com/yaobus/ThinkITAM", null);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
              
            }


        }

        private void IpamNote_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenUrlClass.OpenUrlInSpecificBrowser("https://github.com/yaobus/IPAM-NOTE", null);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                
            }
        }


        /// <summary>
        /// 显示反馈页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Feedback_OnClick(object sender, RoutedEventArgs e)
        {
            var feedbackPage = new FeedbackPage();
            var window = Window.GetWindow(this);
            if (window != null)
            {
                feedbackPage.Owner = window;
            }

            feedbackPage.ShowDialog();

        }

        private void DonateButton_OnClick(object sender, RoutedEventArgs e)
        {
            var donatePage = new DonatePage();

            var window = Window.GetWindow(this);
            if (window != null)
            {
                donatePage.Owner = window;
            }

            donatePage.ShowDialog();
        }
    }
}
