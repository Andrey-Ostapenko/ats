SELECT 
  r.Id,
  r.ReplacementAccount,
  r.FilterExp, 
  i.InlineAccount,
  i.Filter
FROM 
  InlineAccounts i,
  ReplacementAccounts r
WHERE
  r.Id = i.ReplacementAccountId
ORDER BY
  r.Id
