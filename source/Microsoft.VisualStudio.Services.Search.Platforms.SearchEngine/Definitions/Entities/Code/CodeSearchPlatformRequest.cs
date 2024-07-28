// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code.CodeSearchPlatformRequest
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code
{
  public class CodeSearchPlatformRequest : EntitySearchPlatformRequest
  {
    internal CodeSearchPlatformRequest()
    {
    }

    public string HighlightField { get; set; }

    public override IEnumerable<string> Fields
    {
      get => throw new InvalidOperationException("Not allowed on code");
      set => throw new InvalidOperationException("Not allowed on code");
    }

    public IEnumerable<CodeFileContract.CodeContractQueryableElement> SearchElements
    {
      get => (this.DocumentContract as CodeFileContract).SearchElements;
      set => (this.DocumentContract as CodeFileContract).SearchElements = value;
    }

    public CodeSearchPlatformResponse Search(IVssRequestContext requestContext) => this.DocumentContract.Search(requestContext, (EntitySearchPlatformRequest) this) as CodeSearchPlatformResponse;

    public static CodeSearchPlatformRequest CreateCodeSearchPlatformRequest(
      ISearchPlatform searchPlatform,
      DocumentContractType contractType,
      SearchOptions options,
      string requestId,
      IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexInfo> indexInfo,
      IExpression queryParseTree,
      IDictionary<string, IEnumerable<string>> searchFilters,
      IExpression scopeFiltersExpression,
      SearchQuery searchQuery)
    {
      AbstractSearchDocumentContract codeContract = (AbstractSearchDocumentContract) CodeFileContract.CreateCodeContract(contractType, searchPlatform);
      CodeSearchPlatformRequest searchPlatformRequest = new CodeSearchPlatformRequest();
      searchPlatformRequest.Options = options;
      searchPlatformRequest.RequestId = requestId;
      searchPlatformRequest.IndexInfo = indexInfo;
      searchPlatformRequest.QueryParseTree = queryParseTree;
      searchPlatformRequest.SearchFilters = searchFilters;
      searchPlatformRequest.ScopeFiltersExpression = scopeFiltersExpression;
      searchPlatformRequest.SkipResults = searchQuery.SkipResults;
      searchPlatformRequest.TakeResults = searchQuery.TakeResults;
      searchPlatformRequest.ContractType = contractType;
      searchPlatformRequest.SortOptions = searchQuery.SortOptions;
      searchPlatformRequest.DocumentContract = codeContract;
      return searchPlatformRequest;
    }

    public static CodeSearchPlatformRequest CreateCodeSearchPlatformScrollRequest(
      DocumentContractType contractType,
      string requestId,
      ISearchPlatform searchPlatform,
      IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexInfo> indexInfo,
      IExpression queryParseTree,
      IDictionary<string, IEnumerable<string>> searchFilters,
      IExpression scopeFiltersExpression,
      int scrollSize,
      string scrollId)
    {
      AbstractSearchDocumentContract codeContract = (AbstractSearchDocumentContract) CodeFileContract.CreateCodeContract(contractType, searchPlatform);
      IDictionary<string, IEnumerable<string>> dictionary = (IDictionary<string, IEnumerable<string>>) new Dictionary<string, IEnumerable<string>>();
      foreach (KeyValuePair<string, IEnumerable<string>> searchFilter in (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) searchFilters)
      {
        if (searchFilter.Key == "AccountFilters")
          dictionary.Add("CollectionFilters", searchFilter.Value);
        else
          dictionary.Add(searchFilter);
      }
      CodeSearchPlatformRequest platformScrollRequest = new CodeSearchPlatformRequest();
      platformScrollRequest.RequestId = requestId;
      platformScrollRequest.IndexInfo = indexInfo;
      platformScrollRequest.QueryParseTree = queryParseTree;
      platformScrollRequest.SearchFilters = dictionary;
      platformScrollRequest.ScopeFiltersExpression = scopeFiltersExpression;
      platformScrollRequest.ScrollSize = scrollSize;
      platformScrollRequest.ScrollId = scrollId;
      platformScrollRequest.ContractType = contractType;
      platformScrollRequest.DocumentContract = codeContract;
      return platformScrollRequest;
    }
  }
}
