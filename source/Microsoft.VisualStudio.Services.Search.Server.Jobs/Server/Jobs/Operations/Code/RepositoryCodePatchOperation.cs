// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.RepositoryCodePatchOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class RepositoryCodePatchOperation : RepositoryCodeIndexingOperation
  {
    public RepositoryCodePatchOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent, new TraceMetaData(1080622, "Indexing Pipeline", "IndexingOperation"))
    {
    }

    protected IEnumerable<PatchDescription> Patches { get; set; }

    protected Patch Patch { get; set; }

    protected string LastIndexedChangeId { get; set; }

    protected DateTime LastIndexedChangeTime { get; set; }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(this.TraceMetaData.TracePoint, this.TraceMetaData.TraceArea, this.TraceMetaData.TraceLayer, nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        if (executionContext.IsIndexingEnabled() && executionContext.IsCrudOperationsFeatureEnabled())
        {
          if (this.IndexingUnitChangeEvent.ChangeData is RepositoryPatchEventData changeData)
          {
            this.Patch = changeData.Patch;
            IIndexPatchProvider indexPatchProvider = this.GetIndexPatchProvider(executionContext, this.Patch, this.TraceMetaData);
            if (indexPatchProvider == null)
            {
              string str = FormattableString.Invariant(FormattableStringFactory.Create("Could not get Patch Class for the patch {0}", (object) this.Patch));
              coreIndexingExecutionContext.Log.Append(str);
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TraceMetaData.TracePoint, this.TraceMetaData.TraceArea, this.TraceMetaData.TraceLayer, str);
              operationResult.Status = OperationStatus.Failed;
            }
            else if (this.Patch == Patch.ReIndexCppFilesUsingFailedItems)
            {
              int num = 0;
              List<string> files = new List<string>();
              foreach (PatchDescription patch in indexPatchProvider.GetPatches(executionContext, string.Empty, (CodeCrawlSpec) null, this.IndexingUnit))
              {
                files.Add(patch.FilePath);
                if (files.Count == executionContext.ServiceSettings.JobSettings.PatchInMemoryThresholdForMaxDocs)
                {
                  List<ItemLevelFailureRecord> failedItemRecords = this.ConvertToFailedItemRecords(files);
                  executionContext.ItemLevelFailureDataAccess.MergeFailedRecords(executionContext.RequestContext, executionContext.IndexingUnit, (IEnumerable<ItemLevelFailureRecord>) failedItemRecords);
                  num += failedItemRecords.Count;
                  files = new List<string>();
                }
              }
              if (files.Count > 0)
              {
                List<ItemLevelFailureRecord> failedItemRecords = this.ConvertToFailedItemRecords(files);
                num += failedItemRecords.Count;
                executionContext.ItemLevelFailureDataAccess.MergeFailedRecords(executionContext.RequestContext, executionContext.IndexingUnit, (IEnumerable<ItemLevelFailureRecord>) failedItemRecords);
              }
              indexPatchProvider.PostPatchOperation(executionContext, string.Empty, (IEnumerable<PatchDescription>) null);
              executionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully added {0} failed items to re-index.", (object) num)));
            }
            else
            {
              List<string> branchesToIndex = this.GetBranchesToIndex();
              if (branchesToIndex != null && branchesToIndex.Count > 0)
              {
                if (this.Patch == Patch.RepositoryHeal && coreIndexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/SearchShared/Settings/EnableMultiBranchRepoHeal", true))
                {
                  coreIndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Multi branch repo heal started for given indexing unit type:{0} and {1} branches.", (object) this.IndexingUnit.IndexingUnitType, (object) branchesToIndex.Count)));
                  if (this.IndexingUnit.IndexingUnitType == "Git_Repository" && this.IndexingUnit.IsLargeRepository(coreIndexingExecutionContext.RequestContext))
                    this.ExecuteCrawlerParserAndFeeder(executionContext, string.Empty, branchesToIndex);
                  else
                    this.CreateCrawlSpecAndExecuteCrawlParseFeed(executionContext, string.Empty, branchesToIndex, indexPatchProvider);
                }
                else if (this.IndexingUnit.IndexingUnitType == "TFVC_Repository" || this.IndexingUnit.IndexingUnitType == "Git_Repository" && !this.IndexingUnit.IsLargeRepository(coreIndexingExecutionContext.RequestContext))
                {
                  foreach (string branchName in branchesToIndex)
                    this.CreateCrawlSpecAndExecuteCrawlParseFeed(executionContext, branchName, new List<string>(), indexPatchProvider);
                  if (this.GetCountOfFailedFiles(executionContext) > 0)
                    this.QueueFailedItemsPatchOperation(executionContext);
                }
                else
                  coreIndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Patch opearation is not supported for given indexing unit type:{0}", (object) this.IndexingUnit.IndexingUnitType)));
              }
              else
                coreIndexingExecutionContext.Log.Append("There are no branches to heal.");
              new ThresholdViolationIdentifier().IdentifyItemLevelFailureViolationsForIndexingUnit((ExecutionContext) executionContext, executionContext.ItemLevelFailureDataAccess, executionContext.IndexingUnit);
              operationResult.Status = OperationStatus.Succeeded;
            }
          }
          else if (this.IndexingUnit.IndexingUnitType == "Git_Repository")
          {
            if (executionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/CodeEntityBranchConfigurationUpdatesEnabled", true, true))
            {
              coreIndexingExecutionContext.Log.Append(this.HandleGitRepoBranchUpdates(executionContext));
              operationResult.Status = OperationStatus.Succeeded;
            }
            else
            {
              coreIndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Branch Updates are disabled.")));
              operationResult.Status = OperationStatus.Succeeded;
            }
          }
          else
          {
            coreIndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Change Event Data is not of expected type. Actual type: {0}, Expected Type: {1}", (object) this.IndexingUnitChangeEvent.ChangeData.GetType().FullName, (object) typeof (RepositoryPatchEventData).FullName)));
            operationResult.Status = OperationStatus.Failed;
          }
        }
        else
        {
          operationResult.Status = OperationStatus.Succeeded;
          coreIndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Indexing is disabled for {0}", (object) this.IndexingUnit.ToString())));
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(this.TraceMetaData.TracePoint, this.TraceMetaData.TraceArea, this.TraceMetaData.TraceLayer, nameof (RunOperation));
      }
      return operationResult;
    }

    internal virtual List<string> GetBranchesToIndex()
    {
      List<string> branchesToIndex = new List<string>();
      if (this.IndexingUnit.IndexingUnitType == "Git_Repository")
      {
        branchesToIndex = (this.IndexingUnit.TFSEntityAttributes as GitCodeRepoTFSAttributes).BranchesToIndex;
        branchesToIndex.RemoveAll((Predicate<string>) (s => string.IsNullOrWhiteSpace(s)));
      }
      else if (this.IndexingUnit.IndexingUnitType == "ScopedIndexingUnit")
        branchesToIndex = (this.IndexingUnit.Properties as ScopedGitRepositoryIndexingProperties).BranchIndexInfo.Keys.ToList<string>();
      else
        branchesToIndex.Add(string.Empty);
      return branchesToIndex;
    }

    internal List<ItemLevelFailureRecord> ConvertToFailedItemRecords(List<string> files)
    {
      HashSet<string> hashSet = files.ToHashSet<string>();
      List<ItemLevelFailureRecord> failedItemRecords = new List<ItemLevelFailureRecord>(hashSet.Count);
      Branches branches = new Branches();
      branches.AddRange((IEnumerable<string>) this.GetBranchesToIndex());
      foreach (string str in hashSet)
        failedItemRecords.Add(new ItemLevelFailureRecord()
        {
          Metadata = (FailureMetadata) new FileFailureMetadata()
          {
            Branches = branches
          },
          Item = str
        });
      return failedItemRecords;
    }

    internal override CodeCrawlSpec CreateCrawlSpec(
      IndexingExecutionContext iexContext,
      ref string branchName,
      in List<string> branches)
    {
      IVssRequestContext requestContext = iexContext.RequestContext;
      if (this.IndexingUnit.IndexingUnitType == "TFVC_Repository")
      {
        TFSEntityAttributes entityAttributes = this.IndexingUnit.TFSEntityAttributes;
        TfvcPatchIndexCrawlSpec crawlSpec = new TfvcPatchIndexCrawlSpec();
        ((CoreCrawlSpec) crawlSpec).CollectionName = iexContext.CollectionName;
        Guid guid = iexContext.CollectionId;
        ((CoreCrawlSpec) crawlSpec).CollectionId = guid.ToString();
        ((CodeCrawlSpec) crawlSpec).ProjectName = iexContext.ProjectName;
        ((CodeCrawlSpec) crawlSpec).ProjectId = iexContext.ProjectId.ToString();
        ((TfvcCrawlSpec) crawlSpec).ScopePath = "$/" + iexContext.ProjectName + "/";
        ((CodeCrawlSpec) crawlSpec).RepositoryName = "$/" + iexContext.ProjectName;
        guid = this.IndexingUnit.TFSEntityId;
        ((CodeCrawlSpec) crawlSpec).RepositoryId = guid.ToString();
        ((CodeCrawlSpec) crawlSpec).VcType = VersionControlType.TFVC;
        crawlSpec.PatchDescriptions = this.Patches;
        crawlSpec.ShouldUpdateLastChangeInfo = false;
        ((CodeCrawlSpec) crawlSpec).LastIndexedChangeId = this.LastIndexedChangeId;
        ((CodeCrawlSpec) crawlSpec).LastIndexedChangeUtcTime = this.LastIndexedChangeTime;
        crawlSpec.Patch = this.Patch;
        return (CodeCrawlSpec) crawlSpec;
      }
      if (!(this.IndexingUnit.IndexingUnitType == "Git_Repository") && !(this.IndexingUnit.IndexingUnitType == "ScopedIndexingUnit"))
        return (CodeCrawlSpec) null;
      GitCodeRepoTFSAttributes entityAttributes1 = iexContext.RepositoryIndexingUnit.TFSEntityAttributes as GitCodeRepoTFSAttributes;
      List<BranchInfo> branchInfoList = new List<BranchInfo>();
      if (!string.IsNullOrWhiteSpace(branchName))
      {
        BranchInfo branchInfo = new BranchInfo()
        {
          BranchName = branchName,
          ChangeId = RepositoryConstants.DefaultLastIndexCommitId,
          ChangeTime = RepositoryConstants.DefaultLastIndexChangeUtcTime
        };
        branchInfoList.Add(branchInfo);
      }
      else
      {
        if (branches == null || branches.Count <= 0)
          throw new ArgumentException("Branches information is not passed.");
        foreach (string str in branches)
          branchInfoList.Add(new BranchInfo()
          {
            BranchName = str,
            ChangeId = RepositoryConstants.DefaultLastIndexCommitId,
            ChangeTime = RepositoryConstants.DefaultLastIndexChangeUtcTime
          });
      }
      GitPatchIndexCrawlSpec crawlSpec1 = new GitPatchIndexCrawlSpec();
      ((CoreCrawlSpec) crawlSpec1).CollectionName = iexContext.CollectionName;
      ((CoreCrawlSpec) crawlSpec1).CollectionId = iexContext.CollectionId.ToString();
      ((CodeCrawlSpec) crawlSpec1).ProjectName = iexContext.ProjectName;
      ((CodeCrawlSpec) crawlSpec1).ProjectId = iexContext.ProjectId.ToString();
      crawlSpec1.ProjectVisibility = iexContext.ProjectVisibility;
      ((CodeCrawlSpec) crawlSpec1).RepositoryName = iexContext.RepositoryName;
      ((CodeCrawlSpec) crawlSpec1).RepositoryId = iexContext.RepositoryId.ToString();
      ((GitCrawlSpec) crawlSpec1).CurrentBranchesInfo = branchInfoList;
      ((GitCrawlSpec) crawlSpec1).BranchName = branchName;
      ((CodeCrawlSpec) crawlSpec1).VcType = VersionControlType.Git;
      crawlSpec1.PatchDescriptions = this.Patches;
      ((GitCrawlSpec) crawlSpec1).GitRepoRemoteUrl = entityAttributes1.RemoteUrl;
      crawlSpec1.ShouldUpdateLastChangeInfo = false;
      ((GitCrawlSpec) crawlSpec1).IsDefaultBranch = branchName == entityAttributes1.DefaultBranch;
      ((CoreCrawlSpec) crawlSpec1).JobYieldData = (AbstractJobYieldData) new GitIndexJobYieldData();
      ((CodeCrawlSpec) crawlSpec1).DefaultBranchName = entityAttributes1.DefaultBranch;
      crawlSpec1.Patch = this.Patch;
      return (CodeCrawlSpec) crawlSpec1;
    }

    internal CodeCrawlSpec CreateBulkIndexCrawlSpec(
      IndexingExecutionContext iexContext,
      string branchName,
      List<string> branches)
    {
      if (this.IndexingUnit.IndexingUnitType == "TFVC_Repository")
      {
        TfvcCodeRepoIndexingProperties properties = (TfvcCodeRepoIndexingProperties) iexContext.RepositoryIndexingUnit.Properties;
        return (CodeCrawlSpec) new TfvcIndexCrawlSpec(iexContext, this.IndexingUnit.TFSEntityId, "-1", "-1");
      }
      if (this.IndexingUnit.IndexingUnitType == "Git_Repository" || this.IndexingUnit.IndexingUnitType == "ScopedIndexingUnit")
      {
        GitCodeRepoTFSAttributes entityAttributes = iexContext.RepositoryIndexingUnit.TFSEntityAttributes as GitCodeRepoTFSAttributes;
        if (!string.IsNullOrWhiteSpace(branchName))
          return (CodeCrawlSpec) GitIndexCrawlSpec.Create(iexContext, entityAttributes, iexContext.RepositoryId.Value, branchName, (string) null, (List<string>) null, (string) null);
        if (branches == null || branches.Count <= 0)
          throw new ArgumentException("Branches information is not passed");
        string scopedIuElseNull = this.IndexingUnit.GetScopePathFromTFSAttributesIfScopedIUElseNull();
        return (CodeCrawlSpec) GitIndexCrawlSpec.Create(iexContext, entityAttributes, iexContext.RepositoryId.Value, string.Empty, RepositoryConstants.BranchCreationOrDeletionCommitId, branches, scopedIuElseNull);
      }
      throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Indexing unit type : {0} is not supported by {1}", (object) this.IndexingUnit.IndexingUnitType, (object) nameof (RepositoryCodePatchOperation))));
    }

    internal virtual IIndexPatchProvider GetIndexPatchProvider(
      IndexingExecutionContext indexingExecutionContext,
      Patch patch,
      TraceMetaData traceMetaData)
    {
      return IndexPatchProviderFactory.GetPatchProvider(indexingExecutionContext, patch, traceMetaData);
    }

    internal virtual string HandleGitRepoBranchUpdates(
      IndexingExecutionContext indexingExecutionContext)
    {
      return new GitBranchUpdatesHandlerTask(this.IndexingUnit, this.IndexingUnitDataAccess, this.IndexingUnitChangeEvent, new GitRepoSyncAnalyzerFactory().GetGitRepoSyncAnalyzer((ExecutionContext) indexingExecutionContext, this.TraceMetaData, this.IndexingUnitChangeEventHandler, (IEntityType) CodeEntityType.GetInstance())).HandleGitBranchUpdates(indexingExecutionContext);
    }

    internal virtual void CreateCrawlSpecAndExecuteCrawlParseFeed(
      IndexingExecutionContext iexContext,
      string branchName,
      List<string> branches,
      IIndexPatchProvider indexPatchProvider)
    {
      CodeCrawlSpec bulkIndexCrawlSpec = this.CreateBulkIndexCrawlSpec(iexContext, branchName, branches);
      this.Patches = indexPatchProvider.GetPatches(iexContext, branchName, bulkIndexCrawlSpec, this.IndexingUnit) ?? (IEnumerable<PatchDescription>) new List<PatchDescription>();
      this.LastIndexedChangeId = bulkIndexCrawlSpec.LastIndexedChangeId;
      this.LastIndexedChangeTime = bulkIndexCrawlSpec.LastIndexedChangeUtcTime;
      this.ExecuteCrawlerParserAndFeeder(iexContext, branchName, branches);
      indexPatchProvider.PostPatchOperation(iexContext, branchName, this.Patches);
    }

    protected internal override CorePipelineResult ExecuteCrawlerParserAndFeeder(
      IndexingExecutionContext iexContext,
      string branchName,
      List<string> branches)
    {
      using (CodeIndexingPipelineContext pipelineContext = this.GetPipelineContext(iexContext, branchName, branches))
        return this.ExecutePipeline(this.GetPipeline(pipelineContext));
    }

    internal override CodeIndexingPipeline GetPipeline(CodeIndexingPipelineContext pipelineContext)
    {
      IndexingExecutionContext executionContext = pipelineContext.IndexingExecutionContext;
      if (executionContext.RepositoryIndexingUnit.IndexingUnitType == "Git_Repository" && this.IndexingUnit.IndexingUnitType == "ScopedIndexingUnit" || this.IndexingUnit.IsLargeRepository(executionContext.RequestContext))
        this.WorkerPipeline = (CodeIndexingPipeline) new FileGroupPipelineForPatch(pipelineContext);
      else
        this.WorkerPipeline = base.GetPipeline(pipelineContext);
      return this.WorkerPipeline;
    }

    internal override void QueueFailedItemsPatchOperation(
      IndexingExecutionContext indexingExecutionContext)
    {
      int withMaxAttemptCount = indexingExecutionContext.ItemLevelFailureDataAccess.GetCountOfRecordsWithMaxAttemptCount(indexingExecutionContext.RequestContext, indexingExecutionContext.RepositoryIndexingUnit, 1);
      string str = (string) null;
      int num;
      if (withMaxAttemptCount > 0)
      {
        str = this.IndexingUnitChangeEvent.LeaseId;
        num = 0;
        indexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Requeueing Patch operation with leaseId and zero delay since there are {0} pending ILFs with zero patch attempted.", (object) withMaxAttemptCount)));
      }
      else
      {
        num = indexingExecutionContext.ServiceSettings.JobSettings.FailedItemsPatchOperationDelayInSeconds;
        indexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Requeueing Patch operation without leaseId.")));
      }
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent();
      indexingUnitChangeEvent1.IndexingUnitId = indexingExecutionContext.RepositoryIndexingUnit.IndexingUnitId;
      RepositoryPatchEventData repositoryPatchEventData = new RepositoryPatchEventData((ExecutionContext) indexingExecutionContext);
      repositoryPatchEventData.Patch = Patch.ReIndexFailedItems;
      repositoryPatchEventData.Delay = TimeSpan.FromSeconds((double) num);
      indexingUnitChangeEvent1.ChangeData = (ChangeEventData) repositoryPatchEventData;
      indexingUnitChangeEvent1.ChangeType = "Patch";
      indexingUnitChangeEvent1.State = IndexingUnitChangeEventState.Pending;
      indexingUnitChangeEvent1.LeaseId = str;
      indexingUnitChangeEvent1.AttemptCount = (byte) 0;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent2 = indexingUnitChangeEvent1;
      this.IndexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeeded((ExecutionContext) indexingExecutionContext, indexingUnitChangeEvent2);
    }
  }
}
