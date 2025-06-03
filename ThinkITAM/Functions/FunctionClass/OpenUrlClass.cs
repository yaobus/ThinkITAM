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
                if (!string.IsNullOrWhiteSpace(browserPath))//有指定浏览器
                {
                    Functions.FunctionClass.OpenUrlClass.OpenUrlInSpecificBrowser(url, browserPath);
                }
                else
                {
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }



            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);

            }


            //try
            //{
            //    Process.Start(new ProcessStartInfo
            //    {
            //        FileName = browserPath,
            //        Arguments = url,
            //        UseShellExecute = true
            //    });
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Error opening URL: " + ex.Message);
            //}
        }
    }
}
