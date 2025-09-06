using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkITAM.ViewModels.Others
{
   public class NoteBookViewModel
    {
        public string NoteGroup
        {
            get; set;
        }
        public string NoteUnit
        {
            get; set;
        }
        public string NoteName
        {
            get; set;
        }

        public string NoteId
        {
            get;
            set;
        }
        public string CreatedDate
        {
            get;
            set;
        }

        public string EditDate
        {
            get;
            set;
        }

        public string Note
        {
            get;
            set;
        }

        public int DisplayOrder
        {
            get;
            set;
        }

        public int Del
        {
            get;
            set;
        }
    }




    // 表示TreeView的节点
    public class TreeItem
    {
        public string Name
        {
            get; set;
        }
        public object Tag
        {
            get; set;
        } // 可选：用于存储原始数据
        public ObservableCollection<TreeItem> Children { get; set; } = new ObservableCollection<TreeItem>();
    }
}
