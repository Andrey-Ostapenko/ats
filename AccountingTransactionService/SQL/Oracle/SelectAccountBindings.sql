SELECT 
  t1.Id AS AccountBindingTemplateId,
  t1.EntityTypeid1,
  t2.EntityId1, 
  t1.EntityTypeid2,
  t2.EntityId2,
  t1.Data AS Data,
  t2.Description AS AccountName, 
  t2.Account AS Account,
  t2.IsDynamic
FROM 
  AccountBindingTemplates t1,
  AccountBindings t2
WHERE 
  t1.Id = t2.AccountBindingTemplateId 
order by 
  t1.Id