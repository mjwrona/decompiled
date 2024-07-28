// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.VssFederatedCredential
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Linq;
using System.Net;

namespace Microsoft.VisualStudio.Services.Client
{
  [Serializable]
  public sealed class VssFederatedCredential : FederatedCredential
  {
    public VssFederatedCredential(bool useCache = false, VssFederatedToken initialToken = null)
      : base((IssuedToken) initialToken)
    {
      if (!useCache)
        return;
      this.Storage = (IVssCredentialStorage) new VssClientCredentialStorage();
    }

    public override VssCredentialsType CredentialType => VssCredentialsType.Federated;

    public override bool IsAuthenticationChallenge(IHttpResponse webResponse)
    {
      bool isNonAuthenticationChallenge = false;
      return VssFederatedCredential.IsVssFederatedAuthenticationChallenge(webResponse, out isNonAuthenticationChallenge) ?? isNonAuthenticationChallenge;
    }

    protected override IssuedTokenProvider OnCreateTokenProvider(
      Uri serverUrl,
      IHttpResponse response)
    {
      if (response == null && this.InitialToken == null)
        return (IssuedTokenProvider) null;
      Uri uri = (Uri) null;
      string realm = string.Empty;
      string issuer = string.Empty;
      if (response != null)
      {
        string uriString = response.Headers.GetValues("Location").FirstOrDefault<string>() ?? response.Headers.GetValues("X-TFS-FedAuthRedirect").FirstOrDefault<string>();
        if (!string.IsNullOrEmpty(uriString))
          uri = new Uri(uriString);
        VssFederatedCredential.AddParameter(ref uri, "protocol", "javascriptnotify");
        VssFederatedCredential.AddParameter(ref uri, "force", "1");
        VssFederatedCredential.GetRealmAndIssuer(response, out realm, out issuer);
      }
      return (IssuedTokenProvider) new VssFederatedTokenProvider(this, serverUrl, uri, issuer, realm);
    }

    internal static void GetRealmAndIssuer(
      IHttpResponse response,
      out string realm,
      out string issuer)
    {
      realm = response.Headers.GetValues("X-TFS-FedAuthRealm").FirstOrDefault<string>();
      issuer = response.Headers.GetValues("X-TFS-FedAuthIssuer").FirstOrDefault<string>();
      if (string.IsNullOrWhiteSpace(issuer))
        return;
      issuer = new Uri(issuer).GetLeftPart(UriPartial.Authority);
    }

    internal static bool? IsVssFederatedAuthenticationChallenge(
      IHttpResponse webResponse,
      out bool isNonAuthenticationChallenge)
    {
      isNonAuthenticationChallenge = false;
      if (webResponse == null)
        return new bool?(false);
      if (webResponse.StatusCode == HttpStatusCode.Found || webResponse.StatusCode == HttpStatusCode.Found)
        return new bool?(webResponse.Headers.GetValues("Location").Any<string>() && webResponse.Headers.GetValues("X-TFS-FedAuthRealm").Any<string>());
      if (webResponse.StatusCode == HttpStatusCode.Unauthorized)
        return new bool?(webResponse.Headers.GetValues("WWW-Authenticate").Any<string>((Func<string, bool>) (x => x.StartsWith("TFS-Federated", StringComparison.OrdinalIgnoreCase))));
      if (webResponse.StatusCode == HttpStatusCode.Forbidden)
      {
        isNonAuthenticationChallenge = webResponse.Headers.GetValues("X-TFS-FedAuthRedirect").Any<string>();
        if (isNonAuthenticationChallenge)
          return new bool?();
      }
      return new bool?(false);
    }

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
