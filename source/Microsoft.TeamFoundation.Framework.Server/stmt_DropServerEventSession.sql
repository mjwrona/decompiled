SET XACT_ABORT ON

DECLARE @statement     NVARCHAR(MAX)

IF EXISTS (SELECT * FROM sys.server_event_sessions WHERE name = @sessionName)
BEGIN
    SET @statement = N'DROP EVENT SESSION ' + QUOTENAME(@sessionName) + ' ON SERVER'

    EXEC sp_executesql @statement
END