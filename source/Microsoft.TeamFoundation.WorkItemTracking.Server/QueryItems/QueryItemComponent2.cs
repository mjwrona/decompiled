// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItemComponent2
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems
{
  internal class QueryItemComponent2 : QueryItemComponent
  {
    public override IEnumerable<QueryItemEntry> GetRootQueryItemEntries(
      Guid projectId,
      Guid teamFoundationId,
      int? expandDepth,
      bool includeWiql,
      bool includeDeleted,
      int maxResultCount,
      bool includeExecutionInfo = false)
    {
      this.PrepareStoredProcedure("prc_GetRootQueryItems");
      this.BindProjectId(projectId);
      this.BindGuid("@teamFoundationId", teamFoundationId);
      this.BindNullableInt("@expandDepth", expandDepth.HasValue ? expandDepth.Value : -1, -1);
      this.BindBoolean("@includeWiql", includeWiql);
      this.BindIncludeDeleted(includeDeleted);
      this.BindMaxResultCount(maxResultCount);
      try
      {
        List<QueryItemEntry> list = this.ExecuteUnknown<IEnumerable<QueryItemEntry>>((System.Func<IDataReader, IEnumerable<QueryItemEntry>>) (reader => this.GetDrilldownQueryItemEntryBinder().BindAll(reader))).ToList<QueryItemEntry>();
        this.BuildTree((IEnumerable<QueryItemEntry>) list);
        return (IEnumerable<QueryItemEntry>) list.Where<QueryItemEntry>((System.Func<QueryItemEntry, bool>) (qe => qe.ParentId == Guid.Empty)).ToList<QueryItemEntry>();
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    public override IEnumerable<QueryItemEntry> GetQueryItemEntriesByIds(
      IEnumerable<Guid> queryIds,
      int? expandDepth,
      bool includeWiql,
      bool includeDeleted,
      int maxResultCount,
      bool includeExecutionInfo = false,
      Guid? filterUnderProjectId = null)
    {
      this.PrepareStoredProcedure("prc_GetQueryItems");
      this.BindGuidTable("@queryItemIds", queryIds);
      this.BindNullableInt("@expandDepth", expandDepth.HasValue ? expandDepth.Value : -1, -1);
      this.BindBoolean("@includeWiql", includeWiql);
      this.BindIncludeDeleted(includeDeleted);
      this.BindMaxResultCount(maxResultCount);
      try
      {
        List<QueryItemEntry> list = this.ExecuteUnknown<IEnumerable<QueryItemEntry>>((System.Func<IDataReader, IEnumerable<QueryItemEntry>>) (reader => this.GetDrilldownQueryItemEntryBinder().BindAll(reader))).ToList<QueryItemEntry>();
        Dictionary<Guid, QueryItemEntry> queryDictionary = list.ToDictionary<QueryItemEntry, Guid>((System.Func<QueryItemEntry, Guid>) (qe => qe.Id));
        this.BuildTree((IEnumerable<QueryItemEntry>) list);
        QueryItemEntry queryItemEntry;
        return queryIds.Select<Guid, QueryItemEntry>((System.Func<Guid, QueryItemEntry>) (qid => !queryDictionary.TryGetValue(qid, out queryItemEntry) ? (QueryItemEntry) null : queryItemEntry)).Where<QueryItemEntry>((System.Func<QueryItemEntry, bool>) (query => query != null));
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    public override QueryItemEntry GetQueryItemByPath(
      string path,
      Guid projectId,
      Guid teamFoundationId,
      int? expandDepth,
      bool includeWiql,
      bool includeDeleted,
      bool includeExecutionInfo = false)
    {
      this.PrepareStoredProcedure("prc_GetQueryItemByPath");
      this.BindProjectId(projectId);
      this.BindStringTable("@path", (IEnumerable<string>) path.Split(new char[1]
      {
        '/'
      }, StringSplitOptions.RemoveEmptyEntries));
      this.BindGuid("@teamFoundationId", teamFoundationId);
      this.BindNullableInt("@expandDepth", expandDepth.HasValue ? expandDepth.Value : -1, -1);
      this.BindBoolean("@includeWiql", includeWiql);
      this.BindIncludeDeleted(includeDeleted);
      try
      {
        List<QueryItemEntry> list = this.ExecuteUnknown<IEnumerable<QueryItemEntry>>((System.Func<IDataReader, IEnumerable<QueryItemEntry>>) (reader => this.GetDrilldownQueryItemEntryBinder().BindAll(reader))).ToList<QueryItemEntry>();
        this.BuildTree((IEnumerable<QueryItemEntry>) list);
        QueryItemEntry queryItemByPath = list.FirstOrDefault<QueryItemEntry>();
        Dictionary<Guid, QueryItemEntry> dictionary = list.ToDictionary<QueryItemEntry, Guid>((System.Func<QueryItemEntry, Guid>) (qe => qe.Id));
        if (queryItemByPath == null)
          return (QueryItemEntry) null;
        QueryItemEntry queryItemEntry;
        while (dictionary.TryGetValue(queryItemByPath.ParentId, out queryItemEntry))
          queryItemByPath = queryItemEntry;
        return queryItemByPath;
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    public override QueryItemEntry UndeleteQueryItem(
      Guid queryId,
      bool undeleteDescendants,
      Guid teamFoundationId,
      int maxQueryItemChildrenUnderParent)
    {
      this.PrepareStoredProcedure("prc_UndeleteQueryItem");
      this.BindGuid("@queryId", queryId);
      this.BindBoolean("@undeleteDescendants", undeleteDescendants);
      this.BindGuid("@teamFoundationId", teamFoundationId);
      this.BindMaxQueryItemChildrenUnderParent(maxQueryItemChildrenUnderParent);
      List<QueryItemEntry> list = this.ExecuteUnknown<IEnumerable<QueryItemEntry>>((System.Func<IDataReader, IEnumerable<QueryItemEntry>>) (reader => this.GetDrilldownQueryItemEntryBinder().BindAll(reader))).ToList<QueryItemEntry>();
      Dictionary<Guid, QueryItemEntry> dictionary = list.ToDictionary<QueryItemEntry, Guid>((System.Func<QueryItemEntry, Guid>) (qe => qe.Id));
      this.BuildTree((IEnumerable<QueryItemEntry>) list);
      Guid key = queryId;
      return dictionary[key];
    }

    protected override WorkItemTrackingObjectBinder<QueryItemEntry> GetDrilldownQueryItemEntryBinder() => (WorkItemTrackingObjectBinder<QueryItemEntry>) new QueryItemComponent.DrilldownQueryDataBinder2();

    protected virtual void BindMaxResultCount(int maxResultCount)
    {
    }

    protected virtual void BindMaxNonDeletedPublicQueriesCount(int maxResultCount)
    {
    }

    protected virtual void BindGrossMaxResultCount(int grossMaxResultCount)
    {
    }
  }
}
