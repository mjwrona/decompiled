// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.CollectionCodeDuplicateDocumentsDeletionOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class CollectionCodeDuplicateDocumentsDeletionOperation : AbstractIndexingOperation
  {
    public CollectionCodeDuplicateDocumentsDeletionOperation(
      ExecutionContext executionContext,
      IndexingUnit indexingUnit,
      IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      OperationResult operationResult = new OperationResult();
      List<IndexingUnit> indexingUnitList = new List<IndexingUnit>();
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      List<IndexingUnit> indexingUnits1 = executionContext.IndexingUnitDataAccess.GetIndexingUnits(executionContext.RequestContext, "Git_Repository", (IEntityType) CodeEntityType.GetInstance(), -1);
      if (indexingUnits1 != null)
        indexingUnitList.AddRange((IEnumerable<IndexingUnit>) indexingUnits1);
      List<IndexingUnit> indexingUnits2 = executionContext.IndexingUnitDataAccess.GetIndexingUnits(executionContext.RequestContext, "TFVC_Repository", (IEntityType) CodeEntityType.GetInstance(), -1);
      if (indexingUnits2 != null)
        indexingUnitList.AddRange((IEnumerable<IndexingUnit>) indexingUnits2);
      List<IndexingUnit> indexingUnits3 = executionContext.IndexingUnitDataAccess.GetIndexingUnits(executionContext.RequestContext, "CustomRepository", (IEntityType) CodeEntityType.GetInstance(), -1);
      if (indexingUnits3 != null)
        indexingUnitList.AddRange((IEnumerable<IndexingUnit>) indexingUnits3);
      if (indexingUnitList.Count > 0)
      {
        List<IndexingUnitChangeEvent> indexingUnitChangeEventList = new List<IndexingUnitChangeEvent>(indexingUnitList.Count);
        foreach (IndexingUnit indexingUnit in indexingUnitList)
        {
          IndexingUnitChangeEvent indexingUnitChangeEvent = new IndexingUnitChangeEvent()
          {
            IndexingUnitId = indexingUnit.IndexingUnitId,
            ChangeType = "DeleteDuplicateDocuments",
            ChangeData = new ChangeEventData((ExecutionContext) executionContext),
            State = IndexingUnitChangeEventState.Pending,
            AttemptCount = 0
          };
          indexingUnitChangeEventList.Add(indexingUnitChangeEvent);
        }
        this.IndexingUnitChangeEventHandler.HandleEvents((ExecutionContext) executionContext, (IList<IndexingUnitChangeEvent>) indexingUnitChangeEventList);
        executionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Added {0} events to delete duplicate documents.", (object) indexingUnitChangeEventList.Count)));
      }
      operationResult.Status = OperationStatus.Succeeded;
      return operationResult;
    }
  }
}
