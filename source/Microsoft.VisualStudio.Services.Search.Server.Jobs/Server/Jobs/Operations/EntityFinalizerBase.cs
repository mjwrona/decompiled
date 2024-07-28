// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.EntityFinalizerBase
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations
{
  internal abstract class EntityFinalizerBase
  {
    public void FinalizeQueryProperties(IndexingExecutionContext indexingExecutionContext)
    {
      if (!this.CanUpdateQueryProperties(indexingExecutionContext))
        return;
      CollectionIndexingProperties properties = (CollectionIndexingProperties) indexingExecutionContext.IndexingUnit.Properties;
      properties.QueryContractType = properties.IndexContractType;
      properties.QueryESConnectionString = properties.IndexESConnectionString;
      properties.QueryIndices = this.GetQueryIndexInfo(indexingExecutionContext);
      indexingExecutionContext.IndexingUnitDataAccess.UpdateIndexingUnit(indexingExecutionContext.RequestContext, indexingExecutionContext.IndexingUnit);
    }

    public void FinalizeQueryPropertiesOnReindexCompletion(
      IndexingExecutionContext indexingExecutionContext)
    {
      CollectionIndexingProperties properties = (CollectionIndexingProperties) indexingExecutionContext.IndexingUnit.Properties;
      properties.QueryContractType = properties.IndexContractType;
      properties.QueryESConnectionString = properties.IndexESConnectionString;
      properties.QueryIndices = this.GetQueryIndexInfoWhenReIndexCompleted(indexingExecutionContext);
      indexingExecutionContext.IndexingUnitDataAccess.UpdateIndexingUnit(indexingExecutionContext.RequestContext, indexingExecutionContext.IndexingUnit);
    }

    public void FinalizeQueryPropertiesWhenReIndexInProgress(
      IndexingExecutionContext indexingExecutionContext)
    {
      indexingExecutionContext.IndexingUnit.Properties.QueryIndices = this.GetQueryIndexInfoWhenReIndexInProgress(indexingExecutionContext);
      indexingExecutionContext.IndexingUnitDataAccess.UpdateIndexingUnit(indexingExecutionContext.RequestContext, indexingExecutionContext.IndexingUnit);
    }

    public void NotifyIndexPropertiesUpdates(IndexingExecutionContext indexingExecutionContext) => indexingExecutionContext.RequestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(indexingExecutionContext.RequestContext, Microsoft.VisualStudio.Services.Search.Common.SqlNotificationEventClasses.DocumentContractTypeChanged, (string) null);

    public virtual void UpdateFeatureFlagsIfNeeded(IndexingExecutionContext indexingExecutionContext)
    {
    }

    internal virtual void PromoteShadowIndexingUnitsToPrimary(
      IndexingExecutionContext indexingExecutionContext,
      bool isZLRIEnabledForCustom = false)
    {
    }

    internal virtual List<string> GetIndexingUnitsTypeSupportingShadowIndexing(bool rootOnly = false) => new List<string>();

    public virtual IndexingUnitChangeEventPrerequisites QueueFinalizeOperationIfAllowed(
      IndexingExecutionContext indexingExecutionContext,
      IIndexingUnitChangeEventHandler changeEventHandler)
    {
      return (IndexingUnitChangeEventPrerequisites) null;
    }

    public bool CanFinalizeIndex(IndexingExecutionContext indexingExecutionContext) => indexingExecutionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment || !indexingExecutionContext.IsReindexingFailedOrInProgress(indexingExecutionContext.DataAccessFactory, indexingExecutionContext.IndexingUnit.EntityType);

    public virtual bool ShouldFinalizeChildIndexingUnit(
      IndexingExecutionContext indexingExecutionContext,
      IndexingUnit childIndexingUnit)
    {
      return false;
    }

    internal virtual IEnumerable<IndexingUnit> GetLargeChildrenIndexingUnitsToFinalize(
      IndexingExecutionContext indexingExecutionContext,
      bool isShadow = false)
    {
      return this.GetChildrenIndexingUnitsToFinalize(indexingExecutionContext, isShadow).Where<IndexingUnit>((Func<IndexingUnit, bool>) (r => r.IsLargeRepository(indexingExecutionContext.RequestContext)));
    }

    internal virtual IEnumerable<IndexingUnit> GetChildrenIndexingUnitsToFinalize(
      IndexingExecutionContext indexingExecutionContext,
      bool isShadow = false)
    {
      List<IndexingUnit> indexingUnitsToFinalize = new List<IndexingUnit>();
      foreach (string supportedType in CollectionBulkIndexTypes.GetSupportedTypes(indexingExecutionContext, CollectionCodeBulkIndexOperationType.Finalize))
      {
        IEnumerable<IndexingUnit> indexingUnits = (IEnumerable<IndexingUnit>) indexingExecutionContext.IndexingUnitDataAccess.GetIndexingUnits(indexingExecutionContext.RequestContext, supportedType, isShadow, indexingExecutionContext.IndexingUnit.EntityType, -1);
        if (indexingUnits != null && indexingUnits.Any<IndexingUnit>())
          indexingUnitsToFinalize.AddRange(indexingUnits);
      }
      return (IEnumerable<IndexingUnit>) indexingUnitsToFinalize;
    }

    internal virtual bool CanUpdateQueryProperties(IndexingExecutionContext indexingExecutionContext) => true;

    internal void DeleteDataFromOldIndices(
      IndexingExecutionContext indexingExecutionContext,
      IEnumerable<IndexInfo> indicesToClean)
    {
      this.DeleteDataFromIndices(indexingExecutionContext, indicesToClean, this.GetOldIndexDocumentContractType(indexingExecutionContext));
    }

    internal void DeleteDataFromCurrentIndices(
      IndexingExecutionContext indexingExecutionContext,
      IEnumerable<IndexInfo> indicesToClean)
    {
      this.DeleteDataFromIndices(indexingExecutionContext, indicesToClean, this.GetCurrentIndexDocumentContractType(indexingExecutionContext));
    }

    [Info("InternalForTestPurpose")]
    internal abstract List<IndexInfo> GetQueryIndexInfo(
      IndexingExecutionContext indexingExecutionContext);

    [Info("InternalForTestPurpose")]
    internal abstract List<IndexInfo> GetQueryIndexInfoWhenReIndexCompleted(
      IndexingExecutionContext indexingExecutionContext);

    [Info("InternalForTestPurpose")]
    internal abstract List<IndexInfo> GetQueryIndexInfoWhenReIndexInProgress(
      IndexingExecutionContext indexingExecutionContext);

    [Info("InternalForTestPurpose")]
    internal void DeleteDataFromIndices(
      IndexingExecutionContext indexingExecutionContext,
      IEnumerable<IndexInfo> indicesToClean,
      DocumentContractType documentContractType)
    {
      if (indicesToClean == null)
        return;
      foreach (IndexInfo indexInfo in indicesToClean)
      {
        ISearchIndex index = indexingExecutionContext.ProvisioningContext.SearchPlatform.GetIndex(IndexIdentity.CreateIndexIdentity(indexInfo.IndexName));
        IndexOperationsResponse operationsResponse = index.BulkDeleteByQuery<AbstractSearchDocumentContract>((ExecutionContext) indexingExecutionContext, new BulkDeleteByQueryRequest<AbstractSearchDocumentContract>()
        {
          Query = (IExpression) new TermExpression("collectionId", Operator.Equals, indexingExecutionContext.RequestContext.GetCollectionID().ToString().ToLowerInvariant()),
          ContractType = documentContractType,
          Lenient = true
        }, true);
        Guid collectionId = indexingExecutionContext.RequestContext.GetCollectionID();
        if (operationsResponse.Success)
          indexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Deleted documents of collection {0} from index {1} and contract type {2}. ", (object) collectionId, (object) index.IndexIdentity.Name, (object) documentContractType)));
        else
          throw new SearchPlatformException(FormattableString.Invariant(FormattableStringFactory.Create("Deleting documents of collection {0} from index {1} and contract type {2} failed. Response: {3}", (object) collectionId, (object) index.IndexIdentity.Name, (object) documentContractType, (object) operationsResponse.Serialize<IndexOperationsResponse>())));
      }
    }

    private DocumentContractType GetCurrentIndexDocumentContractType(
      IndexingExecutionContext indexingExecutionContext)
    {
      return indexingExecutionContext.RequestContext.GetService<IDocumentContractTypeService>().GetSupportedIndexDocumentContractType(indexingExecutionContext.RequestContext, indexingExecutionContext.IndexingUnit.EntityType);
    }

    private DocumentContractType GetOldIndexDocumentContractType(
      IndexingExecutionContext indexingExecutionContext)
    {
      return indexingExecutionContext.CollectionIndexingUnit != null && indexingExecutionContext.CollectionIndexingUnit.Properties is CollectionIndexingProperties properties && properties.QueryContractTypePreReindexing != DocumentContractType.Unsupported ? properties.QueryContractTypePreReindexing : this.GetCurrentIndexDocumentContractType(indexingExecutionContext);
    }
  }
}
