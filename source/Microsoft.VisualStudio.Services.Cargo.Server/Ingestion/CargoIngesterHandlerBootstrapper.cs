// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Ingestion.CargoIngesterHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.CommitLog;
using Microsoft.VisualStudio.Services.Cargo.Server.JobManagement;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Cargo.Server.Upstreams;
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
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Ingestion
{
  public class CargoIngesterHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<PackageIngestionRequest<CargoPackageIdentity, CargoIngestionInput>, IAddOperationData, IMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>, IUpstreamVersionListService<CargoPackageName, CargoPackageVersion>>
  {
    private readonly IVssRequestContext requestContext;

    public CargoIngesterHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<PackageIngestionRequest<CargoPackageIdentity, CargoIngestionInput>, IAddOperationData> Bootstrap(
      IMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry> metadataAccessor,
      IUpstreamVersionListService<CargoPackageName, CargoPackageVersion> upstreamVersionListService)
    {
      IContentBlobStore blobStore = new ContentBlobStoreFacadeBootstrapper(this.requestContext).Bootstrap();
      IReadMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry> metadataDocumentService = new CargoUpstreamFetchingMetadataServiceBootstrapper(this.requestContext, (IReadMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>) metadataAccessor, upstreamVersionListService).Bootstrap();
      PackageIngester2<CargoPackageIdentity, SimplePackageFileName, ICargoMetadataEntry, CargoIngestionInput, CargoParsedIngestionParams> ingester = new PackageIngester2<CargoPackageIdentity, SimplePackageFileName, ICargoMetadataEntry, CargoIngestionInput, CargoParsedIngestionParams>(FeedValidator.GetFeedIsNotReadOnlyValidator(), (IValidator<IProtocol>) new ProtocolWritableValidator(this.requestContext.GetFeatureFlagFacade()), (IFeedPerms) new FeedPermsFacade(this.requestContext), (IProtocolPackageIngestion<CargoIngestionInput, CargoParsedIngestionParams, ICargoMetadataEntry>) new CargoPackageIngester(PackageSizeValidatingHandler.MaxSizeRegistrySettingDefinition.Bootstrap(this.requestContext), CargoSettings.MaxIngestionManifestLengthSetting.Bootstrap(this.requestContext), CargoSettings.AllowNonAsciiNames.Bootstrap(this.requestContext), blobStore, CargoSettings.EnforceConsistentIdentity.Bootstrap(this.requestContext)), new TerrapinIngestionValidatorBootstrapper(this.requestContext, (ICommitLogWriter<ICommitLogEntry>) new CargoCommitLogFacadeBootstrapper(this.requestContext).Bootstrap(), new CargoChangeProcessingJobQueuerBootstrapper(this.requestContext).Bootstrap(), (IIdentityResolver) CargoIdentityResolver.Instance).Bootstrap(), metadataDocumentService, PackageIngestionHelper.MaxVersionsPerPackageSettingDefinition.Bootstrap(this.requestContext), (IPackagingTracesBasicInfo) new PackagingTracesBasicInfo(this.requestContext), (IProvenanceInfoProvider) new ProvenanceInfoProviderBootstrapper(this.requestContext).Bootstrap(), new BlockedPackageIdentityRequestToExceptionConverterBootstrapper(this.requestContext, BlockedIdentityContext.Upload).Bootstrap().AsThrowingValidator<IPackageRequest>());
      return ByAsyncFuncAsyncHandler.For<PackageIngestionRequest<CargoPackageIdentity, CargoIngestionInput>, IAddOperationData>((Func<PackageIngestionRequest<CargoPackageIdentity, CargoIngestionInput>, Task<IAddOperationData>>) (async request => await ingester.IngestPackageAsync((IFeedRequest) request, request.SourceChain ?? (IEnumerable<UpstreamSourceInfo>) ImmutableList<UpstreamSourceInfo>.Empty, (IPackageIdentity) request.ExpectedIdentity, request.PackageContents)));
    }
  }
}
