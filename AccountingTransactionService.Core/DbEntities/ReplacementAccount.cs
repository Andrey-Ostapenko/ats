using System.Collections.Generic;
using System.Linq;

namespace AccountingTransactionService.DbEntities
{
    public class ReplacementAccount : List<InlineAccount>
    {
        public string FilterExp
        {
            set;
            get;
        }

        public string GetInlineAccountValue(string filter)
        {
            string result = null;

            IEnumerable<string> accounts = from inlineAccount in this
                                           where inlineAccount.Filter == filter
                                           select inlineAccount.Account;

            if (accounts.Count<string>() == 1)
            {
                result = accounts.ElementAt<string>(0);
            }

            return result;
        }
    }

}