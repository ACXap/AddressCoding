using AddressCoding.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AddressCoding.Repository
{
    public interface IRepository
    {
        /// <summary>
        /// Метод получения объекта орпона
        /// </summary>
        /// <param name="data">Данные</param>
        /// <returns>Возвращает объект орпона</returns>
        EntityResult<bool> GetOrpon(EntityOrpon data);
        /// <summary>
        /// Асинхронный метод для получения объекта орпон
        /// </summary>
        /// <param name="data">Данные</param>
        /// <returns>Возвращает задачу</returns>
        Task<EntityResult<bool>> GetOrponAsync(EntityOrpon data);

        /// <summary>
        /// Метод получения объектов орпона
        /// </summary>
        /// <param name="data">Массив данных</param>
        /// <returns>Возвращает объекты орпона</returns>
        EntityResult<bool> GetOrpon(IEnumerable<EntityOrpon> data, bool canCheckSinglObj);
        /// <summary>
        /// Асинхронный метод получения объектов орпона
        /// </summary>
        /// <param name="data">Массив данных</param>
        /// <returns>Возвращает объекты орпона</returns>
        Task<EntityResult<bool>> GetOrponAsync(IEnumerable<EntityOrpon> data, bool canCheckSinglObj);

        /// <summary>
        /// Асинхронный метод проверки подключения
        /// </summary>
        /// <returns>Возвращает результат подключения</returns>
        Task<EntityResult<string>> TestConnectAsync();

        /// <summary>
        /// Метод настройки сервиса 
        /// </summary>
        /// <param name="address">Адрес сервиса</param>
        /// <param name="endpoint">Имя endpoint</param>
        void Initialize(string address, string endpoint);
    }
}