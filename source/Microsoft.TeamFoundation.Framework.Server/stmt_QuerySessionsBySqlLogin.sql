--
-- Query all the sessions owned by specified login
--
-- Parameters:
--   @loginName
--

SELECT  session_id,
        status,
        login_time,
        login_name,
        host_name,
        host_process_id,
        program_name
FROM    sys.dm_exec_sessions
WHERE   login_name = @loginName