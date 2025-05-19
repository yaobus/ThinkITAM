
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;


namespace ThinkITAM.ViewModels.DevicePortManage;
public class PortTypeClass
{
    /// <summary>
    /// 设备端口预设信息数据类型
    /// </summary>
    public class PortInfoClass
    {
        /// <summary>
        /// 端口类型，电口E，光口F，管理口M，硬盘插槽D
        /// </summary>
        public string? PortType
        {
            get; set;
        }



        public string? PortSpeed
        {

            get; set;
        }

        //(E,G0/0/1)


        /// <summary>
        /// 端口标签，F,E,G,XG,25G,40G,100G
        /// </summary>
        public string? PortTag
        {

            get; set;
        }

        /// <summary>
        /// 槽位编号，0,1,2...
        /// </summary>
        public string? SlotNumber
        {
            get; set;
        }

        /// <summary>
        /// 端口前缀，例如/0/
        /// </summary>
        public string? PortPrefix
        {

            get; set;
        }

        /// <summary>
        /// 第一个编号
        /// </summary>
        public int? FirstNumber
        {

            get; set;
        }

        /// <summary>
        /// 端口总数
        /// </summary>
        public int? PortCount
        {

            get; set;
        }


        public override string ToString()
        {
            return $"Port: {PortType}, {PortSpeed}, {SlotNumber}, {PortPrefix}, {FirstNumber}, {PortCount}";
        }
    }

    /// <summary>
    /// 设备设计器预览状态下的端口信息
    /// </summary>
    public class PortDataClass
    {
        /// <summary>
        /// 端口类型，电口E，光口F，管理口M，硬盘插槽D
        /// </summary>
        public string? PortType
        {
            get; set;
        }

        /// <summary>
        /// 端口标签，F,E,G,XG,25G,40G,100G
        /// </summary>
        public string? PortTag
        {

            get; set;
        }

        /// <summary>
        /// 端口标签，0/0/1
        /// </summary>
        public string? FullPortId
        {

            get; set;
        }


        /// <summary>
        /// 使用状态颜色
        /// </summary>
        public Brush? UseStatusColor
        {
            get; set;
        }

        /// <summary>
        /// 用户自定义颜色索引
        /// </summary>
        public PackIconKind IconKind
        {

            get; set;
        }


        /// <summary>
        /// 图标
        /// </summary>
        public int? CustomColor
        {

            get; set;
        }

        public override string ToString()
        {
            return $"Port: {PortType}, {PortTag}, {FullPortId}, {UseStatusColor}, {CustomColor}";
        }
    }

    /// <summary>
    /// 设备端口详细信息表的端口信息
    /// </summary>

    public class PortDetailedInfo : INotifyPropertyChanged
    {
        private int uId;
        public int UID
        {
            get => uId;
            set
            {
                if (uId != value)
                {
                    uId = value;
                    OnPropertyChanged(nameof(UID));
                }
            }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }


        private string? portType;
        public string? PortType
        {
            get => portType;
            set
            {
                if (portType != value)
                {
                    portType = value;
                    OnPropertyChanged(nameof(PortType));
                }
            }
        }

        private string? portTag;
        public string? PortTag
        {
            get => portTag;
            set
            {
                if (portTag != value)
                {
                    portTag = value;
                    OnPropertyChanged(nameof(PortTag));
                }
            }
        }


        private string? portSpeed;
        public string? PortSpeed
        {
            get => portSpeed;
            set
            {
                if (portSpeed != value)
                {
                    portSpeed = value;
                    OnPropertyChanged(nameof(PortSpeed));
                }
            }
        }


        private int? portSlotNumber;
        public int? PortSlotNumber
        {
            get => portSlotNumber;
            set
            {
                if (portSlotNumber != value)
                {
                    portSlotNumber = value;
                    OnPropertyChanged(nameof(PortSlotNumber));
                }
            }
        }

        private string? portId;
        public string? PortId
        {
            get => portId;
            set
            {
                if (portId != value)
                {
                    portId = value;
                    OnPropertyChanged(nameof(PortId));
                }
            }
        }

        private int? status;
        public int? Status
        {
            get => status;
            set
            {
                if (status != value)
                {
                    status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        private string? mode;
        public string? Mode
        {
            get => mode;
            set
            {
                if (mode != value)
                {
                    mode = value;
                    OnPropertyChanged(nameof(Mode));
                }
            }
        }

        private string? portName;
        public string? PortName
        {
            get => portName;
            set
            {
                if (portName != value)
                {
                    portName = value;
                    OnPropertyChanged(nameof(PortName));
                }
            }
        }

        private string? vlanId;
        public string? VlanId
        {
            get => vlanId;
            set
            {
                if (vlanId != value)
                {
                    vlanId = value;
                    OnPropertyChanged(nameof(VlanId));
                }
            }
        }

        private string? deviceLinkId;
        public string? DeviceLinkId
        {
            get => deviceLinkId;
            set 
            {
                if (deviceLinkId != value)
                {
                    deviceLinkId = value;
                    OnPropertyChanged(nameof(DeviceLinkId));
                }
            }
        }

        private int? onTheLine;
        public int? OnTheLine
        {
            get => onTheLine;
            set
            {
                if (onTheLine != value)
                {
                    onTheLine = value;
                    OnPropertyChanged(nameof(OnTheLine));
                }
            }
        }


        /// <summary>
        /// 自定义颜色的索引
        /// </summary>
        private int? portColor;

        public int? PortColor
        {
            get => portColor;
            set
            {
                if (portColor != value)
                {
                    portColor = value;
                    OnPropertyChanged(nameof(PortColor));
                }
            }

        }

        /// <summary>
        /// 自定义的颜色
        /// </summary>
        private Brush? customBrush;
        public Brush? CustomBrush
        {
            get => customBrush;
            set
            {
                if (customBrush != value)
                {
                    customBrush = value;
                    OnPropertyChanged(nameof(CustomBrush));
                }
            }
        }

        private Brush? useStatusColor;
        public Brush? UseStatusColor
        {
            get => useStatusColor;
            set
            {
                if (useStatusColor != value)
                {
                    useStatusColor = value;
                    OnPropertyChanged(nameof(UseStatusColor));
                }
            }
        }

        private string fullPortId;
        public string FullPortId
        {
            get => fullPortId;
            set
            {
                if (fullPortId != value)
                {
                    fullPortId = value;
                    OnPropertyChanged(nameof(FullPortId));
                }
            }
        }

        private string? tagA;
        public string? TagA
        {
            get => tagA;
            set
            {
                if (tagA != value)
                {
                    tagA = value;
                    OnPropertyChanged(nameof(TagA));
                }
            }
        }

        private string? tagB;
        public string? TagB
        {
            get => tagB;
            set
            {
                if (tagB != value)
                {
                    tagB = value;
                    OnPropertyChanged(nameof(TagB));
                }
            }
        }

        private string? tagC;
        public string? TagC
        {
            get => tagC;
            set
            {
                if (tagC != value)
                {
                    tagC = value;
                    OnPropertyChanged(nameof(TagC));
                }
            }
        }

        private string? tagD;
        public string? TagD
        {
            get => tagD;
            set
            {
                if (tagD != value)
                {
                    tagD = value;
                    OnPropertyChanged(nameof(TagD));
                }
            }
        }

        private string? tagE;
        public string? TagE
        {
            get => tagE;
            set
            {
                if (tagE != value)
                {
                    tagE = value;
                    OnPropertyChanged(nameof(TagE));
                }
            }
        }

        private string? tagF;
        public string? TagF
        {
            get => tagF;
            set
            {
                if (tagF != value)
                {
                    tagF = value;
                    OnPropertyChanged(nameof(TagF));
                }
            }
        }

        private string? assetId;
        public string? AssetId
        {
            get => assetId;
            set
            {
                if (assetId != value)
                {
                    assetId = value;
                    OnPropertyChanged(nameof(AssetId));
                }
            }
        }

        private PackIconKind iconKind;

        public PackIconKind IconKind
        {
            get => iconKind;
            set
            {
                if (iconKind != value)
                {
                    iconKind = value;
                    OnPropertyChanged(nameof(IconKind));
                }
            }
        }


        private string toolTip;

        public string ToolTip
        {
            get => toolTip;
            set
            {
                if (toolTip != value)
                {
                    toolTip = value;
                    OnPropertyChanged(nameof(toolTip));
                }
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (PortDetailedInfo)obj;
            return Equals(PortType, other.PortType) &&
                   Equals(PortTag, other.PortTag) &&
                   Equals(PortSlotNumber, other.PortSlotNumber) &&
                   Equals(PortId, other.PortId) &&
                   Equals(Status, other.Status) &&
                   Equals(Mode, other.Mode) &&
                   Equals(PortName, other.PortName) &&
                   Equals(VlanId, other.VlanId) &&
                   Equals(DeviceLinkId, other.DeviceLinkId) &&
                   Equals(OnTheLine, other.onTheLine) &&
                   Equals(PortColor, other.PortColor) &&
                   Equals(UseStatusColor, other.UseStatusColor) &&
                   Equals(FullPortId, other.FullPortId) &&
                   Equals(TagA, other.TagA) &&
                   Equals(TagB, other.TagB) &&
                   Equals(TagC, other.TagC) &&
                   Equals(TagD, other.TagD) &&
                   Equals(TagE, other.TagE) &&
                   Equals(TagF, other.TagF) &&
                   Equals(ToolTip, other.ToolTip) &&
                   Equals(AssetId, other.AssetId);
        }

        //public override int GetHashCode()
        //{
        //    return HashCode.Combine(
        //        PortType, PortTag, PortSlotNumber, PortId, Status, Mode,
        //        PortName, VlanId, DeviceLink, LineLink, CustomColor, UseStatusColor,
        //        FullPortId, TagA, TagB, TagC, TagD, TagE, TagF
        //    );
        //}

        public static bool operator ==(PortDetailedInfo left, PortDetailedInfo right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(PortDetailedInfo left, PortDetailedInfo right)
        {
            return !(left == right);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }



}


