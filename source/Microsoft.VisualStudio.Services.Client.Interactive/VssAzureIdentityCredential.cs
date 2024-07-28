// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.VssAzureIdentityCredential
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Azure.Core;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Linq;
using System.Net;

namespace Microsoft.VisualStudio.Services.Client
{
  [Serializable]
  public sealed class VssAzureIdentityCredential : FederatedCredential
  {
    private readonly TokenCredential tokenCredential;

    public VssAzureIdentityCredential(TokenCredential tokenCredential)
      : base((IssuedToken) null)
    {
      ArgumentUtility.CheckForNull<TokenCredential>(tokenCredential, nameof (tokenCredential));
      this.tokenCredential = tokenCredential;
    }

    public override VssCredentialsType CredentialType => VssCredentialsType.Aad;

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
      return (IssuedTokenProvider) new VssAzureIdentityTokenProvider(this, this.tokenCredential);
    }
  }
}
