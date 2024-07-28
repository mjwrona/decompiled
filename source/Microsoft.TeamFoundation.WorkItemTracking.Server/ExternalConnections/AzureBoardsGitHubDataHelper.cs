// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.AzureBoardsGitHubDataHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.ServiceModels;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.Models.Event;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  public class AzureBoardsGitHubDataHelper
  {
    private const string c_alias_commit = "__commit";
    private const string c_alias_pullrequest = "__pullrequest";
    private const string c_alias_issue = "__issue";
    private const string c_alias_repository = "__repository";
    private const string c_fragmentargumentname_prcommits = "_PRCOMMITS_";
    private const string c_fragmentargumentname_mergeRequirements = "_MERGEREQUIREMENTS_";
    private const string c_fragment_repository = "fragment repository on Repository { __typename id name nameWithOwner isPrivate url defaultBranchRef { id name } }";
    private const string c_fragment_organization = "fragment organization on Organization { __typename id login name avatarUrl}";
    private const string c_fragment_bot = "fragment bot on Bot { __typename id login avatarUrl}";
    private const string c_fragment_user = "fragment user on User { __typename id login name avatarUrl}";
    private const string c_fragment_userConnection = "fragment userConnection on UserConnection { edges { node { ...user } } }";
    private const string c_fragment_pullRequest = "fragment pullRequest on PullRequest { __typename id number title body state createdAt updatedAt mergedAt closedAt baseRefName url repository { ...repository } author { ...user ...bot ...organization } comments { totalCount } assignees(first: 100) { ...userConnection } }";
    private const string c_fragment_pullRequestExtendedWith2Options = "fragment pullRequest on PullRequest { __typename id number title body state createdAt updatedAt mergedAt closedAt baseRefName url isDraft reviewDecision repository { ...repository } author { ...user ...bot ...organization } comments { totalCount } assignees(first: 100) { ...userConnection } _PRCOMMITS_ _MERGEREQUIREMENTS_}";
    private const string c_fragment_commit = "fragment commit on Commit { __typename id oid message status { __typename state } committedDate pushedDate url repository { ...repository } author { __typename name email avatarUrl user { ...user } } }";
    private const string c_fragment_issue = "fragment issue on Issue { __typename id number title body state createdAt updatedAt closedAt url repository { ...repository } author { ...user ...bot ...organization } comments { totalCount } assignees(first: 100) { ...userConnection } }";
    private const string c_fragment_commitComment = "fragment commitComment on CommitComment { __typename id databaseId body createdAt updatedAt url repository { ...repository } }";
    private const string c_fragment_checkrun = "fragment checkrun on CheckRun { __typename startedAt completedAt id name status conclusion checkSuite { workflowRun { workflow { name state id } } } }";
    private const string c_fragment_prcommit = "fragment prcommits on PullRequestCommitConnection { totalCount nodes { commit { abbreviatedOid committedDate message statusCheckRollup { state contexts(last: 100) { totalCount nodes { ...checkrun } } } } } }";
    private const string c_fragment_prcommitEnable = "commits(last: 1) { ...prcommits }";
    private const string c_fragment_mergeRequirements = "fragment mergeRequirements on PullRequestMergeRequirements { state commitAuthor conditions { __typename displayName description message result ... on PullRequestRulesCondition { ruleRollups { ruleType displayName message result } } } }";
    private const string c_fragment_mergeRequirementsEnable = "mergeRequirements { ...mergeRequirements }";
    private const string c_base64Encoding = "base64";
    private const string c_ghostUserId = "MDQ6VXNlcjEwMTM3";
    private const string c_ghostUserLogin = "ghost";
    private const string c_ghostUserName = "Deleted user";
    private const string c_ghostUserAvatarUrl = "https://avatars3.githubusercontent.com/u/10137?v=4";
    private static readonly ExternalGitUser s_ghostUser = new ExternalGitUser()
    {
      AvatarUrl = "https://avatars3.githubusercontent.com/u/10137?v=4"
    }.SetNodeId<ExternalGitUser>("MDQ6VXNlcjEwMTM3").SetName("Deleted user").SetLogin("ghost");
    private IVssRequestContext m_requestContext;

    protected AzureBoardsGitHubDataHelper()
    {
    }

    public static AzureBoardsGitHubDataHelper Create(IVssRequestContext requestContext) => new AzureBoardsGitHubDataHelper()
    {
      GitHubHttpClient = AzureBoardsGitHubUtilities.CreateGitHubHttpClient(requestContext),
      m_requestContext = requestContext
    };

    public GitHubHttpClient GitHubHttpClient { get; private set; }

    public virtual GitHubResult<IEnumerable<GitHubLinkItem>> GetLinkItems(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      IList<GitHubLinkItemIdentifier> identifiers)
    {
      return this.GetLinkItemsInternal(enterpriseUrl, authentication, identifiers, false, false);
    }

    public virtual GitHubResult<IEnumerable<GitHubLinkItem>> GetLinkItemsAdvanced(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      IList<GitHubLinkItemIdentifier> identifiers,
      bool requestPrCheckRunInfo)
    {
      return this.GetLinkItemsInternal(enterpriseUrl, authentication, identifiers, true, requestPrCheckRunInfo);
    }

    private GitHubResult<IEnumerable<GitHubLinkItem>> GetLinkItemsInternal(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      IList<GitHubLinkItemIdentifier> identifiers,
      bool requestPrAdditionalInformation,
      bool requestPrCheckRunInfo)
    {
      ArgumentUtility.CheckForNull<GitHubAuthentication>(authentication, nameof (authentication));
      ArgumentUtility.CheckEnumerableForEmpty((IEnumerable) identifiers, nameof (identifiers));
      (string query, Dictionary<string, object> variables) getLinkItemsQuery = AzureBoardsGitHubDataHelper.GenerateGetLinkItemsQuery(AzureBoardsGitHubDataHelper.GetNumberOrShaWithRepoExternalIdFromIdentifiers(identifiers), requestPrAdditionalInformation, requestPrCheckRunInfo);
      GitHubResult<JObject> gitHubResult = this.GitHubHttpClient.QueryGraphQL(authentication, enterpriseUrl, getLinkItemsQuery.query, (IDictionary<string, object>) getLinkItemsQuery.variables, nameof (GetLinkItemsInternal));
      if (!gitHubResult.IsSuccessful)
        this.m_requestContext.Trace(this.TracePointStart + 1, TraceLevel.Error, this.Area, this.Layer, "GetLinkItems encountered graphQL error '" + JsonConvert.SerializeObject((object) gitHubResult.Errors) + "'.\r\nQuery:'" + getLinkItemsQuery.query + "', Variables:'" + JsonConvert.SerializeObject((object) getLinkItemsQuery.variables) + "'.");
      return new GitHubResult<IEnumerable<GitHubLinkItem>>(gitHubResult.Result == null ? (IEnumerable<GitHubLinkItem>) null : (IEnumerable<GitHubLinkItem>) AzureBoardsGitHubDataHelper.ParseGetLinkItemsQueryResults(gitHubResult.Result, requestPrAdditionalInformation), gitHubResult.Errors, gitHubResult.StatusCode);
    }

    protected static IEnumerable<(string RepoExternalId, string NumberOrSHA, GitHubLinkItemType ItemType)> GetNumberOrShaWithRepoExternalIdFromIdentifiers(
      IList<GitHubLinkItemIdentifier> identifiers)
    {
      return identifiers.Where<GitHubLinkItemIdentifier>((Func<GitHubLinkItemIdentifier, bool>) (n => (n.ItemType == GitHubLinkItemType.Commit || n.ItemType == GitHubLinkItemType.PullRequest || n.ItemType == GitHubLinkItemType.Issue) && !string.IsNullOrEmpty(n.NumberOrSHA) && !string.IsNullOrEmpty(n.RepoExternalId))).Select<GitHubLinkItemIdentifier, (string, string, GitHubLinkItemType)>((Func<GitHubLinkItemIdentifier, (string, string, GitHubLinkItemType)>) (i => (i.RepoExternalId, i.NumberOrSHA, i.ItemType)));
    }

    public virtual GitHubResult<ChangedGitHubItems> GetChangedGitHubItems(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repoNodeId,
      string repoNameWithOwner,
      GitHubRepoData repoData)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(repoNodeId, nameof (repoNodeId));
      List<GitHubError> gitHubErrorList = new List<GitHubError>();
      HttpStatusCode httpStatusCode = HttpStatusCode.OK;
      string changedItemsQuery = AzureBoardsGitHubDataHelper.GenerateGetChangedItemsQuery(repoNodeId, repoData.PullRequestCursor, repoData.IssueCursor, repoData.CommitCommentCursor);
      GitHubResult<JObject> gitHubResult = this.GitHubHttpClient.QueryGraphQL(authentication, enterpriseUrl, changedItemsQuery, entryMethodName: nameof (GetChangedGitHubItems));
      GitHubResult<GitHubData.V3.Comment[]> issueComments = this.GitHubHttpClient.GetIssueComments(enterpriseUrl, authentication, repoNameWithOwner, repoData.IssueCommentLatestUpdate ?? DateTime.UtcNow.AddHours(-2.0).ToString("s") + "Z", "updated", "asc");
      if (!gitHubResult.IsSuccessful && !issueComments.IsSuccessful)
        httpStatusCode = gitHubResult.StatusCode;
      if (!gitHubResult.IsSuccessful || !issueComments.IsSuccessful)
        this.m_requestContext.Trace(this.TracePointStart + 2, TraceLevel.Error, this.Area, this.Layer, "GetChangedGitHubItems encountered graphQL error '" + JsonConvert.SerializeObject((object) gitHubResult.Errors) + "'.\r\nQuery:'" + changedItemsQuery + "''.GetChangedGitHubItems encountered REST error '" + JsonConvert.SerializeObject((object) issueComments.Errors) + "'.\r\n");
      gitHubErrorList.AddRangeIfRangeNotNull<GitHubError, List<GitHubError>>((IEnumerable<GitHubError>) issueComments.Errors);
      gitHubErrorList.AddRangeIfRangeNotNull<GitHubError, List<GitHubError>>((IEnumerable<GitHubError>) gitHubResult.Errors);
      ChangedGitHubItems result = (ChangedGitHubItems) null;
      if (gitHubResult.IsSuccessful)
      {
        if (gitHubResult.Result != null)
        {
          try
          {
            result = AzureBoardsGitHubDataHelper.ParseGetChangedGitHubItemsResult(gitHubResult.Result);
            if (issueComments.IsSuccessful)
            {
              if (issueComments.Result != null)
              {
                bool isIsBotFixEnabled = WorkItemTrackingFeatureFlags.IsIsBotFixEnabled(this.m_requestContext);
                IEnumerable<ExternalPullRequestCommentEvent> source = ((IEnumerable<GitHubData.V3.Comment>) issueComments.Result).Select<GitHubData.V3.Comment, ExternalPullRequestCommentEvent>((Func<GitHubData.V3.Comment, ExternalPullRequestCommentEvent>) (issueComment =>
                {
                  ExternalPullRequestCommentEvent changedGitHubItems = new ExternalPullRequestCommentEvent();
                  changedGitHubItems.Id = issueComment.Id.ToString();
                  changedGitHubItems.CommentBody = issueComment.Body;
                  changedGitHubItems.UpdatedAt = issueComment.Updated_at;
                  ExternalGitUser externalGitUser;
                  if (!isIsBotFixEnabled)
                  {
                    externalGitUser = (ExternalGitUser) null;
                  }
                  else
                  {
                    externalGitUser = new ExternalGitUser();
                    externalGitUser.Name = issueComment.User.Name;
                    externalGitUser.Email = issueComment.User.Email;
                    externalGitUser.AvatarUrl = issueComment.User.Avatar_url;
                    externalGitUser.Type = issueComment.User.Type;
                  }
                  changedGitHubItems.Sender = externalGitUser;
                  return changedGitHubItems;
                })).Where<ExternalPullRequestCommentEvent>((Func<ExternalPullRequestCommentEvent, bool>) (issueComment => !(issueComment.Id == repoData.IssueCommentLatestId) || !(issueComment.UpdatedAt == repoData.IssueCommentLatestUpdate)));
                result.IssueComments = new IssueCommentPageableResult()
                {
                  LatestUpdate = source.LastOrDefault<ExternalPullRequestCommentEvent>()?.UpdatedAt,
                  LatestId = source.LastOrDefault<ExternalPullRequestCommentEvent>()?.Id,
                  Results = source
                };
              }
            }
          }
          catch (Exception ex)
          {
            this.m_requestContext.Trace(this.TracePointStart + 4, TraceLevel.Error, this.Area, this.Layer, string.Format("GetChangedGitHubItems encountered parse exception '{0}'.\r\n", (object) ex) + "Query:'" + changedItemsQuery + "''," + string.Format("Response: '{0}''.", (object) gitHubResult.Result));
            throw;
          }
          if (!string.IsNullOrEmpty(repoData.CurrentHead) && !string.Equals(result.Commits.CurrentHead, repoData.CurrentHead, StringComparison.OrdinalIgnoreCase))
          {
            GitHubResult<IEnumerable<ExternalGitCommit>> diffBetweenCommits = this.GetDiffBetweenCommits(enterpriseUrl, authentication, result.Repository, repoData.CurrentHead, result.Commits.CurrentHead);
            gitHubErrorList.AddRangeIfRangeNotNull<GitHubError, List<GitHubError>>((IEnumerable<GitHubError>) diffBetweenCommits.Errors);
            if (diffBetweenCommits.IsSuccessful)
            {
              result.Commits.Results = diffBetweenCommits.Result;
            }
            else
            {
              httpStatusCode = diffBetweenCommits.StatusCode;
              this.m_requestContext.Trace(this.TracePointStart + 7, TraceLevel.Error, this.Area, this.Layer, "GetChangedGitHubItems encountered REST error on GetDiffBetweenCommits - '" + JsonConvert.SerializeObject((object) diffBetweenCommits.Errors) + "'.\r\n");
            }
          }
        }
      }
      return new GitHubResult<ChangedGitHubItems>(result, (IReadOnlyList<GitHubError>) gitHubErrorList, httpStatusCode);
    }

    public virtual GitHubResult<ExternalArtifactCollection> GetRepoArtifacts(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repoNodeId,
      IEnumerable<PendingExternalArtifactIdentifier> artifactIdentifiers)
    {
      (string query, Dictionary<string, object> variables) getLinkItemsQuery = AzureBoardsGitHubDataHelper.GenerateGetLinkItemsQuery(artifactIdentifiers.Select<PendingExternalArtifactIdentifier, (string, string, GitHubLinkItemType)>((Func<PendingExternalArtifactIdentifier, (string, string, GitHubLinkItemType)>) (identifier => (repoNodeId, identifier.ArtifactId, identifier.ArtifactType))), false);
      GitHubResult<JObject> gitHubResult = this.GitHubHttpClient.QueryGraphQL(authentication, enterpriseUrl, getLinkItemsQuery.query, (IDictionary<string, object>) getLinkItemsQuery.variables, nameof (GetRepoArtifacts));
      if (!gitHubResult.IsSuccessful)
        this.m_requestContext.Trace(this.TracePointStart + 1, TraceLevel.Error, this.Area, this.Layer, "GetRepoArtifacts encountered graphQL error '" + JsonConvert.SerializeObject((object) gitHubResult.Errors) + "'.\r\nQuery:'" + getLinkItemsQuery.query + "', Variables:'" + JsonConvert.SerializeObject((object) getLinkItemsQuery.variables) + "'.");
      return new GitHubResult<ExternalArtifactCollection>(gitHubResult.Result == null ? (ExternalArtifactCollection) null : AzureBoardsGitHubDataHelper.ParseGetRepoArtifactsQueryResults(gitHubResult.Result), gitHubResult.Errors, gitHubResult.StatusCode);
    }

    public virtual GitHubData.V3.InstallationDetails FindUserInstallationDetails(
      IVssRequestContext requestContext,
      Guid projectId,
      GitHubAuthentication authentication,
      string installationId)
    {
      GitHubResult<GitHubData.V3.UserInstallationDetails> installationDetails = this.GitHubHttpClient.GetUserInstallationDetails(authentication);
      if (installationDetails.IsSuccessful)
      {
        GitHubData.V3.UserInstallationDetails result = installationDetails.Result;
        if ((result != null ? (result.Total_count > 0 ? 1 : 0) : 0) != 0)
          return ((IEnumerable<GitHubData.V3.InstallationDetails>) installationDetails.Result.Installations).FirstOrDefault<GitHubData.V3.InstallationDetails>((Func<GitHubData.V3.InstallationDetails, bool>) (i => i.Id == installationId));
      }
      requestContext.Trace(this.TracePointStart + 6, TraceLevel.Info, this.Area, this.Layer, "User does not have access to the installation. " + installationDetails.ErrorMessage);
      return (GitHubData.V3.InstallationDetails) null;
    }

    public virtual ExternalGitRepo GetUserRepository(
      IVssRequestContext requestContext,
      Guid projectId,
      GitHubAuthentication authentication,
      string repoWithOwnerName,
      string enterpriseUrl)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<string>(repoWithOwnerName, nameof (repoWithOwnerName));
      ArgumentUtility.CheckForNull<GitHubAuthentication>(authentication, nameof (authentication));
      GitHubResult<GitHubData.V3.Repository> userRepo = this.GitHubHttpClient.GetUserRepo(enterpriseUrl, authentication, repoWithOwnerName);
      if (userRepo.IsSuccessful)
        return AzureBoardsGitHubDataHelper.ConvertGitHubV3RepositoryToExternalGitRepo(userRepo?.Result);
      requestContext.Trace(this.TracePointStart + 5, TraceLevel.Info, this.Area, this.Layer, "No repositories were found. Error: {0}", (object) userRepo?.ErrorMessage);
      return (ExternalGitRepo) null;
    }

    public ExternalGitRepo[] GetRepositories(
      IVssRequestContext requestContext,
      GitHubAuthentication authentication,
      string enterpriseUrl)
    {
      GitHubResult<GitHubData.V3.Repository[]> gitHubResult = authentication.Scheme != GitHubAuthScheme.InstallationToken ? (authentication.Scheme != GitHubAuthScheme.ApplicationOAuthToken ? this.GitHubHttpClient.GetUserRepos(enterpriseUrl, authentication) : this.GitHubHttpClient.GetUserInstallationRepositories(authentication, authentication.InstallationId.ToString())) : this.GitHubHttpClient.GetInstallationRepos(authentication);
      return gitHubResult.IsSuccessful ? AzureBoardsGitHubDataHelper.ConvertGitHubV3RepositoriesToExternalGitRepoArray((IEnumerable<GitHubData.V3.Repository>) gitHubResult.Result) : throw new GitHubCannotGetRepositoriesException(gitHubResult.ErrorMessage);
    }

    public GitHubData.V3.OrgMembership[] GetUserOrganizationMemberships(
      GitHubAuthentication authentication)
    {
      GitHubResult<GitHubData.V3.OrgMembership[]> gitHubResult = authentication.Scheme == GitHubAuthScheme.Token ? this.GitHubHttpClient.GetUserOrgMembership(authentication) : throw new InvalidAuthorizationSchemeException();
      return gitHubResult.IsSuccessful ? gitHubResult.Result : throw new GitHubCannotGetUserOrgsException(gitHubResult.ErrorMessage);
    }

    public ExternalGitRepo[] GetPagedReposByOrgOrUser(
      GitHubAuthentication authentication,
      string enterpriseUrl,
      string orgOrUserName,
      bool isUserType,
      string continuationToken,
      out string nextToken,
      out int pageLength,
      out int totalPageCount)
    {
      GitHubResult<GitHubData.V3.Repository[]> gitHubResult = !isUserType ? this.GitHubHttpClient.GetPagedOrgRepos(enterpriseUrl, orgOrUserName, authentication, continuationToken, out nextToken, out pageLength, out totalPageCount) : this.GitHubHttpClient.GetPagedUserRepos(enterpriseUrl, authentication, continuationToken, out nextToken, out pageLength, out totalPageCount, true);
      return gitHubResult.IsSuccessful ? AzureBoardsGitHubDataHelper.ConvertGitHubV3RepositoriesToExternalGitRepoArray((IEnumerable<GitHubData.V3.Repository>) gitHubResult.Result) : throw new GitHubCannotGetRepositoriesException(gitHubResult.ErrorMessage);
    }

    public virtual ExternalGitRepo[] GetTopRepositoriesWithUserAdminPermission(
      GitHubAuthentication authentication,
      string enterpriseUrl)
    {
      GitHubResult<GitHubData.V3.Repository[]> gitHubResult = authentication.Scheme != GitHubAuthScheme.ApplicationOAuthToken ? this.GitHubHttpClient.GetTopUserRepos(enterpriseUrl, authentication) : this.GitHubHttpClient.GetUserInstallationRepositories(authentication, authentication.InstallationId.ToString());
      return gitHubResult.IsSuccessful ? AzureBoardsGitHubDataHelper.ConvertGitHubV3RepositoriesToExternalGitRepoArray((IEnumerable<GitHubData.V3.Repository>) this.FilterRepositoriesHavingAdminPermissions(gitHubResult.Result)) : throw new GitHubCannotGetRepositoriesException(gitHubResult.ErrorMessage);
    }

    public ExternalGitRepo[] GetRepositoriesWithUserAdminPermissionByPage(
      GitHubAuthentication authentication,
      string enterpriseUrl,
      string continuationToken,
      out string nextToken,
      out int pageLength,
      out int totalPageCount)
    {
      GitHubResult<GitHubData.V3.Repository[]> gitHubResult = authentication.Scheme != GitHubAuthScheme.ApplicationOAuthToken ? this.GitHubHttpClient.GetPagedUserRepos(enterpriseUrl, authentication, continuationToken, out nextToken, out pageLength, out totalPageCount) : this.GitHubHttpClient.GetPagedUserInstallationRepos(authentication, authentication.InstallationId.ToString(), continuationToken, out nextToken, out pageLength, out totalPageCount);
      return gitHubResult.IsSuccessful ? AzureBoardsGitHubDataHelper.ConvertGitHubV3RepositoriesToExternalGitRepoArray((IEnumerable<GitHubData.V3.Repository>) this.FilterRepositoriesHavingAdminPermissions(gitHubResult.Result)) : throw new GitHubCannotGetRepositoriesException(gitHubResult.ErrorMessage);
    }

    public virtual string GetDefaultBranch(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repoWithOwnerName)
    {
      ArgumentUtility.CheckForNull<GitHubAuthentication>(authentication, nameof (authentication));
      ArgumentUtility.CheckStringForNullOrEmpty(repoWithOwnerName, nameof (repoWithOwnerName));
      GitHubResult<GitHubData.V3.Repository> repo = this.GitHubHttpClient.GetRepo(enterpriseUrl, authentication, repoWithOwnerName);
      if (!repo.IsSuccessful)
        throw new GitHubCannotGetDefaultBranchException(repoWithOwnerName, repo.ErrorMessage);
      return repo.Result.Default_branch;
    }

    public virtual GitHubData.V3.Ref CreateBranchRef(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repoWithOwnerName,
      string newBranch,
      string baseBranch)
    {
      ArgumentUtility.CheckForNull<GitHubAuthentication>(authentication, nameof (authentication));
      ArgumentUtility.CheckStringForNullOrEmpty(repoWithOwnerName, nameof (repoWithOwnerName));
      ArgumentUtility.CheckStringForNullOrEmpty(newBranch, nameof (newBranch));
      ArgumentUtility.CheckStringForNullOrEmpty(baseBranch, nameof (baseBranch));
      GitHubResult<GitHubData.V3.Ref> branchRef1 = this.GitHubHttpClient.GetBranchRef(enterpriseUrl, authentication, repoWithOwnerName, baseBranch);
      if (!branchRef1.IsSuccessful)
        throw new GitHubCannotGetBranchRefException(baseBranch, repoWithOwnerName, branchRef1.ErrorMessage);
      GitHubResult<GitHubData.V3.Ref> branchRef2 = this.GitHubHttpClient.CreateBranchRef(enterpriseUrl, authentication, repoWithOwnerName, newBranch, branchRef1.Result.Object.Sha);
      return branchRef2.IsSuccessful ? branchRef2.Result : throw new GitHubCannotCreateBranchRefException(newBranch, repoWithOwnerName, baseBranch, branchRef2.ErrorMessage);
    }

    public virtual GitHubFileContentData GetReadmeFileContentData(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repoWithOwnerName,
      string branch = null)
    {
      ArgumentUtility.CheckForNull<GitHubAuthentication>(authentication, nameof (authentication));
      ArgumentUtility.CheckStringForNullOrEmpty(repoWithOwnerName, nameof (repoWithOwnerName));
      GitHubResult<GitHubData.V3.ContentData> readmeFileContentData = this.GitHubHttpClient.GetReadmeFileContentData(enterpriseUrl, authentication, repoWithOwnerName, branch);
      if (readmeFileContentData.StatusCode == HttpStatusCode.NotFound)
        throw new FileNotFoundException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.GetGitHubReadmeFileContentData_FileNotFound((object) repoWithOwnerName, (object) (branch ?? string.Empty)));
      if (!readmeFileContentData.IsSuccessful)
        throw new GitHubCannotGetReadmeFileContentDataException(repoWithOwnerName, branch ?? string.Empty, readmeFileContentData.ErrorMessage);
      GitHubResult<GitHubData.V3.BlobObject> blob = this.GitHubHttpClient.GetBlob(enterpriseUrl, authentication, repoWithOwnerName, readmeFileContentData.Result.Sha);
      if (!blob.IsSuccessful)
        throw new GitHubCannotGetReadmeFileContentDataException(repoWithOwnerName, branch ?? string.Empty, readmeFileContentData.ErrorMessage);
      string s = blob.Result.Content ?? string.Empty;
      if (!string.IsNullOrEmpty(s) && string.Equals(blob.Result.Encoding, "base64", StringComparison.OrdinalIgnoreCase))
        s = Encoding.UTF8.GetString(Convert.FromBase64String(s));
      return new GitHubFileContentData()
      {
        Name = readmeFileContentData.Result.Name,
        Path = readmeFileContentData.Result.Path,
        Sha = readmeFileContentData.Result.Sha,
        Content = s
      };
    }

    public virtual GitHubFileContentData CreateFileContent(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repoWithOwnerName,
      string filePath,
      string fileContent,
      string message,
      string branch)
    {
      ArgumentUtility.CheckForNull<GitHubAuthentication>(authentication, nameof (authentication));
      ArgumentUtility.CheckStringForNullOrEmpty(repoWithOwnerName, nameof (repoWithOwnerName));
      ArgumentUtility.CheckStringForNullOrEmpty(filePath, nameof (filePath));
      ArgumentUtility.CheckStringForNullOrEmpty(fileContent, nameof (fileContent));
      ArgumentUtility.CheckStringForNullOrEmpty(message, nameof (message));
      ArgumentUtility.CheckStringForNullOrEmpty(branch, nameof (branch));
      GitHubResult<GitHubData.V3.ContentResponseData> fileContent1 = this.GitHubHttpClient.CreateFileContent(enterpriseUrl, authentication, repoWithOwnerName, filePath, branch, fileContent, message);
      if (!fileContent1.IsSuccessful)
        throw new GitHubCannotCreateFileContentException(repoWithOwnerName, branch, filePath, fileContent1.ErrorMessage);
      return new GitHubFileContentData()
      {
        Name = fileContent1.Result.Content?.Name,
        Path = fileContent1.Result.Content?.Path,
        Sha = fileContent1.Result.Content?.Sha
      };
    }

    public virtual GitHubFileContentData UpdateFileContent(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repoWithOwnerName,
      string filePath,
      string fileSha,
      string fileContent,
      string message,
      string branch)
    {
      ArgumentUtility.CheckForNull<GitHubAuthentication>(authentication, nameof (authentication));
      ArgumentUtility.CheckStringForNullOrEmpty(repoWithOwnerName, nameof (repoWithOwnerName));
      ArgumentUtility.CheckStringForNullOrEmpty(filePath, nameof (filePath));
      ArgumentUtility.CheckStringForNullOrEmpty(fileSha, nameof (fileSha));
      ArgumentUtility.CheckStringForNullOrEmpty(fileContent, nameof (fileContent));
      ArgumentUtility.CheckStringForNullOrEmpty(message, nameof (message));
      ArgumentUtility.CheckStringForNullOrEmpty(branch, nameof (branch));
      GitHubResult<GitHubData.V3.ContentResponseData> gitHubResult = this.GitHubHttpClient.UpdateFileContent(enterpriseUrl, authentication, repoWithOwnerName, filePath, fileSha, branch, fileContent, message);
      if (!gitHubResult.IsSuccessful)
        throw new GitHubCannotUpdateFileContentException(repoWithOwnerName, branch, filePath, gitHubResult.ErrorMessage);
      return new GitHubFileContentData()
      {
        Name = gitHubResult.Result.Content?.Name,
        Path = gitHubResult.Result.Content?.Path,
        Sha = gitHubResult.Result.Content?.Sha
      };
    }

    public virtual GitHubData.V3.PullRequest CreatePullRequest(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repoWithOwnerName,
      string title,
      string description,
      string sourceBranch,
      string targetBranch)
    {
      ArgumentUtility.CheckForNull<GitHubAuthentication>(authentication, nameof (authentication));
      ArgumentUtility.CheckStringForNullOrEmpty(repoWithOwnerName, nameof (repoWithOwnerName));
      ArgumentUtility.CheckStringForNullOrEmpty(title, nameof (title));
      ArgumentUtility.CheckStringForNullOrEmpty(sourceBranch, nameof (sourceBranch));
      ArgumentUtility.CheckStringForNullOrEmpty(targetBranch, nameof (targetBranch));
      GitHubResult<GitHubData.V3.PullRequest> pullRequest = this.GitHubHttpClient.CreatePullRequest(enterpriseUrl, authentication, repoWithOwnerName, targetBranch, sourceBranch, title, description);
      return pullRequest.IsSuccessful ? pullRequest.Result : throw new GitHubCannotCreatePullRequestException(sourceBranch, targetBranch, repoWithOwnerName, pullRequest.ErrorMessage);
    }

    public IEnumerable<ExternalGitPullRequest> GetPagedPullRequestsByRepository(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      string filter,
      int limit,
      string continuationToken,
      out string nextContinuationToken,
      out ExternalRateLimit rateLimit)
    {
      GitHubResult<GitHubData.V4.PullRequest[]> requestsPaginated = this.GitHubHttpClient.GetPullRequestsPaginated(enterpriseUrl, authentication, repository, filter, limit, continuationToken);
      if (!requestsPaginated.IsSuccessful)
        throw new GitHubCannotGetPullRequestsException(requestsPaginated.ErrorMessage);
      nextContinuationToken = requestsPaginated.PageInfo.HasNextPage ? requestsPaginated.PageInfo.EndCursor : (string) null;
      rateLimit = AzureBoardsGitHubDataHelper.ConvertGitHubRateLimitToExternalRateLimit(requestsPaginated.RateLimit);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return ((IEnumerable<GitHubData.V4.PullRequest>) requestsPaginated.Result).Select<GitHubData.V4.PullRequest, ExternalGitPullRequest>(AzureBoardsGitHubDataHelper.\u003C\u003EO.\u003C0\u003E__ConvertGitHubPullRequestToExternalGitPullRequest ?? (AzureBoardsGitHubDataHelper.\u003C\u003EO.\u003C0\u003E__ConvertGitHubPullRequestToExternalGitPullRequest = new Func<GitHubData.V4.PullRequest, ExternalGitPullRequest>(AzureBoardsGitHubDataHelper.ConvertGitHubPullRequestToExternalGitPullRequest)));
    }

    public IEnumerable<ExternalGitCommit> GetPagedCommitsByRepository(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      string filter,
      int limit,
      string continuationToken,
      out string nextContinuationToken,
      out ExternalRateLimit rateLimit)
    {
      if (string.IsNullOrWhiteSpace(filter))
      {
        GitHubResult<GitHubData.V4.Commit[]> commitsPaginated = this.GitHubHttpClient.GetCommitsPaginated(enterpriseUrl, authentication, repository, limit, continuationToken);
        if (!commitsPaginated.IsSuccessful)
          throw new GitHubCannotGetCommitsException(commitsPaginated.ErrorMessage);
        nextContinuationToken = commitsPaginated.PageInfo == null || !commitsPaginated.PageInfo.HasNextPage ? (string) null : commitsPaginated.PageInfo.EndCursor;
        rateLimit = AzureBoardsGitHubDataHelper.ConvertGitHubRateLimitToExternalRateLimit(commitsPaginated.RateLimit);
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return ((IEnumerable<GitHubData.V4.Commit>) commitsPaginated.Result).Select<GitHubData.V4.Commit, ExternalGitCommit>(AzureBoardsGitHubDataHelper.\u003C\u003EO.\u003C1\u003E__ConvertGitHubCommitToExternalGitCommit ?? (AzureBoardsGitHubDataHelper.\u003C\u003EO.\u003C1\u003E__ConvertGitHubCommitToExternalGitCommit = new Func<GitHubData.V4.Commit, ExternalGitCommit>(AzureBoardsGitHubDataHelper.ConvertGitHubCommitToExternalGitCommit)));
      }
      GitHubResult<GitHubData.V3.CommitListItem[]> commitsPaginated1 = this.GitHubHttpClient.GetCommitsPaginated(enterpriseUrl, authentication, repository, filter, limit, continuationToken, out nextContinuationToken);
      rateLimit = commitsPaginated1.IsSuccessful ? AzureBoardsGitHubDataHelper.ConvertGitHubRateLimitToExternalRateLimit(commitsPaginated1.RateLimit) : throw new GitHubCannotGetCommitsException(commitsPaginated1.ErrorMessage);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return ((IEnumerable<GitHubData.V3.CommitListItem>) commitsPaginated1.Result).Select<GitHubData.V3.CommitListItem, ExternalGitCommit>(AzureBoardsGitHubDataHelper.\u003C\u003EO.\u003C2\u003E__ConvertGitHubCommitToExternalGitCommit ?? (AzureBoardsGitHubDataHelper.\u003C\u003EO.\u003C2\u003E__ConvertGitHubCommitToExternalGitCommit = new Func<GitHubData.V3.CommitListItem, ExternalGitCommit>(AzureBoardsGitHubDataHelper.ConvertGitHubCommitToExternalGitCommit)));
    }

    protected GitHubResult<IEnumerable<ExternalGitCommit>> GetDiffBetweenCommits(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      ExternalGitRepo repository,
      string startSha,
      string endSha)
    {
      GitHubResult<GitHubData.V3.CommitsDiff> commitsDiff = this.GitHubHttpClient.GetCommitsDiff(enterpriseUrl, authentication, repository.RepoNameWithOwner(), startSha, endSha);
      IEnumerable<ExternalGitCommit> result = (IEnumerable<ExternalGitCommit>) null;
      IReadOnlyList<GitHubError> errors = commitsDiff.Errors;
      HttpStatusCode httpStatusCode = commitsDiff.StatusCode;
      if (commitsDiff.IsSuccessful)
      {
        result = (IEnumerable<ExternalGitCommit>) ((IEnumerable<GitHubData.V3.CommitListItem>) commitsDiff.Result.Commits).Select<GitHubData.V3.CommitListItem, ExternalGitCommit>((Func<GitHubData.V3.CommitListItem, ExternalGitCommit>) (c =>
        {
          ExternalGitCommit artifact = new ExternalGitCommit();
          artifact.Sha = c.Sha;
          artifact.CommitedDate = DateTime.Parse(c.Commit.Committer.Date);
          ExternalGitUser externalGitUser;
          if (c.Author != null)
            externalGitUser = new ExternalGitUser()
            {
              Email = c.Commit.Author.Email,
              AvatarUrl = c.Author.Avatar_url
            }.SetNodeId<ExternalGitUser>(c.Author.Node_id).SetName(c.Author.Name ?? c.Commit.Author.Name).SetLogin(c.Author.Login);
          else
            externalGitUser = AzureBoardsGitHubDataHelper.s_ghostUser;
          artifact.Author = externalGitUser;
          artifact.Repo = repository;
          artifact.Message = c.Commit.Message;
          artifact.PushedDate = new DateTime?();
          artifact.WebUrl = c.Html_url;
          return artifact.SetNodeId<ExternalGitCommit>(c.Node_id);
        })).ToList<ExternalGitCommit>();
      }
      else
      {
        if (commitsDiff.ErrorMessage != null && commitsDiff.ErrorMessage.StartsWith("No common ancestor between"))
        {
          errors = (IReadOnlyList<GitHubError>) null;
          httpStatusCode = HttpStatusCode.OK;
        }
        this.m_requestContext.Trace(this.TracePointStart + 3, TraceLevel.Error, this.Area, this.Layer, "GetDiffBetweenCommits encountered an error using start Sha:'" + startSha + "', End Sha:'" + endSha + "':\r\n" + JsonConvert.SerializeObject((object) commitsDiff.Errors));
      }
      return new GitHubResult<IEnumerable<ExternalGitCommit>>(result, errors, httpStatusCode);
    }

    protected static IList<GitHubLinkItem> ParseGetLinkItemsQueryResults(
      JObject data,
      bool hasPrAdditionalInformation)
    {
      List<GitHubLinkItem> itemsQueryResults = new List<GitHubLinkItem>();
      foreach (JProperty property in data.Properties())
      {
        if (property.Name.StartsWith("__repository"))
        {
          GitHubData.V4.Repository repo = property.Value is JObject jobject ? jobject.ToObject<GitHubData.V4.Repository>() : (GitHubData.V4.Repository) null;
          if (repo != null)
          {
            foreach (JToken jtoken1 in (IEnumerable<JToken>) property.Value)
            {
              if (jtoken1 is JProperty jproperty && (jproperty.Name.StartsWith("__pullrequest") || jproperty.Name.StartsWith("__commit") || jproperty.Name.StartsWith("__issue")))
              {
                JToken jtoken2 = jproperty.Value;
                if (jproperty.Value.HasValues)
                {
                  GitHubLinkItemType linkItemType = AzureBoardsGitHubDataHelper.GetLinkItemType(jtoken2);
                  switch (linkItemType)
                  {
                    case GitHubLinkItemType.PullRequest:
                    case GitHubLinkItemType.Commit:
                    case GitHubLinkItemType.Issue:
                      itemsQueryResults.Add(AzureBoardsGitHubDataHelper.ConvertJObjectToGitHubLinkItem(jtoken2, linkItemType, repo, hasPrAdditionalInformation));
                      continue;
                    default:
                      continue;
                  }
                }
              }
            }
          }
        }
      }
      return (IList<GitHubLinkItem>) itemsQueryResults;
    }

    protected static ExternalArtifactCollection ParseGetRepoArtifactsQueryResults(JObject data)
    {
      List<ExternalGitCommit> externalGitCommitList = new List<ExternalGitCommit>();
      List<ExternalGitPullRequest> externalGitPullRequestList = new List<ExternalGitPullRequest>();
      List<ExternalGitIssue> externalGitIssueList = new List<ExternalGitIssue>();
      foreach (JProperty property in data.Properties())
      {
        if (property.Name.StartsWith("__repository") && (property.Value is JObject jobject ? jobject.ToObject<GitHubData.V4.Repository>() : (GitHubData.V4.Repository) null) != null)
        {
          foreach (JToken jtoken in (IEnumerable<JToken>) property.Value)
          {
            if (jtoken is JProperty jproperty && (jproperty.Name.StartsWith("__pullrequest") || jproperty.Name.StartsWith("__commit") || jproperty.Name.StartsWith("__issue")))
            {
              JToken jToken = jproperty.Value;
              if (jproperty.Value.HasValues)
              {
                switch (AzureBoardsGitHubDataHelper.GetLinkItemType(jToken))
                {
                  case GitHubLinkItemType.PullRequest:
                    externalGitPullRequestList.Add(AzureBoardsGitHubDataHelper.ConvertJTokenToExternalGitPullRequest(jToken));
                    continue;
                  case GitHubLinkItemType.Commit:
                    externalGitCommitList.Add(AzureBoardsGitHubDataHelper.ConvertJTokenToExternalGitCommit(jToken));
                    continue;
                  case GitHubLinkItemType.Issue:
                    externalGitIssueList.Add(AzureBoardsGitHubDataHelper.ConvertJTokenToExternalGitIssue(jToken));
                    continue;
                  default:
                    throw new Exception("Unexpected");
                }
              }
            }
          }
        }
      }
      return new ExternalArtifactCollection()
      {
        Commits = (IEnumerable<ExternalGitCommit>) externalGitCommitList,
        PullRequests = (IEnumerable<ExternalGitPullRequest>) externalGitPullRequestList,
        Issues = (IEnumerable<ExternalGitIssue>) externalGitIssueList
      };
    }

    protected static ChangedGitHubItems ParseGetChangedGitHubItemsResult(JObject data)
    {
      ChangedGitHubItems gitHubItemsResult = new ChangedGitHubItems();
      JToken connection1 = data.SelectToken("node");
      JToken jtoken = connection1.SelectToken("defaultBranchRef.target.oid");
      gitHubItemsResult.Commits = new CommitPageableResult()
      {
        CurrentHead = jtoken != null ? jtoken.Value<string>() : (string) null
      };
      JToken connection2 = connection1.SelectToken("pullRequests");
      gitHubItemsResult.PullRequests = new PageableResult<ExternalGitPullRequest>()
      {
        PageInfo = AzureBoardsGitHubDataHelper.GetPageInfoFromConnection(connection2),
        Results = AzureBoardsGitHubDataHelper.GetPullRequests(connection2)
      };
      JToken connection3 = connection1.SelectToken("issues");
      gitHubItemsResult.Issues = new PageableResult<ExternalGitIssue>()
      {
        PageInfo = AzureBoardsGitHubDataHelper.GetPageInfoFromConnection(connection3),
        Results = AzureBoardsGitHubDataHelper.GetIssues(connection3)
      };
      JToken connection4 = connection1.SelectToken("commitComments");
      gitHubItemsResult.CommitComments = new PageableResult<ExternalGitCommitComment>()
      {
        PageInfo = AzureBoardsGitHubDataHelper.GetPageInfoFromConnection(connection4),
        Results = AzureBoardsGitHubDataHelper.GetCommitComments(connection4)
      };
      gitHubItemsResult.Repository = AzureBoardsGitHubDataHelper.GetRepo(connection1);
      return gitHubItemsResult;
    }

    private GitHubData.V3.Repository[] FilterRepositoriesHavingAdminPermissions(
      GitHubData.V3.Repository[] repositories)
    {
      return ((IEnumerable<GitHubData.V3.Repository>) repositories).Where<GitHubData.V3.Repository>((Func<GitHubData.V3.Repository, bool>) (repo => repo.Permissions.Admin)).ToArray<GitHubData.V3.Repository>();
    }

    private static ExternalGitRepo GetRepo(JToken connection)
    {
      GitHubData.V4.Repository repository = connection?.ToObject<GitHubData.V4.Repository>(GitHubHttpClient.GraphQLSerializer);
      return repository != null ? AzureBoardsGitHubDataHelper.ConvertGitHubRepositoryToExternalGitRepo(repository) : (ExternalGitRepo) null;
    }

    private static GitHubData.V4.PageInfo GetPageInfoFromConnection(JToken connection) => connection?.SelectToken("pageInfo").ToObject<GitHubData.V4.PageInfo>();

    private static IEnumerable<ExternalGitPullRequest> GetPullRequests(JToken connection)
    {
      GitHubData.V4.Connection<GitHubData.V4.PullRequest> connection1 = connection?.ToObject<GitHubData.V4.Connection<GitHubData.V4.PullRequest>>(GitHubHttpClient.GraphQLSerializer);
      return connection1 != null ? (IEnumerable<ExternalGitPullRequest>) ((IEnumerable<GitHubData.V4.Edge<GitHubData.V4.PullRequest>>) connection1.Edges).Select<GitHubData.V4.Edge<GitHubData.V4.PullRequest>, ExternalGitPullRequest>((Func<GitHubData.V4.Edge<GitHubData.V4.PullRequest>, ExternalGitPullRequest>) (e => AzureBoardsGitHubDataHelper.ConvertGitHubPullRequestToExternalGitPullRequest(e.Node))).ToList<ExternalGitPullRequest>() : (IEnumerable<ExternalGitPullRequest>) null;
    }

    private static IEnumerable<ExternalGitIssue> GetIssues(JToken connection)
    {
      GitHubData.V4.Connection<GitHubData.V4.Issue> connection1 = connection?.ToObject<GitHubData.V4.Connection<GitHubData.V4.Issue>>(GitHubHttpClient.GraphQLSerializer);
      return connection1 != null ? (IEnumerable<ExternalGitIssue>) ((IEnumerable<GitHubData.V4.Edge<GitHubData.V4.Issue>>) connection1.Edges).Select<GitHubData.V4.Edge<GitHubData.V4.Issue>, ExternalGitIssue>((Func<GitHubData.V4.Edge<GitHubData.V4.Issue>, ExternalGitIssue>) (e => AzureBoardsGitHubDataHelper.ConvertGitHubIssueToExternalGitIssue(e.Node))).ToList<ExternalGitIssue>() : (IEnumerable<ExternalGitIssue>) null;
    }

    private static IEnumerable<ExternalGitCommitComment> GetCommitComments(JToken connection)
    {
      GitHubData.V4.Connection<GitHubData.V4.CommitComment> connection1 = connection?.ToObject<GitHubData.V4.Connection<GitHubData.V4.CommitComment>>(GitHubHttpClient.GraphQLSerializer);
      return connection1 != null ? (IEnumerable<ExternalGitCommitComment>) ((IEnumerable<GitHubData.V4.Edge<GitHubData.V4.CommitComment>>) connection1.Edges).Select<GitHubData.V4.Edge<GitHubData.V4.CommitComment>, ExternalGitCommitComment>((Func<GitHubData.V4.Edge<GitHubData.V4.CommitComment>, ExternalGitCommitComment>) (e => AzureBoardsGitHubDataHelper.ConvertGitHubCommitCommentToExternalGitCommitComment(e.Node))).ToList<ExternalGitCommitComment>() : (IEnumerable<ExternalGitCommitComment>) null;
    }

    private static ExternalGitCommit ConvertGitHubCommitToExternalGitCommit(
      GitHubData.V4.Commit commit)
    {
      GitHubData.V4.GitActor author = commit.Author;
      GitHubData.V4.User user = author?.User;
      ExternalGitCommit artifact = new ExternalGitCommit();
      artifact.Repo = AzureBoardsGitHubDataHelper.ConvertGitHubRepositoryToExternalGitRepo(commit.Repository);
      ExternalGitUser externalGitUser;
      if (user != null)
        externalGitUser = new ExternalGitUser()
        {
          Email = author?.Email,
          AvatarUrl = (user.AvatarUrl?.ToString() ?? author?.AvatarUrl?.ToString())
        }.SetNodeId<ExternalGitUser>(user.Id).SetName(user.Name ?? author?.Name).SetLogin(user.Login);
      else
        externalGitUser = AzureBoardsGitHubDataHelper.s_ghostUser;
      artifact.Author = externalGitUser;
      artifact.CommitedDate = commit.CommittedDate;
      artifact.PushedDate = commit.PushedDate;
      artifact.Message = commit.Message;
      artifact.Sha = commit.Oid;
      artifact.WebUrl = commit.Url?.ToString();
      artifact.SetNodeId<ExternalGitCommit>(commit.Id);
      return artifact;
    }

    private static ExternalGitCommit ConvertGitHubCommitToExternalGitCommit(
      GitHubData.V3.CommitListItem commitListItem)
    {
      ExternalGitCommit artifact = new ExternalGitCommit();
      artifact.Message = commitListItem.Commit.Message;
      artifact.Sha = commitListItem.Sha;
      artifact.WebUrl = commitListItem.Html_url;
      artifact.SetNodeId<ExternalGitCommit>(commitListItem.Node_id);
      return artifact;
    }

    private static ExternalGitPullRequest ConvertGitHubPullRequestToExternalGitPullRequest(
      GitHubData.V4.PullRequest pullRequest)
    {
      GitHubData.V4.IActor author = pullRequest.Author;
      GitHubData.V4.User user = author as GitHubData.V4.User;
      GitHubData.V4.Organization organization = author as GitHubData.V4.Organization;
      GitHubData.V4.Bot bot = author as GitHubData.V4.Bot;
      string name = user?.Name ?? organization?.Name;
      string nodeId = user?.Id ?? organization?.Id ?? bot?.Id;
      GitHubData.V4.Connection<GitHubData.V4.PullRequestCommit> commits1 = pullRequest.Commits;
      GitHubData.V4.Commit commit;
      if (commits1 == null)
      {
        commit = (GitHubData.V4.Commit) null;
      }
      else
      {
        GitHubData.V4.PullRequestCommit[] nodes = commits1.Nodes;
        commit = nodes != null ? ((IEnumerable<GitHubData.V4.PullRequestCommit>) nodes).OrderByDescending<GitHubData.V4.PullRequestCommit, DateTime?>((Func<GitHubData.V4.PullRequestCommit, DateTime?>) (x => x.Commit?.CommittedDate)).FirstOrDefault<GitHubData.V4.PullRequestCommit>()?.Commit : (GitHubData.V4.Commit) null;
      }
      if (commit == null)
      {
        GitHubData.V4.Connection<GitHubData.V4.PullRequestCommit> commits2 = pullRequest.Commits;
        if (commits2 == null)
        {
          commit = (GitHubData.V4.Commit) null;
        }
        else
        {
          GitHubData.V4.Edge<GitHubData.V4.PullRequestCommit>[] edges = commits2.Edges;
          if (edges == null)
          {
            commit = (GitHubData.V4.Commit) null;
          }
          else
          {
            IOrderedEnumerable<GitHubData.V4.Edge<GitHubData.V4.PullRequestCommit>> source = ((IEnumerable<GitHubData.V4.Edge<GitHubData.V4.PullRequestCommit>>) edges).OrderByDescending<GitHubData.V4.Edge<GitHubData.V4.PullRequestCommit>, DateTime?>((Func<GitHubData.V4.Edge<GitHubData.V4.PullRequestCommit>, DateTime?>) (x => x.Node?.Commit?.CommittedDate));
            commit = source != null ? source.FirstOrDefault<GitHubData.V4.Edge<GitHubData.V4.PullRequestCommit>>()?.Node?.Commit : (GitHubData.V4.Commit) null;
          }
        }
      }
      List<IGrouping<string, GitHubData.V4.CheckRun>> groupingList;
      if (commit == null)
      {
        groupingList = (List<IGrouping<string, GitHubData.V4.CheckRun>>) null;
      }
      else
      {
        GitHubData.V4.StatusCheckRollup statusCheckRollup = commit.StatusCheckRollup;
        if (statusCheckRollup == null)
        {
          groupingList = (List<IGrouping<string, GitHubData.V4.CheckRun>>) null;
        }
        else
        {
          GitHubData.V4.Connection<GitHubData.V4.IStatusCheckRollupContext> contexts = statusCheckRollup.Contexts;
          if (contexts == null)
          {
            groupingList = (List<IGrouping<string, GitHubData.V4.CheckRun>>) null;
          }
          else
          {
            GitHubData.V4.IStatusCheckRollupContext[] nodes = contexts.Nodes;
            groupingList = nodes != null ? nodes.OfType<GitHubData.V4.CheckRun>().GroupBy<GitHubData.V4.CheckRun, string>((Func<GitHubData.V4.CheckRun, string>) (x => (x.CheckSuite?.WorkflowRun?.Workflow?.Id ?? x.CheckSuite?.WorkflowRun?.Workflow?.Name) + ": " + x.Name)).ToList<IGrouping<string, GitHubData.V4.CheckRun>>() : (List<IGrouping<string, GitHubData.V4.CheckRun>>) null;
          }
        }
      }
      List<IGrouping<string, GitHubData.V4.CheckRun>> source1 = groupingList;
      ExternalGitPullRequest artifact = new ExternalGitPullRequest();
      artifact.Id = pullRequest.Id;
      ExternalGitUser externalGitUser;
      if (author != null)
        externalGitUser = new ExternalGitUser()
        {
          AvatarUrl = author.AvatarUrl?.ToString()
        }.SetNodeId<ExternalGitUser>(nodeId).SetName(name).SetLogin(author?.Login);
      else
        externalGitUser = AzureBoardsGitHubDataHelper.s_ghostUser;
      artifact.Sender = externalGitUser;
      artifact.Number = pullRequest.Number;
      artifact.Title = pullRequest.Title;
      artifact.Description = pullRequest.Body;
      artifact.TargetRef = pullRequest.BaseRefName;
      artifact.State = System.Enum.GetName(typeof (GitHubData.V4.PullRequestState), (object) pullRequest.State);
      artifact.Draft = pullRequest.Draft;
      artifact.CreatedAt = pullRequest.CreatedAt.ToString();
      DateTime dateTime = pullRequest.UpdatedAt;
      artifact.UpdatedAt = dateTime.ToString();
      DateTime? nullable = pullRequest.MergedAt;
      ref DateTime? local1 = ref nullable;
      string str1;
      if (!local1.HasValue)
      {
        str1 = (string) null;
      }
      else
      {
        dateTime = local1.GetValueOrDefault();
        str1 = dateTime.ToString();
      }
      artifact.MergedAt = str1;
      nullable = pullRequest.ClosedAt;
      ref DateTime? local2 = ref nullable;
      string str2;
      if (!local2.HasValue)
      {
        str2 = (string) null;
      }
      else
      {
        dateTime = local2.GetValueOrDefault();
        str2 = dateTime.ToString();
      }
      artifact.ClosedAt = str2;
      artifact.Repo = AzureBoardsGitHubDataHelper.ConvertGitHubRepositoryToExternalGitRepo(pullRequest.Repository);
      artifact.Assignees = AzureBoardsGitHubDataHelper.ConvertGitHubUserConnectionToExternalGitUser(pullRequest.Assignees);
      artifact.WebUrl = pullRequest.Url?.ToString();
      artifact.ReviewDecision = pullRequest?.ReviewDecision.ToString().Replace("_", "");
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      artifact.StatusCheckResults = source1 != null ? (IDictionary<string, IEnumerable<ExternalCheckRun>>) source1.ToDictionary<IGrouping<string, GitHubData.V4.CheckRun>, string, IEnumerable<ExternalCheckRun>>((Func<IGrouping<string, GitHubData.V4.CheckRun>, string>) (x => x.Key), (Func<IGrouping<string, GitHubData.V4.CheckRun>, IEnumerable<ExternalCheckRun>>) (x => x.Select<GitHubData.V4.CheckRun, ExternalCheckRun>(AzureBoardsGitHubDataHelper.\u003C\u003EO.\u003C3\u003E__ConvertGitHubCheckRunToExternalCheckRun ?? (AzureBoardsGitHubDataHelper.\u003C\u003EO.\u003C3\u003E__ConvertGitHubCheckRunToExternalCheckRun = new Func<GitHubData.V4.CheckRun, ExternalCheckRun>(AzureBoardsGitHubDataHelper.ConvertGitHubCheckRunToExternalCheckRun))))) : (IDictionary<string, IEnumerable<ExternalCheckRun>>) null;
      artifact.SetNodeId<ExternalGitPullRequest>(pullRequest.Id);
      return artifact;
    }

    private static ExternalCheckRun ConvertGitHubCheckRunToExternalCheckRun(
      GitHubData.V4.CheckRun checkRun)
    {
      return new ExternalCheckRun()
      {
        Id = checkRun.Id,
        CompletedAt = checkRun.CompletedAt,
        Conclusion = checkRun.Conclusion?.Replace("_", ""),
        Name = checkRun.Name,
        WorkflowName = checkRun?.CheckSuite?.WorkflowRun?.Workflow?.Name,
        StartedAt = checkRun.StartedAt,
        Status = checkRun.Status?.Replace("_", "")
      };
    }

    private static ExternalGitIssue ConvertGitHubIssueToExternalGitIssue(GitHubData.V4.Issue issue)
    {
      GitHubData.V4.IActor author = issue.Author;
      GitHubData.V4.User user = author as GitHubData.V4.User;
      GitHubData.V4.Organization organization = author as GitHubData.V4.Organization;
      GitHubData.V4.Bot bot = author as GitHubData.V4.Bot;
      string name = user?.Name ?? organization?.Name;
      string nodeId = user?.Id ?? organization?.Id ?? bot?.Id;
      string login = author?.Login;
      ExternalGitIssue artifact = new ExternalGitIssue();
      ExternalGitUser externalGitUser;
      if (author != null)
        externalGitUser = new ExternalGitUser()
        {
          AvatarUrl = author.AvatarUrl?.ToString()
        }.SetNodeId<ExternalGitUser>(nodeId).SetName(name).SetLogin(login);
      else
        externalGitUser = AzureBoardsGitHubDataHelper.s_ghostUser;
      artifact.Sender = externalGitUser;
      artifact.Number = issue.Number;
      artifact.Title = issue.Title;
      artifact.Description = issue.Body;
      artifact.State = System.Enum.GetName(typeof (GitHubData.V4.IssueState), (object) issue.State);
      artifact.CreatedAt = issue.CreatedAt.ToString();
      artifact.UpdatedAt = issue.UpdatedAt.ToString();
      DateTime? closedAt = issue.ClosedAt;
      ref DateTime? local = ref closedAt;
      artifact.ClosedAt = local.HasValue ? local.GetValueOrDefault().ToString() : (string) null;
      artifact.Repo = AzureBoardsGitHubDataHelper.ConvertGitHubRepositoryToExternalGitRepo(issue.Repository);
      artifact.Assignees = AzureBoardsGitHubDataHelper.ConvertGitHubUserConnectionToExternalGitUser(issue.Assignees);
      artifact.WebUrl = issue.Url?.ToString();
      artifact.SetNodeId<ExternalGitIssue>(issue.Id);
      return artifact;
    }

    private static ExternalGitCommitComment ConvertGitHubCommitCommentToExternalGitCommitComment(
      GitHubData.V4.CommitComment commitComment)
    {
      ExternalGitCommitComment externalGitComment = new ExternalGitCommitComment();
      externalGitComment.Id = commitComment.Id;
      externalGitComment.CommentBody = commitComment.Body;
      DateTime updatedAt = commitComment.UpdatedAt;
      externalGitComment.UpdatedAt = commitComment.UpdatedAt.ToString();
      externalGitComment.Repo = AzureBoardsGitHubDataHelper.ConvertGitHubRepositoryToExternalGitRepo(commitComment.Repository);
      externalGitComment.SetCommentDatabaseId(commitComment.DatabaseId);
      return externalGitComment;
    }

    private static ExternalGitRepo ConvertGitHubRepositoryToExternalGitRepo(
      GitHubData.V4.Repository repository)
    {
      if (repository == null)
        return (ExternalGitRepo) null;
      ExternalGitRepo externalGitRepo = new ExternalGitRepo();
      externalGitRepo.Id = repository.Id;
      externalGitRepo.Name = repository.Name;
      externalGitRepo.IsPrivate = repository.IsPrivate;
      externalGitRepo.DefaultBranch = repository.DefaultBranchRef?.Name;
      externalGitRepo.WebUrl = repository.Url?.ToString();
      externalGitRepo.SetRepoNameWithOwner(repository.NameWithOwner);
      return externalGitRepo;
    }

    private static ExternalRateLimit ConvertGitHubRateLimitToExternalRateLimit(
      GitHubData.V4.RateLimit rateLimit)
    {
      return new ExternalRateLimit()
      {
        Limit = rateLimit.Limit,
        Remaining = rateLimit.Remaining
      };
    }

    private static ExternalGitRepo ConvertGitHubV3RepositoryToExternalGitRepo(
      GitHubData.V3.Repository repository)
    {
      ExternalGitRepo externalGitRepo = new ExternalGitRepo();
      int? id = repository.Id;
      ref int? local = ref id;
      externalGitRepo.Id = local.HasValue ? local.GetValueOrDefault().ToString() : (string) null;
      externalGitRepo.Name = repository.Name;
      externalGitRepo.IsPrivate = repository.Private;
      externalGitRepo.Url = repository.Url;
      externalGitRepo.DefaultBranch = repository.Default_branch;
      externalGitRepo.WebUrl = repository.Url?.ToString();
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      dictionary.Add("nodeId", (object) repository.Node_Id);
      dictionary.Add("repoNameWithOwner", (object) repository.Full_name);
      GitHubData.V3.RepositoryPermissions permissions = repository.Permissions;
      dictionary.Add("hasAdminPermission", (object) (bool) (permissions != null ? (permissions.Admin ? 1 : 0) : 0));
      dictionary.Add("archived", (object) repository.Archived);
      dictionary.Add("isFork", (object) repository.Fork);
      dictionary.Add("isPrivate", (object) repository.Private);
      dictionary.Add("ownerAvatarUrl", (object) repository.Owner?.Avatar_url);
      externalGitRepo.AdditionalProperties = (IDictionary<string, object>) dictionary;
      return externalGitRepo;
    }

    public static ExternalGitRepo ConvertGitHubRepositoryEventDataToExternalGitRepo(
      GitHubRepository repository)
    {
      return new ExternalGitRepo()
      {
        Id = repository.NodeId,
        Name = repository.FullName,
        WebUrl = repository.WebUrl,
        IsPrivate = repository.Private
      };
    }

    public static ExternalGitRepo[] ConvertGitHubV3RepositoriesToExternalGitRepoArray(
      IEnumerable<GitHubData.V3.Repository> repositories)
    {
      return repositories.Select<GitHubData.V3.Repository, ExternalGitRepo>((Func<GitHubData.V3.Repository, ExternalGitRepo>) (r => AzureBoardsGitHubDataHelper.ConvertGitHubV3RepositoryToExternalGitRepo(r))).ToArray<ExternalGitRepo>();
    }

    private static ICollection<ExternalGitUser> ConvertGitHubUserConnectionToExternalGitUser(
      GitHubData.V4.Connection<GitHubData.V4.User> users)
    {
      return users == null ? (ICollection<ExternalGitUser>) null : (ICollection<ExternalGitUser>) ((IEnumerable<GitHubData.V4.Edge<GitHubData.V4.User>>) users.Edges).Select<GitHubData.V4.Edge<GitHubData.V4.User>, ExternalGitUser>((Func<GitHubData.V4.Edge<GitHubData.V4.User>, ExternalGitUser>) (u => AzureBoardsGitHubDataHelper.ConvertGitHubUserToExternalGitUser(u.Node))).ToList<ExternalGitUser>();
    }

    private static ExternalGitUser ConvertGitHubUserToExternalGitUser(GitHubData.V4.User user)
    {
      if (user == null)
        return AzureBoardsGitHubDataHelper.s_ghostUser;
      return new ExternalGitUser()
      {
        AvatarUrl = user.AvatarUrl?.ToString()
      }.SetNodeId<ExternalGitUser>(user.Id).SetName(user.Name).SetLogin(user.Login);
    }

    private static GitHubLinkItemType GetLinkItemType(JToken jToken)
    {
      if (jToken != null && jToken.HasValues)
      {
        switch (jToken.Value<string>((object) "$type"))
        {
          case "Commit":
            return GitHubLinkItemType.Commit;
          case "PullRequest":
            return GitHubLinkItemType.PullRequest;
          case "Issue":
            return GitHubLinkItemType.Issue;
        }
      }
      return GitHubLinkItemType.Unknown;
    }

    private static ExternalGitCommit ConvertJTokenToExternalGitCommit(JToken jToken)
    {
      GitHubData.V4.Commit commit = jToken.ToObject<GitHubData.V4.Commit>(GitHubHttpClient.GraphQLSerializer);
      return commit == null ? (ExternalGitCommit) null : AzureBoardsGitHubDataHelper.ConvertGitHubCommitToExternalGitCommit(commit);
    }

    private static ExternalGitPullRequest ConvertJTokenToExternalGitPullRequest(JToken jToken)
    {
      GitHubData.V4.PullRequest pullRequest = jToken.ToObject<GitHubData.V4.PullRequest>(GitHubHttpClient.GraphQLSerializer);
      return pullRequest == null ? (ExternalGitPullRequest) null : AzureBoardsGitHubDataHelper.ConvertGitHubPullRequestToExternalGitPullRequest(pullRequest);
    }

    private static ExternalGitIssue ConvertJTokenToExternalGitIssue(JToken jToken)
    {
      GitHubData.V4.Issue issue = jToken.ToObject<GitHubData.V4.Issue>(GitHubHttpClient.GraphQLSerializer);
      return issue == null ? (ExternalGitIssue) null : AzureBoardsGitHubDataHelper.ConvertGitHubIssueToExternalGitIssue(issue);
    }

    private static GitHubLinkItem ConvertJObjectToGitHubLinkItem(
      JToken jObject,
      GitHubLinkItemType type,
      GitHubData.V4.Repository repo = null,
      bool hasPrAdditionalInformation = false)
    {
      switch (type)
      {
        case GitHubLinkItemType.PullRequest:
          GitHubData.V4.PullRequest pullRequest = jObject.ToObject<GitHubData.V4.PullRequest>(GitHubHttpClient.GraphQLSerializer);
          if (pullRequest == null)
            return (GitHubLinkItem) null;
          GitHubLinkItem gitHubLinkItem1 = new GitHubLinkItem();
          gitHubLinkItem1.Author = new Author()
          {
            AvatarUrl = pullRequest.Author?.AvatarUrl?.ToString(),
            DisplayName = pullRequest.Author?.Login
          };
          gitHubLinkItem1.Date = new DateTime?(pullRequest.UpdatedAt);
          gitHubLinkItem1.Title = pullRequest.Title;
          gitHubLinkItem1.State = pullRequest.State.ToString().ToLower();
          gitHubLinkItem1.NumberOrSHA = pullRequest.Number;
          gitHubLinkItem1.Url = pullRequest.Url?.ToString();
          gitHubLinkItem1.ItemType = GitHubLinkItemType.PullRequest;
          gitHubLinkItem1.RepoExternalId = repo?.Id;
          gitHubLinkItem1.RepoNameWithOwner = repo?.NameWithOwner;
          gitHubLinkItem1.SetItem((IExternalArtifact) AzureBoardsGitHubDataHelper.ConvertGitHubPullRequestToExternalGitPullRequest(pullRequest));
          return gitHubLinkItem1;
        case GitHubLinkItemType.Commit:
          GitHubData.V4.Commit commit = jObject.ToObject<GitHubData.V4.Commit>(GitHubHttpClient.GraphQLSerializer);
          if (commit == null)
            return (GitHubLinkItem) null;
          GitHubLinkItem gitHubLinkItem2 = new GitHubLinkItem();
          gitHubLinkItem2.Author = new Author()
          {
            AvatarUrl = commit.Author?.AvatarUrl?.ToString(),
            DisplayName = commit.Author?.User?.Login ?? commit.Author?.Name
          };
          gitHubLinkItem2.Date = new DateTime?(commit.CommittedDate);
          gitHubLinkItem2.Title = commit.Message;
          gitHubLinkItem2.State = (string) null;
          gitHubLinkItem2.NumberOrSHA = commit.Oid;
          gitHubLinkItem2.Url = commit.Url?.ToString();
          gitHubLinkItem2.ItemType = GitHubLinkItemType.Commit;
          gitHubLinkItem2.RepoExternalId = repo?.Id;
          gitHubLinkItem2.RepoNameWithOwner = repo?.NameWithOwner;
          gitHubLinkItem2.SetItem((IExternalArtifact) AzureBoardsGitHubDataHelper.ConvertGitHubCommitToExternalGitCommit(commit));
          return gitHubLinkItem2;
        case GitHubLinkItemType.Issue:
          GitHubData.V4.Issue issue = jObject.ToObject<GitHubData.V4.Issue>(GitHubHttpClient.GraphQLSerializer);
          if (issue == null)
            return (GitHubLinkItem) null;
          GitHubLinkItem gitHubLinkItem3 = new GitHubLinkItem();
          gitHubLinkItem3.Author = new Author()
          {
            AvatarUrl = issue.Author?.AvatarUrl?.ToString(),
            DisplayName = issue.Author?.Login
          };
          gitHubLinkItem3.Date = new DateTime?(issue.UpdatedAt);
          gitHubLinkItem3.Title = issue.Title;
          gitHubLinkItem3.State = issue.State.ToString().ToLower();
          gitHubLinkItem3.NumberOrSHA = issue.Number;
          gitHubLinkItem3.Url = issue.Url?.ToString();
          gitHubLinkItem3.ItemType = GitHubLinkItemType.Issue;
          gitHubLinkItem3.RepoExternalId = repo?.Id;
          gitHubLinkItem3.RepoNameWithOwner = repo?.NameWithOwner;
          gitHubLinkItem3.SetItem((IExternalArtifact) AzureBoardsGitHubDataHelper.ConvertGitHubIssueToExternalGitIssue(issue));
          return gitHubLinkItem3;
        default:
          return (GitHubLinkItem) null;
      }
    }

    protected static (string query, Dictionary<string, object> variables) GenerateGetLinkItemsQuery(
      IEnumerable<(string RepoExternalId, string NumberOrSha, GitHubLinkItemType ItemType)> numberOrShaWithRepoNodeIds,
      bool newPrExperience,
      bool requestPrCheckRunInfo = false,
      bool requestPrMergeInformation = false)
    {
      StringBuilder stringBuilder1 = new StringBuilder();
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      stringBuilder1.AppendLine("query {");
      int num1 = 0;
      int num2 = 0;
      if (numberOrShaWithRepoNodeIds.Any<(string, string, GitHubLinkItemType)>())
      {
        foreach (IGrouping<string, (string, string, GitHubLinkItemType)> grouping in numberOrShaWithRepoNodeIds.GroupBy<(string, string, GitHubLinkItemType), string>((Func<(string, string, GitHubLinkItemType), string>) (o => o.RepoExternalId)))
        {
          StringBuilder stringBuilder2 = new StringBuilder();
          int result;
          foreach ((string, string, GitHubLinkItemType) tuple in (IEnumerable<(string, string, GitHubLinkItemType)>) grouping)
          {
            switch (tuple.Item3)
            {
              case GitHubLinkItemType.PullRequest:
                if (int.TryParse(tuple.Item2, out result))
                {
                  stringBuilder2.Append(string.Format("{0}{1}:pullRequest(number: {2}) {{ ...pullRequest }} ", (object) "__pullrequest", (object) num2++, (object) tuple.Item2));
                  flag1 = true;
                  continue;
                }
                continue;
              case GitHubLinkItemType.Commit:
                stringBuilder2.Append(string.Format("{0}{1}:object(expression: \"{2}\") {{ ...commit }} ", (object) "__commit", (object) num2++, (object) tuple.Item2));
                flag2 = true;
                continue;
              case GitHubLinkItemType.Issue:
                if (int.TryParse(tuple.Item2, out result))
                {
                  stringBuilder2.Append(string.Format("{0}{1}:issue(number: {2}) {{ ...issue }} ", (object) "__issue", (object) num2++, (object) tuple.Item2));
                  flag3 = true;
                  continue;
                }
                continue;
              default:
                continue;
            }
          }
          stringBuilder1.AppendLine(string.Format("{0}{1}:node(id:\"{2}\") {{ ... on Repository {{ __typename id nameWithOwner {3} }} }}", (object) "__repository", (object) num1++, (object) grouping.Key, (object) stringBuilder2));
        }
      }
      stringBuilder1.AppendLine("}");
      if (flag1 | flag2 | flag3)
      {
        stringBuilder1.AppendLine("fragment repository on Repository { __typename id name nameWithOwner isPrivate url defaultBranchRef { id name } }");
        stringBuilder1.AppendLine("fragment user on User { __typename id login name avatarUrl}");
      }
      if (flag1 | flag3)
      {
        stringBuilder1.AppendLine("fragment bot on Bot { __typename id login avatarUrl}");
        stringBuilder1.AppendLine("fragment organization on Organization { __typename id login name avatarUrl}");
        stringBuilder1.AppendLine("fragment userConnection on UserConnection { edges { node { ...user } } }");
      }
      if (flag1)
      {
        if (newPrExperience)
        {
          string str1 = "fragment pullRequest on PullRequest { __typename id number title body state createdAt updatedAt mergedAt closedAt baseRefName url isDraft reviewDecision repository { ...repository } author { ...user ...bot ...organization } comments { totalCount } assignees(first: 100) { ...userConnection } _PRCOMMITS_ _MERGEREQUIREMENTS_}";
          string str2;
          if (requestPrCheckRunInfo)
          {
            stringBuilder1.AppendLine("fragment checkrun on CheckRun { __typename startedAt completedAt id name status conclusion checkSuite { workflowRun { workflow { name state id } } } }");
            stringBuilder1.AppendLine("fragment prcommits on PullRequestCommitConnection { totalCount nodes { commit { abbreviatedOid committedDate message statusCheckRollup { state contexts(last: 100) { totalCount nodes { ...checkrun } } } } } }");
            str2 = str1.Replace("_PRCOMMITS_", "commits(last: 1) { ...prcommits }");
          }
          else
            str2 = str1.Replace("_PRCOMMITS_", "");
          string str3;
          if (requestPrMergeInformation)
          {
            stringBuilder1.AppendLine("fragment mergeRequirements on PullRequestMergeRequirements { state commitAuthor conditions { __typename displayName description message result ... on PullRequestRulesCondition { ruleRollups { ruleType displayName message result } } } }");
            str3 = str2.Replace("_MERGEREQUIREMENTS_", "mergeRequirements { ...mergeRequirements }");
          }
          else
            str3 = str2.Replace("_MERGEREQUIREMENTS_", "");
          stringBuilder1.AppendLine(str3);
        }
        else
          stringBuilder1.AppendLine("fragment pullRequest on PullRequest { __typename id number title body state createdAt updatedAt mergedAt closedAt baseRefName url repository { ...repository } author { ...user ...bot ...organization } comments { totalCount } assignees(first: 100) { ...userConnection } }");
      }
      if (flag2)
        stringBuilder1.AppendLine("fragment commit on Commit { __typename id oid message status { __typename state } committedDate pushedDate url repository { ...repository } author { __typename name email avatarUrl user { ...user } } }");
      if (flag3)
        stringBuilder1.AppendLine("fragment issue on Issue { __typename id number title body state createdAt updatedAt closedAt url repository { ...repository } author { ...user ...bot ...organization } comments { totalCount } assignees(first: 100) { ...userConnection } }");
      return (stringBuilder1.ToString(), dictionary);
    }

    protected static string GenerateGetChangedItemsQuery(
      string repoNodeId,
      string prCursor,
      string issueCursor,
      string commitCommentCursor)
    {
      string str1 = prCursor == null ? "last: 1" : "first: 100";
      string str2 = issueCursor == null ? "last: 1" : "first: 100";
      string str3 = commitCommentCursor == null ? "last: 1" : "first: 100";
      return "\r\nquery {\r\n    node(id: \"" + repoNodeId + "\") {\r\n        ... on Repository {\r\n            ...repository\r\n            defaultBranchRef { \r\n                id \r\n                name\r\n                ...on Ref { \r\n                    target { \r\n                        ... on Commit {\r\n                            __typename\r\n                            oid\r\n                        } \r\n                    } \r\n                } \r\n            }\r\n\r\n            pullRequests(" + str1 + " " + AzureBoardsGitHubDataHelper.GetItemAfterValue(prCursor) + " orderBy:{ field:UPDATED_AT, direction:ASC }) { \r\n                ...prConnection \r\n            }\r\n\r\n            issues(" + str2 + " " + AzureBoardsGitHubDataHelper.GetItemAfterValue(issueCursor) + " orderBy:{ field:UPDATED_AT, direction:ASC }) { \r\n                ...issueConnection \r\n            }\r\n\r\n            commitComments(" + str3 + " " + AzureBoardsGitHubDataHelper.GetItemAfterValue(commitCommentCursor) + ") { \r\n                ...commitCommentConnection\r\n            }\r\n        }\r\n    }\r\n}\r\n\r\nfragment pageInfo on PageInfo { \r\n    endCursor\r\n    hasNextPage \r\n}\r\n\r\nfragment repository on Repository { __typename id name nameWithOwner isPrivate url defaultBranchRef { id name } }\r\nfragment bot on Bot { __typename id login avatarUrl}\r\nfragment organization on Organization { __typename id login name avatarUrl}\r\nfragment user on User { __typename id login name avatarUrl}\r\nfragment userConnection on UserConnection { edges { node { ...user } } }\r\nfragment pullRequest on PullRequest { __typename id number title body state createdAt updatedAt mergedAt closedAt baseRefName url repository { ...repository } author { ...user ...bot ...organization } comments { totalCount } assignees(first: 100) { ...userConnection } }\r\nfragment issue on Issue { __typename id number title body state createdAt updatedAt closedAt url repository { ...repository } author { ...user ...bot ...organization } comments { totalCount } assignees(first: 100) { ...userConnection } }\r\nfragment commitComment on CommitComment { __typename id databaseId body createdAt updatedAt url repository { ...repository } }\r\n\r\nfragment prConnection on PullRequestConnection { \r\n    pageInfo { \r\n        ...pageInfo \r\n    }\r\n    edges { \r\n        cursor \r\n        node { \r\n            ...pullRequest\r\n        } \r\n    } \r\n}\r\n\r\nfragment issueConnection on IssueConnection { \r\n    pageInfo { \r\n        ...pageInfo \r\n    }\r\n    edges { \r\n        cursor \r\n        node { \r\n            ...issue\r\n        } \r\n    } \r\n}\r\n\r\nfragment commitCommentConnection on CommitCommentConnection { \r\n    pageInfo { \r\n        ...pageInfo \r\n    }\r\n    edges { \r\n        cursor \r\n        node { \r\n            ...commitComment\r\n        } \r\n    } \r\n}\r\n";
    }

    protected static string GetItemAfterValue(string value) => !string.IsNullOrEmpty(value) ? "after: \"" + value + "\"" : (string) null;

    private int TracePointStart => 919601;

    private string Layer => nameof (AzureBoardsGitHubDataHelper);

    private string Area => "Services";
  }
}
