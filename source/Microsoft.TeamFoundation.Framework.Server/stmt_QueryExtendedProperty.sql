-- Read the attribute from the extended database property
-- If the attribute is not found then fall back to tbl_ExtendedAttributes 
SET XACT_ABORT ON

DECLARE @rowCount AS INT

IF EXISTS (SELECT * FROM sys.extended_properties WHERE name = @attributeName AND class = 0)
BEGIN
    -- Retrieve the attribute as an extended property
    SELECT  name,
            value
    FROM    fn_listextendedproperty(@attributeName, default, default, default, default, default, default)
END
ELSE
BEGIN 
   -- If the extended property was not found, fallback to tbl_ExtendedAttributes
    IF (OBJECT_ID('tbl_ExtendedAttributes') IS NOT NULL)
    BEGIN
        -- Retrieve the extended property with @attributeName
        SELECT  name,
                value
        FROM    tbl_ExtendedAttributes
        WHERE   name = @attributeName

        -- If The extended property was not found, return a empty set
        SET @rowCount = @@ROWCOUNT
        IF (@rowCount = 0)
        BEGIN
            SELECT  CONVERT(sysname, '') AS name, CONVERT(sql_variant, '') AS value WHERE 1 = 2
        END
    END
END

