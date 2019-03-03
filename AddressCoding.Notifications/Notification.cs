using MahApps.Metro.Controls.Dialogs;
using System;

namespace AddressCoding.Notifications
{
    public class Notification : INotifications
    {
        /// <summary>
        /// Поле для хранения ссылки на координатора диалогов
        /// </summary>
        private readonly IDialogCoordinator _dialogCoordinator = DialogCoordinator.Instance;

        public Notification()
        {

        }
        public async void NotificationAsync(string header, string body)
        {
            await _dialogCoordinator.ShowMessageAsync(this, header, body);
        }
    }
}
