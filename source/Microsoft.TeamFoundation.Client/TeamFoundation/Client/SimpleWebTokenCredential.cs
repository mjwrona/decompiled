// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.SimpleWebTokenCredential
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net;

namespace Microsoft.TeamFoundation.Client
{
  [Obsolete("This class is deprecated and will be removed in a future release. See Microsoft.VisualStudio.Services.Common.VssServiceIdentityCredential instead.", false)]
  [Serializable]
  public sealed class SimpleWebTokenCredential : FederatedCredential
  {
    private string m_userName;
    private string m_password;

    public SimpleWebTokenCredential(string userName, string password)
      : this(userName, password, (SimpleWebToken) null)
    {
    }

    public SimpleWebTokenCredential(string userName, string password, SimpleWebToken initialToken)
      : base((IssuedToken) initialToken)
    {
      this.m_userName = userName;
      this.m_password = password;
    }

    public string UserName => this.m_userName;

    internal string Password => this.m_password;

    protected override VssCredentialsType CredentialType => VssCredentialsType.ServiceIdentity;

    internal override bool IsAuthenticationChallenge(HttpWebResponse webResponse)
    {
      if (webResponse == null || webResponse.StatusCode != HttpStatusCode.Found && webResponse.StatusCode != HttpStatusCode.Found && webResponse.StatusCode != HttpStatusCode.Unauthorized)
        return false;
      string header1 = webResponse.Headers["X-TFS-FedAuthRealm"];
      if (string.IsNullOrEmpty(webResponse.Headers["X-TFS-FedAuthIssuer"]) || string.IsNullOrEmpty(header1))
        return false;
      string header2 = webResponse.Headers[HttpResponseHeader.WwwAuthenticate];
      return webResponse.StatusCode != HttpStatusCode.Unauthorized || header2.Contains("TFS-Federated");
    }

    internal override IssuedTokenProvider OnCreateTokenProvider(
      Uri serverUrl,
      HttpWebResponse response)
    {
      if (response == null && this.InitialToken == null)
        return (IssuedTokenProvider) null;
      Uri signInUrl = (Uri) null;
      string realm = string.Empty;
      if (response != null)
      {
        realm = response.Headers["X-TFS-FedAuthRealm"];
        signInUrl = new Uri(new Uri(response.Headers["X-TFS-FedAuthIssuer"]).GetLeftPart(UriPartial.Authority));
      }
      return (IssuedTokenProvider) new SimpleWebTokenProvider(this, serverUrl, signInUrl, realm);
    }
  }
}
