// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Providers.GitHubExternalApp
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.ConnectedService.Server;
using Microsoft.TeamFoundation.ConnectedService.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.Common;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Pipelines.Server.Providers
{
  public class GitHubExternalApp : IPipelinesExternalApp
  {
    private const string c_layer = "GitHubExternalApp";

    public string AppId => GitHubAppConstants.AppId;

    public string UserLoginPropertyName => GitHubAppConstants.LoginPropertyName;

    public string GetInstallationDetails(IVssRequestContext requestContext, string installationId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(installationId, nameof (installationId));
      IGitHubAppAccessTokenProvider accessTokenProvider = this.GetGitHubAppAccessTokenProvider(requestContext);
      GitHubAuthentication appTokenAuthentication = new GitHubAuthentication(GitHubAuthScheme.ApplicationToken, accessTokenProvider?.CreateEncodedAppAccessToken());
      GitHubResult<GitHubData.V3.InstallationDetails> installationDetails1 = GitHubHttpClientFactory.Create(requestContext, accessTokenProvider).GetInstallationDetails(appTokenAuthentication, installationId);
      string installationDetails2;
      if (installationDetails1.IsSuccessful)
        installationDetails2 = JsonConvert.SerializeObject((object) installationDetails1.Result);
      else
        installationDetails2 = JsonConvert.SerializeObject((object) new GitHubData.V3.InstallationDetails()
        {
          Id = installationId
        });
      return installationDetails2;
    }

    public string GetInstallationIdForRepository(
      IVssRequestContext requestContext,
      string repositoryId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(repositoryId, nameof (repositoryId));
      GitHubResult<GitHubData.V3.InstallationDetails> repositoryInstallation = GitHubHttpClientFactory.Create(requestContext, this.GetGitHubAppAccessTokenProvider(requestContext)).GetRepositoryInstallation(repositoryId);
      return repositoryInstallation.IsSuccessful ? repositoryInstallation.Result.Id : (string) null;
    }

    public string FindMatchingInstallationId(
      IVssRequestContext requestContext,
      string repositoryId,
      out bool isInstalledForRepo)
    {
      GitHubHttpClient gitHubHttpClient = GitHubHttpClientFactory.Create(requestContext);
      GitHubResult<GitHubData.V3.InstallationDetails> repositoryInstallation = gitHubHttpClient.GetRepositoryInstallation(repositoryId);
      if (!repositoryInstallation.IsSuccessful)
      {
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.ExternalApp, nameof (GitHubExternalApp), "Unable to find an installation for repo '{0}'.", (object) repositoryId);
        isInstalledForRepo = false;
        string repoOwner = GitHubHelper.GetRepoOwner(repositoryId);
        GitHubResult<GitHubData.V3.InstallationDetails> gitHubResult = gitHubHttpClient.GetOrgInstallation(repoOwner);
        if (!gitHubResult.IsSuccessful)
        {
          gitHubResult = gitHubHttpClient.GetUserInstallation(repoOwner);
          if (!gitHubResult.IsSuccessful)
          {
            requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.ExternalApp, nameof (GitHubExternalApp), "Unable to find an installation for user account or org '{0}'.", (object) repoOwner);
            return (string) null;
          }
          requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.ExternalApp, nameof (GitHubExternalApp), "Installation '{0}' found for user account '{1}'.", (object) gitHubResult.Result?.Id, (object) repoOwner);
        }
        else
          requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.ExternalApp, nameof (GitHubExternalApp), "Installation '{0}' found for org '{1}'.", (object) gitHubResult.Result?.Id, (object) repoOwner);
        return gitHubResult.Result?.Id;
      }
      isInstalledForRepo = true;
      return repositoryInstallation.Result?.Id;
    }

    public bool UserHasAccess(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryId,
      string user,
      ServiceEndpoint userConnection)
    {
      requestContext.TraceAlways(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.ExternalApp, TraceLevel.Info, Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Area, nameof (GitHubExternalApp), "User login '{0}'.", (object) user);
      if (!string.IsNullOrEmpty(user) && string.Equals(GitHubHelper.GetRepoOwner(repositoryId), user, StringComparison.OrdinalIgnoreCase))
      {
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.ExternalApp, nameof (GitHubExternalApp), "User login '{0}' matches owner of repo '{1}'.", (object) user, (object) repositoryId);
        return true;
      }
      if (userConnection == null)
      {
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.ExternalApp, nameof (GitHubExternalApp), "ServiceEndpoint is null for user login '{0}' and repo '{1}'.", (object) user, (object) repositoryId);
        return false;
      }
      GitHubResult<GitHubData.V3.Repository> userRepo = GitHubHttpClientFactory.Create(requestContext).GetUserRepo(GitHubExternalApp.GetAuth(requestContext, projectId, userConnection), repositoryId);
      if (!userRepo.IsSuccessful)
      {
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.ExternalApp, nameof (GitHubExternalApp), "User doesn't have access to repo {0} via connection id {1}.", (object) repositoryId, (object) userConnection.Id);
        return false;
      }
      GitHubData.V3.RepositoryPermissions permissions = userRepo.Result.Permissions;
      if ((permissions != null ? (!permissions.Push ? 1 : 0) : 1) != 0)
      {
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.ExternalApp, nameof (GitHubExternalApp), "User doesn't have Push permissions to repo {0} via connection id {1}.", (object) repositoryId, (object) userConnection.Id);
        return false;
      }
      if (string.IsNullOrEmpty(user) || !string.Equals(userRepo.Result.Owner?.Type, nameof (user), StringComparison.OrdinalIgnoreCase) || string.Equals(user, userRepo.Result.Owner.Login, StringComparison.OrdinalIgnoreCase))
        return true;
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.ExternalApp, nameof (GitHubExternalApp), "User login '{0}' doesn't match repo owner '{1}'.", (object) user, (object) userRepo.Result.Owner.Login);
      return false;
    }

    private static GitHubAuthentication GetAuth(
      IVssRequestContext requestContext,
      Guid projectId,
      ServiceEndpoint userConnection)
    {
      Guid id = userConnection.Id;
      string accessToken;
      return !(userConnection.Id == Guid.Empty) || userConnection.Authorization?.Parameters == null || !userConnection.Authorization.Parameters.TryGetValue("AccessToken", out accessToken) || string.IsNullOrEmpty(accessToken) ? requestContext.GetAuthentication(userConnection.Id.ToString(), projectId) : new GitHubAuthentication(accessToken);
    }

    public bool UserCanInstall(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryId,
      string user,
      ServiceEndpoint userConnection,
      bool checkRepo)
    {
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.ExternalApp, nameof (GitHubExternalApp), "UserCanInstall - repositoryId={0}, user={1}.", (object) repositoryId, (object) user);
      string repoOwner = GitHubHelper.GetRepoOwner(repositoryId);
      if (!string.IsNullOrEmpty(user) && string.Equals(repoOwner, user, StringComparison.OrdinalIgnoreCase))
      {
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.ExternalApp, nameof (GitHubExternalApp), "User login '{0}' matches owner of repo '{1}'.", (object) user, (object) repositoryId);
        return true;
      }
      GitHubHttpClient gitHubHttpClient = GitHubHttpClientFactory.Create(requestContext);
      GitHubAuthentication auth = GitHubExternalApp.GetAuth(requestContext, projectId, userConnection);
      GitHubResult<GitHubData.V3.Repository> userRepo = gitHubHttpClient.GetUserRepo(auth, repositoryId);
      if (!userRepo.IsSuccessful)
      {
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.ExternalApp, nameof (GitHubExternalApp), "User doesn't have access to repo {0} via connection id {1}.", (object) repositoryId, (object) userConnection.Id);
        return false;
      }
      GitHubData.V3.RepositoryPermissions permissions = userRepo.Result.Permissions;
      if ((permissions != null ? (!permissions.Admin ? 1 : 0) : 1) != 0)
      {
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.ExternalApp, nameof (GitHubExternalApp), "User doesn't have Admin permissions to repo {0} via connection id {1}.", (object) repositoryId, (object) userConnection.Id);
        GitHubResult<GitHubData.V3.OrgMembership> orgMembership = gitHubHttpClient.GetOrgMembership(auth, repoOwner, user);
        if (!orgMembership.IsSuccessful)
        {
          requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.ExternalApp, nameof (GitHubExternalApp), "User '{0}' doesn't have access to org {1} via connection id {2}.", (object) user, (object) repoOwner, (object) userConnection.Id);
          return false;
        }
        if (!string.Equals(orgMembership.Result.Role, "admin", StringComparison.OrdinalIgnoreCase))
        {
          requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.ExternalApp, nameof (GitHubExternalApp), "User '{0}' doesn't have Admin permissions to org {1} via connection id {2}.", (object) user, (object) repoOwner, (object) userConnection.Id);
          return false;
        }
      }
      return true;
    }

    public bool RequireValidation(IVssRequestContext requestContext, string installationId)
    {
      string repositoryId;
      int num = !UserServiceHelper.HasUserContext(requestContext, out repositoryId) ? 1 : (repositoryId == null ? 1 : 0);
      if (num != 0)
        return num != 0;
      this.ThrowIfTampered(requestContext, repositoryId, installationId);
      return num != 0;
    }

    public bool ManageConnections(IVssRequestContext requestContext) => true;

    public string GetValidationUrl(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      ServiceEndpoint connectionToCreate,
      string installationId,
      string redirectUrl)
    {
      UriBuilder uriBuilder = new UriBuilder();
      uriBuilder.AppendQuery("project_id", projectInfo.Id.ToString());
      uriBuilder.AppendQuery("installation_id", installationId);
      uriBuilder.AppendQuery("name", connectionToCreate.Name);
      uriBuilder.AppendQuery(nameof (redirectUrl), redirectUrl);
      ConnectedServiceProvider provider = requestContext.GetService<ConnectedServiceProviderService>().GetProvider(requestContext, "githubapp");
      IVssRequestContext requestContext1 = requestContext;
      Guid id = projectInfo.Id;
      string str = uriBuilder.Query.TrimStart('?');
      Guid oauthConfigurationId = new Guid();
      string callbackQueryParams = str;
      return provider.CreateAuthRequest(requestContext1, id, (AuthRequest) null, oauthConfigurationId, callbackQueryParams: callbackQueryParams).Url;
    }

    private void ThrowIfTampered(
      IVssRequestContext requestContext,
      string repositoryId,
      string installationId)
    {
      string installationIdForRepository = this.GetInstallationIdForRepository(requestContext, repositoryId);
      if (!string.Equals(installationIdForRepository, installationId))
      {
        requestContext.TraceError(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.CreateConnection, nameof (GitHubExternalApp), "provided installation " + installationId + " does not match GitHub reported installation " + installationIdForRepository + " for " + repositoryId + ".");
        throw new ConnectionFailureException("Unrecognized installationId");
      }
    }

    private IGitHubAppAccessTokenProvider GetGitHubAppAccessTokenProvider(
      IVssRequestContext requestContext)
    {
      IGitHubAppAccessTokenProvider accessTokenProvider = GitHubHttpClientFactory.GetAppAccessTokenProvider(requestContext, GitHubAppType.Pipelines);
      if (accessTokenProvider != null)
        return accessTokenProvider;
      requestContext.TraceError(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.CreateConnection, nameof (GitHubExternalApp), "Unable to load the IGitHubAppAccessTokenProvider extension.");
      return accessTokenProvider;
    }
  }
}
