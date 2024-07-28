// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders.WorkItemEntityIndexProvider`1
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Aggregation;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem;
using Nest;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders
{
  internal sealed class WorkItemEntityIndexProvider<T> : EntityIndexProvider<T> where T : class
  {
    [StaticSafe]
    private static readonly FriendlyDictionary<DocumentContractType, AbstractSearchDocumentContract> s_documentContractMapping = new FriendlyDictionary<DocumentContractType, AbstractSearchDocumentContract>()
    {
      [DocumentContractType.WorkItemContract] = (AbstractSearchDocumentContract) new WorkItemContract()
    };
    private bool m_isInstantSearch;

    public WorkItemEntityIndexProvider()
    {
    }

    public WorkItemEntityIndexProvider(bool isInstantSearch) => this.m_isInstantSearch = isInstantSearch;

    public override void BuildSearchComponents(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request,
      string rawQueryString,
      string rawFilterString,
      out Func<QueryContainerDescriptor<T>, QueryContainer> query,
      out Func<QueryContainerDescriptor<T>, QueryContainer> filter,
      out Func<AggregationContainerDescriptor<T>, AggregationContainerDescriptor<T>> aggregations,
      out Func<HighlightDescriptor<T>, IHighlight> highlight,
      out Func<IVssRequestContext, SortDescriptor<T>> sort)
    {
      WorkItemSearchPlatformRequest searchPlatformRequest = (WorkItemSearchPlatformRequest) request;
      query = this.GetFilteredQuery(rawQueryString, rawFilterString);
      filter = new Func<QueryContainerDescriptor<T>, QueryContainer>(new WorkItemFilterBuilder(requestContext, request.SearchFilters).Filters<T>);
      highlight = new WorkItemHighlightBuilder(request.QueryParseTree, request.Options, searchPlatformRequest.InlineSearchFilters, this.m_isInstantSearch).Highlight<T>(requestContext);
      if (!this.m_isInstantSearch)
      {
        aggregations = new Func<AggregationContainerDescriptor<T>, AggregationContainerDescriptor<T>>(new WorkItemAggregationBuilder(requestContext, searchPlatformRequest.SearchFilters, request.Options.HasFlag((Enum) SearchOptions.Faceting)).Aggregates<T>);
        sort = new WorkItemSortBuilder(searchPlatformRequest.SortOptions).Sort<T>();
      }
      else
      {
        aggregations = (Func<AggregationContainerDescriptor<T>, AggregationContainerDescriptor<T>>) null;
        sort = (Func<IVssRequestContext, SortDescriptor<T>>) null;
      }
    }

    public override void BuildCountComponents(
      IVssRequestContext requestContext,
      ResultsCountPlatformRequest request,
      string rawQueryString,
      string rawFilterString,
      out Func<QueryContainerDescriptor<T>, QueryContainer> query)
    {
      query = this.GetFilteredQuery(rawQueryString, rawFilterString);
    }

    public override void BuildSuggestComponents(
      IVssRequestContext requestContext,
      EntitySearchSuggestPlatformRequest suggestRequest,
      string rawFilterString,
      out Func<SuggestContainerDescriptor<T>, SuggestContainerDescriptor<T>> suggest)
    {
      suggest = new DefaultPhraseSuggesterBuilder().Suggest<T>();
    }

    public override EntitySearchPlatformResponse DefaultPlatformResponse(
      EntitySearchPlatformRequest searchRequest)
    {
      return (EntitySearchPlatformResponse) new WorkItemSearchPlatformResponse(0, (IList<SearchHit>) new List<SearchHit>(), false, (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>) new Dictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>());
    }

    public override EntitySearchPlatformResponse PreparePlatformResponse(
      int responseCount,
      bool isTimedOut,
      List<SearchHit> searchResults,
      IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> searchFacets)
    {
      return (EntitySearchPlatformResponse) new WorkItemSearchPlatformResponse(responseCount, (IList<SearchHit>) searchResults, false, searchFacets);
    }

    public override EntitySearchPlatformResponse PreparePlatformResponse(
      AbstractSearchDocumentContract docContract,
      int responseCount,
      bool isTimedOut,
      List<SearchHit> searchResults,
      IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> searchFacets,
      string scrollId)
    {
      return (EntitySearchPlatformResponse) new WorkItemSearchPlatformResponse(responseCount, (IList<SearchHit>) searchResults, false, searchFacets);
    }

    public override IEnumerable<string> GetFieldNames(
      IEnumerable<string> storedFields,
      DocumentContractType contractType)
    {
      if (!this.m_isInstantSearch)
        return storedFields;
      return (IEnumerable<string>) new List<string>()
      {
        "projectId",
        "projectName",
        WorkItemContract.PlatformFieldNames.Id,
        WorkItemContract.PlatformFieldNames.Title,
        WorkItemContract.PlatformFieldNames.WorkItemType
      };
    }

    public override int GetTotalResultCount(
      EntitySearchPlatformRequest request,
      ISearchResponse<T> elasticSearchResponse)
    {
      return (int) elasticSearchResponse.Total;
    }

    public override string GetStoredFieldNameForElasticsearchName(
      string field,
      DocumentContractType contractType)
    {
      return WorkItemEntityIndexProvider<T>.s_documentContractMapping.ContainsKey(contractType) ? WorkItemEntityIndexProvider<T>.s_documentContractMapping[contractType].GetFieldNameForStoredField(field) : field;
    }

    public override string GetStoredFieldValue(
      string field,
      string fieldValue,
      DocumentContractType contractType)
    {
      return WorkItemEntityIndexProvider<T>.s_documentContractMapping.ContainsKey(contractType) ? WorkItemEntityIndexProvider<T>.s_documentContractMapping[contractType].GetStoredFieldValue(field, fieldValue) : fieldValue;
    }

    public override IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> GetSearchFacets(
      EntitySearchPlatformRequest request,
      ISearchResponse<T> elasticSearchResponse)
    {
      FriendlyDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> searchFacets = new FriendlyDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      AggregateDictionary aggregations = elasticSearchResponse.Aggregations;
      if (aggregations != null)
      {
        searchFacets.Add(this.GetTermFacets(aggregations, request, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Project, new string[2]
        {
          "filtered_project_aggs",
          "project_aggs"
        }, true));
        searchFacets.Add(this.GetTermFacets(aggregations, request, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemType, new string[2]
        {
          "filtered_type_aggs",
          "type_aggs"
        }));
        searchFacets.Add(this.GetTermFacets(aggregations, request, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemState, new string[2]
        {
          "filtered_state_aggs",
          "state_aggs"
        }));
        searchFacets.Add(this.GetTermFacets(aggregations, request, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemAssignedTo, new string[2]
        {
          "filtered_assignedto_aggs",
          "assignedto_aggs"
        }));
      }
      return (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>) searchFacets;
    }

    public override SearchHit GetSearchHit(
      IHit<T> hit,
      EntitySearchPlatformRequest request,
      ConcurrentBag<Exception> exceptions,
      ref bool isOperationSuccessful)
    {
      Dictionary<string, IEnumerable<string>> searchHitFields = this.GetSearchHitFields(hit, request, exceptions, ref isOperationSuccessful);
      if (!isOperationSuccessful)
        return (SearchHit) new WorkItemSearchHit();
      if (hit.Highlight != null)
        return (SearchHit) new WorkItemSearchHit((IEnumerable<WorkItemHit>) WorkItemEntityIndexProvider<T>.CalculateHighlightSnippets(hit.Highlight, ref exceptions, ref isOperationSuccessful), (IDictionary<string, IEnumerable<string>>) searchHitFields);
      exceptions.Add((Exception) new SearchPlatformException("ES Response: hit.highlights cannot be null"));
      isOperationSuccessful = false;
      return (SearchHit) new WorkItemSearchHit();
    }

    protected override Dictionary<string, IEnumerable<string>> GetSearchHitSources(
      T sources,
      EntitySearchPlatformRequest request,
      ConcurrentBag<Exception> exceptions,
      ref bool isOperationSuccessful)
    {
      Dictionary<string, IEnumerable<string>> searchHitSources = new Dictionary<string, IEnumerable<string>>();
      if (sources is WorkItemContract workItemContract)
      {
        if (!string.IsNullOrWhiteSpace(workItemContract.CollectionId))
          searchHitSources.Add("collectionId", (IEnumerable<string>) new List<string>()
          {
            workItemContract.CollectionId
          });
        if (!string.IsNullOrWhiteSpace(workItemContract.CollectionName))
          searchHitSources.Add("collectionName", (IEnumerable<string>) new List<string>()
          {
            workItemContract.CollectionName
          });
        if (!string.IsNullOrWhiteSpace(workItemContract.ProjectId))
          searchHitSources.Add("projectId", (IEnumerable<string>) new List<string>()
          {
            workItemContract.ProjectId
          });
        if (!string.IsNullOrWhiteSpace(workItemContract.ProjectName))
          searchHitSources.Add("projectName", (IEnumerable<string>) new List<string>()
          {
            workItemContract.ProjectName
          });
        if (workItemContract.Fields != null && workItemContract.Fields.Any<KeyValuePair<string, object>>())
        {
          foreach (KeyValuePair<string, object> field1 in (IEnumerable<KeyValuePair<string, object>>) workItemContract.Fields)
          {
            KeyValuePair<string, object> field = field1;
            string field2 = "fields" + "." + field.Key;
            if (field.Value is JArray source)
            {
              if (!source.Any<JToken>())
              {
                exceptions.Add((Exception) new SearchPlatformException("ES Response: Field Value cannot be empty for field : " + field.Key));
                isOperationSuccessful = false;
                return searchHitSources;
              }
              searchHitSources.Add(this.GetStoredFieldNameForElasticsearchName(field2, request.ContractType), source.Select<JToken, string>((Func<JToken, string>) (x => this.GetStoredFieldValue(field.Key, x.ToString(), request.ContractType))));
            }
            else if (field.Value is string str)
              searchHitSources.Add(this.GetStoredFieldNameForElasticsearchName(field2, request.ContractType), (IEnumerable<string>) new List<string>()
              {
                str
              });
            else if (field.Value is long)
              searchHitSources.Add(this.GetStoredFieldNameForElasticsearchName(field2, request.ContractType), (IEnumerable<string>) new List<string>()
              {
                field.Value.ToString()
              });
            else if (field.Value is DateTime dateTime)
            {
              searchHitSources.Add(this.GetStoredFieldNameForElasticsearchName(field2, request.ContractType), (IEnumerable<string>) new List<string>()
              {
                dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", (IFormatProvider) CultureInfo.InvariantCulture)
              });
            }
            else
            {
              exceptions.Add((Exception) new SearchPlatformException("ES Response: Unsupported return value for field : " + field.Key));
              isOperationSuccessful = false;
              return searchHitSources;
            }
          }
        }
      }
      return searchHitSources;
    }

    private static ICollection<WorkItemHit> CalculateHighlightSnippets(
      IReadOnlyDictionary<string, IReadOnlyCollection<string>> highlights,
      ref ConcurrentBag<Exception> exceptions,
      ref bool isOperationSuccessful)
    {
      List<WorkItemFragment> workItemFragmentList = new List<WorkItemFragment>();
      List<WorkItemHit> highlightSnippets = new List<WorkItemHit>(highlights.Count);
      foreach (KeyValuePair<string, IReadOnlyCollection<string>> highlight in (IEnumerable<KeyValuePair<string, IReadOnlyCollection<string>>>) highlights)
      {
        string nameForStoredField = WorkItemEntityIndexProvider<T>.s_documentContractMapping[DocumentContractType.WorkItemContract].GetFieldNameForStoredField(highlight.Key);
        foreach (string fragment in (IEnumerable<string>) highlight.Value)
        {
          WorkItemFragment workItemFragment = WorkItemFragment.Parse(nameForStoredField, fragment);
          workItemFragmentList.Add(workItemFragment);
          if (workItemFragment.Exception != null)
          {
            exceptions.Add(workItemFragment.Exception);
            isOperationSuccessful = false;
          }
        }
      }
      foreach (WorkItemFragment workItemFragment in workItemFragmentList)
      {
        if (workItemFragment.ContentLength > 0)
        {
          workItemFragment.Score = (1.0 + workItemFragment.Score) / Math.Sqrt((double) workItemFragment.ContentLength);
        }
        else
        {
          exceptions.Add((Exception) new SearchPlatformException(FormattableString.Invariant(FormattableStringFactory.Create("Content length of scored fragment [{0}] is invalid [{1}].", (object) workItemFragment.Text, (object) workItemFragment.ContentLength))));
          isOperationSuccessful = false;
        }
      }
      workItemFragmentList.Sort();
      foreach (WorkItemFragment workItemFragment in workItemFragmentList)
        highlightSnippets.Add(new WorkItemHit()
        {
          FieldReferenceName = workItemFragment.FieldReferenceName,
          Highlights = (IEnumerable<string>) new List<string>()
          {
            workItemFragment.Text
          }
        });
      return (ICollection<WorkItemHit>) highlightSnippets;
    }

    public override IExpression BuildQueryFilterExpression(
      IVssRequestContext requestContext,
      IDictionary<string, IEnumerable<string>> searchFilters,
      IExpression queryParseTree,
      DocumentContractType contractType)
    {
      return new WorkItemFilterBuilder(requestContext, searchFilters).GetQueryFilterExpression();
    }
  }
}
