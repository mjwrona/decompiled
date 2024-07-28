// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.TfvcRepositoryIndexingPipeline
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  internal class TfvcRepositoryIndexingPipeline : CodeIndexingPipeline
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetaData = new TraceMetaData(1083092, "Indexing Pipeline", "Pipeline");

    internal TfvcRepositoryIndexingPipeline(CodeIndexingPipelineContext pipelineContext)
      : base(TfvcRepositoryIndexingPipeline.s_traceMetaData, nameof (TfvcRepositoryIndexingPipeline), pipelineContext)
    {
    }

    protected internal override Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent QueueContinuationOperation()
    {
      ChangeEventData changeEventData1;
      if (!(this.PipelineContext.IndexingUnitChangeEvent.ChangeType == "BeginBulkIndex"))
        changeEventData1 = (ChangeEventData) new TFVCCodeContinuousIndexEventData((ExecutionContext) this.PipelineContext.IndexingExecutionContext)
        {
          ChangesetId = ((TFVCCodeContinuousIndexEventData) this.PipelineContext.IndexingUnitChangeEvent.ChangeData).ChangesetId
        };
      else
        changeEventData1 = (ChangeEventData) new CodeBulkIndexEventData((ExecutionContext) this.PipelineContext.IndexingExecutionContext);
      ChangeEventData changeEventData2 = changeEventData1;
      return this.PipelineContext.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) this.PipelineContext.IndexingExecutionContext, new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent(this.PipelineContext.IndexingUnitChangeEvent.LeaseId)
      {
        IndexingUnitId = this.IndexingUnit.IndexingUnitId,
        ChangeType = this.PipelineContext.IndexingUnitChangeEvent.ChangeType,
        ChangeData = changeEventData2,
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = (byte) 0
      });
    }

    internal override bool IsPrimaryRun()
    {
      TfvcCodeRepoIndexingProperties properties = this.IndexingUnit.Properties as TfvcCodeRepoIndexingProperties;
      TfvcCodeRepoTFSAttributes entityAttributes = this.IndexingUnit.TFSEntityAttributes as TfvcCodeRepoTFSAttributes;
      return properties == null || entityAttributes == null || properties.TfvcIndexJobYieldData == null || !properties.TfvcIndexJobYieldData.HasData();
    }

    internal override void HandlePipelineError(Exception ex)
    {
      IndexingExecutionContext executionContext = this.PipelineContext.IndexingExecutionContext;
      TfvcCodeRepoIndexingProperties properties = (TfvcCodeRepoIndexingProperties) executionContext.IndexingUnit.Properties;
      bool flag = false;
      if (this.IsYieldDataCleanupRequired(ex, properties))
      {
        properties.TfvcIndexJobYieldData = new TfvcIndexJobYieldData();
        properties.TfvcIndexJobYieldStats = new TfvcIndexJobYieldStats();
        flag = true;
      }
      else if (!properties.TfvcIndexJobYieldData.HasData())
      {
        string currentChangeId = this.PipelineContext.CrawlSpec.CurrentChangeId;
        if (!currentChangeId.Equals(properties.TfvcIndexJobYieldData.LastAttemptedTargetChangesetId))
        {
          properties.TfvcIndexJobYieldData.LastAttemptedTargetChangesetId = currentChangeId;
          flag = true;
        }
      }
      if (!flag)
        return;
      executionContext.IndexingUnit = executionContext.IndexingUnitDataAccess.UpdateIndexingUnit(executionContext.RequestContext, executionContext.IndexingUnit);
      this.IndexingUnit = executionContext.IndexingUnit;
    }

    internal virtual bool IsYieldDataCleanupRequired(
      Exception ex,
      TfvcCodeRepoIndexingProperties repoProperties)
    {
      return ex != null && IndexFaultMapManager.GetFaultMapper(typeof (ContinuationTokenInvalidFaultMapper)).IsMatch(ex) && repoProperties.TfvcIndexJobYieldData.HasData();
    }

    protected internal override bool AllDocumentsAreProcessed()
    {
      if (!(((CoreCrawlSpec) this.PipelineContext.CrawlSpec).JobYieldData is TfvcIndexJobYieldData jobYieldData))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Tfvc Job Yield data: {0} is not of type {1}", (object) ((CoreCrawlSpec) this.PipelineContext.CrawlSpec).JobYieldData, (object) typeof (TfvcIndexJobYieldData).FullName)));
      return !jobYieldData.HasData();
    }
  }
}
