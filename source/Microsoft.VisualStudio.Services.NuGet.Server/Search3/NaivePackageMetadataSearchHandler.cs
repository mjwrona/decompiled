// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Search3.NaivePackageMetadataSearchHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.Search3
{
  public class NaivePackageMetadataSearchHandler : 
    IAsyncHandler<
    #nullable disable
    FeedRequest<NuGetSearchQuery>, NuGetSearchResultsInfo>,
    IHaveInputType<FeedRequest<NuGetSearchQuery>>,
    IHaveOutputType<NuGetSearchResultsInfo>
  {
    private readonly INuGetPackageVersionCountsService namesService;
    private readonly IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry> metadataService;
    private readonly ITracerService tracerService;
    private readonly INuGetPackageMetadataSearchVersionFilteringStrategy filterer;
    private readonly INuGetLatestVersionsFinder latestVersionsFinder;
    private readonly IConverter<IFeedRequest, Guid> viewIdExtractor;
    private readonly IFeatureFlagService featureFlagService;
    private readonly IFactory<UpstreamSource, Task<IUpstreamNuGetClient>> upstreamClientFactory;

    public NaivePackageMetadataSearchHandler(
      INuGetPackageVersionCountsService namesService,
      IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry> metadataService,
      ITracerService tracerService,
      INuGetPackageMetadataSearchVersionFilteringStrategy filterer,
      INuGetLatestVersionsFinder latestVersionsFinder,
      IConverter<IFeedRequest, Guid> viewIdExtractor,
      IFeatureFlagService featureFlagService,
      IFactory<UpstreamSource, Task<IUpstreamNuGetClient>> upstreamClientFactory)
    {
      this.namesService = namesService;
      this.metadataService = metadataService;
      this.tracerService = tracerService;
      this.filterer = filterer;
      this.latestVersionsFinder = latestVersionsFinder;
      this.viewIdExtractor = viewIdExtractor;
      this.featureFlagService = featureFlagService;
      this.upstreamClientFactory = upstreamClientFactory;
    }

    public async Task<NuGetSearchResultsInfo> Handle(FeedRequest<NuGetSearchQuery> request)
    {
      NaivePackageMetadataSearchHandler sendInTheThisObject = this;
      NuGetSearchTelemetryCollector telemetryCollector = new NuGetSearchTelemetryCollector();
      NuGetSearchResultsInfo searchResultsInfo;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (Handle)))
      {
        using (telemetryCollector.OverallTime.BeginTiming())
        {
          NuGetSearchQuery query = request.AdditionalData;
          if (!query.CanExecuteQuery)
          {
            if (query.AllErrors.Count == 1)
              throw query.AllErrors.Single<Exception>();
            throw new InvalidUserInputException(string.Join<Exception>("; ", (IEnumerable<Exception>) query.AllErrors));
          }
          bool queryMatchesAllVersions = query.VersionCategories.Contains(VersionCategorySelector.AllVersions);
          bool queryMatchesLatestVersion = queryMatchesAllVersions || query.VersionCategories.Contains(VersionCategorySelector.LatestVersion);
          bool queryMatchesAbsoluteLatestVersion = queryMatchesAllVersions || query.VersionCategories.Contains(VersionCategorySelector.AbsoluteLatestVersion);
          IReadOnlyList<IPackageNameEntry<VssNuGetPackageName>> packageNames = await sendInTheThisObject.GetPackageNames((IFeedRequest) request, query, telemetryCollector);
          List<NuGetSearchResult> all = new List<NuGetSearchResult>();
          int skip = query.Skip;
          int take = query.Take;
          int packageCount = 0;
          int versionCount = 0;
          foreach (IPackageNameEntry<VssNuGetPackageName> packageNameEntry in (IEnumerable<IPackageNameEntry<VssNuGetPackageName>>) packageNames)
          {
            if (!TookEnoughPackages())
            {
              bool flag1 = NaivePackageMetadataSearchHandler.MatchNameForPointQueries(query, packageNameEntry.Name);
              bool matchNameForNameQueries = NaivePackageMetadataSearchHandler.MatchNameForNameQueries(query, packageNameEntry.Name);
              if (flag1 || matchNameForNameQueries)
              {
                if (packageNameEntry is ILazyNuGetPackageVersionCounts packageVersionCounts1)
                {
                  INuGetPackageVersionCounts packageVersionCounts = packageVersionCounts1.Get();
                  if (packageVersionCounts.Count == 0)
                  {
                    telemetryCollector.SkippedByZeroCountPackages.Add(1);
                    continue;
                  }
                  if (IsSkipping() && !flag1)
                  {
                    int count = 0;
                    if (queryMatchesAllVersions)
                    {
                      count = packageVersionCounts.Count;
                    }
                    else
                    {
                      int num = packageVersionCounts.HasLatestVersion & queryMatchesLatestVersion ? 1 : 0;
                      bool flag2 = packageVersionCounts.HasAbsoluteLatestVersion & queryMatchesAbsoluteLatestVersion;
                      if (num != 0)
                        ++count;
                      if (flag2)
                        ++count;
                      if ((num & (flag2 ? 1 : 0)) != 0 && packageVersionCounts.LatestAndAbsoluteLatestAreSameVersion)
                        --count;
                    }
                    if (count != 0)
                    {
                      if (WouldBeSkipping(packageCount + 1, versionCount + count))
                      {
                        ++packageCount;
                        versionCount += count;
                        telemetryCollector.SkippedByCountVersions.Add(count);
                        continue;
                      }
                    }
                    else
                      continue;
                  }
                }
                MetadataDocument<INuGetMetadataEntry> documentForPackage = await sendInTheThisObject.GetMetadataDocumentForPackage((IPackageNameRequest) new PackageNameRequest<VssNuGetPackageName>((IFeedRequest) request, packageNameEntry.Name), telemetryCollector);
                if (documentForPackage != null)
                {
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  ImmutableList<NuGetSearchResultVersionSummary> immutableList = documentForPackage.Entries.Select<INuGetMetadataEntry, NuGetSearchResultVersionSummary>(NaivePackageMetadataSearchHandler.\u003C\u003EO.\u003C0\u003E__ToNuGetSearchResultVersionSummary ?? (NaivePackageMetadataSearchHandler.\u003C\u003EO.\u003C0\u003E__ToNuGetSearchResultVersionSummary = new Func<INuGetMetadataEntry, NuGetSearchResultVersionSummary>(NuGetPackageMetadataExtensions.ToNuGetSearchResultVersionSummary))).ToImmutableList<NuGetSearchResultVersionSummary>();
                  IReadOnlyList<NuGetSearchResultVersionSummary> resultVersionSummaryList1 = sendInTheThisObject.FilterVersionsStage1((IFeedRequest) request, query, (IEnumerable<NuGetSearchResultVersionSummary>) immutableList, telemetryCollector);
                  if (resultVersionSummaryList1.Any<NuGetSearchResultVersionSummary>())
                  {
                    LatestVersions latestVersions = sendInTheThisObject.latestVersionsFinder.FindLatestVersions((IReadOnlyCollection<NuGetSearchResultVersionSummary>) resultVersionSummaryList1);
                    if (!matchNameForNameQueries)
                      resultVersionSummaryList1 = (IReadOnlyList<NuGetSearchResultVersionSummary>) resultVersionSummaryList1.Where<NuGetSearchResultVersionSummary>((Func<NuGetSearchResultVersionSummary, bool>) (x => NaivePackageMetadataSearchHandler.MatchPointQuery(query, x))).ToList<NuGetSearchResultVersionSummary>();
                    IReadOnlyList<NuGetSearchResultVersionSummary> resultVersionSummaryList2 = (IReadOnlyList<NuGetSearchResultVersionSummary>) sendInTheThisObject.FilterVersionsStage2(resultVersionSummaryList1, query, latestVersions, telemetryCollector);
                    if (resultVersionSummaryList2.Any<NuGetSearchResultVersionSummary>())
                    {
                      ++packageCount;
                      Dictionary<VssNuGetPackageVersion, INuGetMetadataEntry> dictionary = documentForPackage.Entries.ToDictionary<INuGetMetadataEntry, VssNuGetPackageVersion>((Func<INuGetMetadataEntry, VssNuGetPackageVersion>) (x => x.PackageIdentity.Version));
                      foreach (NuGetSearchResultVersionSummary resultVersionSummary in (IEnumerable<NuGetSearchResultVersionSummary>) resultVersionSummaryList2)
                      {
                        if (!TookEnoughVersions())
                        {
                          ++versionCount;
                          if (!IsSkipping())
                          {
                            all.Add(dictionary[resultVersionSummary.PackageIdentity.Version].ToSearchResult((IEnumerable<NuGetSearchResultVersionSummary>) resultVersionSummaryList2, latestVersions));
                            if (query.ResultShape == NuGetSearchResultShape.Packages)
                              break;
                          }
                        }
                        else
                          break;
                      }
                    }
                  }
                }
              }
            }
            else
              break;
          }
          searchResultsInfo = new NuGetSearchResultsInfo((IEnumerable<NuGetSearchResult>) all, telemetryCollector);

          bool IsSkipping() => WouldBeSkipping(packageCount, versionCount);

          bool WouldBeSkipping(int _packageCount, int _versionCount)
          {
            switch (query.ResultShape)
            {
              case NuGetSearchResultShape.Packages:
                return _packageCount <= skip;
              case NuGetSearchResultShape.Versions:
                return _versionCount <= skip;
              default:
                throw new ArgumentOutOfRangeException();
            }
          }

          bool TookEnoughPackages()
          {
            switch (query.ResultShape)
            {
              case NuGetSearchResultShape.Packages:
                return packageCount >= skip + take;
              case NuGetSearchResultShape.Versions:
                return versionCount >= skip + take;
              default:
                throw new ArgumentOutOfRangeException();
            }
          }

          bool TookEnoughVersions()
          {
            switch (query.ResultShape)
            {
              case NuGetSearchResultShape.Packages:
                return false;
              case NuGetSearchResultShape.Versions:
                return versionCount >= skip + take;
              default:
                throw new ArgumentOutOfRangeException();
            }
          }
        }
      }
      telemetryCollector = (NuGetSearchTelemetryCollector) null;
      return searchResultsInfo;
    }

    private async Task<IReadOnlyList<IPackageNameEntry<VssNuGetPackageName>>> GetPackageNames(
      IFeedRequest request,
      NuGetSearchQuery query,
      NuGetSearchTelemetryCollector telemetryCollector)
    {
      IReadOnlyList<IPackageNameEntry<VssNuGetPackageName>> packageNames;
      using (telemetryCollector.GetNameList.BeginTiming())
      {
        NuGetSearchCategoryToggles queryToggles = NuGetSearchCategoryToggles.FromQuery(query);
        GetVersionCountsResult versionCounts = await this.namesService.GetVersionCounts(request, queryToggles);
        telemetryCollector.VersionCountsMetrics = versionCounts.Metrics;
        Dictionary<IPackageName, ImmutableList<ILazyNuGetPackageVersionCounts>> rawPackageNames = versionCounts.Packages.ToDictionary<ILazyNuGetPackageVersionCounts, IPackageName, ImmutableList<ILazyNuGetPackageVersionCounts>>((Func<ILazyNuGetPackageVersionCounts, IPackageName>) (x => (IPackageName) x.Name), (Func<ILazyNuGetPackageVersionCounts, ImmutableList<ILazyNuGetPackageVersionCounts>>) (x => ImmutableList.Create<ILazyNuGetPackageVersionCounts>(x)), (IEqualityComparer<IPackageName>) PackageNameComparer.NormalizedName);
        if (query.NonLocalVersionsAppearToExist && request.Feed.View == null)
        {
          string queryHint = string.Join(" ", (IEnumerable<string>) new HashSet<string>(query.NameQueries.Select<NuGetSearchPackageNameQuery, string>((Func<NuGetSearchPackageNameQuery, string>) (x => x.Value)).Concat<string>(query.PointQueries.Select<VssNuGetPackageIdentity, string>((Func<VssNuGetPackageIdentity, string>) (x => x.Name.NormalizedName)))));
          foreach (UpstreamSource input in request.Feed.GetSourcesForProtocol(request.Protocol))
          {
            using (telemetryCollector.GetUpstreamNameList.BeginTiming())
            {
              foreach (ILazyNuGetPackageVersionCounts package in (await (await this.upstreamClientFactory.Get(input)).GetVersionCounts(queryToggles, queryHint)).Packages)
              {
                ImmutableList<ILazyNuGetPackageVersionCounts> empty;
                if (!rawPackageNames.TryGetValue((IPackageName) package.Name, out empty))
                  empty = ImmutableList<ILazyNuGetPackageVersionCounts>.Empty;
                rawPackageNames[(IPackageName) package.Name] = empty.Add(package);
              }
            }
          }
          queryHint = (string) null;
        }
        List<NaivePackageMetadataSearchHandler.MergingLazyNuGetPackageVersionCounts> list = rawPackageNames.Values.Select<ImmutableList<ILazyNuGetPackageVersionCounts>, NaivePackageMetadataSearchHandler.MergingLazyNuGetPackageVersionCounts>((Func<ImmutableList<ILazyNuGetPackageVersionCounts>, NaivePackageMetadataSearchHandler.MergingLazyNuGetPackageVersionCounts>) (x => new NaivePackageMetadataSearchHandler.MergingLazyNuGetPackageVersionCounts((IEnumerable<ILazyNuGetPackageVersionCounts>) x))).OrderBy<NaivePackageMetadataSearchHandler.MergingLazyNuGetPackageVersionCounts, string>((Func<NaivePackageMetadataSearchHandler.MergingLazyNuGetPackageVersionCounts, string>) (x => x.Name.DisplayName), (IComparer<string>) StringComparer.OrdinalIgnoreCase).ToList<NaivePackageMetadataSearchHandler.MergingLazyNuGetPackageVersionCounts>();
        telemetryCollector.PackageNameEntries.Add(list.Count);
        packageNames = (IReadOnlyList<IPackageNameEntry<VssNuGetPackageName>>) list;
      }
      return packageNames;
    }

    private async Task<MetadataDocument<INuGetMetadataEntry>> GetMetadataDocumentForPackage(
      IPackageNameRequest packageNameRequest,
      NuGetSearchTelemetryCollector nuGetSearchTelemetryCollector)
    {
      using (nuGetSearchTelemetryCollector.GetPackageMetadata.BeginTiming())
      {
        MetadataDocument<INuGetMetadataEntry> statesDocumentAsync = await this.metadataService.GetPackageVersionStatesDocumentAsync(new PackageNameQuery<INuGetMetadataEntry>(packageNameRequest));
        if (statesDocumentAsync == null)
          return (MetadataDocument<INuGetMetadataEntry>) null;
        nuGetSearchTelemetryCollector.PackageMetadataEntries.Add(statesDocumentAsync.Entries.Count);
        return statesDocumentAsync;
      }
    }

    private IReadOnlyList<NuGetSearchResultVersionSummary> FilterVersionsStage1(
      IFeedRequest request,
      NuGetSearchQuery query,
      IEnumerable<NuGetSearchResultVersionSummary> inputVersions,
      NuGetSearchTelemetryCollector nuGetSearchTelemetryCollector)
    {
      using (nuGetSearchTelemetryCollector.FilterVersionsStage1.BeginTiming())
      {
        NuGetSearchCategoryToggles nuGetSearchCategoryToggles = NuGetSearchCategoryToggles.FromQuery(query);
        Guid viewId = this.viewIdExtractor.Convert(request);
        List<NuGetSearchResultVersionSummary> list = inputVersions.Where<NuGetSearchResultVersionSummary>((Func<NuGetSearchResultVersionSummary, bool>) (versionInfo => this.filterer.DoesVersionAppearToExist(nuGetSearchCategoryToggles, viewId, versionInfo))).OrderBy<NuGetSearchResultVersionSummary, IPackageVersion>((Func<NuGetSearchResultVersionSummary, IPackageVersion>) (versionInfo => (IPackageVersion) versionInfo.PackageIdentity.Version), (IComparer<IPackageVersion>) new ReverseVersionComparer<VssNuGetPackageVersion>()).ToList<NuGetSearchResultVersionSummary>();
        nuGetSearchTelemetryCollector.VersionsAfterFilterStage1.Add(list.Count);
        return (IReadOnlyList<NuGetSearchResultVersionSummary>) list;
      }
    }

    private List<NuGetSearchResultVersionSummary> FilterVersionsStage2(
      IReadOnlyList<NuGetSearchResultVersionSummary> inputVersions,
      NuGetSearchQuery query,
      LatestVersions latestVersions,
      NuGetSearchTelemetryCollector nuGetSearchTelemetryCollector)
    {
      using (nuGetSearchTelemetryCollector.FilterVersionsStage2.BeginTiming())
      {
        List<NuGetSearchResultVersionSummary> list = inputVersions.Where<NuGetSearchResultVersionSummary>((Func<NuGetSearchResultVersionSummary, bool>) (version => this.MatchVersion(query, latestVersions, version))).ToList<NuGetSearchResultVersionSummary>();
        nuGetSearchTelemetryCollector.VersionsAfterFilterStage2.Add(list.Count);
        return list;
      }
    }

    private static bool MatchNameForNameQueries(
      NuGetSearchQuery query,
      VssNuGetPackageName packageName)
    {
      return query.NameQueries.Any<NuGetSearchPackageNameQuery>((Func<NuGetSearchPackageNameQuery, bool>) (nameQuery => NaivePackageMetadataSearchHandler.MatchName(nameQuery, packageName)));
    }

    private static bool MatchNameForPointQueries(
      NuGetSearchQuery query,
      VssNuGetPackageName packageName)
    {
      return query.PointQueries.Any<VssNuGetPackageIdentity>((Func<VssNuGetPackageIdentity, bool>) (id => PackageNameComparer.NormalizedName.Equals((IPackageName) packageName, (IPackageName) id.Name)));
    }

    private static bool MatchName(
      NuGetSearchPackageNameQuery nameQuery,
      VssNuGetPackageName packageName)
    {
      switch (nameQuery.MatchType)
      {
        case NameMatchType.Exact:
          return packageName.DisplayName.Equals(nameQuery.Value, nameQuery.StringComparison);
        case NameMatchType.Substring:
          return packageName.DisplayName.IndexOf(nameQuery.Value, nameQuery.StringComparison) >= 0;
        case NameMatchType.Prefix:
          return packageName.DisplayName.StartsWith(nameQuery.Value, nameQuery.StringComparison);
        case NameMatchType.AllNames:
          return true;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private bool MatchVersion(
      NuGetSearchQuery query,
      LatestVersions latestVersions,
      NuGetSearchResultVersionSummary versionInfo)
    {
      return this.filterer.IsVersionSelectable(NuGetSearchCategoryToggles.FromQuery(query), versionInfo) && NaivePackageMetadataSearchHandler.MatchVersionCategories(query, latestVersions, versionInfo);
    }

    private static bool MatchPointQuery(
      NuGetSearchQuery query,
      NuGetSearchResultVersionSummary versionInfo)
    {
      return query.PointQueries.IsEmpty || query.PointQueries.Any<VssNuGetPackageIdentity>((Func<VssNuGetPackageIdentity, bool>) (id => id.Equals(versionInfo.PackageIdentity)));
    }

    private static bool MatchVersionCategories(
      NuGetSearchQuery query,
      LatestVersions latestVersions,
      NuGetSearchResultVersionSummary versionInfo)
    {
      return query.VersionCategories.Any<VersionCategorySelector>((Func<VersionCategorySelector, bool>) (category => NaivePackageMetadataSearchHandler.MatchOneVersionCategorySelector(category, latestVersions, versionInfo)));
    }

    [Conditional("DETAILED_MATCH_TRACE")]
    private static void DetailedTrace(string message)
    {
    }

    private static bool MatchOneVersionCategorySelector(
      VersionCategorySelector versionCategorySelector,
      LatestVersions latestVersions,
      NuGetSearchResultVersionSummary metadataEntry)
    {
      switch (versionCategorySelector)
      {
        case VersionCategorySelector.AllVersions:
          return true;
        case VersionCategorySelector.LatestVersion:
          return metadataEntry.PackageIdentity.Version.Equals(latestVersions.LatestVersion);
        case VersionCategorySelector.AbsoluteLatestVersion:
          return metadataEntry.PackageIdentity.Version.Equals(latestVersions.AbsoluteLatestVersion);
        default:
          throw new ArgumentOutOfRangeException(nameof (versionCategorySelector), (object) versionCategorySelector, (string) null);
      }
    }

    private class MergingLazyNuGetPackageVersionCounts : 
      ILazyNuGetPackageVersionCounts,
      IPackageNameEntry<VssNuGetPackageName>
    {
      private readonly ImmutableList<ILazyNuGetPackageVersionCounts> constituents;
      private readonly Lazy<ImmutableList<INuGetPackageVersionCounts>> resolvedConstituents;

      public MergingLazyNuGetPackageVersionCounts(
        IEnumerable<ILazyNuGetPackageVersionCounts> constituents)
      {
        this.constituents = constituents.ToImmutableList<ILazyNuGetPackageVersionCounts>();
        VssNuGetPackageName firstName = this.constituents.First<ILazyNuGetPackageVersionCounts>().Name;
        if (!this.constituents.TrueForAll((Predicate<ILazyNuGetPackageVersionCounts>) (x => PackageNameComparer.NormalizedName.Equals((IPackageName) firstName, (IPackageName) x.Name))))
          throw new ArgumentException("All constituents of MergingLazyNuGetPackageVersionCounts must have the same package name");
        this.resolvedConstituents = new Lazy<ImmutableList<INuGetPackageVersionCounts>>((Func<ImmutableList<INuGetPackageVersionCounts>>) (() => this.constituents.Select<ILazyNuGetPackageVersionCounts, INuGetPackageVersionCounts>((Func<ILazyNuGetPackageVersionCounts, INuGetPackageVersionCounts>) (x => x.Get())).ToImmutableList<INuGetPackageVersionCounts>()));
      }

      public VssNuGetPackageName Name => this.constituents.First<ILazyNuGetPackageVersionCounts>().Name;

      public DateTime LastUpdatedDateTime => this.constituents.Min<ILazyNuGetPackageVersionCounts, DateTime>((Func<ILazyNuGetPackageVersionCounts, DateTime>) (x => x.LastUpdatedDateTime));

      public INuGetPackageVersionCounts Get()
      {
        ImmutableList<INuGetPackageVersionCounts> source = this.resolvedConstituents.Value;
        return (INuGetPackageVersionCounts) new NuGetPackageVersionCounts(this.Name, this.LastUpdatedDateTime, source.Sum<INuGetPackageVersionCounts>((Func<INuGetPackageVersionCounts, int>) (x => x.Count)), source.Any<INuGetPackageVersionCounts>((Func<INuGetPackageVersionCounts, bool>) (x => x.HasLatestVersion)), source.Any<INuGetPackageVersionCounts>((Func<INuGetPackageVersionCounts, bool>) (x => x.HasAbsoluteLatestVersion)), source.Any<INuGetPackageVersionCounts>((Func<INuGetPackageVersionCounts, bool>) (x => x.LatestAndAbsoluteLatestAreSameVersion)));
      }
    }
  }
}
