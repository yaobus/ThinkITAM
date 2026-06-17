using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;

namespace ThinkITAM.FunctionPage;

public partial class IndexPage : UserControl
{
    ObservableCollection<ViewModels.Index.IndexGroupViewModel> groups =
        new ObservableCollection<ViewModels.Index.IndexGroupViewModel>();

    private string? lastQuery = null;

    public IndexPage()
    {
        InitializeComponent();
    }

    private async void UserControl_Loaded(object? sender, RoutedEventArgs e)
    {
        GroupsListView.ItemsSource = groups;
        IndexPanel.ItemsSource = tags;
        LoadIndexGroups();
        DataBridge.DataBridge.modifyIndexTags.CollectionChanged += ModifyIndexTags_CollectionChanged;
    }

    private void ModifyIndexTags_CollectionChanged(object? sender,
        System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (lastQuery != null)
            ReloadIndex(lastQuery);
    }

    private async void ReloadIndex(string query)
    {
        tags.Clear();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            foreach (var row in rows)
            {
                var info = GroupsListView.SelectedItem as ViewModels.Index.IndexGroupViewModel;
                var tagInfo = new ViewModels.Index.IndexTagViewModel
                {
                    IndexId = row["IndexId"].ToString(),
                    Name = row["Name"].ToString(),
                    Protocol = row["Protocol"].ToString(),
                    Host = row["Host"].ToString(),
                    Port = row["Port"].ToString(),
                    Browser = row["Browser"].ToString()
                };

                string url = $"{tagInfo.Protocol}{tagInfo.Host}";
                tagInfo.Url = string.IsNullOrWhiteSpace(tagInfo.Port) ? url : $"{url}:{tagInfo.Port}";

                int colorIndex = 0;
                try { colorIndex = Convert.ToInt32(row["Color"]); }
                catch { colorIndex = 0; }
                tagInfo.Color = colorIndex;

                var item = tags.FirstOrDefault(t => t.IndexId == tagInfo.IndexId);
                if (item != null)
                {
                    item.Group = info?.Group;
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
    /// 加载分组信息
    /// </summary>
    private async void LoadIndexGroups(string? searchKeyWord = null)
    {
        groups.Clear();
        tags.Clear();

        string query;
        if (string.IsNullOrWhiteSpace(searchKeyWord))
        {
            query = "SELECT TypeGroup FROM BookmarkGroupOrder WHERE (Del != 1 OR Del IS NULL) ORDER BY DisplayOrder ASC;";
        }
        else
        {
            query = $"SELECT DISTINCT TypeGroup FROM Bookmark WHERE (Del != 1 OR Del IS NULL) AND Name LIKE '%{searchKeyWord}%';";
        }

        var rows = GlobalVariables.DbService.ExecuteQuery(query);

        int index = 0;
        foreach (var row in rows)
        {
            index++;
            var info = new ViewModels.Index.IndexGroupViewModel
            {
                Index = index,
                Group = row["TypeGroup"].ToString()
            };
            string sql = $"SELECT COUNT(*) FROM Bookmark WHERE TypeGroup = '{info.Group}'";
            info.Count = DbClass.ExecuteScalarTableNum(sql);

            await Task.Delay(30);
            groups.Add(info);
        }
    }

    ObservableCollection<ViewModels.Index.IndexTagViewModel> tags =
        new ObservableCollection<ViewModels.Index.IndexTagViewModel>();

    private async void GroupsListView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (GroupsListView.SelectedIndex != -1)
        {
            tags.Clear();

            var info = GroupsListView.SelectedItem as ViewModels.Index.IndexGroupViewModel;
            if (info == null) return;

            string sql;
            string keyWord = SearchKeyWord.Text ?? "";

            if (keyWord.Replace(" ", "").Length == 0)
                sql = $"SELECT * FROM Bookmark WHERE TypeGroup='{info.Group}'";
            else
                sql = $"SELECT * FROM Bookmark WHERE TypeGroup='{info.Group}' AND Name LIKE '%{SearchKeyWord.Text}%';";

            lastQuery = sql;

            var rows = GlobalVariables.DbService.ExecuteQuery(sql);

            foreach (var row in rows)
            {
                var tagInfo = new ViewModels.Index.IndexTagViewModel
                {
                    IndexId = row["IndexId"].ToString(),
                    Group = info.Group,
                    Name = row["Name"].ToString(),
                    Protocol = row["Protocol"].ToString(),
                    Host = row["Host"].ToString(),
                    Port = row["Port"].ToString(),
                    Browser = row["Browser"].ToString()
                };

                string url = $"{tagInfo.Protocol}{tagInfo.Host}";
                tagInfo.Url = string.IsNullOrWhiteSpace(tagInfo.Port) ? url : $"{url}:{tagInfo.Port}";

                int pinToStart;
                try { pinToStart = Convert.ToInt32(row["PinToStart"]); }
                catch { pinToStart = 0; }
                tagInfo.PinToStart = pinToStart;

                int colorIndex = 0;
                try { colorIndex = Convert.ToInt32(row["Color"]); }
                catch { colorIndex = 0; }
                tagInfo.Color = colorIndex;

                tags.Add(tagInfo);
                await Task.Delay(30);
            }

            DeleteButton.IsEnabled = true;
        }
        else
        {
            DeleteButton.IsEnabled = false;
        }
    }

    private void SearchButton_OnClick(object? sender, RoutedEventArgs e)
    {
        LoadIndexGroups(SearchKeyWord.Text);
    }

    private void ClearSearchKeyWord_OnClick(object? sender, RoutedEventArgs e)
    {
        SearchKeyWord.Text = "";
        LoadIndexGroups();
    }

    private void SearchKeyWord_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            LoadIndexGroups(SearchKeyWord.Text);
    }

    private async void AddButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var window = this.VisualRoot as Window;
        if (window == null) return;

        var newWindow = new Windows.NetworkManage.AddressCollectWindow();
        var result = await newWindow.ShowDialog<bool>(window);
        if (result)
        {
            if (lastQuery != null) ReloadIndex(lastQuery);
        }
    }


    private async void DeleteButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (GroupsListView.SelectedIndex == -1) return;

        var index = GroupsListView.SelectedIndex;
        var group = groups[index].Group;

        var confirm = await Services.DialogService.ShowConfirm(
            $"此操作将导致该及其分组中所有的标签被隐藏，是否继续删除？", "注意");

        if (confirm)
        {
            var sql = $"UPDATE BookmarkGroupOrder SET Del=1 WHERE TypeGroup = '{group}'";
            await GlobalVariables.DbService.ExecuteQueryAsync(sql);
        }

        LoadIndexGroups();
    }

    private void AddGroupButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var window = this.VisualRoot as Window;
        if (window == null) return;

        var newWindow = new Windows.Bookmark.AddGroupWindow();
        newWindow.ShowDialog(window).ContinueWith(_ =>
        {
            Dispatcher.UIThread.Post(() => LoadIndexGroups());
        });
    }
}
