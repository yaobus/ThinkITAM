using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.ViewModels.PortPanel;

namespace ThinkITAM.Windows.PortPanel
{
    /// <summary>
    /// AddPortPanelWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddPortPanelWindow : Window
    {
        public AddPortPanelWindow()
        {
            InitializeComponent();

        }



        /// <summary>
        /// 端口号列表
        /// </summary>
        ObservableCollection<string> portList = new ObservableCollection<string>();



        private void PreviewButton_OnClick(object sender, RoutedEventArgs e)
        {
            PortNumberTextBox.Text = "";
            portList.Clear();

            string str = PortIdBox.Text;

            // 检查字符串是否为空或空字符串
            if (!string.IsNullOrWhiteSpace(str))
            {
                // 取出字符串的最后的数字
                string lastChar = ExtractTrailingDigits(str);

                // 判断是否为整数
                int intValue;

                bool isInt = int.TryParse(lastChar, out intValue);


                if (isInt)
                {
                    // 取出除最后数字外的其他部分
                    string withoutLastChar = str.Substring(0, str.Length - lastChar.Length);

                    //int num = Convert.ToInt32(lastChar);

                    //Console.WriteLine(intValue);

                    for (int i = 0; i < PortNumberSlider.Value; i++)
                    {
                        string id = withoutLastChar + (intValue + i);

                        //Console.WriteLine(id);

                        portList.Add(id);

                        if (i == PortNumberSlider.Value - 1)
                        {
                            PortNumberTextBox.Text += $"{id}  ";
                        }
                        else
                        {
                            PortNumberTextBox.Text += $"{id},  ";
                        }


                        //  PortNumberTextBox.Text += $"{id},  ";


                    }


                }
                else
                {
                    for (int i = 1; i < PortNumberSlider.Value + 1; i++)
                    {

                        string id = $"{str}{i}";

                        portList.Add(id);


                        if (i == PortNumberSlider.Value)
                        {
                            PortNumberTextBox.Text += $"{id}  ";
                        }
                        else
                        {
                            PortNumberTextBox.Text += $"{id},  ";
                        }



                    }
                }
            }

        }

        private ObservableCollection<BuildingInfoClass> buildingInfos = new ObservableCollection<BuildingInfoClass>();
        private ObservableCollection<FloorInfoClass> floorInfos = new ObservableCollection<FloorInfoClass>();

        private void AddPortPanelWindow_OnLoaded(object sender, RoutedEventArgs e)
        {



            FloorCombobox.ItemsSource = floorInfos;
            BuildingCombobox.ItemsSource = buildingInfos;
            GroupCombobox.ItemsSource = groups;

            //加载建筑信息
            LoadBuildingInfos();

            //第二步，加载楼层名称

            //第三步，加载房间号

            //第四步，加载端口自定义分组

            //LoadGroups();
        }

        /// <summary>
        /// 取出字符串尾部数字
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ExtractTrailingDigits(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null; // 或返回空字符串 ""

            var match = Regex.Match(input, @"\d+$");
            return match.Success ? match.Value : null;
        }


        /// <summary>
        /// 加载建筑名称
        /// </summary>
        private void LoadBuildingInfos()
        {
            buildingInfos.Clear();

            string sql = "SELECT * FROM Buildings";



            var rows = GlobalVariables.DbService.ExecuteQuery(sql);

            int index = 0;

            foreach (var row in rows)
            {
                index++;
                BuildingInfoClass info = new BuildingInfoClass();

                info.Index = index;
                info.BuildingId = row["BuildingId"].ToString();
                info.Building = row["Building"].ToString();
                info.Address = row["Address"].ToString();
                info.User = row["User"].ToString();
                info.Phone = row["Phone"].ToString();
                info.Note = row["Note"].ToString();

                buildingInfos.Add(info);
            }



            if (DataBridge.DataBridge.SelectBuildingId != null)
            {
                // 使用LINQ查询与targetBuildingId相匹配的BuildingInfoClass实例
                BuildingInfoClass matchedBuildingInfo =
                    buildingInfos.FirstOrDefault(b => b.BuildingId == DataBridge.DataBridge.SelectBuildingId);

                if (matchedBuildingInfo != null)
                {
                    BuildingCombobox.Text = matchedBuildingInfo.Building;

                    // 加载楼层信息
                    LoadFloorInfos(DataBridge.DataBridge.SelectBuildingId);

                    if (DataBridge.DataBridge.SelectFloor != "")
                    {
                        FloorCombobox.Text = DataBridge.DataBridge.SelectFloor;
                    }

                }


            }


        }


        /// <summary>
        /// 加载楼层信息,楼层信息存放在 SlotId 中 SlotId就是Floor
        /// </summary>
        /// <param name="buildingId"></param>
        private void LoadFloorInfos(string buildingId)
        {
            floorInfos.Clear();

            string sql = $"SELECT DISTINCT SlotId FROM Bu_{buildingId}";


            var rows = GlobalVariables.DbService.ExecuteQuery(sql);

            int index = 0;


            foreach (var row in rows)
            {
                index++;
                FloorInfoClass floor = new FloorInfoClass();

                floor.Floor = row["SlotId"].ToString();

                floorInfos.Add(floor);
            }


        }

        private void FloorCombobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FloorCombobox.SelectedIndex != -1)
            {
                //Console.WriteLine(FloorCombobox.SelectedIndex);
                //加载房间号
                RoomNote.Text = "";


                DataBridge.DataBridge.SelectFloor = floorInfos[FloorCombobox.SelectedIndex].Floor;

                LoadRoomId();

                RoomCombobox.ItemsSource = roomIds;

            }

        }


        private ObservableCollection<string> roomIds = new ObservableCollection<string>();

        private void LoadRoomId()
        {
            roomIds.Clear();
            string sql =
                $"SELECT DISTINCT RoomId FROM Bu_{DataBridge.DataBridge.SelectBuildingId}  WHERE SlotId ='{DataBridge.DataBridge.SelectFloor}'";


            var rows = GlobalVariables.DbService.ExecuteQuery(sql);

            foreach (var row in rows)
            {
                string room = row["RoomId"].ToString();

                roomIds.Add(room);
            }


        }

        /// <summary>
        /// 当前选中的楼宇信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BuildingCombobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            floorInfos.Clear();
            roomIds.Clear();
            RoomNote.Text = "";

            if (BuildingCombobox.SelectedIndex != -1)
            {

                string buildingId = buildingInfos[BuildingCombobox.SelectedIndex].BuildingId;

                DataBridge.DataBridge.SelectBuildingId = buildingId;

                LoadFloorInfos(buildingId);


            }



        }


        private ObservableCollection<string> groups = new ObservableCollection<string>();

        /// <summary>
        /// 加载自定义分组
        /// </summary>
        private void LoadGroups()
        {
            string sql = $"SELECT DISTINCT PortGroup FROM PortPanels ";


            var rows = GlobalVariables.DbService.ExecuteQuery(sql);

            int index = 0;

            foreach (var row in rows)
            {
                string group = row["PortGroup"].ToString();
                groups.Add(group);
            }


        }

        /// <summary>
        /// 保存端口面板信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {

            var info = CheckInput();

            if (info.Item1 == 0)
            {
                PreviewButton_OnClick(null, null);

                var buildingId = buildingInfos[BuildingCombobox.SelectedIndex].BuildingId;

                DbClass.CreateDynamicsTableIfNotExists(buildingId, 1);

                var floor = FloorCombobox.Text;

                var room = RoomCombobox.Text;

                var portType = PortTypeCombobox.Text;

                var portGroup = GroupCombobox.Text;

                var roomNote = RoomNote.Text;



                //如果填写了备注则保存
                if (!string.IsNullOrWhiteSpace(roomNote))
                {
                    //先看备注是否存在
                    var noteId = $"{buildingId}{floor}{room}";

                    var query = $"SELECT COUNT(*) FROM Notes WHERE NoteId='{noteId}'";

                    var countNum = DbClass.ExecuteScalarTableNum(query);

                    string sqlNote;

                    if (countNum == 0)
                    {

                        var noteInfo = new { NoteId = noteId, Note = roomNote };

                        //sqlNote = $"INSERT INTO \"Notes\" (\"NoteId\", \"Note\") VALUES ('{noteId}', '{roomNote}')";

                        GlobalVariables.DbService.InsertEntity("Notes", noteInfo);
                    }
                    else
                    {
                        sqlNote = $"UPDATE  Notes  SET  Note  = '{roomNote}' WHERE  NoteId  = '{noteId}'";
                        GlobalVariables.DbService.ExecuteNonQuery(sqlNote);
                    }


                }

                var message = "以下端口已存在,该端口将不会被添加:\r";
                var count = 0;

                foreach (var port in portList)
                {
                    //先看端口是否存在
                    var sqlTemp = $"SELECT COUNT(*) FROM Bu_{buildingId} WHERE SlotId='{floor}' AND RoomId='{room}' AND PortId='{port}'";
                    var countNum = DbClass.ExecuteScalarTableNum(sqlTemp);


                    if (countNum == 0)//端口不存在
                    {
                        var uid = GetNextAvailableNumber(buildingId);

                        var buildingInfo = new
                        {
                            UID = uid,
                            SlotId = floor,
                            RoomId = room,
                            PortType = portType,
                            PortId = port,
                            PortGroup = portGroup,
                            PortColor = portColor
                        };

                        //string sql = $"INSERT INTO \"Bu_{buildingId}\" (\"UID\", \"SlotId\", \"RoomId\", \"PortType\", \"PortId\", \"PortGroup\",  \"PortColor\") VALUES ( '{uid}', '{floor}', '{room}', '{portType}', '{port}', '{portGroup}','{portColor}')";


                        GlobalVariables.DbService.InsertEntity($"Bu_{buildingId}", buildingInfo);

                    }
                    else
                    {
                        message += port + "\r";
                        count++;
                    }



                }

                if (count > 0)
                {
                    MessageBox.Show(message, "存在重复端口", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                //更新房间备注
                UpdateRoomNote(room);

               
            }
            else
            {
                MessageBox.Show(info.Item2, "注意！", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// 检查输入是否完整
        /// </summary>
        /// <returns></returns>
        private (int, string) CheckInput()
        {
            int index = 0;

            string message = "当前存在以下问题需要解决:\r";

            string building = BuildingCombobox.Text;

            string floor = FloorCombobox.Text;

            string room = RoomCombobox.Text;

            string portId = PortIdBox.Text;






            if (building == null || building.Replace(" ", "").Length < 1)
            {
                index++;

                message += index.ToString() + ":未选择端口面板所在建筑\r";
            }


            if (floor == null || floor.Replace(" ", "").Length < 1)
            {
                index++;

                message += index.ToString() + ":未选择(设定)端口所在楼层\r";
            }

            if (room == null || room.Replace(" ", "").Length < 1)
            {
                index++;

                message += index.ToString() + ":未选择(设定)端口所在房间\r";
            }


            if (PortTypeCombobox.SelectedIndex == -1)
            {
                index++;

                message += index.ToString() + ":未选择端口类型\r";
            }

            if (portId == null || portId.Replace(" ", "").Length < 1)
            {
                index++;

                message += index.ToString() + ":未选择(设定)端口编号\r";
            }

            return (index, message);
        }


        /// <summary>
        /// 端口面板房间号选择,选择后加载备注
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoomCombobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //TODO 加载TAG


            //RoomNote.Text = "";

            //if (RoomCombobox.SelectedIndex != -1 )
            //{
            //    string room = roomIds[RoomCombobox.SelectedIndex];



            //    string sql =
            //        $"SELECT RoomNote FROM PortPanels WHERE BuildingId='{DataBridge.DataBridge.SelectBuildingId}' AND Floor='{DataBridge.DataBridge.SelectFloor}' AND RoomNumber='{room}' LIMIT 1";

            //    string roomNote = dbClass.ExecuteScalarQuery(sql);

            //    if (roomNote != null)
            //    {
            //        RoomNote.Text = roomNote;

            //    }
            //}





        }

        /// <summary>
        /// 更新房间备注
        /// </summary>
        private void UpdateRoomNote(string room)
        {
            //string roomNote = RoomNote.Text;

            //string sql =
            //    $"UPDATE \"PortPanels\" SET \"RoomNote\" = '{roomNote}' WHERE BuildingId = '{DataBridge.DataBridge.SelectBuildingId}' AND Floor='{DataBridge.DataBridge.SelectFloor}' AND RoomNumber='{room}'";
            //dbClass.ExecuteQuery(sql);
        }

        /// <summary>
        /// 默认颜色
        /// </summary>
        private int portColor = 0;

        private void PortColor_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = PortColor.SelectedIndex;

            int x = 0;


            if (index != -1)
            {
                foreach (var selectedItem in PortColor.Items)
                {
                    var item = selectedItem as ListBoxItem;

                    if (item != null && index == x)
                    {
                        item.Opacity = 1;
                        item.BorderBrush = SystemColors.ActiveBorderBrush;
                        item.BorderThickness = new Thickness(2);

                        portColor = index;

                    }
                    else
                    {
                        item.Opacity = 0.1;
                        item.BorderBrush = null;
                        item.BorderThickness = new Thickness(0);
                    }

                    x++;
                }

            }
        }


        /// <summary>
        /// 保存并关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAndCloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            var info = CheckInput();

            if (info.Item1 == 0)
            {
                PreviewButton_OnClick(null, null);

                var buildingId = buildingInfos[BuildingCombobox.SelectedIndex].BuildingId;

                DbClass.CreateDynamicsTableIfNotExists(buildingId, 1);

                var floor = FloorCombobox.Text;

                var room = RoomCombobox.Text;

                var portType = PortTypeCombobox.Text;

                var portGroup = GroupCombobox.Text;

                var roomNote = RoomNote.Text;



                //如果填写了备注则保存
                if (!string.IsNullOrWhiteSpace(roomNote))
                {
                    //先看备注是否存在
                    var noteId = $"{buildingId}{floor}{room}";

                    var query = $"SELECT COUNT(*) FROM Notes WHERE NoteId='{noteId}'";

                    var countNum = DbClass.ExecuteScalarTableNum(query);

                    string sqlNote;

                    if (countNum == 0)
                    {

                        var noteInfo = new { NoteId = noteId, Note = roomNote };

                        //sqlNote = $"INSERT INTO \"Notes\" (\"NoteId\", \"Note\") VALUES ('{noteId}', '{roomNote}')";

                        GlobalVariables.DbService.InsertEntity("Notes", noteInfo);
                    }
                    else
                    {
                        sqlNote = $"UPDATE  Notes  SET  Note  = '{roomNote}' WHERE  NoteId  = '{noteId}'";
                        GlobalVariables.DbService.ExecuteNonQuery(sqlNote);
                    }


                }

                var message = "以下端口已存在,该端口将不会被添加:\r";
                var count = 0;

                foreach (var port in portList)
                {
                    //先看端口是否存在
                    var sqlTemp =$"SELECT COUNT(*) FROM Bu_{buildingId} WHERE SlotId='{floor}' AND RoomId='{room}' AND PortId='{port}'";
                    var countNum = DbClass.ExecuteScalarTableNum(sqlTemp);


                    if (countNum == 0)//端口不存在
                    {
                        var uid = GetNextAvailableNumber(buildingId);

                        var buildingInfo = new
                        {
                            UID = uid,
                            SlotId = floor,
                            RoomId = room,
                            PortType = portType,
                            PortId = port,
                            PortGroup = portGroup,
                            PortColor = portColor
                        };

                        //string sql = $"INSERT INTO \"Bu_{buildingId}\" (\"UID\", \"SlotId\", \"RoomId\", \"PortType\", \"PortId\", \"PortGroup\",  \"PortColor\") VALUES ( '{uid}', '{floor}', '{room}', '{portType}', '{port}', '{portGroup}','{portColor}')";


                        GlobalVariables.DbService.InsertEntity($"Bu_{buildingId}", buildingInfo);

                    }
                    else
                    {
                        message += port + "\r";
                        count++;
                    }



                }

                if (count > 0)
                {
                    MessageBox.Show(message, "存在重复端口", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                //更新房间备注
                UpdateRoomNote(room);

                DialogResult = true;
            }
            else
            {
                MessageBox.Show(info.Item2, "注意！", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// 获取下一个可用的编号
        /// </summary>
        /// <param name="tableName">建筑ID</param>
        /// <returns></returns>
        public int GetNextAvailableNumber(string tableName)
        {
            var usedNumbers = new HashSet<int>();

            string sql = $"SELECT UID FROM  Bu_{tableName}"; // 假设Del为0表示未删除的记录   WHERE Del != 1 OR Del IS NULL

            var rows = GlobalVariables.DbService.ExecuteQuery(sql);

            foreach (var row in rows)
            {
                usedNumbers.Add(Convert.ToInt32(row["UID"]));
            }





            int nextNumber = 1; // Start with the smallest possible number
            while (usedNumbers.Contains(nextNumber))
            {
                nextNumber++;
            }

            return nextNumber;
        }

    }
}
