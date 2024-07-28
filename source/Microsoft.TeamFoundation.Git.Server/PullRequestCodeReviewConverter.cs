// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PullRequestCodeReviewConverter
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class PullRequestCodeReviewConverter
  {
    public static GitPullRequestCommentThread ToGitPullRequestItem(
      this Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentThread commentThread,
      IVssRequestContext rc,
      ISecuredObject securedObject,
      IDictionary<Guid, IdentityRef> cachedIdentities = null)
    {
      if (commentThread == null)
        return (GitPullRequestCommentThread) null;
      cachedIdentities = cachedIdentities ?? (IDictionary<Guid, IdentityRef>) new Dictionary<Guid, IdentityRef>();
      GitPullRequestCommentThread gitPullRequestItem = new GitPullRequestCommentThread();
      gitPullRequestItem.Id = commentThread.DiscussionId;
      gitPullRequestItem.ArtifactUri = commentThread.ArtifactUri;
      gitPullRequestItem.LastUpdatedDate = commentThread.LastUpdatedDate;
      gitPullRequestItem.PublishedDate = commentThread.PublishedDate;
      Dictionary<string, IdentityRef> identities;
      gitPullRequestItem.Properties = ThreadPropertiesConverter.GetPRThreadProperties(rc, (DiscussionThread) commentThread, cachedIdentities, securedObject, out identities);
      gitPullRequestItem.Identities = identities;
      gitPullRequestItem.Status = commentThread.Status.ToCommentStatus();
      gitPullRequestItem.Comments = ((IEnumerable<DiscussionComment>) commentThread.Comments).ToCommentList();
      gitPullRequestItem.ThreadContext = commentThread.ThreadContext.ToThreadContext();
      gitPullRequestItem.PullRequestThreadContext = commentThread.ThreadContext.ToPRThreadContext();
      gitPullRequestItem.IsDeleted = commentThread.IsDeleted;
      gitPullRequestItem.MarkForUpdate = commentThread.IsDirty;
      gitPullRequestItem.SetSecuredObject(securedObject);
      return gitPullRequestItem;
    }

    public static GitPullRequestCommentThreadContext ToPRThreadContext(
      this Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentThreadContext crThreadContext)
    {
      if (crThreadContext == null)
        return (GitPullRequestCommentThreadContext) null;
      return new GitPullRequestCommentThreadContext()
      {
        ChangeTrackingId = crThreadContext.ChangeTrackingId,
        IterationContext = crThreadContext.IterationContext.ToPRIterationContext(),
        TrackingCriteria = crThreadContext.TrackingCriteria.ToPRCommentTrackingCriteria()
      };
    }

    public static CommentIterationContext ToPRIterationContext(
      this IterationContext iterationContext)
    {
      if (iterationContext == null)
        return (CommentIterationContext) null;
      return new CommentIterationContext()
      {
        FirstComparingIteration = iterationContext.FirstComparingIteration,
        SecondComparingIteration = iterationContext.SecondComparingIteration
      };
    }

    public static Microsoft.TeamFoundation.SourceControl.WebApi.CommentTrackingCriteria ToPRCommentTrackingCriteria(
      this Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentTrackingCriteria commentTrackingCriteria)
    {
      if (commentTrackingCriteria == null)
        return (Microsoft.TeamFoundation.SourceControl.WebApi.CommentTrackingCriteria) null;
      return new Microsoft.TeamFoundation.SourceControl.WebApi.CommentTrackingCriteria()
      {
        FirstComparingIteration = commentTrackingCriteria.FirstComparingIteration,
        SecondComparingIteration = commentTrackingCriteria.SecondComparingIteration,
        OrigFilePath = commentTrackingCriteria.OrigFilePath,
        OrigLeftFileStart = commentTrackingCriteria.OrigLeftFileStart.ToCommentPosition(),
        OrigLeftFileEnd = commentTrackingCriteria.OrigLeftFileEnd.ToCommentPosition(),
        OrigRightFileStart = commentTrackingCriteria.OrigRightFileStart.ToCommentPosition(),
        OrigRightFileEnd = commentTrackingCriteria.OrigRightFileEnd.ToCommentPosition()
      };
    }

    public static GitPullRequestIteration ToGitPullRequestItem(
      this Iteration iteration,
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      ISecuredObject securedObject)
    {
      if (iteration == null)
        return (GitPullRequestIteration) null;
      List<GitPullRequestChange> pullRequestChangeList = (List<GitPullRequestChange>) null;
      if (iteration.ChangeList != null)
        pullRequestChangeList = iteration.ChangeList.Select<ChangeEntry, GitPullRequestChange>((Func<ChangeEntry, GitPullRequestChange>) (cl => cl.ToGitPullRequestItem(false, securedObject))).ToList<GitPullRequestChange>();
      string str1 = (string) null;
      iteration.Properties?.TryGetValue<string>(PullRequestCodeReviewSdkExtensions.s_NewTargetRefName, out str1);
      string str2 = (string) null;
      iteration.Properties?.TryGetValue<string>(PullRequestCodeReviewSdkExtensions.s_OldTargetRefName, out str2);
      string empty = string.Empty;
      iteration.Properties?.TryGetValue<string>(PullRequestCodeReviewSdkExtensions.s_ConflictResolutionHash, out empty);
      Sha1Id id;
      Sha1Id.TryParse(empty, out id);
      IterationReason iterationReason = !(id != Sha1Id.Empty) ? (!string.IsNullOrEmpty(str1) ? IterationReason.Retarget : IterationReason.Push) : IterationReason.ResolveConflicts;
      GitPullRequestIteration gitPullRequestItem = new GitPullRequestIteration();
      gitPullRequestItem.Author = iteration.Author;
      gitPullRequestItem.ChangeList = (IList<GitPullRequestChange>) pullRequestChangeList;
      gitPullRequestItem.CreatedDate = iteration.CreatedDate;
      gitPullRequestItem.Description = iteration.Description;
      gitPullRequestItem.Id = iteration.Id;
      gitPullRequestItem.UpdatedDate = iteration.UpdatedDate;
      gitPullRequestItem.SourceRefCommit = PullRequestCodeReviewConverter.ToGitCommitRef(requestContext, iteration.Properties, PullRequestCodeReviewSdkExtensions.s_SourceRefCommit, iteration.Id, pullRequest);
      gitPullRequestItem.TargetRefCommit = PullRequestCodeReviewConverter.ToGitCommitRef(requestContext, iteration.Properties, PullRequestCodeReviewSdkExtensions.s_TargetRefCommit, iteration.Id, pullRequest);
      gitPullRequestItem.CommonRefCommit = PullRequestCodeReviewConverter.ToGitCommitRef(requestContext, iteration.Properties, PullRequestCodeReviewSdkExtensions.s_CommonRefCommit, iteration.Id, pullRequest, false);
      gitPullRequestItem.Push = PullRequestCodeReviewConverter.ToGitPushRef(iteration.Properties, PullRequestCodeReviewSdkExtensions.s_SourcePushId);
      gitPullRequestItem.Reason = iterationReason;
      gitPullRequestItem.NewTargetRefName = str1;
      gitPullRequestItem.OldTargetRefName = str2;
      gitPullRequestItem.SetSecuredObject(securedObject);
      return gitPullRequestItem;
    }

    public static List<GitPullRequestIteration> FilterOutPartiallySavedIterations(
      this List<GitPullRequestIteration> iterations,
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest)
    {
      if (!pullRequest.SupportsIterations)
        return iterations;
      List<GitPullRequestIteration> list = iterations.Where<GitPullRequestIteration>((Func<GitPullRequestIteration, bool>) (iteration => iteration.SourceRefCommit != null && iteration.TargetRefCommit != null)).ToList<GitPullRequestIteration>();
      if (list.Count != iterations.Count)
        requestContext.TraceAlways(1013878, TraceLevel.Warning, GitServerUtils.TraceArea, "TfsGitPullRequest", string.Format("Partially saved pull request iterations were ignored, originalIterationsCount: '{0}', ignoredIterationsCount={1}", (object) iterations.Count, (object) (iterations.Count - list.Count)));
      return list;
    }

    public static IEnumerable<GitPullRequestChange> ToGetPullRequestItem(
      this IEnumerable<ChangeEntry> changeList,
      bool computeChangeType,
      ISecuredObject securedObject)
    {
      IEnumerable<GitPullRequestChange> getPullRequestItem = (IEnumerable<GitPullRequestChange>) null;
      if (changeList != null)
        getPullRequestItem = changeList.Select<ChangeEntry, GitPullRequestChange>((Func<ChangeEntry, GitPullRequestChange>) (cl => cl.ToGitPullRequestItem(computeChangeType, securedObject)));
      return getPullRequestItem;
    }

    public static GitPullRequestStatus ToGitPullRequestStatusItem(
      this Status status,
      ISecuredObject securedObject)
    {
      if (status == null)
        return (GitPullRequestStatus) null;
      GitPullRequestStatus requestStatusItem = new GitPullRequestStatus();
      requestStatusItem.Id = status.Id;
      requestStatusItem.State = status.State.ToPRState();
      requestStatusItem.Context = new GitStatusContext()
      {
        Name = status.Context?.Name,
        Genre = status.Context?.Genre
      };
      requestStatusItem.Description = status.Description;
      requestStatusItem.TargetUrl = status.TargetUrl;
      DateTime? nullable = status.CreatedDate;
      DateTime minValue1;
      if (!nullable.HasValue)
      {
        minValue1 = DateTime.MinValue;
      }
      else
      {
        nullable = status.CreatedDate;
        minValue1 = nullable.Value;
      }
      requestStatusItem.CreationDate = minValue1;
      nullable = status.UpdatedDate;
      DateTime minValue2;
      if (!nullable.HasValue)
      {
        minValue2 = DateTime.MinValue;
      }
      else
      {
        nullable = status.UpdatedDate;
        minValue2 = nullable.Value;
      }
      requestStatusItem.UpdatedDate = minValue2;
      requestStatusItem.IterationId = status.IterationId;
      requestStatusItem.CreatedBy = status.Author;
      requestStatusItem.Properties = status.Properties;
      requestStatusItem.SetSecuredObject(securedObject);
      return requestStatusItem;
    }

    public static GitStatusContext ToGitStatusContext(
      this StatusContext statusContext,
      ISecuredObject securedObject)
    {
      if (statusContext == null)
        return (GitStatusContext) null;
      GitStatusContext gitStatusContext = new GitStatusContext();
      gitStatusContext.Name = statusContext.Name;
      gitStatusContext.Genre = statusContext.Genre;
      gitStatusContext.SetSecuredObject(securedObject);
      return gitStatusContext;
    }

    private static GitStatusState ToPRState(this MetaState state)
    {
      switch (state)
      {
        case MetaState.NotSet:
          return GitStatusState.NotSet;
        case MetaState.Pending:
          return GitStatusState.Pending;
        case MetaState.Succeeded:
          return GitStatusState.Succeeded;
        case MetaState.Failed:
          return GitStatusState.Failed;
        case MetaState.Error:
          return GitStatusState.Error;
        case MetaState.NotApplicable:
          return GitStatusState.NotApplicable;
        default:
          return GitStatusState.NotSet;
      }
    }

    private static GitCommitRef ToGitCommitRef(
      IVssRequestContext requestContext,
      PropertiesCollection iterationProperties,
      string propertyName,
      int? iterationId,
      TfsGitPullRequest pullRequest,
      bool traceIfEmpty = true)
    {
      string str = (string) null;
      bool? nullable = iterationProperties?.TryGetValue<string>(propertyName, out str);
      bool flag = true;
      if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
        return new GitCommitRef() { CommitId = str };
      if (pullRequest.SupportsIterations & traceIfEmpty)
        requestContext.TraceAlways(1013875, TraceLevel.Warning, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request iteration properties are empty or missing '{0}', isEmpty={1} pullRequestId={2}, repoId={3}, iterationId={4}, stackTrace={5}", (object) propertyName, (object) (iterationProperties == null), (object) pullRequest.PullRequestId, (object) pullRequest.RepositoryId, (object) iterationId, (object) new StackTrace().ToString());
      return (GitCommitRef) null;
    }

    private static GitPushRef ToGitPushRef(
      PropertiesCollection iterationProperties,
      string propertyName)
    {
      if (iterationProperties == null)
        return (GitPushRef) null;
      int num;
      if (!iterationProperties.TryGetValue<int>(propertyName, out num))
        return (GitPushRef) null;
      return new GitPushRef() { PushId = num };
    }

    private static string GetOriginalPath(
      ChangeEntryFileInfo baseInfo,
      ChangeEntryFileInfo modifiedInfo)
    {
      string path1 = baseInfo?.Path;
      string path2 = modifiedInfo?.Path;
      return path1 == null || path2 == null || !path1.Equals(path2) ? path1 : (string) null;
    }

    private static VersionControlChangeType DetermineChangeType(ChangeEntry change)
    {
      ChangeEntryFileInfo changeEntryFileInfo = change.Base;
      ChangeEntryFileInfo modified = change.Modified;
      if (changeEntryFileInfo == null && modified != null)
        return VersionControlChangeType.Add;
      if (changeEntryFileInfo != null && modified == null)
        return VersionControlChangeType.Delete;
      string path1 = changeEntryFileInfo?.Path;
      string path2 = modified?.Path;
      if (path1 == null || path2 == null)
        return change.Type.ToVCChangeType();
      VersionControlChangeType changeType = VersionControlChangeType.None;
      if (path1.CompareTo(path2) != 0)
        changeType |= VersionControlChangeType.Rename;
      if (!string.Equals(changeEntryFileInfo.SHA1Hash, modified.SHA1Hash, StringComparison.OrdinalIgnoreCase))
        changeType |= VersionControlChangeType.Edit;
      return changeType;
    }

    public static GitPullRequestChange ToGitPullRequestItem(
      this ChangeEntry changeEntry,
      bool computeChangeType,
      ISecuredObject securedObject)
    {
      if (changeEntry == null)
        return (GitPullRequestChange) null;
      GitPullRequestChange gitPullRequestItem = new GitPullRequestChange();
      GitItem gitItem = new GitItem();
      gitItem.Path = changeEntry.Modified != null ? changeEntry.Modified.Path : (string) null;
      gitItem.ObjectId = changeEntry.Modified != null ? changeEntry.Modified.SHA1Hash : (string) null;
      gitItem.OriginalObjectId = changeEntry.Base != null ? changeEntry.Base.SHA1Hash : (string) null;
      gitPullRequestItem.Item = gitItem;
      gitPullRequestItem.ChangeType = computeChangeType ? PullRequestCodeReviewConverter.DetermineChangeType(changeEntry) : changeEntry.Type.ToVCChangeType();
      gitPullRequestItem.OriginalPath = PullRequestCodeReviewConverter.GetOriginalPath(changeEntry.Base, changeEntry.Modified);
      int? changeId = changeEntry.ChangeId;
      int num;
      if (!changeId.HasValue)
      {
        num = 0;
      }
      else
      {
        changeId = changeEntry.ChangeId;
        num = changeId.Value;
      }
      gitPullRequestItem.ChangeId = num;
      gitPullRequestItem.ChangeTrackingId = changeEntry.ChangeTrackingId;
      gitPullRequestItem.SetSecuredObject(securedObject);
      return gitPullRequestItem;
    }

    public static VersionControlChangeType ToVCChangeType(this ChangeType changeType) => (VersionControlChangeType) (0 | (changeType.HasFlag((Enum) ChangeType.Add) ? 1 : 0) | (changeType.HasFlag((Enum) ChangeType.Edit) ? 2 : 0) | (changeType.HasFlag((Enum) ChangeType.Rename) ? 8 : 0) | (changeType.HasFlag((Enum) ChangeType.Move) ? 8 : 0) | (changeType.HasFlag((Enum) ChangeType.Delete) ? 16 : 0));

    public static Microsoft.TeamFoundation.SourceControl.WebApi.Attachment ToGitAttachment(
      Microsoft.VisualStudio.Services.CodeReview.WebApi.Attachment attachment,
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      RepoKey repoKey)
    {
      if (attachment == null)
        return (Microsoft.TeamFoundation.SourceControl.WebApi.Attachment) null;
      Uri resourceUri = requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "git", GitWebApiConstants.PullRequestAttachmentsLocationId, (object) new
      {
        project = repoKey.ProjectId,
        repositoryId = pullRequest.RepositoryId,
        pullRequestId = pullRequest.PullRequestId,
        fileName = Uri.EscapeDataString(attachment.DisplayName)
      });
      string securable = GitUtils.CalculateSecurable(repoKey.ProjectId, repoKey.RepoId, (string) null);
      string absoluteUri = resourceUri.AbsoluteUri;
      Microsoft.TeamFoundation.SourceControl.WebApi.Attachment gitAttachment = new Microsoft.TeamFoundation.SourceControl.WebApi.Attachment(securable)
      {
        Id = attachment.Id,
        Author = attachment.Author,
        ContentHash = attachment.ContentHash,
        CreatedDate = attachment.CreatedDate,
        Description = attachment.Description,
        Links = new ReferenceLinks(),
        Properties = attachment.Properties,
        DisplayName = attachment.DisplayName,
        Url = absoluteUri
      };
      gitAttachment.Links.AddLink("self", absoluteUri, (ISecuredObject) gitAttachment);
      return gitAttachment;
    }
  }
}
