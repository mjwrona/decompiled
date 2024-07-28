// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.VssServiceIdentityCredential
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Common
{
  [Serializable]
  public sealed class VssServiceIdentityCredential : FederatedCredential
  {
    private readonly string m_userName;
    private readonly string m_password;
    [NonSerialized]
    private readonly DelegatingHandler m_innerHandler;

    public VssServiceIdentityCredential(string userName, string password)
      : this(userName, password, (VssServiceIdentityToken) null)
    {
    }

    public VssServiceIdentityCredential(
      string userName,
      string password,
      VssServiceIdentityToken initialToken)
      : this(userName, password, initialToken, (DelegatingHandler) null)
    {
    }

    public VssServiceIdentityCredential(VssServiceIdentityToken token)
      : this((string) null, (string) null, token, (DelegatingHandler) null)
    {
    }

    public VssServiceIdentityCredential(
      string userName,
      string password,
      VssServiceIdentityToken initialToken,
      DelegatingHandler innerHandler)
      : base((IssuedToken) initialToken)
    {
      this.m_userName = userName;
      this.m_password = password;
      this.m_innerHandler = innerHandler;
    }

    public override VssCredentialsType CredentialType => VssCredentialsType.ServiceIdentity;

    public string UserName => this.m_userName;

    internal string Password => this.m_password;

    public override bool IsAuthenticationChallenge(IHttpResponse webResponse)
    {
      if (webResponse == null || webResponse.StatusCode != HttpStatusCode.Found && webResponse.StatusCode != HttpStatusCode.Found && webResponse.StatusCode != HttpStatusCode.Unauthorized)
        return false;
      string str1 = webResponse.Headers.GetValues("X-TFS-FedAuthRealm").FirstOrDefault<string>();
      string str2 = webResponse.Headers.GetValues("X-TFS-FedAuthIssuer").FirstOrDefault<string>();
      IEnumerable<string> values = webResponse.Headers.GetValues("WWW-Authenticate");
      if (string.IsNullOrEmpty(str2) || string.IsNullOrEmpty(str1))
        return false;
      return webResponse.StatusCode != HttpStatusCode.Unauthorized || values.Any<string>((Func<string, bool>) (x => x.StartsWith("TFS-Federated", StringComparison.OrdinalIgnoreCase)));
    }

    internal override string GetAuthenticationChallenge(IHttpResponse webResponse) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TFS-Federated realm={0}, issuer={1}", (object) webResponse.Headers.GetValues("X-TFS-FedAuthRealm").FirstOrDefault<string>(), (object) webResponse.Headers.GetValues("X-TFS-FedAuthIssuer").FirstOrDefault<string>());

    protected override IssuedTokenProvider OnCreateTokenProvider(
      Uri serverUrl,
      IHttpResponse response)
    {
      if (response == null && this.InitialToken == null)
        return (IssuedTokenProvider) null;
      Uri signInUrl = (Uri) null;
      string realm = string.Empty;
      if (response != null)
      {
        realm = response.Headers.GetValues("X-TFS-FedAuthRealm").FirstOrDefault<string>();
        signInUrl = new Uri(new Uri(response.Headers.GetValues("X-TFS-FedAuthIssuer").FirstOrDefault<string>()).GetLeftPart(UriPartial.Authority));
      }
      return (IssuedTokenProvider) new VssServiceIdentityTokenProvider(this, serverUrl, signInUrl, realm, this.m_innerHandler);
    }
  }
}
