-- Creates a login for NT AUTHORITY\NETWORK SERVICE account if it does not exist.
-- This method tries to create NT AUTHORITY\NETWORK SERVICE (English) login first.
-- If create login command fails with 15401 error (Windows NT user or group '<group name>' not found),
-- the script tries to create NT-AUTORITÄT\NETZWERKDIENST (German), and so on.
-- @localizedNetworkServiceNames table contains a list localized NT AUTHORITY\NETWORK SERVICE names.
-- If this scripted failed to create an account after trying every known localized name for NT AUTHORITY\NETWORK SERVICE,
-- this script return empty resultset, otherwise it returns loginname, sid, the hasaccess flag, and whether the login is enabled for NS account.

DECLARE @networkServiceName NVARCHAR(80)
DECLARE @status INT

-- query NT AUTHORITY\NETWORK SERVICE account using well known sid (S-1-5-20)
SELECT  @networkServiceName = loginname 
FROM    sys.syslogins 
WHERE   sid = 0x010100000000000514000000

SET @status = @@ERROR

IF @status <> 0
BEGIN
    RAISERROR (800001, 16, -1, 'CreateNetworkServiceLogin', @status, N'SELECT', N'sys.syslogins')
    RETURN
END

IF @networkServiceName IS NULL
BEGIN
    DECLARE @localizedNetworkServiceNames TABLE
    (
        Name NVARCHAR(80) NOT NULL
    )
--  See http://pathfinderx/ for localized accounts info 
    INSERT @localizedNetworkServiceNames
    VALUES
    ('NT AUTHORITY\NETWORK SERVICE'),
    ('NT-AUTORITÄT\NETZWERKDIENST'),    -- German
    ('AUTORITE NT\SERVICE RÉSEAU'),     -- French
    ('NT AUTHORITY\SERVICIO DE RED'),   -- Spanish
    ('NT AUTHORITY\NETVÆRKSTJENESTE'),  -- Danish
    ('NT AUTHORITY\Netwerkservice'),    -- Dutch
    ('NT-HALLINTA\Verkkopalvelu'),      -- Finish
    ('NT AUTHORITY\HÁLÓZATI SZOLGÁLTATÁS'), -- Hungarian
    ('NT AUTHORITY\SERVIZIO DI RETE'),  -- Italian
    ('NT-MYNDIGHET\NETTVERKSTJENESTE'), -- Norwegian
    ('ZARZĄDZANIE NT\USŁUGA SIECIOWA'), -- Polish
    ('NT AUTHORITY\Serviço de rede'),   -- Portuguese
    ('AUTORIDADE NT\NETWORK SERVICE'),  -- Portuguese(Brazil)
    ('NT INSTANS\NETWORK SERVICE')      -- Swedish

    IF @status <> 0
    BEGIN
        RAISERROR (800001, 16, -1, 'CreateNetworkServiceLogin', @status, N'INSERT', N'@localizedNetworkServiceNames')
        RETURN
    END

    DECLARE networkServiceCursor CURSOR FOR
        SELECT  Name
        FROM    @localizedNetworkServiceNames

    OPEN networkServiceCursor

    FETCH NEXT FROM networkServiceCursor
    INTO    @networkServiceName

    WHILE @@FETCH_STATUS = 0
    BEGIN
        DECLARE @stmt NVARCHAR(1000)
        SET @stmt = 'CREATE LOGIN ' + QUOTENAME(@networkServiceName) + ' FROM WINDOWS'
        BEGIN TRY
            EXEC sp_executesql @stmt
            BREAK
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
            -- Windows NT user or group '<group or user name>' not found.
            IF @errorNumber <> 15401 
            BEGIN
                RAISERROR (@errorMessage, @errorSeverity, @errorState)
                BREAK
            END
        END CATCH
        FETCH NEXT FROM networkServiceCursor
        INTO    @networkServiceName
    END

    CLOSE networkServiceCursor
    DEALLOCATE networkServiceCursor
END

SELECT  l.loginName loginName,
        l.sid sid,
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
WHERE   l.sid = 0x010100000000000514000000     -- S-1-5-20