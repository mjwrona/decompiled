// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.OAuth.VssOAuthCredential
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Linq;
using System.Net;

namespace Microsoft.VisualStudio.Services.OAuth
{
  public class VssOAuthCredential : FederatedCredential
  {
    private VssOAuthTokenParameters m_tokenParameters;
    private readonly Uri m_authorizationUrl;
    private readonly VssOAuthGrant m_grant;
    private readonly VssOAuthClientCredential m_clientCredential;

    public VssOAuthCredential(
      Uri authorizationUrl,
      VssOAuthGrant grant,
      VssOAuthClientCredential clientCredential,
      VssOAuthTokenParameters tokenParameters = null,
      VssOAuthAccessToken accessToken = null)
      : base((IssuedToken) accessToken)
    {
      ArgumentUtility.CheckForNull<Uri>(authorizationUrl, nameof (authorizationUrl));
      ArgumentUtility.CheckForNull<VssOAuthGrant>(grant, nameof (grant));
      this.m_authorizationUrl = authorizationUrl;
      this.m_grant = grant;
      this.m_tokenParameters = tokenParameters;
      this.m_clientCredential = clientCredential;
    }

    public override VssCredentialsType CredentialType => VssCredentialsType.OAuth;

    public Uri AuthorizationUrl => this.m_authorizationUrl;

    public VssOAuthGrant Grant => this.m_grant;

    public VssOAuthClientCredential ClientCredential => this.m_clientCredential;

    public VssOAuthTokenParameters TokenParameters
    {
      get
      {
        if (this.m_tokenParameters == null)
          this.m_tokenParameters = new VssOAuthTokenParameters();
        return this.m_tokenParameters;
      }
    }

    public override bool IsAuthenticationChallenge(IHttpResponse webResponse) => webResponse != null && (webResponse.StatusCode == HttpStatusCode.Found || webResponse.StatusCode == HttpStatusCode.Unauthorized) && webResponse.Headers.GetValues("WWW-Authenticate").Any<string>((Func<string, bool>) (x => x.IndexOf("Bearer", StringComparison.OrdinalIgnoreCase) >= 0));

    protected override IssuedTokenProvider OnCreateTokenProvider(
      Uri serverUrl,
      IHttpResponse response)
    {
      return (IssuedTokenProvider) new VssOAuthTokenProvider(this, serverUrl);
    }
  }
}
