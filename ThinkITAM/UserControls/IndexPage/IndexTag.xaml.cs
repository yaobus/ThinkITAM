using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.Windows.NetworkManage;

namespace ThinkITAM.UserControls.IndexPage
{
    /// <summary>
    /// IndexTag.xaml 的交互逻辑
    /// </summary>
    public partial class IndexTag : UserControl
    {
        public IndexTag()
        {
            InitializeComponent();
           
        }


        private void IndexButton_OnClick(object sender, RoutedEventArgs e)
        {
            var tagInfo = (sender as Button).DataContext as ViewModels.Index.IndexTagViewModel;

            var url = FixSmbPath($"{tagInfo.Protocol}{tagInfo.Host}");

            var browser = tagInfo.Browser;




            if (string.IsNullOrWhiteSpace( tagInfo.Port))//未配置端口
            {
                tagInfo.Url = url;
            }
            else
            {
                tagInfo.Url = $"{url}:{tagInfo.Port}";
            }




            try
            {
                if (!string.IsNullOrWhiteSpace(browser))//有指定浏览器
                {
                   
                    OpenUrlClass.OpenUrlInSpecificBrowser(tagInfo.Url, browser);

                }
                else
                {

                    var type = ProtocolDetector.DetectProtocol(tagInfo.Url);



                    switch (type)
                    {
                        case ProtocolType.HTTP:
                        case ProtocolType.HTTPS:
                            OpenUrlClass.OpenUrlInSpecificBrowser(tagInfo.Url, browser);
                           
                            break;

                        case ProtocolType.FILE:
                        case ProtocolType.SMB:
                        case ProtocolType.OTHER:
                        case ProtocolType.Unknown:
                        case ProtocolType.FTP:
                            Process.Start("explorer.exe", tagInfo.Url);
                            break;
                    }


                }


            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);

            }





        }

        /// <summary>
        /// 修正SMB路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string FixSmbPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return path;

            // 判断是否是 SMB 路径（以 \\ 或 // 开头）
            if ((path.StartsWith("\\\\") || path.StartsWith("//")) == false)
            {
                return path; // 非 SMB 路径，直接返回
            }

            // 统一使用反斜杠
            path = path.Replace('/', '\\');

            // 分割路径中的层级
            string[] parts = path.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

            // SMB 路径最少需要 server 和 share，所以至少要两个部分
            if (parts.Length < 2)
                return path;

            // 构造基础路径：\\server\share
            string basePath = "\\\\" + parts[0] + "\\" + parts[1];

            // 补足路径层级至 4 层
            if (parts.Length < 4)
            {
                for (int i = parts.Length; i < 4; i++)
                {
                    basePath += "\\";
                }
            }

            return basePath;
        }
    

        /// <summary>
        /// 右键菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
           


                ViewModels.Index.IndexTagViewModel tagInfo = null;
                var menuItem = sender as MenuItem;
                if (menuItem != null)
                {
                    var contextMenu = menuItem.Parent as ContextMenu;
                    if (contextMenu != null)
                    {
                        var button = contextMenu.PlacementTarget as Button;
                        if (button != null)
                        {
                            tagInfo = button.DataContext as ViewModels.Index.IndexTagViewModel;
                            // 在这里进行进一步的操作...
                        }
                    }
                }

                //string url = DataBridge.DataBridge.SelectNetwork + selectAddress;



                if (menuItem != null)
                {
                    // 根据菜单项的不同进行相应的处理
                    switch (menuItem.Header.ToString())
                    {
                        case "删除标签":

                            string portSql;

                            if (tagInfo.Port.Length == 0)
                            {
                                portSql = $"(Port='' OR Port IS NULL) ";
                            }
                            else
                            {
                                portSql = $"Port = '{tagInfo.Port}' ";
                            }


                            var sql = $"DELETE FROM Bookmark WHERE ( TypeGroup='{tagInfo.Group}' AND Protocol='{tagInfo.Protocol}' AND Host='{tagInfo.Host}' AND {portSql})";




                            GlobalVariables.DbService.ExecuteNonQuery(sql);

                            DataBridge.DataBridge.modifyIndexTags.Add("1");

                            break;

                        case "编辑标签":

                            if (tagInfo != null)
                            {
                                var addressCollectWindow = new AddressCollectWindow(null, tagInfo);
                                //窗口放中间
                                var window = Window.GetWindow(this);
                                if (window != null)
                                {
                                    addressCollectWindow.Owner = window;
                                }

                                addressCollectWindow.ShowDialog();


                            }

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


        private void TextBlock2_OnSizeChanged(object sender, SizeChangedEventArgs e)
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
                MaskBrush2.GradientStops[0].Offset = 0;
                MaskBrush2.GradientStops[1].Offset = 0.75;
                MaskBrush2.GradientStops[2].Offset = 1;
            }
            else
            {
                // 不需要遮罩
                MaskBrush2.GradientStops[0].Offset = 1;
                MaskBrush2.GradientStops[1].Offset = 1;
                MaskBrush2.GradientStops[2].Offset = 1;
            }
        }





        private void IndexButton_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            // 判断条件
            if (DataBridge.DataBridge.SelectedFunction == 1)
            {
                // 条件不满足时，取消右键菜单弹出
                e.Handled = true;
            }
        }
    }
}
