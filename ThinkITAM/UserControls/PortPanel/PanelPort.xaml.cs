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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThinkITAM.ChildrenWindows.LinkWindows;
using ThinkITAM.ChildrenWindows.PortPanel;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.FunctionClass;
using ThinkITAM.UserControls.LinkPage;
using ThinkITAM.ViewModes.DevicePortManage;
using ThinkITAM.ViewModes.LinkManage;
using ThinkITAM.ViewModes.PortPanel;

namespace ThinkITAM.UserControls.PortPanel
{
    /// <summary>
    /// EthernetPort.xaml 的交互逻辑
    /// </summary>
    public partial class PanelPort : UserControl
    {
        public static readonly DependencyProperty RackInfoProperty =
            DependencyProperty.Register("PortPanelInfo", typeof(PortPanelClass), typeof(Port), new PropertyMetadata(null));



        public PortPanelClass PortPanelInfo
        {
            get
            {
                return (PortPanelClass)GetValue(RackInfoProperty);
            }
            set
            {
                SetValue(RackInfoProperty, value);
            }
        }



        public PanelPort()
        {
            InitializeComponent();
            this.DataContext = PortPanelInfo;


        }

        private void Port_OnLoaded(object sender, RoutedEventArgs e)
        {




        }


        /// <summary>
        /// 端口被选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PortButton_OnClick(object sender, RoutedEventArgs e)
        {

            // 获取 rack 层的 DataContext
            var rackDataContext = (sender as FrameworkElement)?.FindAncestor<Rack>()?.DataContext;

            if (rackDataContext != null)
            {
                var info = rackDataContext as MdfRackClass;

                if (info != null)
                {
                    DataBridge.DataBridge.SelectRackId.Clear();
                    DataBridge.DataBridge.SelectRackId.Add(info.RackId);
                }

            }

            // 获取 Slot 层的 DataContext
            var slotDataContext = (sender as FrameworkElement)?.FindAncestor<MDF>()?.DataContext;
            if (slotDataContext != null)
            {
                var slotInfo = slotDataContext as SlotClass;

                PortClass port = (PortClass)this.DataContext;

                DataBridge.DataBridge.SelectPortInfo = port;

                DataBridge.DataBridge.SelectSlotInfo = slotInfo;

            }

            switch (DataBridge.DataBridge.LinkManageMode)
            {
                case 0:
                    
                    MessageBox.Show("TEMP");

                    break;


                case 1:
                    MessageBox.Show("PER");
                    break;


                case 2:
                    MessageBox.Show("LINK");
                    break;
            }





        }




        /// <summary>
        /// 端口标签被选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TagButton_OnClick(object sender, RoutedEventArgs e)
        {
            PortClass port = (PortClass)this.DataContext;



            PanelPortTagModify add = new PanelPortTagModify(port);

            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                add.Owner = window;
            }



            if (add.ShowDialog() == true)
            {
                DataBridge.DataBridge.PortPanelModifyTagList.Add($"{port.PortIndex}");
                // 当子窗口关闭后执行这里的代码

            }
        }

        private void ColorTagSet_OnClick(object sender, RoutedEventArgs e)
        {
            PortClass port = (PortClass)this.DataContext;



            PortPanelColorSetWindow add = new PortPanelColorSetWindow(port);

            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                add.Owner = window;
            }


            if (add.ShowDialog() == true)
            {
                DataBridge.DataBridge.PortPanelModifyTagList.Add($"{port.PortIndex}");
                // 当子窗口关闭后执行这里的代码

            }


        }



    }
}
