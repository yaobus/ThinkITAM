using System;
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


            GlobalVariables.DbService.ExecuteNonQuery(sql);






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

                        GlobalVariables.DbService.ExecuteNonQuery(query);

  

                    }



                    break;


                case 1://建筑物端口详表
                    table = $"Bu_{tableName}";

                    if (!GlobalVariables.DbService.IsTableExists(table))
                    {

                        string query=$"CREATE TABLE \"{table}\" (\r\n  \"UID\" INTEGER NOT NULL,\r\n  \"SlotId\" text,\r\n  \"RoomId\" TEXT,\r\n  \"PortId\" text,\r\n  \"PortType\" text,\r\n  \"PortGroup\" TEXT,\r\n  \"PortColor\" INTEGER,\r\n  \"PortTag\" TEXT,\r\n  \"PortStatus\" TEXT,\r\n  \"OnTheLine\" INTEGER,\r\n  \"TagA\" TEXT,\r\n  \"TagB\" TEXT,\r\n  \"TagC\" TEXT,\r\n  \"TagD\" TEXT,\r\n  \"TagE\" TEXT,\r\n  \"TagF\" TEXT,\r\n  PRIMARY KEY (\"UID\")\r\n);";

                        GlobalVariables.DbService.ExecuteNonQuery(query);




                    }

                    break;

                case 2://设备端口详表
                    table = $"De_{tableName}";

                    if (!GlobalVariables.DbService.IsTableExists(table))
                    {
                        string query =$"CREATE TABLE \"{table}\" (\r\n  \"UID\" INTEGER NOT NULL,\r\n  \"AssetId\" TEXT,\r\n  \"PortType\" TEXT,\r\n  \"PortTag\" TEXT,\r\n  \"PortSlotNumber\" integer,\r\n  \"PortId\" TEXT,\r\n  \"Status\" integer,\r\n  \"Mode\" TEXT,\r\n  \"PortName\" TEXT,\r\n  \"VlanId\" TEXT,\r\n  \"PortColor\" integer,\r\n  \"OnTheLine\" INTEGER,\r\n  \"TagA\" TEXT,\r\n  \"TagB\" TEXT,\r\n  \"TagC\" TEXT,\r\n  \"TagD\" TEXT,\r\n  \"TagE\" TEXT,\r\n  \"TagF\" TEXT,\r\n  PRIMARY KEY (\"UID\")\r\n);";

                        GlobalVariables.DbService.ExecuteNonQuery(query);


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
            return ExecuteScalarTableNum($"SELECT COUNT(PortId) FROM  'bu_{buildingId}' WHERE SlotId ='{floor}'");
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
            string sql = $"SELECT COUNT(*) FROM bu_{buildingId} WHERE SlotId ='{floor}' AND RoomId='{roomNumber}'";

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
                var query =$"INSERT INTO \"WindowTag\" (\"Window\", \"Tags\") VALUES ('{windowName}', '{tags}')";

                GlobalVariables.DbService.ExecuteNonQuery(query);

            }
            else//库中已有，更新数据
            {
                var query = $"UPDATE \"WindowTag\" SET \"Tags\" = '{tags}' WHERE \"Window\" = '{windowName}'";

                GlobalVariables.DbService.ExecuteNonQuery(query);
               

            }






        }

    }
}
