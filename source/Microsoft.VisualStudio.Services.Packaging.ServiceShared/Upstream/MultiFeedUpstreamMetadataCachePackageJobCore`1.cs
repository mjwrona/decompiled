// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.MultiFeedUpstreamMetadataCachePackageJobCore`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream
{
  public class MultiFeedUpstreamMetadataCachePackageJobCore<TPackageName> where TPackageName : IPackageName
  {
    private readonly IFeedService feedService;
    private readonly IPackagingTracesBasicInfo packagingTracesBasicInfo;
    private readonly IFactory<IPackageNameRequest<TPackageName>, IAsyncHandler<IPackageNameRequest<TPackageName, PushDrivenUpstreamsNotificationTelemetry>, JobResult>> handlerFactory;

    public MultiFeedUpstreamMetadataCachePackageJobCore(
      IFeedService feedService,
      IPackagingTracesBasicInfo packagingTracesBasicInfo,
      IFactory<IPackageNameRequest<TPackageName>, IAsyncHandler<IPackageNameRequest<TPackageName, PushDrivenUpstreamsNotificationTelemetry>, JobResult>> handlerFactory)
    {
      this.feedService = feedService;
      this.packagingTracesBasicInfo = packagingTracesBasicInfo;
      this.handlerFactory = handlerFactory;
    }

    public async Task<IEnumerable<JobResult>> RefreshFeedsAsync(
      IEnumerable<Guid> feedIds,
      TPackageName packageName,
      PushDrivenUpstreamsNotificationTelemetry notificationTelemetry)
    {
      List<JobResult> results = new List<JobResult>();
      foreach (Guid feedId in feedIds)
      {
        try
        {
          List<JobResult> jobResultList = results;
          jobResultList.Add(await this.RefreshOneFeedAsync(feedId, packageName, notificationTelemetry));
          jobResultList = (List<JobResult>) null;
        }
        catch (Exception ex)
        {
          JobTelemetry telemetry = new JobTelemetry();
          telemetry.LogException(ex);
          results.Add(JobResult.Failed(telemetry));
        }
      }
      IEnumerable<JobResult> jobResults = (IEnumerable<JobResult>) results;
      results = (List<JobResult>) null;
      return jobResults;
    }

    private async Task<JobResult> RefreshOneFeedAsync(
      Guid feedId,
      TPackageName packageName,
      PushDrivenUpstreamsNotificationTelemetry notificationTelemetry)
    {
      PackageNameRequest<TPackageName, PushDrivenUpstreamsNotificationTelemetry> packageNameRequest = new FeedRequest(await this.feedService.GetFeedByIdForAnyScopeAsync(feedId, rejectCachedFeedIf: PackageRefreshJobHelper.RejectCachedFeedIfTriggeringUpstreamNotPresent(notificationTelemetry)), packageName.Protocol).WithPackageName<TPackageName>(packageName).WithData<TPackageName, PushDrivenUpstreamsNotificationTelemetry>(notificationTelemetry);
      this.packagingTracesBasicInfo.SetFromFeedRequest((IProtocolAgnosticFeedRequest) packageNameRequest);
      return await this.handlerFactory.Get((IPackageNameRequest<TPackageName>) packageNameRequest).Handle((IPackageNameRequest<TPackageName, PushDrivenUpstreamsNotificationTelemetry>) packageNameRequest);
    }
  }
}
