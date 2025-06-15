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
using ThinkITAM.UserControls.IndexPage;
using ThinkITAM.ViewModels.AssetManage;
using ThinkITAM.ViewModels.Preset;

namespace ThinkITAM.Windows.PresetWindows
{
    /// <summary>
    /// AddAddressWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddAddressWindow : Window
    {
        public AddAddressWindow(AddressInfoViewModel info = null)
        {
            InitializeComponent();
            if (info != null)
            {
                addressInfo = info;
                inputAddress = addressInfo.Location;
                this.DataContext = addressInfo;
            }
        }

        private string inputAddress = string.Empty;

        private AddressInfoViewModel addressInfo = null;

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            string address = Location.Text;

            if (!string.IsNullOrWhiteSpace(address))
            {

                if (addressInfo != null)//UPDATE
                {
                    
                    var locationInfo = new { Location = address, Note = Note.Text };

                    var conditions = new { Location = inputAddress };

                    GlobalVariables.DbService.UpdateEntity("Address", locationInfo, conditions);

                    this.DialogResult = true;


                }
                else//INSERT
                {
                    string sqlTemp = $"SELECT COUNT(*) FROM Address WHERE Location ='{address}';";

                    var num = DbClass.ExecuteScalarTableNum(sqlTemp);

                    if (num == 0)
                    {
                       
                        var locationInfo = new { Location = address, Note = Note.Text };


                        GlobalVariables.DbService.InsertEntity("Address", locationInfo);

                        this.DialogResult = true;

                    }
                    else
                    {

                        string sql = $"SELECT COUNT(*) FROM Address WHERE Location ='{address}' AND Del = 1 ;";
                        var num2 = DbClass.ExecuteScalarTableNum(sql);

                        if (num2 == 1)
                        {
                            var result = MessageBox.Show($"当前添加的 {address} ，在数据库中已被标记为删除，是否进行恢复？", "请注意", MessageBoxButton.YesNo, MessageBoxImage.Warning);


                            if (result == MessageBoxResult.Yes)
                            {
                                string sql2 = $"UPDATE Address SET Del = NULL WHERE  Location = '{address}'";

                                GlobalVariables.DbService.ExecuteNonQuery(sql2);

                                this.DialogResult = true;

                            }


                        }
                        else
                        {
                            MessageBox.Show($"地址{address}已存在", "请注意", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }

                    }

                }

            }
        }


    }
}