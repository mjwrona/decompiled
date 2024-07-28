// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ConnectedService.Server.ConnectedServiceHttpClient
// Assembly: Microsoft.TeamFoundation.ConnectedService.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0D040FD2-0366-4FA8-B2F4-4380C0B19F54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ConnectedService.WebApi.dll

using Microsoft.TeamFoundation.ConnectedService.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.ConnectedService.Server
{
  [ResourceArea("E921B68F-92D6-44D4-AA88-19C84BE1C4C7")]
  public class ConnectedServiceHttpClient : VssHttpClientBase
  {
    public ConnectedServiceHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ConnectedServiceHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ConnectedServiceHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ConnectedServiceHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ConnectedServiceHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<AuthRequest> CreateAuthRequestAsync(
      AuthRequest authRequest,
      string project,
      string providerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e921b68f-92d6-44d4-aa88-19c84be1c4c7");
      object obj1 = (object) new
      {
        project = project,
        providerId = providerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<AuthRequest>(authRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<AuthRequest>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<AuthRequest> CreateAuthRequestAsync(
      AuthRequest authRequest,
      Guid project,
      string providerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e921b68f-92d6-44d4-aa88-19c84be1c4c7");
      object obj1 = (object) new
      {
        project = project,
        providerId = providerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<AuthRequest>(authRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<AuthRequest>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<AuthRequest> CreateAuthRequestAsync(
      AuthRequest authRequest,
      string project,
      string providerId,
      Guid configurationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e921b68f-92d6-44d4-aa88-19c84be1c4c7");
      object obj1 = (object) new
      {
        project = project,
        providerId = providerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<AuthRequest>(authRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (configurationId), configurationId.ToString());
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
      return this.SendAsync<AuthRequest>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<AuthRequest> CreateAuthRequestAsync(
      AuthRequest authRequest,
      Guid project,
      string providerId,
      Guid configurationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e921b68f-92d6-44d4-aa88-19c84be1c4c7");
      object obj1 = (object) new
      {
        project = project,
        providerId = providerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<AuthRequest>(authRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (configurationId), configurationId.ToString());
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
      return this.SendAsync<AuthRequest>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<AuthRequest> CreateAuthRequestAsync(
      AuthRequest authRequest,
      string project,
      string providerId,
      Guid configurationId,
      string scope,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e921b68f-92d6-44d4-aa88-19c84be1c4c7");
      object obj1 = (object) new
      {
        project = project,
        providerId = providerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<AuthRequest>(authRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (configurationId), configurationId.ToString());
      collection.Add(nameof (scope), scope);
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
      return this.SendAsync<AuthRequest>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<AuthRequest> CreateAuthRequestAsync(
      AuthRequest authRequest,
      Guid project,
      string providerId,
      Guid configurationId,
      string scope,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e921b68f-92d6-44d4-aa88-19c84be1c4c7");
      object obj1 = (object) new
      {
        project = project,
        providerId = providerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<AuthRequest>(authRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (configurationId), configurationId.ToString());
      collection.Add(nameof (scope), scope);
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
      return this.SendAsync<AuthRequest>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<AuthRequest> CreateAuthRequestAsync(
      AuthRequest authRequest,
      string project,
      string providerId,
      Guid configurationId,
      string scope,
      string callbackQueryParams,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e921b68f-92d6-44d4-aa88-19c84be1c4c7");
      object obj1 = (object) new
      {
        project = project,
        providerId = providerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<AuthRequest>(authRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (configurationId), configurationId.ToString());
      collection.Add(nameof (scope), scope);
      collection.Add(nameof (callbackQueryParams), callbackQueryParams);
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
      return this.SendAsync<AuthRequest>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<AuthRequest> CreateAuthRequestAsync(
      AuthRequest authRequest,
      Guid project,
      string providerId,
      Guid configurationId,
      string scope,
      string callbackQueryParams,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e921b68f-92d6-44d4-aa88-19c84be1c4c7");
      object obj1 = (object) new
      {
        project = project,
        providerId = providerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<AuthRequest>(authRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (configurationId), configurationId.ToString());
      collection.Add(nameof (scope), scope);
      collection.Add(nameof (callbackQueryParams), callbackQueryParams);
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
      return this.SendAsync<AuthRequest>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<AuthRequest> CreateAuthRequestAsync(
      AuthRequest authRequest,
      string project,
      string providerId,
      Guid configurationId,
      string scope,
      string callbackQueryParams,
      string endpointType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e921b68f-92d6-44d4-aa88-19c84be1c4c7");
      object obj1 = (object) new
      {
        project = project,
        providerId = providerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<AuthRequest>(authRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (configurationId), configurationId.ToString());
      collection.Add(nameof (scope), scope);
      collection.Add(nameof (callbackQueryParams), callbackQueryParams);
      collection.Add(nameof (endpointType), endpointType);
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
      return this.SendAsync<AuthRequest>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<AuthRequest> CreateAuthRequestAsync(
      AuthRequest authRequest,
      Guid project,
      string providerId,
      Guid configurationId,
      string scope,
      string callbackQueryParams,
      string endpointType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e921b68f-92d6-44d4-aa88-19c84be1c4c7");
      object obj1 = (object) new
      {
        project = project,
        providerId = providerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<AuthRequest>(authRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (configurationId), configurationId.ToString());
      collection.Add(nameof (scope), scope);
      collection.Add(nameof (callbackQueryParams), callbackQueryParams);
      collection.Add(nameof (endpointType), endpointType);
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
      return this.SendAsync<AuthRequest>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<List<Installation>> GetAppInstallationsAsync(
      string project,
      string providerId,
      string oauthTokenKey,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("e921b68f-92d6-44d4-aa88-19c84be1c4c7");
      object routeValues = (object) new
      {
        project = project,
        providerId = providerId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (oauthTokenKey), oauthTokenKey);
      return this.SendAsync<List<Installation>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<Installation>> GetAppInstallationsAsync(
      Guid project,
      string providerId,
      string oauthTokenKey,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("e921b68f-92d6-44d4-aa88-19c84be1c4c7");
      object routeValues = (object) new
      {
        project = project,
        providerId = providerId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (oauthTokenKey), oauthTokenKey);
      return this.SendAsync<List<Installation>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
