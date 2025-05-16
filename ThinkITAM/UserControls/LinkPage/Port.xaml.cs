using System.Windows;
using System.Windows.Controls;
using ThinkITAM.Windows.LinkWindows;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.ViewModels.LinkManage;

namespace ThinkITAM.UserControls.LinkPage
{
    /// <summary>
    /// EthernetPort.xaml 的交互逻辑
    /// </summary>
    public partial class Port : UserControl
    {
        public static readonly DependencyProperty RackInfoProperty =
            DependencyProperty.Register("PortInfo", typeof(PortClass), typeof(Port), new PropertyMetadata(null));



        public PortClass PortInfo
        {
            get
            {
                return (PortClass)GetValue(RackInfoProperty);
            }
            set
            {
                SetValue(RackInfoProperty, value);
            }
        }



        public Port()
        {
            InitializeComponent();
            this.DataContext = PortInfo;


        }



        private void Port_OnLoaded(object sender, RoutedEventArgs e)
        {




        }


        /// <summary>
        /// 端口被选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PortButton_OnClick(object sender, RoutedEventArgs e)
        {
            //发生修改的端口信息，含配线架及槽位号
            PortLinkClass portInfo = new PortLinkClass();

            // 获取 rack 层的 DataContext
            var rackDataContext = (sender as FrameworkElement)?.FindAncestor<Rack>()?.DataContext;

            if (rackDataContext != null)
            {
                var rackInfo = rackDataContext as MdfRackClass;

               

                int count = DataBridge.DataBridge.LinkManageList.Count;

                
                if (rackInfo != null)
                {
                    DataBridge.DataBridge.SelectRackId.Clear();

                    DataBridge.DataBridge.SelectRackId.Add(rackInfo.RackId);

                    //保存MDF信息
                    portInfo.MdfRackClass = DbClass.GetRackInfo(rackInfo.RackId); ;
                }



                // 获取位置信息
                PortLinkLocationInfo location = new PortLinkLocationInfo();


                //保存位置信息
                portInfo.PortLinkLocationInfo = location;


                // 获取 Slot 层的 DataContext
                var slotDataContext = (sender as FrameworkElement)?.FindAncestor<MDF>()?.DataContext;
                PortClass port = (PortClass)this.DataContext;

                if (slotDataContext != null)
                {
                    var slotInfo = slotDataContext as SlotClass;

                   

                    DataBridge.DataBridge.SelectPortInfo = port;

                    DataBridge.DataBridge.SelectSlotInfo = slotInfo;

                    port.RackId = rackInfo.RackId;

                    port.SlotIndex = slotInfo.SlotIndex.ToString();

                    //保存Slot信息
                    portInfo.SlotClass = slotInfo;

                    //保存端口信息
                    portInfo.PortClass = port;


                }

               

                switch (DataBridge.DataBridge.LinkManageMode)
                {
                       

                    case 0://添加顺藤摸瓜起点
                        //DataBridge.DataBridge.LinkManageSelectPorts.Clear();

                        //DataBridge.DataBridge.LinkManageSelectPorts.Add(portInfo);
                        if (port.OnTheLine!=null && port.OnTheLine>0)
                        {
                           
                           DataBridge.DataBridge.LinkManageList = DbClass.GetLinkDetail(port.OnTheLine);
                        }
                        else
                        {
                            DataBridge.DataBridge.LinkManageList.Clear();
                        }


                       



                        break;


                    case 1://链路管理模式

                        if (count > 0)
                        {
                            //var mdf = DataBridge.DataBridge.PermanentManageList[count - 1].MdfRackClass;
                            //var slot = DataBridge.DataBridge.PermanentManageList[count - 1].SlotClass;
                            //var port = DataBridge.DataBridge.PermanentManageList[count - 1].PortClass;


                            //if (mdf.RackId.Substring(0, 1) == "8")
                            //{
                            //    slot.SlotType = port.PortType;

                            //}

                            //判断列表中是否已有同一个配线架
                            var mdfs = DataBridge.DataBridge.LinkManageList.Where(item => item.MdfRackClass.RackId == rackInfo.RackId).ToList();

                            if (mdfs.Count > 0)//已有一个同配线架的端口
                            {
                                //如果是同一个配线架同一个端口，则移除该端口
                                var items = DataBridge.DataBridge.LinkManageList.Where(item => item.PortClass == port).ToList();
                                
                                if (items.Count > 0)
                                {
                                    port.IsSelected = false;
                                    port.NodeIndex = 0;
                                    Console.WriteLine(port.UID);
                                    DataBridge.DataBridge.LinkManageList.Remove(items[0]);
                                   
                                }
                                else
                                {
                                    MessageBox.Show("链路不应该重复通过同一个配线架");
                                }


                               

                            }
                            else//不是同一个
                            {
                                //判断两个端口是否是同一大类
                                string nowType = port.PortType;
                                string oldType = string.Empty; ;

                                foreach (var node in DataBridge.DataBridge.LinkManageList)
                                {

                                    if (node.MdfRackClass.RackId.Substring(0, 1) != "2")//不是设备类型
                                    {
                                        oldType = node.PortClass.PortType;
                                        break;
                                    }
                                }

                                Console.WriteLine("NowType:" + nowType);
                                Console.WriteLine("OldType:" + oldType);


                                if (CheckStrings(nowType, oldType) == false)
                                {
                                    MessageBox.Show("链路介质类型应该保持一致");

                                }
                                else
                                {
                                    //创建链路
                                    //先获取节点是否已经在链路上

                                    if (portInfo?.PortClass?.OnTheLine == -1)
                                    {
                                        if (!DataBridge.DataBridge.LinkManageList.Contains(portInfo))
                                        {
                                            DataBridge.DataBridge.LinkManageList.Add(portInfo);
                                            portInfo.PortClass.IsSelected = true;
                                            Console.WriteLine("Count:" + DataBridge.DataBridge.LinkManageList.Count);
                                        }
                                        else
                                        {
                                            Console.WriteLine("该端口信息已添加到操作列表，请勿重复添加");
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("该端口信息已存在关联信息，如需修改请先删除关联信息");
                                    }

                                }






                            }

                        }
                        else
                        {

                            if (portInfo?.PortClass?.OnTheLine == -1)
                            {
                                if (!DataBridge.DataBridge.LinkManageList.Contains(portInfo))
                                {
                                    DataBridge.DataBridge.LinkManageList.Add(portInfo);
                                    portInfo.PortClass.IsSelected = true;
                                    Console.WriteLine("Count:" + DataBridge.DataBridge.LinkManageList.Count);
                                }
                                else
                                {
                                    Console.WriteLine("该端口信息已添加到操作列表，请勿重复添加");
                                }
                            }
                            else
                            {
                                MessageBox.Show("该端口信息已存在关联信息，如需修改请先删除关联信息");
                            }


                        }



                        break;


                    case 2:
                        MessageBox.Show("LINK");
                        break;

                    case 3://链路清除模式

                        if (DataBridge.DataBridge.LinkPerClear == 1)
                        {



                            string tableHeaerA = "ra";

                            if (portInfo.MdfRackClass.RackId.Substring(0, 1) == "8")
                            {
                                tableHeaerA = "bu";
                            }



                            //清除本端
                            string sql = $"UPDATE \"{tableHeaerA}_{portInfo.MdfRackClass.RackId}\" SET \"PermanentType\" = NULL, \"PermanentRackId\" = NULL, \"PermanentSlot\" = NULL, \"PermanentRoom\" = NULL, \"PermanentPort\" = '' WHERE SlotId = '{portInfo.SlotClass.SlotIndex}' AND PortId='{portInfo.PortClass.PortIndex}'";

                            
                            GlobalVariables.DbService.ExecuteNonQuery(sql);
                            DataBridge.DataBridge.SelectUpdateRackId.Add(portInfo.MdfRackClass.RackId);


                            //DataBridge.DataBridge.ModifyTagList.Add("PortPerLinkClear");
                        }

                        if (DataBridge.DataBridge.LinkTempClear == 1)
                        {



                            string tableHeaerB = "ra";

                            if (portInfo.MdfRackClass.RackId.Substring(0, 1) == "8")
                            {
                                tableHeaerB = "bu";
                            }


                            //清除本端
                            string sql = $"UPDATE \"{tableHeaerB}_{portInfo.MdfRackClass.RackId}\" SET \"TempType\" = NULL, \"TempRackId\" = NULL, \"TempSlot\" = NULL, \"TempRoom\" = NULL, \"TempPort\" = '' WHERE SlotId = '{portInfo.SlotClass.SlotIndex}' AND PortId='{portInfo.PortClass.PortIndex}'";

                           
                            GlobalVariables.DbService.ExecuteNonQuery(sql);
                            DataBridge.DataBridge.SelectUpdateRackId.Add(portInfo.MdfRackClass.RackId);

                            //DataBridge.DataBridge.ModifyTagList.Add("PortTempLinkClear");
                        }


                        break;
                }








            }







        }

        /// <summary>
        /// 判断两个端口类型是否一致
        /// </summary>
        /// <param name="stringA"></param>
        /// <param name="stringB"></param>
        /// <returns></returns>
      private  bool CheckStrings(string stringA, string stringB)
        {
            bool containsEInA = stringA.Contains('E') || string.IsNullOrWhiteSpace(stringA);
            bool containsEInB = stringB.Contains('E') || string.IsNullOrWhiteSpace(stringB);

            // 如果两个字符串都包含E（无论大小写），返回true
            // 如果只有一个字符串包含E，返回false
            // 如果两个都不包含E，返回true
            if (containsEInA && containsEInB)
            {
                return true;
            }
            else if (containsEInA != containsEInB) // 一个为true，另一个为false
            {
                return false;
            }
            else // 两个都不包含E的情况
            {
                return true;
            }
        }


        /// <summary>
        /// 端口标签被选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TagButton_OnClick(object sender, RoutedEventArgs e)
        {
            PortClass port = (PortClass)this.DataContext;


            // 获取 Slot 层的 DataContext
            var slotDataContext = (sender as FrameworkElement)?.FindAncestor<MDF>()?.DataContext;
            if (slotDataContext != null)
            {
                DataBridge.DataBridge.SelectSlotInfo = slotDataContext as SlotClass;

            }

            // 获取 rack 层的 DataContext
            var rackDataContext = (sender as FrameworkElement)?.FindAncestor<Rack>()?.DataContext;
            if (rackDataContext != null)
            {
                var info = rackDataContext as MdfRackClass;
                if (info != null)
                {
                    DataBridge.DataBridge.SelectRackId.Clear();
                    DataBridge.DataBridge.SelectRackId.Add(info.RackId);
                }

            }


            TagModifyWindow add = new TagModifyWindow("port", port);

            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                add.Owner = window;
            }



            if (add.ShowDialog() == true)
            {
                DataBridge.DataBridge.ModifyTagList.Add("PortTag");
                // 当子窗口关闭后执行这里的代码

            }
        }

        private void ColorTagSet_OnClick(object sender, RoutedEventArgs e)
        {
            PortClass port = (PortClass)this.DataContext;

            // 获取 Slot 层的 DataContext
            var slotDataContext = (sender as FrameworkElement)?.FindAncestor<MDF>()?.DataContext;
            if (slotDataContext != null)
            {
                DataBridge.DataBridge.SelectSlotInfo = slotDataContext as SlotClass;

            }

            // 获取 rack 层的 DataContext
            var rackDataContext = (sender as FrameworkElement)?.FindAncestor<Rack>()?.DataContext;
            if (rackDataContext != null)
            {
                var info = rackDataContext as MdfRackClass;

                if (info != null)
                {
                    DataBridge.DataBridge.SelectRackId.Clear();
                    DataBridge.DataBridge.SelectRackId.Add(info.RackId);
                }


            }

            PortColorSetWindow add = new PortColorSetWindow(port);

            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                add.Owner = window;
            }


            if (add.ShowDialog() == true)
            {
                DataBridge.DataBridge.ModifyTagList.Add("PortColorTag");
                // 当子窗口关闭后执行这里的代码

            }


        }
    }
}
