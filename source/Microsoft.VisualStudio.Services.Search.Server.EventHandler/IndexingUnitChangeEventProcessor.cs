// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.EventHandler.IndexingUnitChangeEventProcessor
// Assembly: Microsoft.VisualStudio.Services.Search.Server.EventHandler, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 86A812E9-C14F-422E-83C2-D709899BDEBA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.EventHandler.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.EventHandler
{
  internal class IndexingUnitChangeEventProcessor : 
    IIndexingUnitChangeEventProcessor,
    IVssFrameworkService
  {
    private const string TraceArea = "Indexing Pipeline";
    private const string TraceLayer = "IndexingUnitChangeEventHandler";
    internal static readonly int s_maxRequestToBeFetch = 100;

    internal IDataAccessFactory DataAccessFactory { get; set; }

    internal IndexingUnitLockManager IndexingUnitLockManager { get; set; }

    internal IIndexingUnitChangeEventDataAccess IndexingUnitChangeEventDataAccess { get; set; }

    public IndexingUnitChangeEventProcessor()
      : this(Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance(), new IndexingUnitLockManager())
    {
    }

    internal IndexingUnitChangeEventProcessor(
      IDataAccessFactory dataAccessFactory,
      IndexingUnitLockManager indexingUnitLockManager)
    {
      this.DataAccessFactory = dataAccessFactory;
      this.IndexingUnitLockManager = indexingUnitLockManager;
      this.IndexingUnitChangeEventDataAccess = dataAccessFactory.GetIndexingUnitChangeEventDataAccess();
    }

    public void Process(ExecutionContext executionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", nameof (Process));
      try
      {
        IEnumerable<IGrouping<string, IndexingUnitChangeEventDetailsWithEntityType>> source = this.GetNextEventsForProcessing(executionContext, -1, (IEntityType) AllEntityType.GetInstance()).GroupBy<IndexingUnitChangeEventDetailsWithEntityType, string>((Func<IndexingUnitChangeEventDetailsWithEntityType, string>) (x => x.EntityType.Name));
        if (!source.Any<IGrouping<string, IndexingUnitChangeEventDetailsWithEntityType>>())
          return;
        if (!source.Any<IGrouping<string, IndexingUnitChangeEventDetailsWithEntityType>>((Func<IGrouping<string, IndexingUnitChangeEventDetailsWithEntityType>, bool>) (x => x.Key == null)))
        {
          foreach (IGrouping<string, IndexingUnitChangeEventDetailsWithEntityType> grouping in source)
          {
            IEntityType entityType = grouping.First<IndexingUnitChangeEventDetailsWithEntityType>().EntityType;
            this.ProcessEvents(executionContext, -1, entityType, (IEnumerable<IndexingUnitChangeEventDetails>) grouping);
          }
        }
        else
        {
          this.Process(executionContext, (IEntityType) CodeEntityType.GetInstance());
          this.Process(executionContext, (IEntityType) WorkItemEntityType.GetInstance());
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", nameof (Process));
      }
    }

    public void Process(ExecutionContext executionContext, IEntityType entityType)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", nameof (Process));
      try
      {
        IEnumerable<IndexingUnitChangeEventDetails> eventsForProcessing = (IEnumerable<IndexingUnitChangeEventDetails>) this.GetNextEventsForProcessing(executionContext, -1, entityType);
        this.ProcessEvents(executionContext, -1, entityType, eventsForProcessing);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", nameof (Process));
      }
    }

    public void Process(ExecutionContext executionContext, int indexingUnitId)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", nameof (Process));
      try
      {
        if (indexingUnitId < 0)
        {
          this.Process(executionContext);
        }
        else
        {
          IIndexingUnitDataAccess indexingUnitDataAccess = this.DataAccessFactory.GetIndexingUnitDataAccess();
          if (executionContext == null)
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceAlways(1080522, TraceLevel.Error, "Indexing Pipeline", "IndexingUnitChangeEventHandler", "ExecutionContetxt is null, exiting.");
          }
          else
          {
            Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = indexingUnitDataAccess.GetIndexingUnit(executionContext.RequestContext, indexingUnitId);
            if (indexingUnit == null)
            {
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceAlways(1080522, TraceLevel.Error, "Indexing Pipeline", "IndexingUnitChangeEventHandler", "Indexing Unit is null for indexingUnitId " + indexingUnitId.ToString() + ", exiting");
            }
            else
            {
              IEnumerable<IndexingUnitChangeEventDetails> eventsForProcessing = (IEnumerable<IndexingUnitChangeEventDetails>) this.GetNextEventsForProcessing(executionContext, indexingUnitId, indexingUnit.EntityType);
              this.ProcessEvents(executionContext, indexingUnitId, indexingUnit.EntityType, eventsForProcessing);
            }
          }
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", nameof (Process));
      }
    }

    private void ProcessEvents(
      ExecutionContext executionContext,
      int indexingUnitId,
      IEntityType entityType,
      IEnumerable<IndexingUnitChangeEventDetails> filteredEvents)
    {
      IVssRequestContext requestContext = executionContext.RequestContext;
      if (filteredEvents == null || !filteredEvents.Any<IndexingUnitChangeEventDetails>())
        return;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", "Getting events for a job with priority");
      IDictionary<int, List<IndexingUnitChangeEventDetails>> toChangeEventsMap = this.GetIndexingUnitIdToChangeEventsMap(filteredEvents);
      IDictionary<Guid, int> indexingUnitIdMap = this.GetJobIdToIndexingUnitIdMap(toChangeEventsMap);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", "Constructing a map of Job Priortity to Job Ids.");
      int runningJobsCount = this.GetQueuedAndRunningJobsCount(requestContext, entityType);
      JobResourcesRequest jobResourcesRequest = new JobResourcesRequest()
      {
        UsedResources = runningJobsCount,
        RequestedResources = indexingUnitIdMap.Count,
        EntityType = entityType
      };
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("NumOfRequestedJobResources", "Indexing Pipeline", (double) jobResourcesRequest.RequestedResources);
      bool needCallBack;
      JobResourcesResponse resourcesToProcessEvents = this.GetResourcesToProcessEvents(executionContext, jobResourcesRequest, out needCallBack, entityType);
      int totalMinutes = (int) resourcesToProcessEvents.DelayToQueueCallback.TotalMinutes;
      int totalSeconds = (int) resourcesToProcessEvents.DelayToQueueJobs.TotalSeconds;
      int allottedResources = resourcesToProcessEvents.AllottedResources;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("NumOfAllottedJobResources", "Indexing Pipeline", (double) allottedResources);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("Indexing Pipeline", "Indexing Pipeline", (IDictionary<string, object>) new FriendlyDictionary<string, object>()
      {
        ["RequestedResources"] = (object) jobResourcesRequest.RequestedResources,
        ["UsedResources"] = (object) jobResourcesRequest.UsedResources,
        ["EntityType"] = (object) jobResourcesRequest.EntityType,
        ["AllotedResources"] = (object) resourcesToProcessEvents.AllottedResources
      });
      this.QueueJobs(executionContext, indexingUnitIdMap, toChangeEventsMap, allottedResources, totalSeconds);
      if (!needCallBack)
        return;
      ITeamFoundationTaskService service = IVssRequestContextExtensions.ElevateAsNeeded(requestContext).GetService<ITeamFoundationTaskService>();
      TeamFoundationTask teamFoundationTask = new TeamFoundationTask(new Microsoft.TeamFoundation.Framework.Server.TeamFoundationTaskCallback(this.TeamFoundationTaskCallback), (object) new TeamFoundationCallbackTaskArgs()
      {
        IndexingUnitId = indexingUnitId,
        EntityType = entityType,
        TracerCICorrelationDetails = executionContext.ExecutionTracerContext.TracerCICorrelationDetails,
        EventProcessingContext = executionContext.EventProcessingContext
      }, DateTime.UtcNow.AddMinutes((double) totalMinutes), 0);
      Guid currentHostId = requestContext.GetCurrentHostId();
      TeamFoundationTask task = teamFoundationTask;
      service.AddTask(currentHostId, task);
    }

    private IEnumerable<IndexingUnitChangeEventDetailsWithEntityType> GetNextEventsForProcessing(
      ExecutionContext executionContext,
      int indexingUnitId,
      IEntityType entityType)
    {
      IVssRequestContext requestContext = executionContext.RequestContext;
      int eventsQueryBatchSize = executionContext.EventProcessingContext.EventsQueryBatchSize;
      if (executionContext.RequestContext.IsFeatureEnabled("Search.Server.IndexingUnitLocking"))
        this.ReleaseLocksOfCompletedEvents(executionContext);
      IEnumerable<IndexingUnitChangeEventDetailsWithEntityType> filteredEvents;
      while (true)
      {
        IEnumerable<IndexingUnitChangeEventDetailsWithEntityType> detailsWithEntityTypes = indexingUnitId >= 0 || entityType.Name.Equals("All") ? this.GetNextSetOfEvents(requestContext, indexingUnitId, eventsQueryBatchSize) : this.GetNextSetOfEvents(requestContext, entityType, eventsQueryBatchSize);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", "Filtering Indexing Unit Change Events by applying prerequisites");
        filteredEvents = this.GetFilteredEvents(executionContext, detailsWithEntityTypes);
        if ((filteredEvents == null || !filteredEvents.Any<IndexingUnitChangeEventDetailsWithEntityType>()) && detailsWithEntityTypes.Count<IndexingUnitChangeEventDetailsWithEntityType>() == eventsQueryBatchSize)
          checked { eventsQueryBatchSize += eventsQueryBatchSize; }
        else
          break;
      }
      return filteredEvents;
    }

    internal virtual void ReleaseLocksOfCompletedEvents(ExecutionContext executionContext)
    {
      IIndexingUnitChangeEventDataAccess changeEventDataAccess = this.DataAccessFactory.GetIndexingUnitChangeEventDataAccess();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", "Releasing locks of events which have successfully completed.");
      IVssRequestContext requestContext = executionContext.RequestContext;
      changeEventDataAccess.ReleaseLocksOfCompletedEvents(requestContext);
    }

    [Info("InternalForTestPurpose")]
    internal void TeamFoundationTaskCallback(IVssRequestContext requestContext, object args)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", nameof (TeamFoundationTaskCallback));
      try
      {
        TeamFoundationCallbackTaskArgs callbackTaskArgs = args as TeamFoundationCallbackTaskArgs;
        ExecutionContext executionContext = new ExecutionContext(requestContext, callbackTaskArgs.TracerCICorrelationDetails, callbackTaskArgs.EventProcessingContext);
        if (callbackTaskArgs.IndexingUnitId < 0 && !callbackTaskArgs.EntityType.Name.Equals("All"))
          this.Process(executionContext, callbackTaskArgs.EntityType);
        else
          this.Process(executionContext, callbackTaskArgs.IndexingUnitId);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", nameof (TeamFoundationTaskCallback));
      }
    }

    internal virtual JobResourcesResponse GetResourcesToProcessEvents(
      ExecutionContext executionContext,
      JobResourcesRequest jobResourcesRequest,
      out bool needCallBack,
      IEntityType entityType)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", nameof (GetResourcesToProcessEvents));
      try
      {
        JobResourcesResponse resourcesToQueueJobs = IVssRequestContextExtensions.ElevateAsNeeded(executionContext.RequestContext).GetService<ICoreResourceManagementService>().GetJobResourceController(executionContext.RequestContext, entityType).GetResourcesToQueueJobs(executionContext, jobResourcesRequest);
        needCallBack = resourcesToQueueJobs.DelayToQueueCallback.Minutes != -1;
        return resourcesToQueueJobs;
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", nameof (GetResourcesToProcessEvents));
      }
    }

    internal virtual HashSet<Guid> QueueJobs(
      ExecutionContext executionContext,
      IDictionary<Guid, int> jobIdToIndexingUnitIdMap,
      IDictionary<int, List<IndexingUnitChangeEventDetails>> indexingUnitIdToChangeEvents,
      int availableResources,
      int jobQueuingDelayInSeconds)
    {
      IDictionary<JobPriorityLevel, List<IndexingUnitChangeEventDetails>> priorityToBeProcessed = this.GetListOfEventsWithPriorityToBeProcessed(executionContext, indexingUnitIdToChangeEvents, executionContext.EventProcessingContext.IndexingUnitChangeEventSelector);
      IDictionary<JobPriorityLevel, HashSet<Guid>> jobIdsMap = this.ConstructJobPriorityLevelToJobIdsMap(priorityToBeProcessed);
      List<IndexingUnitChangeEventDetails> indexingUnitChangeEvents = new List<IndexingUnitChangeEventDetails>();
      foreach (List<IndexingUnitChangeEventDetails> collection in (IEnumerable<List<IndexingUnitChangeEventDetails>>) priorityToBeProcessed.Values)
        indexingUnitChangeEvents.AddRange((IEnumerable<IndexingUnitChangeEventDetails>) collection);
      IDictionary<int, List<IndexingUnitChangeEventDetails>> toChangeEventsMap = this.GetIndexingUnitIdToChangeEventsMap((IEnumerable<IndexingUnitChangeEventDetails>) indexingUnitChangeEvents);
      HashSet<Guid> guidSet = new HashSet<Guid>();
      bool flag1 = executionContext.RequestContext.IsFeatureEnabled("Search.Server.IndexingUnitLocking");
      foreach (KeyValuePair<JobPriorityLevel, HashSet<Guid>> keyValuePair in (IEnumerable<KeyValuePair<JobPriorityLevel, HashSet<Guid>>>) jobIdsMap.OrderByDescending<KeyValuePair<JobPriorityLevel, HashSet<Guid>>, int>((Func<KeyValuePair<JobPriorityLevel, HashSet<Guid>>, int>) (t => (int) t.Key)))
      {
        foreach (Guid guid in keyValuePair.Value)
        {
          int toIndexingUnitId = jobIdToIndexingUnitIdMap[guid];
          List<IndexingUnitChangeEventDetails> source = toChangeEventsMap[toIndexingUnitId];
          List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> list = source != null ? source.Select<IndexingUnitChangeEventDetails, Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>((Func<IndexingUnitChangeEventDetails, Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) (x => x.IndexingUnitChangeEvent)).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>() : (List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) null;
          if (list != null && list.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>())
          {
            if (availableResources <= 0)
            {
              this.PublishTelemetryForChangeEventActionTaken(executionContext, list, DateTime.UtcNow, 3);
            }
            else
            {
              DateTime minimumQueuingTime = this.GetMinimumQueuingTime(list);
              int num1 = string.IsNullOrWhiteSpace(list.First<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>().LeaseId) ? 1 : 0;
              bool flag2 = true;
              bool flag3 = true;
              if (num1 != 0 && flag1)
              {
                flag2 = this.IndexingUnitLockManager.AcquireNecessaryLocks(executionContext, toIndexingUnitId, (IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) list).LockAcquired;
                flag3 = false;
              }
              if (flag3)
              {
                list.ForEach((Action<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) (x => x.State = IndexingUnitChangeEventState.Queued));
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", FormattableString.Invariant(FormattableStringFactory.Create("Updating ({0}) Indexing Unit Change Events to Queued state", (object) string.Join<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>(",", (IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) list))));
                this.IndexingUnitChangeEventDataAccess.UpdateIndexingUnitChangeEvents(executionContext.RequestContext, (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) list);
              }
              DateTime utcNow = DateTime.UtcNow;
              if (flag2)
              {
                int totalSeconds = minimumQueuingTime > utcNow ? (int) (minimumQueuingTime - utcNow).TotalSeconds : 0;
                int num2 = Math.Max(jobQueuingDelayInSeconds, totalSeconds);
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", FormattableString.Invariant(FormattableStringFactory.Create("Queueing Job {0} with delay {1}", (object) guid, (object) num2)));
                this.QueueJobsWithPriority(executionContext.RequestContext, keyValuePair.Key, guid, num2);
                guidSet.Add(guid);
                --availableResources;
                this.PublishTelemetryForChangeEventActionTaken(executionContext, list, utcNow, 0, num2);
              }
              else
              {
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", FormattableString.Invariant(FormattableStringFactory.Create("At Time: {0} JobId: {1} failed with ChangeEvents {2}", (object) utcNow, (object) guid, (object) string.Join<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>(", ", (IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) list))));
                this.PublishTelemetryForChangeEventActionTaken(executionContext, list, utcNow, 1);
              }
            }
          }
        }
      }
      return guidSet;
    }

    internal virtual void PublishTelemetryForChangeEventActionTaken(
      ExecutionContext executionContext,
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> changeEvents,
      DateTime currentTimeUtc,
      int processorAction,
      int queueDelayTime = -1)
    {
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent changeEvent in changeEvents)
      {
        double totalSeconds = currentTimeUtc.Subtract(changeEvent.LastModificationTimeUtc).TotalSeconds;
        double num = IndexingUnitChangeEventProcessor.FetchDelayFromTriggerTime(currentTimeUtc, changeEvent.ChangeData);
        ClientTraceData clientTraceData = new ClientTraceData();
        clientTraceData.Add("ChangeEventActionTakenByProcessor", (object) processorAction);
        clientTraceData.Add("ChangeEventWaitTimeInSec", (object) totalSeconds);
        clientTraceData.Add("ChangeEventCorrelationId", (object) changeEvent.ChangeData.CorrelationId);
        clientTraceData.Add("ChangeEventId", (object) changeEvent.Id);
        clientTraceData.Add("ChangeType", (object) changeEvent.ChangeType);
        clientTraceData.Add("IndexingUnitId", (object) changeEvent.IndexingUnitId);
        clientTraceData.Add("JobTrigger", (object) changeEvent.ChangeData.Trigger);
        clientTraceData.Add("JobTriggerTime", (object) changeEvent.ChangeData.TriggerTimeUtc);
        clientTraceData.Add("TimeDifferenceFromTriggerTimeInSec", (object) num);
        if (queueDelayTime != -1)
          clientTraceData.Add("DelayAddedWhileQueuingJobInSec", (object) queueDelayTime);
        executionContext.ExecutionTracerContext.PublishClientTrace("Indexing Pipeline", "Indexer", clientTraceData);
      }
    }

    private static double FetchDelayFromTriggerTime(
      DateTime currentTimeUtc,
      ChangeEventData changeEventData)
    {
      double num = -1.0;
      try
      {
        num = currentTimeUtc.Subtract(changeEventData.TriggerTimeUtc).TotalSeconds;
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", FormattableString.Invariant(FormattableStringFactory.Create("FetchDelayFromTriggerTime failed with exception : {0}", (object) ex.ToString())));
      }
      return num;
    }

    internal virtual void QueueJobsWithPriority(
      IVssRequestContext requestContext,
      JobPriorityLevel priority,
      Guid jobId,
      int delayInSeconds)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", nameof (QueueJobsWithPriority));
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      IVssRequestContext requestContext1 = requestContext;
      List<Guid> jobIds = new List<Guid>();
      jobIds.Add(jobId);
      int maxDelaySeconds = delayInSeconds;
      int priorityLevel = (int) priority;
      service.QueueDelayedJobs(requestContext1, (IEnumerable<Guid>) jobIds, maxDelaySeconds, (JobPriorityLevel) priorityLevel);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", nameof (QueueJobsWithPriority));
    }

    internal IEnumerable<IndexingUnitChangeEventDetailsWithEntityType> GetFilteredEvents(
      ExecutionContext executionContext,
      IEnumerable<IndexingUnitChangeEventDetailsWithEntityType> indexingUnitChangeEvents)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", nameof (GetFilteredEvents));
      try
      {
        if (indexingUnitChangeEvents == null || !indexingUnitChangeEvents.Any<IndexingUnitChangeEventDetailsWithEntityType>())
          return (IEnumerable<IndexingUnitChangeEventDetailsWithEntityType>) new List<IndexingUnitChangeEventDetailsWithEntityType>();
        List<IndexingUnitChangeEventDetailsWithEntityType> filteredEvents = new List<IndexingUnitChangeEventDetailsWithEntityType>();
        foreach (IndexingUnitChangeEventDetailsWithEntityType indexingUnitChangeEvent in indexingUnitChangeEvents)
        {
          bool flag = true;
          IndexingUnitChangeEventPrerequisites prerequisites = indexingUnitChangeEvent.IndexingUnitChangeEvent.Prerequisites;
          if (prerequisites != null)
            flag = prerequisites.Evaluate(executionContext, this.DataAccessFactory, indexingUnitChangeEvent.EntityType, indexingUnitChangeEvent.IndexingUnitChangeEvent);
          if (flag)
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", FormattableString.Invariant(FormattableStringFactory.Create("Prerequisites matched for Indexing Unit Change Event {0}", (object) indexingUnitChangeEvent.IndexingUnitChangeEvent.ToString())));
            filteredEvents.Add(indexingUnitChangeEvent);
          }
        }
        this.PublishTelemetryForChangeEventActionTaken(executionContext, indexingUnitChangeEvents.Where<IndexingUnitChangeEventDetailsWithEntityType>((Func<IndexingUnitChangeEventDetailsWithEntityType, bool>) (ce1 => filteredEvents.All<IndexingUnitChangeEventDetailsWithEntityType>((Func<IndexingUnitChangeEventDetailsWithEntityType, bool>) (ce2 => ce2.IndexingUnitChangeEvent.Id != ce1.IndexingUnitChangeEvent.Id)))).Select<IndexingUnitChangeEventDetailsWithEntityType, Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>((Func<IndexingUnitChangeEventDetailsWithEntityType, Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) (x => x.IndexingUnitChangeEvent)).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>(), DateTime.UtcNow, 4);
        return (IEnumerable<IndexingUnitChangeEventDetailsWithEntityType>) filteredEvents;
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", nameof (GetFilteredEvents));
      }
    }

    internal virtual IDictionary<JobPriorityLevel, List<IndexingUnitChangeEventDetails>> GetListOfEventsWithPriorityToBeProcessed(
      ExecutionContext executionContext,
      IDictionary<int, List<IndexingUnitChangeEventDetails>> indexingUnitIdToChangeEvents,
      IIndexingUnitChangeEventSelector indexingUnitChangeEventSelector)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", nameof (GetListOfEventsWithPriorityToBeProcessed));
      IDictionary<JobPriorityLevel, List<IndexingUnitChangeEventDetails>> priorityToBeProcessed = (IDictionary<JobPriorityLevel, List<IndexingUnitChangeEventDetails>>) new Dictionary<JobPriorityLevel, List<IndexingUnitChangeEventDetails>>();
      try
      {
        if (indexingUnitIdToChangeEvents == null || !indexingUnitIdToChangeEvents.Any<KeyValuePair<int, List<IndexingUnitChangeEventDetails>>>())
          return priorityToBeProcessed;
        foreach (KeyValuePair<int, List<IndexingUnitChangeEventDetails>> unitIdToChangeEvent in (IEnumerable<KeyValuePair<int, List<IndexingUnitChangeEventDetails>>>) indexingUnitIdToChangeEvents)
        {
          int key1 = unitIdToChangeEvent.Key;
          List<IndexingUnitChangeEventDetails> changeEvents = unitIdToChangeEvent.Value;
          foreach (KeyValuePair<JobPriorityLevel, List<IndexingUnitChangeEventDetails>> keyValuePair in (IEnumerable<KeyValuePair<JobPriorityLevel, List<IndexingUnitChangeEventDetails>>>) indexingUnitChangeEventSelector.GetChangeEventsToBeProcessed(executionContext, key1, (IEnumerable<IndexingUnitChangeEventDetails>) changeEvents))
          {
            List<IndexingUnitChangeEventDetails> source = keyValuePair.Value;
            if (source != null && source.Any<IndexingUnitChangeEventDetails>())
            {
              JobPriorityLevel key2 = keyValuePair.Key;
              if (!priorityToBeProcessed.ContainsKey(key2))
                priorityToBeProcessed[key2] = new List<IndexingUnitChangeEventDetails>();
              priorityToBeProcessed[key2].AddRange((IEnumerable<IndexingUnitChangeEventDetails>) keyValuePair.Value);
            }
          }
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", nameof (GetListOfEventsWithPriorityToBeProcessed));
      }
      return priorityToBeProcessed;
    }

    internal IDictionary<JobPriorityLevel, HashSet<Guid>> ConstructJobPriorityLevelToJobIdsMap(
      IDictionary<JobPriorityLevel, List<IndexingUnitChangeEventDetails>> eventsWithPriority)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", nameof (ConstructJobPriorityLevelToJobIdsMap));
      IDictionary<JobPriorityLevel, HashSet<Guid>> jobIdsMap = (IDictionary<JobPriorityLevel, HashSet<Guid>>) new Dictionary<JobPriorityLevel, HashSet<Guid>>();
      try
      {
        if (eventsWithPriority == null || !eventsWithPriority.Any<KeyValuePair<JobPriorityLevel, List<IndexingUnitChangeEventDetails>>>())
          return jobIdsMap;
        foreach (KeyValuePair<JobPriorityLevel, List<IndexingUnitChangeEventDetails>> keyValuePair in (IEnumerable<KeyValuePair<JobPriorityLevel, List<IndexingUnitChangeEventDetails>>>) eventsWithPriority)
        {
          JobPriorityLevel key = keyValuePair.Key;
          if (!jobIdsMap.ContainsKey(key))
            jobIdsMap[key] = new HashSet<Guid>();
          foreach (IndexingUnitChangeEventDetails changeEventDetails in keyValuePair.Value)
            jobIdsMap[key].Add(changeEventDetails.AssociatedJobId.Value);
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", nameof (ConstructJobPriorityLevelToJobIdsMap));
      }
      return jobIdsMap;
    }

    internal IEnumerable<IndexingUnitChangeEventDetailsWithEntityType> GetNextSetOfEvents(
      IVssRequestContext requestContext,
      int indexingUnitId,
      int numberOfEventsToFetch)
    {
      IIndexingUnitChangeEventDataAccess changeEventDataAccess = this.DataAccessFactory.GetIndexingUnitChangeEventDataAccess();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", "Retrieving Pending and FailedAndRetry Indexing Unit Change Events");
      IVssRequestContext requestContext1 = requestContext;
      int count = numberOfEventsToFetch;
      int indexingUnitId1 = indexingUnitId;
      return (changeEventDataAccess.GetNextSetOfEventsToProcess(requestContext1, count, indexingUnitId1) ?? (IEnumerable<IndexingUnitChangeEventDetailsWithEntityType>) new List<IndexingUnitChangeEventDetailsWithEntityType>()).Where<IndexingUnitChangeEventDetailsWithEntityType>((Func<IndexingUnitChangeEventDetailsWithEntityType, bool>) (changeEvent => changeEvent.AssociatedJobId.HasValue));
    }

    internal IEnumerable<IndexingUnitChangeEventDetailsWithEntityType> GetNextSetOfEvents(
      IVssRequestContext requestContext,
      IEntityType entityType,
      int numberOfEventsToFetch)
    {
      IIndexingUnitChangeEventDataAccess changeEventDataAccess = this.DataAccessFactory.GetIndexingUnitChangeEventDataAccess();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", "Retrieving Pending and FailedAndRetry Indexing Unit Change Events");
      IVssRequestContext requestContext1 = requestContext;
      int count = numberOfEventsToFetch;
      IEntityType entityType1 = entityType;
      return (changeEventDataAccess.GetNextSetOfEventsToProcess(requestContext1, count, entityType1) ?? (IEnumerable<IndexingUnitChangeEventDetailsWithEntityType>) new List<IndexingUnitChangeEventDetailsWithEntityType>()).Where<IndexingUnitChangeEventDetailsWithEntityType>((Func<IndexingUnitChangeEventDetailsWithEntityType, bool>) (changeEvent => changeEvent.AssociatedJobId.HasValue));
    }

    internal int GetQueuedAndRunningJobsCount(
      IVssRequestContext requestContext,
      IEntityType entityType)
    {
      IIndexingUnitChangeEventDataAccess changeEventDataAccess = this.DataAccessFactory.GetIndexingUnitChangeEventDataAccess();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", "Retrieving InProgress and Queued Indexing Unit Change Events");
      IVssRequestContext requestContext1 = requestContext;
      IEntityType entityType1 = entityType;
      return changeEventDataAccess.GetCountOfIndexingUnitChangeEventsInProgressOrQueued(requestContext1, entityType1);
    }

    internal IDictionary<Guid, int> GetJobIdToIndexingUnitIdMap(
      IDictionary<int, List<IndexingUnitChangeEventDetails>> indexingUnitIdToChangeEvents)
    {
      IDictionary<Guid, int> indexingUnitIdMap = (IDictionary<Guid, int>) new Dictionary<Guid, int>();
      foreach (KeyValuePair<int, List<IndexingUnitChangeEventDetails>> unitIdToChangeEvent in (IEnumerable<KeyValuePair<int, List<IndexingUnitChangeEventDetails>>>) indexingUnitIdToChangeEvents)
      {
        int key1 = unitIdToChangeEvent.Key;
        Guid key2 = unitIdToChangeEvent.Value.First<IndexingUnitChangeEventDetails>().AssociatedJobId.Value;
        if (key2 != Guid.Empty)
          indexingUnitIdMap[key2] = key1;
      }
      return indexingUnitIdMap;
    }

    internal IDictionary<int, List<IndexingUnitChangeEventDetails>> GetIndexingUnitIdToChangeEventsMap(
      IEnumerable<IndexingUnitChangeEventDetails> indexingUnitChangeEvents)
    {
      IDictionary<int, List<IndexingUnitChangeEventDetails>> toChangeEventsMap = (IDictionary<int, List<IndexingUnitChangeEventDetails>>) new Dictionary<int, List<IndexingUnitChangeEventDetails>>();
      foreach (IndexingUnitChangeEventDetails indexingUnitChangeEvent in indexingUnitChangeEvents)
      {
        int indexingUnitId = indexingUnitChangeEvent.IndexingUnitChangeEvent.IndexingUnitId;
        if (!toChangeEventsMap.ContainsKey(indexingUnitId))
          toChangeEventsMap[indexingUnitId] = new List<IndexingUnitChangeEventDetails>();
        toChangeEventsMap[indexingUnitId].Add(indexingUnitChangeEvent);
      }
      return toChangeEventsMap;
    }

    internal virtual DateTime GetMinimumQueuingTime(List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> changeEvents)
    {
      if (changeEvents == null || !changeEvents.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>())
        return DateTime.UtcNow;
      DateTime minimumQueuingTime = DateTime.MaxValue;
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent changeEvent in changeEvents)
      {
        DateTime dateTime = changeEvent.LastModificationTimeUtc + changeEvent.ChangeData.Delay;
        if (dateTime < minimumQueuingTime)
          minimumQueuingTime = dateTime;
      }
      return minimumQueuingTime;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
