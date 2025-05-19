using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkITAM.ViewModels.LinkManage;

/// <summary>
/// 机架的基础信息
/// </summary>
public class RackInfo
{

    public int  Index { get; set; }

    public string rackId
    {
        get;
        set;
    }

    public string rackGroup { get; set; }



    public string rackName
    {
        get;
        set;
    }

    public string rackNote
    {
        get;
        set;
    }
    /// <summary>
    /// 槽位信息合集
    /// </summary>
    public ObservableCollection<SlotClass>  slotInfos { get; set; }


    public int slotCount
    {
        get;
        set;
    }

    public int portCount
    {
        get;
        set;
    }
}
