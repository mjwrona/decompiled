// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.WikiSearchControllerBase
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Constants;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders;
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService;
using Microsoft.VisualStudio.Services.Search.Query.SecurityScopeFilterService;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  public abstract class WikiSearchControllerBase : SearchControllerBase
  {
    private ISearchQueryForwarder<WikiSearchQuery, WikiQueryResponse> m_wikiSearchQueryForwarder;
    private readonly ISearchSecurityScopeFilterExpressionBuilder m_searchSecurityScopeFilterExpressionBuilder;
    private const string WikiOnboardingIsTriggeredRegistryPrefix = "WikiOnboardingTriggered";

    protected WikiSearchControllerBase()
    {
      this.IndexMapper = (IIndexMapper) new Microsoft.VisualStudio.Services.Search.Query.IndexMapper((IEntityType) WikiEntityType.GetInstance());
      this.m_searchSecurityScopeFilterExpressionBuilder = (ISearchSecurityScopeFilterExpressionBuilder) new WikiSearchSecurityScopeFilterExpressionBuilder();
    }

    protected WikiSearchControllerBase(
      IIndexMapper indexMapper,
      ISearchQueryForwarder<WikiSearchQuery, WikiQueryResponse> wikiSearchQueryForwarder)
    {
      this.IndexMapper = indexMapper;
      this.m_wikiSearchQueryForwarder = wikiSearchQueryForwarder;
      this.m_searchSecurityScopeFilterExpressionBuilder = (ISearchSecurityScopeFilterExpressionBuilder) new WikiSearchSecurityScopeFilterExpressionBuilder();
    }

    protected override void InitializeInternal(HttpControllerContext controllerContext)
    {
      base.InitializeInternal(controllerContext);
      this.InitializeQueryForwarder(this.TfsRequestContext);
    }

    internal override void InitializeSearchTextLimitCap(IVssRequestContext requestContext) => this.SearchTextLimitCap = requestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/WikiSearchMaxSearchTextLength", 15000);

    internal void InitializeQueryForwarder(IVssRequestContext requestContext)
    {
      if (this.m_wikiSearchQueryForwarder != null)
        return;
      this.m_wikiSearchQueryForwarder = (ISearchQueryForwarder<WikiSearchQuery, WikiQueryResponse>) new WikiSearchQueryForwarder(this.IndexMapper.GetESConnectionString(requestContext), requestContext.GetElasticsearchPlatformSettings("/Service/ALMSearch/Settings/ATSearchPlatformSettings"), SearchOptions.Highlighting | SearchOptions.Faceting | SearchOptions.Ranking | SearchOptions.Rescore, requestContext.ExecutionEnvironment.IsOnPremisesDeployment, new WikiEntityIndexProvider<WikiContract>(), new WikiSearchPlatformRequest.Builder());
    }

    protected WikiQueryResponse HandlePostWikiQueryRequest(
      IVssRequestContext requestContext,
      WikiSearchQuery request,
      ProjectInfo projectInfo = null)
    {
      Stopwatch e2eQueryTimer = Stopwatch.StartNew();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfQueryRequests", "Query Pipeline", 1.0, true);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("EntityType", "Query Pipeline", (double) WikiEntityType.GetInstance().ID, true);
      if (request == null)
        throw new InvalidQueryException(SearchWebApiResources.NullQueryMessage);
      WikiQueryResponse response = (WikiQueryResponse) null;
      try
      {
        if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          this.OnboardCollectionIfNotAlreadyDone(requestContext);
        this.HandleNullProperties((EntitySearchQuery) request);
        this.ValidateQuery((EntitySearchQuery) request, requestContext, projectInfo);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080093, "REST-API", "REST-API", request.ToString());
        this.PublishRequest((EntitySearchQuery) request);
        if (this.EnableSkipTakeOverride)
        {
          request.SkipResults = this.SkipResults;
          request.TakeResults = this.TakeResults;
        }
        DocumentContractType documentContractType = this.IndexMapper.GetDocumentContractType(requestContext);
        requestContext.GetService<IWikiSecurityChecksService>().ValidateAndSetUserPermissionsForSearchService(requestContext);
        bool noResultAccessible;
        IExpression filterExpression = this.m_searchSecurityScopeFilterExpressionBuilder.GetScopeFilterExpression(requestContext, this.EnableSecurityChecksInQueryPipeline, out noResultAccessible, projectInfo);
        IEnumerable<IndexInfo> indexInfo = this.IndexMapper.GetIndexInfo(requestContext);
        if (noResultAccessible)
        {
          response = this.m_wikiSearchQueryForwarder.GetZeroResultResponse(request);
          response.AddError(new ErrorData()
          {
            ErrorCode = "WorkItemsNotAccessible",
            ErrorType = ErrorType.Warning
          });
        }
        else
          response = this.m_wikiSearchQueryForwarder.ForwardSearchRequest(requestContext, request, indexInfo, filterExpression, requestContext.ActivityId.ToString(), documentContractType);
        this.SetIndexingStatusInResponse((EntitySearchResponse) response, this.GetIndexingStatus(requestContext, (IEntityType) WikiEntityType.GetInstance()));
        this.PopulateSearchSecuredObjectInResponse(requestContext, (EntitySearchResponse) response);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080093, "REST-API", "REST-API", response.ToString());
        this.PublishResponse((EntitySearchResponse) response);
        e2eQueryTimer.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("E2EQueryTime", "Query Pipeline", (double) e2eQueryTimer.ElapsedMilliseconds, true);
        PerformanceTimer.SendCustomerIntelligenceData(requestContext, (Action<CustomerIntelligenceData>) (ciData =>
        {
          ciData.Add("Timings", requestContext.GetTraceTimingAsString());
          ciData.Add("ElapsedMillis", requestContext.LastTracedBlockElapsedMilliseconds());
          ciData.Add("NoOfWikiSearchResults", (double) response.Results.Values.Count<WikiResult>());
          ciData.Add("E2EQueryTime", (double) e2eQueryTimer.ElapsedMilliseconds);
        }));
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfFailedQueryRequests", "Query Pipeline", 1.0, true);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1080094, "REST-API", "REST-API", ex);
        SearchPlatformExceptionLogger.LogSearchPlatformException(ex);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishOnPremiseIndicator("TFS/Search/Query");
      }
      return response;
    }

    private void OnboardCollectionIfNotAlreadyDone(IVssRequestContext requestContext)
    {
      if (WikiSearchControllerBase.IsOnboardingTriggered(requestContext))
        return;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext, WikiSearchControllerBase.\u003C\u003EO.\u003C0\u003E__FaultInJobQueueCallback ?? (WikiSearchControllerBase.\u003C\u003EO.\u003C0\u003E__FaultInJobQueueCallback = new TeamFoundationTaskCallback(WikiSearchControllerBase.FaultInJobQueueCallback)));
    }

    private static void FaultInJobQueueCallback(IVssRequestContext requestContext, object args)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      try
      {
        requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().SetFeatureState(requestContext, "Search.Server.Wiki.Indexing", FeatureAvailabilityState.On);
        TeamFoundationJobService service = requestContext.GetService<TeamFoundationJobService>();
        if (WikiSearchControllerBase.IsFaultInJobQueued(requestContext, (ITeamFoundationJobService) service))
          return;
        if (WikiSearchControllerBase.CollectionIndexingUnitExists(requestContext))
          WikiSearchControllerBase.RecordOnboardingWasTriggered(requestContext);
        else
          WikiSearchControllerBase.QueueFaultInJob(requestContext, (ITeamFoundationJobService) service);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080095, "REST-API", "REST-API", FormattableString.Invariant(FormattableStringFactory.Create("Failed to queue wiki account fault-in job. It will be retried on next search attempt. Exception [{0}].", (object) ex)));
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    private static bool IsFaultInJobQueued(
      IVssRequestContext requestContext,
      ITeamFoundationJobService jobService)
    {
      List<TeamFoundationJobQueueEntry> foundationJobQueueEntryList = jobService.QueryJobQueue(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        JobConstants.WikiAccountFaultInJobId
      });
      return foundationJobQueueEntryList != null && foundationJobQueueEntryList.Count == 1 && foundationJobQueueEntryList[0] != null;
    }

    private static bool CollectionIndexingUnitExists(IVssRequestContext requestContext) => DataAccessFactory.GetInstance().GetIndexingUnitDataAccess().GetIndexingUnit(requestContext, requestContext.GetCollectionID(), "Collection", (IEntityType) WikiEntityType.GetInstance()) != null;

    private static void QueueFaultInJob(
      IVssRequestContext requestContext,
      ITeamFoundationJobService jobService)
    {
      if (jobService.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        JobConstants.WikiAccountFaultInJobId
      }, true) == 1)
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080095, "REST-API", "REST-API", "Wiki account fault-in job got queued successfully.");
      else
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080095, "REST-API", "REST-API", "Wiki account fault-in job did not get queued. It will be retried on next search attempt.");
    }

    private static bool IsOnboardingTriggered(IVssRequestContext requestContext) => new RegistryManagerV2(requestContext, "Common").GetRegistryEntry("WikiOnboardingTriggered", requestContext.GetCollectionID().ToString()) != null;

    private static void RecordOnboardingWasTriggered(IVssRequestContext requestContext) => new RegistryManagerV2(requestContext, "Common").AddOrUpdateRegistryValue("WikiOnboardingTriggered", requestContext.GetCollectionID().ToString(), true.ToString((IFormatProvider) CultureInfo.InvariantCulture));

    internal override void PublishRequest(EntitySearchQuery request)
    {
      WikiSearchQuery wikiSearchQuery = request as WikiSearchQuery;
      FriendlyDictionary<string, object> properties = new FriendlyDictionary<string, object>()
      {
        {
          "WikiSSkip",
          (object) wikiSearchQuery.SkipResults
        },
        {
          "WikiSTake",
          (object) wikiSearchQuery.TakeResults
        }
      };
      if (wikiSearchQuery.SearchFilters != null)
      {
        properties.Add("WikiSSearchFilters", (object) string.Join(" | ", wikiSearchQuery.SearchFilters.Select<KeyValuePair<string, IEnumerable<string>>, string>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (kvp => FormattableString.Invariant(FormattableStringFactory.Create("Name = {0}; Count = {1}", (object) kvp.Key, (object) kvp.Value.Count<string>()))))));
        foreach (KeyValuePair<string, IEnumerable<string>> searchFilter in (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) wikiSearchQuery.SearchFilters)
          properties[searchFilter.Key] = (object) string.Join(",", searchFilter.Value);
      }
      properties["WikiSSearchText"] = wikiSearchQuery.SearchText != null ? (object) wikiSearchQuery.SearchText : (object) string.Empty;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("REST-API", "Query Pipeline", (IDictionary<string, object>) properties);
    }

    protected override void PublishResponse(EntitySearchResponse response)
    {
      base.PublishResponse(response);
      WikiQueryResponse wikiQueryResponse = response as WikiQueryResponse;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("REST-API", "Query Pipeline", (IDictionary<string, object>) new FriendlyDictionary<string, object>()
      {
        ["WikiSTotalMatches"] = (object) wikiQueryResponse.Results.Count,
        ["WikiSFacets"] = (object) string.Join<FilterCategory>(" | ", wikiQueryResponse.FilterCategories)
      });
    }
  }
}
