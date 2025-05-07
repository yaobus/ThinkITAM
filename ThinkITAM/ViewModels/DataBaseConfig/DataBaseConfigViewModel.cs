using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ThinkITAM.ViewModels.DataBaseConfig
{
    public class DataBaseConfigViewModel : INotifyPropertyChanged
    {

        private string type;
        public string Type
        {
            get => type;
            set
            {
                if (type != value)
                {
                    type = value;
                    OnPropertyChanged(nameof(Type));
                }
            }
        }

        private string nickName;
        public string NickName   {
            get => nickName;
            set
            {
                if (nickName != value)
                {
                    nickName = value;
                    OnPropertyChanged(nameof(NickName));
                }
            }
        }

        private string path;

        public string Path
        {
            get => path;
            set
            {
                if (path != value)
                {
                    path = value;
                    OnPropertyChanged(nameof(Path));
                }
            }
        }



        private string host;

        public string Host
        {
            get => host;
            set
            {
                if (host != value)
                {
                    host = value;
                    OnPropertyChanged(nameof(Host));
                }
            }
        }


        private int  port;
        public int Port  {
            get => port;
            set
            {
                if (port != value)
                {
                    port = value;
                    OnPropertyChanged(nameof(Port));
                }
            }
        }

        private string userName;
        public string UserName   {
            get => userName;
            set
            {
                if (userName != value)
                {
                    userName = value;
                    OnPropertyChanged(nameof(UserName));
                }
            }
        }

        private string password;
        public string Password   {    
            get => password;
            set
            {
                if (password != value)
                {
                    password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        private string databaseName;
        public string DatabaseName   {    
            get => databaseName;
            set
            {
                if (databaseName != value)
                {
                    databaseName = value;
                    OnPropertyChanged(nameof(DatabaseName));
                }
            }
        }




        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
