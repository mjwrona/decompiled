-- Enables/disables foreign key constraint
-- Parameters:
--   @foreignKeyName     - foreign key name to enable or disable
--   @parentTableName    - parent table name
--   @schemaName         - schema name (usualyy dbo)
--   @enable  (BIT)      - 1 - enable FK, 0 - disable FK


DECLARE @stmt NVARCHAR(4000)
DECLARE @checkNoCheck NVARCHAR(20)

IF @enable = 1
BEGIN
    SET @checkNoCheck = 'CHECK'
END
ELSE
BEGIN
    SET @checkNoCheck = 'NOCHECK'
END    

SET @stmt = 'ALTER TABLE ' + QUOTENAME(@schemaName) + '.' + QUOTENAME(@parentTableName) + ' ' +
            + @checkNoCheck + ' CONSTRAINT ' + QUOTENAME(@foreignKeyName)

EXEC sp_executeSql @stmt