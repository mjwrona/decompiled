// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.VssAzureIdentityTokenProvider
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Azure.Core;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Diagnostics;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Client
{
  internal sealed class VssAzureIdentityTokenProvider : IssuedTokenProvider
  {
    private readonly TokenCredential tokenCredential;

    public VssAzureIdentityTokenProvider(
      VssAzureIdentityCredential credential,
      TokenCredential tokenCredential)
      : base((IssuedTokenCredential) credential, (Uri) null, (Uri) null)
    {
      ArgumentUtility.CheckForNull<TokenCredential>(tokenCredential, nameof (tokenCredential));
      this.tokenCredential = tokenCredential;
    }

    public override bool GetTokenIsInteractive => false;

    protected override async Task<IssuedToken> OnGetTokenAsync(
      IssuedToken failedToken,
      CancellationToken cancellationToken)
    {
      VssAzureIdentityTokenProvider provider = this;
      string token;
      try
      {
        AccessToken accessToken = await provider.tokenCredential.GetTokenAsync(new TokenRequestContext(VssAadSettings.DefaultScopes, (string) null, (string) null, (string) null, false), cancellationToken).ConfigureAwait(false);
        token = ((AccessToken) ref accessToken).Token;
      }
      catch (Exception ex)
      {
        VssHttpEventSource.Log.AuthenticationError(VssTraceActivity.Current, (IssuedTokenProvider) provider, ex);
        throw new VssAuthenticationException("Unable to aquire a bearer token from TokenCredential.", ex);
      }
      if (string.IsNullOrEmpty(token))
      {
        VssHttpEventSource.Log.AuthenticationError(VssTraceActivity.Current, (IssuedTokenProvider) provider, "Null or empty token aquired from TokenCredential.");
        throw new VssAuthenticationException("Null or empty token aquired from TokenCredential.");
      }
      return (IssuedToken) new VssAadToken("Bearer", token);
    }
  }
}
