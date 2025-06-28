using System.Windows.Media;

namespace ThinkITAM.ViewModels.LinkManage;
public class PortInfoClass
{
    public SolidColorBrush Color
    {
        get; set;
    }

    public string PortTag
    {
        get; set;
    }

    public string PortStatus
    {
        get; set;
    }
    /// <summary>
    /// 永久链路机架ID
    /// </summary>
    public string PermanentRackId
    {
        get; set;
    }
    /// <summary>
    /// 永久链路机架槽位
    /// </summary>
    public string PermanentSlot
    {
        get; set;
    }
    /// <summary>
    /// 永久链路机架端口
    /// </summary>
    public string PermanentPort
    {
        get; set;
    }


    /// <summary>
    /// 临时链路机架ID
    /// </summary>
    public string TempRackId
    {
        get; set;
    }
    /// <summary>
    /// 临时链路机架槽位
    /// </summary>
    public string TempSlot
    {
        get; set;
    }
    /// <summary>
    /// 临时链路机架端口
    /// </summary>
    public string TempPort
    {
        get; set;
    }
}
