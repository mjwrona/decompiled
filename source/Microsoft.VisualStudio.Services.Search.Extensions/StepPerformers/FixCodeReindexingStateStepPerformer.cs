// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.StepPerformers.FixCodeReindexingStateStepPerformer
// Assembly: Microsoft.VisualStudio.Services.Search.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1D8FF195-304B-4BBA-9D1C-F4A6093CE2E1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Extensions.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using System;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Search.Extensions.StepPerformers
{
  [StepPerformer("FixCodeReindexingState")]
  public class FixCodeReindexingStateStepPerformer : TeamFoundationStepPerformerBase
  {
    [ServicingStep]
    public void FixCodeReindexingState(
      IVssRequestContext requestContext,
      ServicingContext servicingContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      try
      {
        if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          return;
        ITracerCICorrelationDetails correlationDetails = TracerCICorrelationDetailsFactory.GetCICorrelationDetails(requestContext, MethodBase.GetCurrentMethod().Name, 45);
        if (!requestContext.IsServicingContext)
        {
          using (IVssRequestContext requestContext1 = requestContext.GetService<TeamFoundationHostManagementService>().BeginRequest(requestContext, requestContext.ServiceHost.InstanceId, RequestContextType.ServicingContext, true, true))
            this.FixCodeReindexingStateAndQueueCollectionFinalize(requestContext1, correlationDetails, servicingContext);
        }
        else
          this.FixCodeReindexingStateAndQueueCollectionFinalize(requestContext, correlationDetails, servicingContext);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    private void FixCodeReindexingStateAndQueueCollectionFinalize(
      IVssRequestContext requestContext,
      ITracerCICorrelationDetails tracerCICorrelationDetails,
      ServicingContext servicingContext)
    {
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      Guid collectionId = requestContext.GetCollectionID();
      IReindexingStatusDataAccess statusDataAccess = (IReindexingStatusDataAccess) new ReindexingStatusDataAccess();
      ReindexingStatusEntry reindexingStatusEntry = statusDataAccess.GetReindexingStatusEntry(requestContext1, collectionId, (IEntityType) CodeEntityType.GetInstance());
      if (reindexingStatusEntry == null || reindexingStatusEntry.Status != Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.InProgress)
        return;
      reindexingStatusEntry.Status = Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.Completed;
      statusDataAccess.AddOrUpdateReindexingStatusEntry(requestContext1, reindexingStatusEntry);
      servicingContext.LogInfo("Successfully set the reindexing status to completed for collection Id :'{0}' to ensure code collection finalize works fine.", new object[1]
      {
        (object) collectionId
      });
      IndexingUnitDataAccess indexingUnitDataAccess = new IndexingUnitDataAccess();
      IIndexingUnitChangeEventHandler changeEventHandler = (IIndexingUnitChangeEventHandler) new IndexingUnitChangeEventHandler();
      ExecutionContext executionContext = requestContext.GetExecutionContext(tracerCICorrelationDetails);
      IVssRequestContext requestContext2 = executionContext.RequestContext;
      Guid TFSEntityId = collectionId;
      CodeEntityType instance = CodeEntityType.GetInstance();
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = indexingUnitDataAccess.GetIndexingUnit(requestContext2, TFSEntityId, "Collection", (IEntityType) instance);
      if (indexingUnit == null)
        return;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = indexingUnit.IndexingUnitId,
        ChangeType = "CompleteBulkIndex",
        ChangeData = new ChangeEventData(executionContext),
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      changeEventHandler.HandleEvent(executionContext, indexingUnitChangeEvent);
      servicingContext.LogInfo("Successfully queueud code collection finalize operation for collection Id :'{0}' to ensure we have correct routing information.", new object[1]
      {
        (object) collectionId
      });
    }
  }
}
