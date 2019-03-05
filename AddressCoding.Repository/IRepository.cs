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
        EntityResult<Entities.Orpon> GetOrpon(string data);
        /// <summary>
        /// Асинхронный метод для получения объекта орпон
        /// </summary>
        /// <param name="data">Данные</param>
        /// <returns>Возвращает задачу</returns>
        Task<EntityResult<Entities.Orpon>> GetOrponAsync(string data);

        /// <summary>
        /// Метод получения объектов орпона
        /// </summary>
        /// <param name="data">Массив данных</param>
        /// <returns>Возвращает объекты орпона</returns>
        EntityResult<IEnumerable<Entities.Orpon>> GetOrpon(IEnumerable<string> data);
        /// <summary>
        /// Асинхронный метод получения объектов орпона
        /// </summary>
        /// <param name="data">Массив данных</param>
        /// <returns>Возвращает объекты орпона</returns>
        Task<EntityResult<IEnumerable<Entities.Orpon>>> GetOrponAsync(IEnumerable<string> data);

        Task<EntityResult<string>> TestConnectAsync();

        /// <summary>
        /// Метод настройки сервиса 
        /// </summary>
        /// <param name="address">Адрес сервиса</param>
        /// <param name="endpoint">Имя endpoint</param>
        void Initialize(string address, string endpoint);
    }
}