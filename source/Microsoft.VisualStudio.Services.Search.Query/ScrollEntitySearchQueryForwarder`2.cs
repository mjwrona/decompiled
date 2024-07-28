// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.ScrollEntitySearchQueryForwarder`2
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  public abstract class ScrollEntitySearchQueryForwarder<TRequest, TResponse> : 
    IScrollSearchQueryForwarder<TRequest, TResponse>
    where TRequest : ScrollSearchRequest
    where TResponse : ScrollSearchResponse
  {
    internal readonly SearchOptions m_searchOptions;

    protected ISearchPlatform SearchPlatform { get; }

    protected ScrollEntitySearchQueryForwarder(
      string searchPlatformConnectionString,
      string searchPlatformSettings,
      SearchOptions searchOptions,
      bool isOnPrem)
    {
      if (string.IsNullOrEmpty(searchPlatformConnectionString))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0} is null or empty.", (object) nameof (searchPlatformConnectionString))));
      if (string.IsNullOrEmpty(searchPlatformSettings))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0} is null or empty.", (object) nameof (searchPlatformSettings))));
      this.SearchPlatform = SearchPlatformFactory.GetInstance().Create(searchPlatformConnectionString, searchPlatformSettings, isOnPrem);
      this.m_searchOptions = searchOptions;
    }

    protected ScrollEntitySearchQueryForwarder(ISearchPlatform searchPlatform)
    {
      this.SearchPlatform = searchPlatform;
      this.m_searchOptions = SearchOptions.None;
    }

    protected abstract TResponse GetZeroResultResponse(TRequest searchQuery);

    public abstract TResponse ForwardSearchRequest(
      IVssRequestContext requestContext,
      TRequest searchQuery,
      IEnumerable<IndexInfo> indexInfo,
      IExpression scopeFiltersExpression,
      string requestId,
      DocumentContractType contractType);
  }
}
