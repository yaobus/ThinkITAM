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
using ThinkITAM.ViewModes.LinkManage;
using ThinkITAM.ViewModes.PortPanel;

namespace ThinkITAM.UserControls.PresetPage
{
    /// <summary>
    /// BuildingsUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class BuildingsUserControl : UserControl
    {
        public BuildingsUserControl()
        {
            InitializeComponent();
        }

        private DbClass dbClass;

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddBuildingWindow add = new AddBuildingWindow();


            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                add.Owner = window;
            }

            if (add.ShowDialog() == true)
            {

                LoadBuildingInfos();

            }
        }

        private void BuildingsUserControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            dbClass = new DbClass(DataBridge.DataBridge.dbFilePath);
            dbClass.OpenConnection();

            BuildingListView.ItemsSource = buildingInfos;
            LoadBuildingInfos();
        }


        private ObservableCollection<BuildingInfoClass> buildingInfos = new ObservableCollection<BuildingInfoClass>();

        private void LoadBuildingInfos()
        {
            buildingInfos.Clear();
            string sql = "SELECT * FROM Buildings";


            SQLiteCommand command = new SQLiteCommand(sql, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();

            int index = 0;

            while (reader.Read())
            {
                index++;
                BuildingInfoClass info = new BuildingInfoClass();

                info.Index = index;
                info.BuildingId = reader["BuildingId"].ToString();
                info.Building = reader["Building"].ToString();
                info.Address = reader["Address"].ToString();
                info.User = reader["User"].ToString();
                info.Phone = reader["Phone"].ToString();
                info.Note = reader["Note"].ToString();

                buildingInfos.Add(info);



            }
        }
    }
}
