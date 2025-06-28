using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;

namespace ThinkITAM.Windows.PresetWindows;
/// <summary>
/// AddOrganizationWindow.xaml 的交互逻辑
/// </summary>
public partial class AddOrganization2Window : Window
{
    public AddOrganization2Window(int index = -1)
    {
        InitializeComponent();
        if (index != -1)
        {
            organizationIndex = index;
        }
    }

    private int organizationIndex = -1;
    private void AddOrganization2Window_OnLoaded(object sender, RoutedEventArgs e)
    {

        LoadOrganizationInfo();

        if (organizationIndex != -1)
        {
            Organization.SelectedIndex = organizationIndex;
        }
    }


    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (Organization.Text.Length > 0 && Department.Text.Length > 0)
        {
            SaveOrganizationInfo(Organization.Text, Department.Text);

        }
        else
        {
            MessageBox.Show("信息不完整！", "请注意", MessageBoxButton.OK, MessageBoxImage.Warning);
        }


    }



    private void SaveOrganizationInfo(string organization, string department)
    {
        var organizationInfo = organization.Replace(" ", "");
        var departmentInfo = department.Replace(" ", "");


        string sqlTemp = $"SELECT COUNT(*) FROM Organization WHERE Organization ='{organizationInfo}' AND Department = '{departmentInfo}' AND (Groups IS NULL OR Groups = '');";




        var num = DbClass.ExecuteScalarTableNum(sqlTemp);

        if (num <= 0)
        {
            var org = new { Organization = organizationInfo, Department = departmentInfo };


            //string sql = $"INSERT INTO  \"Organization\" (\"Organization\", \"Department\") VALUES ('{organizationInfo}', '{departmentInfo}')";


            GlobalVariables.DbService.InsertEntity("Organization", org);
            this.DialogResult = true;
            this.Close();
        }
        else
        {

            string sql = $"SELECT COUNT( * )  FROM Organization  WHERE Organization = '{organizationInfo}' AND  Department = '{departmentInfo}' AND (Groups IS NULL OR Groups = '') AND Del = 1 ;";


            var num2 = DbClass.ExecuteScalarTableNum(sql);

            if (num2 == 1)
            {
                var result = MessageBox.Show($"当前添加的 {departmentInfo} ，在数据库中已被标记为删除，是否进行恢复？", "请注意", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {



                    string sql2 =
                        $"UPDATE  Organization  SET  Del  = NULL WHERE Organization = '{organizationInfo}' AND  Department = '{departmentInfo}' AND (Groups IS NULL OR Groups = '') ";

                    GlobalVariables.DbService.ExecuteNonQuery(sql2);
                    this.DialogResult = true;
                    this.Close();
                }

            }
            else
            {
                MessageBox.Show($"当前添加的 {departmentInfo} ，在数据库中已存在，请重新添加！", "请注意", MessageBoxButton.OK, MessageBoxImage.Warning);
            }



        }

    }


    private ObservableCollection<string> organizationInfo = new ObservableCollection<string>();

    /// <summary>
    /// 加载组织信息
    /// </summary>
    private void LoadOrganizationInfo()
    {
        organizationInfo.Clear();

        string query = $"SELECT DISTINCT Organization FROM Organization WHERE ( Department IS  NULL OR Department = '') AND ( GROUPS IS NULL OR GROUPS = '' ) AND (Del != 1 OR Del IS NULL);";


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



}
