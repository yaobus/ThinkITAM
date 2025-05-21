using System;
using System.Collections;
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
using Newtonsoft.Json;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.ViewModels.AssetManage;
using ThinkITAM.ViewModels.LinkManage;
using ThinkITAM.ViewModels.NetworkManage;

namespace ThinkITAM.Windows.DevicePortManage;
/// <summary>
/// DeviceEditWindow.xaml 的交互逻辑
/// </summary>
public partial class DeviceEditWindow : Window
{
    public DeviceEditWindow()
    {
        InitializeComponent();
    }

    private void DeviceEditWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        LoadRoomInfo();
        LoadTags();
        LoadHierarchyInfo();
        LoadDeviceInfo(DataBridge.DataBridge.SelectDeviceTableInfo.AssetId);

    }

    private void LoadDeviceInfo(string assetId)
    {
        var sql = $"SELECT * FROM Devices WHERE AssetId = '{assetId}'";

        var rows = GlobalVariables.DbService.ExecuteQuery(sql);

        var info = new DeviceEditViewModel();

        foreach (var row in rows)
        {
            info.AssetNumber = row["AssetNumber"].ToString();
            info.Description = row["Description"].ToString();
            info.EnableDate = row["EnableDate"].ToString();
            info.DeviceRoom = row["DeviceRoom"].ToString();
            info.DeviceCabinet = row["DeviceCabinet"].ToString();
            info.TagA = row["TagA"].ToString();
            info.TagB = row["TagB"].ToString();
            info.TagC = row["TagC"].ToString();
            info.TagD = row["TagD"].ToString();
            info.TagE = row["TagE"].ToString();
            info.TagF = row["TagF"].ToString();

            if (info.TagA != null)
            {
                TagA.SelectedIndex = parentList.IndexOf(info.TagA);
            }

            if (info.TagB != null)
            {
                TagB.SelectedIndex = childList.IndexOf(info.TagB);
            }

            if (info.DeviceRoom != null)
            {

                // 使用LINQ查询找到第一个匹配的元素，并获取其索引。
                var index = roomInfos.IndexOf(roomInfos.FirstOrDefault(item => item.DeviceRoomQrId == info.DeviceRoom));
                
                RoomCombobox.SelectedIndex = index;
            }

            if (info.DeviceCabinet != null)
            {
                // 使用LINQ查询找到第一个匹配的元素，并获取其索引。
                var index = cabinetInfos.IndexOf(cabinetInfos.FirstOrDefault(item => item.CabinetId == info.DeviceCabinet));

                CabinetCombobox.SelectedIndex = index;
            }

        }

        this.DataContext = info;
    }



    private ObservableCollection<DeviceRoomClass> roomInfos = new ObservableCollection<DeviceRoomClass>();

    private void LoadRoomInfo()
    {
        RoomCombobox.ItemsSource = roomInfos;

        roomInfos.Clear();
        string query = "SELECT * FROM DeviceRoom WHERE Del != 1 OR Del IS NULL";

        var rows = GlobalVariables.DbService.ExecuteQuery(query);

        foreach (var row in rows)
        {
            DeviceRoomClass room = new DeviceRoomClass();
            room.DeviceRoomQrId = row["DeviceRoomQrId"].ToString();
            room.Name = row["RoomName"].ToString();
            room.Location = row["Location"].ToString();
            room.User = row["User"].ToString();
            room.UserPhone = row["UserPhone"].ToString();
            room.Note = row["Note"].ToString();
            roomInfos.Add(room);
        }



    }



    /// <summary>
    /// 加载自定义标签
    /// </summary>
    private void LoadTags()
    {

        var tags = DbClass.LoadWindowTag("AddDevice");

        if (tags != null)
        {
            dynamic settings = JsonConvert.DeserializeObject(tags);

            LabelA.Content = settings.TagA + ":";
            LabelB.Content = settings.TagB + ":";
            LabelC.Content = settings.TagC + ":";
            LabelD.Content = settings.TagD + ":";
            LabelE.Content = settings.TagE + ":";
            LabelF.Content = settings.TagF + ":";
        }

    }



    private List<string> parentList = new List<string>();


    /// <summary>
    /// 加载组织信息
    /// </summary>
    private void LoadHierarchyInfo()
    {
        parentList.Clear();

        string query = "SELECT DISTINCT TagA FROM Devices;";


        var rows = GlobalVariables.DbService.ExecuteQuery(query);

        foreach (var row in rows)
        {
            parentList.Add(row["TagA"].ToString());
        }


        TagA.ItemsSource = parentList;


    }


    private void UseDepartment_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }


    private List<string> childList = new List<string>();

    private void TagA_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        childList.Clear();
        TagB.ItemsSource = null;
        if (TagA.SelectedIndex != -1)
        {
            string sql = $"SELECT DISTINCT TagB FROM Devices WHERE TagA='{parentList[TagA.SelectedIndex]}'";

            var rows = GlobalVariables.DbService.ExecuteQuery(sql);

            foreach (var row in rows)
            {
                childList.Add(row["TagB"].ToString());
            }


            TagB.ItemsSource = childList;

        }
    }

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        var id = DataBridge.DataBridge.SelectDeviceTableInfo.AssetId;

        var description = Description.Text;

        if (!string.IsNullOrWhiteSpace(description))
        {
            var enableDate = EnableDate.Text;
            var deviceRoom = roomInfos[RoomCombobox.SelectedIndex].DeviceRoomQrId;
            var deviceCabinet = cabinetInfos[CabinetCombobox.SelectedIndex].CabinetId;
            var tagA = TagA.Text;
            var tagB = TagB.Text;
            var tagC = TagC.Text;
            var tagD = TagD.Text;
            var tagE = TagE.Text;
            var tagF = TagF.Text;


            var sql = $"UPDATE  Devices  SET  Description  = '{description}', EnableDate='{enableDate}',DeviceRoom='{deviceRoom}',DeviceCabinet='{deviceCabinet}',TagA='{tagA}',TagB='{tagB}',TagC='{tagC}',TagD='{tagD}',TagE='{tagE}',TagF='{tagF}' WHERE AssetId = '{id}'";


            GlobalVariables.DbService.ExecuteNonQuery(sql);

            this.DialogResult = true;
        }
        else
        {
            MessageBox.Show("请输入设备描述", "缺少必要信息", MessageBoxButton.OK);
        }




    }


    private ObservableCollection<CabinetClass> cabinetInfos = new ObservableCollection<CabinetClass>();


    private void RoomCombobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        CabinetCombobox.ItemsSource = cabinetInfos;

        if (RoomCombobox.SelectedIndex != -1)
        {
            CabinetCombobox.IsEnabled = true;
            cabinetInfos.Clear();

            string roomId = roomInfos[RoomCombobox.SelectedIndex].DeviceRoomQrId;

            string query = $"SELECT * FROM  DeviceCabinet WHERE DeviceRoomQrId = '{roomId}' AND (Del != 1 OR Del IS NULL)";


            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            foreach (var row in rows)
            {
                CabinetClass cabinet = new CabinetClass();
                cabinet.CabinetId = row["CabinetId"].ToString();
                cabinet.Name = row["CabinetName"].ToString();
                cabinet.Position = row["Position"].ToString();
                cabinet.Note = row["Note"].ToString();

                cabinetInfos.Add(cabinet);
            }


        }
        else
        {
            CabinetCombobox.IsEnabled = false;
        }
    }


    /// <summary>
    /// 机柜ID
    /// </summary>
    private string cabinetId = "";
    private void CabinetCombobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CabinetCombobox.SelectedIndex != -1)
        {
            cabinetId = cabinetInfos[CabinetCombobox.SelectedIndex].CabinetId;

        }
        else
        {
            cabinetId = "";
        }
    }
}
