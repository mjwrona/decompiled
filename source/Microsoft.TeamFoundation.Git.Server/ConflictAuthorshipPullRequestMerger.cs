// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ConflictAuthorshipPullRequestMerger
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using LibGit2Sharp;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Native;
using Microsoft.TeamFoundation.Git.Server.Services.PullRequest;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class ConflictAuthorshipPullRequestMerger : GitIndexPullRequestMerger
  {
    private const string c_Layer = "ConflictAuthorshipPullRequestMerger";

    public ConflictAuthorshipPullRequestMerger(
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

    protected override Sha1Id[] GetParentCommits(
      PullRequestMergerBase.ConflictSet conflicts,
      LibGit2NativeLibrary nativeLibrary)
    {
      Sha1Id[] sha1IdArray = new Sha1Id[2]
      {
        this.m_mergeSourceCommitId,
        this.m_mergeTargetCommitId
      };
      string commitMessage = string.Format("Conflict resolutions for !{0}", (object) this.m_pullRequest.PullRequestId);
      IReadOnlyDictionary<Guid, LibGit2Sharp.Signature> signatureLookup = ITfsGitRepositoryExtensions.CreateSignatures(this.m_requestContext, this.m_requestContext.GetService<ITeamFoundationIdentityService>(), conflicts.ConflictsForThisMerge.SelectMany<GitConflict, Guid>((Func<GitConflict, IEnumerable<Guid>>) (conflict => (IEnumerable<Guid>) new Guid[2]
      {
        conflict.ResolutionAuthor,
        conflict.ResolvedBy
      })).Distinct<Guid>().Where<Guid>((Func<Guid, bool>) (id => id != Guid.Empty)).ToArray<Guid>());
      Guid mergeCompleterId = this.m_pullRequest.CompleteWhenMergedAuthority == Guid.Empty ? this.m_pullRequest.Creator : this.m_pullRequest.CompleteWhenMergedAuthority;
      foreach (IGrouping<(Guid, Guid), GitConflict> grouping in (IEnumerable<IGrouping<(Guid, Guid), GitConflict>>) conflicts.ConflictsForThisMerge.GroupBy<GitConflict, (Guid, Guid)>((Func<GitConflict, (Guid, Guid)>) (c => (!signatureLookup.ContainsKey(c.ResolutionAuthor) || !(signatureLookup[c.ResolutionAuthor] != (LibGit2Sharp.Signature) null) ? c.ResolvedBy : c.ResolutionAuthor, c.ResolvedBy))).Where<IGrouping<(Guid, Guid), GitConflict>>((Func<IGrouping<(Guid, Guid), GitConflict>, bool>) (g => g.Key.AuthorId != this.m_pullRequest.Creator || g.Key.CommitterId != mergeCompleterId)).OrderBy<IGrouping<(Guid, Guid), GitConflict>, string>((Func<IGrouping<(Guid, Guid), GitConflict>, string>) (g => signatureLookup[g.Key.AuthorId].Name)).ThenBy<IGrouping<(Guid, Guid), GitConflict>, string>((Func<IGrouping<(Guid, Guid), GitConflict>, string>) (g => signatureLookup[g.Key.CommitterId].Name)))
      {
        IGrouping<(Guid, Guid), GitConflict> authorGroup = grouping;
        using (this.m_requestContext.TimeRegion(nameof (ConflictAuthorshipPullRequestMerger), nameof (GetParentCommits)))
        {
          try
          {
            nativeLibrary.CreateNewCommitsWithTreeDefinition(signatureLookup[authorGroup.Key.Item1], signatureLookup[authorGroup.Key.Item2], commitMessage, sha1IdArray, (Action<Repository, TreeDefinition[]>) ((nativeRepo, trees) => this.ApplyAllResolutions(nativeRepo, (IEnumerable<GitConflict>) authorGroup, trees)));
          }
          catch (Exception ex) when (TracepointUtils.TraceException(this.m_requestContext, 1013036, GitServerUtils.TraceArea, nameof (ConflictAuthorshipPullRequestMerger), ex, (object) new
          {
            RepositoryId = this.m_pullRequest.RepositoryId,
            PullRequestId = this.m_pullRequest.PullRequestId
          }, caller: nameof (GetParentCommits)))
          {
          }
        }
      }
      return ((IEnumerable<Sha1Id>) sha1IdArray).Reverse<Sha1Id>().ToArray<Sha1Id>();
    }

    internal void ApplyAllResolutions(
      Repository nativeRepo,
      IEnumerable<GitConflict> conflicts,
      TreeDefinition[] trees)
    {
      foreach (GitConflict conflict in conflicts)
      {
        if (conflict.ResolutionStatus == GitResolutionStatus.Resolved)
          this.ApplyOneResolution(nativeRepo, conflict, trees);
      }
    }

    private void ApplyOneResolution(
      Repository nativeRepo,
      GitConflict conflict,
      TreeDefinition[] trees)
    {
      try
      {
        TreeDefinition tree1 = trees[0];
        TreeDefinition tree2 = trees[1];
        switch (conflict.ConflictType)
        {
          case GitConflictType.AddAdd:
            ResolutionMethods.Resolve_AddAdd_EditEdit_RenameRename(nativeRepo, (IGitConflict) conflict, tree1, tree2);
            break;
          case GitConflictType.AddRename:
            ResolutionMethods.Resolve_AddRename(nativeRepo, (IGitConflict) conflict, tree1, tree2);
            break;
          case GitConflictType.DeleteEdit:
            ResolutionMethods.Resolve_EditDelete_DeleteEdit(nativeRepo, (IGitConflict) conflict, tree1, tree2);
            break;
          case GitConflictType.DeleteRename:
            ResolutionMethods.Resolve_DeleteRename(nativeRepo, (IGitConflict) conflict, tree1, tree2);
            break;
          case GitConflictType.EditDelete:
            ResolutionMethods.Resolve_EditDelete_DeleteEdit(nativeRepo, (IGitConflict) conflict, tree1, tree2);
            break;
          case GitConflictType.EditEdit:
            ResolutionMethods.Resolve_AddAdd_EditEdit_RenameRename(nativeRepo, (IGitConflict) conflict, tree1, tree2);
            break;
          case GitConflictType.Rename1to2:
            ResolutionMethods.Resolve_Rename1to2(nativeRepo, (IGitConflict) conflict, tree1, tree2);
            break;
          case GitConflictType.Rename2to1:
            ResolutionMethods.Resolve_Rename2to1(nativeRepo, (IGitConflict) conflict, tree1, tree2);
            break;
          case GitConflictType.RenameAdd:
            ResolutionMethods.Resolve_RenameAdd(nativeRepo, (IGitConflict) conflict, tree1, tree2);
            break;
          case GitConflictType.RenameDelete:
            ResolutionMethods.Resolve_RenameDelete(nativeRepo, (IGitConflict) conflict, tree1, tree2);
            break;
          case GitConflictType.RenameRename:
            ResolutionMethods.Resolve_AddAdd_EditEdit_RenameRename(nativeRepo, (IGitConflict) conflict, tree1, tree2);
            break;
        }
      }
      catch (Exception ex)
      {
        TracepointUtils.TraceException(this.m_requestContext, 1013031, GitServerUtils.TraceArea, nameof (ConflictAuthorshipPullRequestMerger), ex, (object) new
        {
          RepositoryId = this.m_pullRequest.RepositoryId,
          PullRequestId = this.m_pullRequest.PullRequestId
        }, caller: nameof (ApplyOneResolution));
      }
    }
  }
}
