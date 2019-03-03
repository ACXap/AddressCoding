using AddressCoding.Entities;
using Microsoft.Win32;
using System;

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
        /// Метод выбора файла
        /// </summary>
        /// <param name="defaultFolder">Папка по умолчанию</param>
        /// <returns>Возвращает полный путь к файлу</returns>
        EntityResult<string> IFileService.GetFile(string defaultFolder)
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

    }
}