// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItemComponent
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems
{
  internal class QueryItemComponent : WorkItemTrackingResourceComponent
  {
    private static readonly IDictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = (IDictionary<int, SqlExceptionFactory>) new Dictionary<int, SqlExceptionFactory>();
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[14]
    {
      (IComponentCreator) new ComponentCreator<QueryItemComponent>(1),
      (IComponentCreator) new ComponentCreator<QueryItemComponent2>(2),
      (IComponentCreator) new ComponentCreator<QueryItemComponent3>(3),
      (IComponentCreator) new ComponentCreator<QueryItemComponent4>(4),
      (IComponentCreator) new ComponentCreator<QueryItemComponent5>(5),
      (IComponentCreator) new ComponentCreator<QueryItemComponent6>(6),
      (IComponentCreator) new ComponentCreator<QueryItemComponent7>(7),
      (IComponentCreator) new ComponentCreator<QueryItemComponent8>(8),
      (IComponentCreator) new ComponentCreator<QueryItemComponent9>(9),
      (IComponentCreator) new ComponentCreator<QueryItemComponent10>(10),
      (IComponentCreator) new ComponentCreator<QueryItemComponent11>(11),
      (IComponentCreator) new ComponentCreator<QueryItemComponent12>(12),
      (IComponentCreator) new ComponentCreator<QueryItemComponent13>(13),
      (IComponentCreator) new ComponentCreator<QueryItemComponent14>(14)
    }, "QueryHierarchy", "WorkItem");

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => QueryItemComponent.s_sqlExceptionFactories;

    protected virtual SqlParameter BindProjectId(Guid projectId) => this.BindGuid("@projectId", projectId);

    protected void BuildTree(IEnumerable<QueryItemEntry> queryEntries)
    {
      Dictionary<Guid, QueryItemEntry> dictionary = queryEntries.ToDictionary<QueryItemEntry, Guid>((System.Func<QueryItemEntry, Guid>) (entry => entry.Id));
      foreach (QueryItemEntry queryEntry in queryEntries)
      {
        QueryItemEntry queryItemEntry;
        if (dictionary.TryGetValue(queryEntry.ParentId, out queryItemEntry))
          queryItemEntry.AddChild(queryEntry);
      }
    }

    public virtual QueryItemEntry GetQueryItemEntryById(
      Guid queryId,
      int? expandDepth,
      bool includeWiql,
      bool includeDeleted,
      int maxResultCount,
      bool includeExecutionInfo = false,
      Guid? filterUnderProjectId = null)
    {
      return this.GetQueryItemEntriesByIds((IEnumerable<Guid>) new Guid[1]
      {
        queryId
      }, expandDepth, includeWiql, includeDeleted, maxResultCount, includeExecutionInfo, filterUnderProjectId).FirstOrDefault<QueryItemEntry>();
    }

    static QueryItemComponent()
    {
      QueryItemComponent.s_sqlExceptionFactories[604000] = WorkItemTrackingResourceComponent.CreateFactory<UndeletedQueryItemNotFound>();
      QueryItemComponent.s_sqlExceptionFactories[604001] = WorkItemTrackingResourceComponent.CreateFactory<UndeletedQueryItemParentNotFound>();
      QueryItemComponent.s_sqlExceptionFactories[604002] = WorkItemTrackingResourceComponent.CreateFactory<UndeletedQueryHasSiblingWithSameName>();
      QueryItemComponent.s_sqlExceptionFactories[600314] = WorkItemTrackingResourceComponent.CreateFactory<UndeletedQuerysParentHasTooManyChildren>();
      QueryItemComponent.s_sqlExceptionFactories[600315] = new SqlExceptionFactory(typeof (TooManyQueryItemsRequestedException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(QueryItemComponent.GetExceptionObject));
    }

    private static Exception GetExceptionObject(
      IVssRequestContext requestContext,
      int errorNumber,
      SqlException sqlException,
      SqlError sqlError)
    {
      return (Exception) new TooManyQueryItemsRequestedException(int.Parse(sqlError.ExtractString("MaxLimit")));
    }

    public virtual IEnumerable<QueryItemEntry> GetQueryItemEntriesByIds(
      IEnumerable<Guid> queryIds,
      int? expandDepth,
      bool includeWiql,
      bool includeDeleted,
      int maxResultCount,
      bool includeExecutionInfo = false,
      Guid? filterUnderProjectId = null)
    {
      throw new NotImplementedException();
    }

    public virtual IEnumerable<QueryItemEntry> GetRootQueryItemEntries(
      Guid projectId,
      Guid teamFoundationId,
      int? expandDepth,
      bool includeWiql,
      bool includeDeleted,
      int maxResultCount,
      bool includeExecutionInfo = false)
    {
      throw new NotImplementedException();
    }

    public virtual QueryItemEntry GetQueryItemByPath(
      string path,
      Guid projectId,
      Guid teamFoundationId,
      int? expandDepth,
      bool includeWiql,
      bool includeDeleted,
      bool includeExecutionInfo = false)
    {
      throw new NotImplementedException();
    }

    protected virtual void BindMaxQueryItemChildrenUnderParent(int maxQueryItemChildrenUnderParent)
    {
    }

    public virtual QueryItemEntry UndeleteQueryItem(
      Guid queryId,
      bool undeleteDescendants,
      Guid teamFoundationId,
      int maxQueryItemChildrenUnderParent)
    {
      throw new NotImplementedException();
    }

    protected virtual WorkItemTrackingObjectBinder<QueryItemEntry> GetDrilldownQueryItemEntryBinder() => (WorkItemTrackingObjectBinder<QueryItemEntry>) new QueryItemComponent.DrilldownQueryDataBinder1(this.RequestContext);

    protected virtual WorkItemTrackingObjectBinder<QueryItemEntry> GetFullTreeQueryItemEntryBinder() => (WorkItemTrackingObjectBinder<QueryItemEntry>) new QueryItemComponent.FullTreeQueryDataBinder1();

    public virtual void BindIncludeDeleted(bool includeDeleted)
    {
    }

    public virtual void BindIncludeExecutionInfo(bool includeExecutionInfo)
    {
    }

    public virtual IEnumerable<QueryItemEntry> GetChangedPublicQueryItems(
      Guid projectId,
      long? timestamp,
      int maxResultCount,
      out long newTimestamp)
    {
      this.PrepareDynamicProcedure("dynprc_GetChangedPublicQueryItems", "\r\nDECLARE @projectIntegerId INT\r\nDECLARE @actionName NVARCHAR(256)\r\nSET @actionName = 'GetChangedPublicQueryItems'\r\nDECLARE @tfError NVARCHAR(4000)\r\n\r\n--Get the project ID\r\nSELECT  @projectIntegerID = T.[ID]\r\nFROM    [dbo].[TreeNodes] T\r\nWHERE   T.[CSSNodeId] = @projectId\r\n    AND T.[TypeID]    = -42\r\n    AND T.[fDeleted]  = 0\r\n    AND T.PartitionId = @partitionId\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n\r\nDECLARE @publicQueryItemCount INT\r\nSELECT @publicQueryItemCount = COUNT(*)\r\nFROM QueryItems Q\r\nWHERE Q.[ProjectID] = @projectIntegerId\r\nAND Q.[fPublic] = 1\r\nAND Q.[fDeleted] = 0\r\nAND Q.[PartitionId] = @partitionId\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n\r\nIF @publicQueryItemCount > @maxNonDeletedPublicQueries\r\nBEGIN\r\n        SET @tfError = dbo.func_GetMessage(600315); RAISERROR(@tfError, 16, 1, @actionName, @maxNonDeletedPublicQueries)\r\n        RETURN\r\nEND\r\n\r\nCreate Table #QueryIdsAndWatermarks\r\n(\r\n    ID UNIQUEIDENTIFIER PRIMARY KEY,\r\n    CacheStamp BIGINT\r\n)\r\n\r\nINSERT INTO #QueryIdsAndWatermarks\r\nSELECT  Q.[ID],\r\n        Q.[CacheStamp]\r\nFROM QueryItems Q\r\nWHERE Q.[ProjectID] = @projectIntegerId\r\n    AND Q.[fPublic] = 1\r\n    AND Q.[PartitionId] = @partitionId\r\n    AND Q.[CacheStamp] > @timestamp\r\n\r\nIF @@ROWCOUNT = 0 SELECT @timestamp AS TimeStamp\r\nELSE SELECT MAX(Q.[CacheStamp]) AS TimeStamp FROM #QueryIdsAndWatermarks Q\r\n\r\nSELECT  Q.[ID] AS Id,\r\n        @projectId AS ProjectId,\r\n        Q.[ParentID] AS ParentId,\r\n        Q.[Name] AS Name,\r\n        Q.[CreateTime] AS CreatedDate,\r\n        CMB.[TeamFoundationId] AS ModifiedById,\r\n        Q.[LastWriteTime] AS ModifiedDate,\r\n        Q.[Text] AS Wiql,\r\n        Q.[fFolder] AS IsFolder,\r\n        Q.[fDeleted] AS IsDeleted,\r\n        NULL AS PrivateQueryOwner\r\nFROM QueryItems Q\r\n    JOIN #QueryIdsAndWatermarks IDS\r\n        ON IDS.[Id] = Q.[Id]\r\n    LEFT OUTER JOIN Constants as CMB\r\n        ON CMB.[ConstID] = Q.[LastWriteConstID] AND CMB.PartitionId = @partitionId\r\nWHERE Q.[PartitionId] = @partitionId\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n            ");
      this.BindProjectId(projectId);
      this.BindLong("@timestamp", timestamp.HasValue ? timestamp.Value : -1L);
      this.BindInt("@maxNonDeletedPublicQueries", maxResultCount);
      long tempTimestamp = 0;
      List<QueryItemEntry> list = this.ExecuteUnknown<IEnumerable<QueryItemEntry>>((System.Func<IDataReader, IEnumerable<QueryItemEntry>>) (reader =>
      {
        reader.Read();
        tempTimestamp = reader.GetInt64(0);
        reader.NextResult();
        return this.GetFullTreeQueryItemEntryBinder().BindAll(reader);
      })).ToList<QueryItemEntry>();
      newTimestamp = tempTimestamp;
      return (IEnumerable<QueryItemEntry>) list;
    }

    public virtual QueryItemEntry GetPublicQueryItemsHierarchy(
      Guid projectId,
      int maxResultCount,
      out long timestamp,
      out int queryEntryCount)
    {
      this.PrepareDynamicProcedure("dynprc_GetPublicQueryItemsHierarchy", "\r\nDECLARE @projectIntegerId INT\r\nDECLARE @actionName NVARCHAR(256)\r\nSET @actionName = 'GetPublicQueryItems'\r\nDECLARE @tfError NVARCHAR(4000)\r\n\r\n--Get the project ID\r\nSELECT  @projectIntegerID = T.[ID]\r\nFROM    [dbo].[TreeNodes] T\r\nWHERE   T.[CSSNodeId] = @projectId\r\n    AND T.[TypeID]    = -42\r\n    AND T.[fDeleted]  = 0\r\n    AND T.PartitionId = @partitionId\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n\r\nCreate Table #QueryIdsAndWatermarks\r\n(\r\n    ID UNIQUEIDENTIFIER PRIMARY KEY,\r\n    CacheStamp BIGINT\r\n)\r\n\r\nINSERT INTO #QueryIdsAndWatermarks\r\nSELECT  Q.[ID],\r\n        Q.[CacheStamp]\r\nFROM QueryItems Q\r\nWHERE Q.[ProjectID] = @projectIntegerId\r\n    AND Q.[fPublic] = 1\r\n    AND Q.[fDeleted] = 0\r\n    AND Q.[PartitionId] = @partitionId\r\n\r\nSELECT MAX(Q.[CacheStamp]) AS TimeStamp\r\nFROM #QueryIdsAndWatermarks Q\r\n\r\n DECLARE @publicQueryItems AS TABLE(\r\n            [ID] [UNIQUEIDENTIFIER] NOT NULL,\r\n            [ProjectID] [UNIQUEIDENTIFIER],\r\n            [ParentID] [UNIQUEIDENTIFIER] NULL,\r\n            [Name] [NVARCHAR](255) NOT NULL,\r\n            [CreatedDate] DATETIME,\r\n            [ModifiedById] UNIQUEIDENTIFIER,\r\n            [ModifiedDate] DATETIME,\r\n            [Wiql] [nvarchar](max) NULL,\r\n            [IsFolder] [bit] NOT NULL,\r\n            [IsDeleted] [bit] NOT NULL,\r\n            [PrivateQueryOwner] [UNIQUEIDENTIFIER] NULL)\r\n\r\nINSERT INTO @publicQueryItems\r\nSELECT  Q.[ID] AS Id,\r\n        @projectId AS ProjectId,\r\n        Q.[ParentID] AS ParentId,\r\n        Q.[Name] AS Name,\r\n        Q.[CreateTime] AS CreatedDate,\r\n        CMB.[TeamFoundationId] AS ModifiedById,\r\n        Q.[LastWriteTime] AS ModifiedDate,\r\n        Q.[Text] AS Wiql,\r\n        Q.[fFolder] AS IsFolder,\r\n        Q.[fDeleted] AS IsDeleted,\r\n        NULL AS PrivateQueryOwner\r\nFROM QueryItems Q\r\n    JOIN #QueryIdsAndWatermarks IDS\r\n        ON IDS.[Id] = Q.[Id]\r\n    LEFT OUTER JOIN Constants as CMB\r\n        ON CMB.[ConstID] = Q.[LastWriteConstID] AND CMB.PartitionId = @partitionId\r\nWHERE Q.[PartitionId] = @partitionId\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n\r\nDECLARE @publicQueryItemCount INT = @@ROWCOUNT\r\n\r\n    IF @publicQueryItemCount > @maxResultCount\r\n    BEGIN\r\n            SET @tfError = dbo.func_GetMessage(600315); RAISERROR(@tfError, 16, 1, @actionName, @maxResultCount)\r\n            RETURN\r\n    END\r\n\r\n    SELECT [ID],\r\n           [ProjectID],\r\n           [ParentID],\r\n           [Name],\r\n           [CreatedDate] ,\r\n           [ModifiedById],\r\n           [ModifiedDate],\r\n           [Wiql],\r\n           [IsFolder],\r\n           [IsDeleted],\r\n           [PrivateQueryOwner]\r\n    FROM @publicQueryItems\r\n            ");
      this.BindProjectId(projectId);
      this.BindInt("@maxResultCount", maxResultCount);
      long tempTimestamp = 0;
      List<QueryItemEntry> list = this.ExecuteUnknown<IEnumerable<QueryItemEntry>>((System.Func<IDataReader, IEnumerable<QueryItemEntry>>) (reader =>
      {
        reader.Read();
        tempTimestamp = reader.GetInt64(0);
        reader.NextResult();
        return this.GetFullTreeQueryItemEntryBinder().BindAll(reader);
      })).ToList<QueryItemEntry>();
      queryEntryCount = list.Count;
      timestamp = tempTimestamp;
      this.BuildTree((IEnumerable<QueryItemEntry>) list);
      return list.Where<QueryItemEntry>((System.Func<QueryItemEntry, bool>) (qe => qe.ParentId == Guid.Empty)).FirstOrDefault<QueryItemEntry>();
    }

    public virtual QueryItemEntry GetPrivateQueryItemsHierarchy(
      Guid projectId,
      Guid teamFoundationId,
      int maxResultCount,
      int grandMaxResultCount)
    {
      this.PrepareDynamicProcedure("dynprc_GetPrivateQueryItemsHierarchy", "\r\nDECLARE @projectIntegerId INT\r\nDECLARE @userConstId INT\r\nDECLARE @actionName NVARCHAR(256)\r\n    SET @actionName = 'prc_GetPrivateQueryItems'\r\nDECLARE @tfError NVARCHAR(4000)\r\n--Get the project ID\r\nSELECT  @projectIntegerID = T.[ID]\r\nFROM    [dbo].[TreeNodes] T\r\nWHERE   T.[CSSNodeId] = @projectId\r\n    AND T.[TypeID]    = -42\r\n    AND T.[fDeleted]  = 0\r\n    AND T.PartitionId = @partitionId\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n\r\n--get the user const id\r\nEXEC prc_GetConstIDFromTeamFoundationId @teamFoundationId, @partitionId, @userConstId OUTPUT\r\n\r\nDECLARE @relevantQueryIds typ_GuidTable\r\n\r\nINSERT INTO @relevantQueryIds\r\nSELECT  Q.[ID]\r\nFROM QueryItems Q\r\nWHERE Q.[ProjectID] = @projectIntegerId\r\n    AND Q.[ConstID] = @userConstId\r\n    AND Q.[fDeleted] = 0\r\n    AND Q.[PartitionId] = @partitionId\r\n\r\nDECLARE @privateQueryItems AS TABLE(\r\n            [ID] [UNIQUEIDENTIFIER] NOT NULL,\r\n            [ProjectID] [UNIQUEIDENTIFIER],\r\n            [ParentID] [UNIQUEIDENTIFIER] NULL,\r\n            [Name] [NVARCHAR](255) NOT NULL,\r\n            [CreatedDate] DATETIME,\r\n            [ModifiedById] UNIQUEIDENTIFIER,\r\n            [ModifiedDate] DATETIME,\r\n            [Wiql] [nvarchar](max) NULL,\r\n            [IsFolder] [bit] NOT NULL,\r\n            [IsDeleted] [bit] NOT NULL,\r\n            [PrivateQueryOwner] [UNIQUEIDENTIFIER] NOT NULL)\r\n\r\nINSERT INTO @privateQueryItems\r\nSELECT  Q.[ID] AS Id,\r\n        @projectId AS ProjectId,\r\n        Q.[ParentID] AS ParentId,\r\n        Q.[Name] AS Name,\r\n        Q.[CreateTime] AS CreatedDate,\r\n        CMB.[TeamFoundationId] AS ModifiedById,\r\n        Q.[LastWriteTime] AS ModifiedDate,\r\n        Q.[Text] AS Wiql,\r\n        Q.[fFolder] AS IsFolder,\r\n        Q.[fDeleted] AS IsDeleted,\r\n        @teamFoundationId AS PrivateQueryOwner\r\nFROM QueryItems Q\r\n    JOIN @relevantQueryIds IDS\r\n        ON IDS.[Id] = Q.[Id]\r\n    LEFT OUTER JOIN Constants as CMB\r\n        ON CMB.[ConstID] = Q.[LastWriteConstID] AND CMB.[PartitionId] = @partitionId\r\nWHERE Q.[PartitionId] = @partitionId\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n\r\nDECLARE @privateQueryItemCount INT = @@ROWCOUNT\r\n\r\n    IF @privateQueryItemCount > @maxResultCount\r\n    BEGIN\r\n            SET @tfError = dbo.func_GetMessage(600315); RAISERROR(@tfError, 16, 1, @actionName, @grandMaxResultCount)\r\n            RETURN\r\n    END\r\n\r\n    SELECT [ID],\r\n           [ProjectID],\r\n           [ParentID],\r\n           [Name],\r\n           [CreatedDate] ,\r\n           [ModifiedById],\r\n           [ModifiedDate],\r\n           [Wiql],\r\n           [IsFolder],\r\n           [IsDeleted],\r\n           [PrivateQueryOwner]\r\n    FROM @privateQueryItems\r\n            ");
      this.BindProjectId(projectId);
      this.BindGuid("@teamFoundationId", teamFoundationId);
      this.BindInt("@maxResultCount", maxResultCount);
      this.BindInt("@grandMaxResultCount", grandMaxResultCount);
      List<QueryItemEntry> list = this.ExecuteUnknown<IEnumerable<QueryItemEntry>>((System.Func<IDataReader, IEnumerable<QueryItemEntry>>) (reader => this.GetFullTreeQueryItemEntryBinder().BindAll(reader))).ToList<QueryItemEntry>();
      this.BuildTree((IEnumerable<QueryItemEntry>) list);
      return list.Where<QueryItemEntry>((System.Func<QueryItemEntry, bool>) (qe => qe.ParentId == Guid.Empty)).FirstOrDefault<QueryItemEntry>();
    }

    public virtual long GetPublicQueryItemsTimestamp(Guid projectId)
    {
      this.PrepareDynamicProcedure("dynprc_GetPublicQueryItemsTimestamp", "\r\nDECLARE @projectIntegerId INT\r\n\r\n--Get the project ID\r\nSELECT  @projectIntegerID = T.[ID]\r\nFROM    [dbo].[TreeNodes] T\r\nWHERE   T.[CSSNodeId] = @projectId\r\n    AND T.[TypeID]    = -42\r\n    AND T.[fDeleted]  = 0\r\n    AND T.PartitionId = @partitionId\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n\r\nSELECT CAST(MAX(Q.[CacheStamp]) AS BIGINT) AS TimeStamp\r\nFROM QueryItems Q\r\nWHERE Q.[PartitionId] = @partitionId\r\n    AND Q.[ProjectID] = @projectIntegerId\r\n    AND Q.[fPublic] = 1\r\n            ");
      this.BindProjectId(projectId);
      return this.ExecuteUnknown<long>((System.Func<IDataReader, long>) (reader =>
      {
        reader.Read();
        return !reader.IsDBNull(0) ? reader.GetInt64(0) : -1L;
      }));
    }

    public virtual QueryItemEntry CreateRootQueryFolder(
      Guid projectId,
      Guid teamFoundationId,
      string rootQueryFolderName,
      bool isPublic)
    {
      this.PrepareDynamicProcedure("dynprc_CreateRootQueryFolder", "\r\n                SET XACT_ABORT ON\r\n                SET NOCOUNT ON\r\n    \r\n                --get the user const id\r\n                DECLARE @userConstID INT\r\n                EXEC prc_GetConstIDFromTeamFoundationId @teamFoundationId, @partitionId, @userConstId OUTPUT\r\n\r\n                DECLARE @projectNodeId INT\r\n                EXEC prc_GetTreeNodeIdFromCssNodeId @partitionId, @projectID, @projectNodeId OUTPUT\r\n\r\n                DECLARE @changeDate DATETIME\r\n                SET @changeDate = GETUTCDATE()\r\n\r\n                BEGIN TRAN\r\n                BEGIN\r\n                    -- Merge the Query Items to avoid multiple threads accessing at the same time\r\n                    MERGE QueryItems AS Target\r\n                    USING\r\n                    (\r\n                        SELECT \r\n                        @partitionId, \r\n                        @isPublic, \r\n                        CASE WHEN @isPublic = 0 THEN @userConstId ELSE 0 END, \r\n                        @projectNodeId\r\n                    ) \r\n                    AS Source(PartitionId, fPublic, ConstID, ProjectID)\r\n                    ON\r\n                    (\r\n                        Target.PartitionId = Source.PartitionId \r\n                        AND Target.fPublic = Source.fPublic \r\n                        AND Target.ConstID = Source.ConstID \r\n                        AND Target.ProjectID = Source.ProjectID\r\n                        AND Target.ParentID IS NULL \r\n                    )\r\n                    WHEN NOT MATCHED THEN\r\n                        INSERT\r\n                        (\r\n                            PartitionId,\r\n                            ID, \r\n                            ParentID,\r\n                            ProjectID,\r\n                            ConstID,\r\n                            Name,\r\n                            CreateTime,\r\n                            LastWriteConstID,\r\n                            LastWriteTime,\r\n                            fPublic,\r\n                            fFolder\r\n                        )\r\n                        VALUES\r\n                        (\r\n                            Source.PartitionId, \r\n                            NEWID(), \r\n                            NULL, \r\n                            Source.ProjectID, \r\n                            Source.ConstID, \r\n                            @rootQueryFolderName, \r\n                            @changeDate, \r\n                            @userConstID, \r\n                            @changeDate,\r\n                            @isPublic,\r\n                            1\r\n                        );\r\n                    END;\r\n                COMMIT TRAN\r\n\r\n                -- If creating the Private Folder select the Private data\r\n                IF ( @isPublic = 0 )\r\n                    SELECT  Q.[ID] AS Id,\r\n                            @projectId AS ProjectId,\r\n                            Q.[ParentID] AS ParentId,\r\n                            Q.[Name] AS Name,\r\n                            Q.[CreateTime] AS CreatedDate,\r\n                            @teamFoundationId AS ModifiedById,\r\n                            Q.[LastWriteTime] AS ModifiedDate,\r\n                            Q.[Text] AS Wiql,\r\n                            Q.[fFolder] AS IsFolder,\r\n                            Q.[fDeleted] AS IsDeleted,\r\n                            @teamFoundationId AS PrivateQueryOwner\r\n                    FROM QueryItems Q\r\n                    WHERE Q.PartitionId = @partitionId\r\n                        AND Q.fPublic = 0\r\n                        AND Q.ConstID = @userConstId\r\n                        AND Q.ParentID IS NULL\r\n                        AND Q.ProjectID = @projectNodeId\r\n                    OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n                ELSE\r\n                    SELECT  Q.[ID] AS Id,\r\n                        @projectId AS ProjectId,\r\n                        Q.[ParentID] AS ParentId,\r\n                        Q.[Name] AS Name,\r\n                        Q.[CreateTime] AS CreatedDate,\r\n                        CMB.[TeamFoundationId] AS ModifiedById,\r\n                        Q.[LastWriteTime] AS ModifiedDate,\r\n                        Q.[Text] AS Wiql,\r\n                        Q.[fFolder] AS IsFolder,\r\n                        Q.[fDeleted] AS IsDeleted,\r\n                        NULL AS PrivateQueryOwner\r\n                        FROM QueryItems Q\r\n                        LEFT OUTER JOIN Constants as CMB\r\n                        ON CMB.[ConstID] = Q.[LastWriteConstID] AND CMB.PartitionId = @partitionId\r\n                   WHERE Q.PartitionId = @partitionId\r\n                        AND Q.fPublic = 1\r\n                        AND Q.ParentID IS NULL\r\n                        AND Q.ProjectID = @projectNodeId\r\n                   OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n            ");
      this.BindProjectId(projectId);
      this.BindGuid("@teamFoundationId", teamFoundationId);
      this.BindString("@rootQueryFolderName", rootQueryFolderName, (int) byte.MaxValue, false, SqlDbType.NVarChar);
      this.BindBoolean("@isPublic", isPublic);
      return this.ExecuteUnknown<QueryItemEntry>((System.Func<IDataReader, QueryItemEntry>) (reader =>
      {
        reader.Read();
        return this.GetFullTreeQueryItemEntryBinder().Bind(reader);
      }));
    }

    public virtual IList<QueryItemEntry> SearchQueries(
      Guid projectId,
      Guid teamFoundationId,
      int maxResultCount,
      bool includeWiql,
      string searchText,
      bool includeDeleted,
      bool includeExecutionInfo = false)
    {
      throw new NotImplementedException();
    }

    protected virtual WorkItemTrackingObjectBinder<QueryItemEntry> GetQueryItemDataBinder() => (WorkItemTrackingObjectBinder<QueryItemEntry>) new QueryItemComponent.QueryItemDataBinder();

    public virtual IEnumerable<QueryItemEntry> GetQueriesExceedingMaxWiqlLength(int maxWiqlLength) => throw new NotImplementedException();

    public virtual void DeleteQueriesExceedingMaxWiqlLength(int maxWiqlLength) => throw new NotImplementedException();

    public virtual void UpdateQueryHash(IList<QueryHash> queryHashInfo) => throw new NotImplementedException();

    public virtual IList<QueryItemEntry> FetchTopQueryItemsWithoutExecutionInfo(int top = 1000) => throw new NotImplementedException();

    public virtual IList<QueryItemEntry> FetchTopQueryItemsWithoutQueryType(int top = 1000) => throw new NotImplementedException();

    public virtual void UpdateQueryType(IList<QueryTypeInfo> queryTypeInfos) => throw new NotImplementedException();

    public virtual int RemoveDeletedQueries(DateTime earliestDate, int batchSize) => throw new NotImplementedException();

    public virtual IList<QueryItemEntry> GetDeletedQueryItems(Guid projectId, int maxResultCount) => throw new NotImplementedException();

    protected class DrilldownQueryDataBinder1 : WorkItemTrackingObjectBinder<QueryItemEntry>
    {
      private WorkItemTrackingRequestContext m_witRequestContext;
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      private SqlColumnBinder ParentId = new SqlColumnBinder(nameof (ParentId));
      private SqlColumnBinder Project = new SqlColumnBinder(nameof (Project));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder CreatedDate = new SqlColumnBinder(nameof (CreatedDate));
      private SqlColumnBinder ModifiedById = new SqlColumnBinder(nameof (ModifiedById));
      private SqlColumnBinder ModifiedDate = new SqlColumnBinder(nameof (ModifiedDate));
      private SqlColumnBinder Wiql = new SqlColumnBinder(nameof (Wiql));
      private SqlColumnBinder IsFolder = new SqlColumnBinder(nameof (IsFolder));
      private SqlColumnBinder IsPublic = new SqlColumnBinder(nameof (IsPublic));

      public DrilldownQueryDataBinder1(IVssRequestContext requestContext) => this.m_witRequestContext = requestContext.WitContext();

      public override QueryItemEntry Bind(IDataReader reader)
      {
        bool boolean = this.IsPublic.GetBoolean(reader);
        return new QueryItemEntry()
        {
          Id = this.Id.GetGuid(reader),
          ParentId = this.ParentId.GetGuid(reader, true),
          ProjectId = this.m_witRequestContext.RequestContext.GetService<IProjectService>().GetProjectId(this.m_witRequestContext.RequestContext.Elevate(), this.Project.GetString(reader, false)),
          Name = this.Name.GetString(reader, false),
          CreatedDate = this.CreatedDate.GetDateTime(reader),
          ModifiedById = this.ModifiedById.GetGuid(reader, true),
          ModifiedDate = this.ModifiedDate.GetDateTime(reader),
          Wiql = this.Wiql.GetString(reader, true),
          IsFolder = this.IsFolder.GetBoolean(reader),
          IsPublic = boolean,
          CreatedById = boolean ? Guid.Empty : this.m_witRequestContext.RequestIdentity.Id
        };
      }
    }

    protected class DrilldownQueryDataBinder2 : WorkItemTrackingObjectBinder<QueryItemEntry>
    {
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      private SqlColumnBinder ParentId = new SqlColumnBinder(nameof (ParentId));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder CreatedDate = new SqlColumnBinder(nameof (CreatedDate));
      private SqlColumnBinder ModifiedById = new SqlColumnBinder(nameof (ModifiedById));
      private SqlColumnBinder ModifiedDate = new SqlColumnBinder(nameof (ModifiedDate));
      private SqlColumnBinder Wiql = new SqlColumnBinder(nameof (Wiql));
      private SqlColumnBinder IsFolder = new SqlColumnBinder(nameof (IsFolder));
      private SqlColumnBinder SecurityToken = new SqlColumnBinder(nameof (SecurityToken));
      private SqlColumnBinder HasChildren = new SqlColumnBinder(nameof (HasChildren));
      private SqlColumnBinder ProjectId = new SqlColumnBinder(nameof (ProjectId));
      private SqlColumnBinder PrivateQueryOwner = new SqlColumnBinder(nameof (PrivateQueryOwner));
      private SqlColumnBinder Path = new SqlColumnBinder(nameof (Path));

      public override QueryItemEntry Bind(IDataReader reader)
      {
        Guid guid = this.PrivateQueryOwner.GetGuid(reader, true);
        return new QueryItemEntry()
        {
          Id = this.Id.GetGuid(reader),
          ParentId = this.ParentId.GetGuid(reader, true),
          ProjectId = this.ProjectId.GetGuid(reader, false),
          Name = this.Name.GetString(reader, false),
          CreatedById = guid,
          CreatedDate = this.CreatedDate.GetDateTime(reader),
          ModifiedById = this.ModifiedById.GetGuid(reader, true),
          ModifiedDate = this.ModifiedDate.GetDateTime(reader),
          Wiql = this.Wiql.GetString(reader, true),
          IsFolder = this.IsFolder.GetBoolean(reader),
          IsPublic = guid == Guid.Empty,
          SecurityToken = this.SecurityToken.GetString(reader, false),
          Path = this.Path.GetString(reader, false),
          HasChildren = this.HasChildren.GetBoolean(reader),
          IsDeleted = false
        };
      }
    }

    protected class DrilldownQueryDataBinder3 : QueryItemComponent.DrilldownQueryDataBinder2
    {
      private SqlColumnBinder IsDeleted = new SqlColumnBinder(nameof (IsDeleted));

      public override QueryItemEntry Bind(IDataReader reader)
      {
        QueryItemEntry queryItemEntry = base.Bind(reader);
        queryItemEntry.IsDeleted = this.IsDeleted.GetBoolean(reader);
        return queryItemEntry;
      }
    }

    protected class DrilldownQueryDataBinder4 : WorkItemTrackingObjectBinder<QueryItemEntry>
    {
      private QueryItemComponent m_queryItemComponent;
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      private SqlColumnBinder ParentId = new SqlColumnBinder(nameof (ParentId));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder CreatedDate = new SqlColumnBinder(nameof (CreatedDate));
      private SqlColumnBinder ModifiedById = new SqlColumnBinder(nameof (ModifiedById));
      private SqlColumnBinder ModifiedDate = new SqlColumnBinder(nameof (ModifiedDate));
      private SqlColumnBinder Wiql = new SqlColumnBinder(nameof (Wiql));
      private SqlColumnBinder IsFolder = new SqlColumnBinder(nameof (IsFolder));
      private SqlColumnBinder SecurityToken = new SqlColumnBinder(nameof (SecurityToken));
      private SqlColumnBinder HasChildren = new SqlColumnBinder(nameof (HasChildren));
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder PrivateQueryOwner = new SqlColumnBinder(nameof (PrivateQueryOwner));
      private SqlColumnBinder Path = new SqlColumnBinder(nameof (Path));
      private SqlColumnBinder IsDeleted = new SqlColumnBinder(nameof (IsDeleted));

      public DrilldownQueryDataBinder4(QueryItemComponent queryItemComponent) => this.m_queryItemComponent = queryItemComponent;

      public override QueryItemEntry Bind(IDataReader reader)
      {
        Guid guid = this.PrivateQueryOwner.GetGuid(reader, true);
        return new QueryItemEntry()
        {
          Id = this.Id.GetGuid(reader),
          ParentId = this.ParentId.GetGuid(reader, true),
          ProjectId = this.m_queryItemComponent.GetDataspaceIdentifier(this.DataspaceId.GetInt32(reader)),
          Name = this.Name.GetString(reader, false),
          CreatedDate = this.CreatedDate.GetDateTime(reader),
          ModifiedById = this.ModifiedById.GetGuid(reader, true),
          ModifiedDate = this.ModifiedDate.GetDateTime(reader),
          Wiql = this.Wiql.GetString(reader, true),
          IsFolder = this.IsFolder.GetBoolean(reader),
          IsPublic = guid == Guid.Empty,
          CreatedById = guid,
          SecurityToken = this.SecurityToken.GetString(reader, false),
          Path = this.Path.GetString(reader, false),
          HasChildren = this.HasChildren.GetBoolean(reader),
          IsDeleted = this.IsDeleted.GetBoolean(reader)
        };
      }
    }

    protected class DrilldownQueryDataBinder5 : WorkItemTrackingObjectBinder<QueryItemEntry>
    {
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      private SqlColumnBinder ParentId = new SqlColumnBinder(nameof (ParentId));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder CreatedById = new SqlColumnBinder(nameof (CreatedById));
      private SqlColumnBinder CreatedByName = new SqlColumnBinder(nameof (CreatedByName));
      private SqlColumnBinder CreatedDate = new SqlColumnBinder(nameof (CreatedDate));
      private SqlColumnBinder ModifiedById = new SqlColumnBinder(nameof (ModifiedById));
      private SqlColumnBinder ModifiedByName = new SqlColumnBinder(nameof (ModifiedByName));
      private SqlColumnBinder ModifiedDate = new SqlColumnBinder(nameof (ModifiedDate));
      private SqlColumnBinder Wiql = new SqlColumnBinder(nameof (Wiql));
      private SqlColumnBinder IsFolder = new SqlColumnBinder(nameof (IsFolder));
      private SqlColumnBinder IsPublic = new SqlColumnBinder(nameof (IsPublic));
      private SqlColumnBinder IsDeleted = new SqlColumnBinder(nameof (IsDeleted));
      private SqlColumnBinder SecurityToken = new SqlColumnBinder(nameof (SecurityToken));
      private SqlColumnBinder HasChildren = new SqlColumnBinder(nameof (HasChildren));
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder Path = new SqlColumnBinder(nameof (Path));
      private QueryItemComponent m_queryItemComponent;

      public DrilldownQueryDataBinder5(QueryItemComponent queryItemComponent) => this.m_queryItemComponent = queryItemComponent;

      public override QueryItemEntry Bind(IDataReader reader) => new QueryItemEntry()
      {
        Id = this.Id.GetGuid(reader),
        ParentId = this.ParentId.GetGuid(reader, true),
        ProjectId = this.m_queryItemComponent.GetDataspaceIdentifier(this.DataspaceId.GetInt32(reader)),
        Name = this.Name.GetString(reader, false),
        CreatedById = this.CreatedById.GetGuid(reader, true),
        CreatedByName = this.CreatedByName.GetString(reader, true),
        CreatedDate = this.CreatedDate.GetDateTime(reader),
        ModifiedById = this.ModifiedById.GetGuid(reader, true),
        ModifiedByName = this.ModifiedByName.GetString(reader, true),
        ModifiedDate = this.ModifiedDate.GetDateTime(reader),
        Wiql = this.Wiql.GetString(reader, true),
        IsFolder = this.IsFolder.GetBoolean(reader),
        IsPublic = this.IsPublic.GetBoolean(reader),
        IsDeleted = this.IsDeleted.GetBoolean(reader),
        SecurityToken = this.SecurityToken.GetString(reader, false),
        Path = this.Path.GetString(reader, false),
        HasChildren = this.HasChildren.GetBoolean(reader)
      };
    }

    protected class DrilldownQueryDataBinder6 : WorkItemTrackingObjectBinder<QueryItemEntry>
    {
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      private SqlColumnBinder ParentId = new SqlColumnBinder(nameof (ParentId));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder CreatedById = new SqlColumnBinder(nameof (CreatedById));
      private SqlColumnBinder CreatedByName = new SqlColumnBinder(nameof (CreatedByName));
      private SqlColumnBinder CreatedDate = new SqlColumnBinder(nameof (CreatedDate));
      private SqlColumnBinder ModifiedById = new SqlColumnBinder(nameof (ModifiedById));
      private SqlColumnBinder ModifiedByName = new SqlColumnBinder(nameof (ModifiedByName));
      private SqlColumnBinder ModifiedDate = new SqlColumnBinder(nameof (ModifiedDate));
      private SqlColumnBinder Wiql = new SqlColumnBinder(nameof (Wiql));
      private SqlColumnBinder IsFolder = new SqlColumnBinder(nameof (IsFolder));
      private SqlColumnBinder IsPublic = new SqlColumnBinder(nameof (IsPublic));
      private SqlColumnBinder IsDeleted = new SqlColumnBinder(nameof (IsDeleted));
      private SqlColumnBinder SecurityToken = new SqlColumnBinder(nameof (SecurityToken));
      private SqlColumnBinder HasChildren = new SqlColumnBinder(nameof (HasChildren));
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder Path = new SqlColumnBinder(nameof (Path));
      private SqlColumnBinder LastExecutedById = new SqlColumnBinder(nameof (LastExecutedById));
      private SqlColumnBinder LastExecutedDate = new SqlColumnBinder(nameof (LastExecutedDate));
      private QueryItemComponent m_queryItemComponent;

      public DrilldownQueryDataBinder6(QueryItemComponent queryItemComponent) => this.m_queryItemComponent = queryItemComponent;

      public override QueryItemEntry Bind(IDataReader reader) => new QueryItemEntry()
      {
        Id = this.Id.GetGuid(reader),
        ParentId = this.ParentId.GetGuid(reader, true),
        ProjectId = this.m_queryItemComponent.GetDataspaceIdentifier(this.DataspaceId.GetInt32(reader)),
        Name = this.Name.GetString(reader, false),
        CreatedById = this.CreatedById.GetGuid(reader, true),
        CreatedByName = this.CreatedByName.GetString(reader, true),
        CreatedDate = this.CreatedDate.GetDateTime(reader),
        ModifiedById = this.ModifiedById.GetGuid(reader, true),
        ModifiedByName = this.ModifiedByName.GetString(reader, true),
        ModifiedDate = this.ModifiedDate.GetDateTime(reader),
        Wiql = this.Wiql.GetString(reader, true),
        IsFolder = this.IsFolder.GetBoolean(reader),
        IsPublic = this.IsPublic.GetBoolean(reader),
        IsDeleted = this.IsDeleted.GetBoolean(reader),
        SecurityToken = this.SecurityToken.GetString(reader, false),
        Path = this.Path.GetString(reader, false),
        LastExecutedById = this.LastExecutedById.GetNullableGuid(reader),
        LastExecutedDate = this.LastExecutedDate.GetNullableDateTime(reader),
        HasChildren = this.HasChildren.GetBoolean(reader)
      };
    }

    protected class DrilldownQueryDataBinder7 : WorkItemTrackingObjectBinder<QueryItemEntry>
    {
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      private SqlColumnBinder ParentId = new SqlColumnBinder(nameof (ParentId));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder CreatedById = new SqlColumnBinder(nameof (CreatedById));
      private SqlColumnBinder CreatedByName = new SqlColumnBinder(nameof (CreatedByName));
      private SqlColumnBinder CreatedDate = new SqlColumnBinder(nameof (CreatedDate));
      private SqlColumnBinder ModifiedById = new SqlColumnBinder(nameof (ModifiedById));
      private SqlColumnBinder ModifiedByName = new SqlColumnBinder(nameof (ModifiedByName));
      private SqlColumnBinder ModifiedDate = new SqlColumnBinder(nameof (ModifiedDate));
      private SqlColumnBinder Wiql = new SqlColumnBinder(nameof (Wiql));
      private SqlColumnBinder IsFolder = new SqlColumnBinder(nameof (IsFolder));
      private SqlColumnBinder IsPublic = new SqlColumnBinder(nameof (IsPublic));
      private SqlColumnBinder IsDeleted = new SqlColumnBinder(nameof (IsDeleted));
      private SqlColumnBinder SecurityToken = new SqlColumnBinder(nameof (SecurityToken));
      private SqlColumnBinder HasChildren = new SqlColumnBinder(nameof (HasChildren));
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder Path = new SqlColumnBinder(nameof (Path));
      private SqlColumnBinder LastExecutedById = new SqlColumnBinder(nameof (LastExecutedById));
      private SqlColumnBinder LastExecutedDate = new SqlColumnBinder(nameof (LastExecutedDate));
      private SqlColumnBinder QueryType = new SqlColumnBinder(nameof (QueryType));
      private QueryItemComponent m_queryItemComponent;

      public DrilldownQueryDataBinder7(QueryItemComponent queryItemComponent) => this.m_queryItemComponent = queryItemComponent;

      public override QueryItemEntry Bind(IDataReader reader) => new QueryItemEntry()
      {
        Id = this.Id.GetGuid(reader),
        ParentId = this.ParentId.GetGuid(reader, true),
        ProjectId = this.m_queryItemComponent.GetDataspaceIdentifier(this.DataspaceId.GetInt32(reader)),
        Name = this.Name.GetString(reader, false),
        CreatedById = this.CreatedById.GetGuid(reader, true),
        CreatedByName = this.CreatedByName.GetString(reader, true),
        CreatedDate = this.CreatedDate.GetDateTime(reader),
        ModifiedById = this.ModifiedById.GetGuid(reader, true),
        ModifiedByName = this.ModifiedByName.GetString(reader, true),
        ModifiedDate = this.ModifiedDate.GetDateTime(reader),
        Wiql = this.Wiql.GetString(reader, true),
        IsFolder = this.IsFolder.GetBoolean(reader),
        IsPublic = this.IsPublic.GetBoolean(reader),
        IsDeleted = this.IsDeleted.GetBoolean(reader),
        SecurityToken = this.SecurityToken.GetString(reader, false),
        Path = this.Path.GetString(reader, false),
        LastExecutedById = this.LastExecutedById.GetNullableGuid(reader),
        LastExecutedDate = this.LastExecutedDate.GetNullableDateTime(reader),
        HasChildren = this.HasChildren.GetBoolean(reader),
        QueryType = this.QueryType.GetNullableInt32(reader)
      };
    }

    protected class FullTreeQueryDataBinder1 : WorkItemTrackingObjectBinder<QueryItemEntry>
    {
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      private SqlColumnBinder ProjectId = new SqlColumnBinder(nameof (ProjectId));
      private SqlColumnBinder ParentId = new SqlColumnBinder(nameof (ParentId));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder CreatedDate = new SqlColumnBinder(nameof (CreatedDate));
      private SqlColumnBinder ModifiedById = new SqlColumnBinder(nameof (ModifiedById));
      private SqlColumnBinder ModifiedDate = new SqlColumnBinder(nameof (ModifiedDate));
      private SqlColumnBinder Wiql = new SqlColumnBinder(nameof (Wiql));
      private SqlColumnBinder IsFolder = new SqlColumnBinder(nameof (IsFolder));
      private SqlColumnBinder IsDeleted = new SqlColumnBinder(nameof (IsDeleted));
      private SqlColumnBinder PrivateQueryOwner = new SqlColumnBinder(nameof (PrivateQueryOwner));

      public override QueryItemEntry Bind(IDataReader reader)
      {
        Guid guid = this.PrivateQueryOwner.GetGuid(reader, true);
        return new QueryItemEntry()
        {
          Id = this.Id.GetGuid(reader),
          ParentId = this.ParentId.GetGuid(reader, true),
          ProjectId = this.ProjectId.GetGuid(reader, false),
          Name = this.Name.GetString(reader, false),
          CreatedById = guid,
          CreatedDate = this.CreatedDate.GetDateTime(reader),
          ModifiedById = this.ModifiedById.GetGuid(reader, true),
          ModifiedDate = this.ModifiedDate.GetDateTime(reader),
          Wiql = this.Wiql.GetString(reader, true),
          IsFolder = this.IsFolder.GetBoolean(reader),
          IsPublic = guid == Guid.Empty,
          IsDeleted = this.IsDeleted.GetBoolean(reader)
        };
      }
    }

    protected class FullTreeQueryDataBinder2 : WorkItemTrackingObjectBinder<QueryItemEntry>
    {
      private QueryItemComponent m_queryItemComponent;
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder ParentId = new SqlColumnBinder(nameof (ParentId));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder CreatedDate = new SqlColumnBinder(nameof (CreatedDate));
      private SqlColumnBinder ModifiedById = new SqlColumnBinder(nameof (ModifiedById));
      private SqlColumnBinder ModifiedDate = new SqlColumnBinder(nameof (ModifiedDate));
      private SqlColumnBinder Wiql = new SqlColumnBinder(nameof (Wiql));
      private SqlColumnBinder IsFolder = new SqlColumnBinder(nameof (IsFolder));
      private SqlColumnBinder IsDeleted = new SqlColumnBinder(nameof (IsDeleted));
      private SqlColumnBinder PrivateQueryOwner = new SqlColumnBinder(nameof (PrivateQueryOwner));

      public FullTreeQueryDataBinder2(QueryItemComponent queryItemComponent) => this.m_queryItemComponent = queryItemComponent;

      public override QueryItemEntry Bind(IDataReader reader)
      {
        Guid guid = this.PrivateQueryOwner.GetGuid(reader, true);
        return new QueryItemEntry()
        {
          Id = this.Id.GetGuid(reader),
          ParentId = this.ParentId.GetGuid(reader, true),
          ProjectId = this.m_queryItemComponent.GetDataspaceIdentifier(this.DataspaceId.GetInt32(reader)),
          Name = this.Name.GetString(reader, false),
          CreatedById = guid,
          CreatedDate = this.CreatedDate.GetDateTime(reader),
          ModifiedById = this.ModifiedById.GetGuid(reader, true),
          ModifiedDate = this.ModifiedDate.GetDateTime(reader),
          Wiql = this.Wiql.GetString(reader, true),
          IsFolder = this.IsFolder.GetBoolean(reader),
          IsPublic = guid == Guid.Empty,
          IsDeleted = this.IsDeleted.GetBoolean(reader)
        };
      }
    }

    protected class FullTreeQueryDataBinder3 : WorkItemTrackingObjectBinder<QueryItemEntry>
    {
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder ParentId = new SqlColumnBinder(nameof (ParentId));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder CreatedDate = new SqlColumnBinder(nameof (CreatedDate));
      private SqlColumnBinder ModifiedById = new SqlColumnBinder(nameof (ModifiedById));
      private SqlColumnBinder ModifiedDate = new SqlColumnBinder(nameof (ModifiedDate));
      private SqlColumnBinder Wiql = new SqlColumnBinder(nameof (Wiql));
      private SqlColumnBinder IsFolder = new SqlColumnBinder(nameof (IsFolder));
      private SqlColumnBinder IsPublic = new SqlColumnBinder(nameof (IsPublic));
      private SqlColumnBinder IsDeleted = new SqlColumnBinder(nameof (IsDeleted));
      private SqlColumnBinder PrivateQueryOwner = new SqlColumnBinder(nameof (PrivateQueryOwner));
      private SqlColumnBinder CreatedById = new SqlColumnBinder(nameof (CreatedById));
      private SqlColumnBinder CreatedByName = new SqlColumnBinder(nameof (CreatedByName));
      private SqlColumnBinder ModifiedByName = new SqlColumnBinder(nameof (ModifiedByName));
      private QueryItemComponent m_queryItemComponent;

      public FullTreeQueryDataBinder3(QueryItemComponent queryItemComponent) => this.m_queryItemComponent = queryItemComponent;

      public override QueryItemEntry Bind(IDataReader reader) => new QueryItemEntry()
      {
        Id = this.Id.GetGuid(reader),
        ParentId = this.ParentId.GetGuid(reader, true),
        ProjectId = this.m_queryItemComponent.GetDataspaceIdentifier(this.DataspaceId.GetInt32(reader)),
        Name = this.Name.GetString(reader, false),
        CreatedById = this.CreatedById.GetGuid(reader, true),
        CreatedByName = this.CreatedByName.GetString(reader, true),
        CreatedDate = this.CreatedDate.GetDateTime(reader),
        ModifiedById = this.ModifiedById.GetGuid(reader, true),
        ModifiedByName = this.ModifiedByName.GetString(reader, true),
        ModifiedDate = this.ModifiedDate.GetDateTime(reader),
        Wiql = this.Wiql.GetString(reader, true),
        IsFolder = this.IsFolder.GetBoolean(reader),
        IsPublic = this.IsPublic.GetBoolean(reader),
        IsDeleted = this.IsDeleted.GetBoolean(reader)
      };
    }

    protected class QueryItemDataBinder : WorkItemTrackingObjectBinder<QueryItemEntry>
    {
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder Wiql = new SqlColumnBinder(nameof (Wiql));
      private SqlColumnBinder IsFolder = new SqlColumnBinder(nameof (IsFolder));
      private SqlColumnBinder IsPublic = new SqlColumnBinder(nameof (IsPublic));
      private SqlColumnBinder IsDeleted = new SqlColumnBinder(nameof (IsDeleted));

      public override QueryItemEntry Bind(IDataReader reader) => new QueryItemEntry()
      {
        Id = this.Id.GetGuid(reader),
        Name = this.Name.GetString(reader, false),
        Wiql = this.Wiql.GetString(reader, true),
        IsFolder = this.IsFolder.GetBoolean(reader),
        IsPublic = this.IsPublic.GetBoolean(reader),
        IsDeleted = this.IsDeleted.GetBoolean(reader)
      };
    }

    protected class QueryItemWiqlDataBinder : WorkItemTrackingObjectBinder<QueryItemEntry>
    {
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      private SqlColumnBinder Wiql = new SqlColumnBinder(nameof (Wiql));

      public override QueryItemEntry Bind(IDataReader reader) => new QueryItemEntry()
      {
        Id = this.Id.GetGuid(reader),
        Wiql = this.Wiql.GetString(reader, true)
      };
    }

    internal interface ITableValuedParameter<T> where T : QueryItemComponent
    {
      void BindTable(T component);
    }

    public class QueryHashTable : QueryItemComponent.ITableValuedParameter<QueryItemComponent11>
    {
      private string parameterName;
      private IEnumerable<QueryHash> queryHashInfo;
      private static readonly SqlMetaData[] typ_QueryExecutionInformationTable = new SqlMetaData[2]
      {
        new SqlMetaData("QueryId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("QueryHash", SqlDbType.VarChar, 64L)
      };

      public QueryHashTable(string parameterName, IEnumerable<QueryHash> queryHashInfo)
      {
        this.parameterName = parameterName;
        this.queryHashInfo = queryHashInfo;
      }

      public void BindTable(QueryItemComponent11 component) => component.Bind<QueryHash>(this.parameterName, this.queryHashInfo, QueryItemComponent.QueryHashTable.typ_QueryExecutionInformationTable, "typ_QueryHashTable", (Action<SqlDataRecord, QueryHash>) ((record, info) =>
      {
        if (info.QueryId != Guid.Empty)
          record.SetGuid(0, info.QueryId);
        else
          record.SetDBNull(0);
        if (!string.IsNullOrEmpty(info.Hash))
          record.SetString(1, info.Hash);
        else
          record.SetDBNull(1);
      }));
    }

    public class QueryTypeTable : QueryItemComponent.ITableValuedParameter<QueryItemComponent13>
    {
      private string parameterName;
      private IEnumerable<QueryTypeInfo> queryTypeInfos;
      private static readonly SqlMetaData[] typ_QueryTypeTable = new SqlMetaData[2]
      {
        new SqlMetaData("QueryId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("QueryType", SqlDbType.Int)
      };

      public QueryTypeTable(string parameterName, IEnumerable<QueryTypeInfo> queryTypeInfos)
      {
        this.parameterName = parameterName;
        this.queryTypeInfos = queryTypeInfos;
      }

      public void BindTable(QueryItemComponent13 component) => component.Bind<QueryTypeInfo>(this.parameterName, this.queryTypeInfos, QueryItemComponent.QueryTypeTable.typ_QueryTypeTable, "typ_QueryTypeTable", (Action<SqlDataRecord, QueryTypeInfo>) ((record, info) =>
      {
        if (info.QueryId != Guid.Empty)
          record.SetGuid(0, info.QueryId);
        else
          record.SetDBNull(0);
        record.SetInt32(1, (int) info.QueryType);
      }));
    }
  }
}
