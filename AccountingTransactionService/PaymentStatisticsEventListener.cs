using System;
using System.Threading;
using AccountingTransactionService.Configuration;
using AccountingTransactionService.DbEntities;
using AccountingTransactionService.Enums;
using AccountingTransactionService.Interfaces;
using Utilities.Logging;

namespace AccountingTransactionService
{
    class PaymentStatisticsEventListener
    {
        private Thread thread;
        private bool threadRunningFlag;
        private IDbProvider dbProvider;
        private IGatewayProvider gatewayProvider;
        private object syncObject;

        public PaymentStatisticsEventListener(object syncObject, IDbProvider dbProvider, IGatewayProvider gatewayProvider)
        {
            this.dbProvider = dbProvider;
            this.gatewayProvider = gatewayProvider;
            this.syncObject = syncObject;
            threadRunningFlag = false;
        }

        private void readEvents()
        {
            try
            {
                PaymentStatisticsEventCollection payStatEvents = dbProvider.GetPaymentStatisticsEvents();

                foreach (PaymentStatisticsEvent payStatEvent in payStatEvents)
                {
                    try
                    {
                        Log.WriteToFile("*".PadRight(80, '*'));
                        Log.WriteToFile(payStatEvent.ToString());
                        PaymentStatisticsCollection paymentStatistics = dbProvider.GetPaymentStatistics(payStatEvent);
                        Log.WriteToFile("Cтатистика по платежам сформированна успешно.");

                        gatewayProvider.SendPaymentStatistics(paymentStatistics);
                        Log.WriteToFile("Cтатистика по платежам сохранена в базе данных.");
                   
                        payStatEvent.SendDate = DateTime.Now;
                        payStatEvent.Status = EventStatus.DataSaved;
                        dbProvider.UpdatePaymentStatisticsEvent(payStatEvent);
                        Log.WriteToFile(string.Format("Статус события по формированию статистики {0} изменён на {1}", payStatEvent.Id, payStatEvent.Status));
                    }
                    catch (Exception ex)
                    {
                        Log.WriteToFile("Произошла ошибка:");
                        Log.WriteToFile(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Log.WriteToFile("Произошла ошибка:");
                    Log.WriteToFile(ex);
                }
                catch
                {
                }
            }
        }

        private void writeEvents()
        {
            try
            {
                EventPeriod eventPeriod = dbProvider.GetLastPaymentStatisticsEventPeriod();

                while (eventPeriod.DateFrom < eventPeriod.DateTo)
                {
                    PaymentStatisticsEvent payStatEvent = new PaymentStatisticsEvent();
                    DateTime dateTo = eventPeriod.DateFrom.AddDays(1);
                    payStatEvent.DateFrom = eventPeriod.DateFrom;
                    payStatEvent.DateTo = dateTo;
                    eventPeriod.DateFrom = dateTo;
                    dbProvider.InsertPaymentStatisticsEvent(payStatEvent);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Log.WriteToFile("Произошла ошибка:");
                    Log.WriteToFile(ex);
                }
                catch
                {
                }
            }
        }

        private void listen()
        {
            try
            {
                while (true)
                {
                    lock (syncObject)
                    {
                        writeEvents();
                        readEvents();
                    }

                    Thread.Sleep(ConfigurationProvider.PollTimerInterval);
                }
            }
            catch (ThreadAbortException)
            {
                try
                {
                    Thread.ResetAbort();
                }
                catch
                {
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Log.WriteToFile(ex);
                }
                catch
                {
                }
            }
        }

        public void Start()
        {
            if (!threadRunningFlag)
            {
                thread = new Thread(new ThreadStart(listen));
                thread.Start();
                threadRunningFlag = true;
            }
        }

        public void Stop()
        {
            if (threadRunningFlag)
            {
                lock (syncObject)
                {
                    thread.Abort();
                }
            }
        }
    }
}
