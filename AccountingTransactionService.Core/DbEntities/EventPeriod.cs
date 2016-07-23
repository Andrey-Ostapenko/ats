using System;

namespace AccountingTransactionService.DbEntities
{
    public class EventPeriod
    {
        public EventPeriod(DateTime dateFrom, DateTime dateTo)
        {
            DateFrom = dateFrom;
            DateTo = dateTo;
        }

        public DateTime DateFrom
        {
            set;
            get;
        }

        public DateTime DateTo
        {
            set;
            get;
        }
    }
}