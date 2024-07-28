--Returns a table with 2 columns (DataFolderPath, LogFolderPath) and a single row

DECLARE @dataPath    NVARCHAR(512)
DECLARE @logPath     NVARCHAR(512)

DECLARE @hkeyLocal          NVARCHAR(18) = N'HKEY_LOCAL_MACHINE'
DECLARE @msSqlServerRegPath NVARCHAR(31) = N'SOFTWARE\Microsoft\MSSQLServer'
DECLARE @instanceRegPath    SYSNAME      = @mssqlServerRegPath + N'\MSSQLServer'

exec master.dbo.xp_instance_regread @hkeyLocal, @instanceRegPath, N'DefaultData', @dataPath OUTPUT
exec master.dbo.xp_instance_regread @hkeyLocal, @instanceRegPath, N'DefaultLog',  @logPath OUTPUT

if (@dataPath IS NULL)
BEGIN
        -- Read the location of data files of master database
        SELECT  TOP 1 @dataPath = SUBSTRING(physical_name, 1, LEN(physical_name) - CHARINDEX('\', REVERSE(physical_name)))
        FROM    sys.master_files 
        WHERE   type = 0            -- 0 = data file
        AND     database_id = 1     -- 1 = master database id
        ORDER BY file_id
END

IF (@logPath IS NULL)
BEGIN
        -- Read the location of data files of master database
        SELECT  TOP 1 @logPath = SUBSTRING(physical_name, 1, LEN(physical_name) - CHARINDEX('\', REVERSE(physical_name)))
        FROM    sys.master_files 
        WHERE   type = 1            -- 1 = log file
        AND     database_id = 1     -- 1 = master database id
        ORDER BY file_id
END

SELECT @dataPath as DataFolderPath, @logPath as LogFolderPath