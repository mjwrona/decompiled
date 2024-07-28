-- Creates a physical database on SQL Server
-- Parameters:
-- @databaseName NVARCHAR   - the name of the database to be created
-- @collation               - collation of the database.

DECLARE @statement        NVARCHAR(MAX)
DECLARE @collationClause  NVARCHAR(MAX) = N''
DECLARE @safeCollation    SYSNAME
DECLARE @alterCollation   BIT = 0

IF @collation IS NOT NULL
BEGIN
    SET @safeCollation = (
        SELECT  Name
        FROM    fn_helpcollations()
        WHERE   Name = @collation)

    IF @safeCollation IS NOT NULL
    BEGIN
        SET @collationClause = ' COLLATE ' + @safeCollation
    END
    ELSE
    BEGIN
        -- Chinese_Simplified_Pinyin_160 and Chinese_Simplified_Stroke_Order_160 collations were implemented in SQL Server 2022 CU12
        -- Since they were implemented in a cummulative update and users must be able to restore SQL Server 2022 CU12+ databases on SQL Server 2022 RTM,
        -- these collations are hidden. To create database with these collations we need to do the following:
        -- CREATE DATABASE <dbName>
        -- USE <dbName>
        -- EXEC sp_db_gb18030_unicode_collations <dbName>, 'ON'
        -- ALTER DATABASE <dbName> COLLATE <collation>
        IF (@collation NOT IN ('Chinese_Simplified_Pinyin_160_CI_AS_SC_UTF8', 'Chinese_Simplified_Stroke_Order_160_CI_AS_SC_UTF8') OR SERVERPROPERTY('ProductVersion') < '16.0.4115.4')
        BEGIN
            RAISERROR('Unable to create database: The supplied collation is not supported', 16, 1)
            RETURN
        END
        SET @alterCollation = 1
    END
END
ELSE
BEGIN
    SELECT @safeCollation = REPLACE(CONVERT(NVARCHAR(100), SERVERPROPERTY('collation')), 'CI_AI', 'CI_AS')
    SET @collationClause = ' COLLATE ' + @safeCollation
END

DECLARE @dataPath    NVARCHAR(512)
DECLARE @logPath     NVARCHAR(512)

DECLARE @dataFileName           NVARCHAR(512) -- the full name of the data file.
DECLARE @logFileName            NVARCHAR(512) -- the full name of the log file. Can be NULL. Used only when dataFileName is not NULL.
DECLARE @logicalDataFileName    NVARCHAR(512) -- logical name of the data file. Used only when dataFileName is not NULL.
DECLARE @logicalLogFileName     NVARCHAR(512) -- logical name of the log file. Used only when dataFileName is not NULL.

DECLARE @attempt INT = 0

EXEC sp_getapplock @Resource = 'TfsCreateDB', @LockMode = 'Exclusive', @LockOwner = 'Session', @LockTimeout = -1

WHILE (0 = 0)
BEGIN
    IF @attempt = 0
    BEGIN
        SELECT @statement = 'CREATE DATABASE ' + quotename(@databaseName) + @collationClause
    END
    ELSE
    BEGIN
        IF (@attempt = 1)
        BEGIN
            DECLARE @hkeyLocal          NVARCHAR(18) = N'HKEY_LOCAL_MACHINE'
            DECLARE @msSqlServerRegPath NVARCHAR(31) = N'SOFTWARE\Microsoft\MSSQLServer'
            DECLARE @instanceRegPath    SYSNAME      = @mssqlServerRegPath + N'\MSSQLServer'

            -- Read DefaultData and DefaultLog. These values are described in
            -- http://support.microsoft.com/default.aspx/kb/836873
            exec master.dbo.xp_instance_regread @hkeyLocal, @instanceRegPath, N'DefaultData', @dataPath OUTPUT
            exec master.dbo.xp_instance_regread @hkeyLocal, @instanceRegPath, N'DefaultLog',  @logPath OUTPUT

            if (@dataPath IS NULL)
            BEGIN
                    -- Read the location of data files of master database
                    SELECT  TOP 1 @dataPath = SUBSTRING(physical_name, 1, LEN(physical_name) - CHARINDEX('\', REVERSE(REPLACE(physical_name, '/', '\'))))
                    FROM    sys.master_files
                    WHERE   type = 0            -- 0 = data file
                    AND     database_id = 1     -- 1 = master database id
                    ORDER BY file_id
            END

            IF (@logPath IS NULL)
            BEGIN
                    -- Read the location of data files of master database
                    SELECT  TOP 1 @logPath = SUBSTRING(physical_name, 1, LEN(physical_name) - CHARINDEX('\', REVERSE(REPLACE(physical_name, '/', '\'))))
                    FROM    sys.master_files
                    WHERE   type = 1            -- 1 = log file
                    AND     database_id = 1     -- 1 = master database id
                    ORDER BY file_id
            END
        END

        DECLARE @attemptAsText VARCHAR(40)

        IF @attempt < 250
            SET @attemptAsText = CONVERT(VARCHAR(5), @attempt)
        ELSE
            SET @attemptAsText = CONVERT(VARCHAR(40), NEWID())

        SELECT @dataFileName        = @dataPath + '\' + @databaseName + '_' +  @attemptAsText + '.mdf',
               @logicalDataFileName = @databaseName   + '_' +  @attemptAsText,
               @logFileName         = @dataPath + '\' + @databaseName + '_' +  @attemptAsText + '_log.ldf',
               @logicalLogFileName  = @databaseName   + '_' +  @attemptAsText + '_log'

        -- We need to create the following statement:
        --CREATE DATABASE [<dbname>] ON  PRIMARY
        --( NAME = N'<logicalDataFileName>', FILENAME = N'<dataFileName>' )
        --LOG ON
        --( NAME = N'<logicalLogFileName>', FILENAME = N'<dataFileName>')

        SET @statement = 'CREATE DATABASE ' + QUOTENAME(@databaseName) + ' ON PRIMARY ' + CHAR(10) +
            '(Name = N''' + REPLACE(@logicalDataFileName, '''', '''''') + ''', FILENAME = N''' + REPLACE(@dataFileName, '''', '''''') + ''') ' + CHAR(10) +
            'LOG ON ' + CHAR(10) +
            '(Name = N''' + REPLACE(@logicalLogFileName, '''', '''''') + ''', FILENAME = N''' + REPLACE(@logFileName, '''', '''''') + ''')' +
            @collationClause
    END
    BEGIN TRY
        EXEC sp_executesql @statement
        BREAK
    END TRY
    BEGIN CATCH
        DECLARE @errorMessage NVARCHAR(4000)
        DECLARE @errorSeverity INT
        DECLARE @errorState INT
        DECLARE @errorNumber INT

        SELECT @errorMessage    = ERROR_MESSAGE(),
               @errorSeverity   = ERROR_SEVERITY(),
               @errorState      = ERROR_STATE(),
               @errorNumber     = ERROR_NUMBER()

        IF @attempt >= 256 OR @errorNumber <> 1802
        BEGIN
            RAISERROR('Unable to create database %s. The following error was reported: Msg %d, Level %d, State %d %s', 16, 0,
                @databaseName,
                @errorNumber,
                @errorSeverity,
                @errorState,
                @errorMessage)
            BREAK
        END
    END CATCH
    SET @attempt = @attempt + 1
END

DECLARE @recovery VARCHAR(40)
SELECT @recovery = CONVERT(VARCHAR(40), DATABASEPROPERTYEX(@databaseName, 'RECOVERY'))

--If the create worked and recovery isn't FULL, make it FULL so our backup works properly
IF @recovery IS NOT NULL AND @recovery != 'FULL'
BEGIN
    SELECT @statement = 'ALTER DATABASE ' + QUOTENAME(@databaseName) + ' SET RECOVERY FULL WITH NO_WAIT'
    EXEC sp_executesql @statement
END

IF (DATABASEPROPERTYEX(@databaseName, 'IsParameterizationForced') = '1')
BEGIN
    SELECT @statement = 'ALTER DATABASE ' + QUOTENAME(@databaseName) + ' SET PARAMETERIZATION SIMPLE WITH NO_WAIT'
    EXEC sp_executesql @statement
END

IF (@alterCollation = 1)
BEGIN
    SELECT @statement = 'USE ' + QUOTENAME(@databaseName) + '
EXEC sp_db_gb18030_unicode_collations ' + QUOTENAME(@databaseName) + ', ''ON'''

    EXEC sp_executesql @statement

    SELECT @statement = 'USE ' + QUOTENAME(@databaseName) + '
ALTER DATABASE ' + QUOTENAME(@databaseName) + ' COLLATE ' + @collation
    EXEC sp_executesql @statement
END

EXEC sp_releaseapplock @Resource = 'TfsCreateDB', @LockOwner = 'Session'