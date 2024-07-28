// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.GitHubPendingArtifactResolver
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.ServiceModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts
{
  public class GitHubPendingArtifactResolver
  {
    private const int MaximumAttemptCount = 3;
    private const string UnableToResolveMessage = "Unable to resolve item from GitHub";

    public GitHubPendingArtifactResolverResult ResolvePendingArtifacts(
      IVssRequestContext requestContext)
    {
      GitHubPendingArtifactResolverResult artifactResolverResult = new GitHubPendingArtifactResolverResult();
      IExternalConnectionService service1 = requestContext.GetService<IExternalConnectionService>();
      IExternalArtifactService service2 = requestContext.GetService<IExternalArtifactService>();
      IEnumerable<PendingExternalArtifactIdentifier> pendingArtifacts = service2.GetPendingArtifacts(requestContext);
      AzureBoardsGitHubDataHelper gitHubDataHelper = AzureBoardsGitHubDataHelper.Create(requestContext);
      foreach (IGrouping<string, PendingExternalArtifactIdentifier> source1 in pendingArtifacts.GroupBy<PendingExternalArtifactIdentifier, string>((Func<PendingExternalArtifactIdentifier, string>) (p => p.ProviderKey)))
      {
        IReadOnlyCollection<ExternalConnection> externalConnections = service1.GetExternalConnections(requestContext, new Guid?(), source1.Key, includeAuthorization: true);
        foreach (IGrouping<string, PendingExternalArtifactIdentifier> source2 in source1.GroupBy<PendingExternalArtifactIdentifier, string>((Func<PendingExternalArtifactIdentifier, string>) (p => p.ExternalRepositoryId)))
        {
          Guid internalRepositoryId = source2.First<PendingExternalArtifactIdentifier>().InternalRepositoryId;
          string externalRepositoryId = source2.Key;
          ExternalConnection externalConnection = externalConnections.Where<ExternalConnection>((Func<ExternalConnection, bool>) (c => c.ExternalGitRepos.Any<ExternalGitRepo>((Func<ExternalGitRepo, bool>) (r => r.NodeId() == externalRepositoryId)))).FirstOrDefault<ExternalConnection>();
          if (externalConnection != null)
          {
            ServiceEndpoint rawServiceEndpoint = externalConnection.ServiceEndpoint.RawServiceEndpoint;
            GitHubAuthentication hubAuthentication = rawServiceEndpoint.GetGitHubAuthentication(requestContext, externalConnection.ProjectId);
            string enterpriseUrl = rawServiceEndpoint.GetEnterpriseUrl();
            GitHubResult<ExternalArtifactCollection> repoArtifacts = gitHubDataHelper.GetRepoArtifacts(enterpriseUrl, hubAuthentication, externalRepositoryId, (IEnumerable<PendingExternalArtifactIdentifier>) source2);
            if (repoArtifacts.IsSuccessful || repoArtifacts.Result != null)
            {
              ExternalArtifactCollection result = repoArtifacts.Result;
              List<ExternalArtifactAndHydrationStatus<ExternalGitCommit>> commits = new List<ExternalArtifactAndHydrationStatus<ExternalGitCommit>>();
              if (result.Commits != null)
                commits.AddRange(result.Commits.Select<ExternalGitCommit, ExternalArtifactAndHydrationStatus<ExternalGitCommit>>((Func<ExternalGitCommit, ExternalArtifactAndHydrationStatus<ExternalGitCommit>>) (c => new ExternalArtifactAndHydrationStatus<ExternalGitCommit>()
                {
                  ExternalArtifact = c,
                  HydrationStatus = ExternalArtifactHydrationStatus.Full,
                  UpdateOnly = true
                })));
              List<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>> pullRequests = new List<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>();
              if (result.PullRequests != null)
                pullRequests.AddRange(result.PullRequests.Select<ExternalGitPullRequest, ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>((Func<ExternalGitPullRequest, ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>) (c => new ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>()
                {
                  ExternalArtifact = c,
                  HydrationStatus = ExternalArtifactHydrationStatus.Full,
                  UpdateOnly = true
                })));
              List<ExternalArtifactAndHydrationStatus<ExternalGitIssue>> issues = new List<ExternalArtifactAndHydrationStatus<ExternalGitIssue>>();
              if (result.Issues != null)
                issues.AddRange(result.Issues.Select<ExternalGitIssue, ExternalArtifactAndHydrationStatus<ExternalGitIssue>>((Func<ExternalGitIssue, ExternalArtifactAndHydrationStatus<ExternalGitIssue>>) (c => new ExternalArtifactAndHydrationStatus<ExternalGitIssue>()
                {
                  ExternalArtifact = c,
                  HydrationStatus = ExternalArtifactHydrationStatus.Full,
                  UpdateOnly = true
                })));
              artifactResolverResult.ResolvedCommits += commits.Count;
              artifactResolverResult.ResolvedPullRequests += pullRequests.Count;
              artifactResolverResult.ResolvedIssues += issues.Count;
              List<ExternalArtifactAndHydrationStatus<ExternalGitCommit>> list1 = source2.Where<PendingExternalArtifactIdentifier>((Func<PendingExternalArtifactIdentifier, bool>) (i => i.ArtifactType == GitHubLinkItemType.Commit && !commits.Any<ExternalArtifactAndHydrationStatus<ExternalGitCommit>>((Func<ExternalArtifactAndHydrationStatus<ExternalGitCommit>, bool>) (c => string.Equals(c.ExternalArtifact.Sha, i.ArtifactId, StringComparison.OrdinalIgnoreCase))))).Select<PendingExternalArtifactIdentifier, ExternalArtifactAndHydrationStatus<ExternalGitCommit>>((Func<PendingExternalArtifactIdentifier, ExternalArtifactAndHydrationStatus<ExternalGitCommit>>) (c =>
              {
                ExternalArtifactAndHydrationStatus<ExternalGitCommit> andHydrationStatus = new ExternalArtifactAndHydrationStatus<ExternalGitCommit>();
                andHydrationStatus.ExternalArtifact = new ExternalGitCommit()
                {
                  Sha = c.ArtifactId,
                  Repo = new ExternalGitRepo().SetRepoInternalId(c.InternalRepositoryId)
                };
                ExternalArtifactHydrationStatusDetails hydrationStatusDetails1 = c.HydrationStatusDetails;
                andHydrationStatus.HydrationStatus = (hydrationStatusDetails1 != null ? (hydrationStatusDetails1.Attempts >= 3 ? 1 : 0) : 0) != 0 ? ExternalArtifactHydrationStatus.Failed : c.HydrationStatus;
                ExternalArtifactHydrationStatusDetails hydrationStatusDetails2 = new ExternalArtifactHydrationStatusDetails();
                ExternalArtifactHydrationStatusDetails hydrationStatusDetails3 = c.HydrationStatusDetails;
                hydrationStatusDetails2.Attempts = (hydrationStatusDetails3 != null ? hydrationStatusDetails3.Attempts : 0) + 1;
                hydrationStatusDetails2.Message = "Unable to resolve item from GitHub";
                andHydrationStatus.HydrationStatusDetails = hydrationStatusDetails2;
                andHydrationStatus.UpdateOnly = true;
                return andHydrationStatus;
              })).ToList<ExternalArtifactAndHydrationStatus<ExternalGitCommit>>();
              List<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>> list2 = source2.Where<PendingExternalArtifactIdentifier>((Func<PendingExternalArtifactIdentifier, bool>) (i => i.ArtifactType == GitHubLinkItemType.PullRequest && !pullRequests.Any<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>((Func<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>, bool>) (pr => string.Equals(pr.ExternalArtifact.Number, i.ArtifactId, StringComparison.OrdinalIgnoreCase))))).Select<PendingExternalArtifactIdentifier, ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>((Func<PendingExternalArtifactIdentifier, ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>) (pr =>
              {
                ExternalArtifactAndHydrationStatus<ExternalGitPullRequest> andHydrationStatus = new ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>();
                andHydrationStatus.ExternalArtifact = new ExternalGitPullRequest()
                {
                  Number = pr.ArtifactId,
                  Repo = new ExternalGitRepo().SetRepoInternalId(pr.InternalRepositoryId)
                };
                ExternalArtifactHydrationStatusDetails hydrationStatusDetails4 = pr.HydrationStatusDetails;
                andHydrationStatus.HydrationStatus = (hydrationStatusDetails4 != null ? (hydrationStatusDetails4.Attempts >= 3 ? 1 : 0) : 0) != 0 ? ExternalArtifactHydrationStatus.Failed : pr.HydrationStatus;
                ExternalArtifactHydrationStatusDetails hydrationStatusDetails5 = new ExternalArtifactHydrationStatusDetails();
                ExternalArtifactHydrationStatusDetails hydrationStatusDetails6 = pr.HydrationStatusDetails;
                hydrationStatusDetails5.Attempts = (hydrationStatusDetails6 != null ? hydrationStatusDetails6.Attempts : 0) + 1;
                hydrationStatusDetails5.Message = "Unable to resolve item from GitHub";
                andHydrationStatus.HydrationStatusDetails = hydrationStatusDetails5;
                andHydrationStatus.UpdateOnly = true;
                return andHydrationStatus;
              })).ToList<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>();
              List<ExternalArtifactAndHydrationStatus<ExternalGitIssue>> list3 = source2.Where<PendingExternalArtifactIdentifier>((Func<PendingExternalArtifactIdentifier, bool>) (i => i.ArtifactType == GitHubLinkItemType.Issue && !issues.Any<ExternalArtifactAndHydrationStatus<ExternalGitIssue>>((Func<ExternalArtifactAndHydrationStatus<ExternalGitIssue>, bool>) (issue => string.Equals(issue.ExternalArtifact.Number, i.ArtifactId, StringComparison.OrdinalIgnoreCase))))).Select<PendingExternalArtifactIdentifier, ExternalArtifactAndHydrationStatus<ExternalGitIssue>>((Func<PendingExternalArtifactIdentifier, ExternalArtifactAndHydrationStatus<ExternalGitIssue>>) (issue =>
              {
                ExternalArtifactAndHydrationStatus<ExternalGitIssue> andHydrationStatus = new ExternalArtifactAndHydrationStatus<ExternalGitIssue>();
                andHydrationStatus.ExternalArtifact = new ExternalGitIssue()
                {
                  Number = issue.ArtifactId,
                  Repo = new ExternalGitRepo().SetRepoInternalId(issue.InternalRepositoryId)
                };
                ExternalArtifactHydrationStatusDetails hydrationStatusDetails7 = issue.HydrationStatusDetails;
                andHydrationStatus.HydrationStatus = (hydrationStatusDetails7 != null ? (hydrationStatusDetails7.Attempts >= 3 ? 1 : 0) : 0) != 0 ? ExternalArtifactHydrationStatus.Failed : issue.HydrationStatus;
                ExternalArtifactHydrationStatusDetails hydrationStatusDetails8 = new ExternalArtifactHydrationStatusDetails();
                ExternalArtifactHydrationStatusDetails hydrationStatusDetails9 = issue.HydrationStatusDetails;
                hydrationStatusDetails8.Attempts = (hydrationStatusDetails9 != null ? hydrationStatusDetails9.Attempts : 0) + 1;
                hydrationStatusDetails8.Message = "Unable to resolve item from GitHub";
                andHydrationStatus.HydrationStatusDetails = hydrationStatusDetails8;
                andHydrationStatus.UpdateOnly = true;
                return andHydrationStatus;
              })).ToList<ExternalArtifactAndHydrationStatus<ExternalGitIssue>>();
              if (list1.Any<ExternalArtifactAndHydrationStatus<ExternalGitCommit>>())
              {
                commits.AddRange((IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitCommit>>) list1);
                int num1 = list1.Count<ExternalArtifactAndHydrationStatus<ExternalGitCommit>>((Func<ExternalArtifactAndHydrationStatus<ExternalGitCommit>, bool>) (c => c.HydrationStatus != ExternalArtifactHydrationStatus.Failed));
                int num2 = list1.Count<ExternalArtifactAndHydrationStatus<ExternalGitCommit>>((Func<ExternalArtifactAndHydrationStatus<ExternalGitCommit>, bool>) (c => c.HydrationStatus == ExternalArtifactHydrationStatus.Failed));
                artifactResolverResult.UnresolvedCommits += num1;
                artifactResolverResult.FailedCommits += num2;
                if (num2 > 0)
                  artifactResolverResult.FailedRepositories.Add(internalRepositoryId);
              }
              if (list2.Any<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>())
              {
                pullRequests.AddRange((IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>) list2);
                int num3 = list2.Count<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>((Func<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>, bool>) (pr => pr.HydrationStatus != ExternalArtifactHydrationStatus.Failed));
                int num4 = list2.Count<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>((Func<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>, bool>) (pr => pr.HydrationStatus == ExternalArtifactHydrationStatus.Failed));
                artifactResolverResult.UnresolvedPullRequests += num3;
                artifactResolverResult.FailedPullRequests += num4;
                if (num4 > 0)
                  artifactResolverResult.FailedRepositories.Add(internalRepositoryId);
              }
              if (list3.Any<ExternalArtifactAndHydrationStatus<ExternalGitIssue>>())
              {
                issues.AddRange((IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitIssue>>) list3);
                int num5 = list3.Count<ExternalArtifactAndHydrationStatus<ExternalGitIssue>>((Func<ExternalArtifactAndHydrationStatus<ExternalGitIssue>, bool>) (issue => issue.HydrationStatus != ExternalArtifactHydrationStatus.Failed));
                int num6 = list3.Count<ExternalArtifactAndHydrationStatus<ExternalGitIssue>>((Func<ExternalArtifactAndHydrationStatus<ExternalGitIssue>, bool>) (issue => issue.HydrationStatus == ExternalArtifactHydrationStatus.Failed));
                artifactResolverResult.UnresolvedIssues += num5;
                artifactResolverResult.FailedIssues += num6;
                if (num6 > 0)
                  artifactResolverResult.FailedRepositories.Add(internalRepositoryId);
              }
              ExternalArtifactCollectionWithStatus artifacts = new ExternalArtifactCollectionWithStatus()
              {
                Commits = (IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitCommit>>) commits,
                PullRequests = (IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>) pullRequests,
                Issues = (IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitIssue>>) issues
              };
              service2.SaveArtifacts(requestContext, externalConnection.ProviderKey, internalRepositoryId, artifacts);
            }
          }
        }
      }
      return artifactResolverResult;
    }
  }
}
