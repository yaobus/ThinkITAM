using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.UserControls.General;
using ThinkITAM.ViewModels.Preset;


namespace ThinkITAM.Windows.PresetWindows;
/// <summary>
/// AddPeopleWindow.xaml 的交互逻辑
/// </summary>
public partial class AddPeopleWindow : Window
{
    public AddPeopleWindow(PeopleViewModel? info = null)
    {
        InitializeComponent();

        if (info != null)
        {
            peopleInfo = info;
            this.DataContext = peopleInfo;
        }


    }

    private readonly PeopleViewModel? peopleInfo;
    private void AddPeopleWindow_OnLoaded(object sender, RoutedEventArgs e)
    {

        LoadOrganizationInfo();

        LoadUserNumberPrefix();//加载用户编号前缀

        Department.ItemsSource = departmentInfo;



        if (peopleInfo != null)
        {
            UserNumber.IsEnabled = false;
            UserNumber.Text = peopleInfo.Number;
        }
        else
        {
            UserNumber.Dispatcher.Invoke(() =>
            {
                UserNumber.Text = GetNextAvailableNumber().ToString();
            });
        }

    }

    /// <summary>
    /// 加载用户编号前缀
    /// </summary>
    private void LoadUserNumberPrefix()
    {
        string query = "SELECT Content FROM CustomSetting WHERE CustomOption='UserNumberPrefix';";

        var prefix = GlobalVariables.DbService.ExecuteScalar(query);

        if (prefix != null)
        {
            UserNumberPrefix.Dispatcher.Invoke(() =>
                {
                    UserNumberPrefix.Text = prefix.ToString();
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

        string query = "SELECT DISTINCT Organization FROM Organization WHERE  Department IS NULL AND (Del != 1 OR Del IS NULL);";
       

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

            string query = $"SELECT DISTINCT Department FROM Organization WHERE Organization='{organizationInfo[Organization.SelectedIndex].ToString()}' AND (Department IS NOT NULL OR Department != '') AND UserGroups IS NULL AND (Del != 1 OR Del IS NULL);";



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
            SaveOrganizationInfo(Organization.Text, Department.Text, Groups.Text, Units.Text);

            if (peopleInfo != null)
            {
                UpdateUserInfo(UserNumber.Text, User.Text, Organization.Text, Department.Text, Groups.Text, Units.Text, Phone.Text, Note.Text);
            }
            else
            {
                SaveUserInfo(UserNumber.Text, User.Text, Organization.Text, Department.Text, Groups.Text, Units.Text, Phone.Text, Note.Text);
            }

        }
        else
        {


            var dialog = new ConfirmationDialog
            {
                Title = "注意",
                Prompt = $"{info.Item2}",
                ConfirmButtonText = "确认",


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

        if (peopleInfo == null)
        {
            //检查用户编号是否已存在

            string sqlTemp = $"SELECT COUNT(*) FROM UserInfo WHERE Number ='{UserNumber.Text}'";

            //查询记录是否存在
            var countNum = DbClass.ExecuteScalarTableNum(sqlTemp);


            if (countNum > 0)
            {
                index++;
                message += index.ToString() + $":用户编号{UserNumber.Text}已使用\r(双击用户编号框可获取最小可用编号)\r";
            }
        }




        return (index, message);
    }


    private void SaveOrganizationInfo(string organization, string department, string groups, string units)
    {

        var organizationInfo = organization.Replace(" ", "");
        var departmentInfo = department.Replace(" ", "");
        var groupsInfo = groups.Replace(" ", "");
        var unitsInfo = units.Replace(" ", "");

        string sqlTemp = $"SELECT COUNT(*) FROM Organization WHERE Organization ='{organizationInfo}' AND Department = '{departmentInfo}' AND UserGroups ='{groupsInfo}' AND UserUnit ='{unitsInfo}'";



        var num = DbClass.ExecuteScalarTableNum(sqlTemp);

        if (num <= 0)
        {
            var info = new { Organization = organizationInfo, Department = departmentInfo, UserGroups = groupsInfo, UserUnit = unitsInfo };


            GlobalVariables.DbService.InsertEntity("Organization", info);

        }


    }

    private void SaveUserInfo(string _userNumber, string _userName, string _organization, string _department, string _groups, string _unit, string _phone, string _note)
    {

        string userId = $"9{AssetCodeClass.GenerateChecksum(AssetIdCreate.CreateAssetId(Guid.NewGuid().ToString())).ToUpper()}";

        var name = _userName.Replace(" ", "");
        string number = _userNumber.Replace(" ", "");
        var organization = _organization.Replace(" ", "");
        var department = _department.Replace(" ", "");
        var group = _groups.Replace(" ", "");
        var unit = _unit.Replace(" ", "");
        var phone = _phone;
        var note = _note;

        var info = new
        {
            UserId = userId,
            Name = name,
            Number = number,
            Organization = organization,
            Department = department,
            UserGroup = group,
            UserUnit = unit,
            Phone = phone,
            Note = note
        };


        GlobalVariables.DbService.InsertEntity("UserInfo", info);
        this.DialogResult = true;


    }

    /// <summary>
    /// 更新用户信息
    /// </summary>
    /// <param name="_userNumber"></param>
    /// <param name="_userName"></param>
    /// <param name="_organization"></param>
    /// <param name="_department"></param>
    /// <param name="_groups"></param>
    /// <param name="_phone"></param>
    /// <param name="_note"></param>
    private void UpdateUserInfo(string _userNumber, string _userName, string _organization, string _department, string _groups, string _unit, string _phone, string _note)
    {

        var name = _userName.Replace(" ", "");
        var number = Convert.ToInt32(_userNumber);
        var organization = _organization.Replace(" ", "");
        var department = _department.Replace(" ", "");
        var group = _groups.Replace(" ", "");
        var unit = _unit.Replace(" ", "");
        var phone = _phone;
        var note = _note;

        var info = new
        {
            UserId = peopleInfo.UserId,
            Name = name,
            Number = number,
            Organization = organization,
            Department = department,
            UserGroup = group,
            UserUnit = unit,
            Phone = phone,
            Note = note
        };

        var conditions = new { UserId = peopleInfo.UserId };

        GlobalVariables.DbService.UpdateEntity("UserInfo", info, conditions);

        this.DialogResult = true;

    }

    /// <summary>
    /// 获取用户编号前缀
    /// </summary>
    private string GetUserNumberPrefix()
    {
        string query = "SELECT Content  FROM CustomSetting WHERE CustomOption='UserNumberPrefix';";


        var prefix = GlobalVariables.DbService.ExecuteScalar(query);

        if (prefix != null)
        {
            return prefix.ToString();
        }
        else
        {
            return "";
        }

    }


    private ObservableCollection<string> groupsInfo = new ObservableCollection<string>();

    private void Department_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Department.SelectedIndex != -1)
        {
            groupsInfo.Clear();

            string query = $"SELECT DISTINCT UserGroups FROM Organization WHERE Organization='{organizationInfo[Organization.SelectedIndex]}' AND Department = '{departmentInfo[Department.SelectedIndex]}' AND (UserGroups IS NOT NULL OR UserGroups != '') AND UserUnit IS NULL AND (Del != 1 OR Del IS NULL);";



            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            foreach (var row in rows)
            {
                groupsInfo.Add(row["UserGroups"].ToString());
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
            string sqlTemp = $"SELECT COUNT(*) FROM CustomSetting WHERE CustomOption ='UserNumberPrefix'";

            //查询记录是否存在
            var countNum = DbClass.ExecuteScalarTableNum(sqlTemp);

            if (countNum == 0)//判断记录是否存在，不存在的情况
            {
                var info = new { CustomOption = "UserNumberPrefix", Content = NameTextBox.Text };


                GlobalVariables.DbService.InsertEntity("CustomSetting", info);


            }
            else//存在
            {
                string sql = $"UPDATE  CustomSetting  SET  Content  = '{NameTextBox.Text}' WHERE CustomOption ='UserNumberPrefix'";

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

    private ObservableCollection<string> unitInfos = new ObservableCollection<string>();


    private void Groups_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Groups.SelectedIndex != -1)
        {
            unitInfos.Clear();

            string query = $"SELECT DISTINCT UserUnit FROM Organization WHERE Organization='{organizationInfo[Organization.SelectedIndex]}' AND Department = '{departmentInfo[Department.SelectedIndex]}'  AND UserGroups = '{groupsInfo[Groups.SelectedIndex]}' AND (UserUnit IS NOT NULL OR UserUnit != '') AND (Del != 1 OR Del IS NULL);";



            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            foreach (var row in rows)
            {
                unitInfos.Add(row["UserUnit"].ToString());
            }



            Units.ItemsSource = unitInfos;
        }
        else
        {
            unitInfos.Clear();
        }
    }
}
