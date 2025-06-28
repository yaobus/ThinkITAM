using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.ViewModels.Preset;
using ThinkITAM.Windows.PresetWindows;

namespace ThinkITAM.UserControls.PresetPage
{
    /// <summary>
    /// AddressUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class AddressUserControl : UserControl
    {
        public AddressUserControl()
        {
            InitializeComponent();
        }

        private DbClass dbClass;

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddAddressWindow add = new AddAddressWindow();

            var window = Window.GetWindow(this);
            if (window != null)
            {
                add.Owner = window;
            }


            if (add.ShowDialog() == true)
            {

                LoadAddress();

            }
        }

        private void AddressUserControl_OnLoaded(object sender, RoutedEventArgs e)
        {


            AddressListView.ItemsSource = addressInfos;

            LoadAddress();
        }

        private ObservableCollection<AddressInfoViewModel> addressInfos = new ObservableCollection<AddressInfoViewModel>();
        private void LoadAddress()
        {
            addressInfos.Clear();

            string query = "SELECT * FROM Address WHERE (Del != 1 OR Del IS NULL);";

            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            int index = 0;

            foreach (var row in rows)
            {
                index++;
                AddressInfoViewModel info = new AddressInfoViewModel();

                info.Index = index;
                info.Location = row["Location"].ToString();
                info.Note = row["Note"].ToString();

                addressInfos.Add(info);
            }



            AddressListView.ItemsSource = addressInfos;


        }

        private void AddressListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = AddressListView.SelectedIndex;

            if (index != -1)
            {
                EditAddressButton.IsEnabled = true;
                DeleteAddressButton.IsEnabled = true;

            }
            else
            {
                EditAddressButton.IsEnabled = false;
                DeleteAddressButton.IsEnabled = false;
            }
        }

        private void EditAddressButton_OnClick(object sender, RoutedEventArgs e)
        {
            var index = AddressListView.SelectedIndex;

            if (index != -1)
            {

                var info = addressInfos[index];

                AddAddressWindow add = new AddAddressWindow(info);
                //窗口放中间
                var window = Window.GetWindow(this);
                if (window != null)
                {
                    add.Owner = window;
                }

                if (add.ShowDialog() == true)
                {

                    LoadAddress();

                }


            }
        }

        private void AddressListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var index = AddressListView.SelectedIndex;

            if (index != -1)
            {

                var info = addressInfos[index];

                AddAddressWindow add = new AddAddressWindow(info);
                //窗口放中间
                var window = Window.GetWindow(this);
                if (window != null)
                {
                    add.Owner = window;
                }

                if (add.ShowDialog() == true)
                {

                    LoadAddress();

                }


            }
        }

        private void DeleteAddressButton_OnClick(object sender, RoutedEventArgs e)
        {
            var index = AddressListView.SelectedIndex;

            if (index != -1)
            {

                var info = addressInfos[index];


                var message = $"确定要删除吗？\r地址:{info.Location}\r备注:{info.Note}";


                var result = MessageBox.Show(message, "警告", MessageBoxButton.YesNo, MessageBoxImage.Question);


                if (result == MessageBoxResult.Yes)
                {
                    string query = $"UPDATE Address SET Del = 1 WHERE Location ='{info.Location}';";


                    GlobalVariables.DbService.ExecuteNonQuery(query);

                    LoadAddress();

                }


            }
        }
    }
}
