--
-- Back up database to disk
--
-- Parameters:
--   @databaseName
--   @backupDir
--   @shrinkDatabase
--   @disablePrefixCompression
--
DECLARE @stmt NVARCHAR(MAX) = ''

SET  @stmt = 
    'ALTER DATABASE ' + QUOTENAME(@databaseName) + ' SET RECOVERY SIMPLE WITH ROLLBACK IMMEDIATE' + CHAR(13) + CHAR(10)

IF @shrinkDatabase = 1
BEGIN
SET  @stmt = @stmt +
    'DBCC SHRINKDATABASE (' + QUOTENAME(@databaseName) + ')' + CHAR(13) + CHAR(10)
END

IF @disablePrefixCompression = 1
BEGIN
SET  @stmt = @stmt +
    'DELETE ' + QUOTENAME(@databaseName) + '..tbl_ResourceManagementSetting WHERE Name = ''DisablePrefixCompression''' + CHAR(13) + CHAR(10) +
    'INSERT ' + QUOTENAME(@databaseName) + '..tbl_ResourceManagementSetting (Name, Value) VALUES (''DisablePrefixCompression'', ''1'')' + CHAR(13) + CHAR(10) +
    'EXEC ' + QUOTENAME(@databaseName) + '..prc_ChangeCompression @online = 0' + CHAR(13) + CHAR(10)
END

SET  @stmt = @stmt +
    'ALTER DATABASE ' + QUOTENAME(@databaseName) + ' SET RECOVERY FULL WITH ROLLBACK IMMEDIATE' + CHAR(13) + CHAR(10) +
    'BACKUP DATABASE ' + QUOTENAME(@databaseName)+ ' TO  DISK = ''' + @backupDir + ''' WITH NOFORMAT, INIT, SKIP, NOREWIND, NOUNLOAD,  COMPRESSION'

EXEC sp_executeSql @stmt
