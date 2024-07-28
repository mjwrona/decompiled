// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.CollectionCodeFinalizeHelper
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class CollectionCodeFinalizeHelper : EntityFinalizerBase
  {
    public override IndexingUnitChangeEventPrerequisites QueueFinalizeOperationIfAllowed(
      IndexingExecutionContext indexingExecutionContext,
      IIndexingUnitChangeEventHandler changeEventHandler)
    {
      IndexingUnitChangeEventPrerequisites eventPrerequisites1 = (IndexingUnitChangeEventPrerequisites) null;
      if (this.CanFinalizeIndex(indexingExecutionContext))
      {
        IndexingUnitChangeEvent indexingUnitChangeEvent1 = new IndexingUnitChangeEvent()
        {
          IndexingUnitId = indexingExecutionContext.CollectionIndexingUnit.IndexingUnitId,
          ChangeData = (ChangeEventData) new CodeIndexPublishData((ExecutionContext) indexingExecutionContext),
          ChangeType = "CompleteBulkIndex",
          State = IndexingUnitChangeEventState.Pending,
          AttemptCount = 0
        };
        IndexingUnitChangeEvent indexingUnitChangeEvent2 = changeEventHandler.HandleEventWithAddingEventWhenNeeded((ExecutionContext) indexingExecutionContext, indexingUnitChangeEvent1);
        IndexingUnitChangeEventPrerequisites eventPrerequisites2 = new IndexingUnitChangeEventPrerequisites();
        eventPrerequisites2.Add(new IndexingUnitChangeEventPrerequisitesFilter()
        {
          Id = indexingUnitChangeEvent2.Id,
          Operator = IndexingUnitChangeEventFilterOperator.Contains,
          PossibleStates = new List<IndexingUnitChangeEventState>()
          {
            IndexingUnitChangeEventState.Succeeded,
            IndexingUnitChangeEventState.Failed
          }
        });
        eventPrerequisites1 = eventPrerequisites2;
      }
      return eventPrerequisites1;
    }

    public override bool ShouldFinalizeChildIndexingUnit(
      IndexingExecutionContext executionContext,
      IndexingUnit childIndexingUnit)
    {
      if (childIndexingUnit == null)
        throw new ArgumentNullException(nameof (childIndexingUnit));
      bool flag = true;
      if (childIndexingUnit.IsShadow)
        return false;
      string indexingIndexName = childIndexingUnit.GetIndexingIndexName();
      string str = (string) null;
      if (childIndexingUnit.Properties != null && childIndexingUnit.Properties.IndexIndices != null && childIndexingUnit.Properties.IndexIndices.Count == 1)
        str = childIndexingUnit.Properties.IndexIndices.First<IndexInfo>().Routing;
      if (!string.IsNullOrWhiteSpace(indexingIndexName) && !string.IsNullOrWhiteSpace(str) && executionContext.CollectionIndexingUnit.Properties.QueryIndices != null)
      {
        foreach (IndexInfo queryIndex in executionContext.CollectionIndexingUnit.Properties.QueryIndices)
        {
          if (indexingIndexName.Equals(queryIndex.IndexName, StringComparison.Ordinal) && !string.IsNullOrWhiteSpace(queryIndex.Routing))
          {
            if (((IEnumerable<string>) queryIndex.Routing.Split(',')).Contains<string>(str, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
              flag = false;
          }
        }
      }
      return flag;
    }

    internal override bool CanUpdateQueryProperties(IndexingExecutionContext executionContext) => executionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/UpdateCodeQueryProperties", defaultValue: true);

    internal override List<IndexInfo> GetQueryIndexInfoWhenReIndexCompleted(
      IndexingExecutionContext executionContext)
    {
      IndexingExecutionContext differentIndexingUnit = executionContext.ToIndexingExecutionContextWithDifferentIndexingUnit(executionContext.IndexingUnit);
      IEnumerable<IndexingUnit> indexingUnitsToFinalize = this.GetChildrenIndexingUnitsToFinalize(executionContext, true);
      IndexIdentity index = differentIndexingUnit.GetIndex();
      return this.CreateIndexScopesFromRepositoryInfo(indexingUnitsToFinalize, executionContext, index);
    }

    internal override List<IndexInfo> GetQueryIndexInfo(IndexingExecutionContext executionContext)
    {
      IEnumerable<IndexingUnit> indexingUnitsToFinalize = this.GetChildrenIndexingUnitsToFinalize(executionContext);
      IndexIdentity index = executionContext.ToIndexingExecutionContextWithDifferentIndexingUnit(executionContext.IndexingUnit).GetIndex();
      return this.CreateIndexScopesFromRepositoryInfo(indexingUnitsToFinalize, executionContext, index);
    }

    internal override List<IndexInfo> GetQueryIndexInfoWhenReIndexInProgress(
      IndexingExecutionContext executionContext)
    {
      executionContext.ToIndexingExecutionContextWithDifferentIndexingUnit(executionContext.IndexingUnit);
      IEnumerable<IndexingUnit> indexingUnitsToFinalize = this.GetChildrenIndexingUnitsToFinalize(executionContext);
      string indexIdentity1 = (string) null;
      foreach (IndexingUnit indexingUnit in indexingUnitsToFinalize)
      {
        if (!indexingUnit.IsLargeRepository(executionContext.RequestContext))
        {
          IndexInfo indexInfo = indexingUnit.GetIndexInfo();
          if (indexInfo != null)
          {
            indexIdentity1 = indexInfo.IndexName;
            break;
          }
        }
      }
      IndexIdentity indexIdentity2 = !string.IsNullOrEmpty(indexIdentity1) ? IndexIdentity.CreateIndexIdentity(indexIdentity1) : (IndexIdentity) null;
      return this.CreateIndexScopesFromRepositoryInfo(indexingUnitsToFinalize, executionContext, indexIdentity2);
    }

    internal virtual bool HasDedicatedIndex(
      IndexingExecutionContext executionContext,
      IndexingUnit indexingUnit)
    {
      bool currentHostConfigValue = executionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/Routing/SizeBasedCustomRoutingEnabled", true);
      if (indexingUnit.IsLargeRepository(executionContext.RequestContext))
      {
        bool flag = executionContext.IsReindexingFailedOrInProgress(executionContext.DataAccessFactory, (IEntityType) CodeEntityType.GetInstance());
        if (!currentHostConfigValue || indexingUnit.IndexingUnitType == "Git_Repository" || indexingUnit.IndexingUnitType == "CustomRepository" && !flag)
          return true;
      }
      return false;
    }

    private List<IndexInfo> CreateIndexScopesFromRepositoryInfo(
      IEnumerable<IndexingUnit> repositoriesToFinalize,
      IndexingExecutionContext executionContext,
      IndexIdentity collectionIndex)
    {
      IDictionary<IndexingUnit, IndexIdentity> largeRepositoryIndices = (IDictionary<IndexingUnit, IndexIdentity>) new FriendlyDictionary<IndexingUnit, IndexIdentity>();
      IList<IndexSubScope> nonLargeRepositories = (IList<IndexSubScope>) new List<IndexSubScope>();
      Guid organizationId = executionContext.RequestContext.GetOrganizationID();
      Guid collectionId = executionContext.RequestContext.GetCollectionID();
      foreach (IndexingUnit indexingUnit in repositoriesToFinalize)
      {
        if (this.HasDedicatedIndex(executionContext, indexingUnit))
        {
          IndexIdentity index = executionContext.ToIndexingExecutionContextWithDifferentIndexingUnit(indexingUnit).GetIndex();
          largeRepositoryIndices.Add(indexingUnit, index);
        }
        else
        {
          IndexInfo indexInfo = indexingUnit.GetIndexInfo();
          if (!string.IsNullOrWhiteSpace(indexInfo?.Routing))
            nonLargeRepositories.Add(new IndexSubScope()
            {
              AccountId = organizationId.ToString(),
              CollectionId = collectionId.ToString(),
              IndexRouting = indexInfo
            });
        }
      }
      return this.ConstructQueryIndicesFromRepoData(collectionIndex, largeRepositoryIndices, nonLargeRepositories, executionContext);
    }

    private List<IndexInfo> ConstructQueryIndicesFromRepoData(
      IndexIdentity collectionIndex,
      IDictionary<IndexingUnit, IndexIdentity> largeRepositoryIndices,
      IList<IndexSubScope> nonLargeRepositories,
      IndexingExecutionContext executionContext)
    {
      IndexingExecutionContext differentIndexingUnit = executionContext.ToIndexingExecutionContextWithDifferentIndexingUnit(executionContext.IndexingUnit);
      ISearchPlatform platformForQueryCluster = this.GetSearchPlatformForQueryCluster(differentIndexingUnit);
      List<IndexInfo> indexInfoList = new List<IndexInfo>();
      if (collectionIndex != null)
      {
        string[] strArray = IndexSubScope.GetRoutingValuesToPublishIndex(differentIndexingUnit.GetRouteLevel(), nonLargeRepositories).Split(new char[1]
        {
          ','
        }, StringSplitOptions.RemoveEmptyEntries);
        HashSet<string> values = new HashSet<string>();
        foreach (string str in strArray)
        {
          if (!string.IsNullOrWhiteSpace(str))
            values.Add(str);
        }
        string str1 = string.Join(",", (IEnumerable<string>) values);
        indexInfoList.Add(new IndexInfo()
        {
          EntityName = executionContext.IndexingUnit.Properties.Name,
          IndexName = collectionIndex.Name,
          Version = new int?(platformForQueryCluster.GetIndexVersion((IEntityType) CodeEntityType.GetInstance(), collectionIndex.Name)),
          Routing = str1
        });
      }
      if (largeRepositoryIndices.Any<KeyValuePair<IndexingUnit, IndexIdentity>>())
      {
        foreach (KeyValuePair<IndexingUnit, IndexIdentity> largeRepositoryIndex in (IEnumerable<KeyValuePair<IndexingUnit, IndexIdentity>>) largeRepositoryIndices)
          indexInfoList.Add(new IndexInfo()
          {
            EntityName = largeRepositoryIndex.Key.Properties.Name,
            IndexName = largeRepositoryIndex.Value.Name,
            Version = new int?(platformForQueryCluster.GetIndexVersion((IEntityType) CodeEntityType.GetInstance(), largeRepositoryIndex.Value.Name)),
            Routing = IndexSubScope.GetRoutingValuesToPublishIndex(differentIndexingUnit.GetRouteLevel(), (IList<IndexSubScope>) null)
          });
      }
      return indexInfoList;
    }

    internal virtual ISearchPlatform GetSearchPlatformForQueryCluster(
      IndexingExecutionContext collectionIndexingExecutionContext)
    {
      return SearchPlatformFactory.GetInstance().Create(((CollectionIndexingProperties) collectionIndexingExecutionContext.IndexingUnit.Properties).QueryESConnectionString, collectionIndexingExecutionContext.ProvisioningContext.SearchPlatformSettings, collectionIndexingExecutionContext.ProvisioningContext.IsOnPrem);
    }

    internal override void PromoteShadowIndexingUnitsToPrimary(
      IndexingExecutionContext indexingExecutionContext,
      bool isZLRIEnabledForCustom = false)
    {
      List<string> supportingShadowIndexing = this.GetIndexingUnitsTypeSupportingShadowIndexing(false);
      if (!isZLRIEnabledForCustom)
        supportingShadowIndexing.Remove("CustomRepository");
      indexingExecutionContext.IndexingUnitDataAccess.PromoteShadowIndexingUnitsToPrimary(indexingExecutionContext.RequestContext, supportingShadowIndexing, (IEntityType) CodeEntityType.GetInstance());
    }

    internal override List<string> GetIndexingUnitsTypeSupportingShadowIndexing(bool rootOnly = false)
    {
      if (rootOnly)
        return new List<string>()
        {
          "Git_Repository",
          "TFVC_Repository",
          "CustomRepository"
        };
      return new List<string>()
      {
        "Git_Repository",
        "ScopedIndexingUnit",
        "TemporaryIndexingUnit",
        "TFVC_Repository",
        "CustomRepository"
      };
    }
  }
}
