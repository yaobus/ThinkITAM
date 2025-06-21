using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkITAM.ViewModels.NetworkManage
{
   public class SubNetworkInfoViewModel
    {

        //索引
        public float Index
        {
            get; set;
        }

        public string? TableName
        {
            get; set;
        }

        public string? Network
        {
            get; set;
        }

        public string? Netmask
        {
            get; set;
        }


        public string? Range
        {
            get; set;
        }

        //使用率
        public double Percentage
        {
            get; set;
        }

        //使用率
        public string Note
        {
            get; set;
        }
    }
}
