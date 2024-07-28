// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.CookieCredential
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Client.Controls;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net;

namespace Microsoft.TeamFoundation.Client
{
  [Obsolete("This class is deprecated and will be removed in a future release. See Microsoft.VisualStudio.Services.Client.VssFederatedCredential instead.", false)]
  [Serializable]
  public sealed class CookieCredential : FederatedCredential
  {
    private readonly bool m_useCache;
    [NonSerialized]
    private bool m_cacheInitialized;
    [NonSerialized]
    private VssFederatedCredentialPrompt m_prompt;
    [NonSerialized]
    private string m_tenantId;

    public CookieCredential()
      : this(true)
    {
    }

    public CookieCredential(bool useCache)
      : this(useCache, (CookieToken) null)
    {
    }

    public CookieCredential(bool useCache, CookieToken initialToken)
      : base((IssuedToken) initialToken)
    {
      this.m_useCache = useCache;
      if (!useCache)
        return;
      this.TokenStorage = new TfsClientCredentialStorage();
      this.m_cacheInitialized = true;
    }

    internal VssFederatedCredentialPrompt Prompt
    {
      get => this.m_prompt;
      set => this.m_prompt = value;
    }

    internal string TenantId
    {
      get => this.m_tenantId;
      set => this.m_tenantId = value;
    }

    protected override VssCredentialsType CredentialType => VssCredentialsType.Federated;

    internal override bool IsAuthenticationChallenge(HttpWebResponse webResponse)
    {
      if (webResponse.StatusCode == HttpStatusCode.Found || webResponse.StatusCode == HttpStatusCode.Found)
        return webResponse.Headers[HttpResponseHeader.Location] != null && webResponse.Headers["X-TFS-FedAuthRealm"] != null;
      if (webResponse.StatusCode != HttpStatusCode.Unauthorized)
        return false;
      string header = webResponse.Headers[HttpResponseHeader.WwwAuthenticate];
      return header != null && header.Contains("TFS-Federated");
    }

    internal override IssuedTokenProvider OnCreateTokenProvider(
      Uri serverUrl,
      HttpWebResponse response)
    {
      if (this.InitialToken == null)
      {
        if (this.TokenStorage == null && this.m_useCache && !this.m_cacheInitialized)
        {
          this.TokenStorage = new TfsClientCredentialStorage();
          this.m_cacheInitialized = true;
        }
        if (this.TokenStorage != null)
          this.InitialToken = this.TokenStorage.RetrieveToken(serverUrl, this.CredentialType);
      }
      if (response == null && this.InitialToken == null)
        return (IssuedTokenProvider) null;
      Uri uri = (Uri) null;
      Uri replyTo = (Uri) null;
      string realm = (string) null;
      string issuer = (string) null;
      this.m_tenantId = (string) null;
      if (response != null)
      {
        if (!string.IsNullOrWhiteSpace(response.Headers[HttpResponseHeader.Location]))
          uri = new Uri(response.Headers[HttpResponseHeader.Location]);
        else if (!string.IsNullOrWhiteSpace(response.Headers["X-TFS-FedAuthRedirect"]))
          uri = new Uri(response.Headers["X-TFS-FedAuthRedirect"]);
        CookieCredential.AddParameter(ref uri, "protocol", "javascriptnotify");
        CookieCredential.AddParameter(ref uri, "force", "1");
        CookieCredential.GetRealmAndIssuer(response, out realm, out issuer);
        this.m_tenantId = CookieCredential.GetTenantInfo(response);
        replyTo = response.ResponseUri;
      }
      return (IssuedTokenProvider) new CookieTokenProvider(this, serverUrl, uri, issuer, realm, replyTo);
    }

    internal static void GetRealmAndIssuer(
      HttpWebResponse response,
      out string realm,
      out string issuer)
    {
      realm = response.Headers["X-TFS-FedAuthRealm"];
      issuer = response.Headers["X-TFS-FedAuthIssuer"];
      if (string.IsNullOrWhiteSpace(issuer))
        return;
      issuer = new Uri(issuer).GetLeftPart(UriPartial.Authority);
    }

    internal static string GetTenantInfo(HttpWebResponse response) => response.Headers["X-VSS-ResourceTenant"];

    private static void AddParameter(ref Uri uri, string name, string value)
    {
      if (uri.Query.IndexOf(name + "=", StringComparison.OrdinalIgnoreCase) >= 0)
        return;
      UriBuilder uriBuilder = new UriBuilder(uri);
      uriBuilder.Query = uriBuilder.Query.TrimStart('?') + "&" + name + "=" + value;
      uri = uriBuilder.Uri;
    }
  }
}
