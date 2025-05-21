using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
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
using ThinkITAM.Windows.NetworkManage;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.ViewModels.AssetManage;
using ThinkITAM.ViewModels.DevicePortManage;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using static ThinkITAM.ViewModels.DevicePortManage.PortTypeClass;
using System.Reflection.Emit;
using ThinkITAM.DataBridge;
using ThinkITAM.UserControls.DevicePortManage;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;
using System.Security.Cryptography;
using ThinkITAM.ViewModels.LinkManage;

namespace ThinkITAM.Windows.DevicePortManage
{
    /// <summary>
    /// DeviceCreateGuideWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceCreateGuideWindow : Window
    {
        public DeviceCreateGuideWindow()
        {
            InitializeComponent();
            DataContext = this;
            MessageQueue = new SnackbarMessageQueue();
        }



        /// <summary>
        /// 端口配置信息列表，用于保存到预设库
        /// </summary>
        private ObservableCollection<PortTypeClass.PortInfoClass> portInfos =
            new ObservableCollection<PortTypeClass.PortInfoClass>();

        /// <summary>
        /// 增加端口按钮的点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddPortButton_OnClick(object sender, RoutedEventArgs e)
        {
            #region 端口信息，用于存储到预设

            string portType = "E";
            int slotNumber = 0;
            string portSpeed = "G";
            string portPrefix = "/0/";
            int firstNumber = 0;
            int portCount = 0;

            int index = PortType.SelectedIndex;

            if (index != -1)
            {
                switch (index)
                {
                    case 0:
                        portType = "E";
                        break;

                    case 1:
                        portType = "F";
                        break;

                    case 2:
                        portType = "M";
                        break;

                    case 3:
                        portType = "D";
                        break;

                }
            }

            //if (SlotNumber.SelectedIndex != -1)
            //{
            if (SlotNumber.Text != "" && SlotNumber.Text != null)
            {
                slotNumber =Convert.ToInt32( SlotNumber.Text);
            }


            //}

            //if (PortTag.Text != "" && PortTag.Text != null)
            //{
            portSpeed = PortSpeed.Text;
            //}

            //if (PortPrefix.Text != "" && PortPrefix.Text != null)
            //{
            portPrefix = PortPrefix.Text;
            //}

            //if (FirstNumber.Text != "" && FirstNumber.Text != null)
            //{
            firstNumber = Convert.ToInt32(FirstNumber.Text);
            //}


            portCount = Convert.ToInt32(PortSlider.Value);


            //生成配置预设信息，应用存储到预设库
            var conf = new PortTypeClass.PortInfoClass();
            conf.PortType = portType;
            conf.PortSpeed = portSpeed;
            conf.SlotNumber = slotNumber.ToString();
            conf.PortPrefix = portPrefix;
            conf.FirstNumber = firstNumber;
            conf.PortCount = portCount;
            portInfos.Add(conf);


            #endregion


            for (int i = firstNumber; i < portCount + firstNumber + 1; i++)
            {
                var info = new PortTypeClass.PortDetailedInfo();
                info.PortType = portType;
                info.PortSpeed = portSpeed;
                info.PortSlotNumber = slotNumber;

                var port = new UserControls.DevicePortManage.DevicePort();

                if (portType=="D")
                {
                    info.FullPortId = $"{slotNumber}{i}";
                }
                else
                {
                    info.FullPortId = $"{slotNumber}{portPrefix}{i}";
                }

                
                port.DataContext= info;
                PreviewPlan.Children.Add(port);

                //添加到配置表

            }







            Separator separator = new Separator();
            separator.Width = 10000; // 设置横线的宽度，根据需要调整
            separator.Opacity = 0.3;
            PreviewPlan.Children.Add(separator);


        }

        private ObservableCollection<string> portSpeeds = new ObservableCollection<string>();
        private ObservableCollection<string> portPrefix = new ObservableCollection<string>();

        /// <summary>
        /// 加载自定义端口标签
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeviceCreateGuideWindow_OnLoaded(object sender, RoutedEventArgs e)
        {

            PortSpeed.ItemsSource = portSpeeds;
            PortPrefix.ItemsSource = portPrefix;
            LoadPortSpeed();
            LoadTags();
            LoadRoomInfo();
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
        /// Snackbar消息
        /// </summary>
        public SnackbarMessageQueue MessageQueue
        {
            get;
            set;
        }

        /// <summary>
        /// 加载网络端口标签
        /// </summary>
        private void LoadPortSpeed()
        {
            portSpeeds.Clear();
            portPrefix.Clear();

            portSpeeds.Add("F");
            portSpeeds.Add("E");
            portSpeeds.Add("G");
            portSpeeds.Add("XG");
            portSpeeds.Add("25G");
            portSpeeds.Add("40G");
            portSpeeds.Add("100G");
            portSpeeds.Add("MEth");
            portSpeeds.Add("MGMT");

            portPrefix.Add("/0/");
            portPrefix.Add("/1/");
            portPrefix.Add("/2/");
            portPrefix.Add("/3/");
            portPrefix.Add("");
        }


        /// <summary>
        /// 加载磁盘标签
        /// </summary>
        private void LoadDiskTag()
        {
            portSpeeds.Clear();
            portSpeeds.Add("Slot");
            portSpeeds.Add("Disk");

            portPrefix.Clear();

        }

        private void PortType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            int index = PortType.SelectedIndex;
            if (index != -1 && this.IsLoaded == true)
            {

                switch (index)
                {
                    case 0: //网口
                        LoadPortSpeed();
                        SlotNumber.SelectedIndex = 0;
                        PortSpeed.SelectedIndex = 2;
                        PortPrefix.SelectedIndex = 0;


                        break;

                    case 1: //光口
                        LoadPortSpeed();

                        SlotNumber.SelectedIndex = 0;
                        PortSpeed.SelectedIndex = 2;
                        PortPrefix.SelectedIndex = 0;

                        break;

                    case 2: //管理口
                        LoadPortSpeed();
                        SlotNumber.SelectedIndex = 12;
                        PortSpeed.SelectedIndex = 7;
                        PortPrefix.SelectedIndex = 4;
                        break;

                    case 3: //硬盘位

                        LoadDiskTag();
                        SlotNumber.SelectedIndex = 12;
                        PortSpeed.SelectedIndex = 0;
                        break;

                }






            }





        }

        /// <summary>
        /// 清空配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearPortButton_OnClick(object sender, RoutedEventArgs e)
        {
            PresetList.SelectedIndex = -1;
            portInfos.Clear();
            PreviewPlan.Children.Clear();
        }

        /// <summary>
        /// 保存预设
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SavePreset_OnClick(object sender, RoutedEventArgs e)
        {
            string model = Model.Text;


            if (portInfos.Count > 0)
            {
                if (model.Replace(" ", "").Length > 0)
                {
                    string presetInfo = JsonConvert.SerializeObject(portInfos);

                    Console.WriteLine(presetInfo);

                    DbClass.SaveModelPreset(model, presetInfo);

                    MessageQueue.Enqueue("预设信息已保存");
                }
                else
                {
                    MessageBox.Show("预设配置信息对应的设备型号不得为空", "信息不完整", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }
            else
            {
                MessageBox.Show("预设配置信息为空，无法保存！", "信息不完整", MessageBoxButton.OK, MessageBoxImage.Warning);

            }





        }



        private ObservableCollection<string> modelPresetList = new ObservableCollection<string>();

        private void LoadPreset_OnClick(object sender, RoutedEventArgs e)
        {
            string model = Model.Text;

            if (model.Length > 0)
            {

                modelPresetList = DbClass.LoadModelPresetList(model);


                if (modelPresetList != null && modelPresetList.Count > 0)
                {
                    PresetList.ItemsSource = modelPresetList;
                    PresetList.SelectedIndex = -1;
                    PresetList.IsEnabled = true;
                    MessageQueue.Enqueue($"找到{modelPresetList.Count}条预设信息，请在'预设模版'选择预设");
                }
                else
                {
                    MessageQueue.Enqueue($"未找到{Model.Text}对应的预设信息");
                }
            }
            else
            {
                MessageBox.Show("型号参数不能为空！", "注意！", MessageBoxButton.OK, MessageBoxImage.Warning);
            }





        }



        /// <summary>
        /// 加载预设模版
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PresetList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = PresetList.SelectedIndex;


            if (index != -1)
            {

                MessageQueue.Enqueue($"正在加载预设请稍候");



                string model = modelPresetList[index];



                var infoJson = DbClass.LoadModelPreset(model);



                if (infoJson != null)
                {
                    //解析预设信息
                    AnalysisPreset(infoJson);

                }


            }
        }

        private void AnalysisPreset(string presetJson)
        {
            PreviewPlan.Children.Clear();


            ObservableCollection<PortTypeClass.PortInfoClass> deserializedArray =
                JsonConvert.DeserializeObject<ObservableCollection<PortTypeClass.PortInfoClass>>(presetJson);

            portInfos = deserializedArray;

            foreach (var info in deserializedArray)
            {

                string portType = info.PortType;


                for (int i = Convert.ToInt32(info.FirstNumber); i < info.PortCount + info.FirstNumber + 1; i++)
                {
                    var port = new DevicePort();

                    port.BorderThickness = new Thickness(5);
                    var portInfo = new PortDetailedInfo();


                    portInfo.PortType = portType;
                    portInfo.PortSpeed = info.PortSpeed;

                    portInfo.FullPortId = $"{info.SlotNumber}{info.PortPrefix}{i}";
                    
                    portInfo.Status = 0;

                    port.DataContext = portInfo;

                    PreviewPlan.Children.Add(port);

                }



                Separator separator = new Separator();
                separator.Width = 10000; // 设置横线的宽度，根据需要调整
                separator.Opacity = 0.3;
                PreviewPlan.Children.Add(separator);
            }

        }


        /// <summary>
        /// 查找资产
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindAsset_OnClick(object sender, RoutedEventArgs e)
        {
            if (modelPresetList != null)
            {
                modelPresetList.Clear();
            }

            
            DataBridge.DataBridge.SelectAssetInfo = null;
            FindAssetWindow findAsset = new FindAssetWindow();

            if (findAsset.ShowDialog() == true)
            {
                if (DataBridge.DataBridge.SelectAssetInfo != null)
                {
                    var info = DataBridge.DataBridge.SelectAssetInfo;

                    AssetType.DataContext = info;
                    DeviceType.DataContext = info;
                    AssetNumber.DataContext = info;
                    Model.DataContext = info;
                    User.DataContext = info;
                    UserPhone.DataContext = info;


                }


                //加载资产信息

            }
        }

        /// <summary>
        /// 调整端口数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PortSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int num = Convert.ToInt32(FirstNumber.Text);

            sliderValue = Convert.ToInt32(PortSlider.Value);

            LastNumber.Text = (num + sliderValue).ToString();

            PortCount.Text = $"共{(PortSlider.Value + 1).ToString()}个";
        }

        private int sliderValue = 0;

        /// <summary>
        /// 限制用户只能输入数字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FirstNumber_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            return text.All(char.IsDigit);
        }

        /// <summary>
        /// 失去焦点时，判断数字是否在合理范围
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FirstNumber_OnLostFocus(object sender, RoutedEventArgs e)
        {
            int num = Convert.ToInt32(FirstNumber.Text);

            if (num > 59)
            {
                MessageBox.Show("一般来说一台盒式交换机或板卡端口数量不超过60\r因此起始端口号应该小于60\r设定端口编号应符合现实设备情况", "编号异常", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                FirstNumber.Text = "0";
            }
            else
            {
                LastNumber.Text = (num + sliderValue).ToString();
                int maxNumber = 60 - num;
                PortSlider.Maximum = maxNumber;

            }
        }

        private void FirstNumber_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FirstNumber_OnLostFocus(null, null);
            }
        }




        private void EnableDate_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EnableDate.SelectedDate = DateTime.Today;
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {

            if (!string.IsNullOrWhiteSpace(Description.Text) && RoomCombobox.SelectedIndex !=-1 && CabinetCombobox.SelectedIndex!=-1)
            {
                if (portInfos.Count > 0)
                {
                    //1.保存设备到设备信息总表,然后初始化设备端口信息表
                    SaveDeviceInfoAddInitializationTable();
                }
                else
                {
                    MessageBox.Show("请先添加端口配置信息", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show($"设备名称或设备所在机房机柜信息未输入\r请输入必须的信息，以便于您对设备进行快速识别", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }




        }

        /// <summary>
        /// 将设备信息保存到数据库,然后创建对应的设备端口表并进行初始化
        /// </summary>
        private void SaveDeviceInfoAddInitializationTable()
        {
            var info = DataBridge.DataBridge.SelectAssetInfo;

            if (Model.Text.Length > 0)
            {

                string sqlTemp = $"SELECT COUNT(*) FROM Devices WHERE AssetId ='{info.AssetId}'";

                var countNum = DbClass.ExecuteScalarTableNum(sqlTemp);

                if (countNum == 0)
                {
                    var deviceInfo = new ViewModels.DatabaseEntity.Device.DeviceViewModel()
                    {
                        AssetId = info.AssetId,
                        AssetNumber = info.AssetNumber,
                        AssetType    = info.AssetType,
                        DeviceType = info.DeviceType,
                        Model = info.Model,
                        Description = Description.Text,
                        User = info.User,
                        UserPhone = info.UserPhone,
                        EnableDate = EnableDate.SelectedDate.ToString(),
                        DeviceRoom = roomInfos[RoomCombobox.SelectedIndex].DeviceRoomQrId,
                        DeviceCabinet = cabinetId,
                        TagA = TagA.Text,
                        TagB = TagB.Text,
                        TagC = TagC.Text,
                        TagD = TagD.Text,
                        TagE = TagE.Text,
                        TagF = TagF.Text

                    };



                    //插入设备信息到总表

                    GlobalVariables.DbService.InsertEntity("Devices", deviceInfo);

                   
                    //创建设备信息详表
                    DbClass.CreateDynamicsTableIfNotExists(info.AssetId, 2);



                    MessageQueue.Enqueue($"正在初始化设备信息表，请稍候");

                    //初始化设备信息详表
                    InitializationTable(info.AssetId, portInfos);

                    //关闭窗口
                    this.DialogResult = true;
                    


                }
                else
                {
                    MessageBox.Show($"资产{info.AssetNumber}已存在对应设备端口信息表，请勿重复添加", "设备重复", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }
            else
            {
                MessageBox.Show("必要信息不完整，请填写设备相关信息", "必要信息不完整", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            //判断设备是否已经存在


        }




        /// <summary>
        /// 初始化设备表
        /// </summary>
        /// <param name="assetId"></param>
        /// <param name="portInfo"></param>
        private void InitializationTable(string assetId, ObservableCollection<PortTypeClass.PortInfoClass> portInfo)
        {
            string table = $"De_{assetId}";

            int uid = 1;

            foreach (var info in portInfo)
            {
                string portType = info.PortType;
                string portSpeed = info.PortSpeed;
                int portSlotNumber = 0;

                if (info.SlotNumber != "")
                {
                    portSlotNumber = Convert.ToInt32(info.SlotNumber);
                }

                for (int i = Convert.ToInt32(info.FirstNumber); i < info.PortCount + info.FirstNumber + 1; i++)
                {
                    string portId = $"{info.PortPrefix}{i}";

                    var port = new
                    {
                        UID = uid,
                        PortType = portType,
                        PortSpeed = portSpeed,
                        PortSlotNumber = portSlotNumber,
                        PortId = portId,
                        PortStatus = 0
                    };



                    GlobalVariables.DbService.InsertEntity(table, port);
                    uid++;
                }



            }


        }



        /// <summary>
        /// 设置设备自定义字段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddDeviceWindowSet set = new AddDeviceWindowSet();


            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                set.Owner = window;
            }


            if (set.ShowDialog() == true)
            {

                LoadTags();

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
}
