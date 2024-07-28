-- Renames the database
-- Parameter: @dbName
-- Parameter: @newName (input/output)
SET NOCOUNT ON

DECLARE @sql AS NVARCHAR(4000)
DECLARE @status AS INT
DECLARE @newNameCopy AS SYSNAME

SET @newNameCopy = @newName
PRINT 'Setting the database to a single user mode...'
SET @sql = 'ALTER DATABASE ' + QUOTENAME(@dbName) + ' SET SINGLE_USER WITH ROLLBACK IMMEDIATE'
EXEC(@sql)

SET @status = @@ERROR

IF @status <> 0 
BEGIN
    PRINT 'Attempt to set the database to a single user mode failed.'
    RETURN
END

DECLARE @attempt as INT
SET @attempt = 0
WHILE @attempt < 32767
BEGIN
    IF @attempt > 0
    BEGIN
        SET @newName = @newNameCopy + '_' + CONVERT(VARCHAR(5), @attempt)
    END
    
    PRINT 'Renaming ' + @dbname + ' to ' + @newName
    SET @sql = 'ALTER DATABASE ' + QUOTENAME(@dbName) + ' MODIFY NAME = ' + QUOTENAME(@newName)
    BEGIN TRY
    EXEC(@sql)
    SET @status = 0
    BREAK
    END TRY
    BEGIN CATCH
        SET @status = ERROR_NUMBER()
        IF @status <> 1801 -- 1801 - the database already exists
        BEGIN
            DECLARE @ErrorMessage NVARCHAR(4000);
            DECLARE @ErrorSeverity INT;
            DECLARE @ErrorState INT;

            SELECT  @ErrorMessage = ERROR_MESSAGE(),
                    @ErrorSeverity = ERROR_SEVERITY(),
                    @ErrorState = ERROR_STATE();

            RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
            BREAK
        END
        ELSE
        BEGIN
            PRINT 'Error: ' + ERROR_MESSAGE()
        END
    END CATCH
    SET @attempt = @attempt + 1
END

IF @status <> 0 
BEGIN
    -- Failed to rename the database. Switch database back multiuser mode and return.
    PRINT 'Database rename failed. Setting the database to multi user mode...'
    SET @sql = 'ALTER DATABASE ' + QUOTENAME(@dbName) + ' SET MULTI_USER'
    EXEC(@sql)
    RETURN
END

IF (SELECT  COUNT(file_id) 
    FROM    sys.master_files
    WHERE   database_id = DB_ID(@newName) AND name = @dbName + '_log') = 1 AND
   (SELECT  COUNT(file_id) 
    FROM    sys.master_files
    WHERE   database_id = DB_ID(@newName) AND name = @dbName) = 1
BEGIN
    -- Eat errors during rename of logical files.
    BEGIN TRY
        PRINT 'Modifying logical file names...'
        SET @sql = 'ALTER DATABASE ' + QUOTENAME(@newName) + ' MODIFY FILE (NAME = ' + QUOTENAME(@dbName) + ', NEWNAME = ' + QUOTENAME(@newName) + ')'
        EXEC(@sql)
        SET @sql = 'ALTER DATABASE ' + QUOTENAME(@newName) + ' MODIFY FILE (NAME = ' + QUOTENAME(@dbName + '_log') + ', NEWNAME = ' + QUOTENAME(@newName + '_log') + ')'
        EXEC(@sql)
    END TRY
    BEGIN CATCH
        PRINT 'Modify logical file name failed.'
        PRINT 'Error: ' + CONVERT(NVARCHAR(20), ERROR_NUMBER())
        PRINT 'Message: ' + ERROR_MESSAGE()
    END CATCH
END

PRINT 'Setting the database to multi user mode...'
SET @sql = 'ALTER DATABASE ' + QUOTENAME(@newName) + ' SET MULTI_USER'
EXEC(@sql)

SET @status = @@ERROR
IF @status = 0
BEGIN
    PRINT 'The database has been set to multi user mode.'
END
