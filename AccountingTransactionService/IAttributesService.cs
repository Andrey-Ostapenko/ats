using System.ServiceModel;

namespace AccountingTransactionService
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IAttributesService" в коде и файле конфигурации.
    [ServiceContract]
    public interface IAttributesService
    {
        [OperationContract]
        int GetAttributes(int pointId, string sessionNumber, out string attributes);

        [OperationContract]
        int GetRequisities(string providerId, string parameters, int pointId, out string requisities);
    }
}
