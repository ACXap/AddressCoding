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
        /// ���� ��� �������� ������ �� ������ ������ � �������
        /// </summary>
        private readonly IFileService _fileService;
        /// <summary>
        /// ���� ��� �������� ������ �� ������ ������ � ������������
        /// </summary>
        private readonly INotifications _notification;
        /// <summary>
        /// ���� ��� �������� ������ �� ������ ������ �� �����������
        /// </summary>
        private readonly StatisticsViewModel _stat;
        /// <summary>
        /// ���� ��� �������� ������ �� ������ ������ � �����������
        /// </summary>
        private readonly SettingsViewModel _set;

        /// <summary>
        /// ���� ��� �������� ������ �� ��������� ������
        /// </summary>
        private ObservableCollection<EntityOrpon> _collection;
        /// <summary>
        /// ���� ��� �������� ����� �������� �����
        /// </summary>
        private string _fileInput = string.Empty;
        /// <summary>
        /// ���� ��� �������� ����� ��������� �����
        /// </summary>
        private string _fileOutput = string.Empty;
        /// <summary>
        /// ���� ��� �������� ��������� �� �������� ���� �� �����
        /// </summary>
        private bool _canBreakFileOutput = false;
        /// <summary>
        /// ���� ��� �������� �� ������� ������ ��������� �������� ����
        /// </summary>
        private int _maxSizePart = 0;
        /// <summary>
        /// ���� ��� �������� �������� �� ���������
        /// </summary>
        private bool _isStartOrponing = false;
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
        /// <summary>
        /// ���� ��� �������� ������� ������ ����� ��� ����������
        /// </summary>
        private RelayCommand _commandSetFileOutput;
        /// <summary>
        /// ���� ��� �������� ������� ��� ������� �����������
        /// </summary>
        private RelayCommand _commandGetAllOrpon;
        /// <summary>
        /// ���� ��� �������� ������� ��������� �����������
        /// </summary>
        private RelayCommand _commandStopOrponing;
        /// <summary>
        /// ���� ��� �������� ������� �������� �����
        /// </summary>
        private RelayCommand<string> _commandOpenFolder;
        /// <summary>
        /// ���� ��� �������� ������� ������ ������ � ����
        /// </summary>
        private RelayCommand _commandSaveData;
        #endregion PrivateField

        #region PublicProperties
        /// <summary>
        /// ��������� ������
        /// </summary>
        public ObservableCollection<EntityOrpon> Collection
        {
            get => _collection;
            set => Set(ref _collection, value);
        }
        /// <summary>
        /// �������� �� ���������
        /// </summary>
        public bool IsStartOrponing
        {
            get => _isStartOrponing;
            set => Set(ref _isStartOrponing, value);
        }
        /// <summary>
        /// ��� �������� �����
        /// </summary>
        public string FileInput
        {
            get => _fileInput;
            set => Set(ref _fileInput, value);
        }
        /// <summary>
        /// ��� ��������� �����
        /// </summary>
        public string FileOutput
        {
            get => _fileOutput;
            set => Set(ref _fileOutput, value);
        }
        /// <summary>
        /// ��������� �� �������� ���� �� �����
        /// </summary>
        public bool CanBreakFileOutput
        {
            get => _canBreakFileOutput;
            set => Set(ref _canBreakFileOutput, value);
        }
        /// <summary>
        /// �� ������� ������ ��������� �������� ����
        /// </summary>
        public int MaxSizePart
        {
            get => _maxSizePart;
            set => Set(ref _maxSizePart, value);
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
        /// ������� ��� ��������� �����
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
        /// ������� ��������� ������ �� �����
        /// </summary>
        public RelayCommand CommandGetDataFromFile =>
        _commandGetDataFromFile ?? (_commandGetDataFromFile = new RelayCommand(
                    () =>
                    {
                        GetDataFromFile();
                    }));

        /// <summary>
        /// ������� ��� ������ ����� ��� ���������� ������
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
        /// ������� ������� �����������
        /// </summary>
        public RelayCommand CommandGetAllOrpon =>
        _commandGetAllOrpon ?? (_commandGetAllOrpon = new RelayCommand(
            () =>
            {

            }));

        /// <summary>
        /// ������� ��� ��������� �������� �����������
        /// </summary>
        public RelayCommand CommandStopOrponing =>
        _commandStopOrponing ?? (_commandStopOrponing = new RelayCommand(
            () =>
            {

            }));

        /// <summary>
        /// ������� �������� �����
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
        /// ������� ��� ���������� ������ � ����
        /// </summary>
        public RelayCommand CommandSaveData =>
        _commandSaveData ?? (_commandSaveData = new RelayCommand(
            () =>
            {
                var data = new List<string>(_collection.Count)
                {
                    $"�����;QualityCode;CheckStatus;ParsingLevelCode;GlobalID"
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