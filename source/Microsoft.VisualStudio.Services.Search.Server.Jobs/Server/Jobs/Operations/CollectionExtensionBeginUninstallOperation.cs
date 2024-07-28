// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.CollectionExtensionBeginUninstallOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations
{
  internal class CollectionExtensionBeginUninstallOperation : AbstractIndexingOperation
  {
    private Microsoft.VisualStudio.Services.Search.Common.IndexingUnit m_collectionIndexingUnit;
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetadata = new TraceMetaData(1080696, "Indexing Pipeline", "IndexingOperation");

    public CollectionExtensionBeginUninstallOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
      this.m_collectionIndexingUnit = indexingUnit;
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(CollectionExtensionBeginUninstallOperation.s_traceMetadata, nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder stringBuilder = new StringBuilder();
      try
      {
        IIndexingUnitDataAccess indexingUnitDataAccess = this.DataAccessFactory.GetIndexingUnitDataAccess();
        IIndexingUnitChangeEventDataAccess changeEventDataAccess = this.DataAccessFactory.GetIndexingUnitChangeEventDataAccess();
        Microsoft.VisualStudio.Services.Search.Server.EventHandler.IndexingUnitChangeEventHandler indexingUnitChangeEventHandler = new Microsoft.VisualStudio.Services.Search.Server.EventHandler.IndexingUnitChangeEventHandler(Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance());
        this.QueueDeleteOperationsForCollection((ExecutionContext) coreIndexingExecutionContext, indexingUnitDataAccess, changeEventDataAccess, indexingUnitChangeEventHandler);
        stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Collection Extension BeginUninstall Operation Completed for {0}.", (object) this.IndexingUnit)));
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        operationResult.Message = stringBuilder.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(CollectionExtensionBeginUninstallOperation.s_traceMetadata, nameof (RunOperation));
      }
      return operationResult;
    }

    public virtual void QueueDeleteOperationsForCollection(
      ExecutionContext executionContext,
      IIndexingUnitDataAccess indexingUnitDataAccess,
      IIndexingUnitChangeEventDataAccess indexingUnitChangeEventDataAccess,
      Microsoft.VisualStudio.Services.Search.Server.EventHandler.IndexingUnitChangeEventHandler indexingUnitChangeEventHandler)
    {
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> unitsWithGivenParent1 = indexingUnitDataAccess.GetIndexingUnitsWithGivenParent(executionContext.RequestContext, this.m_collectionIndexingUnit.IndexingUnitId, -1);
      IndexingUnitChangeEventPrerequisites collectionDeleteFinalizePreReq = new IndexingUnitChangeEventPrerequisites();
      if (unitsWithGivenParent1 != null && unitsWithGivenParent1.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
      {
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in unitsWithGivenParent1)
        {
          IndexingUnitChangeEventPrerequisites projectDeleteFinalizePreReq = new IndexingUnitChangeEventPrerequisites();
          List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> unitsWithGivenParent2 = indexingUnitDataAccess.GetIndexingUnitsWithGivenParent(executionContext.RequestContext, indexingUnit.IndexingUnitId, -1);
          if (unitsWithGivenParent2 != null && unitsWithGivenParent2.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
            this.QueueRepositoryIndexingUnitDeleteOperation(executionContext, unitsWithGivenParent2, projectDeleteFinalizePreReq, indexingUnitChangeEventHandler);
          this.CreateProjectDeleteFinalizeOperation(executionContext, indexingUnit.IndexingUnitId, projectDeleteFinalizePreReq, (IIndexingUnitChangeEventHandler) indexingUnitChangeEventHandler, collectionDeleteFinalizePreReq);
        }
      }
      this.CreateCollectionDeleteFinalizeOperation(executionContext, this.m_collectionIndexingUnit.IndexingUnitId, collectionDeleteFinalizePreReq, (IIndexingUnitChangeEventHandler) indexingUnitChangeEventHandler);
    }

    internal override bool ShouldSkipOperation(
      IVssRequestContext requestContext,
      out string reasonToSkipOperation)
    {
      reasonToSkipOperation = string.Empty;
      return false;
    }

    private void QueueRepositoryIndexingUnitDeleteOperation(
      ExecutionContext executionContext,
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> repoIndexingUnits,
      IndexingUnitChangeEventPrerequisites projectDeleteFinalizePreReq,
      Microsoft.VisualStudio.Services.Search.Server.EventHandler.IndexingUnitChangeEventHandler indexingUnitChangeEventHandler)
    {
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList = (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit repoIndexingUnit in repoIndexingUnits)
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
        {
          IndexingUnitId = repoIndexingUnit.IndexingUnitId,
          ChangeType = "Delete",
          ChangeData = new ChangeEventData(executionContext),
          State = IndexingUnitChangeEventState.Pending,
          AttemptCount = 0
        };
        indexingUnitChangeEventList.Add(indexingUnitChangeEvent);
      }
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> source = indexingUnitChangeEventHandler.HandleEvents(executionContext, indexingUnitChangeEventList);
      projectDeleteFinalizePreReq.AddRange((IEnumerable<IndexingUnitChangeEventPrerequisitesFilter>) source.Select<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent, IndexingUnitChangeEventPrerequisitesFilter>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent, IndexingUnitChangeEventPrerequisitesFilter>) (deleteRepoEvent => new IndexingUnitChangeEventPrerequisitesFilter()
      {
        Id = deleteRepoEvent.Id,
        Operator = IndexingUnitChangeEventFilterOperator.Contains,
        PossibleStates = new List<IndexingUnitChangeEventState>()
        {
          IndexingUnitChangeEventState.Succeeded,
          IndexingUnitChangeEventState.Failed
        }
      })).ToList<IndexingUnitChangeEventPrerequisitesFilter>());
    }

    protected void CreateProjectDeleteFinalizeOperation(
      ExecutionContext executionContext,
      int projectIndexingUnitId,
      IndexingUnitChangeEventPrerequisites projectDeleteFinalizePreReq,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler,
      IndexingUnitChangeEventPrerequisites collectionDeleteFinalizePreReq)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = projectIndexingUnitId,
        ChangeType = "Delete",
        ChangeData = new ChangeEventData(executionContext),
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0,
        Prerequisites = projectDeleteFinalizePreReq
      };
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent2 = indexingUnitChangeEventHandler.HandleEvent(executionContext, indexingUnitChangeEvent1);
      collectionDeleteFinalizePreReq.Add(new IndexingUnitChangeEventPrerequisitesFilter()
      {
        Id = indexingUnitChangeEvent2.Id,
        Operator = IndexingUnitChangeEventFilterOperator.Contains,
        PossibleStates = new List<IndexingUnitChangeEventState>()
        {
          IndexingUnitChangeEventState.Succeeded,
          IndexingUnitChangeEventState.Failed
        }
      });
    }

    protected void CreateCollectionDeleteFinalizeOperation(
      ExecutionContext executionContext,
      int collectionIndexingUnitId,
      IndexingUnitChangeEventPrerequisites collectionDeleteFinalizePreReq,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = collectionIndexingUnitId,
        ChangeType = "ExtensionFinalizeUninstall",
        ChangeData = new ChangeEventData(executionContext),
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0,
        Prerequisites = collectionDeleteFinalizePreReq
      };
      indexingUnitChangeEventHandler.HandleEvent(executionContext, indexingUnitChangeEvent);
    }
  }
}
