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