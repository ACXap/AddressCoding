using GalaSoft.MvvmLight;

namespace AddressCoding.Entities.Settings
{
    public class BDSettings:ViewModelBase
    {

        private string _bdName = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string BDName
        {
            get => _bdName;
            set => Set(ref _bdName, value);
        }

        private string _server = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Server
        {
            get => _server;
            set => Set(ref _server, value);
        }

        private int _port = 5432;
        /// <summary>
        /// 
        /// </summary>
        public int Port
        {
            get => _port;
            set => Set(ref _port, value);
        }

        private string _login = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Login
        {
            get => _login;
            set => Set(ref _login, value);
        }

        private string _password = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
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