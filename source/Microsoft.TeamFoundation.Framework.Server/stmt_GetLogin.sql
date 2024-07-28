-- Queries login info for the specified login
--
-- Parameter(s);
--  @loginName - SQL Server/SQL Azure login name

IF (CAST(SERVERPROPERTY('Edition') AS SYSNAME) COLLATE DATABASE_DEFAULT = N'SQL Azure')
BEGIN
    SELECT  l.name loginName,
            l.sid,
            CONVERT(BIT, 1) hasAccess,
            ~is_disabled enabled,
            CONVERT(BIT, 0) isSysAdmin,
            CONVERT(BIT, 0) isNTGroup,
            CONVERT(BIT, 0) isNTUser,
            DATEADD(second, DATEDIFF(second, GETDATE(), GETUTCDATE()), create_date) AS utcCreateDate,
            DATEADD(second, DATEDIFF(second, GETDATE(), GETUTCDATE()), modify_date) AS utcModifyDate
    FROM    sys.sql_logins l
    WHERE   l.name = @loginName
END
ELSE
BEGIN
    SELECT  l.loginname loginName,
            l.sid,
            CONVERT(BIT, l.hasaccess) hasAccess,
            ~sp.is_disabled enabled,
            CONVERT(BIT, l.sysadmin) isSysAdmin,
            CONVERT(BIT, l.isntgroup) isNTGroup,
            CONVERT(BIT, l.isntuser) isNTUser,
            DATEADD(second, DATEDIFF(second, GETDATE(), GETUTCDATE()), sp.create_date) AS utcCreateDate,
            DATEADD(second, DATEDIFF(second, GETDATE(), GETUTCDATE()), sp.modify_date) AS utcModifyDate
    FROM    sys.syslogins l
    JOIN    sys.server_principals sp
    ON      l.sid = sp.sid
    WHERE   loginName = @loginName
END