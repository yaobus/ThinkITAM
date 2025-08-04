using System.ComponentModel;
using System.Windows.Media;


namespace ThinkITAM.ViewModels.NetworkManage
{
    public class IpAddressInfoListViewMode : INotifyPropertyChanged
    {
        private bool isSelected;
        private int address;
        private string fullAddress;
        private bool addressType;
        private int addressStatus;
        private int addressColor;
        private Brush useStatusColor;
        private string pingTime;
        private Brush pingStatusColor;
        private string user;
        private string name;
        private string organization;
        private string department;
        private string group;
        private string unit;
        private string phone;
        private string hostName;
        private string macAddress;
        private Brush nowHostNameStatus;
        private string nowHostName;
        private Brush nowMacAddressStatus;
        private string nowMacAddress;
        private string linkDevice;
        private string linkDeviceId;
        private string linkDeviceAssetTag;
        private string linkDeviceAssetNumber;
        private string tagA;
        private string tagB;
        private string tagC;
        private string tagD;
        private string tagE;
        private string tagF;
        private string addressToolTip;
        private string tableName;

        /// <summary>
        /// 地址索引
        /// </summary>
        public int Index
        {
            get; set;
        }
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


        public int Address
        {
            get => address;
            set
            {
                if (address != value)
                {
                    address = value;
                    OnPropertyChanged(nameof(Address));
                }
            }
        }

        /// <summary>
        /// 完整地址
        /// </summary>
        public string FullAddress
        {
            get => fullAddress;
            set
            {
                if (fullAddress != value)
                {
                    fullAddress = value;
                    OnPropertyChanged(nameof(FullAddress));
                }
            }
        }


        public bool AddressType
        {
            get => addressType;
            set
            {
                if (addressType != value)
                {
                    addressType = value;
                    OnPropertyChanged(nameof(AddressType));
                }
            }
        }

        public int AddressStatus
        {
            get => addressStatus;
            set
            {
                if (addressStatus != value)
                {
                    addressStatus = value;
                    OnPropertyChanged(nameof(AddressStatus));
                }
            }
        }

        /// <summary>
        /// 端口颜色标签
        /// </summary>
        public int AddressColor
        {
            get => addressColor;
            set
            {
                if (addressColor != value)
                {
                    addressColor = value;
                    OnPropertyChanged(nameof(AddressColor));
                }

            }
        }


        public Brush UseStatusColor
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

        public string PingTime
        {
            get => pingTime;
            set
            {
                if (pingTime != value)
                {
                    pingTime = value;
                    OnPropertyChanged(nameof(PingTime));
                }
            }
        }

        public Brush PingStatusColor
        {
            get => pingStatusColor;
            set
            {
                if (pingStatusColor != value)
                {
                    pingStatusColor = value;
                    OnPropertyChanged(nameof(PingStatusColor));
                }
            }
        }

        public string User
        {
            get => user;
            set
            {
                if (user != value)
                {
                    user = value;
                    OnPropertyChanged(nameof(User));
                }
            }
        }

        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string Organization
        {
            get => organization;
            set
            {
                if (organization != value)
                {
                    organization = value;
                    OnPropertyChanged(nameof(Organization));
                }
            }
        }

        public string Department
        {
            get => department;
            set
            {
                if (department != value)
                {
                    department = value;
                    OnPropertyChanged(nameof(Department));
                }
            }
        }

        public string Group
        {
            get => group;
            set
            {
                if (group != value)
                {
                    group = value;
                    OnPropertyChanged(nameof(Group));
                }
            }
        }

        public string Unit
        {
            get => unit;
            set
            {
                if (unit != value)
                {
                    unit = value;
                    OnPropertyChanged(nameof(Unit));
                }
            }
        }

        public string Phone
        {
            get => phone;
            set
            {
                if (phone != value)
                {
                    phone = value;
                    OnPropertyChanged(nameof(Phone));
                }
            }
        }

        public string HostName
        {
            get => hostName;
            set
            {
                if (hostName != value)
                {
                    hostName = value;
                    OnPropertyChanged(nameof(HostName));
                }
            }
        }

        public string MacAddress
        {
            get => macAddress;
            set
            {
                if (macAddress != value)
                {
                    macAddress = value;
                    OnPropertyChanged(nameof(MacAddress));
                }
            }
        }

        public Brush NowHostNameStatus
        {
            get => nowHostNameStatus;
            set
            {
                if (nowHostNameStatus != value)
                {
                    nowHostNameStatus = value;
                    OnPropertyChanged(nameof(NowHostNameStatus));
                }
            }
        }

        public string NowHostName
        {
            get => nowHostName;
            set
            {
                if (nowHostName != value)
                {
                    nowHostName = value;
                    OnPropertyChanged(nameof(NowHostName));
                }
            }
        }

        public Brush NowMacAddressStatus
        {
            get => nowMacAddressStatus;
            set
            {
                if (nowMacAddressStatus != value)
                {
                    nowMacAddressStatus = value;
                    OnPropertyChanged(nameof(NowMacAddressStatus));
                }
            }
        }

        public string NowMacAddress
        {
            get => nowMacAddress;
            set
            {
                if (nowMacAddress != value)
                {
                    nowMacAddress = value;
                    OnPropertyChanged(nameof(NowMacAddress));
                }
            }
        }

        public string LinkDevice
        {
            get => linkDevice;
            set
            {
                if (linkDevice != value)
                {
                    linkDevice = value;
                    OnPropertyChanged(nameof(LinkDevice));
                }
            }
        }


        public string LinkDeviceId
        {
            get => linkDeviceId;
            set
            {
                if (linkDeviceId != value)
                {
                    linkDeviceId = value;
                    OnPropertyChanged(nameof(LinkDeviceId));
                }
            }
        }


        public string LinkDeviceAssetTag
        {
            get => linkDeviceAssetTag;
            set
            {
                if (linkDeviceAssetTag != value)
                {
                    linkDeviceAssetTag = value;
                    OnPropertyChanged(nameof(LinkDeviceAssetTag));
                }
            }
        }

        public string LinkDeviceAssetNumber
        {
            get => linkDeviceAssetNumber;
            set
            {
                if (linkDeviceAssetNumber != value)
                {
                    linkDeviceAssetNumber = value;
                    OnPropertyChanged(nameof(LinkDeviceAssetNumber));
                }
            }
        }

        public string TagA
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

        public string TagB
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

        public string TagC
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

        public string TagD
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

        public string TagE
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

        public string TagF
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

        public string AddressToolTip
        {
            get => addressToolTip;
            set
            {
                if (addressToolTip != value)
                {
                    addressToolTip = value;
                    OnPropertyChanged(nameof(AddressToolTip));
                }
            }
        }

        public string TableName
        {
            get => tableName;
            set
            {
                if (tableName != value)
                {
                    tableName = value;
                    OnPropertyChanged(nameof(TableName));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }


    public class IpAddressInfoListExportViewModel : INotifyPropertyChanged
    {
        private bool isSelected;
        private string address;
        private bool addressType;
        private int addressStatus;
        private int addressColor;
        private Brush useStatusColor;
        private string pingTime;
        private Brush pingStatusColor;
        private string user;
        private string name;
        private string organization;
        private string department;
        private string group;
        private string unit;
        private string phone;
        private string hostName;
        private string macAddress;
        private Brush nowHostNameStatus;
        private string nowHostName;
        private Brush nowMacAddressStatus;
        private string nowMacAddress;
        private string linkDevice;
        private string linkDeviceId;
        private string linkDeviceAssetTag;
        private string linkDeviceAssetNumber;
        private string tagA;
        private string tagB;
        private string tagC;
        private string tagD;
        private string tagE;
        private string tagF;
        private string addressToolTip;


        /// <summary>
        /// 地址索引
        /// </summary>
        public int Index
        {
            get; set;
        }
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


        public string Address
        {
            get => address;
            set
            {
                if (address != value)
                {
                    address = value;
                    OnPropertyChanged(nameof(Address));
                }
            }
        }



        public bool AddressType
        {
            get => addressType;
            set
            {
                if (addressType != value)
                {
                    addressType = value;
                    OnPropertyChanged(nameof(AddressType));
                }
            }
        }

        public int AddressStatus
        {
            get => addressStatus;
            set
            {
                if (addressStatus != value)
                {
                    addressStatus = value;
                    OnPropertyChanged(nameof(AddressStatus));
                }
            }
        }

        /// <summary>
        /// 端口颜色标签
        /// </summary>
        public int AddressColor
        {
            get => addressColor;
            set
            {
                if (addressColor != value)
                {
                    addressColor = value;
                    OnPropertyChanged(nameof(AddressColor));
                }

            }
        }


        public Brush UseStatusColor
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

        public string PingTime
        {
            get => pingTime;
            set
            {
                if (pingTime != value)
                {
                    pingTime = value;
                    OnPropertyChanged(nameof(PingTime));
                }
            }
        }

        public Brush PingStatusColor
        {
            get => pingStatusColor;
            set
            {
                if (pingStatusColor != value)
                {
                    pingStatusColor = value;
                    OnPropertyChanged(nameof(PingStatusColor));
                }
            }
        }

        public string User
        {
            get => user;
            set
            {
                if (user != value)
                {
                    user = value;
                    OnPropertyChanged(nameof(User));
                }
            }
        }

        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string Organization
        {
            get => organization;
            set
            {
                if (organization != value)
                {
                    organization = value;
                    OnPropertyChanged(nameof(Organization));
                }
            }
        }

        public string Department
        {
            get => department;
            set
            {
                if (department != value)
                {
                    department = value;
                    OnPropertyChanged(nameof(Department));
                }
            }
        }

        public string Group
        {
            get => group;
            set
            {
                if (group != value)
                {
                    group = value;
                    OnPropertyChanged(nameof(Group));
                }
            }
        }

        public string Unit
        {
            get => unit;
            set
            {
                if (unit != value)
                {
                    unit = value;
                    OnPropertyChanged(nameof(Unit));
                }
            }
        }

        public string Phone
        {
            get => phone;
            set
            {
                if (phone != value)
                {
                    phone = value;
                    OnPropertyChanged(nameof(Phone));
                }
            }
        }

        public string HostName
        {
            get => hostName;
            set
            {
                if (hostName != value)
                {
                    hostName = value;
                    OnPropertyChanged(nameof(HostName));
                }
            }
        }

        public string MacAddress
        {
            get => macAddress;
            set
            {
                if (macAddress != value)
                {
                    macAddress = value;
                    OnPropertyChanged(nameof(MacAddress));
                }
            }
        }

        public Brush NowHostNameStatus
        {
            get => nowHostNameStatus;
            set
            {
                if (nowHostNameStatus != value)
                {
                    nowHostNameStatus = value;
                    OnPropertyChanged(nameof(NowHostNameStatus));
                }
            }
        }

        public string NowHostName
        {
            get => nowHostName;
            set
            {
                if (nowHostName != value)
                {
                    nowHostName = value;
                    OnPropertyChanged(nameof(NowHostName));
                }
            }
        }

        public Brush NowMacAddressStatus
        {
            get => nowMacAddressStatus;
            set
            {
                if (nowMacAddressStatus != value)
                {
                    nowMacAddressStatus = value;
                    OnPropertyChanged(nameof(NowMacAddressStatus));
                }
            }
        }

        public string NowMacAddress
        {
            get => nowMacAddress;
            set
            {
                if (nowMacAddress != value)
                {
                    nowMacAddress = value;
                    OnPropertyChanged(nameof(NowMacAddress));
                }
            }
        }

        public string LinkDevice
        {
            get => linkDevice;
            set
            {
                if (linkDevice != value)
                {
                    linkDevice = value;
                    OnPropertyChanged(nameof(LinkDevice));
                }
            }
        }


        public string LinkDeviceId
        {
            get => linkDeviceId;
            set
            {
                if (linkDeviceId != value)
                {
                    linkDeviceId = value;
                    OnPropertyChanged(nameof(LinkDeviceId));
                }
            }
        }


        public string LinkDeviceAssetTag
        {
            get => linkDeviceAssetTag;
            set
            {
                if (linkDeviceAssetTag != value)
                {
                    linkDeviceAssetTag = value;
                    OnPropertyChanged(nameof(LinkDeviceAssetTag));
                }
            }
        }

        public string LinkDeviceAssetNumber
        {
            get => linkDeviceAssetNumber;
            set
            {
                if (linkDeviceAssetNumber != value)
                {
                    linkDeviceAssetNumber = value;
                    OnPropertyChanged(nameof(LinkDeviceAssetNumber));
                }
            }
        }

        public string TagA
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

        public string TagB
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

        public string TagC
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

        public string TagD
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

        public string TagE
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

        public string TagF
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

        public string AddressToolTip
        {
            get => addressToolTip;
            set
            {
                if (addressToolTip != value)
                {
                    addressToolTip = value;
                    OnPropertyChanged(nameof(AddressToolTip));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}


