// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Upstreams.IngestRawPackageIfNotAlreadyIngestedBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Parsing;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.Server.Upstreams
{
  public class IngestRawPackageIfNotAlreadyIngestedBootstrapper : 
    RequireAggHandlerBootstrapper<IRawPackageRequest, NullResult, INpmMetadataService, IMetadataCacheService, IUpstreamVersionListService<NpmPackageName, SemanticVersion>>
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

    public static IBootstrapper<IAsyncHandler<IRawPackageRequest, NullResult>> Create(
      IVssRequestContext requestContext,
      BlockedIdentityContext blockedIdentityContext)
    {
      return ExistingInstanceBootstrapper.Create<IAsyncHandler<IRawPackageRequest, NullResult>>(NpmAggregationResolver.Bootstrap(requestContext).HandlerFor<IRawPackageRequest, NullResult>((IRequireAggBootstrapper<IAsyncHandler<IRawPackageRequest, NullResult>>) new IngestRawPackageIfNotAlreadyIngestedBootstrapper(requestContext, blockedIdentityContext)));
    }

    protected override IAsyncHandler<IRawPackageRequest, NullResult> Bootstrap(
      INpmMetadataService metadataService,
      IMetadataCacheService metadataCacheService,
      IUpstreamVersionListService<NpmPackageName, SemanticVersion> upstreamVersionListService)
    {
      IConverter<IRawPackageRequest, PackageRequest<NpmPackageIdentity>> converter = new NpmRawPackageRequestToRequestConverterBootstrapper(this.requestContext).Bootstrap();
      ITracerService tracerFacade = this.requestContext.GetTracerFacade();
      AddStorageInfoToPackageFileRequestHandler<NpmPackageIdentity, INpmMetadataEntry> handler1 = new AddStorageInfoToPackageFileRequestHandler<NpmPackageIdentity, INpmMetadataEntry>(metadataCacheService, new NpmUpstreamFetchingMetadataServiceBootstrapper(this.requestContext, (IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>) metadataService, upstreamVersionListService).Bootstrap(), UpstreamEntriesValidChecker.Bootstrap(this.requestContext), tracerFacade, (ICache<string, object>) new RequestContextItemsAsCacheFacade(this.requestContext), (IHandler<IFeedRequest, IEnumerable<UpstreamSource>>) new UpstreamsFromFeedHandler(this.requestContext.GetFeatureFlagFacade()), new BlockedPackageIdentityRequestToExceptionConverterBootstrapper(this.requestContext, this.blockedIdentityContext).Bootstrap().AsThrowingValidator<IPackageRequest>());
      IAsyncHandler<IPackageFileRequest<NpmPackageIdentity, IStorageId>, ContentResult> handler2 = IngestPackageIfNotAlreadyIngestedBootstrapper.Create(this.requestContext).Bootstrap();
      return (IAsyncHandler<IRawPackageRequest, NullResult>) converter.ThenDelegateTo<IRawPackageRequest, PackageRequest<NpmPackageIdentity>, PackageFileRequest<NpmPackageIdentity>>(ConvertFrom.OutputTypeOf<PackageRequest<NpmPackageIdentity>>((IHaveOutputType<PackageRequest<NpmPackageIdentity>>) converter).By<PackageRequest<NpmPackageIdentity>, PackageFileRequest<NpmPackageIdentity>>((Func<PackageRequest<NpmPackageIdentity>, PackageFileRequest<NpmPackageIdentity>>) (r => new PackageFileRequest<NpmPackageIdentity>((IPackageRequest<NpmPackageIdentity>) r, r.PackageId.ToTgzFilePath())))).ThenDelegateTo<IRawPackageRequest, PackageFileRequest<NpmPackageIdentity>, IPackageFileRequest<NpmPackageIdentity, IStorageId>>((IAsyncHandler<PackageFileRequest<NpmPackageIdentity>, IPackageFileRequest<NpmPackageIdentity, IStorageId>>) handler1).ThenDelegateTo<IRawPackageRequest, IPackageFileRequest<NpmPackageIdentity, IStorageId>, ContentResult>(handler2).ThenReturnNullResult<IRawPackageRequest, ContentResult>();
    }
  }
}
