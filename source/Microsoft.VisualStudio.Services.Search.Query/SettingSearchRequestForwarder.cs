// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.SettingSearchRequestForwarder
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
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Setting;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Setting;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  public class SettingSearchRequestForwarder : 
    EntitySearchRequestForwarder<SettingSearchRequest, SettingSearchResponse>
  {
    public SettingSearchRequestForwarder(
      string searchPlatformConnectionString,
      string searchPlatformSettings,
      SearchOptions searchOptions,
      bool isOnPrem)
      : base(searchPlatformConnectionString, searchPlatformSettings, searchOptions, isOnPrem)
    {
    }

    public SettingSearchRequestForwarder(ISearchPlatform searchPlatform)
      : base(searchPlatform)
    {
    }

    public override SettingSearchResponse ForwardSearchRequest(
      IVssRequestContext requestContext,
      SettingSearchRequest searchRequest,
      IEnumerable<IndexInfo> indexInfo,
      IExpression scopeFiltersExpression,
      string requestId,
      DocumentContractType contractType,
      bool includeSuggestions,
      out IEnumerable<string> suggestions)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1083129, "Query Pipeline", "Query", nameof (ForwardSearchRequest));
      SettingSearchResponse settingSearchResponse = (SettingSearchResponse) null;
      suggestions = (IEnumerable<string>) null;
      try
      {
        this.ValidateInputParameters(searchRequest);
        if (indexInfo == null || !indexInfo.Any<IndexInfo>())
        {
          settingSearchResponse = this.GetZeroResultResponseWithError(searchRequest, InfoCodes.IndexingNotStarted, out suggestions);
        }
        else
        {
          SettingSearchQueryTagger tagger;
          SettingSearchPlatformRequest searchQueryRequest = this.PreProcessSearchRequestAndFormPlatformRequest(requestContext, searchRequest, indexInfo, scopeFiltersExpression, requestId, contractType, out IDictionary<string, IEnumerable<string>> _, out tagger);
          if (tagger.Tags.Contains(EntitySearchQueryTagger.UnfilteredPrefixWildcard))
            settingSearchResponse = this.GetZeroResultResponseWithError(searchRequest, InfoCodes.PrefixWildcardQueryNotSupported, out suggestions);
          else if (searchQueryRequest != null)
          {
            Stopwatch stopwatch = Stopwatch.StartNew();
            settingSearchResponse = SettingSearchPlatformResponse.PrepareSearchResponse(this.SearchPlatform.Search<SettingContract>(requestContext, (EntitySearchPlatformRequest) searchQueryRequest, EntityPluginsFactory.GetEntityType(requestContext, "Setting"), this.GetIndexProvider()) as SettingSearchPlatformResponse, searchRequest);
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("SettingQeuryE2EQueryTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
          }
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1083130, "Query Pipeline", "Query", nameof (ForwardSearchRequest));
      }
      return settingSearchResponse;
    }

    public override SettingSearchResponse GetZeroResultResponse(
      SettingSearchRequest searchRequest,
      out IEnumerable<string> suggestions)
    {
      suggestions = (IEnumerable<string>) new List<string>();
      SettingSearchResponse zeroResultResponse = new SettingSearchResponse();
      zeroResultResponse.Count = 0;
      zeroResultResponse.Results = Enumerable.Empty<SettingResult>();
      IDictionary<string, IEnumerable<string>> filters = searchRequest.Filters;
      zeroResultResponse.Facets = filters != null ? (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) filters.ToDictionary<KeyValuePair<string, IEnumerable<string>>, string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (fc => fc.Key), (Func<KeyValuePair<string, IEnumerable<string>>, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) (fc =>
      {
        IEnumerable<string> source = fc.Value;
        return source == null ? (IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>) null : source.Select<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>((Func<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>) (f => new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter(f, f, 0)));
      })) : (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) null;
      return zeroResultResponse;
    }

    protected virtual EntityIndexProvider<SettingContract> GetIndexProvider() => (EntityIndexProvider<SettingContract>) new SettingEntityIndexProvider<SettingContract>();

    protected void ValidateInputParameters(SettingSearchRequest searchRequest)
    {
      if (searchRequest == null)
        throw new ArgumentNullException(nameof (searchRequest));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1083131, "Query Pipeline", "Query", (Func<string>) (() => searchRequest.ToString()));
    }

    protected SettingSearchResponse GetZeroResultResponseWithError(
      SettingSearchRequest searchRequest,
      InfoCodes infocode,
      out IEnumerable<string> suggestions)
    {
      SettingSearchResponse zeroResultResponse = this.GetZeroResultResponse(searchRequest, out suggestions);
      zeroResultResponse.InfoCode = (int) infocode;
      return zeroResultResponse;
    }

    internal SettingSearchPlatformRequest PreProcessSearchRequestAndFormPlatformRequest(
      IVssRequestContext requestContext,
      SettingSearchRequest searchRequest,
      IEnumerable<IndexInfo> indexInfo,
      IExpression scopeFilterExpression,
      string requestId,
      DocumentContractType contractType,
      out IDictionary<string, IEnumerable<string>> searchFilters,
      out SettingSearchQueryTagger tagger)
    {
      SettingSearchPlatformRequest searchPlatformRequest1 = (SettingSearchPlatformRequest) null;
      IExpression expression = SettingSearchQueryTransformer.Correct(requestContext, SettingSearchQueryTransformer.Parse(searchRequest.SearchText));
      searchFilters = searchRequest.Filters ?? (IDictionary<string, IEnumerable<string>>) new Dictionary<string, IEnumerable<string>>();
      tagger = new SettingSearchQueryTagger(expression, searchFilters);
      tagger.Compute();
      tagger.Publish();
      if (!tagger.Tags.Contains(EntitySearchQueryTagger.UnfilteredPrefixWildcard))
      {
        SearchOptions platformSearchRequest = this.GetSearchOptionsForPlatformSearchRequest((EntitySearchRequest) searchRequest);
        SettingSearchPlatformRequest searchPlatformRequest2 = new SettingSearchPlatformRequest();
        searchPlatformRequest2.Options = platformSearchRequest;
        searchPlatformRequest2.RequestId = requestId;
        searchPlatformRequest2.IndexInfo = indexInfo;
        searchPlatformRequest2.QueryParseTree = expression;
        searchPlatformRequest2.SearchFilters = searchFilters;
        searchPlatformRequest2.ScopeFiltersExpression = scopeFilterExpression;
        searchPlatformRequest2.SkipResults = searchRequest.Skip;
        searchPlatformRequest2.TakeResults = searchRequest.Top;
        searchPlatformRequest2.Fields = (IEnumerable<string>) new string[6]
        {
          "description",
          "routeId",
          "routeParameterMapping",
          "scope",
          "title",
          "icon"
        };
        searchPlatformRequest2.ContractType = contractType;
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
  }
}
