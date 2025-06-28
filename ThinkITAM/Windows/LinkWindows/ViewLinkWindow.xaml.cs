using System.Collections.ObjectModel;
using System.Windows;
using ThinkITAM.ViewModels.LinkManage;

namespace ThinkITAM.Windows.LinkWindows
{
    /// <summary>
    /// ViewLinkWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ViewLinkWindow : Window
    {


        public ViewLinkWindow(ObservableCollection<PortLinkClass> nodes, int linkId)
        {
            InitializeComponent();
            ViewNodePanel.ItemsSource = nodes;
            this.Title = "查看链路:" + linkId;
        }
    }
}
