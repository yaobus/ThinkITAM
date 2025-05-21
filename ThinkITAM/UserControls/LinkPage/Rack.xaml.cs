using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ThinkITAM.Windows.LinkWindows;
using ThinkITAM.ViewModels.LinkManage;
using Point = System.Windows.Point;

namespace ThinkITAM.UserControls.LinkPage
{
    /// <summary>
    /// Rack.xaml 的交互逻辑
    /// </summary>
    public partial class Rack : UserControl
    {
        public static readonly DependencyProperty RackInfoProperty =
            DependencyProperty.Register("RackInfo", typeof(MdfRackClass), typeof(Rack), new PropertyMetadata(null));

        /// <summary>
        /// 设置Rack的坐标
        /// </summary>
        private Point _location;
        public Point Location
        {
            get => _location;
            set
            {
                if (_location != value)
                {
                    _location = value;
                    OnPropertyChanged(nameof(Location));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        public MdfRackClass RackInfo
        {
            get
            {
                return (MdfRackClass)GetValue(RackInfoProperty);
            }
            set
            {
                SetValue(RackInfoProperty, value);
            }
        }




        public Rack()
        {
            InitializeComponent();
        }

        private void Rack_OnLoaded(object sender, RoutedEventArgs e)
        {
            

            this.DataContext = RackInfo;

            if (RackInfo != null)
            {

                //第一步，计算设备高度
                this.Height = 10 + RackInfo.SlotCount * 96;



                int slotIndex = 0;

                foreach (var slot in RackInfo.Slots)
                {


                    MDF mdf = new MDF();

                    //端口面板的列数
                    mdf.PortPanel.Columns = slot.Ports.Count;

                    for (int i = 0; i < slot.Ports.Count; i++)
                    {
                        PortClass portInfo = slot.Ports[i];
                       
                        

                        UserControl userControl = CreatPortControl(portInfo);

                        userControl.DataContext = portInfo;

                        mdf.PortPanel.Children.Add(userControl);

                    }

                    mdf.DataContext = RackInfo.Slots[slotIndex];

                    SlotPanel.Children.Add(mdf);

                    slotIndex++;
                }
            }

        }

        private UserControl CreatPortControl(PortClass portInfo)
        {

            var portControl = new Port();
            portControl.DataContext = portInfo;
            return portControl;

        }

        /// <summary>
        /// 修改机架名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RackButton_OnClick(object sender, RoutedEventArgs e)
        {
            MdfRackClass rack = (MdfRackClass)this.DataContext;

            if (rack != null)
            {

                DataBridge.DataBridge.SelectRackId.Clear();
                DataBridge.DataBridge.SelectRackId.Add(rack.RackId);

            }

            TagModifyWindow add = new TagModifyWindow("rack",rack);

            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                add.Owner = window;
            }



            if (add.ShowDialog() == true)
            {
                // 当子窗口关闭后执行这里的代码
                // 创建事件参数

                DataBridge.DataBridge.ModifyTagList.Add("RackTag");

               


            }

        }


        private void Rack_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                MdfRackClass rack = (MdfRackClass)this.DataContext;

                if (rack != null)
                {
                    if (DataBridge.DataBridge.SelectRackId.Count > 0 )
                    {
                        if (DataBridge.DataBridge.SelectRackId[0] != rack.RackId)
                        {
                            //Console.WriteLine("当前选中项由："+DataBridge.DataBridge.SelectRackId[0] + "变为：" + rack.RackId);
                            DataBridge.DataBridge.SelectRackId.Clear();
                            DataBridge.DataBridge.SelectRackId.Add(rack.RackId);
                        }
     
                    }
                    else
                    {
                        DataBridge.DataBridge.SelectRackId.Add(rack.RackId);
                       
                    }



                }
            }
        }
    }
}

