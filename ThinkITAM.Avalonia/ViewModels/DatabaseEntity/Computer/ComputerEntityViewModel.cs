using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkITAM.ViewModels.DatabaseEntity.Computer
{
    public class ComputerEntityViewModel
    {
        public int UID { get; set; }
       public string DeviceId { get; set; }

       public string AssetId { get; set; }
       public string AssetUser { get; set; }
       public string PortId { get; set; }
       public string PortTag { get; set; }
       public string PortType { get; set; }

       public string PortStatus { get; set; }

       public int OnTheLine { get; set; }

       public int PortColor { get; set; }

        public string PortGroup { get; set; }

        public string BuildingId { get; set; }

        public string Floor { get; set; }

        public string Room { get; set; }

        public string LinkIp { get; set; }

        public string TagA { get; set; }

        public string TagB { get; set; }
        public string TagC { get; set; }
        public string TagD { get; set; }
        public string TagE { get; set; }
        public string TagF { get; set; }
    }
}
