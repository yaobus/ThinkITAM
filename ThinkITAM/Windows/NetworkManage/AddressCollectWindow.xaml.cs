using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.UserControls.General;
using ThinkITAM.ViewModels.Index;
using ThinkITAM.ViewModels.Preset;
using ListBoxItem = System.Windows.Controls.ListBoxItem;

namespace ThinkITAM.Windows.NetworkManage;
/// <summary>
/// AddressCollectWindow.xaml 的交互逻辑
/// </summary>
public partial class AddressCollectWindow : Window
{
    public AddressCollectWindow(string addressInput = null, IndexTagViewModel indexTag = null)
    {
        InitializeComponent();

        LoadGroupsInfo();
        LoadBrowserInfo();

        if (indexTag != null)
        {
            this.DataContext = indexTag;
            updateTag = indexTag;
            dbIndexId = updateTag.IndexId;





            var matchedItem = browserInfos.FirstOrDefault(item => item.Path == indexTag.Browser);

            //Console.WriteLine(matchedItem.Path);

            if (matchedItem != null)
            {
                int index = browserInfos.IndexOf(matchedItem);

                indexTag.Index = index;
                BrowserCombobox.SelectedIndex = index;
                //Console.WriteLine($"BrowserIndex{index}");
            }
            else
            {
                indexTag.Index = -1;
                BrowserCombobox.SelectedIndex = -1;
                //Console.WriteLine($"BrowserIndex -1");
            }



        }
        else
        {
            address = addressInput;
        }
    }
    private string address;
    private IndexTagViewModel updateTag;

    private readonly string dbIndexId;






    private void AddressCollectWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        Groups.ItemsSource = groups;


        BrowserCombobox.ItemsSource = browserInfos;
        if (address != null)
        {
            var info = new IndexTagViewModel();
            info.Host = address;
            info.Protocol = "http://";
            info.Index = -1;
            this.DataContext = info;
        }


        if (address == null && updateTag == null)
        {
            var tempTag = new IndexTagViewModel()
            {
                Protocol = "Http://",
                Index = -1

            };

            this.DataContext = tempTag;
        }



    }

    ObservableCollection<string> groups = new ObservableCollection<string>();

    /// <summary>
    /// 加载组织信息
    /// </summary>
    private void LoadGroupsInfo()
    {
        groups.Clear();

        string query = $"SELECT DISTINCT  TypeGroup  FROM BookmarkGroupOrder  WHERE (Del != 1 OR Del IS NULL);";



        var rows = GlobalVariables.DbService.ExecuteQuery(query);

        foreach (var row in rows)
        {
            groups.Add(row["TypeGroup"].ToString());
        }



    }



    private ObservableCollection<BrowserInfoViewModel> browserInfos = new ObservableCollection<BrowserInfoViewModel>();

    /// <summary>
    /// 加载浏览器路径
    /// </summary>
    private void LoadBrowserInfo()
    {

        browserInfos.Clear();
        //browserInfos.Add(new BrowserInfoViewModel());

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


    }



    /// <summary>
    /// 删除分组
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
    {
        int index = Groups.SelectedIndex;


        if (index != -1)
        {

            CollectDeleteDialogHost.IsOpen = true;

        }
    }

    private void DeleteDialogHost_OnDialogClosed(object sender, DialogClosedEventArgs eventargs)
    {

    }

    /// <summary>
    /// 确实删除
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Agree_OnClick(object sender, RoutedEventArgs e)
    {
        int index = Groups.SelectedIndex;
        string group = groups[index];
        string sql = $"UPDATE  BookmarkGroupOrder  SET Del=1 WHERE TypeGroup = '{group}'";

        GlobalVariables.DbService.ExecuteNonQuery(sql);
        Reject_OnClick(null, null);
        LoadGroupsInfo();
    }

    private void Reject_OnClick(object sender, RoutedEventArgs e)
    {
        // 直接调用 DialogHost 的 IsOpen 属性来打开对话框
        // 打开对话框
        CollectDeleteDialogHost.IsOpen = false;
    }

    private async void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {

        if (updateTag != null)//更新
        {
            var info = CheckInput();

            if (info.Item1 == 0)//输入合法性通过
            {
                //检查地址是否已存在
                string url = $"{Protocol.Text}{Host.Text}{Port.Text}";

                string sqlTemp = $"SELECT COUNT(*) FROM Bookmark WHERE ( TypeGroup='{updateTag.Group}' AND Protocol='{Protocol.Text}' AND Host='{Host.Text}' AND Port='{Port.Text}')";

                Console.WriteLine("232:"+sqlTemp);

                //查询记录是否存在
                var countNum = DbClass.ExecuteScalarTableNum(sqlTemp);


                if (countNum > 1)
                {
                    MessageBox.Show($"地址已存在,请勿重复添加\r\n{url}", "注意", MessageBoxButton.OK);


                }
                else
                {

                    string browser = "";
                    if (BrowserCombobox.SelectedIndex != -1)
                    {
                        browser = browserInfos[BrowserCombobox.SelectedIndex].Path;
                    }

                    int status = 0;

                    if (PinToStart.IsChecked == true)
                    {
                        status = 1;
                    }



                    var bookmarkInfo = new ViewModels.DatabaseEntity.Bookmark.BookmarkViewModel
                    {
                        IndexId = dbIndexId,
                        TypeGroup = Groups.Text,
                        Name = Name.Text,
                        Protocol = Protocol.Text,
                        Host = Host.Text,
                        Port = Port.Text,
                        Color = IndexColor.SelectedIndex,
                        Browser = browser,
                        PinToStart = status
                    };

                    var conditions = new { IndexId = $"{dbIndexId}" };


                    GlobalVariables.DbService.UpdateEntity("Bookmark", bookmarkInfo, conditions);

                    GroupCheck(Groups.Text);//分组检查，如果不存在则添加

                    DataBridge.DataBridge.modifyIndexTags.Add(url);

                    this.Close();
                }


            }
            else
            {
                //var dialog = new ConfirmationDialog
                //{
                //    Title = "注意",
                //    Prompt = $"{info.Item2}",
                //    ConfirmButtonText = "确认",


                //};

                //// 显示对话框
                //await DialogHost.Show(dialog, "CollectDeleteDialogHost");

                MessageBox.Show(info.Item2, "注意", MessageBoxButton.OK);


            }

        }
        else//新增数据
        {
            var info = CheckInput();

            if (info.Item1 == 0)//通过
            {
                //检查地址是否已存在
                string url = $"{Protocol.Text}{Host.Text}{Port.Text}";

                string sqlTemp = $"SELECT COUNT(*) FROM Bookmark WHERE (Protocol='{Protocol.Text}' AND Host='{Host.Text}' AND Port='{Port.Text}')";

                Console.WriteLine("320:" + sqlTemp);
                //查询记录是否存在
                var countNum = DbClass.ExecuteScalarTableNum(sqlTemp);


                if (countNum > 0)
                {
                    //var dialog = new ConfirmationDialog
                    //{
                    //    Title = "注意",
                    //    Prompt = ,
                    //    ConfirmButtonText = "确认",


                    //};

                    //// 显示对话框
                    //await DialogHost.Show(dialog, "CollectDeleteDialogHost");

                    MessageBox.Show($"地址已存在,请勿重复添加\r\n{url}", "注意", MessageBoxButton.OK);



                }
                else
                {
                    string browser = "";
                    if (BrowserCombobox.SelectedIndex != -1)
                    {
                        browser = browserInfos[BrowserCombobox.SelectedIndex].Path;
                    }

                    string group = Groups.Text;

                    string sql2 = $"SELECT COUNT(TypeGroup) FROM BookmarkGroupOrder WHERE TypeGroup = '{group}' AND  Del = 1 ";

                    Console.WriteLine("356:" + sql2);
                    if (DbClass.ExecuteScalarTableNum(sql2) > 0)
                    {

                        var dialog = new ConfirmationDialog
                        {
                            Title = "注意",
                            Prompt = $"数据库中存在名为{group}的分组，但是被标记为删除，继续添加将会恢复该分组，是否继续添加？",
                            ConfirmButtonText = "继续",


                        };

                        // 显示对话框
                        bool result = (bool)await DialogHost.Show(dialog, "CollectDeleteDialogHost");

                        if (result)
                        {
                            sql2 = $"UPDATE BookmarkGroupOrder SET Del = NULL WHERE TypeGroup = '{group}'";

                            GlobalVariables.DbService.ExecuteNonQuery(sql2);
                        }
                        else
                        {
                            return;
                        }


                    }

                    string indexId = $"X{AssetCodeClass.GenerateChecksum(AssetIdCreate.CreateAssetId(Guid.NewGuid().ToString())).ToUpper()}";

                    int status = 0;

                    if (PinToStart.IsChecked == true)
                    {
                        status = 1;
                    }

                    var bookmarkInfo = new ViewModels.DatabaseEntity.Bookmark.BookmarkViewModel
                    {
                        IndexId = indexId,
                        TypeGroup = group,
                        Name = Name.Text,
                        Protocol = Protocol.Text,
                        Host = Host.Text,
                        Port = Port.Text,
                        Color = IndexColor.SelectedIndex,
                        Browser = browser,
                        PinToStart = status
                    };


                    GlobalVariables.DbService.InsertEntity("Bookmark", bookmarkInfo);
                    GroupCheck(Groups.Text);//分组检查，如果不存在则添加
                    DataBridge.DataBridge.modifyIndexTags.Add(url);
                    this.Close();
                }


            }
            else
            {
                //var dialog = new ConfirmationDialog
                //{
                //    Title = "注意",
                //    Prompt = $"{info.Item2}",
                //    ConfirmButtonText = "确认",


                //};

                //// 显示对话框
                //await DialogHost.Show(dialog, "CollectDeleteDialogHost");

                MessageBox.Show(info.Item2, "注意", MessageBoxButton.OK);


            }


        }




    }


    private void GroupCheck(string groupName)
    {
        var sql = $"SELECT COUNT(TypeGroup) FROM BookmarkGroupOrder WHERE TypeGroup = '{groupName}' AND ( Del != 1 OR Del IS NULL)";

        Console.WriteLine("449:" + sql);
        var count = Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(sql));

        if (count == 0)//不存在，则添加
        {
            var index = DbClass.GetNextAvailableNumber("BookmarkGroupOrder", "DisplayOrder");
            var info = new { TypeGroup = groupName, DisplayOrder = index };
            GlobalVariables.DbService.InsertEntity("BookmarkGroupOrder", info);
        }



    }


    private (int, string) CheckInput()
    {
        int index = 0;
        string message = "当前存在以下问题需要解决:\r";

        if (string.IsNullOrWhiteSpace(Host.Text))
        {
            index++;
            message += index.ToString() + ":主机地址不得为空\r";
        }


        //if (Host.Text == null || Host.Text.Replace(" ", "").Length < 2)
        //{
        //    index++;
        //    message += index.ToString() + ":主机地址不得为空\r";
        //}

        if (string.IsNullOrWhiteSpace(Protocol.Text))
        {
            index++;
            message += index.ToString() + ":协议头不得为空\r";
        }


        //if (Protocol.Text == null || Protocol.Text.Replace(" ", "").Length < 2)
        //{
        //    index++;
        //    message += index.ToString() + ":协议头不得为空\r";
        //}


        if (Groups.SelectedIndex == -1)
        {
            // 如果 ComboBox 的 Text 属性也为空，则抛出异常
            if (string.IsNullOrWhiteSpace(Groups.Text))
            {
                index++;
                message += index.ToString() + ":未设置或选择分组\r";
            }
        }

        if (string.IsNullOrWhiteSpace(Name.Text))
        {
            index++;
            message += index.ToString() + ":标签名称不得为空\r";
        }

        //if (Name.Text == null || Name.Text.Replace(" ", "").Length < 2)
        //{
        //    index++;
        //    message += index.ToString() + ":标签名称不得为空\r";
        //}

        return (index, message);
    }

    private void IndexColor_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        int index = IndexColor.SelectedIndex;

        int x = 0;


        if (index != -1)
        {
            foreach (var selectedItem in IndexColor.Items)
            {
                var item = selectedItem as ListBoxItem;

                if (item != null && index == x)
                {
                    item.Opacity = 1;
                    item.BorderBrush = SystemColors.ActiveBorderBrush;
                    item.BorderThickness = new Thickness(2);


                }
                else
                {
                    item.Opacity = 0.1;
                    item.BorderBrush = null;
                    item.BorderThickness = new Thickness(0);

                }

                x++;
            }

        }
    }


}
