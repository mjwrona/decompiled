// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.TenantWikiSearchController
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders;
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [ClientIgnore]
  [VersionedApiControllerCustomName(Area = "search", ResourceName = "tenantWikiQueryResults")]
  public class TenantWikiSearchController : WikiSearchControllerBase
  {
    private const string DefaultTenantName = "Microsoft";
    private ISearchQueryForwarder<WikiSearchQuery, WikiQueryResponse> m_tenantWikiSearchQueryForwarder;

    public TenantWikiSearchController() => this.IndexMapper = (IIndexMapper) new Microsoft.VisualStudio.Services.Search.Query.IndexMapper((IEntityType) TenantWikiEntityType.GetInstance());

    protected TenantWikiSearchController(
      IIndexMapper indexMapper,
      ISearchQueryForwarder<WikiSearchQuery, WikiQueryResponse> tenantWikiSearchQueryForwarder)
    {
      this.IndexMapper = indexMapper;
      this.m_tenantWikiSearchQueryForwarder = tenantWikiSearchQueryForwarder;
    }

    protected override void InitializeInternal(HttpControllerContext controllerContext)
    {
      base.InitializeInternal(controllerContext);
      if (this.m_tenantWikiSearchQueryForwarder != null)
        return;
      SearchOptions searchOptions = this.GetSearchOptions(this.TfsRequestContext) & ~SearchOptions.Highlighting & ~SearchOptions.Ranking;
      this.m_tenantWikiSearchQueryForwarder = (ISearchQueryForwarder<WikiSearchQuery, WikiQueryResponse>) new WikiSearchQueryForwarder(this.TfsRequestContext.GetConfigValue("/Service/ALMSearch/Settings/ATSearchPlatformConnectionString"), this.TfsRequestContext.GetElasticsearchPlatformSettings("/Service/ALMSearch/Settings/CustomSearchPlatformSettings", "ConnectionTimeout=180"), searchOptions, this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment, new WikiEntityIndexProvider<WikiContract>(), new WikiSearchPlatformRequest.Builder());
    }

    [HttpPost]
    [ClientLocationId("B31D5903-7ACF-40D2-9E92-5DC04A686BAE")]
    public WikiQueryResponse PostTenantWikiQuery(WikiSearchQuery query)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1083124, "REST-API", "REST-API", nameof (PostTenantWikiQuery));
      try
      {
        return this.HandlePostTenantWikiQueryRequest(this.TfsRequestContext, query);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1083125, "REST-API", "REST-API", nameof (PostTenantWikiQuery));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    protected WikiQueryResponse HandlePostTenantWikiQueryRequest(
      IVssRequestContext requestContext,
      WikiSearchQuery query)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfTenantQueryRequests", "Query Pipeline", 1.0);
      WikiQueryResponse response = (WikiQueryResponse) null;
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
        if (!requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableCustomTenant", TeamFoundationHostType.ProjectCollection))
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Setting '{0}' is not enabled.", (object) "/Service/ALMSearch/Settings/EnableCustomTenant")));
        if (query == null)
          throw new InvalidQueryException("The query is null.");
        this.HandleNullProperties((EntitySearchQuery) query);
        this.ValidateQuery((EntitySearchQuery) query, requestContext, this.ProjectInfo);
        Stopwatch e2eQueryTimer = Stopwatch.StartNew();
        this.ProcessQuery(query, requestContext);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1083126, "REST-API", "REST-API", query.ToString());
        this.PublishRequest((EntitySearchQuery) query);
        IEnumerable<IndexInfo> tenantWikiQueryAlias = this.IndexMapper.GetTenantWikiQueryAlias("Microsoft");
        DocumentContractType contractType = DocumentContractType.WikiContract;
        IExpression scopeFiltersExpression = (IExpression) new EmptyExpression();
        response = this.m_tenantWikiSearchQueryForwarder.ForwardSearchRequest(this.TfsRequestContext, query, tenantWikiQueryAlias, scopeFiltersExpression, requestContext.ActivityId.ToString(), contractType);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1083126, "REST-API", "REST-API", response.ToString());
        this.PublishResponse((EntitySearchResponse) response);
        e2eQueryTimer.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("E2ETenantQueryTime", "Query Pipeline", (double) e2eQueryTimer.ElapsedMilliseconds);
        PerformanceTimer.SendCustomerIntelligenceData(this.TfsRequestContext, (Action<CustomerIntelligenceData>) (ciData =>
        {
          ciData.Add("Timings", this.TfsRequestContext.GetTraceTimingAsString());
          ciData.Add("ElapsedMillis", this.TfsRequestContext.LastTracedBlockElapsedMilliseconds());
          ciData.Add("NoOfTenantWikiSearchResults", (double) response.Results.Values.Count<WikiResult>());
          ciData.Add("E2ETenantQueryTime", (double) e2eQueryTimer.ElapsedMilliseconds);
        }));
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfFailedTenantQueryRequests", "Query Pipeline", 1.0);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1083127, "REST-API", "REST-API", ex);
        switch (ex)
        {
          case InvalidQueryException _:
          case UnsupportedHostTypeException _:
          case NotSupportedException _:
            ExceptionDispatchInfo.Capture(ex).Throw();
            break;
          default:
            throw new SearchException(SearchWebApiResources.UnexpectedSearchErrorMessage);
        }
      }
      return response;
    }

    internal override void PublishRequest(EntitySearchQuery request) => Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("REST-API", "Query Pipeline", "TWSSearchFilters", (object) string.Join<SearchFilter>(" | ", (request as WikiSearchQuery).Filters));

    protected override void PublishResponse(EntitySearchResponse response)
    {
      base.PublishResponse(response);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("REST-API", "Query Pipeline", "TWSFacets", (object) string.Join<FilterCategory>(" | ", (response as WikiQueryResponse).FilterCategories));
    }

    private void ProcessQuery(WikiSearchQuery query, IVssRequestContext requestContext)
    {
      string collectionName = requestContext.GetCollectionName();
      query.SearchFilters = (IDictionary<string, IEnumerable<string>>) new Dictionary<string, IEnumerable<string>>()
      {
        {
          "CollectionFilters",
          (IEnumerable<string>) new List<string>()
          {
            collectionName
          }
        }
      };
      query.TakeResults = 0;
    }
  }
}
