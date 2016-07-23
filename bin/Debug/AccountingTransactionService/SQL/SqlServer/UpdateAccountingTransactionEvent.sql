UPDATE 
  AccountingTransactionEvents 
SET 
  CreateDate = @CreateDate,
  SendDate = @SendDate, 
  Status = @Status 
WHERE Id = @Id