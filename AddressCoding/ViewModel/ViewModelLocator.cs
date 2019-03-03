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
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}