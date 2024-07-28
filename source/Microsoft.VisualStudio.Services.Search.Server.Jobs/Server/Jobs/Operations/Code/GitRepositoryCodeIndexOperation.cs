// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.GitRepositoryCodeIndexOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.ChangeEventData;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Crawler;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Crawler.StalenessAnalyzer;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class GitRepositoryCodeIndexOperation : RepositoryCodeIndexingOperation
  {
    private GitRepositoryStalenessAnalyzer m_gitRepositoryStalenessAnalyzer;

    public GitRepositoryCodeIndexOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : this(executionContext, indexingUnit, indexingUnitChangeEvent, new RegistryManager(executionContext.RequestContext, "IndexingOperation"), new GitRepositoryStalenessAnalyzer(new GitHttpClientWrapper(executionContext, new TraceMetaData(1080613, "Indexing Pipeline", "IndexingOperation"))))
    {
    }

    internal GitRepositoryCodeIndexOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      RegistryManager registryManager,
      GitRepositoryStalenessAnalyzer gitRepositoryStalenessAnalyzer)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent, new TraceMetaData(1080613, "Indexing Pipeline", "IndexingOperation"))
    {
      this.RegistryManager = registryManager;
      this.m_gitRepositoryStalenessAnalyzer = gitRepositoryStalenessAnalyzer;
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(this.TraceMetaData.TracePoint, this.TraceMetaData.TraceArea, this.TraceMetaData.TraceLayer, nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        if (!executionContext.IsIndexingEnabled())
        {
          executionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Indexing is disabled for this {0}", (object) executionContext.RepositoryIndexingUnit.ToString())));
          operationResult.Status = OperationStatus.Succeeded;
          return operationResult;
        }
        if (coreIndexingExecutionContext.IndexingUnit.IndexingUnitType == "Git_Repository")
        {
          GitCodeRepoTFSAttributes entityAttributes = executionContext.RepositoryIndexingUnit.TFSEntityAttributes as GitCodeRepoTFSAttributes;
          GitCodeRepoIndexingProperties properties1 = executionContext.RepositoryIndexingUnit.Properties as GitCodeRepoIndexingProperties;
          List<string> nextSetOfBranches = this.GetNextSetOfBranches(executionContext, entityAttributes);
          List<string> stringList1 = new List<string>();
          List<string> stringList2 = new List<string>();
          if (nextSetOfBranches != null && nextSetOfBranches.Any<string>())
          {
            foreach (string key in nextSetOfBranches)
            {
              if (properties1 != null && properties1.BranchIndexInfo != null)
              {
                GitBranchIndexInfo gitBranchIndexInfo;
                if (properties1.BranchIndexInfo.TryGetValue(key, out gitBranchIndexInfo))
                {
                  if (gitBranchIndexInfo.IsDefaultLastIndexedCommitId())
                    stringList1.Add(key);
                  else
                    stringList2.Add(key);
                }
                else
                  stringList1.Add(key);
              }
              else
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TraceMetaData, "repoIndexingProperties or BranchIndexInfo is null");
            }
            coreIndexingExecutionContext.ExecutionTracerContext.PublishKpi("BulkIndexing", "Indexing Pipeline", (double) stringList1.Count, true);
            coreIndexingExecutionContext.ExecutionTracerContext.PublishKpi("ContinuousIndexing", "Indexing Pipeline", (double) stringList2.Count, true);
            if (stringList2.Count > 0 && !executionContext.IsCrudOperationsFeatureEnabled())
            {
              coreIndexingExecutionContext.Log.Append("CI feature flag is off, hence not processing the CI.");
              return operationResult;
            }
          }
          if (coreIndexingExecutionContext.IndexingUnit.IsLargeRepository(coreIndexingExecutionContext.RequestContext))
          {
            int num = 0;
            Guid guid;
            if (coreIndexingExecutionContext.IndexingUnit.IndexingUnitType == "Git_Repository")
            {
              Stopwatch stopwatch = Stopwatch.StartNew();
              Dictionary<string, string> branchToCommitId;
              Dictionary<string, CodeIndexingStalenessData> dictionary = this.FetchStalenessForGitLargeRepository((ExecutionContext) coreIndexingExecutionContext, coreIndexingExecutionContext.IndexingUnit, out branchToCommitId);
              stopwatch.Stop();
              executionContext.Log.Append("\nTime taken by FetchStalenessForGitLargeRepository in ms: " + stopwatch.ElapsedMilliseconds.ToString());
              List<CustomerIntelligenceData> indexingStalenessCiData = this.GetCodeIndexingStalenessCIData(coreIndexingExecutionContext.IndexingUnit, dictionary, true);
              this.PublishCITelemetry((ExecutionContext) coreIndexingExecutionContext, indexingStalenessCiData);
              if (coreIndexingExecutionContext.IndexingUnit.Properties is GitCodeRepoIndexingProperties properties2)
              {
                num = properties2.BranchIndexInfo.Count;
                this.UpdateRepositoryIndexingPropertiesForLargeRepo(properties2, dictionary, branchToCommitId);
              }
              int count = properties2.BranchIndexInfo.Count;
              if (num != count)
              {
                CoreIndexingExecutionContext.OutputLog log = executionContext.Log;
                string[] strArray = new string[8]
                {
                  "\nBranch mismatch in Indexing Properties: \nBranches before getting staleness: ",
                  num.ToString(),
                  "\nBranches after getting staleness: ",
                  count.ToString(),
                  "\nRepository Id: ",
                  null,
                  null,
                  null
                };
                guid = executionContext.RepositoryIndexingUnit.TFSEntityId;
                strArray[5] = guid.ToString();
                strArray[6] = "\nRepository Name: ";
                strArray[7] = properties2.Name;
                string s = string.Concat(strArray);
                log.Append(s);
              }
              this.IndexingUnitDataAccess.UpdateIndexingUnit(coreIndexingExecutionContext.RequestContext, coreIndexingExecutionContext.IndexingUnit);
            }
            if (nextSetOfBranches != null && nextSetOfBranches.Any<string>())
            {
              if (OperationStatus.PartiallySucceeded == this.ExecuteCrawlerParserAndFeeder(executionContext, string.Empty, nextSetOfBranches).OperationStatus)
                operationResult.Status = OperationStatus.Succeeded;
              Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
              {
                IndexingUnitId = executionContext.RepositoryIndexingUnit.IndexingUnitId,
                ChangeData = (Microsoft.VisualStudio.Services.Search.Common.ChangeEventData) new CodeBulkIndexEventData((ExecutionContext) coreIndexingExecutionContext),
                ChangeType = this.IndexingUnitChangeEvent.ChangeType,
                State = IndexingUnitChangeEventState.Pending,
                AttemptCount = 0
              };
              Microsoft.VisualStudio.Services.Search.Common.ChangeEventData changeData = indexingUnitChangeEvent.ChangeData;
              guid = Guid.NewGuid();
              string str = guid.ToString();
              changeData.CorrelationId = str;
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Queued {0} to process remaining branches", (object) this.IndexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeeded((ExecutionContext) coreIndexingExecutionContext, indexingUnitChangeEvent))));
            }
            else
            {
              coreIndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("No more branches left to index. Hence returning.")));
              return operationResult;
            }
          }
          else if (OperationStatus.PartiallySucceeded == this.ExecuteCrawlerParserAndFeeder(executionContext, string.Empty, nextSetOfBranches).OperationStatus)
            operationResult.Status = OperationStatus.Succeeded;
          if (this.WorkerPipeline == null || this.WorkerPipeline.IsPrimaryRun())
            this.UpdateSecurityInformation(executionContext, executionContext.ProjectIndexingUnit);
          FriendlyDictionary<string, object> properties3 = new FriendlyDictionary<string, object>()
          {
            ["GitBranchesForBulkIndexing"] = (object) stringList1,
            ["GitBranchesForContinuousIndexing"] = (object) stringList2
          };
          coreIndexingExecutionContext.ExecutionTracerContext.PublishClientTrace("Indexing Pipeline", "IndexingOperation", (IDictionary<string, object>) properties3);
          string empty = string.Empty;
          if (stringList1.Count > 0)
            empty += FormattableString.Invariant(FormattableStringFactory.Create("Number of branches for Bulk Indexing: {0}. ", (object) stringList1.Count));
          if (stringList2.Count > 0)
            empty += FormattableString.Invariant(FormattableStringFactory.Create("Number of branches for Continuous Indexing: {0}.", (object) stringList2.Count));
          coreIndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully indexed {0} with Id {1}. {2}", (object) executionContext.RepositoryIndexingUnit.IndexingUnitType, (object) executionContext.RepositoryIndexingUnit.TFSEntityId, (object) empty)));
        }
        else if (coreIndexingExecutionContext.IndexingUnit.IndexingUnitType == "ScopedIndexingUnit")
        {
          Dictionary<string, ScopedGitBranchIndexInfo> branchIndexInfo = (this.IndexingUnitChangeEvent.ChangeData as LargeRepositoryMetadataCrawlerEventData).BranchIndexInfo;
          CorePipelineResult corePipelineResult = this.ExecuteCrawlerParserAndFeeder(executionContext, string.Empty, branchIndexInfo.Keys.ToList<string>());
          operationResult.Status = corePipelineResult.OperationStatus;
          if (OperationStatus.PartiallySucceeded == operationResult.Status)
          {
            operationResult.Status = OperationStatus.Succeeded;
            return operationResult;
          }
        }
        else if (coreIndexingExecutionContext.IndexingUnit.IndexingUnitType == "TemporaryIndexingUnit" && OperationStatus.PartiallySucceeded == this.ExecuteCrawlerParserAndFeeder(executionContext, string.Empty, new List<string>()).OperationStatus)
        {
          operationResult.Status = OperationStatus.Succeeded;
          return operationResult;
        }
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(this.TraceMetaData.TracePoint, this.TraceMetaData.TraceArea, this.TraceMetaData.TraceLayer, nameof (RunOperation));
      }
      return operationResult;
    }

    internal override CodeCrawlSpec CreateCrawlSpec(
      IndexingExecutionContext iexContext,
      ref string branchName,
      in List<string> branches)
    {
      GitCodeRepoTFSAttributes entityAttributes = iexContext.RepositoryIndexingUnit.TFSEntityAttributes as GitCodeRepoTFSAttributes;
      if (iexContext.IndexingUnit.IndexingUnitType == "TemporaryIndexingUnit")
      {
        FileSplitGroupData changeData = this.IndexingUnitChangeEvent.ChangeData as FileSplitGroupData;
        return (CodeCrawlSpec) FileGroupCrawlSpec.Create(iexContext, changeData.StartingId, changeData.LastId, changeData.TakeCount, entityAttributes.DefaultBranch, entityAttributes);
      }
      if (iexContext.IndexingUnit.IndexingUnitType == "ScopedIndexingUnit")
        return (CodeCrawlSpec) GitIndexCrawlSpec.Create(iexContext, entityAttributes, iexContext.RepositoryIndexingUnit.TFSEntityId, branchName, RepositoryConstants.BranchCreationOrDeletionCommitId, (List<string>) null, (string) null);
      if (iexContext.IndexingUnit.IndexingUnitType == "Git_Repository")
      {
        if (iexContext.IndexingUnit.IsLargeRepository(iexContext.RequestContext))
          return (CodeCrawlSpec) GitIndexCrawlSpec.Create(iexContext, entityAttributes, iexContext.RepositoryIndexingUnit.TFSEntityId, string.Empty, RepositoryConstants.BranchCreationOrDeletionCommitId, (List<string>) null, (string) null);
        if (!string.IsNullOrWhiteSpace(branchName) && branches != null && branches.Count > 0)
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Both BranchName and Braches(list) fields are populated in crawlspec")));
        if (!string.IsNullOrWhiteSpace(branchName))
        {
          branches.Clear();
          branches.Add(branchName);
          branchName = (string) null;
        }
        return (CodeCrawlSpec) GitIndexCrawlSpec.Create(iexContext, entityAttributes, iexContext.RepositoryIndexingUnit.TFSEntityId, string.Empty, RepositoryConstants.BranchCreationOrDeletionCommitId, branches, (string) null);
      }
      throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Unsupported IndexingUnitType {0} for GitRepositoryCodeBulkIndexOperation.", (object) iexContext.IndexingUnit.IndexingUnitType)));
    }

    internal override CodeIndexingPipelineContext GetPipelineContext(
      IndexingExecutionContext iexContext,
      string branchName,
      List<string> branches)
    {
      CodeCrawlSpec crawlSpec = this.CreateCrawlSpec(iexContext, ref branchName, in branches);
      bool isSingleExecutionPipeline = iexContext.RepositoryIndexingUnit.IsLargeRepository(iexContext.RequestContext);
      return new CodeIndexingPipelineContext(isSingleExecutionPipeline ? iexContext.IndexingUnit : iexContext.RepositoryIndexingUnit, iexContext, crawlSpec, this.IndexingUnitChangeEvent, this.IndexingUnitChangeEventHandler, branchName, branches, this.SupportsStoringFiles, isSingleExecutionPipeline);
    }

    internal override CodeIndexingPipeline GetPipeline(CodeIndexingPipelineContext pipelineContext)
    {
      IndexingExecutionContext executionContext = pipelineContext.IndexingExecutionContext;
      if (executionContext.RepositoryIndexingUnit.IsLargeRepository(executionContext.RequestContext))
        this.WorkerPipeline = (CodeIndexingPipeline) new FileGroupPipeline(pipelineContext);
      else
        this.WorkerPipeline = (CodeIndexingPipeline) new GitRepositoryIndexingPipeline(pipelineContext);
      return this.WorkerPipeline;
    }

    internal virtual List<string> GetNextSetOfBranches(
      IndexingExecutionContext indexingExecutionContext,
      GitCodeRepoTFSAttributes repoAttributes)
    {
      List<string> nextSetOfBranches = repoAttributes.BranchesToIndex;
      if (indexingExecutionContext.RepositoryIndexingUnit.IsLargeRepository(indexingExecutionContext.RequestContext))
      {
        int configValue = indexingExecutionContext.GetConfigValue<int>("/Service/ALMSearch/Settings/LargeRepositoryNumberOfBranchesForPeriodicRefresh");
        RegistryEntry registryEntry = this.RegistryManager.GetRegistryEntry("LargeRepositoryMultipeBranchIndexing", indexingExecutionContext.RepositoryIndexingUnit.TFSEntityId.ToString()) ?? this.RegistryManager.GetRegistryEntry("LargeRepositoryMultipeBranchIndexing", indexingExecutionContext.RepositoryIndexingUnit.IndexingUnitId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        int index = registryEntry != null ? registryEntry.GetValue<int>() : 0;
        List<string> stringList = new List<string>((IEnumerable<string>) repoAttributes.BranchesToIndex);
        stringList.Sort();
        int val2 = stringList.Count - index;
        int count = Math.Min(configValue, val2);
        if (count <= 0)
        {
          this.RegistryManager.RemoveRegistryEntry("LargeRepositoryMultipeBranchIndexing", indexingExecutionContext.RepositoryIndexingUnit.TFSEntityId.ToString());
          this.RegistryManager.RemoveRegistryEntry("LargeRepositoryMultipeBranchIndexing", indexingExecutionContext.RepositoryIndexingUnit.IndexingUnitId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          if (indexingExecutionContext.IsCrudOperationsFeatureEnabled())
          {
            if (indexingExecutionContext.IsReindexingFailedOrInProgress(this.DataAccessFactory, indexingExecutionContext.IndexingUnit.EntityType) && indexingExecutionContext.RequestContext.IsCodeReindexingWithZeroStalenessFeatureEnabled())
            {
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Not queueing another change event to periodically refresh index as ZLRI is in-progress.")));
            }
            else
            {
              int num = indexingExecutionContext.RepositoryIndexingUnit.IsShadow ? indexingExecutionContext.GetConfigValue<int>("/Service/ALMSearch/Settings/LargeRepositoryPeriodicRefreshDelayInSeconds") : indexingExecutionContext.GetConfigValue<int>("/Service/ALMSearch/Settings/LargeRepositoryReIndexingPeriodicRefreshDelayInSeconds");
              CodeBulkIndexEventData bulkIndexEventData1 = new CodeBulkIndexEventData((ExecutionContext) indexingExecutionContext);
              bulkIndexEventData1.Delay = TimeSpan.FromSeconds((double) num);
              bulkIndexEventData1.Trigger = 31;
              CodeBulkIndexEventData bulkIndexEventData2 = bulkIndexEventData1;
              Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
              {
                IndexingUnitId = indexingExecutionContext.RepositoryIndexingUnit.IndexingUnitId,
                ChangeData = (Microsoft.VisualStudio.Services.Search.Common.ChangeEventData) bulkIndexEventData2,
                ChangeType = this.IndexingUnitChangeEvent.ChangeType,
                State = IndexingUnitChangeEventState.Pending,
                AttemptCount = 0
              };
              indexingUnitChangeEvent.ChangeData.CorrelationId = Guid.NewGuid().ToString();
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Queued {0} to process next set of commits with a delay of {1} seconds.", (object) this.IndexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeeded((ExecutionContext) indexingExecutionContext, indexingUnitChangeEvent), (object) num)));
            }
          }
          else
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Not queueing another change event to periodically refresh index as CI FF is off.")));
          return new List<string>();
        }
        nextSetOfBranches = stringList.GetRange(index, count);
        this.CreateRegistryEntry(indexingExecutionContext, index + nextSetOfBranches.Count);
      }
      else if (this.IndexingUnitChangeEvent.ChangeData is GitRepositoryBIEventData changeData && changeData.BranchesToBeBulkIndexed != null && changeData.BranchesToBeBulkIndexed.Any<string>())
        nextSetOfBranches = changeData.BranchesToBeBulkIndexed;
      return nextSetOfBranches;
    }

    internal virtual void CreateRegistryEntry(
      IndexingExecutionContext indexingExecutionContext,
      int value)
    {
      this.RegistryManager.RemoveRegistryEntry("LargeRepositoryMultipeBranchIndexing", indexingExecutionContext.RepositoryIndexingUnit.TFSEntityId.ToString());
      this.RegistryManager.AddOrUpdateRegistryValue("LargeRepositoryMultipeBranchIndexing", indexingExecutionContext.RepositoryIndexingUnit.IndexingUnitId.ToString((IFormatProvider) CultureInfo.InvariantCulture), value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    internal virtual void UpdateSecurityInformation(
      IndexingExecutionContext indexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit projectIndexingUnit)
    {
      GitCodeRepoIndexingProperties properties = indexingExecutionContext.RepositoryIndexingUnit.Properties as GitCodeRepoIndexingProperties;
      RepositorySecurityAcesUpdater securityAcesUpdater = new RepositorySecurityAcesUpdater((ExecutionContext) indexingExecutionContext);
      byte[] currentTfsSecHash = (byte[]) null;
      try
      {
        currentTfsSecHash = securityAcesUpdater.GetSecurityHashCodeForRepository(indexingExecutionContext.RepositoryIndexingUnit.TFSEntityId, projectIndexingUnit.TFSEntityId);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Security Hash computation failed for repo Id '{0}' with exception {1}", (object) indexingExecutionContext.RepositoryIndexingUnit.TFSEntityId, (object) ex)));
      }
      if (!SecurityChecksUtils.ShouldUpdateHash(properties.SecurityHashcode, currentTfsSecHash))
        return;
      properties.SecurityHashcode = currentTfsSecHash;
      indexingExecutionContext.RepositoryIndexingUnit = this.IndexingUnitDataAccess.UpdateIndexingUnit(indexingExecutionContext.RequestContext, indexingExecutionContext.RepositoryIndexingUnit);
    }

    internal virtual Dictionary<string, CodeIndexingStalenessData> FetchStalenessForGitLargeRepository(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit gitLargeRepository,
      out Dictionary<string, string> branchToCommitId)
    {
      if (gitLargeRepository == null)
        throw new ArgumentException("gitLargeRepository is null");
      if (executionContext == null)
        throw new ArgumentException("executionContext is null");
      Dictionary<string, CodeIndexingStalenessData> dictionary1 = new Dictionary<string, CodeIndexingStalenessData>();
      branchToCommitId = new Dictionary<string, string>();
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> unitsWithGivenParent = this.IndexingUnitDataAccess.GetIndexingUnitsWithGivenParent(executionContext.RequestContext, gitLargeRepository.IndexingUnitId, -1);
      DateTime utcNow = DateTime.UtcNow;
      Dictionary<GitRepositoryCodeIndexOperation.BranchCommitIdKey, CodeIndexingStalenessData> dictionary2 = new Dictionary<GitRepositoryCodeIndexOperation.BranchCommitIdKey, CodeIndexingStalenessData>();
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in unitsWithGivenParent)
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit scopedIndexingUnit = indexingUnit;
        if (!(scopedIndexingUnit.Properties is ScopedGitRepositoryIndexingProperties properties))
          throw new InvalidCastException("scopedIndexingUnit does not have expected type of IndexingProperties i.e. ScopedGitRepositoryIndexingProperties.");
        foreach (KeyValuePair<string, ScopedGitBranchIndexInfo> keyValuePair in properties.BranchIndexInfo)
        {
          string branch = keyValuePair.Key;
          string lastIndexedCommitIdInScopePath = keyValuePair.Value.CommitId;
          GitRepositoryCodeIndexOperation.BranchCommitIdKey branchCommitIdKey = new GitRepositoryCodeIndexOperation.BranchCommitIdKey(branch, lastIndexedCommitIdInScopePath);
          bool flag = executionContext.RequestContext.IsFeatureEnabled("Search.Server.Code.FetchStalenessRetrials");
          CodeIndexingStalenessData codeIndexingStalenessData;
          if (!dictionary2.TryGetValue(branchCommitIdKey, out codeIndexingStalenessData) || codeIndexingStalenessData.FirstUnprocessedPushInfo.PushId == -3L && !flag)
          {
            Stopwatch stopwatch = Stopwatch.StartNew();
            codeIndexingStalenessData = this.m_gitRepositoryStalenessAnalyzer.GetStalenessData(gitLargeRepository.TFSEntityId, branch, lastIndexedCommitIdInScopePath, utcNow);
            stopwatch.Stop();
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080631, "Indexing Pipeline", "LargeRepoStalenessAnalyzer", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Time taken by GetStalenessData in milliSec: {0} for branch: {1}", (object) stopwatch.ElapsedMilliseconds, (object) branch));
            dictionary2[branchCommitIdKey] = codeIndexingStalenessData;
            try
            {
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1083158, this.TraceMetaData.TraceArea, this.TraceMetaData.TraceLayer, (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "BranchCommitInfo => branch: {0}, commitId: {1}, pushId: {2}, scopedUnit: {3}, hashCode: {4}", (object) branch, (object) lastIndexedCommitIdInScopePath, (object) codeIndexingStalenessData.FirstUnprocessedPushInfo.PushId, (object) scopedIndexingUnit.TFSEntityId, (object) branchCommitIdKey.GetHashCode())));
            }
            catch (Exception ex)
            {
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TraceMetaData, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GitLargeRepoStaleness tracer failed with exception: {0}", (object) ex));
            }
          }
          if (codeIndexingStalenessData != null)
          {
            CodeIndexingStalenessData indexingStalenessData;
            if (!dictionary1.TryGetValue(branch, out indexingStalenessData))
            {
              dictionary1[branch] = codeIndexingStalenessData;
              branchToCommitId[branch] = lastIndexedCommitIdInScopePath;
            }
            else if (indexingStalenessData.FirstUnprocessedPushInfo.PushId == -3L)
            {
              dictionary1[branch] = codeIndexingStalenessData;
              branchToCommitId[branch] = lastIndexedCommitIdInScopePath;
            }
            else if (codeIndexingStalenessData.FirstUnprocessedPushInfo.PushId != -2L && codeIndexingStalenessData.FirstUnprocessedPushInfo.PushId != -3L && codeIndexingStalenessData.FirstUnprocessedPushInfo.PushId < indexingStalenessData.FirstUnprocessedPushInfo.PushId)
            {
              dictionary1[branch] = codeIndexingStalenessData;
              branchToCommitId[branch] = lastIndexedCommitIdInScopePath;
            }
          }
        }
      }
      return dictionary1;
    }

    internal virtual List<CustomerIntelligenceData> GetCodeIndexingStalenessCIData(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit gitRepository,
      Dictionary<string, CodeIndexingStalenessData> branchStalenessData,
      bool isLargeRepository = false)
    {
      List<CustomerIntelligenceData> indexingStalenessCiData = new List<CustomerIntelligenceData>();
      foreach (KeyValuePair<string, CodeIndexingStalenessData> keyValuePair in branchStalenessData)
      {
        string key = keyValuePair.Key;
        CodeIndexingStalenessData indexingStalenessData = keyValuePair.Value;
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
        intelligenceData.Add("CIProcessingDelaySource", "GitRepositoryCodeIndexingOperation");
        intelligenceData.Add("IsGitLargeRepository", isLargeRepository);
        intelligenceData.Add("IndexingUnitType", gitRepository.IndexingUnitType);
        intelligenceData.Add("IsShadow", gitRepository.IsShadow.ToString());
        intelligenceData.Add("RepositoryId", gitRepository.TFSEntityId.ToString());
        intelligenceData.Add("BranchName", key);
        intelligenceData.Add("LatestPushId", (double) indexingStalenessData.LatestPushInfo.PushId);
        intelligenceData.Add("LatestPushTime", (object) indexingStalenessData.LatestPushInfo.PushTime);
        intelligenceData.Add("LastProcessedPushId", (double) indexingStalenessData.LastProcessedPushInfo.PushId);
        intelligenceData.Add("LastProcessedPushTime", (object) indexingStalenessData.LastProcessedPushInfo.PushTime);
        intelligenceData.Add("FirstUnprocessedPushId", (double) indexingStalenessData.FirstUnprocessedPushInfo.PushId);
        intelligenceData.Add("FirstUnprocessedPushTime", (object) indexingStalenessData.FirstUnprocessedPushInfo.PushTime);
        intelligenceData.Add("ReferenceTimeForStalenessMeasurement", (object) indexingStalenessData.ReferenceTimeForStaleness);
        intelligenceData.Add("CIProcessingDelayInMiliseconds", indexingStalenessData.CIProcessingDelayInMiliseconds);
        indexingStalenessCiData.Add(intelligenceData);
      }
      return indexingStalenessCiData;
    }

    internal virtual void PublishCITelemetry(
      ExecutionContext executionContext,
      List<CustomerIntelligenceData> ciDataList)
    {
      foreach (CustomerIntelligenceData ciData in ciDataList)
        executionContext.ExecutionTracerContext.PublishCi(this.TraceMetaData.TraceArea, this.TraceMetaData.TraceLayer, ciData);
    }

    internal virtual void UpdateRepositoryIndexingPropertiesForLargeRepo(
      GitCodeRepoIndexingProperties gitCodeRepoIndexingProperties,
      Dictionary<string, CodeIndexingStalenessData> repositoryBranchStalenessData,
      Dictionary<string, string> branchToCommitId)
    {
      Dictionary<string, GitBranchIndexInfo> branchIndexInfo = gitCodeRepoIndexingProperties.BranchIndexInfo;
      gitCodeRepoIndexingProperties.BranchIndexInfo = new Dictionary<string, GitBranchIndexInfo>();
      foreach (KeyValuePair<string, CodeIndexingStalenessData> keyValuePair in repositoryBranchStalenessData)
      {
        string key = keyValuePair.Key;
        CodeIndexingStalenessData indexingStalenessData = keyValuePair.Value;
        if (indexingStalenessData.LastProcessedPushInfo.PushId < 0L)
        {
          if (branchIndexInfo.ContainsKey(key))
          {
            gitCodeRepoIndexingProperties.BranchIndexInfo[key] = branchIndexInfo[key];
            if (branchToCommitId.ContainsKey(key))
              gitCodeRepoIndexingProperties.BranchIndexInfo[key].LastIndexedCommitId = branchToCommitId[key];
          }
          else
            gitCodeRepoIndexingProperties.BranchIndexInfo[key] = new GitBranchIndexInfo()
            {
              LastIndexedCommitId = RepositoryConstants.DefaultLastIndexCommitId,
              LastIndexedCommitUtcTime = RepositoryConstants.DefaultLastIndexChangeUtcTime
            };
        }
        else
          gitCodeRepoIndexingProperties.BranchIndexInfo[key] = new GitBranchIndexInfo()
          {
            LastIndexedCommitId = branchToCommitId[key],
            LastIndexedCommitUtcTime = indexingStalenessData.LastProcessedPushInfo.PushTime
          };
        if (branchIndexInfo.ContainsKey(key) && gitCodeRepoIndexingProperties.BranchIndexInfo.ContainsKey(key))
          gitCodeRepoIndexingProperties.BranchIndexInfo[key].BranchLastProcessedTime = branchIndexInfo[key].BranchLastProcessedTime;
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080629, "Indexing Pipeline", "IndexingOperation", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "LastIndexedCommitId updated to:{0}", (object) gitCodeRepoIndexingProperties.BranchIndexInfo[key].LastIndexedCommitId));
      }
    }

    public class BranchCommitIdKey : IEquatable<GitRepositoryCodeIndexOperation.BranchCommitIdKey>
    {
      public string BranchName { get; set; }

      public string CommitId { get; set; }

      public BranchCommitIdKey(string branchName, string commitId)
      {
        this.BranchName = branchName;
        this.CommitId = commitId;
      }

      public override bool Equals(object obj) => this.Equals(obj as GitRepositoryCodeIndexOperation.BranchCommitIdKey);

      public bool Equals(
        GitRepositoryCodeIndexOperation.BranchCommitIdKey other)
      {
        return other != null && this.BranchName == other.BranchName && this.CommitId == other.CommitId;
      }

      public override int GetHashCode() => (1412216455 * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.BranchName)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.CommitId);
    }
  }
}
