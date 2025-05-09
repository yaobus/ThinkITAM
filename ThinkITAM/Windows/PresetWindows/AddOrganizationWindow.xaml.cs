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
public partial class AddOrganizationWindow : Window
{
    public AddOrganizationWindow()
    {
        InitializeComponent();
    }

    private DbClass dbClass;

    private void AddOrganizationWindow_OnLoaded(object sender, RoutedEventArgs e)
    {

        LoadOrganizationInfo();
    }


    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (Organization.Text.Length > 0)
        {
            SaveOrganizationInfo(Organization.Text);

        }
        else
        {
            MessageBox.Show("信息不完整！", "请注意", MessageBoxButton.OK, MessageBoxImage.Warning);
        }


    }



    private void SaveOrganizationInfo(string organization)
    {
        var organizationInfo = organization.Replace(" ", "");
       
        
        string sqlTemp = $"SELECT COUNT( * )  FROM Organization  WHERE Organization = '{organization}' AND (Department IS NULL OR Department = '') AND (Groups IS NULL OR Groups = '');";

        
        var num = DbClass.ExecuteScalarTableNum(sqlTemp);

        if (num <= 0)
        {
            string sql = $"INSERT INTO  \"Organization\" (\"Organization\") VALUES ('{organizationInfo}')";

     
            GlobalVariables.DbService.ExecuteNonQuery(sql);
            this.DialogResult = true;
            this.Close();
        }
        else
        {
            //MessageBox.Show("信息已存在，请勿重复添加！", "请注意", MessageBoxButton.OK, MessageBoxImage.Warning);

            string sql = $"SELECT COUNT( * )  FROM Organization  WHERE Organization = '{organization}' AND (Department IS NULL OR Department = '') AND (Groups IS NULL OR Groups = '') AND Note = '0';";

            var num2 = DbClass.ExecuteScalarTableNum(sql);

            if (num2 == 1)
            {
                var result = MessageBox.Show($"当前添加的{organization}，在数据库中已被标记为删除，是否进行恢复？", "请注意", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    string sql2 =
                        $"UPDATE \"Organization\" SET \"Note\" = '' WHERE Organization = '{organization}' AND (Department IS NULL OR Department = '') AND (Groups IS NULL OR Groups = '') AND Note = '0'";
       
                    GlobalVariables.DbService.ExecuteNonQuery(sql2);
                    this.DialogResult = true;
                    this.Close();
                }

            }
            else
            {
                MessageBox.Show($"当前添加的 {organizationInfo} ，在数据库中已存在，请重新添加！", "请注意", MessageBoxButton.OK, MessageBoxImage.Warning);
            }



        }

    }


    private ObservableCollection<string> organizationInfo= new ObservableCollection<string>();

    private string sqlsub = "WHERE (Note != '0' OR Note IS NULL)";

    /// <summary>
    /// 加载组织信息
    /// </summary>
    private void LoadOrganizationInfo()
    {
        organizationInfo.Clear();

        string query = $"SELECT DISTINCT Organization FROM Organization WHERE ( Department IS  NULL OR Department = '') AND ( GROUPS IS NULL OR GROUPS = '' ) AND (Note != '0' OR Note IS NULL);";

        // Console.WriteLine(query);

        var rows = GlobalVariables.DbService.ExecuteQuery(query);

        foreach (var row in rows)
        {
             organizationInfo.Add(row["Organization"].ToString());
        }



        Organization.ItemsSource = organizationInfo;


    }



    private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(obj, i);
            if (child != null && child is T)
                return (T)child;
            else
            {
                T childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                    return childOfChild;
            }
        }
        return null;
    }
}
