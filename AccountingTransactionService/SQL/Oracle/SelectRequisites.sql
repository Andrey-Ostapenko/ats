SELECT 
  t3.ParentID AS DealerId, 
  t5.Name AS DealerName,
  to_char(t6.Contents) AS DealerAttributes, 
  t2.SubdealerID AS SubdealerId, 
  t3.Name AS SubdealerName,
  to_char(t4.Contents) AS SubdealerAttributes, 
  t2.TerminalID AS PointId, 
  t2.ExternalCode AS PointExternalCode,
  t2.Name AS PointName,
  t1.ProcessingType AS GatewayId, 
  t7.Name AS GatewayName,
  to_char(t9.Contents) AS GatewayAttributes, 
  t1.OperatorId,
  t10.Name AS OperatorName,
  to_char(t12.Contents) AS OperatorAttributes,
  t10.Composite AS OperatorComposite,
  t10.PaymentTarget
FROM 
  ProfileData t1, 
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
  SubjectRequisities t12 
WHERE 
  t2.ProfileId = t1.ProfileId AND
  t2.SubdealerID = t3.SubdealerId AND
  t3.BankRequisitiesId = t4.Id AND
  t3.ParentId = t5.SubdealerId(+) AND
  t5.BankRequisitiesId = t6.Id(+) AND
  t1.ProcessingType = t7.Id AND
  t7.BankRequisitiesId = t8.Id AND 
  t8.OwnRequisitiesId = t9.Id AND
  t10.OperatorId = t1.OperatorId AND
  t11.Id(+) = t10.BankRequisitiesId AND
  t12.Id(+) = t11.OwnRequisitiesId AND
  t2.TerminalID = :TerminalId AND 
  t1.OperatorId = :ProviderId