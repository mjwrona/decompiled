-- Queries login information for database users in the specified role
-- This statement is not supported on Sql Azure
--
-- Parameter(s);
--  @roleName - database role

SELECT  l.name loginName, 
        l.sid, 
        CONVERT(BIT, l.hasaccess) hasAccess,
        ~sp.is_disabled enabled,
        CONVERT(BIT, l.sysadmin) isSysAdmin,
        CONVERT(BIT, l.isntgroup) isNTGroup,
        CONVERT(BIT, l.isntuser) isNTUser,
        DATEADD(second, DATEDIFF(second, GETDATE(), GETUTCDATE()), sp.create_date) AS utcCreateDate,
        DATEADD(second, DATEDIFF(second, GETDATE(), GETUTCDATE()), sp.modify_date) AS utcModifyDate
FROM    sys.syslogins l
JOIN    sys.database_principals dp
ON      l.sid = dp.sid
JOIN    sys.server_principals sp
ON      l.sid = sp.sid
JOIN    sys.database_role_members drm
ON      dp.principal_id = drm.member_principal_id
JOIN    sys.database_principals dpr                -- database role members role
ON      dpr.principal_id = drm.role_principal_id
WHERE   dpr.name = @roleName