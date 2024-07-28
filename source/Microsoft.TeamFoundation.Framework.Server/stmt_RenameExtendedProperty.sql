
-- Set the value for the extended property
SET XACT_ABORT ON

-- Initialize the ProcedureName for error messages.
DECLARE @rowCount       INT
DECLARE @attributeValue SQL_VARIANT
DECLARE @status         INT

BEGIN TRAN


SELECT  @attributeValue = Value
FROM    fn_listextendedproperty(@attributeName, default, default, default, default, default, default)

SET @rowCount = @@ROWCOUNT

IF (@rowCount = 1)
BEGIN
    EXEC @status = sys.sp_dropextendedproperty @attributeName

    IF (@status <> 0)
    BEGIN
        ROLLBACK TRAN
        RAISERROR(N'Error dropping the extended property', 16, -1)
        RETURN
    END

    EXEC @status = sys.sp_addextendedproperty @newAttributeName, @attributeValue
    IF (@status <> 0)
    BEGIN
        ROLLBACK TRAN
        RAISERROR(N'Error adding the extended property', 16, -1)
        RETURN
    END
END

SELECT @rowCount

COMMIT TRAN
