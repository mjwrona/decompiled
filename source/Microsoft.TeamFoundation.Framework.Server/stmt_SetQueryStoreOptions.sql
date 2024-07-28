-- Copyright (c) Microsoft Corporation.  All rights reserved.

-- Parameters:
--DECLARE @databaseName            SYSNAME -- Name of the database
--DECLARE @operationMode           INT     -- The operation mode of the query store: 1=READ_ONLY, 2=READ_WRITE
--DECLARE @staleQueryThresholdDays BIGINT  -- Number of days that queries with no policy settings are kept in Query Store
--DECLARE @flushIntervalSeconds    BIGINT  -- Defines period for regular flushing of Query Store data to disk
--DECLARE @maxStorageSizeMB        BIGINT  -- Maximum disk size for the Query Store
--DECLARE @intervalLengthMinutes   BIGINT  -- The statistics aggregation interval
--DECLARE @sizeBasedCleanupMode    INT     -- Controls whether cleanup will be automatically activated when total amount of data gets close to maximum size: 0=OFF, 1=AUTO
--DECLARE @queryCaptureMode        INT     -- The currently active query capture mode: 1=ALL, 2=AUTO, 3=NONE
--DECLARE @maxPlansPerQuery        BIGINT  -- The maximum number of stored plans

DECLARE @productVersion INT = CONVERT(INT, LEFT(CONVERT(SYSNAME, SERVERPROPERTY('ProductVersion')), CHARINDEX('.', CONVERT(SYSNAME, SERVERPROPERTY('ProductVersion')), 0) - 1))

-- SQL Server Query Store is supported in SQL Azure 12 and later, and SQL Server 2016 CTP 2 and later
IF ((CONVERT(SYSNAME, SERVERPROPERTY('Edition')) LIKE '%SQL Azure%' AND @productVersion >= 12)
    OR @productVersion >= 13)
BEGIN
    DECLARE @queryCaptureModeText NVARCHAR(4) = 
    CASE @queryCaptureMode
        WHEN 1 THEN 'ALL'
        WHEN 2 THEN 'AUTO'
        WHEN 3 THEN 'NONE'
    END

    DECLARE @sizeBasedCleanupModeText NVARCHAR(4) = 
    CASE @sizeBasedCleanupMode
        WHEN 0 THEN 'OFF'
        WHEN 1 THEN 'AUTO'
    END

    DECLARE @operationModeText NVARCHAR(10) = 
    CASE @operationMode
        WHEN 1 THEN 'READ_ONLY'
        WHEN 2 THEN 'READ_WRITE'
    END

    DECLARE @statement NVARCHAR(MAX) = '
    ALTER DATABASE ##databaseName## SET QUERY_STORE = ON (
        OPERATION_MODE = ##operationModeText##,
        CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = ##staleQueryThresholdDays##),
        DATA_FLUSH_INTERVAL_SECONDS = ##flushIntervalSeconds##,
        MAX_STORAGE_SIZE_MB = ##maxStorageSizeMB##,
        INTERVAL_LENGTH_MINUTES = ##intervalLengthMinutes##,
        SIZE_BASED_CLEANUP_MODE = ##sizeBasedCleanupModeText##,
        QUERY_CAPTURE_MODE = ##queryCaptureModeText##,
        MAX_PLANS_PER_QUERY = ##maxPlansPerQuery##
    )'

    SET @statement = REPLACE(@statement, '##databaseName##', QUOTENAME(@databaseName))
    SET @statement = REPLACE(@statement, '##operationModeText##', @operationModeText)
    SET @statement = REPLACE(@statement, '##staleQueryThresholdDays##', @staleQueryThresholdDays)
    SET @statement = REPLACE(@statement, '##flushIntervalSeconds##', @flushIntervalSeconds)
    SET @statement = REPLACE(@statement, '##maxStorageSizeMB##', @maxStorageSizeMB)
    SET @statement = REPLACE(@statement, '##intervalLengthMinutes##', @intervalLengthMinutes)
    SET @statement = REPLACE(@statement, '##sizeBasedCleanupModeText##', @sizeBasedCleanupModeText)
    SET @statement = REPLACE(@statement, '##queryCaptureModeText##', @queryCaptureModeText)
    SET @statement = REPLACE(@statement, '##maxPlansPerQuery##', @maxPlansPerQuery)

    PRINT @statement

    EXEC sp_executesql @statement
END
ELSE
BEGIN
    PRINT 'Skipping setting Query Store because it is not supported by this version of SQL Server'
END