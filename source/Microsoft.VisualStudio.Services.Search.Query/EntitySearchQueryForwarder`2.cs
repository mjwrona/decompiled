// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.EntitySearchQueryForwarder`2
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.Engine;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.Rules;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.RulesProviders;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  public abstract class EntitySearchQueryForwarder<TRequest, TResponse> : 
    ISearchQueryForwarder<TRequest, TResponse>
    where TRequest : EntitySearchQuery
    where TResponse : EntitySearchResponse
  {
    private readonly SearchOptions m_searchOptions;

    protected ISearchPlatform SearchPlatform { get; }

    protected EntitySearchQueryForwarder(
      string searchPlatformConnectionString,
      string searchPlatformSettings,
      SearchOptions searchOptions,
      bool isOnPrem)
    {
      if (string.IsNullOrEmpty(searchPlatformConnectionString))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0} is invalid", (object) nameof (searchPlatformConnectionString))));
      if (string.IsNullOrEmpty(searchPlatformSettings))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0} is invalid", (object) nameof (searchPlatformSettings))));
      this.SearchPlatform = SearchPlatformFactory.GetInstance().Create(searchPlatformConnectionString, searchPlatformSettings, isOnPrem);
      this.m_searchOptions = searchOptions;
    }

    protected EntitySearchQueryForwarder(ISearchPlatform searchPlatform)
    {
      this.SearchPlatform = searchPlatform;
      this.m_searchOptions = SearchOptions.Highlighting | SearchOptions.Faceting | SearchOptions.Ranking | SearchOptions.Rescore;
    }

    public abstract TResponse GetZeroResultResponse(TRequest searchRequest);

    public abstract TResponse ForwardSearchRequest(
      IVssRequestContext requestContext,
      TRequest searchQuery,
      IEnumerable<IndexInfo> indexInfo,
      IExpression scopeFiltersExpression,
      string requestId,
      DocumentContractType contractType);

    protected void UpdateRelevanceRules(
      IVssRequestContext requestContext,
      IExpression queryParseTree,
      IEntityType entity,
      DocumentContractType contractType,
      Func<IVssRequestContext, IEnumerable<RelevanceRule>, IEnumerable<RelevanceRule>> ruleCorrectionDelegate)
    {
      if (!requestContext.IsFeatureEnabled("Search.Server.Relevance.RulesEngine"))
        return;
      IRelevanceRulesProvider provider = EntityRelevanceRulesProvider.GetProvider(entity, contractType);
      if (provider == null)
        return;
      IEnumerable<RelevanceRule> queryRelevanceRules = provider.GetChildQueryRelevanceRules(requestContext);
      if (queryRelevanceRules != null && queryRelevanceRules.Any<RelevanceRule>())
      {
        IEnumerable<RelevanceRule> rules = ruleCorrectionDelegate(requestContext, queryRelevanceRules);
        RelevanceEngine.ProcessChildRules(queryParseTree, rules);
      }
      IEnumerable<RelevanceRule> relevanceRules = provider.GetRelevanceRules(requestContext);
      if (relevanceRules == null || !relevanceRules.Any<RelevanceRule>())
        return;
      IEnumerable<RelevanceRule> rules1 = ruleCorrectionDelegate(requestContext, relevanceRules);
      RelevanceEngine.ProcessRules(queryParseTree, rules1);
    }

    protected virtual SearchOptions GetSearchOptionsForPlatformSearchRequest(
      EntitySearchQuery SearchQuery)
    {
      return !SearchQuery.SummarizedHitCountsNeeded ? this.m_searchOptions & ~SearchOptions.Faceting : this.m_searchOptions | SearchOptions.Faceting;
    }

    protected virtual void SetWarningInReponseForFacetingAskWhenFacetingIsOffAtHostLevel(
      EntitySearchQuery searchQuery,
      EntitySearchResponse searchResponse)
    {
      if (this.m_searchOptions.HasFlag((Enum) SearchOptions.Faceting) || !searchQuery.SummarizedHitCountsNeeded)
        return;
      searchResponse.AddError(new ErrorData()
      {
        ErrorCode = "FacetingNotEnabledAtScaleUnit",
        ErrorType = ErrorType.Warning,
        ErrorMessage = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "You asked for summary of Hit counts. HitCount Summary is not enabled at the host level")
      });
    }
  }
}
