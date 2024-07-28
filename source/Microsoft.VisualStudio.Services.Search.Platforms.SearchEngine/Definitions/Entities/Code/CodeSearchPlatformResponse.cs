// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code.CodeSearchPlatformResponse
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code
{
  public class CodeSearchPlatformResponse : EntitySearchPlatformResponse
  {
    private CodeFileContract m_contract;

    public CodeSearchPlatformResponse(
      CodeFileContract contract,
      int totalMatches,
      IList<SearchHit> results,
      bool isTimedOut,
      IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> facets)
      : base(totalMatches, results, isTimedOut, facets)
    {
      this.m_contract = contract;
    }

    public CodeSearchPlatformResponse(
      CodeFileContract contract,
      int totalMatches,
      IList<SearchHit> results,
      string scrollId,
      bool isTimedOut,
      IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> facets)
      : base(totalMatches, results, scrollId, isTimedOut, facets)
    {
      this.m_contract = contract;
    }

    public static CodeQueryResponse PrepareSearchResponse(
      CodeSearchPlatformResponse platformSearchResponse,
      SearchQuery searchQuery)
    {
      CodeResults codeResults = platformSearchResponse != null ? platformSearchResponse.GetSearchResults() : throw new ArgumentNullException(nameof (platformSearchResponse));
      IEnumerable<FilterCategory> filterCategories = Enumerable.Empty<FilterCategory>();
      if (searchQuery.SummarizedHitCountsNeeded)
        filterCategories = platformSearchResponse.GetFacets();
      CodeQueryResponse codeQueryResponse = new CodeQueryResponse();
      codeQueryResponse.Query = searchQuery;
      codeQueryResponse.Results = codeResults;
      codeQueryResponse.FilterCategories = filterCategories;
      return codeQueryResponse;
    }

    public static ScrollSearchResponse PrepareSearchResponse(
      CodeSearchPlatformResponse platformSearchResponse)
    {
      CodeSearchResponse codeSearchResponse = platformSearchResponse != null ? (CodeSearchResponse) platformSearchResponse.GetScrollSearchResults() : throw new ArgumentNullException(nameof (platformSearchResponse));
      IEnumerable<FilterCategory> facets = platformSearchResponse.GetFacets();
      ScrollSearchResponse scrollSearchResponse = new ScrollSearchResponse();
      scrollSearchResponse.Results = codeSearchResponse.Results;
      scrollSearchResponse.Facets = CodeSearchResponseConvertor.UpdateFilterCategories(facets);
      scrollSearchResponse.ScrollId = platformSearchResponse.ScrollId;
      scrollSearchResponse.Count = platformSearchResponse.TotalMatches;
      return scrollSearchResponse;
    }

    public override int GetTotalHighlights()
    {
      int totalHighlights = 0;
      if (this.Results.IsNullOrEmpty<SearchHit>())
        return totalHighlights;
      foreach (CodeFileContract.CodeSearchHit result in (IEnumerable<SearchHit>) this.Results)
      {
        ICollection<IEnumerable<Hit>> values = result.Matches.Values;
        if (!values.IsNullOrEmpty<IEnumerable<Hit>>())
        {
          foreach (IEnumerable<Hit> source in (IEnumerable<IEnumerable<Hit>>) values)
            totalHighlights += source.Count<Hit>();
        }
      }
      return totalHighlights;
    }

    private CodeResults GetSearchResults()
    {
      List<Microsoft.VisualStudio.Services.Search.WebApi.Legacy.CodeResult> searchHits = new List<Microsoft.VisualStudio.Services.Search.WebApi.Legacy.CodeResult>((IEnumerable<Microsoft.VisualStudio.Services.Search.WebApi.Legacy.CodeResult>) new Microsoft.VisualStudio.Services.Search.WebApi.Legacy.CodeResult[this.Results.Count]);
      Parallel.For(0, this.Results.Count, (Action<int>) (i =>
      {
        CodeFileContract.CodeSearchHit result = this.Results[i] as CodeFileContract.CodeSearchHit;
        searchHits[i] = CodeFileContract.CodeSearchHit.CreateCodeResult(this.m_contract, result, "Git");
      }));
      return new CodeResults(this.TotalMatches, (IEnumerable<Microsoft.VisualStudio.Services.Search.WebApi.Legacy.CodeResult>) searchHits);
    }

    private ScrollSearchResponse GetScrollSearchResults()
    {
      List<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult> searchHits = new List<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult>((IEnumerable<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult>) new Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult[this.Results.Count]);
      Parallel.For(0, this.Results.Count, (Action<int>) (i =>
      {
        CodeFileContract.CodeSearchHit result = this.Results[i] as CodeFileContract.CodeSearchHit;
        searchHits[i] = CodeFileContract.CodeSearchHit.CreateScrollCodeResult(this.m_contract, result, "Git");
      }));
      ScrollSearchResponse scrollSearchResults = new ScrollSearchResponse();
      scrollSearchResults.Count = this.TotalMatches;
      scrollSearchResults.Results = (IEnumerable<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult>) searchHits;
      return scrollSearchResults;
    }

    private IEnumerable<FilterCategory> GetFacets() => DocumentContractTypeExtension.IsValidCodeDocumentContractType(this.m_contract.ContractType) ? this.Facets.Select<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>, FilterCategory>((Func<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>, FilterCategory>) (f => new FilterCategory()
    {
      Name = f.Key == "CollectionFilters" ? "AccountFilters" : f.Key,
      Filters = f.Value
    })) : this.Facets.Select<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>, FilterCategory>((Func<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>, FilterCategory>) (f => new FilterCategory()
    {
      Name = f.Key,
      Filters = f.Value
    }));
  }
}
