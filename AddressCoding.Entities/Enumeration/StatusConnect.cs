namespace AddressCoding.Entities
{
    /// <summary>
    /// Перечень статусов подключения к базе данных
    /// </summary>
    public enum StatusConnect
    {
        /// <summary>
        /// не подключено
        /// </summary>
        NotConnect,
        /// <summary>
        /// Подключение успешно
        /// </summary>
        OK,
        /// <summary>
        /// Подключение с ошибкой
        /// </summary>
        Error,
        /// <summary>
        /// Подключение в данный момент
        /// </summary>
        ConnectNow
    }
}