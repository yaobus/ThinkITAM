using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DocumentFormat.OpenXml.EMMA;
using ThinkITAM.DataBridge;
using ThinkITAM.ViewModels.PortPanel;
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


                var result = MessageBox.Show("确定要删除该建筑吗？该操作不可恢复！\r此操作将同步导致终端管理页面和面板管理页面无法访问该建筑信息！", "警告", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {

                        var query = $"SELECT COUNT(OnTheLine) FROM Bu_{info.BuildingId} WHERE OnTheLine > 0";

                        int count = Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(query));


                        if (count > 0)
                        {
                            MessageBox.Show("该建筑物内有端口位于链路上，无法进行删除", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            var sql = $"UPDATE Buildings SET Del = 1 WHERE BuildingId ='{info.BuildingId}';";

                        GlobalVariables.DbService.ExecuteNonQuery(sql);

                            LoadBuildingInfos();
                        }


                }


            }
        }
    }
}
