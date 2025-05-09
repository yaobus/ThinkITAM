using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace ThinkITAM.ViewModels.LinkManage
{


/// <summary>
/// 配线机架信息
/// </summary>
public class MdfRackClass : INotifyPropertyChanged
{
    private string _rackId;
    private string _rackName;//机架名称或房间号
    private string _rackGroup;
    private string _rackNote;
    private int _slotCount;
    private string _roomName;//机房名称或建筑名称
    private string _cabinetName;//机柜名称或楼层
    
    private ObservableCollection<SlotClass> _slots;

    /// <summary>
    /// Rack ID
    /// </summary>
    public string RackId
    {
        get => _rackId;
        set
        {
            if (_rackId != value)
            {
                _rackId = value;
                OnPropertyChanged(nameof(RackId));
            }
        }
    }

    /// <summary>
    /// Rack名称
    /// </summary>
    public string RackName
    {
        get => _rackName;
        set
        {
            if (_rackName != value)
            {
                _rackName = value;
                OnPropertyChanged(nameof(RackName));
            }
        }
    }

    public string RackGroup
    {
        get => _rackGroup;
        set
        {
            if (_rackGroup != value)
            {
                _rackGroup = value;
                OnPropertyChanged(nameof(RackGroup));
            }
        }
    }

    /// <summary>
    /// Rack备注
    /// </summary>
    public string RackNote
    {
        get => _rackNote;
        set
        {
            if (_rackNote != value)
            {
                _rackNote = value;
                OnPropertyChanged(nameof(RackNote));
            }
        }
    }

    /// <summary>
    /// 槽位总数
    /// </summary>
    public int SlotCount
    {
        get => _slotCount;
        set
        {
            if (_slotCount != value)
            {
                _slotCount = value;
                OnPropertyChanged(nameof(SlotCount));
            }
        }
    }


        public string RoomName
    {
        get => _roomName;
        set
        {
            if (_roomName != value)
            {
                _roomName = value;
                OnPropertyChanged(nameof(RoomName));
            }
        }
    }

        public string CabinetName
    {
        get => _cabinetName;
        set
        {
            if (_cabinetName != value)
            {
                _cabinetName = value;
                OnPropertyChanged(nameof(CabinetName));
            }
        }
    }


    /// <summary>
    /// 槽位群组
    /// </summary>
    public ObservableCollection<SlotClass> Slots
    {
        get => _slots;
        set
        {
            if (_slots != value)
            {
                _slots = value;
                OnPropertyChanged(nameof(Slots));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

/// <summary>
/// 槽位信息
/// </summary>
public class SlotClass : INotifyPropertyChanged
{
    private string _slotIndex;
    private string _slotName;
    private string _slotTag;
    private string _slotType;
    private int _portCount;
    private ObservableCollection<PortClass> _ports;

    /// <summary>
    /// 槽位编号
    /// </summary>
    public string SlotIndex
    {
        get => _slotIndex;
        set
        {
            if (_slotIndex != value)
            {
                _slotIndex = value;
                OnPropertyChanged(nameof(SlotIndex));
            }
        }
    }

    /// <summary>
    /// 槽位名称
    /// </summary>
    public string SlotName
    {
        get => _slotName;
        set
        {
            if (_slotName != value)
            {
                _slotName = value;
                OnPropertyChanged(nameof(SlotName));
            }
        }
    }

    /// <summary>
    /// 槽位标签
    /// </summary>
    public string SlotTag
    {
        get => _slotTag;
        set
        {
            if (_slotTag != value)
            {
                _slotTag = value;
                OnPropertyChanged(nameof(SlotTag));
            }
        }
    }

    /// <summary>
    /// 槽位类型，光纤SC,FC,LC
    /// </summary>
    public string SlotType
    {
        get => _slotType;
        set
        {
            if (_slotType != value)
            {
                _slotType = value;
                OnPropertyChanged(nameof(SlotType));
            }
        }
    }

    /// <summary>
    /// 该槽位端口数量
    /// </summary>
    public int PortCount
    {
        get => _portCount;
        set
        {
            if (_portCount != value)
            {
                _portCount = value;
                OnPropertyChanged(nameof(PortCount));
            }
        }
    }

    /// <summary>
    /// 端口组
    /// </summary>
    public ObservableCollection<PortClass> Ports
    {
        get => _ports;
        set
        {
            if (_ports != value)
            {
                _ports = value;
                OnPropertyChanged(nameof(Ports));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

/// <summary>
/// 端口信息
/// </summary>
public class PortClass : INotifyPropertyChanged
{
    private int _uID;
    private bool _isSelected;
    private string _rackId;
    private string _slotIndex;
    private string _room;
    private string _portIndex;
    private string _portTag;
    private string _portType;
    private int _portColor;
    private Brush _portTagBrush;
    private string _portStatus;
    private int _onTheLine;
    private int _nodeIndex;


        /// <summary>
        /// 端口UID号
        /// </summary>
        public int UID
    {
        get => _uID;
        set
        {
            if (_uID != value)
            {
                _uID = value;
                OnPropertyChanged(nameof(UID));
            }
        }
    }


        public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }
    }

    /// <summary>
    /// 端口所在机架号
    /// </summary>
    public string RackId
    {
        get => _rackId;
        set
        {
            if (_rackId != value)
            {
                _rackId = value;
                OnPropertyChanged(nameof(RackId));
            }
        }
    }

    /// <summary>
    /// 端口所在槽位号
    /// </summary>
    public string SlotIndex
    {
        get => _slotIndex;
        set
        {
            if (_slotIndex != value)
            {
                _slotIndex = value;
                OnPropertyChanged(nameof(SlotIndex));
            }
        }
    }

    /// <summary>
    /// 该端口所在的房间号(适用于房间内面板上的端口)
    /// </summary>
    public string Room
    {
        get => _room;
        set
        {
            if (_room != value)
            {
                _room = value;
                OnPropertyChanged(nameof(Room));
            }
        }
    }

    /// <summary>
    /// 端口编号
    /// </summary>
    public string PortIndex
    {
        get => _portIndex;
        set
        {
            if (_portIndex != value)
            {
                _portIndex = value;
                OnPropertyChanged(nameof(PortIndex));
            }
        }
    }

    /// <summary>
    /// 端口标签
    /// </summary>
    public string PortTag
    {
        get => _portTag;
        set
        {
            if (_portTag != value)
            {
                _portTag = value;
                OnPropertyChanged(nameof(PortTag));
            }
        }
    }

    /// <summary>
    /// 端口类型
    /// </summary>
    public string PortType
    {
        get => _portType;
        set
        {
            if (_portType != value)
            {
                _portType = value;
                OnPropertyChanged(nameof(PortType));
            }
        }
    }

    /// <summary>
    /// 端口颜色
    /// </summary>
    public int PortColor
    {
        get => _portColor;
        set
        {
            if (_portColor != value)
            {
                _portColor = value;
                OnPropertyChanged(nameof(PortColor));
            }
        }
    }

    /// <summary>
    /// 端口颜色
    /// </summary>
    public Brush PortTagBrush
    {
        get => _portTagBrush;
        set
        {
            if (_portTagBrush != value)
            {
                _portTagBrush = value;
                OnPropertyChanged(nameof(PortTagBrush));
            }
        }
    }

    /// <summary>
    /// 端口状态
    /// </summary>
    public string PortStatus
    {
        get => _portStatus;
        set
        {
            if (_portStatus != value)
            {
                _portStatus = value;
                OnPropertyChanged(nameof(PortStatus));
            }
        }
    }

    /// <summary>
    /// 该端口的永久链路信息
    /// </summary>
    public int OnTheLine
    {
        get => _onTheLine;
        set
        {
            if (_onTheLine != value)
            {
                _onTheLine = value;
                OnPropertyChanged(nameof(OnTheLine));
            }
        }
    }

    /// <summary>
    /// 节点索引
    /// </summary>
    public int NodeIndex
    {
        get => _nodeIndex;
        set
        {
            if (_nodeIndex != value)
            {
                _nodeIndex = value;
                OnPropertyChanged(nameof(NodeIndex));
            }
        }
    }


        public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}







    /// <summary>
    /// 永久链路信息
    /// </summary>
    public class PermanentLink 
    {

        /// <summary>
        /// 该端口所连接的配线架机架号
        /// </summary>
        public string RackID { get; set; }

        /// <summary>
        /// 该端口所连接的配线架槽位号
        /// </summary>
        public string SlotIndex { get; set; }

        /// <summary>
        /// 该端口所在的房间号(适用于房间内面板上的端口)
        /// </summary>
        public string Room { get; set; }

        /// <summary>
        /// 该端口所连接的配线架端口号
        /// </summary>
        public string PortIndex { get; set; }

        /// <summary>
        /// 该永久链路对端设备类型,0:配线架，1:终端盒，2:面板，3:网线，4:设备
        /// </summary>
        public int? Type { get; set; }=null;
    }



}
