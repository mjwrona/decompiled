-- Get replication links from master database. It will show all the geo replication databases on this server.

IF (CAST(SERVERPROPERTY('Edition') AS SYSNAME) COLLATE DATABASE_DEFAULT = N'SQL Azure')
BEGIN
    SELECT  link_guid,
            partner_server,
            partner_database,
            replication_state_desc,
            [role],
            secondary_allow_connections
    FROM    sys.geo_replication_links
END
ELSE
BEGIN
    SELECT  NULL AS link_guid,
            NULL AS partner_server,
            NULL AS partner_database,
            NULL AS replication_state_desc,
            NULL AS [role],
            NULL AS secondary_allow_connections
    WHERE   1 = 2
END
