// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Services.PullRequest.GitIndexPullRequestMerger
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using LibGit2Sharp;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Native;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Services.PullRequest
{
  internal class GitIndexPullRequestMerger : PullRequestMergerBase
  {
    private const string c_Layer = "GitIndexPullRequestMerger";

    public GitIndexPullRequestMerger(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      bool detectRenameFalsePositives,
      MergeWithConflictsOptions mergeOptions,
      Sha1Id mergeSourceCommitId,
      Sha1Id mergeTargetCommitId,
      IEnumerable<Sha1Id> parentIdsForMergeCommit,
      ClientTraceData ctData)
      : base(requestContext, repository, pullRequest, detectRenameFalsePositives, mergeOptions, mergeSourceCommitId, mergeTargetCommitId, parentIdsForMergeCommit, ctData)
    {
    }

    protected override PullRequestMergerBase.Result ResolveConflicts(
      PullRequestMergerBase.ConflictSet conflicts,
      MergeWithConflictsResult originalMergeResult,
      LibGit2NativeLibrary nativeLibrary)
    {
      using (this.m_requestContext.TimeRegion(nameof (GitIndexPullRequestMerger), nameof (ResolveConflicts)))
      {
        MergeWithConflictsResult ourResult = originalMergeResult;
        try
        {
          Index index = originalMergeResult.Index;
          IndexResolver resolver = new IndexResolver(this.m_repository, index);
          this.ApplyManualResolutionsToIndex(conflicts.ConflictsForThisMerge, resolver);
          if (conflicts.ConflictsForThisMerge.All<Microsoft.TeamFoundation.Git.Server.GitConflict>((Func<Microsoft.TeamFoundation.Git.Server.GitConflict, bool>) (c => c.ResolutionStatus == GitResolutionStatus.Resolved)))
          {
            resolver.Resolve_Trivials((IEnumerable<TrivialConflict>) conflicts.TrivialConflicts);
            if (index.IsFullyMerged)
            {
              Tree tree = index.WriteToTree();
              Sha1Id[] parentCommits = this.GetParentCommits(conflicts, nativeLibrary);
              Commit commit = nativeLibrary.WriteCommit(this.m_mergeOptions.CommitDetails, tree, (IEnumerable<Sha1Id>) parentCommits);
              ourResult.MergeCommitId = new Sha1Id(commit.Id.RawId);
              ourResult.Status = PullRequestAsyncStatus.Succeeded;
              ourResult.FailureType = PullRequestMergeFailureType.None;
            }
          }
        }
        catch (Exception ex) when (nativeLibrary.HandleExceptionOnCommitMerge(ex, ourResult))
        {
        }
        return new PullRequestMergerBase.Result()
        {
          FailureMessage = ourResult.FailureMessage,
          FailureType = ourResult.FailureType,
          MergeCommitId = ourResult.MergeCommitId,
          MergeStatus = ourResult.Status
        };
      }
    }

    protected virtual Sha1Id[] GetParentCommits(
      PullRequestMergerBase.ConflictSet conflicts,
      LibGit2NativeLibrary nativeLibrary)
    {
      return this.m_parentIdsForMergeCommit;
    }

    internal void ApplyManualResolutionsToIndex(List<Microsoft.TeamFoundation.Git.Server.GitConflict> conflicts, IndexResolver resolver)
    {
      using (this.m_requestContext.TimeRegion(nameof (GitIndexPullRequestMerger), nameof (ApplyManualResolutionsToIndex)))
      {
        List<int> applyResolutionSucceeded = new List<int>();
        Dictionary<int, GitResolutionError> applyResolutionFailed = new Dictionary<int, GitResolutionError>();
        Stopwatch timer = Stopwatch.StartNew();
        try
        {
          for (int index1 = 0; index1 < conflicts.Count; ++index1)
          {
            Microsoft.TeamFoundation.Git.Server.GitConflict conflict = conflicts[index1];
            if (conflict.ResolutionStatus == GitResolutionStatus.Resolved)
            {
              Microsoft.TeamFoundation.Git.Server.GitConflict index2 = this.ApplyOneResolutionToIndex(conflict, resolver);
              if (index2.ResolutionStatus == GitResolutionStatus.Resolved)
                applyResolutionSucceeded.Add(index2.ConflictId);
              else
                applyResolutionFailed[index2.ConflictId] = index2.ResolutionError;
              conflicts[index1] = index2;
            }
          }
        }
        finally
        {
          TracepointUtils.Tracepoint(this.m_requestContext, 1013819, GitServerUtils.TraceArea, nameof (GitIndexPullRequestMerger), (Func<object>) (() => (object) new
          {
            RepositoryId = this.m_pullRequest.RepositoryId,
            PullRequestId = this.m_pullRequest.PullRequestId,
            applyResolutionSucceeded = applyResolutionSucceeded,
            applyResolutionFailed = applyResolutionFailed,
            ElapsedMilliseconds = timer.ElapsedMilliseconds
          }), caller: nameof (ApplyManualResolutionsToIndex));
        }
      }
    }

    internal Microsoft.TeamFoundation.Git.Server.GitConflict ApplyOneResolutionToIndex(
      Microsoft.TeamFoundation.Git.Server.GitConflict conflict,
      IndexResolver resolver)
    {
      if (conflict.ResolutionStatus != GitResolutionStatus.Resolved || conflict.ResolvedBy == Guid.Empty || conflict.ResolvedDate == DateTime.MinValue)
      {
        if (conflict.ResolutionStatus == GitResolutionStatus.Resolved)
        {
          Microsoft.TeamFoundation.Git.Server.GitConflict gitConflict = conflict;
          GitResolutionStatus? nullable = new GitResolutionStatus?(GitResolutionStatus.Unresolved);
          GitConflictSourceType? conflictSourceType = new GitConflictSourceType?();
          int? conflictSourceId = new int?();
          int? conflictId = new int?();
          Sha1Id? mergeBaseCommitId = new Sha1Id?();
          Sha1Id? mergeSourceCommitId = new Sha1Id?();
          Sha1Id? mergeTargetCommitId = new Sha1Id?();
          GitConflictType? conflictType = new GitConflictType?();
          Sha1Id? baseObjectId = new Sha1Id?();
          Sha1Id? baseObjectIdForTarget = new Sha1Id?();
          Sha1Id? sourceObjectId = new Sha1Id?();
          Sha1Id? targetObjectId = new Sha1Id?();
          GitResolutionStatus? resolutionStatus = nullable;
          GitResolutionError? resolutionError = new GitResolutionError?();
          byte? resolutionAction = new byte?();
          byte? resolutionMergeType = new byte?();
          Sha1Id? resolutionObjectId = new Sha1Id?();
          Guid? resolvedBy = new Guid?();
          DateTime? resolvedDate = new DateTime?();
          Guid? resolutionAuthor = new Guid?();
          conflict = gitConflict.CopyAndUpdate(conflictSourceType, conflictSourceId, conflictId, mergeBaseCommitId, mergeSourceCommitId, mergeTargetCommitId, conflictType, baseObjectId, baseObjectIdForTarget, sourceObjectId, targetObjectId, resolutionStatus, resolutionError, resolutionAction, resolutionMergeType, resolutionObjectId, resolvedBy, resolvedDate, resolutionAuthor);
        }
        return conflict;
      }
      GitResolutionError gitResolutionError = GitResolutionError.None;
      try
      {
        switch (conflict.ConflictType)
        {
          case GitConflictType.AddAdd:
            resolver.Resolve_AddAdd_EditEdit((IGitConflict) conflict);
            break;
          case GitConflictType.AddRename:
            resolver.Resolve_AddRename((IGitConflict) conflict);
            break;
          case GitConflictType.DeleteEdit:
            resolver.Resolve_DeleteEdit((IGitConflict) conflict);
            break;
          case GitConflictType.DeleteRename:
            resolver.Resolve_DeleteRename((IGitConflict) conflict);
            break;
          case GitConflictType.EditDelete:
            resolver.Resolve_EditDelete((IGitConflict) conflict);
            break;
          case GitConflictType.EditEdit:
            resolver.Resolve_AddAdd_EditEdit((IGitConflict) conflict);
            break;
          case GitConflictType.Rename1to2:
            resolver.Resolve_Rename1to2((IGitConflict) conflict);
            break;
          case GitConflictType.Rename2to1:
            resolver.Resolve_Rename2to1((IGitConflict) conflict);
            break;
          case GitConflictType.RenameAdd:
            resolver.Resolve_RenameAdd((IGitConflict) conflict);
            break;
          case GitConflictType.RenameDelete:
            resolver.Resolve_RenameDelete((IGitConflict) conflict);
            break;
          case GitConflictType.RenameRename:
            resolver.Resolve_RenameRename((IGitConflict) conflict);
            break;
        }
      }
      catch (GitApplyResolutionException ex)
      {
        gitResolutionError = ex.ResolutionError;
      }
      catch (Exception ex)
      {
        TracepointUtils.TraceException(this.m_requestContext, 1013817, GitServerUtils.TraceArea, nameof (GitIndexPullRequestMerger), ex, (object) new
        {
          ProjectId = this.m_repository.Key.ProjectId,
          RepositoryId = this.m_pullRequest.RepositoryId,
          SourceRepositoryId = this.m_pullRequest.SourceRepositoryId,
          PullRequestId = this.m_pullRequest.PullRequestId,
          ConflictId = conflict.ConflictId,
          MergeBaseCommitId = conflict.MergeBaseCommitId,
          MergeSourceCommitId = conflict.MergeSourceCommitId,
          MergeTargetCommitId = conflict.MergeTargetCommitId,
          SourcePath = conflict.SourcePath,
          TargetPath = conflict.TargetPath,
          BaseObjectId = conflict.BaseObjectId,
          BaseObjectIdForTarget = conflict.BaseObjectIdForTarget,
          SourceObjectId = conflict.SourceObjectId,
          TargetObjectId = conflict.TargetObjectId,
          ResolutionAction = conflict.ResolutionAction,
          ResolutionMergeType = conflict.ResolutionMergeType,
          ResolutionPath = conflict.ResolutionPath,
          ResolutionObjectId = conflict.ResolutionObjectId
        }, caller: nameof (ApplyOneResolutionToIndex));
        gitResolutionError = GitResolutionError.OtherError;
      }
      if (gitResolutionError != GitResolutionError.None)
      {
        Microsoft.TeamFoundation.Git.Server.GitConflict gitConflict = conflict;
        GitResolutionStatus? nullable1 = new GitResolutionStatus?(GitResolutionStatus.Unresolved);
        DateTime? nullable2 = new DateTime?(DateTime.MinValue);
        GitResolutionError? nullable3 = new GitResolutionError?(gitResolutionError);
        GitConflictSourceType? conflictSourceType = new GitConflictSourceType?();
        int? conflictSourceId = new int?();
        int? conflictId = new int?();
        Sha1Id? mergeBaseCommitId = new Sha1Id?();
        Sha1Id? mergeSourceCommitId = new Sha1Id?();
        Sha1Id? mergeTargetCommitId = new Sha1Id?();
        GitConflictType? conflictType = new GitConflictType?();
        Sha1Id? baseObjectId = new Sha1Id?();
        Sha1Id? baseObjectIdForTarget = new Sha1Id?();
        Sha1Id? sourceObjectId = new Sha1Id?();
        Sha1Id? targetObjectId = new Sha1Id?();
        GitResolutionStatus? resolutionStatus = nullable1;
        GitResolutionError? resolutionError = nullable3;
        byte? resolutionAction = new byte?();
        byte? resolutionMergeType = new byte?();
        Sha1Id? resolutionObjectId = new Sha1Id?();
        Guid? resolvedBy = new Guid?();
        DateTime? resolvedDate = nullable2;
        Guid? resolutionAuthor = new Guid?();
        conflict = gitConflict.CopyAndUpdate(conflictSourceType, conflictSourceId, conflictId, mergeBaseCommitId, mergeSourceCommitId, mergeTargetCommitId, conflictType, baseObjectId, baseObjectIdForTarget, sourceObjectId, targetObjectId, resolutionStatus, resolutionError, resolutionAction, resolutionMergeType, resolutionObjectId, resolvedBy, resolvedDate, resolutionAuthor);
      }
      return conflict;
    }
  }
}
