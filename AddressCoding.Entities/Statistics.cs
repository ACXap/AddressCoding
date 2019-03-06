using GalaSoft.MvvmLight;
using System;

namespace AddressCoding.Entities
{
    public class Statistics:ViewModelBase
    {
        private int _allEntity = 0;
        /// <summary>
        /// Всего объектов в коллекции
        /// </summary>
        public int AllEntity
        {
            get => _allEntity;
            set => Set(ref _allEntity, value);
        }
        private int _ok = 0;
        /// <summary>
        /// Всего объектов со статусом "ОК"
        /// </summary>
        public int OK
        {
            get => _ok;
            set => Set(ref _ok, value);
        }
        private int _error = 0;
        /// <summary>
        /// Всего объектов со статусом "Ошибка"
        /// </summary>
        public int Error
        {
            get => _error;
            set => Set(ref _error, value);
        }
        private int _notOrponing = 0;
        /// <summary>
        /// Всего объектов со статусом "Неорпонизированые"
        /// </summary>
        public int NotOrponing
        {
            get => _notOrponing;
            set => Set(ref _notOrponing, value);
        }
        private int _orponingNow = 0;
        /// <summary>
        /// Всего объектов со статусом "Орпонизируется сейчас"
        /// </summary>
        public int OrponingNow
        {
            get => _orponingNow;
            set => Set(ref _orponingNow, value);
        }
        private int _house = 0;
        /// <summary>
        /// Всего объектов с типом объекта "Дом"
        /// </summary>
        public int House
        {
            get => _house;
            set => Set(ref _house, value);
        }
        private int _exact = 0;
        /// <summary>
        /// Всего объектов с качеством орпонизации "Точное орпонизирование"
        /// </summary>
        public int Exact
        {
            get => _exact;
            set => Set(ref _exact, value);
        }
        private int _notFound = 0;
        /// <summary>
        /// Всего объектов для которых не найдены варианты
        /// </summary>
        public int NotFound
        {
            get => _notFound;
            set => Set(ref _notFound, value);
        }

        private TimeSpan _timeGeoCod;
        /// <summary>
        /// Время орпонизации в секундах
        /// </summary>
        public TimeSpan TimeGeoCod
        {
            get => _timeGeoCod;
            set => Set(ref _timeGeoCod, value);
        }

        private TimeSpan _timeLeftGeoCod;
        /// <summary>
        /// Время оставшееся до окончания
        /// </summary>
        public TimeSpan TimeLeftGeoCod
        {
            get => _timeLeftGeoCod;
            set => Set(ref _timeLeftGeoCod, value);
        }

        private double _percent;
        /// <summary>
        /// Процент выполненной орпонизации
        /// </summary>
        public double Percent
        {
            get => _percent;
            set => Set(ref _percent, value);
        }
    }
}