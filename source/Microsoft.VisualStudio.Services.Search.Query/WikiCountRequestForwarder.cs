// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.WikiCountRequestForwarder
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
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  public class WikiCountRequestForwarder : CountRequestForwarderBase
  {
    [StaticSafe]
    private static readonly IDictionary<string, string> s_crossEntityFilterMap = (IDictionary<string, string>) new FriendlyDictionary<string, string>()
    {
      [Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Project] = "ProjectFilters"
    };
    private WikiSearchQuery m_searchQuery = new WikiSearchQuery();

    public WikiCountRequestForwarder(
      string searchPlatformConnectionString,
      string searchPlatformSettings,
      bool isOnPrem)
      : base(searchPlatformConnectionString, searchPlatformSettings, isOnPrem)
    {
    }

    public WikiCountRequestForwarder(ISearchPlatform searchPlatform)
      : base(searchPlatform)
    {
    }

    protected override IExpression CreateCorrectedParseTree(
      IVssRequestContext requestContext,
      CountRequest countRequest)
    {
      ISearchQueryTransformer transformerInstance = SearchQueryTransformerFactory.GetTransformerInstance(requestContext, (IEntityType) WikiEntityType.GetInstance());
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
      return this.SearchPlatform.Count<WikiContract>(requestContext, request, (IEntityType) WikiEntityType.GetInstance(), (EntityIndexProvider<WikiContract>) new WikiEntityIndexProvider<WikiContract>());
    }

    protected override bool IsSupportedFilter(string filter) => this.m_searchQuery.IsSupportedFilter(filter);

    protected override IDictionary<string, string> GetCrossEntityFilterMap() => WikiCountRequestForwarder.s_crossEntityFilterMap;

    internal override EntitySearchQueryTagger GetEntitySpecificSearchTagger(
      IExpression correctedQueryParseTree,
      IDictionary<string, IEnumerable<string>> searchFilters)
    {
      return (EntitySearchQueryTagger) new WikiSearchQueryTagger(correctedQueryParseTree, searchFilters);
    }
  }
}
