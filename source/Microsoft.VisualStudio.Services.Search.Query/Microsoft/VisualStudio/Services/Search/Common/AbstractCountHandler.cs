// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.AbstractCountHandler
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public abstract class AbstractCountHandler : ICountHandler
  {
    protected ICountRequestForwarder CountRequestForwarder { get; set; }

    protected IIndexMapper IndexMapper { get; set; }

    protected IEntityType EntityType { get; set; }

    protected bool IsSecurityChecksEnabled { get; set; }

    protected AbstractCountHandler()
    {
    }

    protected AbstractCountHandler(ICountRequestForwarder forwarder) => this.CountRequestForwarder = forwarder;

    protected abstract void InitializeForwarder(IVssRequestContext requestContext);

    protected abstract void ValidateUserPermission(IVssRequestContext requestContext);

    protected abstract IExpression CreateScopeFiltersExpression(
      IVssRequestContext requestContext,
      CountRequest query,
      out bool noResultAccessible,
      ProjectInfo projectInfo);

    protected abstract void SetIndexingStatusInResponse(
      IVssRequestContext requestContext,
      CountResponse response);

    protected virtual DocumentContractType GetDocumentContractType(IVssRequestContext requestContext) => this.IndexMapper.GetDocumentContractType(requestContext);

    protected virtual void ValidateExtensionActive(IVssRequestContext requestContext)
    {
    }

    public virtual CountResponse HandleCountRequest(
      IVssRequestContext requestContext,
      CountRequest request,
      ProjectInfo projectInfo = null)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfResultsCountRequests", "Query Pipeline", 1.0, true);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishCi("REST-API", "Query Pipeline", "EntityType", FormattableString.Invariant(FormattableStringFactory.Create("{0}_Count", (object) this.EntityType)));
      this.ValidateExtensionActive(requestContext);
      this.ValidateUserPermission(requestContext);
      CountResponse countResponse = (CountResponse) null;
      try
      {
        this.ValidateCountRequest(request, requestContext, projectInfo);
        this.InitializeForwarder(requestContext);
        Stopwatch e2eQueryTimer = Stopwatch.StartNew();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1083007, "REST-API", "REST-API", (Func<string>) (() => request.ToString()));
        this.PublishRequest(request);
        DocumentContractType documentContractType = this.GetDocumentContractType(requestContext);
        bool noResultAccessible;
        IExpression filtersExpression = this.CreateScopeFiltersExpression(requestContext, request, out noResultAccessible, projectInfo);
        IEnumerable<IndexInfo> indexInfo = this.ReturnIndexingInfo(requestContext, request);
        countResponse = !noResultAccessible ? this.CountRequestForwarder.ForwardCountRequest(requestContext, request, indexInfo, filtersExpression, requestContext.ActivityId.ToString(), documentContractType) : this.CountRequestForwarder.GetZeroResultResponse(request);
        this.SetIndexingStatusInResponse(requestContext, countResponse);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1083007, "REST-API", "REST-API", (Func<string>) (() => countResponse.ToString()));
        this.PublishResponse(countResponse);
        e2eQueryTimer.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("E2EQueryTime", "Query Pipeline", (double) e2eQueryTimer.ElapsedMilliseconds, true);
        PerformanceTimer.SendCustomerIntelligenceData(requestContext, (Action<CustomerIntelligenceData>) (ciData =>
        {
          ciData.Add("Timings", requestContext.GetTraceTimingAsString());
          ciData.Add("ElapsedMillis", requestContext.LastTracedBlockElapsedMilliseconds());
          ciData.Add("EntityType", (object) this.EntityType);
          ciData.Add("ResultsCount", (double) countResponse.Count);
          ciData.Add("E2EQueryTime", (double) e2eQueryTimer.ElapsedMilliseconds);
        }));
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfFailedQueryRequests", "Query Pipeline", 1.0, true);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1083008, "REST-API", "REST-API", ex);
        SearchPlatformExceptionLogger.LogSearchPlatformException(ex);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishOnPremiseIndicator("TFS/Search/Query");
      }
      return countResponse;
    }

    public virtual IEnumerable<IndexInfo> ReturnIndexingInfo(
      IVssRequestContext requestContext,
      CountRequest request)
    {
      return this.IndexMapper.GetIndexInfo(requestContext);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.IFormatProvider,System.String,System.Object[])", Justification = "Usage of CurrentUICulture is by design")]
    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.IFormatProvider,System.String,System.Object)", Justification = "Usage of CurrentUICulture is by design")]
    private void ValidateCountRequest(
      CountRequest query,
      IVssRequestContext requestContext,
      ProjectInfo projectInfo = null)
    {
      if (string.IsNullOrEmpty(query.SearchText))
        throw new InvalidQueryException(SearchSharedWebApiResources.NullOrEmptySearchTextMessage);
      if (query.SearchFilters != null)
      {
        if (query.SearchFilters.Select<KeyValuePair<string, IEnumerable<string>>, string>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (f => f.Key)).GroupBy<string, string>((Func<string, string>) (x => x)).Any<IGrouping<string, string>>((Func<IGrouping<string, string>, bool>) (x => x.Count<string>() > 1)))
          throw new InvalidQueryException(SearchSharedWebApiResources.DuplicateFilterNameMessage);
        foreach (KeyValuePair<string, IEnumerable<string>> searchFilter in (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) query.SearchFilters)
        {
          if (string.IsNullOrWhiteSpace(searchFilter.Key))
            throw new InvalidQueryException(SearchSharedWebApiResources.NullOrEmptyFilterNameMessage);
          if (searchFilter.Value == null || !searchFilter.Value.Any<string>())
            throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchSharedWebApiResources.NullFilterValuesMessage, (object) searchFilter.Key));
          foreach (string str in searchFilter.Value)
          {
            if (string.IsNullOrWhiteSpace(str))
              throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchSharedWebApiResources.NullFilterValuesMessage, (object) searchFilter.Key));
          }
          if (string.Equals(searchFilter.Key, "ProjectFilters", StringComparison.OrdinalIgnoreCase) || string.Equals(searchFilter.Key, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Project, StringComparison.OrdinalIgnoreCase))
          {
            int num = searchFilter.Value.Count<string>();
            if (num > 1)
              throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchWebApiResources.InvalidProjectFiltersCountMessage));
            if (projectInfo != null && num == 1 && !string.Equals(searchFilter.Value.ElementAt<string>(0), projectInfo.Name, StringComparison.OrdinalIgnoreCase))
              throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchWebApiResources.InvalidProjectFilterValueMessage));
          }
        }
      }
      int configValueOrDefault = requestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/MaxSearchTextLength", 1024);
      if (query.SearchText.Length > configValueOrDefault)
        throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchWebApiResources.MaxSearchTextLengthExceptionMessageFormat, (object) configValueOrDefault));
    }

    private void PublishRequest(CountRequest request)
    {
      FriendlyDictionary<string, object> properties = new FriendlyDictionary<string, object>();
      properties["EntityType"] = (object) FormattableString.Invariant(FormattableStringFactory.Create("{0}_Count", (object) this.EntityType));
      FriendlyDictionary<string, object> friendlyDictionary = properties;
      IDictionary<string, IEnumerable<string>> searchFilters = request.SearchFilters;
      string str = string.Join(" | ", searchFilters != null ? searchFilters.Select<KeyValuePair<string, IEnumerable<string>>, string>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (kvp => FormattableString.Invariant(FormattableStringFactory.Create("Name = {0}; Count = {1}", (object) kvp.Key, (object) kvp.Value.Count<string>())))) : (IEnumerable<string>) null);
      friendlyDictionary["CountRequestSearchFilters"] = (object) str;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("REST-API", "Query Pipeline", (IDictionary<string, object>) properties);
    }

    private void PublishResponse(CountResponse response) => Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("REST-API", "Query Pipeline", (IDictionary<string, object>) new FriendlyDictionary<string, object>()
    {
      ["EntityType"] = (object) FormattableString.Invariant(FormattableStringFactory.Create("{0}_Count", (object) this.EntityType)),
      ["ResultsCount"] = (object) response.Count,
      ["RelationFromExactCount"] = (object) response.Relation,
      ["Errors"] = (object) response.Errors
    });
  }
}
