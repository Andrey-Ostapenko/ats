using System;
using System.ServiceProcess;
using Utilities.Logging;

namespace AccountingTransactionService
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        static void Main()
        {
            try
            {
                ServiceBase.Run(new global::AccountingTransactionService.AccountingTransactionService());
            }
            catch(Exception ex)
            {
                Log.WriteToFile(ex);
            }
        }
    }
}
