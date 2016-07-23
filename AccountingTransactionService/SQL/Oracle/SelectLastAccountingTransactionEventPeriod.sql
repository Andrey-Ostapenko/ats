SELECT 
  nvl(max(t.dateto), trunc(sysdate)) dateFrom, 
  sysdate dateTo
FROM 
  accountingtransactionevents t 
WHERE 
  t.eventtypeid = 1