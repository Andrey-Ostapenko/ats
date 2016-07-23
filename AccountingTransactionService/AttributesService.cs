using System.ServiceModel;
using AccountingTransactionService.Interfaces;

namespace AccountingTransactionService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class AttributesService : IAttributesService
    {
        private readonly PaymentAttributesProvider _attrProvider;

        public AttributesService(IDbProvider dbProvider)
        {
            _attrProvider = new PaymentAttributesProvider(dbProvider);
        }

        public int GetAttributes(int pointId, string sessionNumber, out string attributes)
        {            
            return _attrProvider.GetPaymentAttributes(pointId, sessionNumber, out attributes);
        }

        public int GetRequisities(string providerId, string parameters, int pointId, out string requisities)
        {
            return _attrProvider.GetRequisities(long.Parse(providerId), parameters, pointId, out requisities);
        }
    }
}
