using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.Export;
using ThinkITAM.ViewModels.Preset;
using ThinkITAM.Windows.PresetWindows;

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

            if (prefix != null)
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

            string query = "SELECT * FROM UserInfo WHERE (Del != 1 OR Del IS NULL);";

            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            int index = 0;
            var prefix = GetUserNumberPrefix();
            foreach (var row in rows)
            {
                index++;
                PeopleViewModel info = new PeopleViewModel();
                info.UserId = row["UserId"].ToString();
                info.Index = index;
                info.Number = row["Number"].ToString();
                info.UserNumber = $"{prefix}{row["Number"].ToString()}";
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



        private void PeopleListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = PeopleListView.SelectedIndex;
            if (index != -1)
            {
                EditAssetButton.IsEnabled = true;
                DeleteAssetButton.IsEnabled = true;


            }
            else
            {
                EditAssetButton.IsEnabled = false;
                DeleteAssetButton.IsEnabled = false;
            }
        }

        private void EditAssetButton_OnClick(object sender, RoutedEventArgs e)
        {
            var index = PeopleListView.SelectedIndex;

            if (index != -1)
            {

                var info = peopleInfos[index];

                AddPeopleWindow addPeople = new AddPeopleWindow(info);


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

        private void PeopleListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var index = PeopleListView.SelectedIndex;

            if (index != -1)
            {

                var info = PeopleListView.SelectedItem as PeopleViewModel;

                var addPeople = new AddPeopleWindow(info);


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

        private void DeleteAssetButton_OnClick(object sender, RoutedEventArgs e)
        {
            var index = PeopleListView.SelectedIndex;

            if (index != -1)
            {

                var info = PeopleListView.SelectedItem as PeopleViewModel;

                var message = $"确定要删除吗？\r人员:{info.Name}\r所在一级组织:{info.Organization}\r所在二级组织:{info.Department}\r所在三级组织:{info.Group}";


                var result = MessageBox.Show(message, "警告", MessageBoxButton.YesNo, MessageBoxImage.Question);


                if (result == MessageBoxResult.Yes)
                {
                    string query = $"UPDATE UserInfo SET Del = 1 WHERE UserId ='{info.UserId}';";


                    GlobalVariables.DbService.ExecuteNonQuery(query);

                    LoadPeopleInfos();

                }


            }
        }

        private void DataImport_OnClick(object sender, RoutedEventArgs e)
        {
            var newWindow = new UserDataImportWindow();


            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                newWindow.Owner = window;
            }

            if (newWindow.ShowDialog() == true)
            {

                LoadPeopleInfos();

            }
        }

        private void DataExport_OnClick(object sender, RoutedEventArgs e)
        {
            if (peopleInfos == null || peopleInfos.Count == 0)
            {
                MessageBox.Show("没有可导出的数据。");
                return;
            }

            var fileName = $"UserInfo";

            // 创建保存文件对话框
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel 文件 (*.xlsx)|*.xlsx|所有文件 (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = fileName  // 默认文件名
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = saveFileDialog.FileName;


                // 调用导出方法
                ExcelExporter.ExportToExcel(peopleInfos, selectedFilePath);
            }

        }
    }
}
