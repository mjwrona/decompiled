// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Package.PackageSearchPlatformResponse
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Package;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Package
{
  public class PackageSearchPlatformResponse : EntitySearchPlatformResponse
  {
    public PackageSearchPlatformResponse(
      int totalMatches,
      IList<SearchHit> results,
      bool isTimedOut,
      IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> facets = null)
      : base(totalMatches, results, isTimedOut, facets)
    {
    }

    internal static PackageSearchResponseContent PrepareSearchResponse(
      PackageSearchPlatformResponse platformSearchResponse,
      PackageSearchPlatformResponse platformSearchLatestVersionResponse,
      PackageSearchRequest searchRequest)
    {
      FriendlyDictionary<string, PackageSearchHit> hitLookup = new FriendlyDictionary<string, PackageSearchHit>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (PackageSearchHit result in (IEnumerable<SearchHit>) platformSearchLatestVersionResponse.Results)
        hitLookup.Add(result.Source.PackageName, result);
      List<PackageResult> results = new List<PackageResult>((IEnumerable<PackageResult>) new PackageResult[platformSearchResponse.Results.Count]);
      Parallel.For(0, platformSearchResponse.Results.Count, (Action<int>) (i =>
      {
        PackageSearchHit result = platformSearchResponse.Results[i] as PackageSearchHit;
        results[i] = PackageSearchPlatformResponse.GetPackageSearchResult(result, hitLookup[result.Source.PackageName]);
      }));
      IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>> dictionary = (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) new FriendlyDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>();
      if (searchRequest.IncludeFacets)
        dictionary = (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) platformSearchResponse.Facets.ToDictionary<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>, string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>((Func<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>, string>) (fc => fc.Key), (Func<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) (fc =>
        {
          IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter> source = fc.Value;
          return source == null ? (IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>) null : source.Select<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>((Func<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>) (f => new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter(f.Name, f.Id, f.ResultCount)));
        }));
      int num = results.Count<PackageResult>();
      PackageSearchResponseContent searchResponseContent = new PackageSearchResponseContent();
      searchResponseContent.Count = num;
      searchResponseContent.Results = (IEnumerable<PackageResult>) results;
      searchResponseContent.Facets = dictionary;
      return searchResponseContent;
    }

    internal static PackageSearchResponseContent PrepareSearchResponse(
      PackageSearchPlatformResponse platformSearchResponse,
      PackageSearchRequest searchRequest)
    {
      int toExclusive = platformSearchResponse.Results.Count < searchRequest.Top ? platformSearchResponse.Results.Count : searchRequest.Top;
      List<PackageResult> results = new List<PackageResult>((IEnumerable<PackageResult>) new PackageResult[toExclusive]);
      Parallel.For(0, toExclusive, (Action<int>) (i =>
      {
        PackageSearchHit result = platformSearchResponse.Results[i] as PackageSearchHit;
        results[i] = PackageSearchPlatformResponse.GetPackageSearchResult(result);
      }));
      IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>> dictionary = (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) new FriendlyDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>();
      if (searchRequest.IncludeFacets)
        dictionary = (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) platformSearchResponse.Facets.Where<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>>((Func<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>, bool>) (i => ((IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>) i.Value.ToArray<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>()).Any<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>())).ToDictionary<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>, string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>((Func<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>, string>) (fc => fc.Key), (Func<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) (fc =>
        {
          IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter> source = fc.Value;
          return source == null ? (IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>) null : source.Select<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>((Func<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>) (f => new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter(f.Name, f.Id, f.ResultCount)));
        }));
      PackageSearchResponseContent searchResponseContent = new PackageSearchResponseContent();
      searchResponseContent.Count = platformSearchResponse.TotalMatches;
      searchResponseContent.Results = (IEnumerable<PackageResult>) results;
      searchResponseContent.Facets = dictionary;
      return searchResponseContent;
    }

    private static PackageResult GetPackageSearchResult(PackageSearchHit hit)
    {
      List<FeedInfo> feeds = new List<FeedInfo>();
      foreach (PackageFeed feed in hit.Feeds)
        feeds.Add(new FeedInfo(feed.CollectionId, feed.CollectionName, feed.FeedId, feed.FeedName, PackageSearchPlatformResponse.GetPackageUrl(hit, feed), feed.LatestVersion, feed.LatestVersion, feed.Views.Select<PackageViewInfo, string>((Func<PackageViewInfo, string>) (i => i.ViewName))));
      return new PackageResult(hit.Source.PackageName, hit.Source.PackageId, hit.Source.Description, hit.Source.Protocol, hit.Hits.Select<Highlight, PackageHit>((Func<Highlight, PackageHit>) (i => new PackageHit(i.Field, i.Highlights))), (IEnumerable<FeedInfo>) feeds);
    }

    private static PackageResult GetPackageSearchResult(
      PackageSearchHit hit,
      PackageSearchHit latestVersionHit)
    {
      FriendlyDictionary<string, PackageFeed> collection = new FriendlyDictionary<string, PackageFeed>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      collection.AddRange<KeyValuePair<string, PackageFeed>, FriendlyDictionary<string, PackageFeed>>(hit.Feeds.Select<PackageFeed, KeyValuePair<string, PackageFeed>>((Func<PackageFeed, KeyValuePair<string, PackageFeed>>) (s => new KeyValuePair<string, PackageFeed>(PackageSearchPlatformResponse.GetJoinKey(s), s))));
      List<FeedInfo> feeds = new List<FeedInfo>();
      foreach (PackageFeed feed in latestVersionHit.Feeds)
      {
        PackageFeed packageFeed = collection[PackageSearchPlatformResponse.GetJoinKey(feed)];
        feeds.Add(new FeedInfo(feed.CollectionId, feed.CollectionName, feed.FeedId, feed.FeedName, PackageSearchPlatformResponse.GetPackageUrl(hit, packageFeed), feed.LatestVersion, packageFeed.LatestVersion, feed.Views.Select<PackageViewInfo, string>((Func<PackageViewInfo, string>) (i => i.ViewName))));
      }
      return new PackageResult(hit.Source.PackageName, hit.Source.PackageId, hit.Source.Description, hit.Source.Protocol, hit.Hits.Select<Highlight, PackageHit>((Func<Highlight, PackageHit>) (i => new PackageHit(i.Field, i.Highlights))), (IEnumerable<FeedInfo>) feeds);
    }

    private static string GetPackageUrl(PackageSearchHit hit, PackageFeed packageFeed) => FormattableString.Invariant(FormattableStringFactory.Create("{0}_packaging?feed={1}&_a=package&package={2}&version={3}&protocolType={4}", (object) packageFeed.CollectionUrl, (object) packageFeed.FeedName, (object) hit.Source.PackageName, (object) Uri.EscapeDataString(packageFeed.LatestVersion), (object) hit.Source.Protocol));

    private static string GetJoinKey(PackageFeed pkgFeed) => FormattableString.Invariant(FormattableStringFactory.Create("{0}_{1}", (object) pkgFeed.FeedId, (object) pkgFeed.PackageId));
  }
}
