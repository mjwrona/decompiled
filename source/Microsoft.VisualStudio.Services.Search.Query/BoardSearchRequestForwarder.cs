// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.BoardSearchRequestForwarder
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Board;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Board;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  public class BoardSearchRequestForwarder : 
    EntitySearchRequestForwarder<BoardSearchRequest, BoardSearchResponse>
  {
    public BoardSearchRequestForwarder(
      string searchPlatformConnectionString,
      string searchPlatformSettings,
      SearchOptions searchOptions,
      bool isOnPrem)
      : base(searchPlatformConnectionString, searchPlatformSettings, searchOptions, isOnPrem)
    {
    }

    public BoardSearchRequestForwarder(ISearchPlatform searchPlatform)
      : base(searchPlatform)
    {
    }

    public override BoardSearchResponse ForwardSearchRequest(
      IVssRequestContext requestContext,
      BoardSearchRequest searchRequest,
      IEnumerable<IndexInfo> indexInfo,
      IExpression scopeFiltersExpression,
      string requestId,
      DocumentContractType contractType,
      bool includeSuggestions,
      out IEnumerable<string> suggestions)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1083109, "Query Pipeline", "Query", nameof (ForwardSearchRequest));
      BoardSearchResponse boardSearchResponse = (BoardSearchResponse) null;
      suggestions = (IEnumerable<string>) null;
      try
      {
        this.ValidateInputParameters(searchRequest, scopeFiltersExpression);
        if (indexInfo == null || !indexInfo.Any<IndexInfo>())
        {
          boardSearchResponse = this.GetZeroResultResponseWithError(searchRequest, InfoCodes.IndexingNotStarted, out suggestions);
        }
        else
        {
          BoardSearchQueryTagger tagger;
          BoardSearchPlatformRequest searchQueryRequest = this.PreProcessSearchRequestAndFormPlatformRequest(requestContext, searchRequest, indexInfo, scopeFiltersExpression, requestId, out IDictionary<string, IEnumerable<string>> _, out tagger);
          if (tagger.Tags.Contains(EntitySearchQueryTagger.UnfilteredPrefixWildcard))
            boardSearchResponse = this.GetZeroResultResponseWithError(searchRequest, InfoCodes.PrefixWildcardQueryNotSupported, out suggestions);
          else if (searchQueryRequest != null)
          {
            Stopwatch stopwatch = Stopwatch.StartNew();
            boardSearchResponse = BoardSearchPlatformResponse.PrepareSearchResponse(this.SearchPlatform.Search<BoardVersionContract>(requestContext, (EntitySearchPlatformRequest) searchQueryRequest, EntityPluginsFactory.GetEntityType(requestContext, "Board"), this.GetIndexProvider()) as BoardSearchPlatformResponse, searchRequest);
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("BoardQueryE2EQueryTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
          }
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1083110, "Query Pipeline", "Query", nameof (ForwardSearchRequest));
      }
      return boardSearchResponse;
    }

    public override BoardSearchResponse GetZeroResultResponse(
      BoardSearchRequest searchRequest,
      out IEnumerable<string> suggestions)
    {
      suggestions = (IEnumerable<string>) new List<string>();
      BoardSearchResponse zeroResultResponse = new BoardSearchResponse();
      zeroResultResponse.Count = 0;
      zeroResultResponse.Results = Enumerable.Empty<BoardResult>();
      IDictionary<string, IEnumerable<string>> filters = searchRequest.Filters;
      zeroResultResponse.Facets = filters != null ? (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) filters.ToDictionary<KeyValuePair<string, IEnumerable<string>>, string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (fc => fc.Key), (Func<KeyValuePair<string, IEnumerable<string>>, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) (fc =>
      {
        IEnumerable<string> source = fc.Value;
        return source == null ? (IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>) null : source.Select<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>((Func<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>) (f => new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter(f, f, 0)));
      })) : (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) null;
      return zeroResultResponse;
    }

    protected virtual EntityIndexProvider<BoardVersionContract> GetIndexProvider() => (EntityIndexProvider<BoardVersionContract>) new BoardEntityIndexProvider<BoardVersionContract>();

    protected void ValidateInputParameters(
      BoardSearchRequest searchRequest,
      IExpression scopeFiltersExpression)
    {
      if (searchRequest == null)
        throw new ArgumentNullException(nameof (searchRequest));
      if (scopeFiltersExpression == null)
        throw new ArgumentNullException(nameof (scopeFiltersExpression));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1083111, "Query Pipeline", "Query", (Func<string>) (() => searchRequest.ToString()));
    }

    internal BoardSearchPlatformRequest PreProcessSearchRequestAndFormPlatformRequest(
      IVssRequestContext requestContext,
      BoardSearchRequest searchRequest,
      IEnumerable<IndexInfo> indexInfo,
      IExpression scopeFiltersExpression,
      string requestId,
      out IDictionary<string, IEnumerable<string>> searchFilters,
      out BoardSearchQueryTagger tagger)
    {
      BoardSearchPlatformRequest searchPlatformRequest1 = (BoardSearchPlatformRequest) null;
      IExpression expression = BoardSearchQueryTransformer.Correct(requestContext, BoardSearchQueryTransformer.Parse(searchRequest.SearchText));
      searchFilters = searchRequest.Filters ?? (IDictionary<string, IEnumerable<string>>) new Dictionary<string, IEnumerable<string>>();
      tagger = new BoardSearchQueryTagger(expression, searchFilters);
      tagger.Compute();
      tagger.Publish();
      if (!tagger.Tags.Contains(EntitySearchQueryTagger.UnfilteredPrefixWildcard))
      {
        SearchOptions platformSearchRequest = this.GetSearchOptionsForPlatformSearchRequest((EntitySearchRequest) searchRequest);
        BoardSearchPlatformRequest searchPlatformRequest2 = new BoardSearchPlatformRequest();
        searchPlatformRequest2.Options = platformSearchRequest;
        searchPlatformRequest2.RequestId = requestId;
        searchPlatformRequest2.IndexInfo = indexInfo;
        searchPlatformRequest2.QueryParseTree = expression;
        searchPlatformRequest2.SearchFilters = searchFilters;
        searchPlatformRequest2.ScopeFiltersExpression = scopeFiltersExpression;
        searchPlatformRequest2.SkipResults = searchRequest.Skip;
        searchPlatformRequest2.TakeResults = searchRequest.Top;
        searchPlatformRequest2.TakeAggResults = searchRequest.Top;
        searchPlatformRequest2.Fields = (IEnumerable<string>) new string[6]
        {
          "collectionName",
          "projectName",
          "projectId",
          "teamName",
          "teamId",
          "boardType"
        };
        searchPlatformRequest2.ContractType = DocumentContractType.BoardContract;
        searchPlatformRequest2.SortOptions = (IEnumerable<EntitySortOption>) (searchRequest.OrderBy ?? (IEnumerable<SortOption>) new List<SortOption>()).Select<SortOption, EntitySortOption>((Func<SortOption, EntitySortOption>) (i => new EntitySortOption()
        {
          Field = i.Field,
          SortOrder = (Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.SortOrder) i.SortOrder,
          SortOrderStr = i.SortOrderStr
        })).ToList<EntitySortOption>();
        searchPlatformRequest1 = searchPlatformRequest2;
      }
      return searchPlatformRequest1;
    }

    protected BoardSearchResponse GetZeroResultResponseWithError(
      BoardSearchRequest searchRequest,
      InfoCodes infoCode,
      out IEnumerable<string> suggestions)
    {
      BoardSearchResponse zeroResultResponse = this.GetZeroResultResponse(searchRequest, out suggestions);
      zeroResultResponse.InfoCode = (int) infoCode;
      return zeroResultResponse;
    }
  }
}
