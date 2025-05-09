using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThinkITAM.FunctionClass;
using ThinkITAM.ViewModes.LinkManage;

namespace ThinkITAM.UserControls.LinkPage
{
    /// <summary>
    /// NodePort.xaml 的交互逻辑
    /// </summary>
    public partial class NodePort : UserControl
    {
        public NodePort()
        {
            InitializeComponent();
        }

        private void PortButton_OnClick(object sender, RoutedEventArgs e)
        {
            // 获取 rack 层的 DataContext
            var portInfo = (sender as FrameworkElement)?.FindAncestor<NodePort>()?.DataContext;

            if (portInfo != null)
            {
                var info = portInfo as PortLinkClass;
                


                Console.WriteLine(info.PortClass.PortType);

                var item = DataBridge.DataBridge.LinkManageList.Where(item => item.PortClass == info.PortClass).FirstOrDefault();

                if (item != null)
                {
                    Console.WriteLine(item.PortClass.UID);
                    item.PortClass.IsSelected = false;
                    item.PortClass.NodeIndex = 0;
                    DataBridge.DataBridge.LinkManageList.Remove(item);

                }
                

            }





        }
    }
}
