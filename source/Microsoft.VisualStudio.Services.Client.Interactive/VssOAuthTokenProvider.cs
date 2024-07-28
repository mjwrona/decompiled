// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.VssOAuthTokenProvider
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Diagnostics;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Client
{
  internal sealed class VssOAuthTokenProvider : IssuedTokenProvider
  {
    public VssOAuthTokenProvider(VssOAuthCredential credential, Uri serverUrl, Uri signInUrl)
      : base((IssuedTokenCredential) credential, serverUrl, signInUrl)
    {
    }

    protected override string AuthenticationScheme => "Bearer";

    public VssOAuthCredential Credential => (VssOAuthCredential) base.Credential;

    public override bool GetTokenIsInteractive => false;

    protected override async Task<IssuedToken> OnGetTokenAsync(
      IssuedToken failedToken,
      CancellationToken cancellationToken)
    {
      VssOAuthTokenProvider provider = this;
      if (string.IsNullOrEmpty(provider.Credential.ClientId) || string.IsNullOrEmpty(provider.Credential.ClientSecret) || !(failedToken is VssOAuthTokenContainer oauthTokenContainer) || oauthTokenContainer.RefreshToken == null)
        return (IssuedToken) null;
      VssTraceActivity traceActivity = VssTraceActivity.Current;
      VssOAuthTokenContainer tokens;
      using (HttpClient client = new HttpClient(provider.CreateMessageHandler()))
      {
        client.BaseAddress = provider.SignInUrl;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(provider.Credential.ClientId + ":" + provider.Credential.ClientSecret)));
        List<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>>(2);
        if (oauthTokenContainer.RefreshToken.TokenType == VssOAuthTokenType.AuthenticationCode)
        {
          nameValueCollection.Add(new KeyValuePair<string, string>("grant_type", "authorization_code"));
          nameValueCollection.Add(new KeyValuePair<string, string>("code", oauthTokenContainer.RefreshToken.Token));
        }
        else if (oauthTokenContainer.RefreshToken.TokenType == VssOAuthTokenType.RefreshToken)
        {
          nameValueCollection.Add(new KeyValuePair<string, string>("grant_type", "refresh_token"));
          nameValueCollection.Add(new KeyValuePair<string, string>("refresh_token", oauthTokenContainer.RefreshToken.Token));
        }
        using (HttpResponseMessage response = await client.PostAsync(new Uri("_oauth/accesstoken/", UriKind.Relative), (HttpContent) new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>) nameValueCollection)).ConfigureAwait(false))
        {
          if (response.IsSuccessStatusCode)
          {
            tokens = VssOAuthTokenContainer.ExtractTokens(await response.Content.ReadAsStreamAsync().ConfigureAwait(false));
          }
          else
          {
            string message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            VssHttpEventSource.Log.AuthenticationError(traceActivity, (IssuedTokenProvider) provider, message);
            return (IssuedToken) null;
          }
        }
      }
      if (provider.Credential.TokensReceived != null)
        provider.Credential.TokensReceived(tokens);
      return (IssuedToken) tokens;
    }

    private HttpMessageHandler CreateMessageHandler() => (HttpMessageHandler) new VssHttpRetryMessageHandler(new VssHttpRetryOptions()
    {
      RetryableStatusCodes = {
        (HttpStatusCode) 429,
        HttpStatusCode.InternalServerError
      }
    }, (HttpMessageHandler) new HttpClientHandler());
  }
}
