using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkITAM.ViewModels.DatabaseEntity.PortPanel;
public class PortPanelEntityViewModel
{

    public int UID
    {
        get; set;
    }

    public string SlotId
    {
        get; set;
    }

    public string RoomId
    {
        get; set;
    }

    public string PortId
    {
        get; set;
    }

    public string PortType
    {
        get; set;
    }

    public string PortGroup
    {
        get; set;
    }

    public string PortColor{ get; set; }
    
    public string PortTag { get; set; }
    
    public string PortStatus { get; set; }

    public int OnTheLine
    {
        get;
        set;
    }

    public string TagA{ get; set; }
    public string TagB{ get; set; }
    public string TagC{ get; set; }
    public string TagD{ get; set; }
    public string TagE{ get; set; }
    public string TagF{ get; set; }


}
