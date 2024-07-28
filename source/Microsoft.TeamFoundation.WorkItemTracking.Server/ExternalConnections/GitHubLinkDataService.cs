// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.GitHubLinkDataService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.ServiceModels;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  public class GitHubLinkDataService : IGitHubLinkDataService, IVssFrameworkService
  {
    private const string c_artifact_url_fragment_commit = "commit";
    private const string c_artifact_url_fragment_pullrequest = "pull";
    private const string c_artifact_url_fragment_issue = "issues";

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<GitHubLinkItem> ResolveGitHubItems(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<GitHubLinkItemIdentifierInput> itemIdentifiers)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckEnumerableForEmpty((IEnumerable) itemIdentifiers, nameof (itemIdentifiers));
      IEnumerable<GitHubLinkItemIdentifier> resolvedIdentifiers = this.ResolveIdentifiersInput(itemIdentifiers);
      if (resolvedIdentifiers.Any<GitHubLinkItemIdentifier>())
      {
        IEnumerable<GitHubLinkItem> resolvedArtifactsFromDatabase = WorkItemTrackingFeatureFlags.IsTakeFreshGithubPRInfoAlwaysEnabled(requestContext) ? this.ResolveGitHubItemsFromDatabase(requestContext, resolvedIdentifiers.Where<GitHubLinkItemIdentifier>((Func<GitHubLinkItemIdentifier, bool>) (r => r.ItemType != GitHubLinkItemType.PullRequest))) : this.ResolveGitHubItemsFromDatabase(requestContext, resolvedIdentifiers);
        resolvedIdentifiers = (IEnumerable<GitHubLinkItemIdentifier>) resolvedIdentifiers.Where<GitHubLinkItemIdentifier>((Func<GitHubLinkItemIdentifier, bool>) (r => !resolvedArtifactsFromDatabase.Any<GitHubLinkItem>((Func<GitHubLinkItem, bool>) (a => r.ItemType == a.ItemType && r.RepoInternalId == a.RepoInternalId && r.NumberOrSHA == a.NumberOrSHA)))).ToList<GitHubLinkItemIdentifier>();
        foreach (GitHubLinkItem gitHubLinkItem in resolvedArtifactsFromDatabase)
          yield return gitHubLinkItem;
      }
      if (resolvedIdentifiers.Any<GitHubLinkItemIdentifier>())
      {
        ReposToConnectionsResolution repoResolution = this.GetRepoResolution(requestContext, projectId);
        Dictionary<ExternalConnection, List<GitHubLinkItemIdentifier>> requests = new Dictionary<ExternalConnection, List<GitHubLinkItemIdentifier>>((IEqualityComparer<ExternalConnection>) ExternalConnectionComparer.Instance);
        List<GitHubLinkItemIdentifier> fallbackItemsForNoRepoConnections = new List<GitHubLinkItemIdentifier>();
        foreach (GitHubLinkItemIdentifier linkItemIdentifier1 in resolvedIdentifiers)
        {
          ExternalConnection key;
          ExternalGitRepo externalGitRepo;
          string str;
          if (!repoResolution.RepoInternalIdToConnectionLookup.TryGetValue(linkItemIdentifier1.RepoInternalId, out key) || !repoResolution.RepoInternalIdToRepoLookup.TryGetValue(linkItemIdentifier1.RepoInternalId, out externalGitRepo) || !repoResolution.RepoInternalIdToExternalIdLookup.TryGetValue(linkItemIdentifier1.RepoInternalId, out str))
          {
            fallbackItemsForNoRepoConnections.Add(linkItemIdentifier1);
          }
          else
          {
            if (!requests.ContainsKey(key))
              requests[key] = new List<GitHubLinkItemIdentifier>();
            GitHubLinkItemIdentifier linkItemIdentifier2 = new GitHubLinkItemIdentifier()
            {
              ItemType = linkItemIdentifier1.ItemType,
              RepoInternalId = linkItemIdentifier1.RepoInternalId,
              RepoExternalId = str,
              RepoNameWithOwner = externalGitRepo.Name,
              NumberOrSHA = linkItemIdentifier1.NumberOrSHA,
              Url = externalGitRepo.WebUrl + "/" + this.ResolveItemType(linkItemIdentifier1.ItemType) + "/" + linkItemIdentifier1.NumberOrSHA
            };
            requests[key].Add(linkItemIdentifier2);
          }
        }
        foreach (GitHubLinkItem processGitHubRequest in this.ProcessGitHubRequests(requestContext, projectId, requests, fallbackItemsForNoRepoConnections, true))
          yield return processGitHubRequest;
      }
    }

    public IEnumerable<GitHubLinkItem> ResolveGitHubUrls(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> urls)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckEnumerableForEmpty((IEnumerable) urls, nameof (urls));
      IEnumerable<GitHubLinkItemIdentifier> linkItemIdentifiers = this.ResolveUrlsInput(urls);
      ReposToConnectionsResolution repoResolution = this.GetRepoResolution(requestContext, projectId);
      List<GitHubLinkItem> second = new List<GitHubLinkItem>();
      Dictionary<ExternalConnection, List<GitHubLinkItemIdentifier>> requests = new Dictionary<ExternalConnection, List<GitHubLinkItemIdentifier>>((IEqualityComparer<ExternalConnection>) ExternalConnectionComparer.Instance);
      foreach (GitHubLinkItemIdentifier linkItemIdentifier in linkItemIdentifiers)
      {
        Guid key1;
        string str;
        ExternalConnection key2;
        if (!repoResolution.RepoNameToInternalIdLookup.TryGetValue(Tuple.Create<string, string>(linkItemIdentifier.HostName, linkItemIdentifier.RepoNameWithOwner), out key1) || !repoResolution.RepoInternalIdToExternalIdLookup.TryGetValue(key1, out str) || !repoResolution.RepoInternalIdToConnectionLookup.TryGetValue(key1, out key2))
          second.Add(new GitHubLinkItem()
          {
            ItemType = linkItemIdentifier.ItemType,
            NumberOrSHA = linkItemIdentifier.NumberOrSHA,
            Url = linkItemIdentifier.Url,
            ErrorMessage = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.GitHubConnectionNotFoundMessage()
          });
        else if (!key2.IsConnectionValid)
        {
          second.Add(new GitHubLinkItem()
          {
            ItemType = linkItemIdentifier.ItemType,
            NumberOrSHA = linkItemIdentifier.NumberOrSHA,
            Url = linkItemIdentifier.Url,
            ErrorMessage = key2.FriendlyStatusMessage ?? Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ExternalConnectionOrRepoHasError((object) key2.Name)
          });
        }
        else
        {
          if (!requests.ContainsKey(key2))
            requests[key2] = new List<GitHubLinkItemIdentifier>();
          requests[key2].Add(new GitHubLinkItemIdentifier()
          {
            ItemType = linkItemIdentifier.ItemType,
            RepoInternalId = key1,
            RepoExternalId = str,
            RepoNameWithOwner = linkItemIdentifier.RepoNameWithOwner,
            NumberOrSHA = linkItemIdentifier.NumberOrSHA,
            Url = linkItemIdentifier.Url
          });
        }
      }
      return this.ProcessGitHubRequests(requestContext, projectId, requests, (List<GitHubLinkItemIdentifier>) null, false).Concat<GitHubLinkItem>((IEnumerable<GitHubLinkItem>) second);
    }

    private IEnumerable<GitHubLinkItem> ProcessGitHubRequests(
      IVssRequestContext requestContext,
      Guid projectId,
      Dictionary<ExternalConnection, List<GitHubLinkItemIdentifier>> requests,
      List<GitHubLinkItemIdentifier> fallbackItemsForNoRepoConnections,
      bool shouldHandleFallbackItems)
    {
      List<GitHubLinkItemIdentifier> fallbackItemsFromRequests = new List<GitHubLinkItemIdentifier>();
      List<GitHubLinkItemIdentifier> itemsFailedToFallback = new List<GitHubLinkItemIdentifier>();
      ClientTraceService clientTraceService = requestContext.GetService<ClientTraceService>();
      try
      {
        foreach (KeyValuePair<ExternalConnection, List<GitHubLinkItemIdentifier>> request in requests)
        {
          ExternalConnection key = request.Key;
          List<GitHubLinkItemIdentifier> identifiers = request.Value;
          if (identifiers.Count > 0)
          {
            List<GitHubLinkItem> list = this.GetItemsFromGitHub(requestContext, projectId, key, identifiers, shouldHandleFallbackItems, fallbackItemsFromRequests).ToList<GitHubLinkItem>();
            this.QueueTaskToSaveArtifacts(requestContext, key.ProviderKey, key.ExternalGitRepos, (IEnumerable<GitHubLinkItem>) list);
            foreach (GitHubLinkItem gitHubLinkItem in list)
              yield return gitHubLinkItem;
          }
        }
        if (shouldHandleFallbackItems)
        {
          List<GitHubLinkItemIdentifier> source = fallbackItemsForNoRepoConnections;
          if ((source != null ? (source.Any<GitHubLinkItemIdentifier>() ? 1 : 0) : 0) != 0)
          {
            IEnumerable<Guid> repositoryIds = fallbackItemsForNoRepoConnections.Select<GitHubLinkItemIdentifier, Guid>((Func<GitHubLinkItemIdentifier, Guid>) (item => item.RepoInternalId));
            Dictionary<Guid, ExternalConnectionRepository> repoLookup = requestContext.GetService<IExternalConnectionService>().GetExternalRepositories(requestContext, repositoryIds).ToDedupedDictionary<ExternalConnectionRepository, Guid, ExternalConnectionRepository>((Func<ExternalConnectionRepository, Guid>) (repo => repo.RepositoryId), (Func<ExternalConnectionRepository, ExternalConnectionRepository>) (repo => repo));
            foreach (GitHubLinkItemIdentifier noRepoConnection in fallbackItemsForNoRepoConnections)
            {
              if (!repoLookup.ContainsKey(noRepoConnection.RepoInternalId))
              {
                itemsFailedToFallback.Add(noRepoConnection);
              }
              else
              {
                ExternalConnectionRepository connectionRepository = repoLookup[noRepoConnection.RepoInternalId];
                string repositoryName = connectionRepository.RepositoryName;
                string str = this.ResolveItemUrl(connectionRepository.WebUrl, noRepoConnection.ItemType, noRepoConnection.NumberOrSHA);
                yield return new GitHubLinkItem()
                {
                  ItemType = noRepoConnection.ItemType,
                  RepoInternalId = noRepoConnection.RepoInternalId,
                  RepoExternalId = noRepoConnection.RepoExternalId,
                  RepoNameWithOwner = repositoryName,
                  NumberOrSHA = noRepoConnection.NumberOrSHA,
                  Url = str
                };
              }
            }
            repoLookup = (Dictionary<Guid, ExternalConnectionRepository>) null;
          }
          foreach (GitHubLinkItemIdentifier linkItemIdentifier in fallbackItemsFromRequests)
            yield return new GitHubLinkItem()
            {
              ItemType = linkItemIdentifier.ItemType,
              RepoInternalId = linkItemIdentifier.RepoInternalId,
              RepoExternalId = linkItemIdentifier.RepoExternalId,
              RepoNameWithOwner = linkItemIdentifier.RepoNameWithOwner,
              NumberOrSHA = linkItemIdentifier.NumberOrSHA,
              Url = linkItemIdentifier.Url
            };
        }
      }
      finally
      {
        ClientTraceService clientTraceService1 = clientTraceService;
        IVssRequestContext requestContext1 = requestContext;
        string area = this.Area;
        string layer = this.Layer;
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("Action", (object) nameof (ProcessGitHubRequests));
        data.Add("ProjectId", (object) projectId);
        data.Add("RequestsCount", (object) requests.Count);
        data.Add("RequestsTotalItemsCount", (object) requests.Sum<KeyValuePair<ExternalConnection, List<GitHubLinkItemIdentifier>>>((Func<KeyValuePair<ExternalConnection, List<GitHubLinkItemIdentifier>>, int>) (r => r.Value.Count)));
        data.Add("RequestsFallbackItemsCount", (object) fallbackItemsFromRequests.Count);
        List<GitHubLinkItemIdentifier> linkItemIdentifierList = fallbackItemsForNoRepoConnections;
        // ISSUE: explicit non-virtual call
        data.Add("NoRepoConnectionItemsCount", linkItemIdentifierList != null ? (object) __nonvirtual (linkItemIdentifierList.Count) : (object) 0);
        data.Add("FailedToFallbackItemsCount", (object) itemsFailedToFallback.Count);
        ClientTraceData properties = new ClientTraceData((IDictionary<string, object>) data);
        clientTraceService1.Publish(requestContext1, area, layer, properties);
      }
    }

    private IEnumerable<GitHubLinkItem> GetItemsFromGitHub(
      IVssRequestContext requestContext,
      Guid projectId,
      ExternalConnection connection,
      List<GitHubLinkItemIdentifier> identifiers,
      bool shouldHandleFallbackItems,
      List<GitHubLinkItemIdentifier> fallbackItemsFromRequests)
    {
      ClientTraceService service = requestContext.GetService<ClientTraceService>();
      ServiceEndpoint rawServiceEndpoint = connection.ServiceEndpoint.RawServiceEndpoint;
      string enterpriseUrl = rawServiceEndpoint.GetEnterpriseUrl();
      Exception exception = (Exception) null;
      GitHubResult<IEnumerable<GitHubLinkItem>> gitHubResult = (GitHubResult<IEnumerable<GitHubLinkItem>>) null;
      Stopwatch stopwatch = Stopwatch.StartNew();
      try
      {
        GitHubAuthentication hubAuthentication = rawServiceEndpoint != null ? rawServiceEndpoint.GetGitHubAuthentication(requestContext, projectId) : (GitHubAuthentication) null;
        AzureBoardsGitHubDataHelper gitHubClient = this.GetGitHubClient(requestContext);
        gitHubResult = WorkItemTrackingFeatureFlags.IsAdvancedGitHubPrExperienceEnabled(requestContext) ? gitHubClient.GetLinkItemsAdvanced(enterpriseUrl, hubAuthentication, (IList<GitHubLinkItemIdentifier>) identifiers, WorkItemTrackingFeatureFlags.IsGitHubPrCheckRunInfoEnabled(requestContext)) : gitHubClient.GetLinkItems(enterpriseUrl, hubAuthentication, (IList<GitHubLinkItemIdentifier>) identifiers);
      }
      catch (Exception ex)
      {
        exception = ex;
      }
      finally
      {
        stopwatch.Stop();
      }
      bool flag1 = gitHubResult != null && !gitHubResult.IsSuccessful && gitHubResult.Result != null && gitHubResult.Result.Any<GitHubLinkItem>();
      bool flag2 = exception == null && gitHubResult != null && gitHubResult.IsSuccessful | flag1;
      ClientTraceService clientTraceService = service;
      IVssRequestContext requestContext1 = requestContext;
      string area = this.Area;
      string layer = this.Layer;
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("Action", (object) "GitHubClient.GetLinkItems");
      data.Add("ItemsCount", (object) identifiers.Count);
      data.Add("ElapsedTimeInMs", (object) stopwatch.ElapsedMilliseconds);
      data.Add("Succeeded", (object) flag2);
      data.Add("IsPartialSuccess", (object) flag1);
      Dictionary<string, object> dictionary = data;
      string errorMessage = gitHubResult?.ErrorMessage;
      object values;
      if (gitHubResult == null)
      {
        values = (object) null;
      }
      else
      {
        IReadOnlyList<GitHubError> errors = gitHubResult.Errors;
        values = errors != null ? (object) errors.Select<GitHubError, string>((Func<GitHubError, string>) (e => e.Type + "-" + e.Message)) : (object) null;
      }
      if (values == null)
        values = (object) new string[0];
      string str1 = string.Join(",", (IEnumerable<string>) values);
      string str2 = errorMessage + " \r\n" + str1;
      dictionary.Add("ErrorResponse", (object) str2);
      data.Add("Exception", (object) string.Format("{0}", (object) exception));
      ClientTraceData properties = new ClientTraceData((IDictionary<string, object>) data);
      clientTraceService.Publish(requestContext1, area, layer, properties);
      if (flag2)
      {
        Dictionary<(string, GitHubLinkItemType, string), GitHubLinkItemIdentifier> itemsLookup = new Dictionary<(string, GitHubLinkItemType, string), GitHubLinkItemIdentifier>();
        foreach (GitHubLinkItemIdentifier identifier in identifiers)
          itemsLookup[(identifier.RepoExternalId, identifier.ItemType, identifier.NumberOrSHA)] = identifier;
        foreach (GitHubLinkItem gitHubLinkItem in gitHubResult.Result)
        {
          GitHubLinkItemIdentifier linkItemIdentifier;
          if (gitHubLinkItem != null && itemsLookup.TryGetValue((gitHubLinkItem.RepoExternalId, gitHubLinkItem.ItemType, gitHubLinkItem.NumberOrSHA), out linkItemIdentifier))
          {
            gitHubLinkItem.ProviderKey = connection.ProviderKey;
            gitHubLinkItem.ConnectionAuthenticationType = connection.ServiceEndpoint.AuthorizationScheme;
            gitHubLinkItem.RepoInternalId = linkItemIdentifier.RepoInternalId;
            gitHubLinkItem.RepoNameWithOwner = !string.IsNullOrEmpty(gitHubLinkItem.RepoNameWithOwner) ? gitHubLinkItem.RepoNameWithOwner : linkItemIdentifier.RepoNameWithOwner;
            itemsLookup.Remove((gitHubLinkItem.RepoExternalId, gitHubLinkItem.ItemType, gitHubLinkItem.NumberOrSHA));
            yield return gitHubLinkItem;
          }
        }
        if (shouldHandleFallbackItems && itemsLookup.Any<KeyValuePair<(string, GitHubLinkItemType, string), GitHubLinkItemIdentifier>>())
          fallbackItemsFromRequests.AddRange((IEnumerable<GitHubLinkItemIdentifier>) itemsLookup.Values);
        itemsLookup = (Dictionary<(string, GitHubLinkItemType, string), GitHubLinkItemIdentifier>) null;
      }
      else if (shouldHandleFallbackItems)
        fallbackItemsFromRequests.AddRange((IEnumerable<GitHubLinkItemIdentifier>) identifiers);
    }

    private IEnumerable<GitHubLinkItem> ResolveGitHubItemsFromDatabase(
      IVssRequestContext requestContext,
      IEnumerable<GitHubLinkItemIdentifier> requestsByIdentifier)
    {
      List<GitHubLinkItem> gitHubLinkItemList1 = new List<GitHubLinkItem>();
      Stopwatch stopwatch = Stopwatch.StartNew();
      IExternalArtifactService service1 = requestContext.GetService<IExternalArtifactService>();
      IEnumerable<(Guid, string)> valueTuples1 = (IEnumerable<(Guid, string)>) null;
      IEnumerable<(Guid, string)> valueTuples2 = (IEnumerable<(Guid, string)>) null;
      IEnumerable<(Guid, string)> valueTuples3 = (IEnumerable<(Guid, string)>) null;
      foreach (IGrouping<GitHubLinkItemType, GitHubLinkItemIdentifier> source in requestsByIdentifier.GroupBy<GitHubLinkItemIdentifier, GitHubLinkItemType>((Func<GitHubLinkItemIdentifier, GitHubLinkItemType>) (i => i.ItemType)))
      {
        switch (source.Key)
        {
          case GitHubLinkItemType.PullRequest:
            valueTuples2 = (IEnumerable<(Guid, string)>) source.Select<GitHubLinkItemIdentifier, (Guid, string)>((Func<GitHubLinkItemIdentifier, (Guid, string)>) (g => (g.RepoInternalId, g.NumberOrSHA))).ToList<(Guid, string)>();
            continue;
          case GitHubLinkItemType.Commit:
            valueTuples1 = (IEnumerable<(Guid, string)>) source.Select<GitHubLinkItemIdentifier, (Guid, string)>((Func<GitHubLinkItemIdentifier, (Guid, string)>) (g => (g.RepoInternalId, g.NumberOrSHA))).ToList<(Guid, string)>();
            continue;
          case GitHubLinkItemType.Issue:
            valueTuples3 = (IEnumerable<(Guid, string)>) source.Select<GitHubLinkItemIdentifier, (Guid, string)>((Func<GitHubLinkItemIdentifier, (Guid, string)>) (g => (g.RepoInternalId, g.NumberOrSHA))).ToList<(Guid, string)>();
            continue;
          default:
            continue;
        }
      }
      if (valueTuples1 != null || valueTuples2 != null || valueTuples3 != null)
      {
        Dictionary<(Guid, GitHubLinkItemType, string), GitHubLinkItemIdentifier> dictionary = new Dictionary<(Guid, GitHubLinkItemType, string), GitHubLinkItemIdentifier>();
        foreach (GitHubLinkItemIdentifier linkItemIdentifier in requestsByIdentifier)
          dictionary[(linkItemIdentifier.RepoInternalId, linkItemIdentifier.ItemType, linkItemIdentifier.NumberOrSHA)] = linkItemIdentifier;
        ExternalArtifactCollectionWithStatus artifacts = service1.GetArtifacts(requestContext, valueTuples1, valueTuples2, valueTuples3);
        if (artifacts.Commits != null)
        {
          foreach (ExternalArtifactAndHydrationStatus<ExternalGitCommit> commit in artifacts.Commits)
          {
            ExternalGitCommit externalArtifact = commit.ExternalArtifact;
            List<GitHubLinkItem> gitHubLinkItemList2 = gitHubLinkItemList1;
            GitHubLinkItem gitHubLinkItem = new GitHubLinkItem();
            gitHubLinkItem.ItemType = GitHubLinkItemType.Commit;
            gitHubLinkItem.NumberOrSHA = externalArtifact.Sha;
            gitHubLinkItem.Title = externalArtifact.Message;
            gitHubLinkItem.Date = new DateTime?(externalArtifact.CommitedDate);
            Author author1 = new Author();
            ExternalGitUser author2 = externalArtifact.Author;
            author1.DisplayName = author2 != null ? author2.Name() : (string) null;
            author1.AvatarUrl = externalArtifact.Author?.AvatarUrl;
            gitHubLinkItem.Author = author1;
            gitHubLinkItem.RepoExternalId = externalArtifact.Repo.NodeId();
            gitHubLinkItem.RepoInternalId = externalArtifact.Repo.GetRepoInternalId();
            gitHubLinkItem.RepoNameWithOwner = externalArtifact.Repo.RepoNameWithOwner();
            gitHubLinkItem.Url = externalArtifact.WebUrl;
            gitHubLinkItemList2.Add(gitHubLinkItem);
          }
        }
        if (artifacts.PullRequests != null)
        {
          foreach (ExternalArtifactAndHydrationStatus<ExternalGitPullRequest> pullRequest in artifacts.PullRequests)
          {
            ExternalGitPullRequest externalArtifact = pullRequest.ExternalArtifact;
            List<GitHubLinkItem> gitHubLinkItemList3 = gitHubLinkItemList1;
            GitHubLinkItem gitHubLinkItem = new GitHubLinkItem();
            gitHubLinkItem.ItemType = GitHubLinkItemType.PullRequest;
            gitHubLinkItem.NumberOrSHA = externalArtifact.Number;
            DateTime result;
            gitHubLinkItem.Date = DateTime.TryParse(externalArtifact.UpdatedAt, out result) ? new DateTime?(result) : new DateTime?();
            gitHubLinkItem.Title = externalArtifact.Title;
            Author author = new Author();
            ExternalGitUser sender = externalArtifact.Sender;
            author.DisplayName = sender != null ? sender.Name() : (string) null;
            author.AvatarUrl = externalArtifact.Sender?.AvatarUrl;
            gitHubLinkItem.Author = author;
            gitHubLinkItem.RepoExternalId = externalArtifact.Repo.NodeId();
            gitHubLinkItem.RepoInternalId = externalArtifact.Repo.GetRepoInternalId();
            gitHubLinkItem.RepoNameWithOwner = externalArtifact.Repo.RepoNameWithOwner();
            gitHubLinkItem.State = externalArtifact.State;
            gitHubLinkItem.Url = externalArtifact.WebUrl;
            gitHubLinkItemList3.Add(gitHubLinkItem);
          }
        }
        if (artifacts.Issues != null)
        {
          foreach (ExternalArtifactAndHydrationStatus<ExternalGitIssue> issue in artifacts.Issues)
          {
            ExternalGitIssue externalArtifact = issue.ExternalArtifact;
            List<GitHubLinkItem> gitHubLinkItemList4 = gitHubLinkItemList1;
            GitHubLinkItem gitHubLinkItem = new GitHubLinkItem();
            gitHubLinkItem.ItemType = GitHubLinkItemType.Issue;
            gitHubLinkItem.NumberOrSHA = externalArtifact.Number;
            DateTime result;
            gitHubLinkItem.Date = DateTime.TryParse(externalArtifact.UpdatedAt, out result) ? new DateTime?(result) : new DateTime?();
            gitHubLinkItem.Title = externalArtifact.Title;
            Author author = new Author();
            ExternalGitUser sender = externalArtifact.Sender;
            author.DisplayName = sender != null ? sender.Name() : (string) null;
            author.AvatarUrl = externalArtifact.Sender?.AvatarUrl;
            gitHubLinkItem.Author = author;
            gitHubLinkItem.RepoExternalId = externalArtifact.Repo.NodeId();
            gitHubLinkItem.RepoInternalId = externalArtifact.Repo.GetRepoInternalId();
            gitHubLinkItem.RepoNameWithOwner = externalArtifact.Repo.RepoNameWithOwner();
            gitHubLinkItem.State = externalArtifact.State;
            gitHubLinkItem.Url = externalArtifact.WebUrl;
            gitHubLinkItemList4.Add(gitHubLinkItem);
          }
        }
        stopwatch.Stop();
        ClientTraceService service2 = requestContext.GetService<ClientTraceService>();
        IVssRequestContext requestContext1 = requestContext;
        string area = this.Area;
        string layer = this.Layer;
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("Action", (object) "GitHubClient.ResolveGitHubItemsFromDatabase");
        data.Add("ElapsedTimeInMs", (object) stopwatch.ElapsedMilliseconds);
        data.Add("Requested Commits", (object) (valueTuples1 != null ? new int?(valueTuples1.Count<(Guid, string)>()) : new int?()));
        data.Add("Requested Pull Requests", (object) (valueTuples2 != null ? new int?(valueTuples2.Count<(Guid, string)>()) : new int?()));
        data.Add("Requested Issues", (object) (valueTuples3 != null ? new int?(valueTuples3.Count<(Guid, string)>()) : new int?()));
        IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitCommit>> commits = artifacts.Commits;
        data.Add("Resolved Commits", (object) (commits != null ? new int?(commits.Count<ExternalArtifactAndHydrationStatus<ExternalGitCommit>>()) : new int?()));
        IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>> pullRequests = artifacts.PullRequests;
        data.Add("Resolved Pull Requests", (object) (pullRequests != null ? new int?(pullRequests.Count<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>()) : new int?()));
        IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitIssue>> issues = artifacts.Issues;
        data.Add("Resolved Issues", (object) (issues != null ? new int?(issues.Count<ExternalArtifactAndHydrationStatus<ExternalGitIssue>>()) : new int?()));
        ClientTraceData properties = new ClientTraceData((IDictionary<string, object>) data);
        service2.Publish(requestContext1, area, layer, properties);
      }
      return (IEnumerable<GitHubLinkItem>) gitHubLinkItemList1;
    }

    protected IEnumerable<GitHubLinkItemIdentifier> ResolveIdentifiersInput(
      IEnumerable<GitHubLinkItemIdentifierInput> itemIdentifiers)
    {
      List<GitHubLinkItemIdentifier> linkItemIdentifierList = new List<GitHubLinkItemIdentifier>();
      foreach (GitHubLinkItemIdentifierInput itemIdentifier in itemIdentifiers)
      {
        if (itemIdentifier.ItemType != GitHubLinkItemType.Unknown && itemIdentifier.RepoInternalId != Guid.Empty && !string.IsNullOrEmpty(itemIdentifier.NumberOrSHA))
          linkItemIdentifierList.Add(new GitHubLinkItemIdentifier()
          {
            ItemType = itemIdentifier.ItemType,
            RepoInternalId = itemIdentifier.RepoInternalId,
            NumberOrSHA = itemIdentifier.NumberOrSHA
          });
      }
      return (IEnumerable<GitHubLinkItemIdentifier>) linkItemIdentifierList;
    }

    protected IEnumerable<GitHubLinkItemIdentifier> ResolveUrlsInput(IEnumerable<string> urls)
    {
      List<GitHubLinkItemIdentifier> linkItemIdentifierList = new List<GitHubLinkItemIdentifier>();
      foreach (string url in urls)
      {
        Uri result;
        if (!string.IsNullOrEmpty(url) && Uri.TryCreate(url, UriKind.Absolute, out result))
        {
          string[] array = ((IEnumerable<string>) result.Segments).Select<string, string>((Func<string, string>) (s => s.TrimEnd('/'))).Where<string>((Func<string, bool>) (v => !string.IsNullOrEmpty(v))).ToArray<string>();
          if (array.Length >= 4)
          {
            bool flag = false;
            GitHubLinkItemIdentifier linkItemIdentifier = new GitHubLinkItemIdentifier();
            for (int index = array.Length - 2; index >= 2; --index)
            {
              if (VssStringComparer.Url.Equals("pull", array[index]))
              {
                linkItemIdentifier.ItemType = GitHubLinkItemType.PullRequest;
                flag = true;
              }
              else if (VssStringComparer.Url.Equals("issues", array[index]))
              {
                linkItemIdentifier.ItemType = GitHubLinkItemType.Issue;
                flag = true;
              }
              else if (VssStringComparer.Url.Equals("commit", array[index]))
              {
                linkItemIdentifier.ItemType = GitHubLinkItemType.Commit;
                flag = true;
              }
              if (flag)
              {
                linkItemIdentifier.Url = url;
                linkItemIdentifier.NumberOrSHA = array[index + 1];
                linkItemIdentifier.HostName = result.Host;
                linkItemIdentifier.RepoNameWithOwner = array[index - 2] + "/" + array[index - 1];
                linkItemIdentifierList.Add(linkItemIdentifier);
                break;
              }
            }
          }
        }
      }
      return (IEnumerable<GitHubLinkItemIdentifier>) linkItemIdentifierList;
    }

    protected virtual AzureBoardsGitHubDataHelper GetGitHubClient(IVssRequestContext requestContext) => AzureBoardsGitHubDataHelper.Create(requestContext);

    protected virtual ReposToConnectionsResolution GetRepoResolution(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      IReadOnlyCollection<ExternalConnection> externalConnections = requestContext.GetService<IExternalConnectionService>().GetExternalConnections(requestContext, new Guid?(projectId), (string) null, true, includeInvalidConnections: true);
      ReposToConnectionsResolution repoResolution = new ReposToConnectionsResolution();
      foreach (ExternalConnection externalConnection in (IEnumerable<ExternalConnection>) externalConnections)
      {
        string providerKey = externalConnection.ProviderKey;
        foreach (ExternalGitRepo externalGitRepo in externalConnection.ExternalGitRepos)
        {
          string id = externalGitRepo.Id;
          Guid repoInternalId = externalConnection.GetRepoInternalId(id);
          string str = externalGitRepo.RepoNameWithOwner();
          repoResolution.RepoNameToInternalIdLookup[Tuple.Create<string, string>(providerKey, str)] = repoInternalId;
          repoResolution.RepoInternalIdToExternalIdLookup[repoInternalId] = id;
          repoResolution.RepoInternalIdToRepoLookup[repoInternalId] = externalGitRepo;
          repoResolution.RepoInternalIdToConnectionLookup[repoInternalId] = externalConnection;
        }
      }
      return repoResolution;
    }

    private string ResolveItemUrl(
      string repoWebUrl,
      GitHubLinkItemType itemType,
      string artifactId)
    {
      return repoWebUrl + "/" + this.ResolveItemType(itemType) + "/" + artifactId;
    }

    private string ResolveItemType(GitHubLinkItemType itemType)
    {
      switch (itemType)
      {
        case GitHubLinkItemType.PullRequest:
          return "pull";
        case GitHubLinkItemType.Commit:
          return "commit";
        case GitHubLinkItemType.Issue:
          return "issues";
        default:
          throw new UnsupportedGitHubItemTypeException(itemType.ToString());
      }
    }

    private void QueueTaskToSaveArtifacts(
      IVssRequestContext requestContext,
      string providerKey,
      IEnumerable<ExternalGitRepo> repos,
      IEnumerable<GitHubLinkItem> items)
    {
      if (items == null || !items.Any<GitHubLinkItem>())
        return;
      requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext, (TeamFoundationTaskCallback) ((taskRequestContext, taskArgs) =>
      {
        ExternalArtifactCollectionWithStatus collectionWithStatus = new ExternalArtifactCollectionWithStatus();
        IEnumerable<GitHubLinkItem> source1 = items;
        collectionWithStatus.Commits = source1 != null ? source1.Where<GitHubLinkItem>((Func<GitHubLinkItem, bool>) (i => i.ItemType == GitHubLinkItemType.Commit)).Select<GitHubLinkItem, ExternalArtifactAndHydrationStatus<ExternalGitCommit>>((Func<GitHubLinkItem, ExternalArtifactAndHydrationStatus<ExternalGitCommit>>) (i => new ExternalArtifactAndHydrationStatus<ExternalGitCommit>()
        {
          ExternalArtifact = i.GetItem<ExternalGitCommit>(),
          HydrationStatus = ExternalArtifactHydrationStatus.Full,
          UpdateOnly = false
        })) : (IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitCommit>>) null;
        IEnumerable<GitHubLinkItem> source2 = items;
        collectionWithStatus.PullRequests = source2 != null ? source2.Where<GitHubLinkItem>((Func<GitHubLinkItem, bool>) (i => i.ItemType == GitHubLinkItemType.PullRequest)).Select<GitHubLinkItem, ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>((Func<GitHubLinkItem, ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>) (i => new ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>()
        {
          ExternalArtifact = i.GetItem<ExternalGitPullRequest>(),
          HydrationStatus = ExternalArtifactHydrationStatus.Full,
          UpdateOnly = false
        })) : (IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>) null;
        IEnumerable<GitHubLinkItem> source3 = items;
        collectionWithStatus.Issues = source3 != null ? source3.Where<GitHubLinkItem>((Func<GitHubLinkItem, bool>) (i => i.ItemType == GitHubLinkItemType.Issue)).Select<GitHubLinkItem, ExternalArtifactAndHydrationStatus<ExternalGitIssue>>((Func<GitHubLinkItem, ExternalArtifactAndHydrationStatus<ExternalGitIssue>>) (i => new ExternalArtifactAndHydrationStatus<ExternalGitIssue>()
        {
          ExternalArtifact = i.GetItem<ExternalGitIssue>(),
          HydrationStatus = ExternalArtifactHydrationStatus.Full,
          UpdateOnly = false
        })) : (IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitIssue>>) null;
        ExternalArtifactCollectionWithStatus artifacts = collectionWithStatus;
        try
        {
          taskRequestContext.GetService<IExternalArtifactService>().SaveArtifacts(taskRequestContext, providerKey, repos, artifacts);
        }
        catch (Exception ex)
        {
          taskRequestContext.TraceException(this.TracePointStart + 10, this.Area, nameof (QueueTaskToSaveArtifacts), ex);
        }
      }));
    }

    private string Layer => nameof (GitHubLinkDataService);

    private string Area => "Services";

    private int TracePointStart => 919500;
  }
}
