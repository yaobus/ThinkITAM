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
using ThinkITAM.DataBridge;
using ThinkITAM.ViewModels.LinkManage;

namespace ThinkITAM.UserControls.Computer
{
    /// <summary>
    /// DevicePort.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceNode : UserControl
    {
        public DeviceNode()
        {
            InitializeComponent();
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


                        if (onTheLine>0)
                        {
                            MessageBox.Show("该设备已经在链路中，无法撤销部署\r如需撤销部署，请先从链路中删除该设备","无法删除",MessageBoxButton.OK,MessageBoxImage.Information);
                        }
                        else
                        {

                            var result = MessageBox.Show("确定要撤销该终端部署吗\r该操作无法撤销！", "警告", MessageBoxButton.YesNo, MessageBoxImage.Question);

                            if (result == MessageBoxResult.Yes)
                            {
                                //从终端列表中删除该终端

                                var sql = $"DELETE FROM  Computer  WHERE UID = {portClass.UID}";
                                GlobalVariables.DbService.ExecuteNonQuery(sql);

                                //从资产库中标记该终端为未使用
                                sql = $"UPDATE  Asset  SET  Deploy = NULL  WHERE  AssetId = '{portClass.AssetId}'";
                                GlobalVariables.DbService.ExecuteNonQuery(sql);

                                //更新选择的房间类终端信息
                                DataBridge.DataBridge.modifyDeployDevices.Add("1");
                            }

                        }




                        break;
                }
            }
        }
    }
}
