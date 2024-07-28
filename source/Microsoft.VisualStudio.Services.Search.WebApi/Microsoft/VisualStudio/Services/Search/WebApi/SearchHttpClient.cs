// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.SearchHttpClient
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9E496DAE-109A-4A16-A97B-F7DEDEC6CB20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Package;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Wiki;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts.TfvcRepositoryStatus;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.WebApi
{
  [ResourceArea("EA48A0A1-269C-42D8-B8AD-DDC8FCDCF578")]
  public class SearchHttpClient : VssHttpClientBase
  {
    public SearchHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public SearchHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public SearchHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public SearchHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public SearchHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<CodeSearchResponse> FetchAdvancedCodeSearchResultsAsync(
      CodeSearchRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6bc8d206-9a7e-4cfb-83e1-c9a81e7bf166");
      HttpContent httpContent = (HttpContent) new ObjectContent<CodeSearchRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CodeSearchResponse>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<CodeSearchResponse> FetchAdvancedCodeSearchResultsAsync(
      CodeSearchRequest request,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6bc8d206-9a7e-4cfb-83e1-c9a81e7bf166");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<CodeSearchRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CodeSearchResponse>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<CodeSearchResponse> FetchAdvancedCodeSearchResultsAsync(
      CodeSearchRequest request,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6bc8d206-9a7e-4cfb-83e1-c9a81e7bf166");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<CodeSearchRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CodeSearchResponse>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<CodeSearchResponse> FetchScrollCodeSearchResultsAsync(
      ScrollSearchRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("852dac94-e8f7-45a2-9910-927ae35766a2");
      HttpContent httpContent = (HttpContent) new ObjectContent<ScrollSearchRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CodeSearchResponse>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<CodeSearchResponse> FetchScrollCodeSearchResultsAsync(
      ScrollSearchRequest request,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("852dac94-e8f7-45a2-9910-927ae35766a2");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ScrollSearchRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CodeSearchResponse>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<CodeSearchResponse> FetchScrollCodeSearchResultsAsync(
      ScrollSearchRequest request,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("852dac94-e8f7-45a2-9910-927ae35766a2");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ScrollSearchRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CodeSearchResponse>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<CodeSearchResponse> FetchCodeSearchResultsAsync(
      CodeSearchRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e7f29993-5b82-4fca-9386-f5cfe683d524");
      HttpContent httpContent = (HttpContent) new ObjectContent<CodeSearchRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CodeSearchResponse>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<CodeSearchResponse> FetchCodeSearchResultsAsync(
      CodeSearchRequest request,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e7f29993-5b82-4fca-9386-f5cfe683d524");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<CodeSearchRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CodeSearchResponse>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<CodeSearchResponse> FetchCodeSearchResultsAsync(
      CodeSearchRequest request,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e7f29993-5b82-4fca-9386-f5cfe683d524");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<CodeSearchRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CodeSearchResponse>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<CustomRepositoryStatusResponse> GetCustomRepositoryStatusAsync(
      string project,
      string repository,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CustomRepositoryStatusResponse>(new HttpMethod("GET"), new Guid("de4c37b8-f55f-4c38-8f3f-a296cfb63121"), (object) new
      {
        project = project,
        repository = repository
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<CustomRepositoryStatusResponse> GetCustomRepositoryStatusAsync(
      Guid project,
      string repository,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CustomRepositoryStatusResponse>(new HttpMethod("GET"), new Guid("de4c37b8-f55f-4c38-8f3f-a296cfb63121"), (object) new
      {
        project = project,
        repository = repository
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<CustomRepositoryBranchStatusResponse> GetCustomRepositoryBranchStatusAsync(
      string project,
      string repository,
      string branch,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CustomRepositoryBranchStatusResponse>(new HttpMethod("GET"), new Guid("5d9391b9-e2c3-4b87-9033-83c3aed4069e"), (object) new
      {
        project = project,
        repository = repository,
        branch = branch
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<CustomRepositoryBranchStatusResponse> GetCustomRepositoryBranchStatusAsync(
      Guid project,
      string repository,
      string branch,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CustomRepositoryBranchStatusResponse>(new HttpMethod("GET"), new Guid("5d9391b9-e2c3-4b87-9033-83c3aed4069e"), (object) new
      {
        project = project,
        repository = repository,
        branch = branch
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task<PackageSearchResponse> FetchPackageSearchResultsAsync(
      PackageSearchRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SearchHttpClient searchHttpClient = this;
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("f62ada48-eedc-4c8e-93f0-de870e4ecce0");
      HttpContent content = (HttpContent) new ObjectContent<PackageSearchRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      PackageSearchResponse packageSearchResponse1;
      using (HttpRequestMessage requestMessage = await searchHttpClient.CreateRequestMessageAsync(method, locationId, version: new ApiResourceVersion("7.2-preview.1"), content: content, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        PackageSearchResponse returnObject = new PackageSearchResponse();
        using (HttpResponseMessage response = await searchHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ActivityId = searchHttpClient.GetHeaderValue(response, "ActivityId");
          PackageSearchResponse packageSearchResponse = returnObject;
          packageSearchResponse.Content = await searchHttpClient.ReadContentAsAsync<PackageSearchResponseContent>(response, cancellationToken).ConfigureAwait(false);
          packageSearchResponse = (PackageSearchResponse) null;
        }
        packageSearchResponse1 = returnObject;
      }
      return packageSearchResponse1;
    }

    public virtual Task<RepositoryStatusResponse> GetRepositoryStatusAsync(
      string project,
      string repository,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<RepositoryStatusResponse>(new HttpMethod("GET"), new Guid("1f60303c-7261-4387-80f1-742a2ecf2964"), (object) new
      {
        project = project,
        repository = repository
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<RepositoryStatusResponse> GetRepositoryStatusAsync(
      Guid project,
      string repository,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<RepositoryStatusResponse>(new HttpMethod("GET"), new Guid("1f60303c-7261-4387-80f1-742a2ecf2964"), (object) new
      {
        project = project,
        repository = repository
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TfvcRepositoryStatusResponse> GetTfvcRepositoryStatusAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TfvcRepositoryStatusResponse>(new HttpMethod("GET"), new Guid("d5bf4e52-e0af-4626-8c50-8a80b18fa69f"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TfvcRepositoryStatusResponse> GetTfvcRepositoryStatusAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TfvcRepositoryStatusResponse>(new HttpMethod("GET"), new Guid("d5bf4e52-e0af-4626-8c50-8a80b18fa69f"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WikiSearchResponse> FetchWikiSearchResultsAsync(
      WikiSearchRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e90e7664-7049-4100-9a86-66b161d81080");
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiSearchRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WikiSearchResponse>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<WikiSearchResponse> FetchWikiSearchResultsAsync(
      WikiSearchRequest request,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e90e7664-7049-4100-9a86-66b161d81080");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiSearchRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WikiSearchResponse>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<WikiSearchResponse> FetchWikiSearchResultsAsync(
      WikiSearchRequest request,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e90e7664-7049-4100-9a86-66b161d81080");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiSearchRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WikiSearchResponse>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<WorkItemSearchResponse> FetchWorkItemSearchResultsAsync(
      WorkItemSearchRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("73b2c9e2-ff9e-4447-8cda-5f5b21ff7cae");
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemSearchRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemSearchResponse>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<WorkItemSearchResponse> FetchWorkItemSearchResultsAsync(
      WorkItemSearchRequest request,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("73b2c9e2-ff9e-4447-8cda-5f5b21ff7cae");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemSearchRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemSearchResponse>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<WorkItemSearchResponse> FetchWorkItemSearchResultsAsync(
      WorkItemSearchRequest request,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("73b2c9e2-ff9e-4447-8cda-5f5b21ff7cae");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemSearchRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemSearchResponse>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
