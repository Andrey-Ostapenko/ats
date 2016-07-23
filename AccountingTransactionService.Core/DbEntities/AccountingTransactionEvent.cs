using System;
using AccountingTransactionService.Configuration;
using AccountingTransactionService.Enums;

namespace AccountingTransactionService.DbEntities
{
    public class AccountingTransactionEvent
    {
        private readonly long _defaultPointId;

        public AccountingTransactionEvent()
        {
            _defaultPointId = ConfigurationProvider.DefaultPointId;
        }

        public int Id
        {
            set;
            get;
        }

        public EventType EventType
        {
            set;
            get;
        }

        public int PointId
        {
            set;
            get;
        }

        public string OperatorAccount
        {
            set;
            get;
        }

        public long Data
        {
            set;
            get;
        }

        public decimal Amount
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

        public DateTime? CreateDate
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

        public string EventName
        {
            get
            {
                string result = string.Empty;

                switch (EventType)
                {
                    case EventType.DayClose:
                        result = "Завершение операционного дня";
                        break;
                    case EventType.PointClose:
                        result = "Завершение работы кассы";
                        break;
                    case EventType.PaymentCompletion:
                        result = "Завершение обработки платежа";
                        break;
                    case EventType.PointCollection:
                        result = "Инкассация терминала";
                        break;
                }

                return result;
            }
        }

        public string InitialSessionNumber
        {
            set;
            get;
        }

        public override string ToString()
        {
            string result;

            if (PointId != _defaultPointId)
            {
                result = string.Format("{0} (Параметры Идентификатор={1}, Статус={2}, Касса={3}, Период обработки платежей с {4:dd.MM.yyyy HH:mm:ss} по {5:dd.MM.yyyy HH:mm:ss})", EventName, Id, Status, PointId, DateFrom, DateTo);
            }
            else
            {
                result = string.Format("{0} (Параметры: Идентификатор={1}, Статус={2}, Период обработки платежей с {3:dd.MM.yyyy HH:mm:ss} по {4:dd.MM.yyyy HH:mm:ss})", EventName, Id, Status, DateFrom, DateTo);
            }

            return result;
        }
    }
}