-- Parameter: @includeStatement BIT = 1 if we should collect statement data, 0 otherwise
-- Parameter: @dbSessionName NVARCHAR(MAX) - Name of the session to alter. Note this parameter is only used for SQL Azure
SET XACT_ABORT ON

DECLARE @databaseName      NVARCHAR(250) = DB_NAME()
DECLARE @sessionName       NVARCHAR(250)
DECLARE @sessionType       NVARCHAR(8)
DECLARE @statement         NVARCHAR(MAX)
DECLARE @whereClause       NVARCHAR(MAX)
DECLARE @dropEvent         BIT = 0

IF CAST(SERVERPROPERTY('Edition') AS SYSNAME) COLLATE DATABASE_DEFAULT = N'SQL Azure'
BEGIN
    SET @sessionName = @dbSessionName
    SET @sessionType = 'DATABASE'
    SET @whereClause = N''
    
    IF EXISTS (
        SELECT  *
        FROM    sys.database_event_session_events ese
        JOIN    sys.database_event_sessions es
        ON      ese.event_session_id = es.event_session_id
        WHERE   es.name = @sessionName
                AND ese.name = 'rpc_completed'
    )
    BEGIN
        SET @dropEvent = 1
    END
END
ELSE
BEGIN
    SET @sessionName = @databaseName
    SET @sessionType = 'SERVER'
    SET @whereClause = N' AND [package0].[equal_i_unicode_string]([sqlserver].[database_name],''' + @databaseName + ''')'
    
    IF EXISTS (
        SELECT  *
        FROM    sys.server_event_session_events ese
        JOIN    sys.server_event_sessions es
        ON      ese.event_session_id = es.event_session_id
        WHERE   es.name = @sessionName
                AND ese.name = 'rpc_completed'
    )
    BEGIN
        SET @dropEvent = 1
    END
END

IF (@dropEvent = 1)
BEGIN
    SET @statement = N'
            ALTER EVENT SESSION ' + QUOTENAME(@sessionName) + ' ON ' + @sessionType + '
            DROP EVENT sqlserver.rpc_completed'

    EXEC sp_executesql @statement
END

SET @statement = N'
        ALTER EVENT SESSION ' + QUOTENAME(@sessionName) + ' ON ' + @sessionType + '
        ADD EVENT sqlserver.rpc_completed
        (
            SET collect_statement=(' + CONVERT(VARCHAR(1), @includeStatement) + ')
            ACTION(package0.event_sequence,sqlserver.context_info)
            WHERE ((
                        [package0].[greater_than_uint64]([cpu_time],(0))
                     OR [package0].[greater_than_uint64]([writes],(0))
                     OR [package0].[greater_than_uint64]([physical_reads],(0))
                   )' + @whereClause + ')
        )'

EXEC sp_executesql @statement