using ThinkITAM.DatabaseOperation;
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
using ThinkITAM.DataBridge;
using ThinkITAM.FunctionClass;
using ThinkITAM.Functions.FunctionClass;

namespace ThinkITAM.Windows.LinkWindows;
/// <summary>
/// AddGroupWindow.xaml 的交互逻辑
/// </summary>
public partial class AddGroupWindow : Window
{
    public AddGroupWindow()
    {
        InitializeComponent();
    }

    private DbClass dbClass;

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        //检查同一个机房是否有同名的机柜
        //机房ID


        if (RoomCombobox.Text.Length > 2 && GroupNameTextBox.Text.Length > 2)
        {
            string deviceRoomQrId = deviceRoomInfo[RoomCombobox.SelectedIndex].DeviceRoomQrId;
            string groupName = GroupNameTextBox.Text;
            string note = Note.Text;
            string position = Position.Text;
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

                //string sql = $"INSERT INTO DeviceCabinet(CabinetId,DeviceRoomQrId,CabinetName,Position,Note) VALUES('{cabinetId}','{deviceRoomQrId}','{groupName}','{position}','{note}')";


                GlobalVariables.DbService.InsertEntity("DeviceCabinet", cabinetInfo);

                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("机柜/分组名重复，请误重复添加", "有问题需要注意", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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

        string query = "SELECT * FROM DeviceRoom;";


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
