using GalaSoft.MvvmLight;

namespace AddressCoding.Entities.Settings
{
    public class RepositorySettings:ViewModelBase
    {
        private string _address = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Address
        {
            get => _address;
            set => Set(ref _address, value);
        }

        private string _nameEndpoint = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string NameEndpoint
        {
            get => _nameEndpoint;
            set => Set(ref _nameEndpoint, value);
        }

        private StatusConnect _statusConnect = StatusConnect.NotConnect;
        /// <summary>
        /// 
        /// </summary>
        public StatusConnect StatusConnect
        {
            get => _statusConnect;
            set => Set(ref _statusConnect, value);
        }

        private int _maxObj = 200;
        /// <summary>
        /// 
        /// </summary>
        public int MaxObj
        {
            get => _maxObj;
            set
            {
                if(value<1)
                {
                    Set(ref _maxObj, 1);
                }

                if(value>500)
                {
                    Set(ref _maxObj, 500);
                }
                if(value<=500 && value>0)
                {
                    Set(ref _maxObj, value);
                }
            }
        }



        private int _maxParallelism = 4;
        /// <summary>
        /// 
        /// </summary>
        public int MaxParallelism
        {
            get => _maxParallelism;
            set
            {
                if(value<1)
                {
                    Set(ref _maxParallelism, 1);
                }
                if(value>8)
                {
                    Set(ref _maxParallelism, 8);
                }
                if(value<=8 && value>0)
                {
                    Set(ref _maxParallelism, value);
                }
            }
        }

        private string _error = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Error
        {
            get => _error;
            set => Set(ref _error, value);
        }
    }
}