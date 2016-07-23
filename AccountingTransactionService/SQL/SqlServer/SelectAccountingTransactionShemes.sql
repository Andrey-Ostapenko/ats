SELECT 
  t1.AccTranSchemeId,
  t1.EventTypeId,  
  t2.DisplayNumber AS AccTranSchemeNumber,
  t1.DisplayNumber AS AccTranNumber,
  t1.IsGroupDocs,
  t1.PlacePaymentId,
  t1.DebetAccSearchTemplateId,
  t1.DebetBankReqTypeId,
  t1.CreditAccSearchTemplateId,
  t1.CreditBankReqTypeId,
  t1.RecipientReqTypeId, 
  t1.DocumentTypeId, 
  t1.AmountTypeId, 
  t1.Symbol,
  t3.Target AS Description  
FROM 
  AccTranSchemeDetails t1,
  AccTranSchemes t2,
  AccTranTargetTemplates t3
WHERE   
  t2.Id = t1.AccTranSchemeId AND
  t3.Id = t1.TargetTemplateId
ORDER BY
  t1.EventTypeId,
  t1.AccTranSchemeId

