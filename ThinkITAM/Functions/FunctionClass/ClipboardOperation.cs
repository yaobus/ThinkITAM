using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ThinkITAM.Functions.FunctionClass
{
    public class ClipboardOperation
    {

        public static bool TrySetClipboardData(object data, int retryCount = 5, int delayMs = 100)
        {
            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    Clipboard.SetDataObject(data, true);
                    return true;
                }
                catch (System.Runtime.InteropServices.COMException ex) when (ex.HResult == unchecked((int)0x800401D0))
                {
                    // CLIPBRD_E_CANT_OPEN，剪贴板被占用
                    Thread.Sleep(delayMs); // 等待一段时间再试
                }
                catch (Exception ex)
                {

                    return false;
                }
            }


            return false;
        }
    }
}
