using AddressCoding.Entities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AddressCoding.FileService
{
    /// <summary>
    /// Класс для работы с файлами с реализацией интерфейса IFileService
    /// </summary>
    public class FileService : IFileService
    {
        #region PrivateField
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
        #endregion PrivateField

        #region PublicProperties
        #endregion PublicProperties

        #region PrivateMethod
        #endregion PrivateMethod

        #region PublicMethod
        
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

        /// <summary>
        /// Метод для получения данных из файла
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <returns>Возвращает коллекцию строк из файла</returns>
        public EntityResult<string> GetData(string fileName)
        {
            EntityResult<string> result = new EntityResult<string>();

            if (File.Exists(fileName))
            {
                var data = new List<string>();
                try
                {
                    using (StreamReader sr = new StreamReader(File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.UTF8))
                    {
                        while (!sr.EndOfStream)
                        {
                            data.Add(sr.ReadLine());
                        }
                    }
                    result.Objects = data;
                    result.Result = true;
                }
                catch (Exception ex)
                {
                    result.Error = ex;
                    result.Result = false;
                }
            }
            else
            {
                result.Result = false;
                result.Error = new FileNotFoundException(fileName);
            }

            return result;
        }

        /// <summary>
        /// Метод для сохранения данных в файл
        /// </summary>
        /// <param name="data">Коллекция данных</param>
        /// <param name="fileName">Имя файла</param>
        /// <returns>Возвращает результат записи</returns>
        public EntityResult<string> SaveData(IEnumerable<string> data, string fileName)
        {
            EntityResult<string> result = new EntityResult<string>();

            try
            {
                if(Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    using (StreamWriter sw = new StreamWriter(File.Create(fileName), Encoding.UTF8))
                    {
                        foreach(var item in data)
                        {
                            sw.WriteLine(item);
                        }
                    }
                }
                else
                {
                    result.Error = new DirectoryNotFoundException();
                }
            }
            catch (Exception ex)
            {
                result.Error = ex;
                result.Result = false;
            }

            return result;
        }
        
        /// <summary>
        /// Метод для открытия папки по имени файла/папки
        /// </summary>
        /// <param name="path">Имя файла или папки</param>
        /// <returns>Возвращает результат открытия</returns>
        public EntityResult<string> OpenFolder(string path)
        {
            EntityResult<string> result = new EntityResult<string>();

            if(string.IsNullOrEmpty(path))
            {
                result.Error = new ArgumentException(nameof(path));
                result.Result = false;
            }
            else
            {
                try
                {
                    if(File.Exists(path))
                    {
                        System.Diagnostics.Process.Start("explorer", @"/select, " + path);
                    }
                    else if (Directory.Exists(path))
                    {
                        System.Diagnostics.Process.Start(path);
                    }
                    else
                    {
                        var p = Path.GetDirectoryName(path);
                        if (Directory.Exists(p))
                        {
                            System.Diagnostics.Process.Start(p);
                        }
                        else
                        {
                            result.Error = new DirectoryNotFoundException();
                            result.Result = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.Error = ex;
                    result.Result = false;
                }
            }

            return result;
        }

        #endregion PublicMethod
    }
}