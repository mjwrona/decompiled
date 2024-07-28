// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.FileGroupPipelineForPatch
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties.Code;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.SQL;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  internal class FileGroupPipelineForPatch : FileGroupPipeline
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetaData = new TraceMetaData(1083096, "Indexing Pipeline", "Pipeline");

    public FileGroupPipelineForPatch(CodeIndexingPipelineContext pipelineContext)
      : base(FileGroupPipelineForPatch.s_traceMetaData, nameof (FileGroupPipelineForPatch), pipelineContext)
    {
    }

    protected internal override OperationStatus PostPostRun(OperationStatus opStatus)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(this.TraceMetaData, nameof (PostPostRun));
      try
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("FileGroupPipelineForPatch triggered for {0} and Branches ({1})", (object) this.IndexingUnit, (object) string.Join(",", (IEnumerable<string>) this.Branches))));
        IndexingExecutionContext executionContext = this.PipelineContext.IndexingExecutionContext;
        if (executionContext.RepositoryName != null && executionContext.RepositoryIndexingUnit != null)
          executionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Repository Name = '{0}', Repository = '{1}'", (object) executionContext.RepositoryName, (object) executionContext.RepositoryIndexingUnit)));
        if (this.IndexingUnit.IndexingUnitType == "Git_Repository")
        {
          List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> scopedIndexingUnits = this.GetScopedIndexingUnits(executionContext);
          this.CleanUpPreviousTempFileRecords(executionContext, scopedIndexingUnits, false);
          List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
          foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in scopedIndexingUnits)
          {
            Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent(this.PipelineContext.IndexingUnitChangeEvent.LeaseId)
            {
              IndexingUnitId = indexingUnit.IndexingUnitId,
              ChangeData = (ChangeEventData) new RepositoryPatchEventData((ExecutionContext) executionContext)
              {
                Patch = Patch.RepositoryHeal
              },
              ChangeType = "Patch",
              State = IndexingUnitChangeEventState.Pending,
              AttemptCount = 0
            };
            indexingUnitChangeEventList.Add(indexingUnitChangeEvent);
          }
          if (indexingUnitChangeEventList.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>())
            this.QueueEventForScopedIndexingUnitInBatches((IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) indexingUnitChangeEventList, executionContext);
          executionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Queued {0} changeEvents for metadata crawling for heal.", (object) indexingUnitChangeEventList.Count)));
          return OperationStatus.Succeeded;
        }
        if (this.IndexingUnit.IndexingUnitType == "ScopedIndexingUnit")
        {
          CodeCrawlSpec crawlSpec = this.PipelineContext.CrawlSpec.Clone();
          this.DiffTreeCrawler = (CodeIndexingPipeline) new DiffMetadataCrawler(this.PipelineContext, crawlSpec);
          CorePipelineResult corePipelineResult = this.DiffTreeCrawler.Run();
          if (corePipelineResult.OperationStatus == OperationStatus.Failed)
            executionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Metadata Crawl Failed for {0}. ScopePath {1}", (object) this.IndexingUnit, (object) (this.IndexingUnit.TFSEntityAttributes as ScopedGitRepositoryAttributes).ScopePath)));
          else if (corePipelineResult.OperationStatus == OperationStatus.Succeeded)
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Metadata Crawl Succeeded for {0}. ScopePath {1}", (object) this.IndexingUnit, (object) (this.IndexingUnit.TFSEntityAttributes as ScopedGitRepositoryAttributes).ScopePath)));
            executionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Metadata Crawl Succeeded for {0}. ScopePath {1}", (object) this.IndexingUnit, (object) (this.IndexingUnit.TFSEntityAttributes as ScopedGitRepositoryAttributes).ScopePath)));
            long startingId;
            long endingId;
            executionContext.TempFileMetadataStoreDataAccess.GetMinAndMaxIds(executionContext.RequestContext, out startingId, out endingId);
            if (endingId >= startingId && startingId > 0L)
              this.QueueIndexingOperationsForTemporaryUnits((ExecutionContext) executionContext, this.IndexingUnit, crawlSpec.TotalItemsCrawled, startingId, endingId);
          }
          opStatus = corePipelineResult.OperationStatus;
          this.PipelineResultData = corePipelineResult.Data;
          return opStatus;
        }
        string str = FormattableString.Invariant(FormattableStringFactory.Create("FileGroupPipelineForPatch.Run() invoked with invalid {0}.", (object) this.IndexingUnit));
        executionContext.Log.Append(str);
        throw new InvalidOperationException(str);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(this.TraceMetaData, nameof (PostPostRun));
      }
    }

    internal override bool CleanUpPreviousTempFileRecords(
      IndexingExecutionContext indexingExecutionContext,
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> scopedIndexingUnits,
      bool deleteFilesWithMaxRetriesExhausted)
    {
      if (scopedIndexingUnits == null || !scopedIndexingUnits.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
        return false;
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit scopedIndexingUnit in scopedIndexingUnits)
      {
        ITempFileMetadataStoreDataAccess metadataStoreDataAccess = this.GetTempFileMetadataStoreDataAccess(scopedIndexingUnit);
        ItemLevelFailureDataAccess failureDataAccess = new ItemLevelFailureDataAccess();
        long startingId;
        long endingId;
        metadataStoreDataAccess.GetMinAndMaxIds(indexingExecutionContext.RequestContext, out startingId, out endingId);
        if (endingId >= startingId && startingId > 0L)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Moving the records ranging from {0} to {1} from TempFileMetadataStore to ItemLevelFailure table.", (object) startingId, (object) endingId)));
          int count = indexingExecutionContext.GetConfigValue<int>("/Service/ALMSearch/Settings/LargeRepositoryMaxFilesToBeIndexedInAJob");
          IEnumerable<TempFileMetadataRecord> nextBatchOfRecords;
          for (long index = startingId; index <= endingId; index = nextBatchOfRecords.Select<TempFileMetadataRecord, long>((Func<TempFileMetadataRecord, long>) (x => x.Id)).Max() + 1L)
          {
            if (index + (long) count > endingId)
              count = (int) (endingId - index) + 1;
            DocumentContractType contractType = indexingExecutionContext.ProvisioningContext.ContractType;
            string indexingUnitType = indexingExecutionContext.IndexingUnit.IndexingUnitType;
            nextBatchOfRecords = metadataStoreDataAccess.GetNextBatchOfRecords(indexingExecutionContext.RequestContext, startingId, count, indexingUnitType, contractType);
            IEnumerable<ItemLevelFailureRecord> levelFailureRecords = this.GetItemLevelFailureRecords(indexingExecutionContext, nextBatchOfRecords);
            failureDataAccess.MergeFailedRecords(indexingExecutionContext.RequestContext, scopedIndexingUnit, levelFailureRecords);
          }
          metadataStoreDataAccess.DeleteRecords(indexingExecutionContext.RequestContext, startingId, endingId);
        }
      }
      return false;
    }
  }
}
