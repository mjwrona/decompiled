// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.AbstractSearchServiceIndexingJob
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Constants;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Api;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.V2Jobs.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public abstract class AbstractSearchServiceIndexingJob : 
    ISearchServiceJobExtension,
    ITeamFoundationJobExtension,
    IRunnable<IDictionary<long, OperationResult>>
  {
    protected static readonly string DateTimeFormat = "dd/MM/yyyy HH:mm:ss.fff";
    protected static readonly string TraceArea = "Indexing Pipeline";
    protected static readonly string TraceLayer = "Job";
    private JobQueueController m_jobQueueController;

    protected AbstractSearchServiceIndexingJob() => this.DataAccessFactory = Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance();

    internal AbstractSearchServiceIndexingJob(IDataAccessFactory dataAccessFactory) => this.DataAccessFactory = dataAccessFactory;

    internal virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnit IndexingUnit { get; set; }

    internal IDataAccessFactory DataAccessFactory { get; set; }

    internal CoreIndexingExecutionContext IndexingExecutionContext { get; set; }

    internal IOperationMapper OperationMapper { get; set; }

    internal IEnumerable<IndexingUnitChangeEventDetails> ChangeEventsToBeProcessed { get; set; }

    public virtual TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(this.TracePoint, AbstractSearchServiceIndexingJob.TraceArea, AbstractSearchServiceIndexingJob.TraceLayer, nameof (Run));
      Stopwatch stopwatch = Stopwatch.StartNew();
      StringBuilder resultMessage1 = new StringBuilder();
      if (!this.Initialize(requestContext, jobDefinition, resultMessage1))
      {
        this.PublishJobFailureKPI(requestContext, resultMessage1);
        resultMessage = resultMessage1.ToString();
        this.PublishOnPremTelemetryAndClearContext();
        return TeamFoundationJobExecutionResult.Failed;
      }
      TeamFoundationJobExecutionResult jobExecutionResult = TeamFoundationJobExecutionResult.Succeeded;
      IDictionary<long, OperationResult> operationResults = (IDictionary<long, OperationResult>) new Dictionary<long, OperationResult>();
      try
      {
        if (this.CheckFaultStateForRequeue(resultMessage1))
          return TeamFoundationJobExecutionResult.Succeeded;
        this.PreRun();
        this.ChangeEventsToBeProcessed = this.GetEventsToBeProcessed();
        operationResults = this.Run((ExecutionContext) this.IndexingExecutionContext);
        resultMessage1.Append(this.GetExecutionSummary(operationResults));
        jobExecutionResult = this.GetJobResultFromOperationResult((IEnumerable<OperationResult>) operationResults.Values);
        this.IndexingExecutionContext.ExecutionTracerContext.PublishCi(AbstractSearchServiceIndexingJob.TraceArea, AbstractSearchServiceIndexingJob.TraceLayer, "OperationStatus", jobExecutionResult.ToString());
        return jobExecutionResult;
      }
      catch (Exception ex)
      {
        resultMessage1.Append((object) ex);
        jobExecutionResult = TeamFoundationJobExecutionResult.Failed;
        return jobExecutionResult;
      }
      finally
      {
        this.PostRun();
        this.IndexingExecutionContext.ExecutionTracerContext.PublishClientTrace(AbstractSearchServiceIndexingJob.TraceArea, AbstractSearchServiceIndexingJob.TraceLayer, this.GetType().FullName, (object) DateTime.UtcNow.ToString(AbstractSearchServiceIndexingJob.DateTimeFormat, (IFormatProvider) CultureInfo.InvariantCulture));
        this.IndexingExecutionContext.ExecutionTracerContext.PublishKpi(this.GetType().Name, AbstractSearchServiceIndexingJob.TraceArea, (double) stopwatch.ElapsedMilliseconds);
        if (jobExecutionResult == TeamFoundationJobExecutionResult.Failed)
          this.PublishJobFailureKPI(this.IndexingExecutionContext.RequestContext, resultMessage1);
        else
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("IndexingJobFailureRate", AbstractSearchServiceIndexingJob.TraceArea, 0.0);
        if (operationResults.Values.Any<OperationResult>((Func<OperationResult, bool>) (x => x.Status == OperationStatus.Failed)))
          this.QueuePeriodicCatchUpJobOnJobFailures();
        resultMessage = resultMessage1.ToString();
        this.PublishOnPremTelemetryAndClearContext();
      }
    }

    internal virtual IEnumerable<IndexingUnitChangeEventDetails> GetEventsToBeProcessed()
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(this.TracePoint, AbstractSearchServiceIndexingJob.TraceArea, AbstractSearchServiceIndexingJob.TraceLayer, nameof (GetEventsToBeProcessed));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TracePoint, AbstractSearchServiceIndexingJob.TraceArea, AbstractSearchServiceIndexingJob.TraceLayer, FormattableString.Invariant(FormattableStringFactory.Create("Retrieving Queued Change Events to be processed for Indexing Unit : {0}", (object) this.IndexingUnit)));
      IEnumerable<IndexingUnitChangeEventDetails> unitChangeEvents = this.DataAccessFactory.GetIndexingUnitChangeEventDataAccess().GetIndexingUnitChangeEvents(this.IndexingExecutionContext.RequestContext, this.IndexingUnit.IndexingUnitId, IndexingUnitChangeEventState.Queued, this.IndexingExecutionContext.EventProcessingContext.EventProcessingBatchSize);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(this.TracePoint, AbstractSearchServiceIndexingJob.TraceArea, AbstractSearchServiceIndexingJob.TraceLayer, nameof (GetEventsToBeProcessed));
      return unitChangeEvents;
    }

    public virtual IDictionary<long, OperationResult> Run(ExecutionContext executionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(this.TracePoint, AbstractSearchServiceIndexingJob.TraceArea, AbstractSearchServiceIndexingJob.TraceLayer, nameof (Run));
      IDictionary<long, OperationResult> dictionary = (IDictionary<long, OperationResult>) new Dictionary<long, OperationResult>();
      try
      {
        if (this.ChangeEventsToBeProcessed != null && this.ChangeEventsToBeProcessed.Any<IndexingUnitChangeEventDetails>())
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(this.TracePoint, AbstractSearchServiceIndexingJob.TraceArea, AbstractSearchServiceIndexingJob.TraceLayer, FormattableString.Invariant(FormattableStringFactory.Create("Found ${0} events to be processed", (object) this.ChangeEventsToBeProcessed.Count<IndexingUnitChangeEventDetails>())));
          foreach (IndexingUnitChangeEventDetails changeEventDetails in this.ChangeEventsToBeProcessed)
          {
            OperationResult operationResult = (OperationResult) null;
            try
            {
              if (changeEventDetails.IndexingUnitChangeEvent.ChangeData.CorrelationId == null)
              {
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TracePoint, AbstractSearchServiceIndexingJob.TraceArea, AbstractSearchServiceIndexingJob.TraceLayer, "CorrelationId not found. Assigning a new one.");
                changeEventDetails.IndexingUnitChangeEvent.ChangeData.CorrelationId = Guid.NewGuid().ToString();
              }
              ITracerCICorrelationDetails correlationDetails = TracerCICorrelationDetailsFactory.GetCICorrelationDetails(executionContext.RequestContext, changeEventDetails.IndexingUnitChangeEvent.ChangeData, changeEventDetails.IndexingUnitChangeEvent.ChangeType, this.IndexingUnit, this.DataAccessFactory.GetIndexingUnitDataAccess());
              CoreIndexingExecutionContext executionContext1 = executionContext.RequestContext.GetService<ICoreContextService>().CreateIndexingExecutionContext(executionContext.RequestContext, this.IndexingUnit, correlationDetails, this.GetIndexingUnitChangeEventHandler());
              executionContext1.FaultService = executionContext.FaultService;
              executionContext1.InitializeNameAndIds(this.DataAccessFactory);
              if (executionContext1.OperationFailureHandler == null)
                executionContext1.OperationFailureHandler = CorePipelinePluginsFactory.GetPipelineFailureHandler(executionContext1);
              operationResult = this.GetOperation(executionContext1, changeEventDetails.IndexingUnitChangeEvent, this.IndexingUnit).Run((ExecutionContext) executionContext1);
            }
            catch (Exception ex)
            {
              operationResult = new OperationResult()
              {
                Message = ex.ToString(),
                Status = OperationStatus.Failed
              };
            }
            finally
            {
              if (operationResult != null)
              {
                dictionary.Add(changeEventDetails.IndexingUnitChangeEvent.Id, operationResult);
                string message = FormattableString.Invariant(FormattableStringFactory.Create("{0} ResultMessage: {1}, Status: {2}", (object) changeEventDetails.IndexingUnitChangeEvent.ToString(), (object) operationResult.Message, (object) operationResult.Status));
                switch (operationResult.Status)
                {
                  case OperationStatus.Succeeded:
                    Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TracePoint, AbstractSearchServiceIndexingJob.TraceArea, AbstractSearchServiceIndexingJob.TraceLayer, message);
                    break;
                  case OperationStatus.PartiallySucceeded:
                    Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(this.TracePoint, AbstractSearchServiceIndexingJob.TraceArea, AbstractSearchServiceIndexingJob.TraceLayer, message);
                    break;
                  case OperationStatus.FailedAndRetry:
                    Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(this.TracePoint, AbstractSearchServiceIndexingJob.TraceArea, AbstractSearchServiceIndexingJob.TraceLayer, message);
                    break;
                  case OperationStatus.Failed:
                    Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TracePoint, AbstractSearchServiceIndexingJob.TraceArea, AbstractSearchServiceIndexingJob.TraceLayer, message);
                    break;
                }
              }
            }
          }
        }
        else
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(this.TracePoint, AbstractSearchServiceIndexingJob.TraceArea, AbstractSearchServiceIndexingJob.TraceLayer, FormattableString.Invariant(FormattableStringFactory.Create("No Queued Operation found for Indexing Unit : {0}", (object) this.IndexingUnit)));
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(this.TracePoint, AbstractSearchServiceIndexingJob.TraceArea, AbstractSearchServiceIndexingJob.TraceLayer, nameof (Run));
      }
      return dictionary;
    }

    internal IRunnable<OperationResult> GetOperation(
      CoreIndexingExecutionContext indexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      return this.GetOperationMapper(indexingExecutionContext).GetOperation(indexingExecutionContext, indexingUnitChangeEvent, indexingUnit);
    }

    public virtual bool Initialize(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      StringBuilder resultMessage)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(this.TracePoint, AbstractSearchServiceIndexingJob.TraceArea, AbstractSearchServiceIndexingJob.TraceLayer, nameof (Initialize));
      try
      {
        if (!this.ExtractIndexingUnit(requestContext, jobDefinition, resultMessage))
          return false;
        if (this.IndexingExecutionContext == null)
        {
          IIndexingUnitDataAccess indexingUnitDataAccess = this.DataAccessFactory.GetIndexingUnitDataAccess();
          ITracerCICorrelationDetails correlationDetails = TracerCICorrelationDetailsFactory.GetCICorrelationDetails(requestContext, jobDefinition.Name, 0, this.IndexingUnit, indexingUnitDataAccess);
          ExecutionContext executionContext = requestContext.GetExecutionContext(correlationDetails);
          this.IndexingExecutionContext = executionContext.RequestContext.GetService<ICoreContextService>().CreateIndexingExecutionContext(executionContext.RequestContext, this.IndexingUnit, correlationDetails, this.GetIndexingUnitChangeEventHandler());
          this.IndexingExecutionContext.FaultService = executionContext.FaultService;
        }
        this.IndexingExecutionContext.InitializeNameAndIds(this.DataAccessFactory);
        this.IndexingExecutionContext.EventProcessingContext.IndexingUnitChangeEventSelector = (IIndexingUnitChangeEventSelector) new PriorityBasedIndexingUnitChangeEventSelector();
        if (this.IndexingExecutionContext.OperationFailureHandler == null)
          this.IndexingExecutionContext.OperationFailureHandler = CorePipelinePluginsFactory.GetPipelineFailureHandler(this.IndexingExecutionContext);
        return true;
      }
      catch (Exception ex)
      {
        resultMessage.Append((object) ex);
        return false;
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(this.TracePoint, AbstractSearchServiceIndexingJob.TraceArea, AbstractSearchServiceIndexingJob.TraceLayer, nameof (Initialize));
      }
    }

    protected internal virtual void PreRun() => this.MarkInProgressEventsAsQueued();

    protected internal virtual void PostRun()
    {
      this.UpdateJobQueueController();
      this.MarkUnprocessedQueuedEventsAsPending();
      this.NotifyIndexingUnitChangeEventProcessor();
    }

    protected virtual void NotifyIndexingUnitChangeEventProcessor() => this.IndexingExecutionContext.RequestContext.GetService<IIndexingUnitChangeEventProcessor>().Process((ExecutionContext) this.IndexingExecutionContext, this.IndexingUnit.EntityType);

    internal virtual bool ExtractIndexingUnit(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      StringBuilder resultMessage)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(this.TracePoint, AbstractSearchServiceIndexingJob.TraceArea, AbstractSearchServiceIndexingJob.TraceLayer, nameof (ExtractIndexingUnit));
      int indexingUnitId = new SearchServiceJobData(jobDefinition.Data).IndexingUnitId;
      this.IndexingUnit = this.DataAccessFactory.GetIndexingUnitDataAccess().GetIndexingUnit(requestContext, indexingUnitId);
      if (this.IndexingUnit == null)
      {
        string message = FormattableString.Invariant(FormattableStringFactory.Create("No Indexing Unit found with {0}. Returning without executing any operation. ", (object) indexingUnitId)) + FormattableString.Invariant(FormattableStringFactory.Create("Host Type: {0}, Host ID: {1}. ", (object) requestContext.ServiceHost.HostType, (object) requestContext.ServiceHost.InstanceId));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(this.TracePoint, AbstractSearchServiceIndexingJob.TraceArea, AbstractSearchServiceIndexingJob.TraceLayer, message);
        resultMessage.Append(message);
        return false;
      }
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(this.TracePoint, AbstractSearchServiceIndexingJob.TraceArea, AbstractSearchServiceIndexingJob.TraceLayer, nameof (ExtractIndexingUnit));
      return true;
    }

    private void UpdateJobQueueController()
    {
      if (!this.IndexingExecutionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment || this.m_jobQueueController.ThrottleStatus.RunStatus != JobRunStatus.ExecuteOnHighCpu)
        return;
      this.m_jobQueueController.PostRunUpdateCpuThresholdJobExecutionCount();
    }

    private bool CheckFaultStateForRequeue(StringBuilder resultMessage)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(this.TracePoint, AbstractSearchServiceIndexingJob.TraceArea, AbstractSearchServiceIndexingJob.TraceLayer, nameof (CheckFaultStateForRequeue));
      try
      {
        this.m_jobQueueController = new JobQueueController(this.IndexingExecutionContext.RequestContext);
        JobThrottleStatus jobThrottleStatus = this.m_jobQueueController.GetJobThrottleStatus();
        if (this.m_jobQueueController.ShouldRequeue(jobThrottleStatus.RunStatus))
        {
          IIndexingUnitChangeEventDataAccess changeEventDataAccess = this.DataAccessFactory.GetIndexingUnitChangeEventDataAccess();
          string message1 = FormattableString.Invariant(FormattableStringFactory.Create("Retrieving Queued Change Events to be processed for Indexing Unit : {0}", (object) this.IndexingUnit));
          IEnumerable<IndexingUnitChangeEventDetails> unitChangeEvents = changeEventDataAccess.GetIndexingUnitChangeEvents(this.IndexingExecutionContext.RequestContext, this.IndexingUnit.IndexingUnitId, IndexingUnitChangeEventState.Queued, int.MaxValue);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TracePoint, AbstractSearchServiceIndexingJob.TraceArea, AbstractSearchServiceIndexingJob.TraceLayer, message1);
          if (unitChangeEvents != null)
          {
            if (unitChangeEvents.Any<IndexingUnitChangeEventDetails>())
            {
              int requeueDelayInSec = (int) jobThrottleStatus.RequeueDelay.TotalSeconds;
              List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> list = unitChangeEvents.Select<IndexingUnitChangeEventDetails, Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>((Func<IndexingUnitChangeEventDetails, Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) (x => x.IndexingUnitChangeEvent)).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
              list.ForEach((Action<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) (x =>
              {
                x.State = IndexingUnitChangeEventState.Pending;
                x.ChangeData.Delay = TimeSpan.FromSeconds((double) requeueDelayInSec);
              }));
              string message2 = FormattableString.Invariant(FormattableStringFactory.Create("Marking {0} events back to Pending state with requeueDelay {1}sec as the IP state is not healthy. Job ID {2}", (object) list.Count<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>(), (object) requeueDelayInSec, (object) this.IndexingUnit.AssociatedJobId));
              changeEventDataAccess.UpdateIndexingUnitChangeEvents(this.IndexingExecutionContext.RequestContext, (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) list);
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TracePoint, AbstractSearchServiceIndexingJob.TraceArea, AbstractSearchServiceIndexingJob.TraceLayer, message2);
              resultMessage.Append(message2);
              return true;
            }
          }
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(this.TracePoint, AbstractSearchServiceIndexingJob.TraceArea, AbstractSearchServiceIndexingJob.TraceLayer, nameof (CheckFaultStateForRequeue));
      }
      return false;
    }

    internal virtual void MarkUnprocessedQueuedEventsAsPending()
    {
      IIndexingUnitChangeEventDataAccess changeEventDataAccess = this.DataAccessFactory.GetIndexingUnitChangeEventDataAccess();
      IEnumerable<IndexingUnitChangeEventDetails> unitChangeEvents = changeEventDataAccess.GetIndexingUnitChangeEvents(this.IndexingExecutionContext.RequestContext, this.IndexingUnit.IndexingUnitId, IndexingUnitChangeEventState.Queued, int.MaxValue);
      if (unitChangeEvents == null || !unitChangeEvents.Any<IndexingUnitChangeEventDetails>())
        return;
      List<IndexingUnitChangeEventDetails> list1 = unitChangeEvents.ToList<IndexingUnitChangeEventDetails>();
      if (this.ChangeEventsToBeProcessed != null && this.ChangeEventsToBeProcessed.Any<IndexingUnitChangeEventDetails>())
      {
        IEnumerable<long> changeEventIds = this.ChangeEventsToBeProcessed.Select<IndexingUnitChangeEventDetails, long>((Func<IndexingUnitChangeEventDetails, long>) (x => x.IndexingUnitChangeEvent.Id));
        list1.RemoveAll((Predicate<IndexingUnitChangeEventDetails>) (x => changeEventIds.Contains<long>(x.IndexingUnitChangeEvent.Id)));
      }
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> list2 = list1.Select<IndexingUnitChangeEventDetails, Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>((Func<IndexingUnitChangeEventDetails, Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) (x => x.IndexingUnitChangeEvent)).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
      if (!list2.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>())
        return;
      list2.ForEach((Action<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) (x => x.State = IndexingUnitChangeEventState.Pending));
      changeEventDataAccess.UpdateIndexingUnitChangeEvents(this.IndexingExecutionContext.RequestContext, (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) list2);
    }

    internal virtual void MarkInProgressEventsAsQueued()
    {
      IIndexingUnitChangeEventDataAccess changeEventDataAccess = this.DataAccessFactory.GetIndexingUnitChangeEventDataAccess();
      IEnumerable<IndexingUnitChangeEventDetails> unitChangeEvents = changeEventDataAccess.GetIndexingUnitChangeEvents(this.IndexingExecutionContext.RequestContext, this.IndexingUnit.IndexingUnitId, IndexingUnitChangeEventState.InProgress, 1);
      if (unitChangeEvents == null || !unitChangeEvents.Any<IndexingUnitChangeEventDetails>())
        return;
      IndexingUnitChangeEventDetails changeEventDetails = unitChangeEvents.First<IndexingUnitChangeEventDetails>();
      changeEventDetails.IndexingUnitChangeEvent.State = IndexingUnitChangeEventState.Queued;
      changeEventDetails.IndexingUnitChangeEvent.ChangeData.Delay = new TimeSpan(0L);
      changeEventDataAccess.UpdateIndexingUnitChangeEvent(this.IndexingExecutionContext.RequestContext, changeEventDetails.IndexingUnitChangeEvent);
    }

    internal virtual TeamFoundationJobExecutionResult GetJobResultFromOperationResult(
      IEnumerable<OperationResult> operationResults)
    {
      if (operationResults == null)
        return TeamFoundationJobExecutionResult.Succeeded;
      TeamFoundationJobExecutionResult fromOperationResult = TeamFoundationJobExecutionResult.Succeeded;
      foreach (OperationResult operationResult in operationResults)
      {
        if (operationResult.Status == OperationStatus.Failed || operationResult.Status == OperationStatus.FailedAndRetry)
          return TeamFoundationJobExecutionResult.Failed;
        if (operationResult.Status == OperationStatus.PartiallySucceeded)
          fromOperationResult = TeamFoundationJobExecutionResult.PartiallySucceeded;
      }
      return fromOperationResult;
    }

    private string GetExecutionSummary(
      IDictionary<long, OperationResult> operationResults)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (!operationResults.Any<KeyValuePair<long, OperationResult>>())
      {
        stringBuilder.Append("No Operations Executed, hence returning. ");
        return stringBuilder.ToString();
      }
      foreach (OperationStatus operationStatus in (IEnumerable<OperationStatus>) Enum.GetValues(typeof (OperationStatus)).Cast<OperationStatus>().ToList<OperationStatus>())
      {
        OperationStatus status = operationStatus;
        IEnumerable<long> longs = operationResults.Where<KeyValuePair<long, OperationResult>>((Func<KeyValuePair<long, OperationResult>, bool>) (kvp => kvp.Value.Status == status)).Select<KeyValuePair<long, OperationResult>, long>((Func<KeyValuePair<long, OperationResult>, long>) (kvp => kvp.Key));
        if (longs.Any<long>())
          stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Events ({0}) completed with status {1}. ", (object) string.Join<long>(",", longs), (object) status)));
      }
      foreach (KeyValuePair<long, OperationResult> operationResult in (IEnumerable<KeyValuePair<long, OperationResult>>) operationResults)
        stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Event {0} completed with message '{1}'. ", (object) operationResult.Key, (object) operationResult.Value.Message)));
      return stringBuilder.ToString();
    }

    internal virtual void PublishJobFailureKPI(
      IVssRequestContext requestContext,
      StringBuilder resultMessage)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("IndexingJobFailureRate", AbstractSearchServiceIndexingJob.TraceArea, 1.0);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace(AbstractSearchServiceIndexingJob.TraceArea, AbstractSearchServiceIndexingJob.TraceLayer, "OperationStatus", (object) "Failed", true);
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      string jsonString = Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.GetOnPremiseTelemetryProperties().ToJsonString();
      resultMessage.Append("OnPremiseTelemetryProperties :" + jsonString);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace(AbstractSearchServiceIndexingJob.TraceArea, AbstractSearchServiceIndexingJob.TraceLayer, "JobExecutionResultMessage", (object) resultMessage.ToString(), true);
    }

    internal virtual void QueuePeriodicCatchUpJobOnJobFailures()
    {
      if (this.IndexingExecutionContext == null)
        return;
      IVssRequestContext requestContext = this.IndexingExecutionContext.RequestContext;
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      switch (this.IndexingUnit.EntityType.Name)
      {
        case "Code":
          List<TeamFoundationJobDefinition> foundationJobDefinitionList1 = service.QueryJobDefinitions(requestContext, (IEnumerable<Guid>) new List<Guid>()
          {
            JobConstants.PeriodicCatchUpJobId
          });
          // ISSUE: explicit non-virtual call
          if (foundationJobDefinitionList1 == null || __nonvirtual (foundationJobDefinitionList1.Count) <= 0 || foundationJobDefinitionList1[0] == null)
            break;
          requestContext.QueuePeriodicCatchUpJob(this.IndexingExecutionContext.ServiceSettings.JobSettings.PeriodicCatchUpJobDelayInSec);
          break;
        case "Wiki":
          List<TeamFoundationJobDefinition> foundationJobDefinitionList2 = service.QueryJobDefinitions(requestContext, (IEnumerable<Guid>) new List<Guid>()
          {
            JobConstants.PeriodicWikiCatchUpJobId
          });
          // ISSUE: explicit non-virtual call
          if (foundationJobDefinitionList2 == null || __nonvirtual (foundationJobDefinitionList2.Count) <= 0 || foundationJobDefinitionList2[0] == null)
            break;
          requestContext.QueuePeriodicWikiCatchUpJob(this.IndexingExecutionContext.ServiceSettings.JobSettings.PeriodicWikiCatchUpJobDelayInSec);
          break;
      }
    }

    internal virtual IIndexingUnitChangeEventHandler GetIndexingUnitChangeEventHandler() => (IIndexingUnitChangeEventHandler) new IndexingUnitChangeEventHandler();

    internal virtual IOperationMapper GetOperationMapper(
      CoreIndexingExecutionContext indexingExecutionContext)
    {
      if (this.OperationMapper == null)
      {
        this.OperationMapper = CorePipelinePluginsFactory.GetOperationMapper(indexingExecutionContext);
        if (this.OperationMapper == null)
          throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Could not find the Mapper class for Indexing Unit Kind: {0}", (object) indexingExecutionContext.IndexingUnit.EntityType.Name)));
      }
      return this.OperationMapper;
    }

    internal virtual void PublishOnPremTelemetryAndClearContext()
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishOnPremiseIndicator("TFS/Search/Indexing");
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(this.TracePoint, AbstractSearchServiceIndexingJob.TraceArea, AbstractSearchServiceIndexingJob.TraceLayer, nameof (PublishOnPremTelemetryAndClearContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      this.IndexingExecutionContext = (CoreIndexingExecutionContext) null;
    }

    protected abstract int TracePoint { get; }
  }
}
