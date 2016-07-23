using System;
using System.Threading;
using AccountingTransactionService.Configuration;
using AccountingTransactionService.CustomExceptions;
using AccountingTransactionService.DbEntities;
using AccountingTransactionService.Enums;
using AccountingTransactionService.Interfaces;
using Utilities.Logging;

namespace AccountingTransactionService
{
    internal class AccountingTransactionEventListener
    {
        private readonly AccountingTransactionProvider accTranProvider;
        private readonly IDbProvider dbProvider;
        private readonly IGatewayProvider gatewayProvider;
        private readonly object syncObject;
        private Thread _thread;
        private bool _threadRunningFlag;

        public AccountingTransactionEventListener(object syncObject, IDbProvider dbProvider,
            IGatewayProvider gatewayProvider)
        {
            this.dbProvider = dbProvider;
            accTranProvider = new AccountingTransactionProvider(dbProvider);
            this.gatewayProvider = gatewayProvider;
            this.syncObject = syncObject;
            _threadRunningFlag = false;
        }

        private void writeAccountingTransactionsToLog(AccountingTransactionCollection accountingTransactions)
        {
            Log.WriteToFile("Сформированы документы:");

            foreach (var accTransaction in accountingTransactions)
            {
                Log.WriteToFile("Документ:");

                foreach (var key in accTransaction.DocumentFields.Keys)
                {
                    Log.WriteToFile(string.Format("Поле {0} : {1}", key, accTransaction.DocumentFields[key]));
                }
            }

            Log.WriteToFile("-".PadRight(80, '-'));
        }

        private void readEvents()
        {
            try
            {
                AccountingTransactionEventCollection accTranEvents;

                try
                {
                    accTranEvents = dbProvider.GetAccountingTransactionEvents();
                }
                catch (Exception ex)
                {
                    Log.WriteToFile(ex);
                    throw ex;
                }

                foreach (var accTranEvent in accTranEvents)
                {
                    try
                    {
                        Log.WriteToFile("*".PadRight(80, '*'));
                        Log.WriteToFile(string.Format("{0}", accTranEvent));

                        AccountingTransactionCollection accTransactions = null;

                        if (accTranEvent.Status == EventStatus.EventCreated)
                        {
                            try
                            {
                                accTransactions = accTranProvider.CreateAccountingTransactions(accTranEvent);
                                Log.WriteToFile("Проводки сформированы.");
                            }
                            catch (AccountingTransactionsException ex)
                            {
                                Log.WriteToFile(ex.Message);
                                throw ex;
                            }
                            catch (Exception ex)
                            {
                                Log.WriteToFile("Произошла ошибка:");
                                Log.WriteToFile(ex);
                                throw new AccountingTransactionsException();
                            }

                            if (accTransactions != null)
                            {
                                try
                                {
                                    dbProvider.InsertAccountingTransactions(accTranEvent, accTransactions);
                                    Log.WriteToFile("Проводоки сохранены в таблицу AccountingTransactions.");
                                    Log.WriteToFile(
                                        $"Статус события {accTranEvent.Id} изменён на {accTranEvent.Status}.");
                                }
                                catch (Exception ex)
                                {
                                    Log.WriteToFile("Произошла ошибка:");
                                    Log.WriteToFile(ex);
                                    throw ex;
                                }
                            }

                            /*
                            try
                            {
                                accTranEvent.CreateDate = DateTime.Now;
                                accTranEvent.SendDate = null;
                                accTranEvent.Status = EventStatus.DataCreated;
                                dbProvider.UpdateAccountingTransactionEvent(accTranEvent);
                                Log.WriteToFile(string.Format("Статус события {0} изменён на {1}.", accTranEvent.Id, accTranEvent.Status));
                            }
                            catch (Exception ex)
                            {
                                Log.WriteToFile("Произошла ошибка:");
                                Log.WriteToFile(ex);
                                throw ex;
                            }
                            */
                        }

                        if (accTranEvent.Status == EventStatus.DataCreated)
                        {
                            try
                            {
                                accTransactions = dbProvider.GetAccountingTransactions(accTranEvent);
                                Log.WriteToFile("Выполнена загрузка проводок из таблицы AccountingTransactions.");
                                writeAccountingTransactionsToLog(accTransactions);
                            }
                            catch (Exception ex)
                            {
                                Log.WriteToFile("Произошла ошибка:");
                                Log.WriteToFile(ex);
                                throw ex;
                            }

                            try
                            {
                                gatewayProvider.SendAccountingTransactions(accTransactions);
                                Log.WriteToFile("Сохранение проводок в базе данных АБС выполнено.");
                            }
                            catch (Exception ex)
                            {
                                Log.WriteToFile("Произошла ошибка:");
                                Log.WriteToFile(ex);
                                throw ex;
                            }

                            try
                            {
                                accTranEvent.SendDate = DateTime.Now;
                                accTranEvent.Status = EventStatus.DataSaved;
                                dbProvider.UpdateAccountingTransactionEvent(accTranEvent);
                                Log.WriteToFile(string.Format("Статус события {0} изменён на {1}.", accTranEvent.Id,
                                    accTranEvent.Status));
                            }
                            catch (Exception ex)
                            {
                                Log.WriteToFile("Произошла ошибка:");
                                Log.WriteToFile(ex);
                                throw ex;
                            }
                        }
                    }
                    catch (AccountingTransactionEventReadException)
                    {
                    }
                    catch (AccountingTransactionsException)
                    {
                    }
                    catch (AccountingTransactionEventStatusUpdateException)
                    {
                    }
                    catch (AccountingTransactionsSaveToFsException)
                    {
                    }
                    catch (AccountingTransactionsReadException)
                    {
                    }
                    catch (AccountingTransactionsSaveToAbsException)
                    {
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
            }
            catch (AccountingTransactionEventReadException ex)
            {
                try
                {
                    Log.WriteToFile(ex.Message);
                }
                catch
                {
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
                var eventPeriod = dbProvider.GetLastAccountingTransactionEventPeriod();
                var dateTo = eventPeriod.DateFrom.Date.AddDays(1).Add(ConfigurationProvider.DayCloseTime);

                if (eventPeriod.DateTo > dateTo) //while (eventPeriod.DateTo > dateTo)
                {
                    var accTranEvent = new AccountingTransactionEvent();
                    accTranEvent.EventType = EventType.DayClose;
                    accTranEvent.DateFrom = eventPeriod.DateFrom;
                    accTranEvent.DateTo = dateTo;
                    //eventPeriod.DateFrom.Date.AddDays(1).Add(ConfigurationProvider.DayCloseTime);
                    //Log.WriteToFile(accTranEvent.DateFrom.ToString("dd.MM.yyyy HH:mm:ss"));
                    //Log.WriteToFile(accTranEvent.DateTo.ToString("dd.MM.yyyy HH:mm:ss"));
                    dbProvider.InsertAccountingTransactionEvent(accTranEvent);
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
                        if (ConfigurationProvider.DayCloseEvent) writeEvents();
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
                    Log.WriteToFile("Произошла ошибка:");
                    Log.WriteToFile(ex);
                }
                catch
                {
                }
            }
        }

        public void Start()
        {
            if (!_threadRunningFlag)
            {
                _thread = new Thread(listen);
                _thread.Start();
                _threadRunningFlag = true;
            }
        }

        public void Stop()
        {
            if (_threadRunningFlag)
            {
                lock (syncObject)
                {
                    _thread.Abort();
                }
            }
        }
    }
}