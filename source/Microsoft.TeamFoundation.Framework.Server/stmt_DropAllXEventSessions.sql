SET XACT_ABORT ON

DECLARE @sessionName   NVARCHAR(250)
DECLARE @statement     NVARCHAR(MAX)

IF CAST(SERVERPROPERTY('Edition') AS SYSNAME) COLLATE DATABASE_DEFAULT <> N'SQL Azure'
BEGIN
    IF EXISTS (SELECT name FROM sys.server_event_sessions WHERE name = @configurationName)
    BEGIN
        SET @statement = N'
                DROP EVENT SESSION ' + QUOTENAME(@configurationName) + ' ON SERVER'

        EXEC sp_executesql @statement
    END

    DECLARE SessionCursor CURSOR LOCAL FAST_FORWARD FOR
    SELECT name FROM sys.server_event_sessions WHERE name LIKE @collectionPrefix + '________-____-____-____-____________'

    OPEN SessionCursor
    FETCH NEXT FROM SessionCursor INTO @sessionName

    WHILE (@@FETCH_STATUS = 0)
    BEGIN
        SET @statement = N'
                DROP EVENT SESSION ' + QUOTENAME(@sessionName) + ' ON SERVER'

        EXEC sp_executesql @statement

        FETCH NEXT FROM SessionCursor INTO @sessionName
    END
END