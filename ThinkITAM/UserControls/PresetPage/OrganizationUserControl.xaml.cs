using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ThinkITAM.DataBridge;
using ThinkITAM.ViewModels.Preset;
using ThinkITAM.Windows.PresetWindows;

namespace ThinkITAM.UserControls.PresetPage
{
    /// <summary>
    /// OrganizationUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class OrganizationUserControl : UserControl
    {
        public OrganizationUserControl()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 新增按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {

            AddOrganizationWindow addOrganizationWindow = new AddOrganizationWindow();


            if (addOrganizationWindow.ShowDialog() == true)
            {

                LoadOrganization();

            }
        }


        private ObservableCollection<OrganizationOneViewModel> organizationInfos = new ObservableCollection<OrganizationOneViewModel>();

        private string sqlsub = "AND (Del != 1 OR Del IS NULL)";


        private void LoadOrganization()
        {
            organizationInfos.Clear();

            string query = $"SELECT DISTINCT Organization FROM Organization WHERE  ( Department IS  NULL OR Department = '') AND ( UserGroups IS NULL OR UserGroups = '' ) {sqlsub}";

            Console.WriteLine(query);


            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            int index = 0;

            foreach (var row in rows)
            {
                index++;
                OrganizationOneViewModel info = new OrganizationOneViewModel();

                info.Index = index;
                info.Organization = row["Organization"].ToString();

                organizationInfos.Add(info);
            }



            //OrganizationListView.ItemsSource = organizationInfos;


        }

        /// <summary>
        /// 初始化加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrganizationUserControl_OnLoaded(object sender, RoutedEventArgs e)
        {

            OneListView.ItemsSource = organizationInfos;
            TowListView.ItemsSource = departmentInfos;
            ThreeListView.ItemsSource = groupsInfos;
            FourListView.ItemsSource = unitInfos;
            LoadOrganization();
        }

        //private ObservableCollection<OrganizationOneViewModel> organizationInfos2 = new ObservableCollection<OrganizationOneViewModel>();



        private void OneListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OneListView.SelectedIndex != -1)
            {
                TowListView.SelectedIndex = -1;
                departmentInfos.Clear();

                ThreeListView.SelectedIndex = -1;
                groupsInfos.Clear();


                string name = organizationInfos[OneListView.SelectedIndex].Organization;

                string query = $"SELECT DISTINCT Department FROM Organization WHERE Organization='{name}' AND ( Department IS NOT NULL OR Department != '') AND ( UserGroups IS NULL OR UserGroups = '')  {sqlsub}";



                var rows = GlobalVariables.DbService.ExecuteQuery(query);

                int index = 0;

                foreach (var row in rows)
                {
                    index++;
                    DepartmentViewModel info = new DepartmentViewModel();

                    info.Index = index;
                    info.Department = row["Department"].ToString();

                    departmentInfos.Add(info);
                }





            }
        }

        //private ObservableCollection<OrganizationOneViewModel> organizationInfos3 = new ObservableCollection<OrganizationOneViewModel>();


        private void TowListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OneListView.SelectedIndex != -1)
            {
                ThreeListView.SelectedIndex = -1;
                groupsInfos.Clear();
                unitInfos.Clear();

                string name = organizationInfos[OneListView.SelectedIndex].Organization;


                if (departmentInfos.Count > 0 && TowListView.SelectedIndex != -1)
                {
                    string name2 = departmentInfos[TowListView.SelectedIndex].Department;

                    string query = $"SELECT DISTINCT UserGroups FROM Organization WHERE Organization='{name}' AND Department='{name2}' AND (UserGroups IS NOT NULL OR UserGroups != '')  {sqlsub}";



                    var rows = GlobalVariables.DbService.ExecuteQuery(query);

                    int index = 0;

                    foreach (var row in rows)
                    {
                        index++;
                        GroupViewModel info = new GroupViewModel();

                        info.Index = index;
                        info.Group = row["UserGroups"].ToString();

                        groupsInfos.Add(info);
                    }



                }



            }
        }

        /// <summary>
        /// 添加一级组织
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add1Button_OnClick(object sender, RoutedEventArgs e)
        {

            AddOrganizationWindow add = new AddOrganizationWindow();


            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                add.Owner = window;
            }



            if (add.ShowDialog() == true)
            {

                // 当子窗口关闭后执行这里的代码

                LoadOrganization();
                //加载设备信息
                //LoadTags();
            }




        }

        private void Add2Button_OnClick(object sender, RoutedEventArgs e)
        {
            int index = -1;

            if (OneListView.SelectedIndex != -1)
            {
                index = OneListView.SelectedIndex;
            }


            AddOrganization2Window add = new AddOrganization2Window(index);



            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                add.Owner = window;
            }



            if (add.ShowDialog() == true)
            {

                // 当子窗口关闭后执行这里的代码

                if (OneListView.SelectedIndex != -1)
                {
                    LoadDepartment(organizationInfos[OneListView.SelectedIndex].Organization);

                }




            }

        }


        private ObservableCollection<DepartmentViewModel> departmentInfos = new ObservableCollection<DepartmentViewModel>();

        /// <summary>
        /// 加载部门
        /// </summary>
        private void LoadDepartment(string organization)
        {
            departmentInfos.Clear();

            string query = $"SELECT DISTINCT Department FROM Organization WHERE Organization ='{organization}' AND(Department IS NOT NULL OR Department !='') AND (UserGroups IS NULL OR UserGroups = '')  {sqlsub}";


            var rows = GlobalVariables.DbService.ExecuteQuery(query);
            int index = 0;

            foreach (var row in rows)
            {
                index++;
                DepartmentViewModel info = new DepartmentViewModel();

                info.Index = index;
                info.Department = row["Department"].ToString();

                departmentInfos.Add(info);
            }




        }


        private ObservableCollection<GroupViewModel> groupsInfos = new ObservableCollection<GroupViewModel>();

        /// <summary>
        /// 加载群组
        /// </summary>
        private void LoadGroups(string organization, string department)
        {
            groupsInfos.Clear();

            string query = $"SELECT DISTINCT UserGroups FROM Organization WHERE Organization ='{organization}' AND Department='{department}' AND (UserGroups IS NOT NULL OR UserGroups != '')  {sqlsub}";

            Console.WriteLine(query);

            var rows = GlobalVariables.DbService.ExecuteQuery(query);
            int index = 0;

            foreach (var row in rows)
            {
                index++;
                GroupViewModel info = new GroupViewModel();

                info.Index = index;
                info.Group = row["UserGroups"].ToString();

                groupsInfos.Add(info);
            }




        }

        private void LoadUserUnits(string organization, string department, string group)
        {
            unitInfos.Clear();

            string query = $"SELECT DISTINCT UserUnit FROM Organization WHERE Organization='{organization}' AND Department='{department}' AND UserGroups='{group}' AND (UserUnit IS NOT NULL OR UserUnit != '')  {sqlsub}";




            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            int index = 0;

            foreach (var row in rows)
            {
                index++;
                var info = new UnitViewModel();

                info.Index = index;
                info.Unit = row["UserUnit"].ToString();

                unitInfos.Add(info);
            }


        }
        private void Add3Button_OnClick(object sender, RoutedEventArgs e)
        {
            int index = -1;

            if (OneListView.SelectedIndex != -1)
            {
                index = OneListView.SelectedIndex;
            }

            int index2 = -1;
            if (TowListView.SelectedIndex != -1)
            {
                index2 = TowListView.SelectedIndex;
            }


            AddOrganization3Window add = new AddOrganization3Window(index, index2);


            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                add.Owner = window;
            }



            if (add.ShowDialog() == true)
            {

                // 当子窗口关闭后执行这里的代码
                //TowListView_OnSelectionChanged(null, null);


                if (TowListView.SelectedIndex != -1)
                {
                    LoadGroups(organizationInfos[OneListView.SelectedIndex].Organization, departmentInfos[TowListView.SelectedIndex].Department);
                }


                //加载设备信息
                //LoadTags();
            }

        }

        private void Delete1Button_OnClick(object sender, RoutedEventArgs e)
        {

            if (OneListView.SelectedIndex == -1)
            {
                return;
            }


            string organization = organizationInfos[OneListView.SelectedIndex].Organization;

            var result = MessageBox.Show($"确定要删除 {organization} 吗？", "注意！", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {


                string sql = $"UPDATE  Organization  SET  Del  = 1 WHERE Organization = '{organization}' AND (Department IS NULL OR Department = '') AND (UserGroups IS NULL OR UserGroups = '')";

                Console.WriteLine(sql);

                GlobalVariables.DbService.ExecuteNonQuery(sql);

                //重新加载数据
                LoadOrganization();
            }

        }

        private void DeleteTowButton_OnClick(object sender, RoutedEventArgs e)
        {

            if (TowListView.SelectedIndex == -1)
            {
                return;
            }

            string organization = organizationInfos[OneListView.SelectedIndex].Organization;
            string department = departmentInfos[TowListView.SelectedIndex].Department;

            var result = MessageBox.Show($"确定要删除 {department} 吗？", "注意！", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                string sql = $"UPDATE  Organization  SET  Del  = 1 WHERE Organization = '{organization}' AND Department = '{department}' AND (UserGroups IS NULL OR UserGroups = '')";


                GlobalVariables.DbService.ExecuteNonQuery(sql);

                //重新加载数据
                LoadDepartment(organization);
            }

        }

        private void DeleteThreeButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ThreeListView.SelectedIndex == -1)
            {
                return;
            }

            string organization = organizationInfos[OneListView.SelectedIndex].Organization;
            string department = departmentInfos[TowListView.SelectedIndex].Department;
            string groups = groupsInfos[ThreeListView.SelectedIndex].Group;
            var result = MessageBox.Show($"确定要删除 {groups} 吗？", "注意！", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                string sql = $"UPDATE  Organization  SET  Del  = 1 WHERE Organization = '{organization}' AND Department = '{department}' AND UserGroups = '{groups}'";


                GlobalVariables.DbService.ExecuteNonQuery(sql);

                //重新加载数据
                LoadGroups(organization, department);
            }
        }


        private ObservableCollection<UnitViewModel> unitInfos = new ObservableCollection<UnitViewModel>();



        /// <summary>
        /// 选择三级层级，加载四级层级
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThreeListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThreeListView.SelectedIndex != -1)
            {
                FourListView.SelectedIndex = -1;



                var organization = organizationInfos[OneListView.SelectedIndex].Organization;
                var department = departmentInfos[TowListView.SelectedIndex].Department;
                var group = groupsInfos[ThreeListView.SelectedIndex].Group;


                LoadUserUnits(organization, department, group);



            }
        }

        private void Add4Button_OnClick(object sender, RoutedEventArgs e)
        {
            int index = -1;

            if (OneListView.SelectedIndex != -1)
            {
                index = OneListView.SelectedIndex;
            }

            int index2 = -1;
            if (TowListView.SelectedIndex != -1)
            {
                index2 = TowListView.SelectedIndex;
            }

            int index3 = -1;
            if (ThreeListView.SelectedIndex != -1)
            {
                index3 = ThreeListView.SelectedIndex;
            }

            AddOrganization4Window add = new AddOrganization4Window(index, index2, index3);


            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                add.Owner = window;
            }



            if (add.ShowDialog() == true)
            {

                // 当子窗口关闭后执行这里的代码
                //TowListView_OnSelectionChanged(null, null);


                if (ThreeListView.SelectedIndex != -1)
                {

                    //加载第四层级
                    LoadUserUnits(organizationInfos[OneListView.SelectedIndex].Organization, departmentInfos[TowListView.SelectedIndex].Department, groupsInfos[ThreeListView.SelectedIndex].Group);
                }



                //加载设备信息
                //LoadTags();
            }

        }

        private void DeleteFourButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (FourListView.SelectedIndex == -1)
            {
                return;
            }


            string organization = organizationInfos[OneListView.SelectedIndex].Organization;
            string department = departmentInfos[TowListView.SelectedIndex].Department;
            string groups = groupsInfos[ThreeListView.SelectedIndex].Group;
            string unit = unitInfos[FourListView.SelectedIndex].Unit;
            var result = MessageBox.Show($"确定要删除 {unit} 吗？", "注意！", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                string sql = $"UPDATE  Organization  SET  Del  = 1 WHERE Organization = '{organization}' AND Department = '{department}' AND UserGroups = '{groups}' AND UserUnit = '{unit}'";


                GlobalVariables.DbService.ExecuteNonQuery(sql);

                //重新加载数据
                LoadUserUnits(organization, department, groups);
            }
        }
    }
}
