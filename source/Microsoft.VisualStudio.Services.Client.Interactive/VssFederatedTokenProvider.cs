// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.VssFederatedTokenProvider
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;
using System.Net;

namespace Microsoft.VisualStudio.Services.Client
{
  internal sealed class VssFederatedTokenProvider : IssuedTokenProvider, ISupportSignOut
  {
    public VssFederatedTokenProvider(
      VssFederatedCredential credential,
      Uri serverUrl,
      Uri signInUrl,
      string issuer,
      string realm)
      : base((IssuedTokenCredential) credential, serverUrl, signInUrl)
    {
      this.Issuer = issuer;
      this.Realm = realm;
    }

    protected override string AuthenticationScheme => "TFS-Federated";

    protected override string AuthenticationParameter => string.IsNullOrEmpty(this.Issuer) && string.IsNullOrEmpty(this.Realm) ? string.Empty : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "issuer=\"{0}\", realm=\"{1}\"", (object) this.Issuer, (object) this.Realm);

    public VssFederatedCredential Credential => (VssFederatedCredential) base.Credential;

    public override bool GetTokenIsInteractive => this.CurrentToken == null;

    public string Issuer { get; private set; }

    public string Realm { get; private set; }

    protected internal override bool IsAuthenticationChallenge(IHttpResponse webResponse)
    {
      if (!base.IsAuthenticationChallenge(webResponse) || this.SignInUrl == (Uri) null)
        return false;
      string realm;
      string issuer;
      VssFederatedCredential.GetRealmAndIssuer(webResponse, out realm, out issuer);
      return this.Realm.Equals(realm, StringComparison.OrdinalIgnoreCase) && this.Issuer.Equals(issuer, StringComparison.OrdinalIgnoreCase);
    }

    protected override IssuedToken OnValidatingToken(IssuedToken token, IHttpResponse webResponse)
    {
      CookieCollection federatedCookies = CookieUtility.GetFederatedCookies(webResponse);
      if (federatedCookies != null)
      {
        VssFederatedToken vssFederatedToken = new VssFederatedToken(federatedCookies);
        vssFederatedToken.Properties = token.Properties;
        vssFederatedToken.UserId = token.UserId;
        vssFederatedToken.UserName = token.UserName;
        token = (IssuedToken) vssFederatedToken;
      }
      return token;
    }

    public void SignOut(Uri signOutUrl, Uri replyToUrl, string identityProvider)
    {
      CookieUtility.DeleteFederatedCookies(replyToUrl);
      if (string.IsNullOrEmpty(identityProvider) || !identityProvider.Equals("Windows Live ID", StringComparison.OrdinalIgnoreCase))
        return;
      CookieUtility.DeleteWindowsLiveCookies();
    }
  }
}
