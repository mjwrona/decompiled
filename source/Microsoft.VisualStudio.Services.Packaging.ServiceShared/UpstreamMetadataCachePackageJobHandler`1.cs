// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamMetadataCachePackageJobHandler`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class UpstreamMetadataCachePackageJobHandler<TPackageName> : 
    IAsyncHandler<IPackageNameRequest<TPackageName, PushDrivenUpstreamsNotificationTelemetry>, JobResult>,
    IHaveInputType<IPackageNameRequest<TPackageName, PushDrivenUpstreamsNotificationTelemetry>>,
    IHaveOutputType<JobResult>
    where TPackageName : IPackageName
  {
    private readonly IFactory<IFeedRequest, Task<IUpstreamMetadataManager>> upstreamMetadataManagerFactory;
    private IFactory<IFeedRequest, IUpstreamMetadataCacheInfoStore> metadataCacheInfoStoreFactory;
    private readonly ITracerService tracerService;

    public UpstreamMetadataCachePackageJobHandler(
      IFactory<IFeedRequest, Task<IUpstreamMetadataManager>> upstreamMetadataManagerFactory,
      IFactory<IFeedRequest, IUpstreamMetadataCacheInfoStore> metadataCacheInfoStoreFactory,
      ITracerService tracerService)
    {
      this.upstreamMetadataManagerFactory = upstreamMetadataManagerFactory;
      this.metadataCacheInfoStoreFactory = metadataCacheInfoStoreFactory;
      this.tracerService = tracerService;
    }

    public async Task<JobResult> Handle(
      IPackageNameRequest<TPackageName, PushDrivenUpstreamsNotificationTelemetry> request)
    {
      UpstreamMetadataCachePackageJobHandler<TPackageName> sendInTheThisObject = this;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (Handle)))
      {
        try
        {
          UpstreamMetadataCacheInfo metadataCacheInfoAsync = await sendInTheThisObject.metadataCacheInfoStoreFactory.Get((IFeedRequest) request).GetMetadataCacheInfoAsync(request.Feed);
          if (metadataCacheInfoAsync?.PackageNames == null || !metadataCacheInfoAsync.PackageNames.Contains((IPackageName) request.PackageName))
          {
            tracer.TraceInfo("Package '" + request.PackageName.DisplayName + "' not contained in metadata list. No refresh needed.");
            return JobResult.Succeeded((JobTelemetry) new PackageUpstreamRefreshJobTelemetry()
            {
              Result = RefreshPackageResult.RefreshNotNeeded(request.Feed, (IPackageName) request.PackageName),
              NotificationData = request.AdditionalData
            });
          }
          IUpstreamMetadataManager upstreamMetadataManager = await sendInTheThisObject.upstreamMetadataManagerFactory.Get((IFeedRequest) request);
          IEnumerable<Guid> collection = request.Feed.UpstreamSources.Where<UpstreamSource>((Func<UpstreamSource, bool>) (x =>
          {
            Guid? internalUpstreamFeedId = x.InternalUpstreamFeedId;
            Guid? upstreamFeedId = request.AdditionalData.UpstreamFeedId;
            if (internalUpstreamFeedId.HasValue != upstreamFeedId.HasValue)
              return false;
            return !internalUpstreamFeedId.HasValue || internalUpstreamFeedId.GetValueOrDefault() == upstreamFeedId.GetValueOrDefault();
          })).Select<UpstreamSource, Guid>((Func<UpstreamSource, Guid>) (x => x.Id));
          IPackageNameRequest<TPackageName, PushDrivenUpstreamsNotificationTelemetry> packageNameRequest = request;
          // ISSUE: variable of a boxed type
          __Boxed<TPackageName> packageName = (object) request.PackageName;
          HashSet<Guid> upstreamVersionListsToForceRefreshByUpstreamId = new HashSet<Guid>(collection);
          PushDrivenUpstreamsNotificationTelemetry additionalData = request.AdditionalData;
          RefreshPackageResult refreshPackageResult = await upstreamMetadataManager.RefreshPackageAsync((IFeedRequest) packageNameRequest, (IPackageName) packageName, false, (ISet<Guid>) upstreamVersionListsToForceRefreshByUpstreamId, false, additionalData, false);
          return JobResult.Succeeded((JobTelemetry) new PackageUpstreamRefreshJobTelemetry()
          {
            Result = refreshPackageResult,
            NotificationData = request.AdditionalData
          });
        }
        catch (Exception ex)
        {
          JobTelemetry telemetry = new JobTelemetry();
          telemetry.LogException(ex);
          telemetry.Message = string.Format("{0} encountered an error while processing feed {1}, package '{2}'", (object) nameof (UpstreamMetadataCachePackageJobHandler<TPackageName>), (object) request.Feed.Id, (object) request.PackageName);
          return JobResult.Failed(telemetry);
        }
      }
    }
  }
}
