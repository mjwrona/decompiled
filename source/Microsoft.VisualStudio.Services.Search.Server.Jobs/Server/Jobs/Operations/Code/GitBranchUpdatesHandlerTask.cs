// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.GitBranchUpdatesHandlerTask
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.V2Jobs.Common.Git;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class GitBranchUpdatesHandlerTask
  {
    [StaticSafe("Grandfathered")]
    private static readonly TraceMetaData s_traceMetadata = new TraceMetaData(1080648, "Indexing Pipeline", "IndexingOperation");
    private readonly GitRepoSyncAnalyzer m_gitRepoSyncAnalyzer;
    private readonly Microsoft.VisualStudio.Services.Search.Common.IndexingUnit m_indexingUnit;
    private readonly IIndexingUnitDataAccess m_indexingUnitDataAccess;
    private readonly Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent m_indexingUnitChangeEvent;

    public GitBranchUpdatesHandlerTask(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IIndexingUnitDataAccess indexingUnitDataAccess,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      GitRepoSyncAnalyzer gitRepoSyncAnalyzer)
    {
      this.m_indexingUnit = indexingUnit;
      this.m_indexingUnitDataAccess = indexingUnitDataAccess;
      this.m_indexingUnitChangeEvent = indexingUnitChangeEvent;
      this.m_gitRepoSyncAnalyzer = gitRepoSyncAnalyzer;
    }

    public string HandleGitBranchUpdates(IndexingExecutionContext indexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(GitBranchUpdatesHandlerTask.s_traceMetadata, nameof (HandleGitBranchUpdates));
      StringBuilder stringBuilder = new StringBuilder();
      try
      {
        List<string> searchConfiguredBranches = this.GetSearchConfiguredBranches(indexingExecutionContext);
        GitCodeRepoTFSAttributes repoTFSEntityAttributes = this.m_indexingUnit.TFSEntityAttributes as GitCodeRepoTFSAttributes;
        GitCodeRepoIndexingProperties repoIndexingProperties = this.m_indexingUnit.Properties as GitCodeRepoIndexingProperties;
        if (repoIndexingProperties != null && repoIndexingProperties.BranchIndexInfo != null)
        {
          HashSet<string> stringSet1 = new HashSet<string>();
          stringSet1.UnionWith((IEnumerable<string>) searchConfiguredBranches.FindAll((Predicate<string>) (x => !repoIndexingProperties.BranchIndexInfo.ContainsKey(x) || repoIndexingProperties.BranchIndexInfo[x].IsDefaultLastIndexedCommitId())));
          HashSet<string> stringSet2 = new HashSet<string>();
          stringSet2.UnionWith((IEnumerable<string>) repoIndexingProperties.BranchIndexInfo.Keys.ToList<string>().FindAll((Predicate<string>) (x => !searchConfiguredBranches.Contains(x) && x != repoTFSEntityAttributes.DefaultBranch)));
          string defaultBranch = repoTFSEntityAttributes.DefaultBranch;
          string newDefaultBranch;
          bool flag = this.HasDefaultBranchChanged(indexingExecutionContext, defaultBranch, out newDefaultBranch);
          if (flag)
          {
            List<string> branchesToBeBulkIndexed;
            List<string> branchesToBeDeleted;
            this.HandleDefaultBranchChange(indexingExecutionContext, newDefaultBranch, searchConfiguredBranches, out branchesToBeBulkIndexed, out branchesToBeDeleted);
            stringSet1.UnionWith((IEnumerable<string>) branchesToBeBulkIndexed);
            stringSet2.UnionWith((IEnumerable<string>) branchesToBeDeleted);
            repoTFSEntityAttributes.DefaultBranch = newDefaultBranch;
          }
          if (repoTFSEntityAttributes.IdentifyBranchChanges(searchConfiguredBranches) | flag)
          {
            repoTFSEntityAttributes.Branches = searchConfiguredBranches;
            this.m_indexingUnitDataAccess.UpdateIndexingUnit(indexingExecutionContext.RequestContext, this.m_indexingUnit);
            stringBuilder.AppendLine(FormattableString.Invariant(FormattableStringFactory.Create("Updated IndexingUnit [{0}] CollectionId: [{1}] with branches [{2}].", (object) this.m_indexingUnit.ToString(), (object) indexingExecutionContext.RequestContext.GetCollectionID(), (object) string.Join(",", (IEnumerable<string>) searchConfiguredBranches))));
            if (flag)
              stringBuilder.AppendLine(FormattableString.Invariant(FormattableStringFactory.Create("Updated default branch to {0} from {1}", (object) repoTFSEntityAttributes.DefaultBranch, (object) defaultBranch)));
          }
          if (stringSet1.Count > 0 && !this.m_indexingUnit.IsLargeRepository(indexingExecutionContext.RequestContext))
          {
            Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = this.m_gitRepoSyncAnalyzer.QueueGitRepoBIOperation((ExecutionContext) indexingExecutionContext, this.m_indexingUnit, stringSet1, this.m_indexingUnitChangeEvent.LeaseId);
            stringBuilder.AppendLine(FormattableString.Invariant(FormattableStringFactory.Create("Queued git BI opeation [{0}] to index branches [{1}].", (object) indexingUnitChangeEvent.ToString(), (object) string.Join(",", (IEnumerable<string>) stringSet1))));
          }
          if (stringSet2.Count > 0)
          {
            Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = this.m_gitRepoSyncAnalyzer.QueueGitBranchDeleteOperation((ExecutionContext) indexingExecutionContext, this.m_indexingUnit, (HashSet<string>) null, true, this.m_indexingUnitChangeEvent.LeaseId);
            stringBuilder.AppendLine(FormattableString.Invariant(FormattableStringFactory.Create("Queued git branch delete operaion [{0}].", (object) indexingUnitChangeEvent.ToString())));
          }
          if (string.IsNullOrWhiteSpace(stringBuilder.ToString()))
            stringBuilder.AppendLine("Processed git branch updates successfully and found no changes.");
        }
        else
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(GitBranchUpdatesHandlerTask.s_traceMetadata, "repoIndexingProperties or BranchIndexInfo is null");
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(GitBranchUpdatesHandlerTask.s_traceMetadata, nameof (HandleGitBranchUpdates));
      }
      return stringBuilder.ToString();
    }

    internal virtual void HandleDefaultBranchChange(
      IndexingExecutionContext indexingExecutionContext,
      string newDefaultBranch,
      List<string> searchEnabledBranches,
      out List<string> branchesToBeBulkIndexed,
      out List<string> branchesToBeDeleted)
    {
      GitCodeRepoTFSAttributes entityAttributes = this.m_indexingUnit.TFSEntityAttributes as GitCodeRepoTFSAttributes;
      GitCodeRepoIndexingProperties properties = this.m_indexingUnit.Properties as GitCodeRepoIndexingProperties;
      string defaultBranch = entityAttributes.DefaultBranch;
      branchesToBeBulkIndexed = new List<string>();
      branchesToBeDeleted = new List<string>();
      List<string> branchToDeleteList = new List<string>();
      if (!string.IsNullOrWhiteSpace(defaultBranch))
      {
        branchToDeleteList.Add(defaultBranch);
        if (searchEnabledBranches.Contains(defaultBranch))
          branchesToBeBulkIndexed.Add(defaultBranch);
      }
      if (!string.IsNullOrWhiteSpace(newDefaultBranch))
      {
        if (entityAttributes.Branches.Contains(newDefaultBranch) || properties.BranchIndexInfo.ContainsKey(newDefaultBranch))
          branchToDeleteList.Add(newDefaultBranch);
        branchesToBeBulkIndexed.Add(newDefaultBranch);
      }
      if (branchToDeleteList.Count <= 0)
        return;
      List<string> values = this.DeleteBranches(indexingExecutionContext, branchToDeleteList);
      if (values != null && values.Count > 0)
        throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("PerformBranchDelete failed for branches {0}", (object) string.Join(",", (IEnumerable<string>) values))));
      foreach (string key in branchToDeleteList)
        properties.BranchIndexInfo.Remove(key);
    }

    internal virtual bool HasDefaultBranchChanged(
      IndexingExecutionContext indexingExecutionContext,
      string oldDefaultBranch,
      out string newDefaultBranch)
    {
      GitRepository gitRepository = this.FetchGitRepositoryFromTfs(indexingExecutionContext);
      newDefaultBranch = gitRepository.DefaultBranch;
      return string.IsNullOrWhiteSpace(newDefaultBranch) && !string.IsNullOrWhiteSpace(oldDefaultBranch) || !string.IsNullOrWhiteSpace(newDefaultBranch) && string.IsNullOrWhiteSpace(oldDefaultBranch) || !string.IsNullOrWhiteSpace(newDefaultBranch) && !string.IsNullOrWhiteSpace(oldDefaultBranch) && !oldDefaultBranch.Equals(newDefaultBranch);
    }

    internal virtual List<string> GetSearchConfiguredBranches(
      IndexingExecutionContext indexingExecutionContext)
    {
      return this.m_gitRepoSyncAnalyzer.GetSearchEnabledBranchesFromTFS(indexingExecutionContext.IndexingUnit, indexingExecutionContext.ProjectId.Value);
    }

    internal virtual List<string> DeleteBranches(
      IndexingExecutionContext indexingExecutionContext,
      List<string> branchToDeleteList)
    {
      return GitBranchDeleterFactory.GetGitBranchDeleter(indexingExecutionContext).PerformBranchDeletionInThisOperation(indexingExecutionContext, branchToDeleteList, indexingExecutionContext.GetIndex() ?? throw new SearchServiceException("Search Index is null."));
    }

    internal virtual GitRepository FetchGitRepositoryFromTfs(
      IndexingExecutionContext indexingExecutionContext)
    {
      return new GitHttpClientWrapper((ExecutionContext) indexingExecutionContext, new TraceMetaData(1080622, "Indexing Pipeline", "IndexingOperation")).GetRepositoryAsync(indexingExecutionContext.RepositoryIndexingUnit.TFSEntityId);
    }
  }
}
