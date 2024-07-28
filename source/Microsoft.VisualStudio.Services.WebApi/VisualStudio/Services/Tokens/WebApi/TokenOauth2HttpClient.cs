// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tokens.WebApi.TokenOauth2HttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Tokens.WebApi
{
  [ResourceArea("c5a2d98b-985c-432e-825e-3c6971edae87")]
  public class TokenOauth2HttpClient : VssHttpClientBase
  {
    public TokenOauth2HttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TokenOauth2HttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TokenOauth2HttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TokenOauth2HttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TokenOauth2HttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<AccessTokenResult> IssueApplicationTokenAsync(
      string clientSecret,
      Guid registrationId,
      Guid hostId,
      Guid? tenantId = null,
      string requestedScopes = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("bbc63806-e448-4e88-8c57-0af77747a323");
      HttpContent httpContent = (HttpContent) new ObjectContent<string>(clientSecret, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (registrationId), registrationId.ToString());
      collection.Add(nameof (hostId), hostId.ToString());
      if (tenantId.HasValue)
        collection.Add(nameof (tenantId), tenantId.Value.ToString());
      if (requestedScopes != null)
        collection.Add(nameof (requestedScopes), requestedScopes);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<AccessTokenResult>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<AccessTokenResult> IssueTokenAsync(
      GrantTokenSecretPair tokenSecretPair,
      GrantType grantType,
      Guid hostId,
      Guid orgHostId,
      Uri audience = null,
      Uri redirectUri = null,
      Guid? accessId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("bbc63806-e448-4e88-8c57-0af77747a323");
      HttpContent httpContent = (HttpContent) new ObjectContent<GrantTokenSecretPair>(tokenSecretPair, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (grantType), grantType.ToString());
      collection.Add(nameof (hostId), hostId.ToString());
      collection.Add(nameof (orgHostId), orgHostId.ToString());
      if (audience != (Uri) null)
        collection.Add(nameof (audience), audience.ToString());
      if (redirectUri != (Uri) null)
        collection.Add(nameof (redirectUri), redirectUri.ToString());
      if (accessId.HasValue)
        collection.Add(nameof (accessId), accessId.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<AccessTokenResult>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
