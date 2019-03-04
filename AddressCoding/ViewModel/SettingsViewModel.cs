using AddressCoding.Entities.Settings;
using GalaSoft.MvvmLight;
using System.IO;

namespace AddressCoding.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private FileSettings _fileSettings = new FileSettings();
        /// <summary>
        /// 
        /// </summary>
        public FileSettings FileSettings
        {
            get => _fileSettings;
            set => Set(ref _fileSettings, value);
        }

        public SettingsViewModel()
        {
            var dir = Directory.GetCurrentDirectory();
            _fileSettings.FolderOutput = dir + "\\Output";
            _fileSettings.FolderInput = dir + "\\Input";
            _fileSettings.FolderTemp = dir + "\\Temp";
            _fileSettings.FolderStatistics = dir + "\\Statistics";
            _fileSettings.FolderErrors = dir + "\\Error";
            _fileSettings.FolderApp = dir;
        }
    }
}