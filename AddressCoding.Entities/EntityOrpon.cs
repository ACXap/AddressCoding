using GalaSoft.MvvmLight;
using System;

namespace AddressCoding.Entities
{
    public class EntityOrpon : ViewModelBase
    {
        private int _id = 0;
        /// <summary>
        /// Айди объекта
        /// </summary>
        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        private string _address = string.Empty;
        /// <summary>
        /// Адрес объекта
        /// </summary>
        public string Address
        {
            get => _address;
            set => Set(ref _address, value);
        }

        private string _addressOrpon = string.Empty;
        /// <summary>
        /// Адрес объекта из базы
        /// </summary>
        public string AddressOrpon
        {
            get => _addressOrpon;
            set => Set(ref _addressOrpon, value);
        }

        private StatusType _status = StatusType.NotOrponing;
        /// <summary>
        /// 
        /// </summary>
        public StatusType Status
        {
            get => _status;
            set => Set(ref _status, value);
        }

        private string _error = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Error
        {
            get => _error;
            set => Set(ref _error, value);
        }

        private DateTime _dateTimeOrponing;
        /// <summary>
        /// 
        /// </summary>
        public DateTime DateTimeOrponing
        {
            get => _dateTimeOrponing;
            set => Set(ref _dateTimeOrponing, value);
        }

        private string _globalIdOriginal;
        /// <summary>
        /// 
        /// </summary>
        public string GlobalIdOriginal
        {
            get => _globalIdOriginal;
            set => Set(ref _globalIdOriginal, value);
        }

        private int _qCode = 0;
        /// <summary>
        /// 
        /// </summary>
        public int QCode
        {
            get => _qCode;
            set => Set(ref _qCode, value);
        }

        private int _qCodeNew = 0;
        /// <summary>
        /// 
        /// </summary>
        public int QCodeNew
        {
            get => _qCodeNew;
            set => Set(ref _qCodeNew, value);
        }

        private string _latitude = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Latitude
        {
            get => _latitude;
            set => Set(ref _latitude, value);
        }

        private string _longitude = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Longitude
        {
            get => _longitude;
            set => Set(ref _longitude, value);
        }

        private Orpon _orpon;
        /// <summary>
        /// ОРПОН - данные об объекте
        /// </summary>
        public Orpon Orpon
        {
            get => _orpon;
            set => Set(ref _orpon, value);
        }
    }
}