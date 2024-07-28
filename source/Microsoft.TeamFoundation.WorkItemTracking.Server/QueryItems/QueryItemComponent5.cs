// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItemComponent5
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems
{
  internal class QueryItemComponent5 : QueryItemComponent4
  {
    public override IEnumerable<QueryItemEntry> GetChangedPublicQueryItems(
      Guid projectId,
      long? timestamp,
      int maxResultCount,
      out long newTimestamp)
    {
      this.PrepareStoredProcedure("prc_GetChangedPublicQueryItems");
      this.BindProjectId(projectId);
      this.BindLong("@timestamp", timestamp.HasValue ? timestamp.Value : -1L);
      this.BindMaxNonDeletedPublicQueriesCount(maxResultCount);
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

    public override QueryItemEntry GetPublicQueryItemsHierarchy(
      Guid projectId,
      int maxResultCount,
      out long timestamp,
      out int queryEntryCount)
    {
      this.PrepareStoredProcedure("prc_GetPublicQueryItems");
      this.BindProjectId(projectId);
      this.BindMaxResultCount(maxResultCount);
      long tempTimestamp = 0;
      List<QueryItemEntry> list = this.ExecuteUnknown<IEnumerable<QueryItemEntry>>((System.Func<IDataReader, IEnumerable<QueryItemEntry>>) (reader =>
      {
        reader.Read();
        tempTimestamp = reader.IsDBNull(0) ? -1L : reader.GetInt64(0);
        reader.NextResult();
        return this.GetFullTreeQueryItemEntryBinder().BindAll(reader);
      })).ToList<QueryItemEntry>();
      queryEntryCount = list.Count;
      timestamp = tempTimestamp;
      this.BuildTree((IEnumerable<QueryItemEntry>) list);
      return list.Where<QueryItemEntry>((System.Func<QueryItemEntry, bool>) (qe => qe.ParentId == Guid.Empty)).FirstOrDefault<QueryItemEntry>();
    }

    public override QueryItemEntry GetPrivateQueryItemsHierarchy(
      Guid projectId,
      Guid teamFoundationId,
      int maxResultCount,
      int grossMaxResultCount)
    {
      this.PrepareStoredProcedure("prc_GetPrivateQueryItems");
      this.BindProjectId(projectId);
      this.BindGuid("@teamFoundationId", teamFoundationId);
      this.BindMaxResultCount(maxResultCount);
      this.BindGrossMaxResultCount(grossMaxResultCount);
      List<QueryItemEntry> list = this.ExecuteUnknown<IEnumerable<QueryItemEntry>>((System.Func<IDataReader, IEnumerable<QueryItemEntry>>) (reader => this.GetFullTreeQueryItemEntryBinder().BindAll(reader))).ToList<QueryItemEntry>();
      this.BuildTree((IEnumerable<QueryItemEntry>) list);
      return list.Where<QueryItemEntry>((System.Func<QueryItemEntry, bool>) (qe => qe.ParentId == Guid.Empty)).FirstOrDefault<QueryItemEntry>();
    }

    public override long GetPublicQueryItemsTimestamp(Guid projectId)
    {
      this.PrepareStoredProcedure("prc_GetPublicQueryItemsTimestamp");
      this.BindProjectId(projectId);
      return this.ExecuteUnknown<long>((System.Func<IDataReader, long>) (reader =>
      {
        reader.Read();
        return !reader.IsDBNull(0) ? reader.GetInt64(0) : -1L;
      }));
    }
  }
}
