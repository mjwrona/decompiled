// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.GitHubAppConnectionHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalEvents;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  public static class GitHubAppConnectionHelper
  {
    private const string GitHubCallbackStorageKey = "GitHubOauthCallback";
    public const string UseWellKnownStrongBoxLocationKey = "useWellKnownStrongBoxLocation";

    public static ServiceEndpoint CreateInMemoryOAuthServiceEndpoint(
      IVssRequestContext requestContext)
    {
      return GitHubAppConnectionHelper.CreateInMemoryOAuthServiceEndpointFromStrongBoxKey(requestContext, "useWellKnownStrongBoxLocation");
    }

    public static ServiceEndpoint CreateInMemoryOAuthServiceEndpointFromStrongBoxKey(
      IVssRequestContext requestContext,
      string strongBoxKey)
    {
      GitHubAppConnectionHelper.OAuthAccessToken oauthAccessToken;
      if (string.Equals(strongBoxKey, "useWellKnownStrongBoxLocation", StringComparison.OrdinalIgnoreCase))
      {
        string lookUpKey = requestContext.GetUserId().ToString("D");
        oauthAccessToken = JsonUtilities.Deserialize<GitHubAppConnectionHelper.OAuthAccessToken>(GitHubAppConnectionHelper.GetStrongBoxContent(requestContext, GitHubConstants.StrongBoxKey.GitHubBoardsAppUserTokenDrawer, lookUpKey));
      }
      else
        oauthAccessToken = new GitHubAppConnectionHelper.OAuthAccessToken()
        {
          Type = "Token",
          Token = GitHubAppConnectionHelper.GetStrongBoxContent(requestContext, "GitHubOauthCallback", strongBoxKey)
        };
      ServiceEndpoint fromStrongBoxKey = new ServiceEndpoint()
      {
        Id = Guid.Empty,
        Type = "GitHub",
        Owner = ServiceEndpointOwner.Boards,
        Authorization = new EndpointAuthorization()
        {
          Scheme = "OAuth"
        }
      };
      fromStrongBoxKey.Authorization.Parameters["AccessToken"] = oauthAccessToken.Token;
      fromStrongBoxKey.Authorization.Parameters["AccessTokenType"] = oauthAccessToken.Type;
      fromStrongBoxKey.Authorization.Parameters["IdToken"] = oauthAccessToken.InstallationId;
      return fromStrongBoxKey;
    }

    public static ServiceEndpoint CreateInMemoryServiceEndpointWithEndpointAuthorization(
      EndpointAuthorization authorization)
    {
      return new ServiceEndpoint()
      {
        Id = Guid.Empty,
        Type = "GitHub",
        Owner = ServiceEndpointOwner.Boards,
        Authorization = authorization
      };
    }

    private static string GetStrongBoxContent(
      IVssRequestContext requestContext,
      string strongBoxName,
      string lookUpKey)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemInfo itemInfo = service.GetItemInfo(vssRequestContext, strongBoxName, lookUpKey, true);
      return service.GetString(vssRequestContext, itemInfo);
    }

    public static void CleanUpMpsEntriesForProject(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      IEnumerable<(ExternalConnection ExternalConnection, string InstallationId, List<string> RepoNodeIds)> projectInstallations = GitHubAppConnectionHelper.GetRepoMappingForSingleProjectInstallations(requestContext, projectId);
      IBoardsGitHubAppHostIdMappingService service1 = requestContext.GetService<IBoardsGitHubAppHostIdMappingService>();
      CustomerIntelligenceService service2 = requestContext.GetService<CustomerIntelligenceService>();
      foreach ((ExternalConnection ExternalConnection, string InstallationId, List<string> RepoNodeIds) tuple in projectInstallations)
      {
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("connection", (object) tuple.ExternalConnection.Id);
        properties.Add("installationId", tuple.InstallationId);
        properties.Add(nameof (projectId), (object) projectId);
        properties.Add("repos", tuple.RepoNodeIds);
        service1.RemoveHostMappingForRepositoriesWithoutChecking(requestContext, tuple.InstallationId, (IReadOnlyList<string>) tuple.RepoNodeIds);
        service2.Publish(requestContext, nameof (GitHubAppConnectionHelper), nameof (CleanUpMpsEntriesForProject), properties);
      }
    }

    public static void RestoreMpsEntriesForProject(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      IEnumerable<(ExternalConnection ExternalConnection, string InstallationId, List<string> RepoNodeIds)> projectInstallations = GitHubAppConnectionHelper.GetRepoMappingForSingleProjectInstallations(requestContext, projectId);
      IBoardsGitHubAppHostIdMappingService service1 = requestContext.GetService<IBoardsGitHubAppHostIdMappingService>();
      IExternalConnectionService service2 = requestContext.GetService<IExternalConnectionService>();
      CustomerIntelligenceService service3 = requestContext.GetService<CustomerIntelligenceService>();
      foreach ((ExternalConnection ExternalConnection, string InstallationId, List<string> RepoNodeIds) tuple in projectInstallations)
      {
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("connection", (object) tuple.ExternalConnection.Id);
        properties.Add("installationId", tuple.InstallationId);
        properties.Add(nameof (projectId), (object) projectId);
        properties.Add("repos", tuple.RepoNodeIds);
        IEnumerable<string> strings = service1.AddHostMappingForRepositories(requestContext, tuple.InstallationId, (IReadOnlyList<string>) tuple.RepoNodeIds);
        if (strings.Any<string>())
        {
          properties.Add("reposMappedToDifferentHost", (object) strings);
          try
          {
            tuple.ExternalConnection.ConnectionMetadata.RepositoriesWithMappingToDifferentOrganization.AddRange<string, ISet<string>>(strings);
            service2.UpdateExternalConnectionMetadata(requestContext, projectId, tuple.ExternalConnection.Id, tuple.ExternalConnection.ConnectionMetadata);
          }
          catch (Exception ex)
          {
            properties.Add("exception", (object) ex);
          }
        }
        service3.Publish(requestContext, nameof (GitHubAppConnectionHelper), nameof (RestoreMpsEntriesForProject), properties);
      }
    }

    private static IEnumerable<(ExternalConnection ExternalConnection, string InstallationId, List<string> RepoNodeIds)> GetRepoMappingForSingleProjectInstallations(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      IReadOnlyCollection<ExternalConnection> externalConnections = requestContext.GetService<IExternalConnectionService>().GetExternalConnections(requestContext.Elevate(), new Guid?(), "github.com");
      Dictionary<string, IList<ExternalConnection>> source = new Dictionary<string, IList<ExternalConnection>>();
      foreach (ExternalConnection externalConnection in (IEnumerable<ExternalConnection>) externalConnections)
      {
        foreach (ExternalGitRepo externalGitRepo in externalConnection.ExternalGitRepos)
        {
          string key = externalGitRepo.NodeId();
          if (!source.ContainsKey(key))
            source.Add(key, (IList<ExternalConnection>) new List<ExternalConnection>());
          source[key].Add(externalConnection);
        }
      }
      string str;
      return source.Where<KeyValuePair<string, IList<ExternalConnection>>>((Func<KeyValuePair<string, IList<ExternalConnection>>, bool>) (kvp => kvp.Value.Count == 1)).Select<KeyValuePair<string, IList<ExternalConnection>>, (string, ExternalConnection)>((Func<KeyValuePair<string, IList<ExternalConnection>>, (string, ExternalConnection)>) (kvp => (kvp.Key, kvp.Value.First<ExternalConnection>()))).Where<(string, ExternalConnection)>((Func<(string, ExternalConnection), bool>) (m => m.externalConnection.ProjectId == projectId && m.externalConnection.ServiceEndpoint?.RawServiceEndpoint?.Authorization?.Parameters != null)).Select<(string, ExternalConnection), (ExternalConnection, string, string)>((Func<(string, ExternalConnection), (ExternalConnection, string, string)>) (m => !m.externalConnection.ServiceEndpoint.RawServiceEndpoint.Authorization.Parameters.TryGetValue("IdToken", out str) ? ((ExternalConnection) null, (string) null, (string) null) : (m.externalConnection, str, m.repoNodeId))).Where<(ExternalConnection, string, string)>((Func<(ExternalConnection, string, string), bool>) (m => m.InstallationId != null && m.RepoNodeId != null)).GroupBy<(ExternalConnection, string, string), string, (ExternalConnection, string), (ExternalConnection, string, List<string>)>((Func<(ExternalConnection, string, string), string>) (m => m.InstallationId), (Func<(ExternalConnection, string, string), (ExternalConnection, string)>) (m => (m.ExternalConnection, m.RepoNodeId)), (Func<string, IEnumerable<(ExternalConnection, string)>, (ExternalConnection, string, List<string>)>) ((InstallationId, ConnectionAndRepoIds) => (ConnectionAndRepoIds.First<(ExternalConnection, string)>().ExternalConnection, InstallationId, ConnectionAndRepoIds.Select<(ExternalConnection, string), string>((Func<(ExternalConnection, string), string>) (r => r.RepoNodeId)).ToList<string>())));
    }

    public class OAuthAccessToken
    {
      public string Type { get; set; }

      public string Token { get; set; }

      public string InstallationId { get; set; }
    }
  }
}
