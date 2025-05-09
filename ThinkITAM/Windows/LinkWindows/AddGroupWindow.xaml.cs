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
using ThinkITAM.FunctionClass;

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

            var countNum = dbClass.ExecuteScalarTableNum(sqlTemp, dbClass.connection);

            if (countNum == 0)
            {
                    //创建资产ID
                string assetId = AssetIdCreate.CreateAssetId(deviceRoomQrId+groupName);

                //创建资产二维码,0为机房，1为机柜，2为设备
                string cabinetId = "1" + FunctionClass.AssetCodeClass.GenerateChecksum(assetId).ToUpper();

                string sql = $"INSERT INTO DeviceCabinet(CabinetId,DeviceRoomQrId,CabinetName,Position,Note) VALUES('{cabinetId}','{deviceRoomQrId}','{groupName}','{position}','{note}')";

                dbClass.ExecuteQuery(sql);

                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("机柜/分组名重复，请误重复添加", "有问题需要注意", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        else
        {
          MessageBox.Show("请输入正确的机房和机柜/组名","信息不完整",MessageBoxButton.OK,MessageBoxImage.Warning);
        }




    }

    private void AddGroupWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\Address_database.db";
        dbClass = new DbClass(dbFilePath);
        dbClass.OpenConnection();

        LoadDeviceRoomInfo();
        RoomCombobox.ItemsSource = deviceRoomInfo;
    }



    /// <summary>
    /// 机房信息列表
    /// </summary>
    private ObservableCollection<ViewModes.LinkManage.DeviceRoomClass> deviceRoomInfo = new ObservableCollection<ViewModes.LinkManage.DeviceRoomClass>();


    /// <summary>
    /// 加载机房信息
    /// </summary>
    private void LoadDeviceRoomInfo()
    {
        deviceRoomInfo.Clear();

        string query = "SELECT * FROM DeviceRoom;";

        SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
        SQLiteDataReader reader = command.ExecuteReader();

        

        while (reader.Read())
        {
            ViewModes.LinkManage.DeviceRoomClass info = new ViewModes.LinkManage.DeviceRoomClass();

            info.DeviceRoomQrId = reader["DeviceRoomQrId"].ToString();
            info.Name = reader["RoomName"].ToString();
            info.Location = reader["Location"].ToString();
            info.User = reader["User"].ToString();
            info.UserPhone = reader["UserPhone"].ToString();
            info.Note = reader["Note"].ToString();

            

            deviceRoomInfo.Add(info);
        }




    }

}
