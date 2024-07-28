// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem.ClassificationNodeSecurityHashRefreshTask
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem
{
  internal class ClassificationNodeSecurityHashRefreshTask : IIndexingPatchTask
  {
    private readonly IndexingUnit m_collectionIndexingUnit;
    private readonly IIndexingUnitChangeEventHandler m_indexingUnitChangeEventHandler;

    public string Name { get; } = nameof (ClassificationNodeSecurityHashRefreshTask);

    public ClassificationNodeSecurityHashRefreshTask(
      IndexingUnit collectionIndexingUnit,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler)
    {
      this.m_indexingUnitChangeEventHandler = indexingUnitChangeEventHandler;
      this.m_collectionIndexingUnit = collectionIndexingUnit;
    }

    public void Patch(IndexingExecutionContext executionContext, StringBuilder resultMessageBuilder)
    {
      IndexingUnitChangeEvent indexingUnitChangeEvent = new IndexingUnitChangeEvent()
      {
        IndexingUnitId = this.m_collectionIndexingUnit.IndexingUnitId,
        ChangeType = "AreaNodeSecurityAcesSync",
        ChangeData = new ChangeEventData((ExecutionContext) executionContext),
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      this.m_indexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeeded((ExecutionContext) executionContext, indexingUnitChangeEvent);
      resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Created [1] {0} event for collection. ", (object) "AreaNodeSecurityAcesSync")));
    }
  }
}
