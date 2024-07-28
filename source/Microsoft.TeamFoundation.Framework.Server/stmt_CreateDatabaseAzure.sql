-- Creates a database on SQL Azure
-- Parameters:
-- @databaseName NVARCHAR   - the name of the database to be created
-- @serviceObjective        - the intial service objective of the database
-- @maxSizeInGB             - the maximum size of the db
-- @collation               - the collation of the database

DECLARE @createStatement NVARCHAR(MAX) = 'CREATE DATABASE ' + QUOTENAME(@databaseName)

IF @collation IS NOT NULL
BEGIN
    IF EXISTS (SELECT 1 FROM sys.fn_helpcollations() WHERE name = @collation)
    BEGIN
        SET @createStatement += ' COLLATE ' + @collation
    END
    ELSE
    BEGIN
        RAISERROR('Collation "%s" is not supported', 18, 0, @collation)
    END
END

-- Build the options list
DECLARE @options NVARCHAR(MAX) = ''
SET @options = ' SERVICE_OBJECTIVE = ' + QUOTENAME(@serviceObjective, '''')

IF @maxSizeInGB > 0
BEGIN
    SET @options += ', MAXSIZE = ' + CONVERT(NVARCHAR(12), @maxSizeInGB) + 'GB'
END

-- Add the options to the end of the create statement
SET @createStatement += ' ( ' + @options + ' )'

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = @databaseName)
BEGIN
    BEGIN TRY
        EXEC sp_executesql @createStatement
    END TRY
    BEGIN CATCH
        -- Throw unless we get a db already exists error
        IF ERROR_NUMBER() != 1801 -- 1801 = Database already exists
            THROW

        RAISERROR('The database already exists. Waiting for the operation to complete.', 10, 0) WITH NOWAIT
    END CATCH

    DECLARE @attempts AS INTEGER = 1;
    DECLARE @maxAttempts AS INTEGER = 120; -- try for 10 minutes and then fail
    DECLARE @state AS INTEGER = 0
    DECLARE @errorCode AS INTEGER
    DECLARE @errorDescription AS NVARCHAR(2048)

    -- Loop until SQL marks the db creation operation as completed
    WHILE (@state = 0 OR @state = 1) -- 0 = Pending, 1 = In progress
    BEGIN
        SET @attempts += 1
        SELECT TOP  1 
                    @state = state,
                    @errorCode = error_code,
                    @errorDescription = error_desc
        FROM        sys.dm_operation_status 
        WHERE       major_resource_id = @databaseName
        AND         operation = 'CREATE DATABASE'
        AND         error_code != 1801 -- ignore previous Database already exists errors
        ORDER BY    start_time DESC

        IF @state = 2 -- Completed
        BEGIN
            RAISERROR('Successfully created the database.', 10, 0) WITH NOWAIT
            BREAK
        END

        IF @state = 3 OR @state = 4 -- 3 = Failure, 4 = Canceled
        BEGIN
            RAISERROR(@errorDescription, 16, @state)
            RETURN
        END

        if @attempts >= @maxAttempts
        BEGIN
            RAISERROR('Timed out waiting for database to be available', 16, 1)
            RETURN
        END
        
        RAISERROR('Waiting for 5 seconds.', 10, 0) WITH NOWAIT
        WAITFOR DELAY '00:00:05'
    END
END
ELSE
BEGIN
    RAISERROR('The database already exists. Returning.', 10, 0) WITH NOWAIT
END