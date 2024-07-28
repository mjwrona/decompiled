// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PublicRepository.PyPiPublicRepository
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.DocumentProvider;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.Server.Upstreams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.PublicRepository
{
  public class PyPiPublicRepository : 
    IPyPiPublicRepository,
    IPublicRepository<IUpstreamPyPiClient>,
    IPublicRepository,
    IPublicRepositoryWithCursorAssistedInvalidation<PyPiPackageName, PyPiPackageVersion, PyPiChangelogCursor>,
    IPublicRepositoryWithDiagnostics
  {
    public PyPiPublicRepository(
      WellKnownUpstreamSource wellKnownUpstreamSource,
      IPublicUpstreamPyPiClient directClient,
      IPublicRepositoryInterestTracker<PyPiPackageName> interestTracker,
      IPublicRepoCacheCore<PyPiPackageName, PyPiPubCachePackageNameFile, PyPiChangelogCursor> cacheCore)
    {
      this.WellKnownUpstreamSource = wellKnownUpstreamSource;
      this.DirectClient = directClient;
      this.InterestTracker = interestTracker;
      // ISSUE: reference to a compiler-generated field
      this.\u003CcacheCore\u003EP = cacheCore;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public WellKnownUpstreamSource WellKnownUpstreamSource { get; }

    public IPublicUpstreamPyPiClient DirectClient { get; }

    public IPublicRepositoryInterestTracker<PyPiPackageName> InterestTracker { get; }

    public async Task<IEnumerable<PublicRepoPackageVersionDiagInfo>> GetVersionsOfPackageDiagnosticsAsync(
      IPackageName packageName,
      PublicRepositoryCacheType? cacheType)
    {
      // ISSUE: reference to a compiler-generated field
      PyPiPubCachePackageNameFile diagnosticsAsync = await this.\u003CcacheCore\u003EP.GetMetadataForDiagnosticsAsync(cacheType, (PyPiPackageName) packageName);
      return (diagnosticsAsync != null ? diagnosticsAsync.Versions.Select<PyPiPubCacheVersionLevelInfo, PublicRepoPackageVersionDiagInfo>((Func<PyPiPubCacheVersionLevelInfo, PublicRepoPackageVersionDiagInfo>) (x => new PublicRepoPackageVersionDiagInfo((IPackageVersion) x.Identity.Version))) : (IEnumerable<PublicRepoPackageVersionDiagInfo>) null) ?? Enumerable.Empty<PublicRepoPackageVersionDiagInfo>();
    }

    public async Task<Stream> GetFileAsync(PyPiPackageIdentity packageIdentity, string filePath) => await this.\u003CdirectClient\u003EP.GetFile(UnusableFeedRequest.Instance, packageIdentity, filePath);

    public async Task<Stream?> GetGpgSignatureForFileAsync(
      PyPiPackageIdentity packageIdentity,
      string filePath)
    {
      // ISSUE: reference to a compiler-generated field
      return await this.\u003CdirectClient\u003EP.GetGpgSignatureForFile(UnusableFeedRequest.Instance, packageIdentity, filePath);
    }

    public async Task<PyPiUpstreamMetadata> GetUpstreamMetadataAsync(
      PyPiPackageIdentity packageIdentity,
      string filePath)
    {
      // ISSUE: reference to a compiler-generated field
      return await this.\u003CdirectClient\u003EP.GetUpstreamMetadata(UnusableFeedRequest.Instance, packageIdentity, filePath);
    }

    public async Task<PyPiPubCachePackageNameFile> GetPackageMetadataAsync(
      PyPiPackageName packageName)
    {
      // ISSUE: reference to a compiler-generated field
      return await this.\u003CcacheCore\u003EP.GetPackageMetadataAsync(packageName);
    }

    public async Task InvalidatePackageVersionDataAsync(
      PyPiPackageName packageName,
      IEnumerable<PyPiPackageVersion> packageVersions,
      PyPiChangelogCursor? minValidCursor,
      bool allowRefresh)
    {
      // ISSUE: reference to a compiler-generated field
      await this.\u003CcacheCore\u003EP.InvalidatePackageVersionDataAsync(packageName, minValidCursor, allowRefresh);
    }

    private static IPyPiPublicRepository Bootstrap(
      IVssRequestContext deploymentContext,
      WellKnownUpstreamSource wellKnownUpstreamSource)
    {
      deploymentContext.CheckDeploymentRequestContext();
      PublicUpstreamPyPiClient directClient = new PublicUpstreamPyPiClient(wellKnownUpstreamSource.Location, (IHttpClient) new HttpClientFacade(deploymentContext, UpstreamHttpClient.ForProtocol((IProtocol) Protocol.PyPi)), (IConverter<PyPiUpstreamJsonMetadataPackageFileMetadataRequest, IReadOnlyDictionary<string, string[]>>) new PyPiUpstreamJsonMetadataToIngestionMetadataConverter(), (IConverter<string, PyPiPackageRegistrationState>) new PyPiDotOrgNameLevelJsonToLimitedMetadatasConverter());
      IAggregationDocumentProcessor<PyPiPubCachePackageNameFile> documentProcessor = PubCacheProtobufDocumentProcessor.Bootstrap<PyPiPubCachePackageNameFile>(PyPiPubCachePackageNameFile.Parser);
      IAggregationDocumentProvider<PyPiPubCachePackageNameFile, PyPiPackageName> docProvider = PublicRepositoryDocumentProvider.Bootstrap<PyPiPubCachePackageNameFile, PyPiPackageName>(deploymentContext, wellKnownUpstreamSource, documentProcessor, (ICacheUniverseProvider) new SingleCacheUniverseProvider());
      return (IPyPiPublicRepository) new PyPiPublicRepository(wellKnownUpstreamSource, (IPublicUpstreamPyPiClient) directClient, (IPublicRepositoryInterestTracker<PyPiPackageName>) PublicRepositoryInterestTracker<PyPiPackageName>.Bootstrap(deploymentContext, PyPiIdentityResolver.Instance.NameResolver), (IPublicRepoCacheCore<PyPiPackageName, PyPiPubCachePackageNameFile, PyPiChangelogCursor>) new PublicRepoCacheCore<PyPiPackageName, PyPiPubCachePackageNameFile, PyPiChangelogCursor>(docProvider, (IETaggedDocumentUpdater) new ETaggedDocumentUpdater(), deploymentContext.GetTracerFacade(), PublicRepoPackageMemoryCacheFacade.Create<PyPiPubCachePackageNameFile>(deploymentContext, (IPublicRepoPackageMemoryCacheService<PyPiPubCachePackageNameFile>) deploymentContext.GetService<PyPiPublicRepositoryPackageMemoryCacheService>()), (ICacheUniverseProvider) new SingleCacheUniverseProvider(), PublicRepositoryCacheConcurrencyConsolidator.Bootstrap<PyPiChangelogCursor, PyPiPubCachePackageNameFile>(deploymentContext, new Guid("9815EBF3-4252-4F3D-A070-A668781E7495")), (IDirectPublicRepoDataFetcher<PyPiPackageName, PyPiPubCachePackageNameFile, PyPiChangelogCursor>) new PyPiDirectPublicRepoDataFetcher((IPublicUpstreamPyPiClient) directClient, (ITimeProvider) new DefaultTimeProvider()), wellKnownUpstreamSource, (IEmptyDocumentProvider<PyPiPubCachePackageNameFile>) documentProcessor));
    }

    public static IPyPiPublicRepository BootstrapPyPi(IVssRequestContext requestContext) => PyPiPublicRepository.Bootstrap(requestContext, WellKnownSources.PyPiOrg);

    public IUpstreamPyPiClient GetProxyClient(CollectionId downstreamCollectionId) => (IUpstreamPyPiClient) new PyPiPublicRepositoryClient(downstreamCollectionId, (IPyPiPublicRepository) this, this.InterestTracker, this.WellKnownUpstreamSource);
  }
}
