using AddressCoding.Entities.Settings;
using AddressCoding.FileService;
using AddressCoding.Notifications;
using GalaSoft.MvvmLight;
using System.IO;

namespace AddressCoding.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IFileService _fileService;
        private readonly INotifications _notification;

        private FileSettings _fileSettings = new FileSettings();
        /// <summary>
        /// Настройки работы с файлами
        /// </summary>
        public FileSettings FileSettings
        {
            get => _fileSettings;
            set => Set(ref _fileSettings, value);
        }

        public SettingsViewModel(IFileService fileService, INotifications notifications)
        {
            _fileService = fileService;
            _notification = notifications;

            var dir = Directory.GetCurrentDirectory();
            _fileSettings.FolderOutput = dir + "\\Output";
            _fileSettings.FolderInput = dir + "\\Input";
            _fileSettings.FolderTemp = dir + "\\Temp";
            _fileSettings.FolderStatistics = dir + "\\Statistics";
            _fileSettings.FolderErrors = dir + "\\Error";
            _fileSettings.FolderApp = dir;

            var result = _fileService.CreateFolder(new string[]
            {
                _fileSettings.FolderOutput,
                _fileSettings.FolderInput,
                _fileSettings.FolderTemp,
                _fileSettings.FolderStatistics,
                _fileSettings.FolderErrors
            });

            if (result != null && result.Error != null)
            {
                _notification.NotificationAsync(null, result.Error.Message);
            }
        }
    }
}