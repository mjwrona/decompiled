// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.OAuth.Client.OAuthHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.OAuth.Client
{
  public class OAuthHttpClient : VssHttpClientBase
  {
    public OAuthHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public OAuthHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public OAuthHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public OAuthHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public OAuthHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    protected override Task HandleResponseAsync(
      HttpResponseMessage response,
      CancellationToken cancellationToken)
    {
      return response.StatusCode != HttpStatusCode.Found ? base.HandleResponseAsync(response, cancellationToken) : (Task) Task.FromResult<bool>(false);
    }

    public Task<AuthorizationResponse> AuthorizeAsync(
      string clientId,
      string responseType,
      string redirectUri,
      string scope,
      string state,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckStringForNullOrEmpty(nameof (clientId), clientId);
      ArgumentUtility.CheckStringForNullOrEmpty(nameof (responseType), responseType);
      List<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>>();
      nameValueCollection.Add(new KeyValuePair<string, string>("client_id", clientId));
      nameValueCollection.Add(new KeyValuePair<string, string>("response_type", responseType));
      if (!string.IsNullOrEmpty(redirectUri))
        nameValueCollection.Add(new KeyValuePair<string, string>("redirect_uri", redirectUri));
      if (!string.IsNullOrEmpty(scope))
        nameValueCollection.Add(new KeyValuePair<string, string>(nameof (scope), scope));
      if (!string.IsNullOrEmpty(state))
        nameValueCollection.Add(new KeyValuePair<string, string>(nameof (state), state));
      return this.SendAsync<AuthorizationResponse>(new HttpRequestMessage(HttpMethod.Post, new Uri(this.BaseAddress, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/_apis/OAuth/Auth")).AbsoluteUri)
      {
        Content = (HttpContent) new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>) nameValueCollection)
      }, userState, cancellationToken);
    }

    public Task<AccessTokenResponse> CreateTokenAsync(
      string grantType,
      string code,
      string refreshToken,
      string scope,
      string redirectUri,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckStringForNullOrEmpty(nameof (grantType), grantType);
      List<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>>();
      nameValueCollection.Add(new KeyValuePair<string, string>("grant_type", grantType));
      if (!string.IsNullOrEmpty(code))
        nameValueCollection.Add(new KeyValuePair<string, string>(nameof (code), code));
      if (!string.IsNullOrEmpty(refreshToken))
        nameValueCollection.Add(new KeyValuePair<string, string>("refresh_token", refreshToken));
      if (!string.IsNullOrEmpty(scope))
        nameValueCollection.Add(new KeyValuePair<string, string>(nameof (scope), scope));
      if (!string.IsNullOrEmpty(redirectUri))
        nameValueCollection.Add(new KeyValuePair<string, string>("redirect_uri", redirectUri));
      return this.SendAsync<AccessTokenResponse>(new HttpRequestMessage(HttpMethod.Post, new Uri(this.BaseAddress, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/_apis/OAuth/Tokens")).AbsoluteUri)
      {
        Content = (HttpContent) new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>) nameValueCollection)
      }, userState, cancellationToken);
    }
  }
}
