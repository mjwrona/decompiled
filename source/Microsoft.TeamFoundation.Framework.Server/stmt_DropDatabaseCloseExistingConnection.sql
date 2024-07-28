--
-- Drops specified database.
--
-- Parameters:
--   @databaseName sysname.
DECLARE @stmt NVARCHAR(MAX) = ''
DECLARE @isAzure BIT = CASE WHEN CONVERT(NVARCHAR(MAX), SERVERPROPERTY('Edition')) LIKE '%Azure%' THEN 1 ELSE 0 END

IF EXISTS (SELECT * FROM sys.databases WHERE name = @databaseName)
BEGIN
    -- only call ALTER DATABASE if we are not restoring
    IF DATABASEPROPERTYEX(@databaseName, 'Status') <> 'RESTORING'
    BEGIN
        IF DATABASEPROPERTYEX(@databaseName, 'UserAccess') = 'SINGLE_USER'
        BEGIN
            SELECT @stmt = @stmt + '
            KILL ' + CONVERT(VARCHAR, t.session_id)
            FROM    (SELECT s.session_id
                    FROM    sys.dm_exec_sessions s
                    WHERE   s.database_id = DB_ID(@databaseName)
                            AND s.session_id > 50
                    UNION
                    SELECT  request_session_id
                    FROM    sys.dm_tran_locks l
                    WHERE   resource_database_id = DB_ID(@databaseName)
                            AND l.request_session_id > 50) t

            EXEC sp_executesql @stmt
            SET @stmt = '
            ALTER DATABASE ' + QUOTENAME(@databaseName) + ' SET MULTI_USER WITH ROLLBACK IMMEDIATE;'
            EXEC sp_executesql @stmt
        END

        SET @stmt = '
             USE ' + QUOTENAME(@databaseName) + '
             ALTER DATABASE ' + QUOTENAME(@databaseName) + ' SET SINGLE_USER WITH ROLLBACK IMMEDIATE'
    END

    SET @stmt = @stmt + '
        USE MASTER
        DROP  DATABASE ' + QUOTENAME(@databaseName)
    BEGIN TRY
        EXEC sp_executesql @stmt
    END TRY
    BEGIN CATCH
        -- Throw unless we get a Internal service error
        IF ERROR_NUMBER() = 42019 -- 42019 = AzureOperationFailed (Internal service error)
            RAISERROR('Azure returns internal error. Waiting for the operation to complete.', 10, 0) WITH NOWAIT
        ELSE
            BEGIN
            IF DATABASEPROPERTYEX(@databaseName, 'Status') = 'ONLINE'
            BEGIN
            SET @stmt = '
                ALTER DATABASE ' + QUOTENAME(@databaseName) + ' SET OFFLINE WITH ROLLBACK IMMEDIATE;
                ALTER DATABASE ' + QUOTENAME(@databaseName) + ' SET ONLINE;
                DROP  DATABASE ' + QUOTENAME(@databaseName)
            END
            ELSE
            BEGIN
                SET @stmt = 'DROP  DATABASE ' + QUOTENAME(@databaseName)
            END
            EXEC sp_executesql @stmt
        END
    END CATCH

    IF @isAzure = 1
    BEGIN
        DECLARE @attempts AS INTEGER = 1
        DECLARE @maxAttempts AS INTEGER = 120 -- try for 10 minutes and then fail
        DECLARE @state AS INTEGER = 0
        DECLARE @errorCode AS INTEGER
        DECLARE @errorDescription AS NVARCHAR(2048)

        -- Loop until SQL marks the db deletion operation as completed
        WHILE (@state = 0 OR @state = 1) -- 0 = Pending, 1 = In progress
        BEGIN
            SET @attempts += 1
            SELECT TOP  1 
                        @state = state,
                        @errorCode = error_code,
                        @errorDescription = error_desc
            FROM        sys.dm_operation_status 
            WHERE       major_resource_id = @databaseName
            AND         operation = 'DROP DATABASE'
            AND         error_code != 42019 -- ignore previous Internal service error
            ORDER BY    start_time DESC

            IF @state = 2 -- Completed
            BEGIN
                RAISERROR('Successfully dropped the database.', 10, 0) WITH NOWAIT
                BREAK
            END

            IF @state = 3 OR @state = 4 -- 3 = Failure, 4 = Canceled
            BEGIN
                RAISERROR(@errorDescription, 16, @state)
                RETURN
            END

            if @attempts >= @maxAttempts
            BEGIN
                RAISERROR('Timed out waiting for database to be dropped', 16, 1)
                RETURN
            END
        
            RAISERROR('Waiting for 5 seconds.', 10, 0) WITH NOWAIT
            WAITFOR DELAY '00:00:05'
        END
    END
END
ELSE
BEGIN
    RAISERROR('The database does not exist. Returning.', 10, 0) WITH NOWAIT
END