// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.EventProcessingContext
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class EventProcessingContext
  {
    public EventProcessingContext(IVssRequestContext requestContext)
      : this(requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/BatchCountForProcessingEvents"), requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/BatchCountForQueryingEvents"), (IIndexingUnitChangeEventSelector) new PriorityBasedIndexingUnitChangeEventSelector())
    {
    }

    public EventProcessingContext(
      int eventProcessingBatchSize,
      int eventsQueryBatchSize,
      IIndexingUnitChangeEventSelector indexingUnitChangeEventSelector)
    {
      this.EventProcessingBatchSize = eventProcessingBatchSize;
      this.EventsQueryBatchSize = eventsQueryBatchSize;
      this.IndexingUnitChangeEventSelector = indexingUnitChangeEventSelector;
    }

    public int EventProcessingBatchSize { get; set; }

    public int EventsQueryBatchSize { get; set; }

    public IIndexingUnitChangeEventSelector IndexingUnitChangeEventSelector { get; set; }
  }
}
