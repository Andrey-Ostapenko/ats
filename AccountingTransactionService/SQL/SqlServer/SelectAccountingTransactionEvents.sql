SELECT 
  * 
FROM 
  AccountingTransactionEvents t 
WHERE 
  t.Status in (0, 1)