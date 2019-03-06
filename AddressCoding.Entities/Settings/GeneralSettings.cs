using GalaSoft.MvvmLight;
using MahApps.Metro;
using System.Collections.ObjectModel;
using System.Windows;

namespace AddressCoding.Entities.Settings
{
    public class GeneralSettings : ViewModelBase
    {
        /// <summary>
        /// Коллекция всех возможных тем оформления окна
        /// </summary>
        public ReadOnlyCollection<Theme> ListTheme => ThemeManager.Themes;

        private Theme _colorTheme = ThemeManager.DetectTheme();
        /// <summary>
        /// Текущая тема оформления окна
        /// </summary>
        public Theme ColorTheme
        {
            get => _colorTheme;
            set
            {
                Set(ref _colorTheme, value);
                if (value != null)
                {
                    ThemeManager.ChangeTheme(Application.Current, value.Name);
                }
                else
                {
                    _colorTheme = ThemeManager.DetectTheme();
                }


            }
        }

        /// <summary>
        /// Поле для хранения параметра орпонизировать все объекты
        /// </summary>
        private bool _canOrponingGetAll = true;
        /// <summary>
        /// Поле для хранения параметра орпонизировать только объекты с ошибками
        /// </summary>
        private bool _canOrponingGetError = false;
        /// <summary>
        /// Поле для хранения параметра орпонизировать неорпонизированные объекты
        /// </summary>
        private bool _canOrponingGetNotOrponing = false;

        /// Поле для хранения параметра сохранить все данные в полном объеме
        /// </summary>
        private bool _canSaveDataAsShot = true;
        /// <summary>
        /// Поле для хранения параметра сохранить все данные в кратком формате
        /// </summary>
        private bool _canSaveDataAsFull = false;
        /// <summary>
        /// Поле для хранения параметра открывать ли папку с результатами после орпонизации и сохранения
        /// </summary>
        private bool _canOpenFolderAfter = false;

        /// <summary>
        /// Орпонизировать все объекты
        /// </summary>
        public bool CanOrponingGetAll
        {
            get => _canOrponingGetAll;
            set => Set(ref _canOrponingGetAll, value);
        }

        /// <summary>
        /// Орпонизировать только объекты с ошибками
        /// </summary>
        public bool CanOrponingGetError
        {
            get => _canOrponingGetError;
            set => Set(ref _canOrponingGetError, value);

        }

        /// <summary>
        /// Орпонизировать неорпонизированные объекты
        /// </summary>
        public bool CanOrponingGetNotOrponing
        {
            get => _canOrponingGetNotOrponing;
            set => Set(ref _canOrponingGetNotOrponing, value);
        }

        /// <summary>
        /// Сохранить все данные геокодирования в полном объеме
        /// </summary>
        public bool CanSaveDataAsShot
        {
            get => _canSaveDataAsShot;
            set => Set(ref _canSaveDataAsShot, value);
        }

        /// <summary>
        /// Сохранить все данные геокодирования в формате для базы данных
        /// </summary>
        public bool CanSaveDataAsFull
        {
            get => _canSaveDataAsFull;
            set => Set(ref _canSaveDataAsFull, value);
        }

        /// <summary>
        /// открывать ли папку с результатом после сохранения
        /// </summary>
        public bool CanOpenFolderAfter
        {
            get => _canOpenFolderAfter;
            set => Set(ref _canOpenFolderAfter, value);
        }
    }
}