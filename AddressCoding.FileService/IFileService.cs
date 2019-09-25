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
        EntityResult<string> GetData(string fileName, bool canUseAnsi = false);
        /// <summary>
        /// Метод для сохранения данных в файл
        /// </summary>
        /// /// <param name="fileName">Имя файла</param>
        /// <param name="data">Коллекция данных</param>
        /// <returns>Возвращает результат записи</returns>
        EntityResult<string> SaveData(string fileName, IEnumerable<string> data);
        /// <summary>
        /// Метод для открытия папки по имени файла/папки
        /// </summary>
        /// <param name="path">Имя файла или папки</param>
        /// <returns>Возвращает результат открытия</returns>
        EntityResult<string> OpenFolder(string path);
        /// <summary>
        /// Метод для создания папки
        /// </summary>
        /// <param name="path">Имя папки</param>
        /// <returns>Возвращает результат создания</returns>
        EntityResult<string> CreateFolder(string path);
        /// <summary>
        /// Метод для создания папок
        /// </summary>
        /// <param name="path">Массив папок</param>
        /// <returns>Возвращает результаты создания</returns>
        EntityResult<string> CreateFolder(IEnumerable<string> path);
        /// <summary>
        /// Метод для проверки существования файла
        /// </summary>
        /// <param name="fileName">Полный путь к файлу</param>
        /// <returns>Возвращает результат проверки существования</returns>
        EntityResult<bool> ExistFile(string fileName);
        /// <summary>
        /// Метод добавления строк в файл
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <param name="data">Массив строк</param>
        /// <returns>Возвращает результат добавления</returns>
        EntityResult<string> AppendToFile(string fileName, IEnumerable<string> data);
    }
}