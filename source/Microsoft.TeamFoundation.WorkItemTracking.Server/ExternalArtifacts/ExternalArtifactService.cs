// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.ExternalArtifactService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.Components;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.ServiceModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts
{
  public class ExternalArtifactService : 
    BaseTeamFoundationWorkItemTrackingService,
    IExternalArtifactService,
    IVssFrameworkService
  {
    private static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
    {
      ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
    };

    public virtual ExternalArtifactCollectionWithStatus GetArtifacts(
      IVssRequestContext requestContext,
      IEnumerable<(Guid repositoryId, string sha)> repositoryAndShas,
      IEnumerable<(Guid repositoryId, string prNumber)> repositoryAndPrNumbers,
      IEnumerable<(Guid repositoryId, string issueNumber)> repositoryAndIssueNumbers)
    {
      return this.ExecuteSql<ExternalArtifactCollectionWithStatus, ExternalArtifactSqlComponent>(requestContext, (Func<ExternalArtifactSqlComponent, ExternalArtifactCollectionWithStatus>) (component =>
      {
        ExternalArtifactResult artifacts1 = component.GetArtifacts(repositoryAndShas, repositoryAndPrNumbers, repositoryAndIssueNumbers);
        List<ExternalArtifactAndHydrationStatus<ExternalGitCommit>> list1 = artifacts1.Commits.Select<ExternalCommitDataset, ExternalArtifactAndHydrationStatus<ExternalGitCommit>>((Func<ExternalCommitDataset, ExternalArtifactAndHydrationStatus<ExternalGitCommit>>) (c =>
        {
          ExternalArtifactAndHydrationStatus<ExternalGitCommit> artifacts2 = new ExternalArtifactAndHydrationStatus<ExternalGitCommit>();
          ExternalGitCommit externalGitCommit = new ExternalGitCommit();
          externalGitCommit.Sha = c.ArtifactId;
          externalGitCommit.Message = c.Message;
          DateTime? nullable = c.CommittedDate;
          SqlDateTime minValue;
          DateTime valueOrDefault3;
          if (!nullable.HasValue)
          {
            minValue = SqlDateTime.MinValue;
            valueOrDefault3 = minValue.Value;
          }
          else
            valueOrDefault3 = nullable.GetValueOrDefault();
          externalGitCommit.CommitedDate = valueOrDefault3;
          nullable = c.PushedDate;
          DateTime valueOrDefault4;
          if (!nullable.HasValue)
          {
            minValue = SqlDateTime.MinValue;
            valueOrDefault4 = minValue.Value;
          }
          else
            valueOrDefault4 = nullable.GetValueOrDefault();
          externalGitCommit.PushedDate = new DateTime?(valueOrDefault4);
          externalGitCommit.Author = new ExternalGitUser()
          {
            Email = c.AuthorEmail,
            AvatarUrl = c.AuthorAvatarUrl
          }.SetNodeId<ExternalGitUser>(c.AuthorId).SetName(c.AuthorName).SetLogin(c.AuthorLogin);
          externalGitCommit.Repo = new ExternalGitRepo()
          {
            WebUrl = c.RepositoryUrl
          }.SetNodeId<ExternalGitRepo>(c.ExternalRepositoryId).SetRepoInternalId(c.InternalRepositoryId).SetRepoNameWithOwner(c.RepositoryNameWithOwner);
          externalGitCommit.WebUrl = c.Url;
          artifacts2.ExternalArtifact = externalGitCommit;
          artifacts2.HydrationStatus = (ExternalArtifactHydrationStatus) c.HydrationStatus;
          artifacts2.HydrationStatusDetails = !string.IsNullOrEmpty(c.HydrationStatusDetails) ? JsonConvert.DeserializeObject<ExternalArtifactHydrationStatusDetails>(c.HydrationStatusDetails, ExternalArtifactService.jsonSerializerSettings) : (ExternalArtifactHydrationStatusDetails) null;
          return artifacts2;
        })).ToList<ExternalArtifactAndHydrationStatus<ExternalGitCommit>>();
        List<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>> list2 = artifacts1.PullRequests.Select<ExternalPullRequestDataset, ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>((Func<ExternalPullRequestDataset, ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>) (pr =>
        {
          ExternalArtifactAndHydrationStatus<ExternalGitPullRequest> artifacts3 = new ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>();
          ExternalGitPullRequest externalGitPullRequest = new ExternalGitPullRequest();
          externalGitPullRequest.Number = pr.ArtifactId;
          externalGitPullRequest.Id = pr.SecondaryId;
          DateTime? nullable = pr.CreatedDate;
          ref DateTime? local8 = ref nullable;
          DateTime valueOrDefault;
          string str8;
          if (!local8.HasValue)
          {
            str8 = (string) null;
          }
          else
          {
            valueOrDefault = local8.GetValueOrDefault();
            str8 = valueOrDefault.ToString("s");
          }
          externalGitPullRequest.CreatedAt = str8;
          nullable = pr.UpdatedDate;
          ref DateTime? local9 = ref nullable;
          string str9;
          if (!local9.HasValue)
          {
            str9 = (string) null;
          }
          else
          {
            valueOrDefault = local9.GetValueOrDefault();
            str9 = valueOrDefault.ToString("s");
          }
          externalGitPullRequest.UpdatedAt = str9;
          nullable = pr.MergedDate;
          ref DateTime? local10 = ref nullable;
          string str10;
          if (!local10.HasValue)
          {
            str10 = (string) null;
          }
          else
          {
            valueOrDefault = local10.GetValueOrDefault();
            str10 = valueOrDefault.ToString("s");
          }
          externalGitPullRequest.MergedAt = str10;
          nullable = pr.ClosedDate;
          ref DateTime? local11 = ref nullable;
          string str11;
          if (!local11.HasValue)
          {
            str11 = (string) null;
          }
          else
          {
            valueOrDefault = local11.GetValueOrDefault();
            str11 = valueOrDefault.ToString("s");
          }
          externalGitPullRequest.ClosedAt = str11;
          externalGitPullRequest.TargetRef = pr.Target;
          externalGitPullRequest.State = pr.State;
          externalGitPullRequest.Title = pr.Title;
          externalGitPullRequest.WebUrl = pr.Url;
          externalGitPullRequest.Sender = new ExternalGitUser()
          {
            AvatarUrl = pr.UserAvatarUrl
          }.SetNodeId<ExternalGitUser>(pr.UserId).SetName(pr.UserName).SetLogin(pr.UserLogin);
          externalGitPullRequest.Repo = new ExternalGitRepo()
          {
            WebUrl = pr.RepositoryUrl
          }.SetNodeId<ExternalGitRepo>(pr.ExternalRepositoryId).SetRepoInternalId(pr.InternalRepositoryId).SetRepoNameWithOwner(pr.RepositoryNameWithOwner);
          artifacts3.ExternalArtifact = externalGitPullRequest;
          artifacts3.HydrationStatus = (ExternalArtifactHydrationStatus) pr.HydrationStatus;
          artifacts3.HydrationStatusDetails = !string.IsNullOrEmpty(pr.HydrationStatusDetails) ? JsonConvert.DeserializeObject<ExternalArtifactHydrationStatusDetails>(pr.HydrationStatusDetails, ExternalArtifactService.jsonSerializerSettings) : (ExternalArtifactHydrationStatusDetails) null;
          return artifacts3;
        })).ToList<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>();
        IEnumerable<ExternalIssueDataset> issues = artifacts1.Issues;
        IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitIssue>> source = (issues != null ? (IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitIssue>>) issues.Select<ExternalIssueDataset, ExternalArtifactAndHydrationStatus<ExternalGitIssue>>((Func<ExternalIssueDataset, ExternalArtifactAndHydrationStatus<ExternalGitIssue>>) (issue =>
        {
          ExternalArtifactAndHydrationStatus<ExternalGitIssue> artifacts4 = new ExternalArtifactAndHydrationStatus<ExternalGitIssue>();
          ExternalGitIssue externalGitIssue = new ExternalGitIssue();
          externalGitIssue.Number = issue.ArtifactId;
          externalGitIssue.Id = issue.SecondaryId;
          DateTime? nullable = issue.CreatedDate;
          ref DateTime? local12 = ref nullable;
          DateTime valueOrDefault;
          string str12;
          if (!local12.HasValue)
          {
            str12 = (string) null;
          }
          else
          {
            valueOrDefault = local12.GetValueOrDefault();
            str12 = valueOrDefault.ToString("s");
          }
          externalGitIssue.CreatedAt = str12;
          nullable = issue.UpdatedDate;
          ref DateTime? local13 = ref nullable;
          string str13;
          if (!local13.HasValue)
          {
            str13 = (string) null;
          }
          else
          {
            valueOrDefault = local13.GetValueOrDefault();
            str13 = valueOrDefault.ToString("s");
          }
          externalGitIssue.UpdatedAt = str13;
          nullable = issue.ClosedDate;
          ref DateTime? local14 = ref nullable;
          string str14;
          if (!local14.HasValue)
          {
            str14 = (string) null;
          }
          else
          {
            valueOrDefault = local14.GetValueOrDefault();
            str14 = valueOrDefault.ToString("s");
          }
          externalGitIssue.ClosedAt = str14;
          externalGitIssue.State = issue.State;
          externalGitIssue.Title = issue.Title;
          externalGitIssue.WebUrl = issue.Url;
          externalGitIssue.Sender = new ExternalGitUser()
          {
            AvatarUrl = issue.UserAvatarUrl
          }.SetNodeId<ExternalGitUser>(issue.UserId).SetName(issue.UserName).SetLogin(issue.UserLogin);
          externalGitIssue.Repo = new ExternalGitRepo()
          {
            WebUrl = issue.RepositoryUrl
          }.SetNodeId<ExternalGitRepo>(issue.ExternalRepositoryId).SetRepoInternalId(issue.InternalRepositoryId).SetRepoNameWithOwner(issue.RepositoryNameWithOwner);
          artifacts4.ExternalArtifact = externalGitIssue;
          artifacts4.HydrationStatus = (ExternalArtifactHydrationStatus) issue.HydrationStatus;
          artifacts4.HydrationStatusDetails = !string.IsNullOrEmpty(issue.HydrationStatusDetails) ? JsonConvert.DeserializeObject<ExternalArtifactHydrationStatusDetails>(issue.HydrationStatusDetails, ExternalArtifactService.jsonSerializerSettings) : (ExternalArtifactHydrationStatusDetails) null;
          return artifacts4;
        })).ToList<ExternalArtifactAndHydrationStatus<ExternalGitIssue>>() : (IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitIssue>>) null) ?? Enumerable.Empty<ExternalArtifactAndHydrationStatus<ExternalGitIssue>>();
        Dictionary<string, IExternalArtifact> dictionary1 = list2.ToDictionary<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>, string, IExternalArtifact>((Func<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>, string>) (pr => string.Format("{0}-{1}", (object) pr.ExternalArtifact.Repo.GetRepoInternalId(), (object) pr.ExternalArtifact.Number)), (Func<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>, IExternalArtifact>) (pr => (IExternalArtifact) pr.ExternalArtifact));
        Dictionary<string, IExternalArtifact> dictionary2 = source.ToDictionary<ExternalArtifactAndHydrationStatus<ExternalGitIssue>, string, IExternalArtifact>((Func<ExternalArtifactAndHydrationStatus<ExternalGitIssue>, string>) (issue => string.Format("{0}-{1}", (object) issue.ExternalArtifact.Repo.GetRepoInternalId(), (object) issue.ExternalArtifact.Number)), (Func<ExternalArtifactAndHydrationStatus<ExternalGitIssue>, IExternalArtifact>) (issue => (IExternalArtifact) issue.ExternalArtifact));
        dictionary1.TryAddRange<string, IExternalArtifact, Dictionary<string, IExternalArtifact>>((IEnumerable<KeyValuePair<string, IExternalArtifact>>) dictionary2);
        foreach (ExternalArtifactUserResultSet user in artifacts1.Users)
        {
          IExternalArtifact externalArtifact;
          if (dictionary1.TryGetValue(string.Format("{0}-{1}", (object) user.InternalRepositoryId, (object) user.ArtifactId), out externalArtifact))
          {
            ICollection<ExternalGitUser> externalGitUsers = (ICollection<ExternalGitUser>) null;
            switch (user.RelationshipType)
            {
              case ExternalArtifactRelationshipType.PullRequestAssignee:
                ExternalGitPullRequest externalGitPullRequest = (ExternalGitPullRequest) externalArtifact;
                if (externalGitPullRequest.Assignees == null)
                  externalGitPullRequest.Assignees = (ICollection<ExternalGitUser>) new List<ExternalGitUser>();
                externalGitUsers = externalGitPullRequest.Assignees;
                break;
              case ExternalArtifactRelationshipType.IssueAssignee:
                ExternalGitIssue externalGitIssue = (ExternalGitIssue) externalArtifact;
                if (externalGitIssue.Assignees == null)
                  externalGitIssue.Assignees = (ICollection<ExternalGitUser>) new List<ExternalGitUser>();
                externalGitUsers = externalGitIssue.Assignees;
                break;
            }
            if (externalGitUsers != null)
              externalGitUsers.Add(new ExternalGitUser()
              {
                AvatarUrl = user.UserAvatarUrl
              }.SetNodeId<ExternalGitUser>(user.UserId).SetName(user.UserName).SetLogin(user.UserLogin));
          }
        }
        return new ExternalArtifactCollectionWithStatus()
        {
          Commits = (IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitCommit>>) list1,
          PullRequests = (IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>) list2,
          Issues = source
        };
      }));
    }

    public virtual IEnumerable<PendingExternalArtifactIdentifier> GetPendingArtifacts(
      IVssRequestContext requestContext)
    {
      return this.ExecuteSql<IEnumerable<PendingExternalArtifactIdentifier>, ExternalArtifactSqlComponent>(requestContext, (Func<ExternalArtifactSqlComponent, IEnumerable<PendingExternalArtifactIdentifier>>) (component =>
      {
        PendingExternalArtifactResult pendingArtifacts = component.GetPendingArtifacts();
        IEnumerable<PendingExternalArtifactIdentifier> first = pendingArtifacts.Commits.Select<PendingExternalArtifactDataSet, PendingExternalArtifactIdentifier>((Func<PendingExternalArtifactDataSet, PendingExternalArtifactIdentifier>) (c => new PendingExternalArtifactIdentifier()
        {
          ArtifactId = c.ArtifactId,
          ArtifactType = GitHubLinkItemType.Commit,
          ExternalRepositoryId = c.ExternalRepositoryId,
          InternalRepositoryId = c.InternalRepositoryId,
          ProviderKey = c.ProviderKey,
          HydrationStatusDetails = !string.IsNullOrEmpty(c.HydrationStatusDetails) ? JsonConvert.DeserializeObject<ExternalArtifactHydrationStatusDetails>(c.HydrationStatusDetails, ExternalArtifactService.jsonSerializerSettings) : (ExternalArtifactHydrationStatusDetails) null
        }));
        IEnumerable<PendingExternalArtifactIdentifier> second1 = pendingArtifacts.PullRequests.Select<PendingExternalArtifactDataSet, PendingExternalArtifactIdentifier>((Func<PendingExternalArtifactDataSet, PendingExternalArtifactIdentifier>) (pr => new PendingExternalArtifactIdentifier()
        {
          ArtifactId = pr.ArtifactId,
          ArtifactType = GitHubLinkItemType.PullRequest,
          ExternalRepositoryId = pr.ExternalRepositoryId,
          InternalRepositoryId = pr.InternalRepositoryId,
          ProviderKey = pr.ProviderKey,
          HydrationStatusDetails = !string.IsNullOrEmpty(pr.HydrationStatusDetails) ? JsonConvert.DeserializeObject<ExternalArtifactHydrationStatusDetails>(pr.HydrationStatusDetails, ExternalArtifactService.jsonSerializerSettings) : (ExternalArtifactHydrationStatusDetails) null
        }));
        IEnumerable<PendingExternalArtifactDataSet> issues = pendingArtifacts.Issues;
        IEnumerable<PendingExternalArtifactIdentifier> second2 = (issues != null ? issues.Select<PendingExternalArtifactDataSet, PendingExternalArtifactIdentifier>((Func<PendingExternalArtifactDataSet, PendingExternalArtifactIdentifier>) (Issue => new PendingExternalArtifactIdentifier()
        {
          ArtifactId = Issue.ArtifactId,
          ArtifactType = GitHubLinkItemType.Issue,
          ExternalRepositoryId = Issue.ExternalRepositoryId,
          InternalRepositoryId = Issue.InternalRepositoryId,
          ProviderKey = Issue.ProviderKey,
          HydrationStatusDetails = !string.IsNullOrEmpty(Issue.HydrationStatusDetails) ? JsonConvert.DeserializeObject<ExternalArtifactHydrationStatusDetails>(Issue.HydrationStatusDetails, ExternalArtifactService.jsonSerializerSettings) : (ExternalArtifactHydrationStatusDetails) null
        })) : (IEnumerable<PendingExternalArtifactIdentifier>) null) ?? Enumerable.Empty<PendingExternalArtifactIdentifier>();
        return (IEnumerable<PendingExternalArtifactIdentifier>) first.Union<PendingExternalArtifactIdentifier>(second1).Union<PendingExternalArtifactIdentifier>(second2).ToList<PendingExternalArtifactIdentifier>();
      }));
    }

    public virtual void SaveArtifacts(
      IVssRequestContext requestContext,
      string providerKey,
      Guid internalRepositoryId,
      ExternalArtifactCollectionWithStatus artifacts)
    {
      this.SaveArtifactsInternal(requestContext, providerKey, (Func<string, Guid>) (nodeId => internalRepositoryId), artifacts);
    }

    public virtual void SaveArtifacts(
      IVssRequestContext requestContext,
      string providerKey,
      IEnumerable<ExternalGitRepo> repos,
      ExternalArtifactCollectionWithStatus artifacts)
    {
      this.SaveArtifactsInternal(requestContext, providerKey, (Func<string, Guid>) (nodeId => repos.First<ExternalGitRepo>((Func<ExternalGitRepo, bool>) (r => r.NodeId() == nodeId)).GetRepoInternalId()), artifacts);
    }

    public virtual void SaveArtifactWatermarks(
      IVssRequestContext requestContext,
      ExternalArtifactCollectionWithStatus artifacts)
    {
      this.SaveArtifactWatermarksInternal(requestContext, artifacts);
    }

    private void SaveArtifactsInternal(
      IVssRequestContext requestContext,
      string providerKey,
      Func<string, Guid> getInternalRepositoryId,
      ExternalArtifactCollectionWithStatus artifacts,
      bool updateOnly = false)
    {
      HashSet<ExternalUserDataset> users = new HashSet<ExternalUserDataset>((IEqualityComparer<ExternalUserDataset>) ExternalUserDatasetComparer.Instance);
      HashSet<ExternalArtifactUserDataset> artifactUsers = new HashSet<ExternalArtifactUserDataset>((IEqualityComparer<ExternalArtifactUserDataset>) ExternalArtifactUsersDatasetComparer.Instance);
      IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>> pullRequests1 = artifacts.PullRequests;
      IEnumerable<ExternalUserDataset> other1 = pullRequests1 != null ? pullRequests1.Where<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>((Func<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>, bool>) (pr => pr?.ExternalArtifact?.Sender != null)).Select<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>, ExternalUserDataset>((Func<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>, ExternalUserDataset>) (pr => this.GetUserDataset(providerKey, pr.ExternalArtifact.Sender, pr.UpdateOnly, getInternalRepositoryId(pr.ExternalArtifact.Repo.NodeId()), pr.ExternalArtifact.Number, ArtifactType.PullRequest))) : (IEnumerable<ExternalUserDataset>) null;
      IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>> pullRequests2 = artifacts.PullRequests;
      IEnumerable<ExternalUserDataset> other2 = pullRequests2 != null ? pullRequests2.Where<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>((Func<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>, bool>) (pr => pr?.ExternalArtifact?.Assignees != null)).SelectMany<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>, ExternalUserDataset>((Func<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>, IEnumerable<ExternalUserDataset>>) (pr => pr.ExternalArtifact.Assignees.Select<ExternalGitUser, ExternalUserDataset>((Func<ExternalGitUser, ExternalUserDataset>) (u => this.GetUserDataset(providerKey, u, pr.UpdateOnly, getInternalRepositoryId(pr.ExternalArtifact.Repo.NodeId()), pr.ExternalArtifact.Number, ArtifactType.PullRequest))))) : (IEnumerable<ExternalUserDataset>) null;
      IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitIssue>> issues1 = artifacts.Issues;
      IEnumerable<ExternalUserDataset> other3 = issues1 != null ? issues1.Where<ExternalArtifactAndHydrationStatus<ExternalGitIssue>>((Func<ExternalArtifactAndHydrationStatus<ExternalGitIssue>, bool>) (issue => issue?.ExternalArtifact?.Sender != null)).Select<ExternalArtifactAndHydrationStatus<ExternalGitIssue>, ExternalUserDataset>((Func<ExternalArtifactAndHydrationStatus<ExternalGitIssue>, ExternalUserDataset>) (issue => this.GetUserDataset(providerKey, issue.ExternalArtifact.Sender, issue.UpdateOnly, getInternalRepositoryId(issue.ExternalArtifact.Repo.NodeId()), issue.ExternalArtifact.Number, ArtifactType.Issue))) : (IEnumerable<ExternalUserDataset>) null;
      IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitIssue>> issues2 = artifacts.Issues;
      IEnumerable<ExternalUserDataset> other4 = issues2 != null ? issues2.Where<ExternalArtifactAndHydrationStatus<ExternalGitIssue>>((Func<ExternalArtifactAndHydrationStatus<ExternalGitIssue>, bool>) (issue => issue?.ExternalArtifact?.Assignees != null)).SelectMany<ExternalArtifactAndHydrationStatus<ExternalGitIssue>, ExternalUserDataset>((Func<ExternalArtifactAndHydrationStatus<ExternalGitIssue>, IEnumerable<ExternalUserDataset>>) (issue => issue.ExternalArtifact.Assignees.Select<ExternalGitUser, ExternalUserDataset>((Func<ExternalGitUser, ExternalUserDataset>) (u => this.GetUserDataset(providerKey, u, issue.UpdateOnly, getInternalRepositoryId(issue.ExternalArtifact.Repo.NodeId()), issue.ExternalArtifact.Number, ArtifactType.Issue))))) : (IEnumerable<ExternalUserDataset>) null;
      if (other1 != null)
        users.UnionWith(other1);
      if (other2 != null)
        users.UnionWith(other2);
      if (other3 != null)
        users.UnionWith(other3);
      if (other4 != null)
        users.UnionWith(other4);
      IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>> pullRequests3 = artifacts.PullRequests;
      IEnumerable<ExternalArtifactUserDataset> other5 = pullRequests3 != null ? pullRequests3.Where<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>((Func<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>, bool>) (pr => pr.ExternalArtifact?.Assignees != null)).SelectMany<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>, ExternalArtifactUserDataset>((Func<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>, IEnumerable<ExternalArtifactUserDataset>>) (pr => pr.ExternalArtifact.Assignees.Select<ExternalGitUser, ExternalArtifactUserDataset>((Func<ExternalGitUser, ExternalArtifactUserDataset>) (r => new ExternalArtifactUserDataset(getInternalRepositoryId(pr.ExternalArtifact.Repo.NodeId()), pr.ExternalArtifact.Number, r.NodeId<ExternalGitUser>(), ExternalArtifactRelationshipType.PullRequestAssignee, pr.UpdateOnly))))) : (IEnumerable<ExternalArtifactUserDataset>) null;
      IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>> pullRequests4 = artifacts.PullRequests;
      IEnumerable<ExternalArtifactUserDataset> other6 = pullRequests4 != null ? pullRequests4.Where<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>((Func<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>, bool>) (pr =>
      {
        if (pr.ExternalArtifact == null)
          return false;
        return pr.ExternalArtifact.Assignees == null || !pr.ExternalArtifact.Assignees.Any<ExternalGitUser>();
      })).Select<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>, ExternalArtifactUserDataset>((Func<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>, ExternalArtifactUserDataset>) (pr => new ExternalArtifactUserDataset(getInternalRepositoryId(pr.ExternalArtifact.Repo.NodeId()), pr.ExternalArtifact.Number, (string) null, ExternalArtifactRelationshipType.PullRequestAssignee, pr.UpdateOnly))) : (IEnumerable<ExternalArtifactUserDataset>) null;
      if (other5 != null)
        artifactUsers.UnionWith(other5);
      if (other6 != null)
        artifactUsers.UnionWith(other6);
      IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitIssue>> issues3 = artifacts.Issues;
      IEnumerable<ExternalArtifactUserDataset> other7 = issues3 != null ? issues3.Where<ExternalArtifactAndHydrationStatus<ExternalGitIssue>>((Func<ExternalArtifactAndHydrationStatus<ExternalGitIssue>, bool>) (issue => issue.ExternalArtifact?.Assignees != null)).SelectMany<ExternalArtifactAndHydrationStatus<ExternalGitIssue>, ExternalArtifactUserDataset>((Func<ExternalArtifactAndHydrationStatus<ExternalGitIssue>, IEnumerable<ExternalArtifactUserDataset>>) (issue => issue.ExternalArtifact.Assignees.Select<ExternalGitUser, ExternalArtifactUserDataset>((Func<ExternalGitUser, ExternalArtifactUserDataset>) (r => new ExternalArtifactUserDataset(getInternalRepositoryId(issue.ExternalArtifact.Repo.NodeId()), issue.ExternalArtifact.Number, r.NodeId<ExternalGitUser>(), ExternalArtifactRelationshipType.IssueAssignee, issue.UpdateOnly))))) : (IEnumerable<ExternalArtifactUserDataset>) null;
      IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitIssue>> issues4 = artifacts.Issues;
      IEnumerable<ExternalArtifactUserDataset> other8 = issues4 != null ? issues4.Where<ExternalArtifactAndHydrationStatus<ExternalGitIssue>>((Func<ExternalArtifactAndHydrationStatus<ExternalGitIssue>, bool>) (issue =>
      {
        if (issue.ExternalArtifact == null)
          return false;
        return issue.ExternalArtifact.Assignees == null || !issue.ExternalArtifact.Assignees.Any<ExternalGitUser>();
      })).Select<ExternalArtifactAndHydrationStatus<ExternalGitIssue>, ExternalArtifactUserDataset>((Func<ExternalArtifactAndHydrationStatus<ExternalGitIssue>, ExternalArtifactUserDataset>) (issue => new ExternalArtifactUserDataset(getInternalRepositoryId(issue.ExternalArtifact.Repo.NodeId()), issue.ExternalArtifact.Number, (string) null, ExternalArtifactRelationshipType.IssueAssignee, issue.UpdateOnly))) : (IEnumerable<ExternalArtifactUserDataset>) null;
      if (other7 != null)
        artifactUsers.UnionWith(other7);
      if (other8 != null)
        artifactUsers.UnionWith(other8);
      (IEnumerable<ExternalCommitDataset> commits, IEnumerable<ExternalPullRequestDataset> pullRequests5, IEnumerable<ExternalIssueDataset> issues5) = this.CreateDataset(artifacts, getInternalRepositoryId);
      this.ExecuteSql<ExternalArtifactSqlComponent>(requestContext, (Action<ExternalArtifactSqlComponent>) (component => component.SaveArtifacts((IEnumerable<ExternalUserDataset>) users, (IEnumerable<ExternalArtifactUserDataset>) artifactUsers, commits, pullRequests5, issues5)));
    }

    private (IEnumerable<ExternalCommitDataset> commits, IEnumerable<ExternalPullRequestDataset> pullRequests, IEnumerable<ExternalIssueDataset> issues) CreateDataset(
      ExternalArtifactCollectionWithStatus artifacts,
      Func<string, Guid> getInternalRepositoryId)
    {
      IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitCommit>> commits = artifacts.Commits;
      IEnumerable<ExternalCommitDataset> externalCommitDatasets = commits != null ? commits.Select<ExternalArtifactAndHydrationStatus<ExternalGitCommit>, ExternalCommitDataset>((Func<ExternalArtifactAndHydrationStatus<ExternalGitCommit>, ExternalCommitDataset>) (c => this.CreateDataset(c.ExternalArtifact, c.HydrationStatus, c.HydrationStatusDetails, c.UpdateOnly, getInternalRepositoryId(c.ExternalArtifact.Repo.NodeId())))) : (IEnumerable<ExternalCommitDataset>) null;
      IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>> pullRequests = artifacts.PullRequests;
      IEnumerable<ExternalPullRequestDataset> pullRequestDatasets1 = pullRequests != null ? pullRequests.Select<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>, ExternalPullRequestDataset>((Func<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>, ExternalPullRequestDataset>) (pr => this.CreateDataset(pr.ExternalArtifact, pr.HydrationStatus, pr.HydrationStatusDetails, pr.UpdateOnly, getInternalRepositoryId(pr.ExternalArtifact.Repo.NodeId())))) : (IEnumerable<ExternalPullRequestDataset>) null;
      IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitIssue>> issues = artifacts.Issues;
      IEnumerable<ExternalIssueDataset> externalIssueDatasets1 = issues != null ? issues.Select<ExternalArtifactAndHydrationStatus<ExternalGitIssue>, ExternalIssueDataset>((Func<ExternalArtifactAndHydrationStatus<ExternalGitIssue>, ExternalIssueDataset>) (i => this.CreateDataset(i.ExternalArtifact, i.HydrationStatus, i.HydrationStatusDetails, i.UpdateOnly, getInternalRepositoryId(i.ExternalArtifact.Repo.NodeId())))) : (IEnumerable<ExternalIssueDataset>) null;
      IEnumerable<ExternalPullRequestDataset> pullRequestDatasets2 = pullRequestDatasets1;
      IEnumerable<ExternalIssueDataset> externalIssueDatasets2 = externalIssueDatasets1;
      return (externalCommitDatasets, pullRequestDatasets2, externalIssueDatasets2);
    }

    private ExternalCommitDataset CreateDataset(
      ExternalGitCommit commit,
      ExternalArtifactHydrationStatus hydrationStatus,
      ExternalArtifactHydrationStatusDetails hydrationStatusDetails,
      bool updateOnly,
      Guid internalId)
    {
      ExternalCommitDataset dataset = new ExternalCommitDataset();
      dataset.InternalRepositoryId = internalId;
      dataset.ArtifactId = commit.Sha;
      dataset.SecondaryId = commit.NodeId<ExternalGitCommit>();
      ExternalGitUser author1 = commit.Author;
      dataset.AuthorId = author1 != null ? author1.NodeId<ExternalGitUser>() : (string) null;
      ExternalGitUser author2 = commit.Author;
      dataset.AuthorName = author2 != null ? author2.Name() : (string) null;
      ExternalGitUser author3 = commit.Author;
      dataset.AuthorLogin = author3 != null ? author3.Login() : (string) null;
      dataset.AuthorEmail = commit.Author?.Email;
      dataset.AuthorAvatarUrl = commit.Author?.AvatarUrl;
      dataset.CommittedDate = this.GetNullableDate(commit.CommitedDate);
      dataset.PushedDate = commit.PushedDate;
      dataset.Message = commit.Message;
      dataset.Url = commit.WebUrl;
      dataset.HydrationStatus = (byte) hydrationStatus;
      dataset.HydrationStatusDetails = hydrationStatusDetails != null ? JsonConvert.SerializeObject((object) hydrationStatusDetails, ExternalArtifactService.jsonSerializerSettings) : (string) null;
      dataset.UpdateOnly = updateOnly;
      return dataset;
    }

    private ExternalPullRequestDataset CreateDataset(
      ExternalGitPullRequest pullRequest,
      ExternalArtifactHydrationStatus hydrationStatus,
      ExternalArtifactHydrationStatusDetails hydrationStatusDetails,
      bool updateOnly,
      Guid internalId)
    {
      ExternalPullRequestDataset dataset = new ExternalPullRequestDataset();
      dataset.InternalRepositoryId = internalId;
      dataset.ArtifactId = pullRequest.Number;
      dataset.SecondaryId = pullRequest.NodeId();
      ExternalGitUser sender = pullRequest.Sender;
      dataset.UserId = sender != null ? sender.NodeId<ExternalGitUser>() : (string) null;
      dataset.Title = pullRequest.Title;
      dataset.Target = pullRequest.TargetRef;
      dataset.State = pullRequest.State;
      dataset.CreatedDate = this.GetNullableDate(pullRequest.CreatedAt);
      dataset.UpdatedDate = this.GetNullableDate(pullRequest.UpdatedAt);
      dataset.MergedDate = this.GetNullableDate(pullRequest.MergedAt);
      dataset.ClosedDate = this.GetNullableDate(pullRequest.ClosedAt);
      dataset.Url = pullRequest.WebUrl;
      dataset.HydrationStatus = (byte) hydrationStatus;
      dataset.HydrationStatusDetails = hydrationStatusDetails != null ? JsonConvert.SerializeObject((object) hydrationStatusDetails, ExternalArtifactService.jsonSerializerSettings) : (string) null;
      dataset.UpdateOnly = updateOnly;
      return dataset;
    }

    private ExternalIssueDataset CreateDataset(
      ExternalGitIssue issue,
      ExternalArtifactHydrationStatus hydrationStatus,
      ExternalArtifactHydrationStatusDetails hydrationStatusDetails,
      bool updateOnly,
      Guid internalId)
    {
      ExternalIssueDataset dataset = new ExternalIssueDataset();
      dataset.InternalRepositoryId = internalId;
      dataset.ArtifactId = issue.Number;
      dataset.SecondaryId = issue.NodeId<ExternalGitIssue>();
      ExternalGitUser sender = issue.Sender;
      dataset.UserId = sender != null ? sender.NodeId<ExternalGitUser>() : (string) null;
      dataset.Title = issue.Title;
      dataset.State = issue.State;
      dataset.CreatedDate = this.GetNullableDate(issue.CreatedAt);
      dataset.UpdatedDate = this.GetNullableDate(issue.UpdatedAt);
      dataset.ClosedDate = this.GetNullableDate(issue.ClosedAt);
      dataset.Url = issue.WebUrl;
      dataset.HydrationStatus = (byte) hydrationStatus;
      dataset.HydrationStatusDetails = hydrationStatusDetails != null ? JsonConvert.SerializeObject((object) hydrationStatusDetails, ExternalArtifactService.jsonSerializerSettings) : (string) null;
      dataset.UpdateOnly = updateOnly;
      return dataset;
    }

    private void SaveArtifactWatermarksInternal(
      IVssRequestContext requestContext,
      ExternalArtifactCollectionWithStatus artifacts)
    {
      IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitCommit>> commits1 = artifacts.Commits;
      IEnumerable<ExternalCommitDataset> commits = commits1 != null ? commits1.Select<ExternalArtifactAndHydrationStatus<ExternalGitCommit>, ExternalCommitDataset>((Func<ExternalArtifactAndHydrationStatus<ExternalGitCommit>, ExternalCommitDataset>) (c =>
      {
        return new ExternalCommitDataset()
        {
          InternalRepositoryId = c.ExternalArtifact.Repo.GetRepoInternalId(),
          ArtifactId = c.ExternalArtifact.Sha,
          HydrationStatus = (byte) c.HydrationStatus,
          HydrationStatusDetails = c.HydrationStatusDetails != null ? JsonConvert.SerializeObject((object) c.HydrationStatusDetails, ExternalArtifactService.jsonSerializerSettings) : (string) null,
          UpdateOnly = false
        };
      })) : (IEnumerable<ExternalCommitDataset>) null;
      IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>> pullRequests1 = artifacts.PullRequests;
      IEnumerable<ExternalPullRequestDataset> pullRequests = pullRequests1 != null ? pullRequests1.Select<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>, ExternalPullRequestDataset>((Func<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>, ExternalPullRequestDataset>) (pr =>
      {
        return new ExternalPullRequestDataset()
        {
          InternalRepositoryId = pr.ExternalArtifact.Repo.GetRepoInternalId(),
          ArtifactId = pr.ExternalArtifact.Number,
          HydrationStatus = (byte) pr.HydrationStatus,
          HydrationStatusDetails = pr.HydrationStatusDetails != null ? JsonConvert.SerializeObject((object) pr.HydrationStatusDetails, ExternalArtifactService.jsonSerializerSettings) : (string) null,
          UpdateOnly = false
        };
      })) : (IEnumerable<ExternalPullRequestDataset>) null;
      IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitIssue>> issues1 = artifacts.Issues;
      IEnumerable<ExternalIssueDataset> issues = issues1 != null ? issues1.Select<ExternalArtifactAndHydrationStatus<ExternalGitIssue>, ExternalIssueDataset>((Func<ExternalArtifactAndHydrationStatus<ExternalGitIssue>, ExternalIssueDataset>) (issue =>
      {
        return new ExternalIssueDataset()
        {
          InternalRepositoryId = issue.ExternalArtifact.Repo.GetRepoInternalId(),
          ArtifactId = issue.ExternalArtifact.Number,
          HydrationStatus = (byte) issue.HydrationStatus,
          HydrationStatusDetails = issue.HydrationStatusDetails != null ? JsonConvert.SerializeObject((object) issue.HydrationStatusDetails, ExternalArtifactService.jsonSerializerSettings) : (string) null,
          UpdateOnly = false
        };
      })) : (IEnumerable<ExternalIssueDataset>) null;
      this.ExecuteSql<ExternalArtifactSqlComponent>(requestContext, (Action<ExternalArtifactSqlComponent>) (component => component.SaveArtifacts((IEnumerable<ExternalUserDataset>) null, (IEnumerable<ExternalArtifactUserDataset>) null, commits, pullRequests, issues)));
    }

    private ExternalUserDataset GetUserDataset(
      string providerKey,
      ExternalGitUser user,
      bool updateOnly,
      Guid relatedInternRepoId,
      string relatedArtifactId,
      ArtifactType relatedArtifactType)
    {
      return new ExternalUserDataset()
      {
        ProviderKey = providerKey,
        UserId = user.NodeId<ExternalGitUser>(),
        UserLogin = user.Login(),
        UserAvatarUrl = user.AvatarUrl,
        UpdateOnly = updateOnly,
        RelatedArtifactId = relatedArtifactId,
        RelatedInternalRepositoryId = relatedInternRepoId,
        RelatedArtifactType = relatedArtifactType
      };
    }

    private DateTime? GetNullableDate(DateTime date) => date == DateTime.MinValue ? new DateTime?() : new DateTime?(date);

    private DateTime? GetNullableDate(string dateAsString)
    {
      if (string.IsNullOrEmpty(dateAsString))
        return new DateTime?();
      DateTime result;
      return !DateTime.TryParse(dateAsString, out result) ? new DateTime?() : new DateTime?(result);
    }
  }
}
