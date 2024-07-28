// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitDiffHelper
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class TfsGitDiffHelper
  {
    private static readonly ByteArrayEqualityComparer s_byteArrayComparer = new ByteArrayEqualityComparer();

    public static IEnumerable<TfsGitDiffEntry> DiffTreeToAllParents(
      ITfsGitRepository repository,
      TfsGitCommit commit,
      NormalizedGitPath path,
      bool fullRecursion = true)
    {
      TfsGitObject tfsGitObject = TfsGitDiffHelper.WalkPath(commit.GetTree(), path);
      if (tfsGitObject != null && tfsGitObject.ObjectType == GitObjectType.Tree)
      {
        TfsGitTree newTree = (TfsGitTree) tfsGitObject;
        IReadOnlyList<TfsGitCommit> parents = commit.GetParents();
        HashSet<string> stringSet = new HashSet<string>();
        Dictionary<string, TfsGitDiffEntry> changedChildrenDiffs = new Dictionary<string, TfsGitDiffEntry>();
        Dictionary<string, int> changedChildrenCount = new Dictionary<string, int>();
        foreach (TfsGitCommit tfsGitCommit in (IEnumerable<TfsGitCommit>) parents)
        {
          if (TfsGitDiffHelper.WalkPath(tfsGitCommit.GetTree(), path) is TfsGitTree oldTree && !(oldTree.ObjectId != newTree.ObjectId))
          {
            yield break;
          }
          else
          {
            foreach (TfsGitDiffEntry diffTree in (IEnumerable<TfsGitDiffEntry>) TfsGitDiffHelper.DiffTrees(repository, oldTree, newTree, false, fullRecursion))
            {
              if (stringSet.Add(diffTree.RelativePath))
              {
                changedChildrenDiffs.Add(diffTree.RelativePath, diffTree);
                changedChildrenCount.Add(diffTree.RelativePath, 1);
              }
              else
                changedChildrenCount[diffTree.RelativePath]++;
            }
          }
        }
        if (parents.Count == 0)
        {
          foreach (TfsGitDiffEntry diffTree in (IEnumerable<TfsGitDiffEntry>) TfsGitDiffHelper.DiffTrees(repository, (TfsGitTree) null, newTree, false, fullRecursion))
          {
            stringSet.Add(diffTree.RelativePath);
            changedChildrenDiffs.Add(diffTree.RelativePath, diffTree);
            changedChildrenCount.Add(diffTree.RelativePath, 1);
          }
        }
        foreach (string key in stringSet)
        {
          if (changedChildrenCount[key] >= parents.Count)
          {
            TfsGitDiffEntry allParent = changedChildrenDiffs[key];
            allParent.RelativePath = path?.ToString() + allParent.RelativePath;
            if (allParent.RenameSourceItemPath != null)
              allParent.RenameSourceItemPath = path?.ToString() + allParent.RenameSourceItemPath;
            yield return allParent;
          }
        }
      }
    }

    public static TfsGitObject WalkPath(TfsGitTree tree, NormalizedGitPath path) => TfsGitDiffHelper.WalkPathWithShortCircuit(tree, path, (Sha1Id?[]) null, GitObjectType.Bad, out Sha1Id?[] _, out GitObjectType _, false);

    public static void WalkPath(
      TfsGitTree tree,
      NormalizedGitPath path,
      out Sha1Id?[] objectIds,
      out GitObjectType objectType)
    {
      TfsGitDiffHelper.WalkPathWithShortCircuit(tree, path, (Sha1Id?[]) null, GitObjectType.Bad, out objectIds, out objectType, true);
    }

    public static bool ComparePaths(
      TfsGitTree tree1,
      TfsGitTree tree2,
      NormalizedGitPath path,
      out Sha1Id?[] objectIds1,
      out GitObjectType objectType1,
      out Sha1Id?[] objectIds2,
      out GitObjectType objectType2)
    {
      int index = 0;
      objectIds1 = new Sha1Id?[path.Parts.Count];
      objectIds2 = new Sha1Id?[path.Parts.Count];
      Sha1Id? nullable1;
      Sha1Id? nullable2;
      for (; index < path.Parts.Count - 1; ++index)
      {
        nullable1 = tree1?.ObjectId;
        nullable2 = tree2?.ObjectId;
        if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
        {
          TfsGitTreeEntry result;
          if (tree1 != null && tree1.FindEntry(path.Parts[index], out result))
          {
            objectIds1[index] = new Sha1Id?(result.ObjectId);
            tree1 = result.Object as TfsGitTree;
          }
          else
            tree1 = (TfsGitTree) null;
          if (tree2 != null && tree2.FindEntry(path.Parts[index], out result))
          {
            objectIds2[index] = new Sha1Id?(result.ObjectId);
            tree2 = result.Object as TfsGitTree;
          }
          else
            tree2 = (TfsGitTree) null;
        }
        else
          break;
      }
      if (index >= path.Parts.Count - 1)
      {
        nullable2 = tree1?.ObjectId;
        nullable1 = tree2?.ObjectId;
        if ((nullable2.HasValue == nullable1.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() == nullable1.GetValueOrDefault() ? 1 : 0) : 1) : 0) == 0)
        {
          TfsGitTreeEntry result;
          Sha1Id? nullable3;
          if (tree1 != null && tree1.FindEntry(path.Parts[index], out result))
          {
            nullable3 = new Sha1Id?(result.ObjectId);
            objectType1 = result.ObjectType;
          }
          else
          {
            nullable3 = new Sha1Id?();
            objectType1 = GitObjectType.Bad;
          }
          Sha1Id? nullable4;
          if (tree2 != null && tree2.FindEntry(path.Parts[index], out result))
          {
            nullable4 = new Sha1Id?(result.ObjectId);
            objectType2 = result.ObjectType;
          }
          else
          {
            nullable4 = new Sha1Id?();
            objectType2 = GitObjectType.Bad;
          }
          objectIds1[path.Parts.Count - 1] = nullable3;
          objectIds2[path.Parts.Count - 1] = nullable4;
          nullable1 = nullable3;
          nullable2 = nullable4;
          return (nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && objectType1 == objectType2;
        }
      }
      objectType1 = GitObjectType.Bad;
      objectType2 = GitObjectType.Bad;
      return true;
    }

    public static bool ComparePaths(
      TfsGitTree tree,
      NormalizedGitPath path,
      Sha1Id?[] oldObjectIds,
      GitObjectType oldObjectType,
      out Sha1Id?[] newObjectIds,
      out GitObjectType newObjectType)
    {
      TfsGitDiffHelper.WalkPathWithShortCircuit(tree, path, oldObjectIds, oldObjectType, out newObjectIds, out newObjectType, true);
      Sha1Id? oldObjectId = oldObjectIds[oldObjectIds.Length - 1];
      Sha1Id? nullable = newObjectIds[newObjectIds.Length - 1];
      if (oldObjectId.HasValue != nullable.HasValue)
        return false;
      return !oldObjectId.HasValue || oldObjectId.GetValueOrDefault() == nullable.GetValueOrDefault();
    }

    private static TfsGitObject WalkPathWithShortCircuit(
      TfsGitTree tree,
      NormalizedGitPath path,
      Sha1Id?[] oldObjectIds,
      GitObjectType oldObjectType,
      out Sha1Id?[] newObjectIds,
      out GitObjectType newObjectType,
      bool loadObjectIds)
    {
      int count = path.Parts.Count;
      newObjectIds = loadObjectIds ? new Sha1Id?[count] : (Sha1Id?[]) null;
      for (int index1 = 0; index1 < count; ++index1)
      {
        TfsGitTreeEntry result;
        bool entry = tree.FindEntry(path.Parts[index1], out result);
        if (loadObjectIds)
        {
          Sha1Id?[] nullableArray = newObjectIds;
          int index2 = index1;
          Sha1Id? nullable1;
          Sha1Id? nullable2;
          if (!entry)
          {
            nullable1 = new Sha1Id?();
            nullable2 = nullable1;
          }
          else
            nullable2 = new Sha1Id?(result.ObjectId);
          nullableArray[index2] = nullable2;
          if (oldObjectIds != null)
          {
            nullable1 = newObjectIds[index1];
            Sha1Id? oldObjectId = oldObjectIds[index1];
            if ((nullable1.HasValue == oldObjectId.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == oldObjectId.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
            {
              Array.Copy((Array) oldObjectIds, index1 + 1, (Array) newObjectIds, index1 + 1, count - (index1 + 1));
              newObjectType = oldObjectType;
              return (TfsGitObject) null;
            }
          }
        }
        if (!entry || index1 < count - 1 && result.PackType != GitPackObjectType.Tree)
        {
          newObjectType = GitObjectType.Bad;
          return (TfsGitObject) null;
        }
        if (index1 == count - 1)
        {
          newObjectType = result.ObjectType;
          return result.Object;
        }
        tree = (TfsGitTree) result.Object;
      }
      newObjectType = GitObjectType.Tree;
      return (TfsGitObject) tree;
    }

    public static IEnumerable<int> GetTreesameIndices(
      this IList<TfsGitTreeEntry> itemList,
      IEnumerable<int> itemIndices,
      IEnumerable<TfsGitTreeEntry> entries)
    {
      using (IEnumerator<TfsGitTreeEntry> entriesIterator = entries.GetEnumerator())
      {
        bool entriesHasMore = entriesIterator.MoveNext();
        foreach (int itemIndex in itemIndices)
        {
          if (itemIndex >= 0)
          {
            TfsGitTreeEntry tfsGitTreeEntry = itemList[itemIndex];
            int num = -1;
            while (entriesHasMore && (num = tfsGitTreeEntry.GitCompareTo(entriesIterator.Current)) > 0)
              entriesHasMore = entriesIterator.MoveNext();
            if (num == 0 && entriesIterator.Current.ObjectId == tfsGitTreeEntry.ObjectId)
              yield return itemIndex;
          }
        }
      }
    }

    public static IList<TfsGitDiffEntry> DiffTrees(
      ITfsGitRepository repository,
      TfsGitTree oldTree,
      TfsGitTree newTree,
      bool detectRenames,
      bool fullRecursion = true)
    {
      return TfsGitDiffHelper.DiffTrees(repository.ObjectMetadata, detectRenames ? repository.OdbSettings.MaxRenameDetectionFileSize : 0, oldTree, newTree, fullRecursion);
    }

    internal static IList<TfsGitDiffEntry> DiffTrees(
      IObjectMetadata objMetadata,
      int maxRenameDetectionFileSize,
      TfsGitTree oldTree,
      TfsGitTree newTree,
      bool fullRecursion = true,
      bool isEntriesCountLimited = false,
      int maxEntriesCount = 2147483647)
    {
      IList<TfsGitDiffEntry> entries = (IList<TfsGitDiffEntry>) new List<TfsGitDiffEntry>();
      foreach (TfsGitDiffEntry tfsGitDiffEntry in TfsGitDiffHelper.DiffTreesLazy(oldTree, newTree, fullRecursion: fullRecursion, isEntriesCountLimited: isEntriesCountLimited, maxEntriesCount: maxEntriesCount))
        entries.Add(tfsGitDiffEntry);
      if (maxRenameDetectionFileSize > 0 && oldTree != null)
        entries = new RenameDetector((IGitObjectSet) oldTree.ObjectSet, objMetadata, maxRenameDetectionFileSize).Detect(entries);
      return entries;
    }

    public static IEnumerable<TfsGitDiffEntry> DiffTreesSegmented(
      TfsGitTree oldTree,
      TfsGitTree newTree,
      DiffContinuationToken continuationToken,
      int top,
      out DiffContinuationToken nextToken)
    {
      ArgumentUtility.CheckForNull<TfsGitTree>(oldTree, nameof (oldTree));
      ArgumentUtility.CheckForNull<TfsGitTree>(newTree, nameof (newTree));
      continuationToken?.Validate(oldTree.ObjectId, newTree.ObjectId);
      IList<TfsGitDiffEntry> tfsGitDiffEntryList = (IList<TfsGitDiffEntry>) new List<TfsGitDiffEntry>();
      nextToken = (DiffContinuationToken) null;
      if (top <= 0)
        return (IEnumerable<TfsGitDiffEntry>) tfsGitDiffEntryList;
      int num = top;
      foreach (TfsGitDiffEntry tfsGitDiffEntry in TfsGitDiffHelper.DiffTreesLazy(oldTree, newTree, continuationToken, true))
      {
        tfsGitDiffEntryList.Add(tfsGitDiffEntry);
        --num;
        if (num == 0)
        {
          GitObjectType objectType = !tfsGitDiffEntry.NewObjectId.HasValue ? tfsGitDiffEntry.OldObjectType : tfsGitDiffEntry.NewObjectType;
          nextToken = new DiffContinuationToken(oldTree.ObjectId, newTree.ObjectId, new NormalizedGitPath(tfsGitDiffEntry.RelativePath), objectType, tfsGitDiffEntry.ChangeType);
          break;
        }
      }
      return (IEnumerable<TfsGitDiffEntry>) tfsGitDiffEntryList;
    }

    internal static IEnumerable<TfsGitDiffEntry> DiffTreesLazy(
      TfsGitTree oldTree,
      TfsGitTree newTree,
      DiffContinuationToken continuationToken = null,
      bool splitObjectTypeChanges = false,
      bool fullRecursion = true,
      bool isEntriesCountLimited = false,
      int maxEntriesCount = 2147483647)
    {
      TfsGitTreeDepthFirstEnumerator enum1 = (TfsGitTreeDepthFirstEnumerator) null;
      TfsGitTreeDepthFirstEnumerator enum2 = (TfsGitTreeDepthFirstEnumerator) null;
      bool enum1Valid = false;
      bool enum2Valid = false;
      bool checkContinuationToken = false;
      try
      {
        if (oldTree != null)
          enum1 = new TfsGitTreeDepthFirstEnumerator(oldTree, fullRecursion, isEntriesCountLimited: isEntriesCountLimited, maxEntriesCount: maxEntriesCount);
        if (newTree != null)
          enum2 = new TfsGitTreeDepthFirstEnumerator(newTree, fullRecursion, isEntriesCountLimited: isEntriesCountLimited, maxEntriesCount: maxEntriesCount);
        if (continuationToken == null)
        {
          enum1Valid = enum1 != null && enum1.MoveNext();
          enum2Valid = enum2 != null && enum2.MoveNext();
        }
        else
        {
          if (!fullRecursion)
            throw new InvalidOperationException("fullRecursion must be set to true if we need to use a continuation token.");
          enum1Valid = enum1 != null && enum1.SkipEntriesUntilPath(continuationToken.Path, continuationToken.ObjectType);
          enum2Valid = enum2 != null && enum2.SkipEntriesUntilPath(continuationToken.Path, continuationToken.ObjectType);
          checkContinuationToken = true;
          if (!enum1Valid && !enum2Valid)
            throw new GitInvalidDiffContinuationTokenException("VS403661: No path was found for either tree.");
        }
        while (enum1Valid & enum2Valid)
        {
          TfsGitDiffEntry diffEntry = (TfsGitDiffEntry) null;
          TfsGitDiffEntry anotherDiffEntryToReturn = (TfsGitDiffEntry) null;
          TreeEntryAndPath current1 = enum1.Current;
          TreeEntryAndPath current2 = enum2.Current;
          if (enum1.CurrentDepth > enum2.CurrentDepth)
          {
            diffEntry = new TfsGitDiffEntry(current1, (TreeEntryAndPath) null);
            enum1Valid = enum1.MoveNext();
          }
          else if (enum1.CurrentDepth < enum2.CurrentDepth)
          {
            diffEntry = new TfsGitDiffEntry((TreeEntryAndPath) null, current2);
            enum2Valid = enum2.MoveNext();
          }
          else
          {
            int num = current1.Entry.GitCompareTo(current2.Entry);
            if (num < 0)
            {
              diffEntry = new TfsGitDiffEntry(current1, (TreeEntryAndPath) null);
              enum1Valid = enum1.MoveNext();
            }
            else if (num > 0)
            {
              diffEntry = new TfsGitDiffEntry((TreeEntryAndPath) null, current2);
              enum2Valid = enum2.MoveNext();
            }
            else if (current1.Entry.ObjectId != current2.Entry.ObjectId)
            {
              if (!splitObjectTypeChanges || current1.Entry.ObjectType == current2.Entry.ObjectType)
              {
                diffEntry = new TfsGitDiffEntry(current1, current2);
              }
              else
              {
                diffEntry = new TfsGitDiffEntry(current1, (TreeEntryAndPath) null);
                anotherDiffEntryToReturn = new TfsGitDiffEntry((TreeEntryAndPath) null, current2);
              }
              enum1Valid = enum1.MoveNext();
              enum2Valid = enum2.MoveNext();
            }
            else if (current1.Entry.ObjectType == GitObjectType.Tree)
            {
              enum1Valid = enum1.SkipSubtreeAndMoveNext();
              enum2Valid = enum2.SkipSubtreeAndMoveNext();
            }
            else
            {
              enum1Valid = enum1.MoveNext();
              enum2Valid = enum2.MoveNext();
            }
          }
          if (checkContinuationToken)
          {
            if (continuationToken.Matches(diffEntry))
            {
              checkContinuationToken = false;
              if (anotherDiffEntryToReturn != null)
                yield return anotherDiffEntryToReturn;
            }
            else if (anotherDiffEntryToReturn != null && continuationToken.Matches(anotherDiffEntryToReturn))
              checkContinuationToken = false;
            else
              throw new GitInvalidDiffContinuationTokenException("VS403661: Continuation token didn't match the next entry found by the diff algorithm. Found entry: " + string.Format("{0},{1},{2},{3}", (object) diffEntry?.RelativePath, (object) diffEntry?.ChangeType, (object) diffEntry?.OldObjectType, (object) diffEntry?.NewObjectType));
          }
          else
          {
            if (diffEntry != null)
            {
              yield return diffEntry;
              if (anotherDiffEntryToReturn != null)
                yield return anotherDiffEntryToReturn;
            }
            anotherDiffEntryToReturn = (TfsGitDiffEntry) null;
          }
        }
        for (; enum1Valid; enum1Valid = enum1.MoveNext())
        {
          TfsGitDiffEntry diffEntry = new TfsGitDiffEntry(enum1.Current, (TreeEntryAndPath) null);
          if (checkContinuationToken)
          {
            if (!continuationToken.Matches(diffEntry))
              throw new GitInvalidDiffContinuationTokenException("VS403661: Continuation token didn't match the next old tree entry found by the diff algorithm");
            checkContinuationToken = false;
          }
          else
            yield return diffEntry;
        }
        for (; enum2Valid; enum2Valid = enum2.MoveNext())
        {
          TfsGitDiffEntry diffEntry = new TfsGitDiffEntry((TreeEntryAndPath) null, enum2.Current);
          if (checkContinuationToken)
          {
            if (!continuationToken.Matches(diffEntry))
              throw new GitInvalidDiffContinuationTokenException("VS403661: Continuation token didn't match the next new tree entry found by the diff algorithm");
            checkContinuationToken = false;
          }
          else
            yield return diffEntry;
        }
      }
      finally
      {
        enum1?.Dispose();
        enum2?.Dispose();
      }
    }
  }
}
