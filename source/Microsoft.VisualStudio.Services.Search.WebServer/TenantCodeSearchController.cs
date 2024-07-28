// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.TenantCodeSearchController
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
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
  [VersionedApiControllerCustomName(Area = "search", ResourceName = "tenantCodeQueryResults")]
  public class TenantCodeSearchController : CodeSearchControllerBase
  {
    private const string DefaultTenantName = "Microsoft";
    private ISearchQueryForwarder<SearchQuery, CodeQueryResponse> m_tenantCodeSearchQueryForwarder;

    public TenantCodeSearchController() => this.IndexMapper = (IIndexMapper) new Microsoft.VisualStudio.Services.Search.Query.IndexMapper((IEntityType) TenantCodeEntityType.GetInstance());

    protected TenantCodeSearchController(
      IIndexMapper indexMapper,
      ISearchQueryForwarder<SearchQuery, CodeQueryResponse> tenantCodeSearchQueryForwarder)
    {
      this.IndexMapper = indexMapper;
      this.m_tenantCodeSearchQueryForwarder = tenantCodeSearchQueryForwarder;
    }

    protected override void InitializeInternal(HttpControllerContext controllerContext)
    {
      base.InitializeInternal(controllerContext);
      if (this.m_tenantCodeSearchQueryForwarder != null)
        return;
      SearchOptions searchOptions = this.GetSearchOptions(this.TfsRequestContext) & ~SearchOptions.Highlighting & ~SearchOptions.Ranking;
      this.m_tenantCodeSearchQueryForwarder = (ISearchQueryForwarder<SearchQuery, CodeQueryResponse>) new CodeSearchQueryForwarder(this.TfsRequestContext.GetConfigValue("/Service/ALMSearch/Settings/TenantSearchPlatformConnectionString"), this.TfsRequestContext.GetElasticsearchPlatformSettings("/Service/ALMSearch/Settings/CustomSearchPlatformSettings", "ConnectionTimeout=180"), searchOptions, this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment);
    }

    [HttpPost]
    [ClientLocationId("21A1F7F9-8DB1-4F7E-8CFB-4AE78E972088")]
    public CodeQueryResponse PostTenantCodeQuery(SearchQuery query)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080070, "REST-API", "REST-API", nameof (PostTenantCodeQuery));
      try
      {
        return this.HandlePostTenantCodeQueryRequest(this.TfsRequestContext, query);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080071, "REST-API", "REST-API", nameof (PostTenantCodeQuery));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    protected CodeQueryResponse HandlePostTenantCodeQueryRequest(
      IVssRequestContext requestContext,
      SearchQuery query)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfTenantQueryRequests", "Query Pipeline", 1.0);
      CodeQueryResponse response = (CodeQueryResponse) null;
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
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080073, "REST-API", "REST-API", query.ToString());
        this.PublishRequest((EntitySearchQuery) query);
        IEnumerable<IndexInfo> tenantCodeQueryAlias = this.IndexMapper.GetTenantCodeQueryAlias("Microsoft");
        DocumentContractType contractType = this.TfsRequestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/IsCustomTenantReindexingComplete") ? (DocumentContractType) Enum.Parse(typeof (DocumentContractType), requestContext.GetConfigValue("/Service/ALMSearch/Settings/DefaultCodeDocumentContractType")) : DocumentContractType.SourceNoDedupeFileContractV3;
        IExpression filtersExpression = this.GetScopeFiltersExpression(contractType);
        response = this.m_tenantCodeSearchQueryForwarder.ForwardSearchRequest(this.TfsRequestContext, query, tenantCodeQueryAlias, filtersExpression, requestContext.ActivityId.ToString(), contractType);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080073, "REST-API", "REST-API", response.ToString());
        this.PublishResponse((EntitySearchResponse) response);
        e2eQueryTimer.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("E2ETenantQueryTime", "Query Pipeline", (double) e2eQueryTimer.ElapsedMilliseconds);
        PerformanceTimer.SendCustomerIntelligenceData(this.TfsRequestContext, (Action<CustomerIntelligenceData>) (ciData =>
        {
          ciData.Add("Timings", this.TfsRequestContext.GetTraceTimingAsString());
          ciData.Add("ElapsedMillis", this.TfsRequestContext.LastTracedBlockElapsedMilliseconds());
          ciData.Add("NoOfCodeSearchResultsAfterTrimming", (double) response.Results.Values.Count<CodeResult>());
          ciData.Add("E2ETenantQueryTime", (double) e2eQueryTimer.ElapsedMilliseconds);
        }));
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfFailedTenantQueryRequests", "Query Pipeline", 1.0);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1080072, "REST-API", "REST-API", ex);
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

    protected IExpression GetScopeFiltersExpression(DocumentContractType contractType) => CodeFileContract.MultiBranchSupportedContracts.Contains(contractType) ? (IExpression) new TermExpression(CodeFileContract.CodeContractQueryableElement.IsDefaultBranch.InlineFilterName(), Operator.Equals, "true") : (IExpression) new EmptyExpression();

    internal override void PublishRequest(EntitySearchQuery request) => Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("REST-API", "Query Pipeline", "TCSSearchFilters", (object) string.Join<SearchFilter>(" | ", (request as SearchQuery).Filters));

    protected override void PublishResponse(EntitySearchResponse response)
    {
      base.PublishResponse(response);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("REST-API", "Query Pipeline", "TCSFacets", (object) string.Join<FilterCategory>(" | ", (response as CodeQueryResponse).FilterCategories));
    }

    private void ProcessQuery(SearchQuery query, IVssRequestContext requestContext)
    {
      List<SearchFilter> searchFilterList = new List<SearchFilter>();
      string collectionName = requestContext.GetCollectionName();
      searchFilterList.Add(new SearchFilter()
      {
        Name = "CollectionFilters",
        Values = (IEnumerable<string>) new List<string>()
        {
          collectionName
        }
      });
      IEnumerable<SearchFilter> filters = query.Filters;
      SearchFilter searchFilter1;
      if (filters == null)
      {
        searchFilter1 = (SearchFilter) null;
      }
      else
      {
        IEnumerable<SearchFilter> source = filters.Where<SearchFilter>((Func<SearchFilter, bool>) (filter => filter.Name.Equals("CodeElementFilters")));
        searchFilter1 = source != null ? source.FirstOrDefault<SearchFilter>() : (SearchFilter) null;
      }
      SearchFilter searchFilter2 = searchFilter1;
      if (searchFilter2 != null)
        searchFilterList.Add(searchFilter2);
      query.Filters = (IEnumerable<SearchFilter>) searchFilterList;
      query.TakeResults = 0;
    }
  }
}
