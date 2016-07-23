SELECT 
  CAST(COALESCE(MAX(t.dateto), DATEADD(day, -1, SYSDATETIME())) AS DATE) AS dateFrom,
  CAST(SYSDATETIME() AS DATE) AS dateTo 
FROM 
  paymentstatisticsevents t