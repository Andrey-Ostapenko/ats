SELECT  
  * 
FROM 
  ( 
    SELECT 
      t1.Id AS PaymentId, 
      t1.InitialSessionNumber, 
      t3.ParentID AS DealerId, 
      t5.Name AS DealerName,
      to_char(t6.Contents) AS DealerAttributes, 
      t2.SubdealerID AS SubdealerId, 
      t3.Name AS SubdealerName,
      to_char(t4.Contents) AS SubdealerAttributes, 
      t1.TerminalID AS PointId, 
      t2.ExternalCode AS PointExternalCode,
      t2.Name AS PointName,
      t1.ProcessingType AS GatewayId, 
      t7.Name AS GatewayName,
      to_char(t9.Contents) AS GatewayAttributes, 
      t1.ProviderId AS OperatorId, 
      t10.Name AS OperatorName,
      t10.PaymentTarget,
      to_char(t12.Contents) AS OperatorAttributes, 
      t1.Amount, 
      t1.Amount - (t1.Amount * t13.percentvalue) AS AmountWithoutCommission, 
      (t1.AmountAll - t1.Amount) AS ClientComissionAmount, 
      (t1.Amount * t13.percentvalue) AS ContractCommissionAmount, 
      t1.AmountAll 
    FROM 
      PreprocessingPayments t1, 
      Terminals t2, 
      Subdealers t3, 
      SubjectRequisities t4, 
      Subdealers t5, 
      SubjectRequisities t6, 
      Gateways t7, 
      ProviderBankRequisities t8, 
      SubjectRequisities t9, 
      Operators t10, 
      ProviderBankRequisities t11,
      SubjectRequisities t12, 
      SubdealerComissionData t13
    WHERE 
      t1.TerminalID = t2.TerminalID AND 
      t2.SubdealerID = t3.SubdealerId AND
      t3.BankRequisitiesId = t4.Id AND
      t3.ParentId = t5.SubdealerId(+) AND
      t5.BankRequisitiesId = t6.Id(+) AND
      t1.ProcessingType = t7.Id AND
      t7.BankRequisitiesId = t8.Id AND 
      t8.OwnRequisitiesId = t9.Id AND
      t10.OperatorId = t1.ProviderId AND
      t11.Id(+) = t10.BankRequisitiesId AND
      t12.Id(+) = t11.OwnRequisitiesId AND
      t13.SubdealerComissionId = t3.SubdealerComissionId AND
      t13.CyberplatOperatorId = t1.CyberplatOperatorId AND 
      t1.terminalid = :terminalId AND
      t1.initialsessionnumber = :initialsessionnumber
    UNION ALL
    SELECT 
      t1.Id AS PaymentId, 
      t1.InitialSessionNumber, 
      t3.ParentID AS DealerId, 
      t5.Name AS DealerName,
      to_char(t6.Contents) AS DealerAttributes,
      t2.SubdealerID AS SubdealerId, 
      t3.Name AS SubdealerName,
      to_char(t4.Contents) AS SubdealerAttributes,
      t1.TerminalID AS PointId, 
      t2.ExternalCode AS PointExternalCode,
      t2.Name AS PointName,
      t1.ProcessingType AS GatewayId, 
      t7.Name AS GatewayName,
      to_char(t9.Contents) AS GatewayAttributes, 
      t1.ProviderId AS OperatorId,
      t10.Name AS OperatorName,
      t10.PaymentTarget,
      to_char(t12.Contents) AS OperatorAttributes, 
      t1.Amount, 
      t1.Amount - (t1.Amount * t13.percentvalue) AS AmountWithoutCommission, 
      (t1.AmountAll - t1.Amount) AS ClientComissionAmount, 
      (t1.Amount * t13.percentvalue) AS ContractCommissionAmount, 
      t1.AmountAll 
    FROM 
      PreprocessingNewPayments t1,
      Terminals t2, 
      Subdealers t3,
      SubjectRequisities t4, 
      Subdealers t5, 
      SubjectRequisities t6, 
      Gateways t7,
      ProviderBankRequisities t8, 
      SubjectRequisities t9, 
      Operators t10, 
      ProviderBankRequisities t11, 
      SubjectRequisities t12, 
      SubdealerComissionData t13
    WHERE 
      t1.TerminalID = t2.TerminalID AND
      t2.SubdealerID = t3.SubdealerId AND
      t3.BankRequisitiesId = t4.Id AND
      t3.ParentId = t5.SubdealerId(+) AND
      t5.BankRequisitiesId = t6.Id(+) AND
      t1.ProcessingType = t7.Id AND
      t7.BankRequisitiesId = t8.Id AND
      t8.OwnRequisitiesId = t9.Id AND
      t10.OperatorId = t1.ProviderId AND
      t11.Id(+) = t10.BankRequisitiesId AND
      t12.Id(+) = t11.OwnRequisitiesId AND
      t13.SubdealerComissionId = t3.SubdealerComissionId AND
      t13.CyberplatOperatorId = t1.CyberplatOperatorId AND
      t1.terminalid = :terminalId AND
      t1.initialsessionnumber = :initialsessionnumber
  ) t 
ORDER BY t.PaymentId