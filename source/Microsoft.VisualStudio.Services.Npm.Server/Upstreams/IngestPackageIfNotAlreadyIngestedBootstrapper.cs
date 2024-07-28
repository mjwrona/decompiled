// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Upstreams.IngestPackageIfNotAlreadyIngestedBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Constants;
using Microsoft.VisualStudio.Services.Npm.Server.Registry;
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
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.Upstreams
{
  public class IngestPackageIfNotAlreadyIngestedBootstrapper : 
    RequireAggHandlerBootstrapper<IPackageFileRequest<NpmPackageIdentity, IStorageId>, ContentResult, INpmMetadataService, IMetadataCacheService>
  {
    private readonly IVssRequestContext requestContext;

    public IngestPackageIfNotAlreadyIngestedBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public static IBootstrapper<IAsyncHandler<IPackageFileRequest<NpmPackageIdentity, IStorageId>, ContentResult>> Create(
      IVssRequestContext requestContext)
    {
      return ExistingInstanceBootstrapper.Create<IAsyncHandler<IPackageFileRequest<NpmPackageIdentity, IStorageId>, ContentResult>>(NpmAggregationResolver.Bootstrap(requestContext).HandlerFor<IPackageFileRequest<NpmPackageIdentity, IStorageId>, ContentResult>((IRequireAggBootstrapper<IAsyncHandler<IPackageFileRequest<NpmPackageIdentity, IStorageId>, ContentResult>>) new IngestPackageIfNotAlreadyIngestedBootstrapper(requestContext)));
    }

    protected override IAsyncHandler<IPackageFileRequest<NpmPackageIdentity, IStorageId>, ContentResult> Bootstrap(
      INpmMetadataService metadataService,
      IMetadataCacheService metadataCacheService)
    {
      IFeatureFlagService featureFlagFacade = this.requestContext.GetFeatureFlagFacade();
      ITracerService tracerFacade = this.requestContext.GetTracerFacade();
      IFactory<UpstreamSource, Task<IUpstreamNpmClient>> upstreamClientFactory = new UpstreamNpmClientFactoryBootstrapper(this.requestContext).Bootstrap();
      RequestContextItemsAsCacheFacade itemsAsCacheFacade = new RequestContextItemsAsCacheFacade(this.requestContext);
      NpmPackageIngestionServiceFacade npmIngestionService = new NpmPackageIngestionServiceFacade(this.requestContext);
      ITracerService tracerService = tracerFacade;
      UpstreamsFromFeedHandler upstreamsFromFeedHandler = new UpstreamsFromFeedHandler(featureFlagFacade);
      RequestContextItemsAsCacheFacade requestContextItems = itemsAsCacheFacade;
      IFeedViewVisibilityValidator upstreamValidator = FeedViewVisibilityValidatingHandler.Bootstrap(this.requestContext);
      IOrgLevelPackagingSetting<bool> propagateDeprecateSetting = FeatureFlagConstants.PropagateDeprecateFromUpstream.Bootstrap(this.requestContext);
      return (IAsyncHandler<IPackageFileRequest<NpmPackageIdentity, IStorageId>, ContentResult>) new DownloadAndIngestTarGzFromUpstreamHandler(upstreamClientFactory, (INpmIngestionService) npmIngestionService, tracerService, (IHandler<IFeedRequest, IEnumerable<UpstreamSource>>) upstreamsFromFeedHandler, (ICache<string, object>) requestContextItems, upstreamValidator, propagateDeprecateSetting);
    }
  }
}
