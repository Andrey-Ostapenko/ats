SELECT 
  nvl(max(t.dateto), trunc(sysdate - 1)) dateFrom, 
  trunc(sysdate) dateTo 
FROM 
  paymentstatisticsevents t