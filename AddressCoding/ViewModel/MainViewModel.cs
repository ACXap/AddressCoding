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
            set
            {
                Set(ref _isStartOrponing, value);
                IsRequestedStop = false;
            }
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
                            }, obj => !_isStartOrponing));

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
                    }, () => !_isStartOrponing));

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
                GetOrponsAsync();

            }, () => CanStartOrponing()));



        /// <summary>
        /// ������� ��� ��������� �������� �����������
        /// </summary>
        public RelayCommand CommandStopOrponing =>
        _commandStopOrponing ?? (_commandStopOrponing = new RelayCommand(
            () =>
            {
                cts.Cancel();
                IsRequestedStop = true;
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
                OpenFolder(obj);
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
                    SaveData();
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
        private async void GetOrponsAsync()
        {
            if (_set.RepositorySettings.StatusConnect != StatusConnect.OK)
            {
                _notification.NotificationAsync(null, "Not set connect OrponService");
                return;
            }

            IEnumerable<IEnumerable<EntityOrpon>> listAddress = null;

            if (_set.GeneralSettings.CanOrponingGetAll)
            {
                listAddress = _collection.Partition(_set.RepositorySettings.MaxObj);
            }
            else if (_set.GeneralSettings.CanOrponingGetError)
            {
                listAddress = _collection.Where(x => x.Status == StatusType.Error).Partition(2);
            }
            else
            {
                listAddress = _collection.Where(x => x.Status == StatusType.NotOrponing).Partition(2);
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
                                foreach (var i in item)
                                {
                                    i.Error = a.Error.Message;
                                    i.Status = StatusType.Error;
                                }
                            }
                        });

                        _notification.NotificationAsync(null, "OK");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }, t);
                SaveData();
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

        /// <summary>
        /// ����� ��� ���������� ������ � ����
        /// </summary>
        private void SaveData()
        {
            if (_set.GeneralSettings.CanSaveDataAsShot)
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
                        $"�����;QualityCode;CheckStatus;ParsingLevelCode;GlobalID;SystemCode;KLADRLocalityId;FIASLocalityId;" +
                        $"KLADRStreetId;FIASStreetId;Street;StreetKind;House;HouseLitera;CornerHouse;BuildingBlock;" +
                        $"BuildingBlockLitera;Building;BuildingLitera;Ownership;OwnershipLitera;FIASHouseId"
                    };
                data.AddRange(_collection.Select(x =>
                {
                    return $"{x.Address};{x.Orpon?.QualityCode};{x.Orpon?.CheckStatus};{x.Orpon?.ParsingLevelCode};{x.Orpon?.GlobalID};{x.Orpon?.SystemCode};" +
                    $"{x.Orpon?.KLADRLocalityId};{x.Orpon?.FIASLocalityId};{x.Orpon?.KLADRStreetId};{x.Orpon?.FIASStreetId};{x.Orpon?.Street};{x.Orpon?.StreetKind};{x.Orpon?.House};" +
                    $"{x.Orpon?.HouseLitera};{x.Orpon?.CornerHouse};{x.Orpon?.BuildingBlock};{x.Orpon?.BuildingBlockLitera};{x.Orpon?.Building};{x.Orpon?.BuildingLitera};" +
                    $"{x.Orpon?.Ownership};{x.Orpon?.OwnershipLitera};{x.Orpon?.FIASHouseId}";
                }));

                var result = _fileService.SaveData($"{_set.FileSettings.FolderTemp}\\{Path.GetFileName(_set.FileSettings.FileOutput)}", data);

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

        #endregion PrivateMethod

        #region PublicMethod
        #endregion PublicMethod

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

        private async void GetOrponAsync(EntityOrpon obj)
        {
            if (_set.RepositorySettings.StatusConnect != StatusConnect.OK)
            {
                _notification.NotificationAsync(null, "Not set connect OrponService");
                return;
            }

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

        private bool _isRequestedStop = false;
        /// <summary>
        /// 
        /// </summary>
        public bool IsRequestedStop
        {
            get => _isRequestedStop;
            set => Set(ref _isRequestedStop, value);
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