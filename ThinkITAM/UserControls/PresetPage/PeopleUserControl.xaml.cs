using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
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
using ThinkITAM.ChildrenWindows.PresetWindows;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.ViewModes.Preset;

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

        private DbClass dbClass;

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
            string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\Address_database.db";
            dbClass = new DbClass(dbFilePath);
            dbClass.OpenConnection();

            PeopleListView.ItemsSource = peopleInfos;

            LoadPeopleInfos();
        }


        private ObservableCollection<PeopleViewModel> peopleInfos = new ObservableCollection<PeopleViewModel>();



        
        /// <summary>
        /// 获取用户编号前缀
        /// </summary>
        private string GetUserNumberPrefix()
        {
            string query = "SELECT *  FROM CustomSetting WHERE Option='UserNumberPrefix';";

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();


            if (reader.Read())
            {
                string prefix = reader["Content"].ToString();

                return prefix;

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

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();



            int index = 0;

            while (reader.Read())
            {
                index++;
                PeopleViewModel info = new PeopleViewModel();
                info.Index = index;
                info.UserNumber =$"{GetUserNumberPrefix()}{reader["Number"].ToString()}";
                info.Name = reader["Name"].ToString();
                info.Organization = reader["Organization"].ToString();
                info.Department = reader["Department"].ToString();
                info.Group = reader["Group"].ToString();
                info.Phone = reader["Phone"].ToString();
                info.Note = reader["Note"].ToString();

                peopleInfos.Add(info);
            }

            PeopleListView.ItemsSource = peopleInfos;


        }

    }
}
