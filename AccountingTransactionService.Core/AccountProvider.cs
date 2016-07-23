using System.Collections.Generic;
using AccountingTransactionService.Configuration;
using AccountingTransactionService.DbEntities;
using AccountingTransactionService.Entities;
using AccountingTransactionService.Enums;
using AccountingTransactionService.Interfaces;

namespace AccountingTransactionService
{
    class AccountProvider
    {
        private readonly AccountSearchRuleCollection _rules;
        private readonly AccountBindingCollection _accounts;
        private readonly ReplacementAccountCollection _replacementAccounts;
        private List<string> log;
        private int mainBank;
        private int pointId;
        private int data;

        public AccountProvider(AccountSearchRuleCollection rules, AccountBindingCollection accounts, ReplacementAccountCollection replacementAccounts)
        {
            mainBank = ConfigurationProvider.MainBank;
            pointId = ConfigurationProvider.DefaultPointId;
            data = ConfigurationProvider.DefaultData;

            this._rules = rules;
            this._accounts = accounts;
            this._replacementAccounts = replacementAccounts;
            log = new List<string>();
        }

        public string[] GetLog()
        {
            return log.ToArray();
        }

        public bool GetAccount(AccountingTransactionEvent accTranEvent, IAccountSearch item, AccountingTransactionTemplateDetail accTranTemplate, AccountPosition position, bool compareData, out Account account)
        {
            bool result = false;
            AccountBinding accBinding;
            account = null;
            log.Clear();

            BankRequisites bankRequisites = position == AccountPosition.Debet ? accTranTemplate.DebetBankReqType : accTranTemplate.CreditBankReqType;

            int accountSearchTemplate = position == AccountPosition.Debet ? accTranTemplate.DebetAccountSearchTemplate : accTranTemplate.CreditAccountSearchTemplate;

            IEnumerable<AccountSearchRule> templateRules = _rules.GetRulesByTemplate(accountSearchTemplate);
            
            foreach (AccountSearchRule rule in templateRules)
            {
                // Исключаем из поиска получателей провайдера или шлюза в зависимости от определения реквизитов
                if (rule.EntityType1 == item.ExcludeRecipientType || rule.EntityType2 == item.ExcludeRecipientType) continue;

                AccountFilter accountSearchData = item.GetAccountFilter(rule.AccountBindingTemplate, rule.EntityType1, rule.EntityType2, bankRequisites, accTranEvent.PointId, accTranEvent.Data);

                // выполняем поиск счёта
                if (_accounts.GetAccountBinding(accountSearchData, compareData, out accBinding))
                {
                    if (result == false) result = true;
                }
                
                // В случае нахождения счёта заполняем информацию
                if (accBinding != null)
                {
                    account = new Account();
                    account.EntityType1 = rule.EntityType1;
                    account.EntityId1 = accBinding.EntityId1;
                    account.EntityName1 = accountSearchData.EntityName1;
                    account.EntityType2 = rule.EntityType2;
                    account.EntityId2 = accBinding.EntityId2;
                    account.EntityName2 = accountSearchData.EntityName2;
                    account.Data = accBinding.Data;
                    account.Name = accBinding.AccountName;                    
                    account.Bank = item.GetBank(bankRequisites);

                    if (accBinding.IsDynamic)
                    {
                        account.Value = item.SubstituteVariableValues(accBinding.Account);
                    }
                    else if (_replacementAccounts.ContainsKey(accBinding.Account))
                    {
                        ReplacementAccount repAccount = _replacementAccounts[accBinding.Account];

                        string filter = item.SubstituteVariableValues(repAccount.FilterExp);
                        
                        log.Add(
                            $"Обнаружен счёт который следует заменить счётом подстановки, заменяемый счёт {accBinding.Account}, выражение для поиска {repAccount.FilterExp}, значение для поиска {filter}");
                                                
                        string inlineAccount = repAccount.GetInlineAccountValue(filter);

                        if (inlineAccount != null)
                        {
                            account.Value = inlineAccount;
                        }
                        else
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        account.Value = accBinding.Account;
                    }

                    break;
                }

                // логируем все шаги которые выполнялись при нахождении счёта
                if (rule.EntityType2 == EntityType.None)
                {
                    log.Add(
                        string.Format("Шаблон проводки={0}, Название шаблона счёта={1}, Тип объекта 1={2}, Идентификатор объекта 1={3}, Название объекта 1={4}.",
                        accTranTemplate.AccTranNumber,
                        rule.AccountBindingTemplateName,
                        rule.EntityType1,
                        accountSearchData.EntityId1,
                        accountSearchData.EntityName1));
                }
                else
                {
                    log.Add(
                        string.Format("Шаблон проводки={0}, Название шаблона счёта={1}, Тип объекта 1={2}, Идентификатор объекта 1={3}, Название объекта 1={4}, Тип объекта 2={5}, Идентификатор объекта 2={6}, Название объекта 2={7}.",
                        accTranTemplate.AccTranNumber,
                        rule.AccountBindingTemplateName,
                        rule.EntityType1,
                        accountSearchData.EntityId1,
                        accountSearchData.EntityName1,
                        rule.EntityType2,
                        accountSearchData.EntityId2,
                        accountSearchData.EntityName2));
                }
            }

            return result;
        }

        public bool GetAccount(AccountFilter filter, out string account)
        {
            AccountBinding accBinding;
            bool result = _accounts.GetAccountBinding(filter, false, out accBinding);
            account = accBinding != null & result ? accBinding.Account : null;
            return result;
        }
    }
}
