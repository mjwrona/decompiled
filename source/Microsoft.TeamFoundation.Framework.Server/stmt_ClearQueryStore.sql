-- Copyright (c) Microsoft Corporation.  All rights reserved.

-- Parameters:
--DECLARE @databaseName            SYSNAME -- Name of the database

DECLARE @productVersion INT = CONVERT(INT, LEFT(CONVERT(SYSNAME, SERVERPROPERTY('ProductVersion')), CHARINDEX('.', CONVERT(SYSNAME, SERVERPROPERTY('ProductVersion')), 0) - 1))

-- SQL Server Query Store is supported in SQL Azure 12 and later, and SQL Server 2016 CTP 2 and later
IF ((CONVERT(SYSNAME, SERVERPROPERTY('Edition')) LIKE '%SQL Azure%' AND @productVersion >= 12)
    OR @productVersion >= 13)
BEGIN
    DECLARE @statement NVARCHAR(MAX) = 'ALTER DATABASE ' + QUOTENAME(@databaseName) + ' SET QUERY_STORE CLEAR'

    PRINT @statement

    EXEC sp_executesql @statement
END
ELSE
BEGIN
    PRINT 'Skipping clearing Query Store because it is not supported by this version of SQL Server'
END