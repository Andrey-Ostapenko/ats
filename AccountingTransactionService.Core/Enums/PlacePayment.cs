namespace AccountingTransactionService.Enums
{
    public enum PlacePayment
    {
        All = 1, // формируется всегда
        Subdealer = 2, // формируется только в филиалах
        Dealer = 3 // формируется только в головном банке
    }
}