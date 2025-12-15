using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DocumentFormat.OpenXml.EMMA;
using Newtonsoft.Json;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.ViewModels.AssetManage;
using ThinkITAM.Windows.NetworkManage;

namespace ThinkITAM.Windows.AssetManage;
/// <summary>
/// AddAssetLogWindow.xaml 的交互逻辑
/// </summary>
public partial class AddAssetLogWindow : Window
{
    public AddAssetLogWindow(AssetViewModel assetInfo)
    {
        InitializeComponent();
        inputAssetInfo=assetInfo;
        AssetNumber.Text = inputAssetInfo.AssetNumber;
    }

    private AssetViewModel inputAssetInfo;
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
        EventDate.SelectedDate = DateTime.Today;
    }

    private void AddAssetLogWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
       LoadTags();
    }

    /// <summary>
    /// 加载自定义标签
    /// </summary>
    private void LoadTags()
    {
        var tags = DbClass.LoadWindowTag("AddAssetLog");

        if (tags != null)
        {
            dynamic settings = JsonConvert.DeserializeObject(tags);
            if (settings != null)
            {
                LabelA.Content = settings.TagA;
                LabelB.Content = settings.TagB;
                LabelC.Content = settings.TagC;
                LabelD.Content = settings.TagD;
                LabelE.Content = settings.TagE;
                LabelF.Content = settings.TagF;
            }

        }



    }




    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        var info = CheckInput();

        if (info.Item1 > 0)
        {
            MessageBox.Show(info.Item2, $"当前存在以下{info.Item1}项问题需要注意", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        else
        {
            var uid = DbClass.GetNextAvailableNumber("AssetLog", "UID");

            var entity = new
            {
                UID = uid,
                AssetId = inputAssetInfo.AssetId,
                EventDate= EventDate.Text,
                EventContent= EventContent.Text,
                AboutUser= DataBridge.DataBridge.SelectPeopleViewModel.UserId,
                Note= Note.Text,
                TagA =TagA.Text,
                TagB =TagB.Text,
                TagC =TagC.Text,
                TagD =TagD.Text,
                TagE =TagE.Text,
                TagF =TagF.Text
            };

            GlobalVariables.DbService.InsertEntity("AssetLog", entity);
            this.DialogResult=true;
        }
    }

    /// <summary>
    /// 检查输入是否合规
    /// </summary>
    /// <returns></returns>
    private (int, string) CheckInput()
    {
        int index = 0;
        string message = "当前存在以下问题需要解决:\r";


        if (string.IsNullOrWhiteSpace(EventDate.Text))
        {
            index++;
            message += index.ToString() + ":事件日期不得为空\r";
        }


        if (string.IsNullOrWhiteSpace(EventContent.Text))
        {
            index++;
            message += index.ToString() + ":事件内容不得为空\r";
        }



        if (string.IsNullOrWhiteSpace(AssetUser.Text))
        {
            index++;
            message += index.ToString() + ":相关人员信息不得为空\r";
        }




        return (index, message);
    }

    private void LogTagsSetting_OnClick(object sender, RoutedEventArgs e)
    {
        var newWindow = new AddAssetLogWindowSet();

        var window = Window.GetWindow(this);

        if (window != null)
        {
            newWindow.Owner = window;
        }

        newWindow.ShowDialog();

        if (newWindow.DialogResult == true)
        {
            LoadTags();
        }
    }
}
