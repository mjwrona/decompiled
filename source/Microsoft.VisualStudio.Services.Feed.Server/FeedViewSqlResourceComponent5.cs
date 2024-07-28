// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedViewSqlResourceComponent5
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.VisualStudio.Services.Feed.Common;
using System;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedViewSqlResourceComponent5 : FeedViewSqlResourceComponent4
  {
    protected override void BindFeedIdentity(
      FeedIdentity feedId,
      bool createMissingDataspace = false,
      string dataspaceName = "@dataspaceId",
      string feedIdName = "@feedId")
    {
      this.BindInt(dataspaceName, this.GetFeedDataspaceId(feedId.ProjectId, createMissingDataspace));
      this.BindGuid(feedIdName, feedId.Id);
    }

    public override int AddPackageVersionToView(
      FeedIdentity feedId,
      Guid viewId,
      Guid packageVersionId)
    {
      this.PrepareStoredProcedure("Feed.prc_AddPackageVersionToFeedView");
      this.BindInt("@dataspaceId", this.GetFeedDataspaceId(feedId.ProjectId));
      this.BindGuid("@viewId", viewId);
      this.BindGuid("@packageVersionId", packageVersionId);
      return this.ExecuteNonQuery();
    }

    public override void RemovePackageVersionFromView(
      FeedIdentity feedId,
      Guid viewId,
      Guid packageVersionId)
    {
      this.PrepareStoredProcedure("Feed.prc_RemovePackageVersionFromFeedView");
      this.BindInt("@dataspaceId", this.GetFeedDataspaceId(feedId.ProjectId));
      this.BindGuid("@viewId", viewId);
      this.BindGuid("@packageVersionId", packageVersionId);
      this.ExecuteNonQuery();
    }
  }
}
