// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.TfvcChangesetIndexOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class TfvcChangesetIndexOperation : RepositoryCodeIndexingOperation
  {
    public TfvcChangesetIndexOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : this(executionContext, indexingUnit, indexingUnitChangeEvent, new RegistryManager(executionContext.RequestContext, "IndexingOperation"))
    {
    }

    public TfvcChangesetIndexOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      RegistryManager registryManager)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent, new TraceMetaData(1080614, "Indexing Pipeline", "IndexingOperation"))
    {
      this.RegistryManager = registryManager;
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Tracer.TraceEnter(this.TraceMetaData.TracePoint, this.TraceMetaData.TraceArea, this.TraceMetaData.TraceLayer, nameof (RunOperation));
      Stopwatch.StartNew();
      OperationResult operationResult = new OperationResult();
      IndexingExecutionContext iexContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        if (coreIndexingExecutionContext.RequestContext.IsContinuousIndexingEnabled(this.IndexingUnit.EntityType))
        {
          if (!this.IsBulkIndexingCompleteForRepo())
          {
            string str1 = FormattableString.Invariant(FormattableStringFactory.Create("Bulk Indexing is not completed for TFVC Repository with {0}.", (object) this.IndexingUnit.TFSEntityId));
            coreIndexingExecutionContext.Log.Append(str1);
            Tracer.TraceInfo(this.TraceMetaData.TracePoint, this.TraceMetaData.TraceArea, this.TraceMetaData.TraceLayer, str1);
            Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent(this.IndexingUnitChangeEvent.LeaseId);
            indexingUnitChangeEvent1.IndexingUnitId = this.IndexingUnit.IndexingUnitId;
            CodeBulkIndexEventData bulkIndexEventData = new CodeBulkIndexEventData((ExecutionContext) coreIndexingExecutionContext);
            bulkIndexEventData.Delay = TimeSpan.FromMinutes((double) coreIndexingExecutionContext.ServiceSettings.JobSettings.TfvcBIEventDelayInMinutes);
            indexingUnitChangeEvent1.ChangeData = (ChangeEventData) bulkIndexEventData;
            indexingUnitChangeEvent1.ChangeType = "BeginBulkIndex";
            indexingUnitChangeEvent1.State = IndexingUnitChangeEventState.Pending;
            indexingUnitChangeEvent1.AttemptCount = (byte) 0;
            Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent2 = indexingUnitChangeEvent1;
            Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent3 = this.IndexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeeded((ExecutionContext) coreIndexingExecutionContext, indexingUnitChangeEvent2);
            CodeQueryScopingCacheUtil.SqlNotifyForRepoAddition(this.DataAccessFactory, coreIndexingExecutionContext.RequestContext, this.IndexingUnit);
            string str2 = FormattableString.Invariant(FormattableStringFactory.Create("Created BulkIndexing IndexingUnitChangeEvent  [Id = {0} IndexingUnitId = {1} ChangeType {2}]", (object) indexingUnitChangeEvent3.Id, (object) indexingUnitChangeEvent3.IndexingUnitId, (object) indexingUnitChangeEvent3.ChangeType));
            coreIndexingExecutionContext.Log.Append(str2);
            Tracer.TraceInfo(this.TraceMetaData.TracePoint, this.TraceMetaData.TraceArea, this.TraceMetaData.TraceLayer, str2);
          }
          else if (OperationStatus.PartiallySucceeded == this.ExecuteCrawlerParserAndFeeder(iexContext, string.Empty, new List<string>()).OperationStatus)
            iexContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Partially indexed TFVC Repository Id {0}", (object) this.IndexingUnit.TFSEntityId)));
          else
            iexContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully indexed TFVC Repository Id {0}", (object) this.IndexingUnit.TFSEntityId)));
        }
        else
          coreIndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Feature flag '{0}' or '{1}' is disabled for this account.", (object) "Search.Server.Code.CrudOperations", (object) "Search.Server.Code.Indexing")));
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        Tracer.TraceLeave(this.TraceMetaData.TracePoint, this.TraceMetaData.TraceArea, this.TraceMetaData.TraceLayer, nameof (RunOperation));
      }
      return operationResult;
    }

    protected internal override TimeSpan GetChangeEventDelay(
      CoreIndexingExecutionContext executionContext,
      Exception e)
    {
      return e != null && IndexFaultMapManager.GetFaultMapper(typeof (VssTimeOutFaultMapper)).IsMatch(e) ? TimeSpan.FromSeconds(executionContext.RequestContext.GetCurrentHostConfigValue<double>("/Service/ALMSearch/Settings/ChangeEventDelayForThrottlingFailureInSeconds", true, 900.0)) : base.GetChangeEventDelay(executionContext, e);
    }

    internal override CodeCrawlSpec CreateCrawlSpec(
      IndexingExecutionContext iexContext,
      ref string branchName,
      in List<string> branches)
    {
      TfvcCodeRepoIndexingProperties properties = this.IndexingUnit.Properties as TfvcCodeRepoIndexingProperties;
      return (CodeCrawlSpec) ContinuousIndexTfvcCrawlSpec.Create(iexContext, iexContext.ProjectId.Value, properties.LastIndexedChangeSetId);
    }

    internal override CodeIndexingPipelineContext GetPipelineContext(
      IndexingExecutionContext iexContext,
      string branchName,
      List<string> branches)
    {
      ((TfvcCodeRepoIndexingProperties) this.IndexingUnit.Properties).TfvcIndexJobYieldData = new TfvcIndexJobYieldData();
      CodeCrawlSpec crawlSpec = this.CreateCrawlSpec(iexContext, ref branchName, in branches);
      return new CodeIndexingPipelineContext(iexContext.IndexingUnit, iexContext, crawlSpec, this.IndexingUnitChangeEvent, this.IndexingUnitChangeEventHandler, branchName, branches, false, false);
    }

    internal override CodeIndexingPipeline GetPipeline(CodeIndexingPipelineContext pipelineContext) => this.WorkerPipeline = (CodeIndexingPipeline) new TfvcRepositoryContinuousIndexingPipeline(pipelineContext);

    private bool IsBulkIndexingCompleteForRepo() => (this.IndexingUnit.Properties as TfvcCodeRepoIndexingProperties).LastIndexedChangeSetId > 0;
  }
}
