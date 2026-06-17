using Avalonia.Controls;
using Avalonia.Interactivity;
using ThinkITAM.DataBridge;

namespace ThinkITAM.Windows.Bookmark;

public partial class AddGroupWindow : Window
{
    public AddGroupWindow()
    {
        InitializeComponent();
        SaveButton.Click += SaveButton_OnClick;
    }

    private void SaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(GroupNameTextBox.Text))
            return;

        string groupName = GroupNameTextBox.Text;
        var sql = $"SELECT COUNT(*) FROM BookmarkGroupOrder WHERE TypeGroup = '{groupName}'";
        int count = Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(sql));

        if (count > 0)
        {
            // 已存在，只需取消删除标记
            sql = $"UPDATE BookmarkGroupOrder SET Del = NULL WHERE TypeGroup = '{groupName}'";
            GlobalVariables.DbService.ExecuteNonQuery(sql);
        }
        else
        {
            sql = $"INSERT INTO BookmarkGroupOrder (TypeGroup, DisplayOrder) VALUES ('{groupName}', 999)";
            GlobalVariables.DbService.ExecuteNonQuery(sql);
        }

        Close(true);
    }
}
