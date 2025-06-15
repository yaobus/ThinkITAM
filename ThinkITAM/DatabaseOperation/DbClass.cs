using System.Collections.ObjectModel;
using System.Windows;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.IPAddressHelper;
using ThinkITAM.UserControls.LinkPage;
using ThinkITAM.ViewModels.LinkManage;
using ThinkITAM.ViewModels.Others;


namespace ThinkITAM.DatabaseOperation
{
    public class DbClass
    {


        /// <summary>
        /// 加载自定义窗口标签
        /// </summary>
        /// <param name="windowName"></param>
        /// <returns></returns>
        public static string LoadWindowTag(string windowName)
        {
            string sql = $"SELECT Tags FROM WindowTag WHERE Window = '{windowName}'";

            var rows = GlobalVariables.DbService.ExecuteQuery(sql);

            if (rows.Count > 0)
            {
                return rows[0]["Tags"].ToString();
            }
            else
            {
                return null;
            }
        }



        /// <summary>
        /// 查询表中的数据数量，返回数据条数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int ExecuteScalarTableNum(string sql)
        {
            //Console.WriteLine(sql);

            int rowCount = Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(sql));

            return rowCount;

        }



        /// <summary>
        /// 创建大型网段对应ip分表,256个地址以上的网段
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="netmask"></param>
        public static void CreateNetworkTableSub(string ipAddress, int netmask, string tableName)
        {
            var info = SubnetCalculator.CalculateSubnets(ipAddress, netmask);

            int index = 0;


            foreach (var sub in info.Item2)
            {
                //创建新的数据表名
                string newName = tableName + $"_Sub{index}";

                CreateNetworkTable(newName);

                index++;
            }





        }


        /// <summary>
        /// 创建网段对应ip详表,256个地址以下的网段
        /// </summary>
        /// <param name="query"></param>
        public static void CreateNetworkTable(string tableName)
        {


            string sql = $"CREATE TABLE Net_{tableName} ( Address INTEGER, AddressStatus INTEGER, AddressColor TEXT,  User TEXT, HostName TEXT, MacAddress TEXT, LinkDevice TEXT, TagA TEXT, TagB TEXT, TagC TEXT, TagD TEXT, TagE TEXT, TagF TEXT  );";

            Console.WriteLine(sql);

            //GlobalVariables.DbService.ExecuteNonQuery(sql);
            GlobalVariables.DbService.CreateTableFromSql(sql);





        }



        /// <summary>
        /// 保存设备端口预设
        /// </summary>
        /// <param name="model"></param>
        /// <param name="presetJson"></param>
        public static void SaveModelPreset(string model, string presetJson)
        {
            string sql = $"SELECT COUNT(*) FROM ModelPreset WHERE Model = '{model}'";




            // 使用 ExecuteScalar 获取总行数
            int rowCount = ExecuteScalarTableNum(sql);

            string query;

            if (rowCount == 0)//库中没有，插入数据
            {
                var preset = new { model = model, preset = presetJson };




                GlobalVariables.DbService.InsertEntity("ModelPreset", preset);


            }
            else//库中已有，更新数据
            {
                MessageBoxResult result = MessageBox.Show("已存在相同型号的预设，是否覆盖?", "预设重复", MessageBoxButton.YesNo, MessageBoxImage.Information);

                if (result == MessageBoxResult.Yes)
                {
                    query = $"UPDATE  ModelPreset SET Preset = '{presetJson}' WHERE Model = '{model}'";
                    GlobalVariables.DbService.ExecuteNonQuery(query);
                }
                else
                {
                    MessageBoxResult result2 = MessageBox.Show("已存在相同型号的预设，是否创建副本?", "创建副本", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (result2 == MessageBoxResult.Yes)
                    {

                        var preset = new { model = $"{model}_{rowCount + 1}", preset = presetJson };




                        GlobalVariables.DbService.InsertEntity("ModelPreset", preset);

                    }


                }




            }





        }


        /// <summary>
        /// 加载设备端口预设
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string LoadModelPreset(string model)
        {
            string sql = $"SELECT COUNT(*) FROM ModelPreset WHERE Model = '{model}'";


            // 使用 ExecuteScalar 获取总行数
            int rowCount = ExecuteScalarTableNum(sql);

            if (rowCount > 0)
            {

                sql = $"SELECT Preset FROM ModelPreset WHERE Model = '{model}'";

                var tag = GlobalVariables.DbService.ExecuteScalar(sql);

                if (tag != null)
                {
                    return tag.ToString();
                }
                else
                {
                    return "";
                }

            }
            else
            {

                return null;
            }

        }



        /// <summary>
        /// 加载模型预设列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static ObservableCollection<string> LoadModelPresetList(string model)
        {

            string sql = $"SELECT COUNT(*) FROM ModelPreset WHERE Model LIKE '{model}%'";





            // 使用 ExecuteScalar 获取总行数
            int rowCount = ExecuteScalarTableNum(sql);

            if (rowCount > 0)
            {

                ObservableCollection<string> modelPresetList = new ObservableCollection<string>();

                sql = $"SELECT * FROM ModelPreset WHERE Model LIKE '{model}%'";


                var rows = GlobalVariables.DbService.ExecuteQuery(sql);

                foreach (var row in rows)
                {
                    modelPresetList.Add(row["Model"].ToString());
                }

                return modelPresetList;

            }
            else
            {

                return null;
            }




        }

        /// <summary>
        /// 判断表是否存在
        /// </summary>
        /// <param name="tableName">动态表ID，为自动生成的字符串</param>
        /// <param name="tableType">动态表类型，0为配线架端口详表，1为建筑物端口详表，2为设备端口详表</param>
        public static void CreateDynamicsTableIfNotExists(string tableName, int tableType)
        {
            string table;

            switch (tableType)
            {
                case 0:  //机架端口详表
                    table = $"Ra_{tableName}";

                    if (!GlobalVariables.DbService.IsTableExists(table))
                    {

                        string query = $"CREATE TABLE {table} (UID INTEGER NOT NULL, SlotId TEXT, RoomId TEXT, PortId TEXT, PortType TEXT, PortGroup TEXT, PortColor TEXT, PortStatus TEXT, PortTag TEXT,  OnTheLine INTEGER, TagA TEXT, TagB TEXT, TagC TEXT, TagD TEXT, TagE TEXT, TagF TEXT, Del INTEGER,   PRIMARY KEY (UID) );";

                        //GlobalVariables.DbService.ExecuteNonQuery(query);

                        GlobalVariables.DbService.CreateTableFromSql(query);

                    }



                    break;


                case 1://建筑物端口详表
                    table = $"Bu_{tableName}";

                    if (!GlobalVariables.DbService.IsTableExists(table))
                    {
                        string query = $"CREATE TABLE {table} (UID INTEGER NOT NULL, SlotId TEXT, RoomId TEXT, PortId TEXT, PortType TEXT, PortGroup TEXT, PortColor TEXT, PortTag TEXT, PortStatus TEXT, OnTheLine INTEGER, TagA TEXT, TagB TEXT, TagC TEXT, TagD TEXT, TagE TEXT, TagF TEXT, Del INTEGER,  PRIMARY KEY (UID) );";

                        GlobalVariables.DbService.CreateTableFromSql(query);

                    }

                    break;

                case 2://设备端口详表
                    table = $"De_{tableName}";

                    if (!GlobalVariables.DbService.IsTableExists(table))
                    {
                        string query = $"CREATE TABLE {table} ( UID INTEGER NOT NULL, AssetId TEXT, PortType TEXT, PortSpeed TEXT, PortSlotNumber INTEGER, PortId TEXT, PortStatus INTEGER,PortTag TEXT, Mode TEXT, PortName TEXT, VlanId TEXT, PortColor INTEGER, OnTheLine INTEGER, TagA TEXT, TagB TEXT, TagC TEXT, TagD TEXT, TagE TEXT, TagF TEXT,Del INTEGER,   PRIMARY KEY (UID) );";

                        GlobalVariables.DbService.CreateTableFromSql(query);
                    }

                    break;


            }




        }


        /// <summary>
        /// 加载所选的机架信息，存到全局变量组
        /// </summary>
        /// <param name="rackId"></param>
        public static void LoadSelectRackInfo(string rackId)
        {
            string query = $"SELECT * FROM Racks WHERE RackId='{rackId}'";

            var rows = GlobalVariables.DbService.ExecuteQuery(query);


            int index = 0;

            foreach (var row in rows)
            {
                index++;
                RackInfo info = new RackInfo();
                info.Index = index;
                info.rackId = row["RackId"].ToString();
                info.rackGroup = row["RackGroup"].ToString();
                info.rackName = row["RackName"].ToString();
                info.rackNote = row["RackNote"].ToString();
                string infos = row["SlotInfos"].ToString();

                var slotInfos = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<SlotClass>>(infos);
                info.slotInfos = slotInfos;

                info.slotCount = Convert.ToInt32(row["SlotCount"]);


                DataBridge.DataBridge.SelectRackInfo = info;
            }



        }


        /// <summary>
        /// 统计建筑总楼层/总房间数量/总端口数量
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        public static string StatisticsPortRoomFloor(string buildingId)
        {

            string sql = $"SELECT COUNT(DISTINCT SlotId) FROM  Bu_{buildingId}";
            int floor = ExecuteScalarTableNum(sql);
            int room = ExecuteScalarTableNum($"SELECT COUNT(DISTINCT RoomId) FROM  Bu_{buildingId}");
            int port = ExecuteScalarTableNum($"SELECT COUNT(*) FROM  Bu_{buildingId}");


            return $"{floor}/{room}/{port}";
        }


        /// <summary>
        /// 通过建筑ID获取建筑名称
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        public static string GetBuildingNameForBuildingId(string buildingId)
        {
            string sql = $"SELECT Building FROM  Buildings WHERE BuildingId='{buildingId}'";


            return GlobalVariables.DbService.ExecuteScalar(sql).ToString();
        }


        /// <summary>
        /// 统计建筑总楼层/总房间数量/总设备数量
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        public static string StatisticsDeviceRoomFloor(string buildingId)
        {
            string sql = $"SELECT COUNT(DISTINCT SlotId) FROM  Bu_{buildingId}";
            int floor = ExecuteScalarTableNum(sql);
            int room = ExecuteScalarTableNum($"SELECT COUNT(DISTINCT RoomId) FROM  Bu_{buildingId}");
            int port = ExecuteScalarTableNum($"SELECT COUNT(*) FROM  Computer WHERE BuildingId='{buildingId}'");

            return $"{floor}/{room}/{port}";
        }

        /// <summary>
        /// 获取指定楼层房间总数量
        /// </summary>
        /// <param name="buildingId"></param>
        /// <param name="floor"></param>
        /// <returns></returns>
        public static int GetRoomForFloorCount(string buildingId, string floor)
        {
            return ExecuteScalarTableNum($"SELECT COUNT(DISTINCT RoomId) FROM  Bu_{buildingId} WHERE SlotId ='{floor}'");
        }

        /// <summary>
        /// 获取指定楼层端口总数量
        /// </summary>
        /// <param name="buildingId"></param>
        /// <param name="floor"></param>
        /// <returns></returns>
        public static int GetPortForFloorCount(string buildingId, string floor)
        {
            return ExecuteScalarTableNum($"SELECT COUNT(PortId) FROM  Bu_{buildingId} WHERE SlotId ='{floor}'");
        }


        /// <summary>
        /// 获取指定楼层设备终端总数量
        /// </summary>
        /// <param name="buildingId"></param>
        /// <param name="floor"></param>
        /// <returns></returns>
        public static int GetDeviceForFloorCount(string buildingId, string floor)
        {
            return ExecuteScalarTableNum($"SELECT COUNT(*) FROM  Computer WHERE Floor ='{floor}'");
        }

        /// <summary>
        /// 获取指定房间端口数量
        /// </summary>
        /// <param name="buildingId">建筑ID</param>
        /// <param name="floor">楼层</param>
        /// <param name="roomNumber">房间号</param>
        /// <returns></returns>
        public static int GetRoomPortCount(string buildingId, string floor, string roomNumber)
        {
            string sql = $"SELECT COUNT(*) FROM Bu_{buildingId} WHERE SlotId ='{floor}' AND RoomId='{roomNumber}'";

            return ExecuteScalarTableNum(sql);
        }


        /// <summary>
        /// 获取指定房间设备数量
        /// </summary>
        /// <param name="buildingId">建筑ID</param>
        /// <param name="floor">楼层</param>
        /// <param name="roomNumber">房间号</param>
        /// <returns></returns>
        public static int GetRoomDeviceCount(string buildingId, string floor, string roomNumber)
        {
            string sql = $"SELECT COUNT(*) FROM Computer WHERE BuildingId='{buildingId}' AND Floor ='{floor}' AND Room='{roomNumber}'";

            return ExecuteScalarTableNum(sql);
        }

        /// <summary>
        /// 加载备注
        /// </summary>
        /// <param name="noteId"></param>
        /// <returns></returns>
        public static string LoadNote(string noteId)
        {
            string sql = $"SELECT Note FROM Notes WHERE NoteId ='{noteId}'";

            var result = GlobalVariables.DbService.ExecuteScalar(sql);


            if (result != null)
            {
                return result.ToString();
            }
            else
            {
                return "";
            }


        }

        /// <summary>
        /// 获取机架端口数量
        /// </summary>
        /// <param name="rackId"></param>
        /// <returns></returns>
        public static int GetRackPortCount(string rackId)
        {
            string sql = $"SELECT COUNT(*) FROM Ra_{rackId} ";

            return ExecuteScalarTableNum(sql);

        }

        /// <summary>
        /// 查询设备槽位数量
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public static int GetDeviceSlotCount(string deviceId)
        {
            string sql = $"SELECT COUNT(DISTINCT PortSlotNumber) FROM  De_{deviceId}";

            return ExecuteScalarTableNum(sql);
        }


        /// <summary>
        /// 获取机架/建筑/设备信息
        /// </summary>
        /// <param name="rackId"></param>
        /// <returns></returns>
        public static MdfRackClass GetRackInfo(string rackId)
        {
            var info = new MdfRackClass();
            info.RackId = rackId;
            string query = null;

            var deviceType = rackId.Substring(0, 1);


            switch (deviceType)
            {
                case "3"://机架


                    query = $"SELECT Racks.RackName, DeviceCabinet.CabinetName, DeviceRoom.RoomName FROM Racks INNER JOIN DeviceCabinet ON Racks.CabinetId = DeviceCabinet.CabinetId INNER JOIN DeviceRoom ON DeviceCabinet.DeviceRoomQrId = DeviceRoom.DeviceRoomQrId WHERE  Racks.RackId = '{rackId}';";


                    var rows3 = GlobalVariables.DbService.ExecuteQuery(query);

                    foreach (var row in rows3)
                    {
                        info.RackName = row["RackName"].ToString();
                        info.CabinetName = row["CabinetName"].ToString();
                        info.RoomName = row["RoomName"].ToString();
                    }




                    break;

                case "8"://建筑

                    query = $"SELECT * FROM Buildings WHERE BuildingId = '{rackId}'";

                    var rows8 = GlobalVariables.DbService.ExecuteQuery(query);

                    foreach (var row8 in rows8)
                    {
                        info.RoomName = row8["Building"].ToString();
                    }




                    break;

                case "2"://设备

                    query = $"SELECT * FROM Devices WHERE AssetId='{rackId}'";

                    var rows2 = GlobalVariables.DbService.ExecuteQuery(query);

                    foreach (var row2 in rows2)
                    {
                        info.RoomName = row2["AssetNumber"].ToString();
                        info.CabinetName = row2["Model"].ToString();
                        info.RackName = row2["Description"].ToString();
                    }

                    break;

                case "4":
                    query = $"SELECT * FROM Computer WHERE DeviceId='{rackId}'";

                    var rows4 = GlobalVariables.DbService.ExecuteQuery(query);

                    foreach (var row4 in rows4)
                    {
                        //info.RoomName = row4["AssetNumber"].ToString();
                        //info.CabinetName = row4["Model"].ToString();
                        //info.RackName = row4["Description"].ToString();

                        //TODO
                    }

                    break;
            }






            return info;
        }


        public static LinkDeviceInfo GetLinkDeviceInfo(string assetId ,int? uid)
        {
            var info = new LinkDeviceInfo();

            string deviceType = assetId.Substring(0, 1);

            switch (deviceType)
            {
                case "2": //设备
                    var query2 = $"SELECT Devices.AssetNumber, Devices.Description, DeviceCabinet.CabinetName, DeviceRoom.RoomName  FROM Devices INNER JOIN DeviceCabinet ON Devices.DeviceCabinet = DeviceCabinet.CabinetId INNER JOIN DeviceRoom ON DeviceCabinet.DeviceRoomQrId = DeviceRoom.DeviceRoomQrId  WHERE Devices.AssetId = '{assetId}';";
                    var rows2 = GlobalVariables.DbService.ExecuteQuery(query2);

                    foreach (var row in rows2)
                    {
                        info.InfoA = "[设备]";
                        info.InfoB = $"机房名称：{row["RoomName"]}"; 
                        info.InfoC = $"机柜名称：{row["CabinetName"]}";
                        info.InfoD = $"设备名称：{row["Description"]}"; 
                        info.InfoE = $"资产编号：{row["AssetNumber"]}";
                    }

                    
                    break;


                case "3": //机架


                    var  query3 = $"SELECT Racks.RackName, DeviceCabinet.CabinetName, DeviceRoom.RoomName FROM Racks INNER JOIN DeviceCabinet ON Racks.CabinetId = DeviceCabinet.CabinetId INNER JOIN DeviceRoom ON DeviceCabinet.DeviceRoomQrId = DeviceRoom.DeviceRoomQrId WHERE  Racks.RackId = '{assetId}';";

                    var rows3 = GlobalVariables.DbService.ExecuteQuery(query3);

                    foreach (var row in rows3)
                    {
                        info.InfoA = "[机架]";
                        info.InfoB = $"机房名称：{row["RoomName"]}";
                        info.InfoC = $"机柜名称：{row["CabinetName"]}";
                        info.InfoD = $"机架名称：{row["RackName"]}"; 
                    }


                    break;

                case "4": //已部署的终端

                    var query4 = $"SELECT Buildings.Building, Computer.Floor, Computer.Room, Asset.AssetTag, Asset.AssetNumber FROM Computer INNER JOIN Buildings ON Computer.BuildingId = Buildings.BuildingId INNER JOIN Asset ON Computer.AssetId = Asset.AssetId WHERE Computer.DeviceId = '{assetId}';";

                    var rows4 = GlobalVariables.DbService.ExecuteQuery(query4);

                    foreach (var row in rows4)
                    {
                        info.InfoA = "[终端]";
                        info.InfoB = $"部署位置：{row["Building"]}";
                        info.InfoC = $"部署楼层：{row["Floor"]}";
                        info.InfoD = $"部署房间：{row["Room"]}"; 
                        info.InfoE = $"资产编号：{row["AssetTag"]}{row["AssetNumber"]}";
                    }

                   
                    break;

                case "8": //建筑内端口

                    var query8 = $"SELECT ( SELECT Building FROM Buildings WHERE BuildingId = '{assetId}' ) AS Building, ( SELECT SlotId FROM  Bu_{assetId}  WHERE UID = {uid} ) AS SlotId, ( SELECT RoomId FROM  Bu_{assetId} WHERE UID = {uid} ) AS RoomId, ( SELECT PortId FROM Bu_{assetId} WHERE UID = {uid} ) AS PortId;";
                    var rows8 = GlobalVariables.DbService.ExecuteQuery(query8);

                    foreach (var row in rows8)
                    {
                        info.InfoA = "[面板]";
                        info.InfoB = $"所在建筑：{row["Building"]}";
                        info.InfoC = $"所在楼层：{row["SlotId"]}"; 
                        info.InfoD = $"所在房间：{row["RoomId"]}";
                        info.InfoE = $"端口编号：{row["PortId"]}"; 
                    }


                   
                    break;
            }


            return info;
        }


        /// <summary>
        /// 获取设备端口所在槽位信息
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static SlotClass GetDeviceSlotInfo(string deviceId, int uid)
        {
            SlotClass slot = new SlotClass();


            string query = $"SELECT * FROM De_{deviceId} WHERE UID='{uid}'";

            var rows = GlobalVariables.DbService.ExecuteQuery(query);


            foreach (var row in rows)
            {
                slot.SlotName = row["PortSlotNumber"].ToString();

                slot.SlotTag = row["PortType"].ToString();
            }



            return slot;


        }


        /// <summary>
        /// 获取建筑物信息
        /// </summary>
        /// <param name="buildingId"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static SlotClass GetBuildingRoomInfo(string buildingId, int uid)
        {
            SlotClass slot = new SlotClass();


            string query = $"SELECT * FROM Bu_{buildingId} WHERE UID='{uid}'";


            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            foreach (var row in rows)
            {
                slot.SlotName = row["SlotId"].ToString();

                slot.SlotTag = row["RoomId"].ToString();
            }


            return slot;
        }


        /// <summary>
        /// 保存自定义窗口标签
        /// </summary>
        /// <param name="window"></param>
        /// <param name="tags"></param>
        public static void SaveWindowTag(string windowName, string tags)
        {
            var sql = $"SELECT COUNT(*) FROM WindowTag WHERE Window = '{windowName}'";



            // 使用 ExecuteScalar 获取总行数
            int rowCount = ExecuteScalarTableNum(sql);


            if (rowCount == 0)//库中没有，插入数据
            {
                var tagInfo = new ViewModels.DatabaseEntity.Window.WindowTagViewModel()
                {
                    Window = windowName,
                    Tags = tags
                };


                GlobalVariables.DbService.InsertEntity("WindowTag", tagInfo);



            }
            else//库中已有，更新数据
            {


                var tagInfo = new ViewModels.DatabaseEntity.Window.WindowTagViewModel()
                {
                    Window = windowName,
                    Tags = tags
                };

                var conditions = new { Window = windowName };

                GlobalVariables.DbService.UpdateEntity("WindowTag", tagInfo, conditions);

            }

        }


        /// <summary>
        /// 获取指定表中指定字段的最小可用编号
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="field">字段名</param>
        /// <returns></returns>
        public static int GetNextAvailableNumber(string tableName, string field)
        {
            var usedNumbers = new HashSet<int>();

            string sql = $"SELECT {field} FROM {tableName} "; // 假设Del为0表示未删除的记录   WHERE Del != 1 OR Del IS NULL


            var rows = GlobalVariables.DbService.ExecuteQuery(sql);

            foreach (var row in rows)
            {
                usedNumbers.Add(Convert.ToInt32(row[$"{field}"]));
            }





            int nextNumber = 1; // Start with the smallest possible number
            while (usedNumbers.Contains(nextNumber))
            {
                nextNumber++;
            }

            return nextNumber;
        }


        /// <summary>
        /// 根据链路ID 获取链路节点详细信息
        /// </summary>
        /// <param name="linkId"></param>
        /// <returns></returns>
        public static ObservableCollection<PortLinkClass> GetLinkDetail(int linkId)
        {

            var nodes = new ObservableCollection<PortLinkClass>();

            string query = $"SELECT * FROM LinkDetail WHERE LinkId='{linkId}'";
            var rows = GlobalVariables.DbService.ExecuteQuery(query);


            foreach (var row in rows)
            {

                var assetId = row["DevicesAssetId"].ToString();

                var sequenceNo = Convert.ToInt32(row["SequenceNo"].ToString());

                //var linkDetailUid = Convert.ToInt32(row["UID"].ToString());
                var portUID = Convert.ToInt32(row["PortUID"].ToString());


                var node = GetLinkNodeInfo(assetId, portUID);

                node.PortClass.NodeIndex = sequenceNo;

                nodes.Add(node);

            }



            // 创建一个新的 ObservableCollection
            var sortedNodes = new ObservableCollection<PortLinkClass>(
                nodes.OrderBy(n => n.PortClass.NodeIndex)
            );


            return sortedNodes;
        }






        /// <summary>
        /// 根据资产ID对应的设备类型的Rack层信息
        /// </summary>
        /// <param name="AssetId"></param>
        /// <returns></returns>
        public static PortLinkClass GetLinkNodeInfo(string assetId, int uid)
        {
            if (string.IsNullOrEmpty(assetId))
                return null;

            var node = new PortLinkClass();
            var slot = new SlotClass();
            var port = new PortClass();

            node.MdfRackClass = GetRackInfo(assetId);
            string sql;

            switch (assetId[0])
            {
                case '3':

                    sql = $"SELECT * FROM Ra_{assetId} WHERE UID='{uid}'";
                    var rows3 = GlobalVariables.DbService.ExecuteQuery(sql);

                    foreach (var row in rows3)
                    {
                        slot.SlotName = row["SlotId"].ToString();
                        slot.SlotTag = row["PortType"].ToString();


                        //端口基础信息
                        port.UID = uid;
                        port.PortIndex = row["PortId"].ToString();

                        //如果获取到的颜色为空，则设置为默认颜色
                        if (row["PortColor"] == DBNull.Value || row["PortColor"] == string.Empty)
                        {
                            port.PortColor = 0;
                        }
                        else
                        {
                            port.PortColor = Convert.ToInt32(row["PortColor"]);
                        }

                        port.RackId = assetId;
                        port.PortTag = row["PortTag"].ToString();
                        port.PortStatus = row["PortStatus"].ToString();
                        port.PortType = row["PortType"].ToString();


                        if (row["OnTheLine"] == DBNull.Value || row["OnTheLine"] == string.Empty)
                        {
                            port.OnTheLine = -1;
                        }
                        else
                        {
                            port.OnTheLine = Convert.ToInt32(row["OnTheLine"]);
                        }



                    }



                    break;


                case '2':

                    sql = $"SELECT * FROM De_{assetId} WHERE UID='{uid}'";

                    var rows = GlobalVariables.DbService.ExecuteQuery(sql);

                    foreach (var row in rows)
                    {
                        slot.SlotName = row["PortSlotNumber"].ToString();

                        slot.SlotTag = row["PortType"].ToString();


                        if (row["OnTheLine"] == DBNull.Value || row["OnTheLine"] == string.Empty)
                        {
                            port.OnTheLine = -1;
                        }
                        else
                        {
                            port.OnTheLine = Convert.ToInt32(row["OnTheLine"]);
                        }



                        port.UID = uid;
                        port.RackId = assetId;


                        string slotNumber = row["PortSlotNumber"].ToString();




                        string portId = row["PortId"].ToString();


                        port.PortType = row["PortType"].ToString();
                        port.PortIndex = $"{slotNumber}{portId}";
                        port.PortTag = row["PortTag"].ToString();
                        port.SlotIndex = slotNumber;






                        if (row["PortColor"] == DBNull.Value || row["PortColor"] == string.Empty)
                        {
                            port.PortColor = 0;
                        }
                        else
                        {
                            port.PortColor = Convert.ToInt32(row["PortColor"].ToString());
                        }


                    }



                    break;

                case '4':



                    sql = $"SELECT   c.*,   a.AssetTag,  a.AssetNumber,  u.Name FROM   Computer c JOIN   Asset a ON c.AssetId = a.AssetId  JOIN   UserInfo u ON c.AssetUser = u.UserId WHERE   c.UID = '{uid}';";

                    var rows4 = GlobalVariables.DbService.ExecuteQuery(sql);


                    foreach (var row in rows4)
                    {
                        // slot.SlotName = row["SlotId"].ToString();
                        //slot.SlotTag = row["RoomId"].ToString();


                        if (row["OnTheLine"] == DBNull.Value || row["OnTheLine"] == string.Empty)
                        {
                            port.OnTheLine = -1;
                        }
                        else
                        {
                            port.OnTheLine = Convert.ToInt32(row["OnTheLine"]);
                        }



                        port.UID = uid;
                        port.RackId = assetId;
                        port.DeviceId = row["DeviceId"].ToString();
                        port.PortType = row["PortType"].ToString();
                        port.PortIndex = row["PortId"].ToString();
                        port.PortTag = row["PortTag"].ToString();
                        port.SlotIndex = row["Floor"].ToString();
                        port.Room = row["Room"].ToString(); ;



                        if (row["PortColor"] == DBNull.Value || row["PortColor"] == string.Empty)
                        {
                            port.PortColor = 0;
                        }
                        else
                        {
                            port.PortColor = Convert.ToInt32(row["PortColor"].ToString());
                        }


                        node.MdfRackClass.RoomName = GetBuildingNameForBuildingId(row["BuildingId"].ToString());
                        node.MdfRackClass.CabinetName = port.SlotIndex;
                        node.MdfRackClass.RackName = port.Room;

                    }










                    break;


                case '8':


                    sql = $"SELECT * FROM Bu_{assetId} WHERE UID='{uid}'";


                    var rows8 = GlobalVariables.DbService.ExecuteQuery(sql);

                    foreach (var row in rows8)
                    {
                        slot.SlotName = row["SlotId"].ToString();
                        slot.SlotTag = row["RoomId"].ToString();


                        if (row["OnTheLine"] == DBNull.Value || row["OnTheLine"] == string.Empty)
                        {
                            port.OnTheLine = -1;
                        }
                        else
                        {
                            port.OnTheLine = Convert.ToInt32(row["OnTheLine"]);
                        }



                        port.UID = uid;
                        port.RackId = assetId;
                        port.PortType = row["PortType"].ToString();
                        port.PortIndex = row["PortId"].ToString();
                        port.PortTag = row["PortTag"].ToString();
                        port.SlotIndex = row["SlotId"].ToString();
                        port.Room = row["RoomId"].ToString(); ;



                        if (row["PortColor"] == DBNull.Value || row["PortColor"] == string.Empty)
                        {
                            port.PortColor = 0;
                        }
                        else
                        {
                            port.PortColor = Convert.ToInt32(row["PortColor"].ToString());
                        }



                        node.MdfRackClass.CabinetName = port.SlotIndex.ToString();
                        node.MdfRackClass.RackName = row["RoomId"].ToString();

                    }

                    break;

            }


            node.SlotClass = slot;
            node.PortClass = port;




            return node;

        }

        /// <summary>
        /// 根据Computer表中的设备ID获取资产ID
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public static string GetRackIdForDeviceId(string deviceId)
        {
            var sql = string.Empty;

            switch (deviceId.Substring(0, 1))
            {
                case "4":

                    sql = $"Select AssetId from Computer WHERE deviceId='{deviceId}'";

                    return GlobalVariables.DbService.ExecuteScalar(sql).ToString();

                default:

                    return deviceId;
            }
        }


        /// <summary>
        /// 表是否存在，不存在则建立表
        /// </summary>
        /// <param name="tableName"></param>
        public static Task<bool> CreateTableIfNotExists(string tableName)
        {


            switch (GlobalVariables.dbConfig.Type.ToLower())
            {


                case "sqlite":

                    if (!GlobalVariables.DbService.IsTableExists(tableName))
                    {
                        string sql = string.Empty;

                        switch (tableName)//根据表名预设创建对应的表
                        {
                            case "SystemUser"://用户表,废弃功能

                                sql = $"CREATE TABLE \"SystemUser\" (   \"Ipam_user\" TEXT,   \"Password\" TEXT ,   \"Del\" integer);";

                                break;

                            case "Network"://网段信息表

                                sql = $"CREATE TABLE \"Network\" (   \"NetworkId\" TEXT,   \"Name\" TEXT,   \"Description\" TEXT,   \"Network\" TEXT,   \"Netmask\" TEXT,   \"Parent\" TEXT,   \"Child\" TEXT,   \"TagA\" TEXT,   \"TagB\" TEXT,   \"TagC\" TEXT,   \"TagD\" TEXT,  \"TagE\" TEXT,   \"TagF\" TEXT,   \"Del\" integer); ";

                                break;
                            case "WindowTag"://窗口注释表

                                sql = $"CREATE TABLE \"WindowTag\" (   \"Window\" TEXT,   \"Tags\" TEXT );";


                                break;


                            case "Hierarchy"://层级关系

                                sql = $"CREATE TABLE \"Hierarchy\" (   \"Parent\" TEXT,   \"Child\" TEXT );";


                                break;

                            case "Organization"://组织架构

                                sql = $"CREATE TABLE \"Organization\" (   \"Organization\" TEXT,   \"Department\" TEXT,   \"Groups\" TEXT,  \"Note\" TEXT ,   \"Del\" integer);";



                                break;

                            case "UserInfo"://人员信息

                                sql = $"CREATE TABLE \"UserInfo\" ( \"UserId\" TEXT,   \"Number\" integer,   \"Name\" TEXT,   \"Organization\" TEXT,   \"Department\" TEXT,   \"UserGroup\" TEXT,   \"Phone\" TEXT,   \"Note\" TEXT,   \"Del\" integer,   PRIMARY KEY (\"Number\") );";



                                break;


                            case "AssetTag"://资产类型标签

                                sql = $"CREATE TABLE \"AssetTag\" (   \"AssetType\" TEXT,   \"DeviceType\" TEXT,   \"AssetTag\" TEXT,   \"Note\" TEXT );";


                                break;

                            case "Address"://地址预设表

                                sql = $"CREATE TABLE \"Address\" (   \"Location\" TEXT,   \"Note\" TEXT,   \"Del\" integer);";


                                break;

                            case "Asset"://资产表

                                sql = $"CREATE TABLE \"Asset\" (   \"Id\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,   \"AssetId\" text,   \"AssetQrCode\" TEXT,   \"AssetType\" TEXT,   \"DeviceType\" TEXT,   \"AssetTag\" TEXT,   \"AssetNumber\" INTEGER,   \"PurchaseDate\" text,   \"PurchasePrice\" INTEGER,   \"Manufacturer\" TEXT,   \"Model\" TEXT,   \"SerialNumber\" text,   \"Configuration\" TEXT,   \"Location\" TEXT,   \"UserOrganization\" TEXT,   \"UserDepartment\" TEXT, \"UserGroup\" TEXT,  \"User\" TEXT,   \"UserPhone\" TEXT,   \"Consumer\" TEXT,   \"AssetStatus\" TEXT,   \"UsedYear\" TEXT,   \"ScrapDate\" text,   \"Notes\" TEXT,   \"TagA\" TEXT,   \"TagB\" TEXT,   \"TagC\" TEXT,   \"TagD\" TEXT,   \"TagE\" TEXT,   \"TagF\" TEXT , \"Deploy\" integer ,   \"Del\" integer );";

                                break;

                            case "Browser"://浏览器路径表

                                sql = $"CREATE TABLE \"Browser\" (   \"Browser\" TEXT,   \"Path\" TEXT,   \"Note\" TEXT );";


                                break;

                            case "PortList":

                                sql = $"CREATE TABLE \"PortList\" (   \"Port\" integer,  \"Note\" TEXT );";


                                break;

                            case "ModelPreset"://型号预设表

                                sql = $"CREATE TABLE \"ModelPreset\" (   \"Model\" TEXT,   \"Preset\" TEXT );";



                                break;

                            case "Devices"://设备总表

                                sql = $"CREATE TABLE \"Devices\" (   \"AssetId\" TEXT NOT NULL,   \"AssetNumber\" TEXT NOT NULL,   \"AssetType\" TEXT,   \"DeviceType\" TEXT,   \"Model\" TEXT,   \"Description\" TEXT,   \"User\" TEXT,   \"UserPhone\" TEXT,   \"EnableDate\" text,   \"DeviceRoom\" TEXT,   \"DeviceCabinet\" TEXT,   \"TagA\" TEXT,   \"TagB\" TEXT,   \"TagC\" TEXT,   \"TagD\" TEXT,   \"TagE\" TEXT,   \"TagF\" TEXT,\"Del\" integer,   PRIMARY KEY (\"AssetId\", \"AssetNumber\") );";


                                break;

                            case "DeviceRoom"://设备间表

                                sql = $"CREATE TABLE \"DeviceRoom\" (   \"DeviceRoomQrId\" text NOT NULL,   \"RoomName\" TEXT,   \"Location\" TEXT,   \"User\" TEXT,   \"UserPhone\" TEXT,   \"Note\" TEXT, \"Del\" integer,  PRIMARY KEY (\"DeviceRoomQrId\") );";



                                break;

                            case "Protocol"://协议表


                                sql = $"CREATE TABLE \"Protocol\" (   \"Protocol\" TEXT,  \"Note\" TEXT );";


                                break;


                            case "DeviceCabinet"://设备间表

                                sql = $"CREATE TABLE \"DeviceCabinet\" (   \"CabinetId\" text NOT NULL,   \"DeviceRoomQrId\" text,   \"CabinetName\" TEXT,   \"Position\" TEXT,   \"Note\" TEXT,  \"Del\" integer,    PRIMARY KEY (\"CabinetId\") ); ";


                                break;


                            case "Racks"://设备间表

                                sql = $"CREATE TABLE \"Racks\" (   \"RackId\" text NOT NULL,   \"CabinetId\" text,   \"RackName\" TEXT,    \"RackGroup\" TEXT,   \"RackNote\" TEXT,    \"SlotInfos\" TEXT,  \"SlotCount\" integer,\"Del\" integer ,  PRIMARY KEY (\"RackId\") );";



                                break;



                            case "Buildings"://建筑物表

                                sql = $"CREATE TABLE \"Buildings\" (   \"BuildingId\" text NOT NULL,   \"Building\" TEXT,   \"Address\" TEXT,   \"User\" TEXT,   \"Phone\" TEXT,   \"Note\" TEXT, \"Del\" integer ,  PRIMARY KEY (\"BuildingId\") );";


                                break;

                            case "ScanPorts"://扫描端口预设表

                                sql = $"CREATE TABLE \"ScanPorts\" (   \"Name\" TEXT,   \"Ports\" TEXT );";


                                break;

                            case "Notes"://笔记表

                                sql = $"CREATE TABLE \"Notes\" (   \"NoteId\" text NOT NULL,   \"Note\" TEXT,   PRIMARY KEY (\"NoteId\") );";



                                break;

                            case "CustomSetting"://自定义设置-用户编号前缀等内容

                                sql = $"CREATE TABLE \"CustomSetting\" (   \"Option\" TEXT,   \"Content\" TEXT,   \"Note\" TEXT);";


                                break;

                            case "Bookmark"://导航索引

                                sql = $"CREATE TABLE \"Bookmark\" (\"IndexId\" TEXT,    \"TypeGroup\" TEXT,   \"Name\" TEXT,   \"Protocol\" TEXT,   \"Host\" TEXT,   \"Port\" TEXT,   \"Color\" integer,   \"Browser\" TEXT, \"PinToStart\" integer,   \"Del\" integer );";



                                break;


                            case "Link"://主链路表

                                sql = $"CREATE TABLE \"Link\" (   \"Link_ID\" INTEGER PRIMARY KEY AUTOINCREMENT,   \"Alias\" TEXT NOT NULL,   \"Create_Time\" DATETIME DEFAULT CURRENT_TIMESTAMP,   \"Update_Time\" DATETIME DEFAULT CURRENT_TIMESTAMP );";



                                break;


                            case "LinkDetail"://链路详表

                                sql = $"CREATE TABLE \"LinkDetail\" (\r\n    \"UID\" INTEGER PRIMARY KEY AUTOINCREMENT,\r\n    \"LinkId\" INTEGER NOT NULL,\r\n    \"SequenceNo\" INTEGER,\r\n    \"DevicesAssetId\" TEXT,\r\n    \"PortUID\" TEXT\r\n);";



                                break;
                            case "WakeOnLan":

                                sql = "CREATE TABLE \"WakeOnLan\" (   \"UID\" INTEGER NOT NULL,   \"HostGroup\" TEXT,   \"Name\" TEXT,   \"IpAddress\" TEXT,   \"NetMask\" TEXT,   \"Mac\" TEXT,   \"Port\" TEXT,   \"Note\" TEXT,  \"PinToStart\" integer,  PRIMARY KEY (\"UID\") );";



                                break;
                            case "Computer":

                                sql = $"CREATE TABLE \"Computer\" ( \"UID\" INTEGER NOT NULL, \"DeviceId\" TEXT, \"AssetId\" TEXT, \"AssetUser\" TEXT, \"PortId\" INTEGER, \"PortTag\" TEXT, \"PortType\" TEXT, \"PortStatus\" TEXT, \"OnTheLine\" INTEGER, \"PortColor\" INTEGER, \"PortGroup\" TEXT, \"BuildingId\" TEXT, \"Floor\" TEXT, \"Room\" TEXT, \"Del\" INTEGER, PRIMARY KEY (\"UID\") );";

                                break;

                            case "Models":

                                sql = $"CREATE TABLE \"Models\" ( \"AssetType\" TEXT, \"DeviceType\" TEXT, \"Model\" TEXT);";

                                break;
                        }


                        Console.WriteLine(sql);
                        return GlobalVariables.DbService.CreateTableFromSqlAsync(sql);

                    }
                    else
                    {
                        return null;
                    }

                    break;
                case "mariadb":
                case "mysql":


                    if (!GlobalVariables.DbService.IsTableExists(tableName))
                    {
                        string sql = string.Empty;

                        switch (tableName)//根据表名预设创建对应的表
                        {
                            case "SystemUser"://用户表,废弃功能

                                sql = $"CREATE TABLE SystemUser (Ipam_user VARCHAR(32) PRIMARY KEY, Password TEXT, Del INT);;";

                                break;

                            case "Network"://网段信息表

                                sql = $"CREATE TABLE Network (   NetworkId VARCHAR(32),   Name TEXT,   Description TEXT,   Network TEXT,   Netmask TEXT,   Parent TEXT,   Child TEXT,   TagA TEXT,   TagB TEXT,   TagC TEXT,   TagD TEXT,   TagE TEXT,   TagF TEXT , Del INT);";

                                break;
                            case "WindowTag"://窗口注释表

                                sql = $"CREATE TABLE WindowTag (Window VARCHAR(32), Tags TEXT);";


                                break;


                            case "Hierarchy"://层级关系

                                sql = $"CREATE TABLE Hierarchy (Parent TEXT, Child TEXT);";


                                break;

                            case "Organization"://组织架构

                                sql =
                                    $"CREATE TABLE Organization (   Organization VARCHAR(32),   Department VARCHAR(32),   Groups VARCHAR(32),   Note VARCHAR(255) , Del INT);";


                                break;

                            case "UserInfo"://人员信息

                                sql =
                                    $"CREATE TABLE UserInfo (   UserId VARCHAR(32),   Number INT AUTO_INCREMENT PRIMARY KEY,   Name TEXT,   Organization TEXT,   Department TEXT,   UserGroup TEXT,   Phone TEXT,   Note TEXT,   Del INT );";


                                break;


                            case "AssetTag"://资产类型标签

                                sql =
                                    $"CREATE TABLE AssetTag (   AssetType VARCHAR(32),   DeviceType VARCHAR(32),   AssetTag VARCHAR(255),   Note TEXT );";

                                break;

                            case "Address"://地址预设表

                                sql = $"CREATE TABLE Address (Location VARCHAR(255),Note TEXT,`Del` INT);";


                                break;

                            case "Asset"://资产表

                                sql =
                                    $"CREATE TABLE `Asset` (   `Id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY,   `AssetId` VARCHAR(32),   `AssetQrCode` VARCHAR(32),   `AssetType` VARCHAR(32),   `DeviceType` VARCHAR(32),   `AssetTag` VARCHAR(32),   `AssetNumber` INT,   `PurchaseDate` VARCHAR(255),   `PurchasePrice` VARCHAR(255),   `Manufacturer` VARCHAR(255),   `Model` VARCHAR(64),   `SerialNumber` VARCHAR(255),   `Configuration` TEXT,   `Location` VARCHAR(255),   `UserOrganization` VARCHAR(32),   `UserDepartment` VARCHAR(32),  `UserGroup` VARCHAR(32),`User` VARCHAR(32),   `UserPhone` VARCHAR(32),   `Consumer` VARCHAR(32),   `AssetStatus` VARCHAR(32),   `UsedYear` TEXT,   `ScrapDate` VARCHAR(255),   `Notes` VARCHAR(255),   `TagA` VARCHAR(255),   `TagB` VARCHAR(255),   `TagC` VARCHAR(255),   `TagD` VARCHAR(255),   `TagE` VARCHAR(255),   `TagF` VARCHAR(255),  `Deploy` INT, `Del` INT  ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";
                                break;

                            case "Browser"://浏览器路径表

                                sql =
                                    $"CREATE TABLE `Browser` (   `Browser` VARCHAR(32),   `Path` TEXT,   `Note` TEXT ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

                                break;

                            case "PortList"://浏览器路径表

                                sql =
                                    $"CREATE TABLE `PortList` (   `Port` INT,   `Note` TEXT ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";
                                break;

                            case "ModelPreset"://型号预设表

                                sql = $"CREATE TABLE `ModelPreset` (   `Model` TEXT,   `Preset` TEXT ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";


                                break;

                            case "Devices"://设备总表

                                sql =
                                    $"CREATE TABLE `Devices` (   `AssetId` VARCHAR(255) NOT NULL,   `AssetNumber` VARCHAR(255) NOT NULL,   `AssetType` TEXT,   `DeviceType` TEXT,   `Model` TEXT,   `Description` TEXT,   `User` TEXT,   `UserPhone` TEXT,   `EnableDate` VARCHAR(255),   `DeviceRoom` TEXT,   `DeviceCabinet` TEXT,   `TagA` TEXT,   `TagB` TEXT,   `TagC` TEXT,   `TagD` TEXT,   `TagE` TEXT,   `TagF` TEXT, `Del` INT,   PRIMARY KEY (`AssetId`, `AssetNumber`) ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

                                break;

                            case "DeviceRoom"://设备间表

                                sql =
                                    $"CREATE TABLE `DeviceRoom` (   `DeviceRoomQrId` VARCHAR(255) NOT NULL,   `RoomName` TEXT,   `Location` TEXT,   `User` TEXT,   `UserPhone` TEXT,   `Note` TEXT, `Del` INT,   PRIMARY KEY (`DeviceRoomQrId`) ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";


                                break;

                            case "Protocol"://协议表


                                sql =
                                    $"CREATE TABLE `Protocol` (   `Protocol` VARCHAR(32),   `Note` TEXT ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

                                break;


                            case "DeviceCabinet"://设备间表

                                sql =
                                    $"CREATE TABLE `DeviceCabinet` (   `CabinetId` VARCHAR(255) NOT NULL,   `DeviceRoomQrId` VARCHAR(255),   `CabinetName` TEXT,   `Position` TEXT,   `Note` TEXT,  `Del` INT,  PRIMARY KEY (`CabinetId`) ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

                                break;


                            case "Racks"://设备间表

                                sql =
                                    $"CREATE TABLE `Racks` (   `RackId` VARCHAR(255) NOT NULL,   `CabinetId` VARCHAR(255),   `RackName` TEXT,   `RackGroup` TEXT,   `RackNote` TEXT,   `SlotInfos` TEXT,   `SlotCount` INT, Del INT,   PRIMARY KEY (`RackId`) ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";


                                break;



                            case "Buildings"://建筑物表

                                sql =
                                    $"CREATE TABLE `Buildings` (   `BuildingId` VARCHAR(255) NOT NULL,   `Building` TEXT,   `Address` TEXT,   `User` TEXT,   `Phone` TEXT,   `Note` TEXT,  Del INT ,  PRIMARY KEY (`BuildingId`) ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

                                break;

                            case "ScanPorts"://扫描端口预设表

                                sql =
                                    $"CREATE TABLE `ScanPorts` (   `Name` TEXT,   `Ports` TEXT ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

                                break;

                            case "Notes"://收藏夹分组

                                sql =
                                    $"CREATE TABLE `Notes` (   `NoteId` VARCHAR(255) NOT NULL,   `Note` TEXT,   PRIMARY KEY (`NoteId`) ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";


                                break;

                            case "CustomSetting"://自定义设置

                                sql =
                                    $"CREATE TABLE `CustomSetting` (   `Option` TEXT,   `Content` TEXT,   `Note` TEXT ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

                                break;

                            case "Bookmark"://导航索引

                                sql =
                                    $"CREATE TABLE `Bookmark` (   `IndexId` VARCHAR(32) PRIMARY KEY,   `TypeGroup` TEXT,   `Name` TEXT,   `Protocol` TEXT,   `Host` TEXT,   `Port` TEXT,   `Color` INT,   `Browser` TEXT, `PinToStart` INT, `Del` INT ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";


                                break;


                            case "Link"://主链路表

                                sql =
                                    $"CREATE TABLE `Link` (   `Link_ID` INT AUTO_INCREMENT PRIMARY KEY,   `Alias` VARCHAR(255) NOT NULL,   `Create_Time` DATETIME DEFAULT CURRENT_TIMESTAMP,   `Update_Time` DATETIME DEFAULT CURRENT_TIMESTAMP ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";


                                break;


                            case "LinkDetail"://链路详表

                                sql = $"CREATE TABLE LinkDetail ( UID INT AUTO_INCREMENT,LinkId INT NOT NULL,SequenceNo INT,DevicesAssetId VARCHAR(16),PortUID VARCHAR(8),PRIMARY KEY (UID));";

                                break;

                            case "WakeOnLan":

                                sql =
                                    $"CREATE TABLE `WakeOnLan` (   `UID` INT NOT NULL,   `HostGroup` TEXT,   `Name` TEXT,   `IpAddress` TEXT,   `NetMask` TEXT,   `Mac` TEXT,   `Port` TEXT,   `Note` TEXT,   `PinToStart` INT,   PRIMARY KEY (`UID`) ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";


                                break;
                            case "Computer":

                                sql =
                                    $"CREATE TABLE `Computer` (   `UID` INT NOT NULL,   `DeviceId` VARCHAR(16),   `AssetId`  VARCHAR(16),   `AssetUser` VARCHAR(16),   `PortId` INT,   `PortTag`  VARCHAR(255),   `PortType` VARCHAR(8),   `PortStatus` VARCHAR(8),   `OnTheLine` INT, `PortColor` INT, `PortGroup` VARCHAR(255),  `BuildingId` VARCHAR(16), `Floor` VARCHAR(16),`Room` VARCHAR(16), `Del` INT,  PRIMARY KEY (`UID`) ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";
                                break;

                            case "Models":

                                sql = $"CREATE TABLE `Models` ( `AssetType` VARCHAR(64), `DeviceType` VARCHAR(64), `Model` VARCHAR(64) )ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

                                break;
                        }


                        Console.WriteLine(sql);
                        return GlobalVariables.DbService.CreateTableFromSqlAsync(sql);

                    }
                    else
                    {
                        return null;
                    }

                    break;

                case "sqlserver":

                    break;

            }

            return null;

        }






    }
}
