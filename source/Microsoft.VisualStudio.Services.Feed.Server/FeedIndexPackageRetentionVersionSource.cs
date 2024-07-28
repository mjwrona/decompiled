// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedIndexPackageRetentionVersionSource
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedIndexPackageRetentionVersionSource : IPackageRetentionVersionSource
  {
    private readonly IFeedRetentionService feedRetentionService;

    public FeedIndexPackageRetentionVersionSource(IFeedRetentionService feedRetentionService) => this.feedRetentionService = feedRetentionService;

    public IEnumerable<ProtocolPackageVersionIdentity> GetPackagesEligibleForDeletion(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      FeedRetentionPolicy retentionPolicy)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(feed, nameof (feed));
      ArgumentUtility.CheckForNull<FeedRetentionPolicy>(retentionPolicy, nameof (retentionPolicy));
      int? countLimit = retentionPolicy.CountLimit;
      if (countLimit.HasValue)
      {
        IEnumerable<Guid> exceedingRetentionLimit = this.GetPackagesExceedingRetentionLimit(requestContext, feed, countLimit.Value);
        if (exceedingRetentionLimit.Any<Guid>())
        {
          int valueOrDefault = retentionPolicy.DaysToKeepRecentlyDownloadedPackages.GetValueOrDefault();
          return this.GetVersionsExceedingRetentionLimit(requestContext, feed, countLimit.Value, exceedingRetentionLimit, valueOrDefault);
        }
      }
      return Enumerable.Empty<ProtocolPackageVersionIdentity>();
    }

    private IEnumerable<Guid> GetPackagesExceedingRetentionLimit(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      int countLimit)
    {
      return this.feedRetentionService.GetPackagesExceedingRetentionLimit(requestContext, feed, countLimit);
    }

    private IEnumerable<ProtocolPackageVersionIdentity> GetVersionsExceedingRetentionLimit(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      int countLimit,
      IEnumerable<Guid> packageGuids,
      int daysToKeepRecentlyDownloadedPackages)
    {
      DateTime packageMetricsEnabledTimestamp = requestContext.GetService<IVssRegistryService>().GetValue<DateTime>(requestContext, (RegistryQuery) "/Configuration/Feed/PackageRetention/PackageMetricsEnabledTimestamp", true, DateTime.Parse("01/01/2019"));
      return this.feedRetentionService.GetVersionsExceedingRetentionLimit(requestContext, feed, countLimit, packageGuids, daysToKeepRecentlyDownloadedPackages, packageMetricsEnabledTimestamp);
    }
  }
}
