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
using System.Windows.Shapes;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using static MaterialDesignThemes.Wpf.Theme;
using TextBox = System.Windows.Controls.TextBox;

namespace ThinkITAM.Windows.PresetWindows;
/// <summary>
/// AddOrganizationWindow.xaml 的交互逻辑
/// </summary>
public partial class AddOrganization3Window : Window
{
    public AddOrganization3Window()
    {
        InitializeComponent();
    }


    private void AddOrganization3Window_OnLoaded(object sender, RoutedEventArgs e)
    {

        LoadOrganizationInfo();
    }


    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (Organization.Text.Length > 0 && Department.Text.Length > 0 && Groups.Text.Length > 0)
        {
            SaveOrganizationInfo(Organization.Text, Department.Text ,Groups.Text);

        }
        else
        {
            MessageBox.Show("信息不完整！", "请注意", MessageBoxButton.OK, MessageBoxImage.Warning);
        }


    }



    private void SaveOrganizationInfo(string organization,string department,string groups )
    {
        var organizationInfo = organization.Replace(" ", "");
        var departmentInfo = department.Replace(" ", "");
        var groupsInfo = groups.Replace(" ", "");

        string sqlTemp = $"SELECT COUNT(*) FROM Organization WHERE Organization ='{organizationInfo}' AND Department = '{departmentInfo}' AND Groups = '{groupsInfo}'";


        var num = DbClass.ExecuteScalarTableNum(sqlTemp);

        if (num <= 0)
        {

            var org = new { Organization = organizationInfo, Department = departmentInfo, Groups = groupsInfo };

            //string sql = $"INSERT INTO  \"Organization\" (\"Organization\", \"Department\", \"Groups\") VALUES ('{organizationInfo}', '{departmentInfo}', '{groupsInfo}')";


            GlobalVariables.DbService.InsertEntity("Organization", org);
            this.DialogResult = true;
            this.Close();
        }
        else
        {

            string sql = $"SELECT COUNT( * )  FROM Organization  WHERE Organization = '{organizationInfo}' AND  Department = '{departmentInfo}' AND   Groups = '{groupsInfo}' AND Del = 1 ;";

            var num2 = DbClass.ExecuteScalarTableNum(sql);

            if (num2 == 1)
            {
                var result = MessageBox.Show($"当前添加的 {groupsInfo} ，在数据库中已被标记为删除，是否进行恢复？", "请注意", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    string sql2 =
                        $"UPDATE Organization SET Del = NULL WHERE Organization = '{organization}' AND  Department = '{department}' AND   Groups = '{groupsInfo}' ";
 
                    GlobalVariables.DbService.ExecuteNonQuery(sql2);
                    this.DialogResult = true;
                    this.Close();
                }

            }
            else
            {
                MessageBox.Show($"当前添加的 {groupsInfo} ，在数据库中已存在，请重新添加！", "请注意", MessageBoxButton.OK, MessageBoxImage.Warning);
            }



        }

    }


    private ObservableCollection<string> organizationInfo= new ObservableCollection<string>();

    /// <summary>
    /// 加载组织信息
    /// </summary>
    private void LoadOrganizationInfo()
    {
        organizationInfo.Clear();

        string query = "SELECT DISTINCT Organization FROM Organization WHERE ( Department IS  NULL OR Department = '') AND ( GROUPS IS NULL OR GROUPS = '' ) AND (Del != 1 OR Del IS NULL);";

        var rows = GlobalVariables.DbService.ExecuteQuery(query);

        foreach (var row in rows)
        {
             organizationInfo.Add(row["Organization"].ToString());
        }


        Organization.ItemsSource = organizationInfo;


    }

    private ObservableCollection<string> departmentInfo = new ObservableCollection<string>();

    private void Organization_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {

        if (Organization.SelectedIndex != -1)
        {
            departmentInfo.Clear();
            
            string query = $"SELECT DISTINCT Department FROM Organization WHERE Organization='{organizationInfo[Organization.SelectedIndex].ToString()}' AND (Department IS NOT NULL OR Department != '') AND (Groups IS NULL OR Groups = '') AND (Del != 1 OR Del IS NULL);";

            Console.WriteLine(query);

            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            foreach (var row in rows)
            {
                 departmentInfo.Add(row["Department"].ToString());
            }


            Department.ItemsSource = departmentInfo;
        }
        else
        {
            departmentInfo.Clear();
        }



    }



    private ObservableCollection<string> groupsInfo = new ObservableCollection<string>();


    private void Department_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {

        if (Department.SelectedIndex != -1)
        {
            groupsInfo.Clear();

            string query = $"SELECT DISTINCT Groups FROM Organization WHERE Organization='{organizationInfo[Organization.SelectedIndex].ToString()}' AND Department = '{departmentInfo[Department.SelectedIndex]}' AND (Groups IS NOT NULL OR Groups != '') AND (Del != 1 OR Del IS NULL);";

            Console.WriteLine(query);
            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            foreach (var row in rows)
            {
                 groupsInfo.Add(row["Groups"].ToString());
            }



            Groups.ItemsSource = groupsInfo;
        }
        else
        {
            groupsInfo.Clear();
        }
    }
}
