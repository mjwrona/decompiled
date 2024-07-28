-- Insert data from temp table tbl_TempUserExtensionLicense to tbl_UserExtensionLicense. If there is any conflicts, we just leave the current one
INSERT INTO Licensing.tbl_UserExtensionLicense (PartitionId, InternalScopeId, UserId, ExtensionId, Source, Status, CollectionId, AssignmentDate, LastUpdated)
SELECT PartitionId,
       InternalScopeId,
       UserId,
       ExtensionId,
       Source,
       Status,
       CollectionId,
       AssignmentDate,
       LastUpdated
FROM   Licensing.tbl_TempUserExtensionLicense tmp
WHERE NOT EXISTS (
    SELECT UserId
    FROM   Licensing.tbl_UserExtensionLicense uel 
    WHERE  uel.UserId = tmp.UserId
    AND    uel.ExtensionId = tmp.ExtensionId 
    AND    uel.Source = tmp.Source 
    AND    uel.PartitionId = tmp.partitionId)