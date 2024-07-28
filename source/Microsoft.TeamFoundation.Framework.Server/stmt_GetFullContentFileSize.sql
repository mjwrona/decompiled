-- Returns the size of the full content files from tbl_FileMetadata
-- Parameters:
-- @partitionId INT

SET NOCOUNT     ON
SET XACT_ABORT  ON

SELECT  CAST(ISNULL(SUM(fm.CompressedLength), 0) AS BIGINT)
FROM    tbl_FileMetadata fm
WHERE   fm.PartitionId = @partitionId
        AND fm.RemoteStoreId = 0 -- SqlServer
        AND fm.ContentType = 1   -- Full
OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))
