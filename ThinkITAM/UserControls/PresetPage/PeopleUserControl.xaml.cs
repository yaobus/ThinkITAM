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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThinkITAM.Windows.PresetWindows;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.ViewModels.Preset;
using ThinkITAM.DataBridge;

namespace ThinkITAM.UserControls.PresetPage
{
    /// <summary>
    /// PeopleUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class PeopleUserControl : UserControl
    {
        public PeopleUserControl()
        {
            InitializeComponent();
        }



        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddPeopleWindow addPeople = new AddPeopleWindow();


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

        private void PeopleUserControl_OnLoaded(object sender, RoutedEventArgs e)
        {


            PeopleListView.ItemsSource = peopleInfos;

            LoadPeopleInfos();
        }


        private ObservableCollection<PeopleViewModel> peopleInfos = new ObservableCollection<PeopleViewModel>();




        /// <summary>
        /// 获取用户编号前缀
        /// </summary>
        private string GetUserNumberPrefix()
        {
            string query = "SELECT Content  FROM CustomSetting WHERE Option='UserNumberPrefix';";


            var prefix = GlobalVariables.DbService.ExecuteScalar(query);

            if (prefix!=null)
            {
                return prefix.ToString();
            }
            else
            {
                return "";
            }

        }




        /// <summary>
        /// 加载人员信息
        /// </summary>
        private void LoadPeopleInfos()
        {
            peopleInfos.Clear();

            string query = "SELECT * FROM UserInfo;";

            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            int index = 0;

            foreach (var row in rows)
            {
                                index++;
                PeopleViewModel info = new PeopleViewModel();
                info.Index = index;
                info.UserNumber = $"{GetUserNumberPrefix()}{row["Number"].ToString()}";
                info.Name = row["Name"].ToString();
                info.Organization = row["Organization"].ToString();
                info.Department = row["Department"].ToString();
                info.Group = row["UserGroup"].ToString();
                info.Phone = row["Phone"].ToString();
                info.Note = row["Note"].ToString();

                peopleInfos.Add(info);
            }



            PeopleListView.ItemsSource = peopleInfos;


        }

    }
}
