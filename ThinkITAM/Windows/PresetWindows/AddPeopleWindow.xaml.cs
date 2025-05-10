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
using ThinkITAM.FunctionClass;
using ThinkITAM.UserControls.General;
using ThinkITAM.ViewModels.Preset;
using MaterialDesignThemes.Wpf;
using ThinkITAM.DataBridge;
using System.Collections;
using ThinkITAM.Functions.FunctionClass;


namespace ThinkITAM.Windows.PresetWindows;
/// <summary>
/// AddPeopleWindow.xaml 的交互逻辑
/// </summary>
public partial class AddPeopleWindow : Window
{
    public AddPeopleWindow()
    {
        InitializeComponent();
    }

    private DbClass dbClass;

    private void AddPeopleWindow_OnLoaded(object sender, RoutedEventArgs e)
    {

        LoadOrganizationInfo();
        LoadUserNumberPrefix();//加载用户编号前缀

        Department.ItemsSource = departmentInfo;



        UserNumber.Dispatcher.Invoke(() =>
        {
            UserNumber.Text = GetNextAvailableNumber().ToString();
        });

    }

    /// <summary>
    /// 加载用户编号前缀
    /// </summary>
    private void LoadUserNumberPrefix()
    {
        string query = "SELECT Content FROM CustomSetting WHERE Option='UserNumberPrefix';";

        var prefix = GlobalVariables.DbService.ExecuteScalar(query).ToString();

        if (!string.IsNullOrWhiteSpace(prefix))
        {
            UserNumberPrefix.Dispatcher.Invoke(() =>
                {
                    UserNumberPrefix.Text = prefix;
                });
        }
        else
        {
            SavePrefixDialogHost.IsOpen = true;
        }







    }


    private ObservableCollection<string> organizationInfo = new ObservableCollection<string>();



    /// <summary>
    /// 获取下一个可用的编号
    /// </summary>
    /// <returns></returns>
    public int GetNextAvailableNumber()
    {
        var usedNumbers = new HashSet<int>();

        string sql = "SELECT Number FROM UserInfo "; // 假设Del为0表示未删除的记录   WHERE Del != 1 OR Del IS NULL


        var rows = GlobalVariables.DbService.ExecuteQuery(sql);

        foreach (var row in rows)
        {
               usedNumbers.Add(Convert.ToInt32(row["Number"]));
        }





        int nextNumber = 1; // Start with the smallest possible number
        while (usedNumbers.Contains(nextNumber))
        {
            nextNumber++;
        }

        return nextNumber;
    }


    /// <summary>
    /// 加载组织信息
    /// </summary>
    private void LoadOrganizationInfo()
    {
        organizationInfo.Clear();

        string query = "SELECT DISTINCT Organization FROM Organization;";


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

            string query = $"SELECT DISTINCT Department FROM Organization WHERE Organization='{organizationInfo[Organization.SelectedIndex].ToString()}' AND (Department IS NOT NULL OR Department != '') AND (Groups IS NULL OR Groups = '') AND (Note != '0' OR Note IS NULL);";

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




    private async void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        var info = CheckInput();

        if (info.Item1 == 0)
        {

            SaveUserInfo(UserNumber.Text, User.Text, Organization.Text, Department.Text, Groups.Text, Phone.Text, Note.Text);
        }
        else
        {
            //MessageBox.Show(info.Item2, "注意！", MessageBoxButton.OK, MessageBoxImage.Error);

            var dialog = new ConfirmationDialog
            {
                Title = "注意",
                Prompt = $"{info.Item2}",
                ConfirmButtonText = "确认",
                TitleColor = Brushes.AliceBlue,
                PromptColor = Brushes.AliceBlue

            };

            // 显示对话框
            await DialogHost.Show(dialog, "MessageDialogHost");



        }

    }

    private (int, string) CheckInput()
    {
        int index = 0;
        string message = "当前存在以下问题需要解决:\r";

        if (string.IsNullOrWhiteSpace(User.Text))
        {
            index++;
            message += index.ToString() + ":用户姓名不得为空\r";
        }


        if (string.IsNullOrWhiteSpace(Organization.Text))
        {
            index++;
            message += index.ToString() + ":未选取用户所在组织\r";
        }

        if (string.IsNullOrWhiteSpace(Department.Text))
        {
            index++;
            message += index.ToString() + ":未选取用户所在组织子级\r";
        }



        //检查用户编号是否已存在

        string sqlTemp = $"SELECT COUNT(*) FROM UserInfo WHERE Number ='{UserNumber.Text}'";

        //查询记录是否存在
        var countNum = DbClass.ExecuteScalarTableNum(sqlTemp);


        if (countNum > 0)
        {
            index++;
            message += index.ToString() + $":用户编号{UserNumber.Text}已使用\r(双击用户编号框可获取最小可用编号)\r";
        }


        return (index, message);
    }


    private void SaveOrganizationInfo(string organization, string department)
    {

        var organizationInfo = organization.Replace(" ", "");
        var departmentInfo = department.Replace(" ", "");

        string sqlTemp = $"SELECT COUNT(*) FROM Organization WHERE Organization ='{organizationInfo}' AND Department = '{departmentInfo}'";



        var num = DbClass.ExecuteScalarTableNum(sqlTemp);

        if (num <= 0)
        {
            string sql = $"INSERT INTO  \"Organization\" (\"Organization\", \"Department\") VALUES ('{organizationInfo}', '{departmentInfo}')";

   
            GlobalVariables.DbService.ExecuteNonQuery(sql);

        }


    }

    private void SaveUserInfo(string _userNumber, string _userName, string _organization, string _department, string _groups, string _phone, string _note)
    {

        string userId =$"9{AssetCodeClass.GenerateChecksum(AssetIdCreate.CreateAssetId(DateTime.Now.ToString("yyyyMMddHHmmss"))).ToUpper()}";

        var name = _userName.Replace(" ", "");
        string number = _userNumber.Replace(" ", "");
        var organization = _organization.Replace(" ", "");
        var department = _department.Replace(" ", "");
        var group = _groups.Replace(" ", "");
        var phone = _phone;
        var note = _note;



        string sql = $"INSERT INTO \"UserInfo\" (\"UserId\",\"Name\",\"Number\", \"Organization\", \"Department\",\"UserGroup\", \"Phone\", \"Note\") VALUES ('{userId}','{name}','{number}', '{organization}', '{department}', '{group}', '{phone}', '{note}')";

    

        GlobalVariables.DbService.ExecuteNonQuery(sql);
        this.DialogResult = true;


    }

    private ObservableCollection<string> groupsInfo = new ObservableCollection<string>();

    private void Department_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Department.SelectedIndex != -1)
        {
            groupsInfo.Clear();

            string query = $"SELECT DISTINCT Groups FROM Organization WHERE Organization='{organizationInfo[Organization.SelectedIndex].ToString()}' AND Department = '{departmentInfo[Department.SelectedIndex]}' AND (Groups IS NOT NULL OR Groups != '') AND (Note != '0' OR Note IS NULL);";

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

    /// <summary>
    /// 保存用户编号前缀
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SavePrefixButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (prefix != NameTextBox.Text)
        {
            string sqlTemp = $"SELECT COUNT(*) FROM CustomSetting WHERE Option ='UserNumberPrefix'";

            //查询记录是否存在
            var countNum = DbClass.ExecuteScalarTableNum(sqlTemp);

            if (countNum == 0)//判断记录是否存在，不存在的情况
            {

                string sql = $"INSERT INTO \"CustomSetting\" (\"Option\", \"Content\") VALUES ('UserNumberPrefix', '{NameTextBox.Text}')";

              
                GlobalVariables.DbService.ExecuteNonQuery(sql);


            }
            else//存在
            {
                string sql = $"UPDATE \"CustomSetting\" SET \"Content\" = '{NameTextBox.Text}' WHERE Option ='UserNumberPrefix'";
                
                GlobalVariables.DbService.ExecuteNonQuery(sql);
            }
            SavePrefixDialogHost.IsOpen = false;
        }
        else
        {

            SavePrefixDialogHost.IsOpen = false;
        }
    }

    /// <summary>
    /// 关闭对话框
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        // 直接调用 DialogHost 的 IsOpen 属性来打开对话框

        SavePrefixDialogHost.IsOpen = false;




    }

    /// <summary>
    /// 关闭对话框后重新加载用户编号前缀
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventargs"></param>
    private void SavePrefixDialogHost_OnDialogClosed(object sender, DialogClosedEventArgs eventargs)
    {
        LoadUserNumberPrefix();
    }


    private string prefix;

    /// <summary>
    /// 编辑用户编号前缀
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EditPrefixButton_OnClick(object sender, RoutedEventArgs e)
    {
        // 直接调用 DialogHost 的 IsOpen 属性来打开对话框

        SavePrefixDialogHost.IsOpen = true;
        NameTextBox.Text = UserNumberPrefix.Text;
        prefix = UserNumberPrefix.Text;

    }

    private void UserNumber_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        UserNumber.Dispatcher.Invoke(() =>
        {
            UserNumber.Text = GetNextAvailableNumber().ToString();
        });
    }
}
