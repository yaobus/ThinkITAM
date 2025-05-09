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
using ThinkITAM.UserControls.LinkPage;
using ThinkITAM.ViewModels.LinkManage;
using ThinkITAM.ViewModes.PortPanel;

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
        private DbClass dbClass;

        private void PortPanelColorSetWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            ColorGrid.Background = GetBrushByIndex(port.PortColor) ;

            dbClass = new DbClass(DataBridge.DataBridge.dbFilePath);
            dbClass.OpenConnection();

        }


        private Brush GetBrushByIndex(int index)
        {

            switch (index)
            {
                case 1:
                    return ColorToBrush("#FF0080");
                case 2:
                    return ColorToBrush("#F15A24");
                case 3:
                    return ColorToBrush("#FBC02D");
                case 4:
                    return ColorToBrush("#64DD17");
                case 5:
                    return ColorToBrush("#00A8FF");
                case 6:
                    return ColorToBrush("#008080");
                case 7:
                    return ColorToBrush("#362391");
                case 8:
                    return ColorToBrush("#212121");
                default:
                    return Brushes.Transparent;
            }
        }

        private SolidColorBrush ColorToBrush(string hexColor)
        {
            return (SolidColorBrush)(new BrushConverter().ConvertFrom(hexColor));
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
           
            string sql = $"UPDATE \"Bu_{DataBridge.DataBridge.SelectBuildingId}\" SET \"PortColor\" = '{PortColor.SelectedIndex}' WHERE  UID='{port.UID}'";

           

            dbClass.ExecuteQuery(sql);

            DialogResult = true;
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
