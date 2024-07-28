--
-- Queries the sessions which holds/is blocked by the application lock
--
-- Parameters:
--   @acquireLock
--   @lockRequestStatus

DECLARE @shortLockName  NVARCHAR(32) = @acquireLock

SELECT  l.request_session_id,
        l.request_status,
        l.resource_type,
        l.request_mode,
        l.request_owner_type,
        l.resource_description,
        l.resource_database_id,
        a.status,
        a.blocking_session_id,
        a.start_time,
        a.total_elapsed_time,
        -- We are generating header for each sproc now. We need to skip it to display useful information.
        CASE
            WHEN CHARINDEX('-- Hash:', t.text) > 0 THEN REPLACE(SUBSTRING(t.Text, CHARINDEX('-- Hash:', t.text) + 51, 255), '@', ' ') -- @ is delimiter
            ELSE REPLACE(SUBSTRING(t.text, 1, 255), '@', ' ')           -- @ is delimiter
        END AS content,
        -- Get the line of stmt we are blocking on
        CASE WHEN a.statement_end_offset IS NOT NULL
            THEN  REPLACE(SUBSTRING(t.text, 1 + a.statement_start_offset / 2, 255), '@', ' ')
            -- only get the first 255 characters
            ELSE NULL
        END AS stmt
FROM	sys.dm_tran_locks l
JOIN    sys.dm_exec_sessions  s WITH (NOLOCK)
ON      l.request_session_id = s.session_id
LEFT JOIN  sys.dm_exec_requests a WITH (NOLOCK)
ON      l.request_session_id = a.session_id
JOIN	sys.dm_exec_connections c WITH (NOLOCK)
ON      l.request_session_id = c.session_id
OUTER APPLY sys.dm_exec_sql_text(c.most_recent_sql_handle) t
WHERE	l.resource_database_id = DB_ID()
AND     l.request_status = @lockRequestStatus
AND     l.resource_type = 'APPLICATION'
AND     l.resource_description like '%[[]' + @shortLockName + ']%'
ORDER BY a.total_elapsed_time DESC