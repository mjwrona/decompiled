// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedSqlResourceComponent4
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedSqlResourceComponent4 : FeedSqlResourceComponent3
  {
    public override Microsoft.VisualStudio.Services.Feed.WebApi.Feed CreateFeed(
      FeedIdentity feedId,
      string feedName,
      string feedDescription,
      bool upstreamEnabled,
      bool allowUpstreamNameConflict,
      bool hideDeletedPackageVersions,
      IList<UpstreamSource> upstreamSources,
      FeedCapabilities capabilities,
      bool badgesEnabled)
    {
      this.PrepareStoredProcedure("Feed.prc_CreateFeed");
      this.BindFeedIdentity(feedId, true);
      this.BindString("@feedName", feedName, 64, false, SqlDbType.NVarChar);
      this.BindString("@feedDescription", feedDescription, (int) byte.MaxValue, true, SqlDbType.NVarChar);
      this.BindBoolean("@upstreamEnabled", upstreamEnabled);
      this.BindBoolean("@allowUpstreamNameConflict", allowUpstreamNameConflict);
      return this.ReadFeed();
    }

    public override Microsoft.VisualStudio.Services.Feed.WebApi.Feed CreateFeed(
      FeedIdentity feedId,
      string feedName,
      string feedDescription,
      bool upstreamEnabled,
      bool allowUpstreamNameConflict,
      int deleteHoldLifetime,
      bool hideDeletedPackageVersions,
      IList<UpstreamSource> upstreamSources,
      FeedCapabilities capabilities,
      bool badgesEnabled,
      bool isTestCode)
    {
      if (!isTestCode)
        throw new NotSupportedException();
      this.PrepareStoredProcedure("Feed.prc_CreateFeed");
      this.BindFeedIdentity(feedId, true);
      this.BindString("@feedName", feedName, 64, false, SqlDbType.NVarChar);
      this.BindString("@feedDescription", feedDescription, (int) byte.MaxValue, true, SqlDbType.NVarChar);
      this.BindBoolean("@upstreamEnabled", upstreamEnabled);
      this.BindBoolean("@allowUpstreamNameConflict", allowUpstreamNameConflict);
      this.BindInt("@deleteHoldLifeTime", deleteHoldLifetime);
      return this.ReadFeed();
    }

    public override Microsoft.VisualStudio.Services.Feed.WebApi.Feed UpdateFeed(
      FeedIdentity feedId,
      string feedDescription,
      bool? upstreamEnabled,
      bool? allowUpstreamNameConflict,
      bool? hideDeletedPackageVersions,
      Guid? defaultReaderViewId,
      IList<UpstreamSource> upstreamSources,
      bool? badgesEnabled)
    {
      this.PrepareStoredProcedure("Feed.prc_UpdateFeed");
      this.BindFeedIdentity(feedId);
      this.BindString("@feedDescription", feedDescription, (int) byte.MaxValue, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindNullableBoolean("@upstreamEnabled", upstreamEnabled);
      this.BindNullableBoolean("@allowUpstreamNameConflict", allowUpstreamNameConflict);
      return this.ReadFeed();
    }
  }
}
