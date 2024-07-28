// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.IngestRawPackageIfNotAlreadyIngestedBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class IngestRawPackageIfNotAlreadyIngestedBootstrapper : 
    RequireAggHandlerBootstrapper<IRawPackageRequest, ContentResult, INuGetMetadataService, IMetadataCacheService, IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly BlockedIdentityContext blockedIdentityContext;

    public IngestRawPackageIfNotAlreadyIngestedBootstrapper(
      IVssRequestContext requestContext,
      BlockedIdentityContext blockedIdentityContext)
    {
      this.requestContext = requestContext;
      this.blockedIdentityContext = blockedIdentityContext;
    }

    public static IBootstrapper<IAsyncHandler<IRawPackageRequest, ContentResult>> Create(
      IVssRequestContext requestContext,
      BlockedIdentityContext blockedIdentityContext)
    {
      return ExistingInstanceBootstrapper.Create<IAsyncHandler<IRawPackageRequest, ContentResult>>(NuGetAggregationResolver.Bootstrap(requestContext).HandlerFor<IRawPackageRequest, ContentResult>((IRequireAggBootstrapper<IAsyncHandler<IRawPackageRequest, ContentResult>>) new IngestRawPackageIfNotAlreadyIngestedBootstrapper(requestContext, blockedIdentityContext)));
    }

    protected override IAsyncHandler<IRawPackageRequest, ContentResult> Bootstrap(
      INuGetMetadataService metadataService,
      IMetadataCacheService metadataCacheService,
      IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion> upstreamVersionListService)
    {
      NuGetRawPackageRequestConverter requestConverter = new NuGetRawPackageRequestConverter((IConverter<IRawPackageRequest, VssNuGetPackageIdentity>) new NuGetRawPackageRequestToIdentityConverter());
      ITracerService tracerFacade = this.requestContext.GetTracerFacade();
      AddStorageInfoToPackageFileRequestHandler<VssNuGetPackageIdentity, INuGetMetadataEntry> handler1 = new AddStorageInfoToPackageFileRequestHandler<VssNuGetPackageIdentity, INuGetMetadataEntry>(metadataCacheService, new NuGetUpstreamFetchingMetadataServiceBootstrapper(this.requestContext, (IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>) metadataService, upstreamVersionListService, true).Bootstrap(), UpstreamEntriesValidChecker.Bootstrap(this.requestContext), tracerFacade, (ICache<string, object>) new RequestContextItemsAsCacheFacade(this.requestContext), (IHandler<IFeedRequest, IEnumerable<UpstreamSource>>) new UpstreamsFromFeedHandler(this.requestContext.GetFeatureFlagFacade()), new BlockedPackageIdentityRequestToExceptionConverterBootstrapper(this.requestContext, this.blockedIdentityContext).Bootstrap().AsThrowingValidator<IPackageRequest>());
      IAsyncHandler<IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>, ContentResult> handler2 = IngestPackageIfNotAlreadyIngestedBootstrapper.Create(this.requestContext).Bootstrap();
      return requestConverter.ThenDelegateTo<IRawPackageRequest, PackageRequest<VssNuGetPackageIdentity>, PackageFileRequest<VssNuGetPackageIdentity>>(ConvertFrom.OutputTypeOf<PackageRequest<VssNuGetPackageIdentity>>((IHaveOutputType<PackageRequest<VssNuGetPackageIdentity>>) requestConverter).By<PackageRequest<VssNuGetPackageIdentity>, PackageFileRequest<VssNuGetPackageIdentity>>((Func<PackageRequest<VssNuGetPackageIdentity>, PackageFileRequest<VssNuGetPackageIdentity>>) (r => new PackageFileRequest<VssNuGetPackageIdentity>((IPackageRequest<VssNuGetPackageIdentity>) r, r.PackageId.ToNupkgFilePath())))).ThenDelegateTo<IRawPackageRequest, PackageFileRequest<VssNuGetPackageIdentity>, IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>>((IAsyncHandler<PackageFileRequest<VssNuGetPackageIdentity>, IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>>) handler1).ThenDelegateTo<IRawPackageRequest, IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>, ContentResult>(handler2);
    }
  }
}
