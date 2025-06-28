using System.Windows;
using System.Windows.Controls;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.ViewModels.LinkManage;

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
