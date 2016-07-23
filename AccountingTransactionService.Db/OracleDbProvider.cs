using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml.Serialization;
using AccountingTransactionService.Configuration;
using AccountingTransactionService.CustomExceptions;
using AccountingTransactionService.DbEntities;
using AccountingTransactionService.Enums;
using AccountingTransactionService.Interfaces;
using AccountingTransactionService.XmlEntities;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using Utilities.Logging;

namespace AccountingTransactionService.Db
{
    public class OracleDbProvider : IDbProvider
    {
        private const int NotDefineValue = -1;

        #region Операции над событиями для формирования проводок

        public EventPeriod GetLastAccountingTransactionEventPeriod()
        {
            try
            {
                EventPeriod result = null;

                using (OracleConnection connection = new OracleConnection(ConfigurationProvider.ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand(ConfigurationProvider.SqlSelectLastAccountingTransactionEventPeriod, connection))
                    {
                        connection.Open();

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result = new EventPeriod(Convert.ToDateTime(reader["datefrom"]), Convert.ToDateTime(reader["dateto"]));
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new AccountingTransactionEventPeriodReadException("Ошибка чтения периода времени операционного дня.", ex);
            }
        }

        public void InsertAccountingTransactionEvent(AccountingTransactionEvent accTranEvent)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationProvider.ConnectionString))
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand(ConfigurationProvider.SqlInsertAccountingTransactionEvent, connection))
                    {
                        command.Parameters.Add(new OracleParameter("eventtypeid", OracleDbType.Int32, (int)accTranEvent.EventType, ParameterDirection.Input));
                        command.Parameters.Add(new OracleParameter("datefrom", OracleDbType.Date, accTranEvent.DateFrom, ParameterDirection.Input));
                        command.Parameters.Add(new OracleParameter("dateto", OracleDbType.Date, accTranEvent.DateTo, ParameterDirection.Input));
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new AccountingTransactionEventSaveException("Ошибка записи в базу данных события для формирования проводок.", ex);
            }
        }

        public AccountingTransactionEventCollection GetAccountingTransactionEvents()
        {
            try
            {
                AccountingTransactionEventCollection result = new AccountingTransactionEventCollection();

                using (OracleConnection connection = new OracleConnection(ConfigurationProvider.ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand(ConfigurationProvider.SqlSelectAccountingTransactionEvents, connection))
                    {
                        connection.Open();

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AccountingTransactionEvent accTranEvent = new AccountingTransactionEvent();
                                accTranEvent.Id = Convert.ToInt32(reader["id"]);
                                accTranEvent.EventType = (EventType)Convert.ToInt32(reader["eventtypeid"]);
                                accTranEvent.PointId = Convert.ToInt32(reader["terminalid"]);
                                accTranEvent.OperatorAccount = reader["operatoraccount"].ToString();
                                accTranEvent.DateFrom = Convert.ToDateTime(reader["datefrom"]);
                                accTranEvent.DateTo = Convert.ToDateTime(reader["dateto"]);
                                accTranEvent.Data = Convert.ToInt64(reader["data"]);
                                accTranEvent.Status = (EventStatus)Convert.ToInt32(reader["status"]);
                                accTranEvent.Amount = !(reader["amount"] is DBNull) ? Convert.ToDecimal(reader["amount"]) : 0M;
                                accTranEvent.InitialSessionNumber = reader["InitialSessionNumber"].ToString();
                                result.Add(accTranEvent);
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new AccountingTransactionEventReadException("События для формирования проводок не прочитаны из базы данных.", ex);
            }
        }

        public void UpdateAccountingTransactionEvent(AccountingTransactionEvent accTranEvent)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationProvider.ConnectionString))
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand(ConfigurationProvider.SqlUpdateAccountingTransactionEvent, connection))
                    {
                        command.Parameters.Add(new OracleParameter("createdate", OracleDbType.Date, accTranEvent.CreateDate, ParameterDirection.Input));
                        command.Parameters.Add(new OracleParameter("senddate", OracleDbType.Date, accTranEvent.SendDate, ParameterDirection.Input));
                        command.Parameters.Add(new OracleParameter("status", OracleDbType.Int32, (int)accTranEvent.Status, ParameterDirection.Input));
                        command.Parameters.Add(new OracleParameter("id", OracleDbType.Int64, accTranEvent.Id, ParameterDirection.Input));
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new AccountingTransactionEventStatusUpdateException(string.Format("Ошибка изменения статуса события {0}.", accTranEvent.Id), ex);
            }
        }

        #endregion Операции над событиями для формирования проводок

        #region Операции над событиями для формирования статистики по платежам

        public EventPeriod GetLastPaymentStatisticsEventPeriod()
        {
            try
            {
                EventPeriod result = null;

                using (OracleConnection connection = new OracleConnection(ConfigurationProvider.ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand(ConfigurationProvider.SqlSelectLastPaymentStatisticsEventPeriod, connection))
                    {
                        connection.Open();

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result = new EventPeriod(Convert.ToDateTime(reader["datefrom"]), Convert.ToDateTime(reader["dateto"]));
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new PaymentStatisticsPeriodReadException("Ошибка чтения периода времени операционного дня.", ex);
            }
        }

        public void InsertPaymentStatisticsEvent(PaymentStatisticsEvent payStatEvent)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationProvider.ConnectionString))
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand(ConfigurationProvider.SqlInsertPaymentStatisticsEvent, connection))
                    {
                        command.Parameters.Add(new OracleParameter("datefrom", OracleDbType.Date, payStatEvent.DateFrom, ParameterDirection.Input));
                        command.Parameters.Add(new OracleParameter("dateto", OracleDbType.Date, payStatEvent.DateTo, ParameterDirection.Input));
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new PaymentStatisticsEventSaveException("Ошибка записи в базу данных события для формирования статистики по платежам.", ex);
            }
        }

        public PaymentStatisticsEventCollection GetPaymentStatisticsEvents()
        {
            try
            {
                PaymentStatisticsEventCollection result = new PaymentStatisticsEventCollection();

                using (OracleConnection connection = new OracleConnection(ConfigurationProvider.ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand(ConfigurationProvider.SqlSelectPaymentStatisticsEvents, connection))
                    {
                        connection.Open();

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PaymentStatisticsEvent payStatEvent = new PaymentStatisticsEvent();
                                payStatEvent.Id = Convert.ToInt64(reader["id"]);
                                payStatEvent.DateFrom = Convert.ToDateTime(reader["datefrom"]);
                                payStatEvent.DateTo = Convert.ToDateTime(reader["dateto"]);
                                payStatEvent.Status = (EventStatus)Convert.ToInt32(reader["status"]);
                                result.Add(payStatEvent);
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new PaymentStatisticsEventReadException("События для формирования статистики по платежам не прочитаны из базы данных.", ex);
            }
        }

        public void UpdatePaymentStatisticsEvent(PaymentStatisticsEvent payStatEvent)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationProvider.ConnectionString))
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand(ConfigurationProvider.SqlUpdatePaymentStatisticsEvent, connection))
                    {
                        command.Parameters.Add(new OracleParameter("senddate", OracleDbType.Date, payStatEvent.SendDate, ParameterDirection.Input));
                        command.Parameters.Add(new OracleParameter("status", OracleDbType.Int32, (int)payStatEvent.Status, ParameterDirection.Input));
                        command.Parameters.Add(new OracleParameter("id", OracleDbType.Int64, payStatEvent.Id, ParameterDirection.Input));
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new AccountingTransactionEventStatusUpdateException(string.Format("Ошибка изменения статуса события {0}.", payStatEvent.Id), ex);
            }
        }

        #endregion Операции над событиями для формирования статистики по платежам

        #region Получение исходных данных о реквизитах платежа

        public Payment GetPayment(int pointId, string sessionNumber)
        {
            Payment result = new Payment();

            using (OracleConnection connection = new OracleConnection(ConfigurationProvider.ConnectionString))
            {
                using (OracleCommand command = new OracleCommand(ConfigurationProvider.SqlSelectPayment, connection))
                {
                    command.Parameters.Add(new OracleParameter("terminalid", OracleDbType.Int32, pointId, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("initialsessionnumber", OracleDbType.Varchar2, 20, sessionNumber, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("terminalid", OracleDbType.Int32, pointId, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("initialsessionnumber", OracleDbType.Varchar2, 20, sessionNumber, ParameterDirection.Input));
                    connection.Open();

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string attributes;

                            result.Id = Convert.ToInt64(reader["PaymentId"]);

                            result.DealerId = reader["DealerId"] is DBNull ? NotDefineValue : Convert.ToInt32(reader["DealerId"]);
                            result.DealerName = reader["DealerName"] is DBNull ? string.Empty : reader["DealerName"].ToString();

                            attributes = reader["DealerAttributes"] is DBNull ? null : reader["DealerAttributes"].ToString();

                            try
                            {
                                result.Dealer = attributes != null ? (Bank)getEntity(attributes, typeof(Bank)) : null;
                            }
                            catch (Exception)
                            {
                                result.Dealer = new Bank();
                                Log.WriteToFile(
                                    $"Ошибка формата реквизитов банка {result.DealerName} ({result.DealerId}).");
                            }

                            result.SubdealerId = Convert.ToInt32(reader["SubdealerId"]);
                            result.SubdealerName = reader["SubdealerName"].ToString();

                            attributes = reader["SubdealerAttributes"] is DBNull ? null : reader["SubdealerAttributes"].ToString();

                            try
                            {
                                result.Subdealer = attributes != null ? (Bank)getEntity(attributes, typeof(Bank)) : null;
                            }
                            catch (Exception)
                            {
                                result.Subdealer = new Bank();
                                Log.WriteToFile(string.Format("Ошибка формата реквизитов филиала {0} ({1}).", result.SubdealerName, result.SubdealerId));
                            }


                            result.PointId = Convert.ToInt32(reader["PointId"]);
                            result.PointName = reader["PointName"].ToString();
                            result.PointExternalCode = reader["PointExternalCode"].ToString();

                            result.GatewayId = Convert.ToInt32(reader["GatewayId"]);
                            result.GatewayName = reader["GatewayName"].ToString();

                            attributes = reader["GatewayAttributes"] is DBNull ? null : reader["GatewayAttributes"].ToString();

                            try
                            {
                                result.GatewayRecipient = attributes != null ? (Organization)getEntity(attributes, typeof(Organization)) : null;
                            }
                            catch (Exception)
                            {
                                result.GatewayRecipient = new Organization();
                                Log.WriteToFile(string.Format("Ошибка формата реквизитов платёжного шлюза {0} ({1}).", result.GatewayName, result.GatewayId));
                            }

                            try
                            {
                                result.GatewayBank = attributes != null ? (Bank)getEntity(attributes, typeof(Bank)) : null;
                            }
                            catch (Exception)
                            {
                                result.GatewayBank = new Bank();
                                Log.WriteToFile(string.Format("Ошибка формата реквизитов платёжного шлюза {0} ({1}).", result.GatewayName, result.GatewayId));
                            }

                            try
                            {
                                Tax taxRequisites = attributes != null ? (Tax)getEntity(attributes, typeof(Tax)) : null;

                                if (taxRequisites != null)
                                {
                                    result.KBK = taxRequisites.KBK;
                                    result.OKATO = taxRequisites.OKATO;
                                }
                            }
                            catch (Exception)
                            {
                                Log.WriteToFile(string.Format("Ошибка формата реквизитов платёжного шлюза {0} ({1}).", result.GatewayName, result.GatewayId));
                            }

                            result.OperatorId = Convert.ToInt64(reader["OperatorId"]);
                            result.OperatorName = reader["OperatorName"].ToString();
                            result.PaymentTarget = reader["PaymentTarget"].ToString();

                            attributes = reader["OperatorAttributes"] is DBNull ? null : reader["OperatorAttributes"].ToString();

                            try
                            {
                                result.OperatorRecipient = attributes != null ? (Organization)getEntity(attributes, typeof(Organization)) : null;
                            }
                            catch (Exception)
                            {
                                result.OperatorRecipient = new Organization();
                                Log.WriteToFile(string.Format("Ошибка формата реквизитов платежа {0} ({1}).", result.OperatorName, result.OperatorId));
                            }

                            try
                            {
                                result.OperatorBank = attributes != null ? (Bank)getEntity(attributes, typeof(Bank)) : null;
                            }
                            catch (Exception)
                            {
                                result.OperatorBank = new Bank();
                                Log.WriteToFile(string.Format("Ошибка формата реквизитов платежа {0} ({1}).", result.OperatorName, result.OperatorId));
                            }

                            result.Amount = Convert.ToDecimal(reader["Amount"]);
                            result.AmountWithoutCommission = Convert.ToDecimal(reader["AmountWithoutCommission"]);
                            result.ClientCommissionAmount = Convert.ToDecimal(reader["ClientComissionAmount"]);
                            result.ContractCommissionAmount = Convert.ToDecimal(reader["ContractCommissionAmount"]);
                        }
                    }
                }
            }

            return result;
        }

        public Payment GetRequsites(long providerId, int pointId)
        {
            Payment result = new Payment();

            using (OracleConnection connection = new OracleConnection(ConfigurationProvider.ConnectionString))
            {
                using (OracleCommand command = new OracleCommand(ConfigurationProvider.SqlSelectRequisites, connection))
                {
                    command.Parameters.Add(new OracleParameter("TerminalId", OracleDbType.Int32, pointId, ParameterDirection.Input));                    
                    command.Parameters.Add(new OracleParameter("ProviderId", OracleDbType.Int64, providerId, ParameterDirection.Input));
                    connection.Open();

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string attributes;

                            result.DealerId = reader["DealerId"] is DBNull ? NotDefineValue : Convert.ToInt32(reader["DealerId"]);
                            result.DealerName = reader["DealerName"] is DBNull ? string.Empty : reader["DealerName"].ToString();

                            attributes = reader["DealerAttributes"] is DBNull ? null : reader["DealerAttributes"].ToString();

                            try
                            {
                                result.Dealer = attributes != null ? (Bank)getEntity(attributes, typeof(Bank)) : null;
                            }
                            catch (Exception)
                            {
                                result.Dealer = new Bank();
                                Log.WriteToFile(string.Format("Ошибка формата реквизитов банка {0} ({1}).", result.DealerName, result.DealerId));
                            }

                            result.SubdealerId = Convert.ToInt32(reader["SubdealerId"]);
                            result.SubdealerName = reader["SubdealerName"].ToString();

                            attributes = reader["SubdealerAttributes"] is DBNull ? null : reader["SubdealerAttributes"].ToString();

                            try
                            {
                                result.Subdealer = attributes != null ? (Bank)getEntity(attributes, typeof(Bank)) : null;
                            }
                            catch (Exception)
                            {
                                result.Subdealer = new Bank();
                                Log.WriteToFile(string.Format("Ошибка формата реквизитов филиала {0} ({1}).", result.SubdealerName, result.SubdealerId));
                            }


                            result.PointId = Convert.ToInt32(reader["PointId"]);
                            result.PointName = reader["PointName"].ToString();
                            result.PointExternalCode = reader["PointExternalCode"].ToString();

                            result.GatewayId = Convert.ToInt32(reader["GatewayId"]);
                            result.GatewayName = reader["GatewayName"].ToString();

                            attributes = reader["GatewayAttributes"] is DBNull ? null : reader["GatewayAttributes"].ToString();

                            try
                            {
                                result.GatewayRecipient = attributes != null ? (Organization)getEntity(attributes, typeof(Organization)) : null;
                            }
                            catch (Exception)
                            {
                                result.GatewayRecipient = new Organization();
                                Log.WriteToFile(string.Format("Ошибка формата реквизитов платёжного шлюза {0} ({1}).", result.GatewayName, result.GatewayId));
                            }

                            try
                            {
                                result.GatewayBank = attributes != null ? (Bank)getEntity(attributes, typeof(Bank)) : null;
                            }
                            catch (Exception)
                            {
                                result.GatewayBank = new Bank();
                                Log.WriteToFile(string.Format("Ошибка формата реквизитов платёжного шлюза {0} ({1}).", result.GatewayName, result.GatewayId));
                            }

                            try
                            {
                                Tax taxRequisites = attributes != null ? (Tax)getEntity(attributes, typeof(Tax)) : null;

                                if (taxRequisites != null)
                                {
                                    result.KBK = taxRequisites.KBK;
                                    result.OKATO = taxRequisites.OKATO;
                                }
                            }
                            catch (Exception)
                            {
                                Log.WriteToFile(string.Format("Ошибка формата реквизитов платёжного шлюза {0} ({1}).", result.GatewayName, result.GatewayId));
                            }

                            result.OperatorId = Convert.ToInt64(reader["OperatorId"]);
                            result.OperatorName = reader["OperatorName"].ToString();
                            result.OperatorComposite = Convert.ToInt32(reader["OperatorComposite"]);

                            attributes = reader["OperatorAttributes"] is DBNull ? null : reader["OperatorAttributes"].ToString();

                            try
                            {
                                result.OperatorRecipient = attributes != null ? (Organization)getEntity(attributes, typeof(Organization)) : null;
                            }
                            catch (Exception)
                            {
                                result.OperatorRecipient = new Organization();
                                Log.WriteToFile(string.Format("Ошибка формата реквизитов платежа {0} ({1}).", result.OperatorName, result.OperatorId));
                            }

                            try
                            {
                                result.OperatorBank = attributes != null ? (Bank)getEntity(attributes, typeof(Bank)) : null;
                            }
                            catch (Exception)
                            {
                                result.OperatorBank = new Bank();
                                Log.WriteToFile(string.Format("Ошибка формата реквизитов платежа {0} ({1}).", result.OperatorName, result.OperatorId));
                            }

                            result.PaymentTarget = reader["PaymentTarget"].ToString();
                        }
                    }
                }
            }

            return result;
        }

        #endregion Получение исходных данных о реквизитах платежа

        #region Получение исходных данных о платежах, инкассациях и счетах и комиссиях        

        public PaymentCollection GetPayments(AccountingTransactionEvent accTranEvent)
        {
            Dictionary<long, Bank> dialers = new Dictionary<long, Bank>();
            Dictionary<long, Bank> subdialers = new Dictionary<long, Bank>();
            Dictionary<long, Bank> gatewayBanks = new Dictionary<long, Bank>();
            Dictionary<long, Organization> gatewayRecipients = new Dictionary<long, Organization>();
            Dictionary<long, Bank> operatorBanks = new Dictionary<long, Bank>();
            Dictionary<long, Organization> operatorRecipients = new Dictionary<long, Organization>();
            Dictionary<long, OrganizationPayer> operatorPayers = new Dictionary<long, OrganizationPayer>();

            PaymentCollection result = new PaymentCollection();

            using (OracleConnection connection = new OracleConnection(ConfigurationProvider.ConnectionString))
            {
                string commandText;

                if (accTranEvent.EventType == EventType.DayClose)
                {
                    commandText = ConfigurationProvider.SqlSelectPayments;
                }
                //else if (accTranEvent.EventType == EventType.PaymentCompletion)
                //{
                //    commandText = ConfigurationProvider.SqlSelectOtpPayment;
                //}
                else
                {
                    commandText = ConfigurationProvider.SqlSelectPointPayments;
                }

                using (OracleCommand command = new OracleCommand(commandText, connection))
                {
                    command.Parameters.Add(new OracleParameter("terminalid", OracleDbType.Int32, accTranEvent.PointId, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("terminalid", OracleDbType.Int32, accTranEvent.PointId, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("datefrom", OracleDbType.Date, accTranEvent.DateFrom, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("dateto", OracleDbType.Date, accTranEvent.DateTo, ParameterDirection.Input));

                    if (accTranEvent.EventType == EventType.PointClose)
                    {
                        command.Parameters.Add(new OracleParameter("operatoraccount", OracleDbType.Varchar2, 64, accTranEvent.OperatorAccount, ParameterDirection.Input));
                    }

                    //if (accTranEvent.EventType == EventType.PaymentCompletion)
                    //{
                    //    command.Parameters.Add(new OracleParameter("initialsessionnumber", OracleDbType.Varchar2, 20, accTranEvent.InitialSessionNumber, ParameterDirection.Input));
                    //}

                    command.Parameters.Add(new OracleParameter("terminalid", OracleDbType.Int32, accTranEvent.PointId, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("terminalid", OracleDbType.Int32, accTranEvent.PointId, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("datefrom", OracleDbType.Date, accTranEvent.DateFrom, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("dateto", OracleDbType.Date, accTranEvent.DateTo, ParameterDirection.Input));
                    
                    if (accTranEvent.EventType == EventType.PointClose)
                    {
                        command.Parameters.Add(new OracleParameter("operatoraccount", OracleDbType.Varchar2, 64, accTranEvent.OperatorAccount, ParameterDirection.Input));
                    }

                    //if (accTranEvent.EventType == EventType.PaymentCompletion)
                    //{
                    //    command.Parameters.Add(new OracleParameter("initialsessionnumber", OracleDbType.Varchar2, 20, accTranEvent.InitialSessionNumber, ParameterDirection.Input));
                    //}

                    connection.Open();

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string attributes;
                            Payment payment = new Payment();

                            payment.Id = Convert.ToInt64(reader["PaymentId"]);

                            payment.SessionNumber = reader["InitialSessionNumber"] is DBNull ? string.Empty : reader["InitialSessionNumber"].ToString();
                            payment.DealerId = reader["DealerId"] is DBNull ? NotDefineValue : Convert.ToInt32(reader["DealerId"]);
                            payment.DealerName = reader["DealerName"] is DBNull ? string.Empty : reader["DealerName"].ToString();

                            attributes = reader["DealerAttributes"] is DBNull ? null : reader["DealerAttributes"].ToString();

                            if (dialers.ContainsKey(payment.DealerId))
                            {
                                payment.Dealer = dialers[payment.DealerId];
                            }
                            else
                            {
                                try
                                {
                                    payment.Dealer = attributes != null ? (Bank)getEntity(attributes, typeof(Bank)) : null;
                                }
                                catch (Exception)
                                {
                                    payment.Dealer = new Bank();
                                    Log.WriteToFile(string.Format("Ошибка формата реквизитов банка {0} ({1}).", payment.DealerName, payment.DealerId));
                                }
                                finally
                                {
                                    dialers.Add(payment.DealerId, payment.Dealer);
                                }
                            }

                            payment.SubdealerId = Convert.ToInt32(reader["SubdealerId"]);
                            payment.SubdealerName = reader["SubdealerName"].ToString();

                            attributes = reader["SubdealerAttributes"] is DBNull ? null : reader["SubdealerAttributes"].ToString();

                            if (subdialers.ContainsKey(payment.SubdealerId))
                            {
                                payment.Subdealer = subdialers[payment.SubdealerId];
                            }
                            else
                            {
                                try
                                {
                                    payment.Subdealer = attributes != null ? (Bank)getEntity(attributes, typeof(Bank)) : null;
                                }
                                catch (Exception)
                                {
                                    payment.Subdealer = new Bank();
                                    Log.WriteToFile(string.Format("Ошибка формата реквизитов филиала {0} ({1}).", payment.SubdealerName, payment.SubdealerId));
                                }
                                finally
                                {
                                    subdialers.Add(payment.SubdealerId, payment.Subdealer);
                                }
                            }

                            if (payment.Subdealer.Main == 1)
                            {
                                payment.DealerId = payment.SubdealerId;
                                payment.Dealer = payment.Subdealer;
                                payment.DealerName = payment.SubdealerName;
                            }

                            payment.PointId = Convert.ToInt32(reader["PointId"]);
                            payment.PointName = reader["PointName"].ToString();
                            payment.PointExternalCode = reader["PointExternalCode"].ToString();

                            payment.GatewayId = Convert.ToInt32(reader["GatewayId"]);
                            payment.GatewayName = reader["GatewayName"].ToString();

                            attributes = reader["GatewayAttributes"] is DBNull ? null : reader["GatewayAttributes"].ToString();

                            if (gatewayRecipients.ContainsKey(payment.GatewayId))
                            {
                                payment.GatewayRecipient = gatewayRecipients[payment.GatewayId];
                            }
                            else
                            {
                                try
                                {                                    
                                    payment.GatewayRecipient = attributes != null ? (Organization)getEntity(attributes, typeof(Organization)) : null;
                                }
                                catch (Exception)
                                {
                                    payment.GatewayRecipient = new Organization();
                                    Log.WriteToFile(string.Format("Ошибка формата реквизитов платёжного шлюза {0} ({1}).", payment.GatewayName, payment.GatewayId));
                                }
                                finally
                                {
                                    gatewayRecipients.Add(payment.GatewayId, payment.GatewayRecipient);
                                }
                            }

                            if (gatewayBanks.ContainsKey(payment.GatewayId))
                            {
                                payment.GatewayBank = gatewayBanks[payment.GatewayId];
                            }
                            else
                            {
                                try
                                {
                                    payment.GatewayBank = attributes != null ? (Bank)getEntity(attributes, typeof(Bank)) : null;
                                }
                                catch (Exception)
                                {
                                    payment.GatewayBank = new Bank();
                                    Log.WriteToFile(string.Format("Ошибка формата реквизитов платёжного шлюза {0} ({1}).", payment.GatewayName, payment.GatewayId));
                                }
                                finally
                                {
                                    gatewayBanks.Add(payment.GatewayId, payment.GatewayBank);
                                }
                            }

                            try
                            {
                                Tax taxRequisites = attributes != null ? (Tax)getEntity(attributes, typeof(Tax)) : null;

                                if (taxRequisites != null)
                                {
                                    payment.KBK = taxRequisites.KBK;
                                    payment.OKATO = taxRequisites.OKATO;
                                }
                            }
                            catch (Exception)
                            {
                                Log.WriteToFile(string.Format("Ошибка формата реквизитов платёжного шлюза {0} ({1}).", payment.GatewayName, payment.GatewayId));
                            }

                            /*
                            try
                            {
                                PaymentPurpose paymentPurpose = attributes != null ? (PaymentPurpose)getEntity(attributes, typeof(PaymentPurpose)) : null;

                                if (paymentPurpose != null)
                                {
                                    payment.PaymentTarget = paymentPurpose.Purpose;
                                }
                            }
                            catch (Exception)
                            {
                                Log.WriteToFile(string.Format("Ошибка формата реквизитов платёжного шлюза {0} ({1}).", payment.GatewayName, payment.GatewayId));
                            }
                            */

                            payment.OperatorId = Convert.ToInt64(reader["OperatorId"]);
                            payment.OperatorName = reader["OperatorName"].ToString();
                            payment.OperatorComposite = Convert.ToInt32(reader["OperatorComposite"]);                            

                            attributes = reader["OperatorAttributes"] is DBNull ? null : reader["OperatorAttributes"].ToString();

                            if (operatorPayers.ContainsKey(payment.OperatorId))
                            {
                                payment.OperatorPayer = operatorPayers[payment.OperatorId];
                            }
                            else
                            {
                                try
                                {
                                    payment.OperatorPayer = attributes != null ? (OrganizationPayer)getEntity(attributes, typeof(OrganizationPayer)) : null;
                                }
                                catch (Exception)
                                {
                                    payment.OperatorPayer = new OrganizationPayer();
                                    Log.WriteToFile(string.Format("Ошибка формата реквизитов провайдера {0} ({1}).", payment.OperatorName, payment.OperatorId));
                                }
                                finally
                                {
                                    operatorPayers.Add(payment.OperatorId, payment.OperatorPayer);
                                }
                            }

                            if (operatorRecipients.ContainsKey(payment.OperatorId))
                            {
                                payment.OperatorRecipient = operatorRecipients[payment.OperatorId];
                            }
                            else
                            {
                                try
                                {
                                    payment.OperatorRecipient = attributes != null ? (Organization)getEntity(attributes, typeof(Organization)) : null;
                                }
                                catch (Exception)
                                {
                                    payment.OperatorRecipient = new Organization();
                                    Log.WriteToFile(string.Format("Ошибка формата реквизитов провайдера {0} ({1}).", payment.OperatorName, payment.OperatorId));
                                }
                                finally
                                {
                                    operatorRecipients.Add(payment.OperatorId, payment.OperatorRecipient);
                                }
                            }

                            if (operatorBanks.ContainsKey(payment.OperatorId))
                            {
                                payment.OperatorBank = operatorBanks[payment.OperatorId];
                            }
                            else
                            {
                                try
                                {
                                    payment.OperatorBank = attributes != null ? (Bank)getEntity(attributes, typeof(Bank)) : null;
                                }
                                catch (Exception)
                                {
                                    payment.OperatorBank = new Bank();
                                    Log.WriteToFile(string.Format("Ошибка формата реквизитов провайдера {0} ({1}).", payment.OperatorName, payment.OperatorId));
                                }
                                finally
                                {
                                    operatorBanks.Add(payment.OperatorId, payment.OperatorBank);
                                }
                            }

                            try
                            {
                                Tax taxRequisites = attributes != null ? (Tax)getEntity(attributes, typeof(Tax)) : null;

                                if (taxRequisites != null)
                                {
                                    payment.KBK = taxRequisites.KBK;
                                    payment.OKATO = taxRequisites.OKATO;
                                }
                            }
                            catch (Exception)
                            {
                                Log.WriteToFile(string.Format("Ошибка формата реквизитов провайдера {0} ({1}).", payment.OperatorName, payment.OperatorId));
                            }

                            /*
                            try
                            {
                                PaymentPurpose paymentPurpose = attributes != null ? (PaymentPurpose)getEntity(attributes, typeof(PaymentPurpose)) : null;
                                
                                if (paymentPurpose != null)
                                {
                                    payment.PaymentTarget = paymentPurpose.Purpose;
                                }
                            }
                            catch (Exception)
                            {
                                Log.WriteToFile(string.Format("Ошибка формата реквизитов провайдера {0} ({1}).", payment.OperatorName, payment.OperatorId));
                            }
                            */

                            payment.Parameters = reader["Params"].ToString();
                            payment.Amount = Convert.ToDecimal(reader["Amount"]);
                            payment.AmountWithoutCommission = Convert.ToDecimal(reader["AmountWithoutCommission"]);
                            payment.ClientCommissionAmount = Convert.ToDecimal(reader["ClientComissionAmount"]);
                            payment.ContractCommissionAmount = Convert.ToDecimal(reader["ContractCommissionAmount"]);
                            payment.AmountAll = Convert.ToDecimal(reader["AmountAll"]);
                            payment.Date = Convert.ToDateTime(reader["PaymentDate"]);
                            payment.PaymentTarget = reader["PaymentTarget"].ToString();                             
                            result.Add(payment);
                        }
                    }
                }
            }

            return result;
        }

        public Payment GetOtpPayment(AccountingTransactionEvent accTranEvent)
        {
            Payment result = new Payment();

            using (OracleConnection connection = new OracleConnection(ConfigurationProvider.ConnectionString))
            {
                using (OracleCommand command = new OracleCommand(ConfigurationProvider.SqlSelectOtpPayment, connection))
                {
                    command.Parameters.Add(new OracleParameter("terminalid", OracleDbType.Int32, accTranEvent.PointId, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("initialsessionnumber", OracleDbType.Varchar2, 20, accTranEvent.InitialSessionNumber, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("terminalid", OracleDbType.Int32, accTranEvent.PointId, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("initialsessionnumber", OracleDbType.Varchar2, 20, accTranEvent.InitialSessionNumber, ParameterDirection.Input));

                    connection.Open();

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Id = Convert.ToInt64(reader["PaymentId"]);
                            result.SessionNumber = reader["SessionNumber"] is DBNull ? string.Empty : reader["SessionNumber"].ToString();
                            result.Date = Convert.ToDateTime(reader["PaymentDate"]);
                            result.Amount = Convert.ToDecimal(reader["Amount"]);
                            result.ClientCommissionAmount = Convert.ToDecimal(reader["ClientComissionAmount"]);
                            result.AmountAll = Convert.ToDecimal(reader["AmountAll"]);
                            result.Parameters = reader["Params"].ToString();
                            result.PointId = Convert.ToInt32(reader["PointId"]);
                            result.PointName = reader["PointName"].ToString();
                            result.PointExternalCode = reader["PointExternalCode"].ToString();
                            result.PointAccount = reader["PointAccount"].ToString();
                            result.GatewayId = Convert.ToInt32(reader["GatewayId"]);
                            result.GatewayName = reader["GatewayName"].ToString();
                            result.OperatorId = Convert.ToInt64(reader["OperatorId"]);
                            result.OperatorName = reader["OperatorName"].ToString();
                            //result.PaymentTarget = reader["PaymentTarget"].ToString();
                            result.IntegratorCommission = GetIntegratorCommission(result.GatewayId, result.OperatorId, result.Date, result.Amount, result.ClientCommissionAmount);

                            string attributes = reader["OperatorAttributes"] is DBNull ? null : reader["OperatorAttributes"].ToString();

                            try
                            {
                                PaymentPurpose purpose = attributes != null ? (PaymentPurpose)getEntity(attributes, typeof(PaymentPurpose)) : null;

                                if (purpose != null)
                                {
                                    result.PaymentTarget = purpose.Purpose;
                                }
                            }
                            catch (Exception)
                            {
                                Log.WriteToFile(string.Format("Ошибка формата назначения платежа платежа {0} ({1}).", result.OperatorName, result.OperatorId));
                            }
                        }
                    }
                }
            }

            return result;
        }

        public TerminalCollection GetTerminalCollection(AccountingTransactionEvent accTranEvent)
        {
            TerminalCollection result = new TerminalCollection();

            using (OracleConnection connection = new OracleConnection(ConfigurationProvider.ConnectionString))
            {
                using (OracleCommand command = new OracleCommand(ConfigurationProvider.SqlSelectTerminalCollection, connection))
                {
                    command.Parameters.Add(new OracleParameter("terminalid", OracleDbType.Int32, accTranEvent.PointId, ParameterDirection.Input));
                    connection.Open();
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Id = Convert.ToInt32(reader["TerminalId"]);
                            result.Name = reader["Name"].ToString();
                            result.ExternalCode = reader["ExternalCode"].ToString();

                            try
                            {
                                result.PayOfficeId = Convert.ToInt32(reader["PayOfficeId"]);
                            }
                            catch (InvalidCastException)
                            {
                                if (reader["PayOfficeId"] is DBNull)
                                {
                                    throw new Exception(string.Format("В настройках терминала не задана касса для инкассации терминала {0}.", result.Id));
                                }
                            }

                            result.PayOfficeName = reader["PayOfficeName"].ToString();
                            result.AccountiongTransactionSheme = Convert.ToInt32(reader["AccTranSchemeId"]);
                            result.SubdealerId = Convert.ToInt32(reader["SubdealerId"]);
                            result.SubdealerName = reader["SubdealerName"].ToString();
                            result.Amount = accTranEvent.Amount;

                            string attributes = reader["SubdealerAttributes"] is DBNull ? null : reader["SubdealerAttributes"].ToString();

                            try
                            {
                                result.Subdealer = attributes != null ? (Bank)getEntity(attributes, typeof(Bank)) : new Bank();
                            }
                            catch (Exception)
                            {
                                result.Subdealer = new Bank();
                                Log.WriteToFile(string.Format("Ошибка формата реквизитов филиала {0} ({1}).", result.SubdealerName, result.SubdealerId));
                            }
                        }
                    }
                }
            }

            return result;
        }

        public PaymentStatisticsCollection GetPaymentStatistics(PaymentStatisticsEvent payStatEvent)
        {
            PaymentStatisticsCollection result = new PaymentStatisticsCollection();

            using (OracleConnection connection = new OracleConnection(ConfigurationProvider.ConnectionString))
            {
                using (OracleCommand selPayStatCommand = new OracleCommand(ConfigurationProvider.SqlSelectPaymentStatistics, connection))
                {
                    selPayStatCommand.Parameters.Add(new OracleParameter("datefrom", OracleDbType.Date, payStatEvent.DateFrom, ParameterDirection.Input));
                    selPayStatCommand.Parameters.Add(new OracleParameter("dateto", OracleDbType.Date, payStatEvent.DateTo, ParameterDirection.Input));
                    selPayStatCommand.Parameters.Add(new OracleParameter("datefrom", OracleDbType.Date, payStatEvent.DateFrom, ParameterDirection.Input));
                    selPayStatCommand.Parameters.Add(new OracleParameter("dateto", OracleDbType.Date, payStatEvent.DateTo, ParameterDirection.Input));
                    connection.Open();
                    using (OracleDataReader selPayStatReader = selPayStatCommand.ExecuteReader())
                    {
                        while (selPayStatReader.Read())
                        {
                            PaymentStatistics payStatistics = new PaymentStatistics();
                            payStatistics.PointId = selPayStatReader["pointId"].ToString();
                            payStatistics.PaymentDate = Convert.ToDateTime(selPayStatReader["paymentdate"]);
                            payStatistics.PaymentName = Convert.ToString(selPayStatReader["paymentname"]);
                            payStatistics.Count = Convert.ToInt32(selPayStatReader["count"]);
                            payStatistics.Amount = Convert.ToDecimal(selPayStatReader["amount"]);
                            payStatistics.CommissionAmount = Convert.ToDecimal(selPayStatReader["commissionamount"]);
                            result.Add(payStatistics);
                        }
                    }
                }
            }

            return result;
        }

        public AccountBindingCollection GetAccountBindings()
        {
            AccountBindingCollection result = new AccountBindingCollection();

            using (OracleConnection connection = new OracleConnection(ConfigurationProvider.ConnectionString))
            {
                using (OracleCommand command = new OracleCommand(ConfigurationProvider.SqlSelectAccountBindings, connection))
                {
                    connection.Open();

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AccountBinding binding = new AccountBinding();
                            binding.AccountBindingTemplatateId = Convert.ToInt32(reader["AccountBindingTemplateId"]);
                            binding.EntityId1 = Convert.ToInt64(reader["EntityId1"]);
                            binding.EntityId2 = Convert.ToInt64(reader["EntityId2"]);
                            binding.Data = Convert.ToInt64(reader["Data"]);
                            binding.AccountName = reader["AccountName"].ToString();
                            binding.Account = reader["Account"].ToString();
                            binding.IsDynamic = Convert.ToInt32(reader["IsDynamic"]) != 0;
                            result.Add(binding);
                        }
                    }
                }
            }

            return result;
        }

        public ReplacementAccountCollection GetReplacementAccounts()
        {
            ReplacementAccountCollection result = new ReplacementAccountCollection();

            using (OracleConnection connection = new OracleConnection(ConfigurationProvider.ConnectionString))
            {
                using (OracleCommand command = new OracleCommand(ConfigurationProvider.SqlSelectReplacementAccounts, connection))
                {
                    connection.Open();

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {                            
                            string key = reader["ReplacementAccount"].ToString();
                            
                            if (!result.ContainsKey(key))
                            {
                                ReplacementAccount account = new ReplacementAccount();
                                account.FilterExp = reader["FilterExp"].ToString();
                                result.Add(key, account);
                            }

                            InlineAccount inlineAccount = new InlineAccount();

                            inlineAccount.Account = reader["InlineAccount"].ToString();
                            inlineAccount.Filter = reader["Filter"].ToString();

                            result[key].Add(inlineAccount);
                        }
                    }
                }
            }

            return result;
        }

        public decimal GetIntegratorCommission(long gatewayId, long operatorId, DateTime paymentDate, decimal amount, decimal clientCommission)
        {
            decimal result;

            using (OracleConnection connection = new OracleConnection(ConfigurationProvider.ConnectionString))
            {
                using (OracleCommand command = new OracleCommand(ConfigurationProvider.SqlGetIntegratorCommission, connection))
                {
                    command.Parameters.Add(new OracleParameter("result", OracleDbType.Int32, ParameterDirection.ReturnValue));
                    command.Parameters.Add(new OracleParameter("p_err_msg", OracleDbType.Varchar2, 4000, DBNull.Value, ParameterDirection.Output));
                    command.Parameters.Add(new OracleParameter("p_integrator_commis", OracleDbType.Decimal, DBNull.Value, ParameterDirection.Output));
                    command.Parameters.Add(new OracleParameter("p_gateway_id", OracleDbType.Int64, gatewayId, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_operator_id", OracleDbType.Int64, operatorId, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_paymentdate", OracleDbType.Date, paymentDate, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_amount", OracleDbType.Decimal, amount, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_client_commis", OracleDbType.Decimal, clientCommission, ParameterDirection.Input));

                    connection.Open();
                    command.ExecuteNonQuery();

                    int res = ((OracleDecimal)command.Parameters["result"].Value).ToInt32();

                    if (res == 0)
                    {
                        result = ((OracleDecimal)command.Parameters["p_integrator_commis"].Value).Value;
                    }
                    else
                    {
                        throw new Exception(command.Parameters["p_err_msg"].Value.ToString());
                    }
                }
            }

            return result;
        }

        #endregion Получение исходных данных о платежах, инкассациях и счетах

        #region Чтение настроек для формирования проводок

        public AccountingTransactionTemplateDetailCollection GetAccountingTransactionShemes()
        {
            AccountingTransactionTemplateDetailCollection result = new AccountingTransactionTemplateDetailCollection();

            using (OracleConnection connection = new OracleConnection(ConfigurationProvider.ConnectionString))
            {
                using (OracleCommand command = new OracleCommand(ConfigurationProvider.SqlSelectAccountingTransactionShemes, connection))
                {
                    connection.Open();
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AccountingTransactionTemplateDetail detail = new AccountingTransactionTemplateDetail();
                            detail.Id = Convert.ToInt32(reader["AccTranSchemeId"]);
                            //detail.Order = Convert.ToInt32(reader["AccTranOrder"]);
                            detail.AccTranSchemeNumber = reader["AccTranSchemeNumber"].ToString();
                            detail.AccTranNumber = reader["AccTranNumber"].ToString();
                            detail.IsGroupDocs = Convert.ToInt32(reader["IsGroupDocs"]) == 1;
                            detail.PlacePayment = (PlacePayment)Convert.ToInt32(reader["PlacePaymentId"]);
                            detail.DebetAccountSearchTemplate = Convert.ToInt32(reader["DebetAccSearchTemplateId"]);
                            detail.DebetBankReqType = (BankRequisites)Convert.ToInt32(reader["DebetBankReqTypeId"]);
                            detail.CreditAccountSearchTemplate = Convert.ToInt32(reader["CreditAccSearchTemplateId"]);
                            detail.CreditBankReqType = (BankRequisites)Convert.ToInt32(reader["CreditBankReqTypeId"]);
                            detail.RecipientReqType = (RecipientRequisites)Convert.ToInt32(reader["RecipientReqTypeId"]);
                            detail.AmountType = (AmountType)Convert.ToInt64(reader["AmountTypeId"]);
                            detail.DocumentType = (DocumentType)Convert.ToInt64(reader["DocumentTypeId"]);
                            detail.Symbol = reader["Symbol"] is DBNull ? null : (int?)Convert.ToInt32(reader["Symbol"]);
                            detail.EventType = (EventType)Convert.ToInt32(reader["EventTypeId"]);
                            detail.Description = reader["Description"].ToString();
                            result.Add(detail);
                        }
                    }
                }
            }

            return result;
        }

        public AccountSearchRuleCollection GetAccountSearchRules()
        {
            AccountSearchRuleCollection result = new AccountSearchRuleCollection();

            using (OracleConnection connection = new OracleConnection(ConfigurationProvider.ConnectionString))
            {
                using (OracleCommand command = new OracleCommand(ConfigurationProvider.SqlSelectAccountSearchRules, connection))
                {
                    connection.Open();
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AccountSearchRule rule = new AccountSearchRule();
                            rule.Id = Convert.ToInt32(reader["Id"]);
                            //rule.AccountingTransactionTemplateId = Convert.ToInt32(reader["AccTranTemplateId"]);
                            //rule.Position = (AccountPosition)Convert.ToInt32(reader["Position"]);
                            rule.AccountSearchTemplate = Convert.ToInt32(reader["AccSearchTemplateId"]);
                            rule.AccountBindingTemplate = Convert.ToInt32(reader["AccountBindingTemplateId"]);
                            rule.AccountBindingTemplateName = reader["AccountBindingTemplate"].ToString();
                            rule.EntityType1 = (EntityType)Convert.ToInt64(reader["EntityTypeId1"]);
                            rule.EntityType2 = (EntityType)Convert.ToInt64(reader["EntityTypeId2"]);
                            rule.Priority = Convert.ToInt32(reader["Priority"]);
                            result.Add(rule);
                        }
                    }
                }
            }

            return result;
        }

        #endregion Чтение настроек для формирования проводок

        #region Операции над сформированными проводками

        public AccountingTransactionCollection GetAccountingTransactions(AccountingTransactionEvent accTranEvent)
        {
            try
            {
                AccountingTransactionCollection result = new AccountingTransactionCollection();

                using (OracleConnection connection = new OracleConnection(ConfigurationProvider.ConnectionString))
                {
                    using (OracleCommand selAccTranCommand = new OracleCommand(ConfigurationProvider.SqlSelectAccountingTransactions, connection))
                    {
                        selAccTranCommand.Parameters.Add(new OracleParameter("eventid", OracleDbType.Int32, accTranEvent.Id, ParameterDirection.Input));

                        using (OracleCommand selAccTranFieldsCommand = new OracleCommand(ConfigurationProvider.SqlSelectAccountingTransactionFields, connection))
                        {
                            selAccTranFieldsCommand.Parameters.Add(new OracleParameter("accountingtransactionid", OracleDbType.Int32, ParameterDirection.Input));
                            connection.Open();

                            using (OracleDataReader selAccTranReader = selAccTranCommand.ExecuteReader())
                            {
                                while (selAccTranReader.Read())
                                {
                                    AccountingTransaction accTransaction = new AccountingTransaction();
                                    accTransaction.Id = Convert.ToInt64(selAccTranReader["id"]);
                                    accTransaction.DocumentType = (DocumentType)Convert.ToInt64(selAccTranReader["documenttypeid"]);
                                    accTransaction.DebetAccount = selAccTranReader["debetaccount"].ToString();
                                    accTransaction.CreditAccount = selAccTranReader["creditaccount"].ToString();
                                    accTransaction.Amount = Convert.ToDecimal(selAccTranReader["amount"]);

                                    selAccTranFieldsCommand.Parameters["accountingtransactionid"].Value = accTransaction.Id;

                                    using (OracleDataReader selAccTranFieldsReader = selAccTranFieldsCommand.ExecuteReader())
                                    {
                                        while (selAccTranFieldsReader.Read())
                                        {
                                            accTransaction.DocumentFields.Add(selAccTranFieldsReader["name"].ToString(), selAccTranFieldsReader["value"].ToString());
                                        }
                                    }

                                    result.Add(accTransaction);
                                }
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new AccountingTransactionsReadException("Ошибка загрузки проводок из таблицы AccountingTransactions.", ex);
            }
        }

        public void InsertAccountingTransactions(AccountingTransactionEvent accTranEvent, AccountingTransactionCollection accountingTransactions)
        {
            using (OracleConnection connection = new OracleConnection(ConfigurationProvider.ConnectionString))
            {
                OracleTransaction transaction = null;

                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    using (OracleCommand insAccTranCommand = new OracleCommand(ConfigurationProvider.SqlInsertAccountingTransaction, connection))
                    {
                        insAccTranCommand.Parameters.Add(new OracleParameter("EventId", OracleDbType.Int32, ParameterDirection.Input));
                        insAccTranCommand.Parameters.Add(new OracleParameter("DebetEntityTypeId1", OracleDbType.Int32, ParameterDirection.Input));
                        insAccTranCommand.Parameters.Add(new OracleParameter("DebetEntityId1", OracleDbType.Int32, ParameterDirection.Input));
                        insAccTranCommand.Parameters.Add(new OracleParameter("DebetEntityTypeId2", OracleDbType.Int32, ParameterDirection.Input));
                        insAccTranCommand.Parameters.Add(new OracleParameter("DebetEntityId2", OracleDbType.Int32, ParameterDirection.Input));
                        insAccTranCommand.Parameters.Add(new OracleParameter("CreditEntityTypeId1", OracleDbType.Int32, ParameterDirection.Input));
                        insAccTranCommand.Parameters.Add(new OracleParameter("CreditEntityId1", OracleDbType.Int32, ParameterDirection.Input));
                        insAccTranCommand.Parameters.Add(new OracleParameter("CreditEntityTypeId2", OracleDbType.Int32, ParameterDirection.Input));
                        insAccTranCommand.Parameters.Add(new OracleParameter("CreditEntityId2", OracleDbType.Int32, ParameterDirection.Input));
                        insAccTranCommand.Parameters.Add(new OracleParameter("DocumentTypeId", OracleDbType.Int32, ParameterDirection.Input));
                        insAccTranCommand.Parameters.Add(new OracleParameter("AmountTypeId", OracleDbType.Int32, ParameterDirection.Input));                        
                        insAccTranCommand.Parameters.Add(new OracleParameter("DebetAccount", OracleDbType.Varchar2, 20, DBNull.Value, ParameterDirection.Input));
                        insAccTranCommand.Parameters.Add(new OracleParameter("CreditAccount", OracleDbType.Varchar2, 20, DBNull.Value, ParameterDirection.Input));
                        insAccTranCommand.Parameters.Add(new OracleParameter("Amount", OracleDbType.Decimal, ParameterDirection.Input));
                        insAccTranCommand.Parameters.Add(new OracleParameter("DisplayNumber", OracleDbType.Varchar2, 15, DBNull.Value, ParameterDirection.Input));
                        insAccTranCommand.Parameters.Add(new OracleParameter("Id", OracleDbType.Int32, ParameterDirection.Output));

                        using (OracleCommand insAccTransPaymentCommand = new OracleCommand(ConfigurationProvider.SqlInsertAccountingTransactionPayment, connection))
                        {
                            insAccTransPaymentCommand.Parameters.Add(new OracleParameter("AccountingTransactionId", OracleDbType.Int32, ParameterDirection.Input));
                            insAccTransPaymentCommand.Parameters.Add(new OracleParameter("PaymentId", OracleDbType.Int32, ParameterDirection.Input));
                            insAccTransPaymentCommand.Parameters.Add(new OracleParameter("Amount", OracleDbType.Decimal, ParameterDirection.Input));

                            using (OracleCommand insAccTranFieldsCommand = new OracleCommand(ConfigurationProvider.SqlInsertAccountingTransactionField, connection))
                            {
                                insAccTranFieldsCommand.Parameters.Add(new OracleParameter("AccountingTransactionId", OracleDbType.Int32, ParameterDirection.Input));
                                insAccTranFieldsCommand.Parameters.Add(new OracleParameter("Name", OracleDbType.Varchar2, 30, null, ParameterDirection.Input));
                                insAccTranFieldsCommand.Parameters.Add(new OracleParameter("Value", OracleDbType.Varchar2, 210, null, ParameterDirection.Input));

                                foreach (AccountingTransaction accountingTransaction in accountingTransactions)
                                {
                                    insAccTranCommand.Parameters["EventId"].Value = accTranEvent.Id;
                                    insAccTranCommand.Parameters["DebetEntityTypeId1"].Value = accountingTransaction.DebetEntityType1;
                                    insAccTranCommand.Parameters["DebetEntityId1"].Value = accountingTransaction.DebetEntityId1;
                                    insAccTranCommand.Parameters["DebetEntityTypeId2"].Value = accountingTransaction.DebetEntityType2;
                                    insAccTranCommand.Parameters["DebetEntityId2"].Value = accountingTransaction.DebetEntityId2;
                                    insAccTranCommand.Parameters["CreditEntityTypeId1"].Value = accountingTransaction.CreditEntityType1;
                                    insAccTranCommand.Parameters["CreditEntityId1"].Value = accountingTransaction.CreditEntityId1;
                                    insAccTranCommand.Parameters["CreditEntityTypeId2"].Value = accountingTransaction.CreditEntityType2;
                                    insAccTranCommand.Parameters["CreditEntityId2"].Value = accountingTransaction.CreditEntityId2;
                                    insAccTranCommand.Parameters["DocumentTypeId"].Value = accountingTransaction.DocumentType;
                                    insAccTranCommand.Parameters["AmountTypeId"].Value = accountingTransaction.AmountType;
                                    insAccTranCommand.Parameters["DebetAccount"].Value = accountingTransaction.DebetAccount;
                                    insAccTranCommand.Parameters["CreditAccount"].Value = accountingTransaction.CreditAccount;
                                    insAccTranCommand.Parameters["Amount"].Value = accountingTransaction.Amount;
                                    insAccTranCommand.Parameters["DisplayNumber"].Value = accountingTransaction.DisplayNumber;
                                    insAccTranCommand.ExecuteNonQuery();

                                    accountingTransaction.Id = long.Parse(insAccTranCommand.Parameters["Id"].Value.ToString());

                                    foreach (string key in accountingTransaction.DocumentFields.Keys)
                                    {
                                        insAccTranFieldsCommand.Parameters["AccountingTransactionId"].Value = accountingTransaction.Id;
                                        insAccTranFieldsCommand.Parameters["Name"].Value = key;
                                        insAccTranFieldsCommand.Parameters["Value"].Value = accountingTransaction.DocumentFields[key];
                                        insAccTranFieldsCommand.ExecuteNonQuery();
                                    }

                                    foreach (AccountingTransactionPayment payment in accountingTransaction)
                                    {
                                        if (payment.Id != NotDefineValue)
                                        {
                                            insAccTransPaymentCommand.Parameters["AccountingTransactionId"].Value = accountingTransaction.Id;
                                            insAccTransPaymentCommand.Parameters["PaymentId"].Value = payment.Id;
                                            insAccTransPaymentCommand.Parameters["Amount"].Value = payment.Amount;
                                            insAccTransPaymentCommand.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Обновление статуса события
                    accTranEvent.CreateDate = DateTime.Now;
                    accTranEvent.SendDate = null;
                    accTranEvent.Status = EventStatus.DataCreated;                    

                    
                    using (OracleCommand command = new OracleCommand(ConfigurationProvider.SqlUpdateAccountingTransactionEvent, connection))
                    {
                        command.Parameters.Add(new OracleParameter("createdate", OracleDbType.Date, accTranEvent.CreateDate, ParameterDirection.Input));
                        command.Parameters.Add(new OracleParameter("senddate", OracleDbType.Date, accTranEvent.SendDate, ParameterDirection.Input));
                        command.Parameters.Add(new OracleParameter("status", OracleDbType.Int32, (int)accTranEvent.Status, ParameterDirection.Input));
                        command.Parameters.Add(new OracleParameter("id", OracleDbType.Int64, accTranEvent.Id, ParameterDirection.Input));
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    try
                    {
                        if (transaction != null) transaction.Rollback();
                    }
                    catch
                    {
                    }

                    throw new AccountingTransactionsSaveToFsException("Проводки не сохранены в таблицу AccountingTransactions.", ex);
                }
                finally
                {
                    try
                    {
                        if (transaction != null) transaction.Dispose();
                    }
                    catch
                    {
                    }
                }
            }
        }

        #endregion Операции над сформированными проводками

        private object getEntity(string requisites, Type type)
        {
            object result;
            XmlSerializer xmlSerializer = new XmlSerializer(type);

            using (StringReader sr = new StringReader(requisites))
            {
                result = xmlSerializer.Deserialize(sr);
            }

            return result;
        }
    }
}
