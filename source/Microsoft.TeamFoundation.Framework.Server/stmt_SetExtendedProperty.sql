-- Set the value for the extended property
SET XACT_ABORT ON
DECLARE @status INT

BEGIN TRAN

    IF EXISTS(SELECT * FROM fn_listextendedproperty(@attributeName, default, default, default, default, default, default))
    BEGIN
        EXEC @status = sys.sp_dropextendedproperty @attributeName
        IF (@status <> 0)
        BEGIN
            ROLLBACK TRAN
            RAISERROR(N'Error dropping the extended property', 16, -1)
            RETURN
        END
    END
    
    EXEC @status = sys.sp_addextendedproperty @attributeName, @attributeValue
    IF (@status <> 0)
    BEGIN
        ROLLBACK TRAN
        RAISERROR(N'Error adding the extended property', 16, -1)
        RETURN
    END
    
COMMIT TRAN
