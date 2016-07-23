SELECT DISTINCT 
  * 
FROM 
  ( 
    SELECT 
      p.Id AS PaymentId, 
      p.InitialSessionNumber,
      s.ParentID AS DealerId, 
      d.Name AS DealerName,
      CONVERT(VARCHAR, dr.Contents) AS DealerAttributes, 
      s.SubdealerID AS SubdealerId, 
      s.Name AS SubdealerName,
      CONVERT(VARCHAR, sr.Contents) AS SubdealerAttributes, 
      t.TerminalID AS PointId, 
      t.ExternalCode AS PointExternalCode,
      t.Name AS PointName,
      t.ExternalCode AS PointExternalCode,
      p.ProcessingType AS GatewayId, 
      g.Name AS GatewayName,
      CONVERT(VARCHAR, gr.Contents) AS GatewayAttributes, 
      p.ProviderId AS OperatorId, 
      o.Name AS OperatorName,
      o.PaymentTarget,
      CONVERT(VARCHAR, opr.Contents) AS OperatorAttributes, 
      p.Amount, 
      p.Amount - (p.Amount * c.percentvalue) AS AmountWithoutCommission, 
      (p.AmountAll - p.Amount) AS ClientComissionAmount, 
      (p.Amount * c.percentvalue) AS ContractCommissionAmount, 
      p.AmountAll 
    FROM 
      PreprocessingPayments p INNER JOIN
      Gateways g ON g.Id = p.ProcessingType LEFT OUTER JOIN
      ProviderBankRequisities gor ON gor.OwnRequisitiesId = g.Id LEFT OUTER JOIN
      SubjectRequisities gr ON gr.Id = gor.OwnRequisitiesId INNER JOIN      
      Operators o ON o.OperatorID = p.ProviderId LEFT OUTER JOIN
      ProviderBankRequisities oor ON oor.OwnRequisitiesId = p.ProviderId LEFT OUTER JOIN
      SubjectRequisities opr ON opr.Id = oor.OwnRequisitiesId INNER JOIN      
      Terminals t ON p.TerminalID = t.TerminalID INNER JOIN
      Subdealers s ON s.SubdealerId = t.SubdealerID LEFT OUTER JOIN 
      SubjectRequisities sr ON sr.Id = s.BankRequisitiesId INNER JOIN
      Subdealers d ON d.SubdealerId = s.ParentId LEFT OUTER JOIN
      SubjectRequisities dr ON dr.Id = d.BankRequisitiesId INNER JOIN
      SubdealerComissionData c ON c.SubdealerComissionID = s.SubdealerComissionID AND c.CyberplatOperatorID = p.CyberplatOperatorID
    WHERE 
      p.terminalid = @terminalId AND
      p.initialsessionnumber = @initialsessionnumber
    UNION ALL
    SELECT 
      p.Id AS PaymentId, 
      p.InitialSessionNumber,
      s.ParentID AS DealerId, 
      d.Name AS DealerName,
      CONVERT(VARCHAR, dr.Contents) AS DealerAttributes, 
      s.SubdealerID AS SubdealerId, 
      s.Name AS SubdealerName,
      CONVERT(VARCHAR, sr.Contents) AS SubdealerAttributes, 
      t.TerminalID AS PointId, 
      t.ExternalCode AS PointExternalCode,
      t.Name AS PointName,
      t.ExternalCode AS PointExternalCode,
      p.ProcessingType AS GatewayId, 
      g.Name AS GatewayName,
      CONVERT(VARCHAR, gr.Contents) AS GatewayAttributes, 
      p.ProviderId AS OperatorId, 
      o.Name AS OperatorName,
      o.PaymentTarget,
      CONVERT(VARCHAR, opr.Contents) AS OperatorAttributes, 
      p.Amount, 
      p.Amount - (p.Amount * c.percentvalue) AS AmountWithoutCommission, 
      (p.AmountAll - p.Amount) AS ClientComissionAmount, 
      (p.Amount * c.percentvalue) AS ContractCommissionAmount, 
      p.AmountAll 
    FROM 
      PreprocessingNewPayments p INNER JOIN
      Gateways g ON g.Id = p.ProcessingType LEFT OUTER JOIN
      ProviderBankRequisities gor ON gor.OwnRequisitiesId = g.Id LEFT OUTER JOIN
      SubjectRequisities gr ON gr.Id = gor.OwnRequisitiesId INNER JOIN      
      Operators o ON o.OperatorID = p.ProviderId LEFT OUTER JOIN
      ProviderBankRequisities oor ON oor.OwnRequisitiesId = p.ProviderId LEFT OUTER JOIN
      SubjectRequisities opr ON opr.Id = oor.OwnRequisitiesId INNER JOIN      
      Terminals t ON p.TerminalID = t.TerminalID INNER JOIN
      Subdealers s ON s.SubdealerId = t.SubdealerID LEFT OUTER JOIN 
      SubjectRequisities sr ON sr.Id = s.BankRequisitiesId INNER JOIN
      Subdealers d ON d.SubdealerId = s.ParentId LEFT OUTER JOIN
      SubjectRequisities dr ON dr.Id = d.BankRequisitiesId INNER JOIN
      SubdealerComissionData c ON c.SubdealerComissionID = s.SubdealerComissionID AND c.CyberplatOperatorID = p.CyberplatOperatorID
    WHERE 
      p.terminalid = @terminalId AND
      p.initialsessionnumber = @initialsessionnumber
  ) t 
ORDER BY t.PaymentId