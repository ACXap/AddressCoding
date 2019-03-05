using GalaSoft.MvvmLight;
using MahApps.Metro;
using System.Collections.ObjectModel;
using System.Windows;

namespace AddressCoding.Entities.Settings
{
    public class GeneralSettings : ViewModelBase
    {
        //private string _theme = string.Empty;
        ///// <summary>
        ///// Название темы
        ///// </summary>
        //public string Thema
        //{
        //    get => _theme;
        //    set => Set(ref _theme, value);
        //}

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
    }
}