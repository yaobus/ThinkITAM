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
    /// AddressUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class AddressUserControl : UserControl
    {
        public AddressUserControl()
        {
            InitializeComponent();
        }

        private DbClass dbClass ;

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddAddressWindow add = new AddAddressWindow();


            if (add.ShowDialog() == true)
            {

                LoadAddress();

            }
        }

        private void AddressUserControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            dbClass = new DbClass(DataBridge.DataBridge.dbFilePath);
            dbClass.OpenConnection();

            AddressListView.ItemsSource = addressInfos;

            LoadAddress();
        }

        private ObservableCollection<AddressInfoViewModel> addressInfos = new ObservableCollection<AddressInfoViewModel>();
        private void LoadAddress()
        {
            addressInfos.Clear();

            string query = "SELECT * FROM Address;";

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();

            int index = 0;

            while (reader.Read())
            {
                index++;
                AddressInfoViewModel info = new AddressInfoViewModel();

                info.Index = index;
                info.Location = reader["Location"].ToString();
                info.Note = reader["Note"].ToString();

                addressInfos.Add(info);
            }

            AddressListView.ItemsSource = addressInfos;


        }

    }
}
