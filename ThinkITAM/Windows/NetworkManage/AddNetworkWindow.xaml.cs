using System.Net;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.Functions.IPAddressHelper;

namespace ThinkITAM.Windows.NetworkManage
{
    /// <summary>
    /// AddNetworkWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddNetworkWindow : Window
    {
        /// <summary>
        /// 页面加载状态（页面未完成加载时不计算IP）
        /// </summary>
        public bool LoadStatus = false;

       
        public AddNetworkWindow()
        {
            InitializeComponent();
        }

        private void AddNetworkWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadStatus = true;


            LoadTags();//加载自定义标签
            LoadHierarchyInfo();//加载层级信息


        }

        private List<string> parentList = new List<string>();


        /// <summary>
        /// 加载组织信息
        /// </summary>
        private void LoadHierarchyInfo()
        {
            parentList.Clear();

            string query = "SELECT DISTINCT Parent FROM Hierarchy;";


            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            foreach (var row in rows)
            {
                 parentList.Add(row["Parent"].ToString());
            }


            TbParent.ItemsSource = parentList;


        }


        private List<string> childList = new List<string>();
        /// <summary>
        /// 加载部门信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Parent_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            childList.Clear();
            Child.ItemsSource = null;
            if (TbParent.SelectedIndex != -1)
            {
                string sql = $"SELECT Child FROM Hierarchy WHERE Parent='{parentList[TbParent.SelectedIndex]}'";

                var rows = GlobalVariables.DbService.ExecuteQuery(sql);

                foreach (var row in rows)
                {
                    childList.Add(row["Child"].ToString());
                }


                Child.ItemsSource = childList;

            }

        }



        /// <summary>
        /// 拖动滑条调整子网掩码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaskSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (LoadStatus == true)
            {
                //判断是不是瞎写的IP地址
                if (IsValidIp(IpTextBox.Text) == true)
                {
                    UpdateIPCalculations();
                }
                else
                {
                    IpTextBox.Text = "";
                    MessageBox.Show("IP地址不合法，请检查IP地址是否正确!", "确定", MessageBoxButton.OK, MessageBoxImage.Information);
                }


            }


        }

        /// <summary>
        /// 判断是否是合法IP
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        static bool IsValidIp(string ipAddress)
        {
            IPAddress address;
            return IPAddress.TryParse(ipAddress, out address);
        }


        /// <summary>
        /// IP地址计算
        /// </summary>
        private void UpdateIPCalculations()
        {
            try
            {
                IPAddress ip;
                if (IPAddress.TryParse(IpTextBox.Text, out ip))
                {
                    int maskLength = (int)MaskSlider.Value;
                    IPAddress mask = IPAddressCalculations.SubnetMaskFromPrefixLength(maskLength);
                    Netmask.Text = mask.ToString();

                    IPAddress networkAddress = ip.GetNetworkAddress(mask);
                    Network.Text = networkAddress.ToString();

                    IPAddress firstAddress = networkAddress.GetFirstUsable(ip.AddressFamily);
                    First.Text = firstAddress.ToString();

                    IPAddress lastAddress = networkAddress.GetLastUsable(ip.AddressFamily, maskLength);

                    Last.Text = lastAddress.ToString();

                    IPAddress broadcastAddress = networkAddress.GetBroadcastAddress(maskLength);
                    Broadcast.Text = broadcastAddress.ToString();


                    long addressCount = IPAddressCalculations.AddressCount(maskLength);
                    NumBox.Text = addressCount.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// 保存网段信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            //判断是不是瞎几把写的IP地址
            if (IsValidIp(IpTextBox.Text) == true)
            {
                //计算IP
                UpdateIPCalculations();

                //网段信息
                string name = TbName.Text;
                string description = Description.Text;
                string parent = TbParent.Text;
                string child = Child.Text;
                string network = Network.Text;
                string netmask = Netmask.Text;
                string tagA = TagA.Text;
                string tagB = TagB.Text;
                string tagC = TagC.Text;
                string tagD = TagD.Text;



                if (name != "")
                {

                    string networkId;

                    string sqlTemp = string.Format("SELECT COUNT(*) FROM Network WHERE `Network` = '{0}' AND `Netmask` = '{1}'", network, netmask);


                    int num = DbClass.ExecuteScalarTableNum(sqlTemp);


                    if (MaskSlider.Value < 24)//如果是大型网段
                    {

                        //IP地址段已存在
                        if (num > 0)
                        {
                            string msg = string.Format("已存在同配置网段{0}个，是否继续添加同配置网段？", num.ToString());

                            MessageBoxResult result = MessageBox.Show(msg, "确认", MessageBoxButton.YesNo,
                                MessageBoxImage.Information);

                            if (result == MessageBoxResult.Yes)
                            {

                                //创建资产ID字符串，网段ID
                                networkId = $"7{AssetCodeClass.GenerateChecksum(AssetIdCreate.CreateAssetId($"{network}" + DateTime.Now.ToString("yyyyMMddHHmmss"))).ToUpper()}";

                                var networkInfo = new
                                {
                                    NetworkId= networkId,
                                    Name = name,
                                    Description = description,
                                    Network = network,
                                    Netmask = netmask,
                                    Parent = parent,
                                    Child = child,
                                    TagA = tagA,
                                    TagB = tagB,
                                    TagC = tagC,
                                    TagD = tagD
                                    

                                };

                                //插入网段信息总表的数据
                                //string sql = $"INSERT INTO \"Network\" (\"NetworkId\", \"Name\", \"Description\", \"Network\", \"Netmask\", \"Parent\", \"Child\", \"TagA\", \"TagB\", \"TagC\", \"TagD\") VALUES ('{NetworkId}', '{name}', '{description}', '{network}', '{netmask}', '{parent}', '{child}', '{tagA}', '{tagB}', '{tagC}', '{tagD}')";

                                //保存组织架构信息
                                //SaveHierarchyInfo(parent, child);

                                //写入ip总表信息

                                GlobalVariables.DbService.InsertEntity("Network", networkInfo);

                                //创建分表
                                DbClass.CreateNetworkTableSub(network, (int)MaskSlider.Value, networkId);


                                //装载初始化数据
                                //InitializedNetworkData(tableName);


                                this.DialogResult = true;
                                this.Close();



                            }
                            else if (result == MessageBoxResult.No)
                            {
                                // 用户点击了"否"按钮，取消操作或进行其他处理


                            }


                        }
                        else//IP地址段不存在
                        {
                            //创建资产ID字符串，网段ID
                            networkId = $"7{AssetCodeClass.GenerateChecksum(AssetIdCreate.CreateAssetId($"{network}" + DateTime.Now.ToString("yyyyMMddHHmmss"))).ToUpper()}";

                            // NetworkId = CreateTableName(network, (int)MaskSlider.Value) + "_1";

                            var networkInfo = new
                            {
                                NetworkId = networkId,
                                Name = name,
                                Description = description,
                                Network = network,
                                Netmask = netmask,
                                Parent = parent,
                                Child = child,
                                TagA = tagA,
                                TagB = tagB,
                                TagC = tagC,
                                TagD = tagD


                            };


                            //Console.WriteLine(networkId);

                            //插入网段信息总表的数据
                            //string sql = $"INSERT INTO \"Network\" (\"NetworkId\", \"Name\", \"Description\", \"Network\", \"Netmask\", \"Parent\", \"Child\", \"TagA\", \"TagB\", \"TagC\", \"TagD\") VALUES ('{NetworkId}', '{name}', '{description}', '{network}', '{netmask}', '{parent}', '{child}', '{tagA}', '{tagB}', '{tagC}', '{tagD}')";

                            GlobalVariables.DbService.InsertEntity("Network", networkInfo);


                            SaveHierarchyInfo(parent, child);



                            //插入网段信息总表的数据

                            //创建分表
                            DbClass.CreateNetworkTableSub(network, (int)MaskSlider.Value,networkId);



                            this.DialogResult = true;
                            this.Close();



                        }






                    }
                    else//如果是小型网段
                    {
                        //IP地址段已存在
                        if (num > 0)
                        {
                            string msg = string.Format("已存在同配置网段{0}个，是否继续添加同配置网段？", num.ToString());

                            MessageBoxResult result = MessageBox.Show(msg, "确认", MessageBoxButton.YesNo,
                                MessageBoxImage.Information);

                            if (result == MessageBoxResult.Yes)
                            {
                                // 用户点击了"是"按钮，执行相关操作
                                //NetworkId = CreateTableName(network, (int)MaskSlider.Value) + "_" + (num + 1).ToString();
                                //创建资产ID字符串，网段ID
                                networkId = $"7{AssetCodeClass.GenerateChecksum(AssetIdCreate.CreateAssetId($"{network}" + DateTime.Now.ToString("yyyyMMddHHmmss"))).ToUpper()}";

                                //Console.WriteLine(tableName);

                                var networkInfo = new
                                {
                                    NetworkId = networkId,
                                    Name = name,
                                    Description = description,
                                    Network = network,
                                    Netmask = netmask,
                                    Parent = parent,
                                    Child = child,
                                    TagA = tagA,
                                    TagB = tagB,
                                    TagC = tagC,
                                    TagD = tagD


                                };

                                //插入网段信息总表的数据
                                // string sql = $"INSERT INTO \"Network\" (\"NetworkId\", \"Name\", \"Description\", \"Network\", \"Netmask\", \"Parent\", \"Child\", \"TagA\", \"TagB\", \"TagC\", \"TagD\") VALUES ('{networkId}', '{name}', '{description}', '{network}', '{netmask}', '{parent}', '{child}', '{tagA}', '{tagB}', '{tagC}', '{tagD}')";

                                //保存组织架构信息
                                SaveHierarchyInfo(parent, child);

                                //写入ip总表信息
                                
                                GlobalVariables.DbService.InsertEntity("Network", networkInfo);

                                //创建表
                                DbClass.CreateNetworkTable(networkId);


                                //装载初始化数据
                                InitializedNetworkData(networkId);


                                this.DialogResult = true;
                                this.Close();



                            }
                            else if (result == MessageBoxResult.No)
                            {
                                // 用户点击了"否"按钮，取消操作或进行其他处理


                            }


                        }
                        else//IP地址段不存在
                        {

                            //NetworkId = CreateTableName(network, (int)MaskSlider.Value) + "_1";
                            //创建资产ID字符串，网段ID
                            networkId = $"7{AssetCodeClass.GenerateChecksum(AssetIdCreate.CreateAssetId($"{network}" + DateTime.Now.ToString("yyyyMMddHHmmss"))).ToUpper()}";

                            Console.WriteLine(networkId);

                            var networkInfo = new
                            {
                                NetworkId = networkId,
                                Name = name,
                                Description = description,
                                Network = network,
                                Netmask = netmask,
                                Parent = parent,
                                Child = child,
                                TagA = tagA,
                                TagB = tagB,
                                TagC = tagC,
                                TagD = tagD


                            };

                            //插入网段信息总表的数据
                            //string sql = $"INSERT INTO \"Network\" (\"NetworkId\", \"Name\", \"Description\", \"Network\", \"Netmask\", \"Parent\", \"Child\", \"TagA\", \"TagB\", \"TagC\", \"TagD\") VALUES ('{networkId}', '{name}', '{description}', '{network}', '{netmask}', '{parent} ', ' {child}', '{tagA}', '{tagB}', '{tagC}', '{tagD}')";

                          
                            GlobalVariables.DbService.InsertEntity("Network", networkInfo);
                            SaveHierarchyInfo(parent, child);



                            //插入网段信息总表的数据

                            //创建表
                            DbClass.CreateNetworkTable(networkId);


                            //装载初始化数据
                            InitializedNetworkData(networkId);


                            this.DialogResult = true;
                            this.Close();



                        }

                    }




                }
                else
                {

                    MessageBox.Show("为了便于后期管理，必须填写网段名称!", "确定", MessageBoxButton.OK, MessageBoxImage.Information);


                }

            }
            else
            {
                MessageBox.Show("IP地址不合法，请检查IP地址是否正确!", "确定", MessageBoxButton.OK, MessageBoxImage.Information);
            }







        }



        /// <summary>
        /// 填充网段表单初始数据
        /// </summary>
        private void InitializedNetworkData(string tableName)
        {
            string[] parts = Network.Text.Split('.');

            //取出第一个IP
            int firstIp = Convert.ToInt32(parts[3]);


            parts = Broadcast.Text.Split('.');

            //取出最后一个IP,广播IP
            int LastIp = Convert.ToInt32(parts[3]);



            int x = Convert.ToInt32(NumBox.Text);

            for (int i = 0; i < x; i++)
            {
                //IP地址锁定状态：0不可用IP，1可用IP
                int addressStatus = 0;

                int ip = firstIp + i;



                List<string> lockip = new List<string>();


                if (ip == firstIp)
                {
                    addressStatus = 0;//网段IP

                }
                else
                {
                    if (ip == LastIp)
                    {

                        addressStatus = 4;//广播IP
                    }
                    else
                    {
                        addressStatus = 1;//1、正常未分配IP，2正常已分配ip，3已分配未启用ip
                    }
                }

                var info = new
                {
                    Address=ip,
                    AddressStatus=addressStatus    
                };

                //Console.WriteLine(sql);
                //异步执行

                GlobalVariables.DbService.InsertEntityAsync($"Net_{tableName}", info);
            }


        }




        /// <summary>
        /// 保存部门信息
        /// </summary>
        private void SaveHierarchyInfo(string parent, string child)
        {
            if (parent != null)
            {
                string sqlTemp = string.Format("SELECT COUNT(*) FROM Hierarchy WHERE `Parent` = '{0}' AND `Child` = '{1}'", parent, child);

                int num = DbClass.ExecuteScalarTableNum(sqlTemp);

                if (num == 0)//不存在，则添加
                {

                    var info = new { Parent=parent, Child=child };

                    //string sql = $"INSERT INTO \"Hierarchy\" (\"Parent\", \"Child\") VALUES ('{parent}', '{child}')";
                  
                    GlobalVariables.DbService.InsertEntity("Hierarchy", info);
                }
            }


        }




        /// <summary>
        /// 生成表名
        /// </summary>
        /// <returns></returns>
        private string CreateTableName(string address, int netmask)
        {
            string name = address + $"_{netmask}";

            name = "tb_" + name.Replace(".", "_");

            return name;
        }

        /// <summary>
        /// 加载自定义标签
        /// </summary>
        private void LoadTags()
        {

            var tags = DbClass.LoadWindowTag("AddNetwork");

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


        /// <summary>
        /// 弹出自定义标签设置按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetButton_OnClick(object sender, RoutedEventArgs e)
        {
            
            AddNetworkWindowSet set = new AddNetworkWindowSet();

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




    }
}
