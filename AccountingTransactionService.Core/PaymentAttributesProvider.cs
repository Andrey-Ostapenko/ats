using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using AccountingTransactionService.Configuration;
using AccountingTransactionService.CustomExceptions;
using AccountingTransactionService.DbEntities;
using AccountingTransactionService.Entities;
using AccountingTransactionService.Interfaces;
using AccountingTransactionService.Service_References.RequisitesResolverService;
using AccountingTransactionService.XmlEntities;
using Utilities.Logging;

namespace AccountingTransactionService
{
    [XmlType("field")]
    public class DocumentField
    {
        public DocumentField()
        {
        }

        public DocumentField(string name, string value)
        {
            Name = name;
            Value = value;
        }

        [XmlAttribute("name")]
        public string Name
        {
            set;
            get;
        }

        [XmlAttribute("value")]
        public string Value
        {
            set;
            get;
        }
    }

    [XmlRoot("document")]
    public class DocumentFields : List<DocumentField>
    {
    }

    public class PaymentAttributesProvider
    {
        private IDbProvider dbProvider;
        private int defaultPointId;
        private int defaultData;
        private int operatorComposite; 
        private XmlWriterSettings xmlWriterSettings;
        private XmlSerializer xmlSerializer;

        public PaymentAttributesProvider(IDbProvider dbProvider)
        {
            this.dbProvider = dbProvider;
            
            defaultPointId = ConfigurationProvider.DefaultPointId;
            defaultData = ConfigurationProvider.DefaultData;
            operatorComposite = ConfigurationProvider.OperatorComposite;

            xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Encoding = Encoding.UTF8;
            xmlWriterSettings.Indent = true;

            xmlSerializer = new XmlSerializer(typeof(DocumentFields));
        }

        /*
          0 - положительный ответ
         -1 - платёж не найден (возвращается пустая строка)
         -2 - реквизиты банка не заданы (возвращается пустая строка)
         -3 - реквизиты получателя не заданы (возвращается пустая строка)
         -2147768 - внутренняя ошибка (возвращается пустая строка)
        */
        public int GetPaymentAttributes(int pointId, string sessionNumber, out string attributes)
        {
            int result = 0;
            attributes = string.Empty;

            try
            {
                Payment payment = dbProvider.GetPayment(pointId, sessionNumber);

                if (payment.PointId != pointId)
                {
                    throw new PymentNotExistException(string.Format("Платёж с параметрами pointId={0}, sessionNumber={1} не найден в базе данных.", pointId, sessionNumber));
                }

                if (payment.Subdealer == null) // ошибка реквизитов банка
                {
                    throw new SubdealerAttributesException(string.Format("Не заданы реквизиты для филиала - Идентификатор={0}, Название={1}.", payment.SubdealerId, payment.SubdealerName));
                }

                if (payment.OperatorRecipient == null && payment.GatewayRecipient == null) // ошибка реквизитов банка получателя платежа
                {
                    throw new SubdealerAttributesException(string.Format("Не заданы реквизиты получателя платежа для шлюза - Идентификатор={0}, Название={1}, или для провайдера Идентификатор={2}, Название={3}.", payment.GatewayId, payment.GatewayName, payment.OperatorId, payment.OperatorName));
                }

                Bank agentBank = payment.Subdealer;
                Organization recipient = payment.OperatorRecipient != null ? payment.OperatorRecipient : payment.GatewayRecipient;
                Bank recipientBank = payment.OperatorBank != null ? payment.OperatorBank : payment.GatewayBank;

                DocumentFields fields = new DocumentFields();

                fields.Add(new DocumentField("f4", DateTime.Now.ToString("dd.MM.yyyy")));
                fields.Add(new DocumentField("f7", payment.Amount.ToString()));
                fields.Add(new DocumentField("f13", recipientBank.Name));
                fields.Add(new DocumentField("f14", recipientBank.BIK));
                fields.Add(new DocumentField("f15", recipientBank.CorrAccount));
                fields.Add(new DocumentField("f16", recipient.Name));
                fields.Add(new DocumentField("f17", recipient.Account));
                fields.Add(new DocumentField("f24", payment.PaymentTarget));
                fields.Add(new DocumentField("f61", recipient.INN));
                fields.Add(new DocumentField("f103", recipient.KPP));

                using (MemoryStream stream = new MemoryStream())
                {
                    XmlWriter xw = XmlWriter.Create(stream, xmlWriterSettings);
                    xmlSerializer.Serialize(xw, fields);
                    attributes = Encoding.UTF8.GetString(stream.ToArray()).Remove(0, 1);
                }

                /*
                try
                {
                    xw.WriteStartDocument();
                    xw.WriteStartElement("document");

                    foreach (DocumentField field in fields)
                    {
                        xw.WriteStartElement("field");
                        xw.WriteAttributeString("name", field.Name);
                        xw.WriteAttributeString("value", field.Value);
                        xw.WriteEndElement();
                    }

                    xw.WriteEndElement();
                    xw.WriteEndDocument();
                    xw.Flush();
                }
                finally
                {
                    xw.Close();
                }
                */

                return result;
            }
            catch (PymentNotExistException ex)
            {
                result = -1;

                try
                {
                    Log.WriteToFile(ex);
                }
                catch
                {
                }
            }
            catch (SubdealerAttributesException ex)
            {
                result = -2;

                try
                {
                    Log.WriteToFile(ex);
                }
                catch
                {
                }
            }
            catch (RecipientAttributesException ex)
            {
                result = -3;

                try
                {
                    Log.WriteToFile(ex);
                }
                catch
                {
                }
            }
            catch (Exception ex)
            {
                result = int.MinValue;

                try
                {
                    Log.WriteToFile(ex);
                }
                catch
                {
                }
            }

            return result;
        }

        public int GetRequisities(long providerId, string parameters, int pointId, out string attributes)
        {
            int result = 0;
            attributes = string.Empty;
            
            try
            {
                //Log.WriteToFile(string.Format("Поступил запрос от точки id={0} на получение реквизитов по провайдеру id={1} c данными={2}", providerId, pointId, parameters));

                Payment payment = dbProvider.GetRequsites(providerId, pointId);
                payment.Parameters = parameters;

                if (payment.PointId != pointId)
                {
                    throw new PymentNotExistException(string.Format("Данные о реквизитах не найдены в базе данных для запроса с параметрами providerId={0}, pointId={1}.", providerId, pointId));                    
                }

                if (payment.Subdealer == null) // ошибка реквизитов банка
                {
                    throw new SubdealerAttributesException(string.Format("Не заданы реквизиты для филиала - Идентификатор={0}, Название={1}.", payment.SubdealerId, payment.SubdealerName));
                }

                if (payment.OperatorRecipient == null && payment.GatewayRecipient == null) // ошибка реквизитов банка получателя платежа
                {
                    throw new SubdealerAttributesException(string.Format("Не заданы реквизиты получателя платежа для шлюза - Идентификатор={0}, Название={1}, или для провайдера Идентификатор={2}, Название={3}.", payment.GatewayId, payment.GatewayName, payment.OperatorId, payment.OperatorName));
                }


                Bank agentBank = payment.Subdealer;

                if (!agentBank.IsValid)
                {
                    throw new SubdealerAttributesException(string.Format("Не заданы реквизиты для филиала - Идентификатор={0}, Название={1}.", payment.SubdealerId, payment.SubdealerName));
                }

                Organization recipient;
                Bank recipientBank;

                if (payment.OperatorComposite != operatorComposite)
                {
                    recipient = payment.OperatorRecipient != null ? payment.OperatorRecipient : payment.GatewayRecipient;
                    recipientBank = payment.OperatorBank != null ? payment.OperatorBank : payment.GatewayBank;
                }
                else //payment.OperatorComposite == operatorComposite
                {
                    try
                    {
                        OperatorRequisite requisites;

                        using (OperatorRequisitesResolverServiceClient client = new OperatorRequisitesResolverServiceClient())
                        {
                            requisites = client.GetOperatorRequisite(providerId, attributes);
                        }

                        if (requisites != null)
                        {
                            recipient = new Organization();
                            recipient.Name = requisites.Organization;
                            recipient.Account = requisites.Account;
                            recipient.INN = requisites.INN;
                            recipient.KPP = requisites.KPP;

                            recipientBank = new Bank();
                            recipientBank.Name = requisites.BankName;
                            recipientBank.BIK = requisites.BankBIK;
                            recipientBank.CorrAccount = requisites.BankCorAccount != null ? requisites.BankCorAccount : string.Empty;
                        }
                        else
                        {
                            throw new RecipientAttributesException(string.Format("Реквизиты провайдера {0} не найдены во внешнем справочнике.", payment.OperatorName));
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteToFile(ex);
                        throw new RecipientAttributesException(string.Format("Ошибка получения реквизитов провайдера {0} из внешнего справочника.", payment.OperatorName));
                    }
                }

                if (!recipientBank.IsValid)
                {
                    throw new RecipientAttributesException(string.Format("Реквизиты получателя {0} не заданы.", payment.OperatorName));
                }

                if (!recipient.IsValid)
                {
                    throw new RecipientAttributesException(string.Format("Реквизиты банка получателя {0} не заданы.", payment.OperatorName));
                }

                PaymentAccountingTransactionHelper helper = new PaymentAccountingTransactionHelper(payment);
                
                //AccountingTransactionPayment payment = new AccountingTransactionPayment(
                
                DocumentFields fields = new DocumentFields();

                fields.Add(new DocumentField("f4", DateTime.Now.ToString("dd.MM.yyyy")));
                fields.Add(new DocumentField("f13", recipientBank.Name));
                fields.Add(new DocumentField("f14", recipientBank.BIK));
                fields.Add(new DocumentField("f15", recipientBank.CorrAccount));
                fields.Add(new DocumentField("f16", recipient.Name));
                fields.Add(new DocumentField("f17", recipient.Account));
                fields.Add(new DocumentField("f24", helper.SubstituteVariableValues(payment.PaymentTarget)));
                fields.Add(new DocumentField("f61", recipient.INN));
                fields.Add(new DocumentField("f103", recipient.KPP));

                using (MemoryStream stream = new MemoryStream())
                {
                    XmlWriter xw = XmlWriter.Create(stream, xmlWriterSettings);
                    xmlSerializer.Serialize(xw, fields);
                    attributes = Encoding.UTF8.GetString(stream.ToArray()).Remove(0, 1);
                }

                //Log.WriteToFile(string.Format("Реквизиты по запросу от точки id={0} на получение реквизитов по провайдеру id={1} c данными={2}: \n {3}", providerId, pointId, parameters, attributes));
                return result;
            }
            catch (PymentNotExistException ex)
            {
                result = -1;

                try
                {
                    Log.WriteToFile(ex);
                }
                catch
                {
                }
            }
            catch (SubdealerAttributesException ex)
            {
                result = -2;

                try
                {
                    Log.WriteToFile(ex);
                }
                catch
                {
                }
            }
            catch (RecipientAttributesException ex)
            {
                result = -3;

                try
                {
                    Log.WriteToFile(ex);
                }
                catch
                {
                }
            }
            catch (Exception ex)
            {
                result = int.MinValue;

                try
                {
                    Log.WriteToFile(ex);
                }
                catch
                {
                }
            }

            return result;
        }
    }
}
