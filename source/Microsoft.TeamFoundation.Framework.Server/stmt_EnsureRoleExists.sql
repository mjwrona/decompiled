/*
** Ensures that role specified by @roleName parameter exists. 
*/
IF(NOT EXISTS(SELECT    * 
               FROM     sys.database_principals 
               WHERE    type = 'R' 
                        AND name = @roleName))
BEGIN
    DECLARE @stmt NVARCHAR(1000)
    SET @stmt = 'CREATE ROLE ' + QUOTENAME(@roleName) + ' AUTHORIZATION dbo'
    EXEC sp_executesql @stmt
END