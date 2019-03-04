using AddressCoding.Entities;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using GalaSoft.MvvmLight.CommandWpf;
using AddressCoding.FileService;
using AddressCoding.Notifications;

namespace AddressCoding.ViewModel
{
    public class StatisticsViewModel :ViewModelBase
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
        #endregion PrivateMethod

        private RelayCommand _commandSaveStatistics;
        public RelayCommand CommandSaveStatistics =>
            _commandSaveStatistics ?? (_commandSaveStatistics = new RelayCommand(
            () =>
            {

            }));

        private RelayCommand<string> _commandOpenFolder;
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

        public StatisticsViewModel(IFileService fileService, SettingsViewModel set, INotifications notification)
        {
            _fileService = fileService;
            _set = set;
            _notification = notification;
        }
    }
}