// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.V3BlobGetFileAsyncHandlerBootstrapper
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
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class V3BlobGetFileAsyncHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<IPackageInnerFileRequest<VssNuGetPackageIdentity, NuGetGetFileData>, HttpResponseMessage, INuGetMetadataService, IMetadataCacheService, IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion>>
  {
    private readonly IVssRequestContext requestContext;
    private static readonly IAsyncHandler<IPackageInnerFileRequest<VssNuGetPackageIdentity>, NullResult> NoExtractValidatingHandler = (IAsyncHandler<IPackageInnerFileRequest<VssNuGetPackageIdentity>, NullResult>) new ByFuncAsyncHandler<IPackageInnerFileRequest<VssNuGetPackageIdentity>, NullResult>((Func<IPackageInnerFileRequest<VssNuGetPackageIdentity>, NullResult>) (request =>
    {
      if (!string.IsNullOrWhiteSpace(request.InnerFilePath))
        throw ExceptionHelper.PackageInnerFileNotFound((IPackageIdentity) request.PackageId, request.FilePath, request.InnerFilePath);
      return (NullResult) null;
    }));

    public V3BlobGetFileAsyncHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<IPackageInnerFileRequest<VssNuGetPackageIdentity, NuGetGetFileData>, HttpResponseMessage> Bootstrap(
      INuGetMetadataService metadataService,
      IMetadataCacheService metadataCacheService,
      IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion> upstreamVersionListService)
    {
      return UntilNonNullHandler.Create<IPackageInnerFileRequest<VssNuGetPackageIdentity, NuGetGetFileData>, HttpResponseMessage>((IAsyncHandler<IPackageInnerFileRequest<VssNuGetPackageIdentity, NuGetGetFileData>, HttpResponseMessage>) this.BuildNuspecHandler((IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>) metadataService, upstreamVersionListService), this.BuildNupkgHandler(metadataService, metadataCacheService, upstreamVersionListService)).ThenForwardOriginalRequestTo<IPackageInnerFileRequest<VssNuGetPackageIdentity, NuGetGetFileData>, HttpResponseMessage>((IAsyncHandler<IPackageInnerFileRequest<VssNuGetPackageIdentity, NuGetGetFileData>, NullResult>) new GetNuGetDownloadCiDataFacadeHandler(this.requestContext, (ICache<string, object>) new RequestContextItemsAsCacheFacade(this.requestContext)).ThenDelegateTo<IPackageFileRequest<VssNuGetPackageIdentity, NuGetGetFileData>, ICiData>((IAsyncHandler<ICiData>) new TelemetryPublisherUsingTracerFacadeBootstrapper(this.requestContext).Bootstrap()));
    }

    private IAsyncHandler<IPackageInnerFileRequest<VssNuGetPackageIdentity>, HttpResponseMessage> BuildNuspecHandler(
      IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry> metadataService,
      IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion> upstreamVersionListService)
    {
      NuSpecStreamingHandler handler = new NuSpecStreamingHandler((IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry>) new ThrowIfNotFoundOrDeletedDelegatingMetadataStore((IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry>) new NuGetUpstreamFetchingMetadataServiceBootstrapper(this.requestContext, metadataService, upstreamVersionListService, true).Bootstrap()), (ICache<string, object>) new RequestContextItemsAsCacheFacade(this.requestContext));
      return V3BlobGetFileAsyncHandlerBootstrapper.NoExtractValidatingHandler.ThenActuallyHandleWith<IPackageInnerFileRequest<VssNuGetPackageIdentity>, NullResult, HttpResponseMessage>((IAsyncHandler<IPackageInnerFileRequest<VssNuGetPackageIdentity>, HttpResponseMessage>) handler).SkipAndReturnNullIf<IPackageInnerFileRequest<VssNuGetPackageIdentity>, HttpResponseMessage>((Func<IPackageInnerFileRequest<VssNuGetPackageIdentity>, bool>) (request => !request.FilePath.EndsWith(".nuspec")));
    }

    private IAsyncHandler<IPackageInnerFileRequest<VssNuGetPackageIdentity, NuGetGetFileData>, HttpResponseMessage> BuildNupkgHandler(
      INuGetMetadataService metadataService,
      IMetadataCacheService metadataCacheService,
      IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion> upstreamVersionListService)
    {
      IAsyncHandler<IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>, ContentResult> ingestingHandler = IngestPackageIfNotAlreadyIngestedBootstrapper.Create(this.requestContext).Bootstrap();
      return UntilNonNullHandler.Create<IPackageInnerFileRequest<VssNuGetPackageIdentity, NuGetGetFileData>, HttpResponseMessage>((IAsyncHandler<IPackageInnerFileRequest<VssNuGetPackageIdentity, NuGetGetFileData>, HttpResponseMessage>) new NupkgNameValidator(), new DownloadFileRequestHandlerBootstrapper<VssNuGetPackageIdentity, INuGetMetadataEntry>(this.requestContext, new NuGetUpstreamFetchingMetadataServiceBootstrapper(this.requestContext, (IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>) metadataService, upstreamVersionListService, true).Bootstrap(), metadataCacheService, ingestingHandler, extraExtractingContentProviders: (IReadOnlyList<ISpecificExtractingStorageContentProvider>) new ISpecificExtractingStorageContentProvider[1]
      {
        ZipExtractingStorageContentProvider.Instance
      }).Bootstrap().CastToMatchInputTypeOf<IPackageInnerFileRequest<VssNuGetPackageIdentity, NuGetGetFileData>, IPackageFileRequest<VssNuGetPackageIdentity>, HttpResponseMessage>((IHaveInputType<IPackageInnerFileRequest<VssNuGetPackageIdentity, NuGetGetFileData>>) this));
    }
  }
}
