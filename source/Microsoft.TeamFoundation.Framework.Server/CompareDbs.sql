--DECLARE @db1Name SYSNAME = 'AzureDevOps_TestUpgrade2020Update1_DefaultCollection'
--DECLARE @db2Name SYSNAME = 'AzureDevOps_TestUpgrade2020Update1_NewCollection'

DECLARE @stmt NVARCHAR(MAX)
SET NOCOUNT ON

-- a temporary table that holds columns information from the first database
CREATE TABLE #db1
(
    schemaName          SYSNAME     NOT NULL,
    tableName           SYSNAME     NOT NULL,
    tableTextInRowLimit INT         NOT NULL,
    columnName          SYSNAME     NOT NULL,
    isNullable          BIT         NOT NULL,
    systemType          SYSNAME     NOT NULL,
    maxLength           SMALLINT    NOT NULL,
    precision           TINYINT     NOT NULL,
    scale               TINYINT     NOT NULL,
    isAnsiPadded        BIT         NOT NULL,
    isRowGuid           BIT         NOT NULL,
    isIdentity          BIT         NOT NULL,
    isNotForReplication BIT         NOT NULL,
    isComputed          BIT         NOT NULL,
    isPersisted         BIT         NULL,
    computedDefinition  NVARCHAR(MAX),
    isSparse            BIT         NOT NULL,
    columnDefault       NVARCHAR(MAX) NULL
    PRIMARY KEY (
        tableName,
        columnName,
        schemaName
    )
)

-- a temporary table that holds columns information from the second database
CREATE TABLE #db2
(
    schemaName          SYSNAME     NOT NULL,
    tableName           SYSNAME     NOT NULL,
    tableTextInRowLimit INT         NOT NULL,
    columnName          SYSNAME     NOT NULL,
    isNullable          BIT         NOT NULL,
    systemType          SYSNAME     NOT NULL,
    maxLength           SMALLINT    NOT NULL,
    precision           TINYINT     NOT NULL,
    scale               TINYINT     NOT NULL,
    isAnsiPadded        BIT         NOT NULL,
    isRowGuid           BIT         NOT NULL,
    isIdentity          BIT         NOT NULL,
    isNotForReplication BIT         NOT NULL,
    isComputed          BIT         NOT NULL,
    isPersisted         BIT         NULL,
    computedDefinition  NVARCHAR(MAX),
    isSparse            BIT         NOT NULL,
    columnDefault       NVARCHAR(MAX) NULL
    PRIMARY KEY (
        tableName,
        columnName,
        schemaName
    )
)

-- a temporary table that holds index information from the first database
CREATE TABLE #index1
(
    schemaName      SYSNAME         NOT NULL,
    tableName       SYSNAME         NOT NULL,
    indexName       SYSNAME         NOT NULL,
    indexName2      SYSNAME         NOT NULL, -- if constraint is system named, indexName2 contains is a truncated system generated name, indexName otherwise
    isSystemNamed   BIT             NOT NULL,
    columnName      SYSNAME         NOT NULL,
    columnNumber    INT             NOT NULL,
    indexType       TINYINT         NOT NULL,
    indexTypeDesc   NVARCHAR(60)    NOT NULL,
    isUnique        BIT             NOT NULL,
    ignoreDup       BIT             NOT NULL,
    isPrimaryKey    BIT             NOT NULL,
    isUniqueConstraint BIT          NOT NULL,
    fill_Factor     TINYINT         NOT NULL,
    isPadded        BIT             NOT NULL,
    isDisabled      BIT             NOT NULL,
    isHypothetical  BIT             NOT NULL,
    allowRowLocks   BIT             NOT NULL,
    allowPageLocks  BIT             NOT NULL,
    hasFilter       BIT             NOT NULL,
    filterDefinition NVARCHAR(MAX),
    isDescendingKey BIT             NOT NULL,
    isIncludedColumn BIT            NOT NULL,
    noRecompute     BIT NOT NULL,
    PRIMARY KEY  (
        tableName,
        schemaName,
        indexName,
        columnName
    )
)

-- a temporary table that holds index information from the second database
CREATE TABLE #index2
(
    schemaName      SYSNAME         NOT NULL,
    tableName       SYSNAME         NOT NULL,
    indexName       SYSNAME         NOT NULL,
    indexName2      SYSNAME         NOT NULL, -- if constraint is system named, indexName2 contains is a truncated system generated name, indexName otherwise
    isSystemNamed   BIT             NOT NULL,
    columnName      SYSNAME         NOT NULL,
    columnNumber    INT             NOT NULL,
    indexType       TINYINT         NOT NULL,
    indexTypeDesc   NVARCHAR(60)    NOT NULL,
    isUnique        BIT             NOT NULL,
    ignoreDup       BIT             NOT NULL,
    isPrimaryKey    BIT             NOT NULL,
    isUniqueConstraint BIT          NOT NULL,
    fill_Factor     TINYINT         NOT NULL,
    isPadded        BIT             NOT NULL,
    isDisabled      BIT             NOT NULL,
    isHypothetical  BIT             NOT NULL,
    allowRowLocks   BIT             NOT NULL,
    allowPageLocks  BIT             NOT NULL,
    hasFilter       BIT             NOT NULL,
    filterDefinition NVARCHAR(MAX),
    isDescendingKey BIT             NOT NULL,
    isIncludedColumn BIT            NOT NULL,
    noRecompute     BIT NOT NULL,
    PRIMARY KEY  (
        tableName,
        schemaName,
        indexName,
        columnName
    )
)

-- a temporary table that holds columns information for the TVPs from the first database
CREATE TABLE #tvp1
(
    schemaName      SYSNAME     NOT NULL,
    tvpName         SYSNAME     NOT NULL,
    columnName      SYSNAME     NOT NULL,
    isNullable      BIT         NOT NULL,
    systemType      SYSNAME     NOT NULL,
    maxLength       SMALLINT    NOT NULL,
    precision       TINYINT     NOT NULL,
    scale           TINYINT     NOT NULL,
    isAnsiPadded    BIT         NOT NULL,
    isComputed      BIT         NOT NULL,
    isPersisted     BIT         NULL,
    computedDefinition NVARCHAR(MAX),
    columnDefault   NVARCHAR(MAX) NULL
    PRIMARY KEY (
        tvpName,
        columnName,
        schemaName
    )
)

-- a temporary table that holds columns information for the TVPs from the second database
CREATE TABLE #tvp2
(
    schemaName      SYSNAME     NOT NULL,
    tvpName         SYSNAME     NOT NULL,
    columnName      SYSNAME     NOT NULL,
    isNullable      BIT         NOT NULL,
    systemType      SYSNAME     NOT NULL,
    maxLength       SMALLINT    NOT NULL,
    precision       TINYINT     NOT NULL,
    scale           TINYINT     NOT NULL,
    isAnsiPadded    BIT         NOT NULL,
    isComputed      BIT         NOT NULL,
    isPersisted     BIT         NULL,
    computedDefinition NVARCHAR(MAX),
    columnDefault   NVARCHAR(MAX) NULL
    PRIMARY KEY (
        tvpName,
        columnName,
        schemaName
    )
)

-- a temporary table that holds information for the tables from the first database
CREATE TABLE #tbl1
(
    schemaName      SYSNAME  NOT NULL,
    tableName       SYSNAME  NOT NULL,
    lockEscalation  TINYINT  NOT NULL
    PRIMARY KEY (
        schemaName,
        tableName
    )
)

-- a temporary table that holds information for the tables from the second database
CREATE TABLE #tbl2
(
    schemaName      SYSNAME  NOT NULL,
    tableName       SYSNAME  NOT NULL,
    lockEscalation  TINYINT  NOT NULL
    PRIMARY KEY (
        schemaName,
        tableName
    )
)

DECLARE @stmtTemplate NVARCHAR(4000) = '
USE $dbName$
INSERT  $tempTableName$
SELECT  OBJECT_SCHEMA_NAME(tbl.object_id),
        tbl.name,
        OBJECTPROPERTY(tbl.object_id, ''TableTextInRowLimit''),
        c.name,
        c.is_nullable,
        tp.name,
        -- tbl_nodes has the following columns
        --   path            NVARCHAR(4000) NULL
        --  indexed_path    AS (left(path,(420))) PERSISTED
        --
        -- IF database collation is UTF8 collation, max_length is 1680 and 840 otherwise
        IIF(c.collation_name LIKE ''%UTF8'' AND tbl.name = ''tbl_nodes'' AND c.name = ''indexed_path'', c.max_length / 2, c.max_length),
        c.precision,
        c.scale,
        c.is_ansi_padded,
        c.is_rowguidcol,
        c.is_identity,
        ISNULL(ic.is_not_for_replication, 0),
        c.is_computed,
        cc.is_persisted,
        cc.definition,
        c.is_sparse,
        d.definition columnDefault
FROM    $dbName$.sys.tables tbl
JOIN    $dbName$.sys.columns c
ON      tbl.object_id = c.object_id
JOIN    $dbName$.sys.types tp
ON      c.system_type_id = tp.system_type_id
        AND c.user_type_id = tp.user_type_id
LEFT JOIN $dbName$.sys.computed_columns cc
ON      c.column_id = cc.column_id
        AND c.object_id = cc.object_id
LEFT JOIN $dbName$.sys.default_constraints d
ON      c.column_id = d.parent_column_id
        AND c.object_id = d.parent_object_id
LEFT JOIN $dbName$.sys.identity_columns ic
ON      c.object_Id = ic.object_id
        AND c.column_id = ic.column_id
WHERE   tbl.name NOT IN (
            ''sysdiagrams'',
            ''tbl_AttachmentContent'',
            ''tbl_BuildInformationField'',
            ''tbl_tmpLeadingKeyStats'',
            ''tbl_WorkItemData'',
            ''Versions'',
            ''WorkItemLinksDestroyed'',
            ''WorkItemLongTexts'',
            ''WorkItemsAre'',
            ''WorkItemsLatest'',
            ''WorkItemsWere'',
            ''xxTree'')
        AND tbl.name NOT LIKE ''tbl_Upgrade%''
        AND tbl.name NOT LIKE ''tbl_RollupEngine%''
        AND NOT (tbl.name = ''tbl_TestResult'' AND tbl.schema_id = 1)
        AND tbl.name NOT LIKE ''%DataspaceBackupToDrop'''

-- Query first database
SET @stmt = REPLACE(REPLACE(@stmtTemplate, '$tempTableName$', '#db1'), '$dbName$', QUOTENAME(@db1Name))
exec sp_executesql @stmt
-- Query second database
SET @stmt = REPLACE(REPLACE(@stmtTemplate, '$tempTableName$', '#db2'), '$dbName$', QUOTENAME(@db2Name))
exec sp_executesql @stmt

-- First result set. Returns tables that only exist in one database
SELECT  DISTINCT
        ISNULL(db1.schemaName, db2.schemaName) schemaName,
        ISNULL(db1.tableName, db2.tableName) tableName,
        CASE
            WHEN db1.tableName IS NULL THEN 2 -- table exists in the second database, but not in the first one
            ELSE 1                            -- table exists in the first database, but not in the first one
        END existsInDb
FROM    #db1 db1
FULL JOIN #db2 db2
ON      db1.tableName = db2.tableName
        AND db1.schemaName = db2.schemaName
WHERE   db1.tableName IS NULL
        OR db2.tableName IS NULL

-- Delete tables from the db1 that do not exist in the db2
DELETE  #db1
FROM    #db1
LEFT JOIN   #db2
ON      #db1.tableName = #db2.tableName
        AND #db1.schemaName = #db2.schemaName
WHERE   #db2.tableName IS NULL

-- Delete tables from the db2 that do not exist in the db1
DELETE  #db2
FROM    #db2
LEFT JOIN   #db1
ON      #db2.tableName = #db1.tableName
        AND #db2.schemaName = #db1.schemaName
WHERE   #db1.tableName IS NULL

-- Filter out computed column definitions where SQL 2016 or SQL Azure has added parenthesis around functions
-- Function: reverse
UPDATE #db1
SET computedDefinition = REPLACE(REPLACE(computedDefinition, '(reverse(', 'reverse('), ') collate', ' collate')
WHERE CHARINDEX('(reverse(', computedDefinition) > 0 AND CHARINDEX(') collate', computedDefinition) > 0

UPDATE #db2
SET computedDefinition = REPLACE(REPLACE(computedDefinition, '(reverse(', 'reverse('), ') collate', ' collate')
WHERE CHARINDEX('(reverse(', computedDefinition) > 0 AND CHARINDEX(') collate', computedDefinition) > 0

-- Function: upper
UPDATE #db1
SET computedDefinition = REPLACE(REPLACE(computedDefinition, '((upper(', '(upper('), ')) collate', ') collate')
WHERE CHARINDEX('((upper(', computedDefinition) > 0 AND CHARINDEX(')) collate', computedDefinition) > 0

UPDATE #db2
SET computedDefinition = REPLACE(REPLACE(computedDefinition, '((upper(', '(upper('), ')) collate', ') collate')
WHERE CHARINDEX('((upper(', computedDefinition) > 0 AND CHARINDEX(')) collate', computedDefinition) > 0

-- filter out extra zeros and parenthesis we don't care about
UPDATE #db1
SET computedDefinition = REPLACE(REPLACE(REPLACE(ISNULL(computedDefinition, ''), ',0)', ')'), '(0)', '0'),',0)', ')')

UPDATE #db2
SET computedDefinition = REPLACE(REPLACE(REPLACE(ISNULL(computedDefinition, ''), ',0)', ')'), '(0)', '0'),',0)', ')')

SELECT  *
FROM
(
    SELECT  ISNULL(db1.schemaName,  db2.schemaName) schemaName,
            ISNULL(db1.tableName,  db2.tableName) tableName,
            ISNULL(db1.columnName, db2.columnName) columnName,
            CASE
                WHEN db2.columnName IS NULL THEN 1  -- first db only
                WHEN db1.columnName IS NULL THEN 2  -- second db only
                ELSE 3                              -- both dbs
            END existsInDb,
            CASE
                WHEN db1.columnName IS NULL THEN 'db2 only'
                WHEN db2.columnName IS NULL THEN 'db1 only'
                WHEN db1.isNullable   <> db2.isNullable THEN 'isNullable'
                WHEN db1.systemType   <> db2.systemType THEN 'systemType'
                WHEN db1.maxLength    <> db2.maxLength THEN 'maxLength'
                WHEN db1.precision    <> db2.precision THEN 'precision'
                WHEN db1.scale        <> db2.scale THEN 'scale'
                WHEN db1.isAnsiPadded <> db2.isAnsiPadded THEN 'isAnsiPadded'
                WHEN db1.isComputed   <> db2.isComputed THEN 'isComputed'
                WHEN db1.isIdentity   <> db2.isIdentity THEN 'isIdentity'
                WHEN db1.isRowGuid    <> db2.isRowGuid  THEN 'isRowGuid'
                WHEN db1.isSparse     <> db2.isSparse THEN 'isSparse'
                WHEN db1.isPersisted  <> db2.isPersisted  THEN 'isPersisted'
                WHEN db1.tableTextInRowLimit <> db2.tableTextInRowLimit THEN 'tableTextInRowLimit'
                --TODO WHEN db1.isNotForReplication <> db2.isNotForReplication THEN 'isNotForReplication'
                WHEN ISNULL(db1.columnDefault, '') <> ISNULL(db2.columnDefault, '') THEN 'columnDefault'
                WHEN ISNULL(db1.computedDefinition, '') <>
                     ISNULL(db2.computedDefinition, '') THEN 'computedDefinition'
            END reason,
            db1.isNullable db1isNullable,
            db2.isNullable db2isNullable,
            db1.systemType db1systemType,
            db2.systemType db2systemType,
            db1.maxLength    db1maxLength,
            db2.maxLength    db2maxLength,
            db1.precision    db1precision,
            db2.precision    db2precision,
            db1.scale        db1scale,
            db2.scale        db2scale,
            db1.isAnsiPadded db1isAnsiPadded,
            db2.isAnsiPadded db2isAnsiPadded,
            db1.isComputed   db1isComputed,
            db2.isComputed   db2isComputed,
            db1.isIdentity   db1isIdentity,
            db2.isIdentity   db2isIdentity,
            db1.isRowGuid    db1isRowGuid,
            db2.isRowGuid    db2isRowGuid,
            db1.isSparse     db1isSparse,
            db2.isSparse     db2isSparse,
            db1.isPersisted  db1isPersisted,
            db2.isPersisted  db2isPersisted,
            db1.columnDefault db1ColumnDefault,
            db2.columnDefault db2ColumnDefault,
            db1.tableTextInRowLimit db1tableTextInRowLimit,
            db2.tableTextInRowLimit db2tableTextInRowLimit,
            db1.computedDefinition db1computedDefinition,
            db2.computedDefinition db2computedDefinition
    FROM    #db1 db1
    FULL JOIN #db2 db2
    ON      db1.schemaName = db2.schemaName
            AND db1.tableName = db2.tableName
            AND db1.columnName = db2.columnName) tmp
    WHERE   reason IS NOT NULL

SET @stmtTemplate = '
INSERT  $tempTable$
SELECT  s.name schema_name,
        t.name table_name,
        i.name index_name,
        CASE
            WHEN ISNULL(kc.is_system_named, 0) = 0 THEN i.name
            ELSE SUBSTRING(i.name, 0, 10)
        END,
        ISNULL(kc.is_system_named, CONVERT(BIT,0)) is_system_named,
        c.name columnName,
        key_ordinal,
        i.type index_type,
        i.type_desc index_type_desc,
        i.is_unique,
        i.ignore_dup_key,
        i.is_primary_key,
        i.is_unique_constraint,
        i.fill_factor, i.is_padded,
        i.is_disabled, i.is_hypothetical, i.allow_row_locks, i.allow_page_locks, i.has_filter, i.filter_definition,
        ic.is_descending_key, ic.is_included_column,
        st.no_recompute
FROM    $dbName$.sys.indexes i
JOIN    $dbName$.sys.tables t
ON      i.object_id = t.object_id
JOIN    $dbName$.sys.schemas s
ON      t.schema_id = s.schema_id
JOIN    $dbName$.sys.index_columns ic
ON      t.object_id = ic.object_id
        AND i.index_id = ic.index_id
JOIN    $dbName$.sys.columns c
ON      c.object_id = t.object_id
        AND c.column_id = ic.column_id
LEFT OUTER JOIN $dbName$.sys.key_constraints kc
ON      kc.name = i.name
        AND kc.parent_object_id = t.object_id
JOIN    $dbName$.sys.stats st
ON      st.name = i.name
        AND st.object_id = t.object_id
WHERE   i.type > 0
        AND t.name NOT IN (
            ''sysdiagrams'',
            ''tbl_BuildInformationField'',
            ''tbl_AttachmentContent'',
            ''tbl_tmpLeadingKeyStats'',
            ''tbl_File_Snapshot'',
            ''tbl_WorkItemData'',
            ''Versions'',
            ''WorkItemLongTexts'',
            ''WorkItemsAre'',
            ''WorkItemsLatest'',
            ''WorkItemsWere'',
            ''xxTree'')
        AND t.name NOT LIKE ''tbl_Upgrade%''
        AND t.name NOT LIKE ''tbl_RollupEngine%''
        AND t.name NOT LIKE ''%DataspaceBackupToDrop''
        AND i.name NOT LIKE ''IX_NAV%''
        AND i.name <> ''IX_tbl_FileReference_Upgrade_MigrateFileId''
        AND NOT (t.name = ''tbl_TestResult'' AND t.schema_id = 1)
--ORDER BY schema_name, table_name, is_primary_key DESC, index_name, key_ordinal'

SET @stmt = REPLACE(REPLACE(@stmtTemplate, '$dbName$', QUOTENAME(@db1Name)), '$tempTable$', '#index1')
exec sp_executesql @stmt

SET @stmt = REPLACE(REPLACE(@stmtTemplate, '$dbName$', QUOTENAME(@db2Name)), '$tempTable$', '#index2')
exec sp_executesql @stmt
DECLARE @diffIndexes AS TABLE
(
    schemaName          SYSNAME NOT NULL,
    tableName           SYSNAME NOT NULL,
    indexName           SYSNAME NOT NULL,
    existsInDb          INT  NOT NULL, -- 1 in db1 only, 2 in db2 only, 3 in both dbs
    reason              NVARCHAR(1000),
    db1allowPageLocks   BIT NULL,
    db2allowPageLocks   BIT NULL,
    db1allowRowLocks    BIT NULL,
    db2allowRowLocks    BIT NULL,
    db1fillFactor       INT NULL,
    db2fillFactor       INT NULL,
    db1hasFilter        BIT NULL,
    db2hasFilter        BIT NULL,
    db1IgnoreDup        BIT NULL,
    db2IgnoreDup        BIT NULL,
    db1IndexType        NVARCHAR(60) NULL,
    db2IndexType        NVARCHAR(60) NULL,
    db1isDisabled       BIT NULL,
    db2isDisabled       BIT NULL,
    db1isHypothetical   BIT NULL,
    db2isHypothetical   BIT NULL,
    db1isPadded         BIT NULL,
    db2isPadded         BIT NULL,
    db1isPrimaryKey     BIT NULL,
    db2isPrimaryKey     BIT NULL,
    db1isSystemNamed    BIT NULL,
    db2isSystemNamed    BIT NULL,
    db1isUnique         BIT NULL,
    db2isUnique         BIT NULL,
    db1isUniqueConstraint BIT NULL,
    db2isUniqueConstraint BIT NULL,
    db1noRecompute        BIT NULL,
    db2noRecompute        BIT NULL
)

INSERT  @diffIndexes
SELECT  *
FROM
(
SELECT  ISNULL(i1.schemaName, i2.schemaName) schemaName,
        ISNULL(i1.tableName, i2.tableName) tableName,
        ISNULL(i1.indexName, i2.indexName) indexName,
        CASE
            WHEN i2.indexName IS NULL THEN 1 -- only in db1
            WHEN i1.indexName IS NULL THEN 2 -- only in db2
            ELSE 3                           -- in both dbs
        END existsInDb,
        CASE
            WHEN i1.tableName IS NULL THEN 'db2 only'
            WHEN i2.tableName IS NULL THEN 'db1 only'
            WHEN i1.allowPageLocks <> i2.allowPageLocks THEN 'allowPageLocks'
            WHEN i1.allowRowLocks <> i2.allowRowLocks THEN 'allowRowLocks'
            WHEN i1.fill_Factor <> i2.fill_Factor THEN 'fillFactor'
            WHEN i1.hasFilter <> i2.hasFilter THEN 'hasFilter'
            WHEN i1.ignoreDup <> i2.ignoreDup THEN 'IgnoreDup'
            WHEN i1.indexType <> i2.indexType THEN 'indexType'
            WHEN i1.isDisabled <> i2.isDisabled THEN 'isDisabled'
            WHEN i1.isHypothetical <> i2.isHypothetical THEN 'isHypothetical'
            WHEN i1.isPadded <> i2.isPadded THEN 'isPadded'
            WHEN i1.isPrimaryKey <> i2.isPrimaryKey THEN 'isPrimaryKey'
            WHEN i1.isUnique <> i2.isUnique THEN 'isUnique'
            WHEN i1.isUniqueConstraint <> i2.isUniqueConstraint THEN 'isUniqueConstraint'
            WHEN i2.noRecompute <> i2.noRecompute THEN 'noRecompute'
        END reason,
        i1.allowPageLocks       db1allowPageLocks,
        i2.allowPageLocks       db2allowPageLocks,
        i1.allowRowLocks        db1allowRowLocks,
        i2.allowRowLocks        db2allowRowLocks,
        i1.fill_Factor          db1fillFactor,
        i2.fill_Factor          db2fillFactor,
        i1.hasFilter            db1hasFilter,
        i2.hasFilter            db2hasFilter,
        i1.ignoreDup            db1IgnoreDup,
        i2.ignoreDup            db2IgnoreDup,
        i1.indexTypeDesc        db1indexType,
        i2.indexTypeDesc        db2indexType,
        i1.isDisabled           db1isDisabled,
        i2.isDisabled           db2isDisabled,
        i1.isHypothetical       db1isHypothetical,
        i2.isHypothetical       db2isHypothetical,
        i1.isPadded             db1isPadded,
        i2.isPadded             db2isPadded,
        i1.isPrimaryKey         db1isPrimaryKey,
        i2.isPrimaryKey         db2isPrimaryKey,
        i1.isSystemNamed        db1isSystemNamed,
        i2.isSystemNamed        db2isSystemNamed,
        i1.isUnique             db1isUnique,
        i2.isUnique             db2isUnique,
        i1.isUniqueConstraint   db1isUniqueConstraint,
        i2.isUniqueConstraint   db2isUniqueConstraint,
        i1.noRecompute          db1noRecompute,
        i2.noRecompute          db2noRecompute
FROM   (SELECT DISTINCT
                schemaName,
                tableName,
                indexName,
                indexName2,
                isSystemNamed,
                indexType,
                indexTypeDesc,
                isUnique,
                ignoreDup,
                isPrimaryKey,
                isUniqueConstraint,
                fill_Factor,
                isPadded,
                isDisabled,
                isHypothetical,
                allowRowLocks,
                allowPageLocks,
                hasFilter,
                filterDefinition,
                noRecompute
                FROM   #index1
        ) i1
FULL JOIN (SELECT DISTINCT
                schemaName,
                tableName,
                indexName,
                indexName2,
                isSystemNamed,
                indexType,
                indexTypeDesc,
                isUnique,
                ignoreDup,
                isPrimaryKey,
                isUniqueConstraint,
                fill_Factor,
                isPadded,
                isDisabled,
                isHypothetical,
                allowRowLocks,
                allowPageLocks,
                hasFilter,
                filterDefinition,
                noRecompute
                FROM   #index2
        ) i2
ON      i1.schemaName = i2.schemaName
        AND i1.tableName = i2.tableName
        AND i1.indexName2 = i2.indexName2
) tmp
WHERE reason IS NOT NULL

SELECT  *
FROM    @diffIndexes i
ORDER BY schemaName, tableName, indexName

-- Deleting indexes that exist in 1 db only
DELETE  i
FROM    #index1 i
JOIN    @diffIndexes d
ON      i.schemaName = d.schemaName
        AND i.tableName = d.tableName
        AND i.indexName = d.indexName
WHERE   d.existsInDb = 1

DELETE  i
FROM    #index2 i
JOIN    @diffIndexes d
ON      i.schemaName = d.schemaName
        AND i.tableName = d.tableName
        AND i.indexName = d.indexName
WHERE   d.existsInDb = 2

SELECT tmp.*
FROM
(
SELECT  ISNULL(i1.schemaName, i2.schemaName) schemaName,
        ISNULL(i1.tableName, i2.tableName) tableName,
        ISNULL(i1.indexName, i2.indexName) indexName,
        ISNULL(i1.columnName, i2.columnName) columnName,
        CASE
            WHEN i2.columnName IS NULL THEN 1 -- only in db1
            WHEN i1.columnName IS NULL THEN 2 -- only in db2
            ELSE 3                            -- in both dbs
        END existsInDb,

        CASE
            WHEN i1.columnName is NULL THEN 'db2 only'
            WHEN i2.columnName is NULL THEN 'db1 only'
            WHEN i1.isDescendingKey <> i2.isDescendingKey THEN 'isDescendingKey'
            WHEN i1.isIncludedColumn <> i2.isIncludedColumn THEN 'isIncludedColumn'
            WHEN i1.columnNumber <> i2.columnNumber THEN 'columnNumber'
        END reason,
        i1.isDescendingKey db1isDescendingKey,
        i2.isDescendingKey db2isDescendingKey,
        i1.isIncludedColumn db1isIncludedColumn,
        i2.isIncludedColumn db2isIncludedColumn,
        i1.columnNumber     db1columnNumber,
        i2.columnNumber     db2columnNumber
FROM    #index1 i1
FULL JOIN #index2 i2
ON      i1.schemaName = i2.schemaName
        AND i1.tableName = i2.tableName
        AND i1.indexName2 = i2.indexName2
        AND i1.columnName = i2.columnName
) tmp
WHERE   reason IS NOT NULL

-- Compare TVPs

SET @stmtTemplate = '
INSERT  $tempTableName$(schemaName,
        tvpName,
        columnName,
        isNullable,
        systemType,
        maxLength,
        precision,
        scale,
        isAnsiPadded,
        isComputed,
        isPersisted,
        computedDefinition,
        columnDefault)
SELECT  sc.name,
        tt.name,
        c.name,
        c.is_nullable,
        tp.name,
        c.max_length,
        c.precision,
        c.scale,
        c.is_ansi_padded,
        c.is_computed,
        cc.is_persisted,
        cc.definition,
        d.definition columnDefault
FROM    $dbname$.sys.table_types tt
JOIN    $dbname$.sys.schemas sc
        ON tt.schema_id = sc.schema_id
JOIN    $dbname$.sys.columns c
ON      tt.type_table_object_id = c.object_id
JOIN    $dbname$.sys.types tp
ON      c.system_type_id = tp.system_type_id
        AND c.user_type_id = tp.user_type_id
LEFT JOIN $dbname$.sys.computed_columns cc
ON      c.column_id = cc.column_id
        AND c.object_id = cc.object_id
LEFT JOIN $dbname$.sys.default_constraints d
ON      c.column_id = d.parent_column_id
        AND c.object_id = d.parent_object_id
WHERE   tt.name NOT IN (
            ''typ_WorkItemTable'',
            ''typ_WorkItemFlagTable'')'

-- Query first database
SET @stmt = REPLACE(REPLACE(@stmtTemplate, '$tempTableName$', '#tvp1'), '$dbName$', QUOTENAME(@db1Name))
exec sp_executesql @stmt
-- Query second database
SET @stmt = REPLACE(REPLACE(@stmtTemplate, '$tempTableName$', '#tvp2'), '$dbName$', QUOTENAME(@db2Name))
exec sp_executesql @stmt

-- Returns TVPs that only exist in one database
SELECT  DISTINCT
        ISNULL(tvp1.schemaName, tvp2.schemaName) schemaName,
        ISNULL(tvp1.tvpName, tvp2.tvpName) tvpName,
        CASE
            WHEN tvp1.tvpName IS NULL THEN 2 -- TVP exists in the second database, but not in the first one
            ELSE 1                           -- TVP exists in the first database, but not in the first one
        END existsInDb
FROM    #tvp1 tvp1
FULL JOIN #tvp2 tvp2
ON      tvp1.schemaName = tvp2.schemaName
        AND tvp1.tvpName = tvp2.tvpName
WHERE   tvp1.tvpName IS NULL
        OR tvp2.tvpName IS NULL

-- Delete TVPs from the #tvp1 that do not exist in the #tvp2
DELETE  #tvp1
FROM    #tvp1
LEFT JOIN   #tvp2
ON      #tvp1.schemaName = #tvp2.schemaName
        AND #tvp1.tvpName = #tvp2.tvpName
WHERE   #tvp2.tvpName IS NULL

-- Delete TVPs from the #tvp2 that do not exist in the #tvp1
DELETE  #tvp2
FROM    #tvp2
LEFT JOIN   #tvp1
ON      #tvp1.schemaName = #tvp2.schemaName
        AND #tvp1.tvpName = #tvp2.tvpName
WHERE   #tvp1.tvpName IS NULL

-- filter out computed column definitions where SQL 2016 or SQL Azure has added parenthesis around the reverse statement preceeding collate
UPDATE #tvp1
SET computedDefinition = REPLACE(REPLACE(computedDefinition, '(reverse(', 'reverse('), ') collate', ' collate')
WHERE CHARINDEX('(reverse(', computedDefinition) > 0 AND CHARINDEX(') collate', computedDefinition) > 0

UPDATE #tvp2
SET computedDefinition = REPLACE(REPLACE(computedDefinition, '(reverse(', 'reverse('), ') collate', ' collate')
WHERE CHARINDEX('(reverse(', computedDefinition) > 0 AND CHARINDEX(') collate', computedDefinition) > 0

-- filter out extra zeros and parenthesis we don't care about
UPDATE #tvp1
SET computedDefinition = REPLACE(REPLACE(REPLACE(ISNULL(computedDefinition, ''), ',0)', ')'), '(0)', '0'),',0)', ')')

UPDATE #tvp2
SET computedDefinition = REPLACE(REPLACE(REPLACE(ISNULL(computedDefinition, ''), ',0)', ')'), '(0)', '0'),',0)', ')')

SELECT  *
FROM
(
    SELECT  ISNULL(tvp1.schemaName,  tvp2.schemaName) schemaName,
            ISNULL(tvp1.tvpName,  tvp2.tvpName) tvpName,
            ISNULL(tvp1.columnName, tvp2.columnName) columnName,
            CASE
                WHEN tvp2.columnName IS NULL THEN 1  -- first db only
                WHEN tvp1.columnName IS NULL THEN 2  -- second db only
                ELSE 3                               -- both dbs
            END existsInDb,
            CASE
                WHEN tvp1.columnName IS NULL THEN 'tvp2 only'
                WHEN tvp2.columnName IS NULL THEN 'tvp1 only'
                WHEN tvp1.isNullable   <> tvp2.isNullable THEN 'isNullable'
                WHEN tvp1.systemType   <> tvp2.systemType THEN 'systemType'
                WHEN tvp1.maxLength    <> tvp2.maxLength THEN 'maxLength'
                WHEN tvp1.precision    <> tvp2.precision THEN 'precision'
                WHEN tvp1.scale        <> tvp2.scale THEN 'scale'
                WHEN tvp1.isAnsiPadded <> tvp2.isAnsiPadded THEN 'isAnsiPadded'
                WHEN tvp1.isComputed   <> tvp2.isComputed THEN 'isComputed'
                WHEN tvp1.isPersisted  <> tvp2.isPersisted  THEN 'isPersisted'
                WHEN ISNULL(tvp1.columnDefault, '') <> ISNULL(tvp2.columnDefault, '') THEN 'columnDefault'
                WHEN ISNULL(tvp1.computedDefinition, '') <>
                     ISNULL(tvp2.computedDefinition, '') THEN 'computedDefinition'
            END reason,
            tvp1.isNullable tvp1isNullable,
            tvp2.isNullable tvp2isNullable,
            tvp1.systemType tvp1systemType,
            tvp2.systemType tvp2systemType,
            tvp1.maxLength    tvp1maxLength,
            tvp2.maxLength    tvp2maxLength,
            tvp1.precision    tvp1precision,
            tvp2.precision    tvp2precision,
            tvp1.scale        tvp1scale,
            tvp2.scale        tvp2scale,
            tvp1.isAnsiPadded tvp1isAnsiPadded,
            tvp2.isAnsiPadded tvp2isAnsiPadded,
            tvp1.isComputed   tvp1isComputed,
            tvp2.isComputed   tvp2isComputed,
            tvp1.isPersisted  tvp1isPersisted,
            tvp2.isPersisted  tvp2isPersisted,
            tvp1.columnDefault tvp1ColumnDefault,
            tvp2.columnDefault tvp2ColumnDefault,
            tvp1.computedDefinition tvp1computedDefinition,
            tvp2.computedDefinition tvp2computedDefinition
    FROM    #tvp1 tvp1
    FULL JOIN #tvp2 tvp2
    ON      tvp1.schemaName = tvp2.schemaName
            AND tvp1.tvpName = tvp2.tvpName
            AND tvp1.columnName = tvp2.columnName) tmp
    WHERE   reason IS NOT NULL

-- Compare stored procedures

CREATE TABLE #proc1 (
    schemaName  SYSNAME NOT NULL,
    name        SYSNAME NOT NULL,
    definition  NVARCHAR(MAX) NULL
)

CREATE TABLE #proc2 (
    schemaName  SYSNAME NOT NULL,
    name        SYSNAME NOT NULL,
    definition  NVARCHAR(MAX) NULL
)

SET @stmt = 'USE ' + QUOTENAME(@db1Name) + '
INSERT  #proc1(schemaName, name, definition)
SELECT  OBJECT_SCHEMA_NAME(object_id), name, OBJECT_DEFINITION(object_id)
FROM    sys.procedures
WHERE   name NOT LIKE ''xxDeadView%''
        AND name NOT IN (
            ''prc_SaveWorkItemsFromValues'',
            ''CreateWorkItemsFromValues'',
            ''CreateWorkItems'',
            ''UpdateWorkItemsFromValues'',
            ''UpdateWorkItems'',
            ''WorkItemApplyChanges'')'

EXEC sp_executeSql @stmt

SET @stmt = 'USE ' + QUOTENAME(@db2Name) + '
INSERT  #proc2(schemaName, name, definition)
SELECT  OBJECT_SCHEMA_NAME(object_id), name, OBJECT_DEFINITION(object_id)
FROM    sys.procedures
WHERE   name NOT LIKE ''xxDeadView%''
        AND name NOT IN (
            ''CreateWorkItemsFromValues'',
            ''CreateWorkItems'',
            ''GetCreteSprintNumber'',
            ''GetSicilySprintNumber'',
            ''GetSicilySprintNumber2'',
            ''GetSprintName'',
            ''prc_SaveWorkItemsFromValues'',
            ''RollupTree'',
            ''TakeSnapshot'',
            ''UpdateTrigger'',
            ''UpdateWorkItemsFromValues'',
            ''UpdateWorkItems'',
            ''WorkItemApplyChanges'')'

EXEC sp_executeSql @stmt

-- returns procedures that only exist in 1 database
SELECT  ISNULL(p1.schemaName, p2.schemaName) schemaName,
        ISNULL(p1.name, p2.name) procName,
        CASE
            WHEN p1.name IS NULL THEN 2 -- proc exists in the second database, but not in the first one
            ELSE 1                      -- proc exists in the first database, but not in the first one
        END existsInDb
FROM    #proc1 p1
FULL JOIN #proc2 p2
ON      p1.schemaName = p2.schemaName
        AND p1.name = p2.name
WHERE   (
            p1.name IS NULL
            OR p2.name IS NULL
        )
        -- These procedures are dropped after migration CTP2->CTP3 is complete so ignore them here to
        -- avoid a race condition between data migration and the schema comparison
        AND ISNULL(p1.name, p2.name) NOT IN ('prc_QueryBuildInformation', 'prc_QueryBuildInformationWithConversion')


-- returns procedures that have different definitions

SELECT  p1.schemaName schemaName,
        p1.name procName,
        p1.definition definition1,
        p2.definition definition2
FROM    #proc1 p1
JOIN    #proc2 p2
ON      p1.schemaName = p2.schemaName
        AND p1.name = p2.name
WHERE   p1.definition <> p2.definition

-- Compare functions

CREATE TABLE #func1 (
    schemaName SYSNAME NOT NULL,
    name SYSNAME NOT NULL,
    definition NVARCHAR(MAX) NULL
)

CREATE TABLE #func2 (
    schemaName SYSNAME NOT NULL,
    name SYSNAME NOT NULL,
    definition NVARCHAR(MAX) NULL
)

SET @stmt = 'USE ' + QUOTENAME(@db1Name) + '
INSERT  #func1(schemaName, name, definition)
SELECT  OBJECT_SCHEMA_NAME(object_id), name, OBJECT_DEFINITION(object_id)
FROM    sys.objects
WHERE   type_desc LIKE ''%FUNCTION%''
        AND name NOT LIKE ''xx%''
        AND name NOT IN (
            ''WorkItemsQueryLatestUsed'',
            ''WorkItemsQueryLatestUsedConstIDs'',
            ''WorkItemsQueryAsOf'',
            ''WorkItemsQueryAsOfConstIDs'',
            ''WorkItemGetComputedColumnsAsOf'',
            ''func_IsDeletedPartition'',
            ''func_IsReadOnlyPartition'',
            ''func_GetEndRangeChar'')'

EXEC sp_executeSql @stmt

SET @stmt = 'USE ' + QUOTENAME(@db2Name) + '
INSERT  #func2(schemaName, name, definition)
SELECT  OBJECT_SCHEMA_NAME(object_id), name, OBJECT_DEFINITION(object_id)
FROM    sys.objects
WHERE   type_desc LIKE ''%FUNCTION%''
        AND name NOT LIKE ''xx%''
        AND name NOT IN (
            ''func_IsDeletedPartition'',
            ''func_IsReadOnlyPartition'',
            ''func_GetEndRangeChar'')'

EXEC sp_executeSql @stmt

-- returns functions that only exist in 1 database
SELECT  ISNULL(f1.schemaName, f2.schemaName) schemaName,
        ISNULL(f1.name, f2.name) funcName,
        CASE
            WHEN f1.name IS NULL THEN 2 -- func exists in the second database, but not in the first one
            ELSE 1                      -- func exists in the first database, but not in the first one
        END existsInDb
FROM    #func1 f1
FULL JOIN #func2 f2
ON      f1.schemaName = f2.schemaName
        AND f1.name = f2.name
WHERE   (
            f1.name IS NULL
            OR f2.name IS NULL
        )

-- returns functions that have different definitions

SELECT  f1.schemaName schemaName,
        f1.name funcName,
        f1.definition definition1,
        f2.definition definition2
FROM    #func1 f1
JOIN    #func2 f2
ON      f1.schemaName = f2.schemaName
        AND f1.name = f2.name
WHERE   f1.definition <> f2.definition

-- Compare Tables
SET @stmtTemplate = '
USE $dbName$
INSERT  $tempTableName$
SELECT  OBJECT_SCHEMA_NAME(tbl.object_id),
        tbl.name,
        tbl.lock_escalation
FROM    $dbName$.sys.tables tbl'

-- Query first database
SET @stmt = REPLACE(REPLACE(@stmtTemplate, '$tempTableName$', '#tbl1'), '$dbName$', QUOTENAME(@db1Name))
exec sp_executesql @stmt
-- Query second database
SET @stmt = REPLACE(REPLACE(@stmtTemplate, '$tempTableName$', '#tbl2'), '$dbName$', QUOTENAME(@db2Name))
exec sp_executesql @stmt

-- returns lockEscalations that have different values
SELECT  tb1.schemaName schemaName,
        tb1.tableName tableName,
        tb1.lockEscalation lockEscalation1,
        tb2.lockEscalation lockEscalation2
FROM    #tbl1 tb1
JOIN    #tbl2 tb2
ON      tb1.schemaName = tb2.schemaName
        AND tb1.tableName = tb2.tableName
WHERE   tb1.lockEscalation <> tb2.lockEscalation

DROP TABLE #db1
DROP TABLE #db2

DROP TABLE #index1
DROP TABLE #index2

DROP TABLE #tvp1
DROP TABLE #tvp2

DROP TABLE #proc1
DROP TABLE #proc2

DROP TABLE #func1
DROP TABLE #func2

DROP TABLE #tbl1
DROP TABLE #tbl2
