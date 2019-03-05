using AddressCoding.Entities;
using AddressCoding.FileService;
using AddressCoding.Notifications;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace AddressCoding.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region PrivateField
        /// <summary>
        /// Поле для хранения ссылки на модуль работы с файлами
        /// </summary>
        private readonly IFileService _fileService;
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
        private RelayCommand _сommandGetOrpon;
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
            set => Set(ref _isStartOrponing, value);
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
                            }));

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
                    }));

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

            }, () => CanStartOrponing()));

        /// <summary>
        /// Команда для остановки процесса орпонизации
        /// </summary>
        public RelayCommand CommandStopOrponing =>
        _commandStopOrponing ?? (_commandStopOrponing = new RelayCommand(
            () =>
            {

            }));

        /// <summary>
        /// Команда запуска орпонизации выбранного объекта
        /// </summary>
        public RelayCommand CommandGetOrpon =>
        _сommandGetOrpon ?? (_сommandGetOrpon = new RelayCommand(
                    () =>
                    {

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
                if (obj == "AppFolder")
                {
                    obj = _set.FileSettings.FolderApp;
                }
                var result = _fileService.OpenFolder(obj);
                if (result != null && result.Error != null)
                {
                    _notification.NotificationAsync(null, result.Error.Message);
                }
            }));

        /// <summary>
        /// Команда для сохранения данных в файл
        /// </summary>
        public RelayCommand CommandSaveData =>
        _commandSaveData ?? (_commandSaveData = new RelayCommand(
            () =>
            {
                if (CanSaveFile())
                {
                    var data = new List<string>(_collection.Count)
                    {
                        $"Адрес;QualityCode;CheckStatus;ParsingLevelCode;GlobalID"
                    };
                    data.AddRange(_collection.Select(x =>
                    {
                        return $"{x.Address};{x.Orpon?.QualityCode};{x.Orpon?.CheckStatus};{x.Orpon?.ParsingLevelCode};{x.Orpon?.GlobalID}";
                    }));

                    var result = _fileService.SaveData(_set.FileSettings.FileOutput, data);

                    if (result != null && result.Error == null)
                    {
                        _notification.NotificationAsync(null, "Save Ok");
                    }
                    else if (result != null && result.Error != null)
                    {
                        _notification.NotificationAsync(null, result.Error.Message);
                    }
                }
                else
                {
                    _notification.NotificationAsync(null, "Error");
                }
            }, () => CanSaveFile()));

        #endregion PublicCommand

        #region PrivateMethod

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
                defName = $"{DateTime.Now.ToString("yyyy_MM_dd")}_{System.IO.Path.GetFileNameWithoutExtension(_set.FileSettings.FileInput)}_{_collection.Count}.csv";
            }
            else
            {
                defName = $"{DateTime.Now.ToString("yyyy_MM_dd")}_{System.IO.Path.GetFileNameWithoutExtension(_set.FileSettings.FileInput)}.csv";
            }

            return $"{_set.FileSettings.FolderOutput}\\{defName}";
        }

        /// <summary>
        /// Метод получения данных из файла
        /// </summary>
        private void GetDataFromFile()
        {
            var result = _fileService.GetData(_set.FileSettings.FileInput);
            var id = 0;
            if (result != null && result.Error == null && result.Objects != null)
            {
                Collection = new ObservableCollection<EntityOrpon>(result.Objects.Select(x =>
                {
                    return new EntityOrpon() { Id = id++, Address = x };
                }));

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

        #endregion PrivateMethod

        #region PublicMethod
        #endregion PublicMethod

        public MainViewModel(IFileService fileService, INotifications notification, StatisticsViewModel stat, SettingsViewModel set)
        {
            _fileService = fileService;
            _notification = notification;
            _stat = stat;
            _set = set;
        }
    }
}