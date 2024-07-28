// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.VssServiceIdentityTokenProvider
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Diagnostics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Common
{
  internal sealed class VssServiceIdentityTokenProvider : IssuedTokenProvider
  {
    private DelegatingHandler m_innerHandler;

    public VssServiceIdentityTokenProvider(
      VssServiceIdentityCredential credential,
      Uri serverUrl,
      Uri signInUrl,
      string realm,
      DelegatingHandler innerHandler)
      : this(credential, serverUrl, signInUrl, realm)
    {
      this.m_innerHandler = innerHandler;
    }

    public VssServiceIdentityTokenProvider(
      VssServiceIdentityCredential credential,
      Uri serverUrl,
      Uri signInUrl,
      string realm)
      : base((IssuedTokenCredential) credential, serverUrl, signInUrl)
    {
      this.Realm = realm;
    }

    protected override string AuthenticationParameter => string.IsNullOrEmpty(this.Realm) && this.SignInUrl == (Uri) null ? string.Empty : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "issuer=\"{0}\", realm=\"{1}\"", (object) this.SignInUrl, (object) this.Realm);

    protected override string AuthenticationScheme => "TFS-Federated";

    public VssServiceIdentityCredential Credential => (VssServiceIdentityCredential) base.Credential;

    public override bool GetTokenIsInteractive => false;

    public string Realm { get; }

    protected internal override bool IsAuthenticationChallenge(IHttpResponse webResponse)
    {
      if (!base.IsAuthenticationChallenge(webResponse) || this.SignInUrl == (Uri) null)
        return false;
      string str = webResponse.Headers.GetValues("X-TFS-FedAuthRealm").FirstOrDefault<string>();
      Uri uri2 = new Uri(new Uri(webResponse.Headers.GetValues("X-TFS-FedAuthIssuer").FirstOrDefault<string>()).GetLeftPart(UriPartial.Authority), UriKind.Absolute);
      return this.Realm.Equals(str, StringComparison.OrdinalIgnoreCase) && Uri.Compare(this.SignInUrl, uri2, UriComponents.AbsoluteUri, UriFormat.Unescaped, StringComparison.OrdinalIgnoreCase) == 0;
    }

    protected override async Task<IssuedToken> OnGetTokenAsync(
      IssuedToken failedToken,
      CancellationToken cancellationToken)
    {
      VssServiceIdentityTokenProvider provider = this;
      if (string.IsNullOrEmpty(provider.Credential.UserName) || string.IsNullOrEmpty(provider.Credential.Password))
        return (IssuedToken) null;
      VssTraceActivity traceActivity = VssTraceActivity.Current;
      using (HttpClient client = new HttpClient(provider.CreateMessageHandler(), false))
      {
        client.BaseAddress = provider.SignInUrl;
        KeyValuePair<string, string>[] nameValueCollection = new KeyValuePair<string, string>[3]
        {
          new KeyValuePair<string, string>("wrap_name", provider.Credential.UserName),
          new KeyValuePair<string, string>("wrap_password", provider.Credential.Password),
          new KeyValuePair<string, string>("wrap_scope", provider.Realm)
        };
        using (HttpResponseMessage response = await client.PostAsync(new Uri("WRAPv0.9/", UriKind.Relative), (HttpContent) new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>) nameValueCollection), cancellationToken).ConfigureAwait(false))
        {
          string str = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
          if (response.IsSuccessStatusCode)
            return (IssuedToken) VssServiceIdentityToken.ExtractToken(str);
          VssHttpEventSource.Log.AuthenticationError(traceActivity, (IssuedTokenProvider) provider, str);
          return (IssuedToken) null;
        }
      }
    }

    private HttpMessageHandler CreateMessageHandler()
    {
      VssHttpRetryOptions options = new VssHttpRetryOptions();
      options.RetryableStatusCodes.Add((HttpStatusCode) 429);
      options.RetryableStatusCodes.Add(HttpStatusCode.InternalServerError);
      HttpMessageHandler innerHandler;
      if (this.m_innerHandler != null)
      {
        if (this.m_innerHandler.InnerHandler == null)
          this.m_innerHandler.InnerHandler = (HttpMessageHandler) new HttpClientHandler();
        innerHandler = (HttpMessageHandler) this.m_innerHandler;
      }
      else
        innerHandler = (HttpMessageHandler) new HttpClientHandler();
      if (innerHandler is HttpClientHandler httpClientHandler && VssHttpMessageHandler.DefaultWebProxy != null)
      {
        httpClientHandler.Proxy = VssHttpMessageHandler.DefaultWebProxy;
        httpClientHandler.UseProxy = true;
      }
      return (HttpMessageHandler) new VssHttpRetryMessageHandler(options, innerHandler);
    }
  }
}
