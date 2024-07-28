-- Checks if the login that is used to connect to SQL Server/Azure instance is an sa (principal id = 1)
-- Result: BIT isSA 

BEGIN TRY
    IF EXISTS(  SELECT  *
                FROM    sys.sql_logins l
                JOIN    sys.database_principals dp
                ON      l.sid = dp.sid
                WHERE   dp.principal_id = DATABASE_PRINCIPAL_ID()
                AND     l.principal_id = 1)
    BEGIN
        SELECT CONVERT(BIT, 1) isSA
    END
    ELSE
        SELECT CONVERT(BIT, 0) isSA
END TRY
        
BEGIN CATCH            
    DECLARE @errorMessage NVARCHAR(4000)
    DECLARE @errorSeverity INT
    DECLARE @errorState INT
    DECLARE @errorNumber INT

    SELECT 
        @errorMessage = ERROR_MESSAGE(),
        @errorSeverity = ERROR_SEVERITY(),
        @errorState = ERROR_STATE(),
        @errorNumber = ERROR_NUMBER()

    -- Msg 229, Level 14: The SELECT permission was denied on the object 'sql_logins', database 'master', schema 'sys'.
    IF @errorNumber <> 229
    BEGIN
        RAISERROR (@ErrorMessage, -- Message text.
               @ErrorSeverity, -- Severity.
               @ErrorState -- State.
               );
    END
    ELSE
    BEGIN
        SELECT CONVERT(BIT, 0) isSA
    END
END CATCH