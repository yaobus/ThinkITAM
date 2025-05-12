using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThinkITAM.Windows.DevicePortManage;
using ThinkITAM.Windows.PresetWindows;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.ViewModels.Preset;
using ThinkITAM.DataBridge;

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

       private DbClass dbClass;

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

        private string sqlsub = "AND (Note != '0' OR Note IS NULL)";


        private void LoadOrganization()
        {
            organizationInfos.Clear();

            string query = $"SELECT DISTINCT Organization FROM Organization WHERE  ( Department IS  NULL OR Department = '') AND ( GROUPS IS NULL OR GROUPS = '' ) {sqlsub}";
            
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

            OneListView.ItemsSource=organizationInfos;
            TowListView.ItemsSource = departmentInfos;
            ThreeListView.ItemsSource = groupsInfos;
            LoadOrganization();
        }

        //private ObservableCollection<OrganizationOneViewModel> organizationInfos2 = new ObservableCollection<OrganizationOneViewModel>();



        private void OneListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OneListView.SelectedIndex != -1)
            {
                TowListView.SelectedIndex=-1;
                departmentInfos.Clear();
                
                ThreeListView.SelectedIndex = -1;
                groupsInfos.Clear();


                string name = organizationInfos[OneListView.SelectedIndex].Organization;

                string query = $"SELECT DISTINCT Department FROM Organization WHERE Organization='{name}' AND ( Department IS NOT NULL OR Department != '') AND ( GROUPS IS NULL OR GROUPS = '')  {sqlsub}";



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
                ThreeListView.SelectedIndex=-1;
                groupsInfos.Clear();

                string name = organizationInfos[OneListView.SelectedIndex].Organization;


                if (departmentInfos.Count > 0 && TowListView.SelectedIndex != -1)
                {
                    string name2 = departmentInfos[TowListView.SelectedIndex].Department;

                    string query = $"SELECT DISTINCT Groups FROM Organization WHERE Organization='{name}' AND Department='{name2}' AND (Groups IS NOT NULL OR Groups != '')  {sqlsub}";

                   

                    var rows = GlobalVariables.DbService.ExecuteQuery(query);

                    int index = 0;

                    foreach (var row in rows)
                    {
                                              index++;
                        GroupViewModel info = new GroupViewModel();

                        info.Index = index;
                        info.Group = row["Groups"].ToString();

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
            AddOrganization2Window add = new AddOrganization2Window();


            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                add.Owner = window;
            }



            if (add.ShowDialog() == true)
            {

                // 当子窗口关闭后执行这里的代码

                if (OneListView.SelectedIndex!=-1)
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

            string query = $"SELECT DISTINCT Department FROM Organization WHERE Organization ='{organization}' AND(Department IS NOT NULL OR Department !='') AND (Groups IS NULL OR Groups = '')  {sqlsub}";


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
        private void LoadGroups(string organization,string department)
        {
            groupsInfos.Clear();

            string query = $"SELECT DISTINCT Groups FROM Organization WHERE Organization ='{organization}' AND Department='{department}' AND (Groups IS NOT NULL OR Groups != '')  {sqlsub}";

            Console.WriteLine(query);

            var rows = GlobalVariables.DbService.ExecuteQuery(query);
            int index = 0;

            foreach (var row in rows)
            {
                                index++;
                GroupViewModel info = new GroupViewModel();

                info.Index = index;
                info.Group = row["Groups"].ToString();

                groupsInfos.Add(info);
            }




        }


        private void Add3Button_OnClick(object sender, RoutedEventArgs e)
        {
            AddOrganization3Window add = new AddOrganization3Window();


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
                

                if (TowListView.SelectedIndex!=-1)
                {
                    LoadGroups(organizationInfos[OneListView.SelectedIndex].Organization, departmentInfos[TowListView.SelectedIndex].Department);
                }


                //加载设备信息
                //LoadTags();
            }

        }

        private void Delete1Button_OnClick(object sender, RoutedEventArgs e)
        {

            string organization = organizationInfos[OneListView.SelectedIndex].Organization;

            var result = MessageBox.Show($"确定要删除 {organization} 吗？", "注意！", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
               

                string sql = $"UPDATE \"Organization\" SET \"Note\" = '0' WHERE Organization = '{organization}' AND (Department IS NULL OR Department = '') AND (Groups IS NULL OR Groups = '')";

                
             
                GlobalVariables.DbService.ExecuteNonQuery(sql);
                
                //重新加载数据
                LoadOrganization();
            }

        }

        private void DeleteTowButton_OnClick(object sender, RoutedEventArgs e)
        {
            string organization = organizationInfos[OneListView.SelectedIndex].Organization;
            string department = departmentInfos[TowListView.SelectedIndex].Department;

            var result = MessageBox.Show($"确定要删除 {department} 吗？", "注意！", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                string sql = $"UPDATE \"Organization\" SET \"Note\" = '0' WHERE Organization = '{organization}' AND Department = '{department}' AND (Groups IS NULL OR Groups = '')";
                
               
                GlobalVariables.DbService.ExecuteNonQuery(sql);

                //重新加载数据
                LoadDepartment(organization);
            }

        }

        private void DeleteThreeButton_OnClick(object sender, RoutedEventArgs e)
        {
            string organization = organizationInfos[OneListView.SelectedIndex].Organization;
            string department = departmentInfos[TowListView.SelectedIndex].Department;
            string groups = groupsInfos[ThreeListView.SelectedIndex].Group;
            var result = MessageBox.Show($"确定要删除 {groups} 吗？", "注意！", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                string sql = $"UPDATE \"Organization\" SET \"Note\" = '0' WHERE Organization = '{organization}' AND Department = '{department}' AND Groups = '{groups}'";

                
                GlobalVariables.DbService.ExecuteNonQuery(sql);

                //重新加载数据
                LoadGroups(organization, department);
            }
        }
    }
}
