using System.Collections;
using System.Configuration;

namespace AccountingTransactionService.Configuration.GatewayConfigurations
{
    public class OtpGatewayConfiguration
    {
        private Hashtable gatewaySettings;

        public OtpGatewayConfiguration()
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

        public string SqlCreateOperation
        {
            get
            {                
                return (string)gatewaySettings["SqlCreateOperation"];
            }
        }
    }
}

