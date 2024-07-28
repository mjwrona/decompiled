// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItemComponent6
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems
{
  internal class QueryItemComponent6 : QueryItemComponent5
  {
    protected override void BindMaxResultCount(int maxResultCount) => this.BindInt("@maxResultCount", maxResultCount);

    protected override void BindMaxNonDeletedPublicQueriesCount(int maxNonDeletedPublicQueriesCount) => this.BindInt("@maxNonDeletedPublicQueries", maxNonDeletedPublicQueriesCount);

    protected override void BindGrossMaxResultCount(int grossMaxResultCount) => this.BindInt("@grossMaxResultCount", grossMaxResultCount);

    public override QueryItemEntry CreateRootQueryFolder(
      Guid projectId,
      Guid teamFoundationId,
      string rootQueryFolderName,
      bool isPublic)
    {
      this.PrepareStoredProcedure("prc_CreateRootQueryFolder");
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
  }
}
