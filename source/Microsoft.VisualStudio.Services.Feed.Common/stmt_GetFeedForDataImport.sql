/*****************************************************************************
* Feed.prc_GetFeedForDataImport
 Returns one or more matching feeds
* - a single non-deleted feed by id
* - the set of non-deleted feeds
*****************************************************************************/
SET NOCOUNT ON

IF @feedId IS NOT NULL
BEGIN
    -- Return a specific non-deleted feed by ID
    SELECT  FeedId,
            FeedName
    FROM    Feed.tbl_Feed
    WHERE   PartitionId = @partitionId
            AND FeedId = @feedId
            AND DeletedDate IS NULL
    OPTION  (OPTIMIZE FOR (@partitionId UNKNOWN))
END
ELSE
BEGIN 
    -- Return all non-deleted feeds (since we're not looking for a feed by ID or by Name)
    SELECT  FeedId,
            FeedName
    FROM    Feed.tbl_Feed
    WHERE   PartitionId = @partitionId
            AND DeletedDate IS NULL
    OPTION  (OPTIMIZE FOR (@partitionId UNKNOWN))
END