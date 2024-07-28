// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.SettingSearchControllerBase
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Setting;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Search.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  public abstract class SettingSearchControllerBase : SearchV2ControllerBase
  {
    private ISearchRequestForwarder<SettingSearchRequest, SettingSearchResponse> m_settingSearchRequestForwarder;

    protected SettingSearchControllerBase() => this.IndexMapper = (IIndexMapper) new Microsoft.VisualStudio.Services.Search.Query.IndexMapper((IEntityType) SettingEntityType.GetInstance());

    protected SettingSearchControllerBase(
      IIndexMapper indexMapper,
      ISearchRequestForwarder<SettingSearchRequest, SettingSearchResponse> settingSearchQueryForwarder)
    {
      this.IndexMapper = indexMapper;
      this.m_settingSearchRequestForwarder = settingSearchQueryForwarder;
    }

    protected override void InitializeInternal(HttpControllerContext controllerContext)
    {
      base.InitializeInternal(controllerContext);
      this.InitializeRequestForwarder(this.TfsRequestContext);
    }

    internal void InitializeRequestForwarder(IVssRequestContext requestContext)
    {
      if (this.m_settingSearchRequestForwarder != null)
        return;
      this.m_settingSearchRequestForwarder = (ISearchRequestForwarder<SettingSearchRequest, SettingSearchResponse>) new SettingSearchRequestForwarder(this.IndexMapper.GetESConnectionString(requestContext), requestContext.GetElasticsearchPlatformSettings("/Service/ALMSearch/Settings/ATSearchPlatformSettings"), SearchOptions.Highlighting | SearchOptions.Faceting | SearchOptions.Ranking | SearchOptions.Rescore, false);
    }

    protected SettingSearchResponse HandlePostSettingQueryRequest(
      IVssRequestContext requestContext,
      SettingSearchRequest request)
    {
      Stopwatch e2eRequestTimer = Stopwatch.StartNew();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfQueryRequests", "Query Pipeline", 1.0);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("EntityType", "Query Pipeline", (double) SettingEntityType.GetInstance().ID);
      if (request == null)
        throw new InvalidQueryException(SearchWebApiResources.NullQueryMessage);
      SettingSearchResponse response = (SettingSearchResponse) null;
      try
      {
        if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          this.HandleNullProperties((EntitySearchRequest) request);
          this.ValidateQuery((EntitySearchRequest) request, requestContext);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1083132, "REST-API", "REST-API", request.ToString());
          this.PublishRequest((EntitySearchRequest) request);
          DocumentContractType documentContractType = this.IndexMapper.GetDocumentContractType(requestContext);
          IEnumerable<IndexInfo> indexInfo = this.IndexMapper.GetIndexInfo(requestContext);
          response = this.m_settingSearchRequestForwarder.ForwardSearchRequest(requestContext, request, indexInfo, (IExpression) new EmptyExpression(), requestContext.ActivityId.ToString(), documentContractType, false, out IEnumerable<string> _);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1083132, "REST-API", "REST-API", response.ToString());
          this.PublishResponse((EntitySearchResponse) response);
          e2eRequestTimer.Stop();
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("E2EQueryTime", "Query Pipeline", (double) e2eRequestTimer.ElapsedMilliseconds);
          PerformanceTimer.SendCustomerIntelligenceData(requestContext, (Action<CustomerIntelligenceData>) (ciData =>
          {
            ciData.Add("Timings", requestContext.GetTraceTimingAsString());
            ciData.Add("ElapsedMillis", requestContext.LastTracedBlockElapsedMilliseconds());
            ciData.Add("NoOfSettingSearchResults", (double) response.Count);
            ciData.Add("E2EQueryTime", (double) e2eRequestTimer.ElapsedMilliseconds);
          }));
        }
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfFailedQueryRequests", "Query Pipeline", 1.0);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1083132, "REST-API", "REST-API", ex);
        SearchPlatformExceptionLogger.LogSearchPlatformException(ex);
      }
      return response;
    }

    protected override void PublishRequest(EntitySearchRequest request)
    {
      SettingSearchRequest settingSearchRequest = request as SettingSearchRequest;
      FriendlyDictionary<string, object> properties = new FriendlyDictionary<string, object>();
      properties.Add("SettingSSkip", (object) settingSearchRequest.Skip);
      properties.Add("SettingSTake", (object) settingSearchRequest.Top);
      if (settingSearchRequest.Filters != null)
        properties.Add("SettingSSearchFilters", (object) string.Join(" | ", settingSearchRequest.Filters.Select<KeyValuePair<string, IEnumerable<string>>, string>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (kvp => FormattableString.Invariant(FormattableStringFactory.Create("Name = {0}; Count = {1}", (object) kvp.Key, (object) kvp.Value.Count<string>()))))));
      properties["SettingSSearchText"] = settingSearchRequest.SearchText != null ? (object) settingSearchRequest.SearchText : (object) string.Empty;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("REST-API", "Query Pipeline", (IDictionary<string, object>) properties);
    }

    protected override void PublishResponse(EntitySearchResponse response)
    {
      base.PublishResponse(response);
      SettingSearchResponse settingSearchResponse = response as SettingSearchResponse;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("REST-API", "Query Pipeline", (IDictionary<string, object>) new FriendlyDictionary<string, object>()
      {
        ["SettingTotalMatches"] = (object) settingSearchResponse.Count
      });
    }
  }
}
