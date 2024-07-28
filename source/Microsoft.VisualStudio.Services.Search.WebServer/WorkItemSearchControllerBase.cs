// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.WorkItemSearchControllerBase
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Constants;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem;
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.WorkItemFieldCache;
using Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  public abstract class WorkItemSearchControllerBase : SearchControllerBase
  {
    private ISearchQueryForwarder<WorkItemSearchRequest, WorkItemSearchResponse> m_workItemSearchQueryForwarder;

    protected WorkItemSearchControllerBase() => this.IndexMapper = (IIndexMapper) new Microsoft.VisualStudio.Services.Search.Query.IndexMapper((IEntityType) WorkItemEntityType.GetInstance());

    protected WorkItemSearchControllerBase(
      IIndexMapper indexMapper,
      ISearchQueryForwarder<WorkItemSearchRequest, WorkItemSearchResponse> workItemSearchQueryForwarder)
    {
      this.IndexMapper = indexMapper;
      this.m_workItemSearchQueryForwarder = workItemSearchQueryForwarder;
    }

    protected override void InitializeInternal(HttpControllerContext controllerContext)
    {
      base.InitializeInternal(controllerContext);
      this.InitializeQueryForwarder(this.TfsRequestContext);
    }

    internal void InitializeQueryForwarder(IVssRequestContext requestContext)
    {
      if (this.m_workItemSearchQueryForwarder != null)
        return;
      this.m_workItemSearchQueryForwarder = (ISearchQueryForwarder<WorkItemSearchRequest, WorkItemSearchResponse>) new WorkItemSearchQueryForwarder(this.IndexMapper.GetESConnectionString(requestContext), requestContext.GetElasticsearchPlatformSettings("/Service/ALMSearch/Settings/ATSearchPlatformSettings"), SearchOptions.Highlighting | SearchOptions.Faceting | SearchOptions.Ranking | SearchOptions.Rescore, requestContext.ExecutionEnvironment.IsOnPremisesDeployment);
    }

    protected WorkItemSearchResponse HandlePostWorkItemQueryRequest(
      IVssRequestContext requestContext,
      WorkItemSearchRequest request,
      ProjectInfo projectInfo = null)
    {
      Stopwatch e2eQueryTimer = Stopwatch.StartNew();
      bool flag1 = request != null ? request.IsInstantSearch : throw new InvalidQueryException(SearchWebApiResources.NullQueryMessage);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfQueryRequests", "Query Pipeline", 1.0, true);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("EntityType", "Query Pipeline", (double) WorkItemEntityType.GetInstance().ID, true);
      if (flag1)
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("IsInstantSearch", "Query Pipeline", 1.0, true);
      this.HandleNullProperties((EntitySearchQuery) request);
      WorkItemSearchResponse response = (WorkItemSearchResponse) null;
      try
      {
        IWorkItemSecurityChecksService service = requestContext.GetService<IWorkItemSecurityChecksService>();
        if (this.IsWarmerRequest(request))
        {
          service.ValidateAndSetUserPermissionsForSearchService(requestContext);
          service.PopulateUserSecurityChecksDataInRequestContext(requestContext);
          if (requestContext.IsFeatureEnabled("Search.Server.WorkItem.RecentActivityCacheEnabled") && projectInfo != null)
            requestContext.GetService<RecentActivityDataProviderService>().PopulateUserRecencyDataInCache(requestContext, projectInfo.Id);
          response = this.GetEmptyWarmerResponse(requestContext, request);
        }
        else
        {
          this.ValidateQuery((EntitySearchQuery) request, requestContext, projectInfo);
          if (!flag1 && !requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
            this.OnboardCollectionIfNotAlreadyDone(requestContext, request.SearchText);
          this.PublishRequest((EntitySearchQuery) request);
          if (this.EnableSkipTakeOverride)
          {
            request.SkipResults = this.SkipResults;
            request.TakeResults = this.TakeResults;
          }
          DocumentContractType documentContractType = this.IndexMapper.GetDocumentContractType(requestContext);
          Stopwatch stopwatch = Stopwatch.StartNew();
          bool flag2 = false;
          IEnumerable<ClassificationNode> classificationNodes;
          try
          {
            service.ValidateAndSetUserPermissionsForSearchService(requestContext);
            bool allAreasAreAccessible;
            IEnumerable<ClassificationNode> userAccessibleAreas = service.GetUserAccessibleAreas(requestContext, out allAreasAreAccessible);
            classificationNodes = allAreasAreAccessible ? (IEnumerable<ClassificationNode>) null : userAccessibleAreas;
            if (classificationNodes != null)
              flag2 = !classificationNodes.Any<ClassificationNode>();
          }
          finally
          {
            stopwatch.Stop();
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("WitSecurityChecksTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
          }
          IExpression filtersExpression = this.CreateScopeFiltersExpression(requestContext, classificationNodes, projectInfo);
          IEnumerable<IndexInfo> indexInfo = this.IndexMapper.GetIndexInfo(requestContext);
          requestContext.RootContext.Items[Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Constants.WorkItemRecencyData.ProjectId] = (object) (projectInfo == null ? Guid.Empty : projectInfo.Id);
          response = this.m_workItemSearchQueryForwarder.ForwardSearchRequest(requestContext, request, indexInfo, filtersExpression, requestContext.ActivityId.ToString(), documentContractType);
          WorkItemSearchControllerBase.SetFieldDisplayNamesInResponse(response, requestContext);
          if (!flag1)
            this.SetIndexingStatusInResponse((EntitySearchResponse) response, this.GetIndexingStatus(requestContext, (IEntityType) WorkItemEntityType.GetInstance()));
          if (flag2)
            response.AddError(new ErrorData()
            {
              ErrorCode = "WorkItemsNotAccessible",
              ErrorType = ErrorType.Warning
            });
          service.ScrubEmailsFromIdentityFieldsForAnonymousPublicUsers(requestContext, response);
        }
        this.PopulateSearchSecuredObjectInResponse(requestContext, (EntitySearchResponse) response);
        this.PublishResponse((EntitySearchResponse) response);
        if (!flag1)
          PerformanceTimer.SendCustomerIntelligenceData(requestContext, (Action<CustomerIntelligenceData>) (ciData =>
          {
            ciData.Add("Timings", requestContext.GetTraceTimingAsString());
            ciData.Add("ElapsedMillis", requestContext.LastTracedBlockElapsedMilliseconds());
            ciData.Add("NoOfCodeSearchResultsAfterTrimming", (double) response.Results.Values.Count<WorkItemResult>());
            ciData.Add("E2EQueryTime", (double) e2eQueryTimer.ElapsedMilliseconds);
            ciData.Add("NumberOfHighlights", (double) response.Results.Values.AsParallel<WorkItemResult>().Sum<WorkItemResult>((Func<WorkItemResult, int>) (result => result.Hits.Count<WorkItemHit>())));
          }));
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfFailedQueryRequests", "Query Pipeline", 1.0, true);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1080013, "REST-API", "REST-API", ex);
        SearchPlatformExceptionLogger.LogSearchPlatformException(ex);
      }
      finally
      {
        e2eQueryTimer.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("E2EQueryTime", "Query Pipeline", (double) e2eQueryTimer.ElapsedMilliseconds, true);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishOnPremiseIndicator("TFS/Search/Query");
      }
      return response;
    }

    private WorkItemSearchResponse GetEmptyWarmerResponse(
      IVssRequestContext requestContext,
      WorkItemSearchRequest request)
    {
      WorkItemSearchResponse emptyWarmerResponse = new WorkItemSearchResponse()
      {
        Query = request,
        Results = new WorkItemResults()
        {
          Values = (IEnumerable<WorkItemResult>) new List<WorkItemResult>()
        }
      };
      if (requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/StopWarmerRequests", TeamFoundationHostType.Deployment))
        emptyWarmerResponse.AddError(new ErrorData()
        {
          ErrorCode = "StopWarmerRequests",
          ErrorType = ErrorType.Warning
        });
      return emptyWarmerResponse;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsWarmerRequest(WorkItemSearchRequest request) => request.TakeResults == 0 && !request.SummarizedHitCountsNeeded;

    protected IExpression CreateScopeFiltersExpression(
      IVssRequestContext requestContext,
      IEnumerable<ClassificationNode> userAccessibleAreas,
      ProjectInfo projectInfo)
    {
      List<IExpression> expressionList = new List<IExpression>();
      expressionList.Add((IExpression) new TermsExpression("collectionId", Operator.In, (IEnumerable<string>) new string[1]
      {
        requestContext.GetCollectionID().ToString().ToLowerInvariant()
      }));
      if (projectInfo != null)
      {
        IExpression expression = (IExpression) new TermsExpression("projectId", Operator.In, (IEnumerable<string>) new string[1]
        {
          projectInfo.Id.ToString().ToLowerInvariant()
        });
        expressionList.Add(expression);
      }
      if (userAccessibleAreas != null)
        expressionList.Add((IExpression) new TermsExpression(WorkItemContract.PlatformFieldNames.AreaId, Operator.In, userAccessibleAreas.Select<ClassificationNode, string>((Func<ClassificationNode, string>) (x => x.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture)))));
      string currentHostConfigValue = requestContext.GetCurrentHostConfigValue<string>("/Service/SearchShared/Settings/SoftDeletedProjectIds");
      if (!string.IsNullOrWhiteSpace(currentHostConfigValue))
        expressionList.Add((IExpression) new NotExpression((IExpression) new TermsExpression("projectId", Operator.In, (IEnumerable<string>) ((IEnumerable<string>) currentHostConfigValue.Split(',')).Select<string, string>((Func<string, string>) (i => i.Trim())).Where<string>((Func<string, bool>) (i => !string.IsNullOrEmpty(i))).ToList<string>())));
      if (requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/WorkItemQueryFilterPerfFix"))
      {
        expressionList.AddRange((IEnumerable<IExpression>) this.GetFiltersToIgnoreDeletedAndDiscussionsOnlyDocuments());
        return expressionList.Count != 1 ? (IExpression) new AndExpression((IEnumerable<IExpression>) expressionList) : expressionList[0];
      }
      expressionList.Add(this.CreateExpressionToIgnoreDeletedAndDiscussionsOnlyDocuments());
      return expressionList.Count != 1 ? expressionList.Aggregate<IExpression>((System.Func<IExpression, IExpression, IExpression>) ((current, filter) => (IExpression) new AndExpression(new IExpression[2]
      {
        current,
        filter
      }))) : expressionList[0];
    }

    private List<IExpression> GetFiltersToIgnoreDeletedAndDiscussionsOnlyDocuments()
    {
      List<IExpression> discussionsOnlyDocuments = new List<IExpression>();
      TermsExpression termsExpression1 = new TermsExpression("isDiscussionOnly", Operator.In, (IEnumerable<string>) new string[1]
      {
        bool.FalseString.ToLowerInvariant()
      });
      TermsExpression termsExpression2 = new TermsExpression(WorkItemContract.PlatformFieldNames.IsDeleted, Operator.In, (IEnumerable<string>) new string[1]
      {
        bool.FalseString.ToLowerInvariant()
      });
      discussionsOnlyDocuments.Add((IExpression) termsExpression1);
      discussionsOnlyDocuments.Add((IExpression) termsExpression2);
      return discussionsOnlyDocuments;
    }

    private IExpression CreateExpressionToIgnoreDeletedAndDiscussionsOnlyDocuments()
    {
      MissingFieldExpression missingFieldExpression1 = new MissingFieldExpression("isDiscussionOnly");
      TermsExpression termsExpression1 = new TermsExpression("isDiscussionOnly", Operator.In, (IEnumerable<string>) new string[1]
      {
        bool.FalseString.ToLowerInvariant()
      });
      TermsExpression termsExpression2 = new TermsExpression(WorkItemContract.PlatformFieldNames.IsDeleted, Operator.In, (IEnumerable<string>) new string[1]
      {
        bool.FalseString.ToLowerInvariant()
      });
      MissingFieldExpression missingFieldExpression2 = new MissingFieldExpression(WorkItemContract.PlatformFieldNames.IsDeleted);
      return (IExpression) new AndExpression(new IExpression[2]
      {
        (IExpression) new OrExpression(new IExpression[2]
        {
          (IExpression) termsExpression1,
          (IExpression) missingFieldExpression1
        }),
        (IExpression) new OrExpression(new IExpression[2]
        {
          (IExpression) missingFieldExpression2,
          (IExpression) termsExpression2
        })
      });
    }

    private static void SetFieldDisplayNamesInResponse(
      WorkItemSearchResponse searchResponse,
      IVssRequestContext requestContext)
    {
      IWorkItemFieldCacheService service = requestContext.GetService<IWorkItemFieldCacheService>();
      foreach (WorkItemResult workItemResult in searchResponse.Results.Values)
      {
        Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemField fieldData;
        foreach (Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.WorkItemField field in workItemResult.Fields)
        {
          if (service.TryGetFieldByRefName(requestContext, field.ReferenceName, out fieldData))
          {
            field.Name = fieldData.Name;
          }
          else
          {
            field.Name = WorkItemSearchControllerBase.GetLastNodeName(field.ReferenceName);
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080018, "REST-API", "REST-API", FormattableString.Invariant(FormattableStringFactory.Create("The field Refname:{0} was not found in the field cache while resolving fields in the results.", (object) field.ReferenceName)));
          }
        }
        foreach (WorkItemHit hit in workItemResult.Hits)
        {
          if (service.TryGetFieldByRefName(requestContext, hit.FieldReferenceName, out fieldData))
          {
            hit.FieldName = fieldData.Name;
          }
          else
          {
            hit.FieldName = WorkItemSearchControllerBase.GetLastNodeName(hit.FieldReferenceName);
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080019, "REST-API", "REST-API", FormattableString.Invariant(FormattableStringFactory.Create("The field Refname:{0} was not found in the field cache while resolving fields in the hits.", (object) hit.FieldReferenceName)));
          }
        }
      }
    }

    internal override void PublishRequest(EntitySearchQuery request)
    {
      WorkItemSearchRequest itemSearchRequest = request as WorkItemSearchRequest;
      FriendlyDictionary<string, object> properties = new FriendlyDictionary<string, object>()
      {
        ["WISSkip"] = (object) itemSearchRequest.SkipResults,
        ["WISTake"] = (object) itemSearchRequest.TakeResults
      };
      if (request.Filters != null)
      {
        properties["WISSearchFilters"] = (object) string.Join<SearchFilter>(" | ", request.Filters);
        foreach (SearchFilter filter in request.Filters)
          properties[filter.Name] = (object) string.Join(",", filter.Values);
      }
      properties["WISearchText"] = itemSearchRequest.SearchText != null ? (object) itemSearchRequest.SearchText : (object) string.Empty;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("REST-API", "Query Pipeline", (IDictionary<string, object>) properties);
    }

    protected override void PublishResponse(EntitySearchResponse response)
    {
      base.PublishResponse(response);
      WorkItemSearchResponse itemSearchResponse = response as WorkItemSearchResponse;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("REST-API", "Query Pipeline", (IDictionary<string, object>) new FriendlyDictionary<string, object>()
      {
        ["WISTotalMatches"] = (object) itemSearchResponse.Results.Count,
        ["WISFacets"] = (itemSearchResponse.Query.SummarizedHitCountsNeeded ? (object) string.Join<FilterCategory>(" | ", itemSearchResponse.FilterCategories) : (object) ""),
        ["CountOfHighlightedFields"] = (object) itemSearchResponse.Results.Values.AsParallel<WorkItemResult>().Sum<WorkItemResult>((Func<WorkItemResult, int>) (result => result.Hits.Count<WorkItemHit>()))
      });
    }

    private static string GetLastNodeName(string referenceName)
    {
      int num = referenceName.LastIndexOf('.');
      return num < 0 ? referenceName : referenceName.Substring(num + 1);
    }

    private void OnboardCollectionIfNotAlreadyDone(
      IVssRequestContext requestContext,
      string searchText)
    {
      if (WorkItemCollectionOnboardingHelper.IsOnboardingTriggered(requestContext))
        return;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishCi("REST-API", "REST-API", (IDictionary<string, object>) new Dictionary<string, object>()
      {
        ["OnboardingTrigger"] = (searchText == "id=0" ? (object) "ConditionalFaultIn" : (object) "FirstSearch")
      });
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext, WorkItemSearchControllerBase.\u003C\u003EO.\u003C0\u003E__FaultInJobQueueCallback ?? (WorkItemSearchControllerBase.\u003C\u003EO.\u003C0\u003E__FaultInJobQueueCallback = new TeamFoundationTaskCallback(WorkItemSearchControllerBase.FaultInJobQueueCallback)));
    }

    private static void FaultInJobQueueCallback(IVssRequestContext requestContext, object args)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      try
      {
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        vssRequestContext.GetService<ITeamFoundationFeatureAvailabilityService>().SetFeatureState(vssRequestContext, "Search.Server.WorkItem.Indexing", FeatureAvailabilityState.On);
        TeamFoundationJobService service = requestContext.GetService<TeamFoundationJobService>();
        if (WorkItemSearchControllerBase.IsFaultInJobQueued(requestContext, (ITeamFoundationJobService) service))
          return;
        if (WorkItemSearchControllerBase.CollectionIndexingUnitExists(requestContext))
          WorkItemCollectionOnboardingHelper.RecordOnboardingWasTriggered(requestContext);
        else
          WorkItemSearchControllerBase.QueueFaultInJob(requestContext, (ITeamFoundationJobService) service);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080082, "REST-API", "REST-API", FormattableString.Invariant(FormattableStringFactory.Create("Failed to queue work item account fault-in job. It will be retried on next search attempt. Exception [{0}].", (object) ex)));
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    private static void QueueFaultInJob(
      IVssRequestContext requestContext,
      ITeamFoundationJobService jobService)
    {
      if (jobService.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        JobConstants.WorkItemAccountFaultInJobId
      }, true) == 1)
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceAlways(1080082, TraceLevel.Info, "REST-API", "REST-API", "Work item account fault-in job got queued successfully.");
      else
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080082, "REST-API", "REST-API", "Work item account fault-in job did not get queued. It will be retried on next search attempt.");
    }

    private static bool CollectionIndexingUnitExists(IVssRequestContext requestContext) => DataAccessFactory.GetInstance().GetIndexingUnitDataAccess().GetIndexingUnit(requestContext, requestContext.GetCollectionID(), "Collection", (IEntityType) WorkItemEntityType.GetInstance()) != null;

    private static bool IsFaultInJobQueued(
      IVssRequestContext requestContext,
      ITeamFoundationJobService jobService)
    {
      List<TeamFoundationJobQueueEntry> foundationJobQueueEntryList = jobService.QueryJobQueue(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        JobConstants.WorkItemAccountFaultInJobId
      });
      return foundationJobQueueEntryList != null && foundationJobQueueEntryList.Count == 1 && foundationJobQueueEntryList[0] != null;
    }
  }
}
