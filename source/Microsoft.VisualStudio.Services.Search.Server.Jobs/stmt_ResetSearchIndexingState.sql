PRINT 'Deleting the search indexing units and other related tables'

IF OBJECT_ID('Search.tbl_IndexingUnit') IS NOT NULL
    DELETE FROM Search.tbl_IndexingUnit
    WHERE PartitionId = @partitionId
    OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))

IF OBJECT_ID('Search.tbl_ClassificationNode') IS NOT NULL
    DELETE FROM Search.tbl_ClassificationNode
    WHERE PartitionId = @partitionId
    OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))

IF OBJECT_ID('Search.tbl_DisabledFiles') IS NOT NULL
    DELETE FROM Search.tbl_DisabledFiles
    WHERE PartitionId = @partitionId
    OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))

IF OBJECT_ID('Search.tbl_IndexingUnitChangeEvent') IS NOT NULL
    DELETE FROM Search.tbl_IndexingUnitChangeEvent
    WHERE PartitionId = @partitionId
    OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))

IF OBJECT_ID('Search.tbl_IndexingUnitWikis') IS NOT NULL
    DELETE FROM Search.tbl_IndexingUnitWikis
    WHERE PartitionId = @partitionId
    OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))

IF OBJECT_ID('Search.tbl_ItemLevelFailures') IS NOT NULL
    DELETE FROM Search.tbl_ItemLevelFailures
    WHERE PartitionId = @partitionId
    OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))

IF OBJECT_ID('Search.tbl_ResourceLockTable') IS NOT NULL
    DELETE FROM Search.tbl_ResourceLockTable
    WHERE PartitionId = @partitionId
    OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))

IF OBJECT_ID('Search.tbl_ClassificationNode') IS NOT NULL
    DELETE FROM Search.tbl_ClassificationNode
    WHERE PartitionId = @partitionId
    OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))

IF OBJECT_ID('Search.tbl_CustomRepositoryInfo') IS NOT NULL
    DELETE FROM Search.tbl_CustomRepositoryInfo
    WHERE PartitionId = @partitionId
    OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))

IF OBJECT_ID('Search.tbl_FileMetadataStore') IS NOT NULL
    DELETE FROM Search.tbl_FileMetadataStore
    WHERE PartitionId = @partitionId
    OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))

IF OBJECT_ID('Search.tbl_IndexingUnitIndexingInformation') IS NOT NULL
    DELETE FROM Search.tbl_IndexingUnitIndexingInformation
    WHERE PartitionId = @partitionId
    OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))

IF OBJECT_ID('Search.tbl_TempFileMetadataStore') IS NOT NULL
    DELETE FROM Search.tbl_TempFileMetadataStore
    WHERE PartitionId = @partitionId
    OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))

EXEC prc_SetRegistryValue @partitionId, @key = '#\Service\ALMSearch\Settings\IsExtensionOperationInProgress\Code\Uninstalled\', @value = NULL
EXEC prc_SetRegistryValue @partitionId, @key = '#\Service\ALMSearch\Settings\IsExtensionOperationInProgress\WorkItem\Uninstalled\', @value = NULL
EXEC prc_SetRegistryValue @partitionId, @key = '#\Service\ALMSearch\Settings\IsExtensionOperationInProgress\Wiki\Uninstalled\', @value = NULL