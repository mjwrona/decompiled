// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.EntitySearchRequestForwarder`2
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  public abstract class EntitySearchRequestForwarder<TRequest, TResponse> : 
    ISearchRequestForwarder<TRequest, TResponse>
    where TRequest : EntitySearchRequest
    where TResponse : EntitySearchResponse
  {
    protected ISearchPlatform SearchPlatform { get; }

    protected SearchOptions SearchOptions { get; }

    protected EntitySearchRequestForwarder(
      string searchPlatformConnectionString,
      string searchPlatformSettings,
      SearchOptions searchOptions,
      bool isOnPrem)
    {
      if (string.IsNullOrEmpty(searchPlatformConnectionString))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0} is null or empty", (object) nameof (searchPlatformConnectionString))));
      if (string.IsNullOrEmpty(searchPlatformSettings))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0} is null or empty", (object) nameof (searchPlatformSettings))));
      this.SearchPlatform = SearchPlatformFactory.GetInstance().Create(searchPlatformConnectionString, searchPlatformSettings, isOnPrem);
      this.SearchOptions = searchOptions;
    }

    protected EntitySearchRequestForwarder(ISearchPlatform searchPlatform)
    {
      this.SearchPlatform = searchPlatform;
      this.SearchOptions = SearchOptions.Highlighting | SearchOptions.Faceting | SearchOptions.Ranking | SearchOptions.Rescore;
    }

    public abstract TResponse GetZeroResultResponse(
      TRequest searchRequest,
      out IEnumerable<string> suggestions);

    public abstract TResponse ForwardSearchRequest(
      IVssRequestContext requestContext,
      TRequest searchRequest,
      IEnumerable<IndexInfo> indexInfo,
      IExpression scopeFiltersExpression,
      string requestId,
      DocumentContractType contractType,
      bool includeSuggestions,
      out IEnumerable<string> suggestions);

    protected virtual SearchOptions GetSearchOptionsForPlatformSearchRequest(
      EntitySearchRequest searchRequest)
    {
      return !searchRequest.IncludeFacets ? this.SearchOptions & ~SearchOptions.Faceting : this.SearchOptions | SearchOptions.Faceting;
    }

    protected virtual void SetWarningInReponseForFacetingAskWhenFacetingIsOffAtHostLevel(
      EntitySearchRequest searchRequest,
      EntitySearchResponse searchResponse)
    {
      if (this.SearchOptions.HasFlag((Enum) SearchOptions.Faceting) || !searchRequest.IncludeFacets)
        return;
      searchResponse.InfoCode = 10;
    }
  }
}
