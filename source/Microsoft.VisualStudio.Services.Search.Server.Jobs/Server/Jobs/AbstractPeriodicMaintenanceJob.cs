// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.AbstractPeriodicMaintenanceJob
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Api;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public abstract class AbstractPeriodicMaintenanceJob : 
    ISearchServiceJobExtension,
    ITeamFoundationJobExtension
  {
    private const int TracePoint = 1080350;
    private const string TraceArea = "Indexing Pipeline";
    private const string TraceLayer = "Job";
    [StaticSafe]
    protected static readonly TraceMetaData s_traceMetaData = new TraceMetaData(1080350, "Indexing Pipeline", "Job");

    internal IDataAccessFactory DataAccessFactory { get; }

    internal ThresholdViolationIdentifier ThresholdViolationIdentifier { get; }

    internal AbstractPeriodicMaintenanceJob(
      IDataAccessFactory dataAccessFactory,
      ThresholdViolationIdentifier thresholdViolationIdentifier)
    {
      this.DataAccessFactory = dataAccessFactory;
      this.ThresholdViolationIdentifier = thresholdViolationIdentifier;
    }

    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      this.ValidateRequestContext(requestContext);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(AbstractPeriodicMaintenanceJob.s_traceMetaData, nameof (Run));
      Stopwatch stopwatch = Stopwatch.StartNew();
      ITracerCICorrelationDetails correlationDetails = TracerCICorrelationDetailsFactory.GetCICorrelationDetails(requestContext, jobDefinition.Name, 19);
      ExecutionContext executionContext = requestContext.GetExecutionContext(correlationDetails);
      StringBuilder resultMessageBuilder = new StringBuilder();
      try
      {
        this.CleanUpOperationStates(executionContext, resultMessageBuilder);
        this.MarkCompletedIndexingUnitsForDelete(executionContext);
        resultMessageBuilder.Append("Successfully Archived IndexingUnitChangeEvent table for completed events.");
        this.LogAnyThresholdViolations(executionContext);
        this.DeleteMarkedIndexingUnits(executionContext, resultMessageBuilder);
        this.PostRun(requestContext, resultMessageBuilder);
        return TeamFoundationJobExecutionResult.Succeeded;
      }
      catch (Exception ex)
      {
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Periodic Maintenance Job failed with exception {0}", (object) ex);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(AbstractPeriodicMaintenanceJob.s_traceMetaData, str);
        TeamFoundationEventLog.Default.Log(str, SearchEventId.PeriodicCatchUpJobFailed, EventLogEntryType.Error);
        resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, str);
        return TeamFoundationJobExecutionResult.Failed;
      }
      finally
      {
        resultMessage = resultMessageBuilder.ToString();
        stopwatch.Stop();
        string name = jobDefinition.Name;
        executionContext.ExecutionTracerContext.PublishKpi(name, "Indexing Pipeline", (double) stopwatch.ElapsedMilliseconds);
        executionContext.ExecutionTracerContext.PublishCi("Indexing Pipeline", "Job", "OperationStatus", "Succeeded");
        this.ReQueueJobIfNeeded(executionContext);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(AbstractPeriodicMaintenanceJob.s_traceMetaData, nameof (Run));
      }
    }

    public abstract void PostRun(
      IVssRequestContext requestContext,
      StringBuilder resultMessageBuilder);

    public abstract void ValidateRequestContext(IVssRequestContext requestContext);

    public abstract void ReQueueJobIfNeeded(ExecutionContext executionContext);

    internal virtual void DeleteMarkedIndexingUnits(
      ExecutionContext executionContext,
      StringBuilder resultMessageBuilder)
    {
      try
      {
        IIndexingUnitDataAccess indexingUnitDataAccess = this.DataAccessFactory.GetIndexingUnitDataAccess();
        this.DataAccessFactory.GetIndexingUnitChangeEventDataAccess().DeleteIndexingUnitChangeEventsOfDeletedUnits(executionContext.RequestContext);
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> deletedIndexingUnits = indexingUnitDataAccess.GetDeletedIndexingUnits(executionContext.RequestContext, -1);
        IEnumerable<Guid> guids = deletedIndexingUnits.Select<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid?>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid?>) (obj => obj.AssociatedJobId)).Where<Guid?>((Func<Guid?, bool>) (g => g.HasValue)).Select<Guid?, Guid>((Func<Guid?, Guid>) (x => x.Value));
        if (guids.Any<Guid>())
        {
          executionContext.RequestContext.GetService<ITeamFoundationJobService>().UpdateJobDefinitions(executionContext.RequestContext, guids, (IEnumerable<TeamFoundationJobDefinition>) null);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(AbstractPeriodicMaintenanceJob.s_traceMetaData, string.Format((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Deleted ({0}) Jobs associated with deleted Indexing Units.", (object) string.Join<Guid>(",", guids)))));
        }
        if (deletedIndexingUnits.Count > 0)
        {
          IEnumerable<int> values = deletedIndexingUnits.Select<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int>) (x => x.IndexingUnitId));
          indexingUnitDataAccess.DeleteIndexingUnitsPermanently(executionContext.RequestContext, deletedIndexingUnits);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(AbstractPeriodicMaintenanceJob.s_traceMetaData, string.Format((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Deleted IndexingUnits with IDs ({0}) from IndexingUnit table.", (object) string.Join<int>(",", values)))));
        }
        resultMessageBuilder.Append("Successfully deleted (if any) marked Indexing Units and the associated jobs.");
      }
      catch (Exception ex)
      {
        resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Failed to delete marked IndexingUnits. ");
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(AbstractPeriodicMaintenanceJob.s_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Delete marked Indexing Units failed with exception: {0}", (object) ex)));
      }
    }

    internal virtual void NotifyIndexingUnitChangeEventProcessor(ExecutionContext executionContext) => executionContext.RequestContext.GetService<IIndexingUnitChangeEventProcessor>().Process(executionContext);

    private void CleanUpOperationStates(
      ExecutionContext executionContext,
      StringBuilder resultMessageBuilder)
    {
      int num = this.CleanUpOperationStates(executionContext);
      resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Successfully cleaned {0} Operation states. ", (object) num)));
      this.NotifyIndexingUnitChangeEventProcessor(executionContext);
      resultMessageBuilder.Append("Successfully notified IndexingUnitChangeEventProcessor to process next set of events. ");
    }

    internal virtual int CleanUpOperationStates(ExecutionContext executionContext)
    {
      IIndexingUnitChangeEventDataAccess changeEventDataAccess = this.DataAccessFactory.GetIndexingUnitChangeEventDataAccess();
      IEnumerable<IndexingUnitChangeEventDetails> unitChangeEvents1 = changeEventDataAccess.GetIndexingUnitChangeEvents(executionContext.RequestContext, IndexingUnitChangeEventState.Queued, int.MaxValue);
      IEnumerable<IndexingUnitChangeEventDetails> unitChangeEvents2 = changeEventDataAccess.GetIndexingUnitChangeEvents(executionContext.RequestContext, IndexingUnitChangeEventState.InProgress, int.MaxValue);
      List<IndexingUnitChangeEventDetails> source1 = new List<IndexingUnitChangeEventDetails>();
      source1.AddRange(unitChangeEvents1 ?? (IEnumerable<IndexingUnitChangeEventDetails>) new List<IndexingUnitChangeEventDetails>());
      source1.AddRange(unitChangeEvents2 ?? (IEnumerable<IndexingUnitChangeEventDetails>) new List<IndexingUnitChangeEventDetails>());
      if (source1.Any<IndexingUnitChangeEventDetails>())
      {
        List<Guid> list = source1.Select<IndexingUnitChangeEventDetails, Guid>((Func<IndexingUnitChangeEventDetails, Guid>) (x => x.AssociatedJobId.Value)).ToList<Guid>();
        IEnumerable<TeamFoundationJobQueueEntry> source2 = executionContext.RequestContext.QueryJobQueue((IEnumerable<Guid>) list);
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
        foreach (IndexingUnitChangeEventDetails changeEventDetails in source1)
        {
          IndexingUnitChangeEventDetails changeEvent = changeEventDetails;
          if ((source2 == null ? 0 : (source2.Any<TeamFoundationJobQueueEntry>((Func<TeamFoundationJobQueueEntry, bool>) (jobQueueEntry => jobQueueEntry != null && jobQueueEntry.JobId == changeEvent.AssociatedJobId.Value)) ? 1 : 0)) == 0)
            indexingUnitChangeEventList.Add(changeEvent.IndexingUnitChangeEvent);
        }
        if (indexingUnitChangeEventList.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>())
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(AbstractPeriodicMaintenanceJob.s_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Found {0} operations for which the corresponding jobs were not running. Marking these operations as Pending again.", (object) indexingUnitChangeEventList.Count)));
          indexingUnitChangeEventList.ForEach((Action<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) (x => x.State = IndexingUnitChangeEventState.Pending));
          changeEventDataAccess.UpdateIndexingUnitChangeEvents(executionContext.RequestContext, (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) indexingUnitChangeEventList);
          return indexingUnitChangeEventList.Count;
        }
      }
      return 0;
    }

    internal virtual void MarkCompletedIndexingUnitsForDelete(ExecutionContext executionContext)
    {
      IIndexingUnitChangeEventDataAccess changeEventDataAccess = this.DataAccessFactory.GetIndexingUnitChangeEventDataAccess();
      IIndexingUnitChangeEventArchiveDataAccess eventArchiveAccess = this.DataAccessFactory.GetIndexingUnitChangeEventArchiveAccess();
      IVssRequestContext requestContext = executionContext.RequestContext;
      int olderEventsInHours = executionContext.ServiceSettings.JobSettings.PeriodicCleanUpOfOlderEventsInHours;
      changeEventDataAccess.ArchiveIndexingUnitChangeEvent(requestContext, olderEventsInHours);
      eventArchiveAccess.DeleteOldChangeEventsFromArchive(executionContext.RequestContext);
    }

    internal virtual IndexingExecutionContext GetIndexingExecutionContext(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit)
    {
      IndexingExecutionContext executionContext1 = new IndexingExecutionContext(executionContext.RequestContext, collectionIndexingUnit, executionContext.ExecutionTracerContext.TracerCICorrelationDetails);
      executionContext1.FaultService = executionContext.FaultService;
      executionContext1.InitializeNameAndIds(this.DataAccessFactory);
      return executionContext1;
    }

    internal virtual void LogAnyThresholdViolations(ExecutionContext executionContext)
    {
      IIndexingUnitChangeEventDataAccess changeEventDataAccess = this.DataAccessFactory.GetIndexingUnitChangeEventDataAccess();
      this.ThresholdViolationIdentifier.IdentifyIndexingStuckForCollection(executionContext, changeEventDataAccess);
      this.ThresholdViolationIdentifier.IdentifyChangeEventWaitTimeThresholdViolation(executionContext, changeEventDataAccess);
      this.ThresholdViolationIdentifier.IdentifyLongWaitingChangeEvents(executionContext, changeEventDataAccess);
      this.ThresholdViolationIdentifier.IdentifyItemLevelFailureViolationsForAllEntities(executionContext, this.DataAccessFactory.GetItemLevelFailureDataAccess());
    }
  }
}
