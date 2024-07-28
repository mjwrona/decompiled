/*
** Queries the disk space used per owner
**
** Parameters:
**  @partitionId
**
** Returns:
**  The file owner and usage sets ordered by the compressed size in descending order
*/
SELECT CASE OwnerId
            WHEN 0 THEN 'Generic'
            WHEN 1 THEN 'VersionControl'
            WHEN 2 THEN 'WorkitemTracking'
            WHEN 3 THEN 'TeamBuild'
            WHEN 4 THEN 'TeamTest'
            WHEN 5 THEN 'Servicing'
            WHEN 6 THEN 'UnitTest'
            WHEN 7 THEN 'WebAccess'
            WHEN 8 THEN 'ProcessTemplate'
            WHEN 9 THEN 'StrongBox'
            WHEN 10 THEN 'Build+Git'
            WHEN 11 THEN 'CodeSense'
            WHEN 12 THEN 'Profile'
            WHEN 13 THEN 'Aad'
            WHEN 14 THEN 'Gallery'
            WHEN 15 THEN 'BlobStore'
            ELSE 'Other'
        END
        [Owner],
        SUM(CompressedLength) AS CompressedSize
FROM    tbl_FileMetadata fm
JOIN
(
    SELECT  ResourceId, MIN(OwnerId) as OwnerId
    FROM    tbl_FileReference
    WHERE   PartitionId = @partitionId
    GROUP BY ResourceId
) fr
ON      fm.ResourceId = fr.ResourceId
WHERE   fm.PartitionId = @partitionId
GROUP BY OwnerId
ORDER BY CompressedSize DESC
OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))