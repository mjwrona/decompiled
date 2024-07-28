// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.StepPerformers.ResetGitBranchesStepPerformer
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
using System.Linq;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Search.Extensions.StepPerformers
{
  [StepPerformer("ResetGitBranchesStepPerformer")]
  public class ResetGitBranchesStepPerformer : TeamFoundationStepPerformerBase
  {
    private static readonly Guid s_collectionIdOnWhichRepoUpdateMetadaShouldNotBeQueued = new Guid("cb55739e-4afe-46a3-970f-1b49d8ee7564");
    private static readonly Guid s_repoIdOnWhichRepoUpdateMetadaShouldNotBeQueued = new Guid("7bc5fd9f-6098-479a-a87e-1533d288d438");

    [ServicingStep]
    public void QueueResetBranchesInGitRepoAttributesOperation(
      IVssRequestContext requestContext,
      ServicingContext servicingContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      try
      {
        ITracerCICorrelationDetails correlationDetails = TracerCICorrelationDetailsFactory.GetCICorrelationDetails(requestContext, MethodBase.GetCurrentMethod().Name, 42);
        ExecutionContext executionContext = requestContext.GetExecutionContext(correlationDetails);
        IIndexingUnitDataAccess indexingUnitDataAccess = (IIndexingUnitDataAccess) new IndexingUnitDataAccess();
        IIndexingUnitChangeEventHandler changeEventHandler = (IIndexingUnitChangeEventHandler) new IndexingUnitChangeEventHandler();
        Guid collectionId = executionContext.RequestContext.GetCollectionID();
        if (indexingUnitDataAccess.GetIndexingUnit(executionContext.RequestContext, collectionId, "Collection", (IEntityType) CodeEntityType.GetInstance()) == null)
          return;
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits = indexingUnitDataAccess.GetIndexingUnits(executionContext.RequestContext, "Git_Repository", (IEntityType) CodeEntityType.GetInstance(), -1);
        if (!indexingUnits.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
          return;
        if (collectionId == ResetGitBranchesStepPerformer.s_collectionIdOnWhichRepoUpdateMetadaShouldNotBeQueued)
          indexingUnits.RemoveAll((Predicate<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (repo => repo.TFSEntityId == ResetGitBranchesStepPerformer.s_repoIdOnWhichRepoUpdateMetadaShouldNotBeQueued));
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in indexingUnits)
        {
          Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
          {
            IndexingUnitId = indexingUnit.IndexingUnitId,
            ChangeType = "ResetBranchesInGitRepoAttributes",
            ChangeData = new ChangeEventData(executionContext),
            State = IndexingUnitChangeEventState.Pending,
            AttemptCount = 0
          };
          indexingUnitChangeEventList.Add(indexingUnitChangeEvent);
        }
        changeEventHandler.HandleEvents(executionContext, (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) indexingUnitChangeEventList);
        servicingContext.LogInfo("Successfully Queueud '{0}' ResetBranchesInGitRepoAttributes Operations to remove branches from git Repos of collection Name :'{1}' Id :'{2}'", new object[3]
        {
          (object) indexingUnitChangeEventList.Count,
          (object) requestContext.GetCollectionName(),
          (object) collectionId
        });
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }
  }
}
