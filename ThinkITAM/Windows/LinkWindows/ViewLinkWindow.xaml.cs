using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ThinkITAM.ViewModels.LinkManage;

namespace ThinkITAM.Windows.LinkWindows
{
    /// <summary>
    /// ViewLinkWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ViewLinkWindow : Window
    {


        public ViewLinkWindow(ObservableCollection<PortLinkClass> nodes,int linkId )
        {
            InitializeComponent();
            ViewNodePanel.ItemsSource = nodes;
            this.Title = "查看链路:" + linkId;
        }
    }
}
