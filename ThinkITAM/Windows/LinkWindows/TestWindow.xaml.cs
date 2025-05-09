using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Drawing;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using ThinkITAM.UserControls.LinkPage;
using ThinkITAM.ViewModes.LinkManage;
using Brushes = System.Windows.Media.Brushes;

namespace ThinkITAM.Windows.LinkWindows
{
    /// <summary>
    /// TestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestWindow : Window
    {


        private const double ZoomIncrement = 0.1; // 每次缩放的比例增量
        private const double MinZoom = 1;      // 最小缩放比例
        private const double MaxZoom = 2.5;         // 最大缩放比例

        public TestWindow()
        {
            InitializeComponent();
        }

        private void TestWindow_OnLoaded(object sender, RoutedEventArgs e)
        {


        }



        /// <summary>
        /// 缩放面板尺寸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Panel_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // 获取当前的缩放比例
            double currentScale = scaleTransform.ScaleX;

            // 根据滚轮的方向调整缩放比例
            if (e.Delta > 0) // 向前滚动，放大
            {
                currentScale += ZoomIncrement;
            }
            else // 向后滚动，缩小
            {
                currentScale -= ZoomIncrement;
            }

            // 限制缩放比例在最小值和最大值之间
            currentScale = Math.Max(MinZoom, Math.Min(currentScale, MaxZoom));

            // 应用新的缩放比例
            scaleTransform.ScaleX = scaleTransform.ScaleY = currentScale;

            // 标记事件已处理，防止默认滚动行为
            e.Handled = true;
        }




        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            int x = Convert.ToInt32(NumBox.Text);
            string type = TypeBox.Text;
            int slotNum = Convert.ToInt32(SlotNumBox.Text);

            //第一步，生成机架信息
            MdfRackClass mdfRack = CreatMdfRack(slotNum);
            mdfRack.RackName = "机架式光纤配线架测试";


            //第二步，添加槽位信息
            ObservableCollection<SlotClass> slots = new ObservableCollection<SlotClass>();
            SlotClass slot = CreatMdfSlot(0, type);
            slot.SlotName = "Slot1";


            //第三部，添加端口信息
            ObservableCollection<PortClass> ports = CreatMdfPorts(x,type);
            slot.Ports= ports;

            slots.Add(slot);


            SlotClass slot2 = CreatMdfSlot(1, "LC");
            slot2.SlotName="Slot2";

            //第三部，添加端口信息
            ObservableCollection<PortClass> ports2 = CreatMdfPorts(x,"LC");
            slot2.Ports = ports2;

            slots.Add(slot2);
            mdfRack.Slots=slots;



            UserControls.LinkPage.Rack rack = new UserControls.LinkPage.Rack()
            {
                RackInfo = mdfRack,
            };

            Panel.Children.Add(rack);

        }

        // 提供一个方法来添加CustomControlB实例
        public UserControl AddCustomControls(int count, int type)
        {
            var panel = new UserControls.LinkPage.MDF();//载体

            panel.PortPanel.Columns=count;

            SolidColorBrush colorBrush;

            int index = ColorBox.SelectedIndex;

            switch (index)
            {
                case 1:

                    colorBrush= Brushes.DarkCyan;
                    break;

                case 2:

                    colorBrush = Brushes.OrangeRed;
                    break;
                case 3:

                    colorBrush = Brushes.YellowGreen;
                    break;
                case 4:

                    colorBrush = Brushes.MediumVioletRed;
                    break;

                default:
                    colorBrush = Brushes.AliceBlue;
                    break;
            }


            for (int i = 0; i < count; i++)
            {
                ViewModes.LinkManage.PortInfoClass info = new ViewModes.LinkManage.PortInfoClass();



                info.Color = colorBrush;
                info.PortTag = (i + 1).ToString();

                var control = new UserControls.LinkPage.Port();
                control.DataContext = info;
                panel.PortPanel.Children.Add(control);
               
                
                //switch (type)
                //{
                //    case 0:
                //        var controlA = new UserControls.LinkPage.Port();
                //        controlA.DataContext = info;

                //        panel.PortPanel.Children.Add(controlA);
                //        break;
                //    case 1:
                //        var controlB = new UserControls.LinkPage.FiberFcPort();
                //        controlB.DataContext = info;

                //        panel.PortPanel.Children.Add(controlB);
                //        break;
                //    case 2:
                //        var controlC = new UserControls.LinkPage.FiberScPort();
                //        panel.PortPanel.Children.Add(controlC);
                //        controlC.DataContext = info;

                //        break;
                //    case 3:
                //        var controlD = new UserControls.LinkPage.FiberLcPort();
                //        panel.PortPanel.Children.Add(controlD);
                //        controlD.DataContext = info;
                //        break;

                //}

            }

            

            return panel;
        }

        /// <summary>
        /// 设备配置分析引擎
        /// </summary>
        /// <param name="mdfRackInfo">配线架信息类型</param>
        public UserControl Dcae(MdfRackClass mdfRackInfo)
        {
              

            MDF mdf=new MDF();






            return null;
        }




        /// <summary>
        /// 创建机架插槽
        /// </summary>
        /// <param name="SlotId">插槽ID</param>
        /// <param name="SlotType">插槽类型，SC,LC,FC</param>
        /// <returns>返回Slot信息class</returns>
        public SlotClass CreatMdfSlot(int slotId,string slotType)
        {

            SlotClass slot = new SlotClass();
            slot.SlotIndex = slotId.ToString();
            slot.SlotName = "槽位名称";
            slot.SlotType = slotType;
            

            return slot;
        }
       
        /// <summary>
        /// 创建机架端口
        /// </summary>
        /// <param name="portNum">端口数量</param>
        /// <returns></returns>
        public ObservableCollection<PortClass> CreatMdfPorts(int portNum,string portType)
        {
            ObservableCollection<PortClass> portList = new ObservableCollection<PortClass>();   

            for (int i = 0; i < portNum; i++)
            {
                PortClass port = new PortClass();
                port.PortIndex =Convert.ToString((i+1));
                port.PortTag = $"{i.ToString()}";
                //port.PermanentLink = new PermanentLink();
                //port.TempLink = new PermanentLink();
                portList.Add(port);
            }

            return portList;
        }




        /// <summary>
        /// 创建机架信息
        /// </summary>
        /// <param name="slotCount">机架槽位总数</param>
        /// <returns></returns>
        private MdfRackClass CreatMdfRack(int slotCount)
        {
            MdfRackClass mdfRack = new MdfRackClass();
            mdfRack.RackId = "机架ID";
            mdfRack.RackName = "机架名称";
            mdfRack.SlotCount = slotCount;

            return mdfRack;    
        }
    }

}
