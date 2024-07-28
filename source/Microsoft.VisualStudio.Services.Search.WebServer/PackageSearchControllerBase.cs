// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.PackageSearchControllerBase
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Constants;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Query.SecurityScopeFilterService;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Package;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Search.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  public abstract class PackageSearchControllerBase : SearchV2ControllerBase
  {
    private ISearchRequestForwarder<PackageSearchRequest, PackageSearchResponseContent> m_packageSearchRequestForwarder;
    private readonly ISearchSecurityScopeFilterExpressionBuilder m_searchSecurityScopeFilterExpressionBuilder;
    private const string PackageOnboardingIsTriggeredRegistryPrefix = "PackageOnboardingTriggered";

    protected PackageSearchControllerBase()
    {
      this.IndexMapper = (IIndexMapper) new Microsoft.VisualStudio.Services.Search.Query.IndexMapper((IEntityType) PackageEntityType.GetInstance());
      this.m_searchSecurityScopeFilterExpressionBuilder = (ISearchSecurityScopeFilterExpressionBuilder) new PackageSearchSecurityScopeFilterExpressionBuilder();
    }

    protected PackageSearchControllerBase(
      IIndexMapper indexMapper,
      ISearchRequestForwarder<PackageSearchRequest, PackageSearchResponseContent> packageSearchRequestForwarder)
    {
      this.IndexMapper = indexMapper;
      this.m_packageSearchRequestForwarder = packageSearchRequestForwarder;
      this.m_searchSecurityScopeFilterExpressionBuilder = (ISearchSecurityScopeFilterExpressionBuilder) new PackageSearchSecurityScopeFilterExpressionBuilder();
    }

    protected override void InitializeInternal(HttpControllerContext controllerContext)
    {
      base.InitializeInternal(controllerContext);
      this.InitializeRequestForwarder(this.TfsRequestContext);
    }

    internal override void InitializeSearchTextLimitCap(IVssRequestContext requestContext) => this.SearchTextLimitCap = requestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/MaxSearchTextLength", 1024);

    internal void InitializeRequestForwarder(IVssRequestContext requestContext)
    {
      if (this.m_packageSearchRequestForwarder != null)
        return;
      this.m_packageSearchRequestForwarder = (ISearchRequestForwarder<PackageSearchRequest, PackageSearchResponseContent>) new CollectionPackageSearchRequestForwarder(this.IndexMapper.GetESConnectionString(requestContext), requestContext.GetElasticsearchPlatformSettings("/Service/ALMSearch/Settings/ATSearchPlatformSettings"), SearchOptions.Highlighting | SearchOptions.Faceting | SearchOptions.Ranking | SearchOptions.Rescore, requestContext.ExecutionEnvironment.IsOnPremisesDeployment);
    }

    protected PackageSearchResponseContent HandlePostPackageQueryRequest(
      IVssRequestContext requestContext,
      PackageSearchRequest request)
    {
      Stopwatch e2eRequestTimer = Stopwatch.StartNew();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfQueryRequests", "Query Pipeline", 1.0, true);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("EntityType", "Query Pipeline", (double) PackageEntityType.GetInstance().ID, true);
      if (request == null)
        throw new InvalidQueryException(SearchWebApiResources.NullQueryMessage);
      PackageSearchResponseContent response = (PackageSearchResponseContent) null;
      try
      {
        if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          this.OnboardCollectionIfNotAlreadyDone(requestContext);
        this.HandleNullProperties((EntitySearchRequest) request);
        this.ValidateQuery((EntitySearchRequest) request, requestContext);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1083087, "REST-API", "REST-API", request.ToString());
        this.PublishRequest((EntitySearchRequest) request);
        if (this.EnableSkipTakeOverride)
        {
          request.Skip = this.SkipResults;
          request.Top = this.TakeResults;
        }
        DocumentContractType documentContractType = this.IndexMapper.GetDocumentContractType(requestContext);
        bool noResultAccessible = true;
        IExpression filterExpression = this.m_searchSecurityScopeFilterExpressionBuilder.GetScopeFilterExpression(requestContext, this.EnableSecurityChecksInQueryPipeline, out noResultAccessible);
        IEnumerable<IndexInfo> indexInfo = this.IndexMapper.GetIndexInfo(requestContext);
        IEnumerable<string> suggestions;
        if (noResultAccessible)
        {
          response = this.m_packageSearchRequestForwarder.GetZeroResultResponse(request, out suggestions);
          response.InfoCode = 11;
        }
        else
          response = this.m_packageSearchRequestForwarder.ForwardSearchRequest(requestContext, request, indexInfo, filterExpression, requestContext.ActivityId.ToString(), documentContractType, false, out suggestions);
        this.SetIndexingStatusInResponse((EntitySearchResponse) response, this.GetIndexingStatus(requestContext, (IEntityType) PackageEntityType.GetInstance()));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1083087, "REST-API", "REST-API", response.ToString());
        this.PublishResponse((EntitySearchResponse) response);
        e2eRequestTimer.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("E2EQueryTime", "Query Pipeline", (double) e2eRequestTimer.ElapsedMilliseconds, true);
        PerformanceTimer.SendCustomerIntelligenceData(requestContext, (Action<CustomerIntelligenceData>) (ciData =>
        {
          ciData.Add("Timings", requestContext.GetTraceTimingAsString());
          ciData.Add("ElapsedMillis", requestContext.LastTracedBlockElapsedMilliseconds());
          ciData.Add("NoOfPackageSearchResults", (double) response.Count);
          ciData.Add("E2EQueryTime", (double) e2eRequestTimer.ElapsedMilliseconds);
        }));
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfFailedQueryRequests", "Query Pipeline", 1.0, true);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1083078, "REST-API", "REST-API", ex);
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
      if (PackageSearchControllerBase.IsOnboardingTriggered(requestContext))
        return;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext, PackageSearchControllerBase.\u003C\u003EO.\u003C0\u003E__FaultInJobQueueCallback ?? (PackageSearchControllerBase.\u003C\u003EO.\u003C0\u003E__FaultInJobQueueCallback = new TeamFoundationTaskCallback(PackageSearchControllerBase.FaultInJobQueueCallback)));
    }

    private static void FaultInJobQueueCallback(IVssRequestContext requestContext, object args)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      try
      {
        requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().SetFeatureState(requestContext, "Search.Server.Package.Indexing", FeatureAvailabilityState.On);
        TeamFoundationJobService service = requestContext.GetService<TeamFoundationJobService>();
        if (PackageSearchControllerBase.IsFaultInJobQueued(requestContext, (ITeamFoundationJobService) service))
          return;
        if (PackageSearchControllerBase.CollectionIndexingUnitExists(requestContext))
          PackageSearchControllerBase.RecordOnboardingWasTriggered(requestContext);
        else
          PackageSearchControllerBase.QueueFaultInJob(requestContext, (ITeamFoundationJobService) service);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080074, "REST-API", "REST-API", FormattableString.Invariant(FormattableStringFactory.Create("Failed to queue package account fault-in job. It will be retried on next search attempt. Exception [{0}].", (object) ex)));
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
        JobConstants.PackageAccountFaultInJobId
      });
      return foundationJobQueueEntryList != null && foundationJobQueueEntryList.Count == 1 && foundationJobQueueEntryList[0] != null;
    }

    private static bool CollectionIndexingUnitExists(IVssRequestContext requestContext) => DataAccessFactory.GetInstance().GetIndexingUnitDataAccess().GetIndexingUnit(requestContext, requestContext.GetCollectionID(), "Collection", (IEntityType) PackageEntityType.GetInstance()) != null;

    private static void QueueFaultInJob(
      IVssRequestContext requestContext,
      ITeamFoundationJobService jobService)
    {
      if (jobService.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        JobConstants.PackageAccountFaultInJobId
      }, true) == 1)
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080074, "REST-API", "REST-API", "Package account fault-in job got queued successfully.");
      else
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080074, "REST-API", "REST-API", "Package account fault-in job did not get queued. It will be retried on next search attempt.");
    }

    private static bool IsOnboardingTriggered(IVssRequestContext requestContext) => new RegistryManagerV2(requestContext, "Common").GetRegistryEntry("PackageOnboardingTriggered", requestContext.GetCollectionID().ToString()) != null;

    private static void RecordOnboardingWasTriggered(IVssRequestContext requestContext) => new RegistryManagerV2(requestContext, "Common").AddOrUpdateRegistryValue("PackageOnboardingTriggered", requestContext.GetCollectionID().ToString(), true.ToString((IFormatProvider) CultureInfo.InvariantCulture));

    protected override void PublishRequest(EntitySearchRequest request)
    {
      PackageSearchRequest packageSearchRequest = request as PackageSearchRequest;
      FriendlyDictionary<string, object> properties = new FriendlyDictionary<string, object>();
      properties.Add("PackageSSkip", (object) packageSearchRequest.Skip);
      properties.Add("PackageSTake", (object) packageSearchRequest.Top);
      if (packageSearchRequest.Filters != null)
        properties.Add("PackageSSearchFilters", (object) string.Join(" | ", packageSearchRequest.Filters.Select<KeyValuePair<string, IEnumerable<string>>, string>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (kvp => FormattableString.Invariant(FormattableStringFactory.Create("Name = {0}; Count = {1}", (object) kvp.Key, (object) kvp.Value.Count<string>()))))));
      properties["PackageSSearchText"] = packageSearchRequest.SearchText != null ? (object) packageSearchRequest.SearchText : (object) string.Empty;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("REST-API", "Query Pipeline", (IDictionary<string, object>) properties);
    }

    protected override void PublishResponse(EntitySearchResponse response)
    {
      base.PublishResponse(response);
      PackageSearchResponseContent searchResponseContent = response as PackageSearchResponseContent;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("REST-API", "Query Pipeline", (IDictionary<string, object>) new FriendlyDictionary<string, object>()
      {
        ["PackageTotalMatches"] = (object) searchResponseContent.Count,
        ["PackageFacets"] = (object) string.Join<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>>(" | ", (IEnumerable<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>>) searchResponseContent.Facets),
        ["CountOfHighlightedFields"] = (object) searchResponseContent.Results.AsParallel<PackageResult>().Sum<PackageResult>((Func<PackageResult, int>) (result => result.Hits.Count<PackageHit>()))
      });
    }
  }
}
