// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.GithubServiceEndpointExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public static class GithubServiceEndpointExtensions
  {
    private static readonly string GitHubAppConnectedServiceProviderId = "GitHubApp";
    private static readonly string GitHubConnectedServiceProviderId = "GitHub";

    public static GitHubAuthentication GetGitHubAuthentication(
      this ServiceEndpoint serviceEndpoint,
      IVssRequestContext requestContext,
      Guid projectId)
    {
      if (serviceEndpoint?.Authorization?.Parameters == null || !serviceEndpoint.IsGitHubFamily())
        return (GitHubAuthentication) null;
      GitHubAuthentication hubAuthentication = (GitHubAuthentication) null;
      if (serviceEndpoint.Authorization.IsToken())
      {
        string accessToken1;
        if (serviceEndpoint.Authorization.Parameters.TryGetValue("AccessToken", out accessToken1) && !string.IsNullOrEmpty(accessToken1))
        {
          string str;
          GitHubAuthScheme result;
          string s;
          hubAuthentication = !serviceEndpoint.Authorization.Parameters.TryGetValue("AccessTokenType", out str) || !Enum.TryParse<GitHubAuthScheme>(str, out result) ? new GitHubAuthentication(accessToken1) : (result != GitHubAuthScheme.ApplicationOAuthToken || !serviceEndpoint.Authorization.Parameters.TryGetValue("IdToken", out s) ? new GitHubAuthentication(result, accessToken1) : new GitHubAuthentication(int.Parse(s), accessToken1));
        }
        else
        {
          string accessToken2;
          if (serviceEndpoint.Authorization.Parameters.TryGetValue("ApiToken", out accessToken2) && !string.IsNullOrEmpty(accessToken2))
            hubAuthentication = new GitHubAuthentication(accessToken2);
          else if (serviceEndpoint.Authorization.Scheme.Equals("InstallationToken", StringComparison.OrdinalIgnoreCase))
          {
            string s;
            serviceEndpoint.Authorization.Parameters.TryGetValue("IdToken", out s);
            hubAuthentication = new GitHubAuthentication(int.Parse(s));
          }
        }
      }
      else if (serviceEndpoint.Authorization.IsBasic())
      {
        string username;
        serviceEndpoint.Authorization.Parameters.TryGetValue("Username", out username);
        string str;
        serviceEndpoint.Authorization.Parameters.TryGetValue("Password", out str);
        if (!string.IsNullOrEmpty(username))
          hubAuthentication = new GitHubAuthentication(username, str);
        else if (!string.IsNullOrEmpty(str))
          hubAuthentication = new GitHubAuthentication(str);
      }
      string a;
      if (hubAuthentication != null && serviceEndpoint.Data != null && serviceEndpoint.Data.TryGetValue("acceptUntrustedCerts", out a))
        hubAuthentication.AcceptUntrustedCertificates = string.Equals(a, "true", StringComparison.OrdinalIgnoreCase);
      return hubAuthentication;
    }

    public static bool IsGitHubFamily(this ServiceEndpoint serviceEndpoint) => string.Equals(serviceEndpoint?.Type, "GitHub", StringComparison.OrdinalIgnoreCase) || string.Equals(serviceEndpoint?.Type, "GitHubEnterprise", StringComparison.OrdinalIgnoreCase) || string.Equals(serviceEndpoint?.Type, "GitHubBoards", StringComparison.OrdinalIgnoreCase) || string.Equals(serviceEndpoint?.Type, "GitHubEnterpriseBoards", StringComparison.OrdinalIgnoreCase);

    public static bool IsGitHub(this ServiceEndpoint serviceEndpoint) => string.Equals(serviceEndpoint?.Type, "GitHub", StringComparison.OrdinalIgnoreCase);

    public static bool AllowsWebhooks(
      this ServiceEndpoint serviceEndpoint,
      IVssRequestContext requestContext)
    {
      requestContext.Trace(34001100, TraceLevel.Info, "DistributedTask", nameof (GithubServiceEndpointExtensions), serviceEndpoint?.Id.ToString() + "scheme:" + serviceEndpoint?.Authorization?.Scheme);
      return !serviceEndpoint.IsGitHub() || serviceEndpoint.Authorization == null || !serviceEndpoint.Authorization.Scheme.Equals("InstallationToken", StringComparison.OrdinalIgnoreCase);
    }

    public static string GetGitHubConnectedServiceProviderName(
      this ServiceEndpoint serviceEndpoint,
      IVssRequestContext requestContext)
    {
      if (!serviceEndpoint.IsGitHub())
        return (string) null;
      return serviceEndpoint.Authorization != null && serviceEndpoint.Authorization.Scheme.Equals("InstallationToken", StringComparison.OrdinalIgnoreCase) ? GithubServiceEndpointExtensions.GitHubAppConnectedServiceProviderId : GithubServiceEndpointExtensions.GitHubConnectedServiceProviderId;
    }
  }
}
