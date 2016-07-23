using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Serialization;
using AccountingTransactionService.Configuration;
using AccountingTransactionService.CustomExceptions;
using AccountingTransactionService.DbEntities;
using AccountingTransactionService.Enums;
using AccountingTransactionService.Interfaces;
using AccountingTransactionService.XmlEntities;
using Utilities.Logging;

namespace AccountingTransactionService.Db
{
    public class SqlServerDbProvider : IDbProvider
    {
        private const int NotDefineValue = -1;

        #region Операции над событиями для формирования проводок

        public EventPeriod GetLastAccountingTransactionEventPeriod()
        {
            try
            {
                EventPeriod result = null;

                using (SqlConnection connection = new SqlConnection(ConfigurationProvider.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(ConfigurationProvider.SqlSelectLastAccountingTransactionEventPeriod, connection))
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
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

        public AccountingTransactionEventCollection GetAccountingTransactionEvents()
        {
            try
            {
                AccountingTransactionEventCollection result = new AccountingTransactionEventCollection();

                using (SqlConnection connection = new SqlConnection(ConfigurationProvider.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(ConfigurationProvider.SqlSelectAccountingTransactionEvents, connection))
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
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
                                accTranEvent.Data = Convert.ToInt32(reader["data"]); ;
                                accTranEvent.Status = (EventStatus)Convert.ToInt32(reader["status"]);
                                accTranEvent.Amount = !(reader["amount"] is DBNull) ? Convert.ToDecimal(reader["amount"]) : 0M;
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

        public void InsertAccountingTransactionEvent(AccountingTransactionEvent accTranEvent)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationProvider.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(ConfigurationProvider.SqlInsertAccountingTransactionEvent, connection))
                    {
                        command.Parameters.Add(new SqlParameter("eventtypeid", SqlDbType.Int));
                        command.Parameters["eventtypeid"].Direction = ParameterDirection.Input;
                        command.Parameters["eventtypeid"].Value = (int)accTranEvent.EventType;
                        command.Parameters.Add(new SqlParameter("datefrom", SqlDbType.DateTime2));
                        command.Parameters["datefrom"].Direction = ParameterDirection.Input;
                        command.Parameters["datefrom"].Value = accTranEvent.DateFrom;
                        command.Parameters.Add(new SqlParameter("dateto", SqlDbType.DateTime2));
                        command.Parameters["dateto"].Direction = ParameterDirection.Input;
                        command.Parameters["dateto"].Value = accTranEvent.DateTo;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new AccountingTransactionEventSaveException("Ошибка записи в базу данных события для формирования проводок.", ex);
            }
        }

        public void UpdateAccountingTransactionEvent(AccountingTransactionEvent accTranEvent)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationProvider.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(ConfigurationProvider.SqlUpdateAccountingTransactionEvent, connection))
                    {
                        command.Parameters.Add(new SqlParameter("createdate", SqlDbType.DateTime2));
                        command.Parameters["createdate"].Direction = ParameterDirection.Input;
                        command.Parameters["createdate"].IsNullable = true;

                        if (accTranEvent.CreateDate != null)
                        {
                            command.Parameters["createdate"].Value = accTranEvent.CreateDate;
                        }
                        else
                        {
                            command.Parameters["createdate"].Value = DBNull.Value;
                        }
                        command.Parameters.Add(new SqlParameter("senddate", SqlDbType.DateTime2));
                        command.Parameters["senddate"].Direction = ParameterDirection.Input;
                        command.Parameters["senddate"].IsNullable = true;

                        if (accTranEvent.SendDate != null)
                        {
                            command.Parameters["senddate"].Value = accTranEvent.SendDate;
                        }
                        else
                        {
                            command.Parameters["senddate"].Value = DBNull.Value;
                        }

                        command.Parameters.Add(new SqlParameter("status", SqlDbType.Int));
                        command.Parameters["status"].Direction = ParameterDirection.Input;
                        command.Parameters["status"].Value = (int)accTranEvent.Status;
                        command.Parameters.Add(new SqlParameter("id", SqlDbType.BigInt));
                        command.Parameters["id"].Direction = ParameterDirection.Input;
                        command.Parameters["id"].Value = accTranEvent.Id;
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

                using (SqlConnection connection = new SqlConnection(ConfigurationProvider.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(ConfigurationProvider.SqlSelectLastPaymentStatisticsEventPeriod, connection))
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
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
                using (SqlConnection connection = new SqlConnection(ConfigurationProvider.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(ConfigurationProvider.SqlInsertPaymentStatisticsEvent, connection))
                    {
                        command.Parameters.Add(new SqlParameter("datefrom", SqlDbType.DateTime2));
                        command.Parameters["datefrom"].Direction = ParameterDirection.Input;
                        command.Parameters["datefrom"].Value = payStatEvent.DateFrom;
                        command.Parameters.Add(new SqlParameter("dateto", SqlDbType.DateTime2));
                        command.Parameters["dateto"].Direction = ParameterDirection.Input;
                        command.Parameters["dateto"].Value = payStatEvent.DateTo;
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

                using (SqlConnection connection = new SqlConnection(ConfigurationProvider.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(ConfigurationProvider.SqlSelectPaymentStatisticsEvents, connection))
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
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
                using (SqlConnection connection = new SqlConnection(ConfigurationProvider.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(ConfigurationProvider.SqlUpdatePaymentStatisticsEvent, connection))
                    {
                        command.Parameters.Add(new SqlParameter("senddate", SqlDbType.DateTime2));
                        command.Parameters["senddate"].Direction = ParameterDirection.Input;
                        command.Parameters["senddate"].Value = payStatEvent.SendDate;
                        command.Parameters.Add(new SqlParameter("status", SqlDbType.Int));
                        command.Parameters["status"].Direction = ParameterDirection.Input;
                        command.Parameters["status"].Value = (int)payStatEvent.Status;
                        command.Parameters.Add(new SqlParameter("id", SqlDbType.BigInt));
                        command.Parameters["id"].Direction = ParameterDirection.Input;
                        command.Parameters["id"].Value = payStatEvent.Id;
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

            using (SqlConnection connection = new SqlConnection(ConfigurationProvider.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(ConfigurationProvider.SqlSelectPayment, connection))
                {
                    command.Parameters.Add(new SqlParameter("terminalid", SqlDbType.Int));
                    command.Parameters["terminalid"].Direction = ParameterDirection.Input;
                    command.Parameters["terminalid"].Value = pointId;

                    command.Parameters.Add(new SqlParameter("initialsessionnumber", SqlDbType.VarChar, 20));
                    command.Parameters["initialsessionnumber"].Direction = ParameterDirection.Input;
                    command.Parameters["initialsessionnumber"].Value = sessionNumber;

                    command.Parameters.Add(new SqlParameter("terminalid", SqlDbType.Int));
                    command.Parameters["terminalid"].Direction = ParameterDirection.Input;
                    command.Parameters["terminalid"].Value = pointId;

                    command.Parameters.Add(new SqlParameter("initialsessionnumber", SqlDbType.VarChar, 20));
                    command.Parameters["initialsessionnumber"].Direction = ParameterDirection.Input;
                    command.Parameters["initialsessionnumber"].Value = sessionNumber;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
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
                                result.Dealer = attributes != null ? (Bank)getEntity(attributes, typeof(Bank)) : new Bank();
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
            throw new NotImplementedException("Mетод не реализован.");
        }

        #endregion

        #region Получение исходных данных о платежах, инкассациях и счетах        

        public PaymentCollection GetPayments(AccountingTransactionEvent accTranEvent)
        {
            Dictionary<long, Bank> dialers = new Dictionary<long, Bank>();
            Dictionary<long, Bank> subdialers = new Dictionary<long, Bank>();
            Dictionary<long, Bank> gatewayBanks = new Dictionary<long, Bank>();
            Dictionary<long, Organization> gatewayRecipients = new Dictionary<long, Organization>();
            Dictionary<long, Bank> operatorBanks = new Dictionary<long, Bank>();
            Dictionary<long, Organization> operatorRecipients = new Dictionary<long, Organization>();

            PaymentCollection result = new PaymentCollection();

            using (SqlConnection connection = new SqlConnection(ConfigurationProvider.ConnectionString))
            {
                string commandText;

                if (accTranEvent.EventType == EventType.DayClose)
                {
                    commandText = ConfigurationProvider.SqlSelectPayments;
                }
                else
                {
                    commandText = ConfigurationProvider.SqlSelectPointPayments;
                }

                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.Add(new SqlParameter("terminalid", SqlDbType.Int));
                    command.Parameters["terminalid"].Direction = ParameterDirection.Input;
                    command.Parameters["terminalid"].Value = accTranEvent.PointId;

                    command.Parameters.Add(new SqlParameter("datefrom", SqlDbType.DateTime2));
                    command.Parameters["datefrom"].Direction = ParameterDirection.Input;
                    command.Parameters["datefrom"].Value = accTranEvent.DateFrom;

                    command.Parameters.Add(new SqlParameter("dateto", SqlDbType.DateTime2));
                    command.Parameters["dateto"].Direction = ParameterDirection.Input;
                    command.Parameters["dateto"].Value = accTranEvent.DateTo;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string attributes;
                            Payment payment = new Payment();

                            payment.Id = Convert.ToInt64(reader["PaymentId"]);

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

                            payment.OperatorId = Convert.ToInt64(reader["OperatorId"]);
                            payment.OperatorName = reader["OperatorName"].ToString();
                            payment.OperatorComposite = Convert.ToInt32(reader["OperatorComposite"]);
                            payment.PaymentTarget = reader["PaymentTarget"].ToString();

                            attributes = reader["OperatorAttributes"] is DBNull ? null : reader["OperatorAttributes"].ToString();

                            Log.WriteToFile(string.Format("оператор {0} реквизиты {1}", payment.OperatorId, attributes));

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
                                    Log.WriteToFile(string.Format("Ошибка формата реквизитов платежа {0} ({1}).", payment.OperatorName, payment.OperatorId));
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
                                    Log.WriteToFile(string.Format("Ошибка формата реквизитов платежа {0} ({1}).", payment.OperatorName, payment.OperatorId));
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
                                Log.WriteToFile(string.Format("Ошибка формата реквизитов платежа {0} ({1}).", payment.OperatorName, payment.OperatorId));
                            }

                            payment.Parameters = reader["Params"].ToString();
                            payment.Amount = Convert.ToDecimal(reader["Amount"]);
                            payment.AmountWithoutCommission = Convert.ToDecimal(reader["AmountWithoutCommission"]);
                            payment.ClientCommissionAmount = Convert.ToDecimal(reader["ClientComissionAmount"]);
                            payment.ContractCommissionAmount = Convert.ToDecimal(reader["ContractCommissionAmount"]);
                            payment.AmountAll = Convert.ToDecimal(reader["AmountAll"]);
                            payment.Date = Convert.ToDateTime(reader["PaymentDate"]);

                            result.Add(payment);
                        }
                    }
                }
            }

            return result;
        }

        public Payment GetOtpPayment(AccountingTransactionEvent accTranEvent)
        {
            throw new NotImplementedException("Данный метод не реализован в версии по для MS SqlServer");
        }

        public TerminalCollection GetTerminalCollection(AccountingTransactionEvent accTranEvent)
        {
            TerminalCollection result = new TerminalCollection();

            using (SqlConnection connection = new SqlConnection(ConfigurationProvider.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(ConfigurationProvider.SqlSelectTerminalCollection, connection))
                {
                    command.Parameters.Add(new SqlParameter("terminalid", SqlDbType.Int));
                    command.Parameters["terminalid"].Direction = ParameterDirection.Input;
                    command.Parameters["terminalid"].Value = accTranEvent.PointId;
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
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

            using (SqlConnection connection = new SqlConnection(ConfigurationProvider.ConnectionString))
            {
                using (SqlCommand selPayStatCommand = new SqlCommand(ConfigurationProvider.SqlSelectPaymentStatistics, connection))
                {
                    selPayStatCommand.Parameters.Add(new SqlParameter("datefrom", SqlDbType.DateTime2));
                    selPayStatCommand.Parameters["datefrom"].Direction = ParameterDirection.Input;
                    selPayStatCommand.Parameters["datefrom"].Value = payStatEvent.DateFrom;
                    selPayStatCommand.Parameters.Add(new SqlParameter("dateto", SqlDbType.DateTime2));
                    selPayStatCommand.Parameters["dateto"].Direction = ParameterDirection.Input;
                    selPayStatCommand.Parameters["dateto"].Value = payStatEvent.DateTo;
                    selPayStatCommand.Parameters.Add(new SqlParameter("datefrom", SqlDbType.DateTime2));
                    selPayStatCommand.Parameters["datefrom"].Direction = ParameterDirection.Input;
                    selPayStatCommand.Parameters["datefrom"].Value = payStatEvent.DateFrom;
                    selPayStatCommand.Parameters.Add(new SqlParameter("dateto", SqlDbType.DateTime2));
                    selPayStatCommand.Parameters["dateto"].Direction = ParameterDirection.Input;
                    selPayStatCommand.Parameters["dateto"].Value = payStatEvent.DateTo;

                    connection.Open();

                    using (SqlDataReader selPayStatReader = selPayStatCommand.ExecuteReader())
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

            using (SqlConnection connection = new SqlConnection(ConfigurationProvider.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(ConfigurationProvider.SqlSelectAccountBindings, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AccountBinding binding = new AccountBinding();
                            binding.AccountBindingTemplatateId = Convert.ToInt32(reader["AccountBindingTemplateId"]);
                            binding.EntityId1 = Convert.ToInt64(reader["EntityId1"]);
                            binding.EntityId2 = Convert.ToInt64(reader["EntityId2"]);
                            binding.Data = Convert.ToInt32(reader["Data"]);
                            binding.AccountName = reader["AccountName"].ToString();
                            binding.Account = reader["Account"].ToString();
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

            using (SqlConnection connection = new SqlConnection(ConfigurationProvider.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(ConfigurationProvider.SqlSelectReplacementAccounts, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
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

        public decimal GetIntegratorCommission(long gatewayId, long providerId, DateTime paymentDate, decimal amount, decimal clientCommission)
        {
            return 0M;
        }

        #endregion Получение исходных данных о платежах, инкассациях и счетах

        #region Чтение настроек для формирования проводок

        public AccountingTransactionTemplateDetailCollection GetAccountingTransactionShemes()
        {
            AccountingTransactionTemplateDetailCollection result = new AccountingTransactionTemplateDetailCollection();

            using (SqlConnection connection = new SqlConnection(ConfigurationProvider.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(ConfigurationProvider.SqlSelectAccountingTransactionShemes, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
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

            using (SqlConnection connection = new SqlConnection(ConfigurationProvider.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(ConfigurationProvider.SqlSelectAccountSearchRules, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AccountSearchRule rule = new AccountSearchRule();
                            rule.Id = Convert.ToInt32(reader["Id"]);
                            //rule.AccountingTransactionTemplateId = Convert.ToInt32(reader["AccTranTemplateId"]);
                            rule.AccountSearchTemplate = Convert.ToInt32(reader["AccSearchTemplateId"]);
                            //rule.Position = (AccountPosition)Convert.ToInt32(reader["Position"]);
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

                using (SqlConnection connection1 = new SqlConnection(ConfigurationProvider.ConnectionString), connection2 = new SqlConnection(ConfigurationProvider.ConnectionString))
                {
                    using (SqlCommand selAccTranCommand = new SqlCommand(ConfigurationProvider.SqlSelectAccountingTransactions, connection1))
                    {
                        selAccTranCommand.Parameters.Add(new SqlParameter("eventid", SqlDbType.Int));
                        selAccTranCommand.Parameters["eventid"].Direction = ParameterDirection.Input;
                        selAccTranCommand.Parameters["eventid"].Value = accTranEvent.Id;

                        connection1.Open();
                        connection2.Open();

                        using (SqlDataReader selAccTranReader = selAccTranCommand.ExecuteReader())
                        {
                            while (selAccTranReader.Read())
                            {
                                AccountingTransaction accTransaction = new AccountingTransaction();
                                accTransaction.Id = Convert.ToInt64(selAccTranReader["id"]);
                                accTransaction.DocumentType = (DocumentType)Convert.ToInt64(selAccTranReader["documenttypeid"]);
                                accTransaction.DebetAccount = selAccTranReader["debetaccount"].ToString();
                                accTransaction.CreditAccount = selAccTranReader["creditaccount"].ToString();
                                accTransaction.Amount = Convert.ToDecimal(selAccTranReader["amount"]);

                                using (SqlCommand selAccTranFieldsCommand = new SqlCommand(ConfigurationProvider.SqlSelectAccountingTransactionFields, connection2))
                                {
                                    selAccTranFieldsCommand.Parameters.Add(new SqlParameter("accountingtransactionid", SqlDbType.Int));
                                    selAccTranFieldsCommand.Parameters["accountingtransactionid"].Direction = ParameterDirection.Input;
                                    selAccTranFieldsCommand.Parameters["accountingtransactionid"].Value = accTransaction.Id;

                                    using (SqlDataReader selAccTranFieldsReader = selAccTranFieldsCommand.ExecuteReader())
                                    {
                                        while (selAccTranFieldsReader.Read())
                                        {
                                            accTransaction.DocumentFields.Add(selAccTranFieldsReader["name"].ToString(), selAccTranFieldsReader["value"].ToString());
                                        }
                                    }
                                }

                                result.Add(accTransaction);
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
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationProvider.ConnectionString))
                {
                    SqlTransaction transaction = null;

                    try
                    {
                        connection.Open();
                        transaction = connection.BeginTransaction();
                        using (SqlCommand insAccTranCommand = new SqlCommand(ConfigurationProvider.SqlInsertAccountingTransaction, connection, transaction))
                        {
                            insAccTranCommand.Parameters.Add(new SqlParameter("EventId", SqlDbType.Int));
                            insAccTranCommand.Parameters["EventId"].Direction = ParameterDirection.Input;
                            insAccTranCommand.Parameters.Add(new SqlParameter("DebetEntityTypeId1", SqlDbType.Int));
                            insAccTranCommand.Parameters["DebetEntityTypeId1"].Direction = ParameterDirection.Input;
                            insAccTranCommand.Parameters.Add(new SqlParameter("DebetEntityId1", SqlDbType.Int));
                            insAccTranCommand.Parameters["DebetEntityId1"].Direction = ParameterDirection.Input;
                            insAccTranCommand.Parameters.Add(new SqlParameter("DebetEntityTypeId2", SqlDbType.Int));
                            insAccTranCommand.Parameters["DebetEntityTypeId2"].Direction = ParameterDirection.Input;
                            insAccTranCommand.Parameters.Add(new SqlParameter("DebetEntityId2", SqlDbType.Int));
                            insAccTranCommand.Parameters["DebetEntityId2"].Direction = ParameterDirection.Input;
                            insAccTranCommand.Parameters.Add(new SqlParameter("CreditEntityTypeId1", SqlDbType.Int));
                            insAccTranCommand.Parameters["CreditEntityTypeId1"].Direction = ParameterDirection.Input;
                            insAccTranCommand.Parameters.Add(new SqlParameter("CreditEntityId1", SqlDbType.Int));
                            insAccTranCommand.Parameters["CreditEntityId1"].Direction = ParameterDirection.Input;
                            insAccTranCommand.Parameters.Add(new SqlParameter("CreditEntityTypeId2", SqlDbType.Int));
                            insAccTranCommand.Parameters["CreditEntityTypeId2"].Direction = ParameterDirection.Input;
                            insAccTranCommand.Parameters.Add(new SqlParameter("CreditEntityId2", SqlDbType.Int));
                            insAccTranCommand.Parameters["CreditEntityId2"].Direction = ParameterDirection.Input;
                            insAccTranCommand.Parameters.Add(new SqlParameter("DocumentTypeId", SqlDbType.Int));
                            insAccTranCommand.Parameters["DocumentTypeId"].Direction = ParameterDirection.Input;
                            insAccTranCommand.Parameters.Add(new SqlParameter("AmountTypeId", SqlDbType.Int));
                            insAccTranCommand.Parameters["AmountTypeId"].Direction = ParameterDirection.Input;
                            insAccTranCommand.Parameters.Add(new SqlParameter("DebetAccount", SqlDbType.VarChar, 20));
                            insAccTranCommand.Parameters["DebetAccount"].Direction = ParameterDirection.Input;
                            insAccTranCommand.Parameters["DebetAccount"].Value = DBNull.Value;
                            insAccTranCommand.Parameters.Add(new SqlParameter("CreditAccount", SqlDbType.VarChar, 20));
                            insAccTranCommand.Parameters["CreditAccount"].Direction = ParameterDirection.Input;
                            insAccTranCommand.Parameters["CreditAccount"].Value = DBNull.Value;
                            insAccTranCommand.Parameters.Add(new SqlParameter("Amount", SqlDbType.Decimal));
                            insAccTranCommand.Parameters["Amount"].Direction = ParameterDirection.Input;
                            insAccTranCommand.Parameters.Add(new SqlParameter("DisplayNumber", SqlDbType.VarChar, 15));
                            insAccTranCommand.Parameters["DisplayNumber"].Direction = ParameterDirection.Input;
                            insAccTranCommand.Parameters["DisplayNumber"].Value = DBNull.Value;

                            using (SqlCommand returnIdCommand = new SqlCommand("SELECT IDENT_CURRENT('AccountingTransactions')", connection, transaction))
                            {
                                using (SqlCommand insAccTransPaymentCommand = new SqlCommand(ConfigurationProvider.SqlInsertAccountingTransactionPayment, connection, transaction))
                                {
                                    insAccTransPaymentCommand.Parameters.Add(new SqlParameter("AccountingTransactionId", SqlDbType.Int));
                                    insAccTransPaymentCommand.Parameters["AccountingTransactionId"].Direction = ParameterDirection.Input;
                                    insAccTransPaymentCommand.Parameters.Add(new SqlParameter("PaymentId", SqlDbType.Int));
                                    insAccTransPaymentCommand.Parameters["PaymentId"].Direction = ParameterDirection.Input;
                                    insAccTransPaymentCommand.Parameters.Add(new SqlParameter("Amount", SqlDbType.Decimal));
                                    insAccTransPaymentCommand.Parameters["Amount"].Direction = ParameterDirection.Input;

                                    using (SqlCommand insAccTranFieldsCommand = new SqlCommand(ConfigurationProvider.SqlInsertAccountingTransactionField, connection, transaction))
                                    {
                                        insAccTranFieldsCommand.Parameters.Add(new SqlParameter("AccountingTransactionId", SqlDbType.Int));
                                        insAccTranFieldsCommand.Parameters["AccountingTransactionId"].Direction = ParameterDirection.Input;
                                        insAccTranFieldsCommand.Parameters.Add(new SqlParameter("Name", SqlDbType.VarChar, 30));
                                        insAccTranFieldsCommand.Parameters["Name"].Direction = ParameterDirection.Input;
                                        insAccTranFieldsCommand.Parameters["Name"].Value = DBNull.Value;
                                        insAccTranFieldsCommand.Parameters.Add(new SqlParameter("Value", SqlDbType.VarChar, 210));
                                        insAccTranFieldsCommand.Parameters["Value"].Direction = ParameterDirection.Input;
                                        insAccTranFieldsCommand.Parameters["Value"].Value = DBNull.Value;

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
                                            insAccTranCommand.Parameters["DisplayNumber"].Value = DBNull.Value; 
                                            insAccTranCommand.ExecuteNonQuery();

                                            accountingTransaction.Id = Convert.ToInt64(returnIdCommand.ExecuteScalar());                                            

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
                        }

                        // Обновление статуса события
                        accTranEvent.CreateDate = DateTime.Now;
                        accTranEvent.SendDate = null;
                        accTranEvent.Status = EventStatus.DataCreated;

                        using (SqlCommand command = new SqlCommand(ConfigurationProvider.SqlUpdateAccountingTransactionEvent, connection))
                        {
                            command.Parameters.Add(new SqlParameter("createdate", SqlDbType.DateTime2));
                            command.Parameters["createdate"].Direction = ParameterDirection.Input;
                            command.Parameters["createdate"].IsNullable = true;

                            if (accTranEvent.CreateDate != null)
                            {
                                command.Parameters["createdate"].Value = accTranEvent.CreateDate;
                            }
                            else
                            {
                                command.Parameters["createdate"].Value = DBNull.Value;
                            }
                            command.Parameters.Add(new SqlParameter("senddate", SqlDbType.DateTime2));
                            command.Parameters["senddate"].Direction = ParameterDirection.Input;
                            command.Parameters["senddate"].IsNullable = true;

                            if (accTranEvent.SendDate != null)
                            {
                                command.Parameters["senddate"].Value = accTranEvent.SendDate;
                            }
                            else
                            {
                                command.Parameters["senddate"].Value = DBNull.Value;
                            }

                            command.Parameters.Add(new SqlParameter("status", SqlDbType.Int));
                            command.Parameters["status"].Direction = ParameterDirection.Input;
                            command.Parameters["status"].Value = (int)accTranEvent.Status;
                            command.Parameters.Add(new SqlParameter("id", SqlDbType.BigInt));
                            command.Parameters["id"].Direction = ParameterDirection.Input;
                            command.Parameters["id"].Value = accTranEvent.Id;
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
            catch (Exception ex)
            {
                throw new AccountingTransactionsSaveToFsException("Проводки не сохранены в таблицу AccountingTransactions.", ex);
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
