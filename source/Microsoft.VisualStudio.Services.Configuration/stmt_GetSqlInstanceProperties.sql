-- Retrives SQL Instance properties: DefaultDataPath, DefaultLogPath, MasterDbPath, MasterDbLogPath
-- No input parameters

DECLARE @hkeyLocal          NVARCHAR(18)
DECLARE @instanceRegPath    SYSNAME
DECLARE @mssqlServerRegPath NVARCHAR(31)

SELECT  @hkeyLocal=N'HKEY_LOCAL_MACHINE'
SELECT  @mssqlServerRegPath=N'SOFTWARE\Microsoft\MSSQLServer'
SELECT  @instanceRegPath=@mssqlServerRegPath + N'\MSSQLServer'

DECLARE @defaultDataPath    NVARCHAR(512)
DECLARE @defaultLogPath     NVARCHAR(512)
DECLARE @masterDbPath       NVARCHAR(512)
DECLARE @masterDbLogPath    NVARCHAR(512)
DECLARE @backupDirectory    NVARCHAR(512)
DECLARE @BackupCompressionDefault INT
DECLARE @isFullTextInstalled      INT
DECLARE @hostPlatform       NVARCHAR(256)
DECLARE @hostDistribution   NVARCHAR(256)
DECLARE @hostRelease        NVARCHAR(256)
DECLARE @sql                NVARCHAR(MAX)
SET @BackupCompressionDefault = -1

-- Since xp_instance_regread is not available on SQL Azure,
-- we need to use dynamic SQL here

IF CAST(SERVERPROPERTY('Edition') AS SYSNAME) COLLATE DATABASE_DEFAULT <> N'SQL Azure'
BEGIN
    DECLARE @stmt NVARCHAR(MAX)
    DECLARE @parmDefinition NVARCHAR(MAX)
    -- Read DefaultData and DefaultLog. These values are described in 
    -- http://support.microsoft.com/default.aspx/kb/836873
    SET @stmt = '
        exec master.dbo.xp_instance_regread @hkeyLocal, @instanceRegPath, N''DefaultData'', @defaultDataPath OUTPUT
        exec master.dbo.xp_instance_regread @hkeyLocal, @instanceRegPath, N''DefaultLog'',  @defaultLogPath OUTPUT'
    
    SET @parmDefinition = N'@hkeyLocal NVARCHAR(18), @instanceRegPath SYSNAME, @defaultDataPath NVARCHAR(512) OUTPUT, @defaultLogPath NVARCHAR(512) OUTPUT'
    EXEC sp_executesql @stmt, @parmDefinition, @hkeyLocal = @hkeyLocal, @instanceRegPath = @instanceRegPath, @defaultDataPath = @defaultDataPath OUTPUT, @defaultLogPath = @defaultLogPath OUTPUT

    -- Read locations of data and log files of master database
    SELECT  TOP 1 @masterDbPath = SUBSTRING(physical_name, 1, LEN(physical_name) - CHARINDEX('\', REVERSE(REPLACE(physical_name, '/', '\'))))
    FROM    sys.master_files 
    WHERE   type = 0            -- 0 = data file
    AND     database_id = 1     -- 1 = master database id
    ORDER BY file_id

    SELECT  TOP 1 @masterDbLogPath = SUBSTRING(physical_name, 1, LEN(physical_name) - CHARINDEX('\', REVERSE(REPLACE(physical_name, '/', '\'))))
    FROM    sys.master_files 
    WHERE   type = 1            -- 1 = log file
    AND     database_id = 1     -- 1 = master database id
    ORDER BY file_id

    -- Read backup directory
    SET @stmt = '
        exec master.dbo.xp_instance_regread @hkeyLocal, @instanceRegPath, N''BackupDirectory'', @backupDirectory OUTPUT'
    SET @parmDefinition = N'@hkeyLocal NVARCHAR(18), @instanceRegPath SYSNAME, @backupDirectory NVARCHAR(512) OUTPUT'
    EXEC sp_executesql @stmt, @parmDefinition, @hkeyLocal = @hkeyLocal, @instanceRegPath = @instanceRegPath, @backupDirectory = @backupDirectory OUTPUT

    SET @BackupCompressionDefault = CONVERT(INT, ISNULL((SELECT value FROM sys.configurations WHERE name = 'backup compression default'), -1))
    SET @sql = 'SELECT @result = CONVERT(INT, SERVERPROPERTY(''IsFullTextInstalled''))'
END
ELSE IF SERVERPROPERTY('ProductVersion') >= '12'
    SET @sql = 'SELECT @result = CONVERT(INT, FULLTEXTSERVICEPROPERTY(''IsFullTextInstalled''))'
ELSE
    SET @sql = 'SELECT @result = CONVERT(INT, SERVERPROPERTY(''IsFullTextInstalled''))'

EXEC sp_executesql @sql, N'@result BIT OUTPUT', @result = @isFullTextInstalled OUTPUT


IF OBJECT_ID(N'sys.dm_os_host_info') IS NOT NULL
BEGIN
    SELECT  @hostPlatform = host_platform,
            @hostDistribution = host_distribution,
            @hostRelease = host_release
    FROM    sys.dm_os_host_info
END
ELSE IF CAST(SERVERPROPERTY('Edition') AS SYSNAME) COLLATE DATABASE_DEFAULT <> N'SQL Azure'
BEGIN
    SELECT  @hostPlatform = 'Windows',
            @hostDistribution = 'Windows',
            @hostRelease = windows_release
    FROM    sys.dm_os_windows_info
END
ELSE
BEGIN
    SELECT  @hostPlatform = 'Windows',
            @hostDistribution = 'Azure',
            @hostRelease = ''
END

SELECT  SERVERPROPERTY('ProductVersion')    ProductVersion,
        SERVERPROPERTY ('ProductLevel')     ProductLevel,
        SERVERPROPERTY ('Edition')          Edition,
        SERVERPROPERTY('EngineEdition')     EngineEdition,
        SERVERPROPERTY('InstanceName')      InstanceName,
        @isFullTextInstalled                IsFullTextInstalled,
        SERVERPROPERTY('IsClustered')       IsClustered,
        SERVERPROPERTY('Collation')         Collation,
        SERVERPROPERTY('LicenseType')       LicenseType,
        SERVERPROPERTY('NumLicenses')       NumLicenses,
        SERVERPROPERTY('MachineName')       MachineName,
        ISNULL(@defaultDataPath, N'')       DefaultDataPath,
        ISNULL(@defaultLogPath, N'')        DefaultLogPath,
        ISNULL(@masterDbPath, N'')          MasterDbPath,
        ISNULL(@masterDbLogPath, N'')       MasterDbLogPath,
        ISNULL(@backupDirectory, N'')       BackupDirectory,
        @BackupCompressionDefault           BackupCompressionDefault,
        @hostPlatform                       HostPlatform,
        @hostDistribution                   HostDistribution,
        @hostRelease                        HostRelease
