using Avalonia.Media;

namespace ThinkITAM.ViewModels.PortPanel
{
    public class PortPanelClass
    {

        public BuildingInfoClass Building
        {
            get; set;
        }

        public string Floor
        {
            get; set;
        }

        public string RoomNumber
        {
            get; set;
        }

        public string PortType
        {
            get; set;
        }

        public string PortId
        {
            get; set;
        }

        public string Group
        {
            get; set;
        }

        /// <summary>
        /// 端口颜色
        /// </summary>
        public IBrush? PortColorBrush
        {
            get;
            set;
        } = Brushes.Transparent;

        public string PortTag
        {
            get; set;
        }

        public string PortStatus
        {
            get; set;
        }



    }

    public class BuildingInfoClass
    {
        public int Index
        {
            get; set;
        }

        public string BuildingId
        {
            get; set;
        }

        public string Building
        {
            get; set;
        }

        public string? Address
        {
            get; set;
        }

        public string User
        {
            get; set;
        }

        public string Phone
        {
            get; set;
        }

        public string Note
        {
            get; set;
        }

        /// <summary>
        /// 楼层/房间/端口数量
        /// </summary>
        public string Count
        {
            get; set;
        }
    }

    public class FloorInfoClass
    {
        public int Index
        {
            get; set;
        }

        public string BuildingId
        {
            get; set;
        }

        public string Floor
        {
            get; set;
        }

        public int RoomCount
        {
            get; set;
        }
        public int PortCount
        {
            get; set;
        }
    }

    public class RoomClass
    {
        public int Index
        {
            get; set;
        }

        public string RoomNumber
        {
            get; set;
        }


        public string RoomNote
        {
            get; set;
        }

        public int PortCount
        {
            get; set;
        }
    }
}
