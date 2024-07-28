// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.CountRequestForwarderBase
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  public abstract class CountRequestForwarderBase : ICountRequestForwarder
  {
    protected const int TerminateAfter = 51;

    protected ISearchPlatform SearchPlatform { get; }

    protected CountRequestForwarderBase(
      string searchPlatformConnectionString,
      string searchPlatformSettings,
      bool isOnPrem)
    {
      if (string.IsNullOrEmpty(searchPlatformConnectionString))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0} is invalid.", (object) nameof (searchPlatformConnectionString))));
      if (string.IsNullOrEmpty(searchPlatformSettings))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0} is invalid.", (object) nameof (searchPlatformSettings))));
      this.SearchPlatform = SearchPlatformFactory.GetInstance().Create(searchPlatformConnectionString, searchPlatformSettings, isOnPrem);
    }

    protected CountRequestForwarderBase(ISearchPlatform searchPlatform) => this.SearchPlatform = searchPlatform;

    protected abstract IExpression CreateCorrectedParseTree(
      IVssRequestContext requestContext,
      CountRequest countRequest);

    protected abstract ResultsCountPlatformResponse GetCountPlatformResponse(
      CountRequest countRequest,
      IDictionary<string, IEnumerable<string>> searchFilters,
      string requestId,
      IEnumerable<IndexInfo> indexInfo,
      IExpression correctedQueryParseTree,
      IExpression scopeFiltersExpression,
      IVssRequestContext requestContext,
      DocumentContractType contractType);

    protected abstract bool IsSupportedFilter(string filter);

    internal abstract EntitySearchQueryTagger GetEntitySpecificSearchTagger(
      IExpression correctedQueryParseTree,
      IDictionary<string, IEnumerable<string>> searchFilters);

    protected virtual List<ErrorData> ValidateQueryParseTree(
      IVssRequestContext requestContext,
      IExpression correctedQueryParseTree,
      IDictionary<string, IEnumerable<string>> searchFilters,
      DocumentContractType contractType = DocumentContractType.Unsupported)
    {
      EntitySearchQueryTagger specificSearchTagger = this.GetEntitySpecificSearchTagger(correctedQueryParseTree, searchFilters);
      specificSearchTagger.Compute();
      specificSearchTagger.Publish();
      List<ErrorData> tree = new List<ErrorData>();
      if (specificSearchTagger.Tags.Contains(EntitySearchQueryTagger.UnfilteredPrefixWildcard))
        tree.Add(new ErrorData()
        {
          ErrorCode = "PrefixWildcardQueryNotSupported",
          ErrorType = ErrorType.Warning
        });
      return tree;
    }

    protected virtual IDictionary<string, string> GetCrossEntityFilterMap() => (IDictionary<string, string>) null;

    protected virtual string UpdateKey(string filterKey)
    {
      string str = string.Empty;
      IDictionary<string, string> crossEntityFilterMap = this.GetCrossEntityFilterMap();
      if (crossEntityFilterMap == null || !crossEntityFilterMap.TryGetValue(filterKey, out str))
        str = filterKey;
      return str;
    }

    public CountResponse ForwardCountRequest(
      IVssRequestContext requestContext,
      CountRequest countRequest,
      IEnumerable<IndexInfo> indexInfo,
      IExpression scopeFiltersExpression,
      string requestId,
      DocumentContractType contractType)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1083004, "Query Pipeline", "Query", nameof (ForwardCountRequest));
      CountResponse countResponse = (CountResponse) null;
      try
      {
        if (countRequest == null)
          throw new ArgumentNullException(nameof (countRequest));
        if (scopeFiltersExpression == null)
          throw new ArgumentNullException(nameof (scopeFiltersExpression));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1083005, "Query Pipeline", "Query", (Func<string>) (() => countRequest.ToString()));
        IExpression correctedParseTree = this.CreateCorrectedParseTree(requestContext, countRequest);
        IDictionary<string, IEnumerable<string>> searchFilters = this.RemoveUnsupportedFilters(this.UpdateFilterNames(countRequest.SearchFilters));
        List<ErrorData> tree = this.ValidateQueryParseTree(requestContext, correctedParseTree, searchFilters, contractType);
        if (tree.Count > 0)
        {
          countResponse = this.GetZeroResultResponse(countRequest);
          foreach (ErrorData errorData in tree)
            countResponse.AddError(errorData);
        }
        else if (indexInfo == null || !indexInfo.Any<IndexInfo>())
        {
          countResponse = this.GetZeroResultResponse(countRequest);
          countResponse.AddError(new ErrorData()
          {
            ErrorCode = "IndexingNotStarted",
            ErrorType = ErrorType.Warning
          });
        }
        else
        {
          Stopwatch stopwatch = Stopwatch.StartNew();
          countResponse = ResultsCountPlatformResponse.PrepareCountResponse(this.GetCountPlatformResponse(countRequest, searchFilters, requestId, indexInfo, correctedParseTree, scopeFiltersExpression, requestContext, contractType), 50);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NonFuzzyE2EPlatformQueryTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1083006, "Query Pipeline", "Query", nameof (ForwardCountRequest));
      }
      return countResponse;
    }

    public CountResponse GetZeroResultResponse(CountRequest countRequest) => new CountResponse();

    private IDictionary<string, IEnumerable<string>> RemoveUnsupportedFilters(
      IDictionary<string, IEnumerable<string>> searchFilters)
    {
      IDictionary<string, IEnumerable<string>> dictionary = (IDictionary<string, IEnumerable<string>>) new FriendlyDictionary<string, IEnumerable<string>>();
      foreach (KeyValuePair<string, IEnumerable<string>> searchFilter in (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) searchFilters)
      {
        if (this.IsSupportedFilter(searchFilter.Key))
          dictionary.Add(searchFilter);
      }
      return dictionary;
    }

    private IDictionary<string, IEnumerable<string>> UpdateFilterNames(
      IDictionary<string, IEnumerable<string>> searchFilters)
    {
      FriendlyDictionary<string, IEnumerable<string>> friendlyDictionary = new FriendlyDictionary<string, IEnumerable<string>>();
      foreach (KeyValuePair<string, IEnumerable<string>> searchFilter in (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) searchFilters)
        friendlyDictionary.Add(this.UpdateKey(searchFilter.Key), searchFilter.Value);
      return (IDictionary<string, IEnumerable<string>>) friendlyDictionary;
    }
  }
}
