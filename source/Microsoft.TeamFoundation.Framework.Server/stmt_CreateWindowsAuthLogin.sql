-- Creates a Windows authentication login if it does not exist
-- Parameter:
--    @loginName - login to create

IF NOT EXISTS(
    SELECT  *
    FROM    sys.syslogins l
    WHERE   l.loginname = @loginName
            AND l.isntname = 1
)
BEGIN
    DECLARE @stmt NVARCHAR(1000)
    SET @stmt = 'CREATE LOGIN ' + QUOTENAME(@loginName) + ' FROM WINDOWS'
    EXEC sp_executesql @stmt
END

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
WHERE   l.loginname = @loginName
