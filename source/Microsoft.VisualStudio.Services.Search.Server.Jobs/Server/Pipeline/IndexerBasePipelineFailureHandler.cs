// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.IndexerBasePipelineFailureHandler
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  public abstract class IndexerBasePipelineFailureHandler : ICorePipelineFailureHandler
  {
    public abstract IEntityType SupportedEntityType { get; }

    public virtual int Weight => 0;

    public virtual bool HandleError(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      Exception exception)
    {
      return false;
    }

    public virtual bool IsItemLevelPersistenceSupported(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      return false;
    }

    public virtual int HandleItemLevelErrors(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      int num = 0;
      if (this.IsItemLevelPersistenceSupported(coreIndexingExecutionContext))
      {
        FailureRecordStore failureRecordStore = ((IndexingExecutionContext) coreIndexingExecutionContext).FailureRecordStore;
        num = failureRecordStore.GetFailedRecords().Count<ItemLevelFailureRecord>();
        failureRecordStore.PersistFailedRecords();
        coreIndexingExecutionContext.ExecutionTracerContext.PublishKpi("TotalNumberOfItemsFailedToIndex", "Indexing Pipeline", (double) num, true);
        coreIndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Number of documents not indexed = {0}.", (object) num)));
      }
      return num;
    }
  }
}
