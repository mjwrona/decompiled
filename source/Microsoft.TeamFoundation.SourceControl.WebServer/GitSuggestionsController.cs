// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitSuggestionsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.Azure.Boards.RecentActivity;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Settings;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class GitSuggestionsController : GitApiController
  {
    [HttpGet]
    [ClientLocationId("9393B4FB-4445-4919-972B-9AD16F442D83")]
    [ClientResponseType(typeof (IList<GitSuggestion>), null, null)]
    public HttpResponseMessage GetSuggestions(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null,
      bool preferCompareBranch = false)
    {
      List<GitSuggestion> collection = new List<GitSuggestion>();
      GitSuggestion gitSuggestion = this.SuggestedPullRequest(repositoryId, projectId, preferCompareBranch);
      if (gitSuggestion != null)
        collection.Add(gitSuggestion);
      return this.GenerateResponse<GitSuggestion>((IEnumerable<GitSuggestion>) collection);
    }

    internal GitSuggestion SuggestedPullRequest(
      string repositoryId,
      string projectId,
      bool preferCompareBranch)
    {
      TfsGitPushMetadata branchPush = (TfsGitPushMetadata) null;
      TfsGitRefLogEntry branchRefUpdate = (TfsGitRefLogEntry) null;
      string targetRepositoryId = repositoryId;
      if (this.TfsRequestContext.IsFeatureEnabled("Git.Push.RecentActivities"))
        this.GetRecentActivity(repositoryId, projectId, out targetRepositoryId, out branchRefUpdate, out branchPush);
      using (ITfsGitRepository tfsGitRepository1 = this.GetTfsGitRepository(targetRepositoryId, projectId))
      {
        if (branchRefUpdate == null)
        {
          ITfsGitRepository repo = tfsGitRepository1;
          DateTime? fromDate = new DateTime?(DateTime.UtcNow.AddDays(-1.0));
          Guid? nullable = new Guid?(this.TfsRequestContext.GetUserId());
          DateTime? toDate = new DateTime?();
          Guid? pusherId = nullable;
          int? skip = new int?(0);
          int? take = new int?(25);
          foreach (TfsGitPushMetadata tfsGitPushMetadata in repo.QueryPushHistory(true, fromDate, toDate, pusherId, skip, take, (string) null))
          {
            branchRefUpdate = tfsGitPushMetadata.RefLog.FirstOrDefault<TfsGitRefLogEntry>((Func<TfsGitRefLogEntry, bool>) (update => update.Name.StartsWith("refs/heads/")));
            if (branchRefUpdate != null)
            {
              branchPush = tfsGitPushMetadata;
              break;
            }
          }
        }
        if (branchPush == null || branchRefUpdate == null)
          return (GitSuggestion) null;
        ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
        if (service.QueryPullRequestsForSuggestions(this.TfsRequestContext, projectId != null ? tfsGitRepository1.Key.GetProjectUri() : (string) null, tfsGitRepository1.Key.RepoId, branchRefUpdate.RepositoryId, (IEnumerable<string>) new List<string>()
        {
          branchRefUpdate.Name
        }).Where<TfsGitPullRequest>((Func<TfsGitPullRequest, bool>) (pr => pr.Status == PullRequestStatus.Active || pr.ClosedDate > branchPush.PushTime)).Count<TfsGitPullRequest>() > 0)
          return (GitSuggestion) null;
        ITeamFoundationGitPullRequestService pullRequestService = service;
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        List<Sha1Id> commits = new List<Sha1Id>();
        commits.Add(branchRefUpdate.NewObjectId);
        RepoKey key = tfsGitRepository1.Key;
        ILookup<Sha1Id, TfsGitPullRequest> lookup = pullRequestService.QueryPullRequestsByMergeCommits(tfsRequestContext, (IEnumerable<Sha1Id>) commits, key);
        if ((lookup != null ? (lookup.Count > 0 ? 1 : 0) : 0) != 0)
          return (GitSuggestion) null;
        string refName = this.TfsRequestContext.GetService<ISettingsService>().GetValue<string>(this.TfsRequestContext, SettingsUserScope.User, "Repository", tfsGitRepository1.Key.RepoId.ToString(), "Branches.Compare", (string) null);
        TfsGitRef targetBranchRef = !this.TfsRequestContext.IsFeatureEnabled("WebAccess.VersionControl.PullRequests.UpdateTargetBranches2") ? (refName == null ? tfsGitRepository1.Refs.GetDefault() : tfsGitRepository1.Refs.MatchingName(refName)) : (!preferCompareBranch || refName == null ? tfsGitRepository1.Refs.GetDefault() : tfsGitRepository1.Refs.MatchingName(refName));
        if (targetBranchRef == null || branchPush.RefLog != null && branchPush.RefLog.Any<TfsGitRefLogEntry>((Func<TfsGitRefLogEntry, bool>) (rle => rle.Name == targetBranchRef.Name)) && tfsGitRepository1.Key.RepoId == branchRefUpdate.RepositoryId)
          return (GitSuggestion) null;
        using (ITfsGitRepository tfsGitRepository2 = this.GetTfsGitRepository(branchRefUpdate.RepositoryId.ToString(), projectId))
        {
          TfsGitRef tfsGitRef = tfsGitRepository2.Refs.MatchingName(branchRefUpdate.Name);
          if (tfsGitRef == null || string.Equals(targetBranchRef.Name, tfsGitRef.Name, StringComparison.Ordinal) && tfsGitRepository1.Key.RepoId == branchRefUpdate.RepositoryId)
            return (GitSuggestion) null;
          TfsGitCommit commit = tfsGitRepository1.LookupObject<TfsGitCommit>(targetBranchRef.ObjectId);
          TfsGitCommit ancestor = tfsGitRepository2.LookupObject<TfsGitCommit>(tfsGitRef.ObjectId);
          if (tfsGitRepository1.IsAncestor(this.TfsRequestContext, commit, ancestor))
            return (GitSuggestion) null;
          return new GitSuggestion()
          {
            Type = "pullRequest",
            Properties = (IDictionary<string, object>) new Dictionary<string, object>()
            {
              {
                "sourceRepository",
                (object) tfsGitRepository2.ToWebApiItem(this.TfsRequestContext)
              },
              {
                "sourceBranch",
                (object) tfsGitRef.Name
              },
              {
                "targetRepositoryId",
                (object) tfsGitRepository1.Key.RepoId
              },
              {
                "targetBranch",
                (object) targetBranchRef.Name
              },
              {
                "pushDate",
                (object) branchPush.PushTime
              }
            }
          };
        }
      }
    }

    private void GetRecentActivity(
      string repositoryId,
      string projectId,
      out string targetRepositoryId,
      out TfsGitRefLogEntry branchRefUpdate,
      out TfsGitPushMetadata branchPush)
    {
      Microsoft.Azure.Boards.RecentActivity.RecentActivity recentActivity = (Microsoft.Azure.Boards.RecentActivity.RecentActivity) null;
      branchRefUpdate = (TfsGitRefLogEntry) null;
      branchPush = (TfsGitPushMetadata) null;
      targetRepositoryId = repositoryId;
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        IGitForkService service = this.TfsRequestContext.GetService<IGitForkService>();
        IEnumerable<GitRepositoryRef> source = service.QueryChildren(this.TfsRequestContext, tfsGitRepository.Key, this.TfsRequestContext.ServiceHost.InstanceId);
        List<KeyValuePair<string, Microsoft.Azure.Boards.RecentActivity.RecentActivity>> list = this.TfsRequestContext.GetService<ITeamFoundationRecentActivityService>().GetUserActivities(this.TfsRequestContext, this.TfsRequestContext.GetUserIdentity().Id, GitPushArtifactKinds.GitPush).OrderByDescending<KeyValuePair<string, Microsoft.Azure.Boards.RecentActivity.RecentActivity>, DateTime>((Func<KeyValuePair<string, Microsoft.Azure.Boards.RecentActivity.RecentActivity>, DateTime>) (a => a.Value.ActivityDate)).ToList<KeyValuePair<string, Microsoft.Azure.Boards.RecentActivity.RecentActivity>>();
        if (list.Count > 0)
        {
          List<Guid> repoIds = source.Select<GitRepositoryRef, Guid>((Func<GitRepositoryRef, Guid>) (fc => fc.Id)).ToList<Guid>().Union<Guid>((IEnumerable<Guid>) new Guid[1]
          {
            tfsGitRepository.Key.RepoId
          }).ToList<Guid>();
          recentActivity = list.FirstOrDefault<KeyValuePair<string, Microsoft.Azure.Boards.RecentActivity.RecentActivity>>((Func<KeyValuePair<string, Microsoft.Azure.Boards.RecentActivity.RecentActivity>, bool>) (a => repoIds.Any<Guid>((Func<Guid, bool>) (id => a.Value.ActivityDetails.Contains(id.ToString()))))).Value;
        }
        if (recentActivity == null)
          return;
        branchRefUpdate = TfsGitRefLogEntryUtilities.DeserializeFromJson(recentActivity.ActivityDetails);
        if (branchRefUpdate == null)
          return;
        branchPush = new TfsGitPushMetadata(branchRefUpdate.RepositoryId, branchRefUpdate.PushId, recentActivity.IdentityId, recentActivity.ActivityDate);
        if (!tfsGitRepository.IsFork || !(branchRefUpdate.RepositoryId == tfsGitRepository.Key.RepoId))
          return;
        GitRepositoryRef parent = service.GetParent(this.TfsRequestContext, tfsGitRepository.Key);
        targetRepositoryId = parent?.Id.ToString() ?? repositoryId;
      }
    }
  }
}
