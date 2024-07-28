--
-- Query all active requests starts before @datetimeSince
--
-- Parameters:
--   DATETIME @datetimeSince
--

SELECT  c.session_id,
        s.status as session_status,
        r.status as request_status,
        c.connect_time,
        c.connection_id,
        c.parent_connection_id,
        s.host_name,
        s.login_time,
        s.login_name,
        r.start_time,
        r.cpu_time,
        r.total_elapsed_time,
        -- Get the line of stmt we are blocking on
        SUBSTRING(t.text, 1 + r.statement_start_offset / 2,
        CASE WHEN (r.statement_end_offset - r.statement_start_offset) > 0
            THEN (r.statement_end_offset - r.statement_start_offset) / 2
            ELSE 255 -- no more than 255 characters
        END ) AS stmt,
        -- We are generating header for each sproc now. We need to skip it to display useful information.
        CASE
            WHEN CHARINDEX('-- Hash:', t.text) > 0 THEN SUBSTRING(t.Text, CHARINDEX('-- Hash:', t.text) + 51, 255)
            ELSE SUBSTRING(t.text, 1, 255)
        END AS content  -- only get the first 255 characters
FROM    sys.dm_exec_sessions s WITH (NOLOCK)
JOIN    sys.dm_exec_connections c WITH (NOLOCK)
ON      s.session_id = c.session_id
JOIN    sys.dm_exec_requests r WITH (NOLOCK)
ON      s.session_id = r.session_id
OUTER APPLY sys.dm_exec_sql_text(c.most_recent_sql_handle) t
WHERE   s.is_user_process = 1
AND     s.database_id = DB_ID()
AND     s.session_id <> @@SPID
AND     r.start_time < @datetimeSince
