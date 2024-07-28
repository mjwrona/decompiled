---------------------------------------------------------------------------------------------
-- Gather some information about load (command counts and execution times) and active users
-- from the activity log. 
--
-- Parameters:
-- DECLARE @endTime    DATETIME = DATEADD(day, -1,  GETUTCDATE())
-- DECLARE @startTime  DATETIME = DATEADD(day, -14, GETUTCDATE())
-- DECLARE @partitionId INT
---------------------------------------------------------------------------------------------

SET NOCOUNT     ON
SET XACT_ABORT  ON

-- Get command count and total execution time per user
SELECT  IdentityName AS Caller, 
        CONVERT(BIGINT, SUM(ExecutionCount)) AS CommandCount,
        CONVERT(FLOAT, SUM(ExecutionTime / 1000000.0) / 60) AS ExecutionTimeInMinutes
FROM    tbl_command WITH (NOLOCK)
WHERE   PartitionId = @partitionId
        AND StartTime BETWEEN ISNULL(@startTime, DATEADD(day, -14, GETUTCDATE()))
                      AND     ISNULL(@endTime,   DATEADD(day, -1,  GETUTCDATE()))
GROUP BY IdentityName
ORDER BY ExecutionTimeInMinutes DESC
OPTION  (OPTIMIZE FOR (@partitionId UNKNOWN))