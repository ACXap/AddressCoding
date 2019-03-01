using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows;

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



        public MainViewModel()
        {

        }
    }
}