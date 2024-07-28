// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.IngestPackageIfNotAlreadyIngestedBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.Constants;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.PublicClient;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class IngestPackageIfNotAlreadyIngestedBootstrapper : 
    RequireAggHandlerBootstrapper<IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>, ContentResult, INuGetMetadataService, IMetadataCacheService>
  {
    private readonly IVssRequestContext requestContext;

    public IngestPackageIfNotAlreadyIngestedBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public static IBootstrapper<IAsyncHandler<IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>, ContentResult>> Create(
      IVssRequestContext requestContext)
    {
      return ExistingInstanceBootstrapper.Create<IAsyncHandler<IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>, ContentResult>>(NuGetAggregationResolver.Bootstrap(requestContext).HandlerFor<IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>, ContentResult>((IRequireAggBootstrapper<IAsyncHandler<IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>, ContentResult>>) new IngestPackageIfNotAlreadyIngestedBootstrapper(requestContext)));
    }

    protected override IAsyncHandler<IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>, ContentResult> Bootstrap(
      INuGetMetadataService metadataService,
      IMetadataCacheService metadataCacheService)
    {
      IFeatureFlagService featureFlagFacade = this.requestContext.GetFeatureFlagFacade();
      ITracerService tracerFacade = this.requestContext.GetTracerFacade();
      IFactory<UpstreamSource, Task<IUpstreamNuGetClient>> nuGetClientFactory = new UpstreamNuGetClientFactoryBootstrapper(this.requestContext).Bootstrap();
      RequestContextItemsAsCacheFacade itemsAsCacheFacade = new RequestContextItemsAsCacheFacade(this.requestContext);
      NuGetPackageIngestionServiceFacade nuGetPackageIngestionService = new NuGetPackageIngestionServiceFacade((IFactory<IPackageIngestionService>) new NuGetPackageIngestionServiceFactory((IFactory<bool>) new ValueFromCacheFactory<bool>("Packaging.BlockWriteOperationOnGetRequest", (ICache<string, object>) itemsAsCacheFacade), featureFlagFacade, "NuGet.Service.SkipIngestionOnCsrfValidationFailure"), this.requestContext);
      ByBlobIdStreamingHandler byBlobIdStreamingHandler = new ByBlobIdStreamingHandler(new ContentBlobStoreFacadeBootstrapper(this.requestContext).Bootstrap());
      ITracerService tracer = tracerFacade;
      TemporarilyStoreStreamAsFileHandler temporarilyStorePackageHandler = new TemporarilyStoreStreamAsFileHandler();
      DisposableRegistrar disposableRegistrar = new DisposableRegistrar(this.requestContext);
      UpstreamsFromFeedHandler upstreamsFromFeedHandler = new UpstreamsFromFeedHandler(featureFlagFacade);
      RequestContextItemsAsCacheFacade requestContextItems = itemsAsCacheFacade;
      IFeedViewVisibilityValidator upstreamValidator = FeedViewVisibilityValidatingHandler.Bootstrap(this.requestContext);
      IOrgLevelPackagingSetting<bool> propagateDelistSetting = FeatureEnabledConstants.PropagateDelistFromUpstream.Bootstrap(this.requestContext);
      return (IAsyncHandler<IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>, ContentResult>) new DownloadAndIngestNupkgFromUpstreamHandler(nuGetClientFactory, (INuGetPackageIngestionService) nuGetPackageIngestionService, (IAsyncHandler<PackageRequest<VssNuGetPackageIdentity, IStorageId>, Stream>) byBlobIdStreamingHandler, tracer, (IAsyncHandler<FeedRequest<Stream>, FileStream>) temporarilyStorePackageHandler, (IDisposableRegistrar) disposableRegistrar, (IHandler<IFeedRequest, IEnumerable<UpstreamSource>>) upstreamsFromFeedHandler, (ICache<string, object>) requestContextItems, upstreamValidator, propagateDelistSetting);
    }
  }
}
