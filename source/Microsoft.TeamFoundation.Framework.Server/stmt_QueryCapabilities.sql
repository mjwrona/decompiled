-- Determines whether this SQL Server supports page compression and online indexing
SELECT  CASE
            WHEN CONVERT(SYSNAME, SERVERPROPERTY('Edition')) LIKE '%Enterprise%'
                OR CONVERT(SYSNAME, SERVERPROPERTY('Edition')) LIKE '%Developer%'
                OR CONVERT(SYSNAME, SERVERPROPERTY('Edition')) LIKE '%Data Center%'
                OR CONVERT(SYSNAME, SERVERPROPERTY('Edition')) LIKE '%SQL Azure%' THEN 3 -- page compression -1 and online indexing = 2
            WHEN CONVERT(SYSNAME, SERVERPROPERTY('ProductVersion')) >= '13.0.4001.0' THEN 1
            ELSE 0
        END Capabilities
