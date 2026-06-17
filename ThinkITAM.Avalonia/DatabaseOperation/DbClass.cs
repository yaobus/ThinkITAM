using ThinkITAM.DataBridge;

namespace ThinkITAM.DatabaseOperation
{
    /// <summary>
    /// 数据库操作工具类
    /// 从 WPF 原项目 DbClass 迁移核心方法
    /// </summary>
    public class DbClass
    {
        /// <summary>
        /// 加载自定义窗口标签
        /// </summary>
        public static string LoadWindowTag(string windowName)
        {
            var sql = $"SELECT Tags FROM WindowTag WHERE WindowName = '{windowName}'";
            var rows = GlobalVariables.DbService.ExecuteQuery(sql);
            if (rows.Count > 0)
                return rows[0]["Tags"].ToString()!;
            else
                return string.Empty;
        }

        /// <summary>
        /// 加载自定义窗口标签（带默认名称和特殊名称）
        /// </summary>
        public static Dictionary<string, string>? LoadWindowTag(string defaultName, string? specialName)
        {
            var result = new Dictionary<string, string>();
            var name = defaultName;

            if (!string.IsNullOrWhiteSpace(specialName))
                name += specialName;

            var sqlTemp = $"SELECT COUNT(*) FROM WindowTag WHERE WindowName ='{name}'";
            var num = ExecuteScalarTableNum(sqlTemp);

            string? tags;
            if (num > 0)
            {
                tags = LoadWindowTag(name);
                result[name] = tags;
            }
            else
            {
                tags = LoadWindowTag(defaultName);
                result[defaultName] = tags;
            }

            if (tags != string.Empty)
                return result;
            else
                return null;
        }

        /// <summary>
        /// 查询表中的数据数量
        /// </summary>
        public static int ExecuteScalarTableNum(string sql)
        {
            return Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(sql));
        }

        /// <summary>
        /// 创建网段IP详表
        /// </summary>
        public static async void CreateNetworkTable(string tableName)
        {
            string sql = $"CREATE TABLE Net_{tableName} ( Address INTEGER, FullAddress TEXT, AddressStatus INTEGER, AddressColor TEXT, User TEXT, HostName TEXT, MacAddress TEXT, LinkDevice TEXT, TagA TEXT, TagB TEXT, TagC TEXT, TagD TEXT, TagE TEXT, TagF TEXT );";
            await GlobalVariables.DbService.CreateTableFromSqlAsync(sql);
        }

        /// <summary>
        /// 创建大型网段子表（256地址以上的网段）
        /// </summary>
        public static async Task CreateNetworkTableSub(string ipAddress, int netmask, string tableName)
        {
            var info = Shared.Network.SubnetCalculator.CalculateSubnets(ipAddress, netmask);
            int index = 0;

            foreach (var sub in info.Item2)
            {
                string newName = tableName + $"_Sub{index}";
                CreateNetworkTable(newName);
                index++;
            }
        }

        /// <summary>
        /// 保存设备端口预设
        /// </summary>
        public static void SaveModelPreset(string model, string presetJson)
        {
            string sql = $"SELECT COUNT(*) FROM ModelPreset WHERE Model = '{model}'";
            int rowCount = ExecuteScalarTableNum(sql);

            if (rowCount == 0)
            {
                var preset = new { model = model, preset = presetJson };
                GlobalVariables.DbService.InsertEntity("ModelPreset", preset);
            }
            else
            {
                string query = $"UPDATE ModelPreset SET Preset = '{presetJson}' WHERE Model = '{model}'";
                GlobalVariables.DbService.ExecuteNonQuery(query);
            }
        }

        /// <summary>
        /// 创建表（如果不存在）
        /// 按模型字段在SQLITE数据库创建表
        /// </summary>
        public static bool CreateTableIfNotExists(string tableName)
        {
            if (GlobalVariables.DbService.IsTableExists(tableName))
                return false;

            string sql = tableName switch
            {
                "Network" => "CREATE TABLE Network (NetworkId TEXT PRIMARY KEY, SortIndex INT, TableName TEXT, Name TEXT, Description TEXT, Network TEXT, Netmask TEXT, Parent TEXT, Child TEXT, TagA TEXT, TagB TEXT, TagC TEXT, TagD TEXT, TagE TEXT, TagF TEXT, Del INT);",
                "UserInfo" => "CREATE TABLE UserInfo (UserId TEXT PRIMARY KEY, Number INT, Name TEXT, Organization TEXT, Department TEXT, UserGroup TEXT, UserUnit TEXT, Phone TEXT, Note TEXT, Del INT);",
                "WindowTag" => "CREATE TABLE WindowTag (WindowName TEXT PRIMARY KEY, Tags TEXT);",
                "Hierarchy" => "CREATE TABLE Hierarchy (HierarchyId TEXT PRIMARY KEY, Name TEXT, ParentId TEXT, Level INT, SortOrder INT, Del INT);",
                "Organization" => "CREATE TABLE Organization (Organization TEXT, Department TEXT, UserGroup TEXT, UserUnit TEXT, Note TEXT, Del INT);",
                "AssetTag" => "CREATE TABLE AssetTag (AssetType TEXT, AssetTag TEXT, Autocomplete INT, Note TEXT, Del INT);",
                "Address" => "CREATE TABLE Address (AddressId TEXT PRIMARY KEY, NetworkId TEXT, Address TEXT, Status INT, Color INT, TagA TEXT, TagB TEXT, TagC TEXT, TagD TEXT, TagE TEXT, TagF TEXT, Del INT);",
                "Asset" => "CREATE TABLE Asset (AssetId TEXT PRIMARY KEY, AssetQrCode TEXT, AssetType TEXT, DeviceType TEXT, AssetTag TEXT, AssetNumber TEXT, PurchaseDate TEXT, PurchasePrice TEXT, Manufacturer TEXT, Model TEXT, SerialNumber TEXT, Configuration TEXT, Location TEXT, UserOrganization TEXT, UserDepartment TEXT, UserGroup TEXT, UserUnit TEXT, User TEXT, UserPhone TEXT, Consumer TEXT, AssetStatus TEXT, UsedYear TEXT, ScrapDate TEXT, Notes TEXT, Deploy TEXT, TagA TEXT, TagB TEXT, TagC TEXT, TagD TEXT, TagE TEXT, TagF TEXT, Del INT);",
                "Browser" => "CREATE TABLE Browser (BrowserId TEXT PRIMARY KEY, Name TEXT, Path TEXT, Note TEXT, Del INT);",
                "PortList" => "CREATE TABLE PortList (Port INT, Note TEXT);",
                "ModelPreset" => "CREATE TABLE ModelPreset (Model TEXT PRIMARY KEY, Preset TEXT);",
                "Devices" => "CREATE TABLE Devices (AssetId TEXT PRIMARY KEY, AssetNumber TEXT, AssetType TEXT, DeviceType TEXT, Model TEXT, Description TEXT, User TEXT, UserPhone TEXT, EnableDate TEXT, DeviceRoom TEXT, DeviceCabinet TEXT, TagA TEXT, TagB TEXT, TagC TEXT, TagD TEXT, TagE TEXT, TagF TEXT, Del INT);",
                "DeviceRoom" => "CREATE TABLE DeviceRoom (RoomId TEXT PRIMARY KEY, RoomName TEXT, BuildingId TEXT, SortIndex INT, Note TEXT, Del INT);",
                "Protocol" => "CREATE TABLE Protocol (Protocol TEXT, Note TEXT);",
                "DeviceCabinet" => "CREATE TABLE DeviceCabinet (CabinetId TEXT PRIMARY KEY, CabinetName TEXT, RoomId TEXT, SortIndex INT, Note TEXT, Del INT);",
                "Racks" => "CREATE TABLE Racks (RackId TEXT PRIMARY KEY, CabinetId TEXT, RackName TEXT, RackGroup TEXT, RackNote TEXT, SlotInfos TEXT, SlotCount INT, Model TEXT, AssetNumber TEXT, Del INT);",
                "Buildings" => "CREATE TABLE Buildings (BuildingId TEXT PRIMARY KEY, Building TEXT, Address TEXT, User TEXT, Phone TEXT, Note TEXT, SortIndex INT, Del INT);",
                "ScanPorts" => "CREATE TABLE ScanPorts (Port INT, Note TEXT);",
                "Notes" => "CREATE TABLE Notes (Id INTEGER PRIMARY KEY AUTOINCREMENT, Content TEXT, CreateDate TEXT, UpdateDate TEXT, Del INT);",
                "CustomSetting" => "CREATE TABLE CustomSetting (Key TEXT PRIMARY KEY, Value TEXT, Del INT);",
                "Bookmark" => "CREATE TABLE Bookmark (IndexId TEXT PRIMARY KEY, TypeGroup TEXT, Name TEXT, Protocol TEXT, Host TEXT, Port TEXT, Color INT, Browser TEXT, PinToStart INT, Del INT);",
                "Link" => "CREATE TABLE Link (LinkId TEXT PRIMARY KEY, LinkName TEXT, SourcePortId TEXT, TargetPortId TEXT, LinkType TEXT, Note TEXT, Del INT);",
                "LinkDetail" => "CREATE TABLE LinkDetail (LinkId TEXT, PortId TEXT, NodeIndex INT, TagA TEXT, TagB TEXT, TagC TEXT, Del INT);",
                "WakeOnLan" => "CREATE TABLE WakeOnLan (DeviceId TEXT PRIMARY KEY, Name TEXT, MacAddress TEXT, IpAddress TEXT, SubnetMask TEXT, Port INT, Note TEXT, Del INT);",
                "Computer" => "CREATE TABLE Computer (UID INTEGER PRIMARY KEY AUTOINCREMENT, DeviceId TEXT, AssetId TEXT, AssetUser TEXT, PortId TEXT, PortTag TEXT, PortType TEXT, PortStatus TEXT, OnTheLine INT, PortColor INT, PortGroup TEXT, BuildingId TEXT, Floor TEXT, Room TEXT, LinkIp TEXT, TagA TEXT, TagB TEXT, TagC TEXT, TagD TEXT, TagE TEXT, TagF TEXT, Del INT);",
                "Models" => "CREATE TABLE Models (ModelId TEXT PRIMARY KEY, ModelName TEXT, Manufacturer TEXT, PortCount INT, SlotCount INT, Note TEXT, Del INT);",
                "BookmarkGroupOrder" => "CREATE TABLE BookmarkGroupOrder (TypeGroup TEXT PRIMARY KEY, DisplayOrder INT, Del INT);",
                "AssetLog" => "CREATE TABLE AssetLog (UID INTEGER PRIMARY KEY AUTOINCREMENT, AssetId TEXT, EventDate TEXT, EventContent TEXT, AboutUser TEXT, Note TEXT, Name TEXT, TagA TEXT, TagB TEXT, TagC TEXT, TagD TEXT, TagE TEXT, TagF TEXT);",
                "NoteBook" => "CREATE TABLE NoteBook (Id INTEGER PRIMARY KEY AUTOINCREMENT, Title TEXT, Content TEXT, CreateDate TEXT, UpdateDate TEXT, Category TEXT, Del INT);",
                _ => $"CREATE TABLE IF NOT EXISTS {tableName} (Id INTEGER PRIMARY KEY AUTOINCREMENT);"
            };

            try
            {
                GlobalVariables.DbService.ExecuteNonQuery(sql);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 创建动态详情表（用于设备端口、机架端口等）
        /// </summary>
        public static void CreateDynamicsTableIfNotExists(string assetId, int type)
        {
            string tablePrefix = type switch
            {
                1 => "De_",  // Device
                2 => "Ra_",  // Rack
                _ => "De_"
            };
            string tableName = $"{tablePrefix}{assetId}";

            if (GlobalVariables.DbService.IsTableExists(tableName))
                return;

            string sql = $"CREATE TABLE {tableName} (UID INTEGER PRIMARY KEY AUTOINCREMENT, PortType TEXT, PortSpeed TEXT, PortTag TEXT, PortSlotNumber INT, PortId TEXT, PortStatus INT, Mode TEXT, PortName TEXT, VlanId TEXT, PortColor INT, OnTheLine INT, AssetId TEXT, TagA TEXT, TagB TEXT, TagC TEXT, TagD TEXT, TagE TEXT, TagF TEXT);";
            GlobalVariables.DbService.ExecuteNonQuery(sql);
        }
    }
}
