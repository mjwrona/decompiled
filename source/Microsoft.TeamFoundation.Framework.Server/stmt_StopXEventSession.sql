-- Parameter: @dbSessionName NVARCHAR(MAX) - Name of the session to stop. Note this parameter is only used for SQL Azure
-- Parameter: @enforceReadScaleOut BIT - If 1 and stmt is used to start readScaleOutSession than the check if the query is running in readScaleOut is enforced.

SET XACT_ABORT ON

DECLARE @databaseName    NVARCHAR(250) = DB_NAME()
DECLARE @sessionName     NVARCHAR(250)
DECLARE @sessionType     NVARCHAR(8)
DECLARE @statement       NVARCHAR(MAX)
DECLARE @sessionStarted  BIT = 0
DECLARE @isRunningInReadScaleOut BIT = 0
DECLARE @readScaleOutSessionName NVARCHAR(27) = 'vsts_xevents_read_scale_out'

IF (CAST(SERVERPROPERTY('Edition') AS SYSNAME) COLLATE DATABASE_DEFAULT = N'SQL Azure')
BEGIN
    SET @sessionName = @dbSessionName
    SET @sessionType = 'DATABASE'

    IF EXISTS (SELECT * FROM sys.dm_xe_database_sessions WHERE name = @sessionName)
    BEGIN
        SET @sessionStarted = 1
    END
END
ELSE
BEGIN
    SET @sessionName = @databaseName
    SET @sessionType = 'SERVER'

    IF EXISTS (SELECT * FROM sys.dm_xe_sessions WHERE name = @sessionName)
    BEGIN
        SET @sessionStarted = 1
    END
END

IF DATABASEPROPERTYEX(DB_NAME(), 'Updateability') = 'READ_ONLY' 
BEGIN
    SET @isRunningInReadScaleOut = 1
END

-- stop the session if it is currently started
-- this will throw if the specified session does not exist
IF (@sessionStarted = 1 AND (@dbSessionName != @readScaleOutSessionName OR @enforceReadScaleOut != 1 OR @isRunningInReadScaleOut = 1))
BEGIN
    SET @statement = N'
            ALTER EVENT SESSION ' + QUOTENAME(@sessionName) + ' ON ' + @sessionType + '
            STATE=STOP'

    EXEC sp_executesql @statement
END

-- this will throw if the specified session does not exist
-- Note that this code can't be executed on the read scale out
-- and it will throw error like "Failed to update database "Tfs_..." because the database is read-only." if tried
if (@dbSessionName != @readScaleOutSessionName)
BEGIN
    SET @statement = N'
        ALTER EVENT SESSION ' + QUOTENAME(@sessionName) + ' ON ' + @sessionType + '
        WITH (STARTUP_STATE=OFF)'

    EXEC sp_executesql @statement
END

SELECT @isRunningInReadScaleOut