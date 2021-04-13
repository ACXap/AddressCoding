using AddressCoding.BDRepository;
using AddressCoding.Entities.Settings;
using AddressCoding.FileService;
using AddressCoding.Notifications;
using AddressCoding.Repository;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MahApps.Metro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AddressCoding.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IFileService _fileService;
        private readonly INotifications _notification;
        private readonly IBDRepository _bd;
        private IRepository _orpon;

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

        private BDSettings _bdSettings = new BDSettings();
        /// <summary>
        /// 
        /// </summary>
        public BDSettings BDSettings
        {
            get => _bdSettings;
            set => Set(ref _bdSettings, value);
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
                    }, () => !string.IsNullOrEmpty(_repositorySettings.Address) && !string.IsNullOrEmpty(_repositorySettings.NameEndpoint)));

        private RelayCommand _commandCheckConnectBd;
        public RelayCommand CommandCheckConnectBd =>
        _commandCheckConnectBd ?? (_commandCheckConnectBd = new RelayCommand(
                    () =>
                    {
                        TestConectBdAsync();
                    }, () => !string.IsNullOrEmpty(_bdSettings.BDName) && !string.IsNullOrEmpty(_bdSettings.Server)));

        private async void TestConectAsync()
        {
            _repositorySettings.StatusConnect = Entities.StatusConnect.ConnectNow;
            var a = await _orpon.TestConnectAsync();
            if (a != null && a.Error == null)
            {
                _repositorySettings.StatusConnect = Entities.StatusConnect.OK;
                _repositorySettings.Error = string.Empty;
            }
            else if (a != null && a.Error != null)
            {
                _repositorySettings.StatusConnect = Entities.StatusConnect.Error;
                _repositorySettings.Error = a.Error.Message;
            }
        }

        private async void TestConectBdAsync()
        {
            _bdSettings.StatusConnect = Entities.StatusConnect.ConnectNow;
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    var r = _bd.CheckConnect(new ConnectionSettingsDb()
                    {
                        BDName = _bdSettings.BDName,
                        Login = _bdSettings.Login,
                        Password = _bdSettings.Password,
                        Port = _bdSettings.Port,
                        Server = _bdSettings.Server
                    });

                    _bdSettings.StatusConnect = Entities.StatusConnect.OK;
                    _bdSettings.Error = string.Empty;
                }
                catch (Exception ex)
                {
                    _bdSettings.StatusConnect = Entities.StatusConnect.Error;
                    _bdSettings.Error = ex.Message;
                }
            });
        }

        public SettingsViewModel(IFileService fileService, INotifications notifications, IBDRepository bd)
        {
            _fileService = fileService;
            _notification = notifications;
            _bd = bd;
            //_orpon = orpon;

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

            //CommandCheckConnect.Execute(null);
            // _orpon.Initialize(_repositorySettings.Address, _repositorySettings.NameEndpoint);
        }

        public void SetRepository(IRepository orpon)
        {
            _orpon = orpon;
            CommandCheckConnect.Execute(null);
        }

        private void GetSettings()
        {
            var fs = Properties.FileSettings.Default;
            _fileSettings.CanBreakFileOutput = fs.CanBreakFileOutput;
            _fileSettings.MaxSizePart = fs.MaxSizePart;
            _fileSettings.CanUseAnsi = fs.CanUseAnsi;

            var gs = Properties.GeneralSettings.Default;
            _generalSettings.ColorTheme = ThemeManager.GetTheme(gs.Theme);
            _generalSettings.CanOpenFolderAfter = gs.CanOpenFolderAfter;
            _generalSettings.CanSaveDataAsShot = gs.CanSaveDataAsShot;
            _generalSettings.CanSaveDataAsFull = gs.CanSaveDataAsFull;
            _generalSettings.SeparatorChar = gs.SeparatorChar;
            _generalSettings.CanUseParsinglevelRus = gs.CanUseParsinglevelRus;
            _generalSettings.CanSaveFileWhithSelectedField = gs.CanSaveFileWhithSelectedField;
            _generalSettings.CollectionFieldForSave = GetCollectionFieldForSave();

            var rs = Properties.RepositorySettings.Default;
            _repositorySettings.MaxObj = rs.MaxObj;
            _repositorySettings.MaxParallelism = rs.MaxParallelism;
            var add = Helpers.ProtectedDataDPAPI.DecryptData(rs.AddressRepository);
            if (add != null && add.Error == null)
            {
                _repositorySettings.Address = add.Object;
            }
            var end = Helpers.ProtectedDataDPAPI.DecryptData(rs.NameEndpointRepository);
            if (end != null && end.Error == null)
            {
                _repositorySettings.NameEndpoint = end.Object;
            }

            var bs = Properties.BDSettings.Default;
            _bdSettings.BDName = bs.BDName;
            _bdSettings.Port = bs.Port;
            _bdSettings.Login = bs.Login;
            _bdSettings.Server = bs.Server;

            var p = Helpers.ProtectedDataDPAPI.DecryptData(bs.Password);
            if (p != null && p.Error == null)
            {
                _bdSettings.Password = p.Object;
            }
        }

        private string _fileSettingsFields = "fields.dat";

        private List<FieldsForSave> GetCollectionFieldForSave()
        {
            if (File.Exists(_fileSettingsFields))
            {
                try
                {
                    var l = JsonConvert.DeserializeObject<List<FieldsForSave>>(File.ReadAllText(_fileSettingsFields));

                    return l;
                }
                catch
                {
                    return GetDefaultCollectionFieldsForSave();
                }
            }
            else
            {
                return GetDefaultCollectionFieldsForSave();
            }
        }

        private void SaveSettings()
        {
            var fs = Properties.FileSettings.Default;
            var gs = Properties.GeneralSettings.Default;
            var rs = Properties.RepositorySettings.Default;
            var bs = Properties.BDSettings.Default;

            fs.CanBreakFileOutput = _fileSettings.CanBreakFileOutput;
            fs.MaxSizePart = _fileSettings.MaxSizePart;
            fs.CanUseAnsi = _fileSettings.CanUseAnsi;

            gs.Theme = _generalSettings.ColorTheme.Name;
            gs.CanOpenFolderAfter = _generalSettings.CanOpenFolderAfter;
            gs.CanSaveDataAsFull = _generalSettings.CanSaveDataAsFull;
            gs.CanSaveDataAsShot = _generalSettings.CanSaveDataAsShot;
            gs.SeparatorChar = _generalSettings.SeparatorChar;
            gs.CanUseParsinglevelRus = _generalSettings.CanUseParsinglevelRus;
            gs.CanSaveFileWhithSelectedField = _generalSettings.CanSaveFileWhithSelectedField;

            rs.MaxObj = _repositorySettings.MaxObj;
            rs.MaxParallelism = _repositorySettings.MaxParallelism;
            var add = Helpers.ProtectedDataDPAPI.EncryptData(_repositorySettings.Address);
            if (add != null && add.Error == null)
            {
                rs.AddressRepository = add.Object;
            }
            var end = Helpers.ProtectedDataDPAPI.EncryptData(_repositorySettings.NameEndpoint);
            if (end != null && end.Error == null)
            {
                rs.NameEndpointRepository = end.Object;
            }

            bs.BDName = _bdSettings.BDName;
            bs.Server = _bdSettings.Server;
            bs.Login = _bdSettings.Login;
            bs.Port = _bdSettings.Port;

            var p = Helpers.ProtectedDataDPAPI.EncryptData(_bdSettings.Password);
            if (p != null && p.Error == null)
            {
                bs.Password = p.Object;
            }

            try
            {
                fs.Save();
                gs.Save();
                rs.Save();
                bs.Save();

                SaveField();
                _notification.NotificationAsync(null, "Ok");
            }
            catch (Exception ex)
            {
                _notification.NotificationAsync(null, ex.Message);
            }
        }

        private void SaveField()
        {
            var str = JsonConvert.SerializeObject(_generalSettings.CollectionFieldForSave);
            File.WriteAllText(_fileSettingsFields, str);
        }

        private List<FieldsForSave> GetDefaultCollectionFieldsForSave()
        {
            var list = new List<FieldsForSave>()
            {
                new FieldsForSave(){CanSave = true, Name = "Адрес", Description="Орпонизируемый адрес" },
                new FieldsForSave(){CanSave = false, Name = "Адрес ОРПОН", Description="Адрес из ОРПОН по ГИДу (для получения требуется подключение к базе ЦХД)" },
                new FieldsForSave(){CanSave = true, Name = "QualityCode", Description="" },
                new FieldsForSave(){CanSave = true, Name = "CheckStatus", Description="" },
                new FieldsForSave(){CanSave = true, Name = "UnparsedParts", Description="Неразобранные части адреса" },
                new FieldsForSave(){CanSave = true, Name = "ParsingLevelCode", Description="Уровень разбора адреса" },
                new FieldsForSave(){CanSave = true, Name = "GlobalID", Description="ГИД адреса" },
                new FieldsForSave(){CanSave = false, Name = "SystemCode", Description="" },
                new FieldsForSave(){CanSave = false, Name = "KLADRLocalityId", Description="" },
                new FieldsForSave(){CanSave = false, Name = "FIASLocalityId", Description="" },
                new FieldsForSave(){CanSave = false, Name = "LocalityGlobalId", Description="" },
                new FieldsForSave(){CanSave = false, Name = "KLADRStreetId", Description="" },
                new FieldsForSave(){CanSave = false, Name = "FIASStreetId", Description="" },
                new FieldsForSave(){CanSave = false, Name = "StreetKind", Description="" },
                new FieldsForSave(){CanSave = false, Name = "StreetGlobalId", Description="" },
                new FieldsForSave(){CanSave = false, Name = "Street", Description="" },
                new FieldsForSave(){CanSave = false, Name = "House", Description="" },
                new FieldsForSave(){CanSave = false, Name = "HouseLitera", Description="" },
                new FieldsForSave(){CanSave = false, Name = "CornerHouse", Description="" },
                new FieldsForSave(){CanSave = false, Name = "BuildingBlock", Description="" },
                new FieldsForSave(){CanSave = false, Name = "BuildingBlockLitera", Description="" },
                new FieldsForSave(){CanSave = false, Name = "Building", Description="" },
                new FieldsForSave(){CanSave = false, Name = "BuildingLitera", Description="" },
                new FieldsForSave(){CanSave = false, Name = "Ownership", Description="" },
                new FieldsForSave(){CanSave = false, Name = "OwnershipLitera", Description="" },
                new FieldsForSave(){CanSave = false, Name = "FIASHouseId", Description="" },
                new FieldsForSave(){CanSave = false, Name = "HouseGlobalId", Description="" },
                new FieldsForSave(){CanSave = false, Name = "Latitude", Description="Широта" },
                new FieldsForSave(){CanSave = false, Name = "Longitude", Description="Долгота" },
                new FieldsForSave(){CanSave = false, Name = "LocationDescription", Description="Описание местоположения" },
                new FieldsForSave(){CanSave = false, Name = "Error", Description="Ошибка при орпонизации" }
            };

            return list;
        }
    }
}