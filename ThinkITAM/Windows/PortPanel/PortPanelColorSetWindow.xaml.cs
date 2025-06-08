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
using ThinkITAM.UserControls.LinkPage;
using ThinkITAM.ViewModels.LinkManage;
using ThinkITAM.ViewModels.PortPanel;

namespace ThinkITAM.Windows.PortPanel
{
    /// <summary>
    /// PortPanelColorSetWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PortPanelColorSetWindow : Window
    {
        public PortPanelColorSetWindow(PortClass portInfo)
        {
            InitializeComponent();
            port = portInfo;
        }


        private PortClass port;


        private void PortPanelColorSetWindow_OnLoaded(object sender, RoutedEventArgs e)
        {

            this.DataContext = port;
        }


        /// <summary>
        /// 选择颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PortColor_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = PortColor.SelectedIndex;

            int x = 0;


            if (index != -1)
            {
                foreach (var selectedItem in PortColor.Items)
                {
                    var item = selectedItem as ListBoxItem;

                    if (item != null && index == x)
                    {
                        item.Opacity = 1;
                        item.BorderBrush = SystemColors.ActiveBorderBrush;
                        item.BorderThickness = new Thickness(2);

                        ColorGrid.Background = item.Background;
                    }
                    else
                    {
                        item.Opacity = 0.1;
                        item.BorderBrush = null;
                        item.BorderThickness = new Thickness(0);
                    }

                    x++;
                }

            }
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
           
            string sql = $"UPDATE  Bu_{DataBridge.DataBridge.SelectBuildingId}  SET  PortColor  = '{PortColor.SelectedIndex}' WHERE  UID='{port.UID}'";

           

          
            GlobalVariables.DbService.ExecuteNonQuery(sql);

            DialogResult = true;
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
