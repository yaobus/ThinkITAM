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

namespace ThinkITAM.Windows.PresetWindows
{
    /// <summary>
    /// AddAddressWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddAddressWindow : Window
    {
        public AddAddressWindow()
        {
            InitializeComponent();
        }



        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            string address = Location.Text;

            if (address.Replace(" ", "").Length > 2)
            {
                string sqlTemp = $"SELECT COUNT(*) FROM Address WHERE Location ='{address}'";

                var num = DbClass.ExecuteScalarTableNum(sqlTemp);

                if (num <= 0)
                {

                    var locationInfo = new { Location = address, Note = Note.Text };



                    //sqlTemp = $"INSERT INTO \"Address\" (\"Location\", \"Note\") VALUES ('{address}', '{Note.Text}')";


                    GlobalVariables.DbService.InsertEntity("Address", locationInfo);

                    this.DialogResult = true;

                    this.Close();


                }
                else
                {
                    MessageBox.Show("地址已存在", "请注意", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void AddAddressWindow_OnLoaded(object sender, RoutedEventArgs e)
        {


        }
    }
}