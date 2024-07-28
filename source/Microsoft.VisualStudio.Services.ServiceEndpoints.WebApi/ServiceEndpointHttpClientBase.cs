// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [ResourceArea("1814AB31-2F4F-4A9F-8761-F4D77DC5A5D7")]
  public abstract class ServiceEndpointHttpClientBase : VssHttpClientBase
  {
    public ServiceEndpointHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ServiceEndpointHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ServiceEndpointHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ServiceEndpointHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ServiceEndpointHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<AzureManagementGroupQueryResult> GetAzureManagementGroupsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<AzureManagementGroupQueryResult>(new HttpMethod("GET"), new Guid("9acb984c-4f88-4e13-9691-2e688dddc047"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<AzureSubscriptionQueryResult> GetAzureSubscriptionsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<AzureSubscriptionQueryResult>(new HttpMethod("GET"), new Guid("18e8f65d-4e19-4a01-a621-cf0f2d938108"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ServiceEndpointRequestResult> ExecuteServiceEndpointRequestAsync(
      string project,
      string endpointId,
      ServiceEndpointRequest serviceEndpointRequest,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("cc63bb57-2a5f-4a7a-b79c-c142d308657e");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ServiceEndpointRequest>(serviceEndpointRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (endpointId), endpointId);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ServiceEndpointRequestResult>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<ServiceEndpointRequestResult> ExecuteServiceEndpointRequestAsync(
      Guid project,
      string endpointId,
      ServiceEndpointRequest serviceEndpointRequest,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("cc63bb57-2a5f-4a7a-b79c-c142d308657e");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ServiceEndpointRequest>(serviceEndpointRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (endpointId), endpointId);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ServiceEndpointRequestResult>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    [Obsolete("Use ExecuteServiceEndpointRequest API Instead")]
    public virtual Task<List<string>> QueryServiceEndpointAsync(
      string project,
      DataSourceBinding binding,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("cc63bb57-2a5f-4a7a-b79c-c142d308657e");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<DataSourceBinding>(binding, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<string>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [Obsolete("Use ExecuteServiceEndpointRequest API Instead")]
    public virtual Task<List<string>> QueryServiceEndpointAsync(
      Guid project,
      DataSourceBinding binding,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("cc63bb57-2a5f-4a7a-b79c-c142d308657e");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<DataSourceBinding>(binding, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<string>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<ServiceEndpoint> CreateServiceEndpointAsync(
      ServiceEndpoint endpoint,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("14e48fdc-2c8b-41ce-a0c3-e26f6cc55bd0");
      HttpContent httpContent = (HttpContent) new ObjectContent<ServiceEndpoint>(endpoint, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ServiceEndpoint>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteServiceEndpointAsync(
      Guid endpointId,
      IEnumerable<string> projectIds,
      bool? deep = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ServiceEndpointHttpClientBase endpointHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("14e48fdc-2c8b-41ce-a0c3-e26f6cc55bd0");
      object routeValues = (object) new
      {
        endpointId = endpointId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (projectIds != null)
        str = string.Join(",", projectIds);
      keyValuePairList.Add(nameof (projectIds), str);
      if (deep.HasValue)
        keyValuePairList.Add(nameof (deep), deep.Value.ToString());
      using (await endpointHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 4), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task ShareServiceEndpointAsync(
      Guid endpointId,
      IEnumerable<ServiceEndpointProjectReference> endpointProjectReferences,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ServiceEndpointHttpClientBase endpointHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("14e48fdc-2c8b-41ce-a0c3-e26f6cc55bd0");
      object obj1 = (object) new{ endpointId = endpointId };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<ServiceEndpointProjectReference>>(endpointProjectReferences, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      ServiceEndpointHttpClientBase endpointHttpClientBase2 = endpointHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await endpointHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<ServiceEndpoint> UpdateServiceEndpointAsync(
      Guid endpointId,
      ServiceEndpoint endpoint,
      string operation = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("14e48fdc-2c8b-41ce-a0c3-e26f6cc55bd0");
      object obj1 = (object) new{ endpointId = endpointId };
      HttpContent httpContent = (HttpContent) new ObjectContent<ServiceEndpoint>(endpoint, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (operation != null)
        collection.Add(nameof (operation), operation);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ServiceEndpoint>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<List<ServiceEndpoint>> UpdateServiceEndpointsAsync(
      IEnumerable<ServiceEndpoint> endpoints,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("14e48fdc-2c8b-41ce-a0c3-e26f6cc55bd0");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<ServiceEndpoint>>(endpoints, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<ServiceEndpoint>>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<ServiceEndpoint> GetServiceEndpointDetailsAsync(
      string project,
      Guid endpointId,
      ServiceEndpointActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e85f1c62-adfc-4b74-b618-11a150fb195e");
      object routeValues = (object) new
      {
        project = project,
        endpointId = endpointId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<ServiceEndpoint>(method, locationId, routeValues, new ApiResourceVersion(7.2, 4), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ServiceEndpoint> GetServiceEndpointDetailsAsync(
      Guid project,
      Guid endpointId,
      ServiceEndpointActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e85f1c62-adfc-4b74-b618-11a150fb195e");
      object routeValues = (object) new
      {
        project = project,
        endpointId = endpointId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<ServiceEndpoint>(method, locationId, routeValues, new ApiResourceVersion(7.2, 4), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<ServiceEndpoint>> GetServiceEndpointsAsync(
      string project,
      string type = null,
      IEnumerable<string> authSchemes = null,
      IEnumerable<Guid> endpointIds = null,
      string owner = null,
      bool? includeFailed = null,
      bool? includeDetails = null,
      ServiceEndpointActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e85f1c62-adfc-4b74-b618-11a150fb195e");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (type != null)
        keyValuePairList.Add(nameof (type), type);
      if (authSchemes != null && authSchemes.Any<string>())
        keyValuePairList.Add(nameof (authSchemes), string.Join(",", authSchemes));
      if (endpointIds != null && endpointIds.Any<Guid>())
        keyValuePairList.Add(nameof (endpointIds), string.Join<Guid>(",", endpointIds));
      if (owner != null)
        keyValuePairList.Add(nameof (owner), owner);
      bool flag;
      if (includeFailed.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeFailed.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeFailed), str);
      }
      if (includeDetails.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDetails.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDetails), str);
      }
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<List<ServiceEndpoint>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 4), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<ServiceEndpoint>> GetServiceEndpointsAsync(
      Guid project,
      string type = null,
      IEnumerable<string> authSchemes = null,
      IEnumerable<Guid> endpointIds = null,
      string owner = null,
      bool? includeFailed = null,
      bool? includeDetails = null,
      ServiceEndpointActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e85f1c62-adfc-4b74-b618-11a150fb195e");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (type != null)
        keyValuePairList.Add(nameof (type), type);
      if (authSchemes != null && authSchemes.Any<string>())
        keyValuePairList.Add(nameof (authSchemes), string.Join(",", authSchemes));
      if (endpointIds != null && endpointIds.Any<Guid>())
        keyValuePairList.Add(nameof (endpointIds), string.Join<Guid>(",", endpointIds));
      if (owner != null)
        keyValuePairList.Add(nameof (owner), owner);
      bool flag;
      if (includeFailed.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeFailed.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeFailed), str);
      }
      if (includeDetails.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDetails.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDetails), str);
      }
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<List<ServiceEndpoint>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 4), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<ServiceEndpoint>> GetServiceEndpointsByNamesAsync(
      string project,
      IEnumerable<string> endpointNames,
      string type = null,
      IEnumerable<string> authSchemes = null,
      string owner = null,
      bool? includeFailed = null,
      bool? includeDetails = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e85f1c62-adfc-4b74-b618-11a150fb195e");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str1 = (string) null;
      if (endpointNames != null)
        str1 = string.Join(",", endpointNames);
      keyValuePairList.Add(nameof (endpointNames), str1);
      if (type != null)
        keyValuePairList.Add(nameof (type), type);
      if (authSchemes != null && authSchemes.Any<string>())
        keyValuePairList.Add(nameof (authSchemes), string.Join(",", authSchemes));
      if (owner != null)
        keyValuePairList.Add(nameof (owner), owner);
      bool flag;
      if (includeFailed.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeFailed.Value;
        string str2 = flag.ToString();
        collection.Add(nameof (includeFailed), str2);
      }
      if (includeDetails.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDetails.Value;
        string str3 = flag.ToString();
        collection.Add(nameof (includeDetails), str3);
      }
      return this.SendAsync<List<ServiceEndpoint>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 4), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<ServiceEndpoint>> GetServiceEndpointsByNamesAsync(
      Guid project,
      IEnumerable<string> endpointNames,
      string type = null,
      IEnumerable<string> authSchemes = null,
      string owner = null,
      bool? includeFailed = null,
      bool? includeDetails = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e85f1c62-adfc-4b74-b618-11a150fb195e");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str1 = (string) null;
      if (endpointNames != null)
        str1 = string.Join(",", endpointNames);
      keyValuePairList.Add(nameof (endpointNames), str1);
      if (type != null)
        keyValuePairList.Add(nameof (type), type);
      if (authSchemes != null && authSchemes.Any<string>())
        keyValuePairList.Add(nameof (authSchemes), string.Join(",", authSchemes));
      if (owner != null)
        keyValuePairList.Add(nameof (owner), owner);
      bool flag;
      if (includeFailed.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeFailed.Value;
        string str2 = flag.ToString();
        collection.Add(nameof (includeFailed), str2);
      }
      if (includeDetails.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDetails.Value;
        string str3 = flag.ToString();
        collection.Add(nameof (includeDetails), str3);
      }
      return this.SendAsync<List<ServiceEndpoint>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 4), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<ServiceEndpoint>> GetServiceEndpointsWithRefreshedAuthenticationAsync(
      string project,
      IEnumerable<Guid> endpointIds,
      IEnumerable<RefreshAuthenticationParameters> refreshAuthenticationParameters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e85f1c62-adfc-4b74-b618-11a150fb195e");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<RefreshAuthenticationParameters>>(refreshAuthenticationParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (endpointIds != null)
        str = string.Join<Guid>(",", endpointIds);
      collection.Add(nameof (endpointIds), str);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<ServiceEndpoint>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<List<ServiceEndpoint>> GetServiceEndpointsWithRefreshedAuthenticationAsync(
      Guid project,
      IEnumerable<Guid> endpointIds,
      IEnumerable<RefreshAuthenticationParameters> refreshAuthenticationParameters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e85f1c62-adfc-4b74-b618-11a150fb195e");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<RefreshAuthenticationParameters>>(refreshAuthenticationParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (endpointIds != null)
        str = string.Join<Guid>(",", endpointIds);
      collection.Add(nameof (endpointIds), str);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<ServiceEndpoint>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<PagedList<ServiceEndpointExecutionRecord>> GetServiceEndpointExecutionRecordsAsync(
      string project,
      Guid endpointId,
      int? top = null,
      long? continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("10a16738-9299-4cd1-9a81-fd23ad6200d0");
      object routeValues = (object) new
      {
        project = project,
        endpointId = endpointId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (top.HasValue)
        keyValuePairList.Add(nameof (top), top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken.HasValue)
        keyValuePairList.Add(nameof (continuationToken), continuationToken.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<PagedList<ServiceEndpointExecutionRecord>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<ServiceEndpointExecutionRecord>> GetServiceEndpointExecutionRecordsAsync(
      Guid project,
      Guid endpointId,
      int? top = null,
      long? continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("10a16738-9299-4cd1-9a81-fd23ad6200d0");
      object routeValues = (object) new
      {
        project = project,
        endpointId = endpointId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (top.HasValue)
        keyValuePairList.Add(nameof (top), top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken.HasValue)
        keyValuePairList.Add(nameof (continuationToken), continuationToken.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<PagedList<ServiceEndpointExecutionRecord>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<ServiceEndpointExecutionRecord>> AddServiceEndpointExecutionRecordsAsync(
      string project,
      ServiceEndpointExecutionRecordsInput input,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("55b9ed4b-5404-41b1-b9d2-7ed757d02bb0");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ServiceEndpointExecutionRecordsInput>(input, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<ServiceEndpointExecutionRecord>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<ServiceEndpointExecutionRecord>> AddServiceEndpointExecutionRecordsAsync(
      Guid project,
      ServiceEndpointExecutionRecordsInput input,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("55b9ed4b-5404-41b1-b9d2-7ed757d02bb0");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ServiceEndpointExecutionRecordsInput>(input, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<ServiceEndpointExecutionRecord>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<OAuthConfiguration> CreateOAuthConfigurationAsync(
      OAuthConfigurationParams configurationParams,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("702edb4e-3952-43fe-a4eb-288938f3ba35");
      HttpContent httpContent = (HttpContent) new ObjectContent<OAuthConfigurationParams>(configurationParams, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<OAuthConfiguration>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<OAuthConfiguration> DeleteOAuthConfigurationAsync(
      Guid configurationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<OAuthConfiguration>(new HttpMethod("DELETE"), new Guid("702edb4e-3952-43fe-a4eb-288938f3ba35"), (object) new
      {
        configurationId = configurationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<OAuthConfiguration> GetOAuthConfigurationAsync(
      Guid configurationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<OAuthConfiguration>(new HttpMethod("GET"), new Guid("702edb4e-3952-43fe-a4eb-288938f3ba35"), (object) new
      {
        configurationId = configurationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<OAuthConfiguration>> GetOAuthConfigurationsAsync(
      string endpointType = null,
      OAuthConfigurationActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("702edb4e-3952-43fe-a4eb-288938f3ba35");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (endpointType != null)
        keyValuePairList.Add(nameof (endpointType), endpointType);
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<List<OAuthConfiguration>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<OAuthConfiguration> UpdateOAuthConfigurationAsync(
      Guid configurationId,
      OAuthConfigurationParams configurationParams,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("702edb4e-3952-43fe-a4eb-288938f3ba35");
      object obj1 = (object) new
      {
        configurationId = configurationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<OAuthConfigurationParams>(configurationParams, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<OAuthConfiguration>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<ProjectReference>> QuerySharedProjectsAsync(
      Guid endpointId,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("86e77201-c1f7-46c9-8672-9dfc2f6f568a");
      object routeValues = (object) new
      {
        endpointId = endpointId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (project), project);
      return this.SendAsync<List<ProjectReference>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task ShareEndpointWithProjectAsync(
      Guid endpointId,
      string fromProject,
      string withProject,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ServiceEndpointHttpClientBase endpointHttpClientBase = this;
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("86e77201-c1f7-46c9-8672-9dfc2f6f568a");
      object routeValues = (object) new
      {
        endpointId = endpointId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (fromProject), fromProject);
      keyValuePairList.Add(nameof (withProject), withProject);
      using (await endpointHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<List<ServiceEndpointType>> GetServiceEndpointTypesAsync(
      string type = null,
      string scheme = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5a7938a4-655e-486c-b562-b78c54a7e87b");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (type != null)
        keyValuePairList.Add(nameof (type), type);
      if (scheme != null)
        keyValuePairList.Add(nameof (scheme), scheme);
      return this.SendAsync<List<ServiceEndpointType>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<string> CreateAadOAuthRequestAsync(
      string tenantId,
      string redirectUri,
      AadLoginPromptOption? promptOption = null,
      string completeCallbackPayload = null,
      bool? completeCallbackByAuthCode = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("47911d38-53e1-467a-8c32-d871599d5498");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (tenantId), tenantId);
      keyValuePairList.Add(nameof (redirectUri), redirectUri);
      if (promptOption.HasValue)
        keyValuePairList.Add(nameof (promptOption), promptOption.Value.ToString());
      if (completeCallbackPayload != null)
        keyValuePairList.Add(nameof (completeCallbackPayload), completeCallbackPayload);
      if (completeCallbackByAuthCode.HasValue)
        keyValuePairList.Add(nameof (completeCallbackByAuthCode), completeCallbackByAuthCode.Value.ToString());
      return this.SendAsync<string>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<string> GetVstsAadTenantIdAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<string>(new HttpMethod("GET"), new Guid("47911d38-53e1-467a-8c32-d871599d5498"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }
  }
}
