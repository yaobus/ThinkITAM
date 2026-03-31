using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ThinkITAM.DataBridge;
using ThinkITAM.ViewModels.Preset;
using ThinkITAM.Windows.PresetWindows;

namespace ThinkITAM.Windows.NetworkManage
{
    /// <summary>
    /// FindUserWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FindUserWindow : Window
    {
        public FindUserWindow()
        {
            InitializeComponent();
        }


        private ObservableCollection<PeopleViewModel> peopleInfos = new ObservableCollection<PeopleViewModel>();

        private void FindUserWindow_OnLoaded(object sender, RoutedEventArgs e)
        {


            PeopleListView.ItemsSource = peopleInfos;

            LoadPeopleInfos();
            LoadOrganizationInfo();
        }

        /// <summary>
        /// 加载人员信息
        /// </summary>
        private void LoadPeopleInfos(string organization = null, string department = null, string group = null, string unit = null, string name = null)
        {
            peopleInfos.Clear();


            string filter = "WHERE ";

            if (organization != null)
            {
                filter += $" Organization='{organization}' ";

                if (department != null)
                {
                    filter += $" AND Department='{department}' ";

                    if (group != null)
                    {
                        filter += $" AND UserGroup ='{group}' ";


                        if (unit != null)
                        {
                            filter += $" AND UserUnit ='{unit}' ";


                            if (name != null)
                            {
                                filter += $" AND Name LIKE '%{name}%' ";
                            }

                        }

                    }

                }
            }
            else
            {
                if (name != null)
                {
                    filter += $"  Name LIKE '%{name}%' ";
                }
            }

            string query;

            if (filter != "WHERE ")
            {
                query = $"SELECT * FROM UserInfo {filter} AND (Del != 1 OR Del IS NULL);";
            }
            else
            {
                query = "SELECT * FROM UserInfo WHERE (Del != 1 OR Del IS NULL) ;";
            }


            var rows = GlobalVariables.DbService.ExecuteQuery(query);


            int index = 0;

            foreach (var row in rows)
            {
                index++;
                PeopleViewModel info = new PeopleViewModel();
                info.Index = index;
                info.UserId = row["UserId"].ToString();
                info.UserNumber = $"{GetUserNumberPrefix()}{row["Number"].ToString()}";
                info.Name = row["Name"].ToString();
                info.Organization = row["Organization"].ToString();
                info.Department = row["Department"].ToString();
                info.Group = row["UserGroup"].ToString();
                info.Unit = row["UserUnit"].ToString();
                info.Phone = row["Phone"].ToString();
                info.Note = row["Note"].ToString();

                peopleInfos.Add(info);
            }



            PeopleListView.ItemsSource = peopleInfos;


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



        private ObservableCollection<string> organizationInfo = new ObservableCollection<string>();

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



            OrganizationBox.ItemsSource = organizationInfo;


        }

        private ObservableCollection<string> departmentInfo = new ObservableCollection<string>();


        private void OrganizationBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrganizationBox.SelectedIndex != -1)
            {
                departmentInfo.Clear();

                string query = $"SELECT DISTINCT Department FROM Organization WHERE Organization='{organizationInfo[OrganizationBox.SelectedIndex].ToString()}' AND (Department IS NOT NULL OR Department != '') AND (UserGroups IS NULL OR UserGroups = '') AND (Del != '0' OR Del IS NULL);";

                Console.WriteLine(query);


                var rows = GlobalVariables.DbService.ExecuteQuery(query);


                foreach (var row in rows)
                {
                    departmentInfo.Add(row["Department"].ToString());

                }


                DepartmentBox.ItemsSource = departmentInfo;
            }
            else
            {
                departmentInfo.Clear();
            }


            string org = OrganizationBox.SelectedIndex != -1 ? organizationInfo[OrganizationBox.SelectedIndex].ToString() : null;



            LoadPeopleInfos(org);

        }

        private ObservableCollection<string> groupsInfo = new ObservableCollection<string>();


        private void DepartmentBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DepartmentBox.SelectedIndex != -1)
            {
                groupsInfo.Clear();

                string query = $"SELECT DISTINCT UserGroups FROM Organization WHERE Organization='{organizationInfo[OrganizationBox.SelectedIndex].ToString()}' AND Department = '{departmentInfo[DepartmentBox.SelectedIndex]}' AND (UserGroups IS NOT NULL OR UserGroups != '') AND (Del != '0' OR Del IS NULL);";

                Console.WriteLine(query);


                var rows = GlobalVariables.DbService.ExecuteQuery(query);

                foreach (var row in rows)
                {
                    groupsInfo.Add(row["UserGroups"].ToString());
                }



                GroupBox.ItemsSource = groupsInfo;
            }
            else
            {
                groupsInfo.Clear();
            }



            string org = OrganizationBox.SelectedIndex != -1 ? organizationInfo[OrganizationBox.SelectedIndex].ToString() : null;
            string dep = DepartmentBox.SelectedIndex != -1 ? departmentInfo[DepartmentBox.SelectedIndex].ToString() : null;


            LoadPeopleInfos(org, dep);

        }

        private ObservableCollection<string> unitInfos = new ObservableCollection<string>();


        private void GroupBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string org = OrganizationBox.SelectedIndex != -1 ? organizationInfo[OrganizationBox.SelectedIndex].ToString() : null;
            string dep = DepartmentBox.SelectedIndex != -1 ? departmentInfo[DepartmentBox.SelectedIndex].ToString() : null;
            string group = GroupBox.SelectedIndex != -1 ? groupsInfo[GroupBox.SelectedIndex].ToString() : null;



            if (GroupBox.SelectedIndex != -1)
            {
                unitInfos.Clear();

                string query = $"SELECT DISTINCT UserUnit FROM Organization WHERE Organization='{organizationInfo[OrganizationBox.SelectedIndex]}' AND Department = '{departmentInfo[DepartmentBox.SelectedIndex]}' AND UserGroups = '{groupsInfo[GroupBox.SelectedIndex]}' AND (UserUnit IS NOT NULL OR UserUnit != '') AND (Del != '0' OR Del IS NULL);";


                var rows = GlobalVariables.DbService.ExecuteQuery(query);

                foreach (var row in rows)
                {
                    unitInfos.Add(row["UserUnit"].ToString());
                }



                UnitBox.ItemsSource = unitInfos;
            }
            else
            {
                unitInfos.Clear();
            }



            LoadPeopleInfos(org, dep, group);
        }

        private void SearchButton_OnClick(object sender, RoutedEventArgs e)
        {
            peopleInfos.Clear();

            var org = OrganizationBox.SelectedIndex != -1 ? organizationInfo[OrganizationBox.SelectedIndex].ToString() : null;
            var dep = DepartmentBox.SelectedIndex != -1 ? departmentInfo[DepartmentBox.SelectedIndex].ToString() : null;
            var group = GroupBox.SelectedIndex != -1 ? groupsInfo[GroupBox.SelectedIndex].ToString() : null;
            var unit = UnitBox.SelectedIndex != -1 ? unitInfos[UnitBox.SelectedIndex].ToString() : null;


            LoadPeopleInfos(org, dep, group, unit, UserName.Text);
        }

        private void PeopleListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PeopleListView.SelectedIndex != -1)
            {
                var info = peopleInfos[PeopleListView.SelectedIndex];
                Number.Text = info.UserNumber;
                UserName.Text = info.Name;
                DataBridge.DataBridge.SelectPeopleViewModel = info;
            }


                         }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (PeopleListView.SelectedIndex != -1)
            {
                var info = peopleInfos[PeopleListView.SelectedIndex];

                DataBridge.DataBridge.SelectPeopleViewModel = info;
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("您还没有选择任何人员", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }


        }

        private void UnitBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            peopleInfos.Clear();

            var org = OrganizationBox.SelectedIndex != -1 ? organizationInfo[OrganizationBox.SelectedIndex].ToString() : null;
            var dep = DepartmentBox.SelectedIndex != -1 ? departmentInfo[DepartmentBox.SelectedIndex].ToString() : null;
            var group = GroupBox.SelectedIndex != -1 ? groupsInfo[GroupBox.SelectedIndex].ToString() : null;
            var unit = UnitBox.SelectedIndex != -1 ? unitInfos[UnitBox.SelectedIndex].ToString() : null;


            LoadPeopleInfos(org, dep, group, unit, UserName.Text);

        }

        private void AddPeopleButton_OnClick(object sender, RoutedEventArgs e)
        {


            var addPeople = new AddPeopleWindow();


            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                addPeople.Owner = window;
            }

            if (addPeople.ShowDialog() == true)
            {

                LoadPeopleInfos();

            }

        }
    }
}
