using System;
using System.Collections;
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
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.ViewModels.LinkManage;
using ThinkITAM.ViewModels.PortPanel;
using ThinkITAM.ViewModels.Preset;
using ThinkITAM.Windows.PresetWindows;

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
            string sql = "SELECT * FROM Buildings WHERE (Del != 1 OR Del IS NULL)";



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

        private void BuildingListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int num = BuildingListView.SelectedIndex;

            if (num != -1)
            {

                EditButton.IsEnabled = true;
                DeleteButton.IsEnabled = true;
            }
            else
            {
                EditButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
            }
        }

        private void BuildingListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int num = BuildingListView.SelectedIndex;
            var info = buildingInfos[num];

            if (num != -1)
            {
                
                AddBuildingWindow add = new AddBuildingWindow(info);


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
        }

        private void EditButton_OnClick(object sender, RoutedEventArgs e)
        {
            int num = BuildingListView.SelectedIndex;
            var info = buildingInfos[num];

            if (num != -1)
            {

                AddBuildingWindow add = new AddBuildingWindow(info);


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
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            var index = BuildingListView.SelectedIndex;

            if (index != -1)
            {

                var info = buildingInfos[index];

                var message = $"确定要删除吗？\r建筑:{info.Building}\r所在地址:{info.Address}";


                var result = MessageBox.Show(message, "警告", MessageBoxButton.YesNo, MessageBoxImage.Question);


                if (result == MessageBoxResult.Yes)
                {
                    string query = $"UPDATE Buildings SET Del = 1 WHERE BuildingId ='{info.BuildingId}';";

                    GlobalVariables.DbService.ExecuteNonQuery(query);

                    LoadBuildingInfos();

                }


            }
        }
    }
}
