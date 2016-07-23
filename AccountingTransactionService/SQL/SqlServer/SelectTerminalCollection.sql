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
  Terminals t1 LEFT OUTER JOIN
  Terminals t2 ON t2.TerminalId = t1.TerminalId INNER JOIN
  Subdealers t3 ON t3.SubdealerId = t1.SubdealerId LEFT OUTER JOIN
  SubjectRequisities t4 ON t4.Id = t3.BankRequisitiesId
WHERE 
  t1.TerminalId = @terminalId
