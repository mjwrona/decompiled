/*
** Executes SQL scripts.
**
** Parameters:
**  @_batches                   typ_SqlBatch2       - batches to execute.
**  @_acquireLock               VARCHAR(64)         - If this param is not empty, then the script begins transaction and acquires an exclusive transaction lock.
**                                                    All statements will be executed in this transaction under the lock.
**  @_inTransaction             BIT                 - Execute batches in a single transaction. Must be set to 1 if _acquireLock is not null or empty string
**  @_serviceVersions           NVARCHAR(MAX)       - An Xml fragment that defines service versions.
**
**  @_testRerunnability         BIT                 - if _testRerunnability param is equal to 1, then this script will execute batches in the following to test rerunnability:
**                                                  1, 1 and 2, 1, 2, and 3, 1, 2, 3, and 4, etc.
**                                                  Note: This parameter is ignored if _acquireLock is not empty string.
**  @_acquireLockMilliseconds   INT                 - if we need to acquire servicing lock, the timeout we allow to acquire a lock
**  @_acquireLockMaxAttempts    INT                 - if we need to acquire servicing lock, the max attempts to acquire the lock
**  @_longRunningThresholdMilliSeconds  INT         - threshold for long running request in milliseconds
**  @_lastAttemptsToKillSessions        INT         - start to kill session in last N attempts of acquiring servicing lock
**  @_isHosted                          BIT         - if it is hosted, login lacks VIEW SERVER STATE permission in on-prem environment
**  Note:
**   - All variables in this scripts starts with underscore to avoid possible collisions with script parameters.
**   - This method uses info messages to communicate with the calling application code, particularly to signal when it is aggressively attempting to aquire a lock
*/
SET XACT_ABORT ON
SET NOCOUNT ON

DECLARE @_batch NVARCHAR(MAX) = ''
DECLARE @_batchIndex INT
DECLARE @_statusMessage NVARCHAR(MAX)

-- First attempt to acquire the lock
DECLARE @_acquireLockAttemptStart DATETIME

-- Time we acquired the current lock
DECLARE @_acquireLockStart DATETIME

-- Milliseconds attempting to acquire the lock
DECLARE @_milliSecondsUnderLock INT

-- Max Milliseconds spent to acuire the lock.
DECLARE @_maxMilliSecondsUnderLock INT

-- Time we start the batch
DECLARE @_batchStartTime DATETIME

DECLARE @_batchMessage VARCHAR(100)

SET @_statusMessage = '@SPID=' + CONVERT(VARCHAR(6), @@SPID)
RAISERROR (@_statusMessage, 0, 231) WITH NOWAIT

SELECT  @_acquireLock = COALESCE(@_acquireLock, '')

IF (@_acquireLock <> '' AND @_inTransaction = 0)
    RAISERROR('if @_acquireLock is not empty, @_inTransaction must be 1', 16, -1)

IF (@_inTransaction = 1)
    BEGIN TRAN

IF (@_acquireLock <> '')
BEGIN
    DECLARE @_result                INT = -1
    DECLARE @_attempt               INT = 1
    DECLARE @_noWaitAttempt         INT = 1
    DECLARE @_shortLockName         NVARCHAR(32) = @_acquireLock

    SET @_statusMessage = 'Trying to Acquire Lock #1:' + CONVERT(VARCHAR, GETUTCDATE(), 109)
    PRINT @_statusMessage

    SET @_acquireLockAttemptStart = GETUTCDATE()


	-- Bug 648253: We found query for dm_tran_locks may cause parallel deployment of dataimport service to fail to create new database.
	-- To avoid this issue, before we start detecting blocking, try to get app lock without wait (and this should always work with new config db).
	EXEC @_result = sp_getapplock  @Resource = @_acquireLock, @LockMode = 'Exclusive', @LockTimeout = 0

	IF @_result < 0
    BEGIN
		-- Try to acquire a lock @_acquireLockMaxAttempts times with @_acquireLockMilliseconds lock timeout. We will sleep for 15 seconds between attempts.
		WHILE @_attempt <= @_acquireLockMaxAttempts
		BEGIN

			SET @_statusMessage = ''

			-- check long running request for hosted only, and make sure it is not in "kill session" mode yet
			IF @_isHosted = 1
			IF @_attempt <= @_acquireLockMaxAttempts - @_lastAttemptsToKillSessions
			BEGIN
				-- Checking if there are any long running requests holding the application lock, if there is at least one such request, we skip acquiring servicing lock
				-- to avoid more blocking and wait for 15 seconds (since it is unlikely we can get the application lock very soon)
				SELECT  TOP 1
						@_statusMessage = 'Longest running request holding application lock:' +
						'@attempt=' + CONVERT(VARCHAR(10), @_attempt) +                 -- int
						'@spid=' + CONVERT(VARCHAR(10), l.request_session_id) +         -- int
						'@oType=' + l.request_owner_type +                              -- nvarchar(60)
						'@mode=' + l.request_mode +                                     -- nvarchar(60)
						'@dbId=' + CONVERT(VARCHAR(10), l.resource_database_id) +       -- int
						'@desc=' + REPLACE(RTRIM(l.resource_description), '@', ' ') +   -- nvarchar(256)
						'@status=' + a.status +                                         -- nvarchar(30)
						'@blkSpid=' + CONVERT(VARCHAR(10), a.blocking_session_id) +     -- smallint
						'@startTime= ' + CONVERT(VARCHAR, a.start_time, 120) +          -- datetime (20)
						'@duration=' + CONVERT(VARCHAR(10), a.total_elapsed_time) +     -- int
						'@hostName=' + s.host_name +                                    -- nvarchar(128)
						'@loginName=' + s.login_name +                                  -- nvarchar(128)
						-- We are generating header for each sproc now. We need to skip it to display useful information.
						'@content=' +
						CASE
							WHEN CHARINDEX('-- Hash:', t.text) > 0 THEN REPLACE(SUBSTRING(t.Text, CHARINDEX('-- Hash:', t.text) + 51, 255), '@', ' ') -- @ is delimiter
							ELSE REPLACE(SUBSTRING(t.text, 1, 255), '@', ' ')           -- @ is delimiter
						END +                                                           -- only get the first 255 characters
						'@stmt=' +
						-- Get the line of stmt we are blocking on
						CASE WHEN a.statement_end_offset IS NOT NULL
							THEN  REPLACE(SUBSTRING(t.text, 1 + a.statement_start_offset / 2, 255), '@', ' ')
							-- only get the first 255 characters
							ELSE NULL
						END
				FROM	sys.dm_tran_locks l
				JOIN    sys.dm_exec_sessions  s WITH (NOLOCK)
				ON      l.request_session_id = s.session_id
						AND l.request_status = 'GRANT'
						AND	l.resource_type = 'APPLICATION'
						AND l.resource_description like '%[[]' + @_shortLockName + ']%'
						AND l.resource_database_id = DB_ID()
				JOIN    sys.dm_exec_requests a WITH (NOLOCK)
				ON      l.request_session_id = a.session_id
						AND a.total_elapsed_time > @_longRunningThresholdMilliSeconds            -- requests running longer than 60 seconds
				JOIN	sys.dm_exec_connections c WITH (NOLOCK)
				ON		l.request_session_id = c.session_id
				OUTER APPLY sys.dm_exec_sql_text(c.most_recent_sql_handle) t
				ORDER BY a.total_elapsed_time DESC
			END

			IF(@_statusMessage <> '')
			BEGIN
				-- If there are long running requests, log the info and skip acquiring with timeout
				RAISERROR('%s', 0, 231, @_statusMessage) WITH NOWAIT

				-- Result message
				SET @_statusMessage = 'Servicing lock attempt ' + CONVERT(VARCHAR(10), @_attempt) + ' result: Long running request detected.'
				RAISERROR (@_statusMessage, 0, 231) WITH NOWAIT
			END
			ELSE -- If there are no long running requests, proceed to acquire lock (or it is one of the last 8 attempts)
			BEGIN
				SET @_acquireLockStart = GETUTCDATE()

				-- This status message is parsed by the application, so the format must match the parsing logic in SqlScriptResourceComponent
				-- The application will attempt to kill any sessions still running after some part of the lock time has passed
				SET @_statusMessage = 'Acquiring servicing lock: @attempt=' + CONVERT(VARCHAR(10), @_attempt) + '@timestamp=' + CONVERT(VARCHAR, GETUTCDATE(), 109)
				RAISERROR (@_statusMessage, 0, 231) WITH NOWAIT

				-- For the last @_lastAttemptsToKillSessions attempt, we will kill the blocking sessions by the end of @_acquireLockMilliseconds,
				-- add 15 more seconds to allow sessions to be killed
				if @_attempt = @_acquireLockMaxAttempts - @_lastAttemptsToKillSessions + 1
				BEGIN
				    -- On a busy server, it can take 10+ seconds to query sys.dm_tran_locks, let us sleep for 5 seconds, while
					-- SqlScriptResourceComponent is killing sessions which have an exclusive lock on TfsDb.
				    WAITFOR DELAY '00:00:05'
				    RAISERROR (@_statusMessage, 0, 231) WITH NOWAIT
					SET @_acquireLockMilliseconds = @_acquireLockMilliseconds + 15 * 1000
				END

				EXEC @_result = sp_getapplock @Resource = @_acquireLock, @LockMode = 'Exclusive', @LockTimeout = @_acquireLockMilliseconds

				if @_result >= 0
					BREAK

				-- Result message
				SET @_statusMessage = 'Servicing lock attempt ' + CONVERT(VARCHAR(10), @_attempt) + ' result: Timed out while acquiring the lock.'
				RAISERROR (@_statusMessage, 0, 231) WITH NOWAIT
			END

			SET @_attempt = @_attempt + 1

			-- Sleep for 15 seconds, test acquire application lock every second, check if it can be granted
			SET @_noWaitAttempt = 1
			WHILE @_noWaitAttempt <= 15
			BEGIN
				WAITFOR DELAY '00:00:01';
				EXEC @_result = sp_getapplock  @Resource = @_acquireLock, @LockMode = 'Exclusive', @LockTimeout = 0
				-- We luckily detect a moment without block, acquire lock immediately
				IF @_result >= 0
					BREAK

				SET @_noWaitAttempt = @_noWaitAttempt + 1
			END

			IF @_result >= 0
				BREAK
		END
	END

    IF @_result < 0
    BEGIN
        ROLLBACK
        RAISERROR('%%error="800070";%%:Failed to acquire an exclusive servicing lock', 16, -1)
        RETURN
    END

    SET @_statusMessage = 'Lock acquired:' + CONVERT(VARCHAR, GETUTCDATE(), 109) + ' time to acquire lock:' + CONVERT(VARCHAR, DateDiff(ms, @_acquireLockAttemptStart, GETUTCDATE()), 109) + ' ms'
    RAISERROR (@_statusMessage, 0, 231) WITH NOWAIT
END

DECLARE @_maxBatchIndex INT = 2147483647 -- MAXINT

IF @_testRerunnability = 1 AND @_acquireLock = ''
BEGIN
    SET @_maxBatchIndex = 1
END

Start:
DECLARE batchCursor CURSOR LOCAL FAST_FORWARD FOR
    SELECT  BatchIndex,
            COALESCE(Batch, BatchN)
    FROM    @_batches
    ORDER BY BatchIndex

OPEN batchCursor

FETCH   NEXT FROM batchCursor
INTO    @_batchIndex,
        @_batch

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @_batchStartTime = GETUTCDATE()

    SET @_batchMessage = '@b=' + CONVERT(VARCHAR(10), @_batchIndex) + '@bt=' + CONVERT(VARCHAR(25), @_batchStartTime, 21)
    RAISERROR (@_batchMessage, 0, 231)

    IF @_testRerunnability = 1
    BEGIN
        -- Add a line to the batch. The chance that SQL Server will reuse execution plan will be smaller in this case.
        SET @_batch = @_batch + CHAR(10) + CHAR(13) + N'-- ' + CONVERT(NVARCHAR(10), @_batchStartTime)
    END

    EXEC sp_ExecuteSql @_batch
    IF @@ERROR <> 0  --- Skip rest of the script since we encountered an error
    BEGIN
        IF @_inTransaction = 1
        BEGIN
            ROLLBACK
        END
        RETURN
    END

    IF @_batchIndex = @_maxBatchIndex
    BEGIN
        CLOSE batchCursor
        DEALLOCATE batchCursor
        SET @_batchMessage = '============= Iteration with _maxBatch = ' + CONVERT(VARCHAR(10), @_maxBatchIndex) + ' completed. ============='
        RAISERROR (@_batchMessage, 10, 0) WITH NOWAIT
        SET @_maxBatchIndex = @_maxBatchIndex + 1
        GOTO start
    END

    FETCH   NEXT FROM batchCursor
    INTO    @_batchIndex,
            @_batch
END
-- Send one more message to compute the execution time of last batch
SET @_batchMessage = '@b=' + CONVERT(VARCHAR(10), @_batchIndex + 1) + '@bt=' + CONVERT(VARCHAR(25), GETUTCDATE(), 21)
RAISERROR (@_batchMessage, 0, 231)

CLOSE batchCursor
DEALLOCATE batchCursor

IF @_serviceVersions IS NOT NULL
BEGIN
    SET @_statusMessage = 'Begin set service version:' + CONVERT(VARCHAR, GETUTCDATE(), 109)
    PRINT @_statusMessage

    -- Set service versions
    DECLARE @_stmt NVARCHAR(MAX) = N''
    DECLARE @_serviceVersionsXml XML = CONVERT(XML, @_serviceVersions)

    SELECT  @_stmt = @_stmt + 'EXEC prc_SetServiceVersion ' +
            '@serviceName = ''' + svc.value('@name', 'VARCHAR(64)') + ''', ' +
            '@version = ' + svc.value('@version', 'VARCHAR(10)') + ', ' +
            '@minVersion = ' + svc.value('@minVersion', 'VARCHAR(10)') + CHAR(13)
    FROM    @_serviceVersionsXml.nodes('/svc') tbl(svc)

    EXEC(@_stmt)

    SET @_statusMessage = 'End set service version:' + CONVERT(VARCHAR, GETUTCDATE(), 109)
    PRINT @_statusMessage
END

IF @_inTransaction = 1
BEGIN
    COMMIT
END
IF @_acquireLock <> ''
BEGIN
    --Time spent under the lock
    SET @_milliSecondsUnderLock = DATEDIFF(ms, @_acquireLockStart,  GETUTCDATE())

    if @_milliSecondsUnderLock >= 10000
        SET @_statusMessage = 'WARNING!! Time under lock exceeds 10 seconds - Time: ' + CONVERT(VARCHAR, @_milliSecondsUnderLock, 109) + ' ms'
    ELSE
        SET @_statusMessage = 'Time under lock: ' + CONVERT(VARCHAR, @_milliSecondsUnderLock, 109)  + ' ms'

    PRINT @_statusMessage
END

IF @@TRANCOUNT > 0
BEGIN
    -- Script failed
    ROLLBACK
END
