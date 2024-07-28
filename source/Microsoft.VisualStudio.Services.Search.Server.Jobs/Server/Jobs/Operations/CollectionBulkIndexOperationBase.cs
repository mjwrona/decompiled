// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.CollectionBulkIndexOperationBase
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations
{
  internal abstract class CollectionBulkIndexOperationBase : AbstractIndexingOperation
  {
    private IList<IndexInfo> m_currentIndexIndices;
    private IList<IndexInfo> m_currentQueryIndices;
    private IList<IndexInfo> m_indicesToClean;
    protected readonly int Tracepoint;

    protected CollectionIndexingProperties CollectionIndexingProperties => this.IndexingUnit.Properties as CollectionIndexingProperties;

    protected internal IEnumerable<IndexInfo> GetIndicesToClean()
    {
      if (this.m_indicesToClean == null)
      {
        this.m_indicesToClean = (IList<IndexInfo>) this.GetCurrentIndexIndices().ToList<IndexInfo>();
        IEnumerable<IndexInfo> currentQueryIndices = this.GetCurrentQueryIndices();
        if (currentQueryIndices != null && currentQueryIndices.Any<IndexInfo>())
        {
          IEnumerable<string> currentQueryIndicesNames = currentQueryIndices.Select<IndexInfo, string>((Func<IndexInfo, string>) (d => d.IndexName));
          this.m_indicesToClean = (IList<IndexInfo>) this.m_indicesToClean.Where<IndexInfo>((Func<IndexInfo, bool>) (o => !currentQueryIndicesNames.Contains<string>(o.IndexName))).ToList<IndexInfo>();
        }
      }
      return (IEnumerable<IndexInfo>) this.m_indicesToClean;
    }

    protected internal IEnumerable<IndexInfo> GetCurrentIndexIndices()
    {
      if (this.m_currentIndexIndices == null)
      {
        this.m_currentIndexIndices = (IList<IndexInfo>) new List<IndexInfo>();
        if (this.IndexingUnit.Properties.IndexIndices != null && this.IndexingUnit.Properties.IndexIndices.Any<IndexInfo>())
          this.m_currentIndexIndices = (IList<IndexInfo>) this.IndexingUnit.Properties.IndexIndices;
      }
      return (IEnumerable<IndexInfo>) this.m_currentIndexIndices;
    }

    protected internal IEnumerable<IndexInfo> GetCurrentQueryIndices()
    {
      if (this.m_currentQueryIndices == null)
      {
        this.m_currentQueryIndices = (IList<IndexInfo>) new List<IndexInfo>();
        if (this.IndexingUnit.Properties.QueryIndices != null && this.IndexingUnit.Properties.QueryIndices.Any<IndexInfo>())
          this.m_currentQueryIndices = (IList<IndexInfo>) this.IndexingUnit.Properties.QueryIndices;
      }
      return (IEnumerable<IndexInfo>) this.m_currentQueryIndices;
    }

    private void CleanupStaleData(IndexingExecutionContext executionContext)
    {
      if (executionContext.IsCrossClusterReindexingInProgress(this.CollectionIndexingProperties))
        return;
      this.FinalizeHelper.DeleteDataFromCurrentIndices(executionContext, this.GetIndicesToClean());
    }

    public CollectionBulkIndexOperationBase(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      int tracepoint)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
      this.Tracepoint = tracepoint;
    }

    protected abstract EntityFinalizerBase FinalizeHelper { get; }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Tracer.TraceEnter(this.Tracepoint, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder resultMessage = new StringBuilder();
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        this.CleanupStaleData(executionContext);
        if (this.CanFinalize(executionContext))
        {
          this.FinalizeHelper.FinalizeQueryProperties(executionContext);
          this.FinalizeHelper.UpdateFeatureFlagsIfNeeded(executionContext);
          this.FinalizeHelper.NotifyIndexPropertiesUpdates(executionContext);
          resultMessage.Append("Updated indexing unit properties to make entity searchable as soon as they are indexed. ");
        }
        Tracer.TraceInfo(this.Tracepoint, "Indexing Pipeline", "IndexingOperation", FormattableString.Invariant(FormattableStringFactory.Create("Erasing Indexing watermarks for {0}", (object) this.IndexingUnit)));
        this.IndexingUnit.EraseIndexingWatermarksOfTree(executionContext, this.IndexingUnitDataAccess, IndexingExecutionContextExtensions.IsShadowIndexingRequired(executionContext, this.IndexingUnitChangeEvent));
        IndexingUnitChangeEventPrerequisites indexPublishPreReq = new IndexingUnitChangeEventPrerequisites();
        IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> operationForSubEntities = this.CreateBulkIndexOperationForSubEntities(executionContext, resultMessage);
        indexPublishPreReq.AddRange((IEnumerable<IndexingUnitChangeEventPrerequisitesFilter>) operationForSubEntities.Select<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent, IndexingUnitChangeEventPrerequisitesFilter>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent, IndexingUnitChangeEventPrerequisitesFilter>) (bulkIndexEvent => new IndexingUnitChangeEventPrerequisitesFilter()
        {
          Id = bulkIndexEvent.Id,
          Operator = IndexingUnitChangeEventFilterOperator.Contains,
          PossibleStates = new List<IndexingUnitChangeEventState>()
          {
            IndexingUnitChangeEventState.Succeeded,
            IndexingUnitChangeEventState.Failed
          }
        })).ToList<IndexingUnitChangeEventPrerequisitesFilter>());
        this.CreateIndexPublishEvent(executionContext, indexPublishPreReq);
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        operationResult.Message = resultMessage.ToString();
        Tracer.TraceLeave(this.Tracepoint, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
      return operationResult;
    }

    protected internal override void HandleOperationFailure(
      IndexingExecutionContext indexingExecutionContext,
      OperationResult result,
      Exception e)
    {
      result.Message = e.ToString();
      result.Status = (int) this.IndexingUnitChangeEvent.AttemptCount < this.m_maxIndexRetryCount ? OperationStatus.FailedAndRetry : OperationStatus.Failed;
    }

    internal abstract IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> CreateBulkIndexOperationForSubEntities(
      IndexingExecutionContext indexingExecutionContext,
      StringBuilder resultMessage);

    internal abstract void CreateIndexPublishEvent(
      IndexingExecutionContext indexingExecutionContext,
      IndexingUnitChangeEventPrerequisites indexPublishPreReq);

    internal abstract bool CanFinalize(IndexingExecutionContext indexingExecutionContext);
  }
}
