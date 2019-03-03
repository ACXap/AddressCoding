using AddressCoding.Entities;
using Microsoft.Win32;
using System;
using System.IO;

namespace AddressCoding.FileService
{
    /// <summary>
    /// Класс для работы с файлами с реализацией интерфейса IFileService
    /// </summary>
    public class FileService : IFileService
    {
        /// <summary>
        /// Фильтр типов расширения для файлов с данными
        /// </summary>
        private const string _filterForGetFile = "Файл - csv (*.csv)|*.csv|Файл - txt (*.txt)|*.txt|Все файлы (*.*)|*.*";
        /// <summary>
        /// Заголовок окна для выбора файла с данными
        /// </summary>
        private const string _titleFileGetDialog = "Выбрать файл с адресами";
        /// <summary>
        /// Фильтр типов расширения для сохраняемого файла
        /// </summary>
        private const string _filterForSaveFile = "Файл - csv (*.csv)|*.csv";
        /// <summary>
        /// Тип расширения для сохраняемого файла
        /// </summary>
        private const string _extensionFileForSave = ".csv";
        /// <summary>
        /// Заголовок для окна выбора файла для сохранения
        /// </summary>
        private const string _titleFileSaveDialog = "Указать имя сохраняемого файла";

        /// <summary>
        /// Метод выбора файла
        /// </summary>
        /// <param name="defaultFolder">Папка по умолчанию</param>
        /// <returns>Возвращает полный путь к файлу</returns>
        public EntityResult<string> GetFile(string defaultFolder)
        {
            EntityResult<string> result = new EntityResult<string>();

            OpenFileDialog fd = new OpenFileDialog()
            {
                Multiselect = false,
                Filter = _filterForGetFile,
                Title = _titleFileGetDialog,
                InitialDirectory = defaultFolder
            };

            try
            {
                if (fd.ShowDialog() == true)
                {
                    result.Object = fd.FileName;
                    result.Result = true;
                }
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.Error = ex;
            }

            return result;
        }

        /// <summary>
        /// Метод для выбора имени(папки) файла для сохранения данных
        /// </summary>
        /// <param name="defaultName">Имя файла по умолчанию</param>
        /// <returns>Возвращает полный путь к файлу для сохранения</returns>
        public EntityResult<string> SetFileForSave(string defaultName = "")
        {
            EntityResult<string> result = new EntityResult<string>();

            SaveFileDialog fd = new SaveFileDialog()
            {
                Filter = _filterForSaveFile,
                AddExtension = true,
                DefaultExt = _extensionFileForSave,
                //InitialDirectory = Path.GetDirectoryName(defaultName),
                Title = _titleFileSaveDialog,
                FileName = defaultName
            };

            try
            {
                if (fd.ShowDialog() == true)
                {
                    result.Object = fd.FileName;
                    result.Result = true;
                }
            }
            catch (Exception ex)
            {
                result.Error = ex;
                result.Result = false;
            }

            return result;
        }
    }
}