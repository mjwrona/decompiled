// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.CodeCountRequestForwarder
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
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  public class CodeCountRequestForwarder : CountRequestForwarderBase
  {
    [StaticSafe]
    private static readonly IDictionary<string, string> s_crossEntityFilterMap = (IDictionary<string, string>) new FriendlyDictionary<string, string>()
    {
      [Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Project] = "ProjectFilters",
      ["Project"] = "ProjectFilters"
    };
    private SearchQuery m_searchQuery = new SearchQuery();
    private bool m_IsWildcardOnlySearch;

    public CodeCountRequestForwarder(
      string searchPlatformConnectionString,
      string searchPlatformSettings,
      bool isOnPrem)
      : base(searchPlatformConnectionString, searchPlatformSettings, isOnPrem)
    {
    }

    public CodeCountRequestForwarder(ISearchPlatform searchPlatform)
      : base(searchPlatform)
    {
    }

    protected override IExpression CreateCorrectedParseTree(
      IVssRequestContext requestContext,
      CountRequest countRequest)
    {
      ISearchQueryTransformer transformerInstance = SearchQueryTransformerFactory.GetTransformerInstance(requestContext, EntityPluginsFactory.GetEntityType(requestContext, "Code"));
      IExpression searchText = transformerInstance.ParseSearchText(countRequest.SearchText);
      this.m_IsWildcardOnlySearch = CodeForwarderHelpers.IsWildCardOnlySearch_Code(searchText);
      return transformerInstance.CorrectQuery(requestContext, searchText);
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
      AbstractSearchDocumentContract codeContract = (AbstractSearchDocumentContract) CodeFileContract.CreateCodeContract(contractType, this.SearchPlatform);
      return new ResultsCountPlatformRequest(requestId, indexInfo, correctedQueryParseTree, searchFilters, scopeFiltersExpression, contractType, codeContract, 51).Count(requestContext);
    }

    protected override List<ErrorData> ValidateQueryParseTree(
      IVssRequestContext requestContext,
      IExpression correctedQueryParseTree,
      IDictionary<string, IEnumerable<string>> searchFilters,
      DocumentContractType contractType)
    {
      List<ErrorData> errorList;
      CodeForwarderHelpers.CheckIfCodeQuerySupported(requestContext, contractType, correctedQueryParseTree, searchFilters, this.m_IsWildcardOnlySearch, out errorList, out CodeSearchQueryTagger _);
      return errorList;
    }

    protected override bool IsSupportedFilter(string filter) => this.m_searchQuery.IsSupportedFilter(filter);

    protected override IDictionary<string, string> GetCrossEntityFilterMap() => CodeCountRequestForwarder.s_crossEntityFilterMap;

    internal override EntitySearchQueryTagger GetEntitySpecificSearchTagger(
      IExpression correctedQueryParseTree,
      IDictionary<string, IEnumerable<string>> searchFilters)
    {
      throw new NotImplementedException();
    }
  }
}
