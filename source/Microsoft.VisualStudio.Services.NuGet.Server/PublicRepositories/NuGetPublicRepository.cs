// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories.NuGetPublicRepository
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.Search3;
using Microsoft.VisualStudio.Services.NuGet.Server.UpstreamClient;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.DocumentProvider;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Diagnostics.CodeAnalysis;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories
{
  [ExcludeFromCodeCoverage]
  public static class NuGetPublicRepository
  {
    public static INuGetPublicRepository<INuGetV3Client> BootstrapNuGetOrg(
      IVssRequestContext deploymentRequestContext)
    {
      return NuGetPublicRepository.BootstrapV3(deploymentRequestContext, WellKnownSources.NugetOrg);
    }

    public static INuGetPublicRepository<IUpstreamNuGetClient> BootstrapPsGallery(
      IVssRequestContext deploymentRequestContext)
    {
      return NuGetPublicRepository.BootstrapV2(deploymentRequestContext, WellKnownSources.PowerShellGallery);
    }

    public static INuGetPublicRepository<INuGetV3Client> BootstrapV3(
      IVssRequestContext deploymentContext,
      WellKnownUpstreamSource wellKnownUpstreamSource)
    {
      deploymentContext.CheckDeploymentRequestContext();
      return (INuGetPublicRepository<INuGetV3Client>) NuGetPublicRepository.BootstrapCore<PublicNuGetHttpClient>(deploymentContext, wellKnownUpstreamSource, new PublicNuGetHttpClient(wellKnownUpstreamSource.Location, (IHttpClient) NuGetPublicRepository.BootstrapRegularHttpClient(deploymentContext), (IPublicServiceIndexService) new PublicServiceIndexFacade(deploymentContext), (IVersionCountsFromFileProvider) NuGetPublicRepository.BootstrapVersionCountsFromFileProvider(), deploymentContext.GetTracerFacade(), (ICancellationFacade) new CancellationFacade(deploymentContext)));
    }

    public static INuGetPublicRepository<IUpstreamNuGetClient> BootstrapV2(
      IVssRequestContext deploymentContext,
      WellKnownUpstreamSource wellKnownUpstreamSource)
    {
      deploymentContext.CheckDeploymentRequestContext();
      return (INuGetPublicRepository<IUpstreamNuGetClient>) NuGetPublicRepository.BootstrapCore<PublicNuGetV2HttpClient>(deploymentContext, wellKnownUpstreamSource, new PublicNuGetV2HttpClient(wellKnownUpstreamSource.Location, (IHttpClient) NuGetPublicRepository.BootstrapRegularHttpClient(deploymentContext), (IVersionCountsFromFileProvider) NuGetPublicRepository.BootstrapVersionCountsFromFileProvider(), (IHttpClient) NuGetPublicRepository.BootstrapNonForwardingHttpClient(deploymentContext)));
    }

    private static INuGetPublicRepository<TClient> BootstrapCore<TClient>(
      IVssRequestContext deploymentContext,
      WellKnownUpstreamSource wellKnownUpstreamSource,
      TClient directClient)
      where TClient : IUpstreamNuGetClient
    {
      return (INuGetPublicRepository<TClient>) new NuGetPublicRepository<TClient>(wellKnownUpstreamSource, directClient, (IPublicRepositoryInterestTracker<VssNuGetPackageName>) PublicRepositoryInterestTracker<VssNuGetPackageName>.Bootstrap(deploymentContext, NuGetIdentityResolver.Instance.NameResolver), (IPublicRepoCacheCore<VssNuGetPackageName, NuGetPubCachePackageNameFile, NuGetCatalogCursor>) NuGetPublicRepository.BootstrapCacheCore(deploymentContext, wellKnownUpstreamSource, (IUpstreamNuGetClient) directClient));
    }

    private static IPublicRepoPackageMemoryCache<NuGetPubCachePackageNameFile> BootstrapMemoryCache(
      IVssRequestContext deploymentContext)
    {
      deploymentContext.CheckDeploymentRequestContext();
      return PublicRepoPackageMemoryCacheFacade.Create<NuGetPubCachePackageNameFile>(deploymentContext, (IPublicRepoPackageMemoryCacheService<NuGetPubCachePackageNameFile>) deploymentContext.GetService<NuGetPublicRepositoryPackageMemoryCacheService>());
    }

    private static PublicRepoCacheCore<VssNuGetPackageName, NuGetPubCachePackageNameFile, NuGetCatalogCursor> BootstrapCacheCore(
      IVssRequestContext deploymentContext,
      WellKnownUpstreamSource wellKnownUpstreamSource,
      IUpstreamNuGetClient directClient)
    {
      IAggregationDocumentProcessor<NuGetPubCachePackageNameFile> documentProcessor = PubCacheProtobufDocumentProcessor.Bootstrap<NuGetPubCachePackageNameFile>(NuGetPubCachePackageNameFile.Parser);
      return new PublicRepoCacheCore<VssNuGetPackageName, NuGetPubCachePackageNameFile, NuGetCatalogCursor>(PublicRepositoryDocumentProvider.Bootstrap<NuGetPubCachePackageNameFile, VssNuGetPackageName>(deploymentContext, wellKnownUpstreamSource, documentProcessor, (ICacheUniverseProvider) new NuGetCacheUniverseProvider(deploymentContext)), (IETaggedDocumentUpdater) new ETaggedDocumentUpdater(), deploymentContext.GetTracerFacade(), NuGetPublicRepository.BootstrapMemoryCache(deploymentContext), (ICacheUniverseProvider) new NuGetCacheUniverseProvider(deploymentContext), PublicRepositoryCacheConcurrencyConsolidator.Bootstrap<NuGetCatalogCursor, NuGetPubCachePackageNameFile>(deploymentContext, new Guid("F4B9853A-F1BD-446F-AD6D-68E1E59E15E6")), (IDirectPublicRepoDataFetcher<VssNuGetPackageName, NuGetPubCachePackageNameFile, NuGetCatalogCursor>) new NuGetDirectPublicRepoDataFetcher(deploymentContext.GetTracerFacade(), (ITimeProvider) new DefaultTimeProvider(), directClient), wellKnownUpstreamSource, (IEmptyDocumentProvider<NuGetPubCachePackageNameFile>) documentProcessor);
    }

    private static VersionCountsFromFileProvider BootstrapVersionCountsFromFileProvider() => new VersionCountsFromFileProvider((INuGetPackageMetadataSearchVersionFilteringStrategy) new NuGetPackageMetadataSearchVersionFilteringStrategy(), (INuGetLatestVersionsFinder) new NuGetLatestVersionsFinder());

    private static HttpClientFacade BootstrapNonForwardingHttpClient(
      IVssRequestContext requestContext)
    {
      return new HttpClientFacade(requestContext, (IRequestContextAwareHttpClient) requestContext.GetService<INonForwardingPublicUpstreamHttpClient>());
    }

    private static HttpClientFacade BootstrapRegularHttpClient(IVssRequestContext requestContext) => new HttpClientFacade(requestContext, (IRequestContextAwareHttpClient) requestContext.GetService<IPublicUpstreamHttpClient>());
  }
}
