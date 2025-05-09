using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkITAM.ViewModels.LinkManage;
public class RackConfigClass
{
    public int SlotIndex { get; set; }

    public string SlotName { get; set; }

    public string SlotType { get; set; }


    public int PortCount
    {
        get; set;
    }

}
