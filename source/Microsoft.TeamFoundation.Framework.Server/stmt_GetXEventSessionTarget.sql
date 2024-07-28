
-- Parameter: @dbSessionName NVARCHAR(MAX) - Name of the session to check.
-- Note that this stmt should only be executed in Azure SQL
SET XACT_ABORT ON

DECLARE @isRunningInReadScaleOut BIT = 0

IF DATABASEPROPERTYEX(DB_NAME(), 'Updateability') = 'READ_ONLY'
BEGIN
    SET @isRunningInReadScaleOut = 1
END

SELECT dst.target_data AS targetData,
       @isRunningInReadScaleOut AS isRunningInReadScaleOut
FROM   sys.dm_xe_database_sessions AS ds 
JOIN   sys.dm_xe_database_session_targets AS dst 
ON     ds.Address = dst.event_session_address 
WHERE  ds.name = @dbSessionName
