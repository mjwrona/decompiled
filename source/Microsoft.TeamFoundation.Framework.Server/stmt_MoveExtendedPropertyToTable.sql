-- This statement will move an extended property on the database into tbl_ExtendedAttributes.
-- This is executed during database export on devfabric, bacpac format does not support extended properties, they must be in a table.
IF CAST(SERVERPROPERTY('Edition') AS SYSNAME) COLLATE DATABASE_DEFAULT <> N'SQL Azure'
BEGIN
    BEGIN TRAN
        DECLARE @attributeValue SQL_VARIANT
        -- Retrieve the extended property with @attributeName
        SELECT  @attributeValue = value
        FROM    fn_listextendedproperty(@attributeName, default, default, default, default, default, default)

        DECLARE @createTable NVARCHAR(MAX) = 
                'CREATE TABLE tbl_ExtendedAttributes 
                (
                    Name    SYSNAME     CONSTRAINT PK_tbl_ExtendedAttributes PRIMARY KEY CLUSTERED, 
                    Value   SQL_VARIANT
                )'

        IF NOT EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('tbl_ExtendedAttributes'))
        BEGIN
            BEGIN TRY
                EXEC sp_executesql @createTable 
            END TRY
            BEGIN CATCH
                -- 2714 - There is already an object named 'tbl_ExtendedAttributes' in the database.
                IF ERROR_NUMBER() <> 2714
                BEGIN
                    -- Run statement again to raise error to the caller
                    EXEC sp_executesql @createTable 
                END
            END CATCH
        END
        IF EXISTS(SELECT * FROM tbl_ExtendedAttributes WITH(UPDLOCK, HOLDLOCK) WHERE name = @attributeName)
        BEGIN
            UPDATE  tbl_ExtendedAttributes
            SET     value = @attributeValue
            WHERE   name = @attributeName
        END
        ELSE
        BEGIN
            INSERT INTO tbl_ExtendedAttributes
            VALUES      (@attributeName, @attributeValue)
        END

        -- Delete the extended property.
        EXEC sys.sp_dropextendedproperty @attributeName
    COMMIT TRAN
END