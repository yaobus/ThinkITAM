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
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.EMMA;
using ThinkITAM.DataBridge;

namespace ThinkITAM.Windows.DevicePortManage;
/// <summary>
/// AddDeviceSlotWindow.xaml 的交互逻辑
/// </summary>
public partial class AddDeviceSlotWindow : Window
{
    public AddDeviceSlotWindow()
    {
        InitializeComponent();
    }

    private ObservableCollection<string> portSpeeds = new ObservableCollection<string>();
    private ObservableCollection<string> portPrefix = new ObservableCollection<string>();

    private void AddDeviceSlotWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        PortSpeed.ItemsSource = portSpeeds;
        PortPrefix.ItemsSource = portPrefix;
        LoadPortSpeed();

        LoadDeviceInfo();
    }


    private void LoadDeviceInfo()
    {

        AssetNumberTextBox.Text = DataBridge.DataBridge.SelectDeviceTableInfo.AssetNumber;

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

    private void FirstNumber_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            FirstNumber_OnLostFocus(null, null);
        }
    }


    private int sliderValue = 0;
    private void FirstNumber_OnLostFocus(object sender, RoutedEventArgs e)
    {
        int num = Convert.ToInt32(FirstNumber.Text);

        if (num > 52)
        {
            MessageBox.Show("一般来说一台盒式交换机或板卡端口数量不超过60\r因此起始端口号应该小于60\r设定端口编号应符合现实设备情况", "编号异常", MessageBoxButton.OK,
                MessageBoxImage.Information);
            FirstNumber.Text = "0";
        }
        else
        {
            LastNumber.Text = (num + sliderValue).ToString();
            //int maxNumber = 52 + num;
            //PortSlider.Maximum = maxNumber;

        }
    }

    private void FirstNumber_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {

        e.Handled = !IsTextAllowed(e.Text);
    }

    private static bool IsTextAllowed(string text)
    {
        return text.All(char.IsDigit);
    }




    private void PortSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        int num = Convert.ToInt32(FirstNumber.Text);

        sliderValue = Convert.ToInt32(PortSlider.Value - 1);

        LastNumber.Text = (num + sliderValue).ToString();

        PortCount.Text = $"共{(PortSlider.Value).ToString()}个";
    }

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        var tableName = $"De_{DataBridge.DataBridge.SelectDeviceTableInfo.AssetId}";

        //先查询是否存在同样的编号
        var portType = "E";
        var slotNumber = 0;
        var portSpeed = "G";
        var portPrefix = "/0/";
        var firstNumber = 0;
        var portCount = 0;

        var index = PortType.SelectedIndex;

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

        if (!string.IsNullOrWhiteSpace(SlotNumber.Text))
        {
            slotNumber = Convert.ToInt32(SlotNumber.Text);
        }

        portSpeed = PortSpeed.Text;

        portPrefix = PortPrefix.Text;

        firstNumber = Convert.ToInt32(FirstNumber.Text);

        portCount = Convert.ToInt32(PortSlider.Value);


        var sql = $"SELECT COUNT(*) FROM {tableName} WHERE PortType='{portType}' AND PortSpeed='{portSpeed}' AND PortSlotNumber='{slotNumber}'";

        var count = Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(sql));

        if (count > 0)
        {
            MessageBox.Show($"槽位{slotNumber}已存在端口类型为：{portType}，端口速度为：{portSpeed}的板卡，无法重复添加！\r如需编辑槽位{slotNumber}中的板卡，请删除该板卡后重新添加！", "参数重复", MessageBoxButton.OK);
        }
        else
        {
            //取出现有最大UID

            var maxUid = Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar($"SELECT MAX(UID) FROM {tableName};"));

            for (var i = 0; i < portCount; i++)
            {
                maxUid += 1;

                var portId = $"{portPrefix}{firstNumber + i}";

                var port = new
                {
                    UID = maxUid,
                    PortType = portType,
                    PortSpeed = portSpeed,
                    PortSlotNumber = slotNumber,
                    PortId = portId,
                    PortStatus = 0
                };

                GlobalVariables.DbService.InsertEntity(tableName, port);
            }

            this.DialogResult = true;
        }


        // 

    }
}
