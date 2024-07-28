/*****************************************************************************
* stmt_ListPartitionKeys
*
* List the partition keys, capped by a total limit, starting from a specified 
* partition key.
*
* Paramters:
*   @total - The total keys to be retruned. 
*   @exclusiveMinPartitionKey 
*          - The key to start listing from, itself exclusive from the result set.
* 
* This is used when importing a pre-M120 TFS deployment (TFS 2017 Update 2).  
* The logic must be synchronized with <REPO>\ArtifactServices\ASTable\Sql\
* SProcs\prc_ListPartitionKeys.sql.
*****************************************************************************/
IF (@total IS NULL) 
BEGIN
    SET @total = 1000
END

IF (@exclusiveMinPartitionKey IS NULL) 
BEGIN
    SET @exclusiveMinPartitionKey = ''
END

SELECT DISTINCT TOP (@total) PartitionKey
FROM     ASTable.tbl_Entity 
WHERE    PartitionId = @partitionId 
         AND TableId = @tableId
         AND PartitionKey > @exclusiveMinPartitionKey
ORDER BY PartitionKey ASC
OPTION   (OPTIMIZE FOR (@partitionId UNKNOWN))