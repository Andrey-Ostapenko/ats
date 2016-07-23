SELECT
  * 
FROM 
  ( 
    SELECT 
      p.Id AS PaymentId, 
      p.InitialSessionNumber,
      s.ParentID AS DealerId, 
      d.Name AS DealerName,
      dr.Contents AS DealerAttributes, 
      s.SubdealerID AS SubdealerId, 
      s.Name AS SubdealerName,
      sr.Contents AS SubdealerAttributes, 
      t.TerminalID AS PointId, 
      t.Name AS PointName,
      t.ExternalCode AS PointExternalCode,
      p.ProcessingType AS GatewayId, 
      g.Name AS GatewayName,
      gr.Contents AS GatewayAttributes, 
      p.ProviderId AS OperatorId, 
      o.Name AS OperatorName,
      o.Composite AS OperatorComposite,
      o.PaymentTarget,
      opr.Contents AS OperatorAttributes, 
      p.Params,
      p.Amount, 
      p.Amount - (p.Amount * c.percentvalue) AS AmountWithoutCommission, 
      (p.AmountAll - p.Amount) AS ClientComissionAmount, 
      (p.Amount * c.percentvalue) AS ContractCommissionAmount, 
      p.AmountAll 
      p.PaymentDateTime  AS PaymentDate  
    FROM 
      PreprocessingPayments p INNER JOIN
      Gateways g ON g.Id = p.ProcessingType LEFT OUTER JOIN
      ProviderBankRequisities gor ON gor.Id = g.BankRequisitiesId LEFT OUTER JOIN
      SubjectRequisities gr ON gr.Id = gor.OwnRequisitiesId INNER JOIN      
      Operators o ON o.OperatorID = p.ProviderId LEFT OUTER JOIN
      ProviderBankRequisities oor ON oor.Id = o.BankRequisitiesId LEFT OUTER JOIN
      SubjectRequisities opr ON opr.Id = oor.OwnRequisitiesId INNER JOIN      
      Terminals t ON p.TerminalID = t.TerminalID INNER JOIN
      Subdealers s ON s.SubdealerId = t.SubdealerID LEFT OUTER JOIN 
      SubjectRequisities sr ON sr.Id = s.BankRequisitiesId INNER JOIN
      Subdealers d ON d.SubdealerId = s.ParentId LEFT OUTER JOIN
      SubjectRequisities dr ON dr.Id = d.BankRequisitiesId INNER JOIN
      SubdealerComissionData c ON c.SubdealerComissionID = s.SubdealerComissionID AND c.CyberplatOperatorID = p.CyberplatOperatorID
    WHERE 
      p.statusid = 7 AND
      (p.terminalid = @terminalId OR -1 = @terminalId) AND
      p.paymentdatetime BETWEEN @datefrom AND @dateto
    UNION ALL
    SELECT 
      p.Id AS PaymentId, 
      p.InitialSessionNumber,
      s.ParentID AS DealerId, 
      d.Name AS DealerName,
      dr.Contents AS DealerAttributes, 
      s.SubdealerID AS SubdealerId, 
      s.Name AS SubdealerName,
      sr.Contents AS SubdealerAttributes, 
      t.TerminalID AS PointId, 
      t.Name AS PointName,
      t.ExternalCode AS PointExternalCode,
      p.ProcessingType AS GatewayId, 
      g.Name AS GatewayName,
      gr.Contents AS GatewayAttributes, 
      p.ProviderId AS OperatorId, 
      o.Name AS OperatorName,
      o.Composite AS OperatorComposite,
      o.PaymentTarget,
      opr.Contents AS OperatorAttributes, 
      p.Params,
      p.Amount, 
      p.Amount - (p.Amount * c.percentvalue) AS AmountWithoutCommission, 
      (p.AmountAll - p.Amount) AS ClientComissionAmount, 
      (p.Amount * c.percentvalue) AS ContractCommissionAmount, 
      p.AmountAll,
      p.PaymentDateTime  AS PaymentDate  
    FROM 
      PreprocessingNewPayments p INNER JOIN
      Gateways g ON g.Id = p.ProcessingType LEFT OUTER JOIN
      ProviderBankRequisities gor ON gor.Id = g.BankRequisitiesId LEFT OUTER JOIN
      SubjectRequisities gr ON gr.Id = gor.OwnRequisitiesId INNER JOIN      
      Operators o ON o.OperatorID = p.ProviderId LEFT OUTER JOIN
      ProviderBankRequisities oor ON oor.Id = o.BankRequisitiesId LEFT OUTER JOIN
      SubjectRequisities opr ON opr.Id = oor.OwnRequisitiesId INNER JOIN      
      Terminals t ON p.TerminalID = t.TerminalID INNER JOIN
      Subdealers s ON s.SubdealerId = t.SubdealerID LEFT OUTER JOIN 
      SubjectRequisities sr ON sr.Id = s.BankRequisitiesId INNER JOIN
      Subdealers d ON d.SubdealerId = s.ParentId LEFT OUTER JOIN
      SubjectRequisities dr ON dr.Id = d.BankRequisitiesId INNER JOIN
      SubdealerComissionData c ON c.SubdealerComissionID = s.SubdealerComissionID AND c.CyberplatOperatorID = p.CyberplatOperatorID
    WHERE 
      p.statusid = 7 AND
      (p.terminalid = @terminalId OR -1 = @terminalId) AND
      p.paymentdatetime BETWEEN @datefrom AND @dateto
  ) t 
ORDER BY t.PaymentId