// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ITfsGitRepositoryExtensions
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Native;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class ITfsGitRepositoryExtensions
  {
    private const string s_Layer = "PullRequests_ITfsGitRepositoryExtensions";

    internal static AsyncRefOperationResult CreateNativeCherryPick(
      this ITfsGitRepository repository,
      IVssRequestContext requestContext,
      GitAsyncRefOperation asyncOperation,
      IReadOnlyList<Sha1Id> initialCommits,
      Action<Sha1Id, double> progressCallback,
      Action<Sha1Id> conflictCallback,
      ClientTraceData ctData,
      out List<Sha1Id> resultCommits,
      out Sha1Id resultBaseCommitId)
    {
      ITeamFoundationIdentityService service = requestContext.GetService<ITeamFoundationIdentityService>();
      LibGit2Sharp.Signature signature = ITfsGitRepositoryExtensions.CreateSignature(requestContext, service, asyncOperation.CreatorId);
      Sha1Id objectId = (repository.Refs.MatchingName(asyncOperation.Parameters.OntoRefName) ?? throw new GitRefNotFoundException(asyncOperation.Parameters.OntoRefName, repository.Key.RepoId.ToString())).ObjectId;
      resultBaseCommitId = objectId;
      AsyncOperationParameters parameters = new AsyncOperationParameters(requestContext, objectId, signature, initialCommits);
      using (LibGit2NativeLibrary git2NativeLibrary = new LibGit2NativeLibrary(requestContext, repository))
      {
        List<(Sha1Id, Sha1Id)> sourceToTargetCommitIds;
        AsyncOperationResult asyncOperationResult = git2NativeLibrary.TryCherryPick(parameters, ctData, progressCallback, conflictCallback, out sourceToTargetCommitIds);
        ctData?.Add("CherryPickSucceeded", (object) asyncOperationResult.Success);
        if (!asyncOperationResult.Success)
        {
          resultCommits = new List<Sha1Id>();
          if (asyncOperationResult.Conflicts != null)
            ctData.Add("AsyncOperationConflicts", (object) asyncOperationResult.Conflicts.Select(c => new
            {
              path = (c.Ancestor?.Path ?? c.Ours?.Path ?? c.Theirs?.Path)?.Replace('\\', '/'),
              baseObjectId = c.Ancestor?.Id,
              sourceObjectId = c.Theirs?.Id,
              targetObjectId = c.Ours?.Id
            }).ToArray());
          return asyncOperationResult.TimeOut ? AsyncRefOperationResult.Timeout : AsyncRefOperationResult.Failure;
        }
        resultCommits = asyncOperationResult.Commits;
        try
        {
          if (requestContext.IsFeatureEnabled("Git.CherryPickRelationship"))
            repository.CherryPickRelationships.Add(sourceToTargetCommitIds);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1013768, GitServerUtils.TraceArea, "PullRequests_ITfsGitRepositoryExtensions", ex);
        }
        return AsyncRefOperationResult.Success;
      }
    }

    internal static AsyncRefOperationResult CreateNativeRevert(
      this ITfsGitRepository repository,
      IVssRequestContext requestContext,
      GitAsyncRefOperation asyncOperation,
      IReadOnlyList<Sha1Id> initialCommits,
      Action<Sha1Id, double> progressCallback,
      Action<Sha1Id> conflictCallback,
      ClientTraceData ctData,
      out List<Sha1Id> resultCommits,
      out Sha1Id resultBaseCommitId)
    {
      ITeamFoundationIdentityService service = requestContext.GetService<ITeamFoundationIdentityService>();
      LibGit2Sharp.Signature signature = ITfsGitRepositoryExtensions.CreateSignature(requestContext, service, asyncOperation.CreatorId);
      Sha1Id objectId = (repository.Refs.MatchingName(asyncOperation.Parameters.OntoRefName) ?? throw new GitRefNotFoundException(asyncOperation.Parameters.OntoRefName, repository.Key.RepoId.ToString())).ObjectId;
      resultBaseCommitId = objectId;
      AsyncOperationParameters parameters = new AsyncOperationParameters(requestContext, objectId, signature, initialCommits);
      using (LibGit2NativeLibrary git2NativeLibrary = new LibGit2NativeLibrary(requestContext, repository))
      {
        AsyncOperationResult asyncOperationResult = git2NativeLibrary.TryRevert(parameters, ctData, progressCallback, conflictCallback);
        ctData?.Add("RevertSucceeded", (object) asyncOperationResult.Success);
        if (!asyncOperationResult.Success)
        {
          resultCommits = new List<Sha1Id>();
          if (asyncOperationResult.Conflicts != null)
            ctData.Add("AsyncOperationConflicts", (object) asyncOperationResult.Conflicts.Select(c => new
            {
              path = (c.Ancestor?.Path ?? c.Ours?.Path ?? c.Theirs?.Path)?.Replace('\\', '/'),
              baseObjectId = c.Ancestor?.Id,
              sourceObjectId = c.Theirs?.Id,
              targetObjectId = c.Ours?.Id
            }).ToArray());
          return asyncOperationResult.TimeOut ? AsyncRefOperationResult.Timeout : AsyncRefOperationResult.Failure;
        }
        resultCommits = asyncOperationResult.Commits;
        return AsyncRefOperationResult.Success;
      }
    }

    internal static TfsGitRefUpdateResultSet UpdateGeneratedBranchRef(
      this ITfsGitRepository repository,
      IVssRequestContext requestContext,
      GitAsyncRefOperation asyncOperation,
      List<Sha1Id> commits,
      Sha1Id resultBaseCommitId)
    {
      TfsGitRefUpdateRequest refUpdateRequest;
      if (commits.Count == 0)
      {
        refUpdateRequest = new TfsGitRefUpdateRequest(asyncOperation.Parameters.GeneratedRefName, Sha1Id.Empty, resultBaseCommitId);
      }
      else
      {
        Sha1Id newObjectId = commits.Last<Sha1Id>();
        refUpdateRequest = new TfsGitRefUpdateRequest(asyncOperation.Parameters.GeneratedRefName, Sha1Id.Empty, newObjectId);
      }
      return repository.updateBranchRef(requestContext, refUpdateRequest);
    }

    internal static TfsGitRefUpdateResultSet UpdateTargetBranchRef(
      this ITfsGitRepository repository,
      IVssRequestContext requestContext,
      GitAsyncRefOperation asyncOperation,
      List<Sha1Id> commits,
      Sha1Id resultBaseCommitId)
    {
      TfsGitRefUpdateRequest refUpdateRequest = new TfsGitRefUpdateRequest(asyncOperation.Parameters.OntoRefName, resultBaseCommitId, commits.Last<Sha1Id>());
      return repository.updateBranchRef(requestContext, refUpdateRequest);
    }

    private static TfsGitRefUpdateResultSet updateBranchRef(
      this ITfsGitRepository repository,
      IVssRequestContext requestContext,
      TfsGitRefUpdateRequest refUpdateRequest)
    {
      TfsGitRefUpdateResultSet refUpdateResultSet = requestContext.GetService<ITeamFoundationGitRefService>().UpdateRefs(requestContext, repository.Key.RepoId, new List<TfsGitRefUpdateRequest>()
      {
        refUpdateRequest
      });
      if (refUpdateResultSet.CountFailed > 0)
      {
        requestContext.Trace(1013572, TraceLevel.Verbose, GitServerUtils.TraceArea, "PullRequests_ITfsGitRepositoryExtensions", "A PR ref failed to update.");
        foreach (TfsGitRefUpdateResult result in refUpdateResultSet.Results)
        {
          if (!result.Succeeded)
          {
            string message = string.Format("Failure: {0}\nOld: {1}\nNew: {2}\nStatus: {3}\nMessage: {4}", (object) result.Name, (object) result.OldObjectId, (object) result.NewObjectId, (object) result.Status, (object) result.CustomMessage);
            requestContext.Trace(1013575, TraceLevel.Verbose, GitServerUtils.TraceArea, "PullRequests_ITfsGitRepositoryExtensions", message);
          }
        }
      }
      return refUpdateResultSet;
    }

    internal static void UpdatePullRequestMergeRef(
      this ITfsGitRepository repository,
      IVssRequestContext rc,
      int pullRequestId,
      Sha1Id? newMergeCommitId)
    {
      using (rc.TimeRegion("PullRequests_ITfsGitRepositoryExtensions", nameof (UpdatePullRequestMergeRef)))
      {
        string mergeRefName = TfsGitPullRequest.GetMergeRefName(pullRequestId);
        TfsGitRef tfsGitRef = repository.Refs.MatchingName(mergeRefName);
        TfsGitRefUpdateRequest refUpdateRequest;
        if (tfsGitRef != null)
        {
          refUpdateRequest = !newMergeCommitId.HasValue ? new TfsGitRefUpdateRequest(mergeRefName, tfsGitRef.ObjectId, Sha1Id.Empty) : new TfsGitRefUpdateRequest(mergeRefName, tfsGitRef.ObjectId, newMergeCommitId.Value);
        }
        else
        {
          if (!newMergeCommitId.HasValue)
            return;
          refUpdateRequest = new TfsGitRefUpdateRequest(mergeRefName, Sha1Id.Empty, newMergeCommitId.Value);
        }
        TfsGitRefUpdateResultSet refUpdateResultSet = rc.GetService<ITeamFoundationGitRefService>().UpdateRefs(rc.Elevate(), repository.Key.RepoId, new List<TfsGitRefUpdateRequest>(1)
        {
          refUpdateRequest
        });
        if (refUpdateResultSet.CountFailed <= 0)
          return;
        rc.Trace(1013338, TraceLevel.Verbose, GitServerUtils.TraceArea, "PullRequests_ITfsGitRepositoryExtensions", "A PR ref failed to update.");
        foreach (TfsGitRefUpdateResult result in refUpdateResultSet.Results)
        {
          if (!result.Succeeded)
          {
            string message = string.Format("Failure: {0}\nOld: {1}\nNew: {2}\nStatus: {3}\nMessage: {4}", (object) result.Name, (object) result.OldObjectId, (object) result.NewObjectId, (object) result.Status, (object) result.CustomMessage);
            rc.Trace(1013338, TraceLevel.Verbose, GitServerUtils.TraceArea, "PullRequests_ITfsGitRepositoryExtensions", message);
          }
        }
      }
    }

    private static MergeParameters PrepareNativeMergeParameters(
      IVssRequestContext rc,
      TfsGitPullRequest pr,
      Sha1Id targetBranchTipCommit,
      Sha1Id sourceCommit,
      GitPullRequestCompletionOptions completionOptions)
    {
      ArgumentUtility.CheckForNull<TfsGitPullRequest>(pr, nameof (pr));
      CommitDetails commitDetails = ITfsGitRepositoryExtensions.CreateCommitDetails(rc, pr, completionOptions?.MergeCommitMessage);
      completionOptions?.Normalize();
      GitPullRequestMergeStrategy? mergeStrategy = (GitPullRequestMergeStrategy?) completionOptions?.MergeStrategy;
      if (mergeStrategy.HasValue)
      {
        switch (mergeStrategy.GetValueOrDefault())
        {
          case GitPullRequestMergeStrategy.Rebase:
          case GitPullRequestMergeStrategy.RebaseMerge:
            throw new NotSupportedException();
        }
      }
      return new MergeParameters()
      {
        CommitDetails = commitDetails,
        Squash = completionOptions.SquashMerge,
        SourceBranchCommitId = sourceCommit,
        TargetBranchCommitId = targetBranchTipCommit
      };
    }

    internal static CommitDetails CreateCommitDetails(
      IVssRequestContext rc,
      TfsGitPullRequest pr,
      string commitMessage = null)
    {
      ITeamFoundationIdentityService service = rc.GetService<ITeamFoundationIdentityService>();
      LibGit2Sharp.Signature signature = ITfsGitRepositoryExtensions.CreateSignature(rc, service, pr.Creator);
      LibGit2Sharp.Signature committer = !(pr.CompleteWhenMergedAuthority != Guid.Empty) ? signature : ITfsGitRepositoryExtensions.CreateSignature(rc, service, pr.CompleteWhenMergedAuthority);
      if (string.IsNullOrWhiteSpace(commitMessage))
        commitMessage = Resources.Format("MergeCommitMessage", (object) pr.PullRequestId, (object) GitUtils.GetFriendlyBranchName(pr.IsFromFork ? pr.ForkSource.RefName : pr.SourceBranchName), (object) GitUtils.GetFriendlyBranchName(pr.TargetBranchName));
      return new CommitDetails(commitMessage, signature, committer);
    }

    internal static CommitDetails CreateCommitDetails(
      IVssRequestContext rc,
      Guid creatorId,
      string commitMessage)
    {
      ITeamFoundationIdentityService service = rc.GetService<ITeamFoundationIdentityService>();
      LibGit2Sharp.Signature signature = ITfsGitRepositoryExtensions.CreateSignature(rc, service, creatorId);
      return new CommitDetails(commitMessage, signature, signature);
    }

    internal static IReadOnlyDictionary<Guid, LibGit2Sharp.Signature> CreateSignatures(
      IVssRequestContext rc,
      ITeamFoundationIdentityService identityService,
      Guid[] identityGuids)
    {
      TeamFoundationIdentity[] userIdentities = IdentityHelper.Instance.GetUserIdentities(rc, identityService, identityGuids);
      using (rc.TimeRegion("PullRequests_ITfsGitRepositoryExtensions", nameof (CreateSignatures)))
      {
        Dictionary<Guid, LibGit2Sharp.Signature> signatures = new Dictionary<Guid, LibGit2Sharp.Signature>();
        for (int index = 0; index < identityGuids.Length; ++index)
        {
          TeamFoundationIdentity identity = userIdentities[index];
          LibGit2Sharp.Signature signature = (LibGit2Sharp.Signature) null;
          if (identity != null)
            ITfsGitRepositoryExtensions.TryCreateSignature(rc, identityService, identity, out signature);
          signatures[identityGuids[index]] = signature;
        }
        return (IReadOnlyDictionary<Guid, LibGit2Sharp.Signature>) signatures;
      }
    }

    internal static LibGit2Sharp.Signature CreateSignature(
      IVssRequestContext rc,
      ITeamFoundationIdentityService identityService,
      Guid identityGuid)
    {
      TeamFoundationIdentity userIdentity = IdentityHelper.Instance.GetUserIdentity(rc, identityService, identityGuid);
      using (rc.TimeRegion("PullRequests_ITfsGitRepositoryExtensions", nameof (CreateSignature)))
      {
        LibGit2Sharp.Signature signature;
        if (ITfsGitRepositoryExtensions.TryCreateSignature(rc, identityService, userIdentity, out signature))
          return signature;
        throw new EmptySignatureException();
      }
    }

    internal static void SyncForkMirrorRef(
      IVssRequestContext rc,
      ITfsGitRepository targetRepo,
      int pullRequestId,
      Sha1Id sourceObjectId,
      ClientTraceData ctData = null)
    {
      string forkSourceRefName = TfsGitPullRequest.GetForkSourceRefName(pullRequestId);
      TfsGitRef tfsGitRef = ITfsGitRepositoryExtensions.VerifyLocalRef(targetRepo, forkSourceRefName, true);
      TfsGitRefUpdateResultSet refUpdateResultSet = rc.GetService<ITeamFoundationGitRefService>().UpdateRefs(rc.Elevate(), targetRepo.Key.RepoId, new List<TfsGitRefUpdateRequest>()
      {
        new TfsGitRefUpdateRequest(forkSourceRefName, tfsGitRef != null ? tfsGitRef.ObjectId : Sha1Id.Empty, sourceObjectId)
      }, ctData: ctData);
      rc.Trace(1013738, TraceLevel.Verbose, GitServerUtils.TraceArea, "PullRequests_ITfsGitRepositoryExtensions", string.Format("Updating Fork Mirror Ref for Pull Request {0}, repository: {1}", (object) pullRequestId, (object) targetRepo.Key.RepoId));
      if (refUpdateResultSet.CountFailed > 0)
      {
        TfsGitRefUpdateResult gitRefUpdateResult = refUpdateResultSet.Results.FindAll((Predicate<TfsGitRefUpdateResult>) (r => !r.Succeeded)).FirstOrDefault<TfsGitRefUpdateResult>();
        throw new GitPullRequestUnableToSyncForkRef(string.Format("CannotSyncForkRef", (object) TfsGitPullRequest.GetForkSourceRefName(pullRequestId), (object) targetRepo.Key.RepoId, (object) gitRefUpdateResult?.OldObjectId, (object) gitRefUpdateResult?.NewObjectId, (object) gitRefUpdateResult?.Status));
      }
    }

    internal static TfsGitRef VerifyLocalRef(
      ITfsGitRepository repo,
      string refName,
      bool allowDeletion = false)
    {
      TfsGitRef tfsGitRef = repo.Refs.MatchingNames((IEnumerable<string>) new string[1]
      {
        refName
      }).SingleOrDefault<TfsGitRef>((Func<TfsGitRef, bool>) (r => r.Name == refName));
      if (tfsGitRef == null & allowDeletion)
        return (TfsGitRef) null;
      if (tfsGitRef == null)
        throw new GitPullRequestCannotBeActivated();
      return tfsGitRef.Name.StartsWith("refs/heads/", StringComparison.Ordinal) || tfsGitRef.Name.StartsWith("refs/pull/", StringComparison.Ordinal) && tfsGitRef.Name.EndsWith("source", StringComparison.Ordinal) ? tfsGitRef : throw new GitPullRequestCannotBeActivated();
    }

    private static bool TryCreateSignature(
      IVssRequestContext rc,
      ITeamFoundationIdentityService identityService,
      TeamFoundationIdentity identity,
      out LibGit2Sharp.Signature signature)
    {
      string email = identityService.GetPreferredEmailAddress(rc, identity.TeamFoundationId);
      string displayName = identity.DisplayName;
      if (string.IsNullOrEmpty(email))
        email = displayName;
      if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(displayName))
      {
        signature = (LibGit2Sharp.Signature) null;
        return false;
      }
      signature = new LibGit2Sharp.Signature(identity.DisplayName, email, DateTimeOffset.Now);
      return true;
    }
  }
}
