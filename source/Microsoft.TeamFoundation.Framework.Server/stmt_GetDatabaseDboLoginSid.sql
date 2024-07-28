-- Returns the owner_sid of a database
--
-- Parameter(s);
--  @databaseName SYSNAME - Database to query.

SELECT	owner_sid
FROM	sys.databases
WHERE   name = @databaseName
