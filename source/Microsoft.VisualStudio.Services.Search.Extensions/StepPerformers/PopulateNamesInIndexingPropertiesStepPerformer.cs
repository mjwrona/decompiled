// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.StepPerformers.PopulateNamesInIndexingPropertiesStepPerformer
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
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Search.Extensions.StepPerformers
{
  [StepPerformer("PopulateNamesInIndexingProperties")]
  public class PopulateNamesInIndexingPropertiesStepPerformer : TeamFoundationStepPerformerBase
  {
    [ServicingStep]
    public void PopulateNamesInIndexingProperties(
      IVssRequestContext requestContext,
      ServicingContext servicingContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      try
      {
        ITracerCICorrelationDetails correlationDetails = TracerCICorrelationDetailsFactory.GetCICorrelationDetails(requestContext, MethodBase.GetCurrentMethod().Name, 39);
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment && !requestContext.IsServicingContext)
        {
          using (IVssRequestContext requestContext1 = requestContext.GetService<TeamFoundationHostManagementService>().BeginRequest(requestContext, requestContext.ServiceHost.InstanceId, RequestContextType.ServicingContext, true, true))
            this.CreateUpdateMetadataEvents(requestContext1, correlationDetails, servicingContext);
        }
        else
          this.CreateUpdateMetadataEvents(requestContext, correlationDetails, servicingContext);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    private void CreateUpdateMetadataEvents(
      IVssRequestContext requestContext,
      ITracerCICorrelationDetails tracerCICorrelationDetails,
      ServicingContext servicingContext)
    {
      ExecutionContext executionContext = requestContext.GetExecutionContext(tracerCICorrelationDetails);
      Guid collectionId = requestContext.GetCollectionID();
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment && !service.GetValue<bool>(requestContext, (RegistryQuery) "/Service/ALMSearch/Settings/IsCollectionIndexed", false, false))
        servicingContext.LogInfo("UpdateIndexingUnitProperties operations not queued for {0} IUs in collection Id :'{1}' since extension is not installed.", new object[2]
        {
          (object) CodeEntityType.GetInstance(),
          (object) collectionId
        });
      else
        this.CreateUpdateMetadataEventsForAnEntityType(executionContext, servicingContext, collectionId, (IEntityType) CodeEntityType.GetInstance());
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment && !service.GetValue<bool>(requestContext, (RegistryQuery) "/Service/ALMSearch/Settings/IsCollectionIndexedForWorkItem", false, false))
        servicingContext.LogInfo("UpdateIndexingUnitProperties operations not queued for {0} IUs in collection Id :'{1}' since extension is not installed.", new object[2]
        {
          (object) "WorkItem",
          (object) collectionId
        });
      else
        this.CreateUpdateMetadataEventsForAnEntityType(executionContext, servicingContext, collectionId, (IEntityType) WorkItemEntityType.GetInstance());
    }

    private void CreateUpdateMetadataEventsForAnEntityType(
      ExecutionContext executionContext,
      ServicingContext servicingContext,
      Guid collectionId,
      IEntityType entityType)
    {
      IIndexingUnitDataAccess indexingUnitDataAccess = (IIndexingUnitDataAccess) new IndexingUnitDataAccess();
      IIndexingUnitChangeEventHandler changeEventHandler = (IIndexingUnitChangeEventHandler) new IndexingUnitChangeEventHandler();
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit1 = indexingUnitDataAccess.GetIndexingUnit(executionContext.RequestContext, collectionId, "Collection", entityType);
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitList = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      if (indexingUnit1 == null)
        return;
      indexingUnitList.Add(indexingUnit1);
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits1 = indexingUnitDataAccess.GetIndexingUnits(executionContext.RequestContext, "Project", entityType, -1);
      if (indexingUnits1 != null && indexingUnits1.Count > 0)
        indexingUnitList.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) indexingUnits1);
      if (entityType.Name == "Code")
      {
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits2 = indexingUnitDataAccess.GetIndexingUnits(executionContext.RequestContext, "Git_Repository", (IEntityType) CodeEntityType.GetInstance(), -1);
        if (indexingUnits2 != null && indexingUnits2.Count > 0)
          indexingUnitList.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) indexingUnits2);
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits3 = indexingUnitDataAccess.GetIndexingUnits(executionContext.RequestContext, "TFVC_Repository", (IEntityType) CodeEntityType.GetInstance(), -1);
        if (indexingUnits3 != null && indexingUnits3.Count > 0)
          indexingUnitList.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) indexingUnits3);
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits4 = indexingUnitDataAccess.GetIndexingUnits(executionContext.RequestContext, "CustomRepository", (IEntityType) CodeEntityType.GetInstance(), -1);
        if (indexingUnits4 != null && indexingUnits4.Count > 0)
          indexingUnitList.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) indexingUnits4);
      }
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit2 in indexingUnitList)
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
        {
          IndexingUnitId = indexingUnit2.IndexingUnitId,
          ChangeType = "UpdateIndexingUnitProperties",
          ChangeData = new ChangeEventData(executionContext),
          State = IndexingUnitChangeEventState.Pending,
          AttemptCount = 0
        };
        indexingUnitChangeEventList.Add(indexingUnitChangeEvent);
      }
      changeEventHandler.HandleEvents(executionContext, (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) indexingUnitChangeEventList);
      servicingContext.LogInfo("Successfully queueud '{0}' UpdateIndexingUnitProperties operations for all the indexing units of collection Id :'{1}' and entity type : '{2}' to update names in indexing properties", new object[3]
      {
        (object) indexingUnitChangeEventList.Count,
        (object) collectionId,
        (object) entityType
      });
    }
  }
}
