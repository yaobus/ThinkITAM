using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ThinkITAM.Functions.FunctionClass
{
   public class OpenUrlClass
    {



        public static void OpenUrlInSpecificBrowser(string url, string browserPath)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = browserPath,
                    Arguments = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening URL: " + ex.Message);
            }
        }
    }
}
