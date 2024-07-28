// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.GitHubAuthentication
// Assembly: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4FE25D33-B783-4B98-BAFC-7E522D8D8D08
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.dll

using System;

namespace Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi
{
  public class GitHubAuthentication
  {
    public GitHubAuthentication(string accessToken)
      : this(GitHubAuthScheme.Token, accessToken)
    {
    }

    public GitHubAuthentication(
      int installationId,
      string installationAccessToken,
      string expiresAt)
    {
      this.Scheme = GitHubAuthScheme.InstallationToken;
      this.InstallationId = installationId;
      this.InstallationAccessToken = new GitHubData.InstallationAccessToken()
      {
        Token = installationAccessToken,
        Expires_at = Convert.ToDateTime(expiresAt)
      };
    }

    public GitHubAuthentication(GitHubAuthScheme scheme, string accessToken)
    {
      this.Scheme = scheme;
      this.AccessToken = accessToken;
    }

    public GitHubAuthentication(GitHubAuthScheme scheme, string accessToken, int installationId)
    {
      this.Scheme = scheme;
      this.AccessToken = accessToken;
      this.InstallationId = installationId;
    }

    public GitHubAuthentication(int installationId, string accessToken)
    {
      this.Scheme = GitHubAuthScheme.ApplicationOAuthToken;
      this.AccessToken = accessToken;
      this.InstallationId = installationId;
    }

    public GitHubAuthentication(string username, string password)
    {
      this.Scheme = GitHubAuthScheme.Basic;
      this.Username = username;
      this.Password = password;
    }

    public GitHubAuthentication(int installationId)
    {
      this.Scheme = GitHubAuthScheme.InstallationToken;
      this.InstallationId = installationId;
    }

    public bool UseFreshAccessToken { get; set; }

    public GitHubPermissionLevel PermissionLevel { get; set; }

    public string AccessToken { get; }

    public string Username { get; }

    public string Password { get; }

    public int InstallationId { get; }

    public GitHubData.InstallationAccessToken InstallationAccessToken { get; set; }

    public GitHubAuthScheme Scheme { get; }

    public bool AcceptUntrustedCertificates { get; set; }

    public string ToTelemetryString() => string.Format("GitHubAuthentication (Scheme: {0})", (object) this.Scheme);
  }
}
