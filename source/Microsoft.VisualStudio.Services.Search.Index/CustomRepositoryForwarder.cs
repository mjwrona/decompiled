// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Index.CustomRepositoryForwarder
// Assembly: Microsoft.VisualStudio.Services.Search.Index, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C10BA9-319D-46E2-AA64-F18680226A42
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Index.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code;
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

namespace Microsoft.VisualStudio.Services.Search.Index
{
  public class CustomRepositoryForwarder : ICustomRepositoryForwarder
  {
    private readonly IDataAccessFactory m_dataAccessFactoryInstance;
    private readonly IIndexingUnitChangeEventHandler m_changeEventHandler;

    public CustomRepositoryForwarder()
      : this(DataAccessFactory.GetInstance(), (IIndexingUnitChangeEventHandler) new IndexingUnitChangeEventHandler())
    {
    }

    internal CustomRepositoryForwarder(
      IDataAccessFactory dataAccessFactory,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler)
    {
      this.m_dataAccessFactoryInstance = dataAccessFactory;
      this.m_changeEventHandler = indexingUnitChangeEventHandler;
    }

    internal EntityFinalizerBase FinalizeHelper => (EntityFinalizerBase) new CollectionCodeFinalizeHelper();

    public Microsoft.VisualStudio.Services.Search.WebApi.CustomRepository ForwardRegisterRepositoryRequest(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.WebApi.CustomRepository repository)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081370, "Query Pipeline", "Query", nameof (ForwardRegisterRepositoryRequest));
      try
      {
        this.Validate(repository);
        ITracerCICorrelationDetails correlationDetails = TracerCICorrelationDetailsFactory.GetCICorrelationDetails(requestContext, MethodBase.GetCurrentMethod().Name, 16);
        EventProcessingContext eventProcessingContext = new EventProcessingContext(requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/BatchCountForCustomRepoProcessing"), requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/BatchCountForCustomRepoProcessing"), (IIndexingUnitChangeEventSelector) new CreationTimeBasedIndexingUnitChangeEventSelector());
        ExecutionContext executionContext = new ExecutionContext(requestContext, correlationDetails, eventProcessingContext)
        {
          FaultService = (IIndexerFaultService) new IndexerV1FaultService(requestContext, (IFaultStore) new RegistryServiceFaultStore(requestContext))
        };
        int num = requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/IsZLRIEnabledForCustom", true) ? 1 : 0;
        bool isShadowIndexingRepositoryRequest = repository.CustomIndexingMode.Equals((object) CustomIndexingMode.ReindexingShadow);
        if (num == 0 || !executionContext.RequestContext.IsCodeReindexingWithZeroStalenessFeatureEnabled())
          isShadowIndexingRepositoryRequest = false;
        repository.RepositoryId = this.CheckAndAddRepository(executionContext, repository, isShadowIndexingRepositoryRequest);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081371, "Query Pipeline", "Query", nameof (ForwardRegisterRepositoryRequest));
      }
      return repository;
    }

    public IEnumerable<string> ForwardGetRepositoriesRequest(
      IVssRequestContext requestContext,
      string projectName)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081370, "Query Pipeline", "Query", nameof (ForwardGetRepositoriesRequest));
      try
      {
        if (string.IsNullOrWhiteSpace(projectName))
          throw new ArgumentException("Project name should not be null or contain only whitespaces", nameof (projectName));
        return this.m_dataAccessFactoryInstance.GetCustomRepositoryDataAccess().GetRepositories(requestContext, requestContext.GetCollectionID(), projectName);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081371, "Query Pipeline", "Query", nameof (ForwardGetRepositoriesRequest));
      }
    }

    public Microsoft.VisualStudio.Services.Search.WebApi.CustomRepository ForwardGetRepositoryRequest(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081370, "Query Pipeline", "Query", nameof (ForwardGetRepositoryRequest));
      CustomRepositoryEntity repositoryEntity = (CustomRepositoryEntity) null;
      try
      {
        if (string.IsNullOrWhiteSpace(projectName))
          throw new ArgumentException("Project name should not be null or contain only whitespaces", nameof (projectName));
        if (string.IsNullOrWhiteSpace(repositoryName))
          throw new ArgumentException("Repository name should not be null or contain only whitespaces", nameof (repositoryName));
        repositoryEntity = this.m_dataAccessFactoryInstance.GetCustomRepositoryDataAccess().GetRepository(requestContext, requestContext.GetCollectionID(), projectName, repositoryName);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081371, "Query Pipeline", "Query", nameof (ForwardGetRepositoryRequest));
      }
      if (repositoryEntity == null)
        return (Microsoft.VisualStudio.Services.Search.WebApi.CustomRepository) null;
      return new Microsoft.VisualStudio.Services.Search.WebApi.CustomRepository()
      {
        ProjectName = repositoryEntity.ProjectName,
        RepositoryName = repositoryEntity.RepositoryName,
        RepositoryId = repositoryEntity.RepositoryId,
        Properties = repositoryEntity.Properties
      };
    }

    private Guid CheckAndAddRepository(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.WebApi.CustomRepository repository,
      bool isShadowIndexingRepositoryRequest)
    {
      ICustomRepositoryDataAccess repositoryDataAccess = this.m_dataAccessFactoryInstance.GetCustomRepositoryDataAccess();
      IIndexingUnitDataAccess indexingUnitDataAccess = this.m_dataAccessFactoryInstance.GetIndexingUnitDataAccess();
      Guid repositoryId = repositoryDataAccess.GetRepositoryId(executionContext.RequestContext, executionContext.RequestContext.GetCollectionID(), repository.ProjectName, repository.RepositoryName);
      if (isShadowIndexingRepositoryRequest)
      {
        if (repositoryId == Guid.Empty)
          throw new InvalidCustomRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchWebApiResources.CustomRepositoryNotRegistered, (object) repository.RepositoryName));
        this.CheckAndAddIndexingUnit(executionContext, indexingUnitDataAccess, repository, repositoryId, isShadowIndexingRepositoryRequest);
      }
      else if (repositoryId == Guid.Empty)
      {
        repositoryId = Guid.NewGuid();
        repositoryDataAccess.AddRepository(executionContext.RequestContext, new CustomRepositoryEntity()
        {
          CollectionId = executionContext.RequestContext.GetCollectionID(),
          ProjectName = repository.ProjectName,
          RepositoryName = repository.RepositoryName,
          RepositoryId = repositoryId,
          Properties = repository.Properties
        });
        this.CheckAndAddIndexingUnit(executionContext, indexingUnitDataAccess, repository, repositoryId);
      }
      else
      {
        repositoryDataAccess.UpdateRepository(executionContext.RequestContext, new CustomRepositoryEntity()
        {
          CollectionId = executionContext.RequestContext.GetCollectionID(),
          ProjectName = repository.ProjectName,
          RepositoryName = repository.RepositoryName,
          RepositoryId = repositoryId,
          Properties = repository.Properties
        });
        this.CheckAndAddIndexingUnit(executionContext, indexingUnitDataAccess, repository, repositoryId);
      }
      return repositoryId;
    }

    internal virtual void CheckAndAddIndexingUnit(
      ExecutionContext executionContext,
      IIndexingUnitDataAccess indexingUnitDataAccess,
      Microsoft.VisualStudio.Services.Search.WebApi.CustomRepository repository,
      Guid repositoryId,
      bool isShadowIndexingRepositoryRequest = false)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit1 = indexingUnitDataAccess.GetIndexingUnit(executionContext.RequestContext, repositoryId, "CustomRepository", (IEntityType) CodeEntityType.GetInstance(), isShadowIndexingRepositoryRequest);
      SDRepositoryProperties properties = (SDRepositoryProperties) repository.Properties;
      List<string> list = properties.BranchDetails.Select<SDBranchDetail, string>((Func<SDBranchDetail, string>) (x => x.BranchName)).ToList<string>();
      bool currentHostConfigValue = executionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/Routing/SizeBasedCustomRoutingEnabled", true);
      IndexingExecutionContext executionContext1;
      if (indexingUnit1 == null)
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit2 = indexingUnitDataAccess.GetIndexingUnit(executionContext.RequestContext, executionContext.RequestContext.GetCollectionID(), "Collection", (IEntityType) CodeEntityType.GetInstance());
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit projectIndexingUnit = this.GetOrCreateProjectIndexingUnit(executionContext, repository.ProjectName, indexingUnitDataAccess, indexingUnit2.IndexingUnitId);
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit3 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnit(repositoryId, "CustomRepository", (IEntityType) CodeEntityType.GetInstance(), projectIndexingUnit.IndexingUnitId, isShadowIndexingRepositoryRequest);
        indexingUnit3.TFSEntityAttributes = (TFSEntityAttributes) new CustomRepoCodeTFSAttributes()
        {
          ProjectName = repository.ProjectName,
          RepositoryName = repository.RepositoryName,
          Branches = properties.BranchDetails,
          DefaultBranch = list[0]
        };
        CustomRepoCodeIndexingProperties indexingProperties = new CustomRepoCodeIndexingProperties();
        indexingProperties.LastIndexedChangeTime = DateTime.UtcNow;
        indexingProperties.LastIndexedRequestId = Guid.Empty.ToString();
        indexingProperties.Name = repository.RepositoryName;
        indexingUnit3.Properties = (IndexingProperties) indexingProperties;
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit4 = indexingUnit3;
        executionContext1 = this.GetIndexingExecutionContext(executionContext, indexingUnit4);
        ((CustomRepoCodeIndexingProperties) indexingUnit4.Properties).RepositorySize = repository.RepositorySize;
        indexingUnit1 = indexingUnitDataAccess.AddIndexingUnit(executionContext.RequestContext, indexingUnit4);
      }
      else
      {
        executionContext1 = this.GetIndexingExecutionContext(executionContext, indexingUnit1);
        if (executionContext.IsReindexingFailedOrInProgress(this.m_dataAccessFactoryInstance, indexingUnit1.EntityType))
          ((CustomRepoCodeIndexingProperties) indexingUnit1.Properties).RepositorySize = repository.RepositorySize;
        ((CustomRepoCodeTFSAttributes) indexingUnit1.TFSEntityAttributes).Branches = properties.BranchDetails;
        indexingUnitDataAccess.UpdateIndexingUnit(executionContext.RequestContext, indexingUnit1);
      }
      if (!(indexingUnit1.IsLargeRepository(executionContext.RequestContext) & currentHostConfigValue))
        this.QueueRoutingAssignmentOperation(executionContext1, indexingUnit1);
      if (this.FinalizeHelper.ShouldFinalizeChildIndexingUnit(executionContext1, indexingUnit1))
        this.FinalizeHelper.QueueFinalizeOperationIfAllowed(executionContext1, this.m_changeEventHandler);
      CodeQueryScopingCacheUtil.SqlNotifyForRepoAddition(this.m_dataAccessFactoryInstance, executionContext.RequestContext, indexingUnit1);
    }

    internal virtual IndexingExecutionContext GetIndexingExecutionContext(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      IndexingExecutionContext executionContext1 = new IndexingExecutionContext(executionContext.RequestContext, indexingUnit, executionContext.ExecutionTracerContext.TracerCICorrelationDetails);
      executionContext1.FaultService = executionContext.FaultService;
      executionContext1.InitializeNameAndIds(this.m_dataAccessFactoryInstance);
      return executionContext1;
    }

    public CustomRepositoryHealthResponse ForwardGetRepositoryHealthRequest(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      string branchName,
      int numberOfResults,
      long continuationToken)
    {
      if (string.IsNullOrWhiteSpace(projectName))
        throw new ArgumentException("Project name should not be null or contain only whitespaces", nameof (projectName));
      if (string.IsNullOrWhiteSpace(repositoryName))
        throw new ArgumentException("Repository name should not be null or contain only whitespaces", nameof (repositoryName));
      if (string.IsNullOrWhiteSpace(branchName))
        throw new ArgumentException("branch name should not be null or contain only whitespaces", nameof (branchName));
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit repositoryIndexingUnit = this.GetCustomRepositoryIndexingUnit(requestContext, projectName, repositoryName);
      if (repositoryIndexingUnit == null)
        throw new InvalidCustomRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchWebApiResources.CustomRepositoryNotRegistered, (object) repositoryName));
      IItemLevelFailureDataAccess failureDataAccess = this.m_dataAccessFactoryInstance.GetItemLevelFailureDataAccess();
      numberOfResults = CustomRepositoryForwarder.GetNumberOfResults(numberOfResults);
      IVssRequestContext requestContext1 = requestContext;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = repositoryIndexingUnit;
      string branchName1 = branchName;
      int topCount = numberOfResults;
      long startingId = continuationToken;
      IEnumerable<ItemLevelFailureRecord> failedItemsForAbranch = failureDataAccess.GetFailedItemsForABranch(requestContext1, indexingUnit, branchName1, topCount, startingId);
      long continuationToken1;
      List<FailedFile> itemLevelFailure = this.GetFailedFilesAndContinuationTokenFromItemLevelFailure(failedItemsForAbranch, out continuationToken1);
      bool endOfResult = this.IsEndOfResult(failedItemsForAbranch, numberOfResults);
      return new CustomRepositoryHealthResponse(continuationToken1, (string) null, endOfResult, (IEnumerable<FailedFile>) itemLevelFailure);
    }

    internal static int GetNumberOfResults(int numberOfResults) => Math.Min(1000, numberOfResults);

    internal bool IsEndOfResult(
      IEnumerable<ItemLevelFailureRecord> itemLevelFailureRecords,
      int numberOfResults)
    {
      return itemLevelFailureRecords.Count<ItemLevelFailureRecord>() < numberOfResults;
    }

    internal List<FailedFile> GetFailedFilesAndContinuationTokenFromItemLevelFailure(
      IEnumerable<ItemLevelFailureRecord> itemLevelFailureRecords,
      out long continuationToken)
    {
      continuationToken = -1L;
      List<FailedFile> itemLevelFailure = new List<FailedFile>();
      foreach (ItemLevelFailureRecord levelFailureRecord in itemLevelFailureRecords)
      {
        continuationToken = Math.Max(levelFailureRecord.Id, continuationToken);
        itemLevelFailure.Add(new FailedFile(levelFailureRecord.Item, levelFailureRecord.AttemptCount));
      }
      if (continuationToken > -1L)
        ++continuationToken;
      return itemLevelFailure;
    }

    internal Microsoft.VisualStudio.Services.Search.Common.IndexingUnit GetCustomRepositoryIndexingUnit(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName)
    {
      ICustomRepositoryDataAccess repositoryDataAccess = this.m_dataAccessFactoryInstance.GetCustomRepositoryDataAccess();
      IIndexingUnitDataAccess indexingUnitDataAccess = this.m_dataAccessFactoryInstance.GetIndexingUnitDataAccess();
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit;
      try
      {
        Guid repositoryId = repositoryDataAccess.GetRepositoryId(requestContext, requestContext.GetCollectionID(), projectName, repositoryName);
        if (repositoryId == new Guid())
          return (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit) null;
        indexingUnit = indexingUnitDataAccess.GetIndexingUnit(requestContext, repositoryId, "CustomRepository", (IEntityType) CodeEntityType.GetInstance());
        if (indexingUnit == null)
          throw new InvalidCustomRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchWebApiResources.CustomRepositoryNotRegistered, (object) repositoryName));
      }
      catch (AggregateException ex)
      {
        throw ex.InnerException;
      }
      return indexingUnit;
    }

    private void QueueRoutingAssignmentOperation(
      IndexingExecutionContext indexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = indexingUnit.IndexingUnitId,
        ChangeData = new ChangeEventData((ExecutionContext) indexingExecutionContext),
        ChangeType = "AssignRouting",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      this.m_changeEventHandler.HandleEventWithAddingEventWhenNeeded((ExecutionContext) indexingExecutionContext, indexingUnitChangeEvent);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1081370, "Query Pipeline", "Query", "Added change events to assign routing to for a custom repository.");
    }

    private void Validate(Microsoft.VisualStudio.Services.Search.WebApi.CustomRepository repository)
    {
      if (repository == null)
        throw new ArgumentNullException(nameof (repository));
      if (string.IsNullOrWhiteSpace(repository.ProjectName))
        throw new ArgumentException("Project name should not be null or contain only whitespaces", "repository.ProjectName");
      if (string.IsNullOrWhiteSpace(repository.RepositoryName))
        throw new ArgumentException("Repository name should not be null or contain only whitespaces", "repository.RepositoryName");
      if (repository.RepositorySize <= 0)
        throw new ArgumentException("Repository size should be a valid positive number", "repository.RepositorySize");
      if ((repository.Properties is SDRepositoryProperties properties ? properties.BranchDetails : (IEnumerable<SDBranchDetail>) null) == null)
        throw new ArgumentException("Repository Properties or branch information should not be null while registering repository.");
    }

    internal virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnit GetOrCreateProjectIndexingUnit(
      ExecutionContext executionContext,
      string projectName,
      IIndexingUnitDataAccess indexingUnitDataAccess,
      int collectionIndexingUnitId)
    {
      IEnumerable<TeamProjectReference> projects = new ProjectHttpClientWrapper(executionContext, new TraceMetaData(1081370, "Query Pipeline", "REST-API")).GetProjects();
      TeamProjectReference teamProject = (TeamProjectReference) null;
      foreach (TeamProjectReference projectReference in projects)
      {
        if (string.Equals(projectReference.Name, projectName, StringComparison.OrdinalIgnoreCase))
        {
          teamProject = projectReference;
          break;
        }
      }
      if (teamProject == null)
        throw new ArgumentException("Either the project " + projectName + " does not exist or you don't have access to the project,It should be created upfront before indexing and access must be there on the project");
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit projectIndexingUnit = indexingUnitDataAccess.GetIndexingUnit(executionContext.RequestContext, teamProject.Id, "Project", (IEntityType) CodeEntityType.GetInstance());
      if (projectIndexingUnit == null)
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit codeIndexingUnit = teamProject.ToProjectCodeIndexingUnit(collectionIndexingUnitId);
        projectIndexingUnit = indexingUnitDataAccess.AddIndexingUnit(executionContext.RequestContext, codeIndexingUnit);
      }
      return projectIndexingUnit;
    }

    public RepositoryIndexingProperties GetLastIndexedChangeId(
      IVssRequestContext requestContext,
      string projectName,
      string repoName,
      string branchName)
    {
      RepositoryIndexingProperties lastIndexedChangeId = new RepositoryIndexingProperties();
      ICustomRepositoryDataAccess repositoryDataAccess = this.m_dataAccessFactoryInstance.GetCustomRepositoryDataAccess();
      IIndexingUnitDataAccess indexingUnitDataAccess = this.m_dataAccessFactoryInstance.GetIndexingUnitDataAccess();
      Guid repositoryId = repositoryDataAccess.GetRepositoryId(requestContext, requestContext.GetCollectionID(), projectName, repoName);
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit;
      try
      {
        indexingUnit = GenericInvoker.Instance.Invoke<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (() => indexingUnitDataAccess.GetIndexingUnit(requestContext, repositoryId, "CustomRepository", (IEntityType) CodeEntityType.GetInstance())), requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MaxIndexingRetryCount"), requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/FaultMediumSeverityJobDelayInSec"), new TraceMetaData(1080619, "Indexing Pipeline", "IndexingOperation"));
      }
      catch (AggregateException ex)
      {
        throw ex.InnerException;
      }
      if (indexingUnit == null || indexingUnit.Properties == null)
        throw new InvalidCustomRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchWebApiResources.InvalidCollectionProjectRepositoryCombination, (object) repoName));
      Dictionary<string, Dictionary<string, DepotIndexInfo>> depotIndexInfo = (indexingUnit.Properties as CustomRepoCodeIndexingProperties).DepotIndexInfo;
      if (depotIndexInfo != null)
      {
        foreach (KeyValuePair<string, Dictionary<string, DepotIndexInfo>> keyValuePair1 in depotIndexInfo)
        {
          foreach (KeyValuePair<string, DepotIndexInfo> keyValuePair2 in keyValuePair1.Value)
          {
            if (string.Equals(keyValuePair2.Key, branchName) && keyValuePair2.Value.LastIndexedChangeId > lastIndexedChangeId.LastIndexedChangeId)
              lastIndexedChangeId.LastIndexedChangeId = keyValuePair2.Value.LastIndexedChangeId;
          }
        }
        lastIndexedChangeId.Accepted = true;
      }
      return lastIndexedChangeId;
    }
  }
}
