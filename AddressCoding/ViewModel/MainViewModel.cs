using AddressCoding.Entities;
using AddressCoding.FileService;
using AddressCoding.Notifications;
using AddressCoding.Repository;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AddressCoding.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        CancellationTokenSource cts;

        #region PrivateField
        /// <summary>
        /// ���� ��� �������� ������ �� ������ ������ � �������
        /// </summary>
        private readonly IFileService _fileService;
        /// <summary>
        /// ���� ��� �������� ������ �� ������ ������ � �������
        /// </summary>
        private readonly IRepository _orpon;
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
        /// ���� ��� �������� ������ �� ������� ���������� �������
        /// </summary>
        private EntityOrpon _currentOrpon;

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
        /// <summary>
        /// ���� ��� �������� ������� ����������� ���������� �������
        /// </summary>
        private RelayCommand<EntityOrpon> _�ommandGetOrpon;
        /// <summary>
        /// ���� ��� �������� ������� ����������� ������ � �����
        /// </summary>
        private RelayCommand _commandCopyAddress;
        /// <summary>
        /// ���� ��� �������� ������� ������� ���������
        /// </summary>
        private RelayCommand _commandClearCollection;
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
        /// ������� ���������� �������
        /// </summary>
        public EntityOrpon CurrentOrpon
        {
            get => _currentOrpon;
            set => Set(ref _currentOrpon, value);
        }
        /// <summary>
        /// �������� �� ���������
        /// </summary>
        public bool IsStartOrponing
        {
            get => _isStartOrponing;
            set => Set(ref _isStartOrponing, value);
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
                        if (CanGetDataFromFile())
                        {
                            GetDataFromFile();
                        }
                    }, () => CanGetDataFromFile()));

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
                   _set.FileSettings.FileOutput = result.Object;
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
                GetOrpons();
            }, () => CanStartOrponing()));



        /// <summary>
        /// ������� ��� ��������� �������� �����������
        /// </summary>
        public RelayCommand CommandStopOrponing =>
        _commandStopOrponing ?? (_commandStopOrponing = new RelayCommand(
            () =>
            {
                cts.Cancel();
            }, () => _isStartOrponing));

        /// <summary>
        /// ������� ������� ����������� ���������� �������
        /// </summary>
        public RelayCommand<EntityOrpon> CommandGetOrpon =>
        _�ommandGetOrpon ?? (_�ommandGetOrpon = new RelayCommand<EntityOrpon>(
                    obj =>
                    {
                        GetOrponAsync(obj);
                        _stat.UpdateStatisticsCollection();
                    }));

        /// <summary>
        /// ������� ��� ����������� ������ � �����
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
        /// ������� ������� ���������
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
        /// ������� �������� �����
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
        /// ������� ��� ���������� ������ � ����
        /// </summary>
        public RelayCommand CommandSaveData =>
        _commandSaveData ?? (_commandSaveData = new RelayCommand(
            () =>
            {
                if (CanSaveFile())
                {
                    var data = new List<string>(_collection.Count)
                    {
                        $"�����;QualityCode;CheckStatus;ParsingLevelCode;GlobalID"
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
        /// ����� ��� ��������� ����� �������� �����
        /// </summary>
        /// <param name="file">��� �����</param>
        private void SetFileInput(string file)
        {
            _set.FileSettings.FileInput = file;
            GetDataFromFile();

            _set.FileSettings.FileOutput = GetDefaultName();
        }

        /// <summary>
        /// ����� ��� ��������� ����� ����� �� ���������
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
        /// ����� ��������� ������ �� �����
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
        /// ����� ��� ��������� ����� �������
        /// </summary>
        private void GetOrpons()
        {
            IsStartOrponing = true;
            _stat.Start();

            var listAddress = _collection.Partition(2);
            cts = new CancellationTokenSource();
            var t = cts.Token;

            ParallelOptions po = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 3,
                CancellationToken = t
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    Parallel.ForEach(listAddress, po, (item) =>
                    {
                        var add = item.Select(x =>
                        {
                            x.Status = StatusType.OrponingNow;
                            return x.Address;
                        }).ToArray();

                        var a = _orpon.GetOrpon(add);
                        if (a != null && a.Error == null)
                        {
                            var indexRow = 0;
                            foreach (var k in a.Objects)
                            {
                                item.ElementAt(indexRow).Orpon = k;
                                item.ElementAt(indexRow).Status = StatusType.OK;
                                indexRow++;
                            }
                        }
                        else if (a != null && a.Error != null)
                        {
                            foreach (var i in _collection)
                            {
                                i.Status = StatusType.Error;
                                i.Error = a.Error.Message;
                            }
                            _notification.NotificationAsync(null, a.Error.Message);
                        }
                    });
                }
                catch (Exception ex)
                {
                    _notification.NotificationAsync(null, ex.Message);
                }

                _notification.NotificationAsync(null, "OK");
                IsStartOrponing = false;
                _stat.Stop();
            }, t);
        }

        /// <summary>
        /// ����� ��� �������� ����������� �������� ������ � �������� ����
        /// </summary>
        /// <returns>���������� true ���� ��������� ����������, ����� ������� � ���� �������� ����</returns>
        private bool CanSaveFile()
        {
            return _collection != null && _collection.Any() && !string.IsNullOrEmpty(_set.FileSettings.FileOutput);
        }

        /// <summary>
        /// ����� ��� �������� ����������� ������� ���������
        /// </summary>
        /// <returns>���������� true, ���� ������� ����������� �� �������, ��������� ����������, � ��������� ���� ��������</returns>
        private bool CanClearCollection()
        {
            return !_isStartOrponing && _collection != null && _collection.Any();
        }

        /// <summary>
        /// ����� ��� �������� ����������� ��������� ������ �� �����
        /// </summary>
        /// <returns>���������� true, ���� ������� ����������� �� �������, ���� ����</returns>
        private bool CanGetDataFromFile()
        {
            return !_isStartOrponing && !string.IsNullOrEmpty(_set.FileSettings.FileInput);
        }

        /// <summary>
        /// ����� ��� �������� ����������� ������� �����������
        /// </summary>
        /// <returns>���������� true, ���� ������� ����������� �� �������, ��������� ����������, ��������� ����� ��������</returns>
        private bool CanStartOrponing()
        {
            return !_isStartOrponing && _collection != null && _collection.Any();
        }

        /// <summary>
        /// ����� ��� �������� ����������� ����������� ���������� �������
        /// </summary>
        /// <returns>���������� true, ���� ������� �� ����� null</returns>
        private bool CanGetOrpon()
        {
            return _currentOrpon != null;
        }

        #endregion PrivateMethod

        #region PublicMethod
        #endregion PublicMethod

        private async void GetOrponAsync(EntityOrpon obj)
        {
            obj.Status = StatusType.OrponingNow;

            var a = await _orpon.GetOrponAsync(obj.Address);
            if (a != null && a.Error == null && a.Object != null)
            {
                obj.Orpon = a.Object;
                obj.Status = StatusType.OK;
            }
            else if (a != null && a.Error != null)
            {
                obj.Status = StatusType.Error;
                obj.Error = a.Error.Message;
            }

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

        public MainViewModel(IFileService fileService, INotifications notification, StatisticsViewModel stat, SettingsViewModel set, IRepository orpon)
        {
            _fileService = fileService;
            _notification = notification;
            _stat = stat;
            _set = set;
            _orpon = orpon;
        }
    }
}