--
-- Queries the blocking session (including zombie session which has no active requests)
--
-- Parameters:
--   @blockedSessionId
--   @blockingMilliSeconds

-- If the @currentSessionId is blocked, find the head of the blocking chain

;WITH BlockingList AS (
    -- Starting from the queried session itself
    SELECT      c.session_id,
                s.status as session_status,
                r.status as request_status,
                r.blocking_session_id
    FROM		sys.dm_exec_requests r WITH (NOLOCK)
    JOIN		sys.dm_exec_connections c WITH (NOLOCK)
    ON			r.session_id = c.session_id
    JOIN        sys.dm_exec_sessions s WITH (NOLOCK)
    ON          r.session_id = s.session_id
    WHERE       s.is_user_process = 1
    AND         r.session_id = @blockedSessionId
    AND         r.wait_time > @blockingMilliseconds
    AND         r.blocking_session_id > 0

    UNION ALL

    SELECT      c.session_id,
                s.status as session_status,
                r.status as request_status,
                r.blocking_session_id 
    FROM        BlockingList bl
    JOIN        sys.dm_exec_connections c WITH (NOLOCK)
    ON		    c.session_id = bl.blocking_session_id
    JOIN        sys.dm_exec_sessions s WITH (NOLOCK)
    ON          s.session_id = c.session_id
    JOIN        sys.dm_exec_requests r WITH (NOLOCK)        -- We can not use outer join in recursive CTE, so we will not directly get the zombie session
    ON          r.session_id = c.session_id
    WHERE       s.is_user_process = 1
    )


SELECT      c.session_id,
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
FROM        sys.dm_exec_connections c WITH (NOLOCK)
JOIN        sys.dm_exec_sessions s WITH (NOLOCK)
ON          s.session_id = c.session_id
LEFT JOIN   sys.dm_exec_requests r WITH (NOLOCK)        -- Left join, can be NULL if it is a blocking zombie session
ON          r.session_id = c.session_id
OUTER APPLY sys.dm_exec_sql_text(c.most_recent_sql_handle) t
WHERE       EXISTS                                      -- If top of tree is an active request blocking_session_id is 0
            (
            SELECT  *
            FROM    BlockingList bl
            WHERE   bl.session_id = c.session_id
            AND     bl.blocking_session_id = 0
            )
            OR
            EXISTS                                      -- If top of tree pointing to a zombie session
            (
            SELECT  *
            FROM    BlockingList bl
            WHERE   bl.blocking_session_id = c.session_id
            AND     bl.blocking_session_id > 0
            AND NOT EXISTS
                (
                SELECT  *                               -- Find the top of the tree (root of blocking chain)
                FROM    BlockingList bl1
                WHERE   bl1.session_id = bl.blocking_session_id
                )
            )