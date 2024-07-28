// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.BoardSearchControllerBase
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
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Search.WebApi;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Board;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  public abstract class BoardSearchControllerBase : SearchV2ControllerBase
  {
    private ISearchRequestForwarder<BoardSearchRequest, BoardSearchResponse> m_boardSearchRequestForwarder;
    private const string BoardOnboardingIsTriggeredRegistryPrefix = "BoardOnboardingTriggered";

    protected BoardSearchControllerBase() => this.IndexMapper = (IIndexMapper) new Microsoft.VisualStudio.Services.Search.Query.IndexMapper((IEntityType) BoardEntityType.GetInstance());

    protected BoardSearchControllerBase(
      IIndexMapper indexMapper,
      ISearchRequestForwarder<BoardSearchRequest, BoardSearchResponse> boardSearchRequestForwarder)
    {
      this.IndexMapper = indexMapper;
      this.m_boardSearchRequestForwarder = boardSearchRequestForwarder;
    }

    protected override void InitializeInternal(HttpControllerContext controllerContext)
    {
      base.InitializeInternal(controllerContext);
      this.InitializeRequestForwarder(this.TfsRequestContext);
    }

    internal override void InitializeSearchTextLimitCap(IVssRequestContext requestContext) => this.SearchTextLimitCap = requestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/MaxSearchTextLength", 1024);

    internal void InitializeRequestForwarder(IVssRequestContext requestContext)
    {
      if (this.m_boardSearchRequestForwarder != null)
        return;
      this.m_boardSearchRequestForwarder = (ISearchRequestForwarder<BoardSearchRequest, BoardSearchResponse>) new BoardSearchRequestForwarder(this.IndexMapper.GetESConnectionString(requestContext), requestContext.GetElasticsearchPlatformSettings("/Service/ALMSearch/Settings/ATSearchPlatformSettings"), SearchOptions.Highlighting | SearchOptions.Faceting | SearchOptions.Ranking | SearchOptions.Rescore, requestContext.ExecutionEnvironment.IsOnPremisesDeployment);
    }

    protected BoardSearchResponse HandlePostBoardQueryRequest(
      IVssRequestContext requestContext,
      BoardSearchRequest request,
      ProjectInfo projectInfo = null)
    {
      Stopwatch e2eRequestTimer = Stopwatch.StartNew();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfQueryRequests", "Query Pipeline", 1.0, true);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("EntityType", "Query Pipeline", (double) BoardEntityType.GetInstance().ID, true);
      if (request == null)
        throw new InvalidQueryException(SearchWebApiResources.NullQueryMessage);
      BoardSearchResponse response = (BoardSearchResponse) null;
      try
      {
        if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          this.OnboardCollectionIfNotAlreadyDone(requestContext);
        this.HandleNullProperties((EntitySearchRequest) request);
        this.ValidateQuery((EntitySearchRequest) request, requestContext, projectInfo);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1083107, "REST-API", "REST-API", request.ToString());
        this.PublishRequest((EntitySearchRequest) request);
        if (this.EnableSkipTakeOverride)
        {
          request.Skip = this.SkipResults;
          request.Top = this.TakeResults;
        }
        DocumentContractType documentContractType = this.IndexMapper.GetDocumentContractType(requestContext);
        IExpression filtersExpression = this.CreateScopeFiltersExpression(requestContext, projectInfo);
        IEnumerable<IndexInfo> indexInfo = this.IndexMapper.GetIndexInfo(requestContext);
        response = this.m_boardSearchRequestForwarder.ForwardSearchRequest(requestContext, request, indexInfo, filtersExpression, requestContext.ActivityId.ToString(), documentContractType, false, out IEnumerable<string> _);
        this.SetIndexingStatusInResponse((EntitySearchResponse) response, this.GetIndexingStatus(requestContext, (IEntityType) BoardEntityType.GetInstance()));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1083107, "REST-API", "REST-API", response.ToString());
        this.PublishResponse((EntitySearchResponse) response);
        e2eRequestTimer.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("E2EQueryTime", "Query Pipeline", (double) e2eRequestTimer.ElapsedMilliseconds, true);
        PerformanceTimer.SendCustomerIntelligenceData(requestContext, (Action<CustomerIntelligenceData>) (ciData =>
        {
          ciData.Add("Timings", requestContext.GetTraceTimingAsString());
          ciData.Add("ElapsedMillis", requestContext.LastTracedBlockElapsedMilliseconds());
          ciData.Add("NoOfBoardSearchResults", (double) response.Count);
          ciData.Add("E2EQueryTime", (double) e2eRequestTimer.ElapsedMilliseconds);
        }));
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfFailedQueryRequests", "Query Pipeline", 1.0, true);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1083112, "REST-API", "REST-API", ex);
        SearchPlatformExceptionLogger.LogSearchPlatformException(ex);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishOnPremiseIndicator("TFS/Search/Query");
      }
      return response;
    }

    protected IExpression CreateScopeFiltersExpression(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo)
    {
      List<IExpression> source = new List<IExpression>();
      source.Add((IExpression) new TermsExpression("collectionId", Operator.In, (IEnumerable<string>) new string[1]
      {
        requestContext.GetCollectionID().ToString().ToLowerInvariant()
      }));
      if (projectInfo != null)
      {
        IExpression expression = (IExpression) new TermsExpression("projectId", Operator.In, (IEnumerable<string>) new string[1]
        {
          projectInfo.Id.ToString().ToLowerInvariant()
        });
        source.Add(expression);
      }
      return source.Count != 1 ? source.Aggregate<IExpression>((System.Func<IExpression, IExpression, IExpression>) ((current, filter) => (IExpression) new AndExpression(new IExpression[2]
      {
        current,
        filter
      }))) : source[0];
    }

    private void OnboardCollectionIfNotAlreadyDone(IVssRequestContext requestContext)
    {
      if (BoardSearchControllerBase.IsOnboardingTriggered(requestContext))
        return;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext, BoardSearchControllerBase.\u003C\u003EO.\u003C0\u003E__FaultInJobQueueCallback ?? (BoardSearchControllerBase.\u003C\u003EO.\u003C0\u003E__FaultInJobQueueCallback = new TeamFoundationTaskCallback(BoardSearchControllerBase.FaultInJobQueueCallback)));
    }

    private static void FaultInJobQueueCallback(IVssRequestContext requestContext, object args)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      try
      {
        requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().SetFeatureState(requestContext, "Search.Server.Board.Indexing", FeatureAvailabilityState.On);
        TeamFoundationJobService service = requestContext.GetService<TeamFoundationJobService>();
        if (BoardSearchControllerBase.IsFaultInJobQueued(requestContext, (ITeamFoundationJobService) service))
          return;
        if (BoardSearchControllerBase.CollectionIndexingUnitExists(requestContext))
          BoardSearchControllerBase.RecordOnboardingWasTriggered(requestContext);
        else
          BoardSearchControllerBase.QueueFaultInJob(requestContext, (ITeamFoundationJobService) service);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1083106, "REST-API", "REST-API", FormattableString.Invariant(FormattableStringFactory.Create("Failed to queue board account fault-in job. It will be retried on next search attempt. Exception [{0}].", (object) ex)));
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
        JobConstants.BoardAccountFaultInJobId
      });
      return foundationJobQueueEntryList != null && foundationJobQueueEntryList.Count == 1 && foundationJobQueueEntryList[0] != null;
    }

    private static bool CollectionIndexingUnitExists(IVssRequestContext requestContext) => DataAccessFactory.GetInstance().GetIndexingUnitDataAccess().GetIndexingUnit(requestContext, requestContext.GetCollectionID(), "Collection", (IEntityType) BoardEntityType.GetInstance()) != null;

    private static void QueueFaultInJob(
      IVssRequestContext requestContext,
      ITeamFoundationJobService jobService)
    {
      if (jobService.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        JobConstants.BoardAccountFaultInJobId
      }, true) == 1)
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1083106, "REST-API", "REST-API", "Board account fault-in job got queued successfully.");
      else
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1083106, "REST-API", "REST-API", "Board account fault-in job did not get queued. It will be retried on next search attempt.");
    }

    private static bool IsOnboardingTriggered(IVssRequestContext requestContext) => new RegistryManagerV2(requestContext, "Common").GetRegistryEntry("BoardOnboardingTriggered", requestContext.GetCollectionID().ToString()) != null;

    private static void RecordOnboardingWasTriggered(IVssRequestContext requestContext) => new RegistryManagerV2(requestContext, "Common").AddOrUpdateRegistryValue("BoardOnboardingTriggered", requestContext.GetCollectionID().ToString(), true.ToString((IFormatProvider) CultureInfo.InvariantCulture));

    protected override void PublishRequest(EntitySearchRequest request)
    {
      BoardSearchRequest boardSearchRequest = request as BoardSearchRequest;
      FriendlyDictionary<string, object> properties = new FriendlyDictionary<string, object>()
      {
        {
          "BoardSSkip",
          (object) boardSearchRequest.Skip
        },
        {
          "BoardSTake",
          (object) boardSearchRequest.Top
        }
      };
      if (boardSearchRequest.Filters != null)
        properties.Add("BoardSSearchFilters", (object) string.Join(" | ", boardSearchRequest.Filters.Select<KeyValuePair<string, IEnumerable<string>>, string>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (kvp => FormattableString.Invariant(FormattableStringFactory.Create("Name = {0}; Count = {1}", (object) kvp.Key, (object) kvp.Value.Count<string>()))))));
      properties["BoardSSearchText"] = boardSearchRequest.SearchText != null ? (object) boardSearchRequest.SearchText : (object) string.Empty;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("REST-API", "Query Pipeline", (IDictionary<string, object>) properties);
    }

    protected override void PublishResponse(EntitySearchResponse response)
    {
      base.PublishResponse(response);
      BoardSearchResponse boardSearchResponse = response as BoardSearchResponse;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("REST-API", "Query Pipeline", (IDictionary<string, object>) new FriendlyDictionary<string, object>()
      {
        ["BoardTotalMatches"] = (object) boardSearchResponse.Count
      });
    }
  }
}
