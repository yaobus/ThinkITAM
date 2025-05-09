using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkITAM.ViewModels.LinkManage;

namespace ThinkITAM.Functions.FunctionClass;


public static class PortLinkFinder
{
    /// <summary>
    /// 查找链路上的所有相关端子
    /// </summary>
    /// <param name="startPort">起始端子信息</param>
    /// <param name="linkType">链路类型 ("PermanentLink" 或 "TempLink")</param>
    /// <param name="connectionString">数据库连接字符串</param>
    /// <returns>链路上的所有相关端子</returns>
    public static ObservableCollection<PortClass> FindLinkedPortsBidirectional(
        PortClass startPort, string linkType, SQLiteConnection connection)
    {
        if (startPort == null || (linkType != "PermanentLink" && linkType != "TempLink"))
            throw new ArgumentException("无效的参数");

        var result = new ObservableCollection<PortClass>();
        var visited = new HashSet<string>(); // 防止重复访问


        void Traverse(PortClass currentPort, string direction)
        {
            if (currentPort == null || visited.Contains(GetPortUniqueKey(currentPort)))
                return;

            // 检查当前端子属性
            //Console.WriteLine($"Traversing Port: RackId={currentPort.RackId}, SlotIndex={currentPort.SlotIndex}, PortIndex={currentPort.PortIndex}");


            //result.Add(currentPort);
            //visited.Add(GetPortUniqueKey(currentPort));

            //var link = direction == "PermanentLink" ? currentPort.PermanentLink : currentPort.TempLink;
            //if (link == null || string.IsNullOrEmpty(link.RackID))
            //    return;

            //var nextPort = LoadPortFromDatabase(link, connection, direction);
            //Traverse(nextPort, direction);
        }

        // 开始递归查找两端
        Traverse(startPort, linkType);

        return result;
    }

    /// <summary>
    /// 从数据库加载一个端子信息
    /// </summary>
    /// <param name="link">链路信息</param>
    /// <param name="connection">数据库连接</param>
    /// <param name="direction">链路方向 ("PermanentLink" 或 "TempLink")</param>
    /// <returns>端子信息</returns>
    private static PortClass LoadPortFromDatabase(PermanentLink link, SQLiteConnection connection, string direction)
    {

        string tableHeader = "ra";

        if (link.RackID.Substring(0, 1) == "8")//判断是不是建筑
        {
            tableHeader = "bu";
        }


        var query = $"SELECT * FROM {tableHeader}_{link.RackID} WHERE SlotId = '{link.SlotIndex}' AND PortId = '{link.PortIndex}'";

        using var command = new SQLiteCommand(query, connection);
        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            var port = new PortClass
            {
                RackId = link.RackID,
                SlotIndex = reader["SlotId"].ToString(),
                PortIndex = reader["PortId"].ToString(),
                Room = reader["RoomId"].ToString(),
                //PermanentLink = reader.IsDBNull(reader.GetOrdinal("PermanentType")) ? null : new PermanentLink
                //{
                //    RackID = reader["PermanentRackId"].ToString(),
                //    SlotIndex = reader["PermanentSlot"].ToString(),
                //    PortIndex = reader["PermanentPort"].ToString(),
                //    Room = reader["PermanentRoom"].ToString(),
                //    Type = Convert.ToInt32(reader["PermanentType"])
                //},
                //TempLink = reader.IsDBNull(reader.GetOrdinal("TempType")) ? null : new PermanentLink
                //{
                //    RackID = reader["TempRackId"].ToString(),
                //    SlotIndex =reader["TempSlot"].ToString(),
                //    PortIndex = reader["TempPort"].ToString(),
                //    Room = reader["PermanentRoom"].ToString(),
                //    Type = Convert.ToInt32(reader["TempType"])
                //}
            };
            return port;
        }

        return null;
    }




    /// <summary>
    /// 获取端子的唯一键值
    /// </summary>
    private static string GetPortUniqueKey(PortClass port)
    {
       // return $"{port.RackId}-{port.SlotIndex}-{port.PortIndex}-{port.PermanentLink?.RackID}-{port.PermanentLink?.SlotIndex}-{port.PermanentLink?.PortIndex}";
       return null;
    }
}
