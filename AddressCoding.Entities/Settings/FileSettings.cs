using GalaSoft.MvvmLight;

namespace AddressCoding.Entities.Settings
{
    /// <summary>
    /// Класс для хранения настроек работы с файлами
    /// </summary>
    public class FileSettings : ViewModelBase
    {
        private string _folderApp = string.Empty;
        /// <summary>
        /// Имя папки программы
        /// </summary>
        public string FolderApp
        {
            get => _folderApp;
            set => Set(ref _folderApp, value);
        }

        private string _folderInput = string.Empty;
        /// <summary>
        /// Имя папки для входящих файлов
        /// </summary>
        public string FolderInput
        {
            get => _folderInput;
            set => Set(ref _folderInput, value);
        }

        private string _folderOutput = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string FolderOutput
        {
            get => _folderOutput;
            set => Set(ref _folderOutput, value);
        }

        private string _folderTemp = string.Empty;
        /// <summary>
        /// Имя папки для временных файлов
        /// </summary>
        public string FolderTemp
        {
            get => _folderTemp;
            set => Set(ref _folderTemp, value);
        }

        private string _folderStatistics = string.Empty;
        /// <summary>
        /// Имя папки для статистики
        /// </summary>
        public string FolderStatistics
        {
            get => _folderStatistics;
            set => Set(ref _folderStatistics, value);
        }

        private string _folderErrors = string.Empty;
        /// <summary>
        /// Имя папки для ошибок
        /// </summary>
        public string FolderErrors
        {
            get => _folderErrors;
            set => Set(ref _folderErrors, value);
        }

        private int _maxSizePart = 0;
        /// <summary>
        /// Максимальное число строк в выходном файле
        /// </summary>
        public int MaxSizePart
        {
            get => _maxSizePart;
            set => Set(ref _maxSizePart, value);
        }

        private bool _canBreakFileOutput = false;
        /// <summary>
        /// Разбивать ли выходной файл построчно
        /// </summary>
        public bool CanBreakFileOutput
        {
            get => _canBreakFileOutput;
            set => Set(ref _canBreakFileOutput, value);
        }

        private string _fileInput = string.Empty;
        /// <summary>
        /// Имя входного файла
        /// </summary>
        public string FileInput
        {
            get => _fileInput;
            set => Set(ref _fileInput, value);
        }

        private string _fileOutput = string.Empty;
        /// <summary>
        /// Имя выходного файла
        /// </summary>
        public string FileOutput
        {
            get => _fileOutput;
            set => Set(ref _fileOutput, value);
        }

        private string _fileError = string.Empty;
        /// <summary>
        /// Имя файла с ошибками
        /// </summary>
        public string FileError
        {
            get => _fileError;
            set => Set(ref _fileError, value);
        }
    }
}