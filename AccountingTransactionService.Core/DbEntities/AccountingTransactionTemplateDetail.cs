using AccountingTransactionService.Enums;

namespace AccountingTransactionService.DbEntities
{
    public class AccountingTransactionTemplateDetail
    {
        public AccountingTransactionTemplateDetail() { }

        public int Id
        {
            set;
            get;
        }

        public bool IsGroupDocs
        {
            set;
            get;
        }

        public PlacePayment PlacePayment
        {
            set;
            get;
        }

        public string AccTranSchemeNumber
        {
            set;
            get;
        }

        public string AccTranNumber
        {
            set;
            get;
        }

        public EventType EventType
        {
            set;
            get;
        }

        public DocumentType DocumentType
        {
            set;
            get;
        }

        public int DebetAccountSearchTemplate
        {
            set;
            get;
        }

        public BankRequisites DebetBankReqType
        {
            set;
            get;
        }

        public int CreditAccountSearchTemplate
        {
            set;
            get;
        }

        public BankRequisites CreditBankReqType
        {
            set;
            get;
        }

        public RecipientRequisites RecipientReqType
        {
            set;
            get;
        }

        public AmountType AmountType
        {
            set;
            get;
        }

        public int? Symbol
        {
            set;
            get;
        }

        public string Description
        {
            set;
            get;
        }
    }
}