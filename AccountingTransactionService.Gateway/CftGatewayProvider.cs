using System;
using System.Data;
using AccountingTransactionService.Configuration;
using AccountingTransactionService.CustomExceptions;
using AccountingTransactionService.DbEntities;
using AccountingTransactionService.Enums;
using AccountingTransactionService.Interfaces;
using Oracle.DataAccess.Client;

namespace AccountingTransactionService.Gateway
{
    class CftGatewayProvider : IGatewayProvider
    {
        public void SendAccountingTransactions(AccountingTransactionCollection accTransactions)
        {
            try
            {
                if (accTransactions.Count > 0)
                {
                    using (OracleConnection connection = new OracleConnection(ConfigurationProvider.CftGatewayConfiguration.ConnectionString))
                    {
                        connection.Open();
                        using (OracleTransaction transaction = connection.BeginTransaction())
                        {
                            try
                            {
                                using (OracleCommand selDocCountCommand = new OracleCommand(ConfigurationProvider.CftGatewayConfiguration.SqlSelectVlbFlexDocCount, connection))
                                {
                                    selDocCountCommand.Parameters.Add(new OracleParameter("id", OracleDbType.Int64, ParameterDirection.Input));

                                    using (OracleCommand insAccTranCommand = new OracleCommand(ConfigurationProvider.CftGatewayConfiguration.SqlInsertVlbFlexDoc, connection))
                                    {
                                        insAccTranCommand.Parameters.Add(new OracleParameter("id", OracleDbType.Int64, ParameterDirection.Input));
                                        insAccTranCommand.Parameters.Add(new OracleParameter("c_date_in", OracleDbType.Date, ParameterDirection.Input));
                                        insAccTranCommand.Parameters.Add(new OracleParameter("c_type", OracleDbType.Varchar2, 10, DBNull.Value, ParameterDirection.Input));
                                        insAccTranCommand.Parameters.Add(new OracleParameter("c_status", OracleDbType.Byte, ParameterDirection.Input));
                                        insAccTranCommand.Parameters.Add(new OracleParameter("c_date_imp", OracleDbType.Date, ParameterDirection.Input));

                                        using (OracleCommand insAccTranFieldsCommand = new OracleCommand(ConfigurationProvider.CftGatewayConfiguration.SqlInsertVlbFlexDocPack, connection))
                                        {
                                            insAccTranFieldsCommand.Parameters.Add(new OracleParameter("id", OracleDbType.Int64, ParameterDirection.Input));
                                            insAccTranFieldsCommand.Parameters.Add(new OracleParameter("c_val_name", OracleDbType.Varchar2, 30, null, ParameterDirection.Input));
                                            insAccTranFieldsCommand.Parameters.Add(new OracleParameter("c_val_value", OracleDbType.Varchar2, 210, null, ParameterDirection.Input));

                                            foreach (AccountingTransaction accountingTransaction in accTransactions)
                                            {
                                                selDocCountCommand.Parameters["id"].Value = accountingTransaction.Id;

                                                bool insert = Convert.ToInt32(selDocCountCommand.ExecuteScalar()) == 0;
                                                
                                                if (insert)
                                                {
                                                    insAccTranCommand.Parameters["id"].Value = accountingTransaction.Id;
                                                    insAccTranCommand.Parameters["c_date_in"].Value = DateTime.Now;

                                                    if (accountingTransaction.DocumentType == DocumentType.IncomeOrder)
                                                    {
                                                        insAccTranCommand.Parameters["c_type"].Value = "in-order";
                                                    }
                                                    else if (accountingTransaction.DocumentType == DocumentType.MemorialOrder)
                                                    {
                                                        insAccTranCommand.Parameters["c_type"].Value = "mem-order";
                                                    }
                                                    else if (accountingTransaction.DocumentType == DocumentType.PaymentOrder)
                                                    {
                                                        insAccTranCommand.Parameters["c_type"].Value = "pay-order";
                                                    }
                                                    else if (accountingTransaction.DocumentType == DocumentType.BankOrder)
                                                    {
                                                        insAccTranCommand.Parameters["c_type"].Value = "bank-order";
                                                    }

                                                    insAccTranCommand.Parameters["c_status"].Value = 0;
                                                    insAccTranCommand.Parameters["c_date_imp"].Value = DBNull.Value;
                                                    insAccTranCommand.ExecuteNonQuery();

                                                    foreach (string key in accountingTransaction.DocumentFields.Keys)
                                                    {
                                                        insAccTranFieldsCommand.Parameters["id"].Value = accountingTransaction.Id;
                                                        insAccTranFieldsCommand.Parameters["c_val_name"].Value = key;
                                                        insAccTranFieldsCommand.Parameters["c_val_value"].Value = accountingTransaction.DocumentFields[key];
                                                        insAccTranFieldsCommand.ExecuteNonQuery();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                if (transaction != null) transaction.Rollback();
                                throw new Exception(ex.Message, ex);                                
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new AccountingTransactionsSaveToAbsException("Ошибка сохранения проводок в таблицах АБС.", ex);
            }
        }

        public void SendPaymentStatistics(PaymentStatisticsCollection payStatistics)
        {
            try
            {
                if (payStatistics.Count > 0)
                {
                    using (OracleConnection connection = new OracleConnection(ConfigurationProvider.CftGatewayConfiguration.ConnectionString))
                    {
                        connection.Open();
                        using (OracleTransaction transaction = connection.BeginTransaction())
                        {
                            try
                            {
                                using (OracleCommand insPayStatCommand = new OracleCommand(ConfigurationProvider.CftGatewayConfiguration.SqlInsertVlbPaymentStatistics, connection))
                                {
                                    insPayStatCommand.Parameters.Add(new OracleParameter("c_date", OracleDbType.Date, ParameterDirection.Input));
                                    insPayStatCommand.Parameters.Add(new OracleParameter("c_term_number", OracleDbType.Varchar2, 10, null, ParameterDirection.Input));
                                    insPayStatCommand.Parameters.Add(new OracleParameter("c_pu", OracleDbType.Varchar2, 150, null, ParameterDirection.Input));
                                    insPayStatCommand.Parameters.Add(new OracleParameter("c_quant", OracleDbType.Int32, ParameterDirection.Input));
                                    insPayStatCommand.Parameters.Add(new OracleParameter("c_summa", OracleDbType.Decimal, ParameterDirection.Input));
                                    insPayStatCommand.Parameters.Add(new OracleParameter("c_summa_com", OracleDbType.Decimal, ParameterDirection.Input));

                                    foreach (PaymentStatistics payStat in payStatistics)
                                    {
                                        insPayStatCommand.Parameters["c_date"].Value = payStat.PaymentDate;
                                        insPayStatCommand.Parameters["c_term_number"].Value = payStat.PointId;
                                        insPayStatCommand.Parameters["c_pu"].Value = payStat.PaymentName;
                                        insPayStatCommand.Parameters["c_quant"].Value = payStat.Count;
                                        insPayStatCommand.Parameters["c_summa"].Value = payStat.Amount;
                                        insPayStatCommand.Parameters["c_summa_com"].Value = payStat.CommissionAmount;
                                        insPayStatCommand.ExecuteNonQuery();
                                    }

                                    transaction.Commit();
                                }
                            }
                            catch (Exception ex)
                            {
                                if (transaction != null) transaction.Rollback();
                                throw new Exception(ex.Message, ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new PaymentStatisticsSaveToAbsException("Ошибка сохранения статистики по платежам в базе данных.", ex);
            }
        }
    }
}
