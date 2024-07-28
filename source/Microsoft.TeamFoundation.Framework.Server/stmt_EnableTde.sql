-- Copyright (c) Microsoft Corporation.  All rights reserved.

DECLARE @productVersion INT = CONVERT(INT, LEFT(CONVERT(SYSNAME, SERVERPROPERTY('ProductVersion')), CHARINDEX('.', CONVERT(SYSNAME, SERVERPROPERTY('ProductVersion')), 0) - 1))

-- Azure SQL TDE is supporeted in SQL Azure 12 and later
IF (CONVERT(SYSNAME, SERVERPROPERTY('Edition')) LIKE '%SQL Azure%' AND @productVersion >= 12)
BEGIN
    DECLARE @databaseName SYSNAME = DB_NAME()
    DECLARE @statement NVARCHAR(MAX) = 'ALTER DATABASE ' + QUOTENAME(@databaseName) + ' SET ENCRYPTION ON'

    BEGIN TRY
        EXEC sp_executesql @statement
    END TRY
    BEGIN CATCH
        DECLARE @errorMessage NVARCHAR(4000)
        DECLARE @errorSeverity INT
        DECLARE @errorState INT
        DECLARE @errorNumber INT

        SELECT @errorMessage    = ERROR_MESSAGE(),
                @errorSeverity   = ERROR_SEVERITY(),
                @errorState      = ERROR_STATE(),
                @errorNumber     = ERROR_NUMBER()

        RAISERROR('Failed to enable TDE on database %s. The following error was reported: Msg %d, Level %d, State %d %s', 16, 0,
            @databaseName,
            @errorNumber,
            @errorSeverity,
            @errorState,
            @errorMessage)
    END CATCH
END
