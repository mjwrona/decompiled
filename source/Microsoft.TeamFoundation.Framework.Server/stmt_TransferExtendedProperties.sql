-- moves all rows from tbl_extendedAttributes to database extended properties
-- used during hosted collection import, then drops tbl_extendedAttributes

BEGIN TRAN
DECLARE propCursor CURSOR FAST_FORWARD FOR
SELECT  name,value
FROM    tbl_ExtendedAttributes

OPEN propCursor
declare @name NVARCHAR(128)
declare @value SQL_VARIANT

FETCH NEXT FROM propCursor
into @name, @value

WHILE @@FETCH_STATUS = 0
BEGIN
      EXEC sp_getapplock @Resource = @name, @LockMode = 'Exclusive', @LockOwner = 'Transaction'
      IF EXISTS(SELECT * FROM fn_listextendedproperty(@name, default, default, default, default, default, default))
      BEGIN
          EXEC sys.sp_dropextendedproperty @name
      END
    
      EXEC sys.sp_addextendedproperty @name, @value

    FETCH NEXT FROM propCursor
    into @name, @value
END
CLOSE propCursor
DEALLOCATE propCursor
DROP TABLE tbl_ExtendedAttributes

COMMIT TRAN
