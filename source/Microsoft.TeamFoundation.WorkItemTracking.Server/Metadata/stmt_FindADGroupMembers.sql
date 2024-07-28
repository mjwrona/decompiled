/*---------------------------------------------------------------------------
// For a list of groups, return all the AD groups as their members 
//
//    Algorithm:
//    Create a temp table (#members) and store expanded members of the input group(@groups) from iteratively expansion
//    The AD group member found are stored in temp table (#interim)
//
// Arguments:
//  @partitionId                Partition Id
//  @groups                     List of group name (in Constants.String format)
//--------------------------------------------------------------------------*/

-- temp table storing expanded members
-- At any time only two generations (@gen, @gen+1) members will exist and older ones are removed
CREATE TABLE #members
( 
    RootConstId   INT,  -- ConstId for the group passed in 
    ParentConstId INT,  -- ConstId of the member's parent
    ConstID       INT,  -- ConstId of the member
    Gen           INT,  -- Generation 
    UNIQUE CLUSTERED (Gen, ConstID, ParentConstId, RootConstId),
    UNIQUE (ConstID, ParentConstId, RootConstId) WITH (IGNORE_DUP_KEY = ON)
)

-- temp table storing AD group members found already
CREATE TABLE #interim
(
    RootConstId INT, -- ConstId for the group passed in as key
    GroupMemberTeamFoundationId uniqueidentifier,
    GroupMemberParentTeamFoundationId uniqueidentifier,
    UNIQUE CLUSTERED (RootConstId, GroupMemberTeamFoundationId, GroupMemberParentTeamFoundationId)
)

DECLARE @gen INT = 0 -- generation of iteration expanding the members

-- insert the groups passed in into #members as initial values (in order to verify if they are AD groups themselves)
INSERT INTO #members (RootConstId, ParentConstId, ConstID, Gen)
SELECT      C.ConstID,
            C.ConstID,
            C.ConstID,
            @gen
FROM        [dbo].[Constants] C
JOIN        @groups g
ON          C.String = g.Data
WHERE       PartitionId = @partitionId 
            AND C.RemovedDate = convert(datetime,'9999',126)
OPTION (OPTIMIZE FOR (@partitionId UNKNOWN)) 
    
WHILE 1=1
BEGIN
    -- For current identity in #members, find AD group members and insert into #interim
    INSERT  #interim (RootConstId, GroupMemberTeamFoundationId, GroupMemberParentTeamFoundationId)
    SELECT  m.RootConstId,
            CMember.TeamFoundationId, 
            CParent.TeamFoundationId
    FROM    #members m
    JOIN    [dbo].[Constants] CMember
    ON      CMember.ConstID = m.ConstID
            AND CMember.PartitionId = @partitionId
    JOIN    [dbo].[Constants] CParent
    ON      CParent.ConstID = m.ParentConstID
            AND CParent.PartitionId = @partitionId
    JOIN    [dbo].[ADObjects] A
    ON      A.TeamFoundationId = CMember.TeamFoundationId
            AND A.PartitionId = @partitionId
    WHERE   A.ObjectCategory = 3 -- IdentityType.WindowsGroup = 3
            AND  A.DomainName <> 'BUILTIN' -- Exclude built-in group 
            AND  A.fDeleted = 0
            AND  CMember.RemovedDate = convert(datetime,'9999',126)
            AND  CMember.IdentityDisplayName IS NOT NULL
            AND  m.Gen = @gen
            AND  NOT EXISTS (
                            SELECT * 
                            FROM #interim i2 
                            WHERE i2.RootConstId = m.RootConstId
                                    AND i2.GroupMemberTeamFoundationId = CMember.TeamFoundationId
                                    AND i2.GroupMemberParentTeamFoundationId = CParent.TeamFoundationId
                            )
    OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))
        
    -- populate next generation members (children from current generation)
    INSERT  #members (RootConstId, ParentConstId, ConstId, Gen)
    SELECT  m.RootConstId,
            S.ParentID,
            S.ConstID,
            @gen + 1 
    FROM    [dbo].[Sets] S
    JOIN    #members m
    ON      S.ParentID = m.ConstID
    WHERE   S.PartitionId = @partitionId
            AND S.fDeleted = 0
    OPTION (OPTIMIZE FOR (@partitionId UNKNOWN)) 

    IF (@@ROWCOUNT = 0)
    BEGIN
        BREAK
    END

    -- throw away generation we already get members 
    DELETE #members WHERE Gen = @gen
    SET @gen = @gen + 1
END

SELECT  CRoot.String AS [Group],
        CMember.TeamFoundationId AS GroupMemberTeamFoundationId,
        CMember.IdentityDisplayName AS GroupMemberIdentityDisplayName,
        CParent.TeamFoundationId AS GroupMemberParentTeamFoundationId,
        CParent.IdentityDisplayName AS GroupMemberParentIdentityDisplayName
FROM    #interim i
JOIN    [dbo].Constants CRoot
ON      CRoot.ConstID = i.RootConstId
        AND CRoot.PartitionId = @partitionId
JOIN    [dbo].Constants CMember
ON      CMember.TeamFoundationId = i.GroupMemberTeamFoundationId
        AND CMember.PartitionId = @partitionId
JOIN    [dbo].Constants CParent
ON      CParent.TeamFoundationId = i.GroupMemberParentTeamFoundationId
        AND CParent.PartitionId = @partitionId
WHERE   CMember.RemovedDate = convert(datetime,'9999',126)
        AND CParent.RemovedDate = convert(datetime,'9999',126)
OPTION (OPTIMIZE FOR (@partitionId UNKNOWN)) 

DROP TABLE #interim
DROP TABLE #members
