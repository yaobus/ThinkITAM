using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ThinkITAM.Functions.FunctionClass
{
    public class NumberCheckClass
    {

        public static int  OddOrEven(int number)
        {
            if (number % 2 == 0)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }


    }
}
