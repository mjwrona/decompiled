-- Query geo replication from database
IF (CAST(SERVERPROPERTY('Edition') AS SYSNAME) COLLATE DATABASE_DEFAULT = N'SQL Azure')
BEGIN
    SELECT  link_guid,
            partner_server,
            partner_database,
            last_replication,
            replication_lag_sec,
            replication_state_desc,
            [role],
            secondary_allow_connections
    FROM    sys.dm_geo_replication_link_status
END
ELSE
BEGIN
    SELECT  NULL AS link_guid,
            NULL AS partner_server,
            NULL AS partner_database,
            NULL AS last_replication,
            NULL AS replication_lag_sec,
            NULL AS replication_state_desc,
            NULL AS [role],
            NULL AS secondary_allow_connections
    WHERE   1 = 2
END