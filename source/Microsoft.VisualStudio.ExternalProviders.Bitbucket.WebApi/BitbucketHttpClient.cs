// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi.BitbucketHttpClient
// Assembly: Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 223C9BE7-A3E9-431B-86B7-A81B8A6447FF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi
{
  public sealed class BitbucketHttpClient
  {
    private readonly IExternalProviderHttpRequesterFactory m_httpRequesterFactory;
    private readonly int m_maxPaginatedResults;
    private readonly TimeSpan m_timeout;
    private readonly CancellationToken? m_cancellationToken;
    private static string s_userAgentValue;

    internal BitbucketHttpClient(
      IExternalProviderHttpRequesterFactory httpRequesterFactory,
      TimeSpan? timeout,
      int maxPaginatedResults = 2000,
      CancellationToken? cancellationToken = null)
    {
      this.m_httpRequesterFactory = httpRequesterFactory;
      this.m_maxPaginatedResults = maxPaginatedResults;
      this.m_timeout = timeout ?? TimeSpan.FromSeconds(30.0);
      this.m_cancellationToken = cancellationToken;
    }

    public BitbucketResult<BitbucketData.Authorization> GetAuthorizationFromCode(
      string clientId,
      string clientSecret,
      string code)
    {
      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://bitbucket.org/site/oauth2/access_token");
      request.Headers.Add("Accept", "application/x-www-form-urlencoded");
      request.Headers.Add("User-Agent", BitbucketHttpClient.GetVsoUserAgentValue());
      request.Content = (HttpContent) new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>) new Dictionary<string, string>()
      {
        ["grant_type"] = "authorization_code",
        ["client_id"] = clientId,
        ["client_secret"] = clientSecret,
        [nameof (code)] = code
      });
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return this.SendRequest<BitbucketData.Authorization>(request, BitbucketHttpClient.\u003C\u003EO.\u003C0\u003E__GetOAuthErrorDetailsForRequest ?? (BitbucketHttpClient.\u003C\u003EO.\u003C0\u003E__GetOAuthErrorDetailsForRequest = new Func<string, string>(BitbucketHttpClient.GetOAuthErrorDetailsForRequest)));
    }

    public BitbucketResult<BitbucketData.Authorization> GetAuthorizationFromToken(
      string clientId,
      string clientSecret,
      string refreshToken)
    {
      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://bitbucket.org/site/oauth2/access_token");
      request.Headers.Add("Accept", "application/x-www-form-urlencoded");
      request.Headers.Add("User-Agent", BitbucketHttpClient.GetVsoUserAgentValue());
      request.Content = (HttpContent) new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>) new Dictionary<string, string>()
      {
        ["grant_type"] = "refresh_token",
        ["client_id"] = clientId,
        ["client_secret"] = clientSecret,
        ["refresh_token"] = refreshToken
      });
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return this.SendRequest<BitbucketData.Authorization>(request, BitbucketHttpClient.\u003C\u003EO.\u003C0\u003E__GetOAuthErrorDetailsForRequest ?? (BitbucketHttpClient.\u003C\u003EO.\u003C0\u003E__GetOAuthErrorDetailsForRequest = new Func<string, string>(BitbucketHttpClient.GetOAuthErrorDetailsForRequest)));
    }

    public BitbucketResult<BitbucketData.V2.AuthenticatedUser> GetCurrentUser(
      BitbucketData.Authentication authentication)
    {
      string url = "https://api.bitbucket.org/2.0/user";
      return this.QueryItem<BitbucketData.V2.AuthenticatedUser>(authentication, url);
    }

    public BitbucketResult<BitbucketData.V2.Repository[]> GetReposUserIsMemberOf(
      BitbucketData.Authentication authentication)
    {
      string url = "https://api.bitbucket.org/2.0/repositories?role=member";
      return this.QueryItems<BitbucketData.V2.Repository>(authentication, url);
    }

    public BitbucketResult<BitbucketData.V2.Repository> GetUserRepoByName(
      BitbucketData.Authentication authentication,
      string repository)
    {
      string url = "https://api.bitbucket.org/2.0/repositories/" + repository;
      return this.QueryItem<BitbucketData.V2.Repository>(authentication, url);
    }

    public BitbucketResult<BitbucketData.V2.Branch[]> GetRepoBranches(
      BitbucketData.Authentication authentication,
      string repository)
    {
      string url = BitbucketUrl.Api.Repository(repository).AppendPathSegments("refs", "branches").AbsoluteUri();
      return this.QueryItems<BitbucketData.V2.Branch>(authentication, url);
    }

    public BitbucketResult<BitbucketData.V2.Branch> GetRepoBranch(
      BitbucketData.Authentication authentication,
      string repository,
      string branch)
    {
      if (branch.StartsWith("refs/heads/", StringComparison.OrdinalIgnoreCase))
        branch = branch.Substring("refs/heads/".Length);
      string url = BitbucketUrl.Api.Repository(repository).AppendPathSegments("refs", "branches", branch).AbsoluteUri();
      return this.QueryItem<BitbucketData.V2.Branch>(authentication, url);
    }

    public BitbucketResult<BitbucketData.V2.Tag> GetRepoTag(
      BitbucketData.Authentication authentication,
      string repository,
      string tag)
    {
      if (tag.StartsWith("refs/tags/", StringComparison.OrdinalIgnoreCase))
        tag = tag.Substring("refs/tags/".Length);
      string url = BitbucketUrl.Api.Repository(repository).AppendPathSegments("refs", "tags", tag).AbsoluteUri();
      return this.QueryItem<BitbucketData.V2.Tag>(authentication, url);
    }

    public BitbucketResult<BitbucketData.V2.Commit[]> GetRepoCommits(
      BitbucketData.Authentication authentication,
      string repoApiUrl,
      IEnumerable<string> includeRefs,
      IEnumerable<string> excludeRefs)
    {
      UriBuilder uriBuilder = new UriBuilder(repoApiUrl).AppendPathSegments("commits");
      foreach (string includeRef in includeRefs)
        uriBuilder.AppendQuery("include", includeRef);
      foreach (string excludeRef in excludeRefs)
        uriBuilder.AppendQuery("exclude", excludeRef);
      return this.QueryItems<BitbucketData.V2.Commit>(authentication, uriBuilder.AbsoluteUri());
    }

    public BitbucketResult<BitbucketData.V2.Commit> GetRepoCommit(
      BitbucketData.Authentication authentication,
      string repoApiUrl,
      string commitHash)
    {
      string url = repoApiUrl + "/commit/" + commitHash;
      return this.QueryItem<BitbucketData.V2.Commit>(authentication, url);
    }

    public BitbucketResult<BitbucketData.V2.Commit[]> GetPullRequestCommits(
      BitbucketData.Authentication authentication,
      string repoApiUrl,
      int pullRequestId)
    {
      string url = new UriBuilder(repoApiUrl).AppendPathSegments("pullrequests", pullRequestId.ToString(), "commits").AbsoluteUri();
      return this.QueryItems<BitbucketData.V2.Commit>(authentication, url);
    }

    public BitbucketResult<BitbucketData.V2.DiffStat[]> GetPullRequestFiles(
      BitbucketData.Authentication authentication,
      string repoApiUrl,
      int pullRequestId)
    {
      string url = new UriBuilder(repoApiUrl).AppendPathSegments("pullrequests", pullRequestId.ToString(), "diffstat").AbsoluteUri();
      return this.QueryItems<BitbucketData.V2.DiffStat>(authentication, url);
    }

    public BitbucketResult<BitbucketData.V2.FileDiff[]> GetCommitsDiff(
      BitbucketData.Authentication authentication,
      string repoApiUrl,
      string startSha,
      string endSha)
    {
      string url = new UriBuilder(repoApiUrl).AppendPathSegments("diffstat", startSha + ".." + endSha).AbsoluteUri();
      return this.QueryItems<BitbucketData.V2.FileDiff>(authentication, url);
    }

    public BitbucketResult<BitbucketData.V2.Hook[]> GetRepoHooks(
      BitbucketData.Authentication authentication,
      string repoApiUrl)
    {
      string url = repoApiUrl + "/hooks";
      return this.QueryItems<BitbucketData.V2.Hook>(authentication, url);
    }

    public BitbucketResult<BitbucketData.V2.Hook> CreateRepoHook(
      BitbucketData.Authentication authentication,
      string repoApiUrl,
      BitbucketData.V2.Hook hook)
    {
      string url = repoApiUrl + "/hooks";
      string jsonIn = JsonConvert.SerializeObject((object) hook);
      return this.CreateItem<BitbucketData.V2.Hook>(authentication, url, jsonIn);
    }

    public BitbucketResult DeleteRepoHook(
      BitbucketData.Authentication authentication,
      string repoApiUrl,
      string hookId)
    {
      string url = repoApiUrl + "/hooks/" + hookId;
      return this.DeleteItem(authentication, url);
    }

    public BitbucketResult<BitbucketData.V2.Build> CreateCommitBuild(
      BitbucketData.Authentication authentication,
      string repoApiUrl,
      string commitHash,
      BitbucketData.V2.Build build)
    {
      string url = repoApiUrl + "/commit/" + commitHash + "/statuses/build";
      string jsonIn = JsonConvert.SerializeObject((object) build);
      return this.CreateItem<BitbucketData.V2.Build>(authentication, url, jsonIn);
    }

    public BitbucketResult<BitbucketData.V2.ContentData[]> GetPathContents(
      BitbucketData.Authentication authentication,
      string repository,
      string version,
      string path,
      int maxDepth = 1)
    {
      return this.BranchToSha(authentication, repository, version).Then<BitbucketData.V2.ContentData[]>((Func<string, BitbucketResult<BitbucketData.V2.ContentData[]>>) (sha =>
      {
        UriBuilder uriBuilder = BitbucketUrl.Api.Repository(repository).AppendPathSegments("src", sha, path).AppendQuery("max_depth", maxDepth.ToString());
        uriBuilder.Path += "/";
        return this.QueryItems<BitbucketData.V2.ContentData>(authentication, uriBuilder.AbsoluteUri());
      }));
    }

    public BitbucketResult<string> GetFileContent(
      BitbucketData.Authentication authentication,
      string repository,
      string version,
      string filePath)
    {
      return this.BranchToSha(authentication, repository, version).Then<string>((Func<string, BitbucketResult<string>>) (sha =>
      {
        UriBuilder uriBuilder = BitbucketUrl.Api.Repository(repository);
        string[] strArray = new string[3]
        {
          "src",
          sha,
          filePath
        };
        return this.QueryItem(authentication, uriBuilder.AppendPathSegments(strArray).AbsoluteUri());
      }));
    }

    public BitbucketResult CreateFileContent(
      BitbucketData.Authentication authentication,
      string repository,
      string filePath,
      string branch,
      string fileContent,
      string message)
    {
      string requestUri = BitbucketUrl.Api.Repository(repository).AppendPathSegments("src").AbsoluteUri();
      Dictionary<string, string> nameValueCollection = new Dictionary<string, string>()
      {
        [filePath] = fileContent,
        [nameof (message)] = message ?? string.Empty
      };
      if (!string.IsNullOrEmpty(branch))
        nameValueCollection.Add(nameof (branch), branch);
      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri)
      {
        Content = (HttpContent) new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>) nameValueCollection)
      };
      BitbucketHttpClient.AddDefaultRequestHeaders(request, authentication);
      request.Headers.Add("User-Agent", BitbucketHttpClient.GetVsoUserAgentValue());
      return (BitbucketResult) this.SendRequest(request);
    }

    public BitbucketResult<BitbucketData.V2.ContentData> GetFileContentData(
      BitbucketData.Authentication authentication,
      string repository,
      string version,
      string filePath)
    {
      return this.BranchToSha(authentication, repository, version).Then<BitbucketData.V2.ContentData>((Func<string, BitbucketResult<BitbucketData.V2.ContentData>>) (sha =>
      {
        UriBuilder uriBuilder = BitbucketUrl.Api.Repository(repository);
        string[] strArray = new string[3]
        {
          "src",
          sha,
          filePath
        };
        return this.QueryItem<BitbucketData.V2.ContentData>(authentication, uriBuilder.AppendPathSegments(strArray).AppendQuery("format", "meta").AbsoluteUri());
      }));
    }

    public BitbucketResult<BitbucketData.V2.PullRequest> GetPullRequest(
      BitbucketData.Authentication authentication,
      string repository,
      int id)
    {
      string url = BitbucketUrl.Api.Repository(repository).AppendPathSegments("pullrequests").AppendPathSegments(id.ToString()).AbsoluteUri();
      return this.QueryItem<BitbucketData.V2.PullRequest>(authentication, url);
    }

    public BitbucketResult<BitbucketData.V2.PullRequest> CreatePullRequest(
      BitbucketData.Authentication authentication,
      string repository,
      string targetBranch,
      string sourceBranch,
      string title,
      string description = null)
    {
      string url = BitbucketUrl.Api.Repository(repository).AppendPathSegments("pullrequests").AbsoluteUri();
      BitbucketData.V2.PullRequest pullRequest = new BitbucketData.V2.PullRequest()
      {
        Title = title,
        Source = new BitbucketData.V2.Target()
        {
          Branch = new BitbucketData.V2.Branch()
          {
            Name = sourceBranch
          }
        }
      };
      if (!string.IsNullOrEmpty(targetBranch))
        pullRequest.Destination = new BitbucketData.V2.Target()
        {
          Branch = new BitbucketData.V2.Branch()
          {
            Name = targetBranch
          }
        };
      if (!string.IsNullOrEmpty(description))
        pullRequest.Description = description;
      string jsonIn = JsonConvert.SerializeObject((object) pullRequest, new JsonSerializerSettings()
      {
        NullValueHandling = NullValueHandling.Ignore
      });
      return this.CreateItem<BitbucketData.V2.PullRequest>(authentication, url, jsonIn);
    }

    private BitbucketResult<string> BranchToSha(
      BitbucketData.Authentication authentication,
      string repository,
      string gitRef)
    {
      return gitRef.Contains<char>('/') ? this.GetRepoBranch(authentication, repository, gitRef).Convert<string>((Func<BitbucketData.V2.Branch, string>) (x => x.Target.Hash)) : BitbucketResult<string>.Success(gitRef, HttpStatusCode.OK);
    }

    private BitbucketResult<T> CreateItem<T>(
      BitbucketData.Authentication authentication,
      string url,
      string jsonIn)
    {
      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
      BitbucketHttpClient.AddDefaultRequestHeaders(request, authentication);
      request.Content = (HttpContent) new StringContent(jsonIn, Encoding.UTF8, "application/json");
      return this.SendRequest<T>(request);
    }

    private BitbucketResult DeleteItem(BitbucketData.Authentication authentication, string url)
    {
      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, url);
      BitbucketHttpClient.AddDefaultRequestHeaders(request, authentication);
      return (BitbucketResult) this.SendRequest<object>(request);
    }

    private BitbucketResult<T> QueryItem<T>(BitbucketData.Authentication authentication, string url)
    {
      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
      BitbucketHttpClient.AddDefaultRequestHeaders(request, authentication);
      return this.SendRequest<T>(request);
    }

    private BitbucketResult<string> QueryItem(
      BitbucketData.Authentication authentication,
      string url)
    {
      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
      BitbucketHttpClient.AddDefaultRequestHeaders(request, authentication);
      return this.SendRequest(request);
    }

    private BitbucketResult<T[]> QueryItems<T>(
      BitbucketData.Authentication authentication,
      string url)
    {
      string str = url.Contains("?") ? "&" : "?";
      string requestUri = url + str + "pagelen=100";
      List<T> source = new List<T>();
      Stopwatch stopwatch = Stopwatch.StartNew();
      HttpStatusCode statusCode;
      do
      {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        BitbucketHttpClient.AddDefaultRequestHeaders(request, authentication);
        BitbucketResult<BitbucketData.V2.PagedResponse<T>> bitbucketResult = this.SendRequest<BitbucketData.V2.PagedResponse<T>>(request);
        statusCode = bitbucketResult.StatusCode;
        if (!bitbucketResult.IsSuccessful)
          return bitbucketResult.ConvertError<T[]>();
        source.AddRange((IEnumerable<T>) bitbucketResult.Result.Values);
        requestUri = bitbucketResult.Result.Next;
      }
      while (requestUri != null && source.Count < this.m_maxPaginatedResults && (double) stopwatch.ElapsedMilliseconds < this.m_timeout.TotalMilliseconds);
      return BitbucketResult<T[]>.Success(source.ToArray<T>(), requestUri == null ? statusCode : HttpStatusCode.PartialContent);
    }

    private BitbucketResult<T> SendRequest<T>(
      HttpRequestMessage request,
      Func<string, string> getErrorFromResponse = null)
    {
      return this.SendRequest(request, getErrorFromResponse).Convert<T>((Func<string, T>) (x => !string.IsNullOrWhiteSpace(x) ? JsonConvert.DeserializeObject<T>(x) : default (T)));
    }

    private BitbucketResult<string> SendRequest(
      HttpRequestMessage request,
      Func<string, string> getErrorFromResponse = null)
    {
      getErrorFromResponse = getErrorFromResponse ?? new Func<string, string>(this.GetErrorDetailsForRequest);
      try
      {
        HttpResponseMessage response = this.SendHttpRequest(request);
        if (response.StatusCode == HttpStatusCode.Found)
          response = this.HandleRedirect(request, response.Headers.Location);
        Task<string> task = Task.Run<string>((Func<Task<string>>) (() => response.Content.ReadAsStringAsync()));
        if (this.m_cancellationToken.HasValue)
          task.Wait(this.m_cancellationToken.Value);
        else
          task.Wait();
        return !response.IsSuccessStatusCode ? BitbucketResult<string>.Error((string.IsNullOrEmpty(task.Result) ? (string) null : getErrorFromResponse(task.Result)) ?? this.GetReasonPhrase(response), response.StatusCode) : BitbucketResult<string>.Success(task.Result, response.StatusCode);
      }
      catch (VssUnauthorizedException ex)
      {
        return BitbucketResult<string>.Error(ex.Message, HttpStatusCode.Unauthorized);
      }
      catch (Exception ex) when (ex is TimeoutException || ex is TaskCanceledException)
      {
        return BitbucketResult<string>.Error(string.Format("Request timed out after {0} ms", (object) this.m_timeout.TotalMilliseconds), HttpStatusCode.RequestTimeout);
      }
    }

    private HttpResponseMessage HandleRedirect(HttpRequestMessage request, Uri redirectUri)
    {
      HttpRequestMessage request1 = new HttpRequestMessage(request.Method, redirectUri);
      if (request.Headers.Authorization != null)
        request1.Headers.Add("Authorization", request.Headers.Authorization.ToString());
      request1.Headers.Add("User-Agent", BitbucketHttpClient.GetVsoUserAgentValue());
      return this.SendHttpRequest(request1);
    }

    private HttpResponseMessage SendHttpRequest(HttpRequestMessage request)
    {
      using (VssHttpMessageHandler httpMessageHandler = new VssHttpMessageHandler(new VssCredentials(), new VssHttpRequestSettings()
      {
        SendTimeout = this.m_timeout
      }))
      {
        using (IExternalProviderHttpRequester requester = this.m_httpRequesterFactory.GetRequester((HttpMessageHandler) httpMessageHandler))
        {
          HttpResponseMessage response;
          requester.SendRequest(request, HttpCompletionOption.ResponseContentRead, out response, out HttpStatusCode _, out string _);
          return response;
        }
      }
    }

    private string GetErrorDetailsForRequest(string responseContent)
    {
      ArgumentUtility.CheckForNull<string>(responseContent, nameof (responseContent));
      try
      {
        return JsonConvert.DeserializeObject<BitbucketData.V2.ErrorResponse>(responseContent).Error.Message;
      }
      catch (Exception ex)
      {
        return (string) null;
      }
    }

    private string GetReasonPhrase(HttpResponseMessage response) => response != null && response.StatusCode == (HttpStatusCode) 429 ? "Too Many Requests" : response?.ReasonPhrase ?? "Unknown error";

    private static string GetOAuthErrorDetailsForRequest(string responseContent)
    {
      ArgumentUtility.CheckForNull<string>(responseContent, nameof (responseContent));
      try
      {
        BitbucketData.AuthorizationError authorizationError = JsonConvert.DeserializeObject<BitbucketData.AuthorizationError>(responseContent);
        return string.IsNullOrEmpty(authorizationError.Description) ? authorizationError.Error : authorizationError.Error + ": " + authorizationError.Description;
      }
      catch (Exception ex)
      {
        return (string) null;
      }
    }

    private static void AddDefaultRequestHeaders(
      HttpRequestMessage request,
      BitbucketData.Authentication authentication)
    {
      if (authentication == null)
        throw new ApplicationException("Authentication must not be null");
      if (!string.IsNullOrEmpty(authentication.AccessToken))
      {
        request.Headers.Add("Authorization", "Bearer " + authentication.AccessToken);
      }
      else
      {
        if (string.IsNullOrEmpty(authentication.Username) || string.IsNullOrEmpty(authentication.Password))
          throw new ApplicationException("Authentication must be either basic or oauth");
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(authentication.Username + ":" + authentication.Password)));
      }
      request.Headers.Add("User-Agent", BitbucketHttpClient.GetVsoUserAgentValue());
    }

    private static string GetVsoUserAgentValue()
    {
      if (BitbucketHttpClient.s_userAgentValue != null)
        return BitbucketHttpClient.s_userAgentValue;
      string str;
      try
      {
        str = typeof (BitbucketHttpClient).Assembly.GetCustomAttributes(false).OfType<AssemblyFileVersionAttribute>().FirstOrDefault<AssemblyFileVersionAttribute>()?.Version ?? "unavailable";
      }
      catch (Exception ex)
      {
        str = "unavailable";
      }
      BitbucketHttpClient.s_userAgentValue = "VSOnline/" + str;
      return BitbucketHttpClient.s_userAgentValue;
    }
  }
}
