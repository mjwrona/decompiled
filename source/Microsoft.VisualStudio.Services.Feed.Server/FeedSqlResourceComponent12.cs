// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedSqlResourceComponent12
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.VisualStudio.Services.Feed.Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedSqlResourceComponent12 : FeedSqlResourceComponent11
  {
    public override IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeeds(
      Guid? projectId,
      bool includeDeletedUpstreams = false)
    {
      this.PrepareStoredProcedure("Feed.prc_GetFeeds");
      this.BindInt("@dataspaceId", this.GetFeedDataspaceId(projectId));
      int num;
      if (projectId.HasValue)
      {
        Guid? nullable = projectId;
        Guid empty = Guid.Empty;
        num = nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1;
      }
      else
        num = 0;
      this.BindBoolean("@isProjectScopedQuery", num != 0);
      this.BindBoolean("@includeDeletedUpstreams", includeDeletedUpstreams);
      return (IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) this.ReadFeedsAndUpstreams();
    }

    public override Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeed(
      string feedName,
      Guid? projectId,
      bool includeDeleted = false,
      bool includeDeletedUpstreams = false)
    {
      this.PrepareStoredProcedure("Feed.prc_GetFeed");
      this.BindInt("@dataspaceId", this.GetFeedDataspaceId(projectId));
      this.BindString("@feedName", feedName, 64, false, SqlDbType.NVarChar);
      this.BindBoolean("@includeDeleted", includeDeleted);
      this.BindBoolean("@includeDeletedUpstreams", includeDeletedUpstreams);
      return this.ReadFeedAndUpstreams();
    }

    protected override void BindFeedIdentity(
      FeedIdentity feedId,
      bool createMissingDataspace = false,
      string dataspaceName = "@dataspaceId",
      string feedIdName = "@feedId")
    {
      this.BindInt(dataspaceName, this.GetFeedDataspaceId(feedId.ProjectId, createMissingDataspace));
      this.BindGuid(feedIdName, feedId.Id);
    }
  }
}
