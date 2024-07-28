// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitRepositoryExtensions
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Graph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class TfsGitRepositoryExtensions
  {
    public static bool IsAncestor(
      this ITfsGitRepository repo,
      IVssRequestContext rc,
      TfsGitCommit commit,
      TfsGitCommit ancestor)
    {
      return repo.IsAncestor(rc, commit.ObjectId, ancestor.ObjectId);
    }

    public static bool IsAncestor(
      this ITfsGitRepository repo,
      IVssRequestContext rc,
      Sha1Id commitId,
      Sha1Id ancestorId)
    {
      return new AncestralGraphAlgorithm<int, Sha1Id>().CanReach((IDirectedGraph<int, Sha1Id>) repo.GetCommitGraph((IEnumerable<Sha1Id>) new Sha1Id[2]
      {
        commitId,
        ancestorId
      }), commitId, ancestorId);
    }

    public static IEnumerable<Sha1Id> GetCommitHistory(
      this ITfsGitRepository repo,
      IVssRequestContext rc,
      Sha1Id inHistoryOfCommit,
      Sha1Id? notInHistoryOfCommit = null,
      bool useCommitDateOrder = true)
    {
      IEnumerable<Sha1Id> sha1Ids = (IEnumerable<Sha1Id>) new Sha1Id[1]
      {
        inHistoryOfCommit
      };
      if (notInHistoryOfCommit.HasValue)
        sha1Ids = sha1Ids.Concat<Sha1Id>((IEnumerable<Sha1Id>) new Sha1Id[1]
        {
          notInHistoryOfCommit.Value
        });
      IGitCommitGraph graph = repo.GetCommitGraph(sha1Ids);
      Sha1Id[] sha1IdArray;
      if (!notInHistoryOfCommit.HasValue)
        sha1IdArray = (Sha1Id[]) null;
      else
        sha1IdArray = new Sha1Id[1]
        {
          notInHistoryOfCommit.Value
        };
      IEnumerable<Sha1Id> notReachableFromSet = (IEnumerable<Sha1Id>) sha1IdArray;
      PriorityDelegate<int> getPriorityOfLabel = useCommitDateOrder ? (PriorityDelegate<int>) (label => graph.GetCommitTime(label)) : (PriorityDelegate<int>) null;
      return new AncestralGraphAlgorithm<int, Sha1Id>().Order<int, Sha1Id>((IDirectedGraph<int, Sha1Id>) graph, (IEnumerable<Sha1Id>) new Sha1Id[1]
      {
        inHistoryOfCommit
      }, notReachableFromSet, getPriorityOfLabel);
    }

    public static List<ReachableSetAndBoundary<Sha1Id>> GetCommitHistoryBatch(
      this ITfsGitRepository repo,
      IVssRequestContext rc,
      IList<TfsGitCommitRange> commitRanges,
      bool useCommitDateOrder = true)
    {
      IEnumerable<Sha1Id> requiredCommits = commitRanges.SelectMany<TfsGitCommitRange, Sha1Id>((Func<TfsGitCommitRange, IEnumerable<Sha1Id>>) (commitRange => commitRange.ReachableFromSet.Concat<Sha1Id>(commitRange.NotReachableFromSet)));
      IGitCommitGraph graph = repo.GetCommitGraph(requiredCommits);
      int num = useCommitDateOrder ? 1 : 0;
      AncestralGraphAlgorithm<int, Sha1Id> ancestralGraphAlgorithm = new AncestralGraphAlgorithm<int, Sha1Id>();
      List<ReachableSetAndBoundary<Sha1Id>> commitHistoryBatch = new List<ReachableSetAndBoundary<Sha1Id>>();
      foreach (TfsGitCommitRange commitRange in (IEnumerable<TfsGitCommitRange>) commitRanges)
      {
        ReachableSetAndBoundary<Sha1Id> reachableWithBoundary = ancestralGraphAlgorithm.GetReachableWithBoundary((IDirectedGraph<int, Sha1Id>) graph, commitRange.ReachableFromSet, commitRange.NotReachableFromSet);
        commitHistoryBatch.Add(reachableWithBoundary);
      }
      return commitHistoryBatch;
    }

    public static IEnumerable<Sha1Id> GetFileHistory(
      this ITfsGitRepository repo,
      IVssRequestContext rc,
      Sha1Id inHistoryOfCommit,
      string path,
      bool useCommitDateOrder = true)
    {
      IGitCommitGraph graph = repo.GetCommitGraph((IEnumerable<Sha1Id>) new Sha1Id[1]
      {
        inHistoryOfCommit
      });
      AncestralGraphAlgorithm<int, Sha1Id> ancestralGraphAlgorithm = new AncestralGraphAlgorithm<int, Sha1Id>();
      FileHistoryGraph simplifiedGraph = new FileHistoryGraph(graph, repo.Objects, new NormalizedGitPath(path));
      PriorityDelegate<int> priorityDelegate = useCommitDateOrder ? (PriorityDelegate<int>) (label => graph.GetCommitTime(label)) : (PriorityDelegate<int>) null;
      FileHistoryGraph graph1 = simplifiedGraph;
      int[] reachableFromSet = new int[1]
      {
        graph.GetLabel(inHistoryOfCommit)
      };
      PriorityDelegate<int> getPriorityOfLabel = priorityDelegate;
      return ancestralGraphAlgorithm.OrderByLabels((IDirectedGraph<int, Sha1Id>) graph1, (IEnumerable<int>) reachableFromSet, getPriorityOfLabel: getPriorityOfLabel).AcceptAndClear((CachedGraphWrapper) simplifiedGraph).Select<int, Sha1Id>((Func<int, Sha1Id>) (label => graph.GetVertex(label)));
    }

    public static IEnumerable<TfsGitCommitChangeWithId> GetFileHistoryWithChanges(
      this ITfsGitRepository repo,
      IVssRequestContext rc,
      Sha1Id inHistoryOfCommit,
      string path,
      bool useCommitDateOrder = true)
    {
      IGitCommitGraph graph = repo.GetCommitGraph((IEnumerable<Sha1Id>) new Sha1Id[1]
      {
        inHistoryOfCommit
      });
      AncestralGraphAlgorithm<int, Sha1Id> ancestralGraphAlgorithm = new AncestralGraphAlgorithm<int, Sha1Id>();
      FileHistoryGraph fileHistoryGraph = new FileHistoryGraph(graph, repo.Objects, new NormalizedGitPath(path));
      PriorityDelegate<int> priorityDelegate = useCommitDateOrder ? (PriorityDelegate<int>) (label => graph.GetCommitTime(label)) : (PriorityDelegate<int>) null;
      FileHistoryGraph graph1 = fileHistoryGraph;
      int[] reachableFromSet = new int[1]
      {
        graph.GetLabel(inHistoryOfCommit)
      };
      PriorityDelegate<int> getPriorityOfLabel = priorityDelegate;
      foreach (Sha1Id commitId in ancestralGraphAlgorithm.OrderByLabels((IDirectedGraph<int, Sha1Id>) graph1, (IEnumerable<int>) reachableFromSet, getPriorityOfLabel: getPriorityOfLabel).AcceptAndClear((CachedGraphWrapper) fileHistoryGraph).Select<int, Sha1Id>((Func<int, Sha1Id>) (label => graph.GetVertex(label))))
      {
        ChangeAndObjectType changeType;
        string relativePath;
        string renameSourceItemPath;
        if (fileHistoryGraph.TryGetChangeType(repo, graph, commitId, path, out changeType, out relativePath, out renameSourceItemPath))
        {
          TfsGitCommitChangeWithId historyWithChange = new TfsGitCommitChangeWithId(commitId, new TfsGitCommitChange());
          historyWithChange.ChangeType = changeType.ChangeType;
          historyWithChange.ObjectType = changeType.ObjectType;
          int num = relativePath.LastIndexOf('/');
          historyWithChange.ChildItem = relativePath.Substring(num + 1);
          historyWithChange.ParentPath = relativePath.Substring(0, num + 1);
          historyWithChange.RenameSourceItemPath = renameSourceItemPath;
          yield return historyWithChange;
        }
      }
    }

    internal static IEnumerable<int> AcceptAndClear(
      this IEnumerable<int> results,
      CachedGraphWrapper simplifiedGraph)
    {
      int? lastLabel = new int?();
      foreach (int label in results)
      {
        if (simplifiedGraph.IsLabelAccepted(label))
          yield return label;
        if (lastLabel.HasValue)
          simplifiedGraph.ClearLabel(lastLabel.Value);
        lastLabel = new int?(label);
      }
      if (lastLabel.HasValue)
        simplifiedGraph.ClearLabel(lastLabel.Value);
    }
  }
}
