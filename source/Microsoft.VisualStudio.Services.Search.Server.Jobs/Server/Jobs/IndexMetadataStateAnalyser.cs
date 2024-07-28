// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.IndexMetadataStateAnalyser
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  internal abstract class IndexMetadataStateAnalyser
  {
    protected readonly IDataAccessFactory DataAccessFactory;
    protected readonly IIndexingUnitDataAccess IndexingUnitDataAccess;
    protected readonly IIndexingUnitChangeEventHandler IndexingUnitChangeEventHandler;
    protected readonly IIndexingUnitChangeEventDataAccess IndexingUnitChangeEventDataAccess;

    public IndexMetadataStateAnalyser()
      : this(Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance(), (IIndexingUnitChangeEventHandler) new Microsoft.VisualStudio.Services.Search.Server.EventHandler.IndexingUnitChangeEventHandler())
    {
    }

    public IndexMetadataStateAnalyser(
      IDataAccessFactory dataAccessFactory,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler)
    {
      this.DataAccessFactory = dataAccessFactory;
      this.IndexingUnitDataAccess = this.DataAccessFactory.GetIndexingUnitDataAccess();
      this.IndexingUnitChangeEventDataAccess = this.DataAccessFactory.GetIndexingUnitChangeEventDataAccess();
      this.IndexingUnitChangeEventHandler = indexingUnitChangeEventHandler;
    }

    protected abstract EntityFinalizerBase FinalizeHelper { get; }

    protected abstract TraceMetaData TraceMetadata { get; }

    public virtual int CreateEntityRenameOperationIfRequired(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit entityIndexingUnit,
      TeamProjectReference tfsProject,
      Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, TeamProjectReference, bool, bool> hasEntityModified,
      bool checkIndexingPropertiesName = false)
    {
      if (entityIndexingUnit == null)
        throw new ArgumentNullException(nameof (entityIndexingUnit));
      if (tfsProject == null)
        throw new ArgumentException("TFS entity cannot be null.", nameof (tfsProject));
      if (!hasEntityModified(entityIndexingUnit, tfsProject, checkIndexingPropertiesName))
      {
        Tracer.TraceInfo(this.TraceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Entity field values in Indexing Unit [{0}] is in sync with TFS. ", (object) entityIndexingUnit.ToString())) + "Not creating a rename event.");
        return 0;
      }
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = entityIndexingUnit.IndexingUnitId,
        ChangeType = "BeginEntityRename",
        ChangeData = (ChangeEventData) new EntityRenameEventData(executionContext),
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      Tracer.TraceInfo(this.TraceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Created IUCE [{0}].", (object) this.IndexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeeded(executionContext, indexingUnitChangeEvent).ToString())));
      return 1;
    }

    public virtual int CreateEntityDeleteOperationIfRequired(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit entityIndexingUnit)
    {
      int numDeleteEventsCreated = 0;
      this.CreateEntityDeleteOperationsIfRequiredInternal(executionContext, entityIndexingUnit, ref numDeleteEventsCreated);
      return numDeleteEventsCreated;
    }

    public virtual int CreateEntityDeleteOperationForFinalize(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit entityIndexingUnit)
    {
      int numDeleteEventsCreated = 0;
      this.CreateEntityDeleteOperationsIfRequiredForFinalizeInternal(executionContext, entityIndexingUnit, ref numDeleteEventsCreated);
      return numDeleteEventsCreated;
    }

    public virtual void ProcessEventForEntity(
      ExecutionContext executionContext,
      IEntityType entityType)
    {
      this.IndexingUnitChangeEventHandler.HandleEventForEntity(executionContext, entityType);
    }

    public virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent CreateEntityDeleteOperationsIfRequired(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit entityIndexingUnit,
      bool allowPreReqFailures = false)
    {
      int numDeleteEventsCreated = 0;
      return this.CreateEntityDeleteOperationsIfRequiredInternal(executionContext, entityIndexingUnit, ref numDeleteEventsCreated, allowPreReqFailures);
    }

    public virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent CreateRoutingAssignmentOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = indexingUnit.IndexingUnitId,
        ChangeData = new ChangeEventData(executionContext),
        ChangeType = "AssignRouting",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent assignmentOperation = this.IndexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeeded(executionContext, indexingUnitChangeEvent);
      Tracer.TraceInfo(this.TraceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Added {0} event to assign routing to {1}.", (object) assignmentOperation.Id, (object) indexingUnit)));
      return assignmentOperation;
    }

    internal virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent CreateBulkIndexEventForRepository(
      IndexingExecutionContext indexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit repoIndexingUnit)
    {
      IndexingUnitChangeEventPrerequisites eventPrerequisites = (IndexingUnitChangeEventPrerequisites) null;
      if (this.FinalizeHelper.ShouldFinalizeChildIndexingUnit(indexingExecutionContext, repoIndexingUnit))
      {
        IndexingExecutionContext executionContext = new IndexingExecutionContext(indexingExecutionContext.RequestContext, collectionIndexingUnit, indexingExecutionContext.ExecutionTracerContext.TracerCICorrelationDetails);
        executionContext.FaultService = indexingExecutionContext.FaultService;
        IndexingExecutionContext indexingExecutionContext1 = executionContext;
        indexingExecutionContext1.InitializeNameAndIds(this.DataAccessFactory);
        eventPrerequisites = this.FinalizeHelper.QueueFinalizeOperationIfAllowed(indexingExecutionContext1, this.IndexingUnitChangeEventHandler);
      }
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = repoIndexingUnit.IndexingUnitId,
        ChangeData = (ChangeEventData) new CodeBulkIndexEventData((ExecutionContext) indexingExecutionContext),
        ChangeType = "BeginBulkIndex",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0,
        Prerequisites = eventPrerequisites
      };
      return this.IndexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeeded((ExecutionContext) indexingExecutionContext, indexingUnitChangeEvent);
    }

    private Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent CreateEntityDeleteOperationsIfRequiredInternal(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit entityIndexingUnit,
      ref int numDeleteEventsCreated,
      bool allowPreReqFailures = false)
    {
      if (entityIndexingUnit == null)
        throw new ArgumentNullException(nameof (entityIndexingUnit));
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent operationInternal = this.CreateEntityDeleteOperationInternal(executionContext, entityIndexingUnit, ref numDeleteEventsCreated, allowPreReqFailures);
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent requiredInternal = this.IndexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeeded(executionContext, operationInternal);
      Tracer.TraceInfo(this.TraceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Created IUCE [{0}].", (object) requiredInternal.ToString())));
      ++numDeleteEventsCreated;
      return requiredInternal;
    }

    private Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent CreateEntityDeleteOperationsIfRequiredForFinalizeInternal(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit entityIndexingUnit,
      ref int numDeleteEventsCreated,
      bool allowPreReqFailures = false)
    {
      if (entityIndexingUnit == null)
        throw new ArgumentNullException(nameof (entityIndexingUnit));
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent internalForFinalize = this.CreateEntityDeleteOperationInternalForFinalize(executionContext, entityIndexingUnit, ref numDeleteEventsCreated, allowPreReqFailures);
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent finalizeInternal = this.IndexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeededForFinalize(executionContext, internalForFinalize);
      Tracer.TraceInfo(this.TraceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Created IUCE [{0}].", (object) finalizeInternal.ToString())));
      ++numDeleteEventsCreated;
      return finalizeInternal;
    }

    private Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent CreateEntityDeleteOperationInternalForFinalize(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      ref int numDeleteEventsCreated,
      bool allowPreReqFailures = false)
    {
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> unitsWithGivenParent = this.IndexingUnitDataAccess.GetIndexingUnitsWithGivenParent(executionContext.RequestContext, indexingUnit.IndexingUnitId, -1);
      IndexingUnitChangeEventPrerequisites eventPrerequisites1 = (IndexingUnitChangeEventPrerequisites) null;
      if (unitsWithGivenParent.Count > 0)
      {
        IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList1 = (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>(unitsWithGivenParent.Count);
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit1 in unitsWithGivenParent)
          indexingUnitChangeEventList1.Add(this.CreateEntityDeleteOperationInternalForFinalize(executionContext, indexingUnit1, ref numDeleteEventsCreated, allowPreReqFailures));
        IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList2 = this.IndexingUnitChangeEventHandler.HandleEventsChildIUCEsForFinalize(executionContext, indexingUnitChangeEventList1);
        numDeleteEventsCreated += indexingUnitChangeEventList2.Count;
        Tracer.TraceInfo(this.TraceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Created [{0}] entity delete operations.", (object) indexingUnitChangeEventList2.Count)));
        eventPrerequisites1 = new IndexingUnitChangeEventPrerequisites();
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent in (IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) indexingUnitChangeEventList2)
        {
          IndexingUnitChangeEventPrerequisites eventPrerequisites2 = eventPrerequisites1;
          IndexingUnitChangeEventPrerequisitesFilter prerequisitesFilter = new IndexingUnitChangeEventPrerequisitesFilter();
          prerequisitesFilter.Id = indexingUnitChangeEvent.Id;
          prerequisitesFilter.Operator = IndexingUnitChangeEventFilterOperator.Contains;
          List<IndexingUnitChangeEventState> changeEventStateList;
          if (!allowPreReqFailures)
          {
            changeEventStateList = new List<IndexingUnitChangeEventState>()
            {
              IndexingUnitChangeEventState.Succeeded
            };
          }
          else
          {
            changeEventStateList = new List<IndexingUnitChangeEventState>();
            changeEventStateList.Add(IndexingUnitChangeEventState.Succeeded);
            changeEventStateList.Add(IndexingUnitChangeEventState.Failed);
          }
          prerequisitesFilter.PossibleStates = changeEventStateList;
          eventPrerequisites2.Add(prerequisitesFilter);
        }
      }
      return new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = indexingUnit.IndexingUnitId,
        ChangeType = "Delete",
        ChangeData = new ChangeEventData(executionContext),
        Prerequisites = eventPrerequisites1,
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
    }

    private Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent CreateEntityDeleteOperationInternal(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      ref int numDeleteEventsCreated,
      bool allowPreReqFailures = false)
    {
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> unitsWithGivenParent = this.IndexingUnitDataAccess.GetIndexingUnitsWithGivenParent(executionContext.RequestContext, indexingUnit.IndexingUnitId, -1);
      IndexingUnitChangeEventPrerequisites eventPrerequisites1 = (IndexingUnitChangeEventPrerequisites) null;
      if (unitsWithGivenParent.Count > 0)
      {
        IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList1 = (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>(unitsWithGivenParent.Count);
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit1 in unitsWithGivenParent)
          indexingUnitChangeEventList1.Add(this.CreateEntityDeleteOperationInternal(executionContext, indexingUnit1, ref numDeleteEventsCreated, allowPreReqFailures));
        IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList2 = this.IndexingUnitChangeEventHandler.HandleEvents(executionContext, indexingUnitChangeEventList1);
        numDeleteEventsCreated += indexingUnitChangeEventList2.Count;
        Tracer.TraceInfo(this.TraceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Created [{0}] entity delete operations.", (object) indexingUnitChangeEventList2.Count)));
        eventPrerequisites1 = new IndexingUnitChangeEventPrerequisites();
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent in (IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) indexingUnitChangeEventList2)
        {
          IndexingUnitChangeEventPrerequisites eventPrerequisites2 = eventPrerequisites1;
          IndexingUnitChangeEventPrerequisitesFilter prerequisitesFilter = new IndexingUnitChangeEventPrerequisitesFilter();
          prerequisitesFilter.Id = indexingUnitChangeEvent.Id;
          prerequisitesFilter.Operator = IndexingUnitChangeEventFilterOperator.Contains;
          List<IndexingUnitChangeEventState> changeEventStateList;
          if (!allowPreReqFailures)
          {
            changeEventStateList = new List<IndexingUnitChangeEventState>()
            {
              IndexingUnitChangeEventState.Succeeded
            };
          }
          else
          {
            changeEventStateList = new List<IndexingUnitChangeEventState>();
            changeEventStateList.Add(IndexingUnitChangeEventState.Succeeded);
            changeEventStateList.Add(IndexingUnitChangeEventState.Failed);
          }
          prerequisitesFilter.PossibleStates = changeEventStateList;
          eventPrerequisites2.Add(prerequisitesFilter);
        }
      }
      return new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = indexingUnit.IndexingUnitId,
        ChangeType = "Delete",
        ChangeData = new ChangeEventData(executionContext),
        Prerequisites = eventPrerequisites1,
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
    }
  }
}
