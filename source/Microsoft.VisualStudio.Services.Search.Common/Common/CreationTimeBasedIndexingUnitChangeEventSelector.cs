// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.CreationTimeBasedIndexingUnitChangeEventSelector
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class CreationTimeBasedIndexingUnitChangeEventSelector : IIndexingUnitChangeEventSelector
  {
    private const string TraceArea = "Indexing Pipeline";
    private const string TraceLayer = "Job";
    private LeaseFilter m_leaseFilter;

    public CreationTimeBasedIndexingUnitChangeEventSelector() => this.m_leaseFilter = new LeaseFilter();

    public IDictionary<JobPriorityLevel, List<IndexingUnitChangeEventDetails>> GetChangeEventsToBeProcessed(
      ExecutionContext executionContext,
      int indexingUnitId,
      IEnumerable<IndexingUnitChangeEventDetails> changeEvents)
    {
      if (executionContext == null)
        throw new ArgumentNullException(nameof (executionContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080522, "Indexing Pipeline", "Job", nameof (GetChangeEventsToBeProcessed));
      IDictionary<JobPriorityLevel, List<IndexingUnitChangeEventDetails>> eventsToBeProcessed = (IDictionary<JobPriorityLevel, List<IndexingUnitChangeEventDetails>>) new FriendlyDictionary<JobPriorityLevel, List<IndexingUnitChangeEventDetails>>();
      try
      {
        IEnumerable<IndexingUnitChangeEventDetails> filteredEvents = this.GetFilteredEvents(changeEvents);
        if (filteredEvents != null)
        {
          if (filteredEvents.Any<IndexingUnitChangeEventDetails>())
          {
            List<IndexingUnitChangeEventDetails> list1 = filteredEvents.OrderBy<IndexingUnitChangeEventDetails, DateTime>((Func<IndexingUnitChangeEventDetails, DateTime>) (x => x.IndexingUnitChangeEvent.CreatedTimeUtc)).ToList<IndexingUnitChangeEventDetails>();
            JobPriorityLevel priority = executionContext.RequestContext.To(TeamFoundationHostType.Deployment).GetService<PriorityManagerService>().GetPriority(executionContext.RequestContext, list1[0].IndexingUnitChangeEvent);
            List<IndexingUnitChangeEventDetails> list2 = list1.Take<IndexingUnitChangeEventDetails>(executionContext.EventProcessingContext.EventProcessingBatchSize).ToList<IndexingUnitChangeEventDetails>();
            eventsToBeProcessed.Add(priority, list2);
          }
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080522, "Indexing Pipeline", "Job", nameof (GetChangeEventsToBeProcessed));
      }
      return eventsToBeProcessed;
    }

    internal IEnumerable<IndexingUnitChangeEventDetails> GetFilteredEvents(
      IEnumerable<IndexingUnitChangeEventDetails> changeEvents)
    {
      IEnumerable<IndexingUnitChangeEventDetails> source = this.m_leaseFilter.GetEventsWithLease(changeEvents);
      if (source == null || !source.Any<IndexingUnitChangeEventDetails>())
        source = changeEvents;
      return source;
    }
  }
}
