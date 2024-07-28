// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts.VersionCountsFromFileProvider
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.Search3;
using Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts
{
  public class VersionCountsFromFileProvider : IVersionCountsFromFileProvider
  {
    private readonly INuGetPackageMetadataSearchVersionFilteringStrategy filteringStrategy;
    private readonly INuGetLatestVersionsFinder latestVersionsFinder;

    public VersionCountsFromFileProvider(
      INuGetPackageMetadataSearchVersionFilteringStrategy filteringStrategy,
      INuGetLatestVersionsFinder latestVersionsFinder)
    {
      this.filteringStrategy = filteringStrategy;
      this.latestVersionsFinder = latestVersionsFinder;
    }

    public IEnumerable<ILazyNuGetPackageVersionCounts> GetVersionCountsFromVersionListFile(
      ILazyVersionListsFile versionListsFile,
      NuGetSearchCategoryToggles query,
      Guid viewId,
      bool isLocal)
    {
      return (IEnumerable<ILazyNuGetPackageVersionCounts>) versionListsFile.Packages.Select<ILazyVersionListsPackage, LazyNuGetPackageVersionCounts>((Func<ILazyVersionListsPackage, LazyNuGetPackageVersionCounts>) (x => new LazyNuGetPackageVersionCounts((IPackageNameEntry<VssNuGetPackageName>) x, (Func<INuGetPackageVersionCounts>) (() => (INuGetPackageVersionCounts) this.CalculateCounts(x, query, viewId, isLocal)))));
    }

    private NuGetPackageVersionCounts CalculateCounts(
      ILazyVersionListsPackage package,
      NuGetSearchCategoryToggles query,
      Guid viewId,
      bool isLocal)
    {
      VssNuGetPackageName name = package.Name;
      DateTime lastUpdatedDateTime = package.LastUpdatedDateTime;
      IEnumerable<NuGetSearchResultVersionSummary> versionSummaries = package.Get().Versions.Select<IVersionListsPackageVersion, NuGetSearchResultVersionSummary>((Func<IVersionListsPackageVersion, NuGetSearchResultVersionSummary>) (version => new NuGetSearchResultVersionSummary(version.PackageIdentity, version.IsListed, (IEnumerable<Guid>) version.ViewIds, version.IsDeleted, isLocal)));
      return this.CalculateCounts(query, viewId, versionSummaries, name, lastUpdatedDateTime);
    }

    public NuGetPackageVersionCounts CalculateCounts(
      NuGetSearchCategoryToggles query,
      Guid viewId,
      IEnumerable<NuGetSearchResultVersionSummary> versionSummaries,
      VssNuGetPackageName packageName,
      DateTime packageLastUpdatedDateTime)
    {
      List<NuGetSearchResultVersionSummary> list1 = versionSummaries.Where<NuGetSearchResultVersionSummary>((Func<NuGetSearchResultVersionSummary, bool>) (version => this.filteringStrategy.DoesVersionAppearToExist(query, viewId, version))).OrderByDescending<NuGetSearchResultVersionSummary, VssNuGetPackageVersion>((Func<NuGetSearchResultVersionSummary, VssNuGetPackageVersion>) (x => x.PackageIdentity.Version)).ToList<NuGetSearchResultVersionSummary>();
      LatestVersions latestVersions = this.latestVersionsFinder.FindLatestVersions((IReadOnlyCollection<NuGetSearchResultVersionSummary>) list1);
      List<NuGetSearchResultVersionSummary> list2 = list1.Where<NuGetSearchResultVersionSummary>((Func<NuGetSearchResultVersionSummary, bool>) (version => this.filteringStrategy.IsVersionSelectable(query, version))).ToList<NuGetSearchResultVersionSummary>();
      bool hasLatestVersion = latestVersions.LatestVersion != null && list2.Any<NuGetSearchResultVersionSummary>((Func<NuGetSearchResultVersionSummary, bool>) (x => x.PackageIdentity.Version.Equals(latestVersions.LatestVersion)));
      bool hasAbsoluteLatestVersion = latestVersions.AbsoluteLatestVersion != null && list2.Any<NuGetSearchResultVersionSummary>((Func<NuGetSearchResultVersionSummary, bool>) (x => x.PackageIdentity.Version.Equals(latestVersions.AbsoluteLatestVersion)));
      bool latestAndAbsoluteLatestAreSameVersion = hasLatestVersion & hasAbsoluteLatestVersion && latestVersions.LatestVersion.Equals(latestVersions.AbsoluteLatestVersion);
      int count = list2.Count;
      return new NuGetPackageVersionCounts(packageName, packageLastUpdatedDateTime, count, hasLatestVersion, hasAbsoluteLatestVersion, latestAndAbsoluteLatestAreSameVersion);
    }
  }
}
