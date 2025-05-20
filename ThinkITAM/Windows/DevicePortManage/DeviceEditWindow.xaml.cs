using System;
using System.Collections;
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
using Newtonsoft.Json;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.ViewModels.AssetManage;
using ThinkITAM.ViewModels.NetworkManage;

namespace ThinkITAM.Windows.DevicePortManage;
/// <summary>
/// DeviceEditWindow.xaml 的交互逻辑
/// </summary>
public partial class DeviceEditWindow : Window
{
    public DeviceEditWindow()
    {
        InitializeComponent();
    }

    private void DeviceEditWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        LoadTags();
        LoadHierarchyInfo();
        LoadDeviceInfo(DataBridge.DataBridge.SelectDeviceTableInfo.AssetId);

    }

    private void LoadDeviceInfo(string assetId)
    {
        var sql = $"SELECT * FROM Devices WHERE AssetId = '{assetId}'";

        var rows = GlobalVariables.DbService.ExecuteQuery(sql);

        var info = new DeviceEditViewModel();

        foreach (var row in rows)
        {
            info.AssetNumber = row["AssetNumber"].ToString();
            info.Description = row["Description"].ToString();
            info.EnableDate = row["EnableDate"].ToString();
            info.UseDepartment = row["UseDepartment"].ToString();
            info.Address = row["Address"].ToString();
            info.TagA = row["TagA"].ToString();
            info.TagB = row["TagB"].ToString();
            info.TagC = row["TagC"].ToString();
            info.TagD = row["TagD"].ToString();
            info.TagE = row["TagE"].ToString();
            info.TagF = row["TagF"].ToString();

            if (info.TagA != null)
            {
                TagA.SelectedIndex = parentList.IndexOf(info.TagA);
            }

            if (info.TagB != null)
            {
                TagB.SelectedIndex = childList.IndexOf(info.TagB);
            }


        }

        this.DataContext = info;
    }




    /// <summary>
    /// 加载自定义标签
    /// </summary>
    private void LoadTags()
    {

        var tags = DbClass.LoadWindowTag("AddDevice");

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



    private List<string> parentList = new List<string>();


    /// <summary>
    /// 加载组织信息
    /// </summary>
    private void LoadHierarchyInfo()
    {
        parentList.Clear();

        string query = "SELECT DISTINCT TagA FROM Devices;";


        var rows = GlobalVariables.DbService.ExecuteQuery(query);

        foreach (var row in rows)
        {
            parentList.Add(row["TagA"].ToString());
        }


        TagA.ItemsSource = parentList;


    }


    private void UseDepartment_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }


    private List<string> childList = new List<string>();

    private void TagA_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        childList.Clear();
        TagB.ItemsSource = null;
        if (TagA.SelectedIndex != -1)
        {
            string sql = $"SELECT DISTINCT TagB FROM Devices WHERE TagA='{parentList[TagA.SelectedIndex]}'";

            var rows = GlobalVariables.DbService.ExecuteQuery(sql);

            foreach (var row in rows)
            {
                childList.Add(row["TagB"].ToString());
            }


            TagB.ItemsSource = childList;

        }
    }

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        var id = DataBridge.DataBridge.SelectDeviceTableInfo.AssetId;

        var description = Description.Text;

        if (!string.IsNullOrWhiteSpace(description))
        {
            var enableDate = EnableDate.Text;
            var useDepartment = UseDepartment.Text;
            var address = Address.Text;
            var tagA = TagA.Text;
            var tagB = TagB.Text;
            var tagC = TagC.Text;
            var tagD = TagD.Text;
            var tagE = TagE.Text;
            var tagF = TagF.Text;


            var sql = $"UPDATE  Devices  SET  Description  = '{description}', EnableDate='{enableDate}',UseDepartment='{useDepartment}',Address='{address}',TagA='{tagA}',TagB='{tagB}',TagC='{tagC}',TagD='{tagD}',TagE='{tagE}',TagF='{tagF}' WHERE AssetId = '{id}'";


            GlobalVariables.DbService.ExecuteNonQuery(sql);

            this.DialogResult = true;
        }
        else
        {
            MessageBox.Show("请输入设备描述", "缺少必要信息", MessageBoxButton.OK);
        }




    }
}
