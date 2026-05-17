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
