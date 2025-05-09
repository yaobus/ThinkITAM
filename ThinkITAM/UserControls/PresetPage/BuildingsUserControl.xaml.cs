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
using ThinkITAM.ViewModels.LinkManage;
using ThinkITAM.ViewModels.PortPanel;
using System.Collections;
using ThinkITAM.DataBridge;

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


            BuildingListView.ItemsSource = buildingInfos;
            LoadBuildingInfos();
        }


        private ObservableCollection<BuildingInfoClass> buildingInfos = new ObservableCollection<BuildingInfoClass>();

        private void LoadBuildingInfos()
        {
            buildingInfos.Clear();
            string sql = "SELECT * FROM Buildings";



            var rows = GlobalVariables.DbService.ExecuteQuery(sql);

            int index = 0;

            foreach (var row in rows)
            {
                                index++;
                BuildingInfoClass info = new BuildingInfoClass();

                info.Index = index;
                info.BuildingId = row["BuildingId"].ToString();
                info.Building = row["Building"].ToString();
                info.Address = row["Address"].ToString();
                info.User = row["User"].ToString();
                info.Phone = row["Phone"].ToString();
                info.Note = row["Note"].ToString();

                buildingInfos.Add(info);


            }

        }
    }
}
