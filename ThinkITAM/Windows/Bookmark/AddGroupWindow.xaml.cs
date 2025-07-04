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

namespace ThinkITAM.Windows.Bookmark
{
    /// <summary>
    /// AddGroupWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddGroupWindow : Window
    {
        public AddGroupWindow()
        {
            InitializeComponent();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {

            var sql = $"SELECT COUNT(*) FROM BookmarkGroupOrder WHERE TypeGroup='{TypeGroup.Text}'";

            var count = Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(sql));

            if (count == 0)
            {
                if (!string.IsNullOrWhiteSpace(TypeGroup.Text))
                {
                    var info = new
                    {
                        TypeGroup = TypeGroup.Text,
                        DisplayOrder = DbClass.GetNextAvailableNumber("BookmarkGroupOrder", "DisplayOrder")
                    };

                    GlobalVariables.DbService.InsertEntity("BookmarkGroupOrder", info);
                    this.DialogResult = true;
                }
                else
                {
                    MessageBox.Show("名称不得为空!", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }
            else
            {
                 sql = $"SELECT COUNT(*) FROM BookmarkGroupOrder WHERE TypeGroup='{TypeGroup.Text}' AND Del = 1";

                 count = Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(sql));

                if (count == 1)
                {
                    var result = MessageBox.Show("该分组已存在但被标记为删除，是否恢复？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                       sql = $"UPDATE BookmarkGroupOrder SET Del = NULL WHERE TypeGroup='{TypeGroup.Text}'";   
                       GlobalVariables.DbService.ExecuteNonQuery(sql);
                       this.DialogResult = true;
                    }
                }
                else
                {
                    MessageBox.Show("分组已存在!\r请勿重复添加", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }


            }


        }
    }
}
