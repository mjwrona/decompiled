// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitPullRequest
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Graph;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TfsGitPullRequest
  {
    private const string PermaLinkPrefix = "_permalink";
    private string m_mergeRef;
    protected IReadOnlyList<TfsGitCommitChange> m_changes;
    protected TfsGitCommit m_lastCommonCommit;
    private IReadOnlyList<TfsGitCommitChange> m_changesForPolicy;
    private IReadOnlyList<Sha1Id> m_commitIds;
    private int? m_commitsSkip;
    private int? m_commitsLimit;
    private IReadOnlyList<GitPullRequestStatus> m_statuses;
    private string m_sourceBranchName;
    private const string c_layer = "TfsGitPullRequest";
    private const int c_numberOfCommitsLimit = 100;

    internal TfsGitPullRequest(
      Guid repositoryId,
      int pullRequestId,
      PullRequestStatus status,
      Guid creator,
      DateTime creationDate,
      DateTime closedDate,
      string title,
      string description,
      string sourceBranchName,
      string targetBranchName,
      PullRequestAsyncStatus mergeStatus,
      Guid mergeId,
      Sha1Id? lastMergeSourceCommit,
      Sha1Id? lastMergeTargetCommit,
      Sha1Id? lastMergeCommit,
      Guid completeWhenMergedAuthority,
      IEnumerable<TfsGitPullRequest.ReviewerWithVote> reviewers,
      int codeReviewId = 0,
      bool supportsIterations = false,
      GitPullRequestCompletionOptions completionOptions = null,
      GitPullRequestMergeOptions mergeOptions = null,
      PullRequestMergeFailureType mergeFailureType = PullRequestMergeFailureType.None,
      string mergeFailureMessage = null,
      DateTime completionQueueTime = default (DateTime),
      Guid autoCompleteAuthority = default (Guid),
      string repositoryName = null,
      bool repositoryCreatedByForking = false,
      string projectUri = null,
      Guid? sourceRepositoryId = null,
      bool isDraft = false,
      DateTime updatedTime = default (DateTime))
    {
      if ((!sourceRepositoryId.HasValue || !(sourceRepositoryId.Value != Guid.Empty) ? 0 : (sourceRepositoryId.Value != repositoryId ? 1 : 0)) != 0)
        this.ForkSource = new TfsGitForkRef(sourceRepositoryId.Value, sourceBranchName);
      this.RepositoryId = repositoryId;
      this.RepositoryName = repositoryName;
      this.RepositoryCreatedByForking = repositoryCreatedByForking;
      this.ProjectUri = projectUri;
      this.PullRequestId = pullRequestId;
      this.Status = status;
      this.Creator = creator;
      this.CreationDate = creationDate;
      this.ClosedDate = closedDate;
      this.Title = title;
      this.Description = description;
      this.TargetBranchName = targetBranchName;
      this.m_sourceBranchName = sourceBranchName;
      this.MergeStatus = mergeStatus;
      this.MergeFailureType = mergeFailureType;
      this.MergeFailureMessage = mergeFailureMessage;
      this.IsDraft = isDraft;
      this.MergeId = mergeId;
      this.LastMergeSourceCommit = lastMergeSourceCommit;
      this.LastMergeTargetCommit = lastMergeTargetCommit;
      this.LastMergeCommit = lastMergeCommit;
      this.Reviewers = reviewers;
      this.CompleteWhenMergedAuthority = completeWhenMergedAuthority;
      this.CodeReviewId = codeReviewId;
      this.SupportsIterations = supportsIterations;
      this.CompletionOptions = completionOptions;
      this.MergeOptions = mergeOptions;
      this.CompletionQueueTime = completionQueueTime;
      this.AutoCompleteAuthority = autoCompleteAuthority;
      this.UpdatedTime = updatedTime;
    }

    public Guid RepositoryId { get; }

    public Guid Creator { get; }

    public DateTime CreationDate { get; }

    public DateTime ClosedDate { get; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string TargetBranchName { get; }

    public PullRequestAsyncStatus MergeStatus { get; }

    public Guid MergeId { get; }

    public Sha1Id? LastMergeSourceCommit { get; }

    public Sha1Id? LastMergeTargetCommit { get; }

    public Sha1Id? LastMergeCommit { get; }

    public IEnumerable<TfsGitPullRequest.ReviewerWithVote> Reviewers { get; set; }

    public int PullRequestId { get; }

    public PullRequestStatus Status { get; }

    public Guid CompleteWhenMergedAuthority { get; set; }

    public int CodeReviewId { get; }

    public GitPullRequestCompletionOptions CompletionOptions { get; }

    public GitPullRequestMergeOptions MergeOptions { get; }

    public PullRequestMergeFailureType MergeFailureType { get; }

    public string MergeFailureMessage { get; }

    public bool IsDraft { get; }

    public bool SupportsIterations { get; }

    public DateTime CompletionQueueTime { get; }

    public Guid AutoCompleteAuthority { get; set; }

    public string ProjectUri { get; }

    public string RepositoryName { get; }

    public bool RepositoryCreatedByForking { get; }

    public TfsGitForkRef ForkSource { get; }

    public DateTime UpdatedTime { get; }

    internal string MergeRef
    {
      get
      {
        if (string.IsNullOrEmpty(this.m_mergeRef))
          this.m_mergeRef = TfsGitPullRequest.GetMergeRefName(this.PullRequestId);
        return this.m_mergeRef;
      }
    }

    internal bool BelongsToNoJob => this.Status == PullRequestStatus.Active && this.CompleteWhenMergedAuthority == Guid.Empty && this.MergeStatus != PullRequestAsyncStatus.Queued;

    internal bool BelongsToMergeJob => this.Status == PullRequestStatus.Active && this.CompleteWhenMergedAuthority == Guid.Empty && this.MergeStatus == PullRequestAsyncStatus.Queued;

    internal bool BelongsToCompletionJob => this.Status == PullRequestStatus.Active && this.CompleteWhenMergedAuthority != Guid.Empty;

    public IReadOnlyList<TfsGitCommitChange> GetChanges(
      IVssRequestContext requestContext,
      ITfsGitRepository repository)
    {
      if (this.m_changes == null)
        this.m_changes = TfsGitPullRequest.GetChanges(requestContext, repository, this, out this.m_lastCommonCommit);
      return this.m_changes;
    }

    public IReadOnlyList<TfsGitCommitChange> GetChanges(
      IVssRequestContext requestContext,
      out TfsGitCommit commonCommit)
    {
      if (this.m_changes == null)
      {
        using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, this.RepositoryId))
          this.m_changes = TfsGitPullRequest.GetChanges(requestContext, repositoryById, this, out commonCommit);
        this.m_lastCommonCommit = commonCommit;
      }
      else
        commonCommit = this.m_lastCommonCommit;
      return this.m_changes;
    }

    public IReadOnlyList<TfsGitCommitChange> GetChanges(IVssRequestContext requestContext) => this.GetChanges(requestContext, out TfsGitCommit _);

    public TfsGitCommit GetCommonCommit(IVssRequestContext requestContext)
    {
      TfsGitCommit commonCommit = (TfsGitCommit) null;
      if (this.m_lastCommonCommit != null)
      {
        commonCommit = this.m_lastCommonCommit;
      }
      else
      {
        using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, this.RepositoryId))
        {
          commonCommit = TfsGitPullRequest.TryGetCommonCommit(requestContext, repositoryById, this, out TfsGitCommit _, out TfsGitCommit _);
          this.m_lastCommonCommit = commonCommit;
        }
      }
      return commonCommit;
    }

    public IReadOnlyList<TfsGitCommitChange> GetChangesetForPolicyApplicability(
      IVssRequestContext requestContext)
    {
      if (this.m_changesForPolicy == null)
      {
        TfsGitCommit commitA = (TfsGitCommit) null;
        TfsGitCommit commitB = (TfsGitCommit) null;
        using (requestContext.TimeRegion(nameof (TfsGitPullRequest), nameof (GetChangesetForPolicyApplicability)))
          this.m_changesForPolicy = TracepointUtils.TraceBlock<IReadOnlyList<TfsGitCommitChange>>(requestContext, 1013786, GitServerUtils.TraceArea, nameof (TfsGitPullRequest), (Func<IReadOnlyList<TfsGitCommitChange>>) (() =>
          {
            using (ITfsGitRepository repositoryOrDefault = GitRequestContextCacheUtil.GetRepositoryOrDefault(requestContext, this.RepositoryId, true))
            {
              if (this.MergeStatus == PullRequestAsyncStatus.Succeeded)
              {
                Sha1Id? nullable = this.LastMergeTargetCommit;
                if (nullable.HasValue)
                {
                  nullable = this.LastMergeCommit;
                  if (nullable.HasValue)
                  {
                    ITfsGitRepository repo1 = repositoryOrDefault;
                    nullable = this.LastMergeTargetCommit;
                    Sha1Id objectId1 = nullable.Value;
                    commitA = repo1.TryLookupObject<TfsGitCommit>(objectId1);
                    ITfsGitRepository repo2 = repositoryOrDefault;
                    nullable = this.LastMergeCommit;
                    Sha1Id objectId2 = nullable.Value;
                    commitB = repo2.TryLookupObject<TfsGitCommit>(objectId2);
                    goto label_15;
                  }
                }
              }
              TfsGitRef tfsGitRef1 = repositoryOrDefault.Refs.MatchingName(this.SourceBranchName);
              TfsGitRef tfsGitRef2 = repositoryOrDefault.Refs.MatchingName(this.TargetBranchName);
              Sha1Id objectId;
              int num1;
              if (tfsGitRef1 == null)
              {
                num1 = 1;
              }
              else
              {
                objectId = tfsGitRef1.ObjectId;
                num1 = objectId.IsEmpty ? 1 : 0;
              }
              if (num1 == 0)
              {
                int num2;
                if (tfsGitRef2 == null)
                {
                  num2 = 1;
                }
                else
                {
                  objectId = tfsGitRef2.ObjectId;
                  num2 = objectId.IsEmpty ? 1 : 0;
                }
                if (num2 == 0)
                {
                  commitA = requestContext.GetService<ITeamFoundationGitCommitService>().GetMergeBase(requestContext, repositoryOrDefault, tfsGitRef1.ObjectId, tfsGitRef2.ObjectId);
                  commitB = repositoryOrDefault.LookupObject<TfsGitCommit>(tfsGitRef1.ObjectId);
                  goto label_15;
                }
              }
              return (IReadOnlyList<TfsGitCommitChange>) Array.Empty<TfsGitCommitChange>();
label_15:
              if (commitA == null || commitB == null)
                return (IReadOnlyList<TfsGitCommitChange>) Array.Empty<TfsGitCommitChange>();
              using (requestContext.TimeRegion(nameof (TfsGitPullRequest), "DiffTrees"))
                return (IReadOnlyList<TfsGitCommitChange>) TfsGitDiffHelper.DiffTrees(repositoryOrDefault, commitA.GetTree(), commitB.GetTree(), false).Select<TfsGitDiffEntry, TfsGitCommitChange>((Func<TfsGitDiffEntry, TfsGitCommitChange>) (d => new TfsGitCommitChange(d))).Where<TfsGitCommitChange>((Func<TfsGitCommitChange, bool>) (c => c.ObjectType == GitObjectType.Blob || c.ObjectType == GitObjectType.Commit)).ToList<TfsGitCommitChange>().AsReadOnly();
            }
          }), (Func<IReadOnlyList<TfsGitCommitChange>, object>) (result => (object) new
          {
            count = (result != null ? new int?(result.Count<TfsGitCommitChange>()) : new int?()),
            PullRequestId = this.PullRequestId,
            commitA = commitA?.ObjectId,
            commitB = commitB?.ObjectId
          }), caller: nameof (GetChangesetForPolicyApplicability));
      }
      return this.m_changesForPolicy;
    }

    public IReadOnlyList<Sha1Id> GetCommits(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int? top = null,
      int? skip = null,
      string sourceCommitId = null,
      string targetCommitId = null)
    {
      if (this.m_commitIds != null)
      {
        int? nullable1 = top;
        int? nullable2 = this.m_commitsLimit;
        if (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() & nullable1.HasValue == nullable2.HasValue)
        {
          nullable2 = skip;
          int? commitsSkip = this.m_commitsSkip;
          if (nullable2.GetValueOrDefault() == commitsSkip.GetValueOrDefault() & nullable2.HasValue == commitsSkip.HasValue)
            goto label_4;
        }
      }
      this.m_commitsLimit = top;
      this.m_commitsSkip = skip;
      this.m_commitIds = TfsGitPullRequest.GetCommits(requestContext, repository, this, this.m_commitsLimit, this.m_commitsSkip, sourceCommitId, targetCommitId);
label_4:
      return this.m_commitIds;
    }

    public IReadOnlyList<Sha1Id> GetCommitsUsingContinuation(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int top,
      out Sha1Id rootCommitId,
      out Sha1Id nextCommitId,
      Sha1Id? previousRootCommitId = null,
      Sha1Id? firstCommitId = null)
    {
      return TfsGitPullRequest.GetCommitsUsingContinuation(requestContext, repository, this, top, out rootCommitId, out nextCommitId, previousRootCommitId, firstCommitId);
    }

    public IList<ReachableSetAndBoundary<Sha1Id>> GetCommitsBatch(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      IEnumerable<GitPullRequestIteration> iterations)
    {
      return TfsGitPullRequest.GetCommitsBatch(requestContext, repository, this, iterations);
    }

    private static IReadOnlyList<Sha1Id> GetCommits(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int? top,
      int? skip,
      string sourceCommitId = null,
      string targetCommitId = null)
    {
      return TfsGitPullRequest.GetCommits(requestContext, repository, pullRequest, out bool _, top, skip, sourceCommitId, targetCommitId);
    }

    private static IReadOnlyList<Sha1Id> GetCommits(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      out bool moreAvailable,
      int? top = null,
      int? skip = null,
      string sourceCommitId = null,
      string targetCommitId = null)
    {
      TfsGitCommit sourceCommit = TfsGitPullRequest.TryGetSourceCommit(requestContext, repository, pullRequest, sourceCommitId);
      TfsGitCommit targetCommit = TfsGitPullRequest.TryGetTargetCommit(requestContext, repository, pullRequest, targetCommitId);
      if (sourceCommit == null || targetCommit == null || sourceCommit.ObjectId == targetCommit.ObjectId)
      {
        moreAvailable = false;
        return (IReadOnlyList<Sha1Id>) new List<Sha1Id>().AsReadOnly();
      }
      IEnumerable<Sha1Id> source = repository.GetCommitHistory(requestContext, sourceCommit.ObjectId, new Sha1Id?(targetCommit.ObjectId));
      if (skip.HasValue)
        source = source.Skip<Sha1Id>(skip.Value);
      if (top.HasValue)
      {
        List<Sha1Id> list = source.Take<Sha1Id>(top.Value + 1).ToList<Sha1Id>();
        moreAvailable = list.Count > top.Value;
        if (moreAvailable)
          list.RemoveRange(top.Value, list.Count - top.Value);
        return (IReadOnlyList<Sha1Id>) list.AsReadOnly();
      }
      moreAvailable = false;
      return (IReadOnlyList<Sha1Id>) source.ToList<Sha1Id>().AsReadOnly();
    }

    private static IReadOnlyList<Sha1Id> GetCommitsUsingContinuation(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int top,
      out Sha1Id rootCommitId,
      out Sha1Id nextCommitId,
      Sha1Id? previousRootCommitId = null,
      Sha1Id? firstCommitId = null)
    {
      TfsGitCommit sourceCommit = TfsGitPullRequest.TryGetSourceCommit(requestContext, repository, pullRequest);
      TfsGitCommit targetCommit = TfsGitPullRequest.TryGetTargetCommit(requestContext, repository, pullRequest);
      rootCommitId = Sha1Id.Empty;
      nextCommitId = Sha1Id.Empty;
      if (sourceCommit == null || targetCommit == null || sourceCommit.ObjectId == targetCommit.ObjectId)
        return (IReadOnlyList<Sha1Id>) new List<Sha1Id>().AsReadOnly();
      IEnumerable<Sha1Id> source = repository.GetCommitHistory(requestContext, sourceCommit.ObjectId, new Sha1Id?(targetCommit.ObjectId));
      rootCommitId = source.FirstOrDefault<Sha1Id>();
      if (firstCommitId.HasValue)
      {
        if (previousRootCommitId.Value != source.First<Sha1Id>())
          throw new GitPullRequestCommitsStaleContinuationTokenException();
        source = source.SkipWhile<Sha1Id>((Func<Sha1Id, bool>) (commitInList =>
        {
          Sha1Id sha1Id = commitInList;
          Sha1Id? nullable = firstCommitId;
          return !nullable.HasValue || sha1Id != nullable.GetValueOrDefault();
        }));
        if (!source.Any<Sha1Id>())
          throw new GitPullRequestCommitsInvalidContinuationTokenException();
      }
      List<Sha1Id> list = source.Take<Sha1Id>(top + 1).ToList<Sha1Id>();
      if (list.Count > top)
      {
        nextCommitId = list[top];
        list.RemoveAt(top);
      }
      return (IReadOnlyList<Sha1Id>) list.AsReadOnly();
    }

    private static IList<ReachableSetAndBoundary<Sha1Id>> GetCommitsBatch(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      IEnumerable<GitPullRequestIteration> iterations)
    {
      List<Tuple<Sha1Id, Sha1Id>> commitIdTuples = new List<Tuple<Sha1Id, Sha1Id>>();
      foreach (GitPullRequestIteration iteration in iterations)
      {
        TfsGitCommit sourceCommit = TfsGitPullRequest.TryGetSourceCommit(requestContext, repository, pullRequest, iteration);
        TfsGitCommit targetCommit = TfsGitPullRequest.TryGetTargetCommit(requestContext, repository, pullRequest, iteration);
        if (sourceCommit == null || targetCommit == null)
          commitIdTuples.Add(new Tuple<Sha1Id, Sha1Id>(Sha1Id.Empty, Sha1Id.Empty));
        else
          commitIdTuples.Add(new Tuple<Sha1Id, Sha1Id>(sourceCommit.ObjectId, targetCommit.ObjectId));
      }
      return GitCommitBatchHelper.GetCommitsBatch(requestContext, repository, (IEnumerable<Tuple<Sha1Id, Sha1Id>>) commitIdTuples);
    }

    private static IReadOnlyList<TfsGitCommitChange> GetChanges(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      out TfsGitCommit commonCommit)
    {
      using (requestContext.TraceBlock(1013362, 1013363, GitServerUtils.TraceArea, nameof (TfsGitPullRequest), nameof (GetChanges)))
      {
        using (requestContext.TimeRegion(nameof (TfsGitPullRequest), nameof (GetChanges)))
        {
          TfsGitCommit sourceCommit;
          commonCommit = TfsGitPullRequest.TryGetCommonCommit(requestContext, repository, pullRequest, out sourceCommit, out TfsGitCommit _);
          requestContext.Trace(1013364, TraceLevel.Info, GitServerUtils.TraceArea, nameof (TfsGitPullRequest), "commonCommit={0}", (object) (commonCommit == null ? Sha1Id.Empty : commonCommit.ObjectId));
          if (commonCommit != null)
            return (IReadOnlyList<TfsGitCommitChange>) TfsGitDiffHelper.DiffTrees(repository, commonCommit.GetTree(), sourceCommit.GetTree(), true).Select<TfsGitDiffEntry, TfsGitCommitChangeWithId>((Func<TfsGitDiffEntry, TfsGitCommitChangeWithId>) (x => new TfsGitCommitChangeWithId(sourceCommit.ObjectId, x))).Where<TfsGitCommitChangeWithId>((Func<TfsGitCommitChangeWithId, bool>) (x => x.ObjectType != GitObjectType.Tree || x.ChangeType != TfsGitChangeType.Edit)).Where<TfsGitCommitChangeWithId>((Func<TfsGitCommitChangeWithId, bool>) (x => !x.ChangeType.HasFlag((Enum) TfsGitChangeType.Delete) || !x.ChangeType.HasFlag((Enum) TfsGitChangeType.SourceRename))).ToList<TfsGitCommitChangeWithId>().AsReadOnly();
        }
        return (IReadOnlyList<TfsGitCommitChange>) Array.Empty<TfsGitCommitChange>();
      }
    }

    private static TfsGitCommit TryGetSourceCommit(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      GitPullRequestIteration iteration = null)
    {
      return TfsGitPullRequest.TryGetSourceCommit(requestContext, repository, pullRequest, iteration?.SourceRefCommit?.CommitId);
    }

    private static TfsGitCommit TryGetSourceCommit(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      string sourceCommitId)
    {
      if (sourceCommitId != null)
        return repository.TryLookupObject(new Sha1Id(sourceCommitId)) as TfsGitCommit;
      if (pullRequest.Status == PullRequestStatus.Active)
        return TfsGitPullRequest.TryGetRefCommit(requestContext, repository, pullRequest.SourceBranchName);
      return pullRequest.LastMergeSourceCommit.HasValue ? repository.TryLookupObject(pullRequest.LastMergeSourceCommit.Value) as TfsGitCommit : (TfsGitCommit) null;
    }

    private static TfsGitCommit TryGetTargetCommit(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      GitPullRequestIteration iteration = null)
    {
      return TfsGitPullRequest.TryGetTargetCommit(requestContext, repository, pullRequest, iteration?.TargetRefCommit?.CommitId);
    }

    private static TfsGitCommit TryGetTargetCommit(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      string targetCommitId)
    {
      if (targetCommitId != null)
        return repository.TryLookupObject(new Sha1Id(targetCommitId)) as TfsGitCommit;
      if (pullRequest.Status == PullRequestStatus.Active)
        return TfsGitPullRequest.TryGetRefCommit(requestContext, repository, pullRequest.TargetBranchName);
      return pullRequest.LastMergeTargetCommit.HasValue ? repository.TryLookupObject(pullRequest.LastMergeTargetCommit.Value) as TfsGitCommit : (TfsGitCommit) null;
    }

    private static TfsGitCommit TryGetCommonCommit(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      out TfsGitCommit sourceCommit,
      out TfsGitCommit targetCommit)
    {
      sourceCommit = TfsGitPullRequest.TryGetSourceCommit(requestContext, repository, pullRequest);
      if (sourceCommit == null)
      {
        targetCommit = (TfsGitCommit) null;
        return (TfsGitCommit) null;
      }
      targetCommit = TfsGitPullRequest.TryGetTargetCommit(requestContext, repository, pullRequest);
      if (targetCommit == null)
        return (TfsGitCommit) null;
      return sourceCommit.ObjectId == targetCommit.ObjectId ? sourceCommit : requestContext.GetService<ITeamFoundationGitCommitService>().GetMergeBase(requestContext, repository, sourceCommit.ObjectId, targetCommit.ObjectId) ?? targetCommit;
    }

    private static TfsGitCommit TryGetRefCommit(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      string refName)
    {
      TfsGitRef tfsGitRef = repository.Refs.MatchingName(refName);
      return tfsGitRef != null ? repository.TryLookupObject(tfsGitRef.ObjectId) as TfsGitCommit : (TfsGitCommit) null;
    }

    public IEnumerable<int> GetAssociatedWorkItems(
      IVssRequestContext requestContext,
      ITfsGitRepository repository)
    {
      return TfsGitPullRequest.QueryWorkItemIds(requestContext, (IEnumerable<string>) new List<string>()
      {
        LinkingUtilities.EncodeUri(PullRequestArtifactId.GetArtifactIdForPullRequest(repository.Key.GetProjectUri(), repository.Key.RepoId, this.PullRequestId))
      }, new Guid?(repository.Key.ProjectId));
    }

    public IEnumerable<int> GetBranchAndCommitsAssociatedWorkItems(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      bool includeBranch,
      bool includeCommits)
    {
      List<string> uris = new List<string>();
      if (includeBranch)
        uris.Add(GitRefArtifactId.GetArtifactUriForRef(repository.Key, "GB" + GitUtils.GetFriendlyBranchName(this.SourceBranchName)));
      if (includeCommits)
      {
        IReadOnlyList<Sha1Id> commits = this.GetCommits(requestContext, repository, new int?(100));
        uris.AddRange(TfsGitPullRequest.ToGitCommitUris((IEnumerable<Sha1Id>) commits, repository.Key));
      }
      return TfsGitPullRequest.QueryWorkItemIds(requestContext, (IEnumerable<string>) uris);
    }

    private static IEnumerable<int> QueryWorkItemIds(
      IVssRequestContext requestContext,
      IEnumerable<string> uris,
      Guid? projectId = null)
    {
      IWorkItemArtifactUriQueryRemotableService service = requestContext.GetService<IWorkItemArtifactUriQueryRemotableService>();
      ArtifactUriQuery artifactUriQuery = new ArtifactUriQuery()
      {
        ArtifactUris = uris
      };
      return (IEnumerable<int>) (!projectId.HasValue ? service.QueryWorkItemsForArtifactUris(requestContext, artifactUriQuery) : service.QueryWorkItemsForArtifactUris(requestContext, artifactUriQuery, projectId.Value)).ArtifactUrisQueryResult.Values.SelectMany<IEnumerable<WorkItemReference>, int>((Func<IEnumerable<WorkItemReference>, IEnumerable<int>>) (workItemRefs => workItemRefs.Select<WorkItemReference, int>((Func<WorkItemReference, int>) (workItemRef => workItemRef.Id)))).ToList<int>();
    }

    private static IEnumerable<string> ToGitCommitUris(
      IEnumerable<Sha1Id> commitIds,
      RepoKey repoKey)
    {
      foreach (Sha1Id commitId in commitIds)
        yield return GitCommitArtifactId.GetArtifactUriForCommit(repoKey, commitId);
    }

    public IReadOnlyList<GitPullRequestStatus> GetStatuses(
      IVssRequestContext requestContext,
      bool includeProperties = false)
    {
      if (this.m_statuses == null)
      {
        using (ITfsGitRepository repositoryOrDefault = GitRequestContextCacheUtil.GetRepositoryOrDefault(requestContext, this.RepositoryId, true))
        {
          ITeamFoundationGitPullRequestService service = requestContext.GetService<ITeamFoundationGitPullRequestService>();
          IVssRequestContext requestContext1 = requestContext;
          ITfsGitRepository repository = repositoryOrDefault;
          bool flag = includeProperties;
          int? iterationId = new int?();
          int num = flag ? 1 : 0;
          this.m_statuses = (IReadOnlyList<GitPullRequestStatus>) service.GetStatuses(requestContext1, repository, this, iterationId, num != 0).ToList<GitPullRequestStatus>().AsReadOnly();
        }
      }
      return this.m_statuses;
    }

    public static string GetPullRequestUri(string repositoryUrl, int pullRequestId)
    {
      if (!repositoryUrl.EndsWith("/", StringComparison.Ordinal))
        repositoryUrl += "/";
      return new Uri(string.Format("{0}pullrequest/{1}", (object) repositoryUrl, (object) pullRequestId.ToString())).AbsoluteUri;
    }

    public static Uri GetPullRequestPermaLinkUri(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId)
    {
      Guid instanceId = requestContext.ServiceHost.CollectionServiceHost.InstanceId;
      Guid repoId = repository.Key.RepoId;
      Guid projectId = repository.Key.ProjectId;
      return UriUtility.Combine(new Uri(requestContext.GetService<ILocationService>().GetPublicAccessMapping(requestContext).AccessPoint), string.Format("{0}/_git/{1}/pullrequest/{2}?collectionId={3}&projectId={4}", (object) "_permalink", (object) repoId, (object) pullRequestId, (object) instanceId, (object) projectId), true);
    }

    public Guid SourceRepositoryId => this.IsFromFork ? this.ForkSource.RepositoryId : this.RepositoryId;

    public bool IsFromFork => this.ForkSource != null && this.ForkSource.RepositoryId != this.RepositoryId;

    public string SourceBranchName => this.IsFromFork ? TfsGitPullRequest.GetForkSourceRefName(this.PullRequestId) : this.m_sourceBranchName;

    public GitRepositoryRef GetForkRepositoryRef(
      IVssRequestContext requestContext,
      ITfsGitRepository targetRepository)
    {
      return TfsGitPullRequest.GetForkRepositoryRef(requestContext.Elevate(), targetRepository, new Guid?(this.SourceRepositoryId));
    }

    public static GitRepositoryRef GetForkRepositoryRef(
      IVssRequestContext requestContext,
      ITfsGitRepository targetRepository,
      Guid? sourceRepositoryId)
    {
      GitRepositoryRef forkRepositoryRef = (GitRepositoryRef) null;
      if (sourceRepositoryId.HasValue)
      {
        Guid? nullable1 = sourceRepositoryId;
        Guid empty = Guid.Empty;
        if ((nullable1.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) == 0)
        {
          Guid? nullable2 = sourceRepositoryId;
          Guid repoId = targetRepository.Key.RepoId;
          if ((nullable2.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() == repoId ? 1 : 0) : 1) : 0) == 0)
          {
            try
            {
              IGitForkService service = requestContext.GetService<IGitForkService>();
              if (targetRepository.IsFork)
              {
                GitRepositoryRef parent = service.GetParent(requestContext, targetRepository.Key);
                int num;
                if (parent == null)
                {
                  num = !sourceRepositoryId.HasValue ? 1 : 0;
                }
                else
                {
                  Guid id = parent.Id;
                  nullable2 = sourceRepositoryId;
                  num = nullable2.HasValue ? (id == nullable2.GetValueOrDefault() ? 1 : 0) : 0;
                }
                if (num != 0)
                  forkRepositoryRef = parent;
              }
              if (forkRepositoryRef == null)
                forkRepositoryRef = service.QueryChildren(requestContext, targetRepository.Key, requestContext.ServiceHost.InstanceId).FirstOrDefault<GitRepositoryRef>((Func<GitRepositoryRef, bool>) (r =>
                {
                  Guid id = r.Id;
                  Guid? nullable3 = sourceRepositoryId;
                  return nullable3.HasValue && id == nullable3.GetValueOrDefault();
                }));
            }
            catch (GitRepositoryNotFoundException ex)
            {
              forkRepositoryRef = (GitRepositoryRef) null;
            }
            if (forkRepositoryRef == null)
              forkRepositoryRef = new GitRepositoryRef()
              {
                Id = sourceRepositoryId.GetValueOrDefault()
              };
            return forkRepositoryRef;
          }
        }
      }
      return (GitRepositoryRef) null;
    }

    public ArtifactId BuildLegacyArtifactId(string teamProjectUri) => LegacyCodeReviewArtifactId.GetLegacyArtifactIdForCodeReview(teamProjectUri, this.PullRequestId);

    public string BuildLegacyEncodedArtifactId(string teamProjectUri) => LinkingUtilities.EncodeUri(LegacyCodeReviewArtifactId.GetLegacyArtifactIdForCodeReview(teamProjectUri, this.PullRequestId));

    public ArtifactId BuildArtifactId(string teamProjectUri) => PullRequestArtifactId.GetArtifactIdForPullRequest(teamProjectUri, this.RepositoryId, this.PullRequestId);

    public string BuildEncodedArtifactId(string teamProjectUri) => LinkingUtilities.EncodeUri(PullRequestArtifactId.GetArtifactIdForPullRequest(teamProjectUri, this.RepositoryId, this.PullRequestId));

    public ArtifactId BuildArtifactIdForDiscussions(string teamProjectUri) => this.CodeReviewId > 0 && this.SupportsIterations ? CodeReviewSdkArtifactId.GetArtifactId(teamProjectUri, this.CodeReviewId) : this.BuildLegacyArtifactId(teamProjectUri);

    public string BuildArtifactUriForDiscussions(string teamProjectUri) => LinkingUtilities.EncodeUri(this.BuildArtifactIdForDiscussions(teamProjectUri));

    public ArtifactId BuildArtifactIdForPullRequests(string teamProjectUri) => this.CodeReviewId > 0 ? this.BuildArtifactId(teamProjectUri) : this.BuildLegacyArtifactId(teamProjectUri);

    public string BuildArtifactUriForPullRequests(string teamProjectUri) => LinkingUtilities.EncodeUri(this.BuildArtifactIdForPullRequests(teamProjectUri));

    public static void CheckMergeStatus(TfsGitPullRequest pullRequest)
    {
      if (pullRequest != null && (pullRequest.MergeFailureType == PullRequestMergeFailureType.Unknown || pullRequest.MergeFailureType == PullRequestMergeFailureType.ObjectTooLarge) && pullRequest.MergeStatus != PullRequestAsyncStatus.RejectedByPolicy)
        throw new GitPullRequestUnknownMergeFailure(pullRequest.MergeFailureMessage);
    }

    internal static string GetMergeRefName(int pullRequestId) => string.Format("{0}{1}/{2}", (object) "refs/pull/", (object) pullRequestId, (object) "merge");

    internal static string GetForkSourceRefName(int pullRequestId) => string.Format("{0}{1}/{2}", (object) "refs/pull/", (object) pullRequestId, (object) "source");

    internal static string GetSourceFixupRefName(int pullRequestId) => string.Format("{0}{1}/{2}", (object) "refs/pull/", (object) pullRequestId, (object) "sourceFixup");

    internal static string GetTargetFixupRefName(int pullRequestId) => string.Format("{0}{1}/{2}", (object) "refs/pull/", (object) pullRequestId, (object) "targetFixup");

    internal static string GetMergedBlobRefName(int pullRequestId, int conflictId) => string.Format("{0}{1}/{2}{3}", (object) "refs/pull/", (object) pullRequestId, (object) "mergedBlob/", (object) conflictId);

    public class ReviewerBase
    {
      public Guid Reviewer { get; }

      public bool IsRequired { get; }

      public ReviewerBase(Guid reviewer, bool isRequired = false)
      {
        this.Reviewer = reviewer;
        this.IsRequired = isRequired;
      }

      public ReviewerBase(TfsGitPullRequest.ReviewerBase other)
      {
        this.Reviewer = other.Reviewer;
        this.IsRequired = other.IsRequired;
      }
    }

    public class ReviewerWithVote : TfsGitPullRequest.ReviewerBase
    {
      public ReviewerWithVote(Guid reviewer)
        : this(reviewer, (short) 0)
      {
      }

      public ReviewerWithVote(Guid reviewer, short vote)
        : this(reviewer, vote, (IEnumerable<Guid>) null)
      {
      }

      public ReviewerWithVote(
        Guid reviewer,
        short vote,
        IEnumerable<Guid> votedFor,
        bool isRequired = false)
        : this(reviewer, vote, ReviewerVoteStatus.None, votedFor, isRequired)
      {
      }

      public ReviewerWithVote(
        Guid reviewer,
        short vote,
        ReviewerVoteStatus status,
        IEnumerable<Guid> votedFor = null,
        bool isRequired = false,
        int? pullRequestId = null,
        bool isFlagged = false,
        bool hasDeclined = false)
        : base(reviewer, isRequired)
      {
        this.Vote = vote;
        this.Status = status;
        this.VotedFor = (IReadOnlyList<Guid>) new List<Guid>((IEnumerable<Guid>) ((object) votedFor ?? (object) Array.Empty<Guid>()));
        this.PullRequestId = pullRequestId;
        this.IsFlagged = isFlagged;
        this.HasDeclined = hasDeclined;
      }

      public ReviewerWithVote(TfsGitPullRequest.ReviewerBase other)
        : base(other)
      {
        if (!(other is TfsGitPullRequest.ReviewerWithVote reviewerWithVote))
        {
          this.VotedFor = (IReadOnlyList<Guid>) new List<Guid>();
        }
        else
        {
          this.Vote = reviewerWithVote.Vote;
          this.VotedFor = (IReadOnlyList<Guid>) new List<Guid>((IEnumerable<Guid>) reviewerWithVote.VotedFor);
          this.Status = reviewerWithVote.Status;
          this.IsFlagged = reviewerWithVote.IsFlagged;
          this.HasDeclined = reviewerWithVote.HasDeclined;
        }
      }

      public IReadOnlyList<Guid> VotedFor { get; }

      public short Vote { get; }

      public bool HasDeclined { get; }

      public bool IsFlagged { get; }

      public ReviewerVoteStatus Status { get; }

      public bool Approves => this.Vote > (short) 0;

      public bool Rejects => this.Vote < (short) 0;

      public bool NotVoted => this.Vote == (short) 0;

      public int? PullRequestId { get; }
    }
  }
}
