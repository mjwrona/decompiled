// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.GitHubApiRoot
// Assembly: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4FE25D33-B783-4B98-BAFC-7E522D8D8D08
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi
{
  public class GitHubApiRoot
  {
    private static readonly Uri githubApiRoot = new Uri("https://api.github.com");
    private static readonly Uri githubGraphQl = new Uri("https://api.github.com/graphql");

    public Uri Uri { get; }

    public Uri GraphQlUri { get; }

    public GitHubApiRoot(string apiRoot)
      : this(apiRoot == null ? (Uri) null : new Uri(apiRoot))
    {
    }

    public GitHubApiRoot(Uri apiRoot = null)
    {
      if (apiRoot == (Uri) null || GitHubRoot.IsGitHubDomain(apiRoot))
      {
        this.Uri = GitHubApiRoot.githubApiRoot;
        this.GraphQlUri = GitHubApiRoot.githubGraphQl;
      }
      else
      {
        this.Uri = new UriBuilder(apiRoot).AppendPathSegments("api", "v3").Uri;
        this.GraphQlUri = new UriBuilder(apiRoot).AppendPathSegments("api", "graphql").Uri;
      }
    }

    public UriBuilder AppendPath(params string[] pathSegments) => new UriBuilder(this.Uri).AppendPathSegments(pathSegments);

    public UriBuilder RepositoryUri(string repository)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(repository, nameof (repository));
      return this.AppendPath("repos", repository);
    }

    public UriBuilder CommitsUri(string repository, string startingFromRef = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(repository, nameof (repository));
      UriBuilder uriBuilder = this.RepositoryUri(repository).AppendPathSegments("commits");
      if (startingFromRef != null)
        uriBuilder.AppendQuery("sha", startingFromRef);
      return uriBuilder;
    }

    public UriBuilder CommitUri(string repository, string sha)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(repository, nameof (repository));
      return this.CommitsUri(repository).AppendPathSegments(sha);
    }

    public UriBuilder PullRequestUri(string repository, string prNumber)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(repository, nameof (repository));
      ArgumentUtility.CheckStringForNullOrEmpty(prNumber, nameof (prNumber));
      return this.RepositoryUri(repository).AppendPathSegments("pulls", prNumber);
    }

    public UriBuilder ContentsUri(string repository, string path, string version = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(repository, nameof (repository));
      ArgumentUtility.CheckForNull<string>(path, nameof (path));
      UriBuilder uriBuilder = this.RepositoryUri(repository).AppendPathSegments("contents", path.TrimStart('/'));
      if (!string.IsNullOrEmpty(version))
        uriBuilder.AppendQuery("ref", version);
      return uriBuilder;
    }

    public UriBuilder SearchCommitsUri() => this.AppendPath("search", "commits");
  }
}
