-- Disables foreign keys in the database and returns a list of foreign keys that were disabled.
-- No parameters

DECLARE @status INT
DECLARE @scriptName NVARCHAR(80)
SET @scriptName = 'stmt_DisableForeignKeys'
SET NOCOUNT ON

DECLARE @foreignKeys TABLE
(
    foreignKeyName    SYSNAME NOT NULL,
    parentTableName   SYSNAME NOT NULL,
    schemaName        SYSNAME NOT NULL
)

INSERT  @foreignKeys(foreignKeyName, parentTableName, schemaName)
SELECT  name,
        OBJECT_NAME(parent_object_id), 
        SCHEMA_NAME(schema_id)
FROM    sys.foreign_keys
WHERE   is_disabled = 0

SET @status = @@ERROR

IF @status <> 0
BEGIN
    RAISERROR (800001, 16, -1, @scriptName, @status, N'INSERT', N'@foreignKeys')
    RETURN
END

DECLARE @stmt NVARCHAR(MAX)
SET @stmt = N''

SELECT  @stmt = @stmt + 'ALTER TABLE ' + QUOTENAME(schemaName) + '.' + QUOTENAME(parentTableName) +
        ' NOCHECK CONSTRAINT ' + QUOTENAME(foreignKeyName) + CHAR(10)
FROM    @foreignKeys

BEGIN TRAN    
EXEC sp_executesql @stmt
SET @status = @@ERROR
IF @status <> 0
BEGIN
    ROLLBACK
    RETURN
END
COMMIT

SELECT  foreignKeyName,
        parentTableName,
        schemaName
FROM    @foreignKeys
