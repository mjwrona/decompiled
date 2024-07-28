// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Services.PullRequest.PullRequestMergerBase
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using LibGit2Sharp;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Native;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Services.PullRequest
{
  internal abstract class PullRequestMergerBase
  {
    protected readonly IVssRequestContext m_requestContext;
    protected readonly ITfsGitRepository m_repository;
    protected readonly MergeWithConflictsOptions m_mergeOptions;
    protected readonly bool m_detectRenameFalsePositives;
    protected readonly ClientTraceData m_ctData;
    protected readonly TfsGitPullRequest m_pullRequest;
    protected readonly Sha1Id m_mergeSourceCommitId;
    protected readonly Sha1Id m_mergeTargetCommitId;
    protected readonly Sha1Id[] m_parentIdsForMergeCommit;
    private const string c_Layer = "PullRequestMergerBase";

    protected PullRequestMergerBase(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      bool detectRenameFalsePositives,
      MergeWithConflictsOptions mergeOptions,
      Sha1Id mergeSourceCommitId,
      Sha1Id mergeTargetCommitId,
      IEnumerable<Sha1Id> parentIdsForMergeCommit,
      ClientTraceData ctData)
    {
      this.m_detectRenameFalsePositives = detectRenameFalsePositives;
      this.m_mergeOptions = mergeOptions;
      this.m_pullRequest = pullRequest;
      this.m_repository = repository;
      this.m_requestContext = requestContext;
      this.m_ctData = ctData;
      this.m_mergeSourceCommitId = mergeSourceCommitId;
      this.m_mergeTargetCommitId = mergeTargetCommitId;
      this.m_parentIdsForMergeCommit = parentIdsForMergeCommit.ToArray<Sha1Id>();
    }

    public PullRequestMergerBase.Result MergeAndResolveConflicts()
    {
      using (this.m_requestContext.TimeRegion(nameof (PullRequestMergerBase), nameof (MergeAndResolveConflicts)))
      {
        using (LibGit2NativeLibrary nativeLibrary = new LibGit2NativeLibrary(this.m_requestContext, this.m_repository))
        {
          this.m_requestContext.TraceAlways(1013708, TraceLevel.Info, GitServerUtils.TraceArea, nameof (PullRequestMergerBase), "Beginning merge. PR: {0}, Source: {1}@{2}, Target: {3}@{4}", (object) this.m_pullRequest.PullRequestId, (object) this.m_pullRequest.SourceBranchName, (object) this.m_mergeSourceCommitId, (object) this.m_pullRequest.TargetBranchName, (object) this.m_mergeTargetCommitId);
          MergeWithConflictsResult conflictTracking;
          using (this.m_requestContext.TimeRegion("LibGit2NativeLibrary", "CreateMergeWithConflictTracking"))
            conflictTracking = nativeLibrary.CreateMergeWithConflictTracking(this.m_mergeSourceCommitId, this.m_mergeTargetCommitId, this.m_mergeOptions, (IEnumerable<Sha1Id>) this.m_parentIdsForMergeCommit, this.m_ctData);
          this.m_requestContext.TraceAlways(1013709, TraceLevel.Info, GitServerUtils.TraceArea, nameof (PullRequestMergerBase), "Completed merge. PR: {0}, Source: {1}@{2}, Target: {3}@{4}", (object) this.m_pullRequest.PullRequestId, (object) this.m_pullRequest.SourceBranchName, (object) this.m_mergeSourceCommitId, (object) this.m_pullRequest.TargetBranchName, (object) this.m_mergeTargetCommitId);
          PullRequestMergerBase.ConflictSet conflicts = this.CheckForConflicts(conflictTracking);
          if (conflicts.NoConflictsForThisMerge())
            return new PullRequestMergerBase.Result()
            {
              Conflicts = (IEnumerable<Microsoft.TeamFoundation.Git.Server.GitConflict>) conflicts.ConflictsForAllPreviousMerges,
              ConflictResolutionHash = Sha1Id.Empty,
              MergeCommitId = conflictTracking.MergeCommitId,
              MergeStatus = conflictTracking.Status,
              FailureMessage = conflictTracking.FailureMessage,
              FailureType = conflictTracking.FailureType
            };
          if (conflicts.HasResolvedConflicts())
          {
            PullRequestMergerBase.Result result1 = this.ResolveConflicts(conflicts, conflictTracking, nativeLibrary);
            result1.Conflicts = conflicts.ConflictsForAllPreviousMerges.Concat<Microsoft.TeamFoundation.Git.Server.GitConflict>((IEnumerable<Microsoft.TeamFoundation.Git.Server.GitConflict>) conflicts.ConflictsForThisMerge);
            if (result1.MergeStatus == PullRequestAsyncStatus.Succeeded)
            {
              result1.ConflictResolutionHash = GitConflictService.GetConflictResolutionHash(this.m_requestContext, conflicts.ConflictsForThisMerge);
              if (result1.ConflictResolutionHash != Sha1Id.Empty)
              {
                ITeamFoundationIdentityService service = this.m_requestContext.GetService<ITeamFoundationIdentityService>();
                Guid guid = conflicts.ConflictsForThisMerge.OrderByDescending<Microsoft.TeamFoundation.Git.Server.GitConflict, DateTime>((Func<Microsoft.TeamFoundation.Git.Server.GitConflict, DateTime>) (conflict => conflict.ResolvedDate)).Select<Microsoft.TeamFoundation.Git.Server.GitConflict, Guid>((Func<Microsoft.TeamFoundation.Git.Server.GitConflict, Guid>) (conflict => conflict.ResolvedBy)).Distinct<Guid>().Where<Guid>((Func<Guid, bool>) (id => id != Guid.Empty)).FirstOrDefault<Guid>();
                IdentityDescriptor identityDescriptor1 = (IdentityDescriptor) null;
                if (guid != Guid.Empty)
                  identityDescriptor1 = ((IEnumerable<TeamFoundationIdentity>) service.ReadIdentities(this.m_requestContext, new Guid[1]
                  {
                    guid
                  })).FirstOrDefault<TeamFoundationIdentity>()?.Descriptor;
                PullRequestMergerBase.Result result2 = result1;
                IdentityDescriptor identityDescriptor2 = identityDescriptor1;
                if ((object) identityDescriptor2 == null)
                  identityDescriptor2 = this.m_requestContext.GetUserIdentity().Descriptor;
                result2.ConflictResolver = identityDescriptor2;
              }
            }
            return result1;
          }
          return new PullRequestMergerBase.Result()
          {
            Conflicts = conflicts.ConflictsForAllPreviousMerges.Concat<Microsoft.TeamFoundation.Git.Server.GitConflict>((IEnumerable<Microsoft.TeamFoundation.Git.Server.GitConflict>) conflicts.ConflictsForThisMerge),
            MergeStatus = conflictTracking.Status,
            FailureMessage = conflictTracking.FailureMessage,
            FailureType = conflictTracking.FailureType
          };
        }
      }
    }

    protected abstract PullRequestMergerBase.Result ResolveConflicts(
      PullRequestMergerBase.ConflictSet conflicts,
      MergeWithConflictsResult originalMergeResult,
      LibGit2NativeLibrary nativeLibrary);

    private PullRequestMergerBase.ConflictSet CheckForConflicts(MergeWithConflictsResult mergeResult)
    {
      using (this.m_requestContext.TimeRegion(nameof (PullRequestMergerBase), nameof (CheckForConflicts)))
      {
        PullRequestMergerBase.ConflictSet conflictSet = new PullRequestMergerBase.ConflictSet();
        conflictSet.TrivialConflicts = new List<TrivialConflict>();
        using (GitConflictComponent component = this.m_requestContext.CreateComponent<GitConflictComponent>())
          conflictSet.ConflictsForAllPreviousMerges = component.QueryGitConflicts(this.m_repository.Key, GitConflictSourceType.PullRequest, this.m_pullRequest.PullRequestId, int.MaxValue, 0, true, false, false);
        List<Microsoft.TeamFoundation.Git.Server.GitConflict> source;
        switch (mergeResult.Status)
        {
          case PullRequestAsyncStatus.Conflicts:
            IVssRequestContext requestContext = this.m_requestContext;
            GitMergeOriginRef mergeOrigin = new GitMergeOriginRef();
            mergeOrigin.PullRequestId = new int?(this.m_pullRequest.PullRequestId);
            Sha1Id mergeBaseCommitId = mergeResult.MergeBaseCommitId;
            Sha1Id mergeSourceCommitId = this.m_mergeSourceCommitId;
            Sha1Id mergeTargetCommitId = this.m_mergeTargetCommitId;
            ConflictCollection conflicts = mergeResult.Index.Conflicts;
            IndexReucEntryCollection resolvedConflicts = mergeResult.Index.Conflicts.ResolvedConflicts;
            Index index = mergeResult.Index;
            int num = this.m_detectRenameFalsePositives ? 1 : 0;
            new LibGit2ConflictMapper(requestContext, mergeOrigin, mergeBaseCommitId, mergeSourceCommitId, mergeTargetCommitId, (IEnumerable<Conflict>) conflicts, (IEnumerable<IndexReucEntry>) resolvedConflicts, (IEnumerable<LibGit2Sharp.IndexEntry>) index, num != 0).MapLibGit2Conflicts(out conflictSet.ConflictsForThisMerge, out conflictSet.TrivialConflicts);
            source = this.ReconcileConflictLists((IList<Microsoft.TeamFoundation.Git.Server.GitConflict>) conflictSet.ConflictsForAllPreviousMerges, (IList<Microsoft.TeamFoundation.Git.Server.GitConflict>) conflictSet.ConflictsForThisMerge);
            break;
          case PullRequestAsyncStatus.Succeeded:
            source = this.ReconcileConflictLists((IList<Microsoft.TeamFoundation.Git.Server.GitConflict>) conflictSet.ConflictsForAllPreviousMerges, (IList<Microsoft.TeamFoundation.Git.Server.GitConflict>) new List<Microsoft.TeamFoundation.Git.Server.GitConflict>());
            break;
          default:
            source = conflictSet.ConflictsForAllPreviousMerges;
            break;
        }
        conflictSet.ConflictsForThisMerge = new List<Microsoft.TeamFoundation.Git.Server.GitConflict>();
        conflictSet.ConflictsForAllPreviousMerges = new List<Microsoft.TeamFoundation.Git.Server.GitConflict>();
        foreach (Microsoft.TeamFoundation.Git.Server.GitConflict gitConflict in (IEnumerable<Microsoft.TeamFoundation.Git.Server.GitConflict>) source.OrderBy<Microsoft.TeamFoundation.Git.Server.GitConflict, DateTime>((Func<Microsoft.TeamFoundation.Git.Server.GitConflict, DateTime>) (c => c.ResolvedDate)))
        {
          if (gitConflict.MergeSourceCommitId == this.m_mergeSourceCommitId && gitConflict.MergeTargetCommitId == this.m_mergeTargetCommitId)
            conflictSet.ConflictsForThisMerge.Add(gitConflict);
          else
            conflictSet.ConflictsForAllPreviousMerges.Add(gitConflict);
        }
        return conflictSet;
      }
    }

    private List<Microsoft.TeamFoundation.Git.Server.GitConflict> ReconcileConflictLists(
      IList<Microsoft.TeamFoundation.Git.Server.GitConflict> existingConflicts,
      IList<Microsoft.TeamFoundation.Git.Server.GitConflict> newConflicts)
    {
      List<Microsoft.TeamFoundation.Git.Server.GitConflict> source = new List<Microsoft.TeamFoundation.Git.Server.GitConflict>();
      Dictionary<IGitConflict, Microsoft.TeamFoundation.Git.Server.GitConflict> dictionary = newConflicts.ToDictionary<Microsoft.TeamFoundation.Git.Server.GitConflict, IGitConflict>((Func<Microsoft.TeamFoundation.Git.Server.GitConflict, IGitConflict>) (c => (IGitConflict) c), IGitConflictExtensions.SameConflictComparer);
      for (int index = 0; index < existingConflicts.Count; ++index)
      {
        Microsoft.TeamFoundation.Git.Server.GitConflict existingConflict = existingConflicts[index];
        Microsoft.TeamFoundation.Git.Server.GitConflict key;
        dictionary.TryGetValue((IGitConflict) existingConflict, out key);
        if (key != null)
        {
          List<Microsoft.TeamFoundation.Git.Server.GitConflict> gitConflictList = source;
          Microsoft.TeamFoundation.Git.Server.GitConflict gitConflict1 = existingConflict;
          Sha1Id? nullable1 = new Sha1Id?(key.MergeBaseCommitId);
          Sha1Id? nullable2 = new Sha1Id?(key.MergeSourceCommitId);
          Sha1Id? nullable3 = new Sha1Id?(key.MergeTargetCommitId);
          GitConflictSourceType? conflictSourceType = new GitConflictSourceType?();
          int? conflictSourceId = new int?();
          int? conflictId = new int?();
          Sha1Id? mergeBaseCommitId = nullable1;
          Sha1Id? mergeSourceCommitId = nullable2;
          Sha1Id? mergeTargetCommitId = nullable3;
          GitConflictType? conflictType = new GitConflictType?();
          Sha1Id? baseObjectId = new Sha1Id?();
          Sha1Id? baseObjectIdForTarget = new Sha1Id?();
          Sha1Id? sourceObjectId = new Sha1Id?();
          Sha1Id? targetObjectId = new Sha1Id?();
          GitResolutionStatus? resolutionStatus = new GitResolutionStatus?();
          GitResolutionError? resolutionError = new GitResolutionError?();
          byte? resolutionAction = new byte?();
          byte? resolutionMergeType = new byte?();
          Sha1Id? resolutionObjectId = new Sha1Id?();
          Guid? resolvedBy = new Guid?();
          DateTime? resolvedDate = new DateTime?();
          Guid? resolutionAuthor = new Guid?();
          Microsoft.TeamFoundation.Git.Server.GitConflict gitConflict2 = gitConflict1.CopyAndUpdate(conflictSourceType, conflictSourceId, conflictId, mergeBaseCommitId, mergeSourceCommitId, mergeTargetCommitId, conflictType, baseObjectId, baseObjectIdForTarget, sourceObjectId, targetObjectId, resolutionStatus, resolutionError, resolutionAction, resolutionMergeType, resolutionObjectId, resolvedBy, resolvedDate, resolutionAuthor);
          gitConflictList.Add(gitConflict2);
          dictionary.Remove((IGitConflict) key);
        }
        else if (existingConflict.MergeSourceCommitId != this.m_mergeSourceCommitId || existingConflict.MergeTargetCommitId != this.m_mergeTargetCommitId)
          source.Add(existingConflict);
      }
      if (dictionary.Count > 0)
      {
        int num = source.DefaultIfEmpty<Microsoft.TeamFoundation.Git.Server.GitConflict>().Max<Microsoft.TeamFoundation.Git.Server.GitConflict>((Func<Microsoft.TeamFoundation.Git.Server.GitConflict, int>) (c => c == null ? 0 : c.ConflictId)) + 1;
        foreach (Microsoft.TeamFoundation.Git.Server.GitConflict key in dictionary.Keys)
        {
          List<Microsoft.TeamFoundation.Git.Server.GitConflict> gitConflictList = source;
          Microsoft.TeamFoundation.Git.Server.GitConflict gitConflict3 = key;
          int? nullable = new int?(num++);
          GitConflictSourceType? conflictSourceType = new GitConflictSourceType?();
          int? conflictSourceId = new int?();
          int? conflictId = nullable;
          Sha1Id? mergeBaseCommitId = new Sha1Id?();
          Sha1Id? mergeSourceCommitId = new Sha1Id?();
          Sha1Id? mergeTargetCommitId = new Sha1Id?();
          GitConflictType? conflictType = new GitConflictType?();
          Sha1Id? baseObjectId = new Sha1Id?();
          Sha1Id? baseObjectIdForTarget = new Sha1Id?();
          Sha1Id? sourceObjectId = new Sha1Id?();
          Sha1Id? targetObjectId = new Sha1Id?();
          GitResolutionStatus? resolutionStatus = new GitResolutionStatus?();
          GitResolutionError? resolutionError = new GitResolutionError?();
          byte? resolutionAction = new byte?();
          byte? resolutionMergeType = new byte?();
          Sha1Id? resolutionObjectId = new Sha1Id?();
          Guid? resolvedBy = new Guid?();
          DateTime? resolvedDate = new DateTime?();
          Guid? resolutionAuthor = new Guid?();
          Microsoft.TeamFoundation.Git.Server.GitConflict gitConflict4 = gitConflict3.CopyAndUpdate(conflictSourceType, conflictSourceId, conflictId, mergeBaseCommitId, mergeSourceCommitId, mergeTargetCommitId, conflictType, baseObjectId, baseObjectIdForTarget, sourceObjectId, targetObjectId, resolutionStatus, resolutionError, resolutionAction, resolutionMergeType, resolutionObjectId, resolvedBy, resolvedDate, resolutionAuthor);
          gitConflictList.Add(gitConflict4);
        }
      }
      return source;
    }

    protected class ConflictSet
    {
      public List<Microsoft.TeamFoundation.Git.Server.GitConflict> ConflictsForAllPreviousMerges;
      public List<Microsoft.TeamFoundation.Git.Server.GitConflict> ConflictsForThisMerge;
      public List<TrivialConflict> TrivialConflicts;

      public bool NoConflictsForThisMerge()
      {
        List<Microsoft.TeamFoundation.Git.Server.GitConflict> conflictsForThisMerge = this.ConflictsForThisMerge;
        if ((conflictsForThisMerge != null ? (!conflictsForThisMerge.Any<Microsoft.TeamFoundation.Git.Server.GitConflict>() ? 1 : 0) : 0) == 0)
          return false;
        List<TrivialConflict> trivialConflicts = this.TrivialConflicts;
        return trivialConflicts != null && !trivialConflicts.Any<TrivialConflict>();
      }

      public bool HasResolvedConflicts()
      {
        List<Microsoft.TeamFoundation.Git.Server.GitConflict> conflictsForThisMerge = this.ConflictsForThisMerge;
        if ((conflictsForThisMerge != null ? (conflictsForThisMerge.Any<Microsoft.TeamFoundation.Git.Server.GitConflict>((Func<Microsoft.TeamFoundation.Git.Server.GitConflict, bool>) (c => c.ResolutionStatus == GitResolutionStatus.Resolved)) ? 1 : 0) : 0) != 0)
          return true;
        List<TrivialConflict> trivialConflicts = this.TrivialConflicts;
        return trivialConflicts != null && trivialConflicts.Any<TrivialConflict>();
      }
    }

    public class Result
    {
      public IEnumerable<Microsoft.TeamFoundation.Git.Server.GitConflict> Conflicts { get; set; } = (IEnumerable<Microsoft.TeamFoundation.Git.Server.GitConflict>) new List<Microsoft.TeamFoundation.Git.Server.GitConflict>();

      public PullRequestAsyncStatus MergeStatus { get; set; }

      public Sha1Id MergeCommitId { get; set; }

      public Sha1Id ConflictResolutionHash { get; set; }

      public IdentityDescriptor ConflictResolver { get; set; }

      public string FailureMessage { get; set; }

      public PullRequestMergeFailureType FailureType { get; set; }

      public Sha1Id SourceFixupCommitId { get; set; } = Sha1Id.Empty;

      public Sha1Id TargetFixupCommitId { get; set; } = Sha1Id.Empty;
    }
  }
}
