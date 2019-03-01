using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
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



        public MainViewModel()
        {

        }
    }
}