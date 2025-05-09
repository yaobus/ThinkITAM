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
    /// AddPortWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddPortWindow : Window
    {
        public AddPortWindow()
        {
            InitializeComponent();
        }



        private void AddPortWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
           

            if (IsNumeric(Port.Text))
            {
                string port = Port.Text;
                string note = Note.Text;

                if (port.Replace(" ", "").Length >= 2)
                {
                    string sqlTemp = $"SELECT COUNT(*) FROM PortList WHERE Port ='{port}'";
                    var num = DbClass.ExecuteScalarTableNum(sqlTemp);

                    if (num <= 0)
                    {
                        sqlTemp = $"INSERT INTO \"PortList\" (\"Port\", \"Note\") VALUES ({port}, '{note}')";

                       
                        GlobalVariables.DbService.ExecuteNonQuery(sqlTemp);

                        this.DialogResult = true;

                        this.Close();


                    }
                    else
                    {
                        MessageBox.Show("该端口已存在", "请注意", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            else
            {
             MessageBox.Show("端口号只能为整数", "请注意", MessageBoxButton.OK, MessageBoxImage.Warning);
            }


        }


        /// <summary>
        /// 判断是否为数字
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool IsNumeric(string input)
        {
            int result;
            return int.TryParse(input, out result);
        }
    }
}
