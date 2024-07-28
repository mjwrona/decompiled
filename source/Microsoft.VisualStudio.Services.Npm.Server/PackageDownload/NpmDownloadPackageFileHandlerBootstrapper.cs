// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.PackageDownload.NpmDownloadPackageFileHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Parsing;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.Telemetry;
using Microsoft.VisualStudio.Services.Npm.Server.Upstreams;
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
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Npm.Server.PackageDownload
{
  public class NpmDownloadPackageFileHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<RawNpmPackageNameWithFileRequest, HttpResponseMessage, INpmMetadataService, IMetadataCacheService, IUpstreamVersionListService<NpmPackageName, SemanticVersion>>
  {
    private readonly IVssRequestContext requestContext;

    public NpmDownloadPackageFileHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<RawNpmPackageNameWithFileRequest, HttpResponseMessage> Bootstrap(
      INpmMetadataService metadataService,
      IMetadataCacheService cacheService,
      IUpstreamVersionListService<NpmPackageName, SemanticVersion> upstreamVersionListService)
    {
      RawNpmPackageNameWithFileRequestConverter requestConverter = new RawNpmPackageNameWithFileRequestConverter();
      IConverter<RawNpmPackageNameWithFileRequest, (RawNpmPackageNameWithFileRequest In, RawPackageRequest Out)> keepInputConverter = requestConverter.KeepInput<RawNpmPackageNameWithFileRequest, RawPackageRequest>();
      IConverter<RawNpmPackageNameWithFileRequest, RawPackageFileRequest> converter1 = ConvertFrom.InputTypeOf<RawNpmPackageNameWithFileRequest>((IHaveInputType<RawNpmPackageNameWithFileRequest>) requestConverter).By<RawNpmPackageNameWithFileRequest, RawPackageFileRequest>((Func<RawNpmPackageNameWithFileRequest, RawPackageFileRequest>) (r =>
      {
        (RawNpmPackageNameWithFileRequest In, RawPackageRequest Out) tuple = keepInputConverter.Convert(r);
        return new RawPackageFileRequest((IFeedRequest) r, RawNpmPackageName.Create(tuple.In.PackageScope, tuple.In.UnscopedPackageName), tuple.Out.PackageVersion, tuple.In.FileName);
      }));
      IValidator<IPackageRequest> validator = new BlockedPackageIdentityRequestToExceptionConverterBootstrapper(this.requestContext).Bootstrap().AsThrowingValidator<IPackageRequest>();
      IConverter<IRawPackageFileRequest, PackageFileRequest<NpmPackageIdentity>> converter2 = new RawPackageFileRequestConverter<NpmPackageIdentity>(new NpmRawPackageRequestToRequestConverterBootstrapper(this.requestContext).Bootstrap()).ValidateResultWith<IRawPackageFileRequest, PackageFileRequest<NpmPackageIdentity>, IPackageRequest>(validator);
      IAsyncHandler<IPackageFileRequest<NpmPackageIdentity>> forwardingToThisHandler = new NpmDownloadPackageCiDataFacadeHandler(this.requestContext, (ICache<string, object>) new RequestContextItemsAsCacheFacade(this.requestContext)).ThenDelegateTo<IPackageFileRequest<NpmPackageIdentity>, ICiData>((IAsyncHandler<ICiData>) new TelemetryPublisherUsingTracerFacadeBootstrapper(this.requestContext).Bootstrap());
      IAsyncHandler<IPackageFileRequest<NpmPackageIdentity, IStorageId>, ContentResult> ingestingHandler = IngestPackageIfNotAlreadyIngestedBootstrapper.Create(this.requestContext).Bootstrap();
      IAsyncHandler<IPackageFileRequest<NpmPackageIdentity>, HttpResponseMessage> currentHandler = new DownloadFileRequestHandlerBootstrapper<NpmPackageIdentity, INpmMetadataEntry>(this.requestContext, new NpmUpstreamFetchingMetadataServiceBootstrapper(this.requestContext, (IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>) metadataService, upstreamVersionListService).Bootstrap(), cacheService, ingestingHandler).Bootstrap();
      IConverter<IRawPackageFileRequest, PackageFileRequest<NpmPackageIdentity>> converter2_1 = converter2;
      return ((IConverter<RawNpmPackageNameWithFileRequest, IRawPackageFileRequest>) converter1).ThenDelegateTo<RawNpmPackageNameWithFileRequest, IRawPackageFileRequest, PackageFileRequest<NpmPackageIdentity>>(converter2_1).ThenDelegateTo<RawNpmPackageNameWithFileRequest, PackageFileRequest<NpmPackageIdentity>, HttpResponseMessage>((IAsyncHandler<PackageFileRequest<NpmPackageIdentity>, HttpResponseMessage>) currentHandler.ThenForwardOriginalRequestTo<IPackageFileRequest<NpmPackageIdentity>, HttpResponseMessage>((IAsyncHandler<IPackageFileRequest<NpmPackageIdentity>, NullResult>) forwardingToThisHandler));
    }
  }
}
