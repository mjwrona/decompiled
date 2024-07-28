SET XACT_ABORT ON

DECLARE @allPlansMigrated BIT = 1

IF EXISTS ( SELECT PlanId
            FROM   tbl_PlanData
            WHERE  PartitionId > 0
                   AND MigrationState != 2)
BEGIN
    SET @allPlansMigrated = 0
END

IF NOT EXISTS  (SELECT  *
                FROM    sys.indexes i
                WHERE   i.object_id = OBJECT_ID('tbl_Point')
                        AND i.name = 'ix_Point_PlanId_SuiteId_IsMigrated') AND @allPlansMigrated = 0
BEGIN
    CREATE NONCLUSTERED INDEX ix_Point_PlanId_SuiteId_IsMigrated
    ON tbl_Point
    (
        PartitionId,
        PlanId,
        SuiteId,
        IsMigrated
    )
    RAISERROR('Created index ix_Point_PlanId_SuiteId_IsMigrated ON tbl_Point', 10, 0) WITH NOWAIT
END

IF NOT EXISTS  (SELECT  *
                FROM    sys.indexes i
                WHERE   i.object_id = OBJECT_ID('tbl_Session')
                        AND i.name = 'ix_Session_DataspaceId_PlanId_IsMigrated') AND @allPlansMigrated = 0
BEGIN
    CREATE NONCLUSTERED INDEX ix_Session_DataspaceId_PlanId_IsMigrated ON
    tbl_Session
    (
        PartitionId,
        DataspaceId,
        TestPlanId,
        IsMigrated
    )
    RAISERROR('Created index ix_Session_DataspaceId_PlanId_IsMigrated ON tbl_Session' , 10, 0) WITH NOWAIT
END

IF NOT EXISTS  (SELECT  *
                FROM    sys.indexes i
                WHERE   i.object_id = OBJECT_ID('tbl_TestRun')
                        AND i.name = 'ix_TestRun_PlanId_IsMigrated') AND @allPlansMigrated = 0
BEGIN
    CREATE NONCLUSTERED INDEX ix_TestRun_PlanId_IsMigrated ON
    tbl_TestRun
    (
        PartitionId,
        TestPlanId,
        IsMigrated
    )
    RAISERROR('Created index ix_TestRun_PlanId_IsMigrated ON tbl_TestRun' , 10, 0) WITH NOWAIT
END

IF EXISTS (SELECT  *
           FROM    sys.indexes i
           WHERE   i.object_id = OBJECT_ID('tbl_PointHistory')
                   AND i.name = 'ix_PointHistory_RV') AND @allPlansMigrated = 0
BEGIN
    DROP INDEX ix_PointHistory_RV ON tbl_PointHistory
    RAISERROR('Dropped index ix_PointHistory_RV ON tbl_PointHistory', 10, 0) WITH NOWAIT
END

IF EXISTS (SELECT  *
           FROM    sys.indexes i
           WHERE   i.object_id = OBJECT_ID('tbl_PointHistory')
                   AND i.name = 'ix_PointHistory_PlanId_LastTestRunId') AND @allPlansMigrated = 0
BEGIN
    DROP INDEX ix_PointHistory_PlanId_LastTestRunId ON tbl_PointHistory
    RAISERROR('Dropped index ix_PointHistory_PlanId_LastTestRunId ON tbl_PointHistory', 10, 0) WITH NOWAIT
END

-- tbl_PointHistory move to tbl_PointHistory_PreMigration for efficient migration
-- Table used in prc_UpdateTcmArtifactsAfterMigrationOnWit - will be empty if migration is done.
IF (OBJECT_ID('dbo.tbl_PointHistory_PreMigration', 'U') IS NULL
    AND @allPlansMigrated = 0)
BEGIN
    BEGIN TRAN

    EXEC sp_rename N'tbl_PointHistory', N'tbl_PointHistory_PreMigration'
    RAISERROR('Renamed tbl_PointHistory table to tbl_PointHistory_PreMigration' , 10, 0) WITH NOWAIT

    CREATE TABLE tbl_PointHistory (
        PartitionId             INT                 NOT NULL,
        PointId                 INT                 NOT NULL,
        ChangeNumber            INT                 NOT NULL,
        State                   TINYINT             NOT NULL,
        Active                  BIT                 NOT NULL,
        FailureType             TINYINT             NULL,
        PlanId                  INT                 NOT NULL,
        LastTestRunId           INT                 NOT NULL,
        LastTestResultId        INT                 NOT NULL,
        TestCaseId              INT                 NOT NULL,
        AssignedTo              UNIQUEIDENTIFIER    NOT NULL,
        LastUpdated             DATETIME            NOT NULL,
        LastUpdatedBy           UNIQUEIDENTIFIER    NOT NULL,
        IsSuiteChanged          BIT                 NOT NULL   DEFAULT 0,
        IsActiveChanged         BIT                 NOT NULL   DEFAULT 0,
        LastResolutionStateId   INT                 NOT NULL   DEFAULT 0,
        IsMigrated              BIT                 NOT NULL   DEFAULT 0,
        RV                      ROWVERSION,
        IsDeleted               BIT                 NOT NULL   DEFAULT 0,
        SuiteId                 INT                 NOT NULL   DEFAULT 0,
        ConfigurationId         INT                 NOT NULL   DEFAULT 0,
        WatermarkDate           DATETIME            NOT NULL   DEFAULT(GETUTCDATE()),
        Outcome                 TINYINT             NULL
    )
    RAISERROR('Created new table tbl_PointHistory' , 10, 0) WITH NOWAIT

    CREATE UNIQUE CLUSTERED INDEX ixc_PointHistory ON tbl_PointHistory
    (
        PartitionId,
        PlanId,
        PointId,
        ChangeNumber
    )
    RAISERROR('Created index ixc_PointHistory ON tbl_PointHistory' , 10, 0) WITH NOWAIT

    COMMIT TRAN
END