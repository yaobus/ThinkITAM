using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GongSolutions.Wpf.DragDrop;

namespace ThinkITAM.Functions.FunctionClass
{
    public class OpenUrlClass
    {

        public static void OpenUrlInSpecificBrowser(string url, string browserPath)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(browserPath))
                {
                    // 直接调用 Process 打开指定浏览器
                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = browserPath,
                        Arguments = url,
                        UseShellExecute = false, // 推荐为 false 当指定了具体路径
                        CreateNoWindow = true
                    });
                }
                else
                {
                    // 使用系统默认浏览器
                    Process.Start(new ProcessStartInfo(url)
                    {
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening URL: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

    }
}
