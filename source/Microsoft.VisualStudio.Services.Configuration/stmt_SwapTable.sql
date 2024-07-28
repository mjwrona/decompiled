SET XACT_ABORT ON
DECLARE @status INT
BEGIN TRAN 

DECLARE @stmt NVARCHAR(MAX)

SET @stmt = 'DROP TABLE ' + QUOTENAME(@sourceTable)

EXEC @status = sp_executeSql @stmt

IF (@status <> 0)
BEGIN
    ROLLBACK
    RETURN
END

EXEC @status = sp_rename @targetTable, @sourceTable

IF (@status <> 0)
BEGIN
    ROLLBACK
    RETURN
END

DECLARE @sourceIndex NVARCHAR(257)
DECLARE @targetIndex NVARCHAR(257)

DECLARE IndexCursor CURSOR FAST_FORWARD FOR
SELECT  @sourceTable + '.' + name,
        LEFT(name, LEN(name) - LEN(@suffix))
FROM    sys.indexes
WHERE   object_id = OBJECT_ID(@sourceTable)
        AND name like '%' + REPLACE( @suffix, '_', '[_]')

OPEN IndexCursor
FETCH NEXT FROM IndexCursor INTO @sourceIndex, @targetIndex

WHILE (@@FETCH_STATUS = 0)
BEGIN
    EXEC @status = sp_rename @sourceIndex, @targetIndex

    IF (@status <> 0)
    BEGIN
        ROLLBACK
        RETURN
    END

    FETCH NEXT FROM IndexCursor INTO @sourceIndex, @targetIndex
END

CLOSE IndexCursor
DEALLOCATE IndexCursor
COMMIT TRAN


