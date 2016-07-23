using System.Collections.Generic;
using System.Linq;
using AccountingTransactionService.Entities;

namespace AccountingTransactionService.DbEntities
{
    public class AccountBindingCollection : List<AccountBinding>
    {
        private readonly Dictionary<AccountFilter, AccountBinding> _accountsCache;

        public AccountBindingCollection()
        {
            _accountsCache = new Dictionary<AccountFilter, AccountBinding>();
        }

        public bool GetAccountBinding(AccountFilter accSearchData, bool compareData, out AccountBinding accountBinding)
        {
            bool result = false;
            accountBinding = null;

            if (_accountsCache.ContainsKey(accSearchData))
            {
                accountBinding = _accountsCache[accSearchData];
                result = true;
            }
            else
            {
                IEnumerable<AccountBinding> accountBindings = from accBinding in this
                                                              where accBinding.AccountBindingTemplatateId == accSearchData.AccountBinddingTemplateId &&
                                                                    accBinding.EntityId1 == accSearchData.EntityId1 &&
                                                                    accBinding.EntityId2 == accSearchData.EntityId2
                                                              select accBinding;

                foreach (AccountBinding tempAccountBinding in accountBindings)
                {
                    result = true;

                    if (compareData)
                    {
                        if (tempAccountBinding.Data == accSearchData.Data)
                        {
                            accountBinding = tempAccountBinding;
                            _accountsCache.Add(accSearchData, accountBinding);
                            break;
                        }
                    }
                    else
                    {
                        accountBinding = tempAccountBinding;
                        break;
                    }
                }
            }

            return result;
        }
    }
}