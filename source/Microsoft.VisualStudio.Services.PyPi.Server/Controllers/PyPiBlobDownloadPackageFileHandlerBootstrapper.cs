// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Controllers.PyPiBlobDownloadPackageFileHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using Microsoft.VisualStudio.Services.PyPi.Server.Aggregations.StorageIdCache;
using Microsoft.VisualStudio.Services.PyPi.Server.Converters.Identity;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.Server.Telemetry;
using Microsoft.VisualStudio.Services.PyPi.Server.Upstreams;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Controllers
{
  public class PyPiBlobDownloadPackageFileHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<RawPackageFileRequest, HttpResponseMessage, IPyPiMetadataAggregationAccessor, IPyPiStorageIdCacheAggregationAccessor, IUpstreamVersionListService<PyPiPackageName, PyPiPackageVersion>>
  {
    private readonly IVssRequestContext requestContext;

    public PyPiBlobDownloadPackageFileHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<RawPackageFileRequest, HttpResponseMessage> Bootstrap(
      IPyPiMetadataAggregationAccessor metadataService,
      IPyPiStorageIdCacheAggregationAccessor cacheService,
      IUpstreamVersionListService<PyPiPackageName, PyPiPackageVersion> upstreamVersionListService)
    {
      IValidator<IPackageRequest> validator = new BlockedPackageIdentityRequestToExceptionConverterBootstrapper(this.requestContext).Bootstrap().AsThrowingValidator<IPackageRequest>();
      IConverter<IRawPackageFileRequest, PackageFileRequest<PyPiPackageIdentity>> converter = new RawPackageFileRequestConverter<PyPiPackageIdentity>(new PyPiRawPackageRequestToRequestConverterBootstrapper(this.requestContext).Bootstrap()).ValidateResultWith<IRawPackageFileRequest, PackageFileRequest<PyPiPackageIdentity>, IPackageRequest>(validator);
      IAsyncHandler<IPackageFileRequest<PyPiPackageIdentity>> forwardingToThisHandler = new PyPiDownloadPackageCiDataFacadeHandler(this.requestContext, (ICache<string, object>) new RequestContextItemsAsCacheFacade(this.requestContext)).ThenDelegateTo<IPackageFileRequest<PyPiPackageIdentity>, ICiData>((IAsyncHandler<ICiData>) new TelemetryPublisherUsingTracerFacadeBootstrapper(this.requestContext).Bootstrap());
      IAsyncHandler<IPackageFileRequest<PyPiPackageIdentity, IStorageId>, ContentResult> ingestingHandler = new IngestPyPiFileIfNotAlreadyIngestedBootstrapper(this.requestContext).Bootstrap();
      IAsyncHandler<IPackageFileRequest<PyPiPackageIdentity>, HttpResponseMessage> handler = new DownloadFileRequestHandlerBootstrapper<PyPiPackageIdentity, IPyPiMetadataEntry>(this.requestContext, new PyPiUpstreamFetchingMetadataServiceBootstrapper(this.requestContext, (IReadMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry>) metadataService, upstreamVersionListService).Bootstrap(), (IMetadataCacheService) cacheService, ingestingHandler).Bootstrap().ThenForwardOriginalRequestTo<IPackageFileRequest<PyPiPackageIdentity>, HttpResponseMessage>((IAsyncHandler<IPackageFileRequest<PyPiPackageIdentity>, NullResult>) forwardingToThisHandler);
      return (IAsyncHandler<RawPackageFileRequest, HttpResponseMessage>) ((IConverter<IRawPackageFileRequest, IPackageFileRequest<PyPiPackageIdentity>>) converter).ThenDelegateTo<IRawPackageFileRequest, IPackageFileRequest<PyPiPackageIdentity>, HttpResponseMessage>(handler);
    }
  }
}
