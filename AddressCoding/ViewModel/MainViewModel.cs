using AddressCoding.FileService;
using AddressCoding.Notifications;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows;
using System.Collections.ObjectModel;
using AddressCoding.Entities;

namespace AddressCoding.ViewModel
{
    public class MainViewModel : ViewModelBase
    {

        #region PrivateField

        /// <summary>
        /// ���� ��� �������� ����� �������� �����
        /// </summary>
        private string _fileInput = string.Empty;

        /// <summary>
        /// ���� ��� �������� ������ �� ������� ��������� ��������������
        /// </summary>
        private RelayCommand<DragEventArgs> _commandDragDrop;

        /// <summary>
        /// ���� ��� �������� ������� ��������� �����
        /// </summary>
        private RelayCommand _commandGetFile;

        /// <summary>
        /// ���� ��� �������� ������� ��������� ������ �� �����
        /// </summary>
        private RelayCommand _commandGetDataFromFile;

        #endregion PrivateField

        #region PublicProperties

        /// <summary>
        /// ��� �������� �����
        /// </summary>
        public string FileInput
        {
            get => _fileInput;
            set => Set(ref _fileInput, value);
        }

        #endregion PublicProperties

        #region PublicCommand

        /// <summary>
        /// ������� ��������� �������������� ������ �� ���� ���������
        /// </summary>
        public RelayCommand<DragEventArgs> CommandDragDrop =>
                _commandDragDrop ?? (_commandDragDrop = new RelayCommand<DragEventArgs>(
                            obj =>
                            {

                            }));

        /// <summary>
        /// ������� ��� ��������� �����
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
        /// ������� ��������� ������ �� �����
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

        public MainViewModel(IFileService fileService, INotifications notification)
        {
            _fileService = fileService;
            _notification = notification;
        }
    }
}