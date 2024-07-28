// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories.NuGetPublicRepository`1
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories;
using Microsoft.VisualStudio.Services.Packaging.Shared.Internal.WebApi.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories
{
  public class NuGetPublicRepository<TDirectClient> : 
    INuGetPublicRepository<TDirectClient>,
    IPublicRepository<IUpstreamNuGetClient>,
    IPublicRepository,
    IPublicRepositoryWithCursorAssistedInvalidation<VssNuGetPackageName, VssNuGetPackageVersion, NuGetCatalogCursor>,
    INuGetPublicRepository,
    IPublicRepositoryWithDiagnostics
    where TDirectClient : IUpstreamNuGetClient
  {
    private readonly IPublicRepoCacheCore<VssNuGetPackageName, NuGetPubCachePackageNameFile, NuGetCatalogCursor> publicRepoCacheCore;

    public NuGetPublicRepository(
      WellKnownUpstreamSource wellKnownUpstreamSource,
      TDirectClient directClient,
      IPublicRepositoryInterestTracker<VssNuGetPackageName> interestTracker,
      IPublicRepoCacheCore<VssNuGetPackageName, NuGetPubCachePackageNameFile, NuGetCatalogCursor> publicRepoCacheCore)
    {
      this.WellKnownUpstreamSource = wellKnownUpstreamSource;
      this.DirectClient = directClient;
      this.InterestTracker = interestTracker;
      this.publicRepoCacheCore = publicRepoCacheCore;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public TDirectClient DirectClient { get; }

    public IPublicRepositoryInterestTracker<VssNuGetPackageName> InterestTracker { get; }

    public WellKnownUpstreamSource WellKnownUpstreamSource { get; }

    public async Task InvalidatePackageVersionDataAsync(
      VssNuGetPackageName packageName,
      IEnumerable<VssNuGetPackageVersion> packageVersions,
      NuGetCatalogCursor? minValidCursor,
      bool allowRefresh)
    {
      await this.publicRepoCacheCore.InvalidatePackageVersionDataAsync(packageName, minValidCursor, allowRefresh);
    }

    public async Task<NuGetPubCachePackageNameFile> GetPackageMetadataAsync(
      VssNuGetPackageName packageName)
    {
      return await this.publicRepoCacheCore.GetPackageMetadataAsync(packageName);
    }

    public async Task<GetVersionCountsResult> GetVersionCountsAsync(
      NuGetSearchCategoryToggles queryCategories,
      string queryHint)
    {
      return await this.DirectClient.GetVersionCounts(queryCategories, queryHint);
    }

    public async Task<Stream> GetNupkgAsync(VssNuGetPackageIdentity packageIdentity) => await this.DirectClient.GetNupkg(UnusableFeedRequest.Instance, packageIdentity);

    public IUpstreamNuGetClient GetProxyClient(CollectionId downstreamCollectionId) => (IUpstreamNuGetClient) new NuGetPublicRepositoryClient(downstreamCollectionId, (INuGetPublicRepository) this, this.InterestTracker, this.WellKnownUpstreamSource);

    public async Task<IEnumerable<PublicRepoPackageVersionDiagInfo>> GetVersionsOfPackageDiagnosticsAsync(
      IPackageName packageName,
      PublicRepositoryCacheType? cacheType)
    {
      VssNuGetPackageName typedName = (VssNuGetPackageName) packageName;
      NuGetPubCachePackageNameFile diagnosticsAsync = await this.publicRepoCacheCore.GetMetadataForDiagnosticsAsync(cacheType, typedName);
      return (diagnosticsAsync != null ? diagnosticsAsync.Versions.Select<NuGetPubCacheVersionLevelInfo, PublicRepoPackageVersionDiagInfo>((Func<NuGetPubCacheVersionLevelInfo, PublicRepoPackageVersionDiagInfo>) (x => new PublicRepoPackageVersionDiagInfo((IPackageVersion) x.Identity.Version))) : (IEnumerable<PublicRepoPackageVersionDiagInfo>) null) ?? Enumerable.Empty<PublicRepoPackageVersionDiagInfo>();
    }
  }
}
