// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedRetentionService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedRetentionService : IFeedRetentionService, IVssFrameworkService
  {
    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext) => requestContext.CheckProjectCollectionRequestContext();

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public IEnumerable<Guid> GetPackagesExceedingRetentionLimit(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      int countLimit)
    {
      using (PackageSqlResourceComponent component = requestContext.CreateComponent<PackageSqlResourceComponent>())
        return component.GetPackagesExceedingRetentionLimit(feed, countLimit);
    }

    public IEnumerable<ProtocolPackageVersionIdentity> GetVersionsExceedingRetentionLimit(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      int countLimit,
      IEnumerable<Guid> packageIds,
      int daysToKeepRecentlyDownloadedPackages,
      DateTime packageMetricsEnabledTimestamp)
    {
      using (PackageSqlResourceComponent component = requestContext.CreateComponent<PackageSqlResourceComponent>())
        return component.GetVersionsExceedingRetentionLimitV2(feed, countLimit, packageIds, daysToKeepRecentlyDownloadedPackages, packageMetricsEnabledTimestamp);
    }
  }
}
