using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ThinkITAM.Windows.NetworkManage;
using ThinkITAM.UserControls.NetworkManage;
using ThinkITAM.ViewModels.NetworkManage;
using ThinkITAM.ViewModels.Preset;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using ThinkITAM.DatabaseOperation;
using static ThinkITAM.DataBridge.DataBridge;
using static ThinkITAM.Windows.NetworkManage.AddressAllocationWindow;
using ThinkITAM.Windows.ToolWindows;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.Functions.IPAddressHelper;
using ThinkITAM.DataBridge;

namespace ThinkITAM.FunctionPage;

/// <summary>
/// NetworkAddressManage.xaml 的交互逻辑
/// </summary>
public partial class NetworkAddressManagePage : UserControl
{
    public NetworkAddressManagePage()
    {
        InitializeComponent();
        DataContext = this;
        MessageQueue = new SnackbarMessageQueue();
    }


    /// <summary>
    /// Snackbar消息
    /// </summary>
    public SnackbarMessageQueue MessageQueue
    {
        get; set;
    }

    /// <summary>
    /// 页面启动的时候加载信息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void NetworkAddressManage_OnLoaded(object sender, RoutedEventArgs e)
    {

        AddressListView.ItemsSource = IpAddressInfoLists;

        AddressPanel.ItemsSource = IpAddressInfoLists;


        LoadNetworkInfo2();

        LoadCustomTag();

        //加载网段信息备注标签
        LoadTags();


        //加载协议预设
        LoadProtocolInfo();


        //加载浏览器路径
        LoadBrowserInfo();

        //加载端口列表
        LoadPort();


        DataBridge.DataBridge.SelectAddress.CollectionChanged += IpAddressInfoLists_CollectionChanged;
    }

    /// <summary>
    /// 查询选中的地址数量
    /// </summary>
    /// <returns></returns>
    public int GetSelectedAddressCount()
    {
        int sum = DataBridge.DataBridge.IpAddressInfoLists.Sum(item => Convert.ToInt32(item.IsSelected));

        return sum;
    }

    /// <summary>
    /// 监听IpAddressInfoLists集合
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void IpAddressInfoLists_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        int sum = GetSelectedAddressCount();


        if (sum > 0)
        {

            AddressNumber.Dispatcher.Invoke(() =>
            {
                AddressNumber.Text = sum.ToString();
            });



            MultipleAllocationPanel.Visibility = Visibility.Visible;
        }
        else
        {

            MultipleAllocationPanel.Visibility = Visibility.Collapsed;
        }



    }




    private ObservableCollection<ViewModels.Preset.ProtocolClass> protocolInfos = new ObservableCollection<ProtocolClass>();

    private void LoadProtocolInfo()
    {
        protocolInfos.Clear();

        ProtocolCombobox.ItemsSource = protocolInfos;

        string query = "SELECT * FROM Protocol;";

        var rows = GlobalVariables.DbService.ExecuteQuery(query);

        int index = 0;

        foreach (var row in rows)
        {
            index++;
            ProtocolClass info = new ProtocolClass();

            info.Index = index;
            info.Protocol = row["Protocol"].ToString();
            info.Note = row["Note"].ToString();

            protocolInfos.Add(info);
        }

    }




    ObservableCollection<PortViewModel> portList = new ObservableCollection<PortViewModel>();

    /// <summary>
    /// 加载端口信息
    /// </summary>
    private void LoadPort()
    {
        portList.Clear();
        PortComboBox.ItemsSource = portList;

        string sqlTemp = $"SELECT COUNT(*) FROM PortList";

        var num = DbClass.ExecuteScalarTableNum(sqlTemp);

        if (num > 0)
        {
            string query = "SELECT * FROM PortList;";

            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            foreach (var row in rows)
            {

                var port = new PortViewModel();

                port.Port = Convert.ToInt32(row["Port"]);

                portList.Add(port);
            }


        }
    }

    /// <summary>
    /// 加载网段信息
    /// </summary>
    private async void LoadNetworkInfo2()
    {
        networkInfos.Clear();
        NetworkTreeView.Items.Clear();

        string sqlTemp = $"SELECT COUNT(*) FROM Network WHERE Del != 1 OR Del IS NULL";


        var num = DbClass.ExecuteScalarTableNum(sqlTemp);

        if (num > 0)
        {
            string query = "SELECT * FROM Network  WHERE Del != 1 OR Del IS NULL;";


            var rows = GlobalVariables.DbService.ExecuteQuery(query);


            int i = 0;


            foreach (var row in rows)
            {
                i++;
                var info = new NetworkInfoViewMode();
                info.Index = i;
                string tableName = "Net_" + row["NetworkId"].ToString();

                info.TableName = tableName;
                info.NetworkId=row["NetworkId"].ToString();
                info.Name = row["Name"].ToString();
                info.Description = row["Description"].ToString();
                info.Network = row["Network"].ToString();
                info.Netmask = row["Netmask"].ToString();
                info.Parent = row["Parent"].ToString();
                info.Child = row["Child"].ToString();
                info.TagA = row["TagA"].ToString();
                info.TagB = row["TagB"].ToString();
                info.TagC = row["TagC"].ToString();
                info.TagD = row["TagD"].ToString();
                info.TagE = row["TagE"].ToString();
                info.TagF = row["TagF"].ToString();
                //info.Percentage = CalculateUseValue(tableName);




                IPAddress mask = IPAddress.Parse(info.Netmask);
                int subMask = IPAddressCalculations.CalculateSubnetMaskLength(mask);

                if (subMask < 24) //如果是大型网段
                {
                    //总使用率
                    info.Percentage = GetUseValue(tableName, info.Netmask, info.Network);

                    NetworkTreeView.Items.Add(GetSubnetsFromDatabase(info, subMask));

                }
                else //如果是普通网段
                {

                    //查询地址使用率
                    info.Percentage = GetUseValue(tableName, info.Netmask);



                    var myCustomControl = new NetworkInfo();
                    myCustomControl.TableName = tableName;
                    myCustomControl.DataContext = info;

                    NetworkTreeView.Items.Add(myCustomControl);
                }



                await Task.Delay(50);


                networkInfos.Add(info);
            }





        }




    }

    /// <summary>
    /// 获取网段使用率
    /// </summary>
    /// <param name="tableName"></param>
    /// <returns></returns>
    private int GetUseValue(string tableName, string netMask, string network = null)
    {

        int maskLength = IPAddressCalculations.SubnetMaskToCidr(netMask);

        var addressCount = IPAddressCalculations.AddressCount(maskLength) - 2;

        int value = 0;

        if (maskLength < 24)//如果是大型网段
        {
            var info = SubnetCalculator.CalculateSubnets(network, maskLength);

            int index = 0;
            int useNum = 0;

            foreach (var sub in info.Item2)
            {
                //创建新的数据表名
                string newName = tableName + $"_Sub{index}";

                useNum += GetNetWorkUsedAddress(newName);

                index++;
            }

            value = Convert.ToInt32((useNum * 100) / addressCount);

        }
        else
        {
            int useNum = GetNetWorkUsedAddress(tableName);

            value = Convert.ToInt32((useNum * 100) / addressCount);
        }






        return value;

    }

    /// <summary>
    /// 获取网段已用地址数量
    /// </summary>
    /// <param name="tableName"></param>
    /// <returns></returns>
    private int GetNetWorkUsedAddress(string tableName)
    {
        string sql = $"SELECT COUNT(*) FROM {tableName} WHERE AddressStatus != '1'";//查询已分配的地址数量


        return DbClass.ExecuteScalarTableNum(sql);
    }



    private ObservableCollection<BrowserInfoViewModel> browserInfos = new ObservableCollection<BrowserInfoViewModel>();

    /// <summary>
    /// 加载浏览器路径
    /// </summary>
    private void LoadBrowserInfo()
    {

        browserInfos.Clear();

        string query = "SELECT * FROM Browser;";


        var rows = GlobalVariables.DbService.ExecuteQuery(query);

        int index = 0;

        foreach (var row in rows)
        {
                      index++;
            BrowserInfoViewModel info = new BrowserInfoViewModel();

            info.Index = index;
            info.Browser = row["Browser"].ToString();
            info.Path = row["Path"].ToString();

            browserInfos.Add(info);  
        }



        BrowserCombobox.ItemsSource = browserInfos;
        if (browserInfos.Count > 0)
        {

            BrowserCombobox.SelectedIndex = 0;
            DataBridge.DataBridge.SelectBrowser = browserInfos[0].Path;

        }




    }



    private TreeViewItem GetSubnetsFromDatabase(NetworkInfoViewMode info, int subnetMask)
    {

        (string baseSubnet, ObservableCollection<string> subnetsRanges) =
            SubnetCalculator.CalculateSubnets(info.Network, subnetMask);


        var rootNode = new TreeViewItem(); //根节点表项

        var network = new NetworkInfo(); //根节点控件

        network.DataContext = info; //根节点文本与根节点关联
        network.TableName = info.TableName;
        rootNode.Header = network; //根节点表项为带有文本关联的根节点控件

        int subIndex = 0;

        foreach (string range in subnetsRanges)
        {
            subIndex++;

            string tableName = info.TableName + $"_Sub{subIndex - 1}";

            int useNum = GetNetWorkUsedAddress(tableName);


            SubNetworkInfoViewModel subNetworkInfo = new SubNetworkInfoViewModel() //子节点控件文本
            {
                Range = range,
                Index = subIndex,
                TableName = tableName,
                Network = info.Network,
                Netmask = info.Netmask,
                Percentage = Convert.ToInt32((useNum * 100) / 254)

            };

            var subNetwork = new SubNetworkInfo(); //子节点控件

            subNetwork.SubTableName = subNetworkInfo.TableName;

            subNetwork.DataContext = subNetworkInfo; //子节点文本与子节点控件关联

            rootNode.Items.Add(subNetwork);

        }



        return rootNode;

    }



    private List<NetworkInfoViewMode> networkInfos = new List<NetworkInfoViewMode>();

    /// <summary>
    /// 加载IP地址信息自定义标签
    /// </summary>
    private void LoadCustomTag()
    {
        var tagWindow = "IpAddressInfoTag" + DataBridge.DataBridge.NetworkTableName;

        string sqlTemp = $"SELECT COUNT(*) FROM WindowTag WHERE Window ='{tagWindow}'";

        var num = DbClass.ExecuteScalarTableNum(sqlTemp);



        if (num > 0) //存在本地自定义标签
        {
            var tags = DbClass.LoadWindowTag(tagWindow);

            if (tags != null)
            {
                dynamic settings = JsonConvert.DeserializeObject(tags);

                //标签存到全局变量
                DataBridge.DataBridge.SelectNetworkTags = settings;

                TagA.Text = settings.TagA;
                TagB.Text = settings.TagB;
                TagC.Text = settings.TagC;
                TagD.Text = settings.TagD;
                TagE.Text = settings.TagE;
                TagF.Text = settings.TagF;

            }

        }
        else //全局标签
        {
            var tags = DbClass.LoadWindowTag("IpAddressInfoTag");

            if (tags != null)
            {
                dynamic settings = JsonConvert.DeserializeObject(tags);

                //标签存到全局变量
                DataBridge.DataBridge.SelectNetworkTags = settings;

                TagA.Text = settings.TagA;
                TagB.Text = settings.TagB;
                TagC.Text = settings.TagC;
                TagD.Text = settings.TagD;
                TagE.Text = settings.TagE;
                TagF.Text = settings.TagF;

            }

        }



    }




    /// <summary>
    /// 计算IP地址使用率
    /// </summary>
    /// <param name="tableName"></param>
    /// <returns></returns>
    private double CalculateUseValue(string tableName)
    {
        //查询表对应共有的IP数量
        string sqlTemp = $"SELECT COUNT(*) FROM {tableName}";

        var all = DbClass.ExecuteScalarTableNum(sqlTemp);



        sqlTemp = $"SELECT COUNT(*) FROM {tableName} WHERE AddressStatus != 1";

        var used = DbClass.ExecuteScalarTableNum(sqlTemp);


        return ((double)used / all) * 100;

    }



    /// <summary>
    /// 网段信息加载状态,0为未加载，1表示正在加载
    /// </summary>
    private int NetworkLoadStatus = 0;

    /// <summary>
    /// 加载模式，0为图形化，1为列表化
    /// </summary>
    private int LoadMode = 0;


    private async void NetworkTreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {

        

        if (e != null)
        {
            NetworkTreeView.IsEnabled = false;


            //选中网段后则启用编辑和禁用按钮
            EditButton.IsEnabled = true;
            DeleteButton.IsEnabled = true;


            if (NetworkLoadStatus == 0)
            {
                AddressListView.Visibility = Visibility.Collapsed;

                GraphicalPlan.Visibility = Visibility.Visible;
                // 获取用户选择的项
                var selectedNode = e.NewValue;

                // 判断选择的项的类型
                if (selectedNode is NetworkInfo)
                {
                    //Console.WriteLine("小型网段");


                    // 如果选择的是树节点类型，则处理树节点的逻辑
                    NetworkInfo treeNode = selectedNode as NetworkInfo;

                    DataBridge.DataBridge.NetworkTableName = treeNode.TableName;

                    var info = treeNode.DataContext as NetworkInfoViewMode;

                    //拼接IP地址前段
                    string[] address = info.Network.Split('.');
                    string addresSegment = $"{address[0]}.{address[1]}.{address[2]}.";
                    DataBridge.DataBridge.SelectNetwork = addresSegment;

                    //Console.WriteLine("NetworkAddressManage546: " + addresSegment);

                    //Console.WriteLine("Select: "+treeNode.TableName);

                    await LoadSelectNetworkInfo(treeNode);


                }
                else if (selectedNode is SubNetworkInfo) //如果是子项
                {

                    // 如果选择的是子节点类型，则处理子节点的逻辑
                    SubNetworkInfo childNode = selectedNode as SubNetworkInfo;

                    SubNetworkInfoViewModel info = childNode.DataContext as SubNetworkInfoViewModel;

                    //拼接IP地址前段
                    string[] address = info.Range.Split('_');
                    string ip = address[0];
                    string[] ipParts1 = ip.Split('.');
                    string addresSegment = $"{ipParts1[0]}.{ipParts1[1]}.{ipParts1[2]}.";



                    DataBridge.DataBridge.SelectNetwork = addresSegment;




                    string tableName = info.TableName;

                    DataBridge.DataBridge.NetworkTableName = tableName;



                    //获取子表数量
                    var m = AnalysisTableNameToNetworkInfo(info.Network, info.Netmask);

                    int subTableNum = m.Item2.Count;

                    //1.判断分表是否为空表
                    string sqlTemp = $"SELECT COUNT(*) FROM {tableName}";

                    var num = DbClass.ExecuteScalarTableNum(sqlTemp);

                    string[] ipParts = tableName.Split('_');

                    int subNum = Convert.ToInt32(ipParts[2].Replace("Sub", ""));



                    //父级节点表名
                    string parentTableName = $"{ipParts[1]}";



                    string query = $"SELECT * FROM Network WHERE NetworkId ='{parentTableName}';";

                    var rows = GlobalVariables.DbService.ExecuteQuery(query);


                    var parentInfo = new NetworkInfoViewMode();

                    foreach (var row in rows)
                    {
                        parentInfo.TableName = parentTableName;
                        parentInfo.Name = row["Name"].ToString();
                        parentInfo.Description = row["Description"].ToString();
                        parentInfo.Network = row["Network"].ToString();
                        parentInfo.Netmask = row["Netmask"].ToString();

                    }


                    DataBridge.DataBridge.SelectNetworkInfo = parentInfo;




                    if (num == 0) //如果是空表
                    {
                        await SendMessage("正在初始化所选地址段，请稍等！");

                        if (subNum == 0) //当前为第一个分表（0表示这是第一个分表），则存在网段IP
                        {
                            //装载初始化数据
                            await InitializationSubNetworkTable(tableName, 0);

                        }
                        else if (subNum == subTableNum - 1) //当前为最后一个表（subTableNum - 1）表示这是最后一个分表
                        {
                            //装载初始化数据
                            await InitializationSubNetworkTable(tableName, 4);
                        }
                        else
                        {
                            //装载初始化数据
                            await InitializationSubNetworkTable(tableName);
                        }
                    }



                    //加载网段标签
                    LoadCustomTag();

                    //加载网段备注
                    //LoadNetworkNote(info);

                    await LoadAddressInfo(tableName);


                }
                else if (selectedNode is TreeViewItem) //如果是带有子节点的表项
                {
                    LoadedNetworkSegment = null;
                    IpAddressInfoLists.Clear();

                    TreeViewItem selectedItem = selectedNode as TreeViewItem;

                    if (selectedItem != null)
                    {
                        // 判断节点是否展开
                        if (selectedItem.IsExpanded)
                        {
                            // 如果已经展开，就折叠节点
                            selectedItem.IsExpanded = false;
                        }
                        else
                        {
                            // 如果未展开，就展开节点
                            selectedItem.IsExpanded = true;
                        }
                    }

                }

            }

            NetworkLoadStatus = 0;
            NetworkTreeView.IsEnabled = true;
            //GraphicsButton.IsEnabled = true;
            //ListButton.IsEnabled = true;
        }
        else
        {
            EditButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
        }

       
    }

    private Task SendMessage(string message)
    {
        return Task.Run(() =>
        {
            // 在后台线程发送消息
            MessageQueue.Enqueue(message);
        });
    }

    /// <summary>
    /// 大型网段，分表初始化数据装填
    /// </summary>
    /// <param name="tableName">分表名称</param>
    /// <param name="addressStatus">是否存在网段地址或广播地址0为存在网段地址，1为普通地址，4为存在广播地址</param>
    private async Task InitializationSubNetworkTable(string tableName, int addressStatus = 1)
    {
        int status; //0、为网段IP，1、正常未分配IP，2正常已分配ip，3已分配未启用ip，4、广播IP


        for (int i = 0; i < 256; i++)
        {
            if (addressStatus == 0 && i == 0) //如果存在网段地址
            {
                status = 0;
            }
            else if (addressStatus == 4 && i == 255)
            {
                status = 4;
            }
            else
            {
                status = 1;
            }






            string sql = $"INSERT INTO `{tableName}` (`Address`, `AddressStatus`) VALUES ({i}, {status})";


            //Console.WriteLine(sql);
            //异步执行

            await GlobalVariables.DbService.ExecuteQueryAsync(sql);


        }


    }


    /// <summary>
    /// 加载所选网段的信息
    /// </summary>
    /// <param name="treeNode"></param>
    private async Task LoadSelectNetworkInfo(NetworkInfo treeNode)
    {
        var nodeInfo = treeNode.DataContext as NetworkInfoViewMode;

        AnalysisTableNameToNetworkInfo(nodeInfo.Network, nodeInfo.Netmask);



        //将表名存到全局变量，便于其他地方调用
        DataBridge.DataBridge.NetworkTableName = treeNode.TableName;


        //将当前选中的网段信息存到全局变量，便于其他地方调用

        var info = treeNode.DataContext as NetworkInfoViewMode;

        DataBridge.DataBridge.SelectNetworkInfo = info;



        //加载网段标签
        LoadCustomTag();

        //加载网段备注
        LoadNetworkNote(info);


        await LoadAddressInfo(treeNode.TableName);



    }




    /// <summary>
    /// 将表信息解析为网络信息，并计算网络详细信息
    /// </summary>
    /// <param name="info"></param>
    /// <returns>返回这个网段有多少个分表</returns>
    private (string, ObservableCollection<string>) AnalysisTableNameToNetworkInfo(string network, string netmask)
    {

        Network.Text = network;
        MaskText.Text = netmask;
        UpdateIPCalculations();

        int mask = SubnetCalculator.SubnetMaskToLength(netmask);

        var networkInfo = SubnetCalculator.CalculateSubnets(network, mask);

        //foreach (var VARIABLE in networkInfo.Item2)
        //{
        //    //Console.WriteLine("NetworkAddressManage797:"+VARIABLE);
        //}


        return networkInfo;
    }


    /// <summary>
    /// 加载网段备注信息
    /// </summary>
    private void LoadNetworkNote(NetworkInfoViewMode networkInfo)
    {


        //加载备注
        NetworkNote.Text = networkInfo.Description;
        NetworkParent.Text = networkInfo.Parent;
        NetworkChild.Text = networkInfo.Child;
        NetworkTagA.Text = networkInfo.TagA;
        NetworkTagB.Text = networkInfo.TagB;
        NetworkTagC.Text = networkInfo.TagC;
        NetworkTagD.Text = networkInfo.TagD;
    }

    /// <summary>
    /// 加载网段信息备注标签
    /// </summary>
    private void LoadTags()
    {

        settingTags = DbClass.LoadWindowTag("AddNetwork");


        if (settingTags != null)
        {
            dynamic settings = JsonConvert.DeserializeObject(settingTags);



            if (settings.TagA != null)
            {
                HintAssist.SetHint(NetworkParent, settings.TagA);
            }
            else
            {
                HintAssist.SetHint(NetworkParent, "自定义父级标签A");
            }


            if (settings.TagB != null)
            {
                HintAssist.SetHint(NetworkChild, settings.TagB);
            }
            else
            {
                HintAssist.SetHint(NetworkChild, "自定义子级标签B");

            }


            if (settings.TagC != null)
            {
                HintAssist.SetHint(NetworkTagA, settings.TagC);
            }
            else
            {
                HintAssist.SetHint(NetworkTagA, "自定义标签C");
            }


            if (settings.TagD != null)
            {
                HintAssist.SetHint(NetworkTagB, settings.TagD);
            }
            else
            {
                HintAssist.SetHint(NetworkTagB, "自定义标签D");
            }


            if (settings.TagE != null)
            {
                HintAssist.SetHint(NetworkTagC, settings.TagE);
            }
            else
            {
                HintAssist.SetHint(NetworkTagC, "自定义标签E");
            }


            if (settings.TagF != null)
            {
                HintAssist.SetHint(NetworkTagD, settings.TagF);
            }
            else
            {
                HintAssist.SetHint(NetworkTagD, "自定义标签F");
            }

        }

    }

    /// <summary>
    /// 自定义标签
    /// </summary>
    private dynamic settingTags = null;



    /// <summary>
    /// IP地址计算
    /// </summary>
    private void UpdateIPCalculations()
    {
        try
        {
            IPAddress ip;
            if (IPAddress.TryParse(Network.Text, out ip))
            {

                IPAddress mask = IPAddress.Parse(MaskText.Text);
                int maskLength = IPAddressCalculations.CalculateSubnetMaskLength(mask);


                IPAddress networkAddress = ip.GetNetworkAddress(mask);
                //Network.Text = networkAddress.ToString();

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
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }









    /// <summary>
    /// 加载地址信息
    /// </summary>
    /// <param name="tableName"></param>
    private async Task LoadAddressInfo(string tableName)
    {

        if (tableName == LoadedNetworkSegment)//表示当前加载的网段与上次加载的网段一致，则需要后台刷新
        {

            string query = $"SELECT  {tableName}.*,  UserInfo.Name, UserInfo.Organization, UserInfo.Department, UserInfo.UserGroup, UserInfo.Phone, Asset.AssetTag, Asset.AssetNumber FROM  {tableName} LEFT JOIN UserInfo  ON {tableName}.User = UserInfo.UserId LEFT JOIN   Asset  ON   {tableName}.LinkDevice = Asset.AssetId ORDER BY Address ASC;";

            //Console.WriteLine(query);

            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            foreach (var row in rows)
            {
                var info = new IpAddressInfoListViewMode();
                info.Address = Convert.ToInt32(row["Address"].ToString());
                int addressStatus = Convert.ToInt32(row["AddressStatus"].ToString());

                info.AddressStatus = addressStatus;

                Brush brush;


                if (addressStatus == 0 || addressStatus == 4)
                {
                    info.AddressType = false;
                }
                else
                {
                    info.AddressType = true;
                }


                try
                {
                    info.AddressColor = row["AddressColor"] != DBNull.Value ? Convert.ToInt32(row["AddressColor"]) : 0;
                }
                catch (Exception e)
                {
                    info.AddressColor = 0;
                }


                info.PingTime = "N/A";
                info.PingStatusColor = Brushes.Azure;
                info.User = row["User"].ToString();
                info.Name = row["Name"].ToString();
                info.Organization = row["Organization"].ToString();
                info.Department = row["Department"].ToString();
                info.Group = row["Group"].ToString();
                info.Phone = row["Phone"].ToString();
                info.HostName = row["HostName"].ToString();
                info.MacAddress = row["MacAddress"].ToString();
                info.LinkDevice = row["LinkDevice"].ToString();
                info.TagA = row["TagA"].ToString();
                info.TagB = row["TagB"].ToString();
                info.TagC = row["TagC"].ToString();
                info.TagD = row["TagD"].ToString();
                info.TagE = row["TagE"].ToString();
                info.TagF = row["TagF"].ToString();
                var tip = JoInTip(info);
                info.AddressToolTip = tip;


                var itemToUpdate = IpAddressInfoLists.FirstOrDefault(item => item.Address == info.Address);
                int index = IpAddressInfoLists.IndexOf(itemToUpdate);

                if (itemToUpdate != null)
                {
                    //Console.WriteLine("UPDATE:" + index);

                    //IpAddressInfoLists.RemoveAt(index);

                    //IpAddressInfoLists.Insert(index, info);


                    itemToUpdate.AddressStatus = info.AddressStatus;
                    itemToUpdate.AddressColor = info.AddressColor;
                    itemToUpdate.PingTime = info.PingTime;
                    itemToUpdate.PingStatusColor = info.PingStatusColor;
                    itemToUpdate.User = info.User;
                    itemToUpdate.Name = info.Name;
                    itemToUpdate.Organization = info.Organization;
                    itemToUpdate.Department = info.Department;
                    itemToUpdate.Group = info.Group;
                    itemToUpdate.Phone = info.Phone;
                    itemToUpdate.HostName = info.HostName;
                    itemToUpdate.MacAddress = info.MacAddress;
                    itemToUpdate.LinkDevice = info.LinkDevice;
                    itemToUpdate.TagA = info.TagA;
                    itemToUpdate.TagB = info.TagB;
                    itemToUpdate.TagC = info.TagC;
                    itemToUpdate.TagD = info.TagD;
                    itemToUpdate.TagE = info.TagE;
                    itemToUpdate.TagF = info.TagF;
                    itemToUpdate.AddressToolTip = tip;




                }


 
            }




        }
        else
        {
            IpAddressInfoLists.Clear();
            //SelectAddress.Clear();
            DataBridge.DataBridge.IpAddressInfoLists.Clear();

            //获取当前选中的网段
            string[] segments = tableName.ToString().Split('_');
            string networkId = segments[1];
            //var networkInfo = dbClass.GetNetworkInfoFromId(networkId);



            //获取子表数量


            if (LoadMode == 0) //切换面板
            {
                AddressListView.Visibility = Visibility.Collapsed;//列表隐藏

                GraphicalPlan.Visibility = Visibility.Visible;//图形外面板显示

            }
            else
            {
                GraphicalPlan.Visibility = Visibility.Collapsed;//图形外面板隐藏

                AddressListView.Visibility = Visibility.Visible;//列表显示

            }




            string query = $"SELECT  {tableName}.*,  UserInfo.Name,  UserInfo.Organization,  UserInfo.Department,  UserInfo.UserGroup,  UserInfo.Phone, Asset.AssetTag,  Asset.AssetNumber FROM  {tableName}  LEFT JOIN  UserInfo  ON  {tableName}.User = UserInfo.UserId LEFT JOIN  Asset  ON  {tableName}.LinkDevice = Asset.AssetId  ORDER BY Address ASC;";

            var rows = GlobalVariables.DbService.ExecuteQuery(query);





            foreach (var row in rows)
            {
                            
                var info = new IpAddressInfoListViewMode();
                info.Address = Convert.ToInt32(row["Address"].ToString());
                int status = Convert.ToInt32(row["AddressStatus"].ToString());

                info.AddressStatus = status;

                Brush brush;


                if (status == 0 || status == 4)
                {
                    info.AddressType = false;
                }
                else
                {
                    info.AddressType = true;
                }


                try
                {
                    info.AddressColor = row["AddressColor"] != DBNull.Value ? Convert.ToInt32(row["AddressColor"]) : 0;
                }
                catch (Exception e)
                {
                    info.AddressColor = 0;
                }


                info.PingTime = "N/A";
                info.PingStatusColor = Brushes.Azure;
                info.User = row["User"].ToString();
                info.Name = row["Name"].ToString();
                info.Organization = row["Organization"].ToString();
                info.Department = row["Department"].ToString();
                info.Group = row["UserGroup"].ToString();
                info.Phone = row["Phone"].ToString();
                info.HostName = row["HostName"].ToString();
                info.MacAddress = row["MacAddress"].ToString();
                info.LinkDeviceAssetTag = row["AssetTag"].ToString();
                info.LinkDeviceAssetNumber = row["AssetNumber"].ToString();
                info.LinkDevice = info.LinkDeviceAssetTag + info.LinkDeviceAssetNumber;
                info.LinkDeviceId = row["LinkDevice"].ToString();
                info.TagA = row["TagA"].ToString();
                info.TagB = row["TagB"].ToString();
                info.TagC = row["TagC"].ToString();
                info.TagD = row["TagD"].ToString();
                info.TagE = row["TagE"].ToString();
                info.TagF = row["TagF"].ToString();

                var tip = JoInTip(info);



                if (LoadMode == 0) //逐步加载
                {
                    await Task.Delay(1);

                }


                info.AddressToolTip = tip;

                IpAddressInfoLists.Add(info);
            }





        }



        //表示当前加载的网段
        LoadedNetworkSegment = tableName;

    }

    /// <summary>
    /// 根据输入内容拼接提示信息
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    private string JoInTip(IpAddressInfoListViewMode info)
    {
        string tip = "";

        switch (info.AddressStatus)
        {
            case 0:
                tip += $"地址状态: 网段地址\r";
                break;
            case 1:
                tip += $"地址状态: 未分配\r";
                break;
            case 2:
                tip += $"地址状态: 已分配\r";
                break;
            case 3:
                tip += $"地址状态: 分配未启用\r";
                break;
            case 4:
                tip += $"地址状态: 广播地址\r";
                break;

        }



        if (!string.IsNullOrWhiteSpace(info.User))
        {
            tip += $"地址用户: {info.Name}\r";

            if (!string.IsNullOrWhiteSpace(info.Organization))
            {
                tip += $"用户组织: {info.Organization}\r";

                if (!string.IsNullOrWhiteSpace(info.Department))
                {
                    tip += $"用户部门: {info.Department}\r";

                    if (!string.IsNullOrWhiteSpace(info.Group))
                    {
                        tip += $"用户群组: {info.Group}\r";
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(info.Phone))
            {
                tip += $"用户电话: {info.Phone}\r";
            }


        }

        if (!string.IsNullOrWhiteSpace(info.HostName))
        {
            tip += $"主机名称: {info.HostName}\r";
        }

        if (!string.IsNullOrWhiteSpace(info.MacAddress))
        {
            tip += $"主机MAC: {info.MacAddress}\r";
        }

        if (!string.IsNullOrWhiteSpace(info.LinkDeviceId))
        {
            tip += $"关联设备: {info.LinkDeviceAssetTag}{info.LinkDeviceAssetNumber}\r";
        }


        if (settingTags != null)
        {
            dynamic settings = JsonConvert.DeserializeObject(settingTags);

            if (!string.IsNullOrWhiteSpace(info.TagA))
            {
                if (!string.IsNullOrWhiteSpace(settings.TagA))
                {
                    tip += $"{settings.TagA}: {info.TagA}\r";
                }
                else
                {
                    tip += $"自定义标签A: {info.TagA}\r";
                }
            }
            if (!string.IsNullOrWhiteSpace(info.TagB))
            {
                if (!string.IsNullOrWhiteSpace(settings.TagB))
                {
                    tip += $"{settings.TagB}: {info.TagB}\r";
                }
                else
                {
                    tip += $"自定义标签B: {info.TagB}\r";
                }
            }
            if (!string.IsNullOrWhiteSpace(info.TagC))
            {
                if (!string.IsNullOrWhiteSpace(settings.TagC))
                {
                    tip += $"{settings.TagC}: {info.TagC}\r";
                }
                else
                {
                    tip += $"自定义标签C: {info.TagC}\r";
                }
            }
            if (!string.IsNullOrWhiteSpace(info.TagD))
            {
                if (!string.IsNullOrWhiteSpace(settings.TagD))
                {
                    tip += $"{settings.TagD}: {info.TagD}\r";
                }
                else
                {
                    tip += $"自定义标签D: {info.TagD}\r";
                }
            }
            if (!string.IsNullOrWhiteSpace(info.TagE))
            {
                if (!string.IsNullOrWhiteSpace(settings.TagE))
                {
                    tip += $"{settings.TagE}: {info.TagE}\r";
                }
                else
                {
                    tip += $"自定义标签A: {info.TagE}\r";
                }
            }
            if (!string.IsNullOrWhiteSpace(info.TagF))
            {
                if (!string.IsNullOrWhiteSpace(settings.TagF))
                {
                    tip += $"{settings.TagF}: {info.TagF}\r";
                }
                else
                {
                    tip += $"自定义标签F: {info.TagF}\r";
                }
            }

        }
        else
        {
            if (!string.IsNullOrWhiteSpace(info.TagA))
            {

                tip += $"自定义标签A: {info.TagA}\r";

            }
            if (!string.IsNullOrWhiteSpace(info.TagB))
            {


                tip += $"自定义标签B: {info.TagB}\r";

            }
            if (!string.IsNullOrWhiteSpace(info.TagC))
            {

                tip += $"自定义标签C: {info.TagC}\r";

            }
            if (!string.IsNullOrWhiteSpace(info.TagD))
            {

                tip += $"自定义标签D: {info.TagD}\r";

            }
            if (!string.IsNullOrWhiteSpace(info.TagE))
            {

                tip += $"自定义标签A: {info.TagE}\r";

            }
            if (!string.IsNullOrWhiteSpace(info.TagF))
            {

                tip += $"自定义标签F: {info.TagF}\r";

            }

        }


        return tip.TrimEnd('\r');
    }


    /// <summary>
    /// 订阅窗口关闭事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void IpAddressInfo_AddressAllocationWindowClosed(object sender, BoolEventArgs e)
    {
        if (e.Result == true)
        {
            //TODO
            //更新显示
            // LoadAddressInfo(DataBridge.DataBridge.NetworkTableName);

            // MessageBox.Show("信息传递成功！");
        }

    }






    /// <summary>
    /// 颜色代码转换为BRUSH
    /// </summary>
    /// <param name="colorCode"></param>
    /// <returns></returns>
    private Brush ColorToBrush(string colorCode)
    {
        SolidColorBrush brush = Brushes.Transparent;

        if (colorCode != null && colorCode != "")
        {
            brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(colorCode));
        }

        return brush;

    }


    /// <summary>
    /// 添加网段按钮
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AddButton_OnClick(object sender, RoutedEventArgs e)
    {
        AddNetworkWindow addNetwork = new AddNetworkWindow();

        //窗口放中间
        var window = Window.GetWindow(this);
        if (window != null)
        {
            addNetwork.Owner = window;
        }


        if (addNetwork.ShowDialog() == true)
        {

            // 当子窗口关闭后执行这里的代码
            LoadNetworkInfo2();

            //加载网段信息备注标签
            LoadTags();
        }

    }






    private void SetButton_Click(object sender, RoutedEventArgs e)
    {
        IpAddressTagWindow set = new IpAddressTagWindow();

        if (set.ShowDialog() == true)
        {

            LoadCustomTag();

        }

    }





    private void LogExpander_OnExpanded(object sender, RoutedEventArgs e)
    {


        if (LogExpander.IsExpanded)
        {
            DataBridge.DataBridge.OperationType = 4;
        }



    }

    private void LogExpander_OnCollapsed(object sender, RoutedEventArgs e)
    {
        DataBridge.DataBridge.OperationType = 0;
        SingleSelectMode.IsChecked = true;

    }





    private async void SingleSelectMode_OnClick(object sender, RoutedEventArgs e)
    {

        if (SingleSelectMode.IsChecked == true)
        {
            DataBridge.DataBridge.OperationType = 0;




            if (GetSelectedAddressCount() > 0)
            {

                ClearSelectedAddress();

                await LoadAddressInfo(DataBridge.DataBridge.NetworkTableName);
            }

        }

    }







    private void MultipleSelectMode_OnClick(object sender, RoutedEventArgs e)
    {
        if (MultipleSelectMode.IsChecked == true)
        {
            ClearSelectedAddress();
            DataBridge.DataBridge.OperationType = 1;
        }




    }



    private void OpenBrowserMode_OnClick(object sender, RoutedEventArgs e)
    {
        if (OpenBrowserMode.IsChecked == true)
        {
            DataBridge.DataBridge.OperationType = 2;
        }

    }

    private void PingMode_OnClick(object sender, RoutedEventArgs e)
    {
        if (PingMode.IsChecked == true)
        {
            DataBridge.DataBridge.OperationType = 3;
        }
    }



    private void LogModeButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (LogModeButton.IsChecked == true)
        {
            DataBridge.DataBridge.OperationType = 4;
        }
    }



    private void BrowserCombobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (BrowserCombobox.SelectedIndex != -1)
        {
            DataBridge.DataBridge.SelectBrowser = browserInfos[BrowserCombobox.SelectedIndex].Path;
        }
    }



    /// <summary>
    /// 批量ping测试
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void StatusTestButton_OnClick(object sender, RoutedEventArgs e)
    {
        ButtonProgressAssist.SetIsIndeterminate(StatusTestButton, true);
        await PingTesterClass.PingAddressesAsync(IpAddressInfoLists);

        int onlineHost = IpAddressInfoLists.Count(item => item.PingTime != "-1");
        OnlineHost.Text = onlineHost.ToString();

        ButtonProgressAssist.SetIsIndeterminate(StatusTestButton, false);
    }

    /// <summary>
    /// 批量分配
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MultipleAllocationButton_OnClick(object sender, RoutedEventArgs e)
    {
        int sum = DataBridge.DataBridge.IpAddressInfoLists.Sum(item => Convert.ToInt32(item.IsSelected));


        if (sum > 0)
        {
            AddressAllocationWindow addressAllocationWindow = new AddressAllocationWindow(DataBridge.DataBridge.IpAddressInfoLists);


            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                addressAllocationWindow.Owner = window;
            }

            if (addressAllocationWindow.ShowDialog() == true)
            {

                LoadAddressInfo(DataBridge.DataBridge.NetworkTableName);

                ClearSelectedAddress();

            }
        }

    }


    /// <summary>
    /// 清除已选地址
    /// </summary>
    private void ClearSelectedAddress()
    {

        // 使用LINQ查询筛选出IsSelected为true的所有项
        var selectedItems = DataBridge.DataBridge.IpAddressInfoLists.Where(item => item.IsSelected == true).ToList();
        // 如果需要，可以将这些筛选出来的项放入一个新的ObservableCollection中
        ObservableCollection<IpAddressInfoListViewMode> selectedItemsCollection = new ObservableCollection<IpAddressInfoListViewMode>(selectedItems);



        foreach (var item in selectedItemsCollection)
        {
            item.IsSelected = false;
        }

        DataBridge.DataBridge.SelectAddress.Add(1);
    }


    private async void ArpTestButton_Click(object sender, RoutedEventArgs e)
    {

        ButtonProgressAssist.SetIsIndeterminate(StatusTestButton, true);
        await PingTesterClass.PingAddressesAsync(IpAddressInfoLists);
        ButtonProgressAssist.SetIsIndeterminate(StatusTestButton, false);

        ButtonProgressAssist.SetIsIndeterminate(ArpTestButton, true);
        await DeviceInfoUpdater.UpdateDeviceInfoListAsync(IpAddressInfoLists);
        ButtonProgressAssist.SetIsIndeterminate(ArpTestButton, false);



    }




    private void PortComboBox_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Delete)//删除端口号
        {
            MessageBoxResult result = MessageBox.Show("是否从历史记录删除所选端口", "删除？", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {

                string sql = $"DELETE FROM  PortList  WHERE Port = {PortComboBox.Text}";

                GlobalVariables.DbService.ExecuteQuery(sql);

                LoadPort();
            }
        }
        else if (e.Key == Key.Enter) //保存端口号
        {
            try
            {
                int port = Convert.ToInt32(PortComboBox.Text);

                if (port > 0 && port < 65536)//判断端口是否合法
                {
                    string sqlTemp = $"SELECT COUNT(*) FROM PortList WHERE Port ={port}";

                    var countNum = DbClass.ExecuteScalarTableNum(sqlTemp);

                    if (countNum == 0)//判断端口是否已存在，不存在的情况
                    {

                        var portInfo = new ViewModels.DatabaseEntity.Network.PortListViewModel()
                        {
                            Port = port
                        };

                        GlobalVariables.DbService.InsertEntity("PortList", portInfo);

                        
                        DataBridge.DataBridge.SelectPort = port.ToString();
                        LoadPort();
                    }

                }
                else
                {
                    MessageBox.Show("端口不合法\r端口值应介于1-65536之间", "端口号有误", MessageBoxButton.OK, MessageBoxImage.Warning);
                    PortComboBox.Text = "";
                }


            }
            catch (Exception exception)
            {
                MessageBox.Show("端口号应该为整数数字", "端口号有误", MessageBoxButton.OK, MessageBoxImage.Warning);
                PortComboBox.Text = "";
            }







        }
    }

    /// <summary>
    /// 端口被改选
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PortComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (PortComboBox.SelectedIndex != -1)
        {
            DataBridge.DataBridge.SelectPort = portList[PortComboBox.SelectedIndex].Port.ToString();
        }
    }

    //协议被改选
    private void ProtocolCombobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {

        int index = ProtocolCombobox.SelectedIndex;



        switch (index)
        {
            case 0:
                DataBridge.DataBridge.Protocol = "http://";
                break;

            case 1:
                DataBridge.DataBridge.Protocol = "https://";
                break;
        }



    }

    private void SelectToggleButton_Checked(object sender, RoutedEventArgs e)
    {

        var toggleButton = sender as FrameworkElement;

        if (toggleButton != null)
        {
            var rowData = toggleButton.DataContext as IpAddressInfoListViewMode;

            if (rowData != null)
            {

                int addressStatus = rowData.AddressStatus;

                int sum = GetSelectedAddressCount();

                Console.WriteLine(sum);

                //1、判断选择的第一个地址是已分配还是未分配
                //1.1 判断是否是第一个地址
                if (sum == 1) //当前选择的是第一个地址
                {
                    //记录当前选择的地址是已分配还是未分配
                    DataBridge.DataBridge.AddressStatus = addressStatus;


                    DataBridge.DataBridge.SelectAddress.Add(rowData.Address);

                }
                else//当前选择的不是第一个地址
                {

                    //判断当前选择的地址是已分配还是未分配
                    if (addressStatus == DataBridge.DataBridge.AddressStatus)//同种类型的地址则添加到列表中，否则不添加
                    {

                        rowData.IsSelected = true;
                        DataBridge.DataBridge.SelectAddress.Add(rowData.Address);

                    }
                    else
                    {
                        rowData.IsSelected = false;
                        DataBridge.DataBridge.SelectAddress.Add(rowData.Address);
                    }

                    //AddressNumber.Text = SelectAddress.Count.ToString();

                }

            }
        }
    }

    private void SelectToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
    {
        var toggleButton = sender as FrameworkElement;
        if (toggleButton != null)
        {
            var rowData = toggleButton.DataContext as IpAddressInfoListViewMode;

            if (rowData != null)
            {
                rowData.IsSelected = false;
                DataBridge.DataBridge.SelectAddress.Add(rowData.Address);

            }
        }
    }


    /// <summary>
    /// 双击弹出分配页面
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AddressListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {


        DependencyObject dep = (DependencyObject)e.OriginalSource;

        // 迭代视觉树以找到 DataGridRow
        while ((dep != null) && !(dep is DataGridRow))
        {
            dep = VisualTreeHelper.GetParent(dep);
        }

        if (dep == null)
            return;

        // 获取 DataGridRow
        DataGridRow row = dep as DataGridRow;
        if (row == null)
            return;

        // 获取行数据对象
        var rowData = row.Item as IpAddressInfoListViewMode;
        if (rowData != null)
        {
            // 逻辑代码
            RunOnDoubleClick(rowData);
        }



    }


    private void RunOnDoubleClick(IpAddressInfoListViewMode rowData)
    {
        ClearSelectedAddress();

        rowData.IsSelected = true;

        DataBridge.DataBridge.AddressStatus = rowData.AddressStatus;//记录所选地址类型

        AddressAllocationWindow addressAllocationWindow = new AddressAllocationWindow();


        //窗口放中间
        var window = Window.GetWindow(this);
        if (window != null)
        {
            addressAllocationWindow.Owner = window;
        }


        if (addressAllocationWindow.ShowDialog() == true)
        {
            LoadAddressInfo(DataBridge.DataBridge.NetworkTableName);
        }
    }

    /// <summary>
    /// 切换列表显示或者图形化显示
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ShowModeButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (ShowModeButton.IsChecked == true)
        {

            LoadMode = 1;

            //ListButton.IsEnabled = false;

            OperationPanel.IsEnabled = false;


            //切换显示模式后清空已选地址
            //selectAddress.Clear();


            if (IpAddressInfoLists.Count > 0)
            {
                GraphicalPlan.Visibility = Visibility.Collapsed;
                AddressListView.Visibility = Visibility.Visible;

                //NetworkTreeView.IsEnabled = false;


                //LoadDataAsync(ipAddressInfoLists);

                //GraphicsButton.IsEnabled = true;
                //NetworkTreeView.IsEnabled = true;
                //ListButton.IsEnabled = true;
            }



        }
        else
        {
            LoadMode = 0;

            OperationPanel.IsEnabled = true;

            //GraphicsButton.IsEnabled = false;

            //SingleSelectMode.IsChecked = true;

            //操作类型改为单选模式
            //OperationType = 0;


            //AddressNumber.Text = null;
            //MultipleAllocationPanel.Visibility = Visibility.Collapsed;

            //切换显示模式后清空已选地址
            //selectAddress.Clear();

            //ListButton.IsEnabled = false;
            GraphicalPlan.Visibility = Visibility.Visible;
            AddressListView.Visibility = Visibility.Collapsed;
            //NetworkTreeView.IsEnabled = false;

            //if (IpAddressInfoLists.Count > 0)
            //{


            //    foreach (var info in IpAddressInfoLists)
            //    {
            //        var ip = new IpAddressInfo();

            //        ip.AddressAllocationWindowClosed += IpAddressInfo_AddressAllocationWindowClosed;

            //        ip.Margin = new Thickness(5);
            //        ip.DataContext = info;

            //       // AddressPanel.Children.Add(ip);
            //        await Task.Delay(1);
            //    }
            //}

            //ListButton.IsEnabled = true;
            //NetworkTreeView.IsEnabled = true;

            //GraphicsButton.IsEnabled = true;
        }
    }




    /// <summary>
    /// 清空网段搜索关键字
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClearSearchKeyWord_OnClick(object sender, RoutedEventArgs e)
    {
        SearchKeyWord.Text = null;
    }



    /// <summary>
    /// 重置排序
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ReSort_OnClick(object sender, RoutedEventArgs e)
    {
        AddressListView.ItemsSource = null;
        foreach (var column in AddressListView.Columns)
        {
            if (column.SortDirection != null)
            {
                column.SortDirection = null;
            }
        }
        AddressListView.ItemsSource = IpAddressInfoLists;
    }



    /// <summary>
    /// 清空已选地址
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClearSelected_OnClick(object sender, RoutedEventArgs e)
    {
        ClearSelectedAddress();
        DataBridge.DataBridge.SelectAddress.Clear();
        // SetBorderThicknessToZero(this);
    }


    /// <summary>
    /// 删除一个网段
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = new InputDialog();


        //窗口放中间
        var window = Window.GetWindow(this);
        if (window != null)
        {
            dialog.Owner = window;
        }

        if (dialog.ShowDialog() == true)
        {


            MessageQueue.Enqueue("开始删除");
        }
        else
        {
            MessageQueue.Enqueue("放弃操作");
        }


    }


    private void EditButton_OnClick(object sender, RoutedEventArgs e)
    {
        NetworkEditWindow newWindow = new NetworkEditWindow();

        //窗口放中间
        var window = Window.GetWindow(this);
        if (window != null)
        {
            newWindow.Owner = window;
        }


        if (newWindow.ShowDialog() == true)
        {

            // 当子窗口关闭后执行这里的代码
            LoadNetworkInfo2();

            //加载网段信息备注标签
            LoadTags();
        }
    }
}
