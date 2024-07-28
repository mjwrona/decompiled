/*
** Create contained AAD database user for SQL JIT service Account
*/

-- DECLARE @serviceAccountName SYSNAME -- Sql Jit service account name
-- DECLARE @serviceAccountId UNIQUEIDENTIFIER  -- Sql Jit service account object ID
-- DECLARE @isSecurityGroup BIT --  is service account a security group

-- We have to use the form "CREATE USER <user> WITH SID" to create AAD user using Sql Auth login, which requires to provide SID (which is a constant).
DECLARE @sid VARBINARY(85) = CAST(@serviceAccountId as VARBINARY(85)) -- SID is varbinary(85)
DECLARE @type NVARCHAR(1) = 'E'  -- type E for external user

IF (@isSecurityGroup = 1)
BEGIN
    SET @type = 'X' -- type X for security group
END

DECLARE @stmt NVARCHAR(MAX)
-- If there is already a user with same SID, just return the user name
IF NOT EXISTS (SELECT name FROM sys.database_principals WHERE sid = @sid)
BEGIN
    IF EXISTS (SELECT name FROM sys.database_principals WHERE name = @serviceAccountName)
    BEGIN
      SET @stmt = 'DROP USER ' + QUOTENAME(@serviceAccountName)
      EXEC sp_executeSql @stmt
    END

    SET @stmt = 'CREATE USER ' + QUOTENAME(@serviceAccountName) + ' WITH SID = ' +  CONVERT(VARCHAR(MAX), @sid, 1) + ', TYPE = ' + @type
    EXEC sp_executeSql @stmt
END

SELECT  name
FROM    sys.database_principals
WHERE   sid = @sid