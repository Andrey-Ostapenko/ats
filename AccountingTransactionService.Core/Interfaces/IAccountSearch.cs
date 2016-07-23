using AccountingTransactionService.Entities;
using AccountingTransactionService.Enums;
using AccountingTransactionService.XmlEntities;

namespace AccountingTransactionService.Interfaces
{
    interface IAccountSearch
    {
        AccountFilter GetAccountFilter(int accountBindingTemplateId, EntityType entityType1, EntityType entityType2, BankRequisites target, int eventPointId, long eventData);
        Bank GetBank(BankRequisites target);
        EntityType ExcludeRecipientType { get; }
        string SubstituteVariableValues(string exp);
    }  
}
