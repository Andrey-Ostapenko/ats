using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using AccountingTransactionService.Configuration;
using AccountingTransactionService.DbEntities;
using AccountingTransactionService.Enums;
using AccountingTransactionService.Interfaces;
using AccountingTransactionService.XmlEntities;

namespace AccountingTransactionService.Entities
{  
    class PaymentAccountingTransactionHelper : IAccountSearch
    {
        private class PaymentVariableMatch
        {
            public PaymentVariableMatch(PaymentVariable key, string variableName)
            {
                Key = key;
                VariableName = variableName;
            }

            public PaymentVariable Key
            {
                private set;
                get;
            }

            public string VariableName
            {
                private set;
                get;
            }
        }

        private const int addValue = 99;
        private const string fieldPrefix = "field";
        private const char parameterSeparator = '&';

        private static int mainBank;
        private static long defaultEntityId;
        private static int defaultEntityData;
        private static Dictionary<PaymentVariable, string> variables;

        private Payment payment;

        static PaymentAccountingTransactionHelper()
        {
            defaultEntityId = ConfigurationProvider.DefaultEntityId;
            defaultEntityData = ConfigurationProvider.DefaultData;
            mainBank = ConfigurationProvider.MainBank;

            variables = new Dictionary<PaymentVariable, string>() { 
               { PaymentVariable.OperatorName, ":Провайдер" },
               { PaymentVariable.PointName, ":Точка" },
               { PaymentVariable.PointExternalCode, ":ВнешнийКодТочки" },
               { PaymentVariable.Field, ":Поле\\d+" } 
            };
        }

        public PaymentAccountingTransactionHelper(Payment payment)
        {
            this.payment = payment;
        }

        private string getVariableValue(PaymentVariable key, string variableName)
        {
            string result = variableName;

            if (variables.ContainsKey(key))
            {
                switch (key)
                {
                    case PaymentVariable.OperatorName:
                        result = payment.OperatorName;
                        break;
                    case PaymentVariable.PointName:
                        result = payment.PointName;
                        break;
                    case PaymentVariable.PointExternalCode:
                        result = payment.PointExternalCode;
                        break;
                    case PaymentVariable.Field:
                        int index;
                        string field;
                        string indexStr = string.Empty;

                        Match match = Regex.Match(variableName, "\\d+");

                        if (match != null)
                        {
                            index = int.Parse(match.Value) + addValue;
                            field = fieldPrefix + index.ToString();

                            index = payment.Parameters.IndexOf(field);

                            if (index >= 0)
                            {
                                index += field.Length + 1;

                                for (int i = index; i < payment.Parameters.Length; i++)
                                {
                                    if (payment.Parameters[i] == parameterSeparator)
                                    {
                                        result = payment.Parameters.Substring(index, i - index);
                                        break;
                                    }
                                    else if (i == payment.Parameters.Length - 1)
                                    {
                                        result = payment.Parameters.Substring(index, payment.Parameters.Length - index);
                                    }
                                }
                            }
                        }

                        break;
                }
            }

            return result;
        }

        private List<PaymentVariableMatch> parseVariables(string exp)
        {
            List<PaymentVariableMatch> result = new List<PaymentVariableMatch>();

            if (!string.IsNullOrEmpty(exp))
            {
                foreach (PaymentVariable key in variables.Keys)
                {
                    MatchCollection mathes = Regex.Matches(exp, variables[key], RegexOptions.IgnoreCase);

                    foreach (Match match in mathes)
                    {
                        result.Add(new PaymentVariableMatch(key, match.Value));
                    }
                }
            }

            return result;
        }

        private void defineEntity(EntityType entityType, BankRequisites target, out long entityId, out string entityName)
        {
            entityId = defaultEntityId;
            entityName = string.Empty;

            switch (entityType)
            {
                case EntityType.Point:
                case EntityType.PoindDay:
                case EntityType.PointEvening:
                case EntityType.PointHoliday:
                    entityId = payment.PointId;
                    entityName = payment.PointName;
                    break;
                case EntityType.Subdialer:
                    if (target == BankRequisites.Dealer)
                    {
                        entityId = payment.DealerId;
                        entityName = payment.DealerName;
                    }
                    else
                    {
                        entityId = payment.SubdealerId;
                        entityName = payment.SubdealerName;
                    }
                    break;
                case EntityType.Dialer:
                    entityId = payment.DealerId;
                    entityName = payment.DealerName;
                    break;
                case EntityType.Operator:
                    entityId = payment.OperatorId;
                    entityName = payment.OperatorName;
                    break;
                case EntityType.Gateway:
                    entityId = payment.GatewayId;
                    entityName = payment.GatewayName;
                    break;
                case EntityType.Сollector:
                    entityId = payment.PointId;
                    entityName = payment.PointName;
                    break;
                default:
                    break;
            }
        }

        public string SubstituteVariableValues(string exp)
        {
            StringBuilder result = new StringBuilder(exp);

            List<PaymentVariableMatch> variables = parseVariables(exp);

            foreach (PaymentVariableMatch variable in variables)
            {
                result.Replace(variable.VariableName, getVariableValue(variable.Key, variable.VariableName));
            }

            return result.ToString();
        }

        public AccountFilter GetAccountFilter(int accBinddingTemplateId, EntityType entityType1, EntityType entityType2, BankRequisites target, int eventPointId, long eventData)
        {
            long entityId1, entityId2, data;
            string entityName1, entityName2;

            defineEntity(entityType1, target, out entityId1, out entityName1);
            defineEntity(entityType2, target, out entityId2, out entityName2);

            data = (entityType1 == EntityType.PoindDay || entityType1 == EntityType.PointEvening || entityType1 == EntityType.PointHoliday) &&
                   entityType2 == EntityType.None && eventPointId == payment.PointId ?
                   eventData : defaultEntityData;

            return new AccountFilter(accBinddingTemplateId, entityId1, entityName1, entityId2, entityName2, data);
        }

        public EntityType ExcludeRecipientType
        {
            get
            {
                return payment.OperatorRecipient != null ? EntityType.Gateway : EntityType.Operator;
            }
        }

        public string GetRequisitesTargetInfo(BankRequisites target)
        {
            const string format = "(Тип Объекта={0}, Идентификатор={1}, Название={2})";
            string result;

            switch (target)
            {
                case BankRequisites.Subdealer:
                    result = string.Format(format, "Филиал", payment.SubdealerId, payment.SubdealerName);
                    break;
                case BankRequisites.Dealer:
                    result = string.Format(format, "Филиал", payment.DealerId, payment.DealerName);
                    break;
                case BankRequisites.Recipient:
                    if (payment.OperatorRecipient != null)
                    {
                        result = string.Format(format, "Провайдер", payment.OperatorId, payment.OperatorName);
                    }
                    else
                    {
                        result = string.Format(format, "Платёжный шлюз", payment.GatewayId, payment.GatewayName);
                    }

                    break;
                default:
                    result = string.Empty;
                    break;
            }

            return result;
        }

        public string GetAccountigTransactionSchema()
        {
            string result = null;

            if (payment.OperatorRecipient != null)
            {
                result = payment.OperatorRecipient.AccountingTransactionShema;
            }
            else if (payment.GatewayRecipient != null)
            {
                result = payment.GatewayRecipient.AccountingTransactionShema;
            }

            return result;
        }        

        public Bank GetBank(BankRequisites target)
        {
            Bank result = null;

            switch (target)
            {
                case BankRequisites.Subdealer:
                    result = payment.Subdealer;
                    break;
                case BankRequisites.Dealer:
                    result = payment.Dealer;
                    break;
                case BankRequisites.Recipient:
                    //result = payment.OperatorBank != null ? payment.OperatorBank : payment.GatewayBank;
                    Bank bank = payment.OperatorBank != null ? payment.OperatorBank : payment.GatewayBank;

                    result = new Bank();
                    result.Main = bank.Main;
                    result.Name = SubstituteVariableValues(bank.Name);
                    result.BIK = SubstituteVariableValues(bank.BIK);
                    result.CorrAccount = SubstituteVariableValues(bank.CorrAccount);                    
                    break;
            }

            return result;
        }

        public OrganizationPayer GetPayer()
        {
            OrganizationPayer result = new OrganizationPayer();
            
            result.Name = SubstituteVariableValues(payment.OperatorPayer.Name);
            result.INN = SubstituteVariableValues(payment.OperatorPayer.INN);
            result.KPP = SubstituteVariableValues(payment.OperatorPayer.KPP);

            return result;
        }

        public Organization GetRecipient(RecipientRequisites target, string account = null)
        {
            Organization result = null;

            switch (target)
            {
                case RecipientRequisites.Dealer:
                    result = new Organization();
                    result.INN = string.Empty;
                    result.KPP = string.Empty;
                    result.Name = payment.Dealer.Name;
                    result.Account = account;
                    break;
                case RecipientRequisites.Subdealer:
                    result = new Organization();
                    result.INN = string.Empty;
                    result.KPP = string.Empty;
                    result.Name = payment.Subdealer.Name;
                    result.Account = account;
                    break;
                case RecipientRequisites.Recipient:
                    Organization recipient = payment.OperatorRecipient != null ? payment.OperatorRecipient : payment.GatewayRecipient;
                    
                    result = new Organization();
                    result.INN = SubstituteVariableValues(recipient.INN); // recipient.INN;
                    result.KPP = SubstituteVariableValues(recipient.KPP); //recipient.KPP;
                    result.Name = SubstituteVariableValues(recipient.Name); //recipient.Name;
                    result.Account = SubstituteVariableValues(recipient.Account); //recipient.Account
                    result.AccountingTransactionShema = recipient.AccountingTransactionShema;
                    break;
            }

            return result;
        }

        public decimal GetAccountingTransactionAmount(AmountType amountType)
        {
            decimal result = 0.0M;

            switch (amountType)
            {
                case AmountType.PaymentWithContractCommission:
                    result = payment.Amount;
                    break;
                case AmountType.PaymentWithoutConractCommission:
                    result = payment.Amount - Math.Round(payment.ContractCommissionAmount, 2, MidpointRounding.AwayFromZero);
                    break;
                case AmountType.ClientCommission:
                    result = payment.ClientCommissionAmount;
                    break;
                case AmountType.ContractCommission:
                    result = Math.Round(payment.ContractCommissionAmount, 2, MidpointRounding.AwayFromZero);
                    break;
                case AmountType.PaymentAll:
                    result = payment.Amount + payment.ClientCommissionAmount;
                    break;
            }

            return result;
        }

        public string GetTarget(AccountingTransactionTemplateDetail template)
        {
            bool conditionExpr = template.DocumentType == DocumentType.PaymentOrder && 
                                 template.RecipientReqType == RecipientRequisites.Recipient && 
                                 !string.IsNullOrWhiteSpace(payment.PaymentTarget);
                                            
            return SubstituteVariableValues(conditionExpr ? payment.PaymentTarget : template.Description);
        }

        public string GetOKATO(RecipientRequisites target)
        {
            return target == RecipientRequisites.Recipient ? SubstituteVariableValues(payment.OKATO) : string.Empty;
        }

        public string GetKBK(RecipientRequisites target)
        {
            return target == RecipientRequisites.Recipient ? SubstituteVariableValues(payment.KBK) : string.Empty;
        }
    }
}
