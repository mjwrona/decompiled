DECLARE @internalScopeId    INT
DECLARE @scopePath          VARBINARY(404)
DECLARE @membersOfCount       INT
DECLARE @level              INT
DECLARE @rootId             UNIQUEIDENTIFIER
    
CREATE TABLE #group (
Sid     VARCHAR(256)        NOT NULL,
Id      UNIQUEIDENTIFIER    NOT NULL
)
    
CREATE UNIQUE CLUSTERED INDEX IX_groups ON #group (Sid)  
    
-- Resolve the scope
SELECT  @internalScopeId = InternalScopeId,
    @scopePath = ParentPath + CONVERT(BINARY(4), InternalScopeId)
FROM    tbl_GroupScope
WHERE   PartitionId = @partitionId
    AND ScopeId = @scopeId    
    AND Active = 1        
OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))

IF (@@ROWCOUNT = 0)
BEGIN
RETURN
END
    
-- Retrieve all the groups within the scope           
INSERT  #group (Sid, Id)
SELECT  grp.Sid,
    grp.Id
FROM    tbl_GroupScope scope
INNER LOOP JOIN tbl_Group grp WITH (FORCESEEK(IX_tbl_Group_InternalScopeId_SpecialType(PartitionId, InternalScopeId)))
ON      scope.InternalScopeId = grp.InternalScopeId  
    AND scope.PartitionId =  grp.PartitionId
WHERE   scope.PartitionId = @partitionId
    AND scope.ParentPath + CONVERT(BINARY(4), scope.InternalScopeId) BETWEEN @scopePath 
    AND dbo.func_GetBinaryEndRange(@scopePath)
OPTION  (OPTIMIZE FOR (@partitionId UNKNOWN))   
    
CREATE TABLE #ancestorMembership (
    [Level]         INT,
    MemberId        UNIQUEIDENTIFIER,
    AncestorId     UNIQUEIDENTIFIER           
)
    
CREATE UNIQUE INDEX IX_members_Id ON #ancestorMembership (Level, MemberId, AncestorId) WITH (IGNORE_DUP_KEY=ON)
      
-- seed the ancestor with all memberships to detect cycles
INSERT  #ancestorMembership (AncestorId, MemberId, [Level])
SELECT  membership.ContainerId,
    membership.MemberId,
    0
FROM    #group groups
INNER JOIN  tbl_GroupMembership membership WITH (FORCESEEK(PK_tbl_GroupMembership(PartitionId, ContainerId)))
ON  groups.Id = membership.ContainerId
    AND membership.PartitionId = @partitionId
    AND membership.Active = 1

SELECT  @membersOfCount = @@ROWCOUNT

SET @level = 0

-- we get all the ancestor until we get to the root
WHILE (@membersOfCount <> 0)
BEGIN
INSERT  #ancestorMembership (AncestorId, MemberId, [Level])
SELECT  directMembership.AncestorId,
        ancestor.MemberId,
        @level + 1
FROM    #ancestorMembership ancestor
INNER LOOP JOIN  #ancestorMembership directMembership
ON     directMembership.MemberId = ancestor.AncestorId
WHERE   ancestor.[Level] = @level
        AND directMembership.[Level] = 0
        AND NOT EXISTS (select * from #ancestorMembership a where a.AncestorId = directMembership.AncestorId and a.MemberId = ancestor.MemberId)        
SELECT  @membersOfCount = @@ROWCOUNT,
        @level = @level + 1
END
    
-- select the cycles by looking for nodes like a --> b and b --> a
SELECT      seededMembership.AncestorId as ContainerId, ancestorGroups.DisplayName as ContainerDisplayName, seededMembership.MemberId, memberGroups.DisplayName as [MemberDisplayName], ancestor.[Level]
FROM        #ancestorMembership seededMembership
INNER JOIN  #ancestorMembership ancestor
ON          seededMembership.AncestorId = ancestor.MemberId and seededMembership.MemberId = ancestor.AncestorId
INNER JOIN  tbl_group ancestorGroups
ON          seededMembership.AncestorId = ancestorGroups.Id and ancestorGroups.PartitionId = @partitionId
INNER JOIN  tbl_group memberGroups
ON          seededMembership.MemberId =  memberGroups.Id and memberGroups.PartitionId = @partitionId
WHERE       seededMembership.[Level] = 0 order by ancestor.[Level] asc