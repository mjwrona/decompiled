// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalMentions.IGitHubMentionService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections;
using Microsoft.VisualStudio.Services.ExternalEvent;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalMentions
{
  [DefaultServiceImplementation(typeof (GitHubMentionService))]
  public interface IGitHubMentionService : IVssFrameworkService
  {
    IEnumerable<ExternalGitCommit> MentionCommits(
      IVssRequestContext requestContext,
      ExternalGitRepo repo,
      IEnumerable<Guid> projectIds,
      IEnumerable<ExternalGitCommit> commits,
      bool canResolveMentions,
      bool checkHistoryForMentions,
      IEnumerable<ExternalConnection> externalConnections);

    bool MentionPullRequest(
      IVssRequestContext requestContext,
      Guid repoInternalId,
      IEnumerable<Guid> projectIds,
      ExternalGitPullRequest pullRequest,
      bool canResolveMentions,
      bool checkHistoryForMentions,
      IEnumerable<ExternalConnection> externalConnections);

    IEnumerable<ExternalGitPullRequest> MentionPullRequests(
      IVssRequestContext requestContext,
      Guid repoInternalId,
      IEnumerable<Guid> projectIds,
      IEnumerable<ExternalGitPullRequest> pullRequests,
      bool canResolveMentions,
      bool checkHistoryForMentions,
      IEnumerable<ExternalConnection> externalConnections);

    bool MentionIssue(
      IVssRequestContext requestContext,
      Guid repoInternalId,
      IEnumerable<Guid> projectIds,
      ExternalGitIssue issue,
      bool checkHistoryForMentions,
      IEnumerable<ExternalConnection> externalConnections);

    IEnumerable<ExternalGitIssue> MentionIssues(
      IVssRequestContext requestContext,
      Guid repoInternalId,
      IEnumerable<Guid> projectIds,
      IEnumerable<ExternalGitIssue> issues,
      bool checkHistoryForMentions,
      IEnumerable<ExternalConnection> externalConnections);

    void MentionIssueComment(
      IVssRequestContext requestContext,
      Guid repoInternalId,
      IEnumerable<Guid> projectIds,
      ExternalPullRequestCommentEvent issueComment,
      IEnumerable<ExternalConnection> externalConnections);

    void MentionIssueComments(
      IVssRequestContext requestContext,
      Guid repoInternalId,
      IEnumerable<Guid> projectIds,
      IEnumerable<ExternalPullRequestCommentEvent> issueComments,
      IEnumerable<ExternalConnection> externalConnections);

    void MentionCommitComment(
      IVssRequestContext requestContext,
      Guid repoInternalId,
      IEnumerable<Guid> projectIds,
      ExternalGitCommitComment commitComment,
      IEnumerable<ExternalConnection> externalConnections);

    void MentionCommitComments(
      IVssRequestContext requestContext,
      Guid repoInternalId,
      IEnumerable<Guid> projectIds,
      IEnumerable<ExternalGitCommitComment> commitComments,
      IEnumerable<ExternalConnection> externalConnections);
  }
}
