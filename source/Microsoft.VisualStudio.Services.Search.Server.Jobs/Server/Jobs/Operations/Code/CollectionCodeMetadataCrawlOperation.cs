// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.CollectionCodeMetadataCrawlOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties.Code;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Api;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class CollectionCodeMetadataCrawlOperation : AbstractIndexingOperation
  {
    private bool m_isReIndexingInProgress;
    private CodeFileContract m_codeFileContract;
    protected internal bool m_isShadowCodeReIndexingRequired;

    protected internal CollectionCodeMetadataCrawlOperation(
      IndexingExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      RegistryManager registryManager)
      : base((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent)
    {
      this.RegistryManager = registryManager;
      this.m_isReIndexingInProgress = executionContext.IsReindexingFailedOrInProgress(executionContext.DataAccessFactory, (IEntityType) CodeEntityType.GetInstance());
    }

    protected RegistryManager RegistryManager { get; private set; }

    internal virtual List<IndexingUnitWithSize> GetLargeRepoScopedIndexingUnitsWithSizeEstimates(
      IndexingExecutionContext indexingExecutionContext,
      List<IndexingUnitWithSize> largeRepoIndexingUnits,
      bool isReindexingInProgress)
    {
      List<IndexingUnitWithSize> withSizeEstimates = new List<IndexingUnitWithSize>();
      Dictionary<Guid, Guid> idToProjectIdMap = this.GetLargeRepositoryIdToProjectIdMap(indexingExecutionContext, largeRepoIndexingUnits);
      foreach (IndexingUnitWithSize repoIndexingUnit in largeRepoIndexingUnits)
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit1 = repoIndexingUnit.IndexingUnit;
        string defaultBranch = (indexingUnit1.TFSEntityAttributes as GitCodeRepoTFSAttributes).DefaultBranch;
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> unitsWithGivenParent = this.IndexingUnitDataAccess.GetIndexingUnitsWithGivenParent(indexingExecutionContext.RequestContext, indexingUnit1.IndexingUnitId, -1);
        int attributesIfGitRepo = indexingUnit1.GetBranchCountFromTFSAttributesIfGitRepo();
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit2 in unitsWithGivenParent)
        {
          int estimatedDocCount;
          int estimatedDocCountGrowth;
          this.GetEstimatedDocCountOfScopedIndexingUnit(indexingExecutionContext, indexingUnit1, defaultBranch, indexingUnit2, idToProjectIdMap, out estimatedDocCount, out estimatedDocCountGrowth);
          int branchCount = Math.Max(1000, attributesIfGitRepo);
          int num = (int) ((double) (estimatedDocCount * branchCount) * indexingExecutionContext.ProvisioningContext.ContractType.GetFileCountFactorForMultibranch(indexingExecutionContext.RequestContext, indexingExecutionContext.IndexingUnit, branchCount));
          if (isReindexingInProgress)
          {
            string scopePath = this.m_codeFileContract.CorrectFilePath(indexingUnit2.GetScopePathFromTFSAttributesIfScopedIUElseNull());
            if (!string.IsNullOrWhiteSpace(scopePath))
            {
              try
              {
                long scopePathDocCount = this.m_codeFileContract.GetScopePathDocCount(indexingExecutionContext.RequestContext, indexingUnit1.TFSEntityId, scopePath, (IEnumerable<IndexInfo>) indexingUnit2.Properties.IndexIndices);
                num = Math.Max(num, Convert.ToInt32(Math.Min(scopePathDocCount, (long) int.MaxValue)));
              }
              catch (Exception ex)
              {
                indexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Unable to get estimates from ES. Exception message: {0}", (object) ex.Message)));
              }
            }
          }
          IndexingUnitWithSize indexingUnitWithSize = new IndexingUnitWithSize(indexingUnit2, num, estimatedDocCountGrowth, true)
          {
            ActualInitialDocCount = num
          };
          withSizeEstimates.Add(indexingUnitWithSize);
        }
      }
      return withSizeEstimates;
    }

    internal virtual void GetEstimatedDocCountOfScopedIndexingUnit(
      IndexingExecutionContext indexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit repoIndexingUnit,
      string defaultBranch,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit scopedIndexingUnit,
      Dictionary<Guid, Guid> repoIdToProjectId,
      out int estimatedDocCount,
      out int estimatedDocCountGrowth)
    {
      if (repoIndexingUnit.IndexingUnitType == "Git_Repository")
      {
        GitHttpClientWrapper httpClientWrapper = this.FetchGitHttpClientWrapper(indexingExecutionContext, repoIdToProjectId[repoIndexingUnit.TFSEntityId], repoIndexingUnit.TFSEntityId);
        ScopedGitRepositoryAttributes entityAttributes = scopedIndexingUnit.TFSEntityAttributes as ScopedGitRepositoryAttributes;
        string commitIdForScopePath = this.GetCommitIdForScopePath(scopedIndexingUnit.Properties as ScopedGitRepositoryIndexingProperties, entityAttributes, defaultBranch);
        string scopePath = entityAttributes.ScopePath;
        estimatedDocCount = httpClientWrapper.GetDocumentCount(indexingExecutionContext.RequestContext, scopePath, commitIdForScopePath);
        estimatedDocCountGrowth = 0;
      }
      else
      {
        estimatedDocCount = 0;
        estimatedDocCountGrowth = 0;
      }
    }

    internal virtual string GetCommitIdForScopePath(
      ScopedGitRepositoryIndexingProperties scopedGitRepositoryIndexingProperties,
      ScopedGitRepositoryAttributes scopedGitRepositoryAttributes,
      string defaultBranch)
    {
      string commitIdForScopePath = string.Empty;
      ScopedGitBranchIndexInfo gitBranchIndexInfo;
      if (!string.IsNullOrWhiteSpace(defaultBranch) && scopedGitRepositoryIndexingProperties.BranchIndexInfo.TryGetValue(defaultBranch, out gitBranchIndexInfo))
        commitIdForScopePath = gitBranchIndexInfo.CommitId;
      if (string.IsNullOrWhiteSpace(commitIdForScopePath))
      {
        foreach (KeyValuePair<string, ScopedGitBranchIndexInfo> keyValuePair in scopedGitRepositoryIndexingProperties.BranchIndexInfo)
        {
          if (!string.IsNullOrWhiteSpace(keyValuePair.Value.CommitId) && keyValuePair.Value.CommitId != RepositoryConstants.DefaultLastIndexCommitId)
          {
            commitIdForScopePath = keyValuePair.Value.CommitId;
            break;
          }
        }
      }
      return commitIdForScopePath;
    }

    internal virtual Dictionary<Guid, Guid> GetLargeRepositoryIdToProjectIdMap(
      IndexingExecutionContext indexingExecutionContext,
      List<IndexingUnitWithSize> largeRepos)
    {
      HashSet<int> hashSet = largeRepos.Select<IndexingUnitWithSize, int>((Func<IndexingUnitWithSize, int>) (x => x.IndexingUnit.ParentUnitId)).ToHashSet<int>();
      IDictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits = this.IndexingUnitDataAccess.GetIndexingUnits(indexingExecutionContext.RequestContext, (IEnumerable<int>) hashSet);
      Dictionary<Guid, Guid> idToProjectIdMap = new Dictionary<Guid, Guid>();
      foreach (IndexingUnitWithSize largeRepo in largeRepos)
        idToProjectIdMap[largeRepo.IndexingUnit.TFSEntityId] = indexingUnits[largeRepo.IndexingUnit.ParentUnitId].TFSEntityId;
      return idToProjectIdMap;
    }

    internal virtual void ProvisionDedicatedIndexForLargeRepository(
      IndexingExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit repository)
    {
      IIndexProvisioner indexProvisioner = executionContext.RequestContext.GetIndexProvisionerFactory(collectionIndexingUnit.EntityType).GetIndexProvisioner(executionContext, repository);
      string indexingIndexName = repository.GetIndexingIndexName();
      IndexingExecutionContext differentIndexingUnit = executionContext.ToIndexingExecutionContextWithDifferentIndexingUnit(repository);
      IndexingExecutionContext indexingExecutionContext = differentIndexingUnit;
      ISearchPlatform searchPlatform = differentIndexingUnit.ProvisioningContext.SearchPlatform;
      IndexIdentity indexIdentity1 = IndexIdentity.CreateIndexIdentity(indexingIndexName);
      IndexIdentity indexIdentity2 = indexProvisioner.ProvisionIndex(indexingExecutionContext, searchPlatform, indexIdentity1);
      if (indexIdentity2.Name.Equals(indexingIndexName))
        throw new SearchPlatformException(FormattableString.Invariant(FormattableStringFactory.Create("Could not provision index (other than current index {0}) for large repository {1}.", (object) indexingIndexName, (object) repository.TFSEntityId)));
      repository.Properties.IndexIndices = this.GetUpdatedIndexInfo(executionContext, repository, repository.Properties.IndexIndices, indexIdentity2.Name);
      this.RegistryManager.RemoveRegistryEntry("LargeRepositoryMultipeBranchIndexing", repository.TFSEntityId.ToString());
      this.RegistryManager.RemoveRegistryEntry("LargeRepositoryMultipeBranchIndexing", repository.IndexingUnitId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    private void UpdateScopedIndexingUnitsIndexInfo(
      IndexingExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit parentRepositoryIndexingUnit,
      string newIndexName)
    {
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> unitsWithGivenParent = this.IndexingUnitDataAccess.GetIndexingUnitsWithGivenParent(executionContext.RequestContext, parentRepositoryIndexingUnit.IndexingUnitId, -1);
      if (unitsWithGivenParent == null || !unitsWithGivenParent.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
        return;
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in unitsWithGivenParent)
        indexingUnit.Properties.SaveIndexingStatePreReindexing();
      this.IndexingUnitDataAccess.UpdateIndexingUnits(executionContext.RequestContext, unitsWithGivenParent.ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>());
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in unitsWithGivenParent)
        indexingUnit.Properties.ResetIndexingStatePreReindexing();
      this.IndexingUnitDataAccess.UpdateIndexingUnits(executionContext.RequestContext, unitsWithGivenParent.ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>());
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in unitsWithGivenParent)
        indexingUnit.Properties.IndexIndices = this.GetUpdatedIndexInfo(executionContext, indexingUnit, indexingUnit.Properties.IndexIndices, newIndexName);
      this.IndexingUnitDataAccess.UpdateIndexingUnits(executionContext.RequestContext, unitsWithGivenParent.ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>());
    }

    private List<IndexInfo> GetUpdatedIndexInfo(
      IndexingExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      List<IndexInfo> currentIndexInfo,
      string updatedIndexName)
    {
      if (!indexingUnit.IsLargeRepository(executionContext.RequestContext) && (currentIndexInfo == null || !currentIndexInfo.Any<IndexInfo>()))
        throw new SearchPlatformException(FormattableString.Invariant(FormattableStringFactory.Create("Index information is not available for non-large repo indexing unit {0}; can't update with new index information {1}.", (object) indexingUnit.TFSEntityId, (object) updatedIndexName)));
      if (currentIndexInfo != null && currentIndexInfo.Count > 0)
        return currentIndexInfo.Select<IndexInfo, IndexInfo>((Func<IndexInfo, IndexInfo>) (i => new IndexInfo()
        {
          IndexName = updatedIndexName,
          Routing = i.Routing,
          Version = i.Version
        })).ToList<IndexInfo>();
      return new List<IndexInfo>()
      {
        new IndexInfo()
        {
          IndexName = updatedIndexName,
          Version = new int?(executionContext.GetIndexVersion(updatedIndexName))
        }
      };
    }

    internal virtual GitHttpClientWrapper FetchGitHttpClientWrapper(
      IndexingExecutionContext indexingExecutionContext,
      Guid projectId,
      Guid repositoryId)
    {
      return new GitHttpClientWrapper((ExecutionContext) indexingExecutionContext, projectId, repositoryId, new TraceMetaData(1080611, "Indexing Pipeline", "IndexingOperation"));
    }

    public CollectionCodeMetadataCrawlOperation(
      IndexingExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : this(executionContext, indexingUnit, indexingUnitChangeEvent, CodeFileContract.CreateCodeContract(executionContext.ProvisioningContext.ContractType, executionContext.ProvisioningContext.SearchPlatform))
    {
    }

    internal CollectionCodeMetadataCrawlOperation(
      IndexingExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      CodeFileContract codeFileContract)
      : this(executionContext, indexingUnit, indexingUnitChangeEvent, new RegistryManager(executionContext.RequestContext, "IndexingOperation"))
    {
      this.m_codeFileContract = codeFileContract;
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080611, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder resultMessage = new StringBuilder();
      IndexingExecutionContext indexingExecutionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        if (!indexingExecutionContext.IsIndexingEnabled())
        {
          indexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Indexing is disabled, bailing out.")));
          operationResult.Status = OperationStatus.Succeeded;
          return operationResult;
        }
        if (this.m_isReIndexingInProgress && indexingExecutionContext.RequestContext.IsCodeReindexingWithZeroStalenessFeatureEnabled())
          this.m_isShadowCodeReIndexingRequired = true;
        bool flag = false;
        List<IndexingUnitWithSize> indexingUnitWithSizeList1 = this.CrawlAllSupportedTypes(indexingExecutionContext, resultMessage);
        if (coreIndexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/Routing/SizeBasedCustomRoutingEnabled", true))
        {
          int currentHostConfigValue1 = coreIndexingExecutionContext.RequestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/Routing/MaxAttemptCountForSizeEstimationOfAllIndexingUnits", true, 5);
          int currentHostConfigValue2 = coreIndexingExecutionContext.RequestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/Routing/MaxAttemptCountForSizeEstimationOfHalfOfTheIndexingUnits", true, 10);
          List<IndexingUnitWithSize> indexingUnitWithSizeList2 = new List<IndexingUnitWithSize>();
          int num1 = 0;
          foreach (IndexingUnitWithSize indexingUnitWithSize in indexingUnitWithSizeList1)
          {
            if (!indexingUnitWithSize.IndexingUnit.Properties.IsDisabled)
            {
              ++num1;
              if (indexingUnitWithSize.SizeEstimationSuccessful && !indexingUnitWithSize.IsEmpty())
                indexingUnitWithSizeList2.Add(indexingUnitWithSize);
            }
          }
          int count1 = indexingUnitWithSizeList1.Count;
          int count2 = indexingUnitWithSizeList2.Count;
          int num2 = count1 - num1;
          if (this.m_isReIndexingInProgress)
            flag = true;
          else if ((int) this.IndexingUnitChangeEvent.AttemptCount <= currentHostConfigValue1 && count2 == num1)
            flag = true;
          else if ((int) this.IndexingUnitChangeEvent.AttemptCount <= currentHostConfigValue2 && (int) this.IndexingUnitChangeEvent.AttemptCount > currentHostConfigValue1 && (double) count2 >= (double) num1 * 0.5 && count2 > 0)
            flag = true;
          else if ((int) this.IndexingUnitChangeEvent.AttemptCount > currentHostConfigValue2 && count2 > 0)
            flag = true;
          if (flag || (int) this.IndexingUnitChangeEvent.AttemptCount > currentHostConfigValue2)
          {
            List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> list1 = indexingUnitWithSizeList1.Select<IndexingUnitWithSize, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<IndexingUnitWithSize, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (x => x.IndexingUnit)).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
            if (list1.Count > 0)
            {
              Dictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> dictionary = this.IndexingUnitDataAccess.AddOrUpdateIndexingUnits(coreIndexingExecutionContext.RequestContext, list1, true).ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (x => x.TFSEntityId), (Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (x => x));
              foreach (IndexingUnitWithSize indexingUnitWithSize in indexingUnitWithSizeList1)
                indexingUnitWithSize.IndexingUnit = dictionary[indexingUnitWithSize.IndexingUnit.TFSEntityId];
            }
            this.ProcessIndexingUnits(indexingExecutionContext, this.IndexingUnit, indexingUnitWithSizeList1, indexingUnitWithSizeList2);
            List<IndexingUnitWithSize> list2 = indexingUnitWithSizeList1.Except<IndexingUnitWithSize>((IEnumerable<IndexingUnitWithSize>) indexingUnitWithSizeList2).ToList<IndexingUnitWithSize>();
            this.AddCollectionCodeBulkIndexOperation((ExecutionContext) coreIndexingExecutionContext, this.IndexingUnit.IndexingUnitId);
            if (list2.Count > 0)
              this.QueueRoutingAssignmentOperation(indexingExecutionContext, list2);
            if (flag)
            {
              resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully queued {0} to index {1} repos. ", (object) "BeginBulkIndex", (object) count2)));
              resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully queued {0} to assign routing to {1} repos. Total Repos {2}.", (object) "AssignRouting", (object) list2.Count, (object) count1)));
            }
            else
              resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Failed to get size estimate of any of the repo. Total Repos {0}.", (object) count1)));
            operationResult.Status = OperationStatus.Succeeded;
          }
          else
          {
            resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Retrying to get the size estimates, Total Repos {0}, Estimates available for {1} repos, Disabled repos: {2}", (object) count1, (object) count2, (object) num2)));
            resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Current Attempt Count {0}. ", (object) this.IndexingUnitChangeEvent.AttemptCount)) + FormattableString.Invariant(FormattableStringFactory.Create("Next attempt will be made after {0} secs.", (object) this.GetChangeEventDelay(coreIndexingExecutionContext, (Exception) null).TotalSeconds)));
            operationResult.Status = OperationStatus.FailedAndRetry;
          }
          IDictionary<string, object> properties = (IDictionary<string, object>) new Dictionary<string, object>();
          properties.Add("TotalTimeTakenForRoutingAssignmentInSeconds", (object) DateTime.UtcNow.Subtract(this.IndexingUnitChangeEvent.CreatedTimeUtc).TotalSeconds);
          properties.Add("TotalAttemptsForRoutingAssignment", (object) this.IndexingUnitChangeEvent.AttemptCount);
          properties.Add("TotalIndexingUnitsForRoutingAssignment", (object) num1);
          properties.Add("TotalIndexingUnitsWithSuccessfulRoutingAssignment", (object) count2);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("Indexing Pipeline", "IndexingOperation", properties, true);
        }
        else
        {
          List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> list = indexingUnitWithSizeList1.Select<IndexingUnitWithSize, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<IndexingUnitWithSize, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (x => x.IndexingUnit)).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
          if (list.Count > 0)
          {
            Dictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> dictionary = this.IndexingUnitDataAccess.AddOrUpdateIndexingUnits(coreIndexingExecutionContext.RequestContext, list, true).ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (x => x.TFSEntityId), (Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (x => x));
            foreach (IndexingUnitWithSize indexingUnitWithSize in indexingUnitWithSizeList1)
              indexingUnitWithSize.IndexingUnit = dictionary[indexingUnitWithSize.IndexingUnit.TFSEntityId];
          }
          this.ProcessIndexingUnits(indexingExecutionContext, this.IndexingUnit, indexingUnitWithSizeList1, new List<IndexingUnitWithSize>());
          this.AddCollectionCodeBulkIndexOperation((ExecutionContext) coreIndexingExecutionContext, this.IndexingUnit.IndexingUnitId);
          resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully queued {0} to index {1} repos. ", (object) "BeginBulkIndex", (object) indexingUnitWithSizeList1.Count)));
          operationResult.Status = OperationStatus.Succeeded;
        }
      }
      finally
      {
        operationResult.Message = resultMessage.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080611, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
      return operationResult;
    }

    internal virtual List<IndexingUnitWithSize> CrawlAllSupportedTypes(
      IndexingExecutionContext indexingExecutionContext,
      StringBuilder resultMessage)
    {
      List<IndexingUnitWithSize> indexingUnitWithSizeList = new List<IndexingUnitWithSize>();
      foreach (string supportedType in CollectionBulkIndexTypes.GetSupportedTypes(indexingExecutionContext, CollectionCodeBulkIndexOperationType.MetadataCrawl))
      {
        List<IndexingUnitWithSize> collection = CollectionCodeMetadataCrawlerFactory.GetCrawler(indexingExecutionContext, this.DataAccessFactory, supportedType, this.m_codeFileContract).CrawlMetadata(indexingExecutionContext, this.IndexingUnit, this.m_isShadowCodeReIndexingRequired);
        resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Successfully added {0} IndexingUnits of type '{1}'.", (object) collection.Count, (object) supportedType, (object) this.IndexingUnit.TFSEntityId);
        indexingUnitWithSizeList.AddRange((IEnumerable<IndexingUnitWithSize>) collection);
      }
      return indexingUnitWithSizeList;
    }

    protected internal override TimeSpan GetChangeEventDelay(
      CoreIndexingExecutionContext executionContext,
      Exception e)
    {
      return e == null ? TimeSpan.FromSeconds((double) (executionContext.RequestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/Routing/RoutingAssignmentOperationDelayInSecs", true, 300) * (int) this.IndexingUnitChangeEvent.AttemptCount)) : base.GetChangeEventDelay(executionContext, e);
    }

    protected override int GetMaxIndexingRetryCount(ExecutionContext executionContext) => executionContext.RequestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/Routing/MaxAttemptCountForSizeEstimationOfHalfOfTheIndexingUnits", 10) + 3;

    internal virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent AddCollectionCodeBulkIndexOperation(
      ExecutionContext executionContext,
      int indexingUnitId)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent(this.IndexingUnitChangeEvent.LeaseId);
      indexingUnitChangeEvent1.IndexingUnitId = indexingUnitId;
      CodeBulkIndexEventData bulkIndexEventData = new CodeBulkIndexEventData(executionContext);
      bulkIndexEventData.Finalize = true;
      bulkIndexEventData.Trigger = this.IndexingUnitChangeEvent.ChangeData.Trigger;
      indexingUnitChangeEvent1.ChangeData = (ChangeEventData) bulkIndexEventData;
      indexingUnitChangeEvent1.ChangeType = "BeginBulkIndex";
      indexingUnitChangeEvent1.State = IndexingUnitChangeEventState.Pending;
      indexingUnitChangeEvent1.AttemptCount = (byte) 0;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent2 = indexingUnitChangeEvent1;
      return this.IndexingUnitChangeEventHandler.HandleEvent(executionContext, indexingUnitChangeEvent2);
    }

    protected internal override void HandleOperationFailure(
      IndexingExecutionContext indexingExecutionContext,
      OperationResult result,
      Exception e)
    {
      base.HandleOperationFailure(indexingExecutionContext, result, e);
      if (result.Status != OperationStatus.Failed)
        return;
      indexingExecutionContext.RequestContext.QueuePeriodicCatchUpJob(indexingExecutionContext.ServiceSettings.JobSettings.PeriodicCatchUpJobDelayInSec);
      TeamFoundationEventLog.Default.Log(result.Message, SearchEventId.CollectionCodeMetadataCrawlOperationFailed, EventLogEntryType.Error);
    }

    internal virtual List<IndexingUnitWithSize> ProcessIndexingUnits(
      IndexingExecutionContext indexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit,
      List<IndexingUnitWithSize> indexingUnitsWithSize,
      List<IndexingUnitWithSize> allRepoIndexingUnitsWithSuccessfulSizeEstimations)
    {
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitList = indexingUnitsWithSize.Select<IndexingUnitWithSize, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<IndexingUnitWithSize, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (x => x.IndexingUnit)).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      bool currentHostConfigValue = indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/Routing/SizeBasedIndexProvisioningEnabled", true);
      IRoutingService service = indexingExecutionContext.RequestContext.GetService<IRoutingService>();
      if (indexingUnitList.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
      {
        if (this.m_isReIndexingInProgress)
        {
          indexingUnitList.ForEach((Action<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (repo => repo.Properties.SaveIndexingStatePreReindexing()));
          List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnit1 = this.IndexingUnitDataAccess.UpdateIndexingUnits(indexingExecutionContext.RequestContext, indexingUnitList);
          foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit2 in indexingUnit1)
          {
            if (!indexingUnit2.IsLargeRepository(indexingExecutionContext.RequestContext))
              indexingUnit2.Properties.ResetIndexingStatePreReindexing();
          }
          indexingUnitList = this.IndexingUnitDataAccess.UpdateIndexingUnits(indexingExecutionContext.RequestContext, indexingUnit1);
        }
        if (indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/Routing/SizeBasedCustomRoutingEnabled", true))
        {
          Dictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> dictionary = indexingUnitList.ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (x => x.TFSEntityId), (Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (x => x));
          foreach (IndexingUnitWithSize successfulSizeEstimation in allRepoIndexingUnitsWithSuccessfulSizeEstimations)
            successfulSizeEstimation.IndexingUnit = dictionary[successfulSizeEstimation.IndexingUnit.TFSEntityId];
          List<IndexingUnitWithSize> all = allRepoIndexingUnitsWithSuccessfulSizeEstimations.FindAll((Predicate<IndexingUnitWithSize>) (x => x.IndexingUnit.IsLargeRepository(indexingExecutionContext.RequestContext)));
          List<IndexingUnitWithSize> list = allRepoIndexingUnitsWithSuccessfulSizeEstimations.Except<IndexingUnitWithSize>((IEnumerable<IndexingUnitWithSize>) all).ToList<IndexingUnitWithSize>();
          List<IndexingUnitWithSize> indexingUnitWithSizeList = new List<IndexingUnitWithSize>();
          if (all.Count > 0)
          {
            List<IndexingUnitWithSize> withSizeEstimates = this.GetLargeRepoScopedIndexingUnitsWithSizeEstimates(indexingExecutionContext, all, this.m_isReIndexingInProgress);
            indexingUnitWithSizeList.AddRange((IEnumerable<IndexingUnitWithSize>) all);
            indexingUnitWithSizeList.AddRange((IEnumerable<IndexingUnitWithSize>) withSizeEstimates);
          }
          if (list.Count > 0)
            indexingUnitWithSizeList.AddRange((IEnumerable<IndexingUnitWithSize>) list);
          if (indexingUnitWithSizeList.Count > 0)
          {
            if (currentHostConfigValue)
            {
              service.AssignIndex(indexingExecutionContext, indexingUnitWithSizeList);
            }
            else
            {
              if (all.Count > 0)
                all.Select<IndexingUnitWithSize, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<IndexingUnitWithSize, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (x => x.IndexingUnit)).ForEach<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Action<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (x => this.ProvisionDedicatedIndexForLargeRepository(indexingExecutionContext, collectionIndexingUnit, x)));
              service.AssignShards(indexingExecutionContext, indexingUnitWithSizeList);
            }
          }
          else if (currentHostConfigValue)
            service.AssignIndex(indexingExecutionContext, allRepoIndexingUnitsWithSuccessfulSizeEstimations);
        }
        else
        {
          foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in indexingUnitList)
          {
            if (indexingUnit.IsLargeRepository(indexingExecutionContext.RequestContext))
            {
              this.ProvisionDedicatedIndexForLargeRepository(indexingExecutionContext, collectionIndexingUnit, indexingUnit);
              this.UpdateScopedIndexingUnitsIndexInfo(indexingExecutionContext, indexingUnit, indexingUnit.Properties.IndexIndices.First<IndexInfo>().IndexName);
            }
            else
              indexingUnit.SetupIndexRouting(indexingExecutionContext, (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) indexingUnitList);
          }
          this.IndexingUnitDataAccess.UpdateIndexingUnits(indexingExecutionContext.RequestContext, indexingUnitList);
        }
      }
      else if (currentHostConfigValue)
        service.AssignIndex(indexingExecutionContext, allRepoIndexingUnitsWithSuccessfulSizeEstimations);
      return indexingUnitsWithSize;
    }

    internal virtual void QueueRoutingAssignmentOperation(
      IndexingExecutionContext indexingExecutionContext,
      List<IndexingUnitWithSize> reposWithNoSizeEstimates)
    {
      if (reposWithNoSizeEstimates == null)
        return;
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>(reposWithNoSizeEstimates.Count);
      foreach (IndexingUnitWithSize withNoSizeEstimate in reposWithNoSizeEstimates)
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
        {
          IndexingUnitId = withNoSizeEstimate.IndexingUnit.IndexingUnitId,
          ChangeData = new ChangeEventData((ExecutionContext) indexingExecutionContext),
          ChangeType = "AssignRouting",
          State = IndexingUnitChangeEventState.Pending,
          AttemptCount = 0,
          LeaseId = this.IndexingUnitChangeEvent.LeaseId
        };
        indexingUnitChangeEventList.Add(indexingUnitChangeEvent);
      }
      this.IndexingUnitChangeEventHandler.HandleEvents((ExecutionContext) indexingExecutionContext, (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) indexingUnitChangeEventList);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080611, "Indexing Pipeline", "IndexingOperation", FormattableString.Invariant(FormattableStringFactory.Create("Added {0} events to assign routing to {1} repos where size estimates wasn't available.", (object) indexingUnitChangeEventList.Count, (object) reposWithNoSizeEstimates.Count)));
    }
  }
}
