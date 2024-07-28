// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitLogService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Graph;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class GitLogService : IGitLogService, IVssFrameworkService
  {
    private const int c_numMillisToWriteStats = 2000;
    private const int c_statisticsTracepoint = 1013680;
    private static readonly string s_layer = nameof (GitLogService);

    public void ServiceEnd(IVssRequestContext src)
    {
    }

    public void ServiceStart(IVssRequestContext src)
    {
    }

    public IEnumerable<GitLogEntry> GitLog(
      IVssRequestContext rc,
      ITfsGitRepository repo,
      IRevisionRange range,
      GitLogArguments logArguments)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(rc, nameof (rc));
      ArgumentUtility.CheckForNull<ITfsGitRepository>(repo, nameof (repo));
      ArgumentUtility.CheckForNull<IRevisionRange>(range, nameof (range));
      ArgumentUtility.CheckForNull<GitLogArguments>(logArguments, nameof (logArguments));
      SecurityHelper.Instance.CheckReadPermission(rc, (RepoScope) repo.Key, repo.Name);
      if (logArguments.StopAtAdds && !logArguments.RewriteParents && (logArguments.HistoryMode == GitLogHistoryMode.FullHistory || logArguments.HistoryMode == GitLogHistoryMode.FullHistorySimplifyMerges))
        throw new ArgumentException("GitLog() does not support FullHistory with StopAtAdds without RewriteParents.");
      return this.GitLogLazy(rc, repo, range, logArguments);
    }

    private IEnumerable<GitLogEntry> GitLogLazy(
      IVssRequestContext rc,
      ITfsGitRepository repo,
      IRevisionRange range,
      GitLogArguments logArguments)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      GitLogStatistics logStats = new GitLogStatistics();
      try
      {
        IGitCommitGraph graphForAlgorithm = (IGitCommitGraph) null;
        IChangeTypeOracle changeTypeOracle = (IChangeTypeOracle) null;
        CachedGraphWrapper simplifiedGraph = (CachedGraphWrapper) null;
        NormalizedGitPath path = new NormalizedGitPath(logArguments.Path ?? "/");
        graphForAlgorithm = repo.GetCommitGraph(range.GetRequiredCommits(repo));
        IReadOnlySet<int> restrictedLabels = range.GetRestrictedLabels(graphForAlgorithm);
        PathInfoOracle pathOracle = (PathInfoOracle) null;
        if (!path.IsRoot)
        {
          pathOracle = new PathInfoOracle(graphForAlgorithm, repo.Objects, path);
          logStats.PathInfoStats = pathOracle.Statistics;
        }
        if (!path.IsRoot && logArguments.HistoryMode == GitLogHistoryMode.FullHistorySimplifyMerges)
        {
          SimplifyMergesGraph simplifyMergesGraph = new SimplifyMergesGraph((IGitCommitGraph) new GitCommitSubgraph(graphForAlgorithm, restrictedLabels), (IPathInfoOracle) pathOracle, range.GetReachableFromCommits(repo).Single<Sha1Id>());
          logStats.SimplifyMergesStats = simplifyMergesGraph.Statistics;
          simplifiedGraph = (CachedGraphWrapper) simplifyMergesGraph;
          changeTypeOracle = (IChangeTypeOracle) simplifyMergesGraph;
        }
        else
        {
          if (logArguments.HistoryMode == GitLogHistoryMode.FirstParent)
            graphForAlgorithm = (IGitCommitGraph) new FirstParentGraph(graphForAlgorithm);
          if (!path.IsRoot)
          {
            FileHistoryGraph fileHistoryGraph = new FileHistoryGraph(graphForAlgorithm, path, (IPathInfoOracle) pathOracle, logArguments.HistoryMode == GitLogHistoryMode.FullHistory, logArguments.StopAtAdds);
            graphForAlgorithm = (IGitCommitGraph) fileHistoryGraph;
            changeTypeOracle = (IChangeTypeOracle) fileHistoryGraph;
            simplifiedGraph = (CachedGraphWrapper) fileHistoryGraph;
          }
          if (logArguments.RewriteParents && simplifiedGraph != null)
            simplifiedGraph = (CachedGraphWrapper) new RewriteParentsWrapper(simplifiedGraph);
        }
        PriorityDelegate<int> getPriorityOfLabel = logArguments.Order != CommitOrder.TopoOrder ? (PriorityDelegate<int>) (label => graphForAlgorithm.GetCommitTime(label)) : (PriorityDelegate<int>) null;
        IEnumerable<int> ints = new AncestralGraphAlgorithm<int, Sha1Id>().OrderByLabels((IDirectedGraph<int, Sha1Id>) graphForAlgorithm, graphForAlgorithm.TryGetLabels<int, Sha1Id>(range.GetReachableFromCommits(repo)), restrictedLabels, getPriorityOfLabel);
        if (simplifiedGraph != null)
          ints = ints.AcceptAndClear(simplifiedGraph);
        IEnumerable<TfsGitCommit> results = ints.Select<int, TfsGitCommit>((Func<int, TfsGitCommit>) (label => repo.LookupObject<TfsGitCommit>(graphForAlgorithm.GetVertex(label))));
        if (logArguments.FromDate.HasValue || logArguments.ToDate.HasValue)
          results = results.FilterByDateRange(logArguments.FromDate, logArguments.ToDate, logArguments.Order == CommitOrder.DateOrder ? 100 : int.MaxValue);
        if (!string.IsNullOrEmpty(logArguments.Author))
          results = results.FilterByAuthor(logArguments.Author);
        if (!string.IsNullOrEmpty(logArguments.Committer))
          results = results.FilterByCommitter(logArguments.Committer);
        int? nullable;
        if (logArguments.Skip.HasValue)
        {
          IEnumerable<TfsGitCommit> source = results;
          nullable = logArguments.Skip;
          int count = nullable.Value;
          results = source.Skip<TfsGitCommit>(count);
        }
        nullable = logArguments.MaxCount;
        if (nullable.HasValue)
        {
          IEnumerable<TfsGitCommit> source = results;
          nullable = logArguments.MaxCount;
          int count = nullable.Value;
          results = source.Take<TfsGitCommit>(count);
        }
        foreach (TfsGitCommit commit in results)
          yield return this.CommitToLogEntry(repo, commit, path, changeTypeOracle, graphForAlgorithm, (IGitCommitGraph) simplifiedGraph ?? graphForAlgorithm, restrictedLabels);
        changeTypeOracle = (IChangeTypeOracle) null;
        simplifiedGraph = (CachedGraphWrapper) null;
        path = (NormalizedGitPath) null;
        restrictedLabels = (IReadOnlySet<int>) null;
      }
      finally
      {
        if ((logStats.ElapsedMillis = (double) stopwatch.ElapsedMilliseconds) >= 2000.0 || rc.IsTracing(1013680, TraceLevel.Info, GitServerUtils.TraceArea, GitLogService.s_layer))
        {
          logStats.LogArguments = logArguments.ToString();
          rc.TraceAlways(1013680, TraceLevel.Info, GitServerUtils.TraceArea, GitLogService.s_layer, JsonConvert.SerializeObject((object) logStats));
        }
      }
    }

    private GitLogEntry CommitToLogEntry(
      ITfsGitRepository repo,
      TfsGitCommit commit,
      NormalizedGitPath path,
      IChangeTypeOracle changeTypeOracle,
      IGitCommitGraph originalGraph,
      IGitCommitGraph finalGraph,
      IReadOnlySet<int> restrictedLabels)
    {
      if (changeTypeOracle == null || path.IsRoot)
        return new GitLogEntry(repo, commit, (TfsGitCommitChange) null, finalGraph.OutNeighbors(commit.ObjectId));
      TfsGitCommitChange change = (TfsGitCommitChange) null;
      ChangeAndObjectType changeType;
      string relativePath;
      string renameSourceItemPath;
      if (changeTypeOracle.TryGetChangeType(repo, originalGraph, commit.ObjectId, path.ToString(), out changeType, out relativePath, out renameSourceItemPath))
      {
        change = new TfsGitCommitChange();
        change.ChangeType = changeType.ChangeType;
        change.ObjectType = changeType.ObjectType;
        int num = relativePath.LastIndexOf('/');
        change.ChildItem = relativePath.Substring(num + 1);
        change.ParentPath = relativePath.Substring(0, num + 1);
        change.RenameSourceItemPath = renameSourceItemPath;
      }
      IEnumerable<Sha1Id> outEdges = finalGraph.OutNeighbors(commit.ObjectId).Where<Sha1Id>((Func<Sha1Id, bool>) (parent => restrictedLabels.Contains(finalGraph.GetLabel(parent))));
      return new GitLogEntry(repo, commit, change, outEdges);
    }
  }
}
