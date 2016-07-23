using System.Collections.Generic;
using System.Linq;

namespace AccountingTransactionService.DbEntities
{
    public class AccountSearchRuleCollection : List<AccountSearchRule>
    {
        public IEnumerable<AccountSearchRule> GetRulesByTemplate(int accSearchTemplateId)
        {
            return from rule in this
                   where rule.AccountSearchTemplate == accSearchTemplateId
                   orderby rule.Priority descending
                   select rule;
        }
    }
}