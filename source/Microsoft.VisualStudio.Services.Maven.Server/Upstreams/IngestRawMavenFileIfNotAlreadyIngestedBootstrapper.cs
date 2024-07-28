// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Upstreams.IngestRawMavenFileIfNotAlreadyIngestedBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Ingestion;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
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

namespace Microsoft.VisualStudio.Services.Maven.Server.Upstreams
{
  public class IngestRawMavenFileIfNotAlreadyIngestedBootstrapper : 
    IBootstrapper<IAsyncHandler<IPackageFileRequest<MavenPackageIdentity, IStorageId>, ContentResult>>
  {
    private readonly IVssRequestContext requestContext;

    public IngestRawMavenFileIfNotAlreadyIngestedBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<IPackageFileRequest<MavenPackageIdentity, IStorageId>, ContentResult> Bootstrap()
    {
      IFeatureFlagService featureFlagFacade = this.requestContext.GetFeatureFlagFacade();
      ITracerService tracerFacade = this.requestContext.GetTracerFacade();
      IFactory<UpstreamSource, Task<IUpstreamMavenClient>> clientFactory = new UpstreamMavenClientFactoryBootstrapper(this.requestContext).Bootstrap();
      RequestContextItemsAsCacheFacade itemsAsCacheFacade = new RequestContextItemsAsCacheFacade(this.requestContext);
      IAsyncHandler<PackageIngestionRequest<MavenPackageIdentity, MavenPackageFileInfo>> ingestionHandler = new MavenPackageIngesterBootstrapper(this.requestContext).Bootstrap();
      ITracerService tracer = tracerFacade;
      TemporarilyStoreStreamAsFileHandler temporarilyStorePackageHandler = new TemporarilyStoreStreamAsFileHandler();
      DisposableRegistrar disposableRegistrar = new DisposableRegistrar(this.requestContext);
      UpstreamsFromFeedHandler upstreamsFromFeedHandler = new UpstreamsFromFeedHandler(featureFlagFacade);
      ValueFromCacheFactory<bool> shouldBlockWriteOperationsFactory = new ValueFromCacheFactory<bool>("Packaging.BlockWriteOperationOnGetRequest", (ICache<string, object>) itemsAsCacheFacade);
      RequestContextItemsAsCacheFacade requestContextItems = itemsAsCacheFacade;
      IFeedViewVisibilityValidator upstreamValidator = FeedViewVisibilityValidatingHandler.Bootstrap(this.requestContext);
      return (IAsyncHandler<IPackageFileRequest<MavenPackageIdentity, IStorageId>, ContentResult>) new DownloadAndIngestMavenFileFromUpstreamHandler(clientFactory, ingestionHandler, tracer, (IAsyncHandler<FeedRequest<Stream>, FileStream>) temporarilyStorePackageHandler, (IDisposableRegistrar) disposableRegistrar, (IHandler<IFeedRequest, IEnumerable<UpstreamSource>>) upstreamsFromFeedHandler, (IFactory<bool>) shouldBlockWriteOperationsFactory, (ICache<string, object>) requestContextItems, upstreamValidator);
    }
  }
}
