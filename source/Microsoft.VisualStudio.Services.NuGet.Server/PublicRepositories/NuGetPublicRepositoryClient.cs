// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories.NuGetPublicRepositoryClient
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.UpstreamCache.V2;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories
{
  public class NuGetPublicRepositoryClient : IUpstreamNuGetClient, IUpstreamPackageNamesClient
  {
    public NuGetPublicRepositoryClient(
      CollectionId downstreamCollectionId,
      INuGetPublicRepository repository,
      IPublicRepositoryInterestTracker<VssNuGetPackageName> interestTracker,
      WellKnownUpstreamSource wellKnownSource)
    {
      // ISSUE: reference to a compiler-generated field
      this.\u003CdownstreamCollectionId\u003EP = downstreamCollectionId;
      // ISSUE: reference to a compiler-generated field
      this.\u003Crepository\u003EP = repository;
      // ISSUE: reference to a compiler-generated field
      this.\u003CinterestTracker\u003EP = interestTracker;
      // ISSUE: reference to a compiler-generated field
      this.\u003CwellKnownSource\u003EP = wellKnownSource;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public async Task<IReadOnlyDictionary<VssNuGetPackageVersion, ContentBytes>> GetNuspecs(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageName packageId,
      IEnumerable<VssNuGetPackageVersion> packageVersions)
    {
      HashSet<IPackageVersion> requestedSet = ((IEnumerable<IPackageVersion>) packageVersions).ToHashSet<IPackageVersion>((IEqualityComparer<IPackageVersion>) PackageVersionComparer.NormalizedVersion);
      // ISSUE: reference to a compiler-generated field
      return (IReadOnlyDictionary<VssNuGetPackageVersion, ContentBytes>) (await this.\u003Crepository\u003EP.GetPackageMetadataAsync(packageId)).Versions.Select(x => new
      {
        Version = x.Identity.Version,
        Info = x
      }).Where(x => requestedSet.Contains((IPackageVersion) x.Version)).ToDictionary(x => x.Version, x => new ContentBytes(x.Info.Nuspec.DeflatedBytes.ToByteArray(), true));
    }

    public async Task<NuGetUpstreamMetadata> GetUpstreamMetadata(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageIdentity packageIdentity)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return new NuGetUpstreamMetadata()
      {
        Listed = ((await this.\u003Crepository\u003EP.GetPackageMetadataAsync(packageIdentity.Name)).Versions.FirstOrDefault<NuGetPubCacheVersionLevelInfo>((Func<NuGetPubCacheVersionLevelInfo, bool>) (x => PackageVersionComparer.NormalizedVersion.Equals((IPackageVersion) x.Identity.Version, (IPackageVersion) packageIdentity.Version))) ?? throw new PackageNotFoundException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_UpstreamReturnedNotFound((object) this.\u003CwellKnownSource\u003EP.LocationUriString))).MutableInfo.Listed,
        SourceChain = (IReadOnlyCollection<UpstreamSourceInfo>) Array.Empty<UpstreamSourceInfo>(),
        StorageId = (IStorageId) null
      };
    }

    public async Task<IReadOnlyList<VersionWithSourceChain<VssNuGetPackageVersion>>> GetPackageVersions(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageName packageName)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      await this.\u003CinterestTracker\u003EP.RegisterInterestAsync(this.\u003CdownstreamCollectionId\u003EP, downstreamFeedRequest, packageName, this.\u003CwellKnownSource\u003EP);
      // ISSUE: reference to a compiler-generated field
      return (IReadOnlyList<VersionWithSourceChain<VssNuGetPackageVersion>>) (await this.\u003Crepository\u003EP.GetPackageMetadataAsync(packageName)).Versions.Select<NuGetPubCacheVersionLevelInfo, VersionWithSourceChain<VssNuGetPackageVersion>>((Func<NuGetPubCacheVersionLevelInfo, VersionWithSourceChain<VssNuGetPackageVersion>>) (x => VersionWithSourceChain.FromExternalSource<VssNuGetPackageVersion>(x.Identity.Version))).ToList<VersionWithSourceChain<VssNuGetPackageVersion>>();
    }

    public async Task<Stream> GetNupkg(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageIdentity packageIdentity)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      await this.\u003CinterestTracker\u003EP.RegisterInterestAsync(this.\u003CdownstreamCollectionId\u003EP, downstreamFeedRequest, packageIdentity.Name, this.\u003CwellKnownSource\u003EP);
      // ISSUE: reference to a compiler-generated field
      return await this.\u003Crepository\u003EP.GetNupkgAsync(packageIdentity);
    }

    public Task<GetVersionCountsResult> GetVersionCounts(
      NuGetSearchCategoryToggles queryCategories,
      string queryHint)
    {
      // ISSUE: reference to a compiler-generated field
      return this.\u003Crepository\u003EP.GetVersionCountsAsync(queryCategories, queryHint);
    }

    public async Task<NuGetPackageRegistrationState> GetRegistrationState(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageName packageName,
      IEnumerable<VssNuGetPackageVersion> versions)
    {
      HashSet<IPackageVersion> requestedSet = ((IEnumerable<IPackageVersion>) versions).ToHashSet<IPackageVersion>((IEqualityComparer<IPackageVersion>) PackageVersionComparer.NormalizedVersion);
      // ISSUE: reference to a compiler-generated field
      NuGetPubCachePackageNameFile packageMetadataAsync = await this.\u003Crepository\u003EP.GetPackageMetadataAsync(packageName);
      return new NuGetPackageRegistrationState((IImmutableDictionary<VssNuGetPackageVersion, NuGetRegistrationState>) packageMetadataAsync.Versions.Select(x => new
      {
        Version = x.Identity.Version,
        Info = x
      }).Where(x => requestedSet.Contains((IPackageVersion) x.Version)).ToImmutableDictionary(x => x.Version, x => NuGetPublicRepositoryClient.ToRegistrationState(x.Info)), packageMetadataAsync.GenerationCursorPosition);
    }

    private static NuGetRegistrationState ToRegistrationState(NuGetPubCacheVersionLevelInfo info)
    {
      return new NuGetRegistrationState(info.Identity, info.MutableInfo.Deprecation != null ? TransformDeprecation(info.MutableInfo.Deprecation) : (NuGetDeprecation) null, info.MutableInfo.Listed, new DateTime?(info.MutableInfo.PublishDate.ToDateTime()), (IImmutableList<NuGetVulnerability>) info.MutableInfo.Vulnerabilities.Select<NuGetPubCacheVulnerability, NuGetVulnerability>(new Func<NuGetPubCacheVulnerability, NuGetVulnerability>(TransformVulnerability)).ToImmutableArray<NuGetVulnerability>(), info.MutableInfo.GenerationCursorPosition);

      static NuGetDeprecation TransformDeprecation(NuGetPubCachePackageDeprecation storedDeprecation) => new NuGetDeprecation((IImmutableList<string>) storedDeprecation.Reasons.ToImmutableArray<string>(), storedDeprecation.NullableMessage, storedDeprecation.AlternatePackage?.Id, storedDeprecation.AlternatePackage?.NullableRange);

      static NuGetVulnerability TransformVulnerability(
        NuGetPubCacheVulnerability storedVulnerability)
      {
        return new NuGetVulnerability(storedVulnerability.AdvisoryUrl, storedVulnerability.Severity);
      }
    }

    Task<IReadOnlyList<IPackageNameEntry<VssNuGetPackageName>>> IUpstreamNuGetClient.GetPackageNames() => throw new NotImplementedException("should not call this api for public upstreams");

    Task<IReadOnlyList<RawPackageNameEntry>> IUpstreamPackageNamesClient.GetPackageNames() => throw new NotImplementedException("should not call this api for public upstreams");
  }
}
