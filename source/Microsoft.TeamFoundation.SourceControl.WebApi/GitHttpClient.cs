// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitHttpClient
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Diagnostics;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [ClientCircuitBreakerSettings(22, 80)]
  public class GitHttpClient : GitHttpClientBase
  {
    protected static readonly TimeSpan s_sendTimeout = TimeSpan.FromHours(24.0);

    public GitHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials, new VssHttpRequestSettings()
      {
        SendTimeout = GitHttpClient.s_sendTimeout
      })
    {
    }

    public GitHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
    {
      Uri baseUrl1 = baseUrl;
      VssCredentials credentials1 = credentials;
      VssHttpRequestSettings settings = new VssHttpRequestSettings();
      settings.SendTimeout = GitHttpClient.s_sendTimeout;
      DelegatingHandler[] delegatingHandlerArray = handlers;
      // ISSUE: explicit constructor call
      base.\u002Ector(baseUrl1, credentials1, settings, delegatingHandlerArray);
    }

    public GitHttpClient(Uri baseUrl, VssCredentials credentials, VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public GitHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public GitHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<List<GitRef>> GetBranchRefsAsync(Guid repositoryId, object userState = null)
    {
      Guid repositoryId1 = repositoryId;
      object obj = userState;
      bool? includeLinks = new bool?();
      bool? includeStatuses = new bool?();
      bool? includeMyBranches = new bool?();
      bool? latestStatusesOnly = new bool?();
      bool? peelTags = new bool?();
      int? top = new int?();
      object userState1 = obj;
      CancellationToken cancellationToken = new CancellationToken();
      return this.GetRefsAsync(repositoryId1, "heads", includeLinks, includeStatuses, includeMyBranches, latestStatusesOnly, peelTags, (string) null, top, (string) null, userState1, cancellationToken);
    }

    public virtual Task<List<GitRef>> GetTagRefsAsync(Guid repositoryId, object userState = null)
    {
      Guid repositoryId1 = repositoryId;
      object obj = userState;
      bool? includeLinks = new bool?();
      bool? includeStatuses = new bool?();
      bool? includeMyBranches = new bool?();
      bool? latestStatusesOnly = new bool?();
      bool? peelTags = new bool?();
      int? top = new int?();
      object userState1 = obj;
      CancellationToken cancellationToken = new CancellationToken();
      return this.GetRefsAsync(repositoryId1, "tags", includeLinks, includeStatuses, includeMyBranches, latestStatusesOnly, peelTags, (string) null, top, (string) null, userState1, cancellationToken);
    }

    public virtual Task<GitRepository> RenameRepositoryAsync(
      GitRepository repositoryToRename,
      string newName,
      object userState = null)
    {
      return this.UpdateRepositoryAsync(new GitRepository()
      {
        Name = newName
      }, repositoryToRename.Id, userState);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<GitItem>> GetItemsPagedAsync(
      string project,
      string repositoryId,
      string scopePath,
      int? top = null,
      string continuationToken = null,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(scopePath))
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      else
        keyValuePairList.Add("$top", "0");
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      return this.SendAsync<IPagedList<GitItem>>(method, locationId, routeValues, new ApiResourceVersion("3.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<GitItem>>>(this.GetPagedList<GitItem>));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<GitItem>> GetItemsPagedAsync(
      string project,
      Guid repositoryId,
      string scopePath,
      int? top = null,
      string continuationToken = null,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetItemsPagedAsync(project, repositoryId.ToString(), scopePath, top, continuationToken, versionDescriptor, userState, cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<GitItem>> GetItemsPagedAsync(
      Guid project,
      string repositoryId,
      string scopePath,
      int? top = null,
      string continuationToken = null,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetItemsPagedAsync(project.ToString(), repositoryId, scopePath, top, continuationToken, versionDescriptor, userState, cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<GitItem>> GetItemsPagedAsync(
      Guid project,
      Guid repositoryId,
      string scopePath,
      int? top = null,
      string continuationToken = null,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetItemsPagedAsync(project.ToString(), repositoryId.ToString(), scopePath, top, continuationToken, versionDescriptor, userState, cancellationToken);
    }

    public virtual Task<IPagedList<GitCommitRef>> GetPullRequestCommitsPagedAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("52823034-34a8-4576-922c-8d8b77e9e4c4");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<GitCommitRef>>>(this.GetPagedList<GitCommitRef>));
    }

    public virtual Task<IPagedList<GitCommitRef>> GetPullRequestCommitsPagedAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetPullRequestCommitsPagedAsync(project.ToString(), repositoryId, pullRequestId, top, continuationToken, userState);
    }

    public virtual Task<IPagedList<GitCommitRef>> GetPullRequestCommitsPagedAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetPullRequestCommitsPagedAsync(project, repositoryId.ToString(), pullRequestId, top, continuationToken, userState);
    }

    public virtual Task<IPagedList<GitCommitRef>> GetPullRequestCommitsPagedAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetPullRequestCommitsPagedAsync(project.ToString(), repositoryId.ToString(), pullRequestId, top, continuationToken, userState);
    }

    protected async Task<IPagedList<T>> GetPagedList<T>(
      HttpResponseMessage responseMessage,
      CancellationToken cancellationToken)
    {
      GitHttpClient gitHttpClient = this;
      string continuationToken = gitHttpClient.GetContinuationToken(responseMessage);
      IPagedList<T> pagedList = (IPagedList<T>) new PagedList<T>((IEnumerable<T>) await gitHttpClient.ReadContentAsAsync<List<T>>(responseMessage, cancellationToken).ConfigureAwait(false), continuationToken);
      continuationToken = (string) null;
      return pagedList;
    }

    protected string GetContinuationToken(HttpResponseMessage responseMessage)
    {
      string continuationToken = (string) null;
      IEnumerable<string> values = (IEnumerable<string>) null;
      if (responseMessage.Headers.TryGetValues("x-ms-continuationtoken", out values))
        continuationToken = values.FirstOrDefault<string>();
      return continuationToken;
    }

    protected Task<T> SendAsync<T>(
      HttpMethod method,
      Guid locationId,
      object routeValues = null,
      ApiResourceVersion version = null,
      HttpContent content = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken),
      Func<HttpResponseMessage, CancellationToken, Task<T>> processResponse = null)
    {
      return this.SendAsync<T>(method, (IEnumerable<KeyValuePair<string, string>>) null, locationId, routeValues, version, content, queryParameters, userState, cancellationToken, processResponse);
    }

    protected async Task<T> SendAsync<T>(
      HttpMethod method,
      IEnumerable<KeyValuePair<string, string>> additionalHeaders,
      Guid locationId,
      object routeValues = null,
      ApiResourceVersion version = null,
      HttpContent content = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken),
      Func<HttpResponseMessage, CancellationToken, Task<T>> processResponse = null)
    {
      GitHttpClient gitHttpClient = this;
      T obj;
      using (VssTraceActivity.GetOrCreate().EnterCorrelationScope())
      {
        using (HttpRequestMessage requestMessage = await gitHttpClient.CreateRequestMessageAsync(method, additionalHeaders, locationId, routeValues, version, content, queryParameters, userState, cancellationToken).ConfigureAwait(false))
          obj = await gitHttpClient.SendAsync<T>(requestMessage, userState, cancellationToken, processResponse).ConfigureAwait(false);
      }
      return obj;
    }

    protected async Task<T> SendAsync<T>(
      HttpRequestMessage message,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken),
      Func<HttpResponseMessage, CancellationToken, Task<T>> processResponse = null)
    {
      GitHttpClient gitHttpClient = this;
      if (processResponse == null)
        processResponse = new Func<HttpResponseMessage, CancellationToken, Task<T>>(((VssHttpClientBase) gitHttpClient).ReadContentAsAsync<T>);
      T obj;
      using (HttpResponseMessage response = await gitHttpClient.SendAsync(message, userState, cancellationToken).ConfigureAwait(false))
        obj = await processResponse(response, cancellationToken).ConfigureAwait(false);
      return obj;
    }

    protected override void AddModelAsQueryParams(
      IList<KeyValuePair<string, string>> queryParams,
      string parameterName,
      object model)
    {
      switch (model)
      {
        case null:
          break;
        case GitVersionDescriptor _ when !(model is GitBaseVersionDescriptor) && !(model is GitTargetVersionDescriptor) && parameterName.Equals("baseVersionDescriptor"):
          base.AddModelAsQueryParams(queryParams, parameterName, model);
          break;
        case GitVersionDescriptor _:
          using (IEnumerator<JProperty> enumerator = JObject.FromObject(model, new VssJsonMediaTypeFormatter().CreateJsonSerializer()).Properties().GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              JProperty current = enumerator.Current;
              if (current.Value != null)
                queryParams.Add(current.Name, current.Value.ToString());
            }
            break;
          }
        default:
          base.AddModelAsQueryParams(queryParams, parameterName, model);
          break;
      }
    }

    internal void TryAddParam<T>(
      IList<KeyValuePair<string, string>> queryParams,
      string paramName,
      T paramValue)
    {
      if (EqualityComparer<T>.Default.Equals(paramValue, default (T)))
        return;
      queryParams.Add(new KeyValuePair<string, string>(paramName, paramValue.ToString()));
    }
  }
}
