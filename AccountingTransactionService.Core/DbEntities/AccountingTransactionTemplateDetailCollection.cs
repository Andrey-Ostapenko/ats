using System.Collections.Generic;
using System.Linq;
using AccountingTransactionService.Enums;

namespace AccountingTransactionService.DbEntities
{
    public class AccountingTransactionTemplateDetailCollection : List<AccountingTransactionTemplateDetail>
    {
        public IEnumerable<AccountingTransactionTemplateDetail> GetPaymentAccountingTransactionDetails(string accTranShemeNumber, EventType eventType)
        {
            return from detail in this
                   where detail.EventType == eventType &&
                         detail.AccTranSchemeNumber == accTranShemeNumber
                   orderby detail.AccTranNumber
                   select detail;
        }

        public IEnumerable<AccountingTransactionTemplateDetail> GetPaymentAccountingTransactionDetails(int shemeId, EventType eventType)
        {
            return from detail in this
                   where detail.EventType == eventType &&
                         detail.Id == shemeId
                   orderby detail.AccTranNumber
                   select detail;
        }

    }
}