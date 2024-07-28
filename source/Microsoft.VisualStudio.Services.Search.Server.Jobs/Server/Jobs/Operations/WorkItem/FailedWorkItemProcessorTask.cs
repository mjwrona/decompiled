// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem.FailedWorkItemProcessorTask
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem
{
  internal class FailedWorkItemProcessorTask : IIndexingPatchTask
  {
    private readonly IIndexingUnitDataAccess m_indexingUnitDataAccess;
    private readonly IIndexingUnitChangeEventHandler m_indexingUnitChangeEventHandler;

    public string Name { get; } = nameof (FailedWorkItemProcessorTask);

    internal FailedWorkItemProcessorTask(
      IDataAccessFactory dataAccessFactory,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler)
    {
      this.m_indexingUnitDataAccess = dataAccessFactory.GetIndexingUnitDataAccess();
      this.m_indexingUnitChangeEventHandler = indexingUnitChangeEventHandler;
    }

    public void Patch(
      IndexingExecutionContext indexingExecutionContext,
      StringBuilder resultMessageBuilder)
    {
      int itemsMaxRetryCount = indexingExecutionContext.ServiceSettings.JobSettings.FailedItemsMaxRetryCount;
      IDictionary<int, int> recordsByIndexingUnit = indexingExecutionContext.ItemLevelFailureDataAccess.GetCountOfRecordsByIndexingUnit(indexingExecutionContext.RequestContext, (IEntityType) WorkItemEntityType.GetInstance(), itemsMaxRetryCount, this.m_indexingUnitDataAccess);
      List<IndexingUnitChangeEvent> indexingUnitChangeEventList = new List<IndexingUnitChangeEvent>();
      foreach (KeyValuePair<int, int> keyValuePair in (IEnumerable<KeyValuePair<int, int>>) recordsByIndexingUnit)
      {
        int key = keyValuePair.Key;
        if (keyValuePair.Value > 0)
        {
          IndexingUnit indexingUnit = this.m_indexingUnitDataAccess.GetIndexingUnit(indexingExecutionContext.RequestContext, key);
          if (indexingUnit != null && indexingUnit.EntityType.Name == "WorkItem" && indexingUnit.IndexingUnitType == "Project")
            indexingUnitChangeEventList.Add(FailedWorkItemProcessorTask.CreateFailedItemsPatchEvent((ExecutionContext) indexingExecutionContext, key));
        }
      }
      if (indexingUnitChangeEventList.Count <= 0)
        return;
      this.m_indexingUnitChangeEventHandler.HandleEvents((ExecutionContext) indexingExecutionContext, (IList<IndexingUnitChangeEvent>) indexingUnitChangeEventList);
    }

    private static IndexingUnitChangeEvent CreateFailedItemsPatchEvent(
      ExecutionContext executionContext,
      int indexingUnitId)
    {
      return new IndexingUnitChangeEvent()
      {
        IndexingUnitId = indexingUnitId,
        ChangeData = (ChangeEventData) new WorkItemPatchEventData(executionContext),
        ChangeType = "Patch",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
    }
  }
}
