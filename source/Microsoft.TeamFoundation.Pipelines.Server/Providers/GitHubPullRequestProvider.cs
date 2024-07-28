// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Providers.GitHubPullRequestProvider
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.Common;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.TeamFoundation.Pipelines.Server.Providers
{
  public class GitHubPullRequestProvider : IPipelinePullRequestProvider
  {
    private const string c_layer = "GitHubPullRequestProvider";
    private const string c_ownerAssociation = "OWNER";
    private const string c_adminPermission = "admin";
    private const string c_writePermission = "write";

    public ExternalGitPullRequest GetExternalPullRequest(
      IVssRequestContext requestContext,
      string pullRequestUrl,
      JObject authentication)
    {
      GitHubAuthentication gitHubAuthentication;
      GitHubResult<JObject> pullRequest = this.CreateGitHubHttpClient(requestContext, authentication, out gitHubAuthentication).GetPullRequest(gitHubAuthentication, pullRequestUrl);
      if (pullRequest.IsSuccessful)
        return PublishingUtils.GitHubPullRequestToExternalGitPullRequest(pullRequest.Result);
      requestContext.TraceError(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.GetExternalPullRequest, nameof (GitHubPullRequestProvider), "Getting external pull request failed. Details: " + pullRequest.ErrorMessage);
      return (ExternalGitPullRequest) null;
    }

    public ExternalGitPullRequest GetExternalPullRequest(
      IVssRequestContext requestContext,
      JObject authentication,
      string repoName,
      int prNumber,
      int lastCommitsToInclude = 0)
    {
      GitHubAuthentication gitHubAuthentication;
      GitHubResult<GitHubData.V4.PullRequest> pullRequest = this.CreateGitHubHttpClient(requestContext, authentication, out gitHubAuthentication).GetPullRequest((string) null, gitHubAuthentication, repoName, prNumber, lastCommitsToInclude);
      if (!pullRequest.IsSuccessful)
      {
        requestContext.TraceError(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.GetExternalPullRequestWithCommits, nameof (GitHubPullRequestProvider), "Getting external pull request with GraphQL failed. Details: " + pullRequest.ErrorMessage);
        return (ExternalGitPullRequest) null;
      }
      if (pullRequest.Result != null && !string.IsNullOrEmpty(pullRequest.Result.Number))
        return PublishingUtils.GitHubPullRequestToExternalGitPullRequest(pullRequest.Result);
      requestContext.TraceError(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.GetExternalPullRequestWithCommits, nameof (GitHubPullRequestProvider), string.Format("Pull request {0} was not found with GraphQL.", (object) prNumber));
      return (ExternalGitPullRequest) null;
    }

    public MergeJobStatus GetMergeJobStatus(ExternalGitPullRequest pullRequest)
    {
      if (string.Equals(pullRequest.State, "closed", StringComparison.OrdinalIgnoreCase))
        return MergeJobStatus.PullRequestClosed;
      if (!pullRequest.IsMergeable.HasValue)
        return MergeJobStatus.NotReady;
      return !pullRequest.IsMergeable.Value ? MergeJobStatus.Conflicts : MergeJobStatus.Finished;
    }

    public void PostComment(
      IVssRequestContext requestContext,
      JObject authentication,
      string repositoryOwner,
      string repositoryName,
      string issueNumber,
      string message)
    {
      GitHubAuthentication gitHubAuthentication;
      this.CreateGitHubHttpClient(requestContext, authentication, out gitHubAuthentication).PostComment((string) null, gitHubAuthentication, repositoryOwner, repositoryName, issueNumber, message);
    }

    public bool DoesUserHaveWritePermissions(
      IVssRequestContext requestContext,
      JObject authentication,
      string userAssociation,
      string repositoryId,
      string userlogin)
    {
      return string.Equals(userAssociation, "OWNER", StringComparison.Ordinal) || this.HasRepoWriteAccess(requestContext, authentication, repositoryId, userlogin);
    }

    private bool HasRepoWriteAccess(
      IVssRequestContext requestContext,
      JObject authentication,
      string repositoryId,
      string userlogin)
    {
      GitHubAuthentication gitHubAuthentication;
      GitHubResult<GitHubData.V3.UserPermission> collaboratorPermissions = this.CreateGitHubHttpClient(requestContext, authentication, out gitHubAuthentication).GetCollaboratorPermissions(gitHubAuthentication, repositoryId, userlogin);
      if (!collaboratorPermissions.IsSuccessful || collaboratorPermissions.Result == null)
      {
        requestContext.TraceError(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HasMemberWriteAccess, nameof (GitHubPullRequestProvider), "Failed to query for collaborator permissions for /repos/" + repositoryId + "/collaborators/" + userlogin + "/permission");
        return false;
      }
      string permission = collaboratorPermissions.Result.Permission;
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HasMemberWriteAccess, nameof (GitHubPullRequestProvider), "For user {0} with repo {2} the permission results are: {3}", (object) userlogin, (object) repositoryId, (object) permission);
      return string.Equals(permission, "admin", StringComparison.Ordinal) || string.Equals(permission, "write", StringComparison.Ordinal);
    }

    private GitHubHttpClient CreateGitHubHttpClient(
      IVssRequestContext requestContext,
      JObject authentication,
      out GitHubAuthentication gitHubAuthentication)
    {
      ArgumentUtility.CheckForNull<JObject>(authentication, nameof (authentication));
      string installationId;
      GitHubHelper.GetAuthenticationElements(authentication, out installationId);
      gitHubAuthentication = new GitHubAuthentication(int.Parse(installationId));
      return GitHubHttpClientFactory.Create(requestContext);
    }
  }
}
