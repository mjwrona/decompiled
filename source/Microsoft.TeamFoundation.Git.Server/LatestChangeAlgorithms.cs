// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.LatestChangeAlgorithms
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Graph;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class LatestChangeAlgorithms
  {
    public static (List<LatestChange> changes, DateTime lastCommitTime, Sha1Id treeId) GetLatestChanges(
      IVssRequestContext rc,
      ITfsGitRepository repo,
      Sha1Id fromCommitId,
      string parentPath,
      double requiredProportion = 1.0,
      int maxBufferCommits = 100)
    {
      NormalizedGitPath normalizedGitPath = new NormalizedGitPath(parentPath);
      parentPath = normalizedGitPath.ToString() + "/";
      Sha1Id[] sha1IdArray = new Sha1Id[1]{ fromCommitId };
      IGitCommitGraph commitGraph = repo.GetCommitGraph((IEnumerable<Sha1Id>) sha1IdArray);
      if (!(TfsGitDiffHelper.WalkPath(repo.LookupObject<TfsGitTree>(commitGraph.GetRootTreeId(commitGraph.GetLabel(fromCommitId))), normalizedGitPath) is TfsGitTree parentTree))
        throw new GitItemNotFoundException(parentPath, repo.Name, fromCommitId.ToString(), fromCommitId.ToString()).Expected("git");
      Sha1Id objectId = parentTree.ObjectId;
      ItemsHistoryGraph itemsHistoryGraph = new ItemsHistoryGraph(commitGraph, repo.Objects, normalizedGitPath, parentTree, requiredProportion, maxBufferCommits);
      IEnumerable<int> ints = new AncestralGraphAlgorithm<int, Sha1Id>().OrderByLabels((IDirectedGraph<int, Sha1Id>) itemsHistoryGraph, commitGraph.GetLabels((IEnumerable<Sha1Id>) sha1IdArray)).AcceptAndClear((CachedGraphWrapper) itemsHistoryGraph);
      DateTime dateTime = DateTime.MinValue;
      List<LatestChange> source = new List<LatestChange>();
      foreach (int label in ints)
        source.AddRange((IEnumerable<LatestChange>) itemsHistoryGraph.GetItemsChangedAtLabel(label));
      if (source.Any<LatestChange>())
      {
        Sha1Id commitId = source[source.Count - 1].CommitId;
        dateTime = GitServerConstants.UtcEpoch.AddSeconds((double) commitGraph.GetCommitTime(commitGraph.GetLabel(commitId)));
      }
      return (source, dateTime, objectId);
    }

    internal static IEnumerable<TfsGitCommitHistoryEntry> QueryCommitItemsFromGraphService(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Sha1Id commitId,
      string parentPath,
      QueryCommitItemsRecursionLevel recursionLevel)
    {
      NormalizedGitPath normParentPath = new NormalizedGitPath(parentPath);
      parentPath = normParentPath.ToString();
      NormalizedGitPath grandparentPath = (NormalizedGitPath) null;
      HashSet<string> changedPathsToFind = new HashSet<string>();
      if (normParentPath.Parts.Count > 0)
      {
        grandparentPath = normParentPath.GetParent();
        changedPathsToFind.Add(parentPath);
      }
      if (recursionLevel != QueryCommitItemsRecursionLevel.None)
      {
        TfsGitObject tfsGitObject = TfsGitDiffHelper.WalkPath(repository.LookupObject<TfsGitCommit>(commitId).GetTree(), normParentPath);
        if (tfsGitObject == null)
          yield break;
        else if (tfsGitObject.ObjectType == GitObjectType.Tree)
        {
          TfsGitTree key1 = (TfsGitTree) tfsGitObject;
          Queue<KeyValuePair<TfsGitTree, string>> keyValuePairQueue = new Queue<KeyValuePair<TfsGitTree, string>>();
          keyValuePairQueue.Enqueue(new KeyValuePair<TfsGitTree, string>(key1, parentPath));
          while (keyValuePairQueue.Count > 0)
          {
            KeyValuePair<TfsGitTree, string> keyValuePair = keyValuePairQueue.Dequeue();
            TfsGitTree key2 = keyValuePair.Key;
            string str1 = keyValuePair.Value;
            foreach (TfsGitTreeEntry treeEntry in key2.GetTreeEntries())
            {
              string str2 = str1 + "/" + treeEntry.Name;
              changedPathsToFind.Add(str2);
              if (recursionLevel == QueryCommitItemsRecursionLevel.Full && treeEntry.ObjectType == GitObjectType.Tree)
              {
                TfsGitTree key3 = repository.LookupObject<TfsGitTree>(treeEntry.ObjectId);
                keyValuePairQueue.Enqueue(new KeyValuePair<TfsGitTree, string>(key3, str2));
              }
            }
          }
        }
        else
          changedPathsToFind.Add(parentPath);
      }
      IEnumerable<Sha1Id> fileHistory = repository.GetFileHistory(requestContext, commitId, parentPath);
      HashSet<string> changedPathsFound = new HashSet<string>();
      bool first = true;
      foreach (Sha1Id result in fileHistory)
      {
        if (first)
        {
          if (grandparentPath != (NormalizedGitPath) null)
          {
            foreach (TfsGitDiffEntry allParent in TfsGitDiffHelper.DiffTreeToAllParents(repository, repository.LookupObject<TfsGitCommit>(result), grandparentPath, false))
            {
              if (parentPath.Equals(allParent.RelativePath))
              {
                changedPathsFound.Add(allParent.RelativePath);
                yield return TfsGitCommitHistoryEntry.FromQueryCommitItems(new TfsGitCommitChangeWithId(result, allParent), new TfsGitCommitMetadata(repository.LookupObject<TfsGitCommit>(result)));
                break;
              }
            }
          }
          first = false;
        }
        if (recursionLevel != QueryCommitItemsRecursionLevel.None)
        {
          foreach (TfsGitDiffEntry allParent in TfsGitDiffHelper.DiffTreeToAllParents(repository, repository.LookupObject<TfsGitCommit>(result), normParentPath, recursionLevel == QueryCommitItemsRecursionLevel.Full))
          {
            if (changedPathsFound.Add(allParent.RelativePath))
              yield return TfsGitCommitHistoryEntry.FromQueryCommitItems(new TfsGitCommitChangeWithId(result, allParent), new TfsGitCommitMetadata(repository.LookupObject<TfsGitCommit>(result)));
          }
        }
        if (changedPathsToFind.All<string>((Func<string, bool>) (s => changedPathsFound.Contains(s))))
          break;
      }
    }

    private class ItemAndObjectIds
    {
      public readonly string ItemName;
      public readonly Sha1Id ObjectId;

      public ItemAndObjectIds(string itemName, Sha1Id objectId)
      {
        this.ItemName = itemName;
        this.ObjectId = objectId;
      }
    }
  }
}
