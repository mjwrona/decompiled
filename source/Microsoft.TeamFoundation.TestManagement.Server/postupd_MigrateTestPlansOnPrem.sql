IF EXISTS  (SELECT  *
            FROM    sys.indexes i
            WHERE   i.object_id = OBJECT_ID('tbl_Point')
                    AND i.name = 'ix_Point_PlanId_SuiteId_IsMigrated')
BEGIN
    DROP INDEX ix_Point_PlanId_SuiteId_IsMigrated ON tbl_Point
    RAISERROR('Dropped index ix_Point_PlanId_SuiteId_IsMigrated ON tbl_Point', 10, 0) WITH NOWAIT
END

IF EXISTS  (SELECT  *
            FROM    sys.indexes i
            WHERE   i.object_id = OBJECT_ID('tbl_Session')
                    AND i.name = 'ix_Session_DataspaceId_PlanId_IsMigrated')
BEGIN
    DROP INDEX ix_Session_DataspaceId_PlanId_IsMigrated ON tbl_Session
    RAISERROR('Dropped index ix_Session_DataspaceId_PlanId_IsMigrated ON tbl_Session' , 10, 0) WITH NOWAIT
END

IF EXISTS  (SELECT  *
            FROM    sys.indexes i
            WHERE   i.object_id = OBJECT_ID('tbl_TestRun')
                    AND i.name = 'ix_TestRun_PlanId_IsMigrated')
BEGIN
    DROP INDEX ix_TestRun_PlanId_IsMigrated ON tbl_TestRun
    RAISERROR('Dropped index ix_TestRun_PlanId_IsMigrated ON tbl_TestRun' , 10, 0) WITH NOWAIT
END

IF EXISTS  (SELECT  *
            FROM    sys.indexes i
            WHERE   i.object_id = OBJECT_ID('tbl_PointHistory')
                    AND i.name = 'ix_PointHistory_PlanId_IsMigrated')
BEGIN
    DROP INDEX ix_PointHistory_PlanId_IsMigrated ON tbl_PointHistory
    RAISERROR('Dropped index ix_PointHistory_PlanId_IsMigrated ON tbl_PointHistory' , 10, 0) WITH NOWAIT
END

IF NOT EXISTS (SELECT  *
               FROM    sys.indexes i
               WHERE   i.object_id = OBJECT_ID('tbl_PointHistory')
                       AND i.name = 'ix_PointHistory_RV')
BEGIN
    CREATE NONCLUSTERED INDEX ix_PointHistory_RV ON
    tbl_PointHistory
    (
        PartitionId,
        RV
    )
    INCLUDE
    (
        Active,
        PlanId,
        PointId,
        ChangeNumber,
        IsSuiteChanged,
        IsActiveChanged,
        IsDeleted
    )
    RAISERROR('Created index ix_PointHistory_RV ON tbl_PointHistory', 10, 0) WITH NOWAIT
END

IF NOT EXISTS (SELECT  *
               FROM    sys.indexes i
               WHERE   i.object_id = OBJECT_ID('tbl_PointHistory')
                       AND i.name = 'ix_PointHistory_PlanId_LastTestRunId')
BEGIN
    CREATE NONCLUSTERED INDEX ix_PointHistory_PlanId_LastTestRunId ON
    tbl_PointHistory
    (
        PartitionId,
        PlanId,
        LastTestRunId
    )
    INCLUDE
    (
        PointId,
        ChangeNumber,
        State,
        IsDeleted
    )
    RAISERROR('Created index ix_PointHistory_PlanId_LastTestRunId ON tbl_PointHistory', 10, 0) WITH NOWAIT
END

IF NOT EXISTS (SELECT  *
               FROM    sys.indexes i
               WHERE   i.object_id = OBJECT_ID('tbl_PointHistory')
                       AND i.name = 'ix_PointHistory_WatermarkDate_PointId_ChangeNumber')
BEGIN
    CREATE NONCLUSTERED INDEX ix_PointHistory_WatermarkDate_PointId_ChangeNumber
    ON tbl_PointHistory 
    (
        PartitionId,
        WatermarkDate,
        PointId,
        ChangeNumber
    )
    RAISERROR('Created index ix_PointHistory_WatermarkDate_PointId_ChangeNumber ON tbl_PointHistory', 10, 0) WITH NOWAIT
END

IF NOT EXISTS (SELECT PlanId
               FROM   tbl_PlanData
               WHERE  PartitionId > 0
                      AND MigrationState != 2)
BEGIN
    -- Table not present if there were no plans to migrate
    IF OBJECT_ID('tbl_PointHistory_PreMigration') IS NOT NULL
    BEGIN
        DROP TABLE tbl_PointHistory_PreMigration
        RAISERROR('Dropped Table tbl_PointHistory_PreMigration', 10, 0) WITH NOWAIT
    END
END
ELSE
BEGIN
    RAISERROR('Not all plans migrated. Retaining table, tbl_PointHistory_PreMigration', 10, 0) WITH NOWAIT
END