// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.VssAadTokenProvider
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.Identity.Client;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Client
{
  internal sealed class VssAadTokenProvider : IssuedTokenProvider
  {
    public VssAadTokenProvider(VssAadCredential credential)
      : base((IssuedTokenCredential) credential, (Uri) null, (Uri) null)
    {
    }

    public override bool GetTokenIsInteractive => false;

    private VssAadToken GetVssAadToken()
    {
      string authorityUri = VssAadSettings.AadInstance + "common";
      HttpClientFactoryWithUserAgent factoryWithUserAgent = new HttpClientFactoryWithUserAgent();
      return new VssAadToken(PublicClientApplicationBuilder.Create("872cd9fa-d31f-45e0-9eab-6e460a02d1f1").WithHttpClientFactory((IMsalHttpClientFactory) factoryWithUserAgent).WithAuthority(authorityUri), this.Credential as VssAadCredential);
    }

    protected override Task<IssuedToken> OnGetTokenAsync(
      IssuedToken failedToken,
      CancellationToken cancellationToken)
    {
      if (failedToken != null && failedToken.CredentialType == VssCredentialsType.Aad && failedToken.IsAuthenticated)
      {
        this.CurrentToken = (IssuedToken) null;
        return Task.FromResult<IssuedToken>((IssuedToken) null);
      }
      try
      {
        return Task.FromResult<IssuedToken>((IssuedToken) this.GetVssAadToken());
      }
      catch
      {
      }
      return Task.FromResult<IssuedToken>((IssuedToken) null);
    }
  }
}
