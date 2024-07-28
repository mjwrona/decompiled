// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.PackageRefreshJobHelper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream
{
  public static class PackageRefreshJobHelper
  {
    public static Func<FeedCore, bool> RejectCachedFeedIfTriggeringUpstreamNotPresent(
      PushDrivenUpstreamsNotificationTelemetry notificationTelemetry)
    {
      if (notificationTelemetry == null)
        return (Func<FeedCore, bool>) null;
      Guid? upstreamViewId1 = notificationTelemetry.UpstreamViewId;
      Guid empty = Guid.Empty;
      return (upstreamViewId1.HasValue ? (upstreamViewId1.HasValue ? (upstreamViewId1.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) != 0 ? (Func<FeedCore, bool>) null : (Func<FeedCore, bool>) (cachedFeed => !cachedFeed.UpstreamEnabled || cachedFeed.UpstreamSources == null || cachedFeed.UpstreamSources.FirstOrDefault<UpstreamSource>((Func<UpstreamSource, bool>) (x =>
      {
        Guid? internalUpstreamViewId = x.InternalUpstreamViewId;
        Guid? upstreamViewId2 = notificationTelemetry.UpstreamViewId;
        if (internalUpstreamViewId.HasValue != upstreamViewId2.HasValue)
          return false;
        return !internalUpstreamViewId.HasValue || internalUpstreamViewId.GetValueOrDefault() == upstreamViewId2.GetValueOrDefault();
      })) == null);
    }

    public static IEnumerable<Guid> GetFeedsToRefresh(UpstreamMetadataCachePackageJobData data)
    {
      if (data == null)
        throw new ArgumentException("No job data to determine what feeds to refresh");
      return data.FeedsToRefresh != null ? (IEnumerable<Guid>) data.FeedsToRefresh : throw new ArgumentException("Job data must contain the feeds to refresh");
    }
  }
}
