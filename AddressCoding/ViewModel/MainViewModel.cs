using AddressCoding.Entities;
using AddressCoding.FileService;
using AddressCoding.Notifications;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.ObjectModel;
using System.Windows;

namespace AddressCoding.ViewModel
{
    public class MainViewModel : ViewModelBase
    {

        #region PrivateField

        /// <summary>
        /// Поле для хранения имени входного файла
        /// </summary>
        private string _fileInput = string.Empty;

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

        #endregion PrivateField

        #region PublicProperties

        /// <summary>
        /// Имя входного файла
        /// </summary>
        public string FileInput
        {
            get => _fileInput;
            set => Set(ref _fileInput, value);
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
                            FileInput = result.Object;
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

                    }));

        #endregion PublicCommand

        #region PrivateMethod
        #endregion PrivateMethod

        #region PublicMethod
        #endregion PublicMethod

        private readonly IFileService _fileService;
        private readonly INotifications _notification;

        private RelayCommand _commandSetFileOutput;
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

        private string _fileOutput;
        public string FileOutput
        {
            get => _fileOutput;
            set => Set(ref _fileOutput, value);
        }

        private ObservableCollection<EntityOrpon> _collection;
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<EntityOrpon> Collection
        {
            get => _collection;
            set => Set(ref _collection, value);
        }

        private RelayCommand _commandGetAllGeoCod;
        public RelayCommand CommandGetAllGeoCod =>
        _commandGetAllGeoCod ?? (_commandGetAllGeoCod = new RelayCommand(
                    () =>
                    {

                    }));

        private bool _isStartOrponing = false;
        /// <summary>
        /// 
        /// </summary>
        public bool IsStartOrponing
        {
            get => _isStartOrponing;
            set => Set(ref _isStartOrponing, value);
        }

        private RelayCommand _commandStopOrponing;
        public RelayCommand CommandStopOrponing =>
        _commandStopOrponing ?? (_commandStopOrponing = new RelayCommand(
                    () =>
                    {

                    }));

        private RelayCommand<string> _commandOpenFolder;
        public RelayCommand<string> CommandOpenFolder =>
        _commandOpenFolder ?? (_commandOpenFolder = new RelayCommand<string>(
                    obj =>
                    {

                    }));


        private bool _canBreakFileOutput = false;
        /// <summary>
        /// 
        /// </summary>
        public bool CanBreakFileOutput
        {
            get => _canBreakFileOutput;
            set => Set(ref _canBreakFileOutput, value);
        }

        private RelayCommand _commandSaveData;
        public RelayCommand CommandSaveData =>
        _commandSaveData ?? (_commandSaveData = new RelayCommand(
                    () =>
                    {

                    }));


        private int _maxSizePart = 0;
        /// <summary>
        /// 
        /// </summary>
        public int MaxSizePart
        {
            get => _maxSizePart;
            set => Set(ref _maxSizePart, value);
        }


        private readonly StatisticsViewModel _stat;

        public MainViewModel(IFileService fileService, INotifications notification, StatisticsViewModel stat)
        {
            _fileService = fileService;
            _notification = notification;
            _stat = stat;
        }
    }
}