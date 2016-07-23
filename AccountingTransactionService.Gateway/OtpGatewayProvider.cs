using System;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using AccountingTransactionService.Configuration;
using AccountingTransactionService.CustomExceptions;
using AccountingTransactionService.DbEntities;
using AccountingTransactionService.Interfaces;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using Utilities.Logging;

namespace AccountingTransactionService.Gateway
{
    public enum OperationType
    {
        Payment = 1,
        Transfer = 2
    }

    public enum TransferType
    {
        Cash_in = 1,
        Encashment = 2
    }

    public enum InputSource
    {
        Terminal = 1
    }

    public enum Hold
    {
        Y = 1,
        N = 2
    }

    public enum TariffType
    {
        Standard = 1
    }

    [XmlRoot("Create_Operation")]
    public class OtpOperation
    {
        [XmlElement("Operation_id")]
        public string OperationId
        {
            set;
            get;
        }

        [XmlElement("Operation_type")]
        public OperationType OperationType
        {
            set;
            get;
        }

        [XmlElement("Transfer_type")]
        public string TransferType
        {
            set;
            get;
        }

        [XmlElement("Operation_date")]
        public string OperationDate
        {
            set;
            get;
        }

        [XmlElement("AccountIdentifier_d")]
        public string DebetAccount
        {
            set;
            get;
        }

        [XmlElement("AccountIdentifier_c")]
        public string CreditAccount
        {
            set;
            get;
        }

        [XmlElement("Amount")]
        public string Amount
        {
            set;
            get;
        }

        [XmlElement("Commission_amt")]
        public string Commission
        {
            set;
            get;
        }

        [XmlElement("Fee_amt")]
        public string Fee
        {
            set;
            get;
        }

        [XmlElement("Tax_amt")]
        public string Tax
        {
            set;
            get;
        }

        [XmlElement("Narrative")]
        public string Narrative
        {
            set;
            get;
        }

        [XmlElement("OP_ID")]
        public string OperatorId
        {
            set;
            get;
        }

        [XmlElement("FS_ID")]
        public string SessionNumber
        {
            set;
            get;
        }

        [XmlElement("Is_Hold")]
        public Hold IsHold
        {
            set;
            get;
        }

        [XmlElement("FS_INTEGRATOR")]
        public string GatewayId
        {
            set;
            get;
        }

        [XmlElement("Input_Source")]
        public InputSource InputSource
        {
            set;
            get;
        }

        [XmlElement("Tariff_type")]
        public string TariffType
        {
            set;
            get;
        }

        [XmlElement("Commission_acct_id")]
        public string CommissionAccount
        {
            set;
            get;
        }
    }

    [XmlRoot("Answer")]
    public class OutXmlMessage
    {
        [XmlElement("CG_Operation_id")]
        public string OperationId
        {
            set;
            get;
        }

        [XmlElement("Commition_amt")]
        public string Commission
        {
            set;
            get;
        }

        [XmlElement("Value_date")]
        public string Date
        {
            set;
            get;
        }
    }

    class OtpGatewayProvider : IGatewayProvider
    {
        private string getInXmlMessage(OtpOperation otpOperation)
        {
            string result;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(OtpOperation));

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Encoding = Encoding.UTF8;
            xmlWriterSettings.Indent = true;
            xmlWriterSettings.OmitXmlDeclaration = true;

            XmlSerializerNamespaces emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            using (MemoryStream stream = new MemoryStream())
            {
                XmlWriter xw = XmlWriter.Create(stream, xmlWriterSettings);
                xmlSerializer.Serialize(xw, otpOperation, emptyNamepsaces);
                result = Encoding.UTF8.GetString(stream.ToArray()).Remove(0, 1);
            }

            return result;
        }

        private OutXmlMessage getOutXmlMessage(string outXmlMessage)
        {
            OutXmlMessage result;            

            using(StringReader sr = new StringReader(outXmlMessage))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(OutXmlMessage));            
                result = xmlSerializer.Deserialize(sr) as OutXmlMessage;
            }
            
            return result;
        }

        public void SendAccountingTransactions(AccountingTransactionCollection accTransactions)
        {
            try
            {
                if (accTransactions.Count > 0)
                {
                    using (OracleConnection connection = new OracleConnection(ConfigurationProvider.OtpGatewayConfiguration.ConnectionString))
                    {
                        using (OracleCommand command = new OracleCommand(ConfigurationProvider.OtpGatewayConfiguration.SqlCreateOperation, connection))
                        {
                            command.Parameters.Add(new OracleParameter("ptransaction_id", OracleDbType.Int64, ParameterDirection.Input));
                            command.Parameters.Add(new OracleParameter("ptypetransaction", OracleDbType.Varchar2, 30, ParameterDirection.Input));
                            command.Parameters.Add(new OracleParameter("pinp_xml_msg", OracleDbType.Clob, ParameterDirection.Input));
                            command.Parameters.Add(new OracleParameter("pout_xml_msg", OracleDbType.Clob, ParameterDirection.Output));
                            command.Parameters.Add(new OracleParameter("perr_code", OracleDbType.Varchar2, 3, DBNull.Value, ParameterDirection.Output));
                            command.Parameters.Add(new OracleParameter("perr_msg", OracleDbType.Varchar2, 4000, DBNull.Value, ParameterDirection.Output));

                            connection.Open();

                            foreach (AccountingTransaction accTran in accTransactions)
                            {
                                OtpOperation otpOperation = new OtpOperation();

                                otpOperation.OperationId = accTran.DocumentFields["F204"];
                                otpOperation.OperationDate = accTran.DocumentFields["F004"];
                                otpOperation.DebetAccount = accTran.DocumentFields["F009"];
                                otpOperation.Amount = accTran.DocumentFields["F007"];
                                otpOperation.Commission = accTran.DocumentFields["F205"];                                
                                otpOperation.Narrative = accTran.DocumentFields["F024"];
                                otpOperation.IsHold = Hold.N;
                                otpOperation.InputSource = InputSource.Terminal;
                                otpOperation.CommissionAccount = accTran.DocumentFields["F209"];
                                    

                                if (accTran.DocumentFields["F208"] == OperationType.Payment.ToString())
                                {
                                    otpOperation.OperationType = OperationType.Payment;
                                    otpOperation.Fee = accTran.DocumentFields["F206"];
                                    otpOperation.Tax = accTran.DocumentFields["F207"];
                                    otpOperation.GatewayId = accTran.DocumentFields["F202"];
                                    otpOperation.OperatorId = accTran.DocumentFields["F203"];
                                    otpOperation.SessionNumber = accTran.DocumentFields["F204"];
                                }
                                else
                                {
                                    otpOperation.OperationType = OperationType.Transfer;
                                    otpOperation.TransferType = TransferType.Cash_in.ToString();
                                    otpOperation.CreditAccount = accTran.DocumentFields["F017"];
                                    otpOperation.TariffType = TariffType.Standard.ToString();
                                }

                                command.Parameters["ptransaction_id"].Value = long.Parse(accTran.DocumentFields["F204"]); //long.Parse(accTran.DocumentFields["F201"]);
                                command.Parameters["ptypetransaction"].Value = otpOperation.OperationType.ToString();
                                command.Parameters["pinp_xml_msg"].Value = getInXmlMessage(otpOperation);
                                
                                Log.WriteToFile(string.Format("Входящий XML пакет:{0}{1}", Environment.NewLine, command.Parameters["pinp_xml_msg"].Value));

                                command.ExecuteNonQuery();                                
                                
                                OutXmlMessage outMessage;
                                
                                if ((command.Parameters["pout_xml_msg"].Value as OracleClob).IsNull)
                                {
                                    Log.WriteToFile(string.Format("Возвращённые процедурой параметры (код ошибки={0}, текст сообщения о ошибке={1})", command.Parameters["perr_code"].Value, command.Parameters["perr_msg"].Value));
                                    throw new InvalidOperationException("Параметр pout_xml_msg = NULL.");
                                }
                                else
                                {
                                    string outXmlMsg = ((OracleClob)command.Parameters["pout_xml_msg"].Value).Value;
                                    Log.WriteToFile(string.Format("Исходящий XML пакет:{0}{1}", Environment.NewLine, outXmlMsg));
                                    outMessage = getOutXmlMessage(outXmlMsg);
                                }

                                if (!string.IsNullOrEmpty(outMessage.OperationId))
                                {
                                    Log.WriteToFile("Запись операции в шлюз выполнена успешно.");
                                }
                                else
                                {
                                    throw new InvalidOperationException("В выходном сообщении отсутствует или не задан идентификатор операции CG_Operation_id.");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new AccountingTransactionsSaveToAbsException("Ошибка передачи проводки в шлюз АБС.", ex);
            }
        }

        public void SendPaymentStatistics(PaymentStatisticsCollection payStatistics)
        {
            throw new NotImplementedException();
        }
    }
}
