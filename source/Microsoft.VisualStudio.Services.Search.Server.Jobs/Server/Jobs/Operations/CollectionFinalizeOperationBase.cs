// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.CollectionFinalizeOperationBase
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations
{
  internal abstract class CollectionFinalizeOperationBase : AbstractIndexingOperation
  {
    private readonly int m_tracepoint;
    [Info("InternalForTestPurpose")]
    protected internal StringBuilder ResultMessage = new StringBuilder();
    private IList<IndexInfo> m_currentIndices;
    private IList<IndexInfo> m_oldSharedIndices;
    protected IList<IndexInfo> m_oldDedicatedIndices;

    public CollectionFinalizeOperationBase(
      Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      int tracepoint)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
      this.m_tracepoint = tracepoint;
    }

    protected internal EntityFinalizerBase FinalizeHelper { get; set; }

    protected abstract string CrudOperationsFeatureName { get; }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(this.m_tracepoint, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        if (executionContext.RequestContext.IsZeroStalenessReindexingEnabled(executionContext.IndexingUnit.EntityType))
        {
          if ((this.IsReindexingFailedInProgress(executionContext) ? 1 : 0) != 0)
          {
            if (this.IndexingUnitChangeEvent.ChangeData.Trigger == 33 && this.FinalizeHelper.CanUpdateQueryProperties(executionContext))
            {
              bool flag = this.IsReindexingSuccessful(executionContext);
              try
              {
                if (flag)
                {
                  this.FinalizeHelper.FinalizeQueryPropertiesOnReindexCompletion(executionContext);
                  this.FinalizeHelper.NotifyIndexPropertiesUpdates(executionContext);
                  bool currentHostConfigValue = executionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/IsZLRIEnabledForCustom", true);
                  this.FinalizeHelper.PromoteShadowIndexingUnitsToPrimary(executionContext, currentHostConfigValue);
                  this.UpdateReindexingStatusAndTurnOnIndexingFeatureFlags(coreIndexingExecutionContext.RequestContext, coreIndexingExecutionContext.IndexingUnit.EntityType, Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.Completed);
                  this.QueueDeleteOperationForShadowIndexingUnits(executionContext);
                  Thread.Sleep(TimeSpan.FromSeconds((double) coreIndexingExecutionContext.GetConfigValue<int>("/Service/ALMSearch/Settings/PreCleanupOldDataDelayInSeconds")));
                  this.CleanupOldData(executionContext);
                  this.ErasePreReindexingState(executionContext);
                  this.PublishReIndexTelemetryForCompletion(coreIndexingExecutionContext);
                }
                else
                {
                  this.UpdateReindexingStatusAndTurnOnIndexingFeatureFlags(coreIndexingExecutionContext.RequestContext, coreIndexingExecutionContext.IndexingUnit.EntityType, Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.Failed);
                  this.PublishReIndexTelemetryForFailure(coreIndexingExecutionContext);
                }
              }
              finally
              {
                coreIndexingExecutionContext.RequestContext.GetService<IReindexingStatusService>().ServiceEnd(coreIndexingExecutionContext.RequestContext);
              }
            }
            else
            {
              this.FinalizeHelper.FinalizeQueryPropertiesWhenReIndexInProgress(executionContext);
              this.FinalizeHelper.NotifyIndexPropertiesUpdates(executionContext);
            }
          }
          else
            this.FinalizeAndNotifyQueryProperties(executionContext);
        }
        else
        {
          bool flag1 = this.IsReindexingFailedInProgress(executionContext);
          bool flag2 = false;
          if (flag1)
            flag2 = this.IsReindexingSuccessful(executionContext);
          if (!flag1 | flag2)
            this.FinalizeAndNotifyQueryProperties(executionContext);
          if (flag2)
          {
            this.UpdateReindexingStatusAndTurnOnIndexingFeatureFlags(coreIndexingExecutionContext.RequestContext, coreIndexingExecutionContext.IndexingUnit.EntityType, Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.Completed);
            if (this.IndexingUnitChangeEvent.ChangeData.Trigger == 33 && this.FinalizeHelper.CanUpdateQueryProperties(executionContext))
            {
              Thread.Sleep(TimeSpan.FromSeconds((double) coreIndexingExecutionContext.GetConfigValue<int>("/Service/ALMSearch/Settings/PreCleanupOldDataDelayInSeconds")));
              this.CleanupOldData(executionContext);
              this.ErasePreReindexingState(executionContext);
            }
            this.PublishReIndexTelemetryForCompletion(coreIndexingExecutionContext);
          }
          else if (flag1 && !flag2)
          {
            this.UpdateReindexingStatusAndTurnOnIndexingFeatureFlags(coreIndexingExecutionContext.RequestContext, coreIndexingExecutionContext.IndexingUnit.EntityType, Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.Failed);
            this.PublishReIndexTelemetryForFailure(coreIndexingExecutionContext);
          }
        }
        this.ResultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully completed CollectionFinalizeOperation for {0}. ", (object) coreIndexingExecutionContext.IndexingUnit)));
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        operationResult.Message = this.ResultMessage.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(this.m_tracepoint, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
      return operationResult;
    }

    internal void QueueDeleteOperationForShadowIndexingUnits(
      IndexingExecutionContext indexingExecutionContext)
    {
      IIndexingUnitDataAccess indexingUnitDataAccess = indexingExecutionContext.IndexingUnitDataAccess;
      IndexMetadataStateAnalyser metadataStateAnalyser = new IndexMetadataStateAnalyserFactory().GetIndexMetadataStateAnalyser(this.DataAccessFactory, this.IndexingUnitChangeEventHandler, this.IndexingUnit.EntityType);
      foreach (string indexingUnitType in this.FinalizeHelper.GetIndexingUnitsTypeSupportingShadowIndexing(true))
      {
        if (!indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/BypassQueueDeletesForShadow"))
        {
          List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits = indexingUnitDataAccess.GetIndexingUnits(indexingExecutionContext.RequestContext, indexingUnitType, true, indexingExecutionContext.IndexingUnit.EntityType, -1);
          if (indexingUnits != null)
          {
            foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit entityIndexingUnit in indexingUnits)
            {
              if (!indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/BypassQueueDeletesForShadow"))
                metadataStateAnalyser.CreateEntityDeleteOperationForFinalize((Microsoft.VisualStudio.Services.Search.Common.ExecutionContext) indexingExecutionContext, entityIndexingUnit);
              else
                break;
            }
          }
        }
        else
          break;
      }
      metadataStateAnalyser.ProcessEventForEntity((Microsoft.VisualStudio.Services.Search.Common.ExecutionContext) indexingExecutionContext, this.IndexingUnit.EntityType);
    }

    internal override bool ShouldSkipOperation(
      IVssRequestContext requestContext,
      out string reasonToSkipOperation)
    {
      reasonToSkipOperation = string.Empty;
      return false;
    }

    internal virtual IEnumerable<IndexingUnitChangeEventPrerequisitesFilter> GetPreRequisitesToRequeueFinalize(
      IndexingExecutionContext iexContext)
    {
      return Enumerable.Empty<IndexingUnitChangeEventPrerequisitesFilter>();
    }

    internal virtual void UpdateReindexingStatusAndTurnOnIndexingFeatureFlags(
      IVssRequestContext requestContext,
      IEntityType entityType,
      Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus reindexingStatus)
    {
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      IReindexingStatusDataAccess statusDataAccess = this.DataAccessFactory.GetReindexingStatusDataAccess();
      ReindexingStatusEntry reindexingStatusEntry = statusDataAccess.GetReindexingStatusEntry(requestContext1, requestContext.ServiceHost.InstanceId, entityType);
      if (reindexingStatusEntry != null && reindexingStatusEntry.Status == Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.InProgress || reindexingStatusEntry != null && reindexingStatusEntry.Status == Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.Failed)
      {
        reindexingStatusEntry.Status = reindexingStatus;
        statusDataAccess.AddOrUpdateReindexingStatusEntry(requestContext1, reindexingStatusEntry);
      }
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.ProjectCollection).Elevate();
      this.SetFeatureFlagForCrudOperation(vssRequestContext.GetService<ITeamFoundationFeatureAvailabilityService>(), vssRequestContext);
      this.ResultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully updated Reindexing status to {0}. ", (object) reindexingStatus)));
    }

    internal virtual bool IsReindexingFailedInProgress(IndexingExecutionContext executionContext) => executionContext.IsReindexingFailedOrInProgress(this.DataAccessFactory, executionContext.IndexingUnit.EntityType);

    internal virtual bool IsReindexingSuccessful(IndexingExecutionContext executionContext) => this.GetReindexingValidator(executionContext).ValidateReindexingCompleteness(this.ResultMessage);

    internal virtual void FinalizeAndNotifyQueryProperties(IndexingExecutionContext executionContext)
    {
      this.FinalizeHelper.FinalizeQueryProperties(executionContext);
      this.FinalizeHelper.UpdateFeatureFlagsIfNeeded(executionContext);
      this.FinalizeHelper.NotifyIndexPropertiesUpdates(executionContext);
    }

    internal virtual void ErasePreReindexingState(IndexingExecutionContext executionContext)
    {
      executionContext.IndexingUnit.Properties.ErasePreReindexingState();
      executionContext.IndexingUnitDataAccess.UpdateIndexingUnit(executionContext.RequestContext, executionContext.IndexingUnit);
      this.ResultMessage.Append("Updated collection indexing unit and erased indexing state pre-reindexing.");
    }

    protected virtual ReindexingValidatorBase GetReindexingValidator(
      IndexingExecutionContext executionContext)
    {
      return new ReindexingValidatorBase(executionContext);
    }

    internal virtual void SetFeatureFlagForCrudOperation(
      ITeamFoundationFeatureAvailabilityService availabilityService,
      IVssRequestContext elevatedContext)
    {
      availabilityService.SetFeatureState(elevatedContext, this.CrudOperationsFeatureName, FeatureAvailabilityState.On);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_tracepoint, "Indexing Pipeline", "IndexingOperation", FormattableString.Invariant(FormattableStringFactory.Create("FeatureFlag:{0} Changed state to: {1}", (object) this.CrudOperationsFeatureName, (object) FeatureAvailabilityState.On)));
    }

    internal virtual List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> GetPartiallySucceededIndexingUnitsAfterProcessingFailedFiles(
      IndexingExecutionContext executionContext)
    {
      return (List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) null;
    }

    internal virtual void UpdatePrimaryIndexingUnits(
      IndexingExecutionContext indexingExecutionContext,
      bool isReindexingSuccessful = true)
    {
    }

    internal void ReQueueCollectionFinalizeOperation(
      IndexingExecutionContext indexingExecutionContext,
      IndexingUnitChangeEventPrerequisites indexPublishPreReq)
    {
      ChangeEventData dataForCollection = this.GetChangeDataForCollection(indexingExecutionContext);
      dataForCollection.Trigger = this.IndexingUnitChangeEvent.ChangeData.Trigger;
      dataForCollection.RetryAttemptCount = this.IndexingUnitChangeEvent.ChangeData.RetryAttemptCount + 1;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = this.IndexingUnit.IndexingUnitId,
        ChangeType = "CompleteBulkIndex",
        ChangeData = dataForCollection,
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0,
        Prerequisites = indexPublishPreReq
      };
      this.IndexingUnitChangeEventHandler.HandleEvent((Microsoft.VisualStudio.Services.Search.Common.ExecutionContext) indexingExecutionContext, indexingUnitChangeEvent);
    }

    internal virtual ChangeEventData GetChangeDataForCollection(
      IndexingExecutionContext indexingExecutionContext)
    {
      throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Shadow indexing is not supported for entiytype {0}", (object) indexingExecutionContext.CollectionIndexingUnit.EntityType)));
    }

    internal virtual IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> CreatePatchOperationsForPartiallySucceededIndexingUnits(
      IndexingExecutionContext iexContext,
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> listOfIndexingUnitsPartiallySucceeded,
      StringBuilder resultMessage)
    {
      return (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) null;
    }

    internal virtual void UpdateItemLevelFailureTableBasedOnReIndexingStatus(
      IndexingExecutionContext iexContext,
      OperationStatus status)
    {
      throw new NotImplementedException();
    }

    protected int GetMaximumFailedItemsCountForSucccessfullReIndexingPerIndexingUnit(
      IndexingExecutionContext indexingExecutionContext)
    {
      return indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/MaximumFailedItemsCountForSucccessfullReIndexingPerIndexingUnit", true, 100);
    }

    private void PublishReIndexTelemetryForCompletion(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      IDictionary<string, object> properties = (IDictionary<string, object>) new FriendlyDictionary<string, object>();
      properties.Add("ReindexingPassed", (object) 1);
      properties.Add("EntityType", (object) coreIndexingExecutionContext.IndexingUnit.EntityType.Name);
      properties.Add("CollectionId", (object) coreIndexingExecutionContext.CollectionIndexingUnit.TFSEntityId.ToString());
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("Indexing Pipeline", "IndexingOperation", properties);
    }

    private void PublishReIndexTelemetryForFailure(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      IDictionary<string, object> properties = (IDictionary<string, object>) new FriendlyDictionary<string, object>();
      properties.Add("ReindexingFailed", (object) 1);
      properties.Add("EntityType", (object) coreIndexingExecutionContext.IndexingUnit.EntityType.Name);
      properties.Add("CollectionId", (object) coreIndexingExecutionContext.CollectionIndexingUnit.TFSEntityId.ToString());
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("Indexing Pipeline", "IndexingOperation", properties);
    }

    protected internal virtual IEnumerable<IndexInfo> GetOldSharedIndices(
      IndexingExecutionContext executionContext)
    {
      if (this.m_oldSharedIndices == null)
      {
        this.m_oldSharedIndices = (IList<IndexInfo>) new List<IndexInfo>();
        if (this.IndexingUnit.Properties.QueryIndicesPreReindexing != null && this.IndexingUnit.Properties.QueryIndicesPreReindexing.Any<IndexInfo>())
          this.m_oldSharedIndices = (IList<IndexInfo>) this.IndexingUnit.Properties.QueryIndicesPreReindexing;
        IEnumerable<IndexInfo> dedicatedIndices = this.GetOldDedicatedIndices(executionContext);
        if (dedicatedIndices != null)
        {
          IEnumerable<string> oldDedicatedIndicesNames = dedicatedIndices.Select<IndexInfo, string>((Func<IndexInfo, string>) (d => d.IndexName));
          this.m_oldSharedIndices = (IList<IndexInfo>) this.m_oldSharedIndices.Where<IndexInfo>((Func<IndexInfo, bool>) (o => !oldDedicatedIndicesNames.Contains<string>(o.IndexName))).ToList<IndexInfo>();
        }
        IEnumerable<IndexInfo> currentIndices = this.GetCurrentIndices();
        if (currentIndices != null)
        {
          IEnumerable<string> currentIndicesNames = currentIndices.Select<IndexInfo, string>((Func<IndexInfo, string>) (d => d.IndexName));
          this.m_oldSharedIndices = (IList<IndexInfo>) this.m_oldSharedIndices.Where<IndexInfo>((Func<IndexInfo, bool>) (o => !currentIndicesNames.Contains<string>(o.IndexName))).ToList<IndexInfo>();
        }
      }
      return (IEnumerable<IndexInfo>) this.m_oldSharedIndices;
    }

    protected internal virtual IEnumerable<IndexInfo> GetOldDedicatedIndices(
      IndexingExecutionContext executionContext)
    {
      if (this.m_oldDedicatedIndices == null)
        this.m_oldDedicatedIndices = (IList<IndexInfo>) new List<IndexInfo>();
      return (IEnumerable<IndexInfo>) this.m_oldDedicatedIndices;
    }

    internal IEnumerable<IndexInfo> GetCurrentIndices()
    {
      if (this.m_currentIndices == null)
      {
        this.m_currentIndices = (IList<IndexInfo>) new List<IndexInfo>();
        if (this.IndexingUnit.Properties.QueryIndices != null && this.IndexingUnit.Properties.QueryIndices.Any<IndexInfo>())
          this.m_currentIndices = (IList<IndexInfo>) this.IndexingUnit.Properties.QueryIndices;
      }
      return (IEnumerable<IndexInfo>) this.m_currentIndices;
    }

    internal virtual void CleanupOldData(IndexingExecutionContext executionContext)
    {
      this.FinalizeHelper.DeleteDataFromOldIndices(executionContext, this.GetOldSharedIndices(executionContext));
      IEnumerable<IndexInfo> dedicatedIndices = this.GetOldDedicatedIndices(executionContext);
      if (dedicatedIndices == null)
        return;
      foreach (IndexInfo indexInfo in dedicatedIndices)
      {
        ISearchIndex index = executionContext.ProvisioningContext.SearchPlatform.GetIndex(IndexIdentity.CreateIndexIdentity(indexInfo.IndexName));
        this.DeleteOldIndex(executionContext, index);
      }
    }

    internal void DeleteOldIndex(IndexingExecutionContext executionContext, ISearchIndex oldIndex)
    {
      IndexOperationsResponse operationsResponse = executionContext.ProvisioningContext.SearchPlatform.DeleteIndex((Microsoft.VisualStudio.Services.Search.Common.ExecutionContext) executionContext, oldIndex.IndexIdentity);
      Guid collectionId = executionContext.RequestContext.GetCollectionID();
      if (operationsResponse.Success)
        executionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Deleting index {0} of collection {1} completed successfully. ", (object) oldIndex.IndexIdentity.Name, (object) collectionId)));
      else
        executionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Deleting index {0} of collection {1} failed", (object) oldIndex.IndexIdentity.Name, (object) collectionId)));
    }
  }
}
