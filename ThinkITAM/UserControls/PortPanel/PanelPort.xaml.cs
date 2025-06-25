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
using ThinkITAM.DataBridge;

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
            DataBridge.DataBridge.PortPanelLinkViewList.Clear();

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


        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            PortClass portClass = (PortClass)DataContext;

            // 获取触发事件的MenuItem
            var menuItem = sender as MenuItem;

            if (menuItem != null)
            {
                // 根据菜单项的不同进行相应的处理
                switch (menuItem.Tag)
                {
                    case "Delete":
                        // 执行选项1的操作

                        var onTheLine = portClass.OnTheLine;


                        if (onTheLine > 0)
                        {
                            MessageBox.Show("该端口已经在链路中，无法删除\r如需删除，请先从链路中删除该端口", "无法删除", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {

                            var result = MessageBox.Show("确定要撤销该端口吗\r该操作无法撤销！", "警告", MessageBoxButton.YesNo, MessageBoxImage.Question);

                            if (result == MessageBoxResult.Yes)
                            {
                                //从终端列表中删除该终端

                                var sql = $"DELETE FROM  Bu_{portClass.AssetId}  WHERE UID = {portClass.UID}";
                                
                                GlobalVariables.DbService.ExecuteNonQuery(sql);

                                

                                //更新选择的房间内端口信息
                                DataBridge.DataBridge.modifyPorts.Add("1");
                            }

                        }




                        break;
                }
            }
        }
    }
}
