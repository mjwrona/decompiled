// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.PackagingApi.PackageVersionDownloadPackageMetadataHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
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
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.PackagingApi
{
  public class PackageVersionDownloadPackageMetadataHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<RawPackageFileRequest<NuGetGetFileData>, HttpResponseMessage, INuGetMetadataService, IMetadataCacheService, IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion>>
  {
    private readonly IVssRequestContext requestContext;

    public PackageVersionDownloadPackageMetadataHandlerBootstrapper(
      IVssRequestContext requestContext)
    {
      this.requestContext = requestContext;
    }

    protected override IAsyncHandler<RawPackageFileRequest<NuGetGetFileData>, HttpResponseMessage> Bootstrap(
      INuGetMetadataService metadataAccessor,
      IMetadataCacheService metadataCacheService,
      IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion> upstreamVersionListService)
    {
      IValidator<IPackageIdentity> validator = new BlockedPackageIdentityToExceptionConverterBootstrapper(this.requestContext).Bootstrap().AsThrowingValidator<IPackageIdentity>();
      RawPackageFileRequestConverter<VssNuGetPackageIdentity, NuGetGetFileData> requestConverter = new RawPackageFileRequestConverter<VssNuGetPackageIdentity, NuGetGetFileData>(new NuGetRawPackageRequestToIdentityConverterBootstrapper(this.requestContext).Bootstrap().ValidateResultWith<IRawPackageRequest, VssNuGetPackageIdentity, IPackageIdentity>(validator));
      IAsyncHandler<IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>, ContentResult> ingestingHandler = IngestPackageIfNotAlreadyIngestedBootstrapper.Create(this.requestContext).Bootstrap();
      IAsyncHandler<IPackageFileRequest<VssNuGetPackageIdentity, NuGetGetFileData>, HttpResponseMessage> handler = ((IAsyncHandler<IPackageFileRequest<VssNuGetPackageIdentity, NuGetGetFileData>, HttpResponseMessage>) new DownloadFileRequestHandlerBootstrapper<VssNuGetPackageIdentity, INuGetMetadataEntry>(this.requestContext, new NuGetUpstreamFetchingMetadataServiceBootstrapper(this.requestContext, (IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>) metadataAccessor, upstreamVersionListService, true).Bootstrap(), metadataCacheService, ingestingHandler).Bootstrap()).ThenForwardOriginalRequestTo<IPackageFileRequest<VssNuGetPackageIdentity, NuGetGetFileData>, HttpResponseMessage>((IAsyncHandler<IPackageFileRequest<VssNuGetPackageIdentity, NuGetGetFileData>, NullResult>) new GetNuGetDownloadCiDataFacadeHandler(this.requestContext, (ICache<string, object>) new RequestContextItemsAsCacheFacade(this.requestContext)).ThenDelegateTo<IPackageFileRequest<VssNuGetPackageIdentity, NuGetGetFileData>, ICiData>((IAsyncHandler<ICiData>) new TelemetryPublisherUsingTracerFacadeBootstrapper(this.requestContext).Bootstrap()));
      return (IAsyncHandler<RawPackageFileRequest<NuGetGetFileData>, HttpResponseMessage>) ((IConverter<IRawPackageFileRequest<NuGetGetFileData>, IPackageFileRequest<VssNuGetPackageIdentity, NuGetGetFileData>>) requestConverter).ThenDelegateTo<IRawPackageFileRequest<NuGetGetFileData>, IPackageFileRequest<VssNuGetPackageIdentity, NuGetGetFileData>, HttpResponseMessage>(handler);
    }
  }
}
