-- Delete the property defined in @attributeName
DECLARE @status INT
IF EXISTS(SELECT * FROM fn_listextendedproperty(@attributeName, default, default, default, default, default, default))
BEGIN
    EXEC @status = sys.sp_dropextendedproperty @attributeName
    IF (@status <> 0)
    BEGIN    
        RAISERROR(N'Error dropping the extended property', 16, -1)
        RETURN
    END
END
