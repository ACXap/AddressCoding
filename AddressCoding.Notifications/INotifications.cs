namespace AddressCoding.Notifications
{
    public interface INotifications
    {
        /// <summary>
        /// Простое оповещение без вида работы и без возможности управлять
        /// </summary>
        /// <param name="header">Заголовок оповещения</param>
        /// <param name="body">Сообщение оповещения</param>
        void NotificationAsync(string header, string body);
    }
}