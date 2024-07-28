// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.CookieTokenProvider
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Client.Controls;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Client
{
  [Obsolete("This class is deprecated and will be removed in a future release. See Microsoft.VisualStudio.Services.Client.VssFederatedTokenProvider instead.", false)]
  internal sealed class CookieTokenProvider : IssuedTokenProvider, ISupportSignOut
  {
    public CookieTokenProvider(
      CookieCredential credential,
      Uri serverUrl,
      Uri signInUrl,
      string issuer,
      string realm,
      Uri replyTo)
      : base((IssuedTokenCredential) credential, serverUrl, signInUrl)
    {
      this.Issuer = issuer;
      this.Realm = realm;
      this.ReplyTo = replyTo;
    }

    public CookieCredential Credential => (CookieCredential) base.Credential;

    public override bool GetTokenIsInteractive => this.CurrentToken == null;

    public string Issuer { get; private set; }

    public string Realm { get; private set; }

    public Uri ReplyTo { get; private set; }

    internal override bool IsAuthenticationChallenge(HttpWebResponse webResponse)
    {
      if (!base.IsAuthenticationChallenge(webResponse) || this.SignInUrl == (Uri) null)
        return false;
      string tenantInfo = CookieCredential.GetTenantInfo(webResponse);
      if (!string.IsNullOrEmpty(tenantInfo))
        this.Credential.TenantId = tenantInfo;
      string realm;
      string issuer;
      CookieCredential.GetRealmAndIssuer(webResponse, out realm, out issuer);
      return this.Realm.Equals(realm, StringComparison.OrdinalIgnoreCase) && this.Issuer.Equals(issuer, StringComparison.OrdinalIgnoreCase);
    }

    protected override IssuedToken OnGetToken(IssuedToken failedToken, TimeSpan timeout)
    {
      if (this.SignInUrl == (Uri) null)
        return (IssuedToken) null;
      CookieCollection cookies = (CookieCollection) null;
      if (this.Credential.Prompt == null)
      {
        this.Credential.Prompt = new VssFederatedCredentialPrompt();
        this.Credential.Prompt.ConnectMode = VssConnectMode.Resource;
        if (this.Credential.TokenStorage != null && this.ServerUrl != (Uri) null)
        {
          string tokenProperty = this.Credential.TokenStorage.GetTokenProperty(this.ServerUrl, "UserName");
          if (tokenProperty != null)
            this.Credential.Prompt.Parameters = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal)
            {
              {
                "user",
                tokenProperty
              }
            };
        }
      }
      try
      {
        if (this.Credential.TenantId != null)
          VssAuthenticationHelper.CheckForValidTokenByTenantAsync(this.Credential.Prompt, this.Credential.TenantId).SyncResultConfigured<bool>();
        cookies = this.Credential.Prompt.GetCookieAsync(this.SignInUrl).Result;
      }
      catch (AggregateException ex)
      {
        ex.Flatten().Handle((Func<Exception, bool>) (x => x is TaskCanceledException));
      }
      if (cookies == null)
        return (IssuedToken) null;
      return (IssuedToken) new CookieToken(cookies);
    }

    protected override IAsyncResult OnBeginGetToken(
      IssuedToken failedToken,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      return (IAsyncResult) new CompletedOperation<IssuedToken>(this.OnGetToken(failedToken, timeout), callback, state);
    }

    protected override IssuedToken OnEndGetToken(IAsyncResult result) => CompletedOperation<IssuedToken>.End(result);

    protected override IssuedToken OnValidatingToken(IssuedToken token, HttpWebResponse webResponse)
    {
      if (webResponse.Cookies != null)
      {
        CookieCollection federatedCookies = FederatedAcsLogon.GetFederatedCookies(webResponse.Cookies);
        if (federatedCookies != null)
        {
          CookieToken cookieToken = new CookieToken(federatedCookies);
          cookieToken.UserId = token.UserId;
          cookieToken.UserName = token.UserName;
          token = (IssuedToken) cookieToken;
        }
      }
      return token;
    }

    protected override void OnTokenValidated(IssuedToken token)
    {
      if (token is CookieToken token1 && !token1.FromStorage && this.Credential.TokenStorage != null)
        this.Credential.TokenStorage.StoreToken(this.ServerUrl, (IssuedToken) token1, true);
      base.OnTokenValidated(token);
    }

    protected override void OnTokenInvalidated(IssuedToken token)
    {
      if (token.CredentialType == VssCredentialsType.Federated && this.Credential.TokenStorage != null)
        this.Credential.TokenStorage.RemoveTokenValue(this.ServerUrl, token);
      base.OnTokenInvalidated(token);
    }

    public void SignOut(Uri signOutUrl, Uri replyToUrl, string identityProvider)
    {
      FederatedAcsLogon.DeleteFederatedCookies(replyToUrl, false);
      if (!string.IsNullOrEmpty(identityProvider) && identityProvider.Equals("Windows Live ID", StringComparison.OrdinalIgnoreCase))
        FederatedAcsLogon.DeleteWindowsLiveCookies();
      this.InvalidateToken(this.CurrentToken);
      this.Credential.InitialToken = (IssuedToken) null;
    }

    private static void ParseUserData(string userData, out Guid userId, out string userName)
    {
      userId = Guid.Empty;
      userName = string.Empty;
      if (string.IsNullOrWhiteSpace(userData))
        return;
      string[] strArray = userData.Split(':');
      if (strArray.Length < 2)
        return;
      userId = Guid.Parse(strArray[0]);
      userName = strArray[1];
    }
  }
}
