// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedMetricsService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.Server.Utils;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedMetricsService : IFeedMetricsService, IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void IngestRawMetrics(
      IVssRequestContext requestContext,
      List<PackageMetricsData> packageMetricsUpdates)
    {
      FeedSecurityHelper.CheckModifyIndexPermissions(requestContext);
      using (PackageMetricsSqlResourceComponent component = requestContext.CreateComponent<PackageMetricsSqlResourceComponent>())
        component.IngestRawMetrics((IEnumerable<PackageMetricsData>) packageMetricsUpdates);
      MetricsJobUtils.QueueMetricsAggregationJob(requestContext, JobConstants.UserDownloadAggregationJob.DownloadAggregationJobId);
    }

    public IEnumerable<PackageMetrics> QueryPackageMetrics(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      PackageMetricsQuery packageIdQuery)
    {
      ArgumentUtility.CheckForNull<PackageMetricsQuery>(packageIdQuery, nameof (packageIdQuery));
      IEnumerable<Guid> packageIds = (IEnumerable<Guid>) packageIdQuery.PackageIds;
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(packageIds, "packageIds");
      FeedSecurityHelper.CheckReadFeedPermissions(requestContext, (FeedCore) feed);
      using (PackageMetricsSqlResourceComponent component = requestContext.CreateComponent<PackageMetricsSqlResourceComponent>())
      {
        FeedIdentity feedIdentity = new FeedIdentity(feed.Project?.Id, feed.Id);
        return component.QueryPackageMetrics(feedIdentity, packageIds.Distinct<Guid>());
      }
    }

    public IEnumerable<PackageVersionMetrics> QueryPackageVersionMetrics(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      PackageVersionMetricsQuery packageVersionIdQuery)
    {
      ArgumentUtility.CheckForNull<PackageVersionMetricsQuery>(packageVersionIdQuery, nameof (packageVersionIdQuery));
      IEnumerable<Guid> packageVersionIds = (IEnumerable<Guid>) packageVersionIdQuery.PackageVersionIds;
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(packageVersionIds, "packageVersionIds");
      FeedSecurityHelper.CheckReadFeedPermissions(requestContext, (FeedCore) feed);
      using (PackageMetricsSqlResourceComponent component = requestContext.CreateComponent<PackageMetricsSqlResourceComponent>())
      {
        FeedIdentity feedIdentity = new FeedIdentity(feed.Project?.Id, feed.Id);
        return component.QueryPackageVersionMetrics(feedIdentity, packageId, packageVersionIds.Distinct<Guid>());
      }
    }
  }
}
