using AddressCoding.Entities;
using AddressCoding.FileService;
using AddressCoding.Notifications;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace AddressCoding.ViewModel
{
    public class StatisticsViewModel : ViewModelBase
    {
        #region PrivateFields
        /// <summary>
        /// Поле для хранения ссылки на модуль работы с файлами
        /// </summary>
        private readonly IFileService _fileService;
        /// <summary>
        /// Поле для работы с настройками
        /// </summary>
        private readonly SettingsViewModel _set;
        /// <summary>
        /// Поле для хранения ссылки на модуль работы с оповещениями
        /// </summary>
        private readonly INotifications _notification;
        /// <summary>
        /// Поле для хранения строки заголовка файла статистики
        /// </summary>
        private readonly string _columnHeader = "DateTime;User;FileInput;FileOutput;FileError;AllEntity;OK;Error;NotOrponing;House;Exact;NotFound;TimeGeoCod";
        /// <summary>
        /// Поле для хранения имени файла для статистики
        /// </summary>
        private readonly string _nameFileStatistics = "Statistics.csv";
        /// <summary>
        /// Поле для хранения статистики
        /// </summary>
        private Statistics _statistics;
        /// <summary>
        /// Интервал работы таймера
        /// </summary>
        private int _interval;
        /// <summary>
        /// Время начала работы таймера
        /// </summary>
        private DateTime _timeStart;
        /// <summary>
        /// Сам таймер
        /// </summary>
        private DispatcherTimer _timer;
        /// <summary>
        /// Коллекция для подсчета статистики
        /// </summary>
        private IEnumerable<EntityOrpon> _collection;

        /// <summary>
        /// Поле для хранения команды для сохранения статистики
        /// </summary>
        private RelayCommand _commandSaveStatistics;
        /// <summary>
        /// Поле для хранения команды открытия папки
        /// </summary>
        private RelayCommand<string> _commandOpenFolder;
        #endregion PrivateFields

        #region PublicProperty
        /// <summary>
        /// Была ли сохранена статистика
        /// </summary>
        public bool IsSave { get; set; } = false;

        /// <summary>
        /// Статистика по выполненной орпонизации
        /// </summary>
        public Statistics Statistics
        {
            get => _statistics;
            set => Set(ref _statistics, value);
        }
        #endregion PublicProperty

        #region PublicMethod

        /// <summary>
        /// Метод инициализации таймера
        /// </summary>
        /// <param name="collection">Коллекция для счета статистики</param>
        /// <param name="interval">Интервал работы таймера (по умолчанию 1сек.)</param>
        public void Init(IEnumerable<EntityOrpon> collection, int interval = 1)
        {
            _interval = interval;
            _collection = collection;
            Statistics = new Statistics();
            UpdateStatisticsCollection();

            if (_timer == null)
            {
                _timer = new DispatcherTimer(TimeSpan.FromSeconds(_interval), DispatcherPriority.DataBind, GetStat, Dispatcher.CurrentDispatcher)
                {
                    IsEnabled = false
                };
            }
        }

        /// <summary>
        /// Метод запуска таймера
        /// </summary>
        public void Start()
        {
            _timer.Start();
            _timeStart = DateTime.Now;
            GetStat(null, null);
        }

        /// <summary>
        /// Метод остановки таймера
        /// </summary>
        public void Stop()
        {
            _timer.Stop();
            GetStat(null, null);
        }

        /// <summary>
        /// Метод обновления статистики по коллекции
        /// </summary>
        public void UpdateStatisticsCollection()
        {
            if (_collection != null)
            {
                _statistics.AllEntity = _collection.Count();
                _statistics.OK = _collection.Count(x => x.Status == StatusType.OK);
                _statistics.NotOrponing = _collection.Count(x => x.Status == StatusType.NotOrponing);
                _statistics.OrponingNow = _collection.Count(x => x.Status == StatusType.OrponingNow);
                _statistics.Error = _collection.Count(x => x.Status == StatusType.Error);
                // _statistics.House = _collection.Count(x => x.MainGeoCod?.Kind == KindType.House);
                // _statistics.Exact = _collection.Count(x => x.MainGeoCod?.Precision == PrecisionType.Exact);
                // _statistics.NotFound = _collection.Count(x => x.CountResult == 0);
                _statistics.Percent = ((_statistics.AllEntity - _statistics.NotOrponing - _statistics.OrponingNow) / (double)_statistics.AllEntity) * 100;
                IsSave = false;
            }
        }

        /// <summary>
        /// Метод сохранения статистики
        /// </summary>
        public void SaveStatistics()
        {
            if (_statistics == null)
            {
                return;
            }
            var nameFile = $"{_set.FileSettings.FolderStatistics}\\{_nameFileStatistics}";

            var result = _fileService.ExistFile(nameFile);
            // если есть файл статистики, добавить
            if (result != null && result.Object)
            {
                var resAdd = _fileService.AppendToFile(nameFile, new string[]
                {
                    GetStringStatistics()
                });
                if (resAdd != null && resAdd.Error != null)
                {
                    _notification.NotificationAsync(null, resAdd.Error.Message);
                }
            }
            // если нет файла статистки, создать и добавить
            else if (result != null && !result.Object && result.Error == null)
            {
                var resSave = _fileService.SaveData(nameFile, new string[]
                {
                    _columnHeader,
                    GetStringStatistics()
                });
                if (resSave != null && resSave.Error != null)
                {
                    _notification.NotificationAsync(null, resSave.Error.Message);
                }
            }
            // если ошибки
            else if (result != null && result.Error != null)
            {
                _notification.NotificationAsync(null, result.Error.Message);
            }
        }
        #endregion PublicMethod

        #region PrivateMethod
        /// <summary>
        /// Метод обновления статистики
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetStat(object sender, EventArgs e)
        {
            UpdateStatisticsCollection();
            _statistics.TimeGeoCod = TimeSpan.FromSeconds((DateTime.Now - _timeStart).TotalSeconds);
            if (_statistics.Percent > 0)
            {
                _statistics.TimeLeftGeoCod = TimeSpan.FromSeconds(((100 / _statistics.Percent) * _statistics.TimeGeoCod.TotalSeconds) - _statistics.TimeGeoCod.TotalSeconds);
            }
        }

        /// <summary>
        /// Метод получения строки статистики
        /// </summary>
        /// <returns>Возвращает строку статистики</returns>
        private string GetStringStatistics()
        {
            return $"{DateTime.Now};{Environment.UserName};{_set.FileSettings.FileInput};{_set.FileSettings.FileOutput};{_set.FileSettings.FileError};{_statistics.AllEntity};{_statistics.OK};{_statistics.Error};" +
                    $"{_statistics.NotOrponing};{_statistics.House};{_statistics.Exact};{_statistics.NotFound};{_statistics.TimeGeoCod}";
        }
        #endregion PrivateMethod

        #region PublicCommand

        /// <summary>
        /// Команда для сохранения статистики
        /// </summary>
        public RelayCommand CommandSaveStatistics =>
            _commandSaveStatistics ?? (_commandSaveStatistics = new RelayCommand(
            () =>
            {
                SaveStatistics();
            }));

        /// <summary>
        /// Команда открытия папки
        /// </summary>
        public RelayCommand<string> CommandOpenFolder =>
        _commandOpenFolder ?? (_commandOpenFolder = new RelayCommand<string>(
            obj =>
            {
                if (obj == "StatFolder")
                {
                    obj = _set.FileSettings.FolderStatistics;
                }
                var result = _fileService.OpenFolder(obj);
                if (result != null && result.Error != null)
                {
                    _notification.NotificationAsync(null, result.Error.Message);
                }
            }));

        #endregion PublicCommand

        public StatisticsViewModel(IFileService fileService, SettingsViewModel set, INotifications notification)
        {
            _fileService = fileService;
            _set = set;
            _notification = notification;
        }
    }
}