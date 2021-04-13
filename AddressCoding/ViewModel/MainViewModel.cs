using AddressCoding.BDRepository;
using AddressCoding.Entities;
using AddressCoding.FileService;
using AddressCoding.Notifications;
using AddressCoding.Repository;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AddressCoding.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private CancellationTokenSource cts;

        #region PrivateField
        /// <summary>
        /// Поле для хранения ссылки на модуль работы с файлами
        /// </summary>
        private readonly IFileService _fileService;
        /// <summary>
        /// Поле для хранения ссылки на модуль работы с орпоном
        /// </summary>
        private readonly IRepository _orpon;

        private readonly IBDRepository _bd;

        /// <summary>
        /// Поле для хранения ссылки на модуль работы с оповещениями
        /// </summary>
        private readonly INotifications _notification;
        /// <summary>
        /// Поле для хранения ссылки на модуль работы со статистикой
        /// </summary>
        private readonly StatisticsViewModel _stat;
        /// <summary>
        /// Поле для хранения ссылки на модуль работы с настройками
        /// </summary>
        private readonly SettingsViewModel _set;
        /// <summary>
        /// Поле для хранения названия столбцов файла для орпанизации геокодера
        /// </summary>
        private readonly string _columnNameForGeoCodingOrpon = @"globalid;address;AddressWeb;Longitude;Latitude;Qcode;Error;Status;DateTimeGeoCod;Kind;Precision;CountResult;Proxy";
        /// <summary>
        /// Поле для хранения ссылки на коллекцию данных
        /// </summary>
        private ObservableCollection<EntityOrpon> _collection;
        /// <summary>
        /// Поле для хранения ссылки на текущий выделенный элемент
        /// </summary>
        private EntityOrpon _currentOrpon;
        /// <summary>
        /// Поле для хранения запущена ли процедура
        /// </summary>
        private bool _isStartOrponing = false;

        private bool _isOrponinGeoData;
        /// <summary>
        /// Поле для хранения ссылки на команду обработки перетаскивания
        /// </summary>
        private RelayCommand<DragEventArgs> _commandDragDrop;
        /// <summary>
        /// Поле для хранения команды получения файла
        /// </summary>
        private RelayCommand _commandGetFile;
        /// <summary>
        /// Поле для хранения команды получения данных из файла
        /// </summary>
        private RelayCommand _commandGetDataFromFile;
        /// <summary>
        /// Поле для хранения команды выбора файла для сохранения
        /// </summary>
        private RelayCommand _commandSetFileOutput;
        /// <summary>
        /// Поле для хранения команды для запуска орпонизации
        /// </summary>
        private RelayCommand _commandGetAllOrpon;
        /// <summary>
        /// Поле для хранения команды остановки орпонизации
        /// </summary>
        private RelayCommand _commandStopOrponing;
        /// <summary>
        /// Поле для хранения команды открытия папки
        /// </summary>
        private RelayCommand<string> _commandOpenFolder;
        /// <summary>
        /// Поле для хранения команды записи данных в файл
        /// </summary>
        private RelayCommand _commandSaveData;
        /// <summary>
        /// Поле для хранения команды орпонизации выбранного объекта
        /// </summary>
        private RelayCommand<EntityOrpon> _сommandGetOrpon;
        /// <summary>
        /// Поле для хранения команды копирования адреса в буфер
        /// </summary>
        private RelayCommand _commandCopyAddress;
        /// <summary>
        /// Поле для хранения команды очистки коллекции
        /// </summary>
        private RelayCommand _commandClearCollection;
        #endregion PrivateField

        #region PublicProperties
        /// <summary>
        /// Коллекция данных
        /// </summary>
        public ObservableCollection<EntityOrpon> Collection
        {
            get => _collection;
            set => Set(ref _collection, value);
        }
        /// <summary>
        /// Текущий выделенный элемент
        /// </summary>
        public EntityOrpon CurrentOrpon
        {
            get => _currentOrpon;
            set => Set(ref _currentOrpon, value);
        }
        /// <summary>
        /// Запущена ли процедура
        /// </summary>
        public bool IsStartOrponing
        {
            get => _isStartOrponing;
            set
            {
                Set(ref _isStartOrponing, value);
                IsRequestedStop = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsOrponingGeoData
        {
            get => _isOrponinGeoData;
            set
            {
                Set(ref _isOrponinGeoData, value);
                if (value)
                {
                    IndexTab = 1;
                }
            }
        }

        #endregion PublicProperties

        #region PublicCommand

        /// <summary>
        /// Команда обработки перетаскивания файлов на окно программы
        /// </summary>
        public RelayCommand<DragEventArgs> CommandDragDrop =>
                _commandDragDrop ?? (_commandDragDrop = new RelayCommand<DragEventArgs>(
                            obj =>
                            {
                                if (obj.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
                                {
                                    var files = (string[])obj.Data.GetData(DataFormats.FileDrop, true);
                                    if (files.Length > 0)
                                    {
                                        SetFileInput(files[0]);
                                    }
                                }
                            }, obj => !_isStartOrponing));

        /// <summary>
        /// Команда для получения файла
        /// </summary>
        public RelayCommand CommandGetFile =>
        _commandGetFile ?? (_commandGetFile = new RelayCommand(
                    () =>
                    {
                        var result = _fileService.GetFile();
                        if (result != null && result.Result && result.Error == null)
                        {
                            SetFileInput(result.Object);
                        }
                        else if (result != null && result.Error != null)
                        {
                            _notification.NotificationAsync(null, result.Error.Message);
                        }
                    }, () => !_isStartOrponing));

        /// <summary>
        /// Команда получения данных из файла
        /// </summary>
        public RelayCommand CommandGetDataFromFile =>
        _commandGetDataFromFile ?? (_commandGetDataFromFile = new RelayCommand(
                    () =>
                    {
                        if (CanGetDataFromFile())
                        {
                            GetDataFromFile();
                        }
                    }, () => CanGetDataFromFile()));

        /// <summary>
        /// Команда для выбора файла для сохранения данных
        /// </summary>
        public RelayCommand CommandSetFileOutput =>
           _commandSetFileOutput ?? (_commandSetFileOutput = new RelayCommand(
           () =>
           {
               var result = _fileService.SetFileForSave();
               if (result != null && result.Result && result.Error == null)
               {
                   _set.FileSettings.FileOutput = result.Object;
               }
               else if (result != null && result.Error != null)
               {
                   _notification.NotificationAsync(null, result.Error.Message);
               }
           }));

        /// <summary>
        /// Команда запуска орпонизации
        /// </summary>
        public RelayCommand CommandGetAllOrpon =>
        _commandGetAllOrpon ?? (_commandGetAllOrpon = new RelayCommand(
            () =>
            {
                GetOrponsAsync();

            }, () => CanStartOrponing()));

        /// <summary>
        /// Команда для остановки процесса орпонизации
        /// </summary>
        public RelayCommand CommandStopOrponing =>
        _commandStopOrponing ?? (_commandStopOrponing = new RelayCommand(
            () =>
            {
                cts.Cancel();
                IsRequestedStop = true;
            }, () => _isStartOrponing));

        /// <summary>
        /// Команда запуска орпонизации выбранного объекта
        /// </summary>
        public RelayCommand<EntityOrpon> CommandGetOrpon =>
        _сommandGetOrpon ?? (_сommandGetOrpon = new RelayCommand<EntityOrpon>(
                    obj =>
                    {
                        GetOrponAsync(obj);
                        _stat.UpdateStatisticsCollection();
                    }));

        /// <summary>
        /// Команда для копирования адреса в буфер
        /// </summary>
        public RelayCommand CommandCopyAddress =>
        _commandCopyAddress ?? (_commandCopyAddress = new RelayCommand(
                    () =>
                    {
                        try
                        {
                            Clipboard.SetText(_currentOrpon.Address, TextDataFormat.UnicodeText);
                        }
                        catch (Exception ex)
                        {
                            _notification.NotificationAsync(null, ex.Message);
                        }
                    }));

        /// <summary>
        /// Команда очистки коллекции
        /// </summary>
        public RelayCommand CommandClearCollection =>
        _commandClearCollection ?? (_commandClearCollection = new RelayCommand(
                    () =>
                    {
                        if (CanClearCollection())
                        {
                            _collection.Clear();
                        }
                    }, () => CanClearCollection()));

        /// <summary>
        /// Команда открытия папки
        /// </summary>
        public RelayCommand<string> CommandOpenFolder =>
        _commandOpenFolder ?? (_commandOpenFolder = new RelayCommand<string>(
            obj =>
            {
                OpenFolder(obj);
            }));

        /// <summary>
        /// Команда для сохранения данных в файл
        /// </summary>
        public RelayCommand CommandSaveData =>
        _commandSaveData ?? (_commandSaveData = new RelayCommand(
            async () =>
            {
                if (CanSaveFile())
                {
                    await SaveDataAsync();
                }
                else
                {
                    _notification.NotificationAsync(null, "Error");
                }
            }, () => CanSaveFile()));

        #endregion PublicCommand

        #region PrivateMethod
        private void OpenFolder(string obj)
        {
            if (obj == "AppFolder")
            {
                obj = _set.FileSettings.FolderApp;
            }
            var result = _fileService.OpenFolder(obj);
            if (result != null && result.Error != null)
            {
                _notification.NotificationAsync(null, result.Error.Message);
            }
        }

        /// <summary>
        /// Метод для установки имени входного файла
        /// </summary>
        /// <param name="file">Имя файла</param>
        private void SetFileInput(string file)
        {
            _set.FileSettings.FileInput = file;
            GetDataFromFile();

            _set.FileSettings.FileOutput = GetDefaultName();
        }

        /// <summary>
        /// Метод для получения имени файла по умолчанию
        /// </summary>
        /// <returns></returns>
        private string GetDefaultName()
        {
            string defName = string.Empty;

            if (_collection != null && _collection.Any())
            {
                defName = $"{DateTime.Now.ToString("yyyy_MM_dd")}_{Path.GetFileNameWithoutExtension(_set.FileSettings.FileInput)}_{_collection.Count}.csv";
            }
            else
            {
                defName = $"{DateTime.Now.ToString("yyyy_MM_dd")}_{Path.GetFileNameWithoutExtension(_set.FileSettings.FileInput)}.csv";
            }

            return $"{_set.FileSettings.FolderOutput}\\{defName}";
        }

        /// <summary>
        /// Метод получения данных из файла
        /// </summary>
        private void GetDataFromFile()
        {
            var result = _fileService.GetData(_set.FileSettings.FileInput, _set.FileSettings.CanUseAnsi);
            var id = 0;
            if (result != null && result.Error == null && result.Objects != null)
            {
                if (result.Objects.First() == _columnNameForGeoCodingOrpon)
                {
                    IsOrponingGeoData = true;
                    var list = new List<EntityOrpon>(result.Objects.Count() - 1);
                    foreach (var item in result.Objects.Skip(1))
                    {
                        var entity = new EntityOrpon();
                        var str = item.Split(';');
                        entity.Address = str[2].TrimStart(new char[] { '"' });

                        entity.GlobalIdOriginal = str[0];
                        entity.Longitude = str[3];
                        entity.Latitude = str[4];

                        int.TryParse(str[5], out int qcode);
                        entity.QCode = qcode;

                        list.Add(entity);
                    }

                    Collection = new ObservableCollection<EntityOrpon>(list);
                }
                else
                {
                    Collection = new ObservableCollection<EntityOrpon>(result.Objects.Select(x =>
                    {
                        return new EntityOrpon() { Id = id++, Address = x.TrimStart(new char[] { '"' }) };
                    }));
                }

                if (_collection != null)
                {
                    _stat.Init(_collection);
                }
            }
            else if (result != null && result.Error != null)
            {
                _notification.NotificationAsync(null, result.Error.Message);
            }
        }

        /// <summary>
        /// Метод для получения орпон объекта
        /// </summary>
        private async void GetOrponsAsync()
        {
            if (_set.RepositorySettings.StatusConnect != StatusConnect.OK)
            {
                _notification.NotificationAsync(null, "Not set connect OrponService");
                return;
            }

            IEnumerable<IEnumerable<EntityOrpon>> listAddress = null;

            if (_isOrponinGeoData)
            {
                if (_set.GeneralSettings.CanOrponingGetAll)
                {
                    listAddress = _collection.Where(x => x.QCode == 1).Partition(_set.RepositorySettings.MaxObj);
                }
                else if (_set.GeneralSettings.CanOrponingGetError)
                {
                    listAddress = _collection.Where(x => x.Status == StatusType.Error && x.QCode == 1).Partition(_set.RepositorySettings.MaxObj);
                }
                else
                {
                    listAddress = _collection.Where(x => x.Status == StatusType.NotOrponing && x.QCode == 1).Partition(_set.RepositorySettings.MaxObj);
                }
            }
            else
            {
                if (_canGetOrponForParsingLevel)
                {
                    listAddress = _collection.Where(x => x.Orpon?.ParsingLevelCode == _currentParsingLevel).Partition(_set.RepositorySettings.MaxObj);
                }
                else if (_set.GeneralSettings.CanOrponingGetAll)
                {
                    listAddress = _collection.Partition(_set.RepositorySettings.MaxObj);
                }
                else if (_set.GeneralSettings.CanOrponingGetError)
                {
                    listAddress = _collection.Where(x => x.Status == StatusType.Error).Partition(_set.RepositorySettings.MaxObj);
                }
                else
                {
                    listAddress = _collection.Where(x => x.Status == StatusType.NotOrponing).Partition(_set.RepositorySettings.MaxObj);
                }
            }

            if (!listAddress.Any())
            {
                _notification.NotificationAsync(null, "Data null");
                return;
            }

            IsStartOrponing = true;
            _stat.Start();

            cts = new CancellationTokenSource();
            var t = cts.Token;

            ParallelOptions po = new ParallelOptions()
            {
                MaxDegreeOfParallelism = _set.RepositorySettings.MaxParallelism,
                CancellationToken = t
            };

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        Parallel.ForEach(listAddress, po, (item) =>
                        {
                            //var add = item.Select(x =>
                            //{
                            //    x.Status = StatusType.OrponingNow;
                            //    return x.Address;
                            //}).ToArray();

                            var a = _orpon.GetOrpon(item, _set.RepositorySettings.CanCheckSinglObj);
                            //if (a != null && a.Error == null)
                            //{
                            //    var indexRow = 0;
                            //    foreach (var k in a.Objects)
                            //    {
                            //        item.ElementAt(indexRow).Orpon = k;
                            //        item.ElementAt(indexRow).Status = StatusType.OK;
                            //        indexRow++;
                            //    }
                            //}
                            //else if (a != null && a.Error != null)
                            //{
                            //    foreach (var i in item)
                            //    {
                            //        i.Error = a.Error.Message;
                            //        i.Status = StatusType.Error;
                            //    }
                            //}
                        });

                        _notification.NotificationAsync(null, "Орпонизация завершена.");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }, t);

                if (_isOrponinGeoData)
                {
                    foreach (var item in _collection)
                    {
                        if (string.IsNullOrEmpty(item.Orpon?.GlobalID))
                        {
                            item.QCodeNew = item.QCode;
                        }
                        else
                        {
                            if (item.GlobalIdOriginal != item.Orpon.GlobalID)
                            {
                                item.QCodeNew = 2;
                            }
                            else
                            {
                                item.QCodeNew = item.QCode;
                            }
                        }
                    }
                }
                if (_isOrponinGeoData)
                {
                    SaveDataGeo();
                }
                else
                {
                    await SaveDataAsync();
                }

                _stat.Stop();
                _stat.SaveStatistics();
            }
            catch (Exception ex)
            {
                _notification.NotificationAsync(null, ex.Message);
            }

            IsStartOrponing = false;
            _stat.Stop();
        }

        /// <summary>
        /// Метод для проверки возможности записать данные в выходной файл
        /// </summary>
        /// <returns>Возвращает true если коллекция существует, имеет объекты и есть выходной файл</returns>
        private bool CanSaveFile()
        {
            return _collection != null && _collection.Any() && !string.IsNullOrEmpty(_set.FileSettings.FileOutput);
        }

        /// <summary>
        /// Метод для проверки возможности очистки коллекции
        /// </summary>
        /// <returns>Возвращает true, если процесс орпонизации не запущен, коллекция существует, в коллекции есть элементы</returns>
        private bool CanClearCollection()
        {
            return !_isStartOrponing && _collection != null && _collection.Any();
        }

        /// <summary>
        /// Метод для проверки возможности получения данных из файла
        /// </summary>
        /// <returns>Возвращает true, если процесс орпонизации не запущен, есть файл</returns>
        private bool CanGetDataFromFile()
        {
            return !_isStartOrponing && !string.IsNullOrEmpty(_set.FileSettings.FileInput);
        }

        /// <summary>
        /// Метод для проверки возможности запуска орпонизации
        /// </summary>
        /// <returns>Возвращает true, если процесс орпонизации не запущен, коллекция существует, коллекция имеет элементы</returns>
        private bool CanStartOrponing()
        {
            return !_isStartOrponing && _collection != null && _collection.Any();
        }

        /// <summary>
        /// метод для проверки возможности орпонизации выбранного объекта
        /// </summary>
        /// <returns>Возвращает true, если элемент не равен null</returns>
        private bool CanGetOrpon()
        {
            return _currentOrpon != null;
        }

        /// <summary>
        /// Метод для сохранения данных в файл
        /// </summary>
        private async Task SaveDataAsync()
        {
            var separator = _set.GeneralSettings.SeparatorChar;

            if (_set.GeneralSettings.CanSaveDataAsShot)
            {
                var data = new List<string>(_collection.Count)
                    {
                        $"Адрес{separator}QualityCode{separator}CheckStatus{separator}ParsingLevelCode{separator}GlobalID"
                    };
                data.AddRange(_collection.Select(x =>
                {
                    return $"{x.Address}{separator}{x.Orpon?.QualityCode}{separator}{x.Orpon?.CheckStatus}{separator}{GetParsingLevel(x.Orpon?.ParsingLevelCode)}{separator}{x.Orpon?.GlobalID}";
                }));

                var result = _fileService.SaveData(_set.FileSettings.FileOutput, data);

                if (result != null && result.Error == null)
                {
                    _notification.NotificationAsync(null, $"Save Ok {_set.FileSettings.FileOutput}");
                }
                else if (result != null && result.Error != null)
                {
                    _notification.NotificationAsync(null, result.Error.Message);
                }
            }

            if (_set.GeneralSettings.CanSaveFileWhithSelectedField)
            {
                if (_set.GeneralSettings.CollectionFieldForSave.FirstOrDefault(x => x.Name == "Адрес ОРПОН")?.CanSave == true)
                {
                    try
                    {
                        var house = await Task.Factory.StartNew(() =>
                        {
                            return _bd.GetCollectionHouse(_collection.Where(x => x.Orpon?.ParsingLevelCode == "FIAS_HOUSE" && int.TryParse(x.Orpon?.GlobalID, out int id)).Select(x =>
                            {
                                return int.Parse(x.Orpon.GlobalID);
                            }), new ConnectionSettingsDb()
                            {
                                BDName = _set.BDSettings.BDName,
                                Login = _set.BDSettings.Login,
                                Password = _set.BDSettings.Password,
                                Port = _set.BDSettings.Port,
                                Server = _set.BDSettings.Server
                            });
                        });

                        var address = await Task.Factory.StartNew(() =>
                        {
                            return _bd.GetCollectionAddress(_collection.Where(x => x.Orpon?.ParsingLevelCode != "FIAS_HOUSE" && int.TryParse(x.Orpon?.GlobalID, out int id)).Select(x =>
                            {
                                return int.Parse(x.Orpon.GlobalID);
                            }), new ConnectionSettingsDb()
                            {
                                BDName = _set.BDSettings.BDName,
                                Login = _set.BDSettings.Login,
                                Password = _set.BDSettings.Password,
                                Port = _set.BDSettings.Port,
                                Server = _set.BDSettings.Server
                            });
                        });

                        Parallel.ForEach(house, (item) =>
                        {
                            System.Diagnostics.Debug.WriteLine(item.Address);
                            var h = _collection.Where(x => x.Orpon?.GlobalID == item.GlobalId.ToString());
                            foreach (var i in h)
                            {
                                i.AddressOrpon = item.Address;
                            }
                        });

                        Parallel.ForEach(address, (item) =>
                        {
                            var h = _collection.Where(x => x.Orpon?.GlobalID == item.GlobalId.ToString());
                            foreach (var i in h)
                            {
                                i.AddressOrpon = item.Address;
                            }
                        });
                    }
                    catch(Exception ex)
                    {
                        _notification.NotificationAsync(null, "При получении адресов произошла ошибка: " + ex.Message);
                    }
                }

                var data = GetDataWhithSelectedField();

                var result = _fileService.SaveData(_set.FileSettings.FileOutput, data);

                if (result != null && result.Error == null)
                {
                    _notification.NotificationAsync(null, $"Save Ok {_set.FileSettings.FileOutput}");
                }
                else if (result != null && result.Error != null)
                {
                    _notification.NotificationAsync(null, result.Error.Message);
                }
            }

            if (_set.GeneralSettings.CanSaveDataAsFull)
            {
                var data = new List<string>(_collection.Count)
                    {
                        $"Адрес{separator}QualityCode{separator}CheckStatus{separator}UnparsedParts{separator}ParsingLevelCode{separator}GlobalID{separator}SystemCode{separator}KLADRLocalityId{separator}FIASLocalityId{separator}LocalityGlobalId{separator}" +
                        $"KLADRStreetId{separator}FIASStreetId{separator}Street{separator}StreetKind{separator}KLADRStreetId{separator}FIASStreetId{separator}StreetGlobalId{separator}House{separator}HouseLitera{separator}CornerHouse{separator}BuildingBlock{separator}" +
                        $"BuildingBlockLitera{separator}Building{separator}BuildingLitera{separator}Ownership{separator}OwnershipLitera{separator}FIASHouseId{separator}HouseGlobalId{separator}Latitude{separator}Longitude{separator}LocationDescription{separator}Error"
                    };
                data.AddRange(_collection.Select(x =>
                {
                    return $"{x.Address}{separator}{x.Orpon?.QualityCode}{separator}{x.Orpon?.CheckStatus}{separator}{x.Orpon?.UnparsedParts}{separator}{GetParsingLevel(x.Orpon?.ParsingLevelCode)}{separator}{x.Orpon?.GlobalID}{separator}{x.Orpon?.SystemCode}{separator}" +
                    $"{x.Orpon?.KLADRLocalityId}{separator}{x.Orpon?.FIASLocalityId}{separator}{x.Orpon?.LocalityGlobalId}{separator}{x.Orpon?.KLADRStreetId}{separator}{x.Orpon?.FIASStreetId}{separator}{x.Orpon?.Street}{separator}{x.Orpon?.StreetKind}{separator}" +
                    $"{x.Orpon?.KLADRStreetId}{separator}{x.Orpon?.FIASStreetId}{separator}{x.Orpon?.StreetGlobalId}{separator}{x.Orpon?.House}{separator}{x.Orpon?.HouseLitera}{separator}{x.Orpon?.CornerHouse}{separator}{x.Orpon?.BuildingBlock}{separator}" +
                    $"{x.Orpon?.BuildingBlockLitera}{separator}{x.Orpon?.Building}{separator}{x.Orpon?.BuildingLitera}{separator}{x.Orpon?.Ownership}{separator}{x.Orpon?.OwnershipLitera}{separator}{x.Orpon?.FIASHouseId}{separator}" +
                    $"{x.Orpon?.HouseGlobalId}{separator}{x.Orpon?.Latitude}{separator}{x.Orpon?.Longitude}{separator}{x.Orpon?.LocationDescription?.Replace('\n', ' ')}{separator}{x.Error}";
                }));

                var result = _fileService.SaveData($"{_set.FileSettings.FolderTemp}\\Temp_{Path.GetFileName(_set.FileSettings.FileOutput)}", data);

                if (result != null && result.Error == null)
                {
                    _notification.NotificationAsync(null, $"Save Ok {_set.FileSettings.FolderTemp}\\{Path.GetFileName(_set.FileSettings.FileOutput)}");
                }
                else if (result != null && result.Error != null)
                {
                    _notification.NotificationAsync(null, result.Error.Message);
                }
            }

            if (_set.GeneralSettings.CanOpenFolderAfter)
            {
                OpenFolder(_set.FileSettings.FileOutput);
            }
        }

        private List<string> GetDataWhithSelectedField()
        {
            var data = new List<string>();

            var header = new StringBuilder();

            foreach (var item in _set.GeneralSettings.CollectionFieldForSave)
            {
                if (item.CanSave)
                {
                    header.Append(item.Name + _set.GeneralSettings.SeparatorChar);
                }
            }

            data.Add(header.ToString());

            var str = new StringBuilder();

            foreach (var item in _collection)
            {
                str.Clear();
                if (_set.GeneralSettings.CollectionFieldForSave[0].CanSave)
                {
                    str.Append(item.Address + _set.GeneralSettings.SeparatorChar);
                }
                if (_set.GeneralSettings.CollectionFieldForSave[1].CanSave)
                {
                    str.Append(item.AddressOrpon + _set.GeneralSettings.SeparatorChar);
                }
                if (_set.GeneralSettings.CollectionFieldForSave[30].CanSave)
                {
                    str.Append(item.Error + _set.GeneralSettings.SeparatorChar);
                }

                if(item.Orpon != null)
                {
                    if (_set.GeneralSettings.CollectionFieldForSave[2].CanSave)
                    {
                        str.Append(item.Orpon.QualityCode + _set.GeneralSettings.SeparatorChar);
                    }
                    if (_set.GeneralSettings.CollectionFieldForSave[3].CanSave)
                    {
                        str.Append(item.Orpon.CheckStatus + _set.GeneralSettings.SeparatorChar);
                    }
                    if (_set.GeneralSettings.CollectionFieldForSave[4].CanSave)
                    {
                        str.Append(item.Orpon.UnparsedParts + _set.GeneralSettings.SeparatorChar);
                    }
                    if (_set.GeneralSettings.CollectionFieldForSave[5].CanSave)
                    {
                        str.Append(GetParsingLevel(item.Orpon.ParsingLevelCode) + _set.GeneralSettings.SeparatorChar);
                    }
                    if (_set.GeneralSettings.CollectionFieldForSave[6].CanSave)
                    {
                        str.Append(item.Orpon.GlobalID + _set.GeneralSettings.SeparatorChar);
                    }
                    if (_set.GeneralSettings.CollectionFieldForSave[7].CanSave)
                    {
                        str.Append(item.Orpon.SystemCode + _set.GeneralSettings.SeparatorChar);
                    }
                    if (_set.GeneralSettings.CollectionFieldForSave[8].CanSave)
                    {
                        str.Append(item.Orpon.KLADRLocalityId + _set.GeneralSettings.SeparatorChar);
                    }
                    if (_set.GeneralSettings.CollectionFieldForSave[9].CanSave)
                    {
                        str.Append(item.Orpon.FIASLocalityId + _set.GeneralSettings.SeparatorChar);
                    }
                    if (_set.GeneralSettings.CollectionFieldForSave[10].CanSave)
                    {
                        str.Append(item.Orpon.LocalityGlobalId + _set.GeneralSettings.SeparatorChar);
                    }
                    if (_set.GeneralSettings.CollectionFieldForSave[11].CanSave)
                    {
                        str.Append(item.Orpon.KLADRStreetId + _set.GeneralSettings.SeparatorChar);
                    }
                    if (_set.GeneralSettings.CollectionFieldForSave[12].CanSave)
                    {
                        str.Append(item.Orpon.FIASStreetId + _set.GeneralSettings.SeparatorChar);
                    }
                    if (_set.GeneralSettings.CollectionFieldForSave[13].CanSave)
                    {
                        str.Append(item.Orpon.StreetKind + _set.GeneralSettings.SeparatorChar);
                    }
                    if (_set.GeneralSettings.CollectionFieldForSave[14].CanSave)
                    {
                        str.Append(item.Orpon.StreetGlobalId + _set.GeneralSettings.SeparatorChar);
                    }
                    if (_set.GeneralSettings.CollectionFieldForSave[15].CanSave)
                    {
                        str.Append(item.Orpon.Street + _set.GeneralSettings.SeparatorChar);
                    }
                    if (_set.GeneralSettings.CollectionFieldForSave[16].CanSave)
                    {
                        str.Append(item.Orpon.House + _set.GeneralSettings.SeparatorChar);
                    }
                    if (_set.GeneralSettings.CollectionFieldForSave[17].CanSave)
                    {
                        str.Append(item.Orpon.HouseLitera + _set.GeneralSettings.SeparatorChar);
                    }
                    if (_set.GeneralSettings.CollectionFieldForSave[18].CanSave)
                    {
                        str.Append(item.Orpon.CornerHouse + _set.GeneralSettings.SeparatorChar);
                    }
                    if (_set.GeneralSettings.CollectionFieldForSave[19].CanSave)
                    {
                        str.Append(item.Orpon.BuildingBlock + _set.GeneralSettings.SeparatorChar);
                    }
                    if (_set.GeneralSettings.CollectionFieldForSave[20].CanSave)
                    {
                        str.Append(item.Orpon.BuildingBlockLitera + _set.GeneralSettings.SeparatorChar);
                    }
                    if (_set.GeneralSettings.CollectionFieldForSave[21].CanSave)
                    {
                        str.Append(item.Orpon.Building + _set.GeneralSettings.SeparatorChar);
                    }
                    if (_set.GeneralSettings.CollectionFieldForSave[22].CanSave)
                    {
                        str.Append(item.Orpon.BuildingLitera + _set.GeneralSettings.SeparatorChar);
                    }
                    if (_set.GeneralSettings.CollectionFieldForSave[23].CanSave)
                    {
                        str.Append(item.Orpon.Ownership + _set.GeneralSettings.SeparatorChar);
                    }
                    if (_set.GeneralSettings.CollectionFieldForSave[24].CanSave)
                    {
                        str.Append(item.Orpon.OwnershipLitera + _set.GeneralSettings.SeparatorChar);
                    }
                    if (_set.GeneralSettings.CollectionFieldForSave[25].CanSave)
                    {
                        str.Append(item.Orpon.FIASHouseId + _set.GeneralSettings.SeparatorChar);
                    }
                    if (_set.GeneralSettings.CollectionFieldForSave[26].CanSave)
                    {
                        str.Append(item.Orpon.HouseGlobalId + _set.GeneralSettings.SeparatorChar);
                    }
                    if (_set.GeneralSettings.CollectionFieldForSave[27].CanSave)
                    {
                        str.Append(item.Orpon.Latitude + _set.GeneralSettings.SeparatorChar);
                    }
                    if (_set.GeneralSettings.CollectionFieldForSave[28].CanSave)
                    {
                        str.Append(item.Orpon.Longitude + _set.GeneralSettings.SeparatorChar);
                    }
                    if (_set.GeneralSettings.CollectionFieldForSave[29].CanSave)
                    {
                        str.Append(item.Orpon.LocationDescription + _set.GeneralSettings.SeparatorChar);
                    }
                }

                data.Add(str.ToString());
            }

            return data;
        }

        private string GetParsingLevel(string level)
        {
            var result = string.Empty;

            if (!_set.GeneralSettings.CanUseParsinglevelRus)
            {
                return level;
            }

            switch (level)
            {
                case "FIAS_HOUSE":
                    result = "Дом";
                    break;

                case "FIAS_STREET":
                    result = "Улица";
                    break;

                case "FIAS_SETTLEMENT":
                    result = "Населенный пункт";
                    break;

                case "FIAS_CITY":
                    result = "Город";
                    break;

                case "FIAS_CITY_AREA":
                    result = "Район города";
                    break;

                case "FIAS_PLANNING_STRUCTURE":
                    result = "Планировочная структура";
                    break;

                case "FIAS_DISTRICT":
                    result = "Район";
                    break;

                case "FIAS_SUBJECT":
                    result = "Субъект";
                    break;

                default:
                    result = level;
                    break;
            }

            return result;
        }

        private void SaveDataGeo()
        {
            var data = new List<string>(_collection.Count)
                    {
                        $"globalid;Latitude;Longitude;Qcode"
                    };

            data.AddRange(_collection.Select(x =>
            {
                return $"{x.GlobalIdOriginal};{x.Latitude};{x.Longitude};{x.QCodeNew}";
            }));

            var result = _fileService.SaveData(_set.FileSettings.FileOutput, data);

            if (result != null && result.Error == null)
            {
                _notification.NotificationAsync(null, $"Save Ok {_set.FileSettings.FileOutput}");
            }
            else if (result != null && result.Error != null)
            {
                _notification.NotificationAsync(null, result.Error.Message);
            }
        }

        #endregion PrivateMethod

        private List<string> _collectionParsingLevel;
        /// <summary>
        /// 
        /// </summary>
        public List<string> CollectionParsingLevel
        {
            get => _collectionParsingLevel;
            set => Set(ref _collectionParsingLevel, value);
        }

        private string _currentParsingLevel = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string CurrentParsingLevel
        {
            get => _currentParsingLevel;
            set => Set(ref _currentParsingLevel, value);
        }

        private bool _canGetOrponForParsingLevel = false;
        /// <summary>
        /// 
        /// </summary>
        public bool CanGetOrponForParsingLevel
        {
            get => _canGetOrponForParsingLevel;
            set => Set(ref _canGetOrponForParsingLevel, value);
        }

        private async void GetOrponAsync(EntityOrpon obj)
        {
            if (_set.RepositorySettings.StatusConnect != StatusConnect.OK)
            {
                _notification.NotificationAsync(null, "Not set connect OrponService");
                return;
            }

            obj.Status = StatusType.OrponingNow;

            var a = await _orpon.GetOrponAsync(obj);
            //if (a != null && a.Error == null && a.Object != null)
            //{
            //    obj.Orpon = a.Object;
            //    obj.Status = StatusType.OK;
            //}
            //else if (a != null && a.Error != null)
            //{
            //    obj.Status = StatusType.Error;
            //    obj.Error = a.Error.Message;
            //}

            obj.DateTimeOrponing = DateTime.Now;
            _stat.UpdateStatisticsCollection();
        }

        private RelayCommand _commandOrponingAddress;
        public RelayCommand CommandOrponingAddress =>
            _commandOrponingAddress ?? (_commandOrponingAddress = new RelayCommand(
             () =>
             {
                 GetOrponAsync(_singlOrpon);
             }));

        private EntityOrpon _singlOrpon = new EntityOrpon();
        public EntityOrpon SinglOrpon
        {
            get => _singlOrpon;
            set => Set(ref _singlOrpon, value);
        }

        private RelayCommand _commandReplaceText;
        public RelayCommand CommandReplaceText =>
        _commandReplaceText ?? (_commandReplaceText = new RelayCommand(
                    () =>
                    {
                        if (_collection != null && _collection.Any())
                        {
                            _collection.AsParallel().ForAll(x =>
                            {
                                x.Address = x.Address.Replace(_oldText, _newText);
                            });
                        }
                    }));

        private string _oldText = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string OldText
        {
            get => _oldText;
            set => Set(ref _oldText, value);
        }

        private string _newText = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string NewText
        {
            get => _newText;
            set => Set(ref _newText, value);
        }


        private int _indexTab = 0;
        /// <summary>
        /// 
        /// </summary>
        public int IndexTab
        {
            get => _indexTab;
            set => Set(ref _indexTab, value);
        }

        private bool _isRequestedStop = false;
        /// <summary>
        /// 
        /// </summary>
        public bool IsRequestedStop
        {
            get => _isRequestedStop;
            set => Set(ref _isRequestedStop, value);
        }

        private RelayCommand _commandAddCollectionAddress;
        public RelayCommand CommandAddCollectionAddress =>
        _commandAddCollectionAddress ?? (_commandAddCollectionAddress = new RelayCommand(
                    () =>
                    {
                        var str = Clipboard.GetText().Replace("\"", "").Replace("\n","").Replace("\r", "\r\n").Replace("\r\n\r\n", "\r\n");
                        var items = str.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                        var id = 0;
                        Collection = new ObservableCollection<EntityOrpon>(items.Select(x =>
                        {
                            return new EntityOrpon() { Id = id++, Address = x.TrimStart(new char[] { '"' }) };
                        }));

                        if (_collection != null)
                        {
                            _stat.Init(_collection);
                        }

                        _set.FileSettings.FileInput = "Clipboard";
                        _set.FileSettings.FileOutput = GetDefaultName();
                    }));

        public MainViewModel(IFileService fileService, INotifications notification, StatisticsViewModel stat, SettingsViewModel set, IRepository orpon, IBDRepository bd)
        {
            _fileService = fileService;
            _notification = notification;
            _stat = stat;
            _set = set;
            //_orpon = new OrponRepository1();
            _orpon = orpon;
            _bd = bd;
            _set.SetRepository(_orpon);
            CollectionParsingLevel = Enum.GetNames(typeof(ParsingLevelCode)).ToList();
        }
    }
}