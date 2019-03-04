using AddressCoding.Entities;
using System.Collections.Generic;

namespace AddressCoding.FileService
{
    /// <summary>
    /// Интерфейс для описания работы с файлами
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Метод выбора файла
        /// </summary>
        /// <param name="defaultFolder">Папка по умолчанию</param>
        /// <returns>Возвращает полный путь к файлу</returns>
        EntityResult<string> GetFile(string defaultFolder = "");
        /// <summary>
        /// Метод для выбора имени(папки) файла для сохранения данных
        /// </summary>
        /// <param name="defaultName">Имя файла по умолчанию</param>
        /// <returns>Возвращает полный путь к файлу для сохранения</returns>
        EntityResult<string> SetFileForSave(string defaultName = "");
        /// <summary>
        /// Метод для получения данных из файла
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <returns>Возвращает коллекцию строк из файла</returns>
        EntityResult<string> GetData(string fileName);
        /// <summary>
        /// Метод для сохранения данных в файл
        /// </summary>
        /// <param name="data">Коллекция данных</param>
        /// <param name="fileName">Имя файла</param>
        /// <returns>Возвращает результат записи</returns>
        EntityResult<string> SaveData(IEnumerable<string> data, string fileName);
        /// <summary>
        /// Метод для открытия папки по имени файла/папки
        /// </summary>
        /// <param name="path">Имя файла или папки</param>
        /// <returns>Возвращает результат открытия</returns>
        EntityResult<string> OpenFolder(string path);
    }
}