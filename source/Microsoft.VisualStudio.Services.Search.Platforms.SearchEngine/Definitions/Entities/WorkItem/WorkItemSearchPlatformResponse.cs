// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem.WorkItemSearchPlatformResponse
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem
{
  public class WorkItemSearchPlatformResponse : EntitySearchPlatformResponse
  {
    public WorkItemSearchPlatformResponse(
      int totalMatches,
      IList<SearchHit> results,
      bool isTimedOut,
      IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> facets)
      : base(totalMatches, results, isTimedOut, facets)
    {
    }

    public static WorkItemSearchResponse PrepareSearchResponse(
      WorkItemSearchPlatformResponse platformSearchResponse,
      WorkItemSearchRequest searchRequest)
    {
      if (platformSearchResponse == null)
        throw new ArgumentNullException(nameof (platformSearchResponse));
      WorkItemResults searchResults = WorkItemSearchPlatformResponse.GetSearchResults(platformSearchResponse.TotalMatches, platformSearchResponse.Results);
      IEnumerable<FilterCategory> filterCategories = Enumerable.Empty<FilterCategory>();
      if (searchRequest.SummarizedHitCountsNeeded)
        filterCategories = platformSearchResponse.Facets.Where<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>>((Func<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>, bool>) (f => f.Value.Any<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>())).Select<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>, FilterCategory>((Func<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>, FilterCategory>) (f => new FilterCategory()
        {
          Name = f.Key,
          Filters = f.Value
        }));
      WorkItemSearchResponse itemSearchResponse = new WorkItemSearchResponse();
      itemSearchResponse.Query = searchRequest;
      itemSearchResponse.Results = searchResults;
      itemSearchResponse.FilterCategories = filterCategories;
      return itemSearchResponse;
    }

    private static WorkItemResults GetSearchResults(int totalMatches, IList<SearchHit> platformHits)
    {
      WorkItemResult[] searchHits = platformHits != null ? new WorkItemResult[platformHits.Count] : throw new ArgumentNullException(nameof (platformHits));
      Parallel.For(0, platformHits.Count, (Action<int>) (i =>
      {
        WorkItemSearchHit platformHit = platformHits[i] as WorkItemSearchHit;
        searchHits[i] = WorkItemSearchHit.CreateWorkItemResult(platformHit);
      }));
      return new WorkItemResults()
      {
        Count = totalMatches,
        Values = (IEnumerable<WorkItemResult>) searchHits
      };
    }
  }
}
