using AddressCoding.FileService;
using AddressCoding.Notifications;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;

namespace AddressCoding.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<IFileService, FileService.FileService>();
            SimpleIoc.Default.Register<INotifications, Notification>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<StatisticsViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public INotifications Notifications
        {
            get
            {
                return ServiceLocator.Current.GetInstance<INotifications>();
            }
        }

        public StatisticsViewModel Stat
        {
            get
            {
                return ServiceLocator.Current.GetInstance<StatisticsViewModel>();
            }
        }

        public SettingsViewModel Settings
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingsViewModel>();
            }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}