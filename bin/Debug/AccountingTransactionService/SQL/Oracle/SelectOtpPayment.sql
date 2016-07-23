SELECT
  * 
FROM 
  ( 
    SELECT 
      p.Id AS PaymentId, 
      p.SessionNumber, 
      p.PaymentDateTime AS PaymentDate,
      p.Amount, 
      (p.AmountAll - p.Amount) AS ClientComissionAmount, 
      p.AmountAll,
      p.Params,
      p.TerminalId AS PointId, 
      t.Name AS PointName,
      t.ExternalCode AS PointExternalCode,
      t.Account AS PointAccount,
      p.ProcessingType AS GatewayId, 
      g.Name AS GatewayName,
      p.ProviderId AS OperatorId, 
      o.Name AS OperatorName,
      to_char(sr.Contents) AS OperatorAttributes
    FROM 
      PreprocessingNewPayments p, 
      Terminals t, 
      Gateways g, 
      Operators o,
      ProviderBankRequisities pbr,
      SubjectRequisities sr 
    WHERE 
      t.TerminalId = p.TerminalId AND 
      g.Id = p.ProcessingType AND
      o.OperatorId = p.ProviderId AND
      pbr.Id(+) = o.BankRequisitiesId AND
      sr.Id(+) = pbr.OwnRequisitiesId AND
      t.TerminalId = :TerminalId AND
      p.InitialSessionNumber = :InitialSessionNumber
    UNION ALL
    SELECT 
      p.Id AS PaymentId, 
      p.SessionNumber, 
      p.PaymentDateTime AS PaymentDate,
      p.Amount, 
      (p.AmountAll - p.Amount) AS ClientComissionAmount, 
      p.AmountAll,
      p.Params,
      p.TerminalId AS PointId, 
      t.Name AS PointName,
      t.ExternalCode AS PointExternalCode,
      t.Account AS PointAccount,
      p.ProcessingType AS GatewayId, 
      g.Name AS GatewayName,
      p.ProviderId AS OperatorId, 
      o.Name AS OperatorName,
      to_char(sr.Contents) AS OperatorAttributes
    FROM 
      PreprocessingPayments p, 
      Terminals t, 
      Gateways g, 
      Operators o,
      ProviderBankRequisities pbr,
      SubjectRequisities sr  
    WHERE 
      t.TerminalId = p.TerminalId AND 
      g.Id = p.ProcessingType AND
      o.OperatorId = p.ProviderId AND
      pbr.Id(+) = o.BankRequisitiesId AND
      sr.Id(+) = pbr.OwnRequisitiesId AND
      t.TerminalId = :TerminalId AND
      p.InitialSessionNumber = :InitialSessionNumber
  ) t 
ORDER BY t.PaymentId