/*
** This code disables non-clustered on the specified table.
** Parameters:
**  @tableName SYSNAME              - The name of the table.
**  @schemaName SYSNAME             - Schema in which table is defined.
**  @excludeIndexes NVARCHAR(MAX)    - Comma separated names of indexes that should not be disabled.
*/
SET XACT_ABORT ON

DECLARE @indexName SYSNAME
DECLARE @sql NVARCHAR(MAX)

DECLARE @exclusions TABLE (
    IndexName SYSNAME
)

INSERT  @exclusions(IndexName)
SELECT  y.i.value('(./text())[1]', 'SYSNAME')
FROM    (SELECT x = CONVERT(XML, '<i>'+ REPLACE(@excludeIndexes, ',', '</i><i>') + '</i>').query('.')) a
        CROSS APPLY x.nodes('i') AS y(i)

DECLARE IndexCursor CURSOR LOCAL FAST_FORWARD FOR
SELECT  si.name
FROM    sys.indexes si
WHERE   si.object_id = OBJECT_ID(QUOTENAME(@schemaName) + '.' + QUOTENAME(@tableName))
        AND si.is_disabled = 0
        AND si.type = 2
        AND si.is_unique_constraint = 0
        AND NOT EXISTS (SELECT *
                        FROM    @exclusions e
                        WHERE   si.name = e.IndexName)

OPEN IndexCursor
FETCH NEXT FROM IndexCursor INTO @indexName

WHILE (@@FETCH_STATUS = 0)
BEGIN
    SET @sql = 'ALTER INDEX ' + QUOTENAME(@indexName) + ' ON ' + QUOTENAME(@schemaName) + '.' + QUOTENAME(@tableName) + ' DISABLE'
    EXEC sp_executesql @sql
    RAISERROR('Index has been disabled: %s.', 10, 0, @indexName) WITH NOWAIT
    FETCH NEXT FROM IndexCursor INTO @indexName
END

