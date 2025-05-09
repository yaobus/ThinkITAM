using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using ThinkITAM.UserControls.IndexPage;
using ThinkITAM.ViewModels.Preset;

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
        private void LoadPeopleInfos(string organization = null, string department = null, string group = null,string name = null)
        {
            peopleInfos.Clear();


            string filter = "WHERE ";

            if (organization != null)
            {
                filter += $" Organization='{organization}' ";

                if (department != null)
                {
                    filter += $" AND Department='{department}' ";
                   
                    if (group!=null)
                    {
                        filter += $" AND \"Group\"='{group}' ";

                        if (name != null)
                        {
                            filter += $" AND Name LIKE '%{name}%' ";
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

            if (filter!= "WHERE ")
            {
                query= $"SELECT * FROM UserInfo {filter} ;";
            }
            else
            {
                query = "SELECT * FROM UserInfo;";
            }



            Console.WriteLine(query);

            


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
                info.Group = row["Group"].ToString();
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
            string query = "SELECT Content  FROM CustomSetting WHERE Option='UserNumberPrefix';";


            var prefix = GlobalVariables.DbService.ExecuteScalar(query).ToString();

            return prefix;




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

                string query = $"SELECT DISTINCT Department FROM Organization WHERE Organization='{organizationInfo[OrganizationBox.SelectedIndex].ToString()}' AND (Department IS NOT NULL OR Department != '') AND (Groups IS NULL OR Groups = '') AND (Note != '0' OR Note IS NULL);";

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

                string query = $"SELECT DISTINCT Groups FROM Organization WHERE Organization='{organizationInfo[OrganizationBox.SelectedIndex].ToString()}' AND Department = '{departmentInfo[DepartmentBox.SelectedIndex]}' AND (Groups IS NOT NULL OR Groups != '') AND (Note != '0' OR Note IS NULL);";

                Console.WriteLine(query);


                var rows = GlobalVariables.DbService.ExecuteQuery(query);

                foreach (var row in rows)
                {
                    groupsInfo.Add(row["Groups"].ToString());
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

        private void GroupBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string org = OrganizationBox.SelectedIndex != -1 ? organizationInfo[OrganizationBox.SelectedIndex].ToString() : null;
            string dep = DepartmentBox.SelectedIndex != -1 ? departmentInfo[DepartmentBox.SelectedIndex].ToString() : null;
            string group = GroupBox.SelectedIndex != -1 ? groupsInfo[GroupBox.SelectedIndex].ToString() : null;



            LoadPeopleInfos(org, dep, group);
        }

        private void SearchButton_OnClick(object sender, RoutedEventArgs e)
        {
            peopleInfos.Clear();

            string org = OrganizationBox.SelectedIndex != -1 ? organizationInfo[OrganizationBox.SelectedIndex].ToString() : null;
            string dep = DepartmentBox.SelectedIndex != -1 ? departmentInfo[DepartmentBox.SelectedIndex].ToString() : null;
            string group = GroupBox.SelectedIndex != -1 ? groupsInfo[GroupBox.SelectedIndex].ToString() : null;



            LoadPeopleInfos(org, dep, group,UserName.Text);
        }

        private void PeopleListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PeopleListView.SelectedIndex!=-1)
            {
                var info = peopleInfos[PeopleListView.SelectedIndex];
                Number.Text = info.UserNumber;
                DataBridge.DataBridge.SelectPeopleViewModel= info;
            }

           


            
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (PeopleListView.SelectedIndex != -1)
            {
                var info = peopleInfos[PeopleListView.SelectedIndex];
              
                DataBridge.DataBridge.SelectPeopleViewModel = info;
            }

            this.DialogResult = true;
        }
    }
}
