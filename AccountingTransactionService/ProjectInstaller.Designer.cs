namespace AccountingTransactionService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.AccountingTransactionServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.AccountingTransactionServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // AccountingTransactionServiceProcessInstaller
            // 
            this.AccountingTransactionServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.AccountingTransactionServiceProcessInstaller.Password = null;
            this.AccountingTransactionServiceProcessInstaller.Username = null;
            // 
            // AccountingTransactionServiceInstaller
            // 
            this.AccountingTransactionServiceInstaller.Description = "Сервис для создания и выгрузки проводок по платежам";
            this.AccountingTransactionServiceInstaller.DisplayName = "AccountingTransactionService";
            this.AccountingTransactionServiceInstaller.ServiceName = "AccountingTransactionService";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.AccountingTransactionServiceProcessInstaller,
            this.AccountingTransactionServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller AccountingTransactionServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller AccountingTransactionServiceInstaller;
    }
}