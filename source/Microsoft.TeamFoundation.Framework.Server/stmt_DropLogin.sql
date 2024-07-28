/*
** Drops SQL login specified by @loginName parameter
*/
--DECLARE @loginName SYSNAME
DECLARE @stmt NVARCHAR(MAX) = N''

-- Maximum number of retries before throwing an error
DECLARE @max_attempts SMALLINT = 150

-- Amount of time to wait between retries
DECLARE @delay NVARCHAR(12) = '00:00:00.100'

DECLARE @dropped BIT = 0
DECLARE @attempt INT = 1

WHILE (@dropped != 1)
BEGIN
    -- Try dropping the login
    BEGIN TRY    
        SET @stmt = N'DROP LOGIN ' + QUOTENAME(@loginName)
        EXEC sp_executesql @stmt
        SET @dropped = 1
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER() <> 15151
        BEGIN
            
            -- Close all sessions for the login
            if @attempt = 1
            BEGIN
                SET @stmt = N'ALTER LOGIN ' + QUOTENAME(@loginName) + ' DISABLE'
                EXEC sp_executesql @stmt

                SET @stmt  = N''
                SELECT  @stmt = 'KILL ' + CONVERT(VARCHAR(10), session_id) + '
                '
                FROM    sys.dm_exec_sessions
                WHERE   login_name = @loginName

                EXEC sp_executesql @stmt
            END

            -- Retry until the max attempts are hit, then throw the original exception
            if @attempt = @max_attempts
                THROW

            -- Wait between attempts
            WAITFOR DELAY @delay
            SET @attempt = @attempt + 1
        END
    END CATCH
END
