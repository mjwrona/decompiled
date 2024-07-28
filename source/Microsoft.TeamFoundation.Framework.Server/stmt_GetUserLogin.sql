-- Queries login information for the specified user.
--
-- Parameter(s);
--  @userName - database user name
-- On Sql Azure this script only works in master datababase.

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
JOIN    sys.database_principals p
ON      l.sid = p.sid
JOIN    sys.server_principals sp
ON      l.sid = sp.sid
WHERE   p.name = @userName