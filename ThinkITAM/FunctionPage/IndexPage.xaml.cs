using ThinkITAM.DatabaseOperation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
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
using ThinkITAM.ChildrenWindows.NetworkManage;
using ThinkITAM.UserControls.General;
using MaterialDesignThemes.Wpf;
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

        ObservableCollection<ViewModes.Index.IndexGroupViewModel> groups =
            new ObservableCollection<ViewModes.Index.IndexGroupViewModel>();

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dbClass = new DbClass(DataBridge.DataBridge.dbFilePath);
            dbClass.OpenConnection();

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

                SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
                SQLiteDataReader reader = command.ExecuteReader();


                while (reader.Read())
                {
                    var info = GroupsListView.SelectedItem as ViewModes.Index.IndexGroupViewModel;
                    var tagInfo = new ViewModes.Index.IndexTagViewModel();
                    tagInfo.IndexId = reader["IndexId"].ToString();
                    tagInfo.Name = reader["Name"].ToString();
                    tagInfo.Protocol = reader["Protocol"].ToString();
                    tagInfo.Host = reader["Host"].ToString();
                    tagInfo.Port = reader["Port"].ToString();
                    tagInfo.Browser = reader["Browser"].ToString();

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
                        colorIndex = Convert.ToInt32(reader["Color"]);
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
                query = $"SELECT DISTINCT \"Group\" FROM \"Index\" WHERE Del != 0 OR Del IS NULL;";
            }
            else
            {
                query = $"SELECT DISTINCT \"Group\" FROM \"Index\" WHERE Del != 0 OR Del IS NULL AND \"Name\" LIKE '%{searchKeyWord}%';";

            }


            

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();

            int index = 0;
            while (reader.Read())
            {
                index++;
                var info = new ViewModes.Index.IndexGroupViewModel();
                info.Index = index;
                info.Group = reader["Group"].ToString();
                string sql = $"SELECT COUNT(*) FROM 'Index' WHERE `group` = '{info.Group}'";

                info.Count = dbClass.ExecuteScalarTableNum(sql, dbClass.connection);

                await Task.Delay(50);
                groups.Add(info);
            }




        }



        private DbClass dbClass;

        ObservableCollection<ViewModes.Index.IndexTagViewModel> tags = new ObservableCollection<ViewModes.Index.IndexTagViewModel>();

        private async void GroupsListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GroupsListView.SelectedIndex != -1)
            {
                tags.Clear();

                var info = GroupsListView.SelectedItem as ViewModes.Index.IndexGroupViewModel;

                string sql;

                string kyeWord = SearchKeyWord.Text;

                if (kyeWord.Replace(" ", "").Length == 0)
                {
                    sql = $"SELECT * FROM \"Index\" WHERE \"Group\"='{info.Group}'";
                }
                else
                {

                    sql = $"SELECT * FROM \"Index\" WHERE \"Group\"='{info.Group}' AND \"Name\" LIKE '%{SearchKeyWord.Text}%';";

                }


                lastQuery = sql;

                SQLiteCommand command = new SQLiteCommand(sql, dbClass.connection);
                SQLiteDataReader reader = command.ExecuteReader();


                while (reader.Read())
                {

                    var tagInfo = new ViewModes.Index.IndexTagViewModel();
                    tagInfo.IndexId= reader["IndexId"].ToString();
                    tagInfo.Group = info.Group;
                    tagInfo.Name = reader["Name"].ToString();
                    tagInfo.Protocol = reader["Protocol"].ToString();
                    tagInfo.Host = reader["Host"].ToString();
                    tagInfo.Port = reader["Port"].ToString();
                    tagInfo.Browser = reader["Browser"].ToString();
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
                        colorIndex = Convert.ToInt32(reader["Color"]);
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
                TitleColor = Brushes.AliceBlue,
                PromptColor = Brushes.AliceBlue

            };

            // 显示对话框
            bool result = (bool)await DialogHost.Show(dialog, "MessageDialogHost");

            if (result)
            {
                int index = GroupsListView.SelectedIndex;
                string group = groups[index].Group;
                string sql = $"UPDATE  \"Index\" SET \"Del\"='0' WHERE \"Group\" = '{group}'";
                dbClass.ExecuteQuery(sql);
            }

            LoadIndexGroups();

        }
    }
}
