// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.WorkItemSearchQueryForwarder
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
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.Rules;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders;
using Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  public class WorkItemSearchQueryForwarder : 
    EntitySearchQueryForwarder<WorkItemSearchRequest, WorkItemSearchResponse>
  {
    public WorkItemSearchQueryForwarder(
      string searchPlatformConnectionString,
      string searchPlatformSettings,
      SearchOptions searchOptions,
      bool isOnPrem)
      : base(searchPlatformConnectionString, searchPlatformSettings, searchOptions, isOnPrem)
    {
    }

    public WorkItemSearchQueryForwarder(ISearchPlatform searchPlatform)
      : base(searchPlatform)
    {
    }

    public override WorkItemSearchResponse ForwardSearchRequest(
      IVssRequestContext requestContext,
      WorkItemSearchRequest searchQuery,
      IEnumerable<IndexInfo> indexInfo,
      IExpression scopeFiltersExpression,
      string requestId,
      DocumentContractType contractType)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081313, "Query Pipeline", "Query", nameof (ForwardSearchRequest));
      WorkItemSearchResponse searchResponse;
      try
      {
        if (searchQuery == null)
          throw new ArgumentNullException(nameof (searchQuery));
        if (scopeFiltersExpression == null)
          throw new ArgumentNullException(nameof (scopeFiltersExpression));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1083157, "Query Pipeline", "Query", (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SearchText : {0}", (object) searchQuery.SearchText)));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1081314, "Query Pipeline", "Query", (Func<string>) (() => searchQuery.ToString()));
        ISearchQueryTransformer transformerInstance = SearchQueryTransformerFactory.GetTransformerInstance(requestContext, EntityPluginsFactory.GetEntityType(requestContext, "WorkItem"));
        IExpression expression = transformerInstance.CorrectQuery(requestContext, transformerInstance.ParseSearchText(searchQuery.SearchText));
        IDictionary<string, IEnumerable<string>> dictionary = QueryForwarderHelpers.ConvertToDictionary(searchQuery.Filters);
        WorkItemSearchQueryTagger searchQueryTagger = new WorkItemSearchQueryTagger(expression, dictionary);
        searchQueryTagger.Compute();
        searchQueryTagger.Publish();
        if (requestContext.IsFeatureEnabled("Search.Server.WorkItem.RecentActivityTracking"))
        {
          Guid userId = requestContext.GetUserIdentity().Id;
          Guid projectId;
          if (requestContext.RootContext.Items[Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Constants.WorkItemRecencyData.ProjectId] != null && Guid.TryParse(requestContext.RootContext.Items[Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Constants.WorkItemRecencyData.ProjectId].ToString(), out projectId) && projectId != Guid.Empty)
          {
            RecencyData recentActivities = requestContext.GetService<RecentActivityDataProviderService>().GetRecentActivities(requestContext, userId, projectId);
            requestContext.RootContext.Items[Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Constants.WorkItemRecencyData.RecentWorkItemIds] = (object) recentActivities.workItemIds;
            requestContext.RootContext.Items[Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Constants.WorkItemRecencyData.RecentAreaIds] = (object) recentActivities.areaIds;
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1081315, "Query Pipeline", "Query", (Func<string>) (() =>
            {
              List<string> values1 = new List<string>();
              List<string> values2 = requestContext.RootContext.Items[Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Constants.WorkItemRecencyData.RecentWorkItemIds] as List<string>;
              List<string> values3 = requestContext.RootContext.Items[Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Constants.WorkItemRecencyData.RecentAreaIds] as List<string>;
              values1.Add(FormattableString.Invariant(FormattableStringFactory.Create("[UserId: {0}]", (object) userId)));
              values1.Add(FormattableString.Invariant(FormattableStringFactory.Create("[ProjectId: {0}]", (object) projectId)));
              values1.Add(FormattableString.Invariant(FormattableStringFactory.Create("[WorkItemIds:{0}]", values2 == null ? (object) "" : (object) string.Join(", ", (IEnumerable<string>) values2))));
              values1.Add(FormattableString.Invariant(FormattableStringFactory.Create("[AreaIds: {0}]", values3 == null ? (object) "" : (object) string.Join(", ", (IEnumerable<string>) values3))));
              return string.Join(",", (IEnumerable<string>) values1);
            }));
          }
          else
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1081315, "Query Pipeline", "Query", (Func<string>) (() => FormattableString.Invariant(FormattableStringFactory.Create("Undefined or invalid project id {0}", (object) (requestContext.RootContext.Items[Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Constants.WorkItemRecencyData.ProjectId] == null ? string.Empty : requestContext.RootContext.Items[Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Constants.WorkItemRecencyData.ProjectId].ToString())))));
        }
        this.UpdateRelevanceRules(requestContext, expression, EntityPluginsFactory.GetEntityType(requestContext, "WorkItem"), contractType, new Func<IVssRequestContext, IEnumerable<RelevanceRule>, IEnumerable<RelevanceRule>>(transformerInstance.CorrectRelevanceRules));
        IEnumerable<string> inlineSearchFilters = WorkItemQueryInlineFiltersExtractor.Extract(expression);
        List<ErrorData> errorDataList = new List<ErrorData>();
        if (searchQueryTagger.Tags.Contains("UnfilteredPrefixWildcard"))
          errorDataList.Add(new ErrorData()
          {
            ErrorCode = "PrefixWildcardQueryNotSupported",
            ErrorType = ErrorType.Warning
          });
        if (errorDataList.Count > 0)
        {
          searchResponse = this.GetZeroResultResponse(searchQuery);
          foreach (ErrorData errorData in errorDataList)
            searchResponse.AddError(errorData);
        }
        else if (indexInfo == null || !indexInfo.Any<IndexInfo>())
        {
          searchResponse = this.GetZeroResultResponse(searchQuery);
          searchResponse.AddError(new ErrorData()
          {
            ErrorCode = "IndexingNotStarted",
            ErrorType = ErrorType.Warning
          });
        }
        else
        {
          SearchOptions platformSearchRequest = this.GetSearchOptionsForPlatformSearchRequest((EntitySearchQuery) searchQuery);
          WorkItemSearchPlatformRequest searchQueryRequest = new WorkItemSearchPlatformRequest(requestContext, platformSearchRequest, requestId, indexInfo, expression, dictionary, scopeFiltersExpression, searchQuery, inlineSearchFilters);
          Stopwatch stopwatch = Stopwatch.StartNew();
          WorkItemSearchPlatformResponse platformSearchResponse = this.SearchPlatform.Search<WorkItemContract>(requestContext, (EntitySearchPlatformRequest) searchQueryRequest, EntityPluginsFactory.GetEntityType(requestContext, "WorkItem"), (EntityIndexProvider<WorkItemContract>) new WorkItemEntityIndexProvider<WorkItemContract>(searchQuery.IsInstantSearch)) as WorkItemSearchPlatformResponse;
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NonFuzzyE2EPlatformQueryTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
          WorkItemSearchRequest searchRequest = searchQuery;
          searchResponse = WorkItemSearchPlatformResponse.PrepareSearchResponse(platformSearchResponse, searchRequest);
          this.SetWarningInReponseForFacetingAskWhenFacetingIsOffAtHostLevel((EntitySearchQuery) searchQuery, (EntitySearchResponse) searchResponse);
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081312, "Query Pipeline", "Query", nameof (ForwardSearchRequest));
      }
      return searchResponse;
    }

    public override WorkItemSearchResponse GetZeroResultResponse(WorkItemSearchRequest searchRequest)
    {
      WorkItemSearchResponse zeroResultResponse = new WorkItemSearchResponse();
      zeroResultResponse.Query = searchRequest;
      zeroResultResponse.Results = new WorkItemResults()
      {
        Count = 0,
        Values = Enumerable.Empty<WorkItemResult>()
      };
      zeroResultResponse.FilterCategories = searchRequest.Filters.Where<SearchFilter>((Func<SearchFilter, bool>) (f => !f.Name.Equals(Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemAreaPath))).Select<SearchFilter, FilterCategory>((Func<SearchFilter, FilterCategory>) (sqf => new FilterCategory()
      {
        Name = sqf.Name,
        Filters = sqf.Values.Select<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>((Func<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>) (v => new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter(v, v, 0, true)))
      }));
      return zeroResultResponse;
    }
  }
}
