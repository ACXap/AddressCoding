using AddressCoding.Entities.Settings;
using AddressCoding.FileService;
using AddressCoding.Notifications;
using AddressCoding.Repository;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MahApps.Metro;
using System;
using System.IO;

namespace AddressCoding.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IFileService _fileService;
        private readonly INotifications _notification;
        private readonly IRepository _orpon;

        private FileSettings _fileSettings = new FileSettings();
        /// <summary>
        /// Настройки работы с файлами
        /// </summary>
        public FileSettings FileSettings
        {
            get => _fileSettings;
            set => Set(ref _fileSettings, value);
        }

        private GeneralSettings _generalSettings = new GeneralSettings();
        /// <summary>
        /// Общие настройки приложения
        /// </summary>
        public GeneralSettings GeneralSettings
        {
            get => _generalSettings;
            set => Set(ref _generalSettings, value);
        }

        private RepositorySettings _repositorySettings = new RepositorySettings();
        /// <summary>
        /// 
        /// </summary>
        public RepositorySettings RepositorySettings
        {
            get => _repositorySettings;
            set => Set(ref _repositorySettings, value);
        }

        /// <summary>
        /// Поле для хранения команды открытия папки
        /// </summary>
        private RelayCommand<string> _commandOpenFolder;

        /// <summary>
        /// Команда открытия папки
        /// </summary>
        public RelayCommand<string> CommandOpenFolder =>
        _commandOpenFolder ?? (_commandOpenFolder = new RelayCommand<string>(
            obj =>
            {
                var result = _fileService.OpenFolder(obj);
                if (result != null && result.Error != null)
                {
                    _notification.NotificationAsync(null, result.Error.Message);
                }
            }));

        private RelayCommand _commandSaveSettings;
        public RelayCommand CommandSaveSettings =>
        _commandSaveSettings ?? (_commandSaveSettings = new RelayCommand(
                    () =>
                    {
                        SaveSettings();
                    }));

        private RelayCommand _commandCheckConnect;
        public RelayCommand CommandCheckConnect =>
        _commandCheckConnect ?? (_commandCheckConnect = new RelayCommand(
                    () =>
                    {
                        _orpon.Initialize(_repositorySettings.Address, _repositorySettings.NameEndpoint);
                        TestConectAsync();
                    }));

        private async void TestConectAsync()
        {
            _repositorySettings.StatusConnect = Entities.StatusConnect.ConnectNow;
            var a = await _orpon.TestConnectAsync();
            if(a!=null && a.Error==null)
            {
                _repositorySettings.StatusConnect = Entities.StatusConnect.OK;
                _repositorySettings.Error = string.Empty;
            }
            else if(a!=null && a.Error!=null)
            {
                _repositorySettings.StatusConnect = Entities.StatusConnect.Error;
                _repositorySettings.Error = a.Error.Message;
            }

        }

        public SettingsViewModel(IFileService fileService, INotifications notifications, IRepository orpon)
        {
            _fileService = fileService;
            _notification = notifications;
            _orpon = orpon;

            var dir = Directory.GetCurrentDirectory();
            _fileSettings.FolderOutput = dir + "\\Output";
            _fileSettings.FolderInput = dir + "\\Input";
            _fileSettings.FolderTemp = dir + "\\Temp";
            _fileSettings.FolderStatistics = dir + "\\Statistics";
            _fileSettings.FolderErrors = dir + "\\Error";
            _fileSettings.FolderApp = dir;

            var result = _fileService.CreateFolder(new string[]
            {
                _fileSettings.FolderOutput,
                _fileSettings.FolderInput,
                _fileSettings.FolderTemp,
                _fileSettings.FolderStatistics,
                _fileSettings.FolderErrors
            });

            if (result != null && result.Error != null)
            {
                _notification.NotificationAsync(null, result.Error.Message);
            }

            GetSettings();

            _orpon.Initialize(_repositorySettings.Address, _repositorySettings.NameEndpoint);
        }

        private void GetSettings()
        {
            var fs = Properties.FileSettings.Default;
            _fileSettings.CanBreakFileOutput = fs.CanBreakFileOutput;
            _fileSettings.MaxSizePart = fs.MaxSizePart;

            var gs = Properties.GeneralSettings.Default;
            _generalSettings.ColorTheme = ThemeManager.GetTheme(gs.Theme);

            var rs = Properties.RepositorySettings.Default;
            var add = Helpers.ProtectedDataDPAPI.DecryptData(rs.AddressRepository);
            if(add!=null && add.Error==null)
            {
                _repositorySettings.Address = add.Object;
            }
            var end = Helpers.ProtectedDataDPAPI.DecryptData(rs.NameEndpointRepository);
            if(end !=null && end.Error == null)
            {
                _repositorySettings.NameEndpoint = end.Object;
            }
        }

        private void SaveSettings()
        {
            var fs = Properties.FileSettings.Default;
            var gs = Properties.GeneralSettings.Default;
            var rs = Properties.RepositorySettings.Default;
            fs.CanBreakFileOutput = _fileSettings.CanBreakFileOutput;
            fs.MaxSizePart = _fileSettings.MaxSizePart;

            gs.Theme = _generalSettings.ColorTheme.Name;

            var add = Helpers.ProtectedDataDPAPI.EncryptData(_repositorySettings.Address);
            if(add!=null && add.Error==null)
            {
                rs.AddressRepository = add.Object;
            }
            var end = Helpers.ProtectedDataDPAPI.EncryptData(_repositorySettings.NameEndpoint);
            if(end!=null && end.Error==null)
            {
                rs.NameEndpointRepository = end.Object;
            }

            try
            {
                fs.Save();
                gs.Save();
                rs.Save();
                _notification.NotificationAsync(null, "Ok");
            }
            catch (Exception ex)
            {
                _notification.NotificationAsync(null, ex.Message);
            }
        }
    }
}