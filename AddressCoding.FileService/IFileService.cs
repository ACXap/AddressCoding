using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressCoding.FileService
{
    /// <summary>
    /// Интерфейс для описания работы с файлами
    /// </summary>
    public interface IFileService
    {

        void GetFile( string defaultFolder = "");
    }
}
