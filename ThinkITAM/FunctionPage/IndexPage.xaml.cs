using ThinkITAM.DatabaseOperation;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThinkITAM.FunctionClass;
using System.Text.RegularExpressions;
using ThinkITAM.Windows.NetworkManage;
using ThinkITAM.UserControls.General;
using MaterialDesignThemes.Wpf;
using ThinkITAM.DataBridge;
using ThinkITAM.UserControls.IndexPage;

namespace ThinkITAM.FunctionPage
{
    /// <summary>
    /// IndexPage.xaml 的交互逻辑
    /// </summary>
    public partial class IndexPage : UserControl
    {
        public IndexPage()
        {
            InitializeComponent();
        }

        ObservableCollection<ViewModels.Index.IndexGroupViewModel> groups =
            new ObservableCollection<ViewModels.Index.IndexGroupViewModel>();

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
           
            GroupsListView.ItemsSource = groups;
            IndexPanel.ItemsSource = tags;


            LoadIndexGroups();

            DataBridge.DataBridge.modifyIndexTags.CollectionChanged += ModifyIndexTags_CollectionChanged;
        }

        private void ModifyIndexTags_CollectionChanged(object? sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //tags.Clear();
            //LoadIndexGroups();
            ReloadIndex(lastQuery);
        }


        private async void ReloadIndex(string query = null)
        {
            if (query != null)
            {

                var rows = GlobalVariables.DbService.ExecuteQuery(query);

                foreach (var row in rows)
                {
                    var info = GroupsListView.SelectedItem as ViewModels.Index.IndexGroupViewModel;
                    var tagInfo = new ViewModels.Index.IndexTagViewModel();
                    tagInfo.IndexId = row["IndexId"].ToString();
                    tagInfo.Name = row["Name"].ToString();
                    tagInfo.Protocol = row["Protocol"].ToString();
                    tagInfo.Host = row["Host"].ToString();
                    tagInfo.Port = row["Port"].ToString();
                    tagInfo.Browser = row["Browser"].ToString();

                    string url = $"{tagInfo.Protocol}{tagInfo.Host}";

                    if (tagInfo.Port.Length == 0)//未配置端口
                    {
                        tagInfo.Url = url;
                    }
                    else
                    {
                        tagInfo.Url = $"{url}:{tagInfo.Port}";
                    }


                    int colorIndex = 0;
                    try
                    {
                        colorIndex = Convert.ToInt32(row["Color"]);
                    }
                    catch (Exception exception)
                    {
                        colorIndex = 0;
                    }

                    tagInfo.Color = colorIndex;


                    var item = tags.FirstOrDefault(item => item.IndexId == tagInfo.IndexId);

                    if (item != null)
                    {
                        item.Group = info.Group;
                        item.Name = tagInfo.Name;
                        item.Protocol = tagInfo.Protocol;
                        item.Host = tagInfo.Host;
                        item.Port = tagInfo.Port;
                        item.Browser = tagInfo.Browser;
                        item.Url = tagInfo.Url;
                        item.Color = tagInfo.Color;

                    }
                    else
                    {
                        tags.Add(tagInfo);
                    }


                }




            }

        }

        /// <summary>
        /// 上一次查询
        /// </summary>
        private string lastQuery = null;

        /// <summary>
        /// 加载组织信息
        /// </summary>
        private async void LoadIndexGroups(string searchKeyWord = "")
        {
            groups.Clear();
            tags.Clear();
            string query;
            if (searchKeyWord.Replace(" ","").Length == 0)
            {
                query = $"SELECT DISTINCT TypeGroup FROM  Bookmark WHERE Del != 0 OR Del IS NULL;";
            }
            else
            {
                query = $"SELECT DISTINCT TypeGroup FROM Bookmark WHERE Del != 0 OR Del IS NULL AND Name LIKE '%{searchKeyWord}%';";

            }


            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            int index = 0;

            foreach (var row in rows)
            {
                index++;
                var info = new ViewModels.Index.IndexGroupViewModel();
                info.Index = index;
                info.Group = row["TypeGroup"].ToString();
                string sql = $"SELECT COUNT(*) FROM Bookmark WHERE TypeGroup = '{info.Group}'";

                info.Count = DbClass.ExecuteScalarTableNum(sql);

                await Task.Delay(50);
                groups.Add(info);
            }

        }




        ObservableCollection<ViewModels.Index.IndexTagViewModel> tags = new ObservableCollection<ViewModels.Index.IndexTagViewModel>();

        private async void GroupsListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GroupsListView.SelectedIndex != -1)
            {
                tags.Clear();

                var info = GroupsListView.SelectedItem as ViewModels.Index.IndexGroupViewModel;

                string sql;

                string kyeWord = SearchKeyWord.Text;

                if (kyeWord.Replace(" ", "").Length == 0)
                {
                    sql = $"SELECT * FROM Bookmark WHERE TypeGroup='{info.Group}'";
                }
                else
                {

                    sql = $"SELECT * FROM Bookmark WHERE TypeGroup='{info.Group}' AND Name LIKE '%{SearchKeyWord.Text}%';";

                }


                lastQuery = sql;

                var rows =  GlobalVariables.DbService.ExecuteQuery(sql);

                foreach (var row in rows)
                {
                    var tagInfo = new ViewModels.Index.IndexTagViewModel();
                    tagInfo.IndexId = row["IndexId"].ToString();
                    tagInfo.Group = info.Group;
                    tagInfo.Name = row["Name"].ToString();
                    tagInfo.Protocol = row["Protocol"].ToString();
                    tagInfo.Host = row["Host"].ToString();
                    tagInfo.Port = row["Port"].ToString();
                    tagInfo.Browser = row["Browser"].ToString();
                    tagInfo.PinToStart=  Convert.ToInt32(row["PinToStart"]);
                    string url = $"{tagInfo.Protocol}{tagInfo.Host}";

                    if (tagInfo.Port.Length == 0)//未配置端口
                    {
                        tagInfo.Url = url;
                    }
                    else
                    {
                        tagInfo.Url = $"{url}:{tagInfo.Port}";
                    }



                    int colorIndex = 0;
                    try
                    {
                        colorIndex = Convert.ToInt32(row["Color"]);
                    }
                    catch (Exception exception)
                    {
                        colorIndex = 0;
                    }

                    tagInfo.Color = colorIndex;
                    tags.Add(tagInfo);
                    await Task.Delay(50);
                }

                DeleteButton.IsEnabled = true;
            }
            else
            {
                DeleteButton.IsEnabled = false;
            }


        }

        /// <summary>
        /// 搜索按钮被单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchButton_OnClick(object sender, RoutedEventArgs e)
        {
            LoadIndexGroups(SearchKeyWord.Text);
        }

        private void ClearSearchKeyWord_OnClick(object sender, RoutedEventArgs e)
        {
            SearchKeyWord.Text="";
            LoadIndexGroups();
        }

        private void SearchKeyWord_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoadIndexGroups(SearchKeyWord.Text);
            }


            
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {

            AddressCollectWindow addressCollectWindow = new AddressCollectWindow();
            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                addressCollectWindow.Owner = window;
            }

            

            if (addressCollectWindow.ShowDialog() == true)
            {

               ReloadIndex(lastQuery);

            }

        }

        private async void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new ConfirmationDialog
            {
                Title = "注意",
                Prompt = $"此操作将导致该及其分组中所有的标签被隐藏，是否继续删除？",
                ConfirmButtonText = "继续",


            };

            // 显示对话框
            bool result = (bool)await DialogHost.Show(dialog, "MessageDialogHost");

            if (result)
            {
                int index = GroupsListView.SelectedIndex;
                string group = groups[index].Group;
                string sql = $"UPDATE  Bookmark SET Del='0' WHERE TypeGroup = '{group}'";
                await GlobalVariables.DbService.ExecuteQueryAsync(sql);
            }

            LoadIndexGroups();

        }
    }
}
