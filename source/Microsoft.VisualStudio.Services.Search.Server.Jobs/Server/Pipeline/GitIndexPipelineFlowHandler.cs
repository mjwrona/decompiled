// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.GitIndexPipelineFlowHandler
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  internal class GitIndexPipelineFlowHandler : CorePipelineFlowHandler
  {
    internal GitIndexPipelineFlowHandler(Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit, TraceMetaData traceMetaData)
      : base(indexingUnit, traceMetaData)
    {
    }

    public override void Prepare(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      IndexingExecutionContext indexingExecutionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      (indexingExecutionContext.RepositoryIndexingUnit.TFSEntityAttributes as GitCodeRepoTFSAttributes).RemoteUrl = this.FetchGitRepositoryFromTfs(indexingExecutionContext).RemoteUrl;
      indexingExecutionContext.RepositoryIndexingUnit = coreIndexingExecutionContext.IndexingUnitDataAccess.UpdateIndexingUnit(coreIndexingExecutionContext.RequestContext, indexingExecutionContext.RepositoryIndexingUnit);
      this.UpdateIndexingUnitIfRequired(indexingExecutionContext.RepositoryIndexingUnit);
    }

    public override void PrePipelineRun(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      CoreCrawlSpec crawlSpec)
    {
      if (crawlSpec == null)
      {
        Tracer.TraceError(this.TraceMetaData, "Crawlspec is null");
        throw new ArgumentException("Crawlspec is null");
      }
      if (!(crawlSpec is GitCrawlSpec gitCrawlSpec))
      {
        Tracer.TraceError(this.TraceMetaData, "Crawlspec is not of type GitCrawlSpec");
        throw new ArgumentException("Crawlspec is not of type GitCrawlSpec");
      }
      IndexingExecutionContext indexingExecutionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      GitCodeRepoIndexingProperties properties = indexingExecutionContext.RepositoryIndexingUnit.Properties as GitCodeRepoIndexingProperties;
      if (!string.IsNullOrWhiteSpace(gitCrawlSpec.BranchName))
      {
        this.InitializeDataInCrawlSpec(indexingExecutionContext, gitCrawlSpec);
        this.ReadAndUpdateIndexingProperties(gitCrawlSpec.BranchName, properties);
        this.ReadAndUpdateCrawlSpec(indexingExecutionContext, gitCrawlSpec, properties);
      }
      if (gitCrawlSpec.CurrentBranchesInfo != null)
      {
        this.InitializeDataInCrawlSpec(indexingExecutionContext, gitCrawlSpec);
        foreach (BranchInfo branchInfo in gitCrawlSpec.CurrentBranchesInfo)
        {
          this.ReadAndUpdateIndexingProperties(branchInfo.BranchName, properties);
          this.ReadAndUpdateCrawlSpec(indexingExecutionContext, gitCrawlSpec, branchInfo, properties);
        }
      }
      crawlSpec.ItemsProcessedAcrossYields = properties.GitIndexJobYieldStats.ItemsProcessedAcrossYields;
      crawlSpec.JobYieldCount = properties.GitIndexJobYieldStats.YieldCount;
      indexingExecutionContext.RepositoryIndexingUnit = coreIndexingExecutionContext.IndexingUnitDataAccess.UpdateIndexingUnit(coreIndexingExecutionContext.RequestContext, indexingExecutionContext.RepositoryIndexingUnit);
      this.UpdateIndexingUnitIfRequired(indexingExecutionContext.RepositoryIndexingUnit);
      Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("PreRunReadIndexingProperties: GitIndex on IndexingUnit: {0} with CrawlSpec: {1}", (object) indexingExecutionContext.RepositoryIndexingUnit, (object) gitCrawlSpec)));
    }

    internal void InitializeDataInCrawlSpec(
      IndexingExecutionContext indexingExecutionContext,
      GitCrawlSpec gitCrawlSpec)
    {
      GitCodeRepoIndexingProperties properties = indexingExecutionContext.RepositoryIndexingUnit.Properties as GitCodeRepoIndexingProperties;
      if (!string.IsNullOrWhiteSpace(gitCrawlSpec.BranchName))
      {
        GitBranchIndexInfo gitBranchIndexInfo;
        if (properties.BranchIndexInfo.TryGetValue(gitCrawlSpec.BranchName, out gitBranchIndexInfo))
          ((CodeCrawlSpec) gitCrawlSpec).LastIndexedChangeId = gitBranchIndexInfo.LastIndexedCommitId;
        else
          ((CodeCrawlSpec) gitCrawlSpec).LastIndexedChangeId = RepositoryConstants.DefaultLastIndexCommitId;
      }
      if (gitCrawlSpec.CurrentBranchesInfo == null)
        return;
      List<BranchInfo> branchInfoList = new List<BranchInfo>();
      gitCrawlSpec.BranchesToJobYieldDataInfo = new Dictionary<string, GitIndexJobYieldData>();
      Dictionary<string, string> dictionary = properties.BranchIndexInfo.ToDictionary<KeyValuePair<string, GitBranchIndexInfo>, string, string>((Func<KeyValuePair<string, GitBranchIndexInfo>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, GitBranchIndexInfo>, string>) (kvp => kvp.Value.LastIndexedCommitId));
      foreach (BranchInfo branchInfo in gitCrawlSpec.CurrentBranchesInfo)
      {
        string str;
        if (dictionary.TryGetValue(branchInfo.BranchName, out str))
          branchInfoList.Add(new BranchInfo()
          {
            BranchName = branchInfo.BranchName,
            ChangeId = str
          });
        else
          branchInfoList.Add(new BranchInfo()
          {
            BranchName = branchInfo.BranchName,
            ChangeId = RepositoryConstants.DefaultLastIndexCommitId
          });
        gitCrawlSpec.BranchesToJobYieldDataInfo.Add(branchInfo.BranchName, new GitIndexJobYieldData());
      }
      gitCrawlSpec.LastIndexedBranchesInfo = branchInfoList;
    }

    internal void ReadAndUpdateIndexingProperties(
      string branchName,
      GitCodeRepoIndexingProperties repoIndexingProperties)
    {
      if (!repoIndexingProperties.BranchIndexInfo.ContainsKey(branchName))
        repoIndexingProperties.BranchIndexInfo[branchName] = new GitBranchIndexInfo();
      if (repoIndexingProperties.BranchIndexInfo[branchName].GitIndexJobYieldData != null)
        return;
      repoIndexingProperties.BranchIndexInfo[branchName].GitIndexJobYieldData = new GitIndexJobYieldData();
    }

    internal void ReadAndUpdateCrawlSpec(
      IndexingExecutionContext indexingExecutionContext,
      GitCrawlSpec gitCrawlSpec,
      GitCodeRepoIndexingProperties repoIndexingProperties)
    {
      if (repoIndexingProperties.BranchIndexInfo[gitCrawlSpec.BranchName].GitIndexJobYieldData.HasData())
      {
        ((CoreCrawlSpec) gitCrawlSpec).JobYieldData = repoIndexingProperties.BranchIndexInfo[gitCrawlSpec.BranchName].GitIndexJobYieldData.Clone();
        ((CoreCrawlSpec) gitCrawlSpec).JobYieldData.InitCrawlResumeCounters();
      }
      else
      {
        if (repoIndexingProperties.BranchIndexInfo[gitCrawlSpec.BranchName].BulkIndexJobYieldData == null || !repoIndexingProperties.BranchIndexInfo[gitCrawlSpec.BranchName].BulkIndexJobYieldData.HasData())
          return;
        repoIndexingProperties.BranchIndexInfo[gitCrawlSpec.BranchName].GitIndexJobYieldData.CopyFrom(repoIndexingProperties.BranchIndexInfo[gitCrawlSpec.BranchName].BulkIndexJobYieldData);
        repoIndexingProperties.BranchIndexInfo[gitCrawlSpec.BranchName].BulkIndexJobYieldData = new GitBulkIndexJobYieldData();
        indexingExecutionContext.RepositoryIndexingUnit = indexingExecutionContext.IndexingUnitDataAccess.UpdateIndexingUnit(indexingExecutionContext.RequestContext, indexingExecutionContext.RepositoryIndexingUnit);
        this.UpdateIndexingUnitIfRequired(indexingExecutionContext.RepositoryIndexingUnit);
        ((CoreCrawlSpec) gitCrawlSpec).JobYieldData = repoIndexingProperties.BranchIndexInfo[gitCrawlSpec.BranchName].GitIndexJobYieldData.Clone();
        ((CoreCrawlSpec) gitCrawlSpec).JobYieldData.InitCrawlResumeCounters();
      }
    }

    internal void ReadAndUpdateCrawlSpec(
      IndexingExecutionContext indexingExecutionContext,
      GitCrawlSpec gitCrawlSpec,
      BranchInfo branchInfo,
      GitCodeRepoIndexingProperties repoIndexingProperties)
    {
      if (repoIndexingProperties.BranchIndexInfo[branchInfo.BranchName].BulkIndexJobYieldData != null && repoIndexingProperties.BranchIndexInfo[branchInfo.BranchName].BulkIndexJobYieldData.HasData())
      {
        repoIndexingProperties.BranchIndexInfo[branchInfo.BranchName].GitIndexJobYieldData.CopyFrom(repoIndexingProperties.BranchIndexInfo[branchInfo.BranchName].BulkIndexJobYieldData);
        repoIndexingProperties.BranchIndexInfo[branchInfo.BranchName].BulkIndexJobYieldData = new GitBulkIndexJobYieldData();
        indexingExecutionContext.RepositoryIndexingUnit = indexingExecutionContext.IndexingUnitDataAccess.UpdateIndexingUnit(indexingExecutionContext.RequestContext, indexingExecutionContext.RepositoryIndexingUnit);
        this.UpdateIndexingUnitIfRequired(indexingExecutionContext.RepositoryIndexingUnit);
        ((CoreCrawlSpec) gitCrawlSpec).JobYieldData = repoIndexingProperties.BranchIndexInfo[gitCrawlSpec.BranchName].GitIndexJobYieldData.Clone();
        ((CoreCrawlSpec) gitCrawlSpec).JobYieldData.InitCrawlResumeCounters();
      }
      else
      {
        GitIndexJobYieldData indexJobYieldData = repoIndexingProperties.BranchIndexInfo[branchInfo.BranchName].GitIndexJobYieldData.Clone() as GitIndexJobYieldData;
        if (indexJobYieldData.HasData())
          indexJobYieldData.InitCrawlResumeCounters();
        if (gitCrawlSpec.BranchesToJobYieldDataInfo.ContainsKey(branchInfo.BranchName))
          gitCrawlSpec.BranchesToJobYieldDataInfo[branchInfo.BranchName] = indexJobYieldData;
        else
          gitCrawlSpec.BranchesToJobYieldDataInfo.Add(branchInfo.BranchName, indexJobYieldData);
      }
    }

    public override void PostPipelineRun(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      CoreCrawlSpec crawlSpec)
    {
      if (crawlSpec == null)
      {
        Tracer.TraceError(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Crawlspec is null")));
        throw new ArgumentException("Crawlspec is null");
      }
      if (!(crawlSpec is GitCrawlSpec gitCrawlSpec))
      {
        Tracer.TraceError(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Crawlspec is not of type GitCrawlSpec")));
        throw new ArgumentException("Crawlspec is not of type GitCrawlSpec");
      }
      IndexingExecutionContext indexingExecutionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      GitCodeRepoIndexingProperties properties = indexingExecutionContext.RepositoryIndexingUnit.Properties as GitCodeRepoIndexingProperties;
      bool flag1 = false;
      DateTime utcNow = DateTime.UtcNow;
      if (!string.IsNullOrWhiteSpace(gitCrawlSpec.BranchName) && !((CoreCrawlSpec) gitCrawlSpec).JobYieldData.IncompleteTreeCrawl)
      {
        this.UpdateLastIndexedCommitInfo(gitCrawlSpec.BranchName, ((CodeCrawlSpec) gitCrawlSpec).LastIndexedChangeId, ((CodeCrawlSpec) gitCrawlSpec).LastIndexedChangeUtcTime, utcNow, properties);
        this.UpdateLastIndexedBranchesInfo(gitCrawlSpec, properties, utcNow);
      }
      if (!string.IsNullOrWhiteSpace(gitCrawlSpec.BranchName))
      {
        if (((CoreCrawlSpec) gitCrawlSpec).JobYieldData.IncompleteTreeCrawl)
        {
          flag1 = true;
          properties.BranchIndexInfo[gitCrawlSpec.BranchName].GitIndexJobYieldData = (GitIndexJobYieldData) ((CoreCrawlSpec) gitCrawlSpec).JobYieldData.Clone();
        }
        else
          properties.BranchIndexInfo[gitCrawlSpec.BranchName].GitIndexJobYieldData = new GitIndexJobYieldData();
      }
      if (gitCrawlSpec.LastIndexedBranchesInfo != null)
      {
        foreach (BranchInfo branchInfo in gitCrawlSpec.LastIndexedBranchesInfo)
        {
          GitIndexJobYieldData indexJobYieldData;
          if (gitCrawlSpec.BranchesToJobYieldDataInfo != null && gitCrawlSpec.BranchesToJobYieldDataInfo.TryGetValue(branchInfo.BranchName, out indexJobYieldData))
          {
            if (!indexJobYieldData.IncompleteTreeCrawl)
            {
              this.UpdateLastIndexedCommitInfo(branchInfo.BranchName, branchInfo.ChangeId, branchInfo.ChangeTime, utcNow, properties);
              properties.BranchIndexInfo[branchInfo.BranchName].GitIndexJobYieldData = new GitIndexJobYieldData();
            }
            else
            {
              flag1 = true;
              properties.BranchIndexInfo[branchInfo.BranchName].GitIndexJobYieldData = (GitIndexJobYieldData) gitCrawlSpec.BranchesToJobYieldDataInfo[branchInfo.BranchName].Clone();
            }
          }
          else
          {
            Tracer.TraceError(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("BranchesToJobYieldDataInfo do not have data for branch {0}", (object) branchInfo.BranchName)));
            throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("BranchesToJobYieldDataInfo do not have data for branch {0}", (object) branchInfo.BranchName)));
          }
        }
      }
      if (flag1)
      {
        ++crawlSpec.JobYieldCount;
        properties.GitIndexJobYieldStats.ItemsProcessedAcrossYields = crawlSpec.ItemsProcessedAcrossYields;
        properties.GitIndexJobYieldStats.YieldCount = crawlSpec.JobYieldCount;
      }
      else
      {
        properties.GitIndexJobYieldStats.ItemsProcessedAcrossYields = 0;
        properties.GitIndexJobYieldStats.YieldCount = 0;
        bool currentHostConfigValue = coreIndexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/Routing/SizeBasedCustomRoutingEnabled", true);
        bool flag2 = true;
        if (gitCrawlSpec.BranchesToJobYieldDataInfo != null)
        {
          if (gitCrawlSpec.BranchesToJobYieldDataInfo.Count != properties.BranchIndexInfo.Count)
          {
            flag2 = false;
          }
          else
          {
            foreach (KeyValuePair<string, GitIndexJobYieldData> keyValuePair in gitCrawlSpec.BranchesToJobYieldDataInfo)
            {
              if (!string.IsNullOrWhiteSpace(keyValuePair.Value.BaseVersion) && keyValuePair.Value.BaseVersion != RepositoryConstants.DefaultLastIndexCommitId)
              {
                flag2 = false;
                break;
              }
            }
          }
        }
        if (flag2 & currentHostConfigValue)
          this.UpdateIndexingUnitDetails(indexingExecutionContext, crawlSpec.ItemsProcessedAcrossYields);
        if (!indexingExecutionContext.RepositoryIndexingUnit.IsLargeRepository(indexingExecutionContext.RequestContext))
          this.PublishIndexingCompletionSLA((CoreIndexingExecutionContext) indexingExecutionContext, crawlSpec);
      }
      indexingExecutionContext.RepositoryIndexingUnit = coreIndexingExecutionContext.IndexingUnitDataAccess.UpdateIndexingUnit(coreIndexingExecutionContext.RequestContext, indexingExecutionContext.RepositoryIndexingUnit);
      this.UpdateIndexingUnitIfRequired(indexingExecutionContext.RepositoryIndexingUnit);
      Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("PostRunUpdateIndexingProperties: GitIndex on IndexingUnit: {0} with CrawlSpec: {1}", (object) indexingExecutionContext.RepositoryIndexingUnit, (object) gitCrawlSpec)));
    }

    internal void UpdateLastIndexedBranchesInfo(
      GitCrawlSpec gitCrawlSpec,
      GitCodeRepoIndexingProperties repoIndexingProperties,
      DateTime branchLastProcessedTime)
    {
      if (gitCrawlSpec.LastIndexedBranchesInfo == null)
        return;
      foreach (BranchInfo branchInfo in gitCrawlSpec.LastIndexedBranchesInfo)
      {
        GitBranchIndexInfo gitBranchIndexInfo;
        if (!repoIndexingProperties.BranchIndexInfo.TryGetValue(branchInfo.BranchName, out gitBranchIndexInfo))
        {
          gitBranchIndexInfo = new GitBranchIndexInfo();
          repoIndexingProperties.BranchIndexInfo[branchInfo.BranchName] = gitBranchIndexInfo;
        }
        if (string.IsNullOrEmpty(branchInfo.ChangeId))
          gitBranchIndexInfo.BranchLastProcessedTime = RepositoryConstants.DefaultLastIndexChangeUtcTime;
        else if (gitBranchIndexInfo.LastIndexedCommitId != branchInfo.ChangeId)
          gitBranchIndexInfo.BranchLastProcessedTime = branchLastProcessedTime;
        gitBranchIndexInfo.LastIndexedCommitId = string.IsNullOrEmpty(branchInfo.ChangeId) ? RepositoryConstants.DefaultLastIndexCommitId : branchInfo.ChangeId;
        gitBranchIndexInfo.LastIndexedCommitUtcTime = branchInfo.ChangeTime == DateTime.MinValue ? RepositoryConstants.DefaultLastIndexChangeUtcTime : branchInfo.ChangeTime;
      }
    }

    internal void UpdateLastIndexedCommitInfo(
      string branchName,
      string lastIndexedChangeId,
      DateTime lastIndexedChangeUtcTime,
      DateTime branchLastProcessedTime,
      GitCodeRepoIndexingProperties repoIndexingProperties)
    {
      if (string.IsNullOrWhiteSpace(branchName))
        return;
      GitBranchIndexInfo gitBranchIndexInfo;
      if (!repoIndexingProperties.BranchIndexInfo.TryGetValue(branchName, out gitBranchIndexInfo))
      {
        gitBranchIndexInfo = new GitBranchIndexInfo();
        repoIndexingProperties.BranchIndexInfo[branchName] = gitBranchIndexInfo;
      }
      if (string.IsNullOrEmpty(lastIndexedChangeId))
        gitBranchIndexInfo.BranchLastProcessedTime = RepositoryConstants.DefaultLastProcessedTime;
      else if (gitBranchIndexInfo.LastIndexedCommitId != lastIndexedChangeId)
        gitBranchIndexInfo.BranchLastProcessedTime = branchLastProcessedTime;
      gitBranchIndexInfo.LastIndexedCommitId = string.IsNullOrEmpty(lastIndexedChangeId) ? RepositoryConstants.DefaultLastIndexCommitId : lastIndexedChangeId;
      gitBranchIndexInfo.LastIndexedCommitUtcTime = lastIndexedChangeUtcTime == DateTime.MinValue ? RepositoryConstants.DefaultLastIndexChangeUtcTime : lastIndexedChangeUtcTime;
    }

    internal virtual void UpdateIndexingUnitDetails(
      IndexingExecutionContext indexingExecutionContext,
      int totalDocumentsIndexed)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit repositoryIndexingUnit = indexingExecutionContext.RepositoryIndexingUnit;
      IndexingUnitDetails indexingUnitDetails = indexingExecutionContext.IndexingUnitDataAccess.GetIndexingUnitDetails(indexingExecutionContext.RequestContext, repositoryIndexingUnit);
      if (indexingUnitDetails == null)
        return;
      string indexName = indexingExecutionContext.CollectionIndexingUnit.GetIndexInfo().IndexName;
      string clusterName = indexingExecutionContext.ProvisioningContext.SearchClusterManagementService.GetClusterName();
      indexingUnitDetails.TFSEntityId = new Guid?(indexingExecutionContext.RepositoryIndexingUnit.TFSEntityId);
      indexingUnitDetails.EntityType = indexingExecutionContext.RepositoryIndexingUnit.EntityType;
      indexingUnitDetails.IsShadow = indexingExecutionContext.RepositoryIndexingUnit.IsShadow;
      indexingUnitDetails.IndexingUnitType = indexingExecutionContext.RepositoryIndexingUnit.IndexingUnitType;
      indexingUnitDetails.ESClusterName = clusterName;
      indexingUnitDetails.IndexName = indexName;
      float currentHostConfigValue = indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<float>("/Service/ALMSearch/Settings/Routing/Code/CodeEntityGitRepositoryGrowthFactor", true, 0.3f);
      int estimatedDocCountGrowth;
      long estimatedSize;
      long estimatedSizeGrowth;
      GitHttpClientWrapper.GetGitRepoSizeEstimates(indexingExecutionContext.ProvisioningContext.ContractType.GetShardDensity(indexingExecutionContext.RequestContext), currentHostConfigValue, totalDocumentsIndexed, out estimatedDocCountGrowth, out estimatedSize, out estimatedSizeGrowth);
      indexingUnitDetails.EstimatedInitialDocCount = totalDocumentsIndexed;
      indexingUnitDetails.EstimatedDocCountGrowth = estimatedDocCountGrowth;
      indexingUnitDetails.EstimatedInitialSize = estimatedSize;
      indexingUnitDetails.EstimatedSizeGrowth = estimatedSizeGrowth;
      indexingExecutionContext.IndexingUnitDataAccess.AddOrResetIndexingUnitDetailsAndUpdateShardDetails(indexingExecutionContext.RequestContext, (IList<IndexingUnitDetails>) new List<IndexingUnitDetails>(1)
      {
        indexingUnitDetails
      }, (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>(1)
      {
        indexingExecutionContext.RepositoryIndexingUnit
      });
    }

    internal virtual GitRepository FetchGitRepositoryFromTfs(
      IndexingExecutionContext indexingExecutionContext)
    {
      return new GitHttpClientWrapper((ExecutionContext) indexingExecutionContext, this.TraceMetaData).GetRepositoryAsync(indexingExecutionContext.RepositoryIndexingUnit.TFSEntityId);
    }

    private void UpdateIndexingUnitIfRequired(Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      if (indexingUnit.IndexingUnitId != this.IndexingUnit.IndexingUnitId)
        return;
      this.IndexingUnit = indexingUnit;
    }
  }
}
