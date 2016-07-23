using System;
using System.Data;
using AccountingTransactionService.Configuration;
using AccountingTransactionService.CustomExceptions;
using AccountingTransactionService.DbEntities;
using AccountingTransactionService.Enums;
using AccountingTransactionService.Interfaces;
using Oracle.DataAccess.Client;
using Utilities.Logging;

namespace AccountingTransactionService.Gateway
{
    class VbGatewayProvider : IGatewayProvider
    {
        public void SendAccountingTransactions(AccountingTransactionCollection accTransactions)
        {
            try
            {
                if (accTransactions.Count > 0)
                {
                    using (OracleConnection connection = new OracleConnection(ConfigurationProvider.ConnectionString))
                    {
                        connection.Open();

                        using (OracleTransaction transaction = connection.BeginTransaction())
                        {
                            try
                            {

                                using (OracleCommand insAccTranCommand = new OracleCommand(ConfigurationProvider.VbGatewayConfiguration.SqlInsertVlbFlexDoc, connection))
                                {
                                    insAccTranCommand.Parameters.Add(new OracleParameter("id", OracleDbType.Int32, ParameterDirection.Input));
                                    insAccTranCommand.Parameters.Add(new OracleParameter("c_date_in", OracleDbType.Date, ParameterDirection.Input));
                                    insAccTranCommand.Parameters.Add(new OracleParameter("c_type", OracleDbType.Varchar2, 10, DBNull.Value, ParameterDirection.Input));
                                    insAccTranCommand.Parameters.Add(new OracleParameter("c_status", OracleDbType.Byte, ParameterDirection.Input));
                                    insAccTranCommand.Parameters.Add(new OracleParameter("c_date_imp", OracleDbType.Date, ParameterDirection.Input));

                                    using (OracleCommand insAccTranFieldsCommand = new OracleCommand(ConfigurationProvider.VbGatewayConfiguration.SqlInsertVlbFlexDocPack, connection))
                                    {
                                        insAccTranFieldsCommand.Parameters.Add(new OracleParameter("id", OracleDbType.Int32, ParameterDirection.Input));
                                        insAccTranFieldsCommand.Parameters.Add(new OracleParameter("c_val_name", OracleDbType.Varchar2, 30, null, ParameterDirection.Input));
                                        insAccTranFieldsCommand.Parameters.Add(new OracleParameter("c_val_value", OracleDbType.Varchar2, 210, null, ParameterDirection.Input));

                                        foreach (AccountingTransaction accountingTransaction in accTransactions)
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

                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                if (transaction != null) transaction.Rollback();
                                throw new Exception(ex.Message, ex);                                
                            }
                        }

                        try
                        {
                            using (OracleCommand generateDocumentsCommand = new OracleCommand(ConfigurationProvider.VbGatewayConfiguration.SqlGenerateDocuments, connection))
                            {
                                generateDocumentsCommand.ExecuteNonQuery();
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Write("Ошибка вызова обработчика выгруженных данных:");
                            Log.Write(ex);
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
            throw new NotImplementedException();
        }
    }
}
