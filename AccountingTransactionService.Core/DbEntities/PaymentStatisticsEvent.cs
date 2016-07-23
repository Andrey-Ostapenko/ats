using System;
using AccountingTransactionService.Enums;

namespace AccountingTransactionService.DbEntities
{
    public class PaymentStatisticsEvent
    {
        public PaymentStatisticsEvent() { }

        public PaymentStatisticsEvent(DateTime dateFrom, DateTime dateTo)
        {
            DateFrom = dateFrom;
            DateTo = dateTo;
        }

        public PaymentStatisticsEvent(long id, DateTime dateFrom, DateTime dateTo)
        {
            Id = id;
            DateFrom = dateFrom;
            DateTo = dateTo;
        }

        public override string ToString()
        {
            return
                $"{"Формирование статистики по платежам"} (Параметры Идентификатор={Id}, Статус={Status}, Период обработки платежей с {DateFrom:dd.MM.yyyy HH:mm:ss} по {DateTo:dd.MM.yyyy HH:mm:ss})";
        }

        public long Id
        {
            set;
            get;
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


        public DateTime? SendDate
        {
            set;
            get;
        }

        public EventStatus Status
        {
            set;
            get;
        }
    }
}