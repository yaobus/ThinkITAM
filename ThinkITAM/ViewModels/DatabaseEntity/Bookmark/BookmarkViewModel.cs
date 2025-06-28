namespace ThinkITAM.ViewModels.DatabaseEntity.Bookmark
{


    /// <summary>
    /// 书签，往数据库中添加的类
    /// </summary>
    public class BookmarkViewModel
    {
        public string IndexId
        {
            get; set;
        }
        public string TypeGroup
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public string Protocol
        {
            get; set;
        }
        public string Host
        {
            get; set;
        }

        public string Port
        {
            get; set;
        }

        public int Color
        {
            get; set;
        }

        public string Browser
        {
            get; set;
        }

        public int PinToStart
        {
            get; set;
        }
    }
}
