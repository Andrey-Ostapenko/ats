using System.Collections;
using System.Configuration;

namespace AccountingTransactionService.Configuration.GatewayConfigurations
{
    public class CftGatewayConfiguration
    {
        private Hashtable gatewaySettings;

        public CftGatewayConfiguration()
        {
            gatewaySettings = ConfigurationManager.GetSection("gatewaySettings") as Hashtable;
        }

        public string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["ABS"].ConnectionString;
            }
        }

        public string SqlSelectVlbFlexDocCount
        {
            get
            {
                return (string)gatewaySettings["SqlSelectVlbFlexDocCount"];
            }
        }

        

        public string SqlInsertVlbFlexDoc
        {
            get
            {
                return (string)gatewaySettings["SqlInsertVlbFlexDoc"];
            }
        }

        public string SqlInsertVlbFlexDocPack
        {
            get
            {
                return (string)gatewaySettings["SqlInsertVlbFlexDocPack"];
            }
        }

        public string SqlInsertVlbPaymentStatistics
        {
            get
            {
                return (string)gatewaySettings["SqlInsertVlbPaymentStatistics"];
            }
        }

    }
}

