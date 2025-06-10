using ThinkITAM.DatabaseOperation;
using ThinkITAM.ViewModels.AssetManage;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using ThinkITAM.DataBridge;
using ThinkITAM.ViewModels.Preset;

namespace ThinkITAM.Windows.PresetWindows
{
    /// <summary>
    /// AddAddressWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddAddressWindow : Window
    {
        public AddAddressWindow(AddressInfoViewModel addressInfo = null)
        {
            InitializeComponent();
            if (addressInfo != null)
            {
                address = addressInfo;
                inputAddress = addressInfo.Location;
                this.DataContext = address;
            }
        }

        private string inputAddress = string.Empty;
        private AddressInfoViewModel address = new AddressInfoViewModel();

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            string address = Location.Text;

            if (address.Replace(" ", "").Length > 2)
            {

                if (address != null)//UPDATE
                {

                    var locationInfo = new { Location = address, Note = Note.Text };

                    var conditions = new { Location = inputAddress };

                    GlobalVariables.DbService.UpdateEntity("Address", locationInfo, conditions);

                    this.DialogResult = true;


                }
                else//INSERT
                {
                    string sqlTemp = $"SELECT COUNT(*) FROM Address WHERE Location ='{address}'";

                    var num = DbClass.ExecuteScalarTableNum(sqlTemp);

                    if (num <= 0)
                    {

                        var locationInfo = new { Location = address, Note = Note.Text };


                        GlobalVariables.DbService.InsertEntity("Address", locationInfo);

                        this.DialogResult = true;

                    }
                    else
                    {
                        MessageBox.Show("地址已存在", "请注意", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }











            }
        }


    }
}