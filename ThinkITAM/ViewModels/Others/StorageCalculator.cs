using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ThinkITAM.ViewModels.Others
{
    public class StorageCalculatorViewModel : INotifyPropertyChanged
    {
        private string codingType;//编码类型
        private string resolution;//分辨率
        private int cameraNumber;//摄像头数量
        private int saveDay;//存储天数
        private string storageSpace;//存储空间


        public string CodingType
        {
            get => codingType;
            set
            {
                if (codingType != value)
                {
                    codingType = value;
                    OnPropertyChanged(nameof(CodingType));
                }
            }
        }

        public string Resolution
        {
            get => resolution;
            set
            {
                if (resolution != value)
                {
                    resolution = value;
                    OnPropertyChanged(nameof(Resolution));
                }
            }
        }

        public int CameraNumber
        {
            get => cameraNumber;
            set
            {
                if (cameraNumber != value)
                {
                    cameraNumber = value;
                    OnPropertyChanged(nameof(CameraNumber));
                }
            }
        }

        public int SaveDay
        {
            get => saveDay;
            set
            {
                if (saveDay != value)
                {
                    saveDay = value;
                    OnPropertyChanged(nameof(SaveDay));
                }
            }
        }

        public string StorageSpace
        {
            get => storageSpace;
            set
            {
                if (storageSpace != value)
                {
                    storageSpace = value;
                    OnPropertyChanged(nameof(StorageSpace));
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }



    public class StorageBitRateViewModel : INotifyPropertyChanged
    {
        private string bitRate;//码率
        private int cameraNumber;//摄像头数量
        private int saveDay;//存储天数
        private string storageSpace;//存储空间



        public string BitRate
        {
            get => bitRate;
            set
            {
                if (bitRate != value)
                {
                    bitRate = value;
                    OnPropertyChanged(nameof(BitRate));
                }
            }
        }






        public int CameraNumber
        {
            get => cameraNumber;
            set
            {
                if (cameraNumber != value)
                {
                    cameraNumber = value;
                    OnPropertyChanged(nameof(CameraNumber));
                }
            }
        }

        public int SaveDay
        {
            get => saveDay;
            set
            {
                if (saveDay != value)
                {
                    saveDay = value;
                    OnPropertyChanged(nameof(SaveDay));
                }
            }
        }

        public string StorageSpace
        {
            get => storageSpace;
            set
            {
                if (storageSpace != value)
                {
                    storageSpace = value;
                    OnPropertyChanged(nameof(StorageSpace));
                }
            }
        }




        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
