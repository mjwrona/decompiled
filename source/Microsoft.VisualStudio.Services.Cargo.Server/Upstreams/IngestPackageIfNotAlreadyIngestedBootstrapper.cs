// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Upstreams.IngestPackageIfNotAlreadyIngestedBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.Ingestion;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Upstreams
{
  public class IngestPackageIfNotAlreadyIngestedBootstrapper : 
    IBootstrapper<IAsyncHandler<IPackageFileRequest<CargoPackageIdentity, IStorageId>, ContentResult?>>
  {
    private readonly IVssRequestContext requestContext;

    public IngestPackageIfNotAlreadyIngestedBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<IPackageFileRequest<CargoPackageIdentity, IStorageId>, ContentResult?> Bootstrap()
    {
      IFeatureFlagService featureFlagFacade = this.requestContext.GetFeatureFlagFacade();
      ITracerService tracerFacade = this.requestContext.GetTracerFacade();
      IFactory<UpstreamSource, Task<IUpstreamCargoClient>> clientFactory = new UpstreamCargoClientFactoryBootstrapper(this.requestContext).Bootstrap();
      RequestContextItemsAsCacheFacade itemsAsCacheFacade = new RequestContextItemsAsCacheFacade(this.requestContext);
      IAsyncHandler<PackageIngestionRequest<CargoPackageIdentity, CargoIngestionInput>, NullResult> ingestionHandler = new CargoPackageIngesterBootstrapper(this.requestContext).Bootstrap();
      ITracerService tracer = tracerFacade;
      TemporarilyStoreStreamAsFileHandler temporarilyStorePackageHandler = new TemporarilyStoreStreamAsFileHandler();
      DisposableRegistrar disposableRegistrar = new DisposableRegistrar(this.requestContext);
      UpstreamsFromFeedHandler upstreamsFromFeedHandler = new UpstreamsFromFeedHandler(featureFlagFacade);
      ValueFromCacheFactory<bool> shouldBlockWriteOperationsFactory = new ValueFromCacheFactory<bool>("Packaging.BlockWriteOperationOnGetRequest", (ICache<string, object>) itemsAsCacheFacade);
      RequestContextItemsAsCacheFacade requestContextItems = itemsAsCacheFacade;
      IFeedViewVisibilityValidator upstreamValidator = FeedViewVisibilityValidatingHandler.Bootstrap(this.requestContext);
      return (IAsyncHandler<IPackageFileRequest<CargoPackageIdentity, IStorageId>, ContentResult>) new DownloadAndIngestCrateFileFromUpstreamHandler(clientFactory, ingestionHandler, tracer, (IAsyncHandler<FeedRequest<Stream>, FileStream>) temporarilyStorePackageHandler, (IDisposableRegistrar) disposableRegistrar, (IHandler<IFeedRequest, IEnumerable<UpstreamSource>>) upstreamsFromFeedHandler, (IFactory<bool>) shouldBlockWriteOperationsFactory, (ICache<string, object>) requestContextItems, upstreamValidator);
    }
  }
}
