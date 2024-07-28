// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.BasicRoutingAssignmentOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders;
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
  internal abstract class BasicRoutingAssignmentOperation : AbstractIndexingOperation
  {
    private readonly TraceMetaData m_traceMetaData;

    protected BasicRoutingAssignmentOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
      this.m_traceMetaData = new TraceMetaData(1080292, "Indexing Pipeline", "IndexingOperation");
    }

    protected abstract EntityFinalizerBase FinalizeHelper { get; }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = coreIndexingExecutionContext.IndexingUnit;
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      OperationResult operationResult = new OperationResult();
      StringBuilder resultMessage = new StringBuilder();
      if (indexingUnit.Properties.IsDisabled)
      {
        resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Indexing is disabled, not assigning any routing values.")));
        operationResult.Status = OperationStatus.Succeeded;
        operationResult.Message = resultMessage.ToString();
        return operationResult;
      }
      if (indexingUnit.Properties.IndexIndices != null && indexingUnit.Properties.IndexIndices.Any<IndexInfo>() && !this.ShouldReassignRouting(executionContext, indexingUnit))
      {
        resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("No need to assign routing. The Indexing Unit {0} already has the routings assigned.", (object) indexingUnit)));
        operationResult.Status = OperationStatus.Succeeded;
        operationResult.Message = resultMessage.ToString();
        return operationResult;
      }
      try
      {
        int num = coreIndexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/Routing/SizeBasedCustomRoutingEnabled", true) ? 1 : 0;
        bool flag1 = false;
        bool flag2 = false;
        bool currentHostConfigValue = coreIndexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/Routing/AssignRoutingToEmptyArtifacts", true);
        if (num != 0)
        {
          IndexingUnitWithSize withSizeEstimates = this.GetIndexingUnitWithSizeEstimates(executionContext);
          if (withSizeEstimates != null)
          {
            if (currentHostConfigValue)
              flag1 = this.AssignRouting(executionContext, withSizeEstimates, resultMessage);
            else if (this.CanAssignRouting(withSizeEstimates))
            {
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Size Estimates available now, trying to assign Routing values.")));
              flag1 = this.AssignRouting(executionContext, withSizeEstimates, resultMessage);
            }
            else
            {
              if (this.ShouldRetryToGetSizeEstimates(executionContext))
              {
                resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Size Estimates not available or the possibly empty artifact. Current Attempt Count {0}. ", (object) this.IndexingUnitChangeEvent.AttemptCount)) + FormattableString.Invariant(FormattableStringFactory.Create("Next attempt to get sizes will be made after {0} secs.", (object) this.GetChangeEventDelay(coreIndexingExecutionContext, (Exception) null).TotalSeconds)));
                operationResult.Status = OperationStatus.FailedAndRetry;
                return operationResult;
              }
              resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Failed to get Size Estimates. Attempt Count {0}. Total Time Taken = {1} secs. ", (object) this.IndexingUnitChangeEvent.AttemptCount, (object) DateTime.UtcNow.Subtract(this.IndexingUnitChangeEvent.CreatedTimeUtc).TotalSeconds)) + FormattableString.Invariant(FormattableStringFactory.Create("Attempting to get Fallback size estimates. ")));
              flag2 = true;
              IndexingUnitWithSize fallbackSizeEstimates = this.GetIndexingUnitWithFallbackSizeEstimates(executionContext);
              if (fallbackSizeEstimates != null)
              {
                resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Fallback size estimates available.")));
                flag1 = this.AssignRouting(executionContext, fallbackSizeEstimates, resultMessage);
              }
              else
                resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Fallback size not available, skipping this indexing unit for shard allocation.")));
            }
          }
          else
          {
            resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Null value of IndexingUnitWithSize. Failing routing assignment operation.")));
            operationResult.Status = OperationStatus.Failed;
            return operationResult;
          }
        }
        else
        {
          indexingUnit.SetupIndexRouting(executionContext);
          indexingUnit = this.IndexingUnitDataAccess.UpdateIndexingUnit(coreIndexingExecutionContext.RequestContext, indexingUnit);
          flag1 = true;
          resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully assigned Routing to {0}.", (object) indexingUnit)));
        }
        if (flag1)
          this.PostRun(coreIndexingExecutionContext, indexingUnit);
        IDictionary<string, object> properties = (IDictionary<string, object>) new Dictionary<string, object>();
        properties.Add("TotalTimeTakenForRoutingAssignmentInSeconds", (object) DateTime.UtcNow.Subtract(this.IndexingUnitChangeEvent.CreatedTimeUtc).TotalSeconds);
        properties.Add("TotalAttemptsForRoutingAssignment", (object) this.IndexingUnitChangeEvent.AttemptCount);
        properties.Add("FallbackSizeEstimationsUsedForShardAllocation", (object) flag2.ToString());
        properties.Add("ShardAllocationComplete", (object) flag1.ToString());
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("Indexing Pipeline", "IndexingOperation", properties, true);
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        operationResult.Message += resultMessage.ToString();
      }
      return operationResult;
    }

    internal virtual bool AssignRouting(
      IndexingExecutionContext indexingExecutionContext,
      IndexingUnitWithSize indexingUnitWithSize,
      StringBuilder resultMessage)
    {
      List<IndexInfo> indexIndices = indexingExecutionContext.CollectionIndexingUnit.Properties.IndexIndices;
      // ISSUE: explicit non-virtual call
      int num = indexIndices != null ? (__nonvirtual (indexIndices.Count) > 0 ? 1 : 0) : 0;
      IRoutingService service = indexingExecutionContext.RequestContext.GetService<IRoutingService>();
      bool currentHostConfigValue = indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/Routing/SizeBasedIndexProvisioningEnabled", true);
      if (num != 0)
      {
        List<ShardAssignmentDetails> source = service.AssignShards(indexingExecutionContext, new List<IndexingUnitWithSize>()
        {
          indexingUnitWithSize
        });
        resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully assigned shards : ({0}) to {1}.", (object) string.Join<int>(", ", source.Select<ShardAssignmentDetails, int>((Func<ShardAssignmentDetails, int>) (x => x.ShardId))), (object) indexingUnitWithSize.IndexingUnit)));
        return true;
      }
      if (!currentHostConfigValue)
        throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("SizeBasedIndexProvisioning is disabled, AccountFaultInJob should have assigned the index.")));
      resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Collection Indexing Unit doesn't have any index assigned, possibly CrawlMetadata operation hasn't run yet. Returning without assigning any routing values.")));
      return false;
    }

    internal virtual void PostRun(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      new IndexMetadataStateAnalyserFactory().GetIndexMetadataStateAnalyser(this.DataAccessFactory, this.IndexingUnitChangeEventHandler, coreIndexingExecutionContext.IndexingUnit.EntityType).CreateBulkIndexEventForRepository((IndexingExecutionContext) coreIndexingExecutionContext, coreIndexingExecutionContext.CollectionIndexingUnit, coreIndexingExecutionContext.IndexingUnit);
    }

    internal virtual IndexingUnitWithSize GetIndexingUnitWithSizeEstimates(
      IndexingExecutionContext indexingExecutionContext)
    {
      return new IndexingUnitWithSize(indexingExecutionContext.IndexingUnit, 0, 0, true);
    }

    internal virtual IndexingUnitWithSize GetIndexingUnitWithFallbackSizeEstimates(
      IndexingExecutionContext indexingExecutionContext)
    {
      return new IndexingUnitWithSize(indexingExecutionContext.IndexingUnit, 0, 0, true);
    }

    internal virtual bool CanAssignRouting(IndexingUnitWithSize indexingUnitWithSize) => true;

    internal virtual bool ShouldReassignRouting(
      IndexingExecutionContext indexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      return false;
    }

    protected internal virtual bool ShouldRetryToGetSizeEstimates(
      IndexingExecutionContext indexingExecutionContext)
    {
      return (int) this.IndexingUnitChangeEvent.AttemptCount < this.GetMaxIndexingRetryCount((ExecutionContext) indexingExecutionContext);
    }

    protected override int GetMaxIndexingRetryCount(ExecutionContext executionContext) => executionContext.RequestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/Routing/MaxRetriesToAssignRouting", true, 1);

    protected internal override TimeSpan GetChangeEventDelay(
      CoreIndexingExecutionContext indexingExecutionContext,
      Exception e)
    {
      return e == null ? TimeSpan.FromSeconds((double) (indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/Routing/RoutingAssignmentOperationDelayInSecs", true, 300) * (int) this.IndexingUnitChangeEvent.AttemptCount)) : base.GetChangeEventDelay(indexingExecutionContext, e);
    }
  }
}
