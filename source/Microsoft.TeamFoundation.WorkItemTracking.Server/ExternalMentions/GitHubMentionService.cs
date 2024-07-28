// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalMentions.GitHubMentionService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalMentions
{
  public class GitHubMentionService : IGitHubMentionService, IVssFrameworkService
  {
    private const int HistoryLookbackBufferInMinutes = -5;
    private const string GitHubClosedAction = "closed";

    public void ServiceStart(IVssRequestContext requestContext) => requestContext.CheckProjectCollectionRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public bool MentionPullRequest(
      IVssRequestContext requestContext,
      Guid repoInternalId,
      IEnumerable<Guid> projectIds,
      ExternalGitPullRequest pullRequest,
      bool canResolveMentions,
      bool checkHistoryForMentions,
      IEnumerable<ExternalConnection> externalConnections)
    {
      bool flag1 = false;
      bool flag2 = !string.IsNullOrEmpty(pullRequest.MergedAt);
      bool allowResolveVerb = this.IsDefaultBranch(pullRequest.Repo.DefaultBranch, pullRequest.TargetRef) & flag2 & canResolveMentions;
      DateTime? resolvedDate = flag2 ? new DateTime?(DateTime.Parse(pullRequest.MergedAt)) : new DateTime?();
      IDictionary<int, WorkItemMention> mentionMap = ExternalMentionTextParser.ParseWorkItemMentions(pullRequest.Title, allowResolveVerb, resolvedDate);
      IDictionary<int, WorkItemMention> workItemMentions = ExternalMentionTextParser.ParseWorkItemMentions(pullRequest.Description, allowResolveVerb, resolvedDate);
      GitHubMentionService.MergeMentionMaps(mentionMap, workItemMentions);
      IVssRequestContext requestContext1 = requestContext;
      int count = mentionMap.Count;
      ExternalGitRepo repo = pullRequest.Repo;
      string repoName = repo != null ? repo.RepoNameWithOwner() : (string) null;
      if (!GitHubMentionService.IsWorkItemsCountExceedLimit(requestContext1, nameof (MentionPullRequest), count, repoName) && mentionMap.Any<KeyValuePair<int, WorkItemMention>>())
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.EnsureServiceIdentity();
        requestContext.RunAsUserContext(identity, (Action<IVssRequestContext>) (userRequestContext =>
        {
          GitHubMentionService.WorkItemsResolvingResult workItemsResolvingResult = GitHubMentionService.UpdateWorkItems(userRequestContext, projectIds, mentionMap.Values, nameof (MentionPullRequest), (Func<WorkItem, IEnumerable<WorkItemResourceLinkUpdate>>) (workItem => this.GetPullRequestResourceLinkUpdates(userRequestContext, repoInternalId, workItem, pullRequest, checkHistoryForMentions)), checkHistoryForMentions, pullRequest.Repo.RepoNameWithOwner(), externalConnections);
          if ((WorkItemTrackingFeatureFlags.IsAzureBoardsBotWILinkingFeedbackEnabled(requestContext) || externalConnections == null) && (!WorkItemTrackingFeatureFlags.IsAzureBoardsBotWILinkingFeedbackEnabled(requestContext) || externalConnections == null || canResolveMentions))
            return;
          this.LinkifyMentionsInDescription(userRequestContext, repoInternalId, DateTime.Parse(pullRequest.UpdatedAt, (IFormatProvider) DateTimeFormatInfo.InvariantInfo), pullRequest.Repo.RepoNameWithOwner(), pullRequest.Number, pullRequest.Description, workItemsResolvingResult, externalConnections, GitHubMentionService.MentionScenario.MentionPullRequest, pullRequest.Sender);
        }));
        flag1 = true;
      }
      return flag1;
    }

    public IEnumerable<ExternalGitPullRequest> MentionPullRequests(
      IVssRequestContext requestContext,
      Guid repoInternalId,
      IEnumerable<Guid> projectIds,
      IEnumerable<ExternalGitPullRequest> pullRequests,
      bool canResolveMentions,
      bool checkHistoryForMentions,
      IEnumerable<ExternalConnection> externalConnections)
    {
      List<ExternalGitPullRequest> externalGitPullRequestList = new List<ExternalGitPullRequest>();
      foreach (ExternalGitPullRequest pullRequest in pullRequests)
      {
        if (this.MentionPullRequest(requestContext, repoInternalId, projectIds, pullRequest, true, checkHistoryForMentions, externalConnections))
          externalGitPullRequestList.Add(pullRequest);
      }
      return (IEnumerable<ExternalGitPullRequest>) externalGitPullRequestList;
    }

    public IEnumerable<ExternalGitCommit> MentionCommits(
      IVssRequestContext requestContext,
      ExternalGitRepo repo,
      IEnumerable<Guid> projectIds,
      IEnumerable<ExternalGitCommit> commits,
      bool canResolveMentions,
      bool checkHistoryForMentions,
      IEnumerable<ExternalConnection> externalConnections)
    {
      Dictionary<int, WorkItemMention> mentionMap = new Dictionary<int, WorkItemMention>();
      Dictionary<int, ISet<ExternalGitCommit>> workItemIdToCommitsMap = new Dictionary<int, ISet<ExternalGitCommit>>();
      int currentCount = 0;
      foreach (ExternalGitCommit commit in commits)
      {
        IDictionary<int, WorkItemMention> workItemMentions = ExternalMentionTextParser.ParseWorkItemMentions(commit.Message, canResolveMentions, new DateTime?(commit.PushedDate ?? commit.CommitedDate));
        currentCount += workItemMentions.Count;
        if (GitHubMentionService.IsWorkItemsCountExceedLimit(requestContext, nameof (MentionCommits), currentCount, repo.RepoNameWithOwner()))
          return (IEnumerable<ExternalGitCommit>) null;
        foreach (KeyValuePair<int, WorkItemMention> keyValuePair in (IEnumerable<KeyValuePair<int, WorkItemMention>>) workItemMentions)
        {
          ISet<ExternalGitCommit> externalGitCommitSet;
          if (!workItemIdToCommitsMap.TryGetValue(keyValuePair.Key, out externalGitCommitSet))
          {
            externalGitCommitSet = (ISet<ExternalGitCommit>) new HashSet<ExternalGitCommit>();
            workItemIdToCommitsMap[keyValuePair.Key] = externalGitCommitSet;
          }
          externalGitCommitSet.Add(commit);
        }
        GitHubMentionService.MergeMentionMaps((IDictionary<int, WorkItemMention>) mentionMap, workItemMentions);
      }
      if (!workItemIdToCommitsMap.Any<KeyValuePair<int, ISet<ExternalGitCommit>>>())
        return (IEnumerable<ExternalGitCommit>) null;
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.EnsureServiceIdentity();
      requestContext.RunAsUserContext(identity, (Action<IVssRequestContext>) (userRequestContext => GitHubMentionService.UpdateWorkItems(userRequestContext, projectIds, (ICollection<WorkItemMention>) mentionMap.Values, nameof (MentionCommits), (Func<WorkItem, IEnumerable<WorkItemResourceLinkUpdate>>) (workItem => this.GetCommitsResourceLinkUpdates(workItem, repo.GetRepoInternalId(), (IEnumerable<ExternalGitCommit>) workItemIdToCommitsMap[workItem.Id], checkHistoryForMentions)), checkHistoryForMentions, repo.RepoNameWithOwner(), externalConnections)));
      return workItemIdToCommitsMap.Values.SelectMany<ISet<ExternalGitCommit>, ExternalGitCommit>((Func<ISet<ExternalGitCommit>, IEnumerable<ExternalGitCommit>>) (c => (IEnumerable<ExternalGitCommit>) c)).Distinct<ExternalGitCommit>((IEqualityComparer<ExternalGitCommit>) ExternalGitCommitComparer.Instance);
    }

    public bool MentionIssue(
      IVssRequestContext requestContext,
      Guid repoInternalId,
      IEnumerable<Guid> projectIds,
      ExternalGitIssue issue,
      bool checkHistoryForMentions,
      IEnumerable<ExternalConnection> externalConnections)
    {
      bool flag = false;
      IDictionary<int, WorkItemMention> mentionMap = ExternalMentionTextParser.ParseWorkItemMentions(issue.Title, false);
      IDictionary<int, WorkItemMention> workItemMentions = ExternalMentionTextParser.ParseWorkItemMentions(issue.Description, false);
      GitHubMentionService.MergeMentionMaps(mentionMap, workItemMentions);
      IVssRequestContext requestContext1 = requestContext;
      int count = mentionMap.Count;
      ExternalGitRepo repo = issue.Repo;
      string repoName = repo != null ? repo.RepoNameWithOwner() : (string) null;
      if (!GitHubMentionService.IsWorkItemsCountExceedLimit(requestContext1, nameof (MentionIssue), count, repoName) && (!WorkItemTrackingFeatureFlags.IsAzureBoardsBotWILinkingFeedbackEnabled(requestContext) && mentionMap.Any<KeyValuePair<int, WorkItemMention>>() || WorkItemTrackingFeatureFlags.IsAzureBoardsBotWILinkingFeedbackEnabled(requestContext) && mentionMap.Any<KeyValuePair<int, WorkItemMention>>() && !issue.IsDeleteAction()))
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.EnsureServiceIdentity();
        requestContext.RunAsUserContext(identity, (Action<IVssRequestContext>) (userRequestContext =>
        {
          GitHubMentionService.WorkItemsResolvingResult workItemsResolvingResult = GitHubMentionService.UpdateWorkItems(userRequestContext, projectIds, mentionMap.Values, nameof (MentionIssue), (Func<WorkItem, IEnumerable<WorkItemResourceLinkUpdate>>) (workItem => this.GetIssueResourceLinkUpdates(userRequestContext, repoInternalId, workItem, issue, checkHistoryForMentions)), checkHistoryForMentions, issue.Repo.RepoNameWithOwner(), externalConnections);
          if ((WorkItemTrackingFeatureFlags.IsAzureBoardsBotWILinkingFeedbackEnabled(requestContext) || externalConnections == null) && (!WorkItemTrackingFeatureFlags.IsAzureBoardsBotWILinkingFeedbackEnabled(requestContext) || externalConnections == null || issue.State.Equals("closed", StringComparison.OrdinalIgnoreCase)))
            return;
          this.LinkifyMentionsInDescription(userRequestContext, repoInternalId, DateTime.Parse(issue.UpdatedAt, (IFormatProvider) DateTimeFormatInfo.InvariantInfo), issue.Repo.RepoNameWithOwner(), issue.Number, issue.Description, workItemsResolvingResult, externalConnections, GitHubMentionService.MentionScenario.MentionIssue, issue.Sender);
        }));
        flag = true;
      }
      return flag;
    }

    public IEnumerable<ExternalGitIssue> MentionIssues(
      IVssRequestContext requestContext,
      Guid repoInternalId,
      IEnumerable<Guid> projectIds,
      IEnumerable<ExternalGitIssue> issues,
      bool checkHistoryForMentions,
      IEnumerable<ExternalConnection> externalConnections)
    {
      List<ExternalGitIssue> externalGitIssueList = new List<ExternalGitIssue>();
      foreach (ExternalGitIssue issue in issues)
      {
        if (this.MentionIssue(requestContext, repoInternalId, projectIds, issue, checkHistoryForMentions, externalConnections))
          externalGitIssueList.Add(issue);
      }
      return (IEnumerable<ExternalGitIssue>) externalGitIssueList;
    }

    public void MentionIssueComment(
      IVssRequestContext requestContext,
      Guid repoInternalId,
      IEnumerable<Guid> projectIds,
      ExternalPullRequestCommentEvent issueComment,
      IEnumerable<ExternalConnection> externalConnections)
    {
      IDictionary<int, WorkItemMention> mentionMap = ExternalMentionTextParser.ParseWorkItemMentions(issueComment.CommentBody, false);
      IVssRequestContext requestContext1 = requestContext;
      int count = mentionMap.Count;
      ExternalGitRepo repo = issueComment.Repo;
      string repoName = repo != null ? repo.RepoNameWithOwner() : (string) null;
      if (GitHubMentionService.IsWorkItemsCountExceedLimit(requestContext1, nameof (MentionIssueComment), count, repoName) || (WorkItemTrackingFeatureFlags.IsAzureBoardsBotWILinkingFeedbackEnabled(requestContext) || !mentionMap.Any<KeyValuePair<int, WorkItemMention>>()) && (!WorkItemTrackingFeatureFlags.IsAzureBoardsBotWILinkingFeedbackEnabled(requestContext) || !mentionMap.Any<KeyValuePair<int, WorkItemMention>>() || issueComment.IsDeleteAction()))
        return;
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.EnsureServiceIdentity();
      requestContext.RunAsUserContext(identity, (Action<IVssRequestContext>) (userRequestContext =>
      {
        GitHubMentionService.WorkItemsResolvingResult workItemsToUnfurl = GitHubMentionService.GetWorkItemsToUnfurl(userRequestContext, mentionMap.Values, projectIds);
        this.LinkifyMentionsInDescription(userRequestContext, repoInternalId, DateTime.Parse(issueComment.UpdatedAt, (IFormatProvider) DateTimeFormatInfo.InvariantInfo), issueComment.Repo.RepoNameWithOwner(), issueComment.PullRequestNumber, issueComment.CommentBody, workItemsToUnfurl, externalConnections, GitHubMentionService.MentionScenario.MentionIssueComment, issueComment.Sender, issueComment.GetCommentDatabaseId());
      }));
    }

    public void MentionIssueComments(
      IVssRequestContext requestContext,
      Guid repoInternalId,
      IEnumerable<Guid> projectIds,
      IEnumerable<ExternalPullRequestCommentEvent> issueComments,
      IEnumerable<ExternalConnection> externalConnections)
    {
      foreach (ExternalPullRequestCommentEvent issueComment in issueComments)
        this.MentionIssueComment(requestContext, repoInternalId, projectIds, issueComment, externalConnections);
    }

    public void MentionCommitComment(
      IVssRequestContext requestContext,
      Guid repoInternalId,
      IEnumerable<Guid> projectIds,
      ExternalGitCommitComment commitComment,
      IEnumerable<ExternalConnection> externalConnections)
    {
      IDictionary<int, WorkItemMention> mentionMap = ExternalMentionTextParser.ParseWorkItemMentions(commitComment.CommentBody, false);
      IVssRequestContext requestContext1 = requestContext;
      int count = mentionMap.Count;
      ExternalGitRepo repo1 = commitComment.Repo;
      string repoName = repo1 != null ? repo1.RepoNameWithOwner() : (string) null;
      if (GitHubMentionService.IsWorkItemsCountExceedLimit(requestContext1, nameof (MentionCommitComment), count, repoName) || !mentionMap.Any<KeyValuePair<int, WorkItemMention>>())
        return;
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.EnsureServiceIdentity();
      requestContext.RunAsUserContext(identity, (Action<IVssRequestContext>) (userRequestContext =>
      {
        GitHubMentionService.WorkItemsResolvingResult workItemsToUnfurl = GitHubMentionService.GetWorkItemsToUnfurl(userRequestContext, mentionMap.Values, projectIds);
        IVssRequestContext requestContext2 = userRequestContext;
        Guid repoInternalId1 = repoInternalId;
        DateTime artifactUpdateTime = DateTime.Parse(commitComment.UpdatedAt, (IFormatProvider) DateTimeFormatInfo.InvariantInfo);
        ExternalGitRepo repo2 = commitComment.Repo;
        string repoNameWithOwner = repo2 != null ? repo2.RepoNameWithOwner() : (string) null;
        string commitId = commitComment.CommitId;
        string commentBody = commitComment.CommentBody;
        GitHubMentionService.WorkItemsResolvingResult workItemsResolvingResult = workItemsToUnfurl;
        IEnumerable<ExternalConnection> externalConnections1 = externalConnections;
        ExternalGitUser sender = commitComment.Sender;
        string commentDatabaseId = commitComment.GetCommentDatabaseId();
        this.LinkifyMentionsInDescription(requestContext2, repoInternalId1, artifactUpdateTime, repoNameWithOwner, commitId, commentBody, workItemsResolvingResult, externalConnections1, GitHubMentionService.MentionScenario.MentionCommitComment, sender, commentDatabaseId);
      }));
    }

    public void MentionCommitComments(
      IVssRequestContext requestContext,
      Guid repoInternalId,
      IEnumerable<Guid> projectIds,
      IEnumerable<ExternalGitCommitComment> commitComments,
      IEnumerable<ExternalConnection> externalConnections)
    {
      foreach (ExternalGitCommitComment commitComment in commitComments)
        this.MentionCommitComment(requestContext, repoInternalId, projectIds, commitComment, externalConnections);
    }

    private bool IsDefaultBranch(string defaultBranchName, string refPath)
    {
      if (string.IsNullOrEmpty(defaultBranchName))
        return false;
      return defaultBranchName.Equals(refPath) || ("refs/heads/" + defaultBranchName).Equals(refPath);
    }

    private static void MergeMentionMaps(
      IDictionary<int, WorkItemMention> mergeIntoMap,
      IDictionary<int, WorkItemMention> mergeFromMap)
    {
      foreach (KeyValuePair<int, WorkItemMention> mergeFrom in (IEnumerable<KeyValuePair<int, WorkItemMention>>) mergeFromMap)
      {
        WorkItemMention workItemMention;
        if (mergeIntoMap.TryGetValue(mergeFrom.Key, out workItemMention))
        {
          if (mergeFrom.Value.ShouldResolve && !workItemMention.ShouldResolve)
            mergeIntoMap[mergeFrom.Key] = mergeFrom.Value;
        }
        else
          mergeIntoMap[mergeFrom.Key] = mergeFrom.Value;
      }
    }

    private IEnumerable<WorkItemResourceLinkUpdate> GetCommitsResourceLinkUpdates(
      WorkItem workItem,
      Guid repoInternalId,
      IEnumerable<ExternalGitCommit> commits,
      bool checkHistoryForMentions)
    {
      foreach (ExternalGitCommit commit in commits)
      {
        string commitArtifactUrl = GitHubMentionService.GetCommitArtifactUrl(repoInternalId, commit.Sha);
        WorkItemResourceLinkUpdate resourceLinkUpdates = this.GetArtifactResourceLinkUpdates(workItem, "GitHub Commit", commitArtifactUrl, checkHistoryForMentions ? new DateTime?(GitHubMentionService.GetDateForHistoryValidation(commit)) : new DateTime?());
        if (resourceLinkUpdates != null)
          yield return resourceLinkUpdates;
      }
    }

    private IEnumerable<WorkItemResourceLinkUpdate> GetPullRequestResourceLinkUpdates(
      IVssRequestContext requestContext,
      Guid repoInternalId,
      WorkItem workItem,
      ExternalGitPullRequest gitPullRequest,
      bool checkHistoryForMentions)
    {
      string requestArtifactUrl = GitHubMentionService.GetPullRequestArtifactUrl(repoInternalId, gitPullRequest.Number);
      WorkItemResourceLinkUpdate resourceLinkUpdates = this.GetArtifactResourceLinkUpdates(workItem, "GitHub Pull Request", requestArtifactUrl, checkHistoryForMentions ? new DateTime?(GitHubMentionService.GetDateForHistoryValidation(requestContext, gitPullRequest)) : new DateTime?());
      if (resourceLinkUpdates != null)
        yield return resourceLinkUpdates;
    }

    private IEnumerable<WorkItemResourceLinkUpdate> GetIssueResourceLinkUpdates(
      IVssRequestContext requestContext,
      Guid repoInternalId,
      WorkItem workItem,
      ExternalGitIssue gitIssue,
      bool checkHistoryForMentions)
    {
      string issueArtifactUrl = GitHubMentionService.GetIssueArtifactUrl(repoInternalId, gitIssue.Number);
      WorkItemResourceLinkUpdate resourceLinkUpdates = this.GetArtifactResourceLinkUpdates(workItem, "GitHub Issue", issueArtifactUrl, checkHistoryForMentions ? new DateTime?(GitHubMentionService.GetDateForHistoryValidation(requestContext, gitIssue)) : new DateTime?());
      if (resourceLinkUpdates != null)
        yield return resourceLinkUpdates;
    }

    private WorkItemResourceLinkUpdate GetArtifactResourceLinkUpdates(
      WorkItem workItem,
      string linkName,
      string linkUrl,
      DateTime? checkHistoryDate = null)
    {
      if ((!checkHistoryDate.HasValue ? (IEnumerable<WorkItemResourceLinkInfo>) workItem.ResourceLinks : workItem.AllResourceLinks.Where<WorkItemResourceLinkInfo>((Func<WorkItemResourceLinkInfo, bool>) (r =>
      {
        DateTime authorizedDate = r.AuthorizedDate;
        DateTime? nullable = checkHistoryDate;
        return nullable.HasValue && authorizedDate >= nullable.GetValueOrDefault();
      }))).Any<WorkItemResourceLinkInfo>((Func<WorkItemResourceLinkInfo, bool>) (rl => rl.ResourceType == ResourceLinkType.ArtifactLink && string.Equals(rl.Location, linkUrl, StringComparison.OrdinalIgnoreCase))))
        return (WorkItemResourceLinkUpdate) null;
      WorkItemResourceLinkUpdate resourceLinkUpdates = new WorkItemResourceLinkUpdate();
      resourceLinkUpdates.Location = linkUrl;
      resourceLinkUpdates.Type = new ResourceLinkType?(ResourceLinkType.ArtifactLink);
      resourceLinkUpdates.Name = linkName;
      resourceLinkUpdates.UpdateType = LinkUpdateType.Add;
      return resourceLinkUpdates;
    }

    private static DateTime GetDateForHistoryValidation(
      IVssRequestContext requestContext,
      ExternalGitPullRequest gitPullRequest)
    {
      DateTime result;
      if (DateTime.TryParse(gitPullRequest.CreatedAt, out result))
        return GitHubMentionService.GetAdjustedDateTime(result);
      requestContext.Trace(920091, TraceLevel.Error, "ExternalMentions", nameof (GitHubMentionService), "GitHub PR " + gitPullRequest.NodeId() + " with id " + gitPullRequest.Number + " had invalid dates. Created At: " + gitPullRequest.CreatedAt + "; Updated At: " + gitPullRequest.UpdatedAt + ";");
      return DateTime.MinValue;
    }

    private static DateTime GetDateForHistoryValidation(
      IVssRequestContext requestContext,
      ExternalGitIssue gitIssue)
    {
      DateTime result;
      if (DateTime.TryParse(gitIssue.CreatedAt, out result))
        return GitHubMentionService.GetAdjustedDateTime(result);
      requestContext.Trace(920092, TraceLevel.Error, "ExternalMentions", nameof (GitHubMentionService), "GitHub Issue " + gitIssue.NodeId<ExternalGitIssue>() + " with id " + gitIssue.Number + " had invalid dates. Created At: " + gitIssue.CreatedAt + "; Updated At: " + gitIssue.UpdatedAt + ";");
      return DateTime.MinValue;
    }

    private static DateTime GetDateForHistoryValidation(ExternalGitCommit commit) => GitHubMentionService.GetAdjustedDateTime(commit.PushedDate ?? commit.CommitedDate);

    private static DateTime GetAdjustedDateTime(DateTime date) => date.AddMinutes(-5.0);

    private static bool IsWorkItemsCountExceedLimit(
      IVssRequestContext requestContext,
      string scenario,
      int currentCount,
      string repoName)
    {
      int externalMentions = requestContext.WitContext().ServerSettings.MaxNumberOfWorkItemsToProcessInExternalMentions;
      if (currentCount < externalMentions)
        return false;
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("Error", string.Format("Exceeded limit of max work item id to process: {0}", (object) externalMentions));
      properties.Add("Scenario", scenario);
      properties.Add("RepoName", repoName);
      properties.Add("Count", (double) currentCount);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "ExternalMentions", "ExtractWorkItems", properties);
      return true;
    }

    private static GitHubMentionService.WorkItemsResolvingResult UpdateWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<Guid> projectIds,
      ICollection<WorkItemMention> workItemMentions,
      string scenario,
      Func<WorkItem, IEnumerable<WorkItemResourceLinkUpdate>> getResourceUpdates,
      bool checkHistoryForMentions,
      string repoNameWithOwner,
      IEnumerable<ExternalConnection> externalConnections)
    {
      GitHubMentionService.WorkItemsResolvingResult itemsResolvingResult = new GitHubMentionService.WorkItemsResolvingResult();
      requestContext.TraceEnter(920040, "ExternalMentions", nameof (GitHubMentionService), nameof (UpdateWorkItems));
      if (workItemMentions.Any<WorkItemMention>())
      {
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add(nameof (scenario), scenario);
        properties.Add(nameof (checkHistoryForMentions), checkHistoryForMentions);
        properties.Add("workItemCount", (double) workItemMentions.Count);
        List<WorkItemUpdate> source1 = new List<WorkItemUpdate>();
        List<WorkItem> workItemList = new List<WorkItem>();
        ITeamFoundationWorkItemService service = requestContext.GetService<ITeamFoundationWorkItemService>();
        Dictionary<int, WorkItem> dictionary = service.GetWorkItems(requestContext, workItemMentions.Select<WorkItemMention, int>((Func<WorkItemMention, int>) (x => x.WorkItemId)), includeWorkItemLinks: false, includeHistory: checkHistoryForMentions, includeTags: false, errorPolicy: WorkItemErrorPolicy.Omit, useWorkItemIdentity: true).ToList<WorkItem>().Where<WorkItem>((Func<WorkItem, bool>) (w => w != null)).ToDictionary<WorkItem, int, WorkItem>((Func<WorkItem, int>) (w => w.Id), (Func<WorkItem, WorkItem>) (w => w));
        int num1 = 0;
        int num2 = 0;
        List<int> intList1 = new List<int>();
        List<int> intList2 = new List<int>();
        foreach (WorkItemMention workItemMention in (IEnumerable<WorkItemMention>) workItemMentions)
        {
          WorkItem workItem;
          if (!dictionary.TryGetValue(workItemMention.WorkItemId, out workItem))
          {
            ++num2;
            intList2.Add(workItemMention.WorkItemId);
            itemsResolvingResult.notResolvedWorkItemIds.Add(workItemMention.WorkItemId);
          }
          else
          {
            Guid projectGuid = workItem.GetProjectGuid(requestContext);
            if (!projectIds.Contains<Guid>(projectGuid))
            {
              ++num1;
              intList1.Add(workItem.Id);
              itemsResolvingResult.notResolvedWorkItemIds.Add(workItemMention.WorkItemId);
            }
            else
            {
              itemsResolvingResult.resolvedWorkItems.Add((workItem, projectGuid));
              IEnumerable<WorkItemResourceLinkUpdate> source2 = getResourceUpdates(workItem);
              if (source2.Any<WorkItemResourceLinkUpdate>())
                source1.Add(new WorkItemUpdate()
                {
                  Id = workItem.Id,
                  ResourceLinkUpdates = source2
                });
              if (workItemMention.ShouldResolve)
              {
                if (checkHistoryForMentions && !GitHubMentionService.WasRecentlyTransitioned(requestContext, workItem, workItemMention.ResolvedDate.Value))
                  workItemList.Add(workItem);
                else if (!checkHistoryForMentions)
                  workItemList.Add(workItem);
              }
            }
          }
        }
        properties.Add("workItemsToMentionCount", (double) workItemMentions.Count);
        properties.Add("workItemsToLinkCount", (double) source1.Count);
        properties.Add("workItemsToResolveCount", (double) workItemMentions.Where<WorkItemMention>((Func<WorkItemMention, bool>) (m => m.ShouldResolve)).Count<WorkItemMention>());
        properties.Add("workItemsToResolveAfterHistoryCheckCount", (double) workItemList.Count);
        properties.Add("failedToReadCount", (double) num2);
        properties.Add("failedToReadWorkItemIds", (object) intList2);
        properties.Add("mismatchedProjectCount", (double) num1);
        properties.Add("mismatchedProjectWorkItemIds", (object) intList1);
        properties.Add(nameof (projectIds), (object) projectIds);
        properties.Add(nameof (repoNameWithOwner), repoNameWithOwner);
        properties.Add("providerKey", externalConnections != null ? externalConnections.FirstOrDefault<ExternalConnection>()?.ProviderKey : (string) null);
        properties.Add("serviceEndpointAuthTypes", externalConnections != null ? (object) externalConnections.ToDedupedDictionary<ExternalConnection, Guid, string>((Func<ExternalConnection, Guid>) (c => c.ServiceEndpoint.RawServiceEndpoint.Id), (Func<ExternalConnection, string>) (c => c.ServiceEndpoint.AuthorizationScheme)) : (object) (Dictionary<Guid, string>) null);
        if (source1.Any<WorkItemUpdate>())
        {
          properties.Add("workItemsToLink", (object) source1.Select<WorkItemUpdate, int>((Func<WorkItemUpdate, int>) (w => w.Id)));
          IEnumerable<WorkItemUpdateResult> source3 = service.UpdateWorkItems(requestContext, (IEnumerable<WorkItemUpdate>) source1, true, useWorkItemIdentity: true);
          if (source3.Any<WorkItemUpdateResult>((Func<WorkItemUpdateResult, bool>) (r => r.Exception != null)))
            properties.Add("linkExceptions", (object) source3.Where<WorkItemUpdateResult>((Func<WorkItemUpdateResult, bool>) (r => r.Exception != null)).Select<WorkItemUpdateResult, string>((Func<WorkItemUpdateResult, string>) (r => r.Exception.ToString())));
        }
        if (workItemList.Any<WorkItem>())
        {
          properties.Add("workItemsToResolve", (object) workItemList.Select<WorkItem, int>((Func<WorkItem, int>) (w => w.Id)));
          IEnumerable<WorkItemUpdateResult> source4 = service.UpdateWorkItemsStateOnCheckin(requestContext, (IEnumerable<WorkItem>) workItemList);
          if (source4.Any<WorkItemUpdateResult>((Func<WorkItemUpdateResult, bool>) (r => r.Exception != null)))
            properties.Add("resolveVerbExceptions", (object) source4.Where<WorkItemUpdateResult>((Func<WorkItemUpdateResult, bool>) (r => r.Exception != null)).Select<WorkItemUpdateResult, string>((Func<WorkItemUpdateResult, string>) (r => r.Exception.ToString())));
        }
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "ExternalMentions", nameof (UpdateWorkItems), properties);
      }
      requestContext.TraceLeave(920049, "ExternalMentions", nameof (GitHubMentionService), nameof (UpdateWorkItems));
      return itemsResolvingResult;
    }

    private static GitHubMentionService.WorkItemsResolvingResult GetWorkItemsToUnfurl(
      IVssRequestContext requestContext,
      ICollection<WorkItemMention> workItemMentions,
      IEnumerable<Guid> projectIds)
    {
      GitHubMentionService.WorkItemsResolvingResult workItemsToUnfurl = new GitHubMentionService.WorkItemsResolvingResult();
      if (workItemMentions.Any<WorkItemMention>())
      {
        Dictionary<int, WorkItem> dictionary = requestContext.GetService<ITeamFoundationWorkItemService>().GetWorkItems(requestContext, workItemMentions.Select<WorkItemMention, int>((Func<WorkItemMention, int>) (x => x.WorkItemId)), false, false, false, false, errorPolicy: WorkItemErrorPolicy.Omit, useWorkItemIdentity: true).Where<WorkItem>((Func<WorkItem, bool>) (w => w != null)).ToDictionary<WorkItem, int, WorkItem>((Func<WorkItem, int>) (w => w.Id), (Func<WorkItem, WorkItem>) (w => w));
        foreach (WorkItemMention workItemMention in (IEnumerable<WorkItemMention>) workItemMentions)
        {
          WorkItem workItem;
          if (!dictionary.TryGetValue(workItemMention.WorkItemId, out workItem))
          {
            workItemsToUnfurl.notResolvedWorkItemIds.Add(workItemMention.WorkItemId);
          }
          else
          {
            Guid projectGuid = workItem.GetProjectGuid(requestContext);
            if (!projectIds.Contains<Guid>(projectGuid))
              workItemsToUnfurl.notResolvedWorkItemIds.Add(workItemMention.WorkItemId);
            else
              workItemsToUnfurl.resolvedWorkItems.Add((workItem, projectGuid));
          }
        }
      }
      return workItemsToUnfurl;
    }

    private void LinkifyMentionsInDescription(
      IVssRequestContext requestContext,
      Guid repoInternalId,
      DateTime artifactUpdateTime,
      string repoNameWithOwner,
      string artifactIdentifer,
      string body,
      GitHubMentionService.WorkItemsResolvingResult workItemsResolvingResult,
      IEnumerable<ExternalConnection> externalConnections,
      GitHubMentionService.MentionScenario scenario,
      ExternalGitUser sender,
      string commentId = null)
    {
      if (!workItemsResolvingResult.resolvedWorkItems.Any<(WorkItem, Guid)>() && (!WorkItemTrackingFeatureFlags.IsAzureBoardsBotWILinkingFeedbackEnabled(requestContext) || !workItemsResolvingResult.notResolvedWorkItemIds.Any<int>()))
        return;
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("Scenario", scenario.ToString());
      properties.Add("WorkItemCount", (double) workItemsResolvingResult.resolvedWorkItems.Count);
      properties.Add("NotResolvedWorkItemCount", (double) workItemsResolvingResult.notResolvedWorkItemIds.Count);
      properties.Add("IsAzureBoardsBotWILinkingFeedbackEnabled", WorkItemTrackingFeatureFlags.IsAzureBoardsBotWILinkingFeedbackEnabled(requestContext));
      properties.Add("GitHubProviderKey", externalConnections.FirstOrDefault<ExternalConnection>()?.ProviderKey);
      bool flag1 = false;
      try
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        properties.Add("GitHubUpdateTime", (object) artifactUpdateTime);
        Dictionary<int, string> replacements = new Dictionary<int, string>();
        ITswaServerHyperlinkService service = requestContext.GetService<ITswaServerHyperlinkService>();
        foreach ((WorkItem workItem, Guid projectId) resolvedWorkItem in workItemsResolvingResult.resolvedWorkItems)
        {
          string link = service.GetWorkItemEditorUrl(requestContext, resolvedWorkItem.projectId, resolvedWorkItem.workItem.Id).ToString();
          string markdownMention = ExternalMentionTextParser.GetMarkdownMention(resolvedWorkItem.workItem.Id, link);
          replacements.Add(resolvedWorkItem.workItem.Id, markdownMention);
        }
        string str = ExternalMentionTextParser.ReplaceWorkItemsWithHyperlinks(body, replacements);
        GitHubHttpClient gitHubHttpClient = AzureBoardsGitHubDataHelper.Create(requestContext).GitHubHttpClient;
        flag1 = true;
        Dictionary<Guid, string> dictionary1 = new Dictionary<Guid, string>();
        Dictionary<Guid, string> dictionary2 = new Dictionary<Guid, string>();
        foreach (ExternalConnection externalConnection in externalConnections)
        {
          bool flag2 = true;
          bool flag3 = true;
          ServiceEndpoint rawServiceEndpoint = externalConnection.ServiceEndpoint.RawServiceEndpoint;
          string enterpriseUrl = rawServiceEndpoint.GetEnterpriseUrl();
          GitHubAuthentication hubAuthentication = rawServiceEndpoint != null ? rawServiceEndpoint.GetGitHubAuthentication(requestContext, externalConnection.ProjectId) : (GitHubAuthentication) null;
          if (hubAuthentication != null)
          {
            if (str != body)
            {
              switch (scenario)
              {
                case GitHubMentionService.MentionScenario.MentionPullRequest:
                case GitHubMentionService.MentionScenario.MentionIssue:
                  flag2 = gitHubHttpClient.UpdateIssue(enterpriseUrl, hubAuthentication, repoNameWithOwner, int.Parse(artifactIdentifer), newDescription: str).IsSuccessful;
                  break;
                case GitHubMentionService.MentionScenario.MentionIssueComment:
                  flag2 = !string.IsNullOrWhiteSpace(commentId) && gitHubHttpClient.UpdateIssueComment(enterpriseUrl, hubAuthentication, repoNameWithOwner, commentId, str).IsSuccessful;
                  break;
                case GitHubMentionService.MentionScenario.MentionCommitComment:
                  flag2 = !string.IsNullOrWhiteSpace(commentId) && gitHubHttpClient.UpdateCommitComment(enterpriseUrl, hubAuthentication, repoNameWithOwner, commentId, str).IsSuccessful;
                  break;
              }
              properties.Add("LinkifyMentionsInBodySuccess", flag2);
            }
            string name1 = "AzureBoardsBotFeedbackCommentCreated";
            string name2 = "AzureBoardsBotFeedbackCommentNotCreatedReason";
            if (WorkItemTrackingFeatureFlags.IsAzureBoardsBotWILinkingFeedbackEnabled(requestContext) && scenario != GitHubMentionService.MentionScenario.MentionCommitComment && !sender.IsBot() && (str != body || workItemsResolvingResult.notResolvedWorkItemIds.Any<int>()))
            {
              string feedbackCommentText = GitHubMentionService.GetFeedbackCommentText(requestContext, workItemsResolvingResult);
              if (feedbackCommentText.Length > 0)
              {
                flag3 = gitHubHttpClient.PostComment(enterpriseUrl, hubAuthentication, repoNameWithOwner, artifactIdentifer, feedbackCommentText).IsSuccessful;
                properties.Add(name1, true);
                if (!flag3)
                  properties.Add(name2, "The HTTP call to create the feedback comment has failed");
                properties.Add("AzureBoardsBotFeedbackCommentSuccess", flag3);
              }
              else
              {
                properties.Add(name1, false);
                properties.Add(name2, "Generated comment text is empty");
              }
            }
            else
            {
              properties.Add(name1, false);
              properties.Add(name2, "The general criteria to post a new comment was not met");
              properties.Add("SenderType", sender?.Type);
              properties.Add("SenderIsBot", (object) (sender != null ? new bool?(sender.IsBot()) : new bool?()));
              properties.Add("BodyWasChanged", str != body);
              properties.Add("NotResolvedWorkItems", workItemsResolvingResult.notResolvedWorkItemIds.Any<int>());
            }
            if (flag2 & flag3)
            {
              dictionary1.Add(rawServiceEndpoint.Id, externalConnection.ServiceEndpoint.AuthorizationScheme);
              stopwatch.Stop();
              properties.Add("GitHubUpdateSucceededServiceEndpoints", (object) dictionary1);
              properties.Add("GitHubProcessingUpdateTimeMilliseconds", stopwatch.Elapsed.TotalMilliseconds);
              break;
            }
            dictionary2.Add(rawServiceEndpoint.Id, externalConnection.ServiceEndpoint.AuthorizationScheme);
            properties.Add("GitHubUpdateFailedServiceEndpoints", (object) dictionary2);
            properties.Add("GitHubUpdateFailedRepoId", (object) repoInternalId);
            properties.Add("GitHubUpdateFailedArtifactIdentifier", artifactIdentifer);
          }
        }
      }
      catch (Exception ex)
      {
        properties.Add("LinkifyMentionsInBodySuccess", false);
        properties.Add("FailureReason", (object) ex);
      }
      finally
      {
        if (flag1)
          requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "ExternalMentions", nameof (LinkifyMentionsInDescription), properties);
      }
    }

    private static string GetFeedbackCommentText(
      IVssRequestContext requestContext,
      GitHubMentionService.WorkItemsResolvingResult workItemsResolvingResult)
    {
      string str1 = "\r\n";
      StringBuilder stringBuilder = new StringBuilder();
      if (workItemsResolvingResult.notResolvedWorkItemIds.Any<int>())
      {
        stringBuilder.Append(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.GitHubBoardsBotLinkingErrorHeading());
        stringBuilder.Append(str1);
        foreach (int resolvedWorkItemId in workItemsResolvingResult.notResolvedWorkItemIds)
          stringBuilder.AppendFormat("- {0}{1}", (object) resolvedWorkItemId, (object) str1);
        stringBuilder.Append(str1);
        stringBuilder.Append(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.GitHubBoardsBotLinkingErrorSuggestion());
        stringBuilder.Append(str1);
      }
      if (workItemsResolvingResult.resolvedWorkItems.Any<(WorkItem, Guid)>())
      {
        stringBuilder.Append(str1);
        stringBuilder.Append(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.GitHubBoardsBotLinkingSuccessHeading());
        stringBuilder.Append(str1);
        ITswaServerHyperlinkService service = requestContext.GetService<ITswaServerHyperlinkService>();
        foreach ((WorkItem workItem, Guid guid) in workItemsResolvingResult.resolvedWorkItems)
        {
          string str2 = service.GetWorkItemEditorUrl(requestContext, guid, workItem.Id).ToString();
          stringBuilder.AppendFormat("- [{0} {1}]({2}){3}", (object) workItem.WorkItemType, (object) workItem.Id, (object) str2, (object) str1);
        }
      }
      return stringBuilder.ToString();
    }

    private static bool WasRecentlyTransitioned(
      IVssRequestContext requestContext,
      WorkItem workItem,
      DateTime expectedTransitionDate)
    {
      IdentityRef changedBy = requestContext.GetUserIdentity().ToIdentityRef(requestContext, false);
      IOrderedEnumerable<WorkItemRevision> source = workItem.Revisions.Union<WorkItemRevision>((IEnumerable<WorkItemRevision>) new WorkItem[1]
      {
        workItem
      }).OrderBy<WorkItemRevision, int>((Func<WorkItemRevision, int>) (r => r.Revision));
      WorkItemRevision firstRevisionEditedByAzureBoards = source.FirstOrDefault<WorkItemRevision>((Func<WorkItemRevision, bool>) (r => r.AuthorizedDate >= GitHubMentionService.GetAdjustedDateTime(expectedTransitionDate) && r.GetFieldValue<IdentityRef>(requestContext, -1).Descriptor == changedBy.Descriptor));
      if (firstRevisionEditedByAzureBoards == null)
        return false;
      IEnumerable<WorkItemRevision> workItemRevisions = source.Where<WorkItemRevision>((Func<WorkItemRevision, bool>) (r => r.Revision >= firstRevisionEditedByAzureBoards.Revision - 1));
      return workItemRevisions.Zip<WorkItemRevision, WorkItemRevision, bool>(workItemRevisions.Skip<WorkItemRevision>(1), (Func<WorkItemRevision, WorkItemRevision, bool>) ((previousRev, newRev) => !string.Equals(previousRev.State, newRev.State, StringComparison.OrdinalIgnoreCase) && newRev.GetFieldValue<IdentityRef>(requestContext, -1).Descriptor == changedBy.Descriptor)).FirstOrDefault<bool>((Func<bool, bool>) (transitioned => transitioned));
    }

    private static string GetCommitArtifactUrl(Guid repoInternalId, string sha) => LinkingUtilities.EncodeUri(new ArtifactId("GitHub", "Commit", GitHubMentionService.GetArtifactId(repoInternalId, sha)));

    private static string GetPullRequestArtifactUrl(Guid repoInternalId, string number) => LinkingUtilities.EncodeUri(new ArtifactId("GitHub", "PullRequest", GitHubMentionService.GetArtifactId(repoInternalId, number)));

    private static string GetIssueArtifactUrl(Guid repoInternalId, string number) => LinkingUtilities.EncodeUri(new ArtifactId("GitHub", "Issue", GitHubMentionService.GetArtifactId(repoInternalId, number)));

    private static string GetArtifactId(Guid repoInternalId, string numberOrSHA) => string.Format("{0}/{1}", (object) repoInternalId, (object) numberOrSHA);

    private enum MentionScenario
    {
      MentionPullRequest,
      MentionIssue,
      MentionIssueComment,
      MentionCommitComment,
    }

    private class WorkItemsResolvingResult
    {
      public HashSet<(WorkItem workItem, Guid projectId)> resolvedWorkItems = new HashSet<(WorkItem, Guid)>();
      public HashSet<int> notResolvedWorkItemIds = new HashSet<int>();
    }
  }
}
