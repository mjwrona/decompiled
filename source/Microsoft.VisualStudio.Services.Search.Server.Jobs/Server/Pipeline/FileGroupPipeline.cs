// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.FileGroupPipeline
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.ChangeEventData;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties.Code;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using Microsoft.VisualStudio.Services.Search.Feeder.Plugins;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.SQL;
using Microsoft.VisualStudio.Services.Search.Server.Jobs;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  internal class FileGroupPipeline : CodeIndexingPipeline
  {
    public const string LargeRepositoryCommitIds = "LargeRepositoryCommitIds";
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetaData = new TraceMetaData(1083095, "Indexing Pipeline", "Pipeline");

    public FileGroupPipeline(CodeIndexingPipelineContext pipelineContext)
      : this(FileGroupPipeline.s_traceMetaData, nameof (FileGroupPipeline), pipelineContext)
    {
    }

    protected FileGroupPipeline(
      TraceMetaData traceMetaData,
      string name,
      CodeIndexingPipelineContext pipelineContext)
      : base(traceMetaData, name, pipelineContext)
    {
      this.Branches = pipelineContext.Branches;
    }

    internal CodeIndexingPipeline DiffTreeCrawler { get; set; }

    internal CodeIndexingPipeline DiffDataCrawler { get; set; }

    internal List<string> Branches { get; }

    [SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals")]
    protected internal override OperationStatus PostPostRun(OperationStatus opStatus)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(this.TraceMetaData, nameof (PostPostRun));
      try
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("FileGroupCPF triggered for {0} and Branches ({1})", (object) this.IndexingUnit, (object) string.Join(",", (IEnumerable<string>) this.Branches))));
        IndexingExecutionContext executionContext = this.PipelineContext.IndexingExecutionContext;
        if (executionContext.RepositoryName != null && executionContext.RepositoryIndexingUnit != null)
          executionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Repository Name = '{0}', Repository = '{1}'", (object) executionContext.RepositoryName, (object) executionContext.RepositoryIndexingUnit)));
        if (this.IndexingUnit.IndexingUnitType == "Git_Repository")
        {
          List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> scopedIndexingUnits = this.GetScopedIndexingUnits(executionContext);
          bool deleteFilesWithMaxRetriesExhausted = true;
          if (this.CleanUpPreviousTempFileRecords(executionContext, scopedIndexingUnits, deleteFilesWithMaxRetriesExhausted))
          {
            executionContext.Log.Append("TempFileRecordStore is not empty. Hence queuing data crawling for existing tempFileRecords.");
            return OperationStatus.PartiallySucceeded;
          }
          foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in scopedIndexingUnits)
          {
            if (this.RemoveBranchMappingsToDeletedFolders(executionContext, indexingUnit, this.Branches))
              executionContext.IndexingUnitDataAccess.UpdateIndexingUnit(executionContext.RequestContext, indexingUnit);
          }
          GitCodeRepoIndexingProperties properties1 = executionContext.RepositoryIndexingUnit.Properties as GitCodeRepoIndexingProperties;
          DateTime lastProcessedTime = properties1.RepositoryLastProcessedTime;
          GitCodeRepoIndexingProperties gitCodeRepoIndexingProperties1 = this.UpdateRepositoryLastProcessedTime(executionContext, properties1, DateTime.UtcNow);
          Dictionary<string, string> commitIdMapRepoLevel = this.GetBranchToIndexedCommitIdMapRepoLevel(scopedIndexingUnits);
          int count = commitIdMapRepoLevel.Count;
          GitCodeRepoIndexingProperties gitCodeRepoIndexingProperties2 = this.UpdateBranchLastProcessedTimeForBranchesInSync(executionContext, gitCodeRepoIndexingProperties1, lastProcessedTime, commitIdMapRepoLevel);
          executionContext.IndexingUnitDataAccess.UpdateIndexingUnit(executionContext.RequestContext, executionContext.RepositoryIndexingUnit);
          IDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int> itemsCountBreakup = this.GetFailedItemsCountBreakup(executionContext, scopedIndexingUnits);
          bool flag1 = itemsCountBreakup.Any<KeyValuePair<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int>>();
          Dictionary<string, GitCommit> dictionary1 = new Dictionary<string, GitCommit>();
          Dictionary<string, List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>> branchToScopedIndexingUnits = new Dictionary<string, List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>>();
          bool currentHostConfigValue = executionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/LargeRepositoryScopedIndexingUnitIdGettingReIndexed", true);
          int num1 = 0;
          int num2 = 0;
          int num3 = 0;
          int num4 = 0;
          HashSet<string> stringSet = new HashSet<string>();
          foreach (string branch in this.Branches)
          {
            GitCommit topCommit = this.GetTopCommit(executionContext, branch);
            if (topCommit == null)
            {
              ++num2;
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Branch '{0}' appears to be deleted. Moving on to next branch.", (object) branch)));
              foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in scopedIndexingUnits)
              {
                ScopedGitRepositoryIndexingProperties properties2 = (ScopedGitRepositoryIndexingProperties) indexingUnit.Properties;
                if (properties2.BranchIndexInfo.ContainsKey(branch) && properties2.BranchIndexInfo[branch].CommitId != RepositoryConstants.DefaultLastIndexCommitId)
                {
                  stringSet.Add(branch);
                  break;
                }
              }
            }
            else
            {
              bool flag2 = !currentHostConfigValue && commitIdMapRepoLevel.ContainsKey(branch) && commitIdMapRepoLevel[branch] == topCommit.CommitId;
              if (flag2)
                ++num3;
              else
                ++num4;
              if (!flag2 || flag1)
              {
                ++num1;
                dictionary1.Add(branch, topCommit);
                List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitList = this.AddOrUpdateScopedIndexingUnits(executionContext, topCommit, branch);
                branchToScopedIndexingUnits.Add(branch, indexingUnitList);
              }
            }
          }
          if (stringSet.Count > 0)
            new GitRepoSyncAnalyzerFactory().GetGitRepoSyncAnalyzer((ExecutionContext) executionContext, this.TraceMetaData, this.PipelineContext.IndexingUnitChangeEventHandler, (IEntityType) CodeEntityType.GetInstance()).QueueGitBranchDeleteOperation((ExecutionContext) executionContext, this.IndexingUnit, stringSet, false);
          List<string> indexedForFirstTime = this.GetBranchesBeingIndexedForFirstTime(executionContext, gitCodeRepoIndexingProperties2, this.Branches);
          List<string> removeFromRegistry = this.GetBranchesToRemoveFromRegistry(executionContext, gitCodeRepoIndexingProperties2, this.Branches, stringSet.ToList<string>());
          string fromTfsAttributes = this.IndexingUnit.GetRepositoryNameFromTFSAttributes();
          string projectName = executionContext.ProjectName;
          string traceLayer = "Common";
          if (projectName != null && fromTfsAttributes != null)
          {
            this.ClearCompletedRegistryValues(executionContext, projectName, fromTfsAttributes, removeFromRegistry, traceLayer);
            this.AddToRegistry(executionContext, projectName, fromTfsAttributes, indexedForFirstTime, traceLayer);
          }
          if (!dictionary1.Any<KeyValuePair<string, GitCommit>>())
          {
            string str = FormattableString.Invariant(FormattableStringFactory.Create("None of the branches in ({0}) have any new commit to index. Hence not continuing with Indexing. ", (object) string.Join(",", (IEnumerable<string>) this.Branches)));
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, str);
            executionContext.Log.Append(str);
            return OperationStatus.Succeeded;
          }
          HashSet<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitSet = new HashSet<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
          foreach (List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> values in branchToScopedIndexingUnits.Values)
            indexingUnitSet.AddRange<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, HashSet<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>>((IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) values);
          if (flag1)
          {
            Dictionary<string, string> scopedIndexingUnitToFailedItemsCountStringForm = new Dictionary<string, string>();
            itemsCountBreakup.ForEach<KeyValuePair<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int>>((Action<KeyValuePair<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int>>) (x => scopedIndexingUnitToFailedItemsCountStringForm[x.Key.ToString()] = x.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
            executionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("FailedItemsCount breakup: {0}. ", (object) scopedIndexingUnitToFailedItemsCountStringForm.ToJsonString())));
          }
          else
            executionContext.Log.Append("No Failed items present.");
          executionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Branches in complete sync: {0}. ", (object) count)));
          executionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Branches for CI Processing : {0}. ", (object) num1)));
          executionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Branches Deleted in TFS : {0}. ", (object) num2)));
          executionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Branches with no new commits to process : {0}. ", (object) num3)));
          executionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Branches with new commits : {0}. ", (object) num4)));
          IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList = (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
          foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit scopedIndexingUnit in indexingUnitSet.ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>().OrderByDescending<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, string>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, string>) (x => ((ScopedGitRepositoryAttributes) x.TFSEntityAttributes).ScopePath)).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
          {
            Dictionary<string, ScopedGitBranchIndexInfo> branchInfoObject = this.GetCurrentBranchInfoObject(scopedIndexingUnit, dictionary1, branchToScopedIndexingUnits);
            Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent(this.PipelineContext.IndexingUnitChangeEvent.LeaseId)
            {
              IndexingUnitId = scopedIndexingUnit.IndexingUnitId,
              ChangeData = (Microsoft.VisualStudio.Services.Search.Common.ChangeEventData) new LargeRepositoryMetadataCrawlerEventData((ExecutionContext) executionContext)
              {
                BranchIndexInfo = branchInfoObject
              },
              ChangeType = "UpdateIndex",
              State = IndexingUnitChangeEventState.Pending,
              AttemptCount = 0
            };
            indexingUnitChangeEventList.Add(indexingUnitChangeEvent);
          }
          if (indexingUnitChangeEventList.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>())
          {
            Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
            foreach (KeyValuePair<string, GitCommit> keyValuePair in dictionary1)
              dictionary2[keyValuePair.Key] = keyValuePair.Value.CommitId;
            this.QueueEventForScopedIndexingUnitInBatches(indexingUnitChangeEventList, executionContext);
          }
          executionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Queued {0} changeEvents for metadata crawling. ", (object) indexingUnitChangeEventList.Count)));
          return OperationStatus.Succeeded;
        }
        if (this.IndexingUnit.IndexingUnitType == "ScopedIndexingUnit")
        {
          List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> scopedIndexingUnits = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>()
          {
            this.IndexingUnit
          };
          if (this.CleanUpPreviousTempFileRecords(executionContext, scopedIndexingUnits, false))
          {
            executionContext.Log.Append("TempFileRecordStore is not empty. Hence queuing data crawling for existing tempFileRecords.");
            return OperationStatus.PartiallySucceeded;
          }
          LargeRepositoryMetadataCrawlerEventData changeData = this.PipelineContext.IndexingUnitChangeEvent.ChangeData as LargeRepositoryMetadataCrawlerEventData;
          CodeCrawlSpec crawlSpec = this.PipelineContext.CrawlSpec.Clone();
          if (executionContext.RepositoryIndexingUnit.IndexingUnitType != "CustomRepository")
          {
            Dictionary<string, ScopedGitBranchIndexInfo> branchIndexInfo = changeData.BranchIndexInfo;
            List<BranchInfo> branchInfoList1 = new List<BranchInfo>();
            Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
            ScopedGitRepositoryAttributes entityAttributes = this.IndexingUnit.TFSEntityAttributes as ScopedGitRepositoryAttributes;
            foreach (KeyValuePair<string, ScopedGitBranchIndexInfo> keyValuePair in branchIndexInfo)
            {
              branchInfoList1.Add(new BranchInfo()
              {
                BranchName = keyValuePair.Key,
                ChangeId = keyValuePair.Value.CommitId
              });
              dictionary.Add(keyValuePair.Key, new List<string>()
              {
                entityAttributes.ScopePath
              });
            }
            Dictionary<string, string> idMapScopedLevel = this.GetBranchToIndexedCommitIdMapScopedLevel(executionContext);
            Dictionary<string, GitIndexJobYieldData> toJobYieldDataMap = this.GetBranchToJobYieldDataMap(executionContext);
            List<BranchInfo> branchInfoList2 = new List<BranchInfo>();
            foreach (KeyValuePair<string, string> keyValuePair in idMapScopedLevel)
              branchInfoList2.Add(new BranchInfo()
              {
                BranchName = keyValuePair.Key,
                ChangeId = keyValuePair.Value
              });
            GitCrawlSpec gitCrawlSpec = crawlSpec as GitCrawlSpec;
            gitCrawlSpec.CurrentBranchesInfo = branchInfoList1;
            gitCrawlSpec.LastIndexedBranchesInfo = branchInfoList2;
            gitCrawlSpec.BranchesToScopePaths = dictionary;
            this.PopulateJobYieldData(gitCrawlSpec, idMapScopedLevel, toJobYieldDataMap);
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Running Diff Crawler on ({0}/{1}) for change event {2}", (object) this.IndexingUnit, (object) string.Join(",", gitCrawlSpec.CurrentBranchesInfo.Select<BranchInfo, string>((Func<BranchInfo, string>) (x => x.BranchName))), (object) this.PipelineContext.IndexingUnitChangeEvent)));
          }
          long endingId1 = 0;
          if (executionContext.RepositoryIndexingUnit.IndexingUnitType == "CustomRepository")
            executionContext.TempFileMetadataStoreDataAccess.GetMinAndMaxIds(executionContext.RequestContext, out long _, out endingId1);
          this.DiffTreeCrawler = (CodeIndexingPipeline) new DiffMetadataCrawler(this.PipelineContext, crawlSpec);
          CorePipelineResult corePipelineResult = this.DiffTreeCrawler.Run();
          if (corePipelineResult.OperationStatus == OperationStatus.Failed)
            executionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Metadata Crawl Failed for {0}. ScopePath {1}", (object) this.IndexingUnit, (object) (this.IndexingUnit.TFSEntityAttributes as ScopedGitRepositoryAttributes).ScopePath)));
          else if (corePipelineResult.OperationStatus == OperationStatus.Succeeded)
          {
            executionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Metadata Crawl Succeeded for {0}. ScopePath {1}", (object) this.IndexingUnit, (object) (this.IndexingUnit.TFSEntityAttributes as ScopedGitRepositoryAttributes).ScopePath)));
            long startingId;
            long endingId2;
            executionContext.TempFileMetadataStoreDataAccess.GetMinAndMaxIds(executionContext.RequestContext, out startingId, out endingId2);
            if (endingId2 >= startingId && startingId > 0L)
            {
              if (executionContext.RepositoryIndexingUnit.IndexingUnitType == "CustomRepository")
                startingId = endingId1 + 1L;
              this.QueueIndexingOperationsForTemporaryUnits((ExecutionContext) executionContext, this.IndexingUnit, crawlSpec.TotalItemsCrawled, startingId, endingId2);
            }
          }
          opStatus = corePipelineResult.OperationStatus;
          this.PipelineResultData = corePipelineResult.Data;
          return opStatus;
        }
        if (this.IndexingUnit.IndexingUnitType == "TemporaryIndexingUnit")
        {
          this.DiffDataCrawler = (CodeIndexingPipeline) new Microsoft.VisualStudio.Services.Search.Server.Pipeline.DiffDataCrawler(this.PipelineContext);
          CorePipelineResult corePipelineResult = this.DiffDataCrawler.Run();
          opStatus = corePipelineResult.OperationStatus;
          this.PipelineResultData = corePipelineResult.Data;
          return opStatus;
        }
        string str1 = FormattableString.Invariant(FormattableStringFactory.Create("FileGroupCPF.Run() invoked with invalid {0}.", (object) this.IndexingUnit));
        executionContext.Log.Append(str1);
        throw new InvalidOperationException(str1);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(this.TraceMetaData, nameof (PostPostRun));
      }
    }

    protected internal override sealed bool AllDocumentsAreProcessed() => true;

    protected override sealed void AnalyzeFeederResponse(
      CorePipelineContext<CodePipelineDocumentId, CodeDocument> pipelineContext,
      ESIndexFeedResponseData responseData,
      int totalItems)
    {
    }

    internal override sealed void HandlePipelineError(Exception ex)
    {
    }

    internal override sealed bool IsPrimaryRun() => true;

    protected internal override sealed OperationStatus PostRun(OperationStatus opStatus) => opStatus;

    protected override sealed void Prepare()
    {
    }

    protected internal override sealed void PrePreRun()
    {
    }

    protected internal override sealed void PreRun()
    {
    }

    internal override sealed IReadOnlyList<CorePipelineStage<CodePipelineDocumentId, CodeDocument>> RegisterStages() => (IReadOnlyList<CorePipelineStage<CodePipelineDocumentId, CodeDocument>>) new CorePipelineStage<CodePipelineDocumentId, CodeDocument>[0];

    internal override sealed bool ShouldRestartPipeline() => false;

    internal virtual void QueueEventForScopedIndexingUnitInBatches(
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> scopedIndexingUnitEvents,
      IndexingExecutionContext indexingExecutionContext)
    {
      int configValue = indexingExecutionContext.GetConfigValue<int>("/Service/ALMSearch/Settings/LargeRepositoryParallelMetadataCrawling");
      int count = 0;
      IndexingUnitChangeEventPrerequisites eventPrerequisites1 = new IndexingUnitChangeEventPrerequisites();
      List<IndexingUnitChangeEventPrerequisites> eventPrerequisitesList = (List<IndexingUnitChangeEventPrerequisites>) null;
      while (scopedIndexingUnitEvents.Count > count)
      {
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> list = scopedIndexingUnitEvents.Skip<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>(count).Take<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>(configValue).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
        count += list.Count;
        if (eventPrerequisitesList != null)
        {
          for (int index = 0; index < list.Count; ++index)
            list[index].Prerequisites = eventPrerequisitesList[index];
        }
        IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList = this.PipelineContext.IndexingUnitChangeEventHandler.HandleEvents((ExecutionContext) indexingExecutionContext, (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) list);
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent in (IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) indexingUnitChangeEventList)
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Queued event {0} to crawl file level metadata for indexing unit {1}.", (object) indexingUnitChangeEvent, (object) indexingUnitChangeEvent.IndexingUnitId)));
        eventPrerequisitesList = new List<IndexingUnitChangeEventPrerequisites>(indexingUnitChangeEventList.Count);
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent in (IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) indexingUnitChangeEventList)
        {
          IndexingUnitChangeEventPrerequisites eventPrerequisites2 = new IndexingUnitChangeEventPrerequisites();
          eventPrerequisites2.Add(new IndexingUnitChangeEventPrerequisitesFilter()
          {
            Id = indexingUnitChangeEvent.Id,
            Operator = IndexingUnitChangeEventFilterOperator.Contains,
            PossibleStates = new List<IndexingUnitChangeEventState>()
            {
              IndexingUnitChangeEventState.Succeeded,
              IndexingUnitChangeEventState.Failed
            }
          });
          eventPrerequisitesList.Add(eventPrerequisites2);
        }
      }
    }

    internal virtual void PopulateJobYieldData(
      GitCrawlSpec gitCrawlSpec,
      Dictionary<string, string> branchToIndexedCommitIdMap = null,
      Dictionary<string, GitIndexJobYieldData> branchToJobYieldDataMap = null)
    {
      if (gitCrawlSpec.BranchesToJobYieldDataInfo == null)
        gitCrawlSpec.BranchesToJobYieldDataInfo = new Dictionary<string, GitIndexJobYieldData>();
      foreach (BranchInfo branchInfo in gitCrawlSpec.CurrentBranchesInfo)
      {
        if (!gitCrawlSpec.BranchesToJobYieldDataInfo.ContainsKey(branchInfo.BranchName))
          gitCrawlSpec.BranchesToJobYieldDataInfo.Add(branchInfo.BranchName, new GitIndexJobYieldData());
        if (branchToIndexedCommitIdMap != null && !branchToIndexedCommitIdMap.TryGetValue(branchInfo.BranchName, out string _))
          gitCrawlSpec.LastIndexedBranchesInfo.Add(new BranchInfo()
          {
            BranchName = branchInfo.BranchName,
            ChangeId = RepositoryConstants.DefaultLastIndexCommitId
          });
        GitIndexJobYieldData indexJobYieldData;
        if (branchToJobYieldDataMap != null && branchToJobYieldDataMap.TryGetValue(branchInfo.BranchName, out indexJobYieldData) && !string.IsNullOrWhiteSpace(indexJobYieldData?.ContinuationToken))
          gitCrawlSpec.BranchesToJobYieldDataInfo[branchInfo.BranchName] = new GitIndexJobYieldData()
          {
            BaseVersion = indexJobYieldData.BaseVersion,
            TargetVersion = indexJobYieldData.TargetVersion,
            TargetVersionDate = indexJobYieldData.TargetVersionDate,
            ContinuationToken = indexJobYieldData.ContinuationToken
          };
      }
      foreach (BranchInfo branchInfo in gitCrawlSpec.LastIndexedBranchesInfo)
      {
        if (!gitCrawlSpec.BranchesToJobYieldDataInfo.ContainsKey(branchInfo.BranchName))
          gitCrawlSpec.BranchesToJobYieldDataInfo.Add(branchInfo.BranchName, new GitIndexJobYieldData());
      }
    }

    internal void PopulateDummyJobYieldData(GitCrawlSpec gitCrawlSpec)
    {
      if (gitCrawlSpec.BranchesToJobYieldDataInfo == null)
        gitCrawlSpec.BranchesToJobYieldDataInfo = new Dictionary<string, GitIndexJobYieldData>();
      foreach (BranchInfo branchInfo in gitCrawlSpec.CurrentBranchesInfo)
      {
        if (!gitCrawlSpec.BranchesToJobYieldDataInfo.ContainsKey(branchInfo.BranchName))
          gitCrawlSpec.BranchesToJobYieldDataInfo.Add(branchInfo.BranchName, new GitIndexJobYieldData());
      }
      foreach (BranchInfo branchInfo in gitCrawlSpec.LastIndexedBranchesInfo)
      {
        if (!gitCrawlSpec.BranchesToJobYieldDataInfo.ContainsKey(branchInfo.BranchName))
          gitCrawlSpec.BranchesToJobYieldDataInfo.Add(branchInfo.BranchName, new GitIndexJobYieldData());
      }
    }

    internal virtual List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> CreateTemporaryIndexingUnits(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit scopedIndexingUnit,
      int ensureMinimum)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(this.TraceMetaData, nameof (CreateTemporaryIndexingUnits));
      try
      {
        if (ensureMinimum <= 0)
          throw new ArgumentException("Value cannot be a negative number", nameof (ensureMinimum));
        if (scopedIndexingUnit.IndexingUnitType != "ScopedIndexingUnit")
          throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Unsupported parent indexing unit {0}", (object) this.IndexingUnit)));
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> source = this.PipelineContext.IndexingExecutionContext.IndexingUnitDataAccess.GetIndexingUnitsWithGivenParent(executionContext.RequestContext, scopedIndexingUnit.IndexingUnitId, -1) ?? new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitList = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
        if (source.Count < ensureMinimum)
        {
          int num = ensureMinimum - source.Count;
          for (int index = 1; index <= num; ++index)
          {
            Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnit(Guid.NewGuid(), "TemporaryIndexingUnit", (IEntityType) CodeEntityType.GetInstance(), scopedIndexingUnit.IndexingUnitId, scopedIndexingUnit.IsShadow)
            {
              Properties = new IndexingProperties(),
              TFSEntityAttributes = new TFSEntityAttributes()
            };
            indexingUnitList.Add(indexingUnit);
          }
          if (!indexingUnitList.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
            return source;
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Creating {0} to index files in parallel.", (object) indexingUnitList.Count)));
          List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> collection = this.PipelineContext.IndexingExecutionContext.IndexingUnitDataAccess.AddOrUpdateIndexingUnits(executionContext.RequestContext, indexingUnitList, false);
          foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in collection)
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Created Indexing Unit {0}.", (object) indexingUnit)));
          source.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) collection);
        }
        return source.Take<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>(ensureMinimum).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(this.TraceMetaData, nameof (CreateTemporaryIndexingUnits));
      }
    }

    internal virtual void QueueIndexingOperationsForTemporaryUnits(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit scopedIndexingUnit,
      int totalNumberOfFiles,
      long startingId,
      long endingId)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(this.TraceMetaData, nameof (QueueIndexingOperationsForTemporaryUnits));
      try
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Queuing Indexing Operations with temporary Indexing Units.")));
        if (startingId <= 0L || endingId <= 0L || startingId > endingId)
          throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Invalid combination of {0} and {1}", (object) startingId, (object) endingId)));
        int configValue1 = executionContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MaximumNumberOfJobsPerRepository");
        int configValue2 = executionContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MinimumNumberOfFilesForJobSplit");
        int ensureMinimum = totalNumberOfFiles % configValue2 == 0 ? totalNumberOfFiles / configValue2 : totalNumberOfFiles / configValue2 + 1;
        if (ensureMinimum > configValue1)
          ensureMinimum = configValue1;
        if (ensureMinimum <= 0)
          return;
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> temporaryIndexingUnits = this.CreateTemporaryIndexingUnits(executionContext, scopedIndexingUnit, ensureMinimum);
        long num1 = (endingId - startingId + 1L) / (long) ensureMinimum;
        if (num1 <= 0L)
          num1 = (long) configValue2;
        long num2 = startingId + num1;
        IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList = (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
        for (int index = 0; index < ensureMinimum; ++index)
        {
          Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = temporaryIndexingUnits[index];
          if (index == ensureMinimum - 1)
            num2 = endingId;
          LargeRepositoryMetadataCrawlerEventData changeData = this.PipelineContext.IndexingUnitChangeEvent.ChangeData as LargeRepositoryMetadataCrawlerEventData;
          Guid guid = Guid.Empty;
          if (changeData != null)
          {
            Guid requestId = changeData.RequestId;
            guid = changeData.RequestId;
          }
          FileSplitGroupData fileSplitGroupData = new FileSplitGroupData(executionContext)
          {
            StartingId = startingId,
            TakeCount = Math.Min(CommonUtils.GetRandomNumberOfFilesToBePickedForIndexingFromTempMetadataStore(executionContext), Convert.ToInt32(num2 - startingId) + 1),
            LastId = num2,
            RequestId = guid
          };
          Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent(this.PipelineContext.IndexingUnitChangeEvent.LeaseId)
          {
            IndexingUnitId = indexingUnit.IndexingUnitId,
            ChangeData = (Microsoft.VisualStudio.Services.Search.Common.ChangeEventData) fileSplitGroupData,
            ChangeType = "UpdateIndex",
            State = IndexingUnitChangeEventState.Pending,
            AttemptCount = 0
          };
          indexingUnitChangeEventList.Add(indexingUnitChangeEvent);
          startingId = num2 + 1L;
          num2 = startingId + num1;
        }
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent handleEvent in (IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) this.PipelineContext.IndexingUnitChangeEventHandler.HandleEvents(executionContext, indexingUnitChangeEventList))
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Queued event {0} to index a group of files {1}.", (object) handleEvent, (object) handleEvent.ChangeData)));
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(this.TraceMetaData, nameof (QueueIndexingOperationsForTemporaryUnits));
      }
    }

    internal virtual List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> AddOrUpdateScopedIndexingUnits(
      IndexingExecutionContext indexingExecutionContext,
      GitCommit topCommit,
      string branch)
    {
      if (this.IndexingUnit.IndexingUnitType != "Git_Repository")
        throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Unsupported parent indexing unit {0}", (object) this.IndexingUnit)));
      GitHttpClientWrapper httpClientWrapper = this.GetGitHttpClientWrapper(indexingExecutionContext);
      GitVersionDescriptor versionDescriptor = new GitVersionDescriptor()
      {
        Version = topCommit.CommitId,
        VersionType = GitVersionType.Commit
      };
      List<GitItem> itemsAsync = httpClientWrapper.GetItemsAsync(versionDescriptor, versionControlRecursionType: VersionControlRecursionType.OneLevel);
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> scopedIndexingUnits = this.GetScopedIndexingUnits(indexingExecutionContext);
      Dictionary<string, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> dictionary1 = new Dictionary<string, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in scopedIndexingUnits)
        dictionary1[(indexingUnit.TFSEntityAttributes as ScopedGitRepositoryAttributes).ScopePath] = indexingUnit;
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> collection = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      int attributesIfGitRepo = this.IndexingUnit.GetBranchCountFromTFSAttributesIfGitRepo();
      List<IndexingUnitWithSize> indexingUnitWithSizeList = new List<IndexingUnitWithSize>();
      int indexingUnitId = this.IndexingUnit.IndexingUnitId;
      foreach (GitItem gitItem in itemsAsync)
      {
        if (gitItem.IsFolder)
        {
          string path = gitItem.Path;
          Microsoft.VisualStudio.Services.Search.Common.IndexingUnit scopedIndexingUnit;
          if (!dictionary1.TryGetValue(path, out scopedIndexingUnit))
            scopedIndexingUnit = this.CreateScopedIndexingUnit(path, indexingUnitId, this.IndexingUnit.IsShadow);
          if (scopedIndexingUnit.Properties.IndexIndices.Count == 0)
          {
            int documentCount = httpClientWrapper.GetDocumentCount(indexingExecutionContext.RequestContext, path, topCommit.CommitId);
            int branchCount = Math.Max(1000, attributesIfGitRepo);
            int num = branchCount;
            int currentEstimatedDocumentCount = (int) ((double) (documentCount * num) * indexingExecutionContext.ProvisioningContext.ContractType.GetFileCountFactorForMultibranch(indexingExecutionContext.RequestContext, indexingExecutionContext.IndexingUnit, branchCount));
            indexingUnitWithSizeList.Add(new IndexingUnitWithSize(scopedIndexingUnit, currentEstimatedDocumentCount, 0, true)
            {
              ActualInitialDocCount = currentEstimatedDocumentCount
            });
          }
          else
            collection.Add(scopedIndexingUnit);
        }
      }
      if (indexingUnitWithSizeList.Count > 0)
      {
        if (indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/Routing/SizeBasedCustomRoutingEnabled", true))
        {
          indexingExecutionContext.RequestContext.GetService<IRoutingService>().AssignShards(indexingExecutionContext, indexingUnitWithSizeList);
        }
        else
        {
          foreach (IndexingUnitWithSize indexingUnitWithSize in indexingUnitWithSizeList)
            indexingUnitWithSize.IndexingUnit.SetupIndexRouting(indexingExecutionContext);
          indexingExecutionContext.IndexingUnitDataAccess.AddOrUpdateIndexingUnits(indexingExecutionContext.RequestContext, indexingUnitWithSizeList.Select<IndexingUnitWithSize, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<IndexingUnitWithSize, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (x => x.IndexingUnit)).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>(), false);
        }
        Dictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> dictionary2 = this.GetScopedIndexingUnits(indexingExecutionContext).ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (x => x), (Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (x => x));
        foreach (IndexingUnitWithSize indexingUnitWithSize in indexingUnitWithSizeList)
        {
          Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit;
          if (dictionary2.TryGetValue(indexingUnitWithSize.IndexingUnit, out indexingUnit))
            collection.Add(indexingUnit);
        }
      }
      collection.AddRange(scopedIndexingUnits.Where<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, bool>) (x => (x.Properties as ScopedGitRepositoryIndexingProperties).BranchIndexInfo.ContainsKey(branch) && !string.IsNullOrWhiteSpace((x.Properties as ScopedGitRepositoryIndexingProperties).BranchIndexInfo[branch].CommitId) && (x.Properties as ScopedGitRepositoryIndexingProperties).BranchIndexInfo[branch].CommitId != RepositoryConstants.DefaultLastIndexCommitId)));
      return new HashSet<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) collection).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
    }

    internal virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnit CreateScopedIndexingUnit(
      string scopePath,
      int repositoryCodeIndexingUnitId,
      bool isShadow)
    {
      return new Microsoft.VisualStudio.Services.Search.Common.IndexingUnit(Guid.NewGuid(), "ScopedIndexingUnit", (IEntityType) CodeEntityType.GetInstance(), repositoryCodeIndexingUnitId, isShadow)
      {
        Properties = (IndexingProperties) new ScopedGitRepositoryIndexingProperties(),
        TFSEntityAttributes = (TFSEntityAttributes) new ScopedGitRepositoryAttributes()
        {
          ScopePath = scopePath
        }
      };
    }

    internal virtual bool CleanUpPreviousTempFileRecords(
      IndexingExecutionContext indexingExecutionContext,
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> scopedIndexingUnits,
      bool deleteFilesWithMaxRetriesExhausted)
    {
      if (scopedIndexingUnits == null || !scopedIndexingUnits.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
        return false;
      if (deleteFilesWithMaxRetriesExhausted)
        this.DeleteFilesWithMaxRetriesExhausted(indexingExecutionContext, scopedIndexingUnits);
      IDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int> numberOfRecords = indexingExecutionContext.TempFileMetadataStoreDataAccess.GetNumberOfRecords(indexingExecutionContext.RequestContext, (IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) scopedIndexingUnits);
      bool flag = false;
      foreach (KeyValuePair<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int> keyValuePair in (IEnumerable<KeyValuePair<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int>>) numberOfRecords)
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit key = keyValuePair.Key;
        int totalNumberOfFiles = keyValuePair.Value;
        string scopePath = (key.TFSEntityAttributes as ScopedGitRepositoryAttributes).ScopePath;
        ITempFileMetadataStoreDataAccess metadataStoreDataAccess = (ITempFileMetadataStoreDataAccess) new TempFileMetadataStoreDataAccess(key);
        if (totalNumberOfFiles > 0)
        {
          long startingId;
          long endingId;
          metadataStoreDataAccess.GetMinAndMaxIds(indexingExecutionContext.RequestContext, out startingId, out endingId);
          if (endingId >= startingId && startingId > 0L)
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Found registry entry thus queuing DataCrawl for {0}, ScopePath {1}. Re-Indexing files ranging from {2} to {3}", (object) key, (object) scopePath, (object) startingId, (object) endingId)));
            this.QueueIndexingOperationsForTemporaryUnits((ExecutionContext) indexingExecutionContext, key, totalNumberOfFiles, startingId, endingId);
            flag = true;
          }
        }
      }
      return flag;
    }

    internal virtual GitHttpClientWrapper GetGitHttpClientWrapper(
      IndexingExecutionContext indexingExecutionContext)
    {
      return new GitHttpClientWrapper((ExecutionContext) indexingExecutionContext, indexingExecutionContext.ProjectIndexingUnit.TFSEntityId, this.IndexingUnit.TFSEntityId, this.TraceMetaData);
    }

    internal virtual GitCommit GetTopCommit(
      IndexingExecutionContext indexingExecutionContext,
      string branch)
    {
      GitHttpClientWrapper httpClientWrapper = this.GetGitHttpClientWrapper(indexingExecutionContext);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Getting Top Commit for {0}/{1}", (object) this.IndexingUnit, (object) branch)));
      return httpClientWrapper.GetLatestCommitFromTFS(branch, indexingExecutionContext.RepositoryIndexingUnit.TFSEntityId);
    }

    internal virtual GitCommit GetGitCommit(
      IndexingExecutionContext indexingExecutionContext,
      string commitId)
    {
      GitHttpClientWrapper httpClientWrapper = this.GetGitHttpClientWrapper(indexingExecutionContext);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Getting Commit for Commit-Id {0}/{1}", (object) this.IndexingUnit, (object) commitId)));
      return httpClientWrapper.GetCommit(commitId);
    }

    internal virtual Dictionary<string, string> GetBranchToIndexedCommitIdMapScopedLevel(
      IndexingExecutionContext indexingExecutionContext)
    {
      if (!(indexingExecutionContext.IndexingUnit.IndexingUnitType == "ScopedIndexingUnit"))
        return (indexingExecutionContext.RepositoryIndexingUnit.Properties as GitCodeRepoIndexingProperties).BranchIndexInfo.ToDictionary<KeyValuePair<string, GitBranchIndexInfo>, string, string>((Func<KeyValuePair<string, GitBranchIndexInfo>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, GitBranchIndexInfo>, string>) (kvp => kvp.Value.LastIndexedCommitId));
      return indexingExecutionContext.IndexingUnit.Properties is ScopedGitRepositoryIndexingProperties properties && properties.BranchIndexInfo != null ? properties.BranchIndexInfo.ToDictionary<KeyValuePair<string, ScopedGitBranchIndexInfo>, string, string>((Func<KeyValuePair<string, ScopedGitBranchIndexInfo>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, ScopedGitBranchIndexInfo>, string>) (kvp => kvp.Value.CommitId)) : new Dictionary<string, string>();
    }

    internal virtual Dictionary<string, string> GetBranchToIndexedCommitIdMapRepoLevel(
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> scopedIndexingUnits)
    {
      Dictionary<string, string> commitIdMapRepoLevel = new Dictionary<string, string>();
      Dictionary<string, HashSet<string>> dictionary = new Dictionary<string, HashSet<string>>();
      HashSet<string> stringSet = (HashSet<string>) null;
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit scopedIndexingUnit in scopedIndexingUnits)
      {
        foreach (KeyValuePair<string, string> keyValuePair in !(scopedIndexingUnit.Properties is ScopedGitRepositoryIndexingProperties properties) || properties.BranchIndexInfo == null ? new Dictionary<string, string>() : properties.BranchIndexInfo.ToDictionary<KeyValuePair<string, ScopedGitBranchIndexInfo>, string, string>((Func<KeyValuePair<string, ScopedGitBranchIndexInfo>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, ScopedGitBranchIndexInfo>, string>) (kvp => kvp.Value.CommitId)))
        {
          string key = keyValuePair.Key;
          string lastIndexCommitId = keyValuePair.Value;
          if (string.IsNullOrWhiteSpace(lastIndexCommitId))
            lastIndexCommitId = RepositoryConstants.DefaultLastIndexCommitId;
          if (dictionary.TryGetValue(key, out stringSet))
            stringSet.Add(lastIndexCommitId);
          else
            dictionary.Add(key, new HashSet<string>()
            {
              lastIndexCommitId
            });
        }
      }
      foreach (KeyValuePair<string, HashSet<string>> keyValuePair in dictionary)
      {
        if (keyValuePair.Value.Count == 1)
          commitIdMapRepoLevel.Add(keyValuePair.Key, keyValuePair.Value.FirstOrDefault<string>());
      }
      return commitIdMapRepoLevel;
    }

    internal virtual List<string> GetBranchesBeingIndexedForFirstTime(
      IndexingExecutionContext iexContext,
      GitCodeRepoIndexingProperties gitCodeRepoIndexingProperties,
      List<string> branchesToBeIndexed)
    {
      List<string> indexedForFirstTime = new List<string>();
      if (gitCodeRepoIndexingProperties != null && gitCodeRepoIndexingProperties.BranchIndexInfo != null && branchesToBeIndexed != null)
      {
        foreach (string key in branchesToBeIndexed)
        {
          if (!gitCodeRepoIndexingProperties.BranchIndexInfo.ContainsKey(key) || string.IsNullOrWhiteSpace(gitCodeRepoIndexingProperties.BranchIndexInfo[key].LastIndexedCommitId) || gitCodeRepoIndexingProperties.BranchIndexInfo[key].LastIndexedCommitId == RepositoryConstants.DefaultLastIndexCommitId)
            indexedForFirstTime.Add(key);
        }
      }
      if (iexContext != null && indexedForFirstTime.Count > 0)
        iexContext.Log.Append("\nBranches being indexed for first time: " + indexedForFirstTime.Count.ToString((IFormatProvider) CultureInfo.CurrentCulture));
      return indexedForFirstTime;
    }

    internal virtual List<string> GetBranchesToRemoveFromRegistry(
      IndexingExecutionContext iexContext,
      GitCodeRepoIndexingProperties gitCodeRepoIndexingProperties,
      List<string> branchesToBeIndexed,
      List<string> branchesToBeDeleted)
    {
      List<string> removeFromRegistry = branchesToBeDeleted ?? new List<string>();
      if (gitCodeRepoIndexingProperties != null && gitCodeRepoIndexingProperties.BranchIndexInfo != null && branchesToBeIndexed != null)
      {
        foreach (string key in branchesToBeIndexed)
        {
          if (gitCodeRepoIndexingProperties.BranchIndexInfo.ContainsKey(key) && !string.IsNullOrWhiteSpace(gitCodeRepoIndexingProperties.BranchIndexInfo[key].LastIndexedCommitId) && gitCodeRepoIndexingProperties.BranchIndexInfo[key].LastIndexedCommitId != RepositoryConstants.DefaultLastIndexCommitId && !removeFromRegistry.Contains(key))
            removeFromRegistry.Add(key);
        }
      }
      if (iexContext != null && removeFromRegistry.Count > 0)
        iexContext.Log.Append("\nBranches being removed from registry: " + removeFromRegistry.Count.ToString((IFormatProvider) CultureInfo.CurrentCulture));
      return removeFromRegistry;
    }

    internal virtual void AddToRegistry(
      IndexingExecutionContext iexContext,
      string projectName,
      string repoName,
      List<string> branchesBeingIndexedForFirstTime,
      string traceLayer)
    {
      RegistryManagerV2 registryManager = new RegistryManagerV2(iexContext.RequestContext, traceLayer);
      foreach (string branchName in branchesBeingIndexedForFirstTime)
      {
        string registryKey = CodeBranchInformationHelperForLargeRepos.GetRegistryKey(projectName, repoName, branchName);
        if (registryKey != null)
          CodeBranchInformationHelperForLargeRepos.AddBranchAsIndexing(registryManager, registryKey, iexContext.RequestContext.GetCollectionID().ToString());
      }
    }

    internal virtual void ClearCompletedRegistryValues(
      IndexingExecutionContext iexContext,
      string projectName,
      string repoName,
      List<string> branchesToRemoveAfterGettingIndexed,
      string traceLayer)
    {
      RegistryManagerV2 registryManager = new RegistryManagerV2(iexContext.RequestContext, traceLayer);
      foreach (string branchName in branchesToRemoveAfterGettingIndexed)
      {
        string registryKey = CodeBranchInformationHelperForLargeRepos.GetRegistryKey(projectName, repoName, branchName);
        if (registryKey != null)
          CodeBranchInformationHelperForLargeRepos.DeleteBranchFromIndexingList(registryManager, registryKey, iexContext.RequestContext.GetCollectionID().ToString());
      }
    }

    internal virtual Dictionary<string, GitIndexJobYieldData> GetBranchToJobYieldDataMap(
      IndexingExecutionContext indexingExecutionContext)
    {
      if (!(indexingExecutionContext.IndexingUnit.IndexingUnitType == "ScopedIndexingUnit"))
        return new Dictionary<string, GitIndexJobYieldData>();
      return indexingExecutionContext.IndexingUnit.Properties is ScopedGitRepositoryIndexingProperties properties && properties.BranchIndexInfo != null ? properties.BranchIndexInfo.ToDictionary<KeyValuePair<string, ScopedGitBranchIndexInfo>, string, GitIndexJobYieldData>((Func<KeyValuePair<string, ScopedGitBranchIndexInfo>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, ScopedGitBranchIndexInfo>, GitIndexJobYieldData>) (kvp => kvp.Value.GitIndexJobYieldData)) : new Dictionary<string, GitIndexJobYieldData>();
    }

    internal virtual List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> GetScopedIndexingUnits(
      IndexingExecutionContext indexingExecutionContext)
    {
      return indexingExecutionContext.IndexingUnitDataAccess.GetIndexingUnitsWithGivenParent(indexingExecutionContext.RequestContext, indexingExecutionContext.RepositoryIndexingUnit.IndexingUnitId, -1).Where<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, bool>) (x => x.IndexingUnitType == "ScopedIndexingUnit")).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
    }

    internal virtual void DeleteFilesWithMaxRetriesExhausted(
      IndexingExecutionContext indexingExecutionContext,
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> scopedIndexingUnits)
    {
      short configValue = indexingExecutionContext.GetConfigValue<short>("/Service/ALMSearch/Settings/LargeRepositoryMaxFileLevelRetryLimit");
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit scopedIndexingUnit in scopedIndexingUnits)
      {
        ITempFileMetadataStoreDataAccess metadataStoreDataAccess = this.GetTempFileMetadataStoreDataAccess(scopedIndexingUnit);
        DocumentContractType contractType = indexingExecutionContext.ProvisioningContext.ContractType;
        string indexingUnitType = indexingExecutionContext.IndexingUnit.IndexingUnitType;
        IEnumerable<TempFileMetadataRecord> withMinAttemptCount = metadataStoreDataAccess.GetFilesWithMinAttemptCount(indexingExecutionContext.RequestContext, configValue, indexingUnitType, contractType);
        if (withMinAttemptCount != null && withMinAttemptCount.Any<TempFileMetadataRecord>())
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Moving all the {0} files for which the retry had exhausted to Failure Record Store.", (object) withMinAttemptCount.Count<TempFileMetadataRecord>())));
          IEnumerable<ItemLevelFailureRecord> levelFailureRecords = this.GetItemLevelFailureRecords(indexingExecutionContext, withMinAttemptCount);
          new ItemLevelFailureDataAccess().MergeFailedRecords(indexingExecutionContext.RequestContext, scopedIndexingUnit, levelFailureRecords);
          metadataStoreDataAccess.DeleteTempFileMetadataRecords(indexingExecutionContext.RequestContext, withMinAttemptCount.Select<TempFileMetadataRecord, long>((Func<TempFileMetadataRecord, long>) (x => x.Id)));
        }
      }
    }

    internal virtual IEnumerable<ItemLevelFailureRecord> GetItemLevelFailureRecords(
      IndexingExecutionContext indexingExecutionContext,
      IEnumerable<TempFileMetadataRecord> records)
    {
      IDictionary<FileAttributes, TempFileMetadataRecord> dictionary = (IDictionary<FileAttributes, TempFileMetadataRecord>) new Dictionary<FileAttributes, TempFileMetadataRecord>();
      foreach (TempFileMetadataRecord record in records)
        dictionary[record.FileAttributes] = record;
      List<ItemLevelFailureRecord> levelFailureRecords = new List<ItemLevelFailureRecord>();
      foreach (TempFileMetadataRecord record in (IEnumerable<TempFileMetadataRecord>) dictionary.Values)
      {
        Microsoft.VisualStudio.Services.Search.Common.Entities.Branches fileMetadataRecord = this.GetBranchesInTempFileMetadataRecord(record);
        levelFailureRecords.Add(new ItemLevelFailureRecord()
        {
          AttemptCount = (int) record.AttemptCount,
          Item = record.FileAttributes.NormalizedFilePath,
          Metadata = (FailureMetadata) new FileFailureMetadata()
          {
            Branches = fileMetadataRecord
          }
        });
      }
      return (IEnumerable<ItemLevelFailureRecord>) levelFailureRecords;
    }

    internal virtual Microsoft.VisualStudio.Services.Search.Common.Entities.Branches GetBranchesInTempFileMetadataRecord(
      TempFileMetadataRecord record)
    {
      HashSet<string> collection = new HashSet<string>();
      foreach (Dictionary<MetaDataStoreUpdateType, List<BranchInfo>> dictionary in record.TemporaryBranchMetadata.BranchMetadata.Values)
      {
        foreach (List<BranchInfo> source in dictionary.Values)
          collection.AddRange<string, HashSet<string>>(source.Select<BranchInfo, string>((Func<BranchInfo, string>) (x => x.BranchName)));
      }
      Microsoft.VisualStudio.Services.Search.Common.Entities.Branches fileMetadataRecord = new Microsoft.VisualStudio.Services.Search.Common.Entities.Branches();
      fileMetadataRecord.AddRange((IEnumerable<string>) collection);
      return fileMetadataRecord;
    }

    internal virtual ITempFileMetadataStoreDataAccess GetTempFileMetadataStoreDataAccess(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit scopedIndexingUnit)
    {
      return (ITempFileMetadataStoreDataAccess) new TempFileMetadataStoreDataAccess(scopedIndexingUnit);
    }

    internal virtual Dictionary<string, ScopedGitBranchIndexInfo> GetCurrentBranchInfoObject(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit scopedIndexingUnit,
      Dictionary<string, GitCommit> branchesToTopCommits,
      Dictionary<string, List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>> branchToScopedIndexingUnits)
    {
      Dictionary<string, ScopedGitBranchIndexInfo> branchInfoObject = new Dictionary<string, ScopedGitBranchIndexInfo>();
      foreach (KeyValuePair<string, List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>> scopedIndexingUnit1 in branchToScopedIndexingUnits)
      {
        string key = scopedIndexingUnit1.Key;
        if (scopedIndexingUnit1.Value.Contains(scopedIndexingUnit))
          branchInfoObject[key] = new ScopedGitBranchIndexInfo()
          {
            CommitId = branchesToTopCommits[key].CommitId
          };
      }
      return branchInfoObject;
    }

    internal virtual HashSet<string> GetBranchesInCompleteSync(
      Dictionary<string, string> branchToIndexedCommitIdMap,
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> currentScopedIndexingUnits)
    {
      HashSet<string> branchesInCompleteSync = new HashSet<string>();
      foreach (KeyValuePair<string, string> toIndexedCommitId in branchToIndexedCommitIdMap)
      {
        string key = toIndexedCommitId.Key;
        string str = toIndexedCommitId.Value;
        if (!string.IsNullOrWhiteSpace(str) && !(str == RepositoryConstants.DefaultLastIndexCommitId) && this.Branches.Contains(key))
        {
          List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitList = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
          foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit scopedIndexingUnit in currentScopedIndexingUnits)
          {
            ScopedGitRepositoryIndexingProperties properties = scopedIndexingUnit.Properties as ScopedGitRepositoryIndexingProperties;
            if (properties.BranchIndexInfo.ContainsKey(key) && !string.IsNullOrWhiteSpace(properties.BranchIndexInfo[key].CommitId) && properties.BranchIndexInfo[key].CommitId != RepositoryConstants.DefaultLastIndexCommitId)
              indexingUnitList.Add(scopedIndexingUnit);
          }
          bool flag = true;
          foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in indexingUnitList)
          {
            if ((indexingUnit.Properties as ScopedGitRepositoryIndexingProperties).BranchIndexInfo[key].CommitId != str)
            {
              flag = false;
              break;
            }
          }
          if (flag)
            branchesInCompleteSync.Add(key);
        }
      }
      return branchesInCompleteSync;
    }

    internal virtual IDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int> GetFailedItemsCountBreakup(
      IndexingExecutionContext indexingExecutionContext,
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> scopedIndexingUnits)
    {
      if (scopedIndexingUnits == null || !scopedIndexingUnits.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
        return (IDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int>) new Dictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int>();
      IDictionary<int, int> recordsByIndexingUnit = indexingExecutionContext.ItemLevelFailureDataAccess.GetCountOfRecordsByIndexingUnit(indexingExecutionContext.RequestContext);
      IDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int> itemsCountBreakup = (IDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int>) new Dictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int>();
      Dictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> dictionary = scopedIndexingUnits.ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int>) (x => x.IndexingUnitId));
      foreach (KeyValuePair<int, int> keyValuePair in (IEnumerable<KeyValuePair<int, int>>) recordsByIndexingUnit)
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit key;
        if (dictionary.TryGetValue(keyValuePair.Key, out key))
          itemsCountBreakup[key] = keyValuePair.Value;
      }
      return itemsCountBreakup;
    }

    internal GitCodeRepoIndexingProperties UpdateRepositoryLastProcessedTime(
      IndexingExecutionContext iexContext,
      GitCodeRepoIndexingProperties gitCodeRepoIndexingProperties,
      DateTime dateTime)
    {
      if (gitCodeRepoIndexingProperties != null)
      {
        gitCodeRepoIndexingProperties.RepositoryLastProcessedTime = dateTime.ToUniversalTime();
        iexContext?.Log.Append("\nRepositoryLastProcessedTime updated to: " + dateTime.ToUniversalTime().ToString((IFormatProvider) CultureInfo.CurrentCulture));
      }
      return gitCodeRepoIndexingProperties;
    }

    internal GitCodeRepoIndexingProperties UpdateBranchLastProcessedTimeForBranchesInSync(
      IndexingExecutionContext iexContext,
      GitCodeRepoIndexingProperties gitCodeRepoIndexingProperties,
      DateTime repositoryLastProcessedTime,
      Dictionary<string, string> branchToIndexedCommitIdMap)
    {
      if (gitCodeRepoIndexingProperties != null && !branchToIndexedCommitIdMap.IsNullOrEmpty<KeyValuePair<string, string>>())
      {
        int count = branchToIndexedCommitIdMap.Count;
        int num = 0;
        foreach (string key in branchToIndexedCommitIdMap.Keys.ToList<string>())
        {
          if (gitCodeRepoIndexingProperties.BranchIndexInfo.ContainsKey(key))
          {
            gitCodeRepoIndexingProperties.BranchIndexInfo[key].BranchLastProcessedTime = repositoryLastProcessedTime;
          }
          else
          {
            gitCodeRepoIndexingProperties.BranchIndexInfo[key] = new GitBranchIndexInfo()
            {
              LastIndexedCommitId = branchToIndexedCommitIdMap[key],
              BranchLastProcessedTime = repositoryLastProcessedTime
            };
            ++num;
            iexContext?.Log.Append("\nAdding dropped branch: " + key + " to Indexing Properties");
          }
        }
        iexContext?.Log.Append("\nBranchLastProcessedTime = " + repositoryLastProcessedTime.ToString((IFormatProvider) CultureInfo.CurrentCulture) + " updated in " + count.ToString() + " branches\nLastIndexedCommitId updated in " + num.ToString() + " branches");
      }
      return gitCodeRepoIndexingProperties;
    }

    internal bool RemoveBranchMappingsToDeletedFolders(
      IndexingExecutionContext iexContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit scopedIndexingUnit,
      List<string> branches)
    {
      bool deletedFolders = false;
      if (iexContext == null || branches == null || branches.Count == 0 || scopedIndexingUnit == null)
        return deletedFolders;
      GitHttpClientWrapper httpClientWrapper = this.GetGitHttpClientWrapper(iexContext);
      string scopePath = (scopedIndexingUnit.TFSEntityAttributes as ScopedGitRepositoryAttributes).ScopePath;
      ScopedGitRepositoryIndexingProperties properties = scopedIndexingUnit.Properties as ScopedGitRepositoryIndexingProperties;
      foreach (string branch in branches)
      {
        if (properties.BranchIndexInfo.ContainsKey(branch) && properties.BranchIndexInfo[branch].CommitId == RepositoryConstants.DefaultLastIndexCommitId)
        {
          GitCommit topCommit = this.GetTopCommit(iexContext, branch);
          if (topCommit != null)
          {
            GitVersionDescriptor versionDescriptor = new GitVersionDescriptor()
            {
              Version = topCommit.CommitId,
              VersionType = GitVersionType.Commit
            };
            if (this.IsFolderDeletedInBranch(httpClientWrapper, scopePath, versionDescriptor))
            {
              properties.BranchIndexInfo.Remove(branch);
              deletedFolders = true;
              iexContext.Log.Append("Branch mapping removed in BranchIndexInfo as this folder is deleted: \nScopedIndexingUnit Id: " + scopedIndexingUnit.IndexingUnitId.ToString() + "\nBranch Name: " + branch + "\nScopePath: " + scopePath);
            }
          }
        }
      }
      return deletedFolders;
    }

    internal virtual bool IsFolderDeletedInBranch(
      GitHttpClientWrapper gitHttpClientWrapper,
      string scopePath,
      GitVersionDescriptor versionDescriptor)
    {
      if (scopePath != "/")
      {
        try
        {
          gitHttpClientWrapper.GetItemAsync(scopePath, versionDescriptor);
        }
        catch (Exception ex)
        {
          if (ex.ToString().Contains("TF401174"))
            return true;
        }
      }
      return false;
    }
  }
}
