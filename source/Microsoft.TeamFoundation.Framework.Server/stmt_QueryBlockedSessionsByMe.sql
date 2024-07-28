--
-- Queries the blocked session by @blockingSessionId
--
-- Parameters:
--   @blockingSessionId
--   @blockingMilliSeconds
--

;WITH BlockingList AS
    (SELECT a.session_id,
            a.blocking_session_id,
            s.status as session_status,
            a.status as request_status,
            a.start_time,
            a.total_elapsed_time as elapsed_milliseconds,
            a.command,
            a.wait_type,
            a.wait_time,
            a.wait_resource
     FROM	sys.dm_exec_requests a WITH (NOLOCK)
     JOIN   sys.dm_exec_sessions s WITH (NOLOCK)
     ON     s.session_id = a.session_id
     WHERE  s.is_user_process = 1
            AND a.session_id <> @@SPID
            AND	a.blocking_session_id = @blockingSessionId

     UNION ALL

     SELECT a.session_id,
            a.blocking_session_id,
            s.status as session_status,
            a.status as request_status,
            a.start_time,
            a.total_elapsed_time as elapsed_milliseconds,
            a.command,
            a.wait_type,
            a.wait_time,
            a.wait_resource
     FROM	sys.dm_exec_requests a WITH (NOLOCK)
     JOIN   sys.dm_exec_sessions s WITH (NOLOCK)
     ON     s.session_id = a.session_id
     INNER JOIN BlockingList bl
     ON		a.blocking_session_id = bl.session_id
     WHERE  s.is_user_process = 1     
            AND a.session_id <> @@SPID
            AND	a.blocking_session_id > 0
     )

SELECT  bl.session_id,
        bl.blocking_session_id,
        bl.session_status,
        bl.request_status,
        bl.start_time,
        bl.elapsed_milliseconds,
        bl.command,
        bl.wait_type,
        bl.wait_time,
        bl.wait_resource,
        -- We are generating header for each sproc now. We need to skip it to display useful information.
        CASE
            WHEN CHARINDEX('-- Hash:', t.text) > 0 THEN SUBSTRING(t.Text, CHARINDEX('-- Hash:', t.text) + 51, 255)
            ELSE SUBSTRING(t.text, 1, 255)
        END AS content  -- only get the first 255 characters
FROM    BlockingList bl
JOIN    sys.dm_exec_connections c WITH (NOLOCK)
ON      c.session_id = bl.session_id
OUTER APPLY sys.dm_exec_sql_text(c.most_recent_sql_handle) t
WHERE   bl.wait_time >= @blockingMilliseconds   -- Blocked more than @blockingMilliseconds