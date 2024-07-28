// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchHttpClient
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5D4CB2D3-3C08-46C7-B9C5-51E638F57F9E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.Legacy.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Legacy
{
  [ResourceArea("EA48A0A1-269C-42D8-B8AD-DDC8FCDCF578")]
  public class SearchHttpClient : VssHttpClientBase
  {
    private static Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>()
    {
      {
        "SearchException",
        typeof (SearchException)
      },
      {
        "InvalidQueryException",
        typeof (InvalidQueryException)
      }
    };

    public SearchHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
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
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
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

    public Task<RepositoryIndexingProperties> GetLastIndexedChangeId(
      string projectName,
      string repositoryName,
      string branchName,
      object userState = null)
    {
      object obj1 = (object) new
      {
        projectName = projectName,
        repositoryName = repositoryName,
        branchName = branchName
      };
      HttpMethod get = HttpMethod.Get;
      Guid changeIdLocationId = SearchConstants.GetLastIndexedChangeIdLocationId;
      object obj2 = userState;
      object routeValues = obj1;
      object userState1 = obj2;
      CancellationToken cancellationToken = new CancellationToken();
      return this.SendAsync<RepositoryIndexingProperties>(get, changeIdLocationId, routeValues, userState: userState1, cancellationToken: cancellationToken);
    }

    public Task<bool> RegisterCustomTenant(CustomTenant tenant, object userState = null) => this.SendAsync<bool>(HttpMethod.Post, SearchConstants.CustomTenantLocationId, content: (HttpContent) new ObjectContent<CustomTenant>(tenant, this.Formatter), userState: userState);

    public Task<CustomRepositoryHealthResponse> GetCustomRepositoryHealth(
      string projectName,
      string repositoryName,
      string branchName,
      int numberOfResults,
      long continuationToken = 0,
      object userState = null)
    {
      object routeValues = (object) new
      {
        projectName = projectName,
        repositoryName = repositoryName,
        branchName = branchName,
        numberOfResults = numberOfResults,
        continuationToken = continuationToken
      };
      return this.SendAsync<CustomRepositoryHealthResponse>(HttpMethod.Get, SearchConstants.CustomRepositoryGetRepositoryHealthLocationId, routeValues, userState: userState);
    }

    public Task<IEnumerable<string>> GetCustomTenantCollectionNames(
      string tenantName,
      object userState = null)
    {
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (tenantName), tenantName);
      List<KeyValuePair<string, string>> queryParameters = collection;
      return this.SendAsync<IEnumerable<string>>(HttpMethod.Get, SearchConstants.CustomTenantGetCollectionNamesLocationId, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState);
    }

    public Task<IEnumerable<string>> GetCustomProjects(object userState = null) => this.SendAsync<IEnumerable<string>>(HttpMethod.Get, SearchConstants.CustomProjectLocationId, userState: userState);

    public Task<CustomRepository> RegisterCustomRepository(
      CustomRepository repository,
      object userState = null)
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid repositoryLocationId = SearchConstants.CustomRepositoryRegisterRepositoryLocationId;
      HttpContent httpContent = (HttpContent) new ObjectContent<CustomRepository>(repository, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      HttpMethod method = httpMethod;
      Guid locationId = repositoryLocationId;
      ApiResourceVersion version = new ApiResourceVersion(6.0, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      object obj = userState;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken = new CancellationToken();
      return this.SendAsync<CustomRepository>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken);
    }

    public Task<IEnumerable<string>> GetCustomRepositories(string projectName, object userState = null)
    {
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (projectName), projectName);
      List<KeyValuePair<string, string>> queryParameters = collection;
      return this.SendAsync<IEnumerable<string>>(HttpMethod.Get, SearchConstants.CustomRepositoryGetRepositoriesLocationId, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState);
    }

    public Task<CustomRepository> GetCustomRepository(
      string projectName,
      string repositoryName,
      object userState = null)
    {
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (projectName), projectName);
      collection.Add(nameof (repositoryName), repositoryName);
      List<KeyValuePair<string, string>> queryParameters = collection;
      return this.SendAsync<CustomRepository>(HttpMethod.Get, SearchConstants.CustomRepositoryGetRepositoryLocationId, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState);
    }

    public Task<BulkCodeIndexResponse> BulkCodeIndexAsync(
      BulkCodeIndexRequest request,
      object userState = null)
    {
      return this.SendAsync<BulkCodeIndexResponse>(HttpMethod.Post, SearchConstants.CustomCodeBulkIndexLocationId, content: (HttpContent) new ObjectContent<BulkCodeIndexRequest>(request, this.Formatter), userState: userState);
    }

    public Task<string> GetFileContent(
      string projectName,
      string repositoryName,
      string branchName,
      string filePath,
      object userState = null)
    {
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (projectName), projectName);
      collection.Add(nameof (repositoryName), repositoryName);
      collection.Add(nameof (branchName), branchName);
      collection.Add(nameof (filePath), filePath);
      List<KeyValuePair<string, string>> queryParameters = collection;
      return this.SendAsync<string>(HttpMethod.Get, SearchConstants.CustomCodeGetFileContentLocationId, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState);
    }

    public Task<FilesMetadataResponse> PostFilesMetadata(
      string projectName,
      string repositoryName,
      string branchName,
      FilesMetadataRequest request,
      object userState = null)
    {
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (projectName), projectName);
      collection.Add(nameof (repositoryName), repositoryName);
      collection.Add(nameof (branchName), branchName);
      List<KeyValuePair<string, string>> queryParameters = collection;
      return this.SendAsync<FilesMetadataResponse>(HttpMethod.Post, SearchConstants.CustomCodeFilesMetadataLocationId, content: (HttpContent) new ObjectContent<FilesMetadataRequest>(request, this.Formatter), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState);
    }

    public Task<OperationStatus> GetOperationStatus(
      string projectName,
      string repositoryName,
      string trackingId,
      object userState = null)
    {
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (projectName), projectName);
      collection.Add(nameof (repositoryName), repositoryName);
      collection.Add(nameof (trackingId), trackingId);
      List<KeyValuePair<string, string>> queryParameters = collection;
      return this.SendAsync<OperationStatus>(HttpMethod.Get, SearchConstants.CustomCodeGetOperationStatusLocationId, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState);
    }

    public Task<CodeQueryResponse> CreateCodeQueryResultsAsync(SearchQuery query, object userState = null) => this.SendAsync<CodeQueryResponse>(HttpMethod.Post, SearchConstants.CodeQueryResultsLocationId, content: (HttpContent) new ObjectContent<SearchQuery>(query, this.Formatter), userState: userState);

    public Task<CodeQueryResponse> CreateCodeQueryResultsAsync(
      SearchQuery query,
      Guid projectId,
      object userState = null)
    {
      object routeValues = (object) new
      {
        project = projectId
      };
      return this.SendAsync<CodeQueryResponse>(HttpMethod.Post, SearchConstants.CodeQueryResultsLocationId, routeValues, content: (HttpContent) new ObjectContent<SearchQuery>(query, this.Formatter), userState: userState);
    }

    public Task<WikiQueryResponse> CreateWikiQueryResultsAsync(
      WikiSearchQuery query,
      object userState = null)
    {
      return this.SendAsync<WikiQueryResponse>(HttpMethod.Post, SearchConstants.WikiQueryResultsLocationId, content: (HttpContent) new ObjectContent<WikiSearchQuery>(query, this.Formatter), userState: userState);
    }

    public Task<CodeQueryResponse> CreateAdvancedCodeQueryResultsAsync(
      SearchQuery query,
      object userState = null)
    {
      return this.SendAsync<CodeQueryResponse>(HttpMethod.Post, SearchConstants.CodeAdvancedQueryResultsLocationId, content: (HttpContent) new ObjectContent<SearchQuery>(query, this.Formatter), userState: userState);
    }

    public Task<CodeQueryResponse> CreateAdvancedCodeQueryResultsAsync(
      SearchQuery query,
      Guid projectId,
      object userState = null)
    {
      object routeValues = (object) new
      {
        project = projectId
      };
      return this.SendAsync<CodeQueryResponse>(HttpMethod.Post, SearchConstants.CodeAdvancedQueryResultsLocationId, routeValues, content: (HttpContent) new ObjectContent<SearchQuery>(query, this.Formatter), userState: userState);
    }

    public Task<CodeQueryResponse> CreateTenantCodeQueryResultsAsync(
      SearchQuery query,
      object userState = null)
    {
      return this.SendAsync<CodeQueryResponse>(HttpMethod.Post, SearchConstants.TenantCodeQueryResultsLocationId, content: (HttpContent) new ObjectContent<SearchQuery>(query, this.Formatter), userState: userState);
    }

    public Task<WorkItemSearchResponse> CreateWorkItemQueryResultsAsync(
      WorkItemSearchRequest request,
      object userState = null)
    {
      return this.SendAsync<WorkItemSearchResponse>(HttpMethod.Post, SearchConstants.WorkItemQueryResultsLocationId, content: (HttpContent) new ObjectContent<WorkItemSearchRequest>(request, this.Formatter), userState: userState);
    }

    public Task<WorkItemSearchResponse> CreateWorkItemQueryResultsAsync(
      WorkItemSearchRequest request,
      Guid projectId,
      object userState = null)
    {
      object routeValues = (object) new
      {
        project = projectId
      };
      return this.SendAsync<WorkItemSearchResponse>(HttpMethod.Post, SearchConstants.WorkItemQueryResultsLocationId, routeValues, content: (HttpContent) new ObjectContent<WorkItemSearchRequest>(request, this.Formatter), userState: userState);
    }

    public Task<IEnumerable<GitRepositoryData>> GetUserAccessibleRepositoriesAsync(object userState = null) => this.SendAsync<IEnumerable<GitRepositoryData>>(HttpMethod.Get, SearchConstants.UserAccessibleRepositoriesLocationId, userState: userState);

    public Task<IEnumerable<GitRepositoryData>> GetUserAccessibleRepositoriesByProjectAsync(
      string projectIdentifier,
      object userState = null)
    {
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (projectIdentifier), projectIdentifier);
      List<KeyValuePair<string, string>> queryParameters = collection;
      return this.SendAsync<IEnumerable<GitRepositoryData>>(HttpMethod.Get, SearchConstants.UserAccessibleRepositoriesLocationId, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState);
    }

    public Task<Guid> CreateOptInRequestForSearchAsync(OptInRequest request, object userState = null) => this.SendAsync<Guid>(HttpMethod.Post, SearchConstants.OptInAccountForSearchLocationId, content: (HttpContent) new ObjectContent<OptInRequest>(request, this.Formatter), userState: userState);

    public Task<OptInRequestStatus> GetOptInRequestStatusAsync(Guid hostId, object userState = null)
    {
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (hostId), hostId.ToString("D"));
      List<KeyValuePair<string, string>> queryParameters = collection;
      return this.SendAsync<OptInRequestStatus>(HttpMethod.Get, SearchConstants.OptInAccountForSearchLocationId, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState);
    }

    public virtual Task<CountResponse> ResultsCountAsync(
      CountRequest query,
      string entityType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d26ef242-f129-40ed-b9db-b31a792065e0");
      HttpContent httpContent = (HttpContent) new ObjectContent<CountRequest>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(entityType))
        collection.Add(nameof (entityType), entityType);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion("5.0-preview.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CountResponse>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<CountResponse> ResultsCountAsync(
      CountRequest query,
      Guid projectId,
      string entityType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      object obj1 = (object) new{ project = projectId };
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d26ef242-f129-40ed-b9db-b31a792065e0");
      HttpContent httpContent = (HttpContent) new ObjectContent<CountRequest>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(entityType))
        collection.Add(nameof (entityType), entityType);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion apiResourceVersion = new ApiResourceVersion("5.0-preview.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object routeValues = obj1;
      ApiResourceVersion version = apiResourceVersion;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CountResponse>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) SearchHttpClient.s_translatedExceptions;
  }
}
