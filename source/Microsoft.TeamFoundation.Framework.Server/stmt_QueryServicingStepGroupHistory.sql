IF OBJECT_ID('dbo.tbl_ServicingStepGroupHistory', 'U') IS NULL
BEGIN
    SELECT  0 Id
    WHERE   1 = 0
END
ELSE
BEGIN
    SELECT  Id,
            ExecutionTime,
            JobId,
            ServicingOperation,
            StepGroupId,
            ExecutionResult
    FROM    tbl_ServicingStepGroupHistory
    WHERE   JobId = @jobId 
            AND PartitionId = @partitionId
    ORDER BY ServicingOperation,
            StepGroupId
    OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))
END