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
        /// Поле для хранения имени входного файла
        /// </summary>
        private string _fileInput = string.Empty;
        /// <summary>
        /// Поле для хранения имени выходного файла
        /// </summary>
        private string _fileOutput = string.Empty;
        /// <summary>
        /// Поле для хранения разбивать ли выходной файл на части
        /// </summary>
        private bool _canBreakFileOutput = false;
        /// <summary>
        /// Поле для хранения на сколько частей разбивать выходной файл
        /// </summary>
        private int _maxSizePart = 0;
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
        /// Запущена ли процедура
        /// </summary>
        public bool IsStartOrponing
        {
            get => _isStartOrponing;
            set => Set(ref _isStartOrponing, value);
        }
        /// <summary>
        /// Имя входного файла
        /// </summary>
        public string FileInput
        {
            get => _fileInput;
            set => Set(ref _fileInput, value);
        }
        /// <summary>
        /// Имя выходного файла
        /// </summary>
        public string FileOutput
        {
            get => _fileOutput;
            set => Set(ref _fileOutput, value);
        }
        /// <summary>
        /// Разбивать ли выходной файл на части
        /// </summary>
        public bool CanBreakFileOutput
        {
            get => _canBreakFileOutput;
            set => Set(ref _canBreakFileOutput, value);
        }
        /// <summary>
        /// На сколько частей разбивать выходной файл
        /// </summary>
        public int MaxSizePart
        {
            get => _maxSizePart;
            set => Set(ref _maxSizePart, value);
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
                        GetDataFromFile();
                    }));

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
                   FileOutput = result.Object;
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

            }));

        /// <summary>
        /// Команда для остановки процесса орпонизации
        /// </summary>
        public RelayCommand CommandStopOrponing =>
        _commandStopOrponing ?? (_commandStopOrponing = new RelayCommand(
            () =>
            {

            }));

        /// <summary>
        /// Команда открытия папки
        /// </summary>
        public RelayCommand<string> CommandOpenFolder =>
        _commandOpenFolder ?? (_commandOpenFolder = new RelayCommand<string>(
            obj =>
            {
                var result = _fileService.OpenFolder(obj);
                if(result!=null && result.Error!=null)
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
                var data = new List<string>(_collection.Count)
                {
                    $"Адрес;QualityCode;CheckStatus;ParsingLevelCode;GlobalID"
                };
                data.AddRange(_collection.Select(x =>
                {
                    return $"{x.Address};{x.Orpon?.QualityCode};{x.Orpon?.CheckStatus};{x.Orpon?.ParsingLevelCode};{x.Orpon?.GlobalID}";
                }));

                var result = _fileService.SaveData(data, _fileOutput);

                if(result != null && result.Error == null)
                {
                    _notification.NotificationAsync(null, "Save Ok");
                }
                else if(result !=null && result.Error!=null)
                {
                    _notification.NotificationAsync(null, result.Error.Message);
                }
            }));

        #endregion PublicCommand

        #region PrivateMethod

        private void SetFileInput(string file)
        {
            FileInput = file;
            GetDataFromFile();

            FileOutput = GetDefaultName();
        }

        private string GetDefaultName()
        {
            string defName = string.Empty;

            if (_collection!=null && _collection.Any())
            {
                defName = $"{DateTime.Now.ToString("yyyy_MM_dd")}_{System.IO.Path.GetFileNameWithoutExtension(_fileInput)}_{_collection.Count}.csv";
            }
            else
            {
                defName = $"{DateTime.Now.ToString("yyyy_MM_dd")}_{System.IO.Path.GetFileNameWithoutExtension(_fileInput)}.csv";
            }

            return $"{_set.FileSettings.FolderOutput}\\{defName}";
        }

        private void GetDataFromFile()
        {
            var result = _fileService.GetData(_fileInput);
            if (result != null && result.Error == null && result.Objects != null)
            {
                Collection = new ObservableCollection<EntityOrpon>(result.Objects.Select(x =>
                {
                    return new EntityOrpon() { Address = x };
                }));

                if(_collection!=null)
                {
                    _stat.Init(_collection);
                }

            }
            else if (result != null && result.Error != null)
            {
                _notification.NotificationAsync(null, result.Error.Message);
            }
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