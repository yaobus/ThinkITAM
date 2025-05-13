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


            AddressListView.ItemsSource = addressInfos;

            LoadAddress();
        }

        private ObservableCollection<AddressInfoViewModel> addressInfos = new ObservableCollection<AddressInfoViewModel>();
        private void LoadAddress()
        {
            addressInfos.Clear();

            string query = "SELECT * FROM Address;";

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

    }
}
