namespace AccountingTransactionService.Enums
{
    public enum EntityType
    {
        None = -1,        // Тип объекта не указан
        Point = 1,        // Терминал
        PoindDay = 2,     // Дневная касса
        PointEvening = 3, // Вечерняя касса
        PointHoliday = 4, // Касса выходного дня
        Dialer = 5,       // Головной банк
        Subdialer = 6,    // Филиал банка
        Gateway = 7,      // Шлюз
        Operator = 8,     // Платёж
        Сollector = 9     // Инкассатор
    }
}