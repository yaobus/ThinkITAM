namespace ThinkITAM.ViewModels.LinkManage
{
    using System.ComponentModel;

    public class PortLinkClass : INotifyPropertyChanged
    {
        private PortLinkLocationInfo _portLinkLocationInfo;
        private MdfRackClass _mdfRackClass;
        private SlotClass _slotClass;
        private PortClass _portClass;


        /// <summary>
        /// 位置信息
        /// </summary>
        public PortLinkLocationInfo PortLinkLocationInfo
        {
            get => _portLinkLocationInfo;
            set
            {
                if (_portLinkLocationInfo != value)
                {
                    _portLinkLocationInfo = value;
                    OnPropertyChanged(nameof(PortLinkLocationInfo));
                }
            }
        }

        /// <summary>
        /// 机架信息
        /// </summary>
        public MdfRackClass MdfRackClass
        {
            get => _mdfRackClass;
            set
            {
                if (_mdfRackClass != value)
                {
                    _mdfRackClass = value;
                    OnPropertyChanged(nameof(MdfRackClass));
                }
            }
        }

        /// <summary>
        /// 槽位信息
        /// </summary>
        public SlotClass SlotClass
        {
            get => _slotClass;
            set
            {
                if (_slotClass != value)
                {
                    _slotClass = value;
                    OnPropertyChanged(nameof(SlotClass));
                }
            }
        }

        /// <summary>
        /// 端口信息
        /// </summary>
        public PortClass PortClass
        {
            get => _portClass;
            set
            {
                if (_portClass != value)
                {
                    _portClass = value;
                    OnPropertyChanged(nameof(PortClass));
                }
            }
        }




        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }



    public class PortLinkLocationInfo
    {
        public static string Location
        {
            get; set;
        }

        public static string DeviceRoom
        {
            get; set;
        }

        public static string Cabinet
        {
            get; set;
        }
    }
}
