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
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.FunctionClass;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.ViewModels.LinkManage;

namespace ThinkITAM.Windows.LinkWindows;
/// <summary>
/// AddGroupWindow.xaml 的交互逻辑
/// </summary>
public partial class AddGroupWindow : Window
{
    public AddGroupWindow(CabinetClass cabinetInfo=null)
    {
        InitializeComponent();
        if (cabinetInfo != null)
        {
            cabinet = cabinetInfo;

           this.DataContext = cabinet;

        }
    }

    private CabinetClass cabinet = null;
    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {

        if (!string.IsNullOrWhiteSpace(RoomCombobox.Text) && !string.IsNullOrWhiteSpace(GroupNameTextBox.Text))
        {
            var deviceRoomQrId = deviceRoomInfo[RoomCombobox.SelectedIndex].DeviceRoomQrId;
            var groupName = GroupNameTextBox.Text;
            var position = Position.Text;
            var note = Note.Text;

            if (cabinet != null)//UPDATE
            {


                var cabinetInfo = new
                {
                    CabinetId = cabinet.CabinetId,
                    DeviceRoomQrId = deviceRoomQrId,
                    CabinetName = groupName,
                    Position = position,
                    Note = note
                };

                var conditions = new { DeviceRoomQrId = deviceRoomQrId, CabinetId = cabinet.CabinetId };

                GlobalVariables.DbService.UpdateEntity("DeviceCabinet", cabinetInfo,conditions);

                this.DialogResult = true;



            }
            else
            {

                string sqlTemp = $"SELECT COUNT(*) FROM DeviceCabinet WHERE DeviceRoomQrId ='{deviceRoomQrId}' AND CabinetName='{groupName}'";

                var countNum = DbClass.ExecuteScalarTableNum(sqlTemp);

                if (countNum == 0)
                {
                    //创建资产ID
                    string assetId = AssetIdCreate.CreateAssetId(deviceRoomQrId + groupName);

                    //创建资产二维码,0为机房，1为机柜，2为设备
                    string cabinetId = "1" + AssetCodeClass.GenerateChecksum(assetId).ToUpper();


                    var cabinetInfo = new
                    {
                        CabinetId = cabinetId,
                        DeviceRoomQrId = deviceRoomQrId,
                        CabinetName = groupName,
                        Position = position,
                        Note = note
                    };

                    GlobalVariables.DbService.InsertEntity("DeviceCabinet", cabinetInfo);

                    this.DialogResult = true;
                }
                else
                {
                    MessageBox.Show("机柜/分组名重复，请误重复添加", "有问题需要注意", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            DataBridge.DataBridge.modifyRooms.Add("1");
        }
        else
        {
            MessageBox.Show("请输入正确的机房和机柜/组名", "信息不完整", MessageBoxButton.OK, MessageBoxImage.Warning);
        }




    }

    private void AddGroupWindow_OnLoaded(object sender, RoutedEventArgs e)
    {


        LoadDeviceRoomInfo();
        RoomCombobox.ItemsSource = deviceRoomInfo;

        if (cabinet != null)
        {
            int index = deviceRoomInfo.IndexOf(deviceRoomInfo.FirstOrDefault(d =>
                d.DeviceRoomQrId == cabinet.DeviceRoomQrId));

            if (index >= 0)
            {
                RoomCombobox.SelectedIndex = index;
            }

        }

    }



    /// <summary>
    /// 机房信息列表
    /// </summary>
    private ObservableCollection<ViewModels.LinkManage.DeviceRoomClass> deviceRoomInfo = new ObservableCollection<ViewModels.LinkManage.DeviceRoomClass>();


    /// <summary>
    /// 加载机房信息
    /// </summary>
    private void LoadDeviceRoomInfo()
    {
        deviceRoomInfo.Clear();

        string query = "SELECT * FROM DeviceRoom WHERE (Del != 1 OR Del IS NULL);";


        var rows = GlobalVariables.DbService.ExecuteQuery(query);

        foreach (var row in rows)
        {
            ViewModels.LinkManage.DeviceRoomClass info = new ViewModels.LinkManage.DeviceRoomClass();

            info.DeviceRoomQrId = row["DeviceRoomQrId"].ToString();
            info.Name = row["RoomName"].ToString();
            info.Location = row["Location"].ToString();
            info.User = row["User"].ToString();
            info.UserPhone = row["UserPhone"].ToString();
            info.Note = row["Note"].ToString();



            deviceRoomInfo.Add(info);
        }






    }

}
