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
using static MaterialDesignThemes.Wpf.Theme;
using TextBox = System.Windows.Controls.TextBox;

namespace ThinkITAM.Windows.PresetWindows;
/// <summary>
/// AddOrganizationWindow.xaml 的交互逻辑
/// </summary>
public partial class AddOrganization2Window : Window
{
    public AddOrganization2Window()
    {
        InitializeComponent();
    }

    private DbClass dbClass;

    private void AddOrganization2Window_OnLoaded(object sender, RoutedEventArgs e)
    {
        string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\Address_database.db";

        dbClass = new DbClass(dbFilePath);
        dbClass.OpenConnection();
        LoadOrganizationInfo();
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



    private void SaveOrganizationInfo(string organization,string department)
    {
        var organizationInfo = organization.Replace(" ", "");
        var departmentInfo = department.Replace(" ", "");


        string sqlTemp = $"SELECT COUNT(*) FROM Organization WHERE Organization ='{organizationInfo}' AND Department = '{departmentInfo}' AND (Groups IS NULL OR Groups = '');";


        

        var num = dbClass.ExecuteScalarTableNum(sqlTemp, dbClass.connection);

        if (num <= 0)
        {
            string sql = $"INSERT INTO  \"Organization\" (\"Organization\", \"Department\") VALUES ('{organizationInfo}', '{departmentInfo}')";

            dbClass.ExecuteQuery(sql);
            this.DialogResult = true;
            this.Close();
        }
        else
        {

            string sql = $"SELECT COUNT( * )  FROM Organization  WHERE Organization = '{organizationInfo}' AND  Department = '{departmentInfo}' AND (Groups IS NULL OR Groups = '') AND Note = '0';";

            Console.WriteLine(sql);

            var num2 = dbClass.ExecuteScalarTableNum(sql, dbClass.connection);

            if (num2 == 1)
            {
                var result = MessageBox.Show($"当前添加的 {departmentInfo} ，在数据库中已被标记为删除，是否进行恢复？", "请注意", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    string sql2 =
                        $"UPDATE \"Organization\" SET \"Note\" = '' WHERE Organization = '{organizationInfo}' AND  Department = '{departmentInfo}' AND (Groups IS NULL OR Groups = '') ";
                    dbClass.ExecuteQuery(sql2);
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


    private ObservableCollection<string> organizationInfo= new ObservableCollection<string>();

    /// <summary>
    /// 加载组织信息
    /// </summary>
    private void LoadOrganizationInfo()
    {
        organizationInfo.Clear();

        string query = $"SELECT DISTINCT Organization FROM Organization WHERE ( Department IS  NULL OR Department = '') AND ( GROUPS IS NULL OR GROUPS = '' ) AND (Note != '0' OR Note IS NULL);";


        SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
        SQLiteDataReader reader = command.ExecuteReader();


        while (reader.Read())
        {
            organizationInfo.Add(reader["Organization"].ToString());
        }

        Organization.ItemsSource = organizationInfo;


    }

    private ObservableCollection<string> departmentInfo = new ObservableCollection<string>();

    private void Organization_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {



        if (Organization.SelectedIndex != -1)
        {
            departmentInfo.Clear();

            string query = $"SELECT DISTINCT Department FROM Organization WHERE Organization='{organizationInfo[Organization.SelectedIndex].ToString()}' AND (Department IS NOT NULL OR Department != '') AND (Groups IS NULL OR Groups = '') AND (Note != '0' OR Note IS NULL);";

            Console.WriteLine(query);

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();


            while (reader.Read())
            {
                departmentInfo.Add(reader["Department"].ToString());
            }

            Department.ItemsSource = departmentInfo;
        }
        else
        {
            departmentInfo.Clear();
        }



    }



}
