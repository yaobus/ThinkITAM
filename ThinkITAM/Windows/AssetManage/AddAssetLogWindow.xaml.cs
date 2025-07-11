using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using ThinkITAM.Windows.NetworkManage;

namespace ThinkITAM.Windows.AssetManage;
/// <summary>
/// AddAssetLogWindow.xaml 的交互逻辑
/// </summary>
public partial class AddAssetLogWindow : Window
{
    public AddAssetLogWindow()
    {
        InitializeComponent();
    }

    private void FindUser_OnClick(object sender, RoutedEventArgs e)
    {
        //DataBridge.DataBridge.SelectAssetInfo = null;
        FindUserWindow find = new FindUserWindow();
        find.Owner = this;
        if (find.ShowDialog() == true)
        {

            AssetUser.Text = DataBridge.DataBridge.SelectPeopleViewModel.Name;


        }
    }

    private void EnableDate_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        EnableDate.SelectedDate = DateTime.Today;
    }
}
