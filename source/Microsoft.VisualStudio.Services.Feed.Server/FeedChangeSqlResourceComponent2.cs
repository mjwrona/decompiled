// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedChangeSqlResourceComponent2
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedChangeSqlResourceComponent2 : FeedChangeSqlResourceComponent
  {
    public override IEnumerable<FeedChange> GetFeedChanges(
      Guid? projectId,
      bool includeDeleted,
      long token,
      int batchSize)
    {
      this.PrepareStoredProcedure("Feed.prc_GetFeedChanges");
      this.BindInt("@dataspaceId", this.GetDataspaceId(Guid.Empty));
      this.BindBoolean("@includeDeleted", includeDeleted);
      this.BindLong("@token", token);
      this.BindInt("@batchSize", batchSize);
      return this.ReadFeedChangesWithUpstreams();
    }

    public override FeedChange GetFeedChange(FeedIdentity feedId)
    {
      this.PrepareStoredProcedure("Feed.prc_GetFeedChange");
      this.BindFeedIdentity(feedId);
      return this.ReadFeedChangeWithUpstreams();
    }
  }
}
