-- Alter authorization on schemas and fulltext catalogs to dbo

DECLARE @stmt NVARCHAR(MAX) = ''

SELECT @stmt = 'ALTER AUTHORIZATION ON SCHEMA::' + QUOTENAME(s.name) + ' TO dbo' + CHAR(13) + CHAR(10)
       + @stmt
FROM   sys.schemas s
JOIN   sys.database_principals u
ON     s.principal_id = u.principal_id
WHERE  u.type = 'S'
AND    u.default_schema_name = 'dbo'
AND    u.name <> 'dbo'

SELECT @stmt = 'ALTER AUTHORIZATION ON FULLTEXT CATALOG::' + QUOTENAME(c.name) + ' TO dbo' + CHAR(13) + CHAR(10)
       + @stmt
FROM   sys.fulltext_catalogs c
JOIN   sys.database_principals u
ON     c.principal_id = u.principal_id
WHERE  u.type = 'S'
AND    u.default_schema_name = 'dbo'
AND    u.name <> 'dbo'

EXEC sp_executeSql @stmt