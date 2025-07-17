using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ThinkITAM.UserControls.Computer
{
    /// <summary>
    /// DeviceRoomInfo.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceBuilding : UserControl
    {
        public DeviceBuilding()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 长度溢出时显示遮罩
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
            var availableWidth = parentGrid.ActualWidth * 0.7; // 90%的Grid宽度

            if (textWidth > availableWidth)
            {
                MaskBrush.GradientStops[0].Offset = 0;
                MaskBrush.GradientStops[1].Offset = 0.7;
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
}
