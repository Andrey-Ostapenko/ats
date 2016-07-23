namespace AccountingTransactionService.Enums
{
    public enum EventType
    {
        DayClose = 1,           // Закрытии операционного дня
        PointClose = 2,         // Завершение работы кассы
        PaymentCompletion = 3,  // Завершение обработки платежа
        PointCollection = 4     // Инкассация терминала
    }
}