// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.GitHubRoot
// Assembly: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4FE25D33-B783-4B98-BAFC-7E522D8D8D08
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;

namespace Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi
{
  public class GitHubRoot
  {
    private static readonly Uri github = new Uri("https://github.com");

    public Uri Uri { get; }

    public GitHubRoot(string root)
      : this(root == null ? (Uri) null : new Uri(root))
    {
    }

    public GitHubRoot(Uri root = null)
    {
      if (root == (Uri) null || GitHubRoot.IsGitHubDomain(root))
        this.Uri = GitHubRoot.github;
      else
        this.Uri = root;
    }

    public UriBuilder AppendPath(params string[] pathSegments) => new UriBuilder(this.Uri).AppendPathSegments(pathSegments);

    public UriBuilder RepositoryUri(string repository)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(repository, nameof (repository));
      return this.AppendPath(repository);
    }

    public Uri CloneUri(string repository)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(repository, nameof (repository));
      return this.AppendPath(repository + ".git").Uri;
    }

    public Uri OauthAuthorizeUri(string clientId, string state, string scope = null, string redirectUri = null)
    {
      ArgumentUtility.CheckForNull<string>(clientId, nameof (clientId));
      ArgumentUtility.CheckForNull<string>(state, nameof (state));
      UriBuilder uriBuilder = this.AppendPath("login", "oauth", "authorize").AppendQuery("client_id", clientId).AppendQuery(nameof (state), state);
      if (scope != null)
        uriBuilder.AppendQuery(nameof (scope), scope);
      if (redirectUri != null)
        uriBuilder.AppendQuery("redirect_uri", redirectUri);
      return uriBuilder.Uri;
    }

    public static bool IsGitHubDomain(Uri uri)
    {
      ArgumentUtility.CheckForNull<Uri>(uri, nameof (uri));
      if (!uri.IsAbsoluteUri)
        throw new ArgumentException(CommonResources.InvalidUriError((object) UriKind.Absolute), nameof (uri));
      return string.Equals(uri.Host, "github.com", StringComparison.OrdinalIgnoreCase) || uri.Host.EndsWith(".github.com", StringComparison.OrdinalIgnoreCase);
    }
  }
}
