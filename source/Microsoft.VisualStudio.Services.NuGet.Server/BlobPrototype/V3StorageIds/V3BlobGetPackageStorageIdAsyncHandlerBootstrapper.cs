// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.V3StorageIds.V3BlobGetPackageStorageIdAsyncHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.Constants;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using System;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.V3StorageIds
{
  public class V3BlobGetPackageStorageIdAsyncHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<IRawPackageRequest, HttpResponseMessage, INuGetMetadataService, IMetadataCacheService>
  {
    private readonly IVssRequestContext requestContext;

    public V3BlobGetPackageStorageIdAsyncHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<IRawPackageRequest, HttpResponseMessage> Bootstrap(
      INuGetMetadataService metadataAccessor,
      IMetadataCacheService cacheService)
    {
      NuGetRawPackageRequestConverter converter = new NuGetRawPackageRequestConverter(new NuGetRawPackageRequestToIdentityConverterBootstrapper(this.requestContext).Bootstrap());
      ThrowIfNotFoundOrDeletedDelegatingMetadataStore metadataService = new ThrowIfNotFoundOrDeletedDelegatingMetadataStore((IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry>) metadataAccessor);
      RequestContextItemsAsCacheFacade requestContextItems = new RequestContextItemsAsCacheFacade(this.requestContext);
      IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>> requestToPackageStorageIdHandler = PackageRequestToStorageIdAsyncHandler.Create(cacheService, (IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry>) metadataService, (ICache<string, object>) requestContextItems);
      IAsyncHandler<IPackageFileRequest<IPackageIdentity, BlobStorageId>, Uri> storageIdToUriHandler = new DownloadBlobPackageFileAsUriHandlerBootstrapper(this.requestContext).Bootstrap();
      IAsyncHandler<IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>, NuGetPackageContentStorageInfo> storageIdToPackageStorageInfoHandler = DowncastingAsyncHandler<IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>, NuGetPackageContentStorageInfo>.CreateBuilder().WhenTypeIs<IPackageFileRequest<VssNuGetPackageIdentity, BlobStorageId>>().Use((IAsyncHandler<IPackageFileRequest<VssNuGetPackageIdentity, BlobStorageId>, NuGetPackageContentStorageInfo>) new NuGetBlobStorageIdToInfoAsyncHandler((IAsyncHandler<IPackageFileRequest<VssNuGetPackageIdentity, BlobStorageId>, Uri>) storageIdToUriHandler)).WhenTypeIs<IPackageFileRequest<VssNuGetPackageIdentity, DropStorageId>>().Use((IAsyncHandler<IPackageFileRequest<VssNuGetPackageIdentity, DropStorageId>, NuGetPackageContentStorageInfo>) new NuGetDropStorageIdToInfoAsyncHandler()).Build();
      NuGetPackageStorageIdResponseAsyncHandler currentHandler = new NuGetPackageStorageIdResponseAsyncHandler(requestToPackageStorageIdHandler, storageIdToPackageStorageInfoHandler);
      IAsyncHandler<(PackageRequest<VssNuGetPackageIdentity>, NuGetStorageInfo)> forwardingToThisHandler = new GetStorageInfoApiDownloadCiDataFacadeHandler(this.requestContext, (ICache<string, object>) requestContextItems).ThenDelegateTo<(PackageRequest<VssNuGetPackageIdentity>, NuGetStorageInfo), ICiData>((IAsyncHandler<ICiData>) new TelemetryBootstrapper(this.requestContext, NuGetTracePoints.Telemetry.TraceInfo).Bootstrap());
      JsonSerializingHttpResponseHandler<NuGetStorageInfo> handler1 = new JsonSerializingHttpResponseHandler<NuGetStorageInfo>();
      IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, HttpResponseMessage> handler2 = currentHandler.ThenForwardOriginalRequestAndResultTo<PackageRequest<VssNuGetPackageIdentity>, NuGetStorageInfo>(forwardingToThisHandler).ThenDelegateTo<PackageRequest<VssNuGetPackageIdentity>, NuGetStorageInfo, HttpResponseMessage>((IAsyncHandler<NuGetStorageInfo, HttpResponseMessage>) handler1);
      return converter.ThenDelegateTo<IRawPackageRequest, PackageRequest<VssNuGetPackageIdentity>, HttpResponseMessage>(handler2);
    }
  }
}
