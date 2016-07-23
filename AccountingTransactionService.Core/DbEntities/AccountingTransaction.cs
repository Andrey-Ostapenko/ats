using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using AccountingTransactionService.Entities;
using AccountingTransactionService.Enums;

namespace AccountingTransactionService.DbEntities
{
    public class AccountingTransaction : AccountingTransactionPaymentCollection
    {
        private static readonly Dictionary<AccountingTransactionVariable, string> variables;

        static AccountingTransaction()
        {
            variables = new Dictionary<AccountingTransactionVariable, string>
            {
                {AccountingTransactionVariable.Count, ":КоличествоПлатежей"},
                {AccountingTransactionVariable.Amount, ":СуммаОтправляемыхСредств"},
                {AccountingTransactionVariable.AmountAll, ":СуммаПринятыхСредств"},
                {AccountingTransactionVariable.RewardAmount, ":СуммаВознаграждения"},
                {AccountingTransactionVariable.PaymentDateFrom, ":ДатаПлатежейС"},
                {AccountingTransactionVariable.PaymentDateTo, ":ДатаПлатежейПо"}
            };
        }

        public AccountingTransaction()
        {
            DescriptionParameters = new AccountingTransactionDescriptionParameters();
            DocumentFields = new Dictionary<string, string>();
        }

        public long Id { set; get; }

        public string DisplayNumber { set; get; }

        public DocumentType DocumentType { set; get; }

        public AmountType AmountType { set; get; }

        public EntityType DebetEntityType1 { set; get; }

        public long DebetEntityId1 { set; get; }

        public EntityType DebetEntityType2 { set; get; }

        public long DebetEntityId2 { set; get; }

        public EntityType CreditEntityType1 { set; get; }

        public long CreditEntityId1 { set; get; }

        public EntityType CreditEntityType2 { set; get; }

        public long CreditEntityId2 { set; get; }

        public string DebetAccount { set; get; }

        public string CreditAccount { set; get; }

        public decimal Amount { set; get; }

        public AccountingTransactionDescriptionParameters DescriptionParameters { set; get; }

        public Dictionary<string, string> DocumentFields { get; }

        private string getVariableValue(AccountingTransactionVariable key, string variableName)
        {
            var result = variableName;

            if (variables.ContainsKey(key))
            {
                switch (key)
                {
                    case AccountingTransactionVariable.Count:
                        result = Count.ToString();
                        break;
                    case AccountingTransactionVariable.AmountAll:
                        result = DescriptionParameters.AmountAll.ToString();
                        break;
                    case AccountingTransactionVariable.RewardAmount:
                        result = DescriptionParameters.RewardAmount.ToString();
                        break;
                    case AccountingTransactionVariable.Amount:
                        result = Amount.ToString(CultureInfo.InvariantCulture);
                        break;
                    case AccountingTransactionVariable.PaymentDateFrom:
                        result = DescriptionParameters.PaymentDateFrom.ToString("dd.MM.yyyy");
                        break;
                    case AccountingTransactionVariable.PaymentDateTo:
                        result = DescriptionParameters.PaymentDateTo.ToString("dd.MM.yyyy");
                        break;
                }
            }

            return result;
        }

        private List<AccTranVariableMatch> parseVariables(string exp)
        {
            var result = new List<AccTranVariableMatch>();

            if (!string.IsNullOrEmpty(exp))
            {
                foreach (var key in variables.Keys)
                {
                    var mathes = Regex.Matches(exp, variables[key], RegexOptions.IgnoreCase);

                    foreach (Match match in mathes)
                    {
                        result.Add(new AccTranVariableMatch(key, match.Value));
                    }
                }
            }

            return result;
        }

        private string substituteVariableValues(string exp)
        {
            var result = new StringBuilder(exp);

            var variables = parseVariables(exp);

            foreach (var variable in variables)
            {
                result.Replace(variable.VariableName, getVariableValue(variable.Key, variable.VariableName));
            }

            return result.ToString();
        }

        public void CreateDocumentFields(AccountingTransactionFields fields)
        {
            switch (DocumentType)
            {
                case DocumentType.IncomeOrder:
                    // наименование документа
                    DocumentFields.Add("F001", "Приходный кассовый ордер");

                    // номер формы по ОКУД
                    DocumentFields.Add("F002", "0402008");

                    // номер приходного ордера
                    DocumentFields.Add("F003", string.Empty);
                    //DocumentFields.Add("F003", number.ToString().PadLeft(5, '0'));

                    // дата составления приходного ордера
                    DocumentFields.Add("F004", DateTime.Today.ToString("dd.MM.yyyy"));

                    // сумма
                    DocumentFields.Add("F007", Amount.ToString());

                    // счёт плательщика
                    DocumentFields.Add("F009", DebetAccount);

                    // банк плательщика
                    DocumentFields.Add("F010", fields.DebetBank.Name);

                    //БИК банка плательщика
                    DocumentFields.Add("F011", fields.DebetBank.BIK);

                    // банк получателя
                    DocumentFields.Add("F013", fields.CreditBank.Name);

                    // БИК банка получателя
                    DocumentFields.Add("F014", fields.CreditBank.BIK);

                    // номер счёта банка получателя
                    DocumentFields.Add("F015", CreditAccount);

                    // описание операции
                    DocumentFields.Add("F024", substituteVariableValues(fields.Description));

                    // Внешний код точки
                    DocumentFields.Add("F197", fields.VbUser);

                    // Внешний код точки
                    DocumentFields.Add("F198", fields.PointExternalCode);

                    // Учётная запись кассира
                    DocumentFields.Add("F199", fields.OperatorAccount);

                    // кассовый символ
                    DocumentFields.Add("F200", fields.Symbol != null ? fields.Symbol.ToString() : string.Empty);

                    break;
                case DocumentType.MemorialOrder:
                    // Приложение 3
                    // к Указанию Банка России
                    // от 29 декабря 2008 г. N 2161-У

                    // наименование документа
                    DocumentFields.Add("F001", "Мемориальный ордер");

                    // номер формы по ОКУД
                    DocumentFields.Add("F002", "0401108");

                    // номер приходного ордера
                    DocumentFields.Add("F003", string.Empty);
                    //DocumentFields.Add("F003", number.ToString().PadLeft(5, '0'));

                    // дата составления приходного ордера
                    DocumentFields.Add("F004", DateTime.Today.ToString("dd.MM.yyyy"));

                    // название счёта по дебету
                    DocumentFields.Add("F007", fields.DebetAccountName);

                    // номер счёта по дебету
                    DocumentFields.Add("F008", DebetAccount);

                    // сумма
                    DocumentFields.Add("F009", Amount.ToString());

                    // название счёта по кредиту
                    DocumentFields.Add("F010", fields.CreditAccountName);

                    // номер счёта по кредиту
                    DocumentFields.Add("F011", CreditAccount);

                    //шифр документа
                    DocumentFields.Add("F013", "09");

                    // Описание операции
                    DocumentFields.Add("F016", substituteVariableValues(fields.Description));

                    break;
                case DocumentType.PaymentOrder:
                    // наименование документа
                    DocumentFields.Add("F001", "Платежное поручение");

                    // номер формы по ОКУД
                    DocumentFields.Add("F002", "0401060");

                    // номер платёжного поручения
                    DocumentFields.Add("F003", string.Empty /*number.ToString().PadLeft(5, '0')*/);

                    // дата составления платёжного поручения
                    DocumentFields.Add("F004", DateTime.Today.ToString("dd.MM.yyyy"));

                    // вид платежа
                    DocumentFields.Add("F005", "электронно");

                    // сумма
                    DocumentFields.Add("F007", Amount.ToString());

                    // плательщик
                    DocumentFields.Add("F008", fields.Payer.Name);

                    // счёт плательщика
                    DocumentFields.Add("F009", DebetAccount);

                    // банк плательщика
                    DocumentFields.Add("F010", fields.DebetBank.Name);

                    //БИК банка плательщика
                    DocumentFields.Add("F011", fields.DebetBank.BIK);

                    //Кор. счёт банка плательщика
                    DocumentFields.Add("F012", fields.DebetBank.CorrAccount);

                    // банк получателя
                    DocumentFields.Add("F013", fields.CreditBank.Name);

                    // БИК банка получателя
                    DocumentFields.Add("F014", fields.CreditBank.BIK);

                    // номер счёта банка получателя
                    DocumentFields.Add("F015", fields.CreditBank.CorrAccount);

                    // получатель средств
                    DocumentFields.Add("F016", fields.Recipient.Name);

                    // номер счёта получателя средств
                    DocumentFields.Add("F017", fields.Recipient.Account);

                    // вид операции
                    DocumentFields.Add("F018", "01");

                    // назначение платежа
                    DocumentFields.Add("F024", substituteVariableValues(fields.Description));

                    // ИНН плательщика
                    DocumentFields.Add("F060", fields.Payer.INN);

                    // ИНН получателя
                    DocumentFields.Add("F061", fields.Recipient.INN);

                    // дата поступления платёжного поручения в банк плательщика
                    DocumentFields.Add("F062", DateTime.Today.ToString("dd.MM.yyyy"));

                    // дата списания денежных средств со счёта плательщика
                    DocumentFields.Add("F071", DateTime.Today.ToString("dd.MM.yyyy"));

                    // КПП плательщика
                    DocumentFields.Add("F102", fields.Payer.KPP);

                    // КПП получателя
                    DocumentFields.Add("F103", fields.Recipient.KPP);

                    // КБК
                    DocumentFields.Add("F104", fields.KBK);

                    // OKATO
                    DocumentFields.Add("F105", fields.OKATO);

                    break;
                case DocumentType.BankOrder:
                    // наименование документа
                    DocumentFields.Add("F001", "Банковский ордер");

                    // номер формы по ОКУД
                    DocumentFields.Add("F002", "0401067");

                    // номер платёжного поручения
                    DocumentFields.Add("F003", string.Empty /*number.ToString().PadLeft(5, '0')*/);

                    // дата составления платёжного поручения
                    DocumentFields.Add("F004", DateTime.Today.ToString("dd.MM.yyyy"));

                    // вид платежа
                    DocumentFields.Add("F005", "электронно");

                    // сумма
                    DocumentFields.Add("F007", Amount.ToString());

                    // плательщик
                    DocumentFields.Add("F008", fields.Payer.Name);

                    // счёт плательщика
                    DocumentFields.Add("F009", DebetAccount);

                    // банк плательщика
                    DocumentFields.Add("F010", fields.DebetBank.Name);

                    //БИК банка плательщика
                    DocumentFields.Add("F011", fields.DebetBank.BIK);

                    //Кор. счёт банка плательщика
                    DocumentFields.Add("F012", fields.DebetBank.CorrAccount);

                    // банк получателя
                    DocumentFields.Add("F013", fields.CreditBank.Name);

                    // БИК банка получателя
                    DocumentFields.Add("F014", fields.CreditBank.BIK);

                    // номер счёта банка получателя
                    DocumentFields.Add("F015", fields.CreditBank.CorrAccount);

                    // получатель средств
                    DocumentFields.Add("F016", fields.Recipient.Name);

                    // номер счёта получателя средств
                    DocumentFields.Add("F017", fields.Recipient.Account);

                    // вид операции
                    DocumentFields.Add("F018", "01");

                    // назначение платежа
                    DocumentFields.Add("F024", substituteVariableValues(fields.Description));

                    // ИНН плательщика
                    DocumentFields.Add("F060", fields.Payer.INN);

                    // ИНН получателя
                    DocumentFields.Add("F061", fields.Recipient.INN);

                    // дата поступления платёжного поручения в банк плательщика
                    DocumentFields.Add("F062", DateTime.Today.ToString("dd.MM.yyyy"));

                    // дата списания денежных средств со счёта плательщика
                    DocumentFields.Add("F071", DateTime.Today.ToString("dd.MM.yyyy"));

                    // КПП плательщика
                    DocumentFields.Add("F102", fields.Payer.KPP);

                    // КПП получателя
                    DocumentFields.Add("F103", fields.Recipient.KPP);

                    break;
                case DocumentType.OtpPaymentOrder:
                    var cultureInfo = CultureInfo.CurrentCulture.Clone() as CultureInfo;
                    cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
                    cultureInfo.DateTimeFormat.DateSeparator = "/";

                    // наименование документа
                    DocumentFields.Add("F001", "Платёжный ордер (ОТП)");

                    // номер платёжного поручения
                    DocumentFields.Add("F003", string.Empty);

                    // дата платежа
                    DocumentFields.Add("F004",
                        DescriptionParameters.PaymentDateFrom.ToString("dd/MM/yyyy HH:mm:ss", cultureInfo));

                    // сумма операции
                    DocumentFields.Add("F007", Amount.ToString("#################0.00", cultureInfo));

                    // счёт плательщика
                    DocumentFields.Add("F009", DebetAccount);

                    // номер счёта получателя средств
                    DocumentFields.Add("F017", CreditAccount);

                    // назначение платежа
                    DocumentFields.Add("F024", substituteVariableValues(fields.Description));

                    // Идентификатор платежа
                    DocumentFields.Add("F201", fields.PaymentId.ToString());

                    // Идентификатор шлюза
                    DocumentFields.Add("F202", fields.GatewayId.ToString());

                    // Идентификатор провайдера
                    DocumentFields.Add("F203", fields.OperatorId.ToString());

                    // Номер сессии платежа
                    DocumentFields.Add("F204", fields.SessionNumber);

                    // Сумма комиссии
                    DocumentFields.Add("F205", fields.ClientCommission.ToString("#####0.00", cultureInfo));

                    // Сумма вознаграждения от провайдера
                    DocumentFields.Add("F206", fields.IntegratorCommission.ToString("#####0.00", cultureInfo));

                    // Сумма НДС
                    DocumentFields.Add("F207", 0M.ToString("#####0.00", cultureInfo));

                    // Тип операции
                    DocumentFields.Add("F208", fields.OtpOperationType.ToString());

                    // Счёт комиссии
                    DocumentFields.Add("F209", fields.CommissionAccount);

                    break;
            }
        }

        private class AccTranVariableMatch
        {
            public AccTranVariableMatch(AccountingTransactionVariable key, string variableName)
            {
                Key = key;
                VariableName = variableName;
            }

            public AccountingTransactionVariable Key { get; }

            public string VariableName { get; }
        }
    }
}