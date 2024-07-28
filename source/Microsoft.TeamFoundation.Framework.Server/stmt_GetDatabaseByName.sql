/*
** Gets attribute for the specified db
** Parameters:
**  @databaseName - sysname
*/
DECLARE @stmt NVARCHAR(MAX)
SET @stmt = '

    IF (CAST(SERVERPROPERTY(''Edition'') AS SYSNAME) COLLATE DATABASE_DEFAULT = N''SQL Azure'')
    BEGIN
        SELECT  NULL as ag_name,
                NULL as agl_dns_name,
                NULL as agl_port,
                NULL as agl_ip_config,
                d.name,
                d.database_id,
                owner_sid,
                NULL as owner,
                DATEADD(second, DATEDIFF(second, GETDATE(), GETUTCDATE()), d.create_date) AS utcCreateDate,
                compatibility_level,
                collation_name,
                user_access,
                is_read_only,
                is_auto_close_on,
                is_auto_shrink_on,
                state,
                snapshot_isolation_state,
                is_read_committed_snapshot_on,
                recovery_model,
                is_broker_enabled,
                is_fulltext_enabled,
                is_parameterization_forced,
                NULL as mirroring_guid,
                d.is_encrypted
        FROM    sys.databases d
        WHERE   d.name = @databaseName
    END
    ELSE
    BEGIN

        DECLARE @agi TABLE (
            database_id     INT PRIMARY KEY,
            ag_name         SYSNAME,
            agl_dns_name    NVARCHAR(63),
            agl_port        INT,
            agl_ip_config   NVARCHAR(MAX)
        )

        IF (OBJECT_ID(''sys.dm_hadr_database_replica_states'', ''V'') IS NOT NULL)
        BEGIN
            BEGIN TRY
                INSERT  @agi(database_id,
                             ag_name,
                             agl_dns_name,
                             agl_port,
                             agl_ip_config)
                SELECT  database_id,
                        ag_name,
                        agl.dns_name,
                        agl.port,
                        agl.ip_configuration_string_from_cluster 
                FROM    sys.dm_hadr_name_id_map m
                JOIN    sys.dm_hadr_database_replica_states drs
                ON      m.ag_id = drs.group_id
                LEFT JOIN sys.availability_group_listeners agl
                ON      drs.group_id = agl.group_id
                WHERE   is_local = 1
            END TRY
            BEGIN CATCH
                -- Getting availability group name is a best effort
                -- Service account does not have permission to query this view.
            END CATCH
        END

        SELECT  agi.ag_name,
                agi.agl_dns_name,
                agi.agl_port,
                agi.agl_ip_config,
                d.name,
                d.database_id,
                owner_sid,
                osp.name owner,
                DATEADD(second, DATEDIFF(second, GETDATE(), GETUTCDATE()), d.create_date) AS utcCreateDate,
                compatibility_level,
                collation_name,
                user_access,
                is_read_only,
                is_auto_close_on,
                is_auto_shrink_on,
                state,
                snapshot_isolation_state,
                is_read_committed_snapshot_on,
                recovery_model,
                is_broker_enabled,
                is_fulltext_enabled,
                is_parameterization_forced,
                dm.mirroring_guid,' +
                (CASE 
                    WHEN SUBSTRING(CONVERT(NVARCHAR, SERVERPROPERTY('ProductVersion')), 1, 1) = 9 THEN ' CONVERT(BIT, 0) is_encrypted '
                    ELSE ' d.is_encrypted '
                END) +   '
        FROM    sys.databases d
        JOIN    sys.database_mirroring dm
        ON      d.database_id = dm.database_id
        LEFT JOIN sys.server_principals osp
        ON      d.owner_sid = osp.sid
        LEFT JOIN @agi agi
        ON      d.database_id = agi.database_id
        WHERE   d.name = @databaseName
    END'

EXEC sp_executesql @stmt, N'@databaseName sysname', @databaseName = @databaseName