-- Create a temp table for copying data over for tbl_TempUserExtensionLicense
IF OBJECT_ID('Licensing.tbl_TempUserExtensionLicense', 'U') IS NULL
CREATE TABLE Licensing.tbl_TempUserExtensionLicense(
    PartitionId         INT                 NOT NULL,
    InternalScopeId     INT                 NOT NULL,
    UserId              UNIQUEIDENTIFIER    NOT NULL,
    ExtensionId         VARCHAR(200)        NOT NULL,
    Source              TINYINT             Not NULL,   --- The value is from LicensingSource
    Status              TINYINT             NOT NULL,   --- The value is from UserExtensionLicenseStatus
    CollectionId        UNIQUEIDENTIFIER,               --- The value is collection Id
    AssignmentDate      DATETIME            NOT NULL    DEFAULT(GETUTCDATE()),
    LastUpdated         DATETIME
)

-- This index is needed since we are trying to detect conflict keys when insert from temp table
IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'PK_tbl_TempUserExtensionLicense' AND object_id = OBJECT_ID('Licensing.tbl_TempUserExtensionLicense'))
CREATE UNIQUE CLUSTERED INDEX PK_tbl_TempUserExtensionLicense ON Licensing.tbl_TempUserExtensionLicense
(
    PartitionId, InternalScopeId, UserId, ExtensionId, Source
)
