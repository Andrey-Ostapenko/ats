SELECT 
  t1.Id, 
  t1.AccSearchTemplateId,
  t1.AccountBindingTemplateId, 
  t2.Description AS AccountBindingTemplate,
  t2.EntityTypeId1 AS EntityTypeId1, 
  t2.EntityTypeId2 AS EntityTypeId2,
  t1.Priority As Priority 
FROM 
  AccountSearchTemplateDetails t1, 
  AccountBindingTemplates t2  
WHERE 
  t2.Id = t1.AccountBindingTemplateId 
ORDER BY 
  AccSearchTemplateId,
  Priority DESC
