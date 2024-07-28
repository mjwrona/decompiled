// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.AbstractIndexingPatchOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  internal abstract class AbstractIndexingPatchOperation : AbstractIndexingOperation
  {
    internal AbstractIndexingPatchOperation(
      ExecutionContext executionContext,
      IndexingUnit indexingUnit,
      IndexingUnitChangeEvent indexingUnitChangeEvent,
      IDataAccessFactory dataAccessFactory)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent, dataAccessFactory)
    {
    }

    protected void ExecutePatchTasks(
      IndexingExecutionContext executionContext,
      IEnumerable<IIndexingPatchTask> patchTasks,
      StringBuilder resultMessageBuilder)
    {
      List<Exception> innerExceptions = new List<Exception>();
      foreach (IIndexingPatchTask patchTask in patchTasks)
      {
        try
        {
          patchTask.Patch(executionContext, resultMessageBuilder);
        }
        catch (Exception ex)
        {
          innerExceptions.Add((Exception) new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Failed to execute patch task [{0}].", (object) patchTask.Name)), ex));
        }
      }
      if (innerExceptions.Count > 0)
        throw new AggregateException((IEnumerable<Exception>) innerExceptions);
    }

    internal virtual void CreatePatchEventsForChildIndexingUnits(
      ExecutionContext executionContext,
      int patchBatchSize,
      int staggerTimeInterval,
      StringBuilder resultMessageBuilder)
    {
      List<IndexingUnit> childIndexingUnits = this.GetChildIndexingUnits(executionContext);
      if (childIndexingUnits != null && childIndexingUnits.Count > 0)
      {
        IList<IndexingUnitChangeEvent> indexingUnitChangeEventList1 = (IList<IndexingUnitChangeEvent>) new List<IndexingUnitChangeEvent>();
        for (int index = 0; index < childIndexingUnits.Count; ++index)
        {
          IndexingUnit indexingUnit = childIndexingUnits[index];
          IndexingUnitChangeEvent indexingUnitChangeEvent = new IndexingUnitChangeEvent()
          {
            IndexingUnitId = indexingUnit.IndexingUnitId,
            ChangeType = "Patch",
            ChangeData = new ChangeEventData(executionContext)
            {
              Delay = TimeSpan.FromSeconds((double) (index / patchBatchSize * staggerTimeInterval))
            },
            State = IndexingUnitChangeEventState.Pending,
            AttemptCount = 0
          };
          indexingUnitChangeEventList1.Add(indexingUnitChangeEvent);
        }
        IList<IndexingUnitChangeEvent> indexingUnitChangeEventList2 = this.IndexingUnitChangeEventHandler.HandleEvents(executionContext, indexingUnitChangeEventList1);
        resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Created [{0}] {1} events for child indexing units of [{2}].", (object) indexingUnitChangeEventList2.Count, (object) "Patch", (object) this.IndexingUnit.ToString())));
      }
      else
        resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Did not find child indexing units of [{0}].", (object) this.IndexingUnit.ToString())));
    }

    internal virtual List<IndexingUnit> GetChildIndexingUnits(ExecutionContext executionContext) => this.IndexingUnitDataAccess.GetIndexingUnitsWithGivenParent(executionContext.RequestContext, this.IndexingUnit.IndexingUnitId, -1);
  }
}
