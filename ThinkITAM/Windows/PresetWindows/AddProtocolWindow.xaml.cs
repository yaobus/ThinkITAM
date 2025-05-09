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
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;

namespace ThinkITAM.Windows.PresetWindows
{
    /// <summary>
    /// AddProtocolWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddProtocolWindow : Window
    {
        public AddProtocolWindow()
        {
            InitializeComponent();
        }



        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            string protocol = ProtocolName.Text;
            string note = this.Note.Text;

            if (protocol.Replace(" ", "").Length >= 2 )
            {
                string sqlTemp = $"SELECT COUNT(*) FROM Protocol WHERE Protocol ='{protocol}'";
                var num = DbClass.ExecuteScalarTableNum(sqlTemp);

                if (num <= 0)
                {
                    sqlTemp = $"INSERT INTO \"Protocol\" (\"Protocol\", \"Note\") VALUES ('{protocol}', '{note}')";

                    GlobalVariables.DbService.ExecuteNonQuery(sqlTemp);

                    this.DialogResult = true;

                    this.Close();


                }
                else
                {
                    MessageBox.Show("该协议已存在", "请注意", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void AddProtocolWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
