-- If we are connecting to the availability group listener, use IP address to figure out which AG we are in
-- we are trying to check availability group from server level

IF (CONVERT(INT, LEFT(CONVERT(SYSNAME, SERVERPROPERTY('ProductVersion')), CHARINDEX('.', CONVERT(SYSNAME, SERVERPROPERTY('ProductVersion')), 0) - 1)) < 11)
BEGIN
    -- SqlAlwaysOn was implemented in SQL Server 2012: https://msdn.microsoft.com/en-us/library/ff878538.aspx
    -- availability_group_listeners is not available in the earlier version of SQL Server
    SELECT  NULL Node,
            NULL Role,
            NULL Health,
            NULL  DatabaseCount
    FROM    sys.objects
    WHERE   0 <> 0
END
ELSE
BEGIN
    -- We are using dynamic sql because CONNECTIONPROPERTY is not available in SQL Server 2005.
    -- We are running this statement during verification in on-prem TFS.
    DECLARE @stmt NVARCHAR(MAX)
    SET @stmt =
   'SELECT  ar.replica_server_name Node,
            ar_state.role Role,
            ar_state.synchronization_health Health,
            COUNT(dc.database_name) DatabaseCount
    FROM    sys.availability_groups ag
    JOIN    sys.availability_replicas ar
    ON      ag.group_id = ar.group_id
    JOIN    sys.availability_group_listeners as listeners
    ON      listeners.group_id = ag.group_id
    LEFT JOIN sys.dm_hadr_availability_replica_states AS ar_state 
    ON      ar.replica_id = ar_state.replica_id
    LEFT JOIN sys.availability_databases_cluster dc
    ON      ar.group_id = dc.group_id
    WHERE   listeners.ip_configuration_string_from_cluster LIKE ''%''''IP Address: '' +  CONVERT(VARCHAR(40), CONNECTIONPROPERTY(''local_net_address'')) + ''''''%''
    GROUP BY ar.replica_server_name, ar_state.role, ar_state.synchronization_health'
EXEC (@stmt)
END
