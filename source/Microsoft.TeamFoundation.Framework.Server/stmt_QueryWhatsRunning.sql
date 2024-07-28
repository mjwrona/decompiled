--
--  Query information about what's currently running on a given SQL database. 
--  This is replicated version of prc_QueryWhatsRunning
--
SELECT  a.session_id, 
        DATEDIFF(ss, a.Start_Time, GETDATE()) as seconds,
        DB_NAME(s.database_id) as databaseName,
        CAST((a.total_elapsed_time / 1000.0) AS FLOAT) as elapsed_time,
        a.command,
        a.blocking_session_id,
        -- We are generating header for each sproc now. We need to skip it to display useful information.
        CASE
            WHEN CHARINDEX('-- Hash:', b.text) > 0 THEN SUBSTRING(b.Text, CHARINDEX('-- Hash:', b.text) + 51, 255)
            ELSE SUBSTRING(b.text, 1, 255)
        END [text],-- only get the first 255 characters
        SUBSTRING(b.text, 1 + a.statement_start_offset / 2, 
            CASE WHEN (a.statement_end_offset - a.statement_start_offset) > 0 
                THEN (a.statement_end_offset - a.statement_start_offset) / 2  
                ELSE 255 -- no more than 255 characters 
            END ) AS stmt,
        a.wait_type, 
        a.wait_time,
        a.last_wait_type,
        a.wait_resource,
        a.reads,
        a.writes,
        a.logical_reads,
        a.cpu_time,
        a.granted_query_memory,
        m.requested_memory_kb,
        m.max_used_memory_kb,
        m.dop,
        c.query_plan as query_plan
FROM    sys.dm_exec_requests a WITH (NOLOCK)
JOIN    sys.dm_exec_sessions s WITH (NOLOCK)
ON      s.session_id = a.session_id
OUTER APPLY sys.dm_exec_sql_text(a.SQL_HANDLE) b
LEFT JOIN sys.dm_exec_query_memory_grants m WITH (NOLOCK)
ON      m.session_id = a.session_id
        AND m.request_id = a.request_id 
OUTER APPLY sys.dm_exec_text_query_plan (a.plan_handle, a.statement_start_offset, a.statement_end_offset) c 
WHERE   s.is_user_process = 1
        AND a.session_id <> @@spid
ORDER BY a.Start_Time  
