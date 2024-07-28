// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.GitHubClientHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.Common;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Cloud;
using System;
using System.Diagnostics;
using System.Net;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal class GitHubClientHelper : IGitHubClientHelper
  {
    private static readonly TimeSpan s_defaultCircuitBreakerTimeout = TimeSpan.FromSeconds(10.0);

    public virtual GitHubData.V3.User[] SearchUsers(
      IVssRequestContext requestContext,
      string searchQuery,
      int maxResults)
    {
      return GitHubUserProvider.SearchUsers(requestContext, searchQuery, maxResults);
    }

    public virtual GitHubData.V3.User GetUserByLogin(
      IVssRequestContext requestContext,
      string userLogin)
    {
      GitHubResult<GitHubData.V3.User> gitHubResult = GitHubClientHelper.ExecuteGitHubOperation<GitHubResult<GitHubData.V3.User>>(requestContext, new Func<GitHubHttpClient, GitHubAuthentication, GitHubResult<GitHubData.V3.User>>(getUserByLogin));
      if (!gitHubResult.IsSuccessful || gitHubResult.Result == null)
      {
        requestContext.TraceAlways(15007010, TraceLevel.Error, "DirectoryDiscovery", "GitHubDirectory", HostingResources.GitHubDirectoryGetUserFailed((object) "login", (object) userLogin, (object) gitHubResult.ErrorMessage));
        GitHubClientHelper.CheckResultStatusCode<GitHubData.V3.User>(gitHubResult);
        throw new DirectoryDiscoveryUserNotFoundException(HostingResources.GitHubDirectoryGetUserFailed((object) "login", (object) userLogin, (object) gitHubResult.ErrorMessage));
      }
      return gitHubResult.Result;

      GitHubResult<GitHubData.V3.User> getUserByLogin(
        GitHubHttpClient client,
        GitHubAuthentication auth)
      {
        return client.GetUserByLogin(userLogin, auth);
      }
    }

    public virtual GitHubData.V3.User GetUserById(IVssRequestContext requestContext, string userId)
    {
      GitHubResult<GitHubData.V3.User> gitHubResult = GitHubClientHelper.ExecuteGitHubOperation<GitHubResult<GitHubData.V3.User>>(requestContext, new Func<GitHubHttpClient, GitHubAuthentication, GitHubResult<GitHubData.V3.User>>(getUserById));
      if (!gitHubResult.IsSuccessful || gitHubResult.Result == null)
      {
        requestContext.TraceAlways(15007020, TraceLevel.Error, "DirectoryDiscovery", "GitHubDirectory", HostingResources.GitHubDirectoryGetUserFailed((object) "id", (object) userId, (object) gitHubResult.ErrorMessage));
        GitHubClientHelper.CheckResultStatusCode<GitHubData.V3.User>(gitHubResult);
        throw new DirectoryDiscoveryUserNotFoundException(HostingResources.GitHubDirectoryGetUserFailed((object) "id", (object) userId, (object) gitHubResult.ErrorMessage));
      }
      return gitHubResult.Result;

      GitHubResult<GitHubData.V3.User> getUserById(
        GitHubHttpClient client,
        GitHubAuthentication auth)
      {
        return client.GetUserById(userId, auth);
      }
    }

    public virtual byte[] GetUserAvatar(IVssRequestContext requestContext, string userId)
    {
      GitHubData.V3.User userResult = this.GetUserById(requestContext, userId);
      return string.IsNullOrWhiteSpace(userResult.Avatar_url) ? (byte[]) null : GitHubClientHelper.ExecuteGitHubOperation<byte[]>(requestContext, new Func<GitHubHttpClient, GitHubAuthentication, byte[]>(getUserAvatar));

      byte[] getUserAvatar(GitHubHttpClient client, GitHubAuthentication auth)
      {
        using (WebClient webClient = new WebClient())
          return webClient.DownloadData(userResult.Avatar_url);
      }
    }

    private static TResult ExecuteGitHubOperation<TResult>(
      IVssRequestContext requestContext,
      Func<GitHubHttpClient, GitHubAuthentication, TResult> operation)
    {
      string errorMessage;
      GitHubAuthentication authentication;
      if (!GitHubUserProvider.TryGetGitHubUserAuthentication(requestContext, out authentication, out errorMessage))
      {
        requestContext.TraceAlways(15007030, TraceLevel.Error, "DirectoryDiscovery", "GitHubDirectory", HostingResources.GitHubUserAuthenticationRetrievalFailed((object) errorMessage));
        throw new DirectoryDiscoveryServiceAccessException(HostingResources.GitHubUserAuthenticationRetrievalFailed((object) errorMessage));
      }
      GitHubHttpClient gitHubclient = GitHubHttpClientFactory.Create(requestContext);
      string name = operation.Method.Name;
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "EnterpriseAuthorization.").AndCommandKey((CommandKey) name).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(GitHubClientHelper.s_defaultCircuitBreakerTimeout).WithExecutionMaxConcurrentRequests(int.MaxValue));
      CommandPropertiesRegistry properties = new CommandPropertiesRegistry(requestContext, (CommandKey) name, setter.CommandPropertiesDefaults);
      return new CommandService<TResult>(requestContext, setter, (ICommandProperties) properties, (Func<TResult>) (() => operation(gitHubclient, authentication))).Execute();
    }

    private static void CheckResultStatusCode<T>(GitHubResult<T> gitHubResult)
    {
      if (gitHubResult.StatusCode == HttpStatusCode.Forbidden)
        throw new DirectoryDiscoveryServiceAccessException(HostingResources.GitHubRequestIsForbidden((object) gitHubResult.ErrorMessage));
    }
  }
}
