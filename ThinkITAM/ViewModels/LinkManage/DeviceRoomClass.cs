using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkITAM.ViewModels.LinkManage
{
    public class DeviceRoomClass
    {
        public int Index
        {
            get; set;
        }
        public string DeviceRoomQrId { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public string User { get; set; }

        public string UserPhone { get; set; }

        public string Note { get; set; }

    }
}
