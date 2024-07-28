// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenGetPackageFileContentHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Converters;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.Server.Upstreams;
using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Maven.Server
{
  internal class MavenGetPackageFileContentHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<
    #nullable disable
    MavenFileRequest, MavenPackageFileResponse, IMavenMetadataAggregationAccessor, IMavenStorageIdCacheAggregationAccessor, IUpstreamVersionListService<MavenPackageName, MavenPackageVersion>>
  {
    private readonly IVssRequestContext requestContext;

    public MavenGetPackageFileContentHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<MavenFileRequest, MavenPackageFileResponse> Bootstrap(
      IMavenMetadataAggregationAccessor metadataAggregation,
      IMavenStorageIdCacheAggregationAccessor cacheAggregation,
      IUpstreamVersionListService<MavenPackageName, MavenPackageVersion> upstreamVersionListService)
    {
      MavenPackageMetricsFactory packageMetricsServiceFactory = new MavenPackageMetricsFactory((IPackageMetricsServiceFacade) new PackageMetricsServiceFacade(this.requestContext));
      IFeatureFlagService featureFlagFacade = this.requestContext.GetFeatureFlagFacade();
      ITracerService tracerFacade = this.requestContext.GetTracerFacade();
      ThrowIfDeletedDelegatingMetadataDocumentStore<MavenPackageIdentity, IMavenMetadataEntry> metadataService = new ThrowIfDeletedDelegatingMetadataDocumentStore<MavenPackageIdentity, IMavenMetadataEntry>((IReadMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry>) metadataAggregation);
      ICache<string, object> cache = (ICache<string, object>) new RequestContextItemsAsCacheFacade(this.requestContext);
      MavenFileRequestToPackageFileRequestConverter converter = new MavenFileRequestToPackageFileRequestConverter();
      IContentBlobStore contentBlobStore = new ContentBlobStoreFacadeBootstrapper(this.requestContext).Bootstrap();
      SASTokenExpiryFacade sasTokenExpiryFactory = new SASTokenExpiryFacade(this.requestContext);
      IValidator<IPackageRequest> validator = new BlockedPackageIdentityRequestToExceptionConverterBootstrapper(this.requestContext).Bootstrap().AsThrowingValidator<IPackageRequest>();
      MavenFileRequestToMavenArtifactFileRequestConverter mavenFileRequestConverter = new MavenFileRequestToMavenArtifactFileRequestConverter();
      IFactory<IPackageRequest<MavenPackageIdentity>, Task<IMavenMetadataEntry>> metadataFactory = ByFuncInputFactory.For<IPackageRequest<MavenPackageIdentity>, Task<IMavenMetadataEntry>>((Func<IPackageRequest<MavenPackageIdentity>, Task<IMavenMetadataEntry>>) (async packageRequest => await metadataAggregation.GetPackageVersionStateAsync(packageRequest))).SingleElementCache<IPackageRequest<MavenPackageIdentity>, Task<IMavenMetadataEntry>>();
      MavenGetPackageFileContentHandler currentHandler1 = new MavenGetPackageFileContentHandler((IMetadataCacheService) cacheAggregation, metadataFactory, contentBlobStore, (IValidator<MavenArtifactFileRequest>) validator, (IConverter<MavenFileRequest, MavenArtifactFileRequest>) mavenFileRequestConverter, (IFactory<DateTimeOffset>) sasTokenExpiryFactory);
      IAsyncHandler<MavenPackageFileResponse> forwardingToThisHandler1 = new MavenDownloadCiDataFacadeHandler(this.requestContext).ThenDelegateTo<MavenPackageFileResponse, ICiData>((IAsyncHandler<ICiData>) new TelemetryPublisherUsingTracerFacadeBootstrapper(this.requestContext).Bootstrap());
      AddStorageInfoToPackageFileRequestHandler<MavenPackageIdentity, IMavenMetadataEntry> currentHandler2 = new AddStorageInfoToPackageFileRequestHandler<MavenPackageIdentity, IMavenMetadataEntry>((IMetadataCacheService) cacheAggregation, new MavenUpstreamFetchingMetadataServiceBootstrapper(this.requestContext, (IReadMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry>) metadataService, upstreamVersionListService).Bootstrap(), UpstreamEntriesValidChecker.Bootstrap(this.requestContext), tracerFacade, cache, (IHandler<IFeedRequest, IEnumerable<UpstreamSource>>) new UpstreamsFromFeedHandler(featureFlagFacade), validator);
      IAsyncHandler<IPackageFileRequest<MavenPackageIdentity>, IPackageFileRequest<MavenPackageIdentity, IStorageId>> exceptionHandler = new AddStorageInfoToPackageFileRequestHandler<MavenPackageIdentity, IMavenMetadataEntry>((IMetadataCacheService) cacheAggregation, (IReadMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry>) metadataService, (IUpstreamEntriesValidChecker) new UpstreamEntriesAreInvalid(), tracerFacade, cache, (IHandler<IFeedRequest, IEnumerable<UpstreamSource>>) new UpstreamsFromFeedHandler(featureFlagFacade, new UpstreamSourceType?(UpstreamSourceType.Public)), validator).ThenForwardResultTo<IPackageFileRequest<MavenPackageIdentity>, IPackageFileRequest<MavenPackageIdentity, IStorageId>>((IAsyncHandler<IPackageFileRequest<MavenPackageIdentity, IStorageId>>) new LogThatWeForcedMavenCentralRetrieval(cache));
      IAsyncHandler<IPackageFileRequest<MavenPackageIdentity, IStorageId>, HttpResponseMessage> handler1 = UntilNonNullHandler.Create<IPackageFileRequest<MavenPackageIdentity, IStorageId>, HttpResponseMessage>((IAsyncHandler<IPackageFileRequest<MavenPackageIdentity, IStorageId>, HttpResponseMessage>) new DownloadPackageFileAsResponseMessageHandler<MavenPackageIdentity>((IAsyncHandler<IPackageFileRequest<MavenPackageIdentity, IStorageId>, ContentResult>) new DownloadPackagePermissionsCheckingDelegatingHandler<MavenPackageIdentity>(new IngestRawMavenFileIfNotAlreadyIngestedBootstrapper(this.requestContext).Bootstrap(), (IFeedPerms) new FeedPermsFacade(this.requestContext), (IExecutionEnvironment) new ExecutionEnvironmentFacade(this.requestContext), FeatureAvailabilityConstants.UpstreamsAllowedForPublicFeeds.Bootstrap(this.requestContext), FeatureAvailabilityConstants.UpstreamsAllowedForPublicFeedsMSFT.Bootstrap(this.requestContext))), (IAsyncHandler<IPackageFileRequest<MavenPackageIdentity, IStorageId>, HttpResponseMessage>) new ByFuncAsyncHandler<IPackageFileRequest<MavenPackageIdentity>, HttpResponseMessage>((Func<IPackageFileRequest<MavenPackageIdentity>, HttpResponseMessage>) (request =>
      {
        throw new InvalidHandlerException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_NoMatchingHandlerForType((object) request.GetType().FullName));
      })));
      ByAsyncFuncAsyncHandler<HttpResponseMessage, MavenPackageFileResponse> handler2 = new ByAsyncFuncAsyncHandler<HttpResponseMessage, MavenPackageFileResponse>((Func<HttpResponseMessage, Task<MavenPackageFileResponse>>) (async m =>
      {
        MavenPackageFileResponse packageFileResponse1 = new MavenPackageFileResponse();
        packageFileResponse1.FileName = m.Content.Headers.ContentDisposition.FileName;
        MavenPackageFileResponse packageFileResponse2 = packageFileResponse1;
        Stream streamImplementation = await m.Content.ReadAsStreamAsync();
        packageFileResponse2.Content = (Stream) new DisposingStreamWrapper(streamImplementation, new IDisposable[1]
        {
          (IDisposable) m
        });
        return packageFileResponse1;
      }));
      ByFuncAsyncHandler<PackageFileRequest<MavenPackageIdentity>, PackageFileRequest<MavenPackageIdentity>> handler3 = new ByFuncAsyncHandler<PackageFileRequest<MavenPackageIdentity>, PackageFileRequest<MavenPackageIdentity>>((Func<PackageFileRequest<MavenPackageIdentity>, PackageFileRequest<MavenPackageIdentity>>) (packagefileRequest =>
      {
        if (MavenIdentityUtility.IsSnapshotVersion(packagefileRequest.PackageId.Version))
          throw ExceptionHelper.PackageNotFound((IPackageIdentity) packagefileRequest.PackageId, packagefileRequest.Feed);
        return packagefileRequest;
      }));
      MavenPackageMetricsHandler forwardingToThisHandler2 = new MavenPackageMetricsHandler((IConverter<MavenFileRequest, MavenArtifactFileRequest>) mavenFileRequestConverter, metadataFactory, (IFactory<MavenArtifactFileRequest, IPackageMetricsServiceFacade>) packageMetricsServiceFactory);
      return new RetryDecoratingHandler<MavenFileRequest, MavenPackageFileResponse>(currentHandler1.CatchHandlerException<MavenFileRequest, MavenPackageFileResponse, Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException>((Func<Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException, bool>) (e => true), converter.ThenDelegateTo<MavenFileRequest, PackageFileRequest<MavenPackageIdentity>, PackageFileRequest<MavenPackageIdentity>>((IAsyncHandler<PackageFileRequest<MavenPackageIdentity>, PackageFileRequest<MavenPackageIdentity>>) handler3).ThenDelegateTo<MavenFileRequest, PackageFileRequest<MavenPackageIdentity>, IPackageFileRequest<MavenPackageIdentity, IStorageId>>((IAsyncHandler<PackageFileRequest<MavenPackageIdentity>, IPackageFileRequest<MavenPackageIdentity, IStorageId>>) currentHandler2.CatchHandlerException<IPackageFileRequest<MavenPackageIdentity>, IPackageFileRequest<MavenPackageIdentity, IStorageId>, Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException>((Func<Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException, bool>) (ex => true), exceptionHandler)).ThenDelegateTo<MavenFileRequest, IPackageFileRequest<MavenPackageIdentity, IStorageId>, HttpResponseMessage>(handler1).ThenDelegateTo<MavenFileRequest, HttpResponseMessage, MavenPackageFileResponse>((IAsyncHandler<HttpResponseMessage, MavenPackageFileResponse>) handler2).ThenForwardOriginalRequestAndResultTo<MavenFileRequest, MavenPackageFileResponse>((IAsyncHandler<(MavenFileRequest, MavenPackageFileResponse)>) new AddFileRequestToMavenPackageFileResponse())), new RetryHelper(this.requestContext.GetTracerFacade(), (IReadOnlyList<TimeSpan>) new List<TimeSpan>()
      {
        TimeSpan.FromMilliseconds(100.0)
      }, (Func<Exception, bool>) (exception => exception is PackageExistsIngestingFromUpstreamException))).ThenForwardOriginalRequestTo<MavenFileRequest, MavenPackageFileResponse>((IAsyncHandler<MavenFileRequest, NullResult>) forwardingToThisHandler2).ThenForwardResultTo<MavenFileRequest, MavenPackageFileResponse>(forwardingToThisHandler1);
    }
  }
}
