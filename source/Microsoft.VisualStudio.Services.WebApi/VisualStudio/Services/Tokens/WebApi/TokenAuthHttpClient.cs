// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tokens.WebApi.TokenAuthHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Tokens.WebApi
{
  [ResourceArea("c5a2d98b-985c-432e-825e-3c6971edae87")]
  public class TokenAuthHttpClient : VssHttpClientBase
  {
    public TokenAuthHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TokenAuthHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TokenAuthHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TokenAuthHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TokenAuthHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<AuthorizationDecision> AuthorizeAsync(
      Guid userId,
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes,
      Guid? authorizationId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("7d7ddc0d-60bd-4978-a0b5-295cb099a400");
      object routeValues = (object) new{ userId = userId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (responseType), responseType.ToString());
      keyValuePairList.Add(nameof (clientId), clientId.ToString());
      string str = (string) null;
      if (redirectUri != (Uri) null)
        str = redirectUri.ToString();
      keyValuePairList.Add(nameof (redirectUri), str);
      keyValuePairList.Add(nameof (scopes), scopes);
      if (authorizationId.HasValue)
        keyValuePairList.Add(nameof (authorizationId), authorizationId.Value.ToString());
      return this.SendAsync<AuthorizationDecision>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<AuthorizationDecision> AuthorizeAsync(
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes,
      Guid? authorizationId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("7d7ddc0d-60bd-4978-a0b5-295cb099a400");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (responseType), responseType.ToString());
      keyValuePairList.Add(nameof (clientId), clientId.ToString());
      string str = (string) null;
      if (redirectUri != (Uri) null)
        str = redirectUri.ToString();
      keyValuePairList.Add(nameof (redirectUri), str);
      keyValuePairList.Add(nameof (scopes), scopes);
      if (authorizationId.HasValue)
        keyValuePairList.Add(nameof (authorizationId), authorizationId.Value.ToString());
      return this.SendAsync<AuthorizationDecision>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<AuthorizationDetails>> GetAuthorizationsAsync(
      Guid? userId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<AuthorizationDetails>>(new HttpMethod("GET"), new Guid("7d7ddc0d-60bd-4978-a0b5-295cb099a400"), (object) new
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
      Guid locationId = new Guid("7d7ddc0d-60bd-4978-a0b5-295cb099a400");
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
      Guid locationId = new Guid("7d7ddc0d-60bd-4978-a0b5-295cb099a400");
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
      TokenAuthHttpClient tokenAuthHttpClient = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("7d7ddc0d-60bd-4978-a0b5-295cb099a400");
      object routeValues = (object) new{ userId = userId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (authorizationId), authorizationId.ToString());
      using (await tokenAuthHttpClient.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<HostAuthorizationDecision> AuthorizeHostAsync(
      Guid clientId,
      Guid hostId,
      Guid? newId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("PATCH");
      Guid locationId = new Guid("817d2b46-1507-4efe-be2b-adccf17ffd3b");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (clientId), clientId.ToString());
      keyValuePairList.Add(nameof (hostId), hostId.ToString());
      if (newId.HasValue)
        keyValuePairList.Add(nameof (newId), newId.Value.ToString());
      return this.SendAsync<HostAuthorizationDecision>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<HostAuthorization>> GetHostAuthorizationsAsync(
      Guid hostId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("817d2b46-1507-4efe-be2b-adccf17ffd3b");
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
      TokenAuthHttpClient tokenAuthHttpClient = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("817d2b46-1507-4efe-be2b-adccf17ffd3b");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (clientId), clientId.ToString());
      if (hostId.HasValue)
        keyValuePairList.Add(nameof (hostId), hostId.Value.ToString());
      using (await tokenAuthHttpClient.SendAsync(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<Registration> CreateRegistrationAsync(
      Registration registration,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("522ad1a0-389d-4c6f-90da-b145fd2d3ad8");
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
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("522ad1a0-389d-4c6f-90da-b145fd2d3ad8");
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
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("522ad1a0-389d-4c6f-90da-b145fd2d3ad8"), (object) new
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
      return this.SendAsync<Registration>(new HttpMethod("GET"), new Guid("522ad1a0-389d-4c6f-90da-b145fd2d3ad8"), (object) new
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
      Guid locationId = new Guid("522ad1a0-389d-4c6f-90da-b145fd2d3ad8");
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
      return this.SendAsync<List<Registration>>(new HttpMethod("GET"), new Guid("522ad1a0-389d-4c6f-90da-b145fd2d3ad8"), version: new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Registration> UpdateRegistrationAsync(
      Registration registration,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("522ad1a0-389d-4c6f-90da-b145fd2d3ad8");
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
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("522ad1a0-389d-4c6f-90da-b145fd2d3ad8");
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
      return this.SendAsync<JsonWebToken>(new HttpMethod("GET"), new Guid("74896548-9cdd-4315-8aeb-9ecd88fceb21"), (object) new
      {
        registrationId = registrationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Registration> RotateSecretAsync(
      Guid registrationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Registration>(new HttpMethod("PUT"), new Guid("74896548-9cdd-4315-8aeb-9ecd88fceb21"), (object) new
      {
        registrationId = registrationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }
  }
}
