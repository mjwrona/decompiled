// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ResolutionMethods
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using LibGit2Sharp;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class ResolutionMethods
  {
    internal static void Resolve_AddAdd_EditEdit_RenameRename(
      Repository repo,
      IGitConflict conflict,
      TreeDefinition sourceTree,
      TreeDefinition targetTree)
    {
      switch (conflict.ResolutionMergeType)
      {
        case 1:
          ResolutionMethods.SetPath(conflict.ConflictPath, sourceTree[conflict.ConflictPath.WithoutLeadingSlash], targetTree, sourceTree);
          break;
        case 2:
          ResolutionMethods.SetPath(conflict.ConflictPath, targetTree[conflict.ConflictPath.WithoutLeadingSlash], targetTree, sourceTree);
          break;
        case 3:
        case 4:
          Blob blob = ResolutionMethods.RequireBlobExists(repo, conflict.ResolutionObjectId);
          Mode mode = sourceTree[conflict.ConflictPath.WithoutLeadingSlash].Mode;
          sourceTree.AddBlob(conflict.ConflictPath, blob, mode);
          targetTree.AddBlob(conflict.ConflictPath, blob, mode);
          break;
        default:
          throw new GitApplyResolutionException(GitResolutionError.UnknownMergeType);
      }
    }

    internal static void Resolve_AddRename(
      Repository repo,
      IGitConflict conflict,
      TreeDefinition sourceTree,
      TreeDefinition targetTree)
    {
      switch (conflict.ResolutionAction)
      {
        case 1:
          ResolutionMethods.SetPath(conflict.TargetPath, (TreeEntryDefinition) null, sourceTree, targetTree);
          ResolutionMethods.PathMustNotExist(conflict.ResolutionPath, sourceTree, targetTree);
          ResolutionMethods.SetPath(conflict.ResolutionPath, targetTree[conflict.ConflictPath.WithoutLeadingSlash], sourceTree, targetTree);
          ResolutionMethods.SetPath(conflict.ConflictPath, sourceTree[conflict.ConflictPath.WithoutLeadingSlash], sourceTree, targetTree);
          break;
        case 2:
          ResolutionMethods.SetPath(conflict.TargetPath, (TreeEntryDefinition) null, sourceTree, targetTree);
          ResolutionMethods.SetPath(conflict.ConflictPath, sourceTree[conflict.ConflictPath.WithoutLeadingSlash], sourceTree, targetTree);
          break;
        case 3:
          ResolutionMethods.SetPath(conflict.TargetPath, (TreeEntryDefinition) null, sourceTree, targetTree);
          ResolutionMethods.PathMustNotExist(conflict.ResolutionPath, sourceTree, targetTree);
          ResolutionMethods.SetPath(conflict.ResolutionPath, sourceTree[conflict.ConflictPath.WithoutLeadingSlash], sourceTree, targetTree);
          ResolutionMethods.SetPath(conflict.ConflictPath, targetTree[conflict.ConflictPath.WithoutLeadingSlash], sourceTree, targetTree);
          break;
        case 4:
          ResolutionMethods.SetPath(conflict.TargetPath, (TreeEntryDefinition) null, sourceTree, targetTree);
          ResolutionMethods.SetPath(conflict.ConflictPath, targetTree[conflict.ConflictPath.WithoutLeadingSlash], sourceTree, targetTree);
          break;
        default:
          throw new GitApplyResolutionException(GitResolutionError.UnknownAction);
      }
    }

    internal static void Resolve_RenameAdd(
      Repository repo,
      IGitConflict conflict,
      TreeDefinition sourceTree,
      TreeDefinition targetTree)
    {
      switch (conflict.ResolutionAction)
      {
        case 1:
          ResolutionMethods.SetPath(conflict.SourcePath, (TreeEntryDefinition) null, targetTree, sourceTree);
          ResolutionMethods.PathMustNotExist(conflict.ResolutionPath, targetTree, sourceTree);
          ResolutionMethods.SetPath(conflict.ResolutionPath, targetTree[conflict.ConflictPath.WithoutLeadingSlash], targetTree, sourceTree);
          ResolutionMethods.SetPath(conflict.ConflictPath, sourceTree[conflict.ConflictPath.WithoutLeadingSlash], targetTree, sourceTree);
          break;
        case 2:
          ResolutionMethods.SetPath(conflict.SourcePath, (TreeEntryDefinition) null, targetTree, sourceTree);
          ResolutionMethods.SetPath(conflict.ConflictPath, sourceTree[conflict.ConflictPath.WithoutLeadingSlash], targetTree, sourceTree);
          break;
        case 3:
          ResolutionMethods.SetPath(conflict.SourcePath, (TreeEntryDefinition) null, targetTree, sourceTree);
          ResolutionMethods.PathMustNotExist(conflict.ResolutionPath, targetTree, sourceTree);
          ResolutionMethods.SetPath(conflict.ResolutionPath, sourceTree[conflict.ConflictPath.WithoutLeadingSlash], targetTree, sourceTree);
          ResolutionMethods.SetPath(conflict.ConflictPath, targetTree[conflict.ConflictPath.WithoutLeadingSlash], targetTree, sourceTree);
          break;
        case 4:
          ResolutionMethods.SetPath(conflict.SourcePath, (TreeEntryDefinition) null, targetTree, sourceTree);
          ResolutionMethods.SetPath(conflict.ConflictPath, targetTree[conflict.ConflictPath.WithoutLeadingSlash], targetTree, sourceTree);
          break;
        default:
          throw new GitApplyResolutionException(GitResolutionError.UnknownAction);
      }
    }

    internal static void Resolve_EditDelete_DeleteEdit(
      Repository repo,
      IGitConflict conflict,
      TreeDefinition sourceTree,
      TreeDefinition targetTree)
    {
      switch ((GitResolutionWhichAction) conflict.ResolutionAction)
      {
        case GitResolutionWhichAction.PickSourceAction:
          ResolutionMethods.SetPath(conflict.ConflictPath, sourceTree[conflict.ConflictPath.WithoutLeadingSlash], targetTree, sourceTree);
          break;
        case GitResolutionWhichAction.PickTargetAction:
          ResolutionMethods.SetPath(conflict.ConflictPath, targetTree[conflict.ConflictPath.WithoutLeadingSlash], targetTree, sourceTree);
          break;
        default:
          throw new GitApplyResolutionException(GitResolutionError.UnknownAction);
      }
    }

    internal static void Resolve_DeleteRename(
      Repository repo,
      IGitConflict conflict,
      TreeDefinition sourceTree,
      TreeDefinition targetTree)
    {
      switch ((GitResolutionWhichAction) conflict.ResolutionAction)
      {
        case GitResolutionWhichAction.PickSourceAction:
          ResolutionMethods.SetPath(conflict.ConflictPath, (TreeEntryDefinition) null, sourceTree, targetTree);
          ResolutionMethods.SetPath(conflict.TargetPath, (TreeEntryDefinition) null, sourceTree, targetTree);
          break;
        case GitResolutionWhichAction.PickTargetAction:
          ResolutionMethods.SetPath(conflict.ConflictPath, (TreeEntryDefinition) null, sourceTree, targetTree);
          ResolutionMethods.SetPath(conflict.TargetPath, targetTree[conflict.TargetPath.WithoutLeadingSlash], sourceTree, targetTree);
          break;
        default:
          throw new GitApplyResolutionException(GitResolutionError.UnknownAction);
      }
    }

    internal static void Resolve_RenameDelete(
      Repository repo,
      IGitConflict conflict,
      TreeDefinition sourceTree,
      TreeDefinition targetTree)
    {
      switch ((GitResolutionWhichAction) conflict.ResolutionAction)
      {
        case GitResolutionWhichAction.PickSourceAction:
          ResolutionMethods.SetPath(conflict.ConflictPath, (TreeEntryDefinition) null, sourceTree, targetTree);
          ResolutionMethods.SetPath(conflict.SourcePath, sourceTree[conflict.SourcePath.WithoutLeadingSlash], sourceTree, targetTree);
          break;
        case GitResolutionWhichAction.PickTargetAction:
          ResolutionMethods.SetPath(conflict.ConflictPath, (TreeEntryDefinition) null, sourceTree, targetTree);
          ResolutionMethods.SetPath(conflict.SourcePath, (TreeEntryDefinition) null, sourceTree, targetTree);
          break;
        default:
          throw new GitApplyResolutionException(GitResolutionError.UnknownAction);
      }
    }

    internal static void Resolve_Rename1to2(
      Repository repo,
      IGitConflict conflict,
      TreeDefinition sourceTree,
      TreeDefinition targetTree)
    {
      switch (conflict.ResolutionAction)
      {
        case 1:
          ResolutionMethods.PathMustNotExist(conflict.SourcePath, targetTree);
          ResolutionMethods.SetPath(conflict.TargetPath, (TreeEntryDefinition) null, sourceTree, targetTree);
          ResolutionMethods.SetPath(conflict.SourcePath, sourceTree[conflict.SourcePath.WithoutLeadingSlash], sourceTree, targetTree);
          break;
        case 2:
          ResolutionMethods.PathMustNotExist(conflict.TargetPath, sourceTree);
          ResolutionMethods.SetPath(conflict.SourcePath, (TreeEntryDefinition) null, sourceTree, targetTree);
          ResolutionMethods.SetPath(conflict.TargetPath, targetTree[conflict.TargetPath.WithoutLeadingSlash], sourceTree, targetTree);
          break;
        case 3:
          ResolutionMethods.PathMustNotExist(conflict.SourcePath, targetTree);
          ResolutionMethods.PathMustNotExist(conflict.TargetPath, sourceTree);
          ResolutionMethods.SetPath(conflict.SourcePath, sourceTree[conflict.SourcePath.WithoutLeadingSlash], sourceTree, targetTree);
          ResolutionMethods.SetPath(conflict.TargetPath, targetTree[conflict.TargetPath.WithoutLeadingSlash], sourceTree, targetTree);
          break;
        default:
          throw new GitApplyResolutionException(GitResolutionError.UnknownAction);
      }
    }

    internal static void Resolve_Rename2to1(
      Repository repo,
      IGitConflict conflict,
      TreeDefinition sourceTree,
      TreeDefinition targetTree)
    {
      switch (conflict.ResolutionAction)
      {
        case 1:
          ResolutionMethods.SetPath(conflict.SourcePath, (TreeEntryDefinition) null, sourceTree, targetTree);
          ResolutionMethods.SetPath(conflict.TargetPath, (TreeEntryDefinition) null, sourceTree, targetTree);
          ResolutionMethods.PathMustNotExist(conflict.ResolutionPath, sourceTree, targetTree);
          ResolutionMethods.SetPath(conflict.ResolutionPath, targetTree[conflict.ConflictPath.WithoutLeadingSlash], sourceTree, targetTree);
          ResolutionMethods.SetPath(conflict.ConflictPath, sourceTree[conflict.ConflictPath.WithoutLeadingSlash], sourceTree, targetTree);
          break;
        case 2:
          ResolutionMethods.SetPath(conflict.SourcePath, (TreeEntryDefinition) null, sourceTree, targetTree);
          ResolutionMethods.SetPath(conflict.TargetPath, (TreeEntryDefinition) null, sourceTree, targetTree);
          ResolutionMethods.SetPath(conflict.ConflictPath, sourceTree[conflict.ConflictPath.WithoutLeadingSlash], sourceTree, targetTree);
          break;
        case 3:
          ResolutionMethods.SetPath(conflict.SourcePath, (TreeEntryDefinition) null, sourceTree, targetTree);
          ResolutionMethods.SetPath(conflict.TargetPath, (TreeEntryDefinition) null, sourceTree, targetTree);
          ResolutionMethods.PathMustNotExist(conflict.ResolutionPath, sourceTree, targetTree);
          ResolutionMethods.SetPath(conflict.ResolutionPath, sourceTree[conflict.ConflictPath.WithoutLeadingSlash], sourceTree, targetTree);
          ResolutionMethods.SetPath(conflict.ConflictPath, targetTree[conflict.ConflictPath.WithoutLeadingSlash], sourceTree, targetTree);
          break;
        case 4:
          ResolutionMethods.SetPath(conflict.TargetPath, (TreeEntryDefinition) null, sourceTree, targetTree);
          ResolutionMethods.SetPath(conflict.SourcePath, (TreeEntryDefinition) null, sourceTree, targetTree);
          ResolutionMethods.SetPath(conflict.ConflictPath, targetTree[conflict.ConflictPath.WithoutLeadingSlash], sourceTree, targetTree);
          break;
        default:
          throw new GitApplyResolutionException(GitResolutionError.UnknownAction);
      }
    }

    internal static void Resolve_Trivials(
      IEnumerable<TrivialConflict> trivialConflicts,
      TreeDefinition sourceTree,
      TreeDefinition targetTree)
    {
      foreach (TrivialConflict trivialConflict in trivialConflicts)
      {
        switch (trivialConflict.Resolution)
        {
          case TrivialConflictResolution.TakeSource:
            ResolutionMethods.SetPath(trivialConflict.Path, sourceTree[trivialConflict.Path.WithoutLeadingSlash], sourceTree, targetTree);
            continue;
          case TrivialConflictResolution.TakeTarget:
            ResolutionMethods.SetPath(trivialConflict.Path, targetTree[trivialConflict.Path.WithoutLeadingSlash], sourceTree, targetTree);
            continue;
          case TrivialConflictResolution.Delete:
            ResolutionMethods.SetPath(trivialConflict.Path, (TreeEntryDefinition) null, sourceTree, targetTree);
            continue;
          case TrivialConflictResolution.TakeExisting:
            TreeEntryDefinition treeEntryDefinition1 = sourceTree[trivialConflict.Path.WithoutLeadingSlash];
            TreeEntryDefinition treeEntryDefinition2 = targetTree[trivialConflict.Path.WithoutLeadingSlash];
            NormalizedGitPath path1 = trivialConflict.Path;
            TreeEntryDefinition entry1 = treeEntryDefinition1;
            if ((object) entry1 == null)
              entry1 = treeEntryDefinition2;
            TreeDefinition[] treeDefinitionArray1 = new TreeDefinition[1]
            {
              sourceTree
            };
            ResolutionMethods.SetPath(path1, entry1, treeDefinitionArray1);
            NormalizedGitPath path2 = trivialConflict.Path;
            TreeEntryDefinition entry2 = treeEntryDefinition2;
            if ((object) entry2 == null)
              entry2 = treeEntryDefinition1;
            TreeDefinition[] treeDefinitionArray2 = new TreeDefinition[1]
            {
              targetTree
            };
            ResolutionMethods.SetPath(path2, entry2, treeDefinitionArray2);
            continue;
          default:
            continue;
        }
      }
    }

    private static void SetPath(
      NormalizedGitPath path,
      TreeEntryDefinition entry,
      params TreeDefinition[] treeDefs)
    {
      try
      {
        string withoutLeadingSlash = path.Depth <= 1 ? (string) null : path.GetParent().WithoutLeadingSlash;
        foreach (TreeDefinition treeDef in treeDefs)
        {
          if (!string.IsNullOrEmpty(withoutLeadingSlash))
          {
            TreeEntryDefinition treeEntryDefinition = treeDef[withoutLeadingSlash];
            if (treeEntryDefinition != (TreeEntryDefinition) null && treeEntryDefinition.TargetType != TreeEntryTargetType.Tree)
              throw new GitApplyResolutionException(GitResolutionError.InvalidPath);
          }
          if (!(treeDef[path.WithoutLeadingSlash] == entry))
          {
            if (entry == (TreeEntryDefinition) null)
              treeDef.Remove(path.WithoutLeadingSlash);
            else
              treeDef.Add(path.WithoutLeadingSlash, entry);
          }
        }
      }
      catch (ArgumentException ex)
      {
        throw new GitApplyResolutionException(GitResolutionError.InvalidPath);
      }
      catch (LibGit2SharpException ex)
      {
        throw new GitApplyResolutionException(GitResolutionError.InvalidPath);
      }
    }

    private static void PathMustNotExist(NormalizedGitPath path, params TreeDefinition[] treeDefs)
    {
      try
      {
        foreach (TreeDefinition treeDef in treeDefs)
        {
          if (treeDef[path.WithoutLeadingSlash] != (TreeEntryDefinition) null)
            throw new GitApplyResolutionException(GitResolutionError.PathInUse);
        }
      }
      catch (ArgumentException ex)
      {
        throw new GitApplyResolutionException(GitResolutionError.InvalidPath);
      }
      catch (LibGit2SharpException ex)
      {
        throw new GitApplyResolutionException(GitResolutionError.InvalidPath);
      }
    }

    private static Blob RequireBlobExists(Repository repo, Sha1Id blobId)
    {
      Blob blob = repo.Lookup<Blob>(blobId.ToObjectId());
      return !((LibGit2Sharp.GitObject) blob == (LibGit2Sharp.GitObject) null) ? blob : throw new GitApplyResolutionException(GitResolutionError.MergeContentNotFound);
    }

    private static void AddBlob(
      this TreeDefinition tree,
      NormalizedGitPath path,
      Blob blob,
      Mode mode)
    {
      try
      {
        tree.Add(path.WithoutLeadingSlash, blob, mode);
      }
      catch (ArgumentException ex)
      {
        throw new GitApplyResolutionException(GitResolutionError.InvalidPath);
      }
      catch (LibGit2SharpException ex)
      {
        throw new GitApplyResolutionException(GitResolutionError.InvalidPath);
      }
    }
  }
}
