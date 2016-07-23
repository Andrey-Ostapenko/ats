using System.Collections;
using System.Configuration;

namespace AccountingTransactionService.Configuration.GatewayConfigurations
{
    public class VbGatewayConfiguration
    {
        public VbGatewayConfiguration()
        {            
            Hashtable gatewaySettings = ConfigurationManager.GetSection("gatewaySettings") as Hashtable;
            SqlInsertVlbFlexDoc = (string)gatewaySettings["SqlInsertVbFlexDoc"];
            SqlInsertVlbFlexDocPack = (string)gatewaySettings["SqlInsertVbFlexDocPack"];
            SqlGenerateDocuments = (string)gatewaySettings["SqlGenerateDocuments"]; 
            ConnectionString = ConfigurationManager.ConnectionStrings["ABS"].ConnectionString;            
        }

        public string ConnectionString
        {
            private set;
            get;
        }

        public string SqlInsertVlbFlexDoc
        {
            private set;
            get;
        }

        public string SqlInsertVlbFlexDocPack
        {
            private set;
            get;
        }

        public string SqlGenerateDocuments
        {
            private set;
            get;
        }
    }
}

