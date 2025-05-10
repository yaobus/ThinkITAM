using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Data.Sqlite;
using Nodify;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.IPAddressHelper;
using ThinkITAM.ViewModels.LinkManage;


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
            string sql = $"CREATE TABLE \"Net_{tableName}\" (\r\n  \"Address\" integer,\r\n  \"Status\" integer,\r\n \"AddressColor\" TEXT,\r\n   \"User\" TEXT,\r\n \"HostName\" TEXT,\r\n  \"MacAddress\" TEXT,\r\n  \"LinkDevice\" TEXT,\r\n \"TagA\" TEXT,\r\n  \"TagB\" TEXT,\r\n  \"TagC\" TEXT,\r\n  \"TagD\" TEXT,\r\n  \"TagE\" TEXT,\r\n  \"TagF\" TEXT\r\n);";


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
                query = $"INSERT INTO \"ModelPreset\" (\"Model\", \"Preset\") VALUES ('{model}', '{presetJson}')";

                GlobalVariables.DbService.ExecuteNonQuery(query);


            }
            else//库中已有，更新数据
            {
                MessageBoxResult result = MessageBox.Show("已存在相同型号的预设，是否覆盖?", "预设重复", MessageBoxButton.YesNo, MessageBoxImage.Information);

                if (result == MessageBoxResult.Yes)
                {
                    query = $"UPDATE  \"ModelPreset\" SET \"Preset\" = '{presetJson}' WHERE \"Model\" = '{model}'";
                    GlobalVariables.DbService.ExecuteNonQuery(query);
                }
                else
                {
                    MessageBoxResult result2 = MessageBox.Show("已存在相同型号的预设，是否创建副本?", "创建副本", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (result2 == MessageBoxResult.Yes)
                    {
                        query = $"INSERT INTO \"ModelPreset\" (\"Model\", \"Preset\") VALUES ('{model}_{rowCount + 1}', '{presetJson}')";
                        GlobalVariables.DbService.ExecuteNonQuery(query);
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

                var tag = GlobalVariables.DbService.ExecuteScalar(sql).ToString();


                return tag;



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

                        string query =$"CREATE TABLE \"{table}\" (\r\n  \"UID\" INTEGER NOT NULL,\r\n  \"SlotId\" text,\r\n  \"RoomId\" TEXT,\r\n  \"PortId\" text,\r\n  \"PortType\" text,\r\n  \"PortGroup\" TEXT,\r\n  \"PortColor\" INTEGER,\r\n  \"PortTag\" TEXT,\r\n  \"PortStatus\" TEXT,\r\n  \"OnTheLine\" INTEGER,\r\n  \"TagA\" TEXT,\r\n  \"TagB\" TEXT,\r\n  \"TagC\" TEXT,\r\n  \"TagD\" TEXT,\r\n  \"TagE\" TEXT,\r\n  \"TagF\" TEXT,\r\n  PRIMARY KEY (\"UID\")\r\n);";

                        //GlobalVariables.DbService.ExecuteNonQuery(query);

                        GlobalVariables.DbService.CreateTableFromSql(query);

                    }



                    break;


                case 1://建筑物端口详表
                    table = $"Bu_{tableName}";

                    if (!GlobalVariables.DbService.IsTableExists(table))
                    {

                        string query=$"CREATE TABLE \"{table}\" (\r\n  \"UID\" INTEGER NOT NULL,\r\n  \"SlotId\" text,\r\n  \"RoomId\" TEXT,\r\n  \"PortId\" text,\r\n  \"PortType\" text,\r\n  \"PortGroup\" TEXT,\r\n  \"PortColor\" INTEGER,\r\n  \"PortTag\" TEXT,\r\n  \"PortStatus\" TEXT,\r\n  \"OnTheLine\" INTEGER,\r\n  \"TagA\" TEXT,\r\n  \"TagB\" TEXT,\r\n  \"TagC\" TEXT,\r\n  \"TagD\" TEXT,\r\n  \"TagE\" TEXT,\r\n  \"TagF\" TEXT,\r\n  PRIMARY KEY (\"UID\")\r\n);";

                        //GlobalVariables.DbService.ExecuteNonQuery(query);
                        GlobalVariables.DbService.CreateTableFromSql(query);



                    }

                    break;

                case 2://设备端口详表
                    table = $"De_{tableName}";

                    if (!GlobalVariables.DbService.IsTableExists(table))
                    {
                        string query =$"CREATE TABLE \"{table}\" (\r\n  \"UID\" INTEGER NOT NULL,\r\n  \"AssetId\" TEXT,\r\n  \"PortType\" TEXT,\r\n  \"PortTag\" TEXT,\r\n  \"PortSlotNumber\" integer,\r\n  \"PortId\" TEXT,\r\n  \"Status\" integer,\r\n  \"Mode\" TEXT,\r\n  \"PortName\" TEXT,\r\n  \"VlanId\" TEXT,\r\n  \"PortColor\" integer,\r\n  \"OnTheLine\" INTEGER,\r\n  \"TagA\" TEXT,\r\n  \"TagB\" TEXT,\r\n  \"TagC\" TEXT,\r\n  \"TagD\" TEXT,\r\n  \"TagE\" TEXT,\r\n  \"TagF\" TEXT,\r\n  PRIMARY KEY (\"UID\")\r\n);";

                        //GlobalVariables.DbService.ExecuteNonQuery(query);

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

            string sql = $"SELECT COUNT(DISTINCT SlotId) FROM  'Bu_{buildingId}'";
            int floor = ExecuteScalarTableNum(sql);
            int room = ExecuteScalarTableNum($"SELECT COUNT(DISTINCT RoomId) FROM  'Bu_{buildingId}'");
            int port = ExecuteScalarTableNum($"SELECT COUNT(*) FROM  'Bu_{buildingId}'");


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
            return ExecuteScalarTableNum($"SELECT COUNT(DISTINCT RoomId) FROM  'Bu_{buildingId}'");
        }

        /// <summary>
        /// 获取指定楼层端口总数量
        /// </summary>
        /// <param name="buildingId"></param>
        /// <param name="floor"></param>
        /// <returns></returns>
        public static int GetPortForFloorCount(string buildingId, string floor)
        {
            return ExecuteScalarTableNum($"SELECT COUNT(PortId) FROM  'Bu_{buildingId}' WHERE SlotId ='{floor}'");
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
        /// 加载备注
        /// </summary>
        /// <param name="noteId"></param>
        /// <returns></returns>
        public static string LoadNote(string noteId)
        {
            string sql = $"SELECT Note FROM Notes WHERE NoteId ='{noteId}'";


           return GlobalVariables.DbService.ExecuteScalar(sql).ToString();

        }


        /// <summary>
        /// 获取机架/建筑/设备信息
        /// </summary>
        /// <param name="rackId"></param>
        /// <returns></returns>
        public static MdfRackClass GetRackInfo(string rackId)
        {
            MdfRackClass info = new MdfRackClass();
            info.RackId = rackId;
            string query = null;

            string deviceType = rackId.Substring(0, 1);


            switch (deviceType)
            {
                case "3"://机架


                    query = $"SELECT \r\n    Racks.RackName, \r\n    DeviceCabinet.CabinetName,\r\n    DeviceRoom.RoomName\r\nFROM \r\n    Racks\r\nINNER JOIN \r\n    DeviceCabinet ON Racks.CabinetId = DeviceCabinet.CabinetId\r\nINNER JOIN \r\n    DeviceRoom ON DeviceCabinet.DeviceRoomQrId = DeviceRoom.DeviceRoomQrId\r\nWHERE \r\n   Racks.RackId = '{rackId}';";


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
                var query = $"INSERT INTO \"WindowTag\" (\"Window\", \"Tags\") VALUES ('{windowName}', '{tags}')";

                GlobalVariables.DbService.ExecuteNonQuery(query);

            }
            else//库中已有，更新数据
            {
                var query = $"UPDATE \"WindowTag\" SET \"Tags\" = '{tags}' WHERE \"Window\" = '{windowName}'";

                GlobalVariables.DbService.ExecuteNonQuery(query);


            }






        }



        /// <summary>
        /// 表是否存在，不存在则建立表
        /// </summary>
        /// <param name="tableName"></param>
        public static Task<bool> CreateTableIfNotExists(string tableName)
        {


            switch (GlobalVariables.dbConfig.Type)
            {
                case "sqlite":

                    if (!GlobalVariables.DbService.IsTableExists(tableName))
                    {
                        string sql = string.Empty;

                        switch (tableName)//根据表名预设创建对应的表
                        {
                            case "SystemUser"://用户表,废弃功能

                                sql = $"CREATE TABLE \"SystemUser\" (\r\n  \"ipam_user\" TEXT,\r\n  \"Password\" TEXT\r\n);";

                                break;

                            case "Network"://网段信息表

                                sql = $"CREATE TABLE \"Network\" (\r\n  \"NetworkId\" TEXT,\r\n  \"Name\" TEXT,\r\n  \"Description\" TEXT,\r\n  \"Network\" TEXT,\r\n  \"Netmask\" TEXT,\r\n  \"Parent\" TEXT,\r\n  \"Child\" TEXT,\r\n  \"TagA\" TEXT,\r\n  \"TagB\" TEXT,\r\n  \"TagC\" TEXT,\r\n  \"TagD\" TEXT,\r\n \"TagE\" TEXT,\r\n  \"TagF\" TEXT);\r\n";

                                break;
                            case "WindowTag"://窗口注释表

                                sql = $"CREATE TABLE \"WindowTag\" (\r\n  \"Window\" TEXT,\r\n  \"Tags\" TEXT\r\n);";


                                break;


                            case "Hierarchy"://层级关系

                                sql = $"CREATE TABLE \"Hierarchy\" (\r\n  \"Parent\" TEXT,\r\n  \"Child\" TEXT\r\n);";


                                break;

                            case "Organization"://组织架构

                                sql = $"CREATE TABLE \"Organization\" (\r\n  \"Organization\" TEXT,\r\n  \"Department\" TEXT,\r\n  \"Groups\" TEXT,\r\n \"Note\" TEXT\r\n);";



                                break;

                            case "UserInfo"://人员信息

                                sql = $"CREATE TABLE \"UserInfo\" ( \"UserId\" TEXT,\r\n  \"Number\" integer,\r\n  \"Name\" TEXT,\r\n  \"Organization\" TEXT,\r\n  \"Department\" TEXT,\r\n  \"UserGroup\" TEXT,\r\n  \"Phone\" TEXT,\r\n  \"Note\" TEXT,\r\n  \"Del\" integer,\r\n  PRIMARY KEY (\"Number\")\r\n);";



                                break;


                            case "AssetTag"://资产类型标签

                                sql = $"CREATE TABLE \"AssetTag\" (\r\n  \"AssetType\" TEXT,\r\n  \"DeviceType\" TEXT,\r\n  \"AssetTag\" TEXT,\r\n  \"Note\" TEXT\r\n);";


                                break;

                            case "Address"://地址预设表

                                sql = $"CREATE TABLE \"Address\" (\r\n  \"Location\" TEXT,\r\n  \"Note\" TEXT);";


                                break;

                            case "Asset"://资产表

                                sql = $"CREATE TABLE \"Asset\" (\r\n  \"Id\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,\r\n  \"AssetId\" text,\r\n  \"AssetQrCode\" TEXT,\r\n  \"AssetType\" TEXT,\r\n  \"DeviceType\" TEXT,\r\n  \"AssetTag\" TEXT,\r\n  \"AssetNumber\" INTEGER,\r\n  \"PurchaseDate\" text,\r\n  \"PurchasePrice\" INTEGER,\r\n  \"Manufacturer\" TEXT,\r\n  \"Model\" TEXT,\r\n  \"SerialNumber\" text,\r\n  \"Configuration\" TEXT,\r\n  \"Location\" TEXT,\r\n  \"UserOrganization\" TEXT,\r\n  \"UserDepartment\" TEXT,\r\n  \"User\" TEXT,\r\n  \"UserPhone\" TEXT,\r\n  \"Consumer\" TEXT,\r\n  \"Status\" TEXT,\r\n  \"UsedYear\" TEXT,\r\n  \"ScrapDate\" text,\r\n  \"Notes\" TEXT,\r\n  \"TagA\" TEXT,\r\n  \"TagB\" TEXT,\r\n  \"TagC\" TEXT,\r\n  \"TagD\" TEXT,\r\n  \"TagE\" TEXT,\r\n  \"TagF\" TEXT\r\n);";

                                break;

                            case "Browser"://浏览器路径表

                                sql = $"CREATE TABLE \"Browser\" (\r\n  \"Browser\" TEXT,\r\n  \"Path\" TEXT,\r\n  \"Note\" TEXT\r\n);";


                                break;

                            case "PortList"://浏览器路径表

                                sql = $"CREATE TABLE \"PortList\" (\r\n  \"Port\" integer,\r\n \"Note\" TEXT\r\n);";


                                break;

                            case "ModelPreset"://型号预设表

                                sql = $"CREATE TABLE \"ModelPreset\" (\r\n  \"Model\" TEXT,\r\n  \"Preset\" TEXT\r\n);";



                                break;

                            case "Devices"://设备总表

                                sql = $"CREATE TABLE \"Devices\" (\r\n  \"AssetId\" TEXT NOT NULL,\r\n  \"AssetNumber\" TEXT NOT NULL,\r\n  \"AssetType\" TEXT,\r\n  \"DeviceType\" TEXT,\r\n  \"Model\" TEXT,\r\n  \"Description\" TEXT,\r\n  \"User\" TEXT,\r\n  \"UserPhone\" TEXT,\r\n  \"EnableDate\" text,\r\n  \"UseDepartment\" TEXT,\r\n  \"Address\" TEXT,\r\n  \"TagA\" TEXT,\r\n  \"TagB\" TEXT,\r\n  \"TagC\" TEXT,\r\n  \"TagD\" TEXT,\r\n  \"TagE\" TEXT,\r\n  \"TagF\" TEXT,\r\n  PRIMARY KEY (\"AssetId\", \"AssetNumber\")\r\n);";


                                break;

                            case "DeviceRoom"://设备间表

                                sql = $"CREATE TABLE \"DeviceRoom\" (\r\n  \"DeviceRoomQrId\" text NOT NULL,\r\n  \"Name\" TEXT,\r\n  \"Location\" TEXT,\r\n  \"User\" TEXT,\r\n  \"UserPhone\" TEXT,\r\n  \"Note\" TEXT,\r\n  PRIMARY KEY (\"DeviceRoomQrId\")\r\n);";



                                break;

                            case "Protocol"://协议表


                                sql = $"CREATE TABLE \"Protocol\" (\r\n  \"Protocol\" TEXT,\r\n \"Note\" TEXT\r\n);";


                                break;


                            case "DeviceCabinet"://设备间表

                                sql = $"CREATE TABLE \"DeviceCabinet\" (\r\n  \"CabinetId\" text NOT NULL,\r\n  \"DeviceRoomQrId\" text,\r\n  \"Name\" TEXT,\r\n  \"Position\" TEXT,\r\n  \"Note\" TEXT,\r\n  PRIMARY KEY (\"CabinetId\")\r\n);\r\n";


                                break;


                            case "Racks"://设备间表

                                sql = $"CREATE TABLE \"Racks\" (\r\n  \"RackId\" text NOT NULL,\r\n  \"CabinetId\" text,\r\n  \"RackName\" TEXT,\r\n   \"RackGroup\" TEXT,\r\n  \"RackNote\" TEXT,\r\n   \"SlotInfos\" TEXT,\r\n \"SlotCount\" integer,\r\n  PRIMARY KEY (\"RackId\")\r\n);";



                                break;



                            case "Buildings"://建筑物表

                                sql = $"CREATE TABLE \"Buildings\" (\r\n  \"BuildingId\" text NOT NULL,\r\n  \"Building\" TEXT,\r\n  \"Address\" TEXT,\r\n  \"User\" TEXT,\r\n  \"Phone\" TEXT,\r\n  \"Note\" TEXT,\r\n  PRIMARY KEY (\"BuildingId\")\r\n);";


                                break;

                            case "ScanPorts"://扫描端口预设表

                                sql = $"CREATE TABLE \"ScanPorts\" (\r\n  \"Name\" TEXT,\r\n  \"Ports\" TEXT\r\n);";


                                break;

                            case "Notes"://收藏夹分组

                                sql = $"CREATE TABLE \"Notes\" (\r\n  \"NoteId\" text NOT NULL,\r\n  \"Note\" TEXT,\r\n  PRIMARY KEY (\"NoteId\")\r\n);";



                                break;

                            case "CustomSetting"://自定义设置

                                sql = $"CREATE TABLE \"CustomSetting\" (\r\n  \"Option\" TEXT,\r\n  \"Content\" TEXT,\r\n  \"Note\" TEXT);";


                                break;

                            case "Bookmark"://导航索引

                                sql = $"CREATE TABLE \"Bookmark\" (\"IndexId\" TEXT,\r\n   \"TypeGroup\" TEXT,\r\n  \"Name\" TEXT,\r\n  \"Protocol\" TEXT,\r\n  \"Host\" TEXT,\r\n  \"Port\" TEXT,\r\n  \"Color\" integer,\r\n  \"Browser\" TEXT,\r\n  \"Del\" integer\r\n);";



                                break;


                            case "Link"://主链路表

                                sql = $"CREATE TABLE \"Link\" (\n  \"Link_ID\" INTEGER PRIMARY KEY AUTOINCREMENT,\n  \"Alias\" TEXT NOT NULL,\n  \"Create_Time\" DATETIME DEFAULT CURRENT_TIMESTAMP,\n  \"Update_Time\" DATETIME DEFAULT CURRENT_TIMESTAMP\n);";



                                break;


                            case "LinkDetail"://链路详表

                                sql = $"CREATE TABLE \"LinkDetail\" (\r\n  \"Detail_ID\" INTEGER PRIMARY KEY AUTOINCREMENT,\r\n  \"Link_ID\" INTEGER NOT NULL,\r\n  \"Sequence_No\" INTEGER NOT NULL,\r\n  \"Device_ID\" text NOT NULL,\r\n  \"Port_UID\" INTEGER,\r\n  FOREIGN KEY (\"Link_ID\") REFERENCES \"Link\" (\"Link_ID\") ON DELETE NO ACTION ON UPDATE NO ACTION\r\n);";

                                break;
                            case "WakeOnLan":

                                sql = "CREATE TABLE \"WakeOnLan\" (\r\n  \"UID\" INTEGER NOT NULL,\r\n  \"HostGroup\" TEXT,\r\n  \"Name\" TEXT,\r\n  \"IpAddress\" TEXT,\r\n  \"NetMask\" TEXT,\r\n  \"Mac\" TEXT,\r\n  \"Port\" TEXT,\r\n  \"Note\" TEXT,\r\n \"PinToStart\" integer,\r\n PRIMARY KEY (\"UID\")\r\n);";



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

                                sql = $"CREATE TABLE SystemUser (Ipam_user VARCHAR(255) PRIMARY KEY, Password TEXT);;";

                                break;

                            case "Network"://网段信息表

                                sql = $"CREATE TABLE Network (\r\n  NetworkId TEXT,\r\n  Name TEXT,\r\n  Description TEXT,\r\n  Network TEXT,\r\n  Netmask TEXT,\r\n  Parent TEXT,\r\n  Child TEXT,\r\n  TagA TEXT,\r\n  TagB TEXT,\r\n  TagC TEXT,\r\n  TagD TEXT,\r\n  TagE TEXT,\r\n  TagF TEXT\r\n);";

                                break;
                            case "WindowTag"://窗口注释表

                                sql = $"CREATE TABLE WindowTag (Window TEXT, Tags TEXT);";


                                break;


                            case "Hierarchy"://层级关系

                                sql = $"CREATE TABLE Hierarchy (Parent TEXT, Child TEXT);";


                                break;

                            case "Organization"://组织架构

                                sql =
                                    $"CREATE TABLE Organization (\r\n  Organization TEXT,\r\n  Department TEXT,\r\n  Groups TEXT,\r\n  Note TEXT\r\n);";


                                break;

                            case "UserInfo"://人员信息

                                sql =
                                    $"CREATE TABLE UserInfo (\r\n  UserId TEXT,\r\n  Number INT AUTO_INCREMENT PRIMARY KEY,\r\n  Name TEXT,\r\n  Organization TEXT,\r\n  Department TEXT,\r\n  UserGroup TEXT,\r\n  Phone TEXT,\r\n  Note TEXT,\r\n  Del INT\r\n);";


                                break;


                            case "AssetTag"://资产类型标签

                                sql =
                                    $"CREATE TABLE AssetTag (\r\n  AssetType VARCHAR(255),\r\n  DeviceType VARCHAR(255),\r\n  AssetTag VARCHAR(255),\r\n  Note TEXT\r\n);";

                                break;

                            case "Address"://地址预设表

                                sql = $"CREATE TABLE Address (\r\n  Location VARCHAR(255),\r\n  Note TEXT\r\n);";


                                break;

                            case "Asset"://资产表

                                sql =
                                    $"CREATE TABLE `Asset` (\r\n  `Id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY,\r\n  `AssetId` VARCHAR(255),\r\n  `AssetQrCode` TEXT,\r\n  `AssetType` TEXT,\r\n  `DeviceType` TEXT,\r\n  `AssetTag` TEXT,\r\n  `AssetNumber` INT,\r\n  `PurchaseDate` VARCHAR(255),\r\n  `PurchasePrice` VARCHAR(255),\r\n  `Manufacturer` TEXT,\r\n  `Model` TEXT,\r\n  `SerialNumber` VARCHAR(255),\r\n  `Configuration` TEXT,\r\n  `Location` TEXT,\r\n  `UserOrganization` TEXT,\r\n  `UserDepartment` TEXT,\r\n  `User` TEXT,\r\n  `UserPhone` TEXT,\r\n  `Consumer` TEXT,\r\n  `Status` TEXT,\r\n  `UsedYear` TEXT,\r\n  `ScrapDate` VARCHAR(255),\r\n  `Notes` TEXT,\r\n  `TagA` TEXT,\r\n  `TagB` TEXT,\r\n  `TagC` TEXT,\r\n  `TagD` TEXT,\r\n  `TagE` TEXT,\r\n  `TagF` TEXT\r\n) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";
                                break;

                            case "Browser"://浏览器路径表

                                sql =
                                    $"CREATE TABLE `Browser` (\r\n  `Browser` TEXT,\r\n  `Path` TEXT,\r\n  `Note` TEXT\r\n) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

                                break;

                            case "PortList"://浏览器路径表

                                sql =
                                    $"CREATE TABLE `PortList` (\r\n  `Port` INT,\r\n  `Note` TEXT\r\n) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";
                                break;

                            case "ModelPreset"://型号预设表

                                sql = $"CREATE TABLE `ModelPreset` (\r\n  `Model` TEXT,\r\n  `Preset` TEXT\r\n) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";


                                break;

                            case "Devices"://设备总表

                                sql =
                                    $"CREATE TABLE `Devices` (\r\n  `AssetId` VARCHAR(255) NOT NULL,\r\n  `AssetNumber` VARCHAR(255) NOT NULL,\r\n  `AssetType` TEXT,\r\n  `DeviceType` TEXT,\r\n  `Model` TEXT,\r\n  `Description` TEXT,\r\n  `User` TEXT,\r\n  `UserPhone` TEXT,\r\n  `EnableDate` VARCHAR(255),\r\n  `UseDepartment` TEXT,\r\n  `Address` TEXT,\r\n  `TagA` TEXT,\r\n  `TagB` TEXT,\r\n  `TagC` TEXT,\r\n  `TagD` TEXT,\r\n  `TagE` TEXT,\r\n  `TagF` TEXT,\r\n  PRIMARY KEY (`AssetId`, `AssetNumber`)\r\n) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

                                break;

                            case "DeviceRoom"://设备间表

                                sql =
                                    $"CREATE TABLE `DeviceRoom` (\r\n  `DeviceRoomQrId` VARCHAR(255) NOT NULL,\r\n  `Name` TEXT,\r\n  `Location` TEXT,\r\n  `User` TEXT,\r\n  `UserPhone` TEXT,\r\n  `Note` TEXT,\r\n  PRIMARY KEY (`DeviceRoomQrId`)\r\n) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";


                                break;

                            case "Protocol"://协议表


                                sql =
                                    $"CREATE TABLE `Protocol` (\r\n  `Protocol` TEXT,\r\n  `Note` TEXT\r\n) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

                                break;


                            case "DeviceCabinet"://设备间表

                                sql =
                                    $"CREATE TABLE `DeviceCabinet` (\r\n  `CabinetId` VARCHAR(255) NOT NULL,\r\n  `DeviceRoomQrId` VARCHAR(255),\r\n  `Name` TEXT,\r\n  `Position` TEXT,\r\n  `Note` TEXT,\r\n  PRIMARY KEY (`CabinetId`)\r\n) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

                                break;


                            case "Racks"://设备间表

                                sql =
                                    $"CREATE TABLE `Racks` (\r\n  `RackId` VARCHAR(255) NOT NULL,\r\n  `CabinetId` VARCHAR(255),\r\n  `RackName` TEXT,\r\n  `RackGroup` TEXT,\r\n  `RackNote` TEXT,\r\n  `SlotInfos` TEXT,\r\n  `SlotCount` INT,\r\n  PRIMARY KEY (`RackId`)\r\n) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";


                                break;



                            case "Buildings"://建筑物表

                                sql =
                                    $"CREATE TABLE `Buildings` (\r\n  `BuildingId` VARCHAR(255) NOT NULL,\r\n  `Building` TEXT,\r\n  `Address` TEXT,\r\n  `User` TEXT,\r\n  `Phone` TEXT,\r\n  `Note` TEXT,\r\n  PRIMARY KEY (`BuildingId`)\r\n) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

                                break;

                            case "ScanPorts"://扫描端口预设表

                                sql =
                                    $"CREATE TABLE `ScanPorts` (\r\n  `Name` TEXT,\r\n  `Ports` TEXT\r\n) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

                                break;

                            case "Notes"://收藏夹分组

                                sql =
                                    $"CREATE TABLE `Notes` (\r\n  `NoteId` VARCHAR(255) NOT NULL,\r\n  `Note` TEXT,\r\n  PRIMARY KEY (`NoteId`)\r\n) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";


                                break;

                            case "CustomSetting"://自定义设置

                                sql =
                                    $"CREATE TABLE `CustomSetting` (\r\n  `Option` TEXT,\r\n  `Content` TEXT,\r\n  `Note` TEXT\r\n) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

                                break;

                            case "Bookmark"://导航索引

                                sql =
                                    $"CREATE TABLE `Bookmark` (\r\n  `IndexId` TEXT,\r\n  `TypeGroup` TEXT,\r\n  `Name` TEXT,\r\n  `Protocol` TEXT,\r\n  `Host` TEXT,\r\n  `Port` TEXT,\r\n  `Color` INT,\r\n  `Browser` TEXT,\r\n  `Del` INT\r\n) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";


                                break;


                            case "Link"://主链路表

                                sql =
                                    $"CREATE TABLE `Link` (\r\n  `Link_ID` INT AUTO_INCREMENT PRIMARY KEY,\r\n  `Alias` VARCHAR(255) NOT NULL,\r\n  `Create_Time` DATETIME DEFAULT CURRENT_TIMESTAMP,\r\n  `Update_Time` DATETIME DEFAULT CURRENT_TIMESTAMP\r\n) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";


                                break;


                            case "LinkDetail"://链路详表

                                sql =
                                    $"CREATE TABLE `LinkDetail` (\r\n  `Detail_ID` INT AUTO_INCREMENT PRIMARY KEY,\r\n  `Link_ID` INT NOT NULL,\r\n  `Sequence_No` INT NOT NULL,\r\n  `Device_ID` VARCHAR(255) NOT NULL,\r\n  `Port_UID` INT,\r\n  FOREIGN KEY (`Link_ID`) REFERENCES `Link` (`Link_ID`)\r\n    ON DELETE NO ACTION\r\n    ON UPDATE NO ACTION\r\n) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";
                                break;
                            case "WakeOnLan":

                                sql =
                                    $"CREATE TABLE `WakeOnLan` (\r\n  `UID` INT NOT NULL,\r\n  `HostGroup` TEXT,\r\n  `Name` TEXT,\r\n  `IpAddress` TEXT,\r\n  `NetMask` TEXT,\r\n  `Mac` TEXT,\r\n  `Port` TEXT,\r\n  `Note` TEXT,\r\n  `PinToStart` INT,\r\n  PRIMARY KEY (`UID`)\r\n) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";


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
