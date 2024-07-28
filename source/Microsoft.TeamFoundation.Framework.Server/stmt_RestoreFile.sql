-- Restores database file.
-- Parameter: @dbName       - the name of the database
-- Parameter: @fileName     - the name of the database file (name from sys.database_files)
-- Parameter: @path         - the name of the backup file to restore.
SET NOCOUNT ON

DECLARE @stmt NVARCHAR(MAX) = ''
SELECT @stmt = @stmt + '
KILL ' + CONVERT(VARCHAR, s.session_id)
FROM    sys.dm_exec_sessions s
WHERE   s.database_id = DB_ID(@dbName)
        AND s.is_user_process = 1

EXEC sp_executesql @stmt

RESTORE DATABASE @dbName FILE = @fileName FROM DISK = @path
