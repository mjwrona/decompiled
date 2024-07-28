// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.OAuthTokenCredential
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net;

namespace Microsoft.TeamFoundation.Client
{
  [Obsolete("This class is deprecated and will be removed in a future release. See Microsoft.VisualStudio.Services.WebApi.VssOAuthCredential instead.", false)]
  [Serializable]
  public sealed class OAuthTokenCredential : FederatedCredential
  {
    private Uri m_authorizationUri;
    private string m_clientId;
    private string m_clientSecret;
    private OAuthTokensReceivedCallback m_callback;

    public OAuthTokenCredential(string accessToken)
      : base((IssuedToken) OAuthTokenContainer.FromAccessToken(accessToken))
    {
    }

    public OAuthTokenCredential(
      Uri authorizationUri,
      string clientId,
      string clientSecret,
      string authCodeOrRefreshToken)
      : this(authorizationUri, clientId, clientSecret, authCodeOrRefreshToken, (OAuthTokensReceivedCallback) null)
    {
    }

    public OAuthTokenCredential(
      Uri authorizationUri,
      string clientId,
      string clientSecret,
      string authCodeOrRefreshToken,
      OAuthTokensReceivedCallback tokensReceived)
      : this(authorizationUri, clientId, clientSecret, OAuthTokenContainer.FromAuthCodeOrRefreshToken(authCodeOrRefreshToken), tokensReceived)
    {
    }

    public OAuthTokenCredential(OAuthTokenContainer initialToken)
      : base((IssuedToken) initialToken)
    {
    }

    public OAuthTokenCredential(
      Uri authorizationUri,
      string clientId,
      string clientSecret,
      OAuthTokenContainer initialToken,
      OAuthTokensReceivedCallback tokensReceived)
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

    public Uri AuthorizationUrl => this.m_authorizationUri;

    public string ClientId => this.m_clientId;

    internal string ClientSecret => this.m_clientSecret;

    internal OAuthTokensReceivedCallback TokensReceived => this.m_callback;

    protected override VssCredentialsType CredentialType => VssCredentialsType.OAuth;

    internal override bool IsAuthenticationChallenge(HttpWebResponse webResponse)
    {
      if (webResponse == null)
        return false;
      if (webResponse.StatusCode == HttpStatusCode.Unauthorized)
      {
        string header = webResponse.Headers[HttpResponseHeader.WwwAuthenticate];
        if (!string.IsNullOrEmpty(header))
          return header.Contains("Bearer");
      }
      else if ((webResponse.StatusCode == HttpStatusCode.Found || webResponse.StatusCode == HttpStatusCode.Found) && !string.IsNullOrEmpty(webResponse.Headers["X-TFS-FedAuthIssuer"]))
        return !string.IsNullOrEmpty(webResponse.Headers["X-TFS-FedAuthRealm"]);
      return false;
    }

    internal override IssuedTokenProvider OnCreateTokenProvider(
      Uri serverUrl,
      HttpWebResponse response)
    {
      OAuthTokenContainer initialToken = this.InitialToken as OAuthTokenContainer;
      return response == null && (initialToken == null || initialToken.AccessToken == null) ? (IssuedTokenProvider) null : (IssuedTokenProvider) new OAuthTokenProvider(this, serverUrl, this.m_authorizationUri);
    }
  }
}
