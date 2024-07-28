// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Ingestion2.PyPiIngester2HandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.PyPi.Server.CommitLog;
using Microsoft.VisualStudio.Services.PyPi.Server.Constants;
using Microsoft.VisualStudio.Services.PyPi.Server.Ingestion;
using Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation;
using Microsoft.VisualStudio.Services.PyPi.Server.JobManagement;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.Server.Upstreams;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.Ingestion2
{
  public class PyPiIngester2HandlerBootstrapper : 
    RequireAggHandlerBootstrapper<
    #nullable disable
    PackageIngestionRequest<PyPiPackageIdentity, PackageIngestionFormData>, IAddOperationData, IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry>, IUpstreamVersionListService<PyPiPackageName, PyPiPackageVersion>>
  {
    private readonly IVssRequestContext requestContext;

    public PyPiIngester2HandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<PackageIngestionRequest<PyPiPackageIdentity, PackageIngestionFormData>, IAddOperationData> Bootstrap(
      IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry> metadataAccessor,
      IUpstreamVersionListService<PyPiPackageName, PyPiPackageVersion> upstreamVersionListService)
    {
      IReadMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry> metadataDocumentService = new PyPiUpstreamFetchingMetadataServiceBootstrapper(this.requestContext, (IReadMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry>) metadataAccessor, upstreamVersionListService).Bootstrap();
      IRegistryWriterService registryFacade = this.requestContext.GetRegistryFacade();
      IContentBlobStore blobStore = new ContentBlobStoreFacadeBootstrapper(this.requestContext).Bootstrap();
      FromFilePathRefCalculatingConverter<PyPiPackageIdentity> refCalculatingConverter = new FromFilePathRefCalculatingConverter<PyPiPackageIdentity>();
      PackageIngester2<PyPiPackageIdentity, SimplePackageFileName, IPyPiMetadataEntry, PackageIngestionFormData, PyPiParsedIngestionParams> ingester = new PackageIngester2<PyPiPackageIdentity, SimplePackageFileName, IPyPiMetadataEntry, PackageIngestionFormData, PyPiParsedIngestionParams>(FeedValidator.GetFeedIsNotReadOnlyValidator(), (IValidator<IProtocol>) new ProtocolWritableValidator(this.requestContext.GetFeatureFlagFacade()), (IFeedPerms) new FeedPermsFacade(this.requestContext), (IProtocolPackageIngestion<PackageIngestionFormData, PyPiParsedIngestionParams, IPyPiMetadataEntry>) new PyPiPackageIngester2(new PyPiMetadataIngestionValidatingHandlerBootstrapper(this.requestContext).Bootstrap(), (IAsyncHandler<(IPyPiStorablePackageInfo, IPyPiMetadataEntry)>) new OnlyOneSourceDistValidatingHandler(), (IAsyncHandler<long>) new PackageSizeValidatingHandler((IRegistryService) registryFacade, Protocol.PyPi.CorrectlyCasedName), (IValidator<long>) new GpgSignatureLengthValidator((IRegistryService) registryFacade), blobStore, (IConverter<IPackageFileRequest<PyPiPackageIdentity>, string>) refCalculatingConverter, PackagingHttpClient.ForProtocol(this.requestContext, (IProtocol) Protocol.PyPi), PyPiFeatureFlags.SuppressHashingInBlobFirstIngestFlows.Bootstrap(this.requestContext), PyPiFeatureFlags.TrustClientProvidedHashInBlobFirstIngestFlows.Bootstrap(this.requestContext)), new TerrapinIngestionValidatorBootstrapper(this.requestContext, (ICommitLogWriter<ICommitLogEntry>) new PyPiCommitLogFacadeBootstrapper(this.requestContext).Bootstrap(), new PyPiChangeProcessingJobQueuerBootstrapper(this.requestContext).Bootstrap(), (IIdentityResolver) PyPiIdentityResolver.Instance).Bootstrap(), metadataDocumentService, PackageIngestionHelper.MaxVersionsPerPackageSettingDefinition.Bootstrap(this.requestContext), (IPackagingTracesBasicInfo) new PackagingTracesBasicInfo(this.requestContext), (IProvenanceInfoProvider) new ProvenanceInfoProviderBootstrapper(this.requestContext).Bootstrap(), new BlockedPackageIdentityRequestToExceptionConverterBootstrapper(this.requestContext, BlockedIdentityContext.Upload).Bootstrap().AsThrowingValidator<IPackageRequest>());
      return ByAsyncFuncAsyncHandler.For<PackageIngestionRequest<PyPiPackageIdentity, PackageIngestionFormData>, IAddOperationData>((Func<PackageIngestionRequest<PyPiPackageIdentity, PackageIngestionFormData>, Task<IAddOperationData>>) (async request => await ingester.IngestPackageAsync((IFeedRequest) request, request.SourceChain ?? (IEnumerable<UpstreamSourceInfo>) ImmutableList<UpstreamSourceInfo>.Empty, (IPackageIdentity) request.ExpectedIdentity, request.PackageContents)));
    }
  }
}
