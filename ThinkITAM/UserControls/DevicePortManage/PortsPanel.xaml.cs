using System.Windows;
using System.Windows.Controls;
using ThinkITAM.DataBridge;
using ThinkITAM.ViewModels.DevicePortManage;

namespace ThinkITAM.UserControls.DevicePortManage;

public partial class PortsPanel : UserControl
{
    public PortsPanel()
    {
        InitializeComponent();
    }

    private void RemoveSlotButton_OnClick(object sender, RoutedEventArgs e)
    {
        var info = this.DataContext as DevicePortsViewModel;

        var port = info.Ports[0];

        

        if (info != null)
        {
            var result = MessageBox.Show("确认要删除该板卡吗？\r该操作不可逆！", "警告", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {

                var tableName = $"De_{DataBridge.DataBridge.SelectDeviceTableInfo.AssetId}";

                //检查该板卡是否存在端口在链路上
                var portType = port.PortType;
                var portSpeed = port.PortSpeed;
                var portSlotNumber = port.PortSlotNumber;

                var count = Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar($"SELECT COUNT(*) FROM {tableName} WHERE PortType='{portType}' AND PortSpeed='{portSpeed}' AND PortSlotNumber={portSlotNumber} AND OnTheLine>0"));


                if (count > 0)
                {
                    MessageBox.Show($"该板卡有{count}个端口位于链路上，无法删除板卡！", "错误", MessageBoxButton.OK);
                }
                else
                {
                    var sql = $"DELETE FROM {tableName} WHERE PortType='{portType}' AND PortSpeed='{portSpeed}' AND PortSlotNumber={portSlotNumber}";

                    GlobalVariables.DbService.ExecuteNonQuery(sql);

                    DataBridge.DataBridge.ChangedDevicePorts.Add(1);

                }


            }

        }
    }
}