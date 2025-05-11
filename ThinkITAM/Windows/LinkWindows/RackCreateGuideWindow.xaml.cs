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
using ThinkITAM.FunctionClass;
using ThinkITAM.ViewModels.AssetManage;
using ThinkITAM.ViewModels.LinkManage;
using Newtonsoft.Json;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.FunctionClass;

namespace ThinkITAM.Windows.LinkWindows;
/// <summary>
/// RackCreateGuideWindow.xaml 的交互逻辑
/// </summary>
public partial class RackCreateGuideWindow : Window
{
    public RackCreateGuideWindow()
    {
        InitializeComponent();
    }

    private DbClass dbClass;

    private ObservableCollection<int> slotNumList = new ObservableCollection<int>();

    private void SlotNumSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        slotNumList.Clear();
        rackSlots.Clear();
        for (int i = 1; i < SlotNumSlider.Value + 1; i++)
        {
            slotNumList.Add(i);
        }


    }

    private void RackCreateGuideWindow_OnLoaded(object sender, RoutedEventArgs e)
    {


        SlotCombobox.ItemsSource = slotNumList;
        PortTypeCombobox.SelectedIndex = 0;
        SlotDataGrid.ItemsSource = rackSlots;

        //加载机房列表
        LoadRoomInfo();


    }



    private ObservableCollection<DeviceRoomClass> roomInfos = new ObservableCollection<DeviceRoomClass>();

    private void LoadRoomInfo()
    {
        RoomCombobox.ItemsSource = roomInfos;
        RoomCombobox2.ItemsSource = roomInfos;
        roomInfos.Clear();
        string query = "SELECT * FROM DeviceRoom";

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
    /// 端口类型选择改变时事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PortTypeCombobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        int index = PortTypeCombobox.SelectedIndex;

        if (index != 0 && index != -1)
        {

            SlotNumSlider.IsEnabled = true;
            SlotCombobox.IsEnabled = true;

        }
        else//如果是以太网口
        {

            SlotNumSlider.Value = 1;
            SlotNumSlider.IsEnabled = false;

            SlotCombobox.SelectedIndex = 0;
            SlotCombobox.IsEnabled = false;
        }
    }



    private void SlotCombobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SlotCombobox.SelectedIndex != -1)
        {
            //SlotName.Text = "slot-" + SlotCombobox.Text;
        }



    }

    /// <summary>
    /// 配线架的槽位配置信息
    /// </summary>
    private ObservableCollection<SlotClass> rackSlots =
        new ObservableCollection<SlotClass>();



    /// <summary>
    /// 添加槽位按钮点击事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AddSlotButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (SlotCombobox.SelectedIndex != -1)
        {

            SlotClass slot = new SlotClass();
            slot.SlotIndex = SlotCombobox.Text;
            slot.SlotName = SlotName.Text;
            slot.SlotType = PortTypeCombobox.Text;

            if (slot.SlotType != "Empty")
            {
                slot.PortCount = Convert.ToInt32(PortNumSlider.Value);

            }
            else
            {
                slot.PortCount = 0;
            }




            slot.SlotTag = $"Slot-{slot.SlotIndex}";
            rackSlots.Add(slot);

            slotNumList.Remove(Convert.ToInt32(SlotCombobox.Text));
            SlotCombobox.ItemsSource = slotNumList;
        }







    }





    private ObservableCollection<CabinetClass> cabinetInfos = new ObservableCollection<CabinetClass>();
    /// <summary>
    /// 房间选择改变事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RoomCombobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        CabinetCombobox.ItemsSource = cabinetInfos;

        if (RoomCombobox.SelectedIndex != -1)
        {
            CabinetCombobox.IsEnabled = true;
            cabinetInfos.Clear();

            string roomId = roomInfos[RoomCombobox.SelectedIndex].DeviceRoomQrId;

            string query = $"SELECT * FROM  DeviceCabinet WHERE DeviceRoomQrId = '{roomId}'";


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

    private string cabinetId = "";
    private string cabinetId2 = "";
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





    private ObservableCollection<CabinetClass> cabinetInfos2 = new ObservableCollection<CabinetClass>();
    private void RoomCombobox2_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        CabinetCombobox2.ItemsSource = cabinetInfos2;

        if (RoomCombobox2.SelectedIndex != -1)
        {
            CabinetCombobox2.IsEnabled = true;
            cabinetInfos2.Clear();

            string roomId = roomInfos[RoomCombobox2.SelectedIndex].DeviceRoomQrId;

            string query = $"SELECT * FROM  DeviceCabinet WHERE DeviceRoomQrId = '{roomId}'";


            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            foreach (var row in rows)
            {
                                    CabinetClass cabinet = new CabinetClass();
                    cabinet.CabinetId = row["CabinetId"].ToString();
                    cabinet.Name = row["CabinetName"].ToString();
                    cabinet.Position = row["Position"].ToString();
                    cabinet.Note = row["Note"].ToString();

                    cabinetInfos2.Add(cabinet);
            }

 
        }
        else
        {
            CabinetCombobox2.IsEnabled = false;
        }

    }







    /// <summary>
    /// 保存机架信息按钮点击事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SaveRack_OnClick(object sender, RoutedEventArgs e)
    {

        var info = CheckInput();


        if (info.Item1 != 0)
        {
            MessageBox.Show(info.Item2, "注意！", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else
        {
            string rackName = RackName.Text;
            string rackNote = RackNote.Text;
            int slotCount = rackSlots.Count;


            //ObservableCollection<RackInfo> rackCreateInfos = new ObservableCollection<RackInfo>();


            RackInfo rackCreateInfo = new RackInfo();
            rackCreateInfo.rackName = rackName;
            rackCreateInfo.rackNote = rackNote;
            rackCreateInfo.slotCount = slotCount;


            rackCreateInfo.slotInfos = rackSlots;




            //保存设备
            SaveRackToDatabase(rackCreateInfo, rackSlots);

        }


    }




    /// <summary>   
    /// 保存机架信息到数据库
    /// </summary>
    private void SaveRackToDatabase(RackInfo rackCreateInfos, ObservableCollection<SlotClass> rackSlots)
    {

        string rackStr1;

        
        //创建资产ID字符串
        rackStr1 = $"3{AssetCodeClass.GenerateChecksum(AssetIdCreate.CreateAssetId(rackCreateInfos.rackName + DateTime.Now.ToString("yyyyMMddHHmmss"))).ToUpper()}";


        string infos = JsonConvert.SerializeObject(rackCreateInfos.slotInfos);


        //创建资产二维码ID,0为机房，1为机柜，2为设备,3为机架
        string rackId = rackStr1;

        //第一步，写入机架信息到机架总表

        string sql = $"INSERT INTO \"Racks\" (\"RackId\", \"CabinetId\", \"RackName\", \"RackNote\", \"SlotInfos\",  \"SlotCount\") VALUES ('{rackId}', '{cabinetId}', '{rackCreateInfos.rackName}', '{rackCreateInfos.rackNote}','{infos}', '{rackCreateInfos.slotCount}')";


        GlobalVariables.DbService.ExecuteNonQuery(sql);

        //第二步，创建机架设备表

        DbClass.CreateDynamicsTableIfNotExists(rackStr1, 0);


        int uid = 1;
        //第三步，写入机架槽位及端口信息
        foreach (var slot in rackSlots)
        {

            for (int i = 1; i < slot.PortCount + 1; i++)
            {
                string sql2 = $"INSERT INTO \"Ra_{rackStr1}\" (\"UID\",\"SlotId\",\"PortId\",\"PortType\") VALUES ({uid},{slot.SlotIndex}, {i},'{slot.SlotType}')";

               
                GlobalVariables.DbService.ExecuteNonQuery(sql2);

                uid++;
            }

        }






        this.DialogResult = true;

    }


    /// <summary>
    /// 检查输入信息
    /// </summary>
    /// <returns></returns>
    private (int, string) CheckInput()
    {
        int index = 0;

        string message = "当前存在以下问题需要解决:\r";

        if (slotNumList.Count > 0 || rackSlots.Count == 0)
        {
            index++;

            message += index.ToString() + ":必须先配置所有槽位信息\r";
        }


        if (RackName.Text.Replace(" ", "").Length < 2)
        {
            index++;

            message += index.ToString() + ":机架名称不得为空\r";
        }



        if (cabinetId == "")
        {
            index++;

            message += index.ToString() + ":必须选择设备所在的机房和机柜\r";
        }

        if (CreateEndDevice.IsChecked == true)
        {
            if (RackName2.Text.Replace(" ", "").Length < 2)
            {
                index++;

                message += index.ToString() + ":对端机架名称不得为空\r";
            }


            if (cabinetId2 == "")
            {
                index++;

                message += index.ToString() + ":必须选择对端设备所在的机房和机柜\r";
            }

        }




        return (index, message);
    }

    private void CabinetCombobox2_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CabinetCombobox2.SelectedIndex != -1)
        {
            cabinetId2 = cabinetInfos2[CabinetCombobox2.SelectedIndex].CabinetId;

        }
        else
        {
            cabinetId2 = "";
        }
    }
}
