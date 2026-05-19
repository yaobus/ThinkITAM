using System;
using System.Collections.Generic;
using System.Globalization;
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
using ThinkITAM.DataBridge;
using ThinkITAM.ViewModels.NetworkManage;
using ThinkITAM.ViewModels.Others;
using ThinkITAM.Windows.NetworkManage;
using ThinkITAM.Windows.ToolWindows;

namespace ThinkITAM.UserControls.WakeOnLan;
/// <summary>
/// HostUserControl.xaml 的交互逻辑
/// </summary>
public partial class HostUserControl : UserControl
{
    public HostUserControl()
    {
        InitializeComponent();
    }

    private void WolButton_OnClick(object sender, RoutedEventArgs e)
    {
        
    }

    private void WolButton_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
       
    }

    
    /// <summary>
    /// 右键菜单响应
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItem_OnClick(object sender, RoutedEventArgs e)
    {

        // 获取触发事件的MenuItem
        var menuItem = sender as MenuItem;

        if (menuItem != null)
        {
            var info = this.DataContext as WakeOnLanHostViewModel;

            // 根据菜单项的不同进行相应的处理
            switch (menuItem.Tag)
            {
                case "DeleteHost":
                   
                    var sql = $"DELETE FROM WakeOnLan WHERE UID = {info.UID}";

                    var result = MessageBox.Show($"确定要删除主机 {info.Name} 吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        GlobalVariables.DbService.ExecuteQuery(sql);
                    }
                    //冒泡到父级控件，刷新列表

                    break;

                case "EditHost":
                    // 执行选项2的操作
                    
                    AddWakeOnLan wake = new AddWakeOnLan(info);



                    var window3 = Window.GetWindow(this);
                    if (window3 != null)
                    {
                        wake.Owner = window3;
                    }

                    wake.ShowDialog();


                    break;

            }
        }

    }

    /// <summary>
    /// 长度溢出则开启遮罩
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TextBlock_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var textBlock = sender as TextBlock;
        var parentGrid = textBlock?.Parent as Grid;

        if (textBlock == null || parentGrid == null) return;

        // 计算文本的实际渲染宽度
        var formattedText = new FormattedText(
            textBlock.Text,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
            textBlock.FontSize,
            Brushes.Black,
            VisualTreeHelper.GetDpi(textBlock).PixelsPerDip);

        var textWidth = formattedText.Width;
        var availableWidth = parentGrid.ActualWidth * 0.75; // 75%的Grid宽度

        if (textWidth > availableWidth)
        {
            MaskBrush.GradientStops[0].Offset = 0;
            MaskBrush.GradientStops[1].Offset = 0.75;
            MaskBrush.GradientStops[2].Offset = 1;
        }
        else
        {
            // 不需要遮罩
            MaskBrush.GradientStops[0].Offset = 1;
            MaskBrush.GradientStops[1].Offset = 1;
            MaskBrush.GradientStops[2].Offset = 1;
        }
    }
}
