using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;

namespace ThinkITAM.UserControls.Asset
{
/// <summary>
    /// AssetTagTemplateUserControl.xaml 的交互逻辑
/// </summary>
public partial class AssetTagTemplateUserControl : UserControl
{
public AssetTagTemplateUserControl()
{
InitializeComponent();
CustomizeCompanyTag.Content=DataBridge.DataBridge.CompanyName;
}

private string prefix;
private void CustomizeTag_OnMouseDown(object sender, MouseButtonEventArgs e)
{
//如果按下的
if (e.LeftButton == MouseButtonState.Pressed)
{
SaveCustomizeTagDialogHost.IsOpen = true;
//如果CustomizeCompanyTag.Content不为空则CompanyNameTextBox.Text = CustomizeCompanyTag.Content.ToString();
if ( CustomizeCompanyTag.Content != null)
{
CompanyNameTextBox.Text = CustomizeCompanyTag.Content.ToString()!;
prefix = CompanyNameTextBox.Text!;
}


}

}


/// <summary>
    /// 保存自定义标签
/// </summary>
/// <param name="sender"></param>
/// <param name="eventArgs"></param>
private void SaveCustomizeTagDialogHost_OnDialogClosed(object sender, DialogClosedEventArgs eventArgs)
{


if (!string.IsNullOrWhiteSpace(DataBridge.DataBridge.CompanyName))
{
CustomizeCompanyTag.Content = DataBridge.DataBridge.CompanyName;
}
else
{
CustomizeCompanyTag.Content = "ThinkITAM-点击修改本字段标签";

}
}

private void SaveCustomizeTag_OnClick(object sender, RoutedEventArgs e)
{
//保存自定义标签

if (prefix != CompanyNameTextBox.Text && !string.IsNullOrWhiteSpace(CompanyNameTextBox.Text))
{
string sqlTemp = $"SELECT COUNT(*) FROM CustomSetting WHERE CustomOption ='CustomizeCompanyTag'";

//查询记录是否存在
var countNum = DbClass.ExecuteScalarTableNum(sqlTemp);

if (countNum == 0)//判断记录是否存在，如果不存在则插入数据
{
var info = new { CustomOption = "CustomizeCompanyTag", Content = CompanyNameTextBox.Text};


GlobalVariables.DbService.InsertEntity("CustomSetting", info);


}
else//存在
{
string sql = $"UPDATE  CustomSetting  SET  Content  = '{CompanyNameTextBox.Text}' WHERE CustomOption ='CustomizeCompanyTag'";

GlobalVariables.DbService.ExecuteNonQuery(sql);
}

SaveCustomizeTagDialogHost.IsOpen = false;

LoadCustomizeCompanyTag();

AssetTagTemplateUserControl_OnLoaded(null, null);
}
else
{


SaveCustomizeTagDialogHost.IsOpen = false;
LoadCustomizeCompanyTag();
}
}

/// <summary>
    /// 加载用户自定义公司标签
/// </summary>
private void LoadCustomizeCompanyTag()
{
var query = "SELECT Content FROM CustomSetting WHERE CustomOption='CustomizeCompanyTag';";

var prefix = GlobalVariables.DbService.ExecuteScalar(query);

if (prefix != null)
{
DataBridge.DataBridge.CompanyName = prefix.ToString();
}
else
{
// SaveCustomizeTagDialogHost.IsOpen = true;
}



}


private void CancelButton_OnClick(object sender, RoutedEventArgs e)
{
SaveCustomizeTagDialogHost.IsOpen = false;
LoadCustomizeCompanyTag();
AssetTagTemplateUserControl_OnLoaded(null,null);
}


private void AssetTagTemplateUserControl_OnLoaded(object sender, RoutedEventArgs e)
{
if (!string.IsNullOrWhiteSpace(DataBridge.DataBridge.CompanyName))
{
CustomizeCompanyTag.Content = DataBridge.DataBridge.CompanyName;
}


}
}
}
