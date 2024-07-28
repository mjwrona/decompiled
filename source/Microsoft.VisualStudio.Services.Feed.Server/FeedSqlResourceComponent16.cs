// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedSqlResourceComponent16
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedSqlResourceComponent16 : FeedSqlResourceComponent15
  {
    public override IEnumerable<DownstreamFeed> GetDownstreamFeedsFromUpstreamFromAllPartitions(
      Guid upstreamCollectionId,
      Guid upstreamFeedId,
      Guid upstreamViewId,
      string protocolType)
    {
      this.PrepareStoredProcedure("Feed.prc_GetDownstreamFeedsFromUpstreamFromAllPartitions", false);
      this.BindGuid("@UpstreamCollectionId", upstreamCollectionId);
      this.BindGuid("@UpstreamFeedId", upstreamFeedId);
      this.BindGuid("@UpstreamViewId", upstreamViewId);
      this.BindString("@ProtocolType", protocolType, 20, false, SqlDbType.NVarChar);
      return this.ReadDownstreamFeeds();
    }

    public override IEnumerable<DownstreamFeed> GetFeedsAffectedByUpstreamChange(
      Guid upstreamCollectionId,
      Guid upstreamFeedId,
      Guid upstreamViewId,
      string protocolType,
      string normalizedPackageName)
    {
      this.PrepareStoredProcedure("Feed.prc_GetFeedsAffectedByUpstreamChange", false);
      this.BindGuid("@UpstreamCollectionId", upstreamCollectionId);
      this.BindGuid("@UpstreamFeedId", upstreamFeedId);
      this.BindGuid("@UpstreamViewId", upstreamViewId);
      this.BindString("@ProtocolType", protocolType, 20, false, SqlDbType.NVarChar);
      this.BindString("@normalizedPackageName", normalizedPackageName, (int) byte.MaxValue, false, SqlDbType.NVarChar);
      return this.ReadDownstreamFeeds();
    }
  }
}
