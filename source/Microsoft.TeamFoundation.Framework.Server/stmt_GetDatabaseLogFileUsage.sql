SELECT	[Log File(s) Size (KB)] / 1024 AS LogFileSizeMB,
        [Log File(s) Used Size (KB)] / 1024 AS LogFileUsedMB,
        ([Log File(s) Size (KB)] - [Log File(s) Used Size (KB)]) / 1024 AS LogFileAvailableMB
FROM
(
    SELECT  *
    FROM    sys.dm_os_performance_counters
    WHERE   instance_name = DB_NAME()
            AND counter_name IN ( 
                'Log File(s) Size (KB)', 
                'Log File(s) Used Size (KB)' )
) perf
PIVOT
(
    MAX(cntr_value)
    FOR counter_name IN ( 
        [Log File(s) Size (KB)], 
        [Log File(s) Used Size (KB)] )
) perf_pivot
