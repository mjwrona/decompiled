// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.WorkItemCountRequestForwarder
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
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  public class WorkItemCountRequestForwarder : CountRequestForwarderBase
  {
    [StaticSafe]
    private static readonly IDictionary<string, string> s_crossEntityFilterMap = (IDictionary<string, string>) new FriendlyDictionary<string, string>()
    {
      ["ProjectFilters"] = Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Project,
      ["Project"] = Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Project
    };
    private WorkItemSearchRequest m_searchRequest = new WorkItemSearchRequest();

    public WorkItemCountRequestForwarder(
      string searchPlatformConnectionString,
      string searchPlatformSettings,
      bool isOnPrem)
      : base(searchPlatformConnectionString, searchPlatformSettings, isOnPrem)
    {
    }

    public WorkItemCountRequestForwarder(ISearchPlatform searchPlatform)
      : base(searchPlatform)
    {
    }

    protected override IExpression CreateCorrectedParseTree(
      IVssRequestContext requestContext,
      CountRequest countRequest)
    {
      ISearchQueryTransformer transformerInstance = SearchQueryTransformerFactory.GetTransformerInstance(requestContext, (IEntityType) WorkItemEntityType.GetInstance());
      return transformerInstance.CorrectQuery(requestContext, transformerInstance.ParseSearchText(countRequest.SearchText));
    }

    protected override ResultsCountPlatformResponse GetCountPlatformResponse(
      CountRequest countRequest,
      IDictionary<string, IEnumerable<string>> searchFilters,
      string requestId,
      IEnumerable<IndexInfo> indexInfo,
      IExpression correctedQueryParseTree,
      IExpression scopeFiltersExpression,
      IVssRequestContext requestContext,
      DocumentContractType contractType)
    {
      ResultsCountPlatformRequest request = new ResultsCountPlatformRequest(requestId, indexInfo, correctedQueryParseTree, searchFilters, scopeFiltersExpression, contractType, (AbstractSearchDocumentContract) null, 51);
      return this.SearchPlatform.Count<WorkItemContract>(requestContext, request, (IEntityType) WorkItemEntityType.GetInstance(), (EntityIndexProvider<WorkItemContract>) new WorkItemEntityIndexProvider<WorkItemContract>());
    }

    protected override List<ErrorData> ValidateQueryParseTree(
      IVssRequestContext requestContext,
      IExpression correctedQueryParseTree,
      IDictionary<string, IEnumerable<string>> searchFilters,
      DocumentContractType contractType)
    {
      WorkItemSearchQueryTagger searchQueryTagger = new WorkItemSearchQueryTagger(correctedQueryParseTree, searchFilters);
      searchQueryTagger.Compute();
      searchQueryTagger.Publish();
      List<ErrorData> tree = new List<ErrorData>();
      if (searchQueryTagger.Tags.Contains("UnfilteredPrefixWildcard"))
        tree.Add(new ErrorData()
        {
          ErrorCode = "PrefixWildcardQueryNotSupported",
          ErrorType = ErrorType.Warning
        });
      return tree;
    }

    protected override bool IsSupportedFilter(string filter) => this.m_searchRequest.IsSupportedFilter(filter);

    protected override IDictionary<string, string> GetCrossEntityFilterMap() => WorkItemCountRequestForwarder.s_crossEntityFilterMap;

    internal override EntitySearchQueryTagger GetEntitySpecificSearchTagger(
      IExpression correctedQueryParseTree,
      IDictionary<string, IEnumerable<string>> searchFilters)
    {
      throw new NotImplementedException();
    }
  }
}
