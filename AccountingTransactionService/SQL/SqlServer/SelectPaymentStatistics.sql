SELECT DISTINCT 
  trunc(t.PaymentDate) AS PaymentDate, 
  t.PointId, 
  t.OperatorName AS PaymentName, 
  COUNT(*) AS Count, 
  SUM(t.Amount) AS Amount, 
  SUM(t.CommissionAmount) AS CommissionAmount 
FROM 
  ( 
    SELECT 
      t1.Id AS PaymentId, 
      t1.PaymentDatetime AS PaymentDate, 
      t1.TerminalID AS PointId, 
      t1.ProcessingType AS GatewayId, 
      t4.Name AS GatewayName, 
      t1.ProviderId AS OperatorId, 
      substr(t5.Name, 1, 150) OperatorName, 
      t1.Amount, 
      (t1.AmountAll - t1.Amount + t1.Amount * t6.percentvalue) AS CommissionAmount
    FROM 
      PreprocessingPayments t1, 
      Terminals t2, 
      Subdealers t3, 
      Gateways t4, 
      Operators t5, 
      SubdealerComissionData t6 
    WHERE 
      t1.TerminalID = t2.TerminalID AND
      t2.SubdealerID = t3.SubdealerId AND
      t1.ProcessingType = t4.Id AND 
      t5.OperatorId = t1.ProviderId AND 
      t6.SubdealerComissionId = t3.SubdealerComissionId AND
      t6.CyberplatOperatorId = t1.CyberplatOperatorId AND
      t1.statusid = 7 AND
      t1.paymentdatetime BETWEEN @datefrom AND @dateto
    UNION ALL
    SELECT 
      t1.Id AS PaymentId, 
      t1.PaymentDatetime AS PaymentDate, 
      t1.TerminalID AS PointId, 
      t1.ProcessingType AS GatewayId, 
      t4.name AS GatewayName, 
      t1.ProviderId AS OperatorId, 
      substr(t5.Name, 1, 150) OperatorName, 
      t1.Amount, 
      (t1.AmountAll - t1.Amount + t1.Amount * t6.percentvalue) AS CommissionAmount 
    FROM 
      PreprocessingNewPayments t1, 
      Terminals t2, 
      Subdealers t3, 
      Gateways t4, 
      Operators t5,
      SubdealerComissionData t6 
    WHERE 
      t1.TerminalID = t2.TerminalID AND
      t2.SubdealerID = t3.SubdealerId AND
      t1.ProcessingType = t4.Id AND
      t5.OperatorId = t1.ProviderId AND 
      t6.SubdealerComissionId = t3.SubdealerComissionId AND
      t6.CyberplatOperatorId = t1.CyberplatOperatorId AND
      t1.StatusId = 7 AND 
      t1.PaymentDatetime BETWEEN @datefrom AND @dateto
  ) t 
GROUP BY 
  t.PointId, 
  t.OperatorName, 
  trunc(t.PaymentDate) 
ORDER BY 
  t.PointId