// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitConflict
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class GitConflict : IGitConflict
  {
    internal GitConflict(
      GitConflictSourceType conflictSourceType,
      int conflictSourceId,
      int conflictId,
      Sha1Id mergeBaseCommitId,
      Sha1Id mergeSourceCommitId,
      Sha1Id mergeTargetCommitId,
      GitConflictType conflictType,
      NormalizedGitPath conflictPath,
      NormalizedGitPath sourcePath = null,
      NormalizedGitPath targetPath = null,
      Sha1Id baseObjectId = default (Sha1Id),
      Sha1Id baseObjectIdForTarget = default (Sha1Id),
      Sha1Id sourceObjectId = default (Sha1Id),
      Sha1Id targetObjectId = default (Sha1Id),
      GitResolutionStatus resolutionStatus = GitResolutionStatus.Unresolved,
      GitResolutionError resolutionError = GitResolutionError.None,
      byte resolutionAction = 0,
      byte resolutionMergeType = 0,
      NormalizedGitPath resolutionPath = null,
      Sha1Id resolutionObjectId = default (Sha1Id),
      Guid resolvedBy = default (Guid),
      DateTime resolvedDate = default (DateTime),
      Guid resolutionAuthor = default (Guid))
    {
      this.ConflictSourceId = conflictSourceId;
      this.ConflictSourceType = conflictSourceType;
      this.ConflictId = conflictId;
      this.MergeBaseCommitId = mergeBaseCommitId;
      this.MergeSourceCommitId = mergeSourceCommitId;
      this.MergeTargetCommitId = mergeTargetCommitId;
      this.ConflictType = conflictType;
      this.ConflictPath = conflictPath;
      this.SourcePath = sourcePath;
      this.TargetPath = targetPath;
      this.BaseObjectId = baseObjectId;
      this.BaseObjectIdForTarget = baseObjectIdForTarget;
      this.SourceObjectId = sourceObjectId;
      this.TargetObjectId = targetObjectId;
      this.ResolutionStatus = resolutionStatus;
      this.ResolutionError = resolutionError;
      this.ResolutionAction = resolutionAction;
      this.ResolutionMergeType = resolutionMergeType;
      this.ResolutionPath = resolutionPath;
      this.ResolutionObjectId = resolutionObjectId;
      this.ResolvedBy = resolvedBy;
      this.ResolvedDate = resolvedDate;
      this.ResolutionAuthor = resolutionAuthor;
    }

    internal GitConflict CopyAndUpdate(
      GitConflictSourceType? conflictSourceType = null,
      int? conflictSourceId = null,
      int? conflictId = null,
      Sha1Id? mergeBaseCommitId = null,
      Sha1Id? mergeSourceCommitId = null,
      Sha1Id? mergeTargetCommitId = null,
      GitConflictType? conflictType = null,
      Sha1Id? baseObjectId = null,
      Sha1Id? baseObjectIdForTarget = null,
      Sha1Id? sourceObjectId = null,
      Sha1Id? targetObjectId = null,
      GitResolutionStatus? resolutionStatus = null,
      GitResolutionError? resolutionError = null,
      byte? resolutionAction = null,
      byte? resolutionMergeType = null,
      Sha1Id? resolutionObjectId = null,
      Guid? resolvedBy = null,
      DateTime? resolvedDate = null,
      Guid? resolutionAuthor = null)
    {
      int num1 = (int) conflictSourceType ?? (int) this.ConflictSourceType;
      int conflictSourceId1 = conflictSourceId ?? this.ConflictSourceId;
      int conflictId1 = conflictId ?? this.ConflictId;
      Sha1Id mergeBaseCommitId1 = mergeBaseCommitId ?? this.MergeBaseCommitId;
      Sha1Id mergeSourceCommitId1 = mergeSourceCommitId ?? this.MergeSourceCommitId;
      Sha1Id mergeTargetCommitId1 = mergeTargetCommitId ?? this.MergeTargetCommitId;
      int conflictType1 = (int) conflictType ?? (int) this.ConflictType;
      NormalizedGitPath conflictPath = this.ConflictPath;
      NormalizedGitPath sourcePath = this.SourcePath;
      NormalizedGitPath targetPath = this.TargetPath;
      Sha1Id? nullable = baseObjectId;
      Sha1Id baseObjectId1 = nullable ?? this.BaseObjectId;
      nullable = baseObjectIdForTarget;
      Sha1Id baseObjectIdForTarget1 = nullable ?? this.BaseObjectIdForTarget;
      nullable = sourceObjectId;
      Sha1Id sourceObjectId1 = nullable ?? this.SourceObjectId;
      nullable = targetObjectId;
      Sha1Id targetObjectId1 = nullable ?? this.TargetObjectId;
      int num2 = (int) resolutionStatus ?? (int) this.ResolutionStatus;
      int resolutionError1 = (int) resolutionError ?? (int) this.ResolutionError;
      int resolutionAction1 = (int) resolutionAction ?? (int) this.ResolutionAction;
      int resolutionMergeType1 = (int) resolutionMergeType ?? (int) this.ResolutionMergeType;
      NormalizedGitPath resolutionPath = this.ResolutionPath;
      nullable = resolutionObjectId;
      Sha1Id resolutionObjectId1 = nullable ?? this.ResolutionObjectId;
      Guid resolvedBy1 = resolvedBy ?? this.ResolvedBy;
      DateTime resolvedDate1 = resolvedDate ?? this.ResolvedDate;
      Guid resolutionAuthor1 = resolutionAuthor ?? this.ResolutionAuthor;
      return new GitConflict((GitConflictSourceType) num1, conflictSourceId1, conflictId1, mergeBaseCommitId1, mergeSourceCommitId1, mergeTargetCommitId1, (GitConflictType) conflictType1, conflictPath, sourcePath, targetPath, baseObjectId1, baseObjectIdForTarget1, sourceObjectId1, targetObjectId1, (GitResolutionStatus) num2, (GitResolutionError) resolutionError1, (byte) resolutionAction1, (byte) resolutionMergeType1, resolutionPath, resolutionObjectId1, resolvedBy1, resolvedDate1, resolutionAuthor1);
    }

    internal GitConflict CopyAndUpdateResolutionPath(NormalizedGitPath resolutionPath) => new GitConflict(this.ConflictSourceType, this.ConflictSourceId, this.ConflictId, this.MergeBaseCommitId, this.MergeSourceCommitId, this.MergeTargetCommitId, this.ConflictType, this.ConflictPath, this.SourcePath, this.TargetPath, this.BaseObjectId, this.BaseObjectIdForTarget, this.SourceObjectId, this.TargetObjectId, this.ResolutionStatus, this.ResolutionError, this.ResolutionAction, this.ResolutionMergeType, resolutionPath, this.ResolutionObjectId, this.ResolvedBy, this.ResolvedDate, this.ResolutionAuthor);

    public GitConflictSourceType ConflictSourceType { get; }

    public int ConflictSourceId { get; }

    public int ConflictId { get; }

    public Sha1Id MergeBaseCommitId { get; }

    public Sha1Id MergeSourceCommitId { get; }

    public Sha1Id MergeTargetCommitId { get; }

    public GitConflictType ConflictType { get; }

    public NormalizedGitPath ConflictPath { get; }

    public NormalizedGitPath SourcePath { get; }

    public NormalizedGitPath TargetPath { get; }

    public Sha1Id BaseObjectId { get; }

    public Sha1Id BaseObjectIdForTarget { get; }

    public Sha1Id SourceObjectId { get; }

    public Sha1Id TargetObjectId { get; }

    public GitResolutionStatus ResolutionStatus { get; }

    public GitResolutionError ResolutionError { get; }

    public byte ResolutionAction { get; }

    public byte ResolutionMergeType { get; }

    public NormalizedGitPath ResolutionPath { get; }

    public Sha1Id ResolutionObjectId { get; }

    public Guid ResolvedBy { get; }

    public DateTime ResolvedDate { get; }

    public Guid ResolutionAuthor { get; }
  }
}
