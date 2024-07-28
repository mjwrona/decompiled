// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedChangeSqlResourceComponent4
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedChangeSqlResourceComponent4 : FeedChangeSqlResourceComponent3
  {
    public override IEnumerable<FeedChange> GetFeedChanges(
      Guid? projectId,
      bool includeDeleted,
      long token,
      int batchSize)
    {
      this.PrepareStoredProcedure("Feed.prc_GetFeedChanges");
      this.BindNullableInt("@dataspaceId", projectId.HasValue ? new int?(this.GetFeedDataspaceId(projectId)) : new int?());
      this.BindBoolean("@includeDeleted", includeDeleted);
      this.BindLong("@token", token);
      this.BindInt("@batchSize", batchSize);
      return this.ReadFeedChangesWithUpstreams();
    }

    protected override void BindFeedIdentity(
      FeedIdentity feedId,
      bool createMissingDataspace = false,
      string dataspaceColumn = "@dataspaceId",
      string feedIdColumn = "@feedId")
    {
      this.BindInt(dataspaceColumn, this.GetFeedDataspaceId(feedId.ProjectId, createMissingDataspace));
      this.BindGuid(feedIdColumn, feedId.Id);
    }
  }
}
