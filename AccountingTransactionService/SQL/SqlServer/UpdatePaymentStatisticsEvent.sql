UPDATE 
  PaymentStatisticsEvents 
SET 
  SendDate = @SendDate, 
  Status = @Status 
WHERE 
  Id = @Id