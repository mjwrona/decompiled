// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.DelegatedAuthorizationHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi
{
  [ResourceArea("A0848FA1-3593-4AEC-949C-694C73F4C4CE")]
  public class DelegatedAuthorizationHttpClient : VssHttpClientBase
  {
    public DelegatedAuthorizationHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public DelegatedAuthorizationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public DelegatedAuthorizationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public DelegatedAuthorizationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public DelegatedAuthorizationHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<AuthorizationDecision> AuthorizeAsync(
      Guid userId,
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("efbf6e0c-1150-43fd-b869-7e2b04fc0d09");
      object routeValues = (object) new{ userId = userId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (responseType), responseType.ToString());
      keyValuePairList.Add(nameof (clientId), clientId.ToString());
      string str = (string) null;
      if (redirectUri != (Uri) null)
        str = redirectUri.ToString();
      keyValuePairList.Add(nameof (redirectUri), str);
      keyValuePairList.Add(nameof (scopes), scopes);
      return this.SendAsync<AuthorizationDecision>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<AuthorizationDecision> AuthorizeAsync(
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("efbf6e0c-1150-43fd-b869-7e2b04fc0d09");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (responseType), responseType.ToString());
      keyValuePairList.Add(nameof (clientId), clientId.ToString());
      string str = (string) null;
      if (redirectUri != (Uri) null)
        str = redirectUri.ToString();
      keyValuePairList.Add(nameof (redirectUri), str);
      keyValuePairList.Add(nameof (scopes), scopes);
      return this.SendAsync<AuthorizationDecision>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<AuthorizationDetails>> GetAuthorizationsAsync(
      Guid? userId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<AuthorizationDetails>>(new HttpMethod("GET"), new Guid("efbf6e0c-1150-43fd-b869-7e2b04fc0d09"), (object) new
      {
        userId = userId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<AuthorizationDescription> InitiateAuthorizationAsync(
      Guid userId,
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("efbf6e0c-1150-43fd-b869-7e2b04fc0d09");
      object routeValues = (object) new{ userId = userId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (responseType), responseType.ToString());
      keyValuePairList.Add(nameof (clientId), clientId.ToString());
      string str = (string) null;
      if (redirectUri != (Uri) null)
        str = redirectUri.ToString();
      keyValuePairList.Add(nameof (redirectUri), str);
      keyValuePairList.Add(nameof (scopes), scopes);
      return this.SendAsync<AuthorizationDescription>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<AuthorizationDescription> InitiateAuthorizationAsync(
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("efbf6e0c-1150-43fd-b869-7e2b04fc0d09");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (responseType), responseType.ToString());
      keyValuePairList.Add(nameof (clientId), clientId.ToString());
      string str = (string) null;
      if (redirectUri != (Uri) null)
        str = redirectUri.ToString();
      keyValuePairList.Add(nameof (redirectUri), str);
      keyValuePairList.Add(nameof (scopes), scopes);
      return this.SendAsync<AuthorizationDescription>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public async Task RevokeAuthorizationAsync(
      Guid authorizationId,
      Guid? userId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DelegatedAuthorizationHttpClient authorizationHttpClient = this;
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("efbf6e0c-1150-43fd-b869-7e2b04fc0d09");
      object routeValues = (object) new{ userId = userId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (authorizationId), authorizationId.ToString());
      using (await authorizationHttpClient.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<HostAuthorizationDecision> AuthorizeHostAsync(
      Guid clientId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("7372fdd9-238c-467c-b0f2-995f4bfe0d94");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (clientId), clientId.ToString());
      return this.SendAsync<HostAuthorizationDecision>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<HostAuthorization>> GetHostAuthorizationsAsync(
      Guid hostId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7372fdd9-238c-467c-b0f2-995f4bfe0d94");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (hostId), hostId.ToString());
      return this.SendAsync<List<HostAuthorization>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public async Task RevokeHostAuthorizationAsync(
      Guid clientId,
      Guid? hostId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DelegatedAuthorizationHttpClient authorizationHttpClient = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("7372fdd9-238c-467c-b0f2-995f4bfe0d94");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (clientId), clientId.ToString());
      if (hostId.HasValue)
        keyValuePairList.Add(nameof (hostId), hostId.Value.ToString());
      using (await authorizationHttpClient.SendAsync(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<Registration> CreateRegistrationAsync(
      Registration registration,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("909cd090-3005-480d-a1b4-220b76cb0afe");
      HttpContent httpContent = (HttpContent) new ObjectContent<Registration>(registration, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Registration>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<Registration> CreateRegistrationAsync(
      Registration registration,
      bool includeSecret,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("909cd090-3005-480d-a1b4-220b76cb0afe");
      HttpContent httpContent = (HttpContent) new ObjectContent<Registration>(registration, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (includeSecret), includeSecret.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Registration>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public async Task DeleteRegistrationAsync(
      Guid registrationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("909cd090-3005-480d-a1b4-220b76cb0afe"), (object) new
      {
        registrationId = registrationId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<Registration> GetRegistrationAsync(
      Guid registrationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Registration>(new HttpMethod("GET"), new Guid("909cd090-3005-480d-a1b4-220b76cb0afe"), (object) new
      {
        registrationId = registrationId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Registration> GetRegistrationAsync(
      Guid registrationId,
      bool includeSecret,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("909cd090-3005-480d-a1b4-220b76cb0afe");
      object routeValues = (object) new
      {
        registrationId = registrationId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (includeSecret), includeSecret.ToString());
      return this.SendAsync<Registration>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<Registration>> GetRegistrationsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Registration>>(new HttpMethod("GET"), new Guid("909cd090-3005-480d-a1b4-220b76cb0afe"), version: new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Registration> UpdateRegistrationAsync(
      Registration registration,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("909cd090-3005-480d-a1b4-220b76cb0afe");
      HttpContent httpContent = (HttpContent) new ObjectContent<Registration>(registration, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Registration>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<Registration> UpdateRegistrationAsync(
      Registration registration,
      bool includeSecret,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("909cd090-3005-480d-a1b4-220b76cb0afe");
      HttpContent httpContent = (HttpContent) new ObjectContent<Registration>(registration, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (includeSecret), includeSecret.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Registration>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<JsonWebToken> GetSecretAsync(
      Guid registrationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<JsonWebToken>(new HttpMethod("GET"), new Guid("f37e5023-dfbe-490e-9e40-7b7fb6b67887"), (object) new
      {
        registrationId = registrationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Registration> RotateSecretAsync(
      Guid registrationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Registration>(new HttpMethod("PUT"), new Guid("f37e5023-dfbe-490e-9e40-7b7fb6b67887"), (object) new
      {
        registrationId = registrationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }
  }
}
