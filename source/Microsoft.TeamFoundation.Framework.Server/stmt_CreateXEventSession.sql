-- Parameter: @blobStoragePath NVARCHAR(MAX) - Blob storage container URL
-- Parameter: @pathSeparator VARCHAR(1) - Path separator character ('/' for SQL Azure, '\' for SQL Server)
-- Parameter: @sharedAccessSignature NVARCHAR(MAX) - Blob storage SAS key to allow the session to write to the container
-- Parameter: @xeventsFilename NVARCHAR(MAX) - Filename for the file inside of a blob container(or filesystem for SQL Server) for the xevents.
-- Parameter: @dbSessionName NVARCHAR(MAX) - Name of the session to create. Note this parameter is only used for SQL Azure
SET XACT_ABORT ON

DECLARE @databaseName       NVARCHAR(250) = DB_NAME()
DECLARE @sessionName        NVARCHAR(250)
DECLARE @sessionType        NVARCHAR(8)
DECLARE @statement          NVARCHAR(MAX)
DECLARE @whereClause        NVARCHAR(MAX)
DECLARE @filename           NVARCHAR(MAX) = @blobStoragePath + @pathSeparator + @xeventsFilename
DECLARE @maxRolloverFiles   NVARCHAR(25)

IF CAST(SERVERPROPERTY('Edition') AS SYSNAME) COLLATE DATABASE_DEFAULT = N'SQL Azure'
BEGIN
    -- CREATE MASTER KEY if one was not already created
    IF NOT EXISTS (SELECT * FROM sys.symmetric_keys WHERE symmetric_key_id = 101)
    BEGIN
        SET @statement = N'
                CREATE MASTER KEY'

        EXEC sp_executesql @statement
    END

    IF NOT EXISTS (SELECT * FROM sys.database_credentials WHERE name = @blobStoragePath)
    BEGIN
        SET @statement = N'
                CREATE DATABASE SCOPED CREDENTIAL ' + QUOTENAME(@blobStoragePath) + N'
                WITH IDENTITY = ''SHARED ACCESS SIGNATURE'',
                       SECRET = ''' + REPLACE(@sharedAccessSignature,'''', '''''') + N''''

        EXEC sp_executesql @statement
    END
    ELSE
    BEGIN
        SET @statement = N'
                ALTER DATABASE SCOPED CREDENTIAL ' + QUOTENAME(@blobStoragePath) + N'
                WITH IDENTITY = ''SHARED ACCESS SIGNATURE'',
                       SECRET = ''' + REPLACE(@sharedAccessSignature,'''', '''''') + N''''

        EXEC sp_executesql @statement
    END

    SET @sessionName = @dbSessionName

    IF NOT EXISTS (SELECT * FROM sys.database_event_sessions WHERE name = @sessionName)
    BEGIN
        SET @sessionType = 'DATABASE'
        SET @whereClause = N''
        SET @maxRolloverFiles = N''
    END
END
ELSE
BEGIN
    SET @sessionName = @databaseName

    IF NOT EXISTS (SELECT * FROM sys.server_event_sessions WHERE name = @sessionName)
    BEGIN
        SET @sessionType = 'SERVER'
        SET @whereClause = N' AND [package0].[equal_i_unicode_string]([sqlserver].[database_name],''' + @databaseName + ''')'
        SET @maxRolloverFiles = N',max_rollover_files=(512)'
    END
END

IF (@sessionType IS NOT NULL)
BEGIN
    SET @statement = N'
            CREATE EVENT SESSION ' + QUOTENAME(@sessionName) + ' ON ' + @sessionType + '
            ADD EVENT sqlserver.rpc_completed
            (
                SET collect_statement=(0)
                ACTION(package0.event_sequence,sqlserver.context_info)
                WHERE ((
                            [package0].[greater_than_uint64]([cpu_time],(0))
                         OR [package0].[greater_than_uint64]([writes],(0))
                         OR [package0].[greater_than_uint64]([physical_reads],(0))
                       )' + @whereClause + ')
            )
            ADD TARGET package0.event_file(SET filename=''' + @filename + ''',max_file_size=(2)' + @maxRolloverFiles + ')
            WITH (STARTUP_STATE=OFF)'

    EXEC sp_executesql @statement
END