// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.PriorityBasedIndexingUnitChangeEventSelector
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class PriorityBasedIndexingUnitChangeEventSelector : IIndexingUnitChangeEventSelector
  {
    private const string TraceArea = "Indexing Pipeline";
    private const string TraceLayer = "IndexingUnitChangeEventHandler";
    private readonly LeaseFilter m_leaseFilter;

    public PriorityBasedIndexingUnitChangeEventSelector() => this.m_leaseFilter = new LeaseFilter();

    public virtual IDictionary<JobPriorityLevel, List<IndexingUnitChangeEventDetails>> GetChangeEventsToBeProcessed(
      ExecutionContext executionContext,
      int indexingUnitId,
      IEnumerable<IndexingUnitChangeEventDetails> changeEvents)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", nameof (GetChangeEventsToBeProcessed));
      IDictionary<JobPriorityLevel, List<IndexingUnitChangeEventDetails>> eventsToBeProcessed = (IDictionary<JobPriorityLevel, List<IndexingUnitChangeEventDetails>>) new FriendlyDictionary<JobPriorityLevel, List<IndexingUnitChangeEventDetails>>();
      try
      {
        if (changeEvents != null)
        {
          IEnumerable<IndexingUnitChangeEventDetails> filteredEvents = this.GetFilteredEvents(executionContext, changeEvents.ToList<IndexingUnitChangeEventDetails>());
          if (filteredEvents != null)
          {
            if (filteredEvents.Any<IndexingUnitChangeEventDetails>())
            {
              PriorityManagerService serv = executionContext.RequestContext.To(TeamFoundationHostType.Deployment).GetService<PriorityManagerService>();
              int num = executionContext.RequestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/LargeRepositoryEnableHighPriorityIndexing") ? 1 : 0;
              List<IndexingUnitChangeEventDetails> list1 = filteredEvents.OrderByDescending<IndexingUnitChangeEventDetails, int>((Func<IndexingUnitChangeEventDetails, int>) (x => (int) serv.GetPriority(executionContext.RequestContext, x.IndexingUnitChangeEvent))).ToList<IndexingUnitChangeEventDetails>();
              JobPriorityLevel key = serv.GetPriority(executionContext.RequestContext, list1[0].IndexingUnitChangeEvent);
              if (num != 0 && list1.Any<IndexingUnitChangeEventDetails>((Func<IndexingUnitChangeEventDetails, bool>) (x => x.IndexingUnitChangeEvent.ChangeData.Trigger == 31)))
                key = JobPriorityLevel.Normal;
              List<IndexingUnitChangeEventDetails> list2 = list1.Take<IndexingUnitChangeEventDetails>(executionContext.EventProcessingContext.EventProcessingBatchSize).ToList<IndexingUnitChangeEventDetails>();
              eventsToBeProcessed.Add(key, list2);
            }
          }
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", nameof (GetChangeEventsToBeProcessed));
      }
      return eventsToBeProcessed;
    }

    internal IEnumerable<IndexingUnitChangeEventDetails> GetFilteredEvents(
      ExecutionContext executionContext,
      List<IndexingUnitChangeEventDetails> allChangeEvents)
    {
      List<IndexingUnitChangeEventDetails> filteredEvents = this.m_leaseFilter.GetEventsWithLease((IEnumerable<IndexingUnitChangeEventDetails>) allChangeEvents).ToList<IndexingUnitChangeEventDetails>();
      if (!filteredEvents.Any<IndexingUnitChangeEventDetails>())
        filteredEvents = allChangeEvents;
      else
        PriorityBasedIndexingUnitChangeEventSelector.PublishTelemetryForChangeEventNotPicked(executionContext, allChangeEvents.Where<IndexingUnitChangeEventDetails>((Func<IndexingUnitChangeEventDetails, bool>) (ce1 => filteredEvents.All<IndexingUnitChangeEventDetails>((Func<IndexingUnitChangeEventDetails, bool>) (ce2 => ce2.IndexingUnitChangeEvent.Id != ce1.IndexingUnitChangeEvent.Id)))).ToList<IndexingUnitChangeEventDetails>());
      return (IEnumerable<IndexingUnitChangeEventDetails>) filteredEvents;
    }

    private static void PublishTelemetryForChangeEventNotPicked(
      ExecutionContext executionContext,
      List<IndexingUnitChangeEventDetails> changeEvents)
    {
      DateTime utcNow = DateTime.UtcNow;
      foreach (IndexingUnitChangeEventDetails changeEvent in changeEvents)
      {
        double totalSeconds = utcNow.Subtract(changeEvent.IndexingUnitChangeEvent.LastModificationTimeUtc).TotalSeconds;
        double num = PriorityBasedIndexingUnitChangeEventSelector.FetchDelayFromTriggerTime(utcNow, changeEvent.IndexingUnitChangeEvent.ChangeData);
        ClientTraceData clientTraceData = new ClientTraceData();
        clientTraceData.Add("ChangeEventActionTakenByProcessor", (object) 2);
        clientTraceData.Add("ChangeEventWaitTimeInSec", (object) totalSeconds);
        clientTraceData.Add("ChangeEventCorrelationId", (object) changeEvent.IndexingUnitChangeEvent.ChangeData.CorrelationId);
        clientTraceData.Add("ChangeEventId", (object) changeEvent.IndexingUnitChangeEvent.Id);
        clientTraceData.Add("ChangeType", (object) changeEvent.IndexingUnitChangeEvent.ChangeType);
        clientTraceData.Add("IndexingUnitId", (object) changeEvent.IndexingUnitChangeEvent.IndexingUnitId);
        clientTraceData.Add("JobTrigger", (object) changeEvent.IndexingUnitChangeEvent.ChangeData.Trigger);
        clientTraceData.Add("JobTriggerTime", (object) changeEvent.IndexingUnitChangeEvent.ChangeData.TriggerTimeUtc);
        clientTraceData.Add("TimeDifferenceFromTriggerTimeInSec", (object) num);
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
  }
}
