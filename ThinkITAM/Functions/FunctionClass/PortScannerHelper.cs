using System.Text.RegularExpressions;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using ThinkITAM.UserControls.General;

namespace ThinkITAM.Functions.FunctionClass
{
    public class PortScannerHelper
    {
        private const int MinPort = 1;
        private const int MaxPort = 65535;

        public static List<int> ParsePortRanges(string input)
        {
            var ports = new HashSet<int>(); // 使用HashSet避免重复端口


            var newInput = input.Replace("，", ",");

            // 先标准化输入：将空格替换成逗号，并且确保只有一个连续的逗号作为分隔符
            string normalizedInput = Regex.Replace(newInput, @"\s*,\s*|\s+", ",");

            // 分割输入字符串得到各个元素
            var elements = normalizedInput.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var element in elements)
            {
                var trimmedElement = element.Trim();

                if (trimmedElement.Contains("-"))
                {
                    // 处理范围格式，例如 "4000-4100"
                    var parts = trimmedElement.Split('-');
                    if (parts.Length == 2 && int.TryParse(parts[0], out int start) && int.TryParse(parts[1], out int end))
                    {
                        if (start <= end && IsPortInRange(start, end))
                        {
                            for (int i = start; i <= end; i++)
                            {
                                ports.Add(i);
                            }
                        }
                        else
                        {

                            //ShowMessageDialog("端口输入有误",$"无效范围或超出界限: {trimmedElement}。请确保范围在{MinPort}到{MaxPort}之间。");

                            return null;

                        }
                    }
                    else
                    {
                        return null;
                        //ShowMessageDialog("端口输入有误", $"无效的范围格式: {trimmedElement}");


                    }
                }
                else if (int.TryParse(trimmedElement, out int singlePort) && IsPortInRange(singlePort))
                {
                    // 单个端口
                    ports.Add(singlePort);
                }
                else
                {
                    return null;
                    //ShowMessageDialog("端口输入有误", $"无法解析或端口超出范围: {trimmedElement}");

                }
            }

            return ports.OrderBy(p => p).ToList(); // 返回排序后的列表
        }

        public static void ShowMessageDialog(string title, string message)
        {
            var dialog = new ConfirmationDialog
            {
                Title = title,
                Prompt = message,
                ConfirmButtonText = "确认",
                TitleColor = Brushes.AliceBlue,
                PromptColor = Brushes.AliceBlue

            };

            // 假设你的 DialogHost 在 XAML 中定义，并且 x:Name 设置为 "RootDialogHost"
            var dialogHost = DialogHost.GetDialogSession("MessageDialogHost"); // 或者直接引用你的 DialogHost, 如: this.RootDialogHost

            if (dialogHost != null)
            {
                // 如果 DialogHost 已经打开，则先关闭它
                dialogHost.Close();
                // 等待一小段时间以确保对话框已关闭，如果必要的话
                // await Task.Delay(100);  // 这可能不需要，视具体情况而定
            }


            // 显示对话框
            DialogHost.Show(dialog, "MessageDialogHost");
        }


        private static bool IsPortInRange(int port)
        {
            return port >= MinPort && port <= MaxPort;
        }

        private static bool IsPortInRange(int startPort, int endPort)
        {
            return startPort >= MinPort && endPort <= MaxPort;
        }

    }



}

