// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.IndexResolver
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using LibGit2Sharp;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class IndexResolver
  {
    private readonly ITfsGitRepository m_repo;
    private readonly Index m_index;

    public IndexResolver(ITfsGitRepository repo, Index index)
    {
      this.m_repo = repo;
      this.m_index = index;
    }

    public void Resolve_AddAdd_EditEdit(IGitConflict conflict) => this.Resolve_AddAdd_EditEdit_RenameRename(conflict);

    public void Resolve_RenameRename(IGitConflict conflict)
    {
      this.Resolve_AddAdd_EditEdit_RenameRename(conflict);
      this.RemoveEntry(conflict.SourcePath);
    }

    public void Resolve_AddRename(IGitConflict conflict)
    {
      this.Resolve_AddRename_RenameAdd(conflict);
      this.RemoveEntry(conflict.TargetPath);
    }

    public void Resolve_RenameAdd(IGitConflict conflict)
    {
      this.Resolve_AddRename_RenameAdd(conflict);
      this.RemoveEntry(conflict.SourcePath);
    }

    public void Resolve_EditDelete(IGitConflict conflict, NormalizedGitPath pathFileExistsAt = null)
    {
      NormalizedGitPath normalizedGitPath = pathFileExistsAt;
      if ((object) normalizedGitPath == null)
        normalizedGitPath = conflict.ConflictPath;
      pathFileExistsAt = normalizedGitPath;
      (Mode mode, Blob blob) = this.GetSourceModeAndBlob(this.RequireNativeConflict(pathFileExistsAt));
      this.RemoveEntry(conflict.ConflictPath);
      if (pathFileExistsAt != conflict.ConflictPath)
        this.RemoveEntry(pathFileExistsAt);
      switch ((GitResolutionWhichAction) conflict.ResolutionAction)
      {
        case GitResolutionWhichAction.PickSourceAction:
          this.AddBlob(pathFileExistsAt, blob, mode);
          break;
        case GitResolutionWhichAction.PickTargetAction:
          break;
        default:
          throw new GitApplyResolutionException(GitResolutionError.UnknownAction);
      }
    }

    public void Resolve_DeleteEdit(IGitConflict conflict, NormalizedGitPath pathFileExistsAt = null)
    {
      NormalizedGitPath normalizedGitPath = pathFileExistsAt;
      if ((object) normalizedGitPath == null)
        normalizedGitPath = conflict.ConflictPath;
      pathFileExistsAt = normalizedGitPath;
      (Mode mode, Blob blob) = this.GetTargetModeAndBlob(this.RequireNativeConflict(pathFileExistsAt));
      this.RemoveEntry(conflict.ConflictPath);
      if (pathFileExistsAt != conflict.ConflictPath)
        this.RemoveEntry(pathFileExistsAt);
      switch ((GitResolutionWhichAction) conflict.ResolutionAction)
      {
        case GitResolutionWhichAction.PickSourceAction:
          break;
        case GitResolutionWhichAction.PickTargetAction:
          this.AddBlob(pathFileExistsAt, blob, mode);
          break;
        default:
          throw new GitApplyResolutionException(GitResolutionError.UnknownAction);
      }
    }

    public void Resolve_DeleteRename(IGitConflict conflict) => this.Resolve_DeleteEdit(conflict, conflict.TargetPath);

    public void Resolve_RenameDelete(IGitConflict conflict) => this.Resolve_EditDelete(conflict, conflict.SourcePath);

    public void Resolve_Rename1to2(IGitConflict conflict)
    {
      bool flag1 = false;
      bool flag2 = false;
      switch (conflict.ResolutionAction)
      {
        case 1:
          flag1 = true;
          break;
        case 2:
          flag2 = true;
          break;
        case 3:
          flag1 = true;
          flag2 = true;
          break;
        default:
          throw new GitApplyResolutionException(GitResolutionError.UnknownAction);
      }
      this.RemoveEntry(conflict.ConflictPath);
      if (flag1)
      {
        (Mode mode, Blob blob) = this.GetSourceModeAndBlob(this.RequireNativeConflict(conflict.SourcePath));
        this.RemoveEntry(conflict.SourcePath);
        this.AddBlob(conflict.SourcePath, blob, mode);
      }
      else
        this.RemoveEntry(conflict.SourcePath);
      if (flag2)
      {
        (Mode mode, Blob blob) = this.GetTargetModeAndBlob(this.RequireNativeConflict(conflict.TargetPath));
        this.RemoveEntry(conflict.TargetPath);
        this.AddBlob(conflict.TargetPath, blob, mode);
      }
      else
        this.RemoveEntry(conflict.TargetPath);
    }

    public void Resolve_Rename2to1(IGitConflict conflict)
    {
      Conflict nativeConflict = this.RequireNativeConflict(conflict.ConflictPath);
      (Mode mode1, Blob blob1) = this.GetSourceModeAndBlob(nativeConflict);
      (Mode mode2, Blob blob2) = this.GetTargetModeAndBlob(nativeConflict);
      this.RemoveEntry(conflict.SourcePath);
      this.RemoveEntry(conflict.TargetPath);
      this.RemoveEntry(conflict.ConflictPath);
      switch (conflict.ResolutionAction)
      {
        case 1:
          this.PathMustNotExist(conflict.ResolutionPath);
          this.AddBlob(conflict.ConflictPath, blob1, mode1);
          this.AddBlob(conflict.ResolutionPath, blob2, mode2);
          break;
        case 2:
          this.AddBlob(conflict.ConflictPath, blob1, mode1);
          break;
        case 3:
          this.PathMustNotExist(conflict.ResolutionPath);
          this.AddBlob(conflict.ConflictPath, blob2, mode2);
          this.AddBlob(conflict.ResolutionPath, blob1, mode1);
          break;
        case 4:
          this.AddBlob(conflict.ConflictPath, blob2, mode2);
          break;
        default:
          throw new GitApplyResolutionException(GitResolutionError.UnknownAction);
      }
    }

    public void Resolve_Trivials(IEnumerable<TrivialConflict> trivialConflicts)
    {
      foreach (TrivialConflict trivialConflict in trivialConflicts)
      {
        Conflict conflict = this.m_index.Conflicts[trivialConflict.Path.WithoutLeadingSlash];
        if (!(conflict == (Conflict) null))
        {
          Blob blob = (Blob) null;
          Mode mode = Mode.Nonexistent;
          switch (trivialConflict.Resolution)
          {
            case TrivialConflictResolution.TakeSource:
              if (conflict.Theirs != (LibGit2Sharp.IndexEntry) null)
              {
                (mode, blob) = this.GetSourceModeAndBlob(conflict);
                break;
              }
              break;
            case TrivialConflictResolution.TakeTarget:
              if (conflict.Ours != (LibGit2Sharp.IndexEntry) null)
              {
                (mode, blob) = this.GetTargetModeAndBlob(conflict);
                break;
              }
              break;
            case TrivialConflictResolution.TakeExisting:
              if (conflict.Ours != (LibGit2Sharp.IndexEntry) null)
              {
                (mode, blob) = this.GetTargetModeAndBlob(conflict);
                break;
              }
              (mode, blob) = this.GetSourceModeAndBlob(conflict);
              break;
          }
          this.RemoveEntry(trivialConflict.Path);
          if ((LibGit2Sharp.GitObject) blob != (LibGit2Sharp.GitObject) null)
            this.AddBlob(trivialConflict.Path, blob, mode);
        }
      }
    }

    private void Resolve_AddAdd_EditEdit_RenameRename(IGitConflict conflict)
    {
      Conflict conflict1 = this.RequireNativeConflict(conflict.ConflictPath);
      Mode mode;
      Blob blob;
      switch (conflict.ResolutionMergeType)
      {
        case 1:
          (mode, blob) = this.GetSourceModeAndBlob(conflict1);
          break;
        case 2:
          (mode, blob) = this.GetTargetModeAndBlob(conflict1);
          break;
        case 3:
        case 4:
          (mode, blob) = this.ValidateUserResolutionModeAndBlob(conflict, conflict1);
          break;
        default:
          throw new GitApplyResolutionException(GitResolutionError.UnknownMergeType);
      }
      this.RemoveEntry(conflict.ConflictPath);
      this.AddBlob(conflict.ConflictPath, blob, mode);
    }

    private (Mode mode, Blob blob) ValidateUserResolutionModeAndBlob(
      IGitConflict conflict,
      Conflict conflictEntry)
    {
      Sha1Id resolutionObjectId = conflict.ResolutionObjectId;
      bool flag = IndexResolver.AreAnyVersionsLinks(conflictEntry);
      if (this.m_repo.TryLookupObjectType(resolutionObjectId) != GitObjectType.Blob)
      {
        if (flag)
          return (Mode.GitLink, (Blob) new IndexResolver.HeadlessBlob(conflict.ResolutionObjectId.ToObjectId()));
        throw new GitApplyResolutionException(GitResolutionError.MergeContentNotFound);
      }
      if (IndexResolver.AreAllVersionsLinks(conflictEntry))
        throw new GitApplyResolutionException(GitResolutionError.OtherError);
      return (IndexResolver.GetFirstNonLinkMode(conflictEntry), (Blob) new IndexResolver.HeadlessBlob(conflict.ResolutionObjectId.ToObjectId()));
    }

    private void Resolve_AddRename_RenameAdd(IGitConflict conflict)
    {
      Conflict nativeConflict = this.RequireNativeConflict(conflict.ConflictPath);
      (Mode mode1, Blob blob1) = this.GetSourceModeAndBlob(nativeConflict);
      (Mode mode2, Blob blob2) = this.GetTargetModeAndBlob(nativeConflict);
      NormalizedGitPath path = (NormalizedGitPath) null;
      Blob blob3 = (Blob) null;
      Mode mode3 = Mode.Nonexistent;
      Blob blob4;
      Mode mode4;
      switch (conflict.ResolutionAction)
      {
        case 1:
          blob4 = blob1;
          mode4 = mode1;
          path = conflict.ResolutionPath;
          blob3 = blob2;
          mode3 = mode2;
          break;
        case 2:
          blob4 = blob1;
          mode4 = mode1;
          break;
        case 3:
          blob4 = blob2;
          mode4 = mode2;
          path = conflict.ResolutionPath;
          blob3 = blob1;
          mode3 = mode1;
          break;
        case 4:
          blob4 = blob2;
          mode4 = mode2;
          break;
        default:
          throw new GitApplyResolutionException(GitResolutionError.UnknownAction);
      }
      if (path != (NormalizedGitPath) null)
      {
        this.PathMustNotExist(path);
        this.AddBlob(path, blob3, mode3);
      }
      this.RemoveEntry(conflict.ConflictPath);
      this.AddBlob(conflict.ConflictPath, blob4, mode4);
    }

    private void PathMustNotExist(NormalizedGitPath path)
    {
      try
      {
        if (this.m_index[path.WithoutLeadingSlash] != (LibGit2Sharp.IndexEntry) null)
          throw new GitApplyResolutionException(GitResolutionError.PathInUse);
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

    private Conflict RequireNativeConflict(NormalizedGitPath path)
    {
      if (path == (NormalizedGitPath) null)
        throw new GitApplyResolutionException(GitResolutionError.InvalidPath);
      Conflict conflict;
      try
      {
        conflict = this.m_index.Conflicts?[path.WithoutLeadingSlash];
      }
      catch (Exception ex) when (ex is LibGit2SharpException || ex is ArgumentException)
      {
        conflict = (Conflict) null;
      }
      return !(conflict == (Conflict) null) ? conflict : throw new GitApplyResolutionException(GitResolutionError.InvalidPath);
    }

    private void RemoveEntry(NormalizedGitPath path) => this.m_index.Remove(path.WithoutLeadingSlash);

    private static Mode GetFirstNonLinkMode(Conflict conflictEntry) => ((IEnumerable<Mode>) new Mode[4]
    {
      IndexResolver.GetSourceMode(conflictEntry),
      IndexResolver.GetTargetMode(conflictEntry),
      IndexResolver.GetBaseMode(conflictEntry),
      Mode.NonExecutableFile
    }).First<Mode>((Func<Mode, bool>) (m => m != Mode.GitLink && m != 0));

    private static bool AreAnyVersionsLinks(Conflict nativeConflict)
    {
      LibGit2Sharp.IndexEntry ours = nativeConflict.Ours;
      if (((object) ours != null ? (ours.Mode == Mode.GitLink ? 1 : 0) : 0) == 0)
      {
        LibGit2Sharp.IndexEntry theirs = nativeConflict.Theirs;
        if (((object) theirs != null ? (theirs.Mode == Mode.GitLink ? 1 : 0) : 0) == 0)
        {
          LibGit2Sharp.IndexEntry ancestor = nativeConflict.Ancestor;
          return (object) ancestor != null && ancestor.Mode == Mode.GitLink;
        }
      }
      return true;
    }

    private static bool AreAllVersionsLinks(Conflict nativeConflict)
    {
      LibGit2Sharp.IndexEntry ours = nativeConflict.Ours;
      if (((object) ours != null ? (int) ours.Mode : 57344) == 57344)
      {
        LibGit2Sharp.IndexEntry theirs = nativeConflict.Theirs;
        if (((object) theirs != null ? (int) theirs.Mode : 57344) == 57344)
        {
          LibGit2Sharp.IndexEntry ancestor = nativeConflict.Ancestor;
          return ((object) ancestor != null ? (int) ancestor.Mode : 57344) == 57344;
        }
      }
      return false;
    }

    private (Mode mode, Blob blob) GetSourceModeAndBlob(Conflict nativeConflict)
    {
      if (nativeConflict.Theirs?.Id?.RawId == null)
        throw new GitConflictResolverInternalException(nativeConflict);
      return this.GetModeAndBlob(nativeConflict.Theirs);
    }

    private (Mode mode, Blob blob) GetTargetModeAndBlob(Conflict nativeConflict)
    {
      if (nativeConflict.Ours?.Id?.RawId == null)
        throw new GitConflictResolverInternalException(nativeConflict);
      return this.GetModeAndBlob(nativeConflict.Ours);
    }

    private (Mode mode, Blob blob) GetModeAndBlob(LibGit2Sharp.IndexEntry entry) => (object) entry != null && entry.Mode == Mode.GitLink ? (Mode.GitLink, (Blob) new IndexResolver.HeadlessBlob(entry.Id)) : (entry.Mode, this.RequireBlobExists(new Sha1Id(entry.Id.RawId)));

    private static Mode GetSourceMode(Conflict nativeConflict)
    {
      LibGit2Sharp.IndexEntry theirs = nativeConflict.Theirs;
      return (object) theirs == null ? Mode.Nonexistent : theirs.Mode;
    }

    private static Mode GetTargetMode(Conflict nativeConflict)
    {
      LibGit2Sharp.IndexEntry ours = nativeConflict.Ours;
      return (object) ours == null ? Mode.Nonexistent : ours.Mode;
    }

    private static Mode GetBaseMode(Conflict nativeConflict)
    {
      LibGit2Sharp.IndexEntry ancestor = nativeConflict.Ancestor;
      return (object) ancestor == null ? Mode.Nonexistent : ancestor.Mode;
    }

    private Blob RequireBlobExists(Sha1Id blobId) => this.m_repo.TryLookupObjectType(blobId) == GitObjectType.Blob ? (Blob) new IndexResolver.HeadlessBlob(blobId.ToObjectId()) : throw new GitApplyResolutionException(GitResolutionError.MergeContentNotFound);

    private void AddBlob(NormalizedGitPath path, Blob blob, Mode mode)
    {
      try
      {
        this.m_index.Add(blob, path.WithoutLeadingSlash, mode);
      }
      catch (Exception ex) when (ex is LibGit2SharpException || ex is ArgumentException)
      {
        throw new GitApplyResolutionException(GitResolutionError.InvalidPath);
      }
    }

    private class HeadlessBlob : Blob
    {
      private readonly ObjectId m_id;

      public HeadlessBlob(ObjectId id) => this.m_id = id;

      public override ObjectId Id => this.m_id;
    }
  }
}
