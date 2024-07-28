// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.PackageCountRequestForwarder
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
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Package;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Package;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  public class PackageCountRequestForwarder : CountRequestForwarderBase
  {
    [StaticSafe]
    protected static readonly IDictionary<string, string> s_crossEntityFilterMap = (IDictionary<string, string>) new FriendlyDictionary<string, string>()
    {
      ["CollectionFilters"] = PackageSearchFilterCategories.Collections
    };
    private PackageSearchRequest m_packageSearchRequest = new PackageSearchRequest();

    public PackageCountRequestForwarder(
      string searchPlatformConnectionString,
      string searchPlatformSettings,
      bool isOnPrem)
      : base(searchPlatformConnectionString, searchPlatformSettings, isOnPrem)
    {
    }

    public PackageCountRequestForwarder(ISearchPlatform searchPlatform)
      : base(searchPlatform)
    {
    }

    protected override IExpression CreateCorrectedParseTree(
      IVssRequestContext requestContext,
      CountRequest countRequest)
    {
      return PackageSearchQueryTransformer.Correct(requestContext, PackageSearchQueryTransformer.Parse(countRequest.SearchText));
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
      PackageSearchPlatformRequest searchPlatformRequest = new PackageSearchPlatformRequest();
      searchPlatformRequest.RequestId = requestId;
      searchPlatformRequest.IndexInfo = indexInfo;
      searchPlatformRequest.QueryParseTree = correctedQueryParseTree;
      searchPlatformRequest.SearchFilters = searchFilters;
      searchPlatformRequest.ScopeFiltersExpression = scopeFiltersExpression;
      searchPlatformRequest.TakeResults = 0;
      searchPlatformRequest.TakeAggResults = 1;
      searchPlatformRequest.Fields = (IEnumerable<string>) new string[3]
      {
        "name",
        "description",
        "version"
      };
      searchPlatformRequest.ContractType = DocumentContractType.PackageVersionContract;
      PackageSearchPlatformRequest searchQueryRequest = searchPlatformRequest;
      return (ResultsCountPlatformResponse) (this.SearchPlatform.Search<PackageVersionContract>(requestContext, (EntitySearchPlatformRequest) searchQueryRequest, (IEntityType) PackageEntityType.GetInstance(), this.GetIndexProvider()) as PackageSearchPlatformResponse);
    }

    protected override bool IsSupportedFilter(string filter) => this.m_packageSearchRequest.IsSupportedFilter(filter);

    protected override IDictionary<string, string> GetCrossEntityFilterMap() => PackageCountRequestForwarder.s_crossEntityFilterMap;

    internal override EntitySearchQueryTagger GetEntitySpecificSearchTagger(
      IExpression correctedQueryParseTree,
      IDictionary<string, IEnumerable<string>> searchFilters)
    {
      return (EntitySearchQueryTagger) new PackageSearchQueryTagger(correctedQueryParseTree, searchFilters);
    }

    internal virtual EntityIndexProvider<PackageVersionContract> GetIndexProvider() => (EntityIndexProvider<PackageVersionContract>) new PackageEntityCountIndexProvider<PackageVersionContract>();
  }
}
