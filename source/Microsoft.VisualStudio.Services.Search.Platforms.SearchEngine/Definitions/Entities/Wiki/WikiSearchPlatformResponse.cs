// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki.WikiSearchPlatformResponse
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki
{
  public class WikiSearchPlatformResponse : EntitySearchPlatformResponse
  {
    public WikiSearchPlatformResponse(
      int totalMatches,
      IList<SearchHit> results,
      bool isTimedOut,
      IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> facets = null)
      : base(totalMatches, results, isTimedOut, facets)
    {
    }

    public static WikiQueryResponse PrepareSearchResponse(
      WikiSearchPlatformResponse platformSearchResponse,
      EntitySearchSuggestPlatformResponse platformSuggestResponse,
      WikiSearchQuery searchQuery)
    {
      if (platformSearchResponse == null)
        throw new ArgumentNullException(nameof (platformSearchResponse));
      WikiResults searchResults = WikiSearchPlatformResponse.GetSearchResults(platformSearchResponse.TotalMatches, platformSearchResponse.Results);
      IEnumerable<FilterCategory> filterCategories = Enumerable.Empty<FilterCategory>();
      if (searchQuery.SummarizedHitCountsNeeded)
        filterCategories = platformSearchResponse.Facets.Where<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>>((Func<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>, bool>) (f => f.Value.Any<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>())).Select<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>, FilterCategory>((Func<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>, FilterCategory>) (f => new FilterCategory()
        {
          Name = f.Key,
          Filters = f.Value
        }));
      IEnumerable<string> suggestResults = WikiSearchPlatformResponse.GetSuggestResults(platformSuggestResponse);
      WikiQueryResponse wikiQueryResponse = new WikiQueryResponse();
      wikiQueryResponse.Query = searchQuery;
      wikiQueryResponse.Results = searchResults;
      wikiQueryResponse.FilterCategories = filterCategories;
      wikiQueryResponse.Suggestions = suggestResults;
      return wikiQueryResponse;
    }

    private static WikiResults GetSearchResults(int totalMatches, IList<SearchHit> platformHits)
    {
      List<WikiResult> searchHits = platformHits != null ? new List<WikiResult>((IEnumerable<WikiResult>) new WikiResult[platformHits.Count]) : throw new ArgumentNullException(nameof (platformHits));
      Parallel.For(0, platformHits.Count, (Action<int>) (i =>
      {
        WikiSearchHit platformHit = platformHits[i] as WikiSearchHit;
        searchHits[i] = WikiSearchHit.CreateWikiSearchResult(platformHit);
      }));
      return new WikiResults(totalMatches, (IEnumerable<WikiResult>) searchHits);
    }

    private static IEnumerable<string> GetSuggestResults(
      EntitySearchSuggestPlatformResponse platformSuggestResponse)
    {
      List<string> suggestResults = new List<string>();
      foreach (SuggestOption suggestion in platformSuggestResponse.Suggestions)
        suggestResults.Add(suggestion.Text);
      return (IEnumerable<string>) suggestResults;
    }
  }
}
