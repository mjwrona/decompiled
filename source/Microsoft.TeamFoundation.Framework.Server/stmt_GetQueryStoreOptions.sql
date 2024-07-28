DECLARE @productVersion INT = CONVERT(INT, LEFT(CONVERT(SYSNAME, SERVERPROPERTY('ProductVersion')), CHARINDEX('.', CONVERT(SYSNAME, SERVERPROPERTY('ProductVersion')), 0) - 1))

-- SQL Server Query Store is supported in SQL Azure 12 and later, and SQL Server 2016 CTP 2 and later
IF ((CONVERT(SYSNAME, SERVERPROPERTY('Edition')) LIKE '%SQL Azure%' AND @productVersion >= 12)
    OR @productVersion >= 13)
BEGIN
    SELECT  DB_NAME() as database_name,
            desired_state, 
            actual_state, 
            readonly_reason, 
            current_storage_size_mb, 
            flush_interval_seconds, 
            interval_length_minutes, 
            max_storage_size_mb, 
            stale_query_threshold_days, 
            max_plans_per_query, 
            query_capture_mode, 
            size_based_cleanup_mode
    FROM    sys.database_query_store_options
END
ELSE
BEGIN
    PRINT 'Unable to get Query Store options because it is not supported by this version of SQL Server'
END
