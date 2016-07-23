SELECT 
  t1.TerminalId,
  t1.Name,
  t1.ExternalCode,
  t1.PayOfficeId,
  t2.Name AS PayOfficeName,
  t1.AccTranSchemeId,
  t1.SubdealerId,
  t3.Name AS SubdealerName,
  t4.Contents AS SubdealerAttributes
FROM 
  Terminals t1,
  Terminals t2,
  Subdealers t3,
  SubjectRequisities t4
WHERE 
  t2.TerminalId = t1.TerminalId AND
  t3.SubdealerId = t1.SubdealerId AND
  t4.Id(+) = t3.BankRequisitiesId AND
  t1.TerminalId = :terminalId
