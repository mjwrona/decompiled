// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.VssOAuthCredential
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Linq;
using System.Net;

namespace Microsoft.VisualStudio.Services.Client
{
  public sealed class VssOAuthCredential : FederatedCredential
  {
    private readonly string m_clientId;
    private readonly string m_clientSecret;
    private readonly Uri m_authorizationUri;
    private readonly Action<VssOAuthTokenContainer> m_callback;

    public VssOAuthCredential(
      Uri authorizationUri,
      string clientId,
      string clientSecret,
      VssOAuthTokenContainer initialToken,
      Action<VssOAuthTokenContainer> tokensReceived)
      : base((IssuedToken) initialToken)
    {
      ArgumentUtility.CheckForNull<Uri>(authorizationUri, nameof (authorizationUri));
      ArgumentUtility.CheckStringForNullOrEmpty(clientId, nameof (clientId));
      ArgumentUtility.CheckStringForNullOrEmpty(clientSecret, nameof (clientSecret));
      this.m_authorizationUri = authorizationUri;
      this.m_clientId = clientId;
      this.m_clientSecret = clientSecret;
      this.m_callback = tokensReceived;
    }

    public override VssCredentialsType CredentialType => VssCredentialsType.OAuth;

    public string ClientId => this.m_clientId;

    internal string ClientSecret => this.m_clientSecret;

    internal Action<VssOAuthTokenContainer> TokensReceived => this.m_callback;

    public override bool IsAuthenticationChallenge(IHttpResponse webResponse)
    {
      if (webResponse == null)
        return false;
      if (webResponse.StatusCode == HttpStatusCode.Unauthorized)
        return webResponse.Headers.GetValues("WWW-Authenticate").Any<string>((Func<string, bool>) (x => x.StartsWith("Bearer", StringComparison.OrdinalIgnoreCase)));
      return (webResponse.StatusCode == HttpStatusCode.Found || webResponse.StatusCode == HttpStatusCode.Found) && webResponse.Headers.GetValues("X-TFS-FedAuthRealm").Any<string>() && webResponse.Headers.GetValues("X-TFS-FedAuthIssuer").Any<string>();
    }

    protected override IssuedTokenProvider OnCreateTokenProvider(
      Uri serverUrl,
      IHttpResponse response)
    {
      VssOAuthTokenContainer initialToken = this.InitialToken as VssOAuthTokenContainer;
      return response == null && (initialToken == null || initialToken.AccessToken == null) ? (IssuedTokenProvider) null : (IssuedTokenProvider) new VssOAuthTokenProvider(this, serverUrl, this.m_authorizationUri);
    }
  }
}
