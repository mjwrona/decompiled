/*
** This code rebuilds disabled indexes on the specified table.
** Parameters:
**  @tableName SYSNAME  - The name of the table.
**  @schemaName SYSNAME - Schema in which table is defined.
**  @online BIT         - If set to 1 then indexes should be rebuilt online, provided that SQL Server supports online index creation.
*/
SET XACT_ABORT ON

DECLARE @sql            NVARCHAR(MAX)
DECLARE @indexName      SYSNAME
DECLARE @compressionSupported BIT = 1

IF ((CONVERT(VARCHAR(MAX), SERVERPROPERTY('Edition')) NOT LIKE '%Enterprise%' 
    AND CONVERT(VARCHAR(MAX), SERVERPROPERTY('Edition')) NOT LIKE '%Developer%'
    AND CONVERT(VARCHAR(MAX), SERVERPROPERTY('Edition')) NOT LIKE '%Data Center%'
    AND CONVERT(VARCHAR(MAX), SERVERPROPERTY('Edition')) NOT LIKE '%Azure%'))
BEGIN
    -- SQL Server 2016 SP1 Std/Express supports page compression, but not online indexing.
    IF (CONVERT(VARCHAR(MAX), SERVERPROPERTY('ProductVersion')) < '13.0.4001.0')
    BEGIN
        SET @compressionSupported = 0
    END
    SET @online = 0
END

IF (OBJECT_ID('dbo.tbl_Compression') IS NULL)
BEGIN
    SET @compressionSupported = 0
END


DECLARE IndexCursor CURSOR LOCAL FAST_FORWARD FOR
SELECT  si.name
FROM    sys.indexes si
WHERE   si.object_id = OBJECT_ID(QUOTENAME(@schemaName) + '.' + QUOTENAME(@tableName))
        AND si.is_disabled = 1

OPEN IndexCursor
FETCH NEXT FROM IndexCursor INTO @indexName

WHILE (@@FETCH_STATUS = 0)
BEGIN
    DECLARE @startTime DATETIME = GETUTCDATE()
    RAISERROR ('Rebuilding index: %s', 10, 0, @indexName) WITH NOWAIT

    SET @sql = 'ALTER INDEX ' + QUOTENAME(@indexName) + ' ON ' + QUOTENAME(@schemaName) + '.' + QUOTENAME(@tableName) + ' REBUILD '

    IF (@online = 1)
    BEGIN
        SET @sql = @sql + 'WITH(ONLINE=ON'
    END
    ELSE
    BEGIN
        SET @sql = @sql + 'WITH(ONLINE=OFF'
    END

    IF (@compressionSupported = 1)
    BEGIN
        IF (EXISTS (SELECT  *
                    FROM    tbl_Compression
                    WHERE   SchemaName = @schemaName
                            -- Make sure that when enable indexes on a _Dataspace tables, we use compression.
                            AND TableName = REPLACE(@tableName, '_Dataspace', '')
                            AND IndexName = REPLACE(@indexName, '_Dataspace', '')))
        BEGIN
            SET @sql = @sql + ', DATA_COMPRESSION=PAGE'
        END
    END

    SET @sql = @sql + ', MAXDOP=0)'
	
	RAISERROR (@sql, 10, 0) WITH NOWAIT
    EXEC sp_executesql @sql

    DECLARE @elapsedTime NVARCHAR(50) = CONVERT(NVARCHAR(50), GETUTCDATE() - @startTime, 14)
    RAISERROR ('Index has been rebuilt: %s. Elapsed time: %s', 10, 0, @indexName, @elapsedTime) WITH NOWAIT

    FETCH NEXT FROM IndexCursor INTO @indexName
END

