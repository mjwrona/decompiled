// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.GitHubHttpClient
// Assembly: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4FE25D33-B783-4B98-BAFC-7E522D8D8D08
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.dll

using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.Models.Cache;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi
{
  public class GitHubHttpClient
  {
    private static readonly string s_rateLimit = "rateLimit";
    private static readonly string s_cost = "cost";
    protected static readonly string s_rateLimitField = GitHubHttpClient.s_rateLimit + " { " + GitHubHttpClient.GraphQLTypeInspector.GetScalarFields<GitHubData.V4.RateLimit>() + " }";
    private static readonly GraphQLQuery s_repositoryOwnerQuery = new GraphQLQuery("query($login:String!) {\r\n                " + GitHubHttpClient.s_rateLimitField + "\r\n                item: repositoryOwner(login: $login) {\r\n                    " + GitHubHttpClient.GraphQLTypeInspector.GetScalarFields<GitHubData.V4.IRepositoryOwner>() + "\r\n                }\r\n            }");
    private static readonly GraphQLQuery s_repoByIdQuery = new GraphQLQuery("query ($nodeIds:[ID!]!) {\r\n                 " + GitHubHttpClient.s_rateLimitField + "\r\n                 item: nodes(ids: $nodeIds) {\r\n                   id\r\n                   ... on Repository {\r\n                     id\r\n                     databaseId\r\n                     name\r\n                     nameWithOwner\r\n                     isPrivate\r\n                     url\r\n                     viewerCanAdminister\r\n                     viewerPermission\r\n                     primaryLanguage {\r\n                       name\r\n                     }\r\n                   }\r\n                 }\r\n               }");
    private static readonly GraphQLQuery s_repoByNameQuery = new GraphQLQuery("query ($nodeNamesList:String!) {\r\n                 " + GitHubHttpClient.s_rateLimitField + "\r\n                 search(type: REPOSITORY, query: $nodeNamesList, first: 100) {\r\n                     item: nodes {\r\n                       ... on Repository {\r\n                         id\r\n                         databaseId\r\n                         name\r\n                         nameWithOwner\r\n                         isPrivate\r\n                         url\r\n                         viewerCanAdminister\r\n                         viewerPermission\r\n                         primaryLanguage {\r\n                           name\r\n                         }\r\n                       }\r\n                     }\r\n                  }\r\n               }");
    private static readonly GraphQLQuery s_viewer = new GraphQLQuery("query {\r\n                 " + GitHubHttpClient.s_rateLimitField + "\r\n                 item: viewer {\r\n                   " + GitHubHttpClient.GraphQLTypeInspector.GetScalarFields<GitHubData.V4.User>() + "\r\n                 }\r\n               }");
    public static readonly string s_fileNotFoundErrorCode = "FileNotFound";
    public static readonly string s_fileTooBigErrorCode = "FileTooBig";
    public static readonly long s_maxFileSizeSupported = 4194304;
    public static readonly string s_registryMaxPageResultsOverride = "/sourceproviders/github/maxpageresults";
    public static readonly string s_base64Encoding = "base64";
    private static string s_userAgentValue = (string) null;
    private static readonly Regex s_nextLinkHeaderRegex = new Regex("\\<([^>]+)\\>; rel=\"next\"", RegexOptions.Compiled);
    private static readonly Regex s_lastLinkHeaderRegex = new Regex("page=([0-9]*)\\>; rel=\"last\"", RegexOptions.Compiled);
    private static readonly DateTime s_epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly string s_acceptHeaderForGitHubChecks = "application/vnd.github.antiope-preview+json";
    private const int s_pageLen = 100;
    public const int c_defaultMaxPaginatedResults = 2000;
    private readonly IGitHubAppAccessTokenProvider m_tokenProvider;
    private readonly IExternalProviderHttpRequesterFactory m_gitHubHttpRequesterFactory;
    private readonly IGitHubRateLimitTracer m_rateLimitTracer;
    private readonly IGitHubConditionalResponseTracer m_conditionalResponseTracer;
    private readonly IGitHubInstallationAccessTokenCache m_tokenCache;
    private readonly int m_maxPaginatedResults;
    private readonly TimeSpan? m_timeout;
    private readonly CancellationToken? m_cancellationToken;

    internal GitHubHttpClient()
    {
    }

    internal GitHubHttpClient(
      IExternalProviderHttpRequesterFactory gitHubHttpRequesterFactory,
      IGitHubRateLimitTracer rateLimitTracer,
      IGitHubConditionalResponseTracer conditionalResponseTracer,
      IGitHubAppAccessTokenProvider tokenProvider = null,
      int maxResults = 2000,
      TimeSpan? timeout = null,
      IGitHubInstallationAccessTokenCache tokenCache = null,
      CancellationToken? cancellationToken = null)
    {
      this.m_gitHubHttpRequesterFactory = gitHubHttpRequesterFactory;
      this.m_rateLimitTracer = rateLimitTracer;
      this.m_tokenProvider = tokenProvider;
      this.m_maxPaginatedResults = maxResults;
      this.m_timeout = timeout;
      this.m_tokenCache = tokenCache;
      this.m_cancellationToken = cancellationToken;
      this.m_conditionalResponseTracer = conditionalResponseTracer;
    }

    public GitHubResult<GitHubData.V3.BlobObject> GetBlob(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      string sha)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository);
      return this.GetBlob(authentication, uriBuilder.AbsoluteUri(), sha);
    }

    public GitHubResult<GitHubData.V3.BlobObject> GetBlob(
      GitHubAuthentication authentication,
      string repoUrl,
      string sha)
    {
      UriBuilder uriBuilder = new UriBuilder(repoUrl);
      uriBuilder.AppendPathSegments("git", "blobs", sha);
      return this.QueryItem<GitHubData.V3.BlobObject>(authentication, uriBuilder.AbsoluteUri(), nameof (GetBlob));
    }

    public GitHubResult<GitHubData.V3.BlobObject> GetBlob(
      GitHubAuthentication authentication,
      string blobUrl)
    {
      UriBuilder uriBuilder = new UriBuilder(blobUrl);
      return this.QueryItem<GitHubData.V3.BlobObject>(authentication, uriBuilder.AbsoluteUri(), nameof (GetBlob));
    }

    public GitHubResult<GitHubData.V3.Owner> GetCurrentUser(GitHubAuthentication authentication) => this.GetCurrentUser((string) null, authentication);

    public GitHubResult<GitHubData.V3.Owner> GetCurrentUser(
      string enterpriseUrl,
      GitHubAuthentication authentication)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).AppendPath("user");
      return this.QueryItem<GitHubData.V3.Owner>(authentication, uriBuilder.AbsoluteUri(), nameof (GetCurrentUser));
    }

    public GitHubResult<GitHubData.V3.SecretPublicKey> GetSecretsPublicKey(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository).AppendPathSegments("actions", "secrets", "public-key");
      return this.QueryItem<GitHubData.V3.SecretPublicKey>(authentication, uriBuilder.AbsoluteUri(), nameof (GetSecretsPublicKey));
    }

    public GitHubResult<GitHubData.V3.Secret> CreateSecret(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      string secretName,
      GitHubData.V3.EncryptedSecret encryptedSecret)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository).AppendPathSegments("actions", "secrets", secretName);
      string jsonIn = JsonConvert.SerializeObject((object) new Dictionary<string, object>(2)
      {
        ["key_id"] = (object) encryptedSecret.Key_id,
        ["encrypted_value"] = (object) encryptedSecret.Encrypted_value
      });
      return this.CreateItem<GitHubData.V3.Secret>(authentication, uriBuilder.AbsoluteUri(), jsonIn, httpMethod: HttpMethod.Put, entryMethodName: nameof (CreateSecret));
    }

    public GitHubResult<GitHubData.V3.Repository[]> GetUserInstallationRepositories(
      GitHubAuthentication authentication,
      string installationId)
    {
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new GitHubApiRoot().AppendPath("user", "installations", installationId, "repositories").AppendQuery("per_page", 100.ToString()).AbsoluteUri()))
      {
        request.Headers.Add("Accept", "application/vnd.github.machine-man-preview+json");
        GitHubResult<GitHubData.InstallationRepositories> gitHubResult = this.SendRequestForSingleItem<GitHubData.InstallationRepositories>(request, authentication, nameof (GetUserInstallationRepositories));
        return new GitHubResult<GitHubData.V3.Repository[]>(gitHubResult.Result?.repositories, gitHubResult.Errors, gitHubResult.StatusCode);
      }
    }

    public GitHubResult<GitHubData.V3.CommitListItem> GetCommit(
      GitHubAuthentication authentication,
      string repoUrl,
      string sha)
    {
      UriBuilder uriBuilder = new UriBuilder(repoUrl).AppendPathSegments("commits", sha);
      return this.QueryItem<GitHubData.V3.CommitListItem>(authentication, uriBuilder.AbsoluteUri(), nameof (GetCommit));
    }

    public GitHubResult<GitHubData.V3.CommitListItem> GetCommit(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      string sha)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).CommitUri(repository, sha);
      return this.QueryItem<GitHubData.V3.CommitListItem>(authentication, uriBuilder.AbsoluteUri(), nameof (GetCommit));
    }

    public GitHubResult<GitHubData.V3.CommitListItem[]> GetCommits(
      GitHubAuthentication authentication,
      string commitsUrl)
    {
      return this.QueryItem<GitHubData.V3.CommitListItem[]>(authentication, commitsUrl, nameof (GetCommits));
    }

    public GitHubResult<GitHubData.V4.Commit[]> GetCommits(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      IEnumerable<string> shas)
    {
      string str1 = "fragment commitInfo on Commit { " + GitHubHttpClient.GraphQLTypeInspector.GetScalarFields<GitHubData.V4.Commit>() + "   author {" + GitHubHttpClient.GraphQLTypeInspector.GetAllFields<GitHubData.V4.GitActor>(1) + "   }   committer {" + GitHubHttpClient.GraphQLTypeInspector.GetAllFields<GitHubData.V4.GitActor>(1) + "   }}";
      string commitTemplate = "_{0}: object(oid: \"{1}\") {{ ...commitInfo }}";
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      string str2 = string.Join(" ", shas.Where<string>((Func<string, bool>) (sha => !string.IsNullOrEmpty(sha))).Select<string, string>(GitHubHttpClient.\u003C\u003EO.\u003C0\u003E__EscapeValue ?? (GitHubHttpClient.\u003C\u003EO.\u003C0\u003E__EscapeValue = new Func<string, string>(GraphQLQuery.EscapeValue))).Select<string, string>((Func<string, int, string>) ((sha, i) => string.Format(commitTemplate, (object) i, (object) sha))));
      GraphQLQuery query = new GraphQLQuery("query($owner:String!, $name:String!) {\r\n                    " + GitHubHttpClient.s_rateLimitField + "\r\n                    item: repository(owner: $owner, name: $name) {\r\n                        " + str2 + "\r\n                    }\r\n                }\r\n                " + str1);
      string owner;
      string name;
      GitHubHttpClient.ExtractRepositoryOwnerAndName(repository, out owner, out name);
      Dictionary<string, object> variables = new Dictionary<string, object>()
      {
        {
          "owner",
          (object) owner
        },
        {
          "name",
          (object) name
        }
      };
      return this.QueryGraphQLItems<GitHubData.V4.Commit>(enterpriseUrl, authentication, query, (IDictionary<string, object>) variables, nameof (GetCommits));
    }

    public GitHubResult<GitHubData.V4.Issue[]> GetIssues(
      GitHubAuthentication authentication,
      string repository,
      IEnumerable<string> issueIds)
    {
      return this.GetIssues((string) null, authentication, repository, issueIds);
    }

    public GitHubResult<GitHubData.V4.Issue[]> GetIssues(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      IEnumerable<string> issueIds)
    {
      string str1 = "fragment issueInfo on Issue {\r\n                    " + GitHubHttpClient.GraphQLTypeInspector.GetScalarFields<GitHubData.V4.Issue>() + "\r\n                    assignees(last: 1) {\r\n                    edges {\r\n                        node {\r\n                        " + GitHubHttpClient.GraphQLTypeInspector.GetScalarFields<GitHubData.V4.User>() + "\r\n                        }\r\n                    }\r\n                    }\r\n                }";
      string issueTemplate = "_{0}: issue(number: {1}) {{ ...issueInfo }}";
      string str2 = string.Join(" ", issueIds.Where<string>((Func<string, bool>) (id => int.TryParse(id, out int _))).Select<string, string>((Func<string, int, string>) ((issueId, i) => string.Format(issueTemplate, (object) i, (object) issueId))));
      GraphQLQuery query = new GraphQLQuery("query($owner:String!, $name:String!) {\r\n                    " + GitHubHttpClient.s_rateLimitField + "\r\n                    item: repository(owner: $owner, name: $name) {\r\n                        " + str2 + "\r\n                    }\r\n                }\r\n                " + str1);
      string owner;
      string name;
      GitHubHttpClient.ExtractRepositoryOwnerAndName(repository, out owner, out name);
      Dictionary<string, object> variables = new Dictionary<string, object>()
      {
        {
          "owner",
          (object) owner
        },
        {
          "name",
          (object) name
        }
      };
      return this.QueryGraphQLItems<GitHubData.V4.Issue>(enterpriseUrl, authentication, query, (IDictionary<string, object>) variables, nameof (GetIssues), true);
    }

    public GitHubResult<GitHubData.V4.PullRequest> GetPullRequest(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      int prNumber,
      int lastCommitsToInclude)
    {
      GraphQLQuery query = new GraphQLQuery("query($owner:String!, $name:String!, $prNumber:Int!, $lastCommitsToInclude:Int!) {\r\n                    " + GitHubHttpClient.s_rateLimitField + "\r\n                    item: repository(owner: $owner, name: $name) {\r\n                        pullRequest(number: $prNumber) {\r\n                            " + GitHubHttpClient.GraphQLTypeInspector.GetAllFields<GitHubData.V4.PullRequest>(1) + "\r\n                            commits(last: $lastCommitsToInclude) {\r\n                                edges {\r\n                                    node {\r\n                                        commit {\r\n                                            " + GitHubHttpClient.GraphQLTypeInspector.GetAllFields<GitHubData.V4.Commit>(1) + "\r\n                                        }\r\n                                    }\r\n                                }\r\n                            },\r\n                            headRef {\r\n                                target {\r\n                                    __typename\r\n                                    ...on Commit {\r\n                                        " + GitHubHttpClient.GraphQLTypeInspector.GetAllFields<GitHubData.V4.Commit>(1) + "\r\n                                    }\r\n                                }\r\n                            }\r\n                        }\r\n                    }\r\n                }");
      string owner;
      string name;
      GitHubHttpClient.ExtractRepositoryOwnerAndName(repository, out owner, out name);
      Dictionary<string, object> variables = new Dictionary<string, object>()
      {
        {
          "owner",
          (object) owner
        },
        {
          "name",
          (object) name
        },
        {
          nameof (prNumber),
          (object) prNumber
        },
        {
          nameof (lastCommitsToInclude),
          (object) lastCommitsToInclude
        }
      };
      return this.QueryGraphQLItem<GitHubData.V4.Repository>(authentication, enterpriseUrl, query, (IDictionary<string, object>) variables, nameof (GetPullRequest)).Convert<GitHubData.V4.PullRequest>((Func<GitHubData.V4.Repository, GitHubData.V4.PullRequest>) (x => x?.PullRequest));
    }

    public GitHubResult<GitHubData.V4.PullRequest[]> GetPullRequestsPaginated(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      string filter,
      int limit,
      string after = null)
    {
      GraphQLQuery query = new GraphQLQuery("query repository($query: String!, $limit: Int!, $after: String) {\r\n                    " + GitHubHttpClient.s_rateLimitField + "\r\n                    paginatedItems: search(type: ISSUE, first: $limit, after: $after, query: $query) {\r\n                        items: nodes {\r\n                            ... on PullRequest {\r\n                                " + GitHubHttpClient.GraphQLTypeInspector.GetScalarFields<GitHubData.V4.PullRequest>() + "\r\n                            }\r\n                        }\r\n                        pageInfo {\r\n                            " + GitHubHttpClient.GraphQLTypeInspector.GetScalarFields<GitHubData.V4.PageInfo>() + "\r\n                        }\r\n                    }\r\n                }");
      string str = string.Empty;
      if (!string.IsNullOrEmpty(filter))
        str = " '" + GraphQLQuery.EscapeValue(filter) + "' in:title";
      Dictionary<string, object> variables = new Dictionary<string, object>()
      {
        {
          "query",
          (object) ("type:pr repo:" + GraphQLQuery.EscapeValue(repository) + " sort:updated-desc" + str)
        },
        {
          nameof (limit),
          (object) limit
        },
        {
          nameof (after),
          (object) after
        }
      };
      return this.QueryGraphQLItemsWithPagination<GitHubData.V4.PullRequest>(enterpriseUrl, authentication, query, (IDictionary<string, object>) variables, nameof (GetPullRequestsPaginated));
    }

    public GitHubResult<GitHubData.V4.Commit[]> GetCommitsPaginated(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      int limit,
      string after)
    {
      GraphQLQuery query = new GraphQLQuery("query repository($repository: String!, $owner: String!, $limit: Int!, $after: String) {\r\n                    " + GitHubHttpClient.s_rateLimitField + "\r\n                    repository(name: $repository, owner: $owner) {\r\n                        defaultBranchRef {\r\n                            target {\r\n                            ... on Commit {\r\n                                    paginatedItems: history(first: $limit, after: $after) {\r\n                                        pageInfo {\r\n                                            " + GitHubHttpClient.GraphQLTypeInspector.GetScalarFields<GitHubData.V4.PageInfo>() + "\r\n                                        }\r\n                                        items: nodes {\r\n                                            " + GitHubHttpClient.GraphQLTypeInspector.GetScalarFields<GitHubData.V4.Commit>() + "\r\n                                        }\r\n                                    }\r\n                                }\r\n                            }\r\n                        }\r\n                    }\r\n                }");
      string owner;
      string name;
      GitHubHttpClient.ExtractRepositoryOwnerAndName(repository, out owner, out name);
      Dictionary<string, object> variables = new Dictionary<string, object>()
      {
        {
          nameof (repository),
          (object) name
        },
        {
          "owner",
          (object) owner
        },
        {
          nameof (limit),
          (object) limit
        },
        {
          nameof (after),
          (object) after
        }
      };
      return this.QueryGraphQLItemsWithPagination<GitHubData.V4.Commit>(enterpriseUrl, authentication, query, (IDictionary<string, object>) variables, nameof (GetCommitsPaginated));
    }

    public GitHubResult<GitHubData.V3.CommitListItem[]> GetCommitsPaginated(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      string filter,
      int limit,
      string after,
      out string nextToken)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(filter, nameof (filter));
      string requestUri;
      if (string.IsNullOrWhiteSpace(after))
      {
        UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).SearchCommitsUri();
        uriBuilder.AppendQuery("q", "repo:" + repository + " " + filter);
        uriBuilder.AppendQuery("per_page", limit.ToString());
        requestUri = uriBuilder.AbsoluteUri();
      }
      else
        requestUri = after;
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUri))
        return this.SendSearchRequestForPaginatedItems<GitHubData.V3.CommitListItem>(request, authentication, out nextToken, nameof (GetCommitsPaginated));
    }

    public GitHubResult<GitHubData.V4.User> GetGraphQLUser(
      GitHubAuthentication authentication,
      string enterpriseUrl = null)
    {
      return this.QueryGraphQLItem<GitHubData.V4.User>(authentication, enterpriseUrl, GitHubHttpClient.s_viewer, entryMethodName: nameof (GetGraphQLUser));
    }

    public GitHubResult<GitHubData.V3.CommitsDiff> GetCommitsDiff(
      GitHubAuthentication authentication,
      string repository,
      string startSha,
      string endSha)
    {
      return this.GetCommitsDiff((string) null, authentication, repository, startSha, endSha);
    }

    public GitHubResult<GitHubData.V3.CommitsDiff> GetCommitsDiff(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      string startSha,
      string endSha)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository).AppendPathSegments("compare", startSha + "..." + endSha);
      return this.QueryItem<GitHubData.V3.CommitsDiff>(authentication, uriBuilder.AbsoluteUri(), nameof (GetCommitsDiff));
    }

    public GitHubResult<GitHubData.V3.CommitListItem[]> GetPullRequestCommits(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      int pullRequestNumber)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository).AppendPathSegments("pulls", pullRequestNumber.ToString(), "commits");
      return this.QueryItems<GitHubData.V3.CommitListItem>(authentication, uriBuilder.AbsoluteUri(), entryMethodName: nameof (GetPullRequestCommits));
    }

    public GitHubResult<GitHubData.V3.File[]> GetPullRequestFiles(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      int pullRequestNumber)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository).AppendPathSegments("pulls", pullRequestNumber.ToString(), "files");
      return this.QueryItems<GitHubData.V3.File>(authentication, uriBuilder.AbsoluteUri(), entryMethodName: nameof (GetPullRequestFiles));
    }

    public GitHubResult<GitHubData.V3.Ref> GetBranchRefHead(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      string branch)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository);
      if (branch.StartsWith("refs/", StringComparison.OrdinalIgnoreCase))
        uriBuilder.AppendPathSegments("git", branch);
      else
        uriBuilder.AppendPathSegments("git", "refs", "heads", branch);
      return this.QueryItem<GitHubData.V3.Ref>(authentication, uriBuilder.AbsoluteUri(), nameof (GetBranchRefHead));
    }

    public GitHubResult<GitHubData.V3.Branch[]> GetRepoBranches(
      GitHubAuthentication authentication,
      string branchesUrl)
    {
      return this.QueryItems<GitHubData.V3.Branch>(authentication, branchesUrl, entryMethodName: nameof (GetRepoBranches));
    }

    public GitHubResult<GitHubData.V3.Branch[]> GetRepoBranches(
      GitHubAuthentication authentication,
      string enterpriseUrl,
      string repository)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository).AppendPathSegments("branches");
      return this.GetRepoBranches(authentication, uriBuilder.AbsoluteUri());
    }

    public GitHubResult<GitHubData.V3.Branch[]> GetRepoBranches(
      GitHubAuthentication authentication,
      string enterpriseUrl,
      string repository,
      Func<string, TaggedResultData<GitHubData.V3.Branch[]>> GetCachedPage,
      Action<string, string, GitHubData.V3.Branch[]> SetCachedPage)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository).AppendPathSegments("branches");
      return this.QueryItems<GitHubData.V3.Branch>(authentication, new Uri(uriBuilder.AbsoluteUri()), GetCachedPage, SetCachedPage, entryMethodName: nameof (GetRepoBranches));
    }

    public GitHubResult<GitHubData.V3.Branch> GetRepoBranch(
      GitHubAuthentication authentication,
      string enterpriseUrl,
      string repository,
      string branch)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository).AppendPathSegments("branches", branch);
      return this.QueryItem<GitHubData.V3.Branch>(authentication, uriBuilder.AbsoluteUri(), nameof (GetRepoBranch));
    }

    public GitHubResult<GitHubData.V3.ContentData[]> GetContent(
      GitHubAuthentication authentication,
      string enterpriseUrl,
      string repository,
      string version,
      string path)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).ContentsUri(repository, path, version);
      return this.GetContent(authentication, uriBuilder.AbsoluteUri());
    }

    public GitHubResult<GitHubData.V3.ContentData[]> GetContent(
      GitHubAuthentication authentication,
      string contentUrl)
    {
      return this.QueryItems<GitHubData.V3.ContentData>(authentication, contentUrl, true, nameof (GetContent));
    }

    public GitHubResult<GitHubData.V3.ContentData> GetFileContentData(
      GitHubAuthentication authentication,
      string enterpriseUrl,
      string repository,
      string version,
      string filePath)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).ContentsUri(repository, filePath, version);
      return this.QueryItem<GitHubData.V3.ContentData>(authentication, uriBuilder.AbsoluteUri(), nameof (GetFileContentData));
    }

    public GitHubResult<GitHubData.V3.ContentData> GetReadmeFileContentData(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      string version = null)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository).AppendPathSegments("readme");
      if (!string.IsNullOrEmpty(version))
        uriBuilder.AppendQuery("ref", version);
      return this.QueryItem<GitHubData.V3.ContentData>(authentication, uriBuilder.AbsoluteUri(), nameof (GetReadmeFileContentData));
    }

    public GitHubResult<string> GetFileContentFromUrl(
      GitHubAuthentication authentication,
      string downloadUrl)
    {
      ArgumentUtility.CheckForNull<string>(downloadUrl, nameof (downloadUrl));
      return this.QueryStringItem(authentication, downloadUrl, nameof (GetFileContentFromUrl));
    }

    public GitHubResult<string> GetFileContent(
      GitHubAuthentication authentication,
      string repository,
      string version,
      string filePath)
    {
      return this.GetFileContent((string) null, authentication, repository, version, filePath);
    }

    public GitHubResult<string> GetFileContent(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      string version,
      string filePath)
    {
      GitHubResult<GitHubData.V3.ContentData> fileContentData = this.GetFileContentData(authentication, enterpriseUrl, repository, version, filePath);
      if (!fileContentData.IsSuccessful)
        return fileContentData.Convert<string>();
      return (long) fileContentData.Result.Size > GitHubHttpClient.s_maxFileSizeSupported ? GitHubResult<string>.Error(GitHubHttpClient.s_fileTooBigErrorCode, HttpStatusCode.RequestEntityTooLarge, (HttpResponseHeaders) null) : fileContentData.Convert<string>((Func<GitHubData.V3.ContentData, string>) (x =>
      {
        string s = x?.Content ?? string.Empty;
        if (!string.IsNullOrEmpty(s) && string.Equals(x.Encoding, GitHubHttpClient.s_base64Encoding, StringComparison.OrdinalIgnoreCase))
          s = Encoding.UTF8.GetString(Convert.FromBase64String(s));
        return s;
      }));
    }

    public GitHubResult<GitHubData.V3.ContentResponseData> CreateFileContent(
      GitHubAuthentication authentication,
      string repository,
      string filePath,
      string branch,
      string fileContent,
      string message,
      GitHubData.V3.User committer = null,
      GitHubData.V3.User author = null)
    {
      return this.CreateFileContent((string) null, authentication, repository, filePath, branch, fileContent, message, committer, author);
    }

    public GitHubResult<GitHubData.V3.ContentResponseData> CreateFileContent(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      string filePath,
      string branch,
      string fileContent,
      string message,
      GitHubData.V3.User committer = null,
      GitHubData.V3.User author = null)
    {
      this.ValidateContentAttributes(repository, fileContent, message, committer, author);
      if ((long) fileContent.Length > GitHubHttpClient.s_maxFileSizeSupported)
        return GitHubResult<GitHubData.V3.ContentResponseData>.Error(GitHubHttpClient.s_fileTooBigErrorCode, HttpStatusCode.BadRequest, (HttpResponseHeaders) null);
      Dictionary<string, object> dictionary = new Dictionary<string, object>(5);
      dictionary["content"] = (object) Convert.ToBase64String(Encoding.UTF8.GetBytes(fileContent));
      dictionary[nameof (message)] = (object) message;
      dictionary[nameof (branch)] = (object) branch;
      if (committer != null)
        dictionary[nameof (committer)] = (object) new
        {
          name = committer.Name,
          email = committer.Email
        };
      if (author != null)
        dictionary[nameof (author)] = (object) new
        {
          name = author.Name,
          email = author.Email
        };
      string jsonIn = JsonConvert.SerializeObject((object) dictionary);
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).ContentsUri(repository, filePath);
      return this.CreateItem<GitHubData.V3.ContentResponseData>(authentication, uriBuilder.AbsoluteUri(), jsonIn, httpMethod: HttpMethod.Put, entryMethodName: nameof (CreateFileContent));
    }

    public GitHubResult<GitHubData.V3.ContentResponseData> UpdateFileContent(
      GitHubAuthentication authentication,
      string repository,
      string filePath,
      string sha,
      string branch,
      string fileContent,
      string message,
      GitHubData.V3.User committer = null,
      GitHubData.V3.User author = null)
    {
      return this.UpdateFileContent((string) null, authentication, repository, filePath, sha, branch, fileContent, message, committer, author);
    }

    public GitHubResult<GitHubData.V3.ContentResponseData> UpdateFileContent(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      string filePath,
      string sha,
      string branch,
      string fileContent,
      string message,
      GitHubData.V3.User committer = null,
      GitHubData.V3.User author = null)
    {
      this.ValidateContentAttributes(repository, fileContent, message, committer, author);
      if ((long) fileContent.Length > GitHubHttpClient.s_maxFileSizeSupported)
        return GitHubResult<GitHubData.V3.ContentResponseData>.Error(GitHubHttpClient.s_fileTooBigErrorCode, HttpStatusCode.BadRequest, (HttpResponseHeaders) null);
      Dictionary<string, object> dictionary = new Dictionary<string, object>(5);
      dictionary["content"] = (object) Convert.ToBase64String(Encoding.UTF8.GetBytes(fileContent));
      dictionary[nameof (message)] = (object) message;
      dictionary[nameof (sha)] = (object) sha;
      dictionary[nameof (branch)] = (object) branch;
      if (committer != null)
        dictionary[nameof (committer)] = (object) new
        {
          name = committer.Name,
          email = committer.Email
        };
      if (author != null)
        dictionary[nameof (author)] = (object) new
        {
          name = author.Name,
          email = author.Email
        };
      string jsonIn = JsonConvert.SerializeObject((object) dictionary);
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).ContentsUri(repository, filePath);
      return this.UpdateItem<GitHubData.V3.ContentResponseData>(authentication, uriBuilder.AbsoluteUri(), jsonIn, httpMethod: HttpMethod.Put, entryMethodName: nameof (UpdateFileContent));
    }

    public GitHubResult<GitHubData.V3.TagData> GetTag(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      string sha)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository).AppendPathSegments("git", "tags", sha);
      return this.QueryItem<GitHubData.V3.TagData>(authentication, uriBuilder.AbsoluteUri(), nameof (GetTag));
    }

    public GitHubResult<GitHubData.V3.Trees> GetTree(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      string sha,
      bool isRecursive)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository).AppendPathSegments("git", "trees", sha);
      if (isRecursive)
        uriBuilder.AppendQuery("recursive", "1");
      return this.QueryItem<GitHubData.V3.Trees>(authentication, uriBuilder.AbsoluteUri(), nameof (GetTree));
    }

    public GitHubResult<GitHubData.V4.Repository> GetRepositoryTree(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      int depth,
      string gitRef)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(gitRef, nameof (gitRef));
      string format = "\r\n                ... on Tree {{\r\n                    entries {{\r\n                        name\r\n                        type\r\n                        object {{\r\n                            __typename\r\n                            id\r\n                            {0}\r\n                        }}\r\n                    }}\r\n                }}";
      string str = string.Empty;
      for (int index = 0; index < depth; ++index)
        str = string.Format(format, (object) str);
      GraphQLQuery query = new GraphQLQuery("query($owner:String!, $name:String!, $gitRef:String!) {\r\n                    " + GitHubHttpClient.s_rateLimitField + "\r\n                    item: repository(owner: $owner, name: $name) {\r\n                        " + GitHubHttpClient.GraphQLTypeInspector.GetAllFields<GitHubData.V4.Repository>(1) + "\r\n                        ref(qualifiedName: $gitRef) {\r\n                            " + GitHubHttpClient.GraphQLTypeInspector.GetScalarFields<GitHubData.V4.Ref>() + "\r\n                            target {\r\n                                __typename\r\n                                ... on Commit {\r\n                                    " + GitHubHttpClient.GraphQLTypeInspector.GetScalarFields<GitHubData.V4.Commit>() + "\r\n                                    tree {\r\n                                        id\r\n                                        " + str + "\r\n                                    }\r\n                                }\r\n                            }\r\n                        }\r\n                    }\r\n                }");
      string owner;
      string name;
      GitHubHttpClient.ExtractRepositoryOwnerAndName(repository, out owner, out name);
      Dictionary<string, object> variables = new Dictionary<string, object>()
      {
        {
          "owner",
          (object) owner
        },
        {
          "name",
          (object) name
        },
        {
          nameof (gitRef),
          (object) gitRef
        }
      };
      return this.QueryGraphQLItem<GitHubData.V4.Repository>(authentication, enterpriseUrl, query, (IDictionary<string, object>) variables, nameof (GetRepositoryTree));
    }

    public GitHubResult<GitHubData.V3.Blob[]> GetCommitTree(
      GitHubAuthentication authentication,
      string treeUrl,
      bool recursive)
    {
      UriBuilder uriBuilder = new UriBuilder(treeUrl);
      if (recursive)
        uriBuilder.AppendQuery(nameof (recursive), "1");
      return this.QueryItem<GitHubData.V3.Trees>(authentication, uriBuilder.AbsoluteUri(), nameof (GetCommitTree)).Convert<GitHubData.V3.Blob[]>((Func<GitHubData.V3.Trees, GitHubData.V3.Blob[]>) (x => x.Tree));
    }

    public GitHubResult<GitHubData.V3.Commit> GetBranchTree(
      GitHubAuthentication authentication,
      string commitUrl)
    {
      return this.QueryItem<GitHubData.V3.Commit>(authentication, commitUrl, nameof (GetBranchTree));
    }

    public GitHubResult<GitHubData.V3.Ref> GetRepoRef(
      GitHubAuthentication authentication,
      string repoRefsUrl,
      string branch)
    {
      UriBuilder uriBuilder = new UriBuilder(repoRefsUrl).AppendPathSegments("heads", branch);
      return this.QueryItem<GitHubData.V3.Ref>(authentication, uriBuilder.AbsoluteUri(), nameof (GetRepoRef));
    }

    public GitHubResult<GitHubData.V3.OrgMembership> GetOrgMembership(
      GitHubAuthentication authentication,
      string org,
      string username)
    {
      return this.GetOrgMembership((string) null, authentication, org, username);
    }

    public GitHubResult<GitHubData.V3.OrgMembership> GetOrgMembership(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string org,
      string username)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).AppendPath("orgs", org, "memberships", username);
      return this.QueryItem<GitHubData.V3.OrgMembership>(authentication, uriBuilder.AbsoluteUri(), nameof (GetOrgMembership));
    }

    public GitHubResult<GitHubData.V3.OrgMembership[]> GetUserOrgMembership(
      GitHubAuthentication authentication)
    {
      UriBuilder uriBuilder = new GitHubApiRoot().AppendPath("user", "memberships", "orgs");
      return this.QueryItem<GitHubData.V3.OrgMembership[]>(authentication, uriBuilder.AbsoluteUri(), nameof (GetUserOrgMembership));
    }

    public GitHubResult<GitHubData.V3.Owner[]> GetOrgMembers(
      GitHubAuthentication authentication,
      string orgId,
      string role = null)
    {
      UriBuilder uriBuilder = new GitHubApiRoot().AppendPath("organizations", orgId, "members");
      if (role != null)
        uriBuilder.Query = "role=" + role;
      return this.QueryItems<GitHubData.V3.Owner>(authentication, uriBuilder.AbsoluteUri(), entryMethodName: nameof (GetOrgMembers));
    }

    public GitHubResult<GitHubData.V3.Org[]> GetUserOrgs(GitHubAuthentication authentication) => this.GetUserOrgs((string) null, authentication);

    public GitHubResult<GitHubData.V3.Org[]> GetUserOrgs(
      string enterpriseUrl,
      GitHubAuthentication authentication)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).AppendPath("user", "orgs");
      return this.QueryItems<GitHubData.V3.Org>(authentication, uriBuilder.AbsoluteUri(), entryMethodName: nameof (GetUserOrgs));
    }

    public GitHubResult<GitHubData.V3.Org> GeOrgByName(
      GitHubAuthentication authentication,
      string orgName)
    {
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new GitHubApiRoot().AppendPath("users", orgName).AbsoluteUri()))
        return this.SendRequestForSingleItem<GitHubData.V3.Org>(request, authentication, nameof (GeOrgByName));
    }

    public GitHubResult<GitHubData.V3.Repository[]> GetOrgRepos(
      GitHubAuthentication authentication,
      string org)
    {
      return this.GetOrgRepos((string) null, authentication, org);
    }

    public GitHubResult<GitHubData.V3.Repository[]> GetOrgRepos(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string org)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).AppendPath("orgs", org, "repos");
      return this.QueryItems<GitHubData.V3.Repository>(authentication, uriBuilder.AbsoluteUri(), entryMethodName: nameof (GetOrgRepos));
    }

    public GitHubResult<GitHubData.V3.Repository[]> GetUserRepos(GitHubAuthentication authentication) => this.GetUserRepos((string) null, authentication);

    public GitHubResult<GitHubData.V3.Repository[]> GetUserRepos(
      string enterpriseUrl,
      GitHubAuthentication authentication)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).AppendPath("user", "repos");
      return this.QueryItems<GitHubData.V3.Repository>(authentication, uriBuilder.AbsoluteUri(), entryMethodName: nameof (GetUserRepos));
    }

    public GitHubResult<GitHubData.V3.Repository[]> GetFirstPageOfUserRepos(
      GitHubAuthentication authentication,
      out string nextToken)
    {
      return this.GetPagedUserRepos((string) null, authentication, (string) null, out nextToken, out int _, out int _);
    }

    public GitHubResult<GitHubData.V3.Repository[]> GetPagedUserRepos(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string continuationToken,
      out string nextToken,
      out int pageLength,
      out int totalPageCount,
      bool ownerOnly = false)
    {
      string str = continuationToken;
      if (str == null)
        str = new GitHubApiRoot(enterpriseUrl).AppendPath("user", "repos").AppendQuery("per_page", 100.ToString()).AppendQuery("type", ownerOnly ? "owner" : "all").AbsoluteUri();
      string requestUri = str;
      pageLength = 100;
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUri))
        return this.SendRequestForPaginatedItems<GitHubData.V3.Repository>(request, authentication, false, out nextToken, out totalPageCount, nameof (GetPagedUserRepos));
    }

    public GitHubResult<GitHubData.V3.Repository[]> GetPagedOrgRepos(
      string enterpriseUrl,
      string orgName,
      GitHubAuthentication authentication,
      string continuationToken,
      out string nextToken,
      out int pageLength,
      out int totalPageCount)
    {
      string str = continuationToken;
      if (str == null)
        str = new GitHubApiRoot(enterpriseUrl).AppendPath("orgs", orgName, "repos").AppendQuery("per_page", 100.ToString()).AbsoluteUri();
      string requestUri = str;
      pageLength = 100;
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUri))
        return this.SendRequestForPaginatedItems<GitHubData.V3.Repository>(request, authentication, false, out nextToken, out totalPageCount, nameof (GetPagedOrgRepos));
    }

    public GitHubResult<GitHubData.V3.User> GetUser(
      string enterpriseUrl,
      GitHubAuthentication authentication)
    {
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new GitHubApiRoot(enterpriseUrl).AppendPath("user").AbsoluteUri()))
      {
        GitHubResult<GitHubData.V3.User> gitHubResult = this.SendRequestForSingleItem<GitHubData.V3.User>(request, authentication, nameof (GetUser));
        HttpStatusCode statusCode = gitHubResult.StatusCode;
        return !gitHubResult.IsSuccessful ? gitHubResult : GitHubResult<GitHubData.V3.User>.Success(gitHubResult.Result, statusCode);
      }
    }

    public virtual GitHubResult<GitHubData.V3.User> GetUserByLogin(
      string userLogin,
      GitHubAuthentication authentication)
    {
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new GitHubApiRoot().AppendPath("users", userLogin).AbsoluteUri()))
      {
        GitHubResult<GitHubData.V3.User> gitHubResult = this.SendRequestForSingleItem<GitHubData.V3.User>(request, authentication, nameof (GetUserByLogin));
        HttpStatusCode statusCode = gitHubResult.StatusCode;
        return !gitHubResult.IsSuccessful ? GitHubResult<GitHubData.V3.User>.Error(gitHubResult.Errors, statusCode, (HttpResponseHeaders) null) : GitHubResult<GitHubData.V3.User>.Success(gitHubResult.Result, statusCode);
      }
    }

    public virtual GitHubResult<GitHubData.V3.User> GetUserById(
      string userId,
      GitHubAuthentication authentication)
    {
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new GitHubApiRoot().AppendPath("user", userId).AbsoluteUri()))
      {
        GitHubResult<GitHubData.V3.User> gitHubResult = this.SendRequestForSingleItem<GitHubData.V3.User>(request, authentication, nameof (GetUserById));
        HttpStatusCode statusCode = gitHubResult.StatusCode;
        return !gitHubResult.IsSuccessful ? GitHubResult<GitHubData.V3.User>.Error(gitHubResult.Errors, statusCode, (HttpResponseHeaders) null) : GitHubResult<GitHubData.V3.User>.Success(gitHubResult.Result, statusCode);
      }
    }

    public virtual GitHubResult<GitHubData.V3.User[]> SearchUsers(
      string searchQuery,
      int perPage,
      GitHubAuthentication authentication)
    {
      perPage = perPage <= 0 || perPage > 100 ? 100 : perPage;
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new GitHubApiRoot().AppendPath("search", "users").AppendQueryEscapeUriString("q", searchQuery + "+type:user").AppendQuery("per_page", perPage.ToString()).AbsoluteUri()))
      {
        GitHubResult<GitHubData.V3.SearchUsersResult> gitHubResult = this.SendRequestForSingleItem<GitHubData.V3.SearchUsersResult>(request, authentication, nameof (SearchUsers));
        HttpStatusCode statusCode = gitHubResult.StatusCode;
        return !gitHubResult.IsSuccessful ? GitHubResult<GitHubData.V3.User[]>.Error(gitHubResult.Errors, statusCode, (HttpResponseHeaders) null) : GitHubResult<GitHubData.V3.User[]>.Success(gitHubResult.Result.Items, statusCode);
      }
    }

    public GitHubResult<GitHubData.V3.Repository[]> GetUserReposByCriteria(
      GitHubAuthentication authentication,
      string searchCriteria)
    {
      string empty = string.Empty;
      GitHubResult<GitHubData.V3.User> user = this.GetUser((string) null, authentication);
      if (user != null && user.Result != null)
      {
        string login = user.Result.Login;
      }
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new GitHubApiRoot().AppendPath("search", "repositories").AppendQueryEscapeUriString("q", searchCriteria + "+in:name").AbsoluteUri()))
      {
        GitHubResult<GitHubData.V3.RepositorySearch> gitHubResult = this.SendRequestForSingleItem<GitHubData.V3.RepositorySearch>(request, authentication, nameof (GetUserReposByCriteria));
        HttpStatusCode statusCode = gitHubResult.StatusCode;
        return !gitHubResult.IsSuccessful ? GitHubResult<GitHubData.V3.Repository[]>.Error(gitHubResult.Errors, statusCode, (HttpResponseHeaders) null) : GitHubResult<GitHubData.V3.Repository[]>.Success(gitHubResult.Result.Items, statusCode);
      }
    }

    public GitHubResult<GitHubData.V3.Repository[]> GetTopUserRepos(
      string enterpriseUrl,
      GitHubAuthentication authentication)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).AppendPath("user", "repos").AppendQuery("affiliation", "owner,collaborator");
      return this.QueryItems<GitHubData.V3.Repository>(authentication, uriBuilder.AbsoluteUri(), entryMethodName: nameof (GetTopUserRepos));
    }

    public GitHubResult<GitHubData.V3.Repository> GetUserRepo(
      GitHubAuthentication authentication,
      string repository)
    {
      return this.GetUserRepo((string) null, authentication, repository);
    }

    public GitHubResult<GitHubData.V3.Repository> GetUserRepo(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository);
      return this.QueryItem<GitHubData.V3.Repository>(authentication, uriBuilder.AbsoluteUri(), nameof (GetUserRepo));
    }

    public GitHubResult<GitHubData.V3.RepositoryLanguage[]> GetRepositoryLanguages(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository).AppendPathSegments("languages");
      GitHubResult<string> gitHubResult = this.QueryStringItem(authentication, uriBuilder.AbsoluteUri(), nameof (GetRepositoryLanguages));
      if (!gitHubResult.IsSuccessful)
        return gitHubResult.Convert<GitHubData.V3.RepositoryLanguage[]>();
      long num = -1;
      GitHubData.V3.RepositoryLanguage repositoryLanguage1 = (GitHubData.V3.RepositoryLanguage) null;
      List<GitHubData.V3.RepositoryLanguage> repositoryLanguageList = new List<GitHubData.V3.RepositoryLanguage>();
      foreach (JProperty property in JObject.Parse(gitHubResult.Result).Properties())
      {
        GitHubData.V3.RepositoryLanguage repositoryLanguage2 = new GitHubData.V3.RepositoryLanguage()
        {
          Name = property.Name,
          ByteCount = (long) property.Value,
          IsPrimaryLanguage = false
        };
        repositoryLanguageList.Add(repositoryLanguage2);
        if (repositoryLanguage2.ByteCount > num)
        {
          repositoryLanguage1 = repositoryLanguage2;
          num = repositoryLanguage2.ByteCount;
        }
      }
      if (repositoryLanguage1 != null)
        repositoryLanguage1.IsPrimaryLanguage = true;
      return GitHubResult<GitHubData.V3.RepositoryLanguage[]>.Success(repositoryLanguageList.ToArray(), gitHubResult.StatusCode, gitHubResult.ResponseHeaders);
    }

    public GitHubResult<GitHubData.V3.Issue> GetIssue(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      int issueNumber)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository).AppendPathSegments("issues", issueNumber.ToString());
      return this.QueryItem<GitHubData.V3.Issue>(authentication, uriBuilder.AbsoluteUri(), nameof (GetIssue));
    }

    public GitHubResult<GitHubData.V3.Issue> UpdateIssue(
      GitHubAuthentication authentication,
      string repository,
      int number,
      string newState = null,
      string newTitle = null,
      string newDescription = null)
    {
      return this.UpdateIssue((string) null, authentication, repository, number, newState, newTitle, newDescription);
    }

    public GitHubResult<GitHubData.V3.Issue> UpdateIssue(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      int number,
      string newState = null,
      string newTitle = null,
      string newDescription = null)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(repository, nameof (repository));
      ArgumentUtility.CheckForNonnegativeInt(number, nameof (number));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (((IEnumerable<string>) new string[3]
      {
        newState,
        newTitle,
        newDescription
      }).All<string>(GitHubHttpClient.\u003C\u003EO.\u003C1\u003E__IsNullOrWhiteSpace ?? (GitHubHttpClient.\u003C\u003EO.\u003C1\u003E__IsNullOrWhiteSpace = new Func<string, bool>(string.IsNullOrWhiteSpace))))
        throw new ArgumentException("At least one of newState, newTitle, or newDescription has to be specified.");
      Dictionary<string, string> dictionary = new Dictionary<string, string>(3);
      if (!string.IsNullOrWhiteSpace(newState))
        dictionary["state"] = newState;
      if (!string.IsNullOrWhiteSpace(newTitle))
        dictionary["title"] = newTitle;
      if (newDescription != null)
        dictionary["body"] = newDescription;
      string jsonIn = JsonConvert.SerializeObject((object) dictionary);
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository).AppendPathSegments("issues", number.ToString());
      return this.UpdateItem<GitHubData.V3.Issue>(authentication, uriBuilder.AbsoluteUri(), jsonIn, entryMethodName: nameof (UpdateIssue));
    }

    public GitHubResult<GitHubData.V3.Comment[]> GetIssueComments(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      string since = null,
      string sort = null,
      string direction = null)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(repository, nameof (repository));
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository).AppendPathSegments("issues", "comments");
      if (since != null)
        uriBuilder.AppendQuery(nameof (since), since);
      if (sort != null)
        uriBuilder.AppendQuery(nameof (sort), sort);
      if (direction != null)
        uriBuilder.AppendQuery(nameof (direction), direction);
      return this.QueryItems<GitHubData.V3.Comment>(authentication, uriBuilder.AbsoluteUri(), entryMethodName: nameof (GetIssueComments));
    }

    public GitHubResult<GitHubData.V3.Comment> UpdateIssueComment(
      GitHubAuthentication authentication,
      string repository,
      string id,
      string newBody)
    {
      return this.UpdateIssueComment((string) null, authentication, repository, id, newBody);
    }

    public GitHubResult<GitHubData.V3.Comment> UpdateIssueComment(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      string id,
      string newBody)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(repository, nameof (repository));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(id, nameof (id));
      ArgumentUtility.CheckForNull<string>(newBody, nameof (newBody));
      string jsonIn = JsonConvert.SerializeObject((object) new Dictionary<string, string>(1)
      {
        ["body"] = newBody
      });
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository).AppendPathSegments("issues", "comments", id);
      return this.UpdateItem<GitHubData.V3.Comment>(authentication, uriBuilder.AbsoluteUri(), jsonIn, entryMethodName: nameof (UpdateIssueComment));
    }

    public GitHubResult<GitHubData.V3.Comment> UpdateCommitComment(
      GitHubAuthentication authentication,
      string repository,
      string id,
      string newBody)
    {
      return this.UpdateCommitComment((string) null, authentication, repository, id, newBody);
    }

    public GitHubResult<GitHubData.V3.Comment> UpdateCommitComment(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      string id,
      string newBody)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(repository, nameof (repository));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(id, nameof (id));
      ArgumentUtility.CheckForNull<string>(newBody, nameof (newBody));
      string jsonIn = JsonConvert.SerializeObject((object) new Dictionary<string, string>(1)
      {
        ["body"] = newBody
      });
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository).AppendPathSegments("comments", id);
      return this.UpdateItem<GitHubData.V3.Comment>(authentication, uriBuilder.AbsoluteUri(), jsonIn, entryMethodName: nameof (UpdateCommitComment));
    }

    public GitHubResult<GitHubData.V3.Repository[]> GetInstallationRepos(
      GitHubAuthentication authentication)
    {
      UriBuilder uriBuilder = new GitHubApiRoot().AppendPath("installation", "repositories");
      return this.QueryPagedItems<GitHubData.InstallationRepositories>(authentication, uriBuilder.AbsoluteUri(), nameof (GetInstallationRepos)).Convert<GitHubData.V3.Repository[]>((Func<GitHubData.InstallationRepositories[], GitHubData.V3.Repository[]>) (x => ((IEnumerable<GitHubData.InstallationRepositories>) x).SelectMany<GitHubData.InstallationRepositories, GitHubData.V3.Repository>((Func<GitHubData.InstallationRepositories, IEnumerable<GitHubData.V3.Repository>>) (y => (IEnumerable<GitHubData.V3.Repository>) y.repositories)).ToArray<GitHubData.V3.Repository>()));
    }

    public GitHubResult<GitHubData.V3.Repository[]> GetPagedInstallationRepos(
      GitHubAuthentication authentication,
      string continuationToken,
      out string nextToken,
      out int pageLength,
      out int totalPageCount)
    {
      string requestUri = continuationToken;
      if (requestUri == null)
        requestUri = new GitHubApiRoot().AppendPath("installation", "repositories").AppendQuery("per_page", 100.ToString()).AbsoluteUri();
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUri))
      {
        GitHubResult<GitHubData.InstallationRepositories> gitHubResult = this.SendRequestForSingleItem<GitHubData.InstallationRepositories>(request, authentication, out nextToken, out totalPageCount, nameof (GetPagedInstallationRepos));
        pageLength = gitHubResult.Result?.repositories != null ? gitHubResult.Result.repositories.Length : 100;
        return gitHubResult.Convert<GitHubData.V3.Repository[]>((Func<GitHubData.InstallationRepositories, GitHubData.V3.Repository[]>) (x => x.repositories));
      }
    }

    public GitHubResult<GitHubData.V3.Repository[]> GetPagedUserInstallationRepos(
      GitHubAuthentication authentication,
      string installationId,
      string continuationToken,
      out string nextToken,
      out int pageLength,
      out int totalPageCount)
    {
      string requestUri = continuationToken;
      if (requestUri == null)
        requestUri = new GitHubApiRoot().AppendPath("user", "installations", installationId, "repositories").AppendQuery("per_page", 100.ToString()).AbsoluteUri();
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUri))
      {
        request.Headers.Add("Accept", "application/vnd.github.machine-man-preview+json");
        GitHubResult<GitHubData.InstallationRepositories> gitHubResult = this.SendRequestForSingleItem<GitHubData.InstallationRepositories>(request, authentication, out nextToken, out totalPageCount, nameof (GetPagedUserInstallationRepos));
        pageLength = gitHubResult.Result?.repositories != null ? gitHubResult.Result.repositories.Length : 100;
        return gitHubResult.Convert<GitHubData.V3.Repository[]>((Func<GitHubData.InstallationRepositories, GitHubData.V3.Repository[]>) (x => x.repositories));
      }
    }

    public GitHubResult<GitHubData.V3.Repository[]> GetTopInstallationRepos(
      GitHubAuthentication authentication)
    {
      UriBuilder uriBuilder = new GitHubApiRoot().AppendPath("installation", "repositories");
      return this.QueryItem<GitHubData.InstallationRepositories>(authentication, uriBuilder.AbsoluteUri(), nameof (GetTopInstallationRepos)).Convert<GitHubData.V3.Repository[]>((Func<GitHubData.InstallationRepositories, GitHubData.V3.Repository[]>) (x => x.repositories));
    }

    public GitHubResult<GitHubData.V3.Hook[]> GetHooks(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository);
      return this.GetHooks(authentication, uriBuilder.AbsoluteUri());
    }

    public GitHubResult<GitHubData.V3.Hook[]> GetHooks(
      GitHubAuthentication authentication,
      string repoUrl)
    {
      UriBuilder uriBuilder = new UriBuilder(repoUrl).AppendPathSegments("hooks");
      return this.QueryItems<GitHubData.V3.Hook>(authentication, uriBuilder.AbsoluteUri(), entryMethodName: nameof (GetHooks));
    }

    public GitHubResult<GitHubData.V3.Hook> CreateHook(
      GitHubAuthentication authentication,
      string repoUrl,
      string locationUrl,
      string secret)
    {
      GitHubAuthentication authentication1 = authentication;
      List<string> events = new List<string>();
      events.Add("push");
      string repoUrl1 = repoUrl;
      string locationUrl1 = locationUrl;
      string secret1 = secret;
      return this.CreateHook(authentication1, (IReadOnlyCollection<string>) events, repoUrl1, locationUrl1, secret1);
    }

    public GitHubResult<GitHubData.V3.Hook> CreateHook(
      GitHubAuthentication authentication,
      IReadOnlyCollection<string> events,
      string repoUrl,
      string locationUrl,
      string secret)
    {
      UriBuilder uriBuilder = new UriBuilder(repoUrl).AppendPathSegments("hooks");
      JObject jobject = new JObject()
      {
        {
          "active",
          (JToken) true
        },
        {
          nameof (events),
          (JToken) new JArray((object) events)
        },
        {
          "config",
          (JToken) new JObject()
          {
            {
              "url",
              (JToken) locationUrl
            },
            {
              "content_type",
              (JToken) "json"
            },
            {
              nameof (secret),
              (JToken) secret
            }
          }
        }
      };
      if (!GitHubRoot.IsGitHubDomain(uriBuilder.Uri))
        jobject["name"] = (JToken) "web";
      string jsonIn = JsonConvert.SerializeObject((object) jobject);
      return this.CreateItem<GitHubData.V3.Hook>(authentication, uriBuilder.AbsoluteUri(), jsonIn, entryMethodName: nameof (CreateHook));
    }

    public GitHubResult<GitHubData.V3.Hook> UpdateHook(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repoNameWithOwner,
      string hookId,
      IReadOnlyCollection<string> events)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repoNameWithOwner);
      return this.UpdateHook(authentication, uriBuilder.AbsoluteUri(), hookId, events);
    }

    public GitHubResult<GitHubData.V3.Hook> UpdateHook(
      GitHubAuthentication authentication,
      string repoUrl,
      string hookId,
      IReadOnlyCollection<string> events)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>()
      {
        {
          nameof (events),
          (object) events
        }
      };
      UriBuilder uriBuilder = new UriBuilder(repoUrl).AppendPathSegments("hooks", hookId);
      string jsonIn = JsonConvert.SerializeObject((object) dictionary);
      return this.UpdateItem<GitHubData.V3.Hook>(authentication, uriBuilder.AbsoluteUri(), jsonIn, entryMethodName: nameof (UpdateHook));
    }

    public GitHubResult DeleteHook(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      string hookId)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository);
      return this.DeleteHook(authentication, uriBuilder.AbsoluteUri(), hookId);
    }

    public GitHubResult DeleteHook(
      GitHubAuthentication authentication,
      string repoUrl,
      string hookId)
    {
      UriBuilder uriBuilder = new UriBuilder(repoUrl).AppendPathSegments("hooks", hookId);
      return this.DeleteItem(authentication, uriBuilder.AbsoluteUri(), nameof (DeleteHook));
    }

    public GitHubResult<GitHubData.V3.TagData> CreateCommitTag(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      string name,
      string message,
      string commitSha)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository).AppendPathSegments("git", "tags");
      string jsonIn = JsonConvert.SerializeObject((object) new
      {
        tag = name,
        message = message,
        @object = commitSha,
        type = "commit"
      });
      return this.CreateItem<GitHubData.V3.TagData>(authentication, uriBuilder.AbsoluteUri(), jsonIn, entryMethodName: nameof (CreateCommitTag));
    }

    public GitHubResult<GitHubData.V3.Ref> CreateRef(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      string name,
      string sha)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository).AppendPathSegments("git", "refs");
      string jsonIn = JsonConvert.SerializeObject((object) new
      {
        @ref = name,
        sha = sha
      });
      return this.CreateItem<GitHubData.V3.Ref>(authentication, uriBuilder.AbsoluteUri(), jsonIn, entryMethodName: nameof (CreateRef));
    }

    public GitHubResult<GitHubData.V3.Ref> GetBranchRef(
      GitHubAuthentication authentication,
      string repository,
      string branch)
    {
      return this.GetBranchRef((string) null, authentication, repository, branch);
    }

    public GitHubResult<GitHubData.V3.Ref> GetBranchRef(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      string branch)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(repository, nameof (repository));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(branch, nameof (branch));
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository).AppendPathSegments("git", "refs", "heads", branch);
      return this.QueryItem<GitHubData.V3.Ref>(authentication, uriBuilder.AbsoluteUri(), nameof (GetBranchRef));
    }

    public GitHubResult<GitHubData.V3.Ref> CreateBranchRef(
      GitHubAuthentication authentication,
      string repository,
      string branch,
      string sha)
    {
      return this.CreateBranchRef((string) null, authentication, repository, branch, sha);
    }

    public GitHubResult<GitHubData.V3.Ref> CreateBranchRef(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      string branch,
      string sha)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(repository, nameof (repository));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(branch, nameof (branch));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(sha, nameof (sha));
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository).AppendPathSegments("git", "refs");
      string jsonIn = JsonConvert.SerializeObject((object) new
      {
        @ref = ("refs/heads/" + branch),
        sha = sha
      });
      return this.CreateItem<GitHubData.V3.Ref>(authentication, uriBuilder.AbsoluteUri(), jsonIn, entryMethodName: nameof (CreateBranchRef));
    }

    public GitHubResult<GitHubData.V3.CheckRun> GetRunStatus(
      GitHubAuthentication authentication,
      string repoName,
      long id)
    {
      UriBuilder uriBuilder = new GitHubApiRoot().RepositoryUri(repoName).AppendPathSegments("check-runs", id.ToString());
      return this.QueryItem<GitHubData.V3.CheckRun>(authentication, uriBuilder.AbsoluteUri(), GitHubHttpClient.s_acceptHeaderForGitHubChecks, (string) null);
    }

    public async Task<GitHubResult<GitHubData.V3.CheckRun>> CreateRunStatusAsync(
      GitHubAuthentication authentication,
      string repoName,
      string sha,
      string status,
      string buildUrl,
      string externalIdentifier,
      string checkName)
    {
      UriBuilder uriBuilder = new GitHubApiRoot().RepositoryUri(repoName).AppendPathSegments("check-runs");
      string jsonIn = JsonConvert.SerializeObject((object) new
      {
        name = checkName,
        head_sha = sha,
        details_url = buildUrl,
        external_id = externalIdentifier,
        status = status
      });
      return await this.CreateItemAsync<GitHubData.V3.CheckRun>(authentication, uriBuilder.AbsoluteUri(), jsonIn, GitHubHttpClient.s_acceptHeaderForGitHubChecks, entryMethodName: nameof (CreateRunStatusAsync));
    }

    public async Task<GitHubResult<GitHubData.V3.CheckRun>> CreateCompletedRunStatusAsync(
      GitHubAuthentication authentication,
      string repoName,
      string sha,
      string status,
      string conclusion,
      string checkName,
      string detailsUrl,
      DateTime startTime,
      DateTime completionTime,
      string output)
    {
      UriBuilder uriBuilder = new GitHubApiRoot().RepositoryUri(repoName).AppendPathSegments("check-runs");
      string jsonIn = JsonConvert.SerializeObject((object) new
      {
        name = checkName,
        head_sha = sha,
        conclusion = conclusion,
        details_url = detailsUrl,
        status = status,
        started_at = GitHubHttpClient.ConvertUTCDateTimeToString(startTime),
        completed_at = GitHubHttpClient.ConvertUTCDateTimeToString(completionTime),
        output = new JRaw((object) output)
      });
      return await this.CreateItemAsync<GitHubData.V3.CheckRun>(authentication, uriBuilder.AbsoluteUri(), jsonIn, GitHubHttpClient.s_acceptHeaderForGitHubChecks, entryMethodName: nameof (CreateCompletedRunStatusAsync));
    }

    public async Task<GitHubResult<GitHubData.V3.CheckRun>> UpdateRunStatusAsync(
      GitHubAuthentication authentication,
      long id,
      string repoName,
      string status,
      string buildUrl,
      string checkName,
      string conclusion,
      DateTime startTime,
      DateTime completionTime,
      string output)
    {
      UriBuilder uriBuilder = new GitHubApiRoot().RepositoryUri(repoName).AppendPathSegments("check-runs", id.ToString());
      JObject jobject = new JObject()
      {
        {
          "name",
          (JToken) checkName
        },
        {
          "details_url",
          (JToken) buildUrl
        },
        {
          nameof (status),
          (JToken) status
        },
        {
          nameof (conclusion),
          (JToken) conclusion
        },
        {
          "started_at",
          (JToken) GitHubHttpClient.ConvertUTCDateTimeToString(startTime)
        },
        {
          "completed_at",
          (JToken) GitHubHttpClient.ConvertUTCDateTimeToString(completionTime)
        }
      };
      if (!string.IsNullOrEmpty(output))
        jobject[nameof (output)] = (JToken) new JRaw((object) output);
      string jsonIn = JsonConvert.SerializeObject((object) jobject);
      return await this.UpdateItemAsync<GitHubData.V3.CheckRun>(authentication, uriBuilder.AbsoluteUri(), jsonIn, GitHubHttpClient.s_acceptHeaderForGitHubChecks, entryMethodName: nameof (UpdateRunStatusAsync));
    }

    public GitHubResult<GitHubData.V3.Status> CreateStatus(
      GitHubAuthentication authentication,
      string repoUrl,
      string sha,
      string state,
      string targetUrl,
      string description,
      string context)
    {
      UriBuilder uriBuilder = new UriBuilder(repoUrl).AppendPathSegments("statuses", sha);
      string jsonIn = JsonConvert.SerializeObject((object) new
      {
        state = state,
        target_url = targetUrl,
        description = description,
        context = context
      });
      return this.CreateItem<GitHubData.V3.Status>(authentication, uriBuilder.AbsoluteUri(), jsonIn, entryMethodName: nameof (CreateStatus));
    }

    public async Task<GitHubResult<GitHubData.V3.Status>> CreateStatusAsync(
      GitHubAuthentication authentication,
      string repoUrl,
      string sha,
      string state,
      string targetUrl,
      string description,
      string context)
    {
      UriBuilder uriBuilder = new UriBuilder(repoUrl).AppendPathSegments("statuses", sha);
      string jsonIn = JsonConvert.SerializeObject((object) new
      {
        state = state,
        target_url = targetUrl,
        description = description,
        context = context
      });
      return await this.CreateItemAsync<GitHubData.V3.Status>(authentication, uriBuilder.AbsoluteUri(), jsonIn, entryMethodName: nameof (CreateStatusAsync));
    }

    public GitHubResult<JObject> UpdateGitHubPipelineStatus(
      GitHubAuthentication authentication,
      string updateUrl,
      string statusPayload)
    {
      UriBuilder uriBuilder = new UriBuilder(updateUrl);
      return this.UpdateItem<JObject>(authentication, uriBuilder.AbsoluteUri(), statusPayload, httpMethod: new HttpMethod("PATCH"), entryMethodName: nameof (UpdateGitHubPipelineStatus));
    }

    public GitHubResult<GitHubData.Action.JustInTimeToken> RequestToken(
      GitHubAuthentication authentication,
      string tokenRequestUrl)
    {
      return this.CreateItem<GitHubData.Action.JustInTimeToken>(authentication, tokenRequestUrl, "{}", entryMethodName: nameof (RequestToken));
    }

    public GitHubResult<GitHubData.V3.DeploymentStatus> CreateDeploymentStatus(
      GitHubAuthentication authentication,
      string statusUrl,
      string state,
      string logUrl,
      string environmentUrl,
      string description,
      bool autoInactive)
    {
      string jsonIn = JsonConvert.SerializeObject((object) new
      {
        state = state,
        log_url = logUrl,
        environment_url = environmentUrl,
        description = description,
        auto_inactive = autoInactive
      });
      return this.CreateItem<GitHubData.V3.DeploymentStatus>(authentication, statusUrl, jsonIn, entryMethodName: nameof (CreateDeploymentStatus));
    }

    private static string ConvertUTCDateTimeToString(DateTime completionTime) => completionTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");

    public GitHubResult<GitHubData.OauthAuthorization> GetOauthAuthorization(
      string clientId,
      string clientSecret,
      string code)
    {
      UriBuilder uriBuilder = new GitHubRoot().AppendPath("login", "oauth", "access_token");
      string content = JsonConvert.SerializeObject((object) new
      {
        client_id = clientId,
        client_secret = clientSecret,
        code = code
      });
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri))
      {
        request.Content = (HttpContent) new StringContent(content, Encoding.UTF8, "application/json");
        request.Headers.Add("Accept", "application/json");
        return this.SendRequestForSingleItem<GitHubData.OauthAuthorization>(request, (GitHubAuthentication) null, nameof (GetOauthAuthorization));
      }
    }

    public GitHubResult<GitHubData.V3.UserPermission> GetCollaboratorPermissions(
      GitHubAuthentication authentication,
      string repository,
      string username)
    {
      return this.GetCollaboratorPermissions((string) null, authentication, repository, username);
    }

    public GitHubResult<GitHubData.V3.UserPermission> GetCollaboratorPermissions(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      string username)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository).AppendPathSegments("collaborators", username, "permission");
      return this.QueryItem<GitHubData.V3.UserPermission>(authentication, uriBuilder.AbsoluteUri(), nameof (GetCollaboratorPermissions));
    }

    public GitHubResult<GitHubData.V3.PullRequest> GetPullRequest(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      int pullRequestNumber)
    {
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).PullRequestUri(repository, pullRequestNumber.ToString());
      return this.QueryItem<GitHubData.V3.PullRequest>(authentication, uriBuilder.AbsoluteUri(), nameof (GetPullRequest));
    }

    public GitHubResult<JObject> GetPullRequest(
      GitHubAuthentication authentication,
      string pullRequestUrl)
    {
      return this.QueryItem<JObject>(authentication, pullRequestUrl, nameof (GetPullRequest));
    }

    public GitHubResult<GitHubData.V3.PullRequest> CreatePullRequest(
      GitHubAuthentication authentication,
      string repository,
      string targetBranch,
      string sourceBranch,
      string title,
      string description = null)
    {
      return this.CreatePullRequest((string) null, authentication, repository, targetBranch, sourceBranch, title, description);
    }

    public GitHubResult<GitHubData.V3.PullRequest> CreatePullRequest(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      string targetBranch,
      string sourceBranch,
      string title,
      string description = null)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(repository, nameof (repository));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(targetBranch, nameof (targetBranch));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(sourceBranch, nameof (sourceBranch));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(title, nameof (title));
      string jsonIn = JsonConvert.SerializeObject((object) new
      {
        title = title,
        @base = targetBranch,
        head = sourceBranch,
        body = (description ?? string.Empty)
      });
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository).AppendPathSegments("pulls");
      return this.CreateItem<GitHubData.V3.PullRequest>(authentication, uriBuilder.AbsoluteUri(), jsonIn, entryMethodName: nameof (CreatePullRequest));
    }

    public GitHubResult<GitHubData.V3.PullRequest> UpdatePullRequest(
      GitHubAuthentication authentication,
      string repository,
      int number,
      string newState = null,
      string newTargetBranch = null,
      string newTitle = null,
      string newDescription = null)
    {
      return this.UpdatePullRequest((string) null, authentication, repository, number, newState, newTargetBranch, newTitle, newDescription);
    }

    public GitHubResult<GitHubData.V3.PullRequest> UpdatePullRequest(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository,
      int number,
      string newState = null,
      string newTargetBranch = null,
      string newTitle = null,
      string newDescription = null)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(repository, nameof (repository));
      ArgumentUtility.CheckForNonnegativeInt(number, nameof (number));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (((IEnumerable<string>) new string[4]
      {
        newState,
        newTargetBranch,
        newTitle,
        newDescription
      }).All<string>(GitHubHttpClient.\u003C\u003EO.\u003C1\u003E__IsNullOrWhiteSpace ?? (GitHubHttpClient.\u003C\u003EO.\u003C1\u003E__IsNullOrWhiteSpace = new Func<string, bool>(string.IsNullOrWhiteSpace))))
        throw new ArgumentException("At least one of newState, newTargetBranch, newTitle, or newDescription has to be specified.");
      Dictionary<string, string> dictionary = new Dictionary<string, string>(4);
      if (!string.IsNullOrWhiteSpace(newState))
        dictionary["state"] = newState;
      if (!string.IsNullOrWhiteSpace(newTargetBranch))
        dictionary["base"] = newTargetBranch;
      if (!string.IsNullOrWhiteSpace(newTitle))
        dictionary["title"] = newTitle;
      if (!string.IsNullOrWhiteSpace(newDescription))
        dictionary["body"] = newDescription;
      string jsonIn = JsonConvert.SerializeObject((object) dictionary);
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository).AppendPathSegments("pulls", number.ToString());
      return this.UpdateItem<GitHubData.V3.PullRequest>(authentication, uriBuilder.AbsoluteUri(), jsonIn, entryMethodName: nameof (UpdatePullRequest));
    }

    public GitHubResult<GitHubData.InstallationAccessToken> CreateInstallationToken(
      string installationId,
      string appToken,
      IEnumerable<int> repositoryIds = null)
    {
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new GitHubApiRoot().AppendPath("app", "installations", installationId, "access_tokens").Uri))
      {
        if (repositoryIds != null && repositoryIds.Any<int>())
        {
          string content = JsonConvert.SerializeObject((object) new
          {
            repository_ids = repositoryIds
          });
          request.Content = (HttpContent) new StringContent(content, Encoding.UTF8, "application/json");
        }
        GitHubAuthentication authentication = new GitHubAuthentication(GitHubAuthScheme.ApplicationToken, appToken);
        return this.SendRequestForSingleItem<GitHubData.InstallationAccessToken>(request, authentication, nameof (CreateInstallationToken));
      }
    }

    public GitHubResult<GitHubData.InstallationAccessToken> CreateInstallationToken(
      GitHubAuthentication authentication,
      IEnumerable<int> repositoryIds = null)
    {
      return this.CreateInstallationToken(authentication, repositoryIds, false);
    }

    public GitHubResult<GitHubData.V3.InstallationDetails> GetOrgInstallation(string org)
    {
      GitHubAuthentication authentication;
      if (!this.TryGetApplicationTokenAuth(out authentication))
        return GitHubResult<GitHubData.V3.InstallationDetails>.Error("'Org installation' was queried with an unsupported authentication type.", HttpStatusCode.BadRequest, (HttpResponseHeaders) null);
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new GitHubApiRoot().AppendPath("orgs", org, "installation").Uri))
        return this.SendRequestForSingleItem<GitHubData.V3.InstallationDetails>(request, authentication, nameof (GetOrgInstallation));
    }

    public GitHubResult<GitHubData.V3.InstallationDetails> GetRepositoryInstallation(
      string repository)
    {
      GitHubAuthentication authentication;
      if (!this.TryGetApplicationTokenAuth(out authentication))
        return GitHubResult<GitHubData.V3.InstallationDetails>.Error("'Repository installation' was queried with an unsupported authentication type.", HttpStatusCode.BadRequest, (HttpResponseHeaders) null);
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new GitHubApiRoot().RepositoryUri(repository).AppendPathSegments("installation").Uri))
        return this.SendRequestForSingleItem<GitHubData.V3.InstallationDetails>(request, authentication, nameof (GetRepositoryInstallation));
    }

    public GitHubResult<GitHubData.V3.InstallationDetails> GetUserInstallation(string userHandle)
    {
      GitHubAuthentication authentication;
      if (!this.TryGetApplicationTokenAuth(out authentication))
        return GitHubResult<GitHubData.V3.InstallationDetails>.Error("'User installation' was queried with an unsupported authentication type.", HttpStatusCode.BadRequest, (HttpResponseHeaders) null);
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new GitHubApiRoot().AppendPath("users", userHandle, "installation").Uri))
        return this.SendRequestForSingleItem<GitHubData.V3.InstallationDetails>(request, authentication, nameof (GetUserInstallation));
    }

    public GitHubResult<GitHubData.V3.Comment> PostComment(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repoNameWithOwner,
      string issueNumber,
      string comment)
    {
      string owner;
      string name;
      GitHubHttpClient.ExtractRepositoryOwnerAndName(repoNameWithOwner, out owner, out name);
      return this.PostComment(enterpriseUrl, authentication, owner, name, issueNumber, comment);
    }

    public GitHubResult<GitHubData.V3.Comment> PostComment(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string owner,
      string repo,
      string issueNumber,
      string comment)
    {
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new GitHubApiRoot(enterpriseUrl).AppendPath("repos", owner, repo, "issues", issueNumber, "comments").Uri))
      {
        string content = JsonConvert.SerializeObject((object) new
        {
          body = comment
        });
        request.Content = (HttpContent) new StringContent(content, Encoding.UTF8, "application/json");
        request.Headers.Add("Accept", "application/json");
        return this.SendRequestForSingleItem<GitHubData.V3.Comment>(request, authentication, nameof (PostComment));
      }
    }

    public GitHubResult<GitHubData.V3.Repository> GetRepo(
      GitHubAuthentication authentication,
      string repoUrl)
    {
      return this.QueryItem<GitHubData.V3.Repository>(authentication, repoUrl, nameof (GetRepo));
    }

    public GitHubResult<GitHubData.V3.Repository> GetRepo(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      string repository)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(repository, nameof (repository));
      UriBuilder uriBuilder = new GitHubApiRoot(enterpriseUrl).RepositoryUri(repository);
      return this.QueryItem<GitHubData.V3.Repository>(authentication, uriBuilder.AbsoluteUri(), nameof (GetRepo));
    }

    private GitHubResult<GitHubData.V4.Repository[]> GetReposByName_impl(
      GitHubAuthentication authentication,
      string enterpriseUrl,
      IEnumerable<string> nodeNames)
    {
      Dictionary<string, object> variables = new Dictionary<string, object>()
      {
        {
          "nodeNamesList",
          (object) string.Join(" ", nodeNames.ToArray<string>())
        }
      };
      return this.QueryGraphQLItemArray<GitHubData.V4.Repository>(authentication, enterpriseUrl, GitHubHttpClient.s_repoByNameQuery, (IDictionary<string, object>) variables, nameof (GetReposByName_impl));
    }

    public (GitHubResult lastResult, IEnumerable<GitHubData.V4.Repository> repos) GetReposByName(
      GitHubAuthentication authentication,
      string enterpriseUrl,
      IList<string> repoNames)
    {
      GitHubResult<GitHubData.V4.Repository[]> gitHubResult = (GitHubResult<GitHubData.V4.Repository[]>) null;
      List<GitHubData.V4.Repository[]> source = new List<GitHubData.V4.Repository[]>();
      for (int count = 0; count < repoNames.Count || count == 0; count += 100)
      {
        IEnumerable<string> nodeNames = repoNames.Skip<string>(count).Take<string>(100);
        gitHubResult = this.GetReposByName_impl(authentication, enterpriseUrl, nodeNames);
        if (!gitHubResult.IsSuccessful && gitHubResult.Result == null)
          return ((GitHubResult) gitHubResult, (IEnumerable<GitHubData.V4.Repository>) null);
        source.Add(gitHubResult.Result);
      }
      return ((GitHubResult) gitHubResult, source.Count == 1 ? (IEnumerable<GitHubData.V4.Repository>) source[0] : source.SelectMany<GitHubData.V4.Repository[], GitHubData.V4.Repository>((Func<GitHubData.V4.Repository[], IEnumerable<GitHubData.V4.Repository>>) (r => (IEnumerable<GitHubData.V4.Repository>) r)));
    }

    private GitHubResult<GitHubData.V4.Repository[]> GetReposByNodeId_impl(
      GitHubAuthentication authentication,
      string enterpriseUrl,
      IEnumerable<string> nodeIds)
    {
      Dictionary<string, object> variables = new Dictionary<string, object>()
      {
        {
          nameof (nodeIds),
          (object) nodeIds
        }
      };
      return this.QueryGraphQLItemArray<GitHubData.V4.Repository>(authentication, enterpriseUrl, GitHubHttpClient.s_repoByIdQuery, (IDictionary<string, object>) variables, nameof (GetReposByNodeId_impl));
    }

    public (GitHubResult lastResult, IEnumerable<GitHubData.V4.Repository> repos) GetReposByNodeId(
      GitHubAuthentication authentication,
      string enterpriseUrl,
      IList<string> nodeIds)
    {
      GitHubResult<GitHubData.V4.Repository[]> gitHubResult = (GitHubResult<GitHubData.V4.Repository[]>) null;
      List<GitHubData.V4.Repository[]> source = new List<GitHubData.V4.Repository[]>();
      for (int count = 0; count < nodeIds.Count || count == 0; count += 100)
      {
        IEnumerable<string> nodeIds1 = nodeIds.Skip<string>(count).Take<string>(100);
        gitHubResult = this.GetReposByNodeId_impl(authentication, enterpriseUrl, nodeIds1);
        if (!gitHubResult.IsSuccessful && gitHubResult.Result == null)
          return ((GitHubResult) gitHubResult, (IEnumerable<GitHubData.V4.Repository>) null);
        source.Add(gitHubResult.Result);
      }
      return ((GitHubResult) gitHubResult, source.Count == 1 ? (IEnumerable<GitHubData.V4.Repository>) source[0] : source.SelectMany<GitHubData.V4.Repository[], GitHubData.V4.Repository>((Func<GitHubData.V4.Repository[], IEnumerable<GitHubData.V4.Repository>>) (r => (IEnumerable<GitHubData.V4.Repository>) r)));
    }

    public GitHubResult<GitHubData.V3.InstallationDetails> GetInstallationDetails(
      string installationId)
    {
      GitHubAuthentication authentication;
      if (!this.TryGetApplicationTokenAuth(out authentication))
        return GitHubResult<GitHubData.V3.InstallationDetails>.Error("Token provider cannot be null.", HttpStatusCode.BadRequest, (HttpResponseHeaders) null);
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new GitHubApiRoot().AppendPath("app", "installations", installationId).Uri))
        return this.SendRequestForSingleItem<GitHubData.V3.InstallationDetails>(request, authentication, nameof (GetInstallationDetails));
    }

    public GitHubResult<GitHubData.V3.InstallationDetails> GetInstallationDetails(
      GitHubAuthentication appTokenAuthentication,
      string installationId)
    {
      if (appTokenAuthentication.Scheme != GitHubAuthScheme.ApplicationToken)
        return GitHubResult<GitHubData.V3.InstallationDetails>.Error(string.Format("Authentication type {0} does not support GitHub Marketplace APIs.", (object) appTokenAuthentication.Scheme), HttpStatusCode.BadRequest, (HttpResponseHeaders) null);
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new GitHubApiRoot().AppendPath("app", "installations", installationId).Uri))
        return this.SendRequestForSingleItem<GitHubData.V3.InstallationDetails>(request, appTokenAuthentication, nameof (GetInstallationDetails));
    }

    public GitHubResult<GitHubData.V3.UserInstallationDetails> GetUserInstallationDetails(
      GitHubAuthentication authentication)
    {
      ArgumentUtility.CheckForNull<GitHubAuthentication>(authentication, nameof (authentication));
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new GitHubApiRoot().AppendPath("user", "installations").Uri))
      {
        request.Headers.Add("Accept", "application/vnd.github.machine-man-preview+json");
        return this.SendRequestForSingleItem<GitHubData.V3.UserInstallationDetails>(request, authentication, nameof (GetUserInstallationDetails));
      }
    }

    public GitHubResult<GitHubData.V3.MarketplaceListing> GetMarketplaceListing(
      GitHubAuthentication appTokenAuthentication,
      string orgId)
    {
      if (appTokenAuthentication.Scheme != GitHubAuthScheme.ApplicationToken)
        return GitHubResult<GitHubData.V3.MarketplaceListing>.Error(string.Format("Authentication type {0} does not support GitHub Marketplace APIs.", (object) appTokenAuthentication.Scheme), HttpStatusCode.BadRequest, (HttpResponseHeaders) null);
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new GitHubApiRoot().AppendPath("marketplace_listing", "accounts", orgId).Uri))
      {
        request.Headers.Add("Accept", "application/vnd.github.valkyrie-preview+json");
        return this.SendRequestForSingleItem<GitHubData.V3.MarketplaceListing>(request, appTokenAuthentication, nameof (GetMarketplaceListing));
      }
    }

    public async Task<GitHubResult<GitHubData.V3.MarketplaceListing>> GetMarketplaceListingAsync(
      GitHubAuthentication appTokenAuthentication,
      string orgId)
    {
      if (appTokenAuthentication.Scheme != GitHubAuthScheme.ApplicationToken)
        return GitHubResult<GitHubData.V3.MarketplaceListing>.Error(string.Format("Authentication type {0} does not support GitHub Marketplace APIs.", (object) appTokenAuthentication.Scheme), HttpStatusCode.BadRequest, (HttpResponseHeaders) null);
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new GitHubApiRoot().AppendPath("marketplace_listing", "accounts", orgId).Uri))
      {
        request.Headers.Add("Accept", "application/vnd.github.valkyrie-preview+json");
        return await this.SendRequestForSingleItemAsync<GitHubData.V3.MarketplaceListing>(request, appTokenAuthentication, nameof (GetMarketplaceListingAsync));
      }
    }

    public GitHubResult<GitHubData.V4.IRepositoryOwner> GetRepositoryOwner(
      GitHubAuthentication authentication,
      string login)
    {
      Dictionary<string, object> variables = new Dictionary<string, object>()
      {
        {
          nameof (login),
          (object) login
        }
      };
      return this.QueryGraphQLItem<GitHubData.V4.IRepositoryOwner>(authentication, (string) null, GitHubHttpClient.s_repositoryOwnerQuery, (IDictionary<string, object>) variables, nameof (GetRepositoryOwner));
    }

    public GitHubResult<JObject> QueryGraphQL(
      GitHubAuthentication authentication,
      string enterpriseUrl,
      string graphQLquery,
      IDictionary<string, object> variables = null,
      [CallerMemberName] string entryMethodName = null,
      bool enableExperimentalFeatures = false)
    {
      ArgumentUtility.CheckForNull<string>(graphQLquery, nameof (graphQLquery));
      ArgumentUtility.CheckForNull<GitHubAuthentication>(authentication, nameof (authentication));
      int startIndex = graphQLquery.IndexOf("query", StringComparison.CurrentCultureIgnoreCase);
      int num = startIndex >= 0 ? graphQLquery.IndexOf('{', startIndex) : throw new ArgumentException("'query' is missing from the GraphQL query");
      if (num < 0)
        throw new ArgumentException("Missing '{' after 'query'");
      graphQLquery = graphQLquery.Insert(num + 1, GitHubHttpClient.s_rateLimitField);
      variables = variables ?? (IDictionary<string, object>) new Dictionary<string, object>();
      string content = JsonConvert.SerializeObject((object) new
      {
        query = new GraphQLQuery(graphQLquery).SingleLineValue,
        variables = variables
      });
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new GitHubApiRoot(enterpriseUrl).GraphQlUri)
      {
        Content = (HttpContent) new StringContent(content)
      })
      {
        GitHubResult<string> gitHubResult = this.SendRequestForSingleItem(request, authentication, entryMethodName, false, enableExperimentalFeatures);
        if (!gitHubResult.IsSuccessful)
          return new GitHubResult<JObject>((JObject) null, gitHubResult.Errors, gitHubResult.StatusCode);
        GitHubHttpClient.GitHubGraphQLResponse hubGraphQlResponse = GitHubHttpClient.GraphQLConverter.DeserializeObject<GitHubHttpClient.GitHubGraphQLResponse>(gitHubResult.Result);
        if (hubGraphQlResponse.Data == null)
          return GitHubResult<JObject>.Error(hubGraphQlResponse.Errors, gitHubResult.StatusCode, (HttpResponseHeaders) null);
        try
        {
          JToken jtoken;
          if (hubGraphQlResponse.Data.TryGetValue(GitHubHttpClient.s_rateLimit, out jtoken))
          {
            hubGraphQlResponse.Data.Remove(GitHubHttpClient.s_rateLimit);
            GitHubData.V4.RateLimit rateLimit = jtoken.ToObject<GitHubData.V4.RateLimit>();
            if (this.m_rateLimitTracer != null)
            {
              if (rateLimit != null)
              {
                if (rateLimit.Limit > 0)
                  this.m_rateLimitTracer.Trace((IGitHubRateLimit) rateLimit, authentication, entryMethodName, request.Method, request.RequestUri);
              }
            }
          }
        }
        catch
        {
        }
        return new GitHubResult<JObject>(hubGraphQlResponse.Data, hubGraphQlResponse.Errors, gitHubResult.StatusCode);
      }
    }

    public string GetGitHubEnterpriseVersion(
      GitHubAuthentication authentication,
      string enterpriseUrl)
    {
      ArgumentUtility.CheckForNull<GitHubAuthentication>(authentication, nameof (authentication));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(enterpriseUrl, nameof (enterpriseUrl));
      string content = JsonConvert.SerializeObject((object) new
      {
        query = new GraphQLQuery("query { " + GitHubHttpClient.s_rateLimit + " { " + GitHubHttpClient.s_cost + " } }").SingleLineValue,
        variables = new Dictionary<string, object>()
      });
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new GitHubApiRoot(enterpriseUrl).GraphQlUri)
      {
        Content = (HttpContent) new StringContent(content)
      })
      {
        GitHubResult<string> gitHubResult = this.SendRequestForSingleItem(request, authentication, nameof (GetGitHubEnterpriseVersion), false);
        IEnumerable<string> values = (IEnumerable<string>) null;
        gitHubResult.ResponseHeaders?.TryGetValues("X-GitHub-Enterprise-Version", out values);
        return values != null ? values.FirstOrDefault<string>() : (string) null;
      }
    }

    public static JsonSerializer GraphQLSerializer => GitHubHttpClient.GraphQLConverter.Serializer;

    private GitHubResult<T> CreateItem<T>(
      GitHubAuthentication authentication,
      string url,
      string jsonIn,
      string acceptHeader = null,
      HttpMethod httpMethod = null,
      [CallerMemberName] string entryMethodName = null)
    {
      HttpMethod method = httpMethod;
      if ((object) method == null)
        method = HttpMethod.Post;
      using (HttpRequestMessage request = new HttpRequestMessage(method, url))
      {
        request.Content = (HttpContent) new StringContent(jsonIn);
        if (!string.IsNullOrEmpty(acceptHeader))
          request.Headers.Add("Accept", acceptHeader);
        return this.SendRequestForSingleItem<T>(request, authentication, entryMethodName);
      }
    }

    private async Task<GitHubResult<T>> CreateItemAsync<T>(
      GitHubAuthentication authentication,
      string url,
      string jsonIn,
      string acceptHeader = null,
      HttpMethod httpMethod = null,
      [CallerMemberName] string entryMethodName = null)
    {
      HttpMethod method = httpMethod;
      if ((object) method == null)
        method = HttpMethod.Post;
      GitHubResult<T> itemAsync;
      using (HttpRequestMessage request = new HttpRequestMessage(method, url))
      {
        request.Content = (HttpContent) new StringContent(jsonIn);
        if (!string.IsNullOrEmpty(acceptHeader))
          request.Headers.Add("Accept", acceptHeader);
        itemAsync = await this.SendRequestForSingleItemAsync<T>(request, authentication, entryMethodName);
      }
      return itemAsync;
    }

    private GitHubResult<T> UpdateItem<T>(
      GitHubAuthentication authentication,
      string url,
      string jsonIn,
      string acceptHeader = null,
      HttpMethod httpMethod = null,
      [CallerMemberName] string entryMethodName = null)
    {
      HttpMethod method = httpMethod;
      if ((object) method == null)
        method = new HttpMethod("PATCH");
      using (HttpRequestMessage request = new HttpRequestMessage(method, url))
      {
        request.Content = (HttpContent) new StringContent(jsonIn);
        if (!string.IsNullOrEmpty(acceptHeader))
          request.Headers.Add("Accept", acceptHeader);
        return this.SendRequestForSingleItem<T>(request, authentication, entryMethodName);
      }
    }

    private async Task<GitHubResult<T>> UpdateItemAsync<T>(
      GitHubAuthentication authentication,
      string url,
      string jsonIn,
      string acceptHeader = null,
      HttpMethod httpMethod = null,
      [CallerMemberName] string entryMethodName = null)
    {
      HttpMethod method = httpMethod;
      if ((object) method == null)
        method = new HttpMethod("PATCH");
      GitHubResult<T> gitHubResult;
      using (HttpRequestMessage request = new HttpRequestMessage(method, url))
      {
        request.Content = (HttpContent) new StringContent(jsonIn);
        if (!string.IsNullOrEmpty(acceptHeader))
          request.Headers.Add("Accept", acceptHeader);
        gitHubResult = await this.SendRequestForSingleItemAsync<T>(request, authentication, entryMethodName);
      }
      return gitHubResult;
    }

    private GitHubResult DeleteItem(
      GitHubAuthentication authentication,
      string url,
      [CallerMemberName] string entryMethodName = null)
    {
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, url))
        return (GitHubResult) this.SendRequestForSingleItem(request, authentication, entryMethodName);
    }

    private GitHubResult<T> QueryItem<T>(
      GitHubAuthentication authentication,
      string url,
      [CallerMemberName] string entryMethodName = null)
    {
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
        return this.SendRequestForSingleItem<T>(request, authentication, entryMethodName);
    }

    private GitHubResult<T> QueryItem<T>(
      GitHubAuthentication authentication,
      string url,
      string acceptHeader,
      [CallerMemberName] string entryMethodName = null)
    {
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
      {
        request.Headers.Add("Accept", acceptHeader);
        return this.SendRequestForSingleItem<T>(request, authentication, entryMethodName);
      }
    }

    private GitHubResult<string> QueryStringItem(
      GitHubAuthentication authentication,
      string url,
      [CallerMemberName] string entryMethodName = null)
    {
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
        return this.SendRequestForSingleItem(request, authentication, entryMethodName);
    }

    private GitHubResult<T[]> QueryItems<T>(
      GitHubAuthentication authentication,
      string url,
      bool failIfArrayIsNotRetrieved = false,
      [CallerMemberName] string entryMethodName = null)
    {
      return this.QueryItems<T>(authentication, new Uri(url), failIfArrayIsNotRetrieved, entryMethodName);
    }

    private GitHubResult<T[]> QueryItems<T>(
      GitHubAuthentication authentication,
      Uri uri,
      bool failIfArrayIsNotRetrieved = false,
      [CallerMemberName] string entryMethodName = null)
    {
      string nextPageUrl = uri.AppendQuery("per_page", 100.ToString()).AbsoluteUri();
      List<T> source = new List<T>();
      HttpStatusCode statusCode;
      do
      {
        using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, nextPageUrl))
        {
          GitHubResult<T[]> gitHubResult = this.SendRequestForPaginatedItems<T>(request, authentication, failIfArrayIsNotRetrieved, out nextPageUrl, out int _, entryMethodName);
          statusCode = gitHubResult.StatusCode;
          if (!gitHubResult.IsSuccessful)
            return gitHubResult;
          source.AddRange((IEnumerable<T>) gitHubResult.Result);
        }
      }
      while (nextPageUrl != null && source.Count < this.m_maxPaginatedResults);
      return GitHubResult<T[]>.Success(source.ToArray<T>(), statusCode);
    }

    private GitHubResult<T[]> QueryItems<T>(
      GitHubAuthentication authentication,
      Uri uri,
      Func<string, TaggedResultData<T[]>> GetCachedPage,
      Action<string, string, T[]> SetCachedPage,
      bool failIfArrayIsNotRetrieved = false,
      [CallerMemberName] string entryMethodName = null)
    {
      int pageNumber = 1;
      string pageUri = GitHubHttpClient.GetPageUri(uri, pageNumber);
      List<T> source = new List<T>();
      HttpStatusCode httpStatusCode;
      do
      {
        string requestUri = pageUri;
        TaggedResultData<T[]> taggedResultData = GetCachedPage(requestUri);
        using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUri))
        {
          this.AddETagMatchHeaders(request, taggedResultData?.Tags);
          string nextPageUrl;
          GitHubResult<T[]> gitHubResult = this.SendRequestForPaginatedItems<T>(request, authentication, failIfArrayIsNotRetrieved, out nextPageUrl, out int _, entryMethodName);
          this.m_conditionalResponseTracer.Trace<T[]>(gitHubResult, authentication, entryMethodName, request.Method, request.RequestUri, taggedResultData?.Tags);
          ++pageNumber;
          if (gitHubResult.IsUnchangedConditionalResult)
          {
            source.AddRange((IEnumerable<T>) taggedResultData?.Value);
            pageUri = ((IEnumerable<T>) taggedResultData.Value).Count<T>() >= 100 ? GitHubHttpClient.GetPageUri(uri, pageNumber) : (string) null;
            httpStatusCode = HttpStatusCode.NotModified;
          }
          else
          {
            httpStatusCode = gitHubResult.StatusCode;
            if (!gitHubResult.IsSuccessful)
              return gitHubResult;
            pageUri = nextPageUrl == null ? (string) null : GitHubHttpClient.GetPageUri(uri, pageNumber);
            SetCachedPage(gitHubResult.ETagValue, requestUri, gitHubResult.Result);
            source.AddRange((IEnumerable<T>) gitHubResult.Result);
          }
        }
      }
      while (pageUri != null && source.Count < this.m_maxPaginatedResults);
      return GitHubResult<T[]>.Success(source.ToArray<T>(), httpStatusCode);
    }

    private static string GetPageUri(Uri uri, int pageNumber) => uri.AppendQuery("per_page", 100.ToString()).AppendQuery("page", pageNumber.ToString()).AbsoluteUri();

    private GitHubResult<T[]> QueryPagedItems<T>(
      GitHubAuthentication authentication,
      string url,
      [CallerMemberName] string entryMethodName = null)
    {
      string nextPageUrl = url;
      List<T> source = new List<T>();
      HttpStatusCode statusCode;
      do
      {
        using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, nextPageUrl))
        {
          GitHubResult<T> gitHubResult = this.SendRequestForSingleItem<T>(request, authentication, out nextPageUrl, out int _, entryMethodName);
          statusCode = gitHubResult.StatusCode;
          if (!gitHubResult.IsSuccessful)
            return GitHubResult<T[]>.Error(gitHubResult.ErrorMessage, statusCode, (HttpResponseHeaders) null);
          source.Add(gitHubResult.Result);
        }
      }
      while (nextPageUrl != null && source.Count * 100 < this.m_maxPaginatedResults);
      return GitHubResult<T[]>.Success(source.ToArray<T>(), statusCode);
    }

    private GitHubResult<T> SendRequestForSingleItem<T>(
      HttpRequestMessage request,
      GitHubAuthentication authentication,
      [CallerMemberName] string entryMethodName = null)
    {
      return this.SendRequestForSingleItem<T>(request, authentication, out string _, out int _, entryMethodName);
    }

    private async Task<GitHubResult<T>> SendRequestForSingleItemAsync<T>(
      HttpRequestMessage request,
      GitHubAuthentication authentication,
      [CallerMemberName] string entryMethodName = null)
    {
      GitHubResult<string> gitHubResult = await this.SendRequestForSingleItemAsync(request, authentication, entryMethodName);
      if (gitHubResult.IsSuccessful)
      {
        if (!string.IsNullOrWhiteSpace(gitHubResult.Result))
        {
          try
          {
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            return gitHubResult.Convert<T>(GitHubHttpClient.\u003CSendRequestForSingleItemAsync\u003EO__152_0<T>.\u003C0\u003E__DeserializeObject ?? (GitHubHttpClient.\u003CSendRequestForSingleItemAsync\u003EO__152_0<T>.\u003C0\u003E__DeserializeObject = new Func<string, T>(JsonConvert.DeserializeObject<T>)));
          }
          catch (Exception ex)
          {
            return GitHubResult<T>.Error("Error deserializing GitHub API response: " + ex.Message + "\r\n --result value: " + gitHubResult.Result, gitHubResult.StatusCode, gitHubResult.ResponseHeaders);
          }
        }
      }
      return new GitHubResult<T>(default (T), gitHubResult.Errors, gitHubResult.StatusCode, gitHubResult.ResponseHeaders);
    }

    private GitHubResult<T> SendRequestForSingleItem<T>(
      HttpRequestMessage request,
      GitHubAuthentication authentication,
      out string nextPageUrl,
      out int totalPageCount,
      [CallerMemberName] string entryMethodName = null,
      bool enableExperimentalFeatures = false)
    {
      GitHubResult<string> gitHubResult = this.SendRequestForSingleItem(request, authentication, entryMethodName, out nextPageUrl, out totalPageCount, enableExperimentalFeatures);
      if (gitHubResult.IsSuccessful)
      {
        if (!string.IsNullOrWhiteSpace(gitHubResult.Result))
        {
          try
          {
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            return gitHubResult.Convert<T>(GitHubHttpClient.\u003CSendRequestForSingleItem\u003EO__153_0<T>.\u003C0\u003E__DeserializeObject ?? (GitHubHttpClient.\u003CSendRequestForSingleItem\u003EO__153_0<T>.\u003C0\u003E__DeserializeObject = new Func<string, T>(JsonConvert.DeserializeObject<T>)));
          }
          catch (Exception ex)
          {
            return GitHubResult<T>.Error("Error deserializing GitHub API response: " + ex.Message + "\r\n --result value: " + gitHubResult.Result, gitHubResult.StatusCode, gitHubResult.ResponseHeaders);
          }
        }
      }
      return new GitHubResult<T>(default (T), gitHubResult.Errors, gitHubResult.StatusCode, gitHubResult.ResponseHeaders);
    }

    private GitHubResult<string> SendRequestForSingleItem(
      HttpRequestMessage request,
      GitHubAuthentication authentication,
      string entryMethodName,
      bool traceRateLimit = true,
      bool enableExperimentalFeatures = false)
    {
      return this.SendRequestForSingleItem(request, authentication, entryMethodName, out string _, out int _, traceRateLimit, enableExperimentalFeatures);
    }

    private GitHubResult<string> SendRequestForSingleItem(
      HttpRequestMessage request,
      GitHubAuthentication authentication,
      string entryMethodName,
      out string nextPageUrl,
      out int totalPageCount,
      bool traceRateLimit = true,
      bool enableExperimentalFeatures = false)
    {
      nextPageUrl = (string) null;
      totalPageCount = -1;
      VssHttpRequestSettings settings;
      GitHubResult<string> gitHubResult1 = this.PrepareRequest(request, authentication, enableExperimentalFeatures, out settings);
      if (!gitHubResult1.IsSuccessful)
        return gitHubResult1;
      HttpResponseMessage response = (HttpResponseMessage) null;
      using (VssHttpMessageHandler httpMessageHandler = new VssHttpMessageHandler(new VssCredentials(), settings))
      {
        using (IExternalProviderHttpRequester requester = this.m_gitHubHttpRequesterFactory.GetRequester((HttpMessageHandler) httpMessageHandler))
        {
          try
          {
            HttpStatusCode code;
            string errorMessage;
            if (!requester.SendRequest(request, HttpCompletionOption.ResponseHeadersRead, out response, out code, out errorMessage))
              return this.HandleSendRequestForSingleItemError(authentication, response, code, errorMessage);
            Task<string> task = Task.Run<string>((Func<Task<string>>) (() => response.Content.ReadAsStringAsync()));
            if (this.m_cancellationToken.HasValue)
              task.Wait(this.m_cancellationToken.Value);
            else
              task.Wait();
            IGitHubRateLimit rateLimit = (IGitHubRateLimit) null;
            if (traceRateLimit && this.m_rateLimitTracer != null && GitHubHttpClient.TryParseRateLimitHeaders(response, out rateLimit))
              this.m_rateLimitTracer.Trace(rateLimit, authentication, entryMethodName, request.Method, request.RequestUri);
            GitHubHttpClient.ParseUrlLinks(request.RequestUri, response.Headers, out nextPageUrl, out totalPageCount);
            GitHubResult<string> gitHubResult2 = GitHubResult<string>.Success(task.Result, code, response.Headers);
            if (rateLimit != null)
              gitHubResult2.RateLimit = new GitHubData.V4.RateLimit()
              {
                Cost = rateLimit.Cost,
                Limit = rateLimit.Limit,
                Remaining = rateLimit.Remaining,
                ResetAt = rateLimit.ResetAt,
                NodeCount = rateLimit.NodeCount
              };
            return gitHubResult2;
          }
          catch (TimeoutException ex)
          {
            return GitHubResult<string>.Error(string.Format("Request timed out after {0} ms", (object) settings.SendTimeout.TotalMilliseconds), HttpStatusCode.RequestTimeout, (HttpResponseHeaders) null);
          }
          finally
          {
            try
            {
              response?.Dispose();
            }
            catch
            {
            }
          }
        }
      }
    }

    private async Task<GitHubResult<string>> SendRequestForSingleItemAsync(
      HttpRequestMessage request,
      GitHubAuthentication authentication,
      string entryMethodName,
      bool traceRateLimit = true,
      bool enableExperimentalFeatures = false)
    {
      VssHttpRequestSettings settings;
      GitHubResult<string> gitHubResult = this.PrepareRequest(request, authentication, enableExperimentalFeatures, out settings);
      if (!gitHubResult.IsSuccessful)
        return gitHubResult;
      HttpResponseMessage response = (HttpResponseMessage) null;
      using (VssHttpMessageHandler httpMessageHandler = new VssHttpMessageHandler(new VssCredentials(), settings))
      {
        using (IExternalProviderHttpRequester httpRequester = this.m_gitHubHttpRequesterFactory.GetRequester((HttpMessageHandler) httpMessageHandler))
        {
          try
          {
            HttpRequestResult httpRequestResult = await httpRequester.SendRequestAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response = httpRequestResult.Response;
            if (!httpRequestResult.Success)
              return this.HandleSendRequestForSingleItemError(authentication, response, httpRequestResult.Code, httpRequestResult.ErrorMessage);
            string result = await response.Content.ReadAsStringAsync();
            IGitHubRateLimit rateLimit;
            if (traceRateLimit && this.m_rateLimitTracer != null && GitHubHttpClient.TryParseRateLimitHeaders(response, out rateLimit))
              this.m_rateLimitTracer.Trace(rateLimit, authentication, entryMethodName, request.Method, request.RequestUri);
            string nextPageUrl;
            int totalPageCount1;
            GitHubHttpClient.ParseUrlLinks(request.RequestUri, response.Headers, out nextPageUrl, out totalPageCount1);
            int code = (int) httpRequestResult.Code;
            int totalPageCount2 = totalPageCount1;
            string nextPageToken = nextPageUrl;
            HttpResponseHeaders headers = response.Headers;
            return GitHubResult<string>.Success(result, (HttpStatusCode) code, 100, totalPageCount2, nextPageToken, headers);
          }
          catch (TimeoutException ex)
          {
            return GitHubResult<string>.Error(string.Format("Request timed out after {0} ms", (object) settings.SendTimeout.TotalMilliseconds), HttpStatusCode.RequestTimeout, (HttpResponseHeaders) null);
          }
          finally
          {
            try
            {
              response?.Dispose();
            }
            catch
            {
            }
          }
        }
      }
    }

    private GitHubResult<string> HandleSendRequestForSingleItemError(
      GitHubAuthentication authentication,
      HttpResponseMessage response,
      HttpStatusCode httpStatusCode,
      string errorMessage)
    {
      if (authentication?.InstallationAccessToken != null && (httpStatusCode == HttpStatusCode.Forbidden || httpStatusCode == HttpStatusCode.Unauthorized))
      {
        string empty = string.Empty;
        if (response != null)
          response.Headers.TryGetSingleValue("x-github-request-id", out empty);
        GitHubData.InstallationAccessToken installationAccessToken = authentication.InstallationAccessToken;
        List<GitHubError> errors = new List<GitHubError>();
        errors.Add(new GitHubError()
        {
          Message = errorMessage
        });
        errors.Add(new GitHubError()
        {
          Message = string.Format("Using InstallationAccessToken is '{0}' expires '{1}' requestId: '{2}'", installationAccessToken.IsValid() ? (object) "valid" : (object) "invalid", (object) authentication.InstallationAccessToken.Expires_at, (object) empty)
        });
        if (this.m_tokenCache != null && httpStatusCode == HttpStatusCode.Unauthorized)
          this.m_tokenCache.Remove(this.m_tokenProvider.AppType, this.GetTokenCacheKey(authentication, (string) null));
        return GitHubResult<string>.Error((IReadOnlyList<GitHubError>) errors, httpStatusCode, response?.Headers);
      }
      return httpStatusCode == HttpStatusCode.NotModified ? GitHubResult<string>.Success((string) null, httpStatusCode, response.Headers) : GitHubResult<string>.Error(errorMessage, httpStatusCode, response?.Headers);
    }

    private GitHubResult<T[]> SendRequestForPaginatedItems<T>(
      HttpRequestMessage request,
      GitHubAuthentication authentication,
      bool failIfArrayIsNotRetrieved,
      out string nextPageUrl,
      out int totalPageCount,
      [CallerMemberName] string entryMethodName = null,
      bool enableExperimentalFeatures = false)
    {
      GitHubResult<string> gitHubResult = this.SendRequestForSingleItem(request, authentication, entryMethodName, out nextPageUrl, out totalPageCount, enableExperimentalFeatures);
      if (!gitHubResult.IsSuccessful)
        return gitHubResult.Convert<T[]>();
      if (failIfArrayIsNotRetrieved && !IsJsonArray(gitHubResult.Result))
        return GitHubResult<T[]>.Error("The response did not contain an array of objects", HttpStatusCode.BadRequest, gitHubResult.ResponseHeaders);
      try
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return gitHubResult.Convert<T[]>(GitHubHttpClient.\u003CSendRequestForPaginatedItems\u003EO__158_0<T>.\u003C0\u003E__DeserializeObject ?? (GitHubHttpClient.\u003CSendRequestForPaginatedItems\u003EO__158_0<T>.\u003C0\u003E__DeserializeObject = new Func<string, T[]>(JsonConvert.DeserializeObject<T[]>)));
      }
      catch (Exception ex)
      {
        return GitHubResult<T[]>.Error(string.Format("Error deserializing GitHub API response: {0}\r\n --result value: {1}", (object) ex.Message, (object) gitHubResult), gitHubResult.StatusCode, gitHubResult.ResponseHeaders);
      }

      static bool IsJsonArray(string json)
      {
        using (StringReader reader = new StringReader(json))
        {
          using (JsonReader jsonReader = (JsonReader) new JsonTextReader((TextReader) reader))
            return jsonReader.Read() && jsonReader.TokenType == JsonToken.StartArray;
        }
      }
    }

    private GitHubResult<T[]> SendSearchRequestForPaginatedItems<T>(
      HttpRequestMessage request,
      GitHubAuthentication authentication,
      out string nextPageUrl,
      [CallerMemberName] string entryMethodName = null)
    {
      GitHubResult<string> gitHubResult = this.SendRequestForSingleItem(request, authentication, entryMethodName, out nextPageUrl, out int _);
      if (!gitHubResult.IsSuccessful)
        return gitHubResult.Convert<T[]>();
      try
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return new GitHubResult<T[]>(gitHubResult.Convert<GitHubHttpClient.GitHubRESTApiSearchResponse<T>>(GitHubHttpClient.\u003CSendSearchRequestForPaginatedItems\u003EO__159_0<T>.\u003C0\u003E__DeserializeObject ?? (GitHubHttpClient.\u003CSendSearchRequestForPaginatedItems\u003EO__159_0<T>.\u003C0\u003E__DeserializeObject = new Func<string, GitHubHttpClient.GitHubRESTApiSearchResponse<T>>(JsonConvert.DeserializeObject<GitHubHttpClient.GitHubRESTApiSearchResponse<T>>))).Result.Items, gitHubResult.Errors, gitHubResult.StatusCode, (GitHubData.V4.PageInfo) null, gitHubResult.RateLimit);
      }
      catch (Exception ex)
      {
        return GitHubResult<T[]>.Error(string.Format("Error deserializing GitHub API response: {0}\r\n --result value: {1}", (object) ex.Message, (object) gitHubResult), gitHubResult.StatusCode, gitHubResult.ResponseHeaders);
      }
    }

    private static void ParseUrlLinks(
      Uri requestUri,
      HttpResponseHeaders headers,
      out string nextPageUrl,
      out int totalPageCount)
    {
      nextPageUrl = (string) null;
      totalPageCount = -1;
      if (headers.Contains("Link"))
      {
        foreach (string input in headers.GetValues("Link"))
        {
          if (input.Contains("rel=\"next\""))
          {
            Match match = GitHubHttpClient.s_nextLinkHeaderRegex.Match(input);
            if (match.Success)
              nextPageUrl = match.Groups[1].Value;
          }
          if (input.Contains("rel=\"last\""))
          {
            Match match = GitHubHttpClient.s_lastLinkHeaderRegex.Match(input);
            if (match.Success)
              int.TryParse(match.Groups[1].Value, out totalPageCount);
          }
        }
      }
      if (!(requestUri != (Uri) null) || nextPageUrl == null)
        return;
      nextPageUrl = UrlHelper.ReplaceBaseUriForNextPageUrl(requestUri, nextPageUrl);
    }

    private GitHubResult<string> PrepareRequest(
      HttpRequestMessage request,
      GitHubAuthentication authentication,
      bool enableExperimentalFeatures,
      out VssHttpRequestSettings settings)
    {
      settings = new VssHttpRequestSettings();
      if (this.m_timeout.HasValue)
        settings.SendTimeout = this.m_timeout.Value;
      this.AddDefaultRequestHeadersAndSsl(request, authentication, settings);
      this.AddAcceptJsonHeaderIfNeeded(request, authentication != null ? authentication.Scheme : GitHubAuthScheme.None);
      if (enableExperimentalFeatures)
        this.AddPullRequestExperimentalHeader(request);
      return this.AddAuthorizationHeaderIfPossible(request, authentication);
    }

    private void AddDefaultRequestHeadersAndSsl(
      HttpRequestMessage request,
      GitHubAuthentication authentication,
      VssHttpRequestSettings requestSettings)
    {
      request.Headers.Add("User-Agent", GitHubHttpClient.GetVsoUserAgentValue());
      if (authentication == null || !(request.RequestUri.Scheme == "https") || !authentication.AcceptUntrustedCertificates)
        return;
      this.AddSslValidationCallback(requestSettings);
    }

    private static string GetVsoUserAgentValue()
    {
      if (GitHubHttpClient.s_userAgentValue == null)
      {
        string str = string.Empty;
        try
        {
          foreach (object customAttribute in typeof (GitHubHttpClient).Assembly.GetCustomAttributes(false))
          {
            if (customAttribute is AssemblyFileVersionAttribute)
            {
              str = ((AssemblyFileVersionAttribute) customAttribute).Version;
              break;
            }
          }
        }
        catch (Exception ex)
        {
          str = "0.0";
        }
        GitHubHttpClient.s_userAgentValue = "vsts-github-httpclient/v." + str;
      }
      return GitHubHttpClient.s_userAgentValue;
    }

    private void AddAcceptJsonHeaderIfNeeded(HttpRequestMessage request, GitHubAuthScheme scheme)
    {
      if (request.Headers.Contains("Accept"))
        return;
      string str;
      switch (scheme)
      {
        case GitHubAuthScheme.InstallationToken:
        case GitHubAuthScheme.ApplicationToken:
          str = "application/vnd.github.machine-man-preview+json";
          break;
        default:
          str = "application/vnd.GitHub.V3+json";
          break;
      }
      request.Headers.Add("Accept", str);
    }

    private void AddPullRequestExperimentalHeader(HttpRequestMessage request)
    {
      if (request.Headers.Contains("GraphQL-Features"))
        return;
      request.Headers.Add("GraphQL-Features", "pull_request_merge_requirements_api");
    }

    private void AddETagMatchHeaders(HttpRequestMessage request, string[] eTags)
    {
      if (eTags == null)
        return;
      ((IEnumerable<string>) eTags).ForEach<string>((Action<string>) (eTag => this.AddETagMatchHeader(request, eTag)));
    }

    private void AddETagMatchHeader(HttpRequestMessage request, string eTag)
    {
      if (string.IsNullOrEmpty(eTag))
        return;
      request.Headers.IfNoneMatch.ParseAdd(eTag);
    }

    internal GitHubResult<string> AddAuthorizationHeaderIfPossible(
      HttpRequestMessage request,
      GitHubAuthentication authentication)
    {
      string result = (string) null;
      GitHubResult<string> gitHubResult = GitHubResult<string>.Success(string.Empty, HttpStatusCode.OK);
      if (authentication != null)
      {
        switch (authentication.Scheme)
        {
          case GitHubAuthScheme.Token:
            result = "Token " + GitHubHttpClient.SanitizeToken(authentication.AccessToken);
            break;
          case GitHubAuthScheme.Basic:
            result = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(authentication.Username + ":" + authentication.Password));
            break;
          case GitHubAuthScheme.InstallationToken:
            if (authentication.InstallationAccessToken == null || !authentication.InstallationAccessToken.IsValid())
            {
              GitHubResult<GitHubData.InstallationAccessToken> installationToken = this.CreateInstallationToken(authentication, (IEnumerable<int>) null, !authentication.UseFreshAccessToken);
              if (installationToken.IsSuccessful)
              {
                authentication.InstallationAccessToken = installationToken.Result;
                result = "Token " + authentication.InstallationAccessToken.Token;
                break;
              }
              gitHubResult = GitHubResult<string>.Error((IReadOnlyList<GitHubError>) new List<GitHubError>((IEnumerable<GitHubError>) installationToken.Errors)
              {
                new GitHubError()
                {
                  Message = "Failed to generate installation token",
                  Type = "Failed_To_Get_Installation_Token"
                }
              }, installationToken.StatusCode, (HttpResponseHeaders) null);
              break;
            }
            result = "Token " + authentication.InstallationAccessToken.Token;
            break;
          case GitHubAuthScheme.ApplicationToken:
          case GitHubAuthScheme.ApplicationOAuthToken:
            result = "Bearer " + GitHubHttpClient.SanitizeToken(authentication.AccessToken);
            break;
          case GitHubAuthScheme.HmacSha512Signature:
            result = "HMAC-SHA512 Signature=" + authentication.AccessToken;
            break;
        }
        if (!string.IsNullOrEmpty(result))
        {
          request.Headers.Add("Authorization", result);
          gitHubResult = GitHubResult<string>.Success(result, HttpStatusCode.OK);
        }
      }
      return gitHubResult;
    }

    public GitHubResult<GitHubData.InstallationAccessToken> CreateInstallationToken(
      GitHubAuthentication authentication,
      IEnumerable<int> repositoryIds,
      bool useCachedTokens)
    {
      GitHubAuthentication authentication1;
      if (authentication.Scheme != GitHubAuthScheme.InstallationToken || !this.TryGetApplicationTokenAuth(out authentication1))
        return GitHubResult<GitHubData.InstallationAccessToken>.Error("Authentication type does not support generating Installation Access Token.", HttpStatusCode.BadRequest, (HttpResponseHeaders) null);
      UriBuilder uriBuilder = new GitHubApiRoot().AppendPath("app", "installations", authentication.InstallationId.ToString(), "access_tokens");
      List<int> list = repositoryIds != null ? repositoryIds.ToList<int>() : (List<int>) null;
      Dictionary<string, object> source = new Dictionary<string, object>();
      Dictionary<string, string> tokenPermissions = this.GetInstallationTokenPermissions(authentication.PermissionLevel);
      if (tokenPermissions != null && tokenPermissions.Any<KeyValuePair<string, string>>())
        source.Add("permissions", (object) tokenPermissions);
      if (list != null && list.Any<int>())
      {
        list.Sort();
        source.Add("repository_ids", (object) list);
      }
      string str = source.Any<KeyValuePair<string, object>>() ? JsonConvert.SerializeObject((object) source) : (string) null;
      string tokenCacheKey = this.GetTokenCacheKey(authentication, str);
      GitHubData.InstallationAccessToken token;
      if (this.m_tokenCache != null & useCachedTokens && this.m_tokenCache.TryGetToken(this.m_tokenProvider.AppType, tokenCacheKey, out token) && token.IsValid())
        return GitHubResult<GitHubData.InstallationAccessToken>.Success(token, HttpStatusCode.OK);
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri))
      {
        if (str != null)
          request.Content = (HttpContent) new StringContent(str, Encoding.UTF8, "application/json");
        GitHubResult<GitHubData.InstallationAccessToken> installationToken = this.SendRequestForSingleItem<GitHubData.InstallationAccessToken>(request, authentication1, nameof (CreateInstallationToken));
        if (this.m_tokenCache != null && installationToken.IsSuccessful)
          this.m_tokenCache.AddOrUpdate(this.m_tokenProvider.AppType, tokenCacheKey, installationToken.Result);
        return installationToken;
      }
    }

    private Dictionary<string, string> GetInstallationTokenPermissions(
      GitHubPermissionLevel permissionLevel)
    {
      if (permissionLevel == GitHubPermissionLevel.Default)
        return new Dictionary<string, string>();
      if (permissionLevel != GitHubPermissionLevel.ForkedPulllRequest)
        return new Dictionary<string, string>();
      return new Dictionary<string, string>()
      {
        {
          "contents",
          "read"
        }
      };
    }

    private string GetTokenCacheKey(GitHubAuthentication authentication, string jsonContent) => authentication.InstallationId.ToString() + jsonContent;

    private static string SanitizeToken(string token) => string.IsNullOrEmpty(token) ? token : token.Replace('\r', ' ').Replace('\n', ' ');

    private static GraphQLTypeInspector GraphQLTypeInspector { get; } = new GraphQLTypeInspector();

    private static GraphQLConverter GraphQLConverter { get; } = new GraphQLConverter(GitHubHttpClient.GraphQLTypeInspector);

    private void AddSslValidationCallback(VssHttpRequestSettings requestSettings) => requestSettings.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback) ((sender, certificate, chain, sslPolicyErrors) =>
    {
      switch (sslPolicyErrors)
      {
        case SslPolicyErrors.None:
          return true;
        case SslPolicyErrors.RemoteCertificateChainErrors:
          if (((IEnumerable<X509ChainStatus>) chain.ChainStatus).Select<X509ChainStatus, X509ChainStatusFlags>((Func<X509ChainStatus, X509ChainStatusFlags>) (s => s.Status)).Aggregate<X509ChainStatusFlags>((Func<X509ChainStatusFlags, X509ChainStatusFlags, X509ChainStatusFlags>) ((x, y) => x | y)) == X509ChainStatusFlags.UntrustedRoot)
            return true;
          break;
      }
      return false;
    });

    private void ValidateContentAttributes(
      string repository,
      string content,
      string message,
      GitHubData.V3.User committer,
      GitHubData.V3.User author)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(repository, nameof (repository));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(content, nameof (content));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(message, nameof (message));
      if (committer != null)
      {
        ArgumentUtility.CheckStringForNullOrWhiteSpace(committer.Name, "Name");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(committer.Email, "Email");
      }
      if (author == null)
        return;
      ArgumentUtility.CheckStringForNullOrWhiteSpace(author.Name, "Name");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(author.Email, "Email");
    }

    private GitHubResult<T> QueryGraphQLItem<T>(
      GitHubAuthentication authentication,
      string enterpriseUrl,
      GraphQLQuery query,
      IDictionary<string, object> variables = null,
      [CallerMemberName] string entryMethodName = null)
    {
      ArgumentUtility.CheckForNull<GraphQLQuery>(query, nameof (query));
      if (variables == null)
        variables = (IDictionary<string, object>) new Dictionary<string, object>();
      string content = JsonConvert.SerializeObject((object) new
      {
        query = query.SingleLineValue,
        variables = variables
      });
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new GitHubApiRoot(enterpriseUrl).GraphQlUri)
      {
        Content = (HttpContent) new StringContent(content)
      })
        return this.SendRequestForSingleGraphQLItem<T>(request, authentication, entryMethodName);
    }

    private GitHubResult<T[]> QueryGraphQLItems<T>(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      GraphQLQuery query,
      IDictionary<string, object> variables = null,
      [CallerMemberName] string entryMethodName = null,
      bool clearErrorsOnSuccess = false)
    {
      GitHubResult<JObject> gitHubResult = this.QueryGraphQLItem<JObject>(authentication, enterpriseUrl, query, variables, entryMethodName);
      return gitHubResult.Result == null ? GitHubResult<T[]>.Error(gitHubResult.Errors, gitHubResult.StatusCode, (HttpResponseHeaders) null) : new GitHubResult<T[]>(gitHubResult.Result.Properties().Where<JProperty>((Func<JProperty, bool>) (prop => prop.Value != null)).Select<JProperty, T>((Func<JProperty, T>) (p => p.Value.ToObject<T>(GitHubHttpClient.GraphQLSerializer))).Where<T>((Func<T, bool>) (p => (object) p != null)).ToArray<T>(), clearErrorsOnSuccess ? (IReadOnlyList<GitHubError>) null : gitHubResult.Errors, gitHubResult.StatusCode);
    }

    private GitHubResult<T[]> QueryGraphQLItemsWithPagination<T>(
      string enterpriseUrl,
      GitHubAuthentication authentication,
      GraphQLQuery query,
      IDictionary<string, object> variables = null,
      [CallerMemberName] string entryMethodName = null,
      bool clearErrorsOnSuccess = false)
    {
      GitHubResult<JArray> gitHubResult = this.QueryGraphQLItem<JArray>(authentication, enterpriseUrl, query, variables, entryMethodName);
      return !gitHubResult.IsSuccessful || gitHubResult.Errors.Any<GitHubError>() ? GitHubResult<T[]>.Error(gitHubResult.Errors, gitHubResult.StatusCode, (HttpResponseHeaders) null) : new GitHubResult<T[]>(gitHubResult.Result == null ? Array.Empty<T>() : gitHubResult.Result.Where<JToken>((Func<JToken, bool>) (item => item != null)).Select<JToken, T>((Func<JToken, T>) (item => item.ToObject<T>(GitHubHttpClient.GraphQLSerializer))).Where<T>((Func<T, bool>) (item => (object) item != null)).ToArray<T>(), clearErrorsOnSuccess ? (IReadOnlyList<GitHubError>) null : gitHubResult.Errors, gitHubResult.StatusCode, gitHubResult.PageInfo, gitHubResult.RateLimit);
    }

    private GitHubResult<T[]> QueryGraphQLItemArray<T>(
      GitHubAuthentication authentication,
      string enterpriseUrl,
      GraphQLQuery query,
      IDictionary<string, object> variables = null,
      [CallerMemberName] string entryMethodName = null)
    {
      GitHubResult<JArray> gitHubResult = this.QueryGraphQLItem<JArray>(authentication, enterpriseUrl, query, variables, entryMethodName);
      return gitHubResult.Result == null ? GitHubResult<T[]>.Error(gitHubResult.Errors, gitHubResult.StatusCode, (HttpResponseHeaders) null) : gitHubResult.Convert<T[]>((Func<JArray, T[]>) (x => x.Select<JToken, T>((Func<JToken, T>) (p => p.ToObject<T>(GitHubHttpClient.GraphQLSerializer))).ToArray<T>()), true);
    }

    private GitHubResult<T> SendRequestForSingleGraphQLItem<T>(
      HttpRequestMessage request,
      GitHubAuthentication authentication,
      [CallerMemberName] string entryMethodName = null)
    {
      GitHubResult<string> gitHubResult = this.SendRequestForSingleItem(request, authentication, entryMethodName, false);
      if (gitHubResult.IsSuccessful)
      {
        if (!string.IsNullOrWhiteSpace(gitHubResult.Result))
        {
          try
          {
            GitHubHttpClient.GitHubGraphQLItemResponse<T> graphQlItemResponse = GitHubHttpClient.GraphQLConverter.DeserializeObject<GitHubHttpClient.GitHubGraphQLItemResponse<T>>(gitHubResult.Result);
            if (this.m_rateLimitTracer != null && graphQlItemResponse?.Data?.RateLimit != null)
              this.m_rateLimitTracer.Trace((IGitHubRateLimit) graphQlItemResponse.Data.RateLimit, authentication, entryMethodName, request.Method, request.RequestUri);
            T result = default (T);
            GitHubData.V4.PageInfo pageInfo = (GitHubData.V4.PageInfo) null;
            GitHubData.V4.RateLimit rateLimit = (GitHubData.V4.RateLimit) null;
            if (graphQlItemResponse.Data != null)
            {
              rateLimit = graphQlItemResponse.Data.RateLimit;
              GitHubHttpClient.GitHubGraphQLItemResponsePagination<T> responsePagination = graphQlItemResponse.Data.PaginatedItems ?? graphQlItemResponse.Data.Repository?.DefaultBranchRef?.Target?.PaginatedItems;
              if (responsePagination != null)
              {
                result = responsePagination.Items;
                pageInfo = responsePagination.PageInfo;
              }
              else
                result = graphQlItemResponse.Data.Search == null ? graphQlItemResponse.Data.Item : graphQlItemResponse.Data.Search.Item;
            }
            return new GitHubResult<T>(result, graphQlItemResponse.Errors, gitHubResult.StatusCode, pageInfo, rateLimit);
          }
          catch (Exception ex)
          {
            return GitHubResult<T>.Error(string.Format("Error deserializing GitHub GraphQL response: {0}\r\n --result value: {1}", (object) ex.Message, (object) gitHubResult), gitHubResult.StatusCode, (HttpResponseHeaders) null);
          }
        }
      }
      return new GitHubResult<T>(default (T), gitHubResult.Errors, gitHubResult.StatusCode);
    }

    private static void ExtractRepositoryOwnerAndName(
      string repository,
      out string owner,
      out string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(repository, nameof (repository));
      string[] strArray = repository.Split('/');
      owner = strArray.Length == 2 ? strArray[0] : throw new ArgumentException("Expected a repository identifier like 'owner/name', got: " + repository, nameof (repository));
      if (string.IsNullOrWhiteSpace(owner))
        throw new ArgumentException("Expected a repository identifier like 'owner/name', got: " + repository, nameof (repository));
      name = strArray[1];
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Expected a repository identifier like 'owner/name', got: " + repository, nameof (repository));
    }

    public static bool TryParseRateLimitHeaders(
      HttpResponseMessage response,
      out IGitHubRateLimit rateLimit)
    {
      return GitHubHttpClient.TryParseRateLimitHeaders(response.Headers, out rateLimit);
    }

    public static bool TryParseRateLimitHeaders(
      HttpResponseHeaders headers,
      out IGitHubRateLimit rateLimit)
    {
      string s1;
      int result1;
      string s2;
      int result2;
      string s3;
      long result3;
      if (headers.TryGetSingleValue("X-RateLimit-Limit", out s1) && int.TryParse(s1, out result1) && headers.TryGetSingleValue("X-RateLimit-Remaining", out s2) && int.TryParse(s2, out result2) && headers.TryGetSingleValue("X-RateLimit-Reset", out s3) && long.TryParse(s3, out result3))
      {
        rateLimit = (IGitHubRateLimit) new GitHubData.V4.RateLimit()
        {
          Limit = result1,
          Remaining = result2,
          ResetAt = GitHubHttpClient.s_epoch.AddSeconds((double) result3),
          Cost = 1,
          NodeCount = -1
        };
        return true;
      }
      rateLimit = (IGitHubRateLimit) null;
      return false;
    }

    private bool TryGetApplicationTokenAuth(out GitHubAuthentication authentication)
    {
      if (this.m_tokenProvider == null)
      {
        authentication = (GitHubAuthentication) null;
        return false;
      }
      string encodedAppAccessToken = this.m_tokenProvider.CreateEncodedAppAccessToken();
      if (string.IsNullOrEmpty(encodedAppAccessToken))
      {
        authentication = (GitHubAuthentication) null;
        return false;
      }
      authentication = new GitHubAuthentication(GitHubAuthScheme.ApplicationToken, encodedAppAccessToken);
      return true;
    }

    [DataContract]
    private class GitHubGraphQLItemResponse<T>
    {
      [DataMember]
      public GitHubHttpClient.GitHubGraphQLItemResponseData<T> Data { get; set; }

      [DataMember]
      public IReadOnlyList<GitHubError> Errors { get; set; }
    }

    [DataContract]
    private class GitHubGraphQLItemResponseDataSearch<T>
    {
      [DataMember]
      public T Item { get; set; }
    }

    [DataContract]
    private class GitHubGraphQLItemResponsePagination<T>
    {
      [DataMember]
      public T Items { get; set; }

      [DataMember]
      public GitHubData.V4.PageInfo PageInfo { get; set; }
    }

    [DataContract]
    private class GitHubGraphQLItemResponseData<T>
    {
      [DataMember]
      public GitHubData.V4.RateLimit RateLimit { get; set; }

      [DataMember]
      public T Item { get; set; }

      [DataMember]
      public GitHubHttpClient.GitHubGraphQLItemResponseDataSearch<T> Search { get; set; }

      [DataMember]
      public GitHubHttpClient.GitHubGraphQLItemResponsePagination<T> PaginatedItems { get; set; }

      [DataMember]
      public GitHubHttpClient.GitHubGraphQLRepositoryResponseData<T> Repository { get; set; }
    }

    [DataContract]
    private class GitHubGraphQLRepositoryResponseData<T>
    {
      [DataMember]
      public GitHubHttpClient.GitHubGraphQLDefaultBranchRefResponseData<T> DefaultBranchRef { get; set; }
    }

    [DataContract]
    private class GitHubGraphQLDefaultBranchRefResponseData<T>
    {
      [DataMember]
      public GitHubHttpClient.GitHubGraphQLTargetResponseData<T> Target { get; set; }
    }

    [DataContract]
    private class GitHubGraphQLTargetResponseData<T>
    {
      [DataMember]
      public GitHubHttpClient.GitHubGraphQLItemResponsePagination<T> PaginatedItems { get; set; }
    }

    [DataContract]
    private class GitHubGraphQLResponse
    {
      [DataMember]
      public JObject Data { get; set; }

      [DataMember]
      public IReadOnlyList<GitHubError> Errors { get; set; }
    }

    [DataContract]
    private class GitHubRESTApiSearchResponse<T>
    {
      [DataMember]
      public int TotalCount { get; set; }

      [DataMember]
      public bool IncompleteResults { get; set; }

      [DataMember]
      public T[] Items { get; set; }
    }
  }
}
