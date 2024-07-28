// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalMentions.GitHubMentionResiliencyProcessor
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.ServiceModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalMentions
{
  public class GitHubMentionResiliencyProcessor
  {
    public GitHubMentionResult ProcessMentions(
      IVssRequestContext requestContext,
      AzureBoardsGitHubDataHelper gitHubDataHelper)
    {
      GitHubMentionResult hubMentionResult = new GitHubMentionResult();
      IEnumerable<ExternalConnection> externalConnections = requestContext.GetService<IExternalConnectionService>().GetExternalConnections(requestContext.Elevate(), new Guid?(), (string) null, includeAuthorization: true, includeRepoProviderData: true).Where<ExternalConnection>((Func<ExternalConnection, bool>) (c =>
      {
        ServiceEndpointViewModel serviceEndpoint = c.ServiceEndpoint;
        bool? nullable;
        if (serviceEndpoint == null)
        {
          nullable = new bool?();
        }
        else
        {
          ServiceEndpoint rawServiceEndpoint = serviceEndpoint.RawServiceEndpoint;
          nullable = rawServiceEndpoint != null ? new bool?(rawServiceEndpoint.IsAzureBoardsGitHubFamilyType()) : new bool?();
        }
        return nullable.GetValueOrDefault();
      }));
      if (!externalConnections.Any<ExternalConnection>())
      {
        hubMentionResult.Message = "No external connections for GitHub/GitHub Enterprise configured, skipping";
        return hubMentionResult;
      }
      Dictionary<ExternalGitRepo, ICollection<ExternalConnection>> availableConnectionsMap = GitHubMentionResiliencyProcessor.GetRepoToAvailableConnectionsMap(externalConnections);
      foreach (ExternalGitRepo key in availableConnectionsMap.Keys)
      {
        GitHubRepoMentionResult result = new GitHubRepoMentionResult()
        {
          RepoNameWithOwner = key.RepoNameWithOwner(),
          ConnectionCount = availableConnectionsMap.Count
        };
        hubMentionResult.RepoStatus.Add(result);
        try
        {
          if (!GitHubMentionResiliencyProcessor.TryMentionRepositoryConnections(requestContext, gitHubDataHelper, key, (IEnumerable<ExternalConnection>) availableConnectionsMap[key], result))
            GitHubMentionResiliencyProcessor.WriteCI(requestContext, "FailedToProcessRepository", ("Repo", (object) key));
        }
        catch (Exception ex)
        {
          GitHubMentionResiliencyProcessor.WriteCI(requestContext, "ProcessRepositoryException", ("Exception", (object) ex.ToString()), ("Repo", (object) key));
          hubMentionResult.Message = ex.ToString();
        }
      }
      return hubMentionResult;
    }

    internal static bool TryMentionRepositoryConnections(
      IVssRequestContext requestContext,
      AzureBoardsGitHubDataHelper gitHubDataHelper,
      ExternalGitRepo repo,
      IEnumerable<ExternalConnection> connections,
      GitHubRepoMentionResult result)
    {
      string str = repo.NodeId();
      List<Guid> list = connections.Select<ExternalConnection, Guid>((Func<ExternalConnection, Guid>) (c => c.ProjectId)).Distinct<Guid>().ToList<Guid>();
      ExternalConnection connection1 = connections.First<ExternalConnection>();
      ExternalGitRepo updatedRepo = (ExternalGitRepo) null;
      string serializedMetadata = repo != null ? repo.GetSerializedMetadata() : (string) null;
      foreach (ExternalConnection connection2 in connections)
      {
        try
        {
          ConnectionErrorType? connectionErrorType;
          if (GitHubMentionResiliencyProcessor.TryMentionRepository(requestContext, gitHubDataHelper, (IEnumerable<Guid>) list, repo, result, connection2, out updatedRepo, out connectionErrorType))
          {
            result.Succeeded = true;
            break;
          }
          result.Message = "At least one connection is invalid, check CI for more details";
          if (connectionErrorType.HasValue)
            connection2.ConnectionMetadata.ConnectionErrorType = connectionErrorType;
          requestContext.GetService<IExternalConnectionService>().UpdateExternalConnectionMetadata(requestContext, connection2.ProjectId, connection2.Id, connection2.ConnectionMetadata);
        }
        catch (Exception ex)
        {
          GitHubMentionResiliencyProcessor.WriteCI(requestContext, "ProcessRepositoryException", ("Exception", (object) ex.ToString()), ("Connection", (object) GitHubMentionResiliencyProcessor.SanitizeExternalConnectionForCI(connection2)), ("Repo", (object) repo));
          result.Message = ex.ToString();
        }
      }
      if (updatedRepo == null)
        GitHubMentionResiliencyProcessor.WriteCI(requestContext, "NullUpdatedRepository", ("Connection", (object) GitHubMentionResiliencyProcessor.SanitizeExternalConnectionForCI(connection1)), ("Repo", (object) repo));
      else if (string.IsNullOrEmpty(repo.GetSerializedMetadata()))
        GitHubMentionResiliencyProcessor.WriteCI(requestContext, "NullMetadata", ("Connection", (object) GitHubMentionResiliencyProcessor.SanitizeExternalConnectionForCI(connection1)), ("Repo", (object) repo));
      else if (!string.Equals(repo.GetSerializedMetadata(), serializedMetadata, StringComparison.OrdinalIgnoreCase) || !string.Equals(repo.RepoNameWithOwner(), updatedRepo.RepoNameWithOwner(), StringComparison.OrdinalIgnoreCase))
      {
        try
        {
          requestContext.GetService<IExternalConnectionService>().SaveExternalRepositoryData(requestContext, (IEnumerable<ExternalConnectionRepository>) new ExternalConnectionRepository[1]
          {
            new ExternalConnectionRepository()
            {
              RepositoryId = repo.GetRepoInternalId(),
              ExternalId = str,
              RepositoryName = updatedRepo.RepoNameWithOwner(),
              IsPrivate = updatedRepo.IsPrivate,
              Url = updatedRepo.Url,
              WebUrl = updatedRepo.WebUrl,
              Metadata = repo.GetSerializedMetadata()
            }
          });
          return true;
        }
        catch (Exception ex)
        {
          GitHubMentionResiliencyProcessor.WriteCI(requestContext, "SaveExternalRepositoryData", ("Exception", (object) ex.ToString()), ("Repo", (object) repo));
          result.Message = ex.ToString();
          result.Succeeded = false;
        }
      }
      else
      {
        GitHubMentionResiliencyProcessor.WriteCI(requestContext, "UnchangedRepository", ("Repo", (object) repo));
        return true;
      }
      return false;
    }

    internal static bool TryMentionRepository(
      IVssRequestContext requestContext,
      AzureBoardsGitHubDataHelper gitHubDataHelper,
      IEnumerable<Guid> projectIds,
      ExternalGitRepo repo,
      GitHubRepoMentionResult result,
      ExternalConnection externalConnection,
      out ExternalGitRepo updatedRepo,
      out ConnectionErrorType? connectionErrorType)
    {
      updatedRepo = (ExternalGitRepo) null;
      connectionErrorType = new ConnectionErrorType?();
      IGitHubMentionService service = requestContext.GetService<IGitHubMentionService>();
      GitHubRepoData gitHubRepoData = repo.GetMetadata<GitHubRepoData>() ?? new GitHubRepoData();
      ServiceEndpoint rawServiceEndpoint = externalConnection.ServiceEndpoint.RawServiceEndpoint;
      GitHubAuthentication hubAuthentication = rawServiceEndpoint.GetGitHubAuthentication(requestContext, externalConnection.ProjectId);
      hubAuthentication.UseFreshAccessToken = true;
      if (rawServiceEndpoint.IsOauthConfigurationDeleted())
      {
        connectionErrorType = new ConnectionErrorType?(ConnectionErrorType.OAuthConfigurationDeleted);
        return false;
      }
      string enterpriseUrl = rawServiceEndpoint.GetEnterpriseUrl();
      GitHubResult<ChangedGitHubItems> changedGitHubItems = gitHubDataHelper.GetChangedGitHubItems(enterpriseUrl, hubAuthentication, repo.NodeId(), repo.RepoNameWithOwner(), gitHubRepoData);
      ChangedGitHubItems result1 = changedGitHubItems.Result;
      if (result1 != null)
      {
        updatedRepo = result1.Repository ?? repo;
        if (gitHubRepoData.CurrentHead == null && gitHubRepoData.PullRequestCursor == null && gitHubRepoData.CommitCommentCursor == null && gitHubRepoData.IssueCursor == null)
        {
          GitHubMentionResiliencyProcessor.SetRepoProcessingWatermark(gitHubRepoData, result1);
        }
        else
        {
          IEnumerable<ExternalGitPullRequest> results1 = result1.PullRequests?.Results;
          IEnumerable<ExternalGitPullRequest> potentiallyMentionedPullRequests = (IEnumerable<ExternalGitPullRequest>) null;
          if (results1 != null && results1.Any<ExternalGitPullRequest>())
          {
            IEnumerable<ExternalGitPullRequest> source = service.MentionPullRequests(requestContext, repo.GetRepoInternalId(), projectIds, results1, true, true, (IEnumerable<ExternalConnection>) new List<ExternalConnection>()
            {
              externalConnection
            });
            potentiallyMentionedPullRequests = (IEnumerable<ExternalGitPullRequest>) (source != null ? source.ToHashSet<ExternalGitPullRequest>((IEqualityComparer<ExternalGitPullRequest>) ExternalGitPullRequestComparer.Instance) : (HashSet<ExternalGitPullRequest>) null);
          }
          IEnumerable<ExternalGitCommit> results2 = result1.Commits?.Results;
          IEnumerable<ExternalGitCommit> potentiallyMentionedCommits = (IEnumerable<ExternalGitCommit>) null;
          if (results2 != null && results2.Any<ExternalGitCommit>())
          {
            IEnumerable<ExternalGitCommit> source = service.MentionCommits(requestContext, repo, projectIds, results2, true, true, (IEnumerable<ExternalConnection>) new ExternalConnection[1]
            {
              externalConnection
            });
            potentiallyMentionedCommits = (IEnumerable<ExternalGitCommit>) (source != null ? source.ToHashSet<ExternalGitCommit>((IEqualityComparer<ExternalGitCommit>) ExternalGitCommitComparer.Instance) : (HashSet<ExternalGitCommit>) null);
          }
          IEnumerable<ExternalGitIssue> results3 = result1.Issues?.Results;
          IEnumerable<ExternalGitIssue> potentiallyMentionedIssues = (IEnumerable<ExternalGitIssue>) null;
          if (results3 != null && results3.Any<ExternalGitIssue>())
          {
            IEnumerable<ExternalGitIssue> source = service.MentionIssues(requestContext, repo.GetRepoInternalId(), projectIds, results3, true, (IEnumerable<ExternalConnection>) new List<ExternalConnection>()
            {
              externalConnection
            });
            potentiallyMentionedIssues = (IEnumerable<ExternalGitIssue>) (source != null ? source.ToHashSet<ExternalGitIssue>((IEqualityComparer<ExternalGitIssue>) ExternalGitIssueComparer.Instance) : (HashSet<ExternalGitIssue>) null);
          }
          IEnumerable<ExternalPullRequestCommentEvent> results4 = result1.IssueComments?.Results;
          if (results4 != null && results4.Any<ExternalPullRequestCommentEvent>())
          {
            List<ExternalPullRequestCommentEvent> list = results4.ToList<ExternalPullRequestCommentEvent>();
            ExternalGitRepo newRepo = updatedRepo;
            list.ForEach((Action<ExternalPullRequestCommentEvent>) (issueComment => issueComment.Repo = newRepo));
            service.MentionIssueComments(requestContext, repo.GetRepoInternalId(), projectIds, (IEnumerable<ExternalPullRequestCommentEvent>) list, (IEnumerable<ExternalConnection>) new List<ExternalConnection>()
            {
              externalConnection
            });
          }
          IEnumerable<ExternalGitCommitComment> results5 = result1.CommitComments.Results;
          if (results5 != null && results5.Any<ExternalGitCommitComment>())
            service.MentionCommitComments(requestContext, repo.GetRepoInternalId(), projectIds, results5, (IEnumerable<ExternalConnection>) new List<ExternalConnection>()
            {
              externalConnection
            });
          IVssRequestContext requestContext1 = requestContext;
          (string, object)[] valueTupleArray = new (string, object)[11];
          valueTupleArray[0] = ("Repo", (object) repo);
          valueTupleArray[1] = ("CommitsCount", (object) (results2 != null ? results2.Count<ExternalGitCommit>() : 0));
          IEnumerable<ExternalGitCommit> source1 = potentiallyMentionedCommits;
          valueTupleArray[2] = ("PotentiallyMentionedCommitsCount", (object) (source1 != null ? source1.Count<ExternalGitCommit>() : 0));
          valueTupleArray[3] = ("PullRequestsCount", (object) (results1 != null ? results1.Count<ExternalGitPullRequest>() : 0));
          IEnumerable<ExternalGitPullRequest> source2 = potentiallyMentionedPullRequests;
          valueTupleArray[4] = ("PotentiallyMentionedPullRequestsCount", (object) (source2 != null ? source2.Count<ExternalGitPullRequest>() : 0));
          valueTupleArray[5] = ("IssuesCount", (object) (results3 != null ? results3.Count<ExternalGitIssue>() : 0));
          IEnumerable<ExternalGitIssue> source3 = potentiallyMentionedIssues;
          valueTupleArray[6] = ("PotentiallyMentionedIssuesCount", (object) (source3 != null ? source3.Count<ExternalGitIssue>() : 0));
          valueTupleArray[7] = ("IssueCommentsCount", (object) (results4 != null ? results4.Count<ExternalPullRequestCommentEvent>() : 0));
          valueTupleArray[8] = ("CommitCommentsCount", (object) (results5 != null ? results5.Count<ExternalGitCommitComment>() : 0));
          valueTupleArray[9] = ("ProviderKey", (object) externalConnection.ProviderKey);
          valueTupleArray[10] = ("AuthType", (object) externalConnection.ServiceEndpoint.AuthorizationScheme);
          GitHubMentionResiliencyProcessor.WriteCI(requestContext1, "ChangedItemCounts", valueTupleArray);
          GitHubRepoMentionResult repoMentionResult = result;
          object[] objArray = new object[8];
          objArray[0] = (object) (results2 != null ? results2.Count<ExternalGitCommit>() : 0);
          IEnumerable<ExternalGitCommit> source4 = potentiallyMentionedCommits;
          objArray[1] = (object) (source4 != null ? source4.Count<ExternalGitCommit>() : 0);
          objArray[2] = (object) (results1 != null ? results1.Count<ExternalGitPullRequest>() : 0);
          IEnumerable<ExternalGitPullRequest> source5 = potentiallyMentionedPullRequests;
          objArray[3] = (object) (source5 != null ? source5.Count<ExternalGitPullRequest>() : 0);
          objArray[4] = (object) (results3 != null ? results3.Count<ExternalGitIssue>() : 0);
          IEnumerable<ExternalGitIssue> source6 = potentiallyMentionedIssues;
          objArray[5] = (object) (source6 != null ? source6.Count<ExternalGitIssue>() : 0);
          objArray[6] = (object) (results4 != null ? results4.Count<ExternalPullRequestCommentEvent>() : 0);
          objArray[7] = (object) (results5 != null ? results5.Count<ExternalGitCommitComment>() : 0);
          string str = string.Format("Commits: {0}, Potentially Mentioned Commits: {1}, Pull Requests: {2}, Potentially Mentioned Pull Requests: {3}, Issues: {4}, Potentially Mentioned Issues: {5}, Issue Comments: {6}, Commit Comments: {7}", objArray);
          repoMentionResult.Message = str;
          GitHubMentionResiliencyProcessor.SetRepoProcessingWatermark(gitHubRepoData, result1);
          try
          {
            ExternalArtifactCollectionWithStatus artifacts = new ExternalArtifactCollectionWithStatus()
            {
              Commits = results2 != null ? (IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitCommit>>) results2.Select<ExternalGitCommit, ExternalArtifactAndHydrationStatus<ExternalGitCommit>>((Func<ExternalGitCommit, ExternalArtifactAndHydrationStatus<ExternalGitCommit>>) (c => new ExternalArtifactAndHydrationStatus<ExternalGitCommit>()
              {
                ExternalArtifact = c,
                HydrationStatus = ExternalArtifactHydrationStatus.Partial,
                UpdateOnly = potentiallyMentionedCommits == null || !potentiallyMentionedCommits.Contains<ExternalGitCommit>(c)
              })).ToList<ExternalArtifactAndHydrationStatus<ExternalGitCommit>>() : (IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitCommit>>) null,
              PullRequests = results1 != null ? (IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>) results1.Select<ExternalGitPullRequest, ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>((Func<ExternalGitPullRequest, ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>) (pr => new ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>()
              {
                ExternalArtifact = pr,
                HydrationStatus = ExternalArtifactHydrationStatus.Full,
                UpdateOnly = potentiallyMentionedPullRequests == null || !potentiallyMentionedPullRequests.Contains<ExternalGitPullRequest>(pr)
              })).ToList<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>() : (IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>) null,
              Issues = results3 != null ? (IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitIssue>>) results3.Select<ExternalGitIssue, ExternalArtifactAndHydrationStatus<ExternalGitIssue>>((Func<ExternalGitIssue, ExternalArtifactAndHydrationStatus<ExternalGitIssue>>) (issue => new ExternalArtifactAndHydrationStatus<ExternalGitIssue>()
              {
                ExternalArtifact = issue,
                HydrationStatus = ExternalArtifactHydrationStatus.Full,
                UpdateOnly = potentiallyMentionedIssues == null || !potentiallyMentionedIssues.Contains<ExternalGitIssue>(issue)
              })).ToList<ExternalArtifactAndHydrationStatus<ExternalGitIssue>>() : (IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitIssue>>) null
            };
            requestContext.GetService<IExternalArtifactService>().SaveArtifacts(requestContext, externalConnection.ProviderKey, repo.GetRepoInternalId(), artifacts);
          }
          catch (Exception ex)
          {
            GitHubMentionResiliencyProcessor.WriteCI(requestContext, "UpdateArtifacts", ("Exception", (object) ex.ToString()), ("Repo", (object) repo));
            requestContext.TraceException(920110, nameof (GitHubMentionResiliencyProcessor), "UpdateArtifacts", ex);
          }
        }
        repo.SetMetadata((object) gitHubRepoData);
      }
      else if (string.Equals(changedGitHubItems.ErrorMessage, "Bad credentials", StringComparison.OrdinalIgnoreCase))
      {
        if (changedGitHubItems.StatusCode != HttpStatusCode.OK)
        {
          connectionErrorType = new ConnectionErrorType?(ConnectionErrorType.BadCredentials);
          GitHubMentionResiliencyProcessor.WriteCI(requestContext, nameof (TryMentionRepository), ("BadCredentialStatus", (object) changedGitHubItems.ErrorMessage), ("Repo", (object) repo), ("Status", (object) changedGitHubItems.StatusCode));
        }
        else
          GitHubMentionResiliencyProcessor.WriteCI(requestContext, nameof (TryMentionRepository), ("BadCredentialError", (object) changedGitHubItems.ErrorMessage), ("Repo", (object) repo));
      }
      else if (changedGitHubItems.Errors.Any<GitHubError>((Func<GitHubError, bool>) (error => string.Equals(error.Type, "Failed_To_Get_Installation_Token", StringComparison.OrdinalIgnoreCase))))
      {
        string installationId = externalConnection.ServiceEndpoint.RawServiceEndpoint.GetInstallationId();
        try
        {
          requestContext.GetService<IAzureBoardsGitHubAppService>().VerifyAndGetInstallationDetails(requestContext, installationId);
        }
        catch (GitHubAppCannotFetchInstallationException ex)
        {
          connectionErrorType = new ConnectionErrorType?(ConnectionErrorType.AppUninstalled);
        }
      }
      return changedGitHubItems.IsSuccessful;
    }

    private static void SetRepoProcessingWatermark(
      GitHubRepoData repoData,
      ChangedGitHubItems changedGitHubItems)
    {
      string endCursor1 = changedGitHubItems.PullRequests.PageInfo?.EndCursor;
      if (!string.IsNullOrEmpty(endCursor1))
        repoData.PullRequestCursor = endCursor1;
      string endCursor2 = changedGitHubItems.Issues.PageInfo?.EndCursor;
      if (!string.IsNullOrEmpty(endCursor2))
        repoData.IssueCursor = endCursor2;
      string latestUpdate = changedGitHubItems.IssueComments?.LatestUpdate;
      string latestId = changedGitHubItems.IssueComments?.LatestId;
      if (!string.IsNullOrEmpty(latestUpdate) && !string.IsNullOrEmpty(latestId))
      {
        repoData.IssueCommentLatestUpdate = latestUpdate;
        repoData.IssueCommentLatestId = latestId;
      }
      string endCursor3 = changedGitHubItems.CommitComments.PageInfo?.EndCursor;
      if (!string.IsNullOrEmpty(endCursor3))
        repoData.CommitCommentCursor = endCursor3;
      repoData.CurrentHead = changedGitHubItems.Commits.CurrentHead;
    }

    private static Dictionary<ExternalGitRepo, ICollection<ExternalConnection>> GetRepoToAvailableConnectionsMap(
      IEnumerable<ExternalConnection> externalConnections)
    {
      Dictionary<ExternalGitRepo, ICollection<ExternalConnection>> availableConnectionsMap = new Dictionary<ExternalGitRepo, ICollection<ExternalConnection>>((IEqualityComparer<ExternalGitRepo>) ExternalGitRepoComparer.Instance);
      foreach (ExternalConnection externalConnection in externalConnections)
      {
        if (externalConnection.IsConnectionMetadataValid)
        {
          foreach (ExternalGitRepo externalGitRepo in externalConnection.ExternalGitRepos)
          {
            ICollection<ExternalConnection> externalConnections1;
            if (!availableConnectionsMap.TryGetValue(externalGitRepo, out externalConnections1))
            {
              externalConnections1 = (ICollection<ExternalConnection>) new List<ExternalConnection>();
              availableConnectionsMap[externalGitRepo] = externalConnections1;
            }
            externalConnections1.Add(externalConnection);
          }
        }
      }
      return availableConnectionsMap;
    }

    private static void WriteCI(
      IVssRequestContext requestContext,
      string feature,
      params (string name, object value)[] nameValueTuples)
    {
      if (nameValueTuples.Length == 0)
        return;
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      foreach ((string name, object value) nameValueTuple in nameValueTuples)
        properties.Add(nameValueTuple.name, nameValueTuple.value);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "ExternalMentions", feature, properties);
    }

    private static ExternalConnection SanitizeExternalConnectionForCI(ExternalConnection connection)
    {
      if (connection != null && connection.ServiceEndpoint != null && connection.ServiceEndpoint.CreatedBy != null)
      {
        connection.ServiceEndpoint.CreatedBy.UniqueName = (string) null;
        connection.ServiceEndpoint.CreatedBy.DisplayName = (string) null;
      }
      return connection;
    }
  }
}
