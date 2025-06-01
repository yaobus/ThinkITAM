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
using ThinkITAM.Windows.LinkWindows;
using ThinkITAM.Windows.PortPanel;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.FunctionClass;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.UserControls.LinkPage;
using ThinkITAM.ViewModels.DevicePortManage;
using ThinkITAM.ViewModels.LinkManage;
using ThinkITAM.ViewModels.PortPanel;
using Nmap.NET.Container;
using Nodify;

namespace ThinkITAM.UserControls.PortPanel
{
    /// <summary>
    /// EthernetPort.xaml 的交互逻辑
    /// </summary>
    public partial class PanelPort : UserControl
    {
        public static readonly DependencyProperty RackInfoProperty =
            DependencyProperty.Register("PortPanelInfo", typeof(PortPanelClass), typeof(LinkPage.Port), new PropertyMetadata(null));



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


                PortClass port = (PortClass)this.DataContext;

                if (port.OnTheLine != null && port.OnTheLine > 0)
                {
                   

                    foreach (var node in DbClass.GetLinkDetail(port.OnTheLine))
                    {
                        if (port.RackId == node.PortClass.RackId)
                        {
                            port.NodeIndex = node.PortClass.NodeIndex;
                           
                            port.IsSelected = true;
                        }


                        DataBridge.DataBridge.PortPanelLinkViewList.Add(node);
                    }



                }
                else
                {
                    DataBridge.DataBridge.PortPanelLinkViewList.Clear();
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
