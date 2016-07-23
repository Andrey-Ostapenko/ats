using System;
using System.Collections.Generic;
using System.Linq;
using AccountingTransactionService.Configuration;
using AccountingTransactionService.CustomExceptions;
using AccountingTransactionService.DbEntities;
using AccountingTransactionService.Entities;
using AccountingTransactionService.Enums;
using AccountingTransactionService.Interfaces;
using AccountingTransactionService.Service_References.RequisitesResolverService;
using AccountingTransactionService.XmlEntities;
using Utilities.Logging;

namespace AccountingTransactionService
{        
    public class AccountingTransactionProvider
    {
        private int operatorComposite;
        private IDbProvider dbProvider;

        public AccountingTransactionProvider(IDbProvider dbProvider /*, bool logging*/)
        {
            this.dbProvider = dbProvider;
            operatorComposite = ConfigurationProvider.OperatorComposite;
        }

        public AccountingTransactionCollection CreateAccountingTransactions(AccountingTransactionEvent accTranEvent)
        {
            AccountingTransactionDetailCollection accountingTransactionDetails = null;
            AccountingTransactionCollection result = null;

            try
            {
                switch (accTranEvent.EventType)
                {
                    case EventType.DayClose:
                    case EventType.PointClose:
                        accountingTransactionDetails = getPaymentsAccountingTransactionDetails(accTranEvent);
                        break;
                    case EventType.PaymentCompletion:
                        accountingTransactionDetails = getOtpPaymentAccountingTransactionDetails(accTranEvent);
                        break;
                    case EventType.PointCollection:
                        accountingTransactionDetails = getCollectionAccountingTransactionDetails(accTranEvent);
                        break;
                }

                writePaymentAccountingTransactionDetailsToLog(accountingTransactionDetails);

                result = getAccountingTransactions(accTranEvent, accountingTransactionDetails);
            }
            catch (PaymentException ex)
            {
                Log.WriteToFile(ex.Message);

                if (ex.AdditionalData != null)
                {
                    foreach (string dataItem in ex.AdditionalData)
                    {
                        Log.WriteToFile(dataItem);
                    }
                }

                throw new AccountingTransactionsException(string.Format("Ошибка обработки платежа {0}.", ex.PaymentId));
            }
            catch (TerminalCollectionException ex)
            {
                Log.WriteToFile(ex.Message);

                if (ex.AdditionalData != null)
                {
                    foreach (string dataItem in ex.AdditionalData)
                    {
                        Log.WriteToFile(dataItem);
                    }
                }

                throw new AccountingTransactionsException(string.Format("Ошибка обработки инкассации терминала {0}.", ex.CollectionId));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            return result;
        }

        private AccountingTransactionDetailCollection getCollectionAccountingTransactionDetails(AccountingTransactionEvent accTranEvent)
        {
            AccountingTransactionDetailCollection result = new AccountingTransactionDetailCollection();
            TerminalCollection terminalCollection = dbProvider.GetTerminalCollection(accTranEvent);
            AccountingTransactionTemplateDetailCollection accTranSheme = dbProvider.GetAccountingTransactionShemes();
            AccountProvider accProvider = new AccountProvider(dbProvider.GetAccountSearchRules(), dbProvider.GetAccountBindings(), new ReplacementAccountCollection());

            IEnumerable<AccountingTransactionTemplateDetail> details = accTranSheme.GetPaymentAccountingTransactionDetails(terminalCollection.AccountiongTransactionSheme, accTranEvent.EventType);

            int n = 0;

            Log.WriteToFile(string.Format("Формирование проводок по инкассации {0} терминала {1}:", accTranEvent.Id, terminalCollection.Id));

            foreach (AccountingTransactionTemplateDetail template in details)
            {
                Account debetAccount = null, creditAccount = null;

                // если не нашли счет по дебету
                if (!accProvider.GetAccount(accTranEvent, terminalCollection, template, AccountPosition.Debet, true, out debetAccount))
                {
                    throw new TerminalCollectionException(terminalCollection.Id, "Не найден счёт по дебету. Поиск выполнялся используя следующие параметры:", accProvider.GetLog());
                }

                // если не нашли счет по кредиту
                if (!accProvider.GetAccount(accTranEvent, terminalCollection, template, AccountPosition.Credit, true, out creditAccount))
                {
                    throw new TerminalCollectionException(terminalCollection.Id, "Не найден счёт по кредиту. Поиск выполнялся используя следующие параметры:", accProvider.GetLog());
                }

                if (debetAccount != null && creditAccount != null)
                {
                    if (debetAccount.Value == creditAccount.Value) continue;

                    //Bank debetBank = terminalCollection.GetBank(debetAccount.EntityType1, debetAccount.EntityType2);
                    Bank debetBank = debetAccount.Bank;

                    if (!debetBank.IsValid)
                    {
                        throw new TerminalCollectionException(terminalCollection.Id, terminalCollection.GetAttributesErrorMessage(debetAccount.EntityType1, debetAccount.EntityType2));
                    }

                    //Bank creditBank = terminalCollection.GetBank(creditAccount.EntityType1, creditAccount.EntityType2);
                    Bank creditBank = creditAccount.Bank;

                    if (!creditBank.IsValid)
                    {
                        throw new TerminalCollectionException(terminalCollection.Id, terminalCollection.GetAttributesErrorMessage(creditAccount.EntityType1, creditAccount.EntityType2));
                    }

                    decimal amount = terminalCollection.Amount;

                    if (amount > 0M)
                    {
                        AccountingTransactionDetail accTranDetail = new AccountingTransactionDetail();

                        accTranDetail.Id = n;
                        accTranDetail.AccTranNumber = template.AccTranNumber;
                        //accTranDetail.AccountingTransactionOrder = template.Order;
                        accTranDetail.PaymentId = -1;
                        accTranDetail.PointExternalCode = terminalCollection.ExternalCode;
                        accTranDetail.DebetBank = debetAccount.Bank;
                        accTranDetail.Payer = new OrganizationPayer();
                        accTranDetail.Recipient = new Organization();
                        accTranDetail.CreditBank = creditAccount.Bank;
                        accTranDetail.DocumentType = template.DocumentType;
                        accTranDetail.AmountType = template.AmountType;
                        accTranDetail.DebetEntityType1 = debetAccount.EntityType1;
                        accTranDetail.DebetEntityId1 = debetAccount.EntityId1;
                        accTranDetail.DebetEntityType2 = debetAccount.EntityType2;
                        accTranDetail.DebetEntityId2 = debetAccount.EntityId2;
                        accTranDetail.CreditEntityType1 = creditAccount.EntityType1;
                        accTranDetail.CreditEntityId1 = creditAccount.EntityId1;
                        accTranDetail.CreditEntityType2 = creditAccount.EntityType2;
                        accTranDetail.CreditEntityId2 = creditAccount.EntityId2;
                        accTranDetail.Symbol = template.Symbol;
                        accTranDetail.DebetAccountName = debetAccount.Name;
                        accTranDetail.DebetAccount = debetAccount.Value;
                        accTranDetail.CreditAccountName = creditAccount.Name;
                        accTranDetail.CreditAccount = creditAccount.Value;
                        accTranDetail.Amount = amount;
                        accTranDetail.AmountAll = 0M;
                        accTranDetail.RewardAmount = 0M;
                        accTranDetail.Description = template.Description;
                        accTranDetail.VbUser = ConfigurationProvider.VbUser;

                        result.Add(accTranDetail);

                        n++;
                    }
                }
            }

            Log.WriteToFile(string.Format("Инкассация {0} терминала {1} обработана успешно.", accTranEvent.Id, terminalCollection.Id));

            return result;
        }

        private AccountingTransactionDetailCollection getOtpPaymentAccountingTransactionDetails(AccountingTransactionEvent accTranEvent)
        {
            AccountingTransactionDetailCollection result = new AccountingTransactionDetailCollection();            
            Payment payment = dbProvider.GetOtpPayment(accTranEvent);
            PaymentAccountingTransactionHelper helper = new PaymentAccountingTransactionHelper(payment);
            AccountProvider accProvider = new AccountProvider(new AccountSearchRuleCollection(), dbProvider.GetAccountBindings(), new ReplacementAccountCollection());            
            //AccountFilter filter = new AccountFilter(ConfigurationProvider.OtpCommissionTemplateId, payment.OperatorId, string.Empty, payment.PointId, string.Empty, ConfigurationProvider.DefaultData);
            AccountFilter filter = new AccountFilter(ConfigurationProvider.OtpCommissionTemplateId, -1, string.Empty, -1, string.Empty, ConfigurationProvider.DefaultData);
            string commissionAccount;

            if (!accProvider.GetAccount(filter, out commissionAccount))
            {
                throw new PaymentException(payment.Id, string.Format("Счёт комиссии не найден в справочнике. (Терминал={0}, Провайдера={1})", payment.PointName, payment.OperatorName));
            }

            Log.WriteToFile("Обработка платежа:");
            Log.WriteToFile(payment.ToString());

            AccountingTransactionDetail accTranDetail = new AccountingTransactionDetail();

            if (string.IsNullOrEmpty(payment.PointAccount)) throw new PaymentException(payment.Id, string.Format("Не задан счёт по дебету. Введите счёт каccы для точки {0}:", payment.PointId));
            
            accTranDetail.Id = 1;
            accTranDetail.OtpOperationType = ConfigurationProvider.OtpCabinetGatewayId == payment.GatewayId ? OtpOperationType.Transfer : OtpOperationType.Payment;
            accTranDetail.PaymentId = payment.Id;
            accTranDetail.InitialSessionNumber = payment.SessionNumber;
            accTranDetail.GatewayId = payment.GatewayId;
            accTranDetail.OperatorId = payment.OperatorId;
            accTranDetail.DebetBank = new Bank();
            accTranDetail.Payer = new OrganizationPayer();
            accTranDetail.CreditBank = new Bank();
            accTranDetail.DocumentType = DocumentType.OtpPaymentOrder;
            accTranDetail.AmountType = AmountType.PaymentWithContractCommission;
            accTranDetail.DebetEntityType1 = EntityType.None;
            accTranDetail.DebetEntityType2 = EntityType.None;
            accTranDetail.CreditEntityType1 = EntityType.None;
            accTranDetail.CreditEntityType2 = EntityType.None;
            accTranDetail.DebetAccount = payment.PointAccount;
            accTranDetail.CommissionAccount = commissionAccount;

            if (accTranDetail.OtpOperationType == OtpOperationType.Transfer)
            {
                accTranDetail.CreditAccount = accTranDetail.OtpOperationType == OtpOperationType.Transfer ? helper.SubstituteVariableValues(":Поле1") : null;
                if (string.IsNullOrEmpty(accTranDetail.CreditAccount)) throw new PaymentException(payment.Id, string.Format("Не найден счёт по кредиту. В параметрах платежа отсутствует счёт."));
            }

            accTranDetail.Amount = payment.Amount;
            accTranDetail.AmountAll = payment.AmountAll;
            accTranDetail.ClientCommission = payment.ClientCommissionAmount;
            accTranDetail.IntegratorCommission = payment.IntegratorCommission;
            accTranDetail.Description = helper.SubstituteVariableValues(payment.PaymentTarget);

            if (string.IsNullOrEmpty(accTranDetail.Description))
            {
                throw new PaymentException(payment.Id, string.Format("Отсутствует назначение платежа. Введите назначение платежа в параметрах провайдера."));
            }

            accTranDetail.PaymentDate = payment.Date;            

            result.Add(accTranDetail);

            Log.WriteToFile(string.Format("Платёж {0} обработан успешно.", payment.Id));


            return result;
        }        

        private AccountingTransactionDetailCollection getPaymentsAccountingTransactionDetails(AccountingTransactionEvent accTranEvent)
        {
            AccountingTransactionDetailCollection result = new AccountingTransactionDetailCollection();

            PaymentCollection payments = dbProvider.GetPayments(accTranEvent);
            AccountingTransactionTemplateDetailCollection accTranSheme = dbProvider.GetAccountingTransactionShemes();

            AccountProvider accProvider = new AccountProvider(dbProvider.GetAccountSearchRules(), dbProvider.GetAccountBindings(), dbProvider.GetReplacementAccounts());

            int n = 1;

            Log.WriteToFile("Обработка платежей:");

            if (payments.Count == 0)
            {
                Log.WriteToFile("Платежи для обработки не найдены.");
            }

            for (int i = 0; i < payments.Count; i++)
            {
                Payment payment = payments[i];
                PaymentAccountingTransactionHelper helper = new PaymentAccountingTransactionHelper(payment);

                //Log.WriteToFile(payment.ToString());

                string accTranSchema = helper.GetAccountigTransactionSchema();

                if (string.IsNullOrEmpty(accTranSchema))
                {
                    throw new PaymentException(payment.Id, string.Format("Не определена схема проводок для платежа {0}.", payment.Id));
                }

                IEnumerable<AccountingTransactionTemplateDetail> paymentDetails = accTranSheme.GetPaymentAccountingTransactionDetails(accTranSchema, accTranEvent.EventType);

                //Log.WriteToFile(string.Format("Cхема проводок № {0}, (количество шаблонов проводок для обработки {1})", accTranSchema, paymentDetails.Count<AccountingTransactionTemplateDetail>()));
                
                foreach (AccountingTransactionTemplateDetail template in paymentDetails)
                {                                        
                    Account debetAccount = null, creditAccount = null;

                    // обработка межфилиальных проводок
                    if (template.PlacePayment != PlacePayment.All)
                    {
                        if (payment.Subdealer.Main == 1 && template.PlacePayment != PlacePayment.Dealer) continue;
                        if (payment.Subdealer.Main != 1 && template.PlacePayment != PlacePayment.Subdealer) continue;
                    }
                    
                    bool compareData = accTranEvent.EventType == EventType.DayClose ? true : false;


                    // если не нашли счет по дебету
                    if (!accProvider.GetAccount(accTranEvent, helper, template, AccountPosition.Debet, compareData, out debetAccount))
                    {
                        throw new PaymentException(payment.Id, "Не найден счёт по дебету. Поиск выполнялся используя следующие параметры:", accProvider.GetLog());
                    }
                    

                    // если не нашли счет по кредиту
                    if (!accProvider.GetAccount(accTranEvent, helper, template, AccountPosition.Credit, compareData, out creditAccount))
                    {
                        throw new PaymentException(payment.Id, "Не найден счёт по кредиту. Поиск выполнялся используя следующие параметры:", accProvider.GetLog());
                    }
                    
                    if (debetAccount != null && creditAccount != null)
                    {
                        //if (debetAccount.Bank.BIK == creditAccount.Bank.BIK && debetAccount.Value == creditAccount.Value) continue;

                        Organization recipient;

                        if (template.RecipientReqType == RecipientRequisites.Recipient)
                        {
                            recipient = helper.GetRecipient(template.RecipientReqType);

                            if (template.CreditBankReqType != BankRequisites.Recipient)
                            {
                                recipient.Name = creditAccount.Name;
                                recipient.Account = creditAccount.Value;
                            }                            
                        }
                        else if (template.RecipientReqType == RecipientRequisites.Subdealer || template.RecipientReqType == RecipientRequisites.Dealer)
                        {
                            // подставляем найденный счёт, в качестве счёта получателя в платёжное поручение
                            recipient = helper.GetRecipient(template.RecipientReqType, creditAccount.Value);
                        }
                        else
                        {
                            recipient = null;
                        }

                        if (recipient != null && payment.OperatorComposite == operatorComposite)
                        {
                            try
                            {
                                OperatorRequisite requisites;

                                using (OperatorRequisitesResolverServiceClient client = new OperatorRequisitesResolverServiceClient())
                                {
                                    requisites = client.GetOperatorRequisite(payment.OperatorId, payment.Parameters);
                                }

                                if (requisites != null)
                                {
                                    recipient = new Organization();
                                    recipient.Name = requisites.Organization;
                                    recipient.Account = requisites.Account;
                                    recipient.INN = requisites.INN;
                                    recipient.KPP = requisites.KPP;

                                    if (template.DebetBankReqType == BankRequisites.Recipient)
                                    {
                                        debetAccount.Bank.Name = requisites.BankName;
                                        debetAccount.Bank.BIK = requisites.BankBIK;
                                        debetAccount.Bank.CorrAccount = requisites.BankCorAccount;
                                    }
                                    else if (template.CreditBankReqType == BankRequisites.Recipient)
                                    {
                                        creditAccount.Bank.Name = requisites.BankName;
                                        creditAccount.Bank.BIK = requisites.BankBIK;
                                        creditAccount.Bank.CorrAccount = requisites.BankCorAccount;
                                    }
                                    else if (template.CreditBankReqType != BankRequisites.Recipient)
                                    {
                                        recipient.Name = creditAccount.Name;
                                        recipient.Account = creditAccount.Value;
                                    }                            
                                }
                                else
                                {
                                    throw new PaymentException(payment.Id, string.Format("Реквизиты провайдера {0} не найдены во внешнем справочнике.", payment.OperatorName));
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.WriteToFile(ex);
                                throw new PaymentException(payment.Id, string.Format("Ошибка получения реквизитов провайдера {0} из внешнего справочника.", payment.OperatorName));
                            }
                        }
                        
                        // Проверка реквизитов банка по дебету
                        if (!debetAccount.Bank.IsValid)
                        {
                            throw new PaymentException(payment.Id, string.Format("Не заданы реквизиты банка {0}", helper.GetRequisitesTargetInfo(template.DebetBankReqType)));
                        }

                        // Проверка реквизитов банка по кредиту
                        if (!creditAccount.Bank.IsValid)
                        {
                            throw new PaymentException(payment.Id, string.Format("Не заданы реквизиты банка {0}", helper.GetRequisitesTargetInfo(template.CreditBankReqType)));
                        }

                        // Проверка реквизитов получателя платежа
                        if (recipient != null && !recipient.IsValid)
                        {
                            throw new PaymentException(payment.Id, string.Format("Не заданы реквизиты получателя {0}", helper.GetRequisitesTargetInfo((BankRequisites)template.RecipientReqType)));
                            //Log.WriteToFile(string.Format("Не заданы реквизиты получателя {0}", helper.GetRequisitesTargetInfo((BankRequisites)template.RecipientReqType)));
                        }                        

                        decimal amount = helper.GetAccountingTransactionAmount(template.AmountType);

                        if (amount > 0M)
                        {
                            AccountingTransactionDetail accTranDetail = new AccountingTransactionDetail();

                            accTranDetail.AccTranNumber = template.AccTranNumber;
                            accTranDetail.PaymentId = payment.Id;
                            accTranDetail.PointExternalCode = payment.PointExternalCode;
                            accTranDetail.DebetBank = debetAccount.Bank;
                            accTranDetail.Payer = payment.OperatorPayer == null ? new OrganizationPayer() : payment.OperatorPayer;
                            accTranDetail.Recipient = recipient;
                            accTranDetail.CreditBank = creditAccount.Bank;
                            accTranDetail.DocumentType = template.DocumentType;
                            accTranDetail.AmountType = template.AmountType;
                            accTranDetail.DebetEntityType1 = debetAccount.EntityType1;
                            accTranDetail.DebetEntityId1 = debetAccount.EntityId1;
                            accTranDetail.DebetEntityType2 = debetAccount.EntityType2;
                            accTranDetail.DebetEntityId2 = debetAccount.EntityId2;
                            accTranDetail.CreditEntityType1 = creditAccount.EntityType1;
                            accTranDetail.CreditEntityId1 = creditAccount.EntityId1;
                            accTranDetail.CreditEntityType2 = creditAccount.EntityType2;
                            accTranDetail.CreditEntityId2 = creditAccount.EntityId2;
                            accTranDetail.Symbol = template.Symbol;
                            accTranDetail.DebetAccountName = debetAccount.Name;
                            accTranDetail.DebetAccount = debetAccount.Value;
                            accTranDetail.CreditAccountName = creditAccount.Name;
                            accTranDetail.CreditAccount = creditAccount.Value;
                            accTranDetail.Amount = amount;
                            accTranDetail.AmountAll = amount + helper.GetAccountingTransactionAmount(AmountType.ContractCommission);
                            accTranDetail.ClientCommission = helper.GetAccountingTransactionAmount(AmountType.ClientCommission);
                            accTranDetail.RewardAmount = helper.GetAccountingTransactionAmount(AmountType.ContractCommission);
                            accTranDetail.KBK = helper.GetKBK(template.RecipientReqType);
                            accTranDetail.OKATO = helper.GetOKATO(template.RecipientReqType);
                            accTranDetail.Description = helper.GetTarget(template);
                            accTranDetail.PaymentDate = payment.Date;
                            accTranDetail.VbUser = ConfigurationProvider.VbUser;

                            if (!template.IsGroupDocs)
                            {
                                accTranDetail.Id = n;
                                n++;
                            }

                            result.Add(accTranDetail);
                        }
                    }
                }

                //Log.WriteToFile(string.Format("Платёж {0} обработан успешно.", payments[i].Id));
            }

            writePaymentsToLog(accTranEvent, accTranSheme, payments);

            return result;
        }

        private AccountingTransactionCollection getAccountingTransactions(AccountingTransactionEvent accTranEvent, AccountingTransactionDetailCollection accTranDetails)
        {
            try
            {
                if (accTranDetails.Count > 0)
                {
                    Log.WriteToFile("Формирование проводок...");
                }

                AccountingTransactionCollection result = new AccountingTransactionCollection();                

                var accTransactionDetailsGroup = from accTranDetail in accTranDetails
                                                 group accTranDetail by
                                                 new
                                                 {                                                     
                                                     Id = accTranDetail.Id,
                                                     DocumentType = accTranDetail.DocumentType,
                                                     DebetBIK = accTranDetail.DebetBank.BIK,
                                                     PayerName = accTranDetail.Payer.Name,
                                                     PayerINN = accTranDetail.Payer.INN,
                                                     PayerKPP = accTranDetail.Payer.KPP,
                                                     DebetCorAccount = accTranDetail.DebetBank.CorrAccount,
                                                     DebetAccount = accTranDetail.DebetAccount,
                                                     CreditBIK = accTranDetail.CreditBank.BIK,
                                                     CreditCorAccount = accTranDetail.CreditBank.CorrAccount,
                                                     CreditAccount = accTranDetail.CreditAccount,
                                                     Description = accTranDetail.Description,
                                                     KBK = accTranDetail.KBK,
                                                     OKATO = accTranDetail.OKATO,
                                                     Symbol = accTranDetail.Symbol
                                                 };
                

                foreach (var accTransactionDetails in accTransactionDetailsGroup)
                {
                    AccountingTransaction accountingTransaction = new AccountingTransaction();
                    AccountingTransactionFields accTranFields = new AccountingTransactionFields();
                    
                    bool flag = true;

                    foreach (AccountingTransactionDetail accTransactionDetail in accTransactionDetails)
                    {
                        if (flag)
                        {
                            // Для платёжного поручения сохраняем номер проводки в схеме, чтобы можно было найти платёжное поручение
                            if (accTransactionDetail.DocumentType == DocumentType.PaymentOrder)
                            {
                                accountingTransaction.DisplayNumber = accTransactionDetail.AccTranNumber;
                            }

                            accountingTransaction.DebetAccount = accTransactionDetail.DebetAccount;
                            accountingTransaction.CreditAccount = accTransactionDetail.CreditAccount;
                            accountingTransaction.DocumentType = accTransactionDetail.DocumentType;
                            accountingTransaction.AmountType = accTransactionDetail.AmountType;
                            accountingTransaction.DebetEntityType1 = accTransactionDetail.DebetEntityType1;
                            accountingTransaction.DebetEntityId1 = accTransactionDetail.DebetEntityId1;
                            accountingTransaction.DebetEntityType2 = accTransactionDetail.DebetEntityType2;
                            accountingTransaction.DebetEntityId2 = accTransactionDetail.DebetEntityId2;                            
                            accountingTransaction.CreditEntityType1 = accTransactionDetail.CreditEntityType1;
                            accountingTransaction.CreditEntityId1 = accTransactionDetail.CreditEntityId1;
                            accountingTransaction.CreditEntityType2 = accTransactionDetail.CreditEntityType2;
                            accountingTransaction.CreditEntityId2 = accTransactionDetail.CreditEntityId2;

                            accTranFields.PaymentId = accTransactionDetail.PaymentId;
                            accTranFields.SessionNumber = accTransactionDetail.InitialSessionNumber;
                            accTranFields.GatewayId = accTransactionDetail.GatewayId;
                            accTranFields.OperatorId = accTransactionDetail.OperatorId;
                            accTranFields.PointExternalCode = accTransactionDetail.PointExternalCode;
                            accTranFields.OperatorAccount = accTranEvent.OperatorAccount;
                            accTranFields.Payer = accTransactionDetail.Payer;
                            accTranFields.DebetAccountName = accTransactionDetail.DebetAccountName;
                            accTranFields.DebetBank = accTransactionDetail.DebetBank;                            
                            accTranFields.Recipient = accTransactionDetail.Recipient;
                            accTranFields.DebetAccountName = accTransactionDetail.CreditAccountName;
                            accTranFields.CreditBank = accTransactionDetail.CreditBank;
                            accTranFields.Description = accTransactionDetail.Description;
                            accTranFields.OtpOperationType = accTransactionDetail.OtpOperationType;
                            accTranFields.CommissionAccount = accTransactionDetail.CommissionAccount;
                            accTranFields.VbUser = accTransactionDetail.VbUser;
                            accTranFields.KBK = accTransactionDetail.KBK;
                            accTranFields.OKATO = accTransactionDetail.OKATO;

                            flag = false;
                        }                        
                        
                        accountingTransaction.DescriptionParameters.PaymentDateFrom = accTransactionDetail.PaymentDate;
                        accountingTransaction.DescriptionParameters.PaymentDateTo = accTransactionDetail.PaymentDate;
                        accountingTransaction.DescriptionParameters.AmountAll += accTransactionDetail.AmountAll;
                        accountingTransaction.DescriptionParameters.RewardAmount += accTransactionDetail.RewardAmount;

                        accountingTransaction.Amount += accTransactionDetail.Amount;
                        accTranFields.ClientCommission += accTransactionDetail.ClientCommission;
                        accTranFields.IntegratorCommission += accTransactionDetail.IntegratorCommission;

                        accountingTransaction.Add(new AccountingTransactionPayment(accTransactionDetail.PaymentId, accTransactionDetail.Amount));
                    }

                    accountingTransaction.CreateDocumentFields(accTranFields);

                    result.Add(accountingTransaction);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private void writePaymentsToLog(AccountingTransactionEvent accTranEvent, AccountingTransactionTemplateDetailCollection accTranSheme, PaymentCollection payments)
        {            
            foreach (var payment in payments)
            {
                PaymentAccountingTransactionHelper helper = new PaymentAccountingTransactionHelper(payment);
                string accTranSchema = helper.GetAccountigTransactionSchema();
                IEnumerable<AccountingTransactionTemplateDetail> paymentDetails = accTranSheme.GetPaymentAccountingTransactionDetails(accTranSchema, accTranEvent.EventType);                
                Log.WriteToFile(payment.ToString());
                Log.WriteToFile(string.Format("Cхема проводок № {0}, (количество шаблонов проводок для обработки {1})", accTranSchema, paymentDetails.Count<AccountingTransactionTemplateDetail>()));                
                Log.WriteToFile(string.Format("Платёж {0} обработан успешно.", payment.Id));
            }
        }        

        private void writePaymentAccountingTransactionDetailsToLog(AccountingTransactionDetailCollection accountingTransactions)
        {
            var paymentAccountTransactionGroups = from accountingTransaction in accountingTransactions
                                                  orderby accountingTransaction.AccountingTransactionOrder
                                                  group accountingTransaction by
                                                  new
                                                  {
                                                      PaymentId = accountingTransaction.PaymentId,
                                                  };                        
            bool header = false;

            foreach (var paymentAccountTransactionGroup in paymentAccountTransactionGroups)
            {
                if (!header)
                {
                    Log.WriteToFile("Детализация проводок:");
                    header = true;
                }

                if (paymentAccountTransactionGroup.Key.PaymentId != -1)
                {
                    Log.WriteToFile(string.Format("Платёж {0}", paymentAccountTransactionGroup.Key.PaymentId));
                }
                
                foreach (AccountingTransactionDetail accTransaction in paymentAccountTransactionGroup)
                {
                    Log.WriteToFile(string.Format("Дт {0} Кт {1} Сумма {2} (Шаблон {3})", accTransaction.DebetAccount, accTransaction.CreditAccount, accTransaction.Amount, accTransaction.AccTranNumber));
                }
            }

            Log.WriteToFile("-".PadRight(80, '-'));
        }        
    }    
}
