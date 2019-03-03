using AddressCoding.Entities;

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
    }
}