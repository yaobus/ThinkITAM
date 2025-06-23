using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkITAM.ViewModels.AssetManage;
using ThinkITAM.ViewModels.LinkManage;
using ThinkITAM.ViewModels.NetworkManage;
using ThinkITAM.ViewModels.Preset;
using static ThinkITAM.ViewModels.DevicePortManage.PortTypeClass;
using PortClass = ThinkITAM.ViewModels.LinkManage.PortClass;

namespace ThinkITAM.DataBridge
{
    class DataBridge
    {

        /// <summary>
        /// 版本号
        /// </summary>
        public static string Version = "1.0.16";

        /// <summary>
        /// 更新次数，用于数据库字段升级
        /// </summary>
        public static int VersionNumber = 16;

        /// <summary>
        /// 当前所选网段名称
        /// </summary>
        public static string NetworkTableName = null;


        /// <summary>
        /// 当前所选网段地址，用于地址分配
        /// </summary>
        public static string SelectNetwork = null;

        /// <summary>
        /// 当前选中的网段信息
        /// </summary>
        public static NetworkInfoViewMode SelectNetworkInfo = new NetworkInfoViewMode();


        /// <summary>
        /// 当前所选网段的全部ip地址信息列表
        /// </summary>
        public static ObservableCollection<IpAddressInfoListViewMode> IpAddressInfoLists = new ObservableCollection<IpAddressInfoListViewMode>();

        /// <summary>
        /// 当前加载的的网段
        /// </summary>
        /// </summary>
        public static string LoadedNetworkSegment;

        /// <summary>
        /// 修改了IP地址颜色列表
        /// </summary>
        public static ObservableCollection<string> ModifyIpColorList = new ObservableCollection<string>();



        /// <summary>
        /// 当前选中网段的自定义标签
        /// </summary>
        public static dynamic SelectNetworkTags;

        /// <summary>
        /// 当前是否是日志查看模式,0为IP分配没啥，1为日志查看模式
        /// </summary>
        public static int LogMode = 0;


        /// <summary>
        /// 端口分配页面当前要关联的自定义资产编号
        /// </summary>
        public static string LinkSelectAssetId;

        /// <summary>
        /// 端口分配页面当前要关联的资产全局唯一ID
        /// </summary>
        public static string LinkAssetId;

        /// <summary>
        /// 操作类型，0为单个分配，1为批量分配，2为浏览器访问，3为PING测试，4为端口检测模式
        /// </summary>
        public static int OperationType = 0;

        /// <summary>
        /// 打开地址的浏览器
        /// </summary>
        public static string SelectBrowser;


        /// <summary>
        /// 当前被批量选中的地址
        /// </summary>
        public static ObservableCollection<int> SelectAddress = new ObservableCollection<int>();

        /// <summary>
        /// 批量选择的第一个地址类型，1为未分配的地址，2为已分配的地址,3为已关联的地址
        /// </summary>
        public static int AddressStatus ;

        /// <summary>
        /// 当前选择的端口
        /// </summary>
        public static string SelectPort;

        /// <summary>
        /// 当前选择的协议头
        /// </summary>
        public static string Protocol = "http://";

        /// <summary>
        /// 当前选择的人员信息
        /// </summary>
        public static PeopleViewModel SelectPeopleViewModel= new PeopleViewModel();

        //---------------------资产模块--------------------

        /// <summary>
        /// 当前所选资产类型
        /// </summary>
        public static AssetTypeViewModel SelectAssetTypeViewmodel = null;



        //---------------------设备端口管理模块--------------------

        /// <summary>
        /// 从数据库读取的端口详细信息列表
        /// </summary>
        public static ObservableCollection<PortDetailedInfo> PortDetailedInfos = new ObservableCollection<PortDetailedInfo>();


        /// <summary>
        /// 新增设备端口页面查找后选择的设备信息
        /// </summary>
        public static AssetViewModel SelectAssetInfo = null;

        /// <summary>
        /// 当前选中的设备表信息
        /// </summary>
        public static DeviceTypeViewModel SelectDeviceTableInfo;


        /// <summary>
        /// 操作类型，0为单个分配，1为批量分配
        /// </summary>
        public static int PortOperationType = 0;



        /// <summary>
        /// 当前选择的端口模式，0为未分配，1为已分配未启用，2为已分配，已启用，3为故障
        /// </summary>
        public static int? SelectPortMode;


        /// <summary>
        /// 当前选择的端口类型，E为以太网口，F为光纤口，D为硬盘，M为管理口
        /// </summary>
        public static string? SelectPortType;

        /// <summary>
        /// 多选模式下被批量选择的端口
        /// </summary>
        public static ObservableCollection<string> PortSelectCount = new ObservableCollection<string>();


        /// <summary>
        /// 是否创建了设备
        /// </summary>
        public static int CreatedDevice = 0;


        //-------------------------LinkManage----------------------------

        /// <summary>
        /// 0为端口链路预览，1为端口链路管理
        /// </summary>
        public static int LinkManageMode = 0;




        /// <summary>
        /// 添加了机架
        /// </summary>
        public static ObservableCollection<string> modifyRacks = new ObservableCollection<string>();

        /// <summary>
        /// 临时链路清除模式，0为不清除
        /// </summary>
        public static int LinkTempClear = 0;

        /// <summary>
        /// 当前选中的机架信息
        /// </summary>
        public static RackInfo SelectRackInfo;

        /// <summary>
        /// 选中准备用于链路的端口
        /// </summary>
        public static ObservableCollection<PortClass> SelectPortsToLink = new ObservableCollection<PortClass>();

        /// <summary>
        /// 当前选中的端口，用于顺藤摸瓜
        /// </summary>
        public static ObservableCollection<PortLinkClass> LinkManageSelectPorts = new ObservableCollection<PortLinkClass>();


        /// <summary>
        /// 后台删除了链路
        /// </summary>
        public static ObservableCollection<int> ChangedLink = new ObservableCollection<int>();


        /// <summary>
        /// 当前选中的机架ID
        /// </summary>
        public static ObservableCollection<string> SelectRackId = new ObservableCollection<string>();

        /// <summary>
        /// 当前需要进行更新的机架
        /// </summary>
        public static ObservableCollection<string> SelectUpdateRackId = new ObservableCollection<string>();



        /// <summary>
        /// 当前选中的机架槽位信息
        /// </summary>
        public static SlotClass SelectSlotInfo;

        /// <summary>
        /// 当前选中的机架槽位端口信息
        /// </summary>
        public static PortClass SelectPortInfo;

        /// <summary>
        /// 发生修改的标签列表
        /// </summary>
        public static ObservableCollection<string> ModifyTagList = new ObservableCollection<string>();




        /// <summary>
        /// 链路管理页面添加的链路节点列表
        /// </summary>
        public static ObservableCollection<PortLinkClass> LinkManageList = new ObservableCollection<PortLinkClass>();

        /// <summary>
        /// 链路展示节点列表
        /// </summary>
        public static ObservableCollection<PortLinkClass> LinkViewList = new ObservableCollection<PortLinkClass>();

        /// <summary>
        /// 配线架上当前选中的链路节点信息
        /// </summary>
        public static PortClass RackSelectPortInfo;

        /// <summary>
        /// 墙面面板端口管理端口列表
        /// </summary>
        public static ObservableCollection<PortLinkClass> PanelPorts = new ObservableCollection<PortLinkClass>();

        /// <summary>
        /// 当前选中的建筑ID/RackId
        /// </summary>
        public static string SelectedBuildingId { get; set; }

        /// <summary>
        /// 当前选中的楼层ID/SlotId
        /// </summary>
        public static string SelectedSlotId { get; set; }

        //-------------------------PortPanel----------------------------
        /// <summary>
        /// 当前选中的建筑ID
        /// </summary>
        public static string SelectBuildingId;

        /// <summary>
        /// 当前选中的楼层
        /// </summary>
        public static string SelectFloor;

        /// <summary>
        /// 当前选中的房间
        /// </summary>
        public static string SelectRoom;

        /// <summary>
        /// 发生修改的PortPanel标签列表
        /// </summary>
        public static ObservableCollection<string> PortPanelModifyTagList = new ObservableCollection<string>();

        /// <summary>
        /// 链路节点展示列表
        /// </summary>
        public static ObservableCollection<PortLinkClass> PortPanelLinkViewList = new ObservableCollection<PortLinkClass>();

        /// <summary>
        /// 删除房间端口
        /// </summary>
        public static ObservableCollection<string> modifyPorts = new ObservableCollection<string>();

        //-------------------------IndexPage----------------------------
        /// <summary>
        /// 发生修改的IndexPage标签列表
        /// </summary>
        public static ObservableCollection<string> modifyIndexTags= new ObservableCollection<string>();


        //-------------------------Computer----------------------------
        /// <summary>
        /// 发生修改的部署设备列表
        /// </summary>
        public static ObservableCollection<string> modifyDeployDevices = new ObservableCollection<string>();

    }
}
