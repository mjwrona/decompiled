// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Index.CustomCodeForwarder
// Assembly: Microsoft.VisualStudio.Services.Search.Index, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C10BA9-319D-46E2-AA64-F18680226A42
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Index.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.ChangeEventData;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties.Code;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Server.Store;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.WebApi;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.CustomRepository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Index
{
  public class CustomCodeForwarder : ICustomCodeForwarder
  {
    private SearchOptions m_searchOptions;
    private ISearchPlatform m_searchPlatform;

    public CustomCodeForwarder(
      string searchPlatformConnectionString,
      string searchPlatformSettings,
      SearchOptions searchOptions,
      bool isOnPrem)
    {
      if (string.IsNullOrEmpty(searchPlatformConnectionString))
        throw new ArgumentNullException(nameof (searchPlatformConnectionString));
      if (string.IsNullOrEmpty(searchPlatformSettings))
        throw new ArgumentNullException("platformSettings");
      this.Initialize(SearchPlatformFactory.GetInstance().Create(searchPlatformConnectionString, searchPlatformSettings, isOnPrem), (IRequestStoreService) RequestStoreService.Instance, (IFileStoreService) FileStoreService.Instance);
      this.m_searchOptions = searchOptions;
    }

    internal CustomCodeForwarder(
      ISearchPlatform searchPlatform,
      IRequestStoreService requestStoreService,
      IFileStoreService fileStoreService)
    {
      this.Initialize(searchPlatform, requestStoreService, fileStoreService);
      this.m_searchOptions = SearchOptions.Faceting;
    }

    internal IDataAccessFactory DataAccessFactoryInstance { get; set; }

    internal IndexingUnitChangeEventHandler ChangeEventHandler { get; set; }

    internal IRequestStoreService RequestStore { get; set; }

    internal IFileStoreService FileStore { get; set; }

    public BulkCodeIndexResponse ForwardBulkIndexRequest(
      IVssRequestContext requestContext,
      BulkCodeIndexRequest request)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081370, "Query Pipeline", "Query", nameof (ForwardBulkIndexRequest));
      string empty = string.Empty;
      try
      {
        if (request == null)
          throw new ArgumentNullException(nameof (request));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1081372, "Query Pipeline", "Query", request.ToString());
        ITracerCICorrelationDetails correlationDetails = TracerCICorrelationDetailsFactory.GetCICorrelationDetails(requestContext, MethodBase.GetCurrentMethod().Name, 15);
        if (!string.IsNullOrWhiteSpace(request.CorrelationId))
          correlationDetails.CorrelationId = request.CorrelationId;
        if (request.TriggerTimeUTC > 0L)
          correlationDetails.TriggerTimeUtc = DateTime.FromFileTimeUtc(request.TriggerTimeUTC);
        EventProcessingContext eventProcessingContext = new EventProcessingContext(requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/BatchCountForCustomRepoProcessing"), requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/BatchCountForCustomRepoProcessing"), (IIndexingUnitChangeEventSelector) new CreationTimeBasedIndexingUnitChangeEventSelector());
        ExecutionContext executionContext = new ExecutionContext(requestContext, correlationDetails, eventProcessingContext)
        {
          FaultService = (IIndexerFaultService) new IndexerV1FaultService(requestContext, (IFaultStore) new RegistryServiceFaultStore(requestContext))
        };
        bool currentHostConfigValue = requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/IsZLRIEnabledForCustom", true);
        if (!request.FileDetail.Any<FileDetail>())
          return new BulkCodeIndexResponse()
          {
            Accepted = true,
            TrackingId = "00000"
          };
        if (currentHostConfigValue && executionContext.RequestContext.IsCodeReindexingWithZeroStalenessFeatureEnabled())
          return this.GetBulkIndexResponseForZeroLatencyReindexing(request, ref empty, executionContext);
        string str = this.AddIndexingUnitChange(executionContext, request, false);
        return new BulkCodeIndexResponse()
        {
          Accepted = true,
          TrackingId = str
        };
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081371, "Query Pipeline", "Query", nameof (ForwardBulkIndexRequest));
      }
    }

    internal BulkCodeIndexResponse GetBulkIndexResponseForZeroLatencyReindexing(
      BulkCodeIndexRequest request,
      ref string trackingId,
      ExecutionContext executionContext)
    {
      ReindexingStatusEntry reindexingStatusEntry = this.DataAccessFactoryInstance.GetReindexingStatusDataAccess().GetReindexingStatusEntry(executionContext.RequestContext.To(TeamFoundationHostType.Deployment), executionContext.RequestContext.GetCollectionID(), (IEntityType) CodeEntityType.GetInstance());
      ErrorData errorData = (ErrorData) null;
      BulkCodeIndexResponse latencyReindexing;
      if (this.ValidateBulkCodeIndexRequest(request, reindexingStatusEntry, executionContext, ref errorData))
      {
        bool isShadowindexingRequest = request.CustomIndexingMode == CustomIndexingMode.ReindexingShadow;
        trackingId = this.AddIndexingUnitChange(executionContext, request, isShadowindexingRequest);
        latencyReindexing = new BulkCodeIndexResponse()
        {
          Accepted = true,
          TrackingId = trackingId
        };
      }
      else
      {
        IList<ErrorData> errorDataList = (IList<ErrorData>) new List<ErrorData>();
        errorDataList.Add(errorData);
        latencyReindexing = new BulkCodeIndexResponse()
        {
          Accepted = false,
          TrackingId = string.Empty,
          Errors = (IEnumerable<ErrorData>) errorDataList
        };
      }
      return latencyReindexing;
    }

    private bool ValidateBulkCodeIndexRequest(
      BulkCodeIndexRequest request,
      ReindexingStatusEntry reindexingStatusEntry,
      ExecutionContext executionContext,
      ref ErrorData errorData)
    {
      Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus status = reindexingStatusEntry.Status;
      bool flag1 = true;
      bool flag2 = reindexingStatusEntry != null && reindexingStatusEntry.IsReindexingFailedOrInProgress();
      CustomCodeForwarder.IsZLRIRequest(request);
      ErrorCode errorCode = ErrorCode.InvalidIndexingMode;
      ErrorType errorType = ErrorType.Error;
      string str = FormattableString.Invariant(FormattableStringFactory.Create("InvalidIndexingMode"));
      if (CustomCodeForwarder.IsZLRIRequest(request))
      {
        if (flag2)
        {
          if (request.CustomIndexingMode == CustomIndexingMode.ReindexingPrimary && executionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/SuspendCodeIndexingOnPrimary"))
          {
            errorCode = ErrorCode.ReindexingPausedForPrimaryIndexingUnit;
            errorType = ErrorType.Error;
            str = FormattableString.Invariant(FormattableStringFactory.Create("The reindexing has paused for primary indexing unit."));
            flag1 = false;
          }
        }
        else if (status == Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.Completed)
        {
          errorCode = ErrorCode.ReindexingCompleted;
          errorType = ErrorType.Error;
          str = FormattableString.Invariant(FormattableStringFactory.Create("The reindexing has finalised"));
          flag1 = false;
        }
      }
      if (!flag1)
        errorData = new ErrorData()
        {
          ErrorCode = errorCode.ToString(),
          ErrorType = errorType,
          ErrorMessage = str
        };
      return flag1;
    }

    private static bool IsZLRIRequest(BulkCodeIndexRequest request) => request.CustomIndexingMode == CustomIndexingMode.ReindexingShadow || request.CustomIndexingMode == CustomIndexingMode.ReindexingPrimary;

    public string ForwardGetFileContentRequest(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      string branchName,
      string filePath)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081370, "Query Pipeline", "Query", nameof (ForwardGetFileContentRequest));
      try
      {
        if (string.IsNullOrWhiteSpace(projectName))
          throw new ArgumentNullException(nameof (projectName), "Project name should not be null or contain only whitespaces");
        if (string.IsNullOrWhiteSpace(repositoryName))
          throw new ArgumentNullException(nameof (repositoryName), "Repository name should not be null or contain only whitespaces");
        if (string.IsNullOrWhiteSpace(branchName))
          throw new ArgumentNullException(nameof (branchName), "Branch name should not be null or contain only whitespaces");
        if (string.IsNullOrWhiteSpace(filePath))
          throw new ArgumentNullException(nameof (filePath), "File path should not be null or contain only whitespaces");
        return this.FileStore.GetFile(requestContext, projectName, repositoryName, branchName, filePath.NormalizePath());
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081371, "Query Pipeline", "Query", nameof (ForwardGetFileContentRequest));
      }
    }

    public virtual FilesMetadataResponse ForwardFilesMetadataRequest(
      IVssRequestContext requestContext,
      IEnumerable<IndexInfo> indexInfo,
      string projectName,
      string repositoryName,
      List<string> branchNameList,
      FilesMetadataRequest request,
      DocumentContractType contractType)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081370, "Query Pipeline", "Query", nameof (ForwardFilesMetadataRequest));
      try
      {
        this.ValidateArguments(indexInfo, projectName, repositoryName, contractType);
        if (request.RequiredMetadata == null || request.RequiredMetadata.Count<MetadataType>() == 0)
          throw new InvalidCustomRequestException(SearchWebApiResources.RequiredMetadataMissing);
        return this.ForwardPostFilesMetadataRequestInternal(requestContext, indexInfo, projectName, repositoryName, branchNameList, request, contractType);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081371, "Query Pipeline", "Query", nameof (ForwardFilesMetadataRequest));
      }
    }

    public OperationStatus ForwardGetOperationStatusRequest(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      string branchName,
      string trackingId)
    {
      if (string.IsNullOrWhiteSpace(projectName))
        throw new ArgumentNullException(nameof (projectName), "Project name should not be null or contain only whitespaces");
      if (string.IsNullOrWhiteSpace(repositoryName))
        throw new ArgumentNullException(nameof (repositoryName), "Repository name should not be null or contain only whitespaces");
      if (string.IsNullOrWhiteSpace(trackingId))
        throw new ArgumentNullException(nameof (trackingId), "Tracking Id should not be null or contain only whitespaces");
      return this.RequestStore.RequestExists(requestContext, projectName, repositoryName, branchName, trackingId) ? OperationStatus.InProgress : OperationStatus.Completed;
    }

    internal FilesMetadataResponse ForwardPostFilesMetadataRequestInternal(
      IVssRequestContext requestContext,
      IEnumerable<IndexInfo> indexInfo,
      string projectName,
      string repositoryName,
      List<string> branchNamesList,
      FilesMetadataRequest request,
      DocumentContractType contractType)
    {
      Guid repositoryId = this.GetRepositoryId(requestContext, projectName, repositoryName);
      List<string> list = request.RequiredMetadata.Select<MetadataType, string>((Func<MetadataType, string>) (x => CodeFileContract.GetFieldName(x, contractType))).ToList<string>();
      string type = "repositoryId";
      string branchNameFieldName = CodeFileContract.GetBranchNameFieldName(contractType);
      List<IExpression> set = new List<IExpression>();
      set.Add((IExpression) new TermExpression(type, Operator.Equals, repositoryId.ToString()));
      if (branchNamesList != null)
        set.Add((IExpression) new TermsExpression(branchNameFieldName, Operator.In, branchNamesList.Select<string, string>((Func<string, string>) (x => x.NormalizePath()))));
      if (request.ScopePaths != null && request.ScopePaths.Count > 0)
        set.Add((IExpression) new TermsExpression(CodeContractField.CodeSearchFieldDesc.FilePath.ElasticsearchFieldName(), Operator.In, (IEnumerable<string>) request.ScopePaths.Select<string, string>((Func<string, string>) (x => x.NormalizePath())).ToList<string>()));
      IExpression filter = (IExpression) new AndExpression((IEnumerable<IExpression>) set);
      int numberOfHitsInOneGo = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/PatchInMemoryThresholdForMaxDocs");
      if (request.NumberOfDocsToFetch > 0)
        numberOfHitsInOneGo = request.NumberOfDocsToFetch;
      List<Dictionary<MetadataType, List<string>>> dictionaryList = new List<Dictionary<MetadataType, List<string>>>();
      string pageId = request.PaginationInfo?.PageId;
      string nextScrollId;
      foreach (IEnumerable<KeyValuePair<string, List<string>>> source in this.m_searchPlatform.GetDocMetadata(requestContext, indexInfo, list, filter, contractType, numberOfHitsInOneGo, out nextScrollId, pageId))
      {
        Dictionary<MetadataType, List<string>> dictionary = source.ToDictionary<KeyValuePair<string, List<string>>, MetadataType, List<string>>((Func<KeyValuePair<string, List<string>>, MetadataType>) (metadata => CodeFileContract.GetMetadataType(metadata.Key)), (Func<KeyValuePair<string, List<string>>, List<string>>) (metadata => metadata.Value));
        dictionaryList.Add(dictionary);
      }
      return new FilesMetadataResponse()
      {
        FilesMetaData = (IEnumerable<IDictionary<MetadataType, List<string>>>) dictionaryList,
        PaginationInfo = new PaginationDetails()
        {
          PageId = nextScrollId
        }
      };
    }

    internal virtual string AddIndexingUnitChange(
      ExecutionContext executionContext,
      BulkCodeIndexRequest request,
      bool isShadowindexingRequest)
    {
      ICustomRepositoryDataAccess repositoryDataAccess = this.DataAccessFactoryInstance.GetCustomRepositoryDataAccess();
      IIndexingUnitDataAccess indexingUnitDataAccess = this.DataAccessFactoryInstance.GetIndexingUnitDataAccess();
      Guid repositoryId = repositoryDataAccess.GetRepositoryId(executionContext.RequestContext, executionContext.RequestContext.GetCollectionID(), request.ProjectName, request.RepositoryName);
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit repoIndexingUnit;
      try
      {
        repoIndexingUnit = GenericInvoker.Instance.Invoke<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (() =>
        {
          Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = indexingUnitDataAccess.GetIndexingUnit(executionContext.RequestContext, repositoryId, "CustomRepository", (IEntityType) CodeEntityType.GetInstance(), isShadowindexingRequest);
          if (indexingUnit != null)
            return indexingUnit;
          if (!isShadowindexingRequest)
            throw new InvalidCustomRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchWebApiResources.CustomRepositoryNotRegistered, (object) request.RepositoryName));
          throw new InvalidCustomRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchWebApiResources.CustomRepositoryShadowNotRegistered, (object) request.RepositoryName));
        }), executionContext.RequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MaxIndexingRetryCount"), executionContext.RequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/FaultMediumSeverityJobDelayInSec"), new TraceMetaData(1080619, "Indexing Pipeline", "IndexingOperation"));
      }
      catch (AggregateException ex)
      {
        throw ex.InnerException;
      }
      bool flag = !string.IsNullOrWhiteSpace(request.TopFolder);
      string branchName = !request.FileDetail.Any<FileDetail>() ? (request.BranchToLatestChangeMap == null || !request.BranchToLatestChangeMap.Any<KeyValuePair<string, DepotLastChangeInfo>>() ? string.Empty : (!flag ? request.BranchToLatestChangeMap.First<KeyValuePair<string, DepotLastChangeInfo>>().Key : string.Empty)) : (!flag ? request.FileDetail.First<FileDetail>().Branches.First<string>() : string.Empty);
      string g = this.RequestStore.AddRequest<BulkCodeIndexRequest>(executionContext.RequestContext, request.ProjectName, request.RepositoryName, request, branchName);
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1;
      if (flag)
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit forCurrentRequest = this.GetOrAddScopedIndexingUnitForCurrentRequest(executionContext, request, repoIndexingUnit, indexingUnitDataAccess);
        IndexingUnitChangeEventHandler changeEventHandler = this.ChangeEventHandler;
        ExecutionContext executionContext1 = executionContext;
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent2 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent();
        indexingUnitChangeEvent2.IndexingUnitId = forCurrentRequest.IndexingUnitId;
        LargeRepositoryMetadataCrawlerEventData crawlerEventData;
        if (!request.IsLastRequest)
        {
          crawlerEventData = new LargeRepositoryMetadataCrawlerEventData(executionContext)
          {
            RequestId = new Guid(g)
          };
        }
        else
        {
          crawlerEventData = new LargeRepositoryMetadataCrawlerEventData(executionContext);
          crawlerEventData.RequestId = new Guid(g);
          crawlerEventData.BranchToLatestChangeIdMap = request.BranchToLatestChangeMap;
        }
        indexingUnitChangeEvent2.ChangeData = (Microsoft.VisualStudio.Services.Search.Common.ChangeEventData) crawlerEventData;
        indexingUnitChangeEvent2.ChangeType = "BeginBulkIndex";
        indexingUnitChangeEvent2.State = IndexingUnitChangeEventState.Pending;
        indexingUnitChangeEvent2.AttemptCount = (byte) 0;
        indexingUnitChangeEvent1 = changeEventHandler.HandleEvent(executionContext1, indexingUnitChangeEvent2, true);
      }
      else
        indexingUnitChangeEvent1 = this.ChangeEventHandler.HandleEvent(executionContext, new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
        {
          AttemptCount = (byte) 0,
          ChangeData = (Microsoft.VisualStudio.Services.Search.Common.ChangeEventData) new CustomRepositoryIndexRequestEventData(executionContext)
          {
            RequestId = new Guid(g),
            Branches = request.FileDetail.First<FileDetail>().Branches
          },
          ChangeType = "BeginBulkIndex",
          IndexingUnitId = repoIndexingUnit.IndexingUnitId,
          State = IndexingUnitChangeEventState.Pending
        }, true);
      if (indexingUnitChangeEvent1 != null)
      {
        ClientTraceData clientTraceData = new ClientTraceData();
        clientTraceData.Add("ChangeEventCreatedTimeUtc", (object) DateTime.UtcNow);
        clientTraceData.Add("ChangeEventIdCreatedOnCheckinNotification", (object) indexingUnitChangeEvent1.Id);
        clientTraceData.Add("IndexingUnitId", (object) indexingUnitChangeEvent1.IndexingUnitId);
        clientTraceData.Add("IsLastFeedRequestByIndexer", (object) request.IsLastRequest);
        executionContext.ExecutionTracerContext.PublishClientTrace("Indexing Pipeline", "Indexer", clientTraceData);
      }
      return g;
    }

    internal Microsoft.VisualStudio.Services.Search.Common.IndexingUnit GetOrAddScopedIndexingUnitForCurrentRequest(
      ExecutionContext executionContext,
      BulkCodeIndexRequest request,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit repoIndexingUnit,
      IIndexingUnitDataAccess indexingUnitDataAccess)
    {
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> list = indexingUnitDataAccess.GetIndexingUnitsWithGivenParent(executionContext.RequestContext, repoIndexingUnit.IndexingUnitId, -1).Where<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, bool>) (x => x.IndexingUnitType == "ScopedIndexingUnit")).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit scopedIndexingUnit;
      if (!this.TryGetIndexingUnitForCurrentRequest(request, list, out scopedIndexingUnit))
      {
        CustomRepoCodeIndexingProperties properties = repoIndexingUnit.Properties as CustomRepoCodeIndexingProperties;
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnit(Guid.NewGuid(), "ScopedIndexingUnit", (IEntityType) CodeEntityType.GetInstance(), repoIndexingUnit.IndexingUnitId, repoIndexingUnit.IsShadow);
        CustomRepoCodeIndexingProperties indexingProperties = new CustomRepoCodeIndexingProperties();
        indexingProperties.LastIndexedChangeTime = properties.LastIndexedChangeTime;
        indexingProperties.LastIndexedRequestId = properties.LastIndexedRequestId;
        indexingProperties.Name = properties.Name;
        indexingProperties.RepositorySize = request.DocCountInTopFolder;
        indexingUnit.Properties = (IndexingProperties) indexingProperties;
        indexingUnit.TFSEntityAttributes = (TFSEntityAttributes) new ScopedGitRepositoryAttributes()
        {
          ScopePath = request.TopFolder
        };
        scopedIndexingUnit = indexingUnit;
        if (!repoIndexingUnit.IsLargeRepository(executionContext.RequestContext))
          scopedIndexingUnit.Properties.IndexIndices = repoIndexingUnit.Properties.IndexIndices;
        scopedIndexingUnit = indexingUnitDataAccess.AddIndexingUnit(executionContext.RequestContext, scopedIndexingUnit);
        if (repoIndexingUnit.IsLargeRepository(executionContext.RequestContext))
          this.QueueRoutingAssignmentOperations(executionContext, scopedIndexingUnit, repoIndexingUnit);
      }
      return scopedIndexingUnit;
    }

    internal bool TryGetIndexingUnitForCurrentRequest(
      BulkCodeIndexRequest request,
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> existingScopedIndexingUnits,
      out Microsoft.VisualStudio.Services.Search.Common.IndexingUnit scopedIndexingUnit)
    {
      scopedIndexingUnit = (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit) null;
      if (existingScopedIndexingUnits != null && request != null)
        scopedIndexingUnit = existingScopedIndexingUnits.FirstOrDefault<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, bool>) (x => x.IndexingUnitType == "ScopedIndexingUnit" && (x.TFSEntityAttributes as ScopedGitRepositoryAttributes).ScopePath.Equals(request.TopFolder)));
      return scopedIndexingUnit != null;
    }

    internal virtual void QueueRoutingAssignmentOperations(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit scopedIndexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit repoIndexingUnit)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = scopedIndexingUnit.IndexingUnitId,
        ChangeData = new Microsoft.VisualStudio.Services.Search.Common.ChangeEventData(executionContext),
        ChangeType = "AssignRouting",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent2 = this.ChangeEventHandler.HandleEvent(executionContext, indexingUnitChangeEvent1);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080619, "Indexing Pipeline", "Indexer", string.Format("Added change events to assign routing to a scoped IU of custom repository. Change EventId: {0} ", (object) indexingUnitChangeEvent2.Id));
      IndexingUnitChangeEventPrerequisites eventPrerequisites1 = new IndexingUnitChangeEventPrerequisites();
      eventPrerequisites1.Add(new IndexingUnitChangeEventPrerequisitesFilter()
      {
        Id = indexingUnitChangeEvent2.Id,
        Operator = IndexingUnitChangeEventFilterOperator.Contains,
        PossibleStates = new List<IndexingUnitChangeEventState>()
        {
          IndexingUnitChangeEventState.Succeeded
        }
      });
      IndexingUnitChangeEventPrerequisites eventPrerequisites2 = eventPrerequisites1;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent3 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = repoIndexingUnit.IndexingUnitId,
        ChangeData = new Microsoft.VisualStudio.Services.Search.Common.ChangeEventData(executionContext),
        ChangeType = "AssignRouting",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0,
        Prerequisites = eventPrerequisites2
      };
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080619, "Indexing Pipeline", "Indexer", string.Format("Added change events to assign routing to a custom repository. Change EventId: {0}", (object) this.ChangeEventHandler.HandleEvent(executionContext, indexingUnitChangeEvent3).Id));
    }

    private Guid GetRepositoryId(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName)
    {
      ICustomRepositoryDataAccess dataAccess = this.DataAccessFactoryInstance.GetCustomRepositoryDataAccess();
      try
      {
        return GenericInvoker.Instance.Invoke<Guid>((Func<Guid>) (() =>
        {
          Guid repositoryId = dataAccess.GetRepositoryId(requestContext, requestContext.GetCollectionID(), projectName, repositoryName);
          return !(repositoryId == Guid.Empty) ? repositoryId : throw new InvalidCustomRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchWebApiResources.InvalidCollectionProjectRepositoryCombination, (object) requestContext.GetCollectionID(), (object) projectName, (object) repositoryName));
        }), requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MaxIndexingRetryCount"), requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/FaultMediumSeverityJobDelayInSec"), new TraceMetaData(1080619, "Indexing Pipeline", "IndexingOperation"));
      }
      catch (AggregateException ex)
      {
        throw ex.InnerException;
      }
    }

    private void ValidateArguments(
      IEnumerable<IndexInfo> indexInfo,
      string projectName,
      string repositoryName,
      DocumentContractType contractType)
    {
      if (indexInfo == null || !indexInfo.Any<IndexInfo>())
        throw new ArgumentNullException(nameof (indexInfo), "Index info should not be null or empty");
      if (string.IsNullOrWhiteSpace(projectName))
        throw new ArgumentNullException(nameof (projectName), "Project name should not be null or contain only whitespaces");
      if (string.IsNullOrWhiteSpace(repositoryName))
        throw new ArgumentNullException(nameof (repositoryName), "Repository name should not be null or contain only whitespaces");
      if (contractType == DocumentContractType.Unsupported)
        throw new ArgumentNullException(nameof (contractType), "Contract type should be a valid and supported type");
    }

    private void Initialize(
      ISearchPlatform searchPlatform,
      IRequestStoreService requestStoreService,
      IFileStoreService fileStoreService)
    {
      this.m_searchPlatform = searchPlatform;
      this.DataAccessFactoryInstance = DataAccessFactory.GetInstance();
      this.ChangeEventHandler = new IndexingUnitChangeEventHandler();
      this.RequestStore = requestStoreService;
      this.FileStore = fileStoreService;
    }
  }
}
